using Lab05.BLL.Exceptions;
using Lab05.BLL.Models;
using Lab05.BLL.Services;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;
using Moq;

namespace Lab05.Tests.BLL;

public class AcrobatServiceTests
{
    private readonly Mock<IAcrobatRepository> _repositoryMock;
    private readonly Mock<IEntityValidator> _validatorMock;
    private readonly Mock<IDataProvider<AcrobatEntity>> _providerMock;
    private readonly AcrobatService _service;

    public AcrobatServiceTests()
    {
        _repositoryMock = new Mock<IAcrobatRepository>();
        _validatorMock = new Mock<IEntityValidator>();
        _providerMock = new Mock<IDataProvider<AcrobatEntity>>();
        _providerMock.SetupGet(p => p.FormatName).Returns("JSON");

        _service = new AcrobatService(
            _repositoryMock.Object,
            _validatorMock.Object,
            new[] { _providerMock.Object });
    }

    [Fact]
    public void GetAll_ReturnsAllAcrobats()
    {
        // Arrange
        var list = new List<AcrobatEntity>
        {
            new() { Id = 1, FirstName = "Олена", LastName = "Марченко", CircusOrTroupe = "Цирк Кобзов", Specialization = "Трапеція" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(list);

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.Single(result);
        Assert.Equal("Марченко", result[0].LastName);
    }

    [Fact]
    public void Add_ValidAcrobat_CallsRepositoryAdd()
    {
        // Arrange
        var model = new AcrobatModel
        {
            FirstName = "Юрій",
            LastName = "Гнатюк",
            CircusOrTroupe = "Національний цирк",
            Specialization = "Жонглювання"
        };

        // Act
        _service.Add(model);

        // Assert
        _repositoryMock.Verify(r => r.Add(It.IsAny<AcrobatEntity>()), Times.Once);
    }

    [Fact]
    public void PerformSafetyAudit_PassAudit_SetsValidCertificate()
    {
        // Arrange
        var entity = new AcrobatEntity { Id = 1, FirstName = "Анна", LastName = "Бондар", SafetyCertificationValid = false };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(entity);

        // Act
        string message = _service.PerformSafetyAudit(1, true);

        // Assert
        Assert.Contains("допущений до виступів", message);
        _repositoryMock.Verify(r => r.Update(It.Is<AcrobatEntity>(e => e.SafetyCertificationValid)), Times.Once);
    }

    [Fact]
    public void PerformSafetyAudit_FailAudit_RevokesCertificate()
    {
        // Arrange
        var entity = new AcrobatEntity { Id = 1, FirstName = "Анна", LastName = "Бондар", SafetyCertificationValid = true };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(entity);

        // Act
        string message = _service.PerformSafetyAudit(1, false);

        // Assert
        Assert.Contains("призупинено", message);
        _repositoryMock.Verify(r => r.Update(It.Is<AcrobatEntity>(e => !e.SafetyCertificationValid)), Times.Once);
    }

    [Fact]
    public void PerformStunt_ValidCertificate_IncrementsCountAndReturnsSuccess()
    {
        // Arrange
        var acrobat = new AcrobatModel
        {
            FirstName = "Валерій",
            LastName = "Степанов",
            Specialization = "Канатоходець",
            SafetyCertificationValid = true,
            PerformancesCount = 5
        };

        // Act
        string message = acrobat.PerformStunt("Стрибок на тросі");

        // Assert
        Assert.Equal(6, acrobat.PerformancesCount);
        Assert.Contains("бездоганно виконав", message);
    }

    [Fact]
    public void PerformStunt_InvalidCertificate_DoesNotIncrementCountAndReturnsWarning()
    {
        // Arrange
        var acrobat = new AcrobatModel
        {
            FirstName = "Валерій",
            LastName = "Степанов",
            Specialization = "Канатоходець",
            SafetyCertificationValid = false,
            PerformancesCount = 5
        };

        // Act
        string message = acrobat.PerformStunt("Стрибок на тросі");

        // Assert
        Assert.Equal(5, acrobat.PerformancesCount);
        Assert.Contains("відсторонений від трюку", message);
    }

    [Fact]
    public void GetById_Existing_ReturnsAcrobat()
    {
        // Arrange
        var entity = new AcrobatEntity { Id = 1, FirstName = "Максим", LastName = "Дорошенко", CircusOrTroupe = "Кобзов", Specialization = "Трюки" };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(entity);

        // Act
        var result = _service.GetById(1);

        // Assert
        Assert.Equal("Дорошенко", result.LastName);
    }

    [Fact]
    public void GetById_NonExisting_ThrowsEntityNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(99)).Returns((AcrobatEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.GetById(99));
    }

    [Fact]
    public void Update_Existing_UpdatesSuccessfully()
    {
        // Arrange
        var model = new AcrobatModel { Id = 1, FirstName = "Максим", LastName = "Дорошенко", CircusOrTroupe = "Кобзов", Specialization = "Трюки" };
        var entity = new AcrobatEntity { Id = 1 };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(entity);

        // Act
        _service.Update(model);

        // Assert
        _repositoryMock.Verify(r => r.Update(It.Is<AcrobatEntity>(e => e.Id == 1)), Times.Once);
    }

    [Fact]
    public void Update_NonExisting_ThrowsEntityNotFoundException()
    {
        // Arrange
        var model = new AcrobatModel { Id = 99, FirstName = "Максим", LastName = "Дорошенко", CircusOrTroupe = "Кобзов", Specialization = "Трюки" };
        _repositoryMock.Setup(r => r.GetById(99)).Returns((AcrobatEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.Update(model));
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
        const string path = "acrobats.json";

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
        Assert.Throws<ValidationException>(() => _service.SaveToFile("acrobats.xyz", "UNKNOWN"));
    }

    [Fact]
    public void SaveToFile_RepositoryThrows_WrapsInStorageException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Save(It.IsAny<string>(), It.IsAny<IDataProvider<AcrobatEntity>>()))
            .Throws(new IOException("Disk error"));

        // Act & Assert
        Assert.Throws<StorageException>(() => _service.SaveToFile("acrobats.json", "JSON"));
    }

    [Fact]
    public void LoadFromFile_RepositoryThrows_WrapsInStorageException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Load(It.IsAny<string>(), It.IsAny<IDataProvider<AcrobatEntity>>()))
            .Throws(new IOException("Disk error"));

        // Act & Assert
        Assert.Throws<StorageException>(() => _service.LoadFromFile("acrobats.json", "JSON"));
    }
}
