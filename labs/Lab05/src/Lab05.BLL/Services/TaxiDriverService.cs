using Lab05.BLL.Exceptions;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Mappers;
using Lab05.BLL.Models;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;

namespace Lab05.BLL.Services;

public class TaxiDriverService : ITaxiDriverService
{
    private readonly ITaxiDriverRepository _repository;
    private readonly IEntityValidator _validator;
    private readonly IDriverLicenseService _licenseService;
    private readonly IEnumerable<IDataProvider<TaxiDriverEntity>> _providers;

    public TaxiDriverService(
        ITaxiDriverRepository repository,
        IEntityValidator validator,
        IDriverLicenseService licenseService,
        IEnumerable<IDataProvider<TaxiDriverEntity>> providers)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _licenseService = licenseService ?? throw new ArgumentNullException(nameof(licenseService));
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public IReadOnlyList<TaxiDriverModel> GetAll()
    {
        return _repository.GetAll().Select(TaxiDriverMapper.ToModel).ToList().AsReadOnly();
    }

    public TaxiDriverModel GetById(int id)
    {
        var entity = _repository.GetById(id);
        if (entity is null)
        {
            throw new EntityNotFoundException($"Водія таксі з ID={id} не знайдено.");
        }

        return TaxiDriverMapper.ToModel(entity);
    }

    public TaxiDriverModel? GetByLicenseNumber(string licenseNumber)
    {
        _validator.ValidateDriverLicense(licenseNumber);
        var entity = _repository.GetByLicenseNumber(licenseNumber);
        return entity is null ? null : TaxiDriverMapper.ToModel(entity);
    }

    public void Add(TaxiDriverModel driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ValidateDriver(driver);

        var existing = _repository.GetByLicenseNumber(driver.LicenseNumber);
        if (existing is not null)
        {
            throw new DuplicateEntityException(
                $"Водій з номером посвідчення '{driver.LicenseNumber}' уже зареєстрований.");
        }

        var entity = TaxiDriverMapper.ToEntity(driver);
        _repository.Add(entity);
        driver.Id = entity.Id;
    }

    public void Update(TaxiDriverModel driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ValidateDriver(driver);

        var existing = _repository.GetById(driver.Id);
        if (existing is null)
        {
            throw new EntityNotFoundException($"Водія таксі з ID={driver.Id} не знайдено.");
        }

        var duplicate = _repository.GetByLicenseNumber(driver.LicenseNumber);
        if (duplicate is not null && duplicate.Id != driver.Id)
        {
            throw new DuplicateEntityException(
                $"Інший водій уже зареєстрований з посвідченням '{driver.LicenseNumber}'.");
        }

        var entity = TaxiDriverMapper.ToEntity(driver);
        _repository.Update(entity);
    }

    public void Delete(int id)
    {
        if (!_repository.Delete(id))
        {
            throw new EntityNotFoundException($"Неможливо видалити: водія таксі з ID={id} не знайдено.");
        }
    }

    public DriverLicenseResult UpgradeLicense(int driverId, string newCategory)
    {
        var driver = GetById(driverId);

        var application = new DriverLicenseApplication
        {
            CandidateName = driver.FullName,
            Age = driver.Age,
            HasMedicalCertificate = driver.HasMedicalCertificate,
            TheoryScore = 20, // Experienced driver passes theory
            PassedPracticalTest = true, // Experienced driver passes practical
            RequestedCategory = newCategory,
            HasCourtBan = false,
            PaidStateFee = true
        };

        var result = _licenseService.ApplyForLicense(application);
        if (result.IsApproved)
        {
            driver.LicenseNumber = result.LicenseNumber;
            driver.HasActiveLicense = true;
            Update(driver);
        }

        return result;
    }

    public void SaveToFile(string filePath, string format = "JSON")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        var provider = ResolveProvider(format);

        try
        {
            _repository.Save(filePath, provider);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка збереження водіїв таксі: {ex.Message}", ex);
        }
    }

    public void LoadFromFile(string filePath, string format = "JSON")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        var provider = ResolveProvider(format);

        try
        {
            _repository.Load(filePath, provider);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка завантаження водіїв таксі: {ex.Message}", ex);
        }
    }

    private void ValidateDriver(TaxiDriverModel driver)
    {
        _validator.ValidateName(driver.FirstName, nameof(driver.FirstName));
        _validator.ValidateName(driver.LastName, nameof(driver.LastName));
        _validator.ValidateCarModel(driver.CarModel);
        _validator.ValidateDriverLicense(driver.LicenseNumber);
        _validator.ValidateExperience(driver.ExperienceYears, driver.Age);
    }

    private IDataProvider<TaxiDriverEntity> ResolveProvider(string format)
    {
        var provider = _providers.FirstOrDefault(p =>
            p.FormatName.Contains(format, StringComparison.OrdinalIgnoreCase));

        if (provider is null)
        {
            throw new ValidationException(
                $"Невідомий формат серіалізації: '{format}'. Доступні: {string.Join(", ", _providers.Select(p => p.FormatName))}.");
        }

        return provider;
    }
}
