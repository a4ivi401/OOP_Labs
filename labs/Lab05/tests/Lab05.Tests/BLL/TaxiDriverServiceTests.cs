using Lab05.BLL.Exceptions;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Models;
using Lab05.BLL.Services;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;
using Moq;

namespace Lab05.Tests.BLL;

public class TaxiDriverServiceTests
{
    private readonly Mock<ITaxiDriverRepository> _repositoryMock;
    private readonly Mock<IEntityValidator> _validatorMock;
    private readonly Mock<IDriverLicenseService> _licenseServiceMock;
    private readonly Mock<IDataProvider<TaxiDriverEntity>> _providerMock;
    private readonly TaxiDriverService _service;

    public TaxiDriverServiceTests()
    {
        _repositoryMock = new Mock<ITaxiDriverRepository>();
        _validatorMock = new Mock<IEntityValidator>();
        _licenseServiceMock = new Mock<IDriverLicenseService>();
        _providerMock = new Mock<IDataProvider<TaxiDriverEntity>>();
        _providerMock.SetupGet(p => p.FormatName).Returns("JSON");

        _service = new TaxiDriverService(
            _repositoryMock.Object,
            _validatorMock.Object,
            _licenseServiceMock.Object,
            new[] { _providerMock.Object });
    }

    [Fact]
    public void GetAll_ReturnsTaxiDrivers()
    {
        // Arrange
        var list = new List<TaxiDriverEntity>
        {
            new() { Id = 1, FirstName = "Василь", LastName = "Петренко", CarModel = "Skoda Octavia", LicenseNumber = "BTA123456" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(list);

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.Single(result);
        Assert.Equal("Петренко", result[0].LastName);
    }

    [Fact]
    public void GetById_Existing_ReturnsDriver()
    {
        // Arrange
        var entity = new TaxiDriverEntity { Id = 2, FirstName = "Сергій", LastName = "Кравчук", LicenseNumber = "BTA654321" };
        _repositoryMock.Setup(r => r.GetById(2)).Returns(entity);

        // Act
        var result = _service.GetById(2);

        // Assert
        Assert.Equal("Кравчук", result.LastName);
    }

    [Fact]
    public void GetById_NonExisting_ThrowsEntityNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(99)).Returns((TaxiDriverEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.GetById(99));
    }

    [Fact]
    public void Add_ValidDriver_AddsSuccessfully()
    {
        // Arrange
        var model = new TaxiDriverModel
        {
            FirstName = "Олег",
            LastName = "Степаненко",
            CarModel = "Toyota Prius",
            LicenseNumber = "BTA777888",
            Age = 30,
            ExperienceYears = 5
        };
        _repositoryMock.Setup(r => r.GetByLicenseNumber(model.LicenseNumber)).Returns((TaxiDriverEntity?)null);

        // Act
        _service.Add(model);

        // Assert
        _repositoryMock.Verify(r => r.Add(It.IsAny<TaxiDriverEntity>()), Times.Once);
    }

    [Fact]
    public void Add_DuplicateLicense_ThrowsDuplicateEntityException()
    {
        // Arrange
        var model = new TaxiDriverModel { LicenseNumber = "BTA111222" };
        var existing = new TaxiDriverEntity { Id = 3, LicenseNumber = "BTA111222" };
        _repositoryMock.Setup(r => r.GetByLicenseNumber(model.LicenseNumber)).Returns(existing);

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => _service.Add(model));
    }

    [Fact]
    public void UpgradeLicense_SuccessfulUpgrade_UpdatesDriverLicense()
    {
        // Arrange
        var entity = new TaxiDriverEntity
        {
            Id = 1,
            FirstName = "Тарас",
            LastName = "Шевченко",
            Age = 35,
            HasMedicalCertificate = true,
            LicenseNumber = "BTA100200"
        };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(entity);

        var licenseResult = new DriverLicenseResult
        {
            IsApproved = true,
            CandidateName = "Шевченко Тарас",
            LicenseNumber = "CXR999888",
            Category = "D",
            IssueDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddYears(2)
        };
        _licenseServiceMock.Setup(l => l.ApplyForLicense(It.IsAny<DriverLicenseApplication>()))
            .Returns(licenseResult);

        // Act
        var result = _service.UpgradeLicense(1, "D");

        // Assert
        Assert.True(result.IsApproved);
        Assert.Equal("CXR999888", result.LicenseNumber);
        _repositoryMock.Verify(r => r.Update(It.Is<TaxiDriverEntity>(e => e.LicenseNumber == "CXR999888")), Times.Once);
    }

    [Fact]
    public void Drive_ActiveLicense_ReturnsDriveMessage()
    {
        // Arrange
        var driver = new TaxiDriverModel
        {
            FirstName = "Ігор",
            LastName = "Коваль",
            CarModel = "Renault Megane",
            HasActiveLicense = true
        };

        // Act
        string message = driver.Drive("Аеропорт Бориспіль");

        // Assert
        Assert.Contains("вирушив за маршрутом", message);
        Assert.Contains("Renault Megane", message);
    }

    [Fact]
    public void Drive_InactiveLicense_ReturnsWarning()
    {
        // Arrange
        var driver = new TaxiDriverModel
        {
            FirstName = "Ігор",
            LastName = "Коваль",
            CarModel = "Renault Megane",
            HasActiveLicense = false // Revoked
        };

        // Act
        string message = driver.Drive("Центральний вокзал");

        // Assert
        Assert.Contains("не може виконати замовлення", message);
    }

    [Fact]
    public void GetByLicenseNumber_Existing_ReturnsDriver()
    {
        // Arrange
        var entity = new TaxiDriverEntity { Id = 1, LicenseNumber = "BTA123456", FirstName = "Іван", LastName = "Сірко" };
        _repositoryMock.Setup(r => r.GetByLicenseNumber("BTA123456")).Returns(entity);

        // Act
        var result = _service.GetByLicenseNumber("BTA123456");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Сірко", result.LastName);
    }

    [Fact]
    public void GetByLicenseNumber_NotFound_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByLicenseNumber("BTA999999")).Returns((TaxiDriverEntity?)null);

        // Act
        var result = _service.GetByLicenseNumber("BTA999999");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Update_ExistingDriver_UpdatesSuccessfully()
    {
        // Arrange
        var model = new TaxiDriverModel { Id = 1, LicenseNumber = "BTA123456", FirstName = "Іван", LastName = "Сірко", CarModel = "Lanos" };
        var existing = new TaxiDriverEntity { Id = 1, LicenseNumber = "BTA123456" };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(existing);
        _repositoryMock.Setup(r => r.GetByLicenseNumber("BTA123456")).Returns(existing);

        // Act
        _service.Update(model);

        // Assert
        _repositoryMock.Verify(r => r.Update(It.Is<TaxiDriverEntity>(e => e.Id == 1)), Times.Once);
    }

    [Fact]
    public void Update_NonExisting_ThrowsEntityNotFoundException()
    {
        // Arrange
        var model = new TaxiDriverModel { Id = 99, LicenseNumber = "BTA999999", FirstName = "Петро", LastName = "Шевчук", CarModel = "Lanos" };
        _repositoryMock.Setup(r => r.GetById(99)).Returns((TaxiDriverEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.Update(model));
    }

    [Fact]
    public void Update_ConflictingLicense_ThrowsDuplicateEntityException()
    {
        // Arrange
        var model = new TaxiDriverModel { Id = 1, LicenseNumber = "BTA222222", FirstName = "Олег", LastName = "Бойко", CarModel = "Kia" };
        var existing = new TaxiDriverEntity { Id = 1, LicenseNumber = "BTA111111" };
        var conflicting = new TaxiDriverEntity { Id = 2, LicenseNumber = "BTA222222" };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(existing);
        _repositoryMock.Setup(r => r.GetByLicenseNumber("BTA222222")).Returns(conflicting);

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => _service.Update(model));
    }

    [Fact]
    public void Delete_Existing_DeletesSuccessfully()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Delete(1)).Returns(true);

        // Act
        _service.Delete(1);

        // Assert
        _repositoryMock.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public void Delete_NonExisting_ThrowsEntityNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Delete(99)).Returns(false);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.Delete(99));
    }

    [Fact]
    public void SaveToFile_And_LoadFromFile_CallRepository()
    {
        // Arrange
        const string path = "drivers.json";

        // Act
        _service.SaveToFile(path, "JSON");
        _service.LoadFromFile(path, "JSON");

        // Assert
        _repositoryMock.Verify(r => r.Save(path, _providerMock.Object), Times.Once);
        _repositoryMock.Verify(r => r.Load(path, _providerMock.Object), Times.Once);
    }

    [Fact]
    public void SaveToFile_UnknownFormat_ThrowsValidationException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _service.SaveToFile("drivers.xyz", "UNKNOWN"));
    }

    [Fact]
    public void SaveToFile_RepositoryThrows_WrapsInStorageException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Save(It.IsAny<string>(), It.IsAny<IDataProvider<TaxiDriverEntity>>()))
            .Throws(new IOException("Disk error"));

        // Act & Assert
        Assert.Throws<StorageException>(() => _service.SaveToFile("drivers.json", "JSON"));
    }

    [Fact]
    public void LoadFromFile_RepositoryThrows_WrapsInStorageException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Load(It.IsAny<string>(), It.IsAny<IDataProvider<TaxiDriverEntity>>()))
            .Throws(new IOException("Disk error"));

        // Act & Assert
        Assert.Throws<StorageException>(() => _service.LoadFromFile("drivers.json", "JSON"));
    }
}
