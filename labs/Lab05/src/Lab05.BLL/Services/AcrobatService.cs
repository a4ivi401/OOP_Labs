using Lab05.BLL.Exceptions;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Mappers;
using Lab05.BLL.Models;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;

namespace Lab05.BLL.Services;

public class AcrobatService : IAcrobatService
{
    private readonly IAcrobatRepository _repository;
    private readonly IEntityValidator _validator;
    private readonly IEnumerable<IDataProvider<AcrobatEntity>> _providers;

    public AcrobatService(
        IAcrobatRepository repository,
        IEntityValidator validator,
        IEnumerable<IDataProvider<AcrobatEntity>> providers)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public IReadOnlyList<AcrobatModel> GetAll()
    {
        return _repository.GetAll().Select(AcrobatMapper.ToModel).ToList().AsReadOnly();
    }

    public AcrobatModel GetById(int id)
    {
        var entity = _repository.GetById(id);
        if (entity is null)
        {
            throw new EntityNotFoundException($"Акробата з ID={id} не знайдено.");
        }

        return AcrobatMapper.ToModel(entity);
    }

    public void Add(AcrobatModel acrobat)
    {
        ArgumentNullException.ThrowIfNull(acrobat);
        ValidateAcrobat(acrobat);

        var entity = AcrobatMapper.ToEntity(acrobat);
        _repository.Add(entity);
        acrobat.Id = entity.Id;
    }

    public void Update(AcrobatModel acrobat)
    {
        ArgumentNullException.ThrowIfNull(acrobat);
        ValidateAcrobat(acrobat);

        var existing = _repository.GetById(acrobat.Id);
        if (existing is null)
        {
            throw new EntityNotFoundException($"Акробата з ID={acrobat.Id} не знайдено.");
        }

        var entity = AcrobatMapper.ToEntity(acrobat);
        _repository.Update(entity);
    }

    public void Delete(int id)
    {
        if (!_repository.Delete(id))
        {
            throw new EntityNotFoundException($"Неможливо видалити: акробата з ID={id} не знайдено.");
        }
    }

    public string PerformSafetyAudit(int acrobatId, bool passAudit)
    {
        var acrobat = GetById(acrobatId);
        acrobat.SafetyCertificationValid = passAudit;
        Update(acrobat);

        return passAudit
            ? $"Аудит безпеки пройдено успішно: акробат {acrobat.FullName} допущений до виступів!"
            : $"Аудит безпеки НЕ пройдено: дію сертифіката акробата {acrobat.FullName} призупинено!";
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
            throw new StorageException($"Помилка збереження акробатів: {ex.Message}", ex);
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
            throw new StorageException($"Помилка завантаження акробатів: {ex.Message}", ex);
        }
    }

    private void ValidateAcrobat(AcrobatModel acrobat)
    {
        _validator.ValidateName(acrobat.FirstName, nameof(acrobat.FirstName));
        _validator.ValidateName(acrobat.LastName, nameof(acrobat.LastName));
        _validator.ValidateAcrobatFields(acrobat.CircusOrTroupe, acrobat.Specialization);
    }

    private IDataProvider<AcrobatEntity> ResolveProvider(string format)
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
