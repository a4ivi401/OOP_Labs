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

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _repositoryMock;
    private readonly Mock<IEntityValidator> _validatorMock;
    private readonly Mock<IDataProvider<StudentEntity>> _providerMock;
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        _repositoryMock = new Mock<IStudentRepository>();
        _validatorMock = new Mock<IEntityValidator>();
        _providerMock = new Mock<IDataProvider<StudentEntity>>();
        _providerMock.SetupGet(p => p.FormatName).Returns("JSON");

        _service = new StudentService(
            _repositoryMock.Object,
            _validatorMock.Object,
            new[] { _providerMock.Object });
    }

    [Fact]
    public void Constructor_NullArguments_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new StudentService(null!, _validatorMock.Object, new[] { _providerMock.Object }));
        Assert.Throws<ArgumentNullException>(() => new StudentService(_repositoryMock.Object, null!, new[] { _providerMock.Object }));
        Assert.Throws<ArgumentNullException>(() => new StudentService(_repositoryMock.Object, _validatorMock.Object, null!));
    }

    [Fact]
    public void GetAll_ReturnsMappedStudentModels()
    {
        // Arrange
        var entities = new List<StudentEntity>
        {
            new() { Id = 1, FirstName = "Іван", LastName = "Франко", Course = 1, StudentId = "KB111111", OriginCity = "Львів" },
            new() { Id = 2, FirstName = "Леся", LastName = "Українка", Course = 2, StudentId = "KB222222", OriginCity = "Луцьк" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Франко", result[0].LastName);
        Assert.Equal("Українка", result[1].LastName);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsStudentModel()
    {
        // Arrange
        var entity = new StudentEntity { Id = 10, FirstName = "Тарас", LastName = "Шевченко", Course = 1, StudentId = "KB123456" };
        _repositoryMock.Setup(r => r.GetById(10)).Returns(entity);

        // Act
        var result = _service.GetById(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Шевченко", result.LastName);
        Assert.Equal("KB123456", result.StudentId);
    }

    [Fact]
    public void GetById_NonExistingId_ThrowsEntityNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(99)).Returns((StudentEntity?)null);

        // Act & Assert
        var ex = Assert.Throws<EntityNotFoundException>(() => _service.GetById(99));
        Assert.Contains("99", ex.Message);
    }

    [Fact]
    public void GetByStudentId_ExistingTicket_ReturnsStudent()
    {
        // Arrange
        const string ticket = "KB123456";
        var entity = new StudentEntity { Id = 1, FirstName = "Богдан", LastName = "Хмельницький", StudentId = ticket, Course = 1 };
        _repositoryMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act
        var result = _service.GetByStudentId(ticket);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticket, result.StudentId);
        _validatorMock.Verify(v => v.ValidateStudentId(ticket), Times.Once);
    }

    [Fact]
    public void GetByStudentId_NonExistingTicket_ThrowsEntityNotFoundException()
    {
        // Arrange
        const string ticket = "KB999999";
        _repositoryMock.Setup(r => r.GetByStudentId(ticket)).Returns((StudentEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.GetByStudentId(ticket));
    }

    [Fact]
    public void Add_ValidStudent_AddsToRepository()
    {
        // Arrange
        var model = new StudentModel
        {
            FirstName = "Михайло",
            LastName = "Грушевський",
            Course = 1,
            StudentId = "KB101010",
            OriginCity = "Холм",
            Passport = "СН112233"
        };
        _repositoryMock.Setup(r => r.GetByStudentId(model.StudentId)).Returns((StudentEntity?)null);

        // Act
        _service.Add(model);

        // Assert
        _repositoryMock.Verify(r => r.Add(It.Is<StudentEntity>(e => e.StudentId == model.StudentId)), Times.Once);
    }

    [Fact]
    public void Add_DuplicateStudentId_ThrowsDuplicateEntityException()
    {
        // Arrange
        var model = new StudentModel { StudentId = "KB101010", FirstName = "Іван", LastName = "Котляревський" };
        var existing = new StudentEntity { Id = 5, StudentId = "KB101010" };
        _repositoryMock.Setup(r => r.GetByStudentId(model.StudentId)).Returns(existing);

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => _service.Add(model));
        _repositoryMock.Verify(r => r.Add(It.IsAny<StudentEntity>()), Times.Never);
    }

    [Fact]
    public void Update_ExistingStudent_UpdatesSuccessfully()
    {
        // Arrange
        var model = new StudentModel { Id = 3, StudentId = "KB303030", FirstName = "Іван", LastName = "Мазепа" };
        var existing = new StudentEntity { Id = 3, StudentId = "KB303030" };
        _repositoryMock.Setup(r => r.GetById(3)).Returns(existing);
        _repositoryMock.Setup(r => r.GetByStudentId(model.StudentId)).Returns(existing);

        // Act
        _service.Update(model);

        // Assert
        _repositoryMock.Verify(r => r.Update(It.Is<StudentEntity>(e => e.Id == 3)), Times.Once);
    }

    [Fact]
    public void Update_NonExistingStudent_ThrowsEntityNotFoundException()
    {
        // Arrange
        var model = new StudentModel { Id = 99, StudentId = "KB999999" };
        _repositoryMock.Setup(r => r.GetById(99)).Returns((StudentEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.Update(model));
        _repositoryMock.Verify(r => r.Update(It.IsAny<StudentEntity>()), Times.Never);
    }

    [Fact]
    public void Update_ConflictingStudentId_ThrowsDuplicateEntityException()
    {
        // Arrange
        var model = new StudentModel { Id = 1, StudentId = "KB222222", FirstName = "Петро", LastName = "Могила" };
        var existing = new StudentEntity { Id = 1, StudentId = "KB111111" };
        var conflicting = new StudentEntity { Id = 2, StudentId = "KB222222" };

        _repositoryMock.Setup(r => r.GetById(1)).Returns(existing);
        _repositoryMock.Setup(r => r.GetByStudentId("KB222222")).Returns(conflicting);

        // Act & Assert
        Assert.Throws<DuplicateEntityException>(() => _service.Update(model));
    }

    [Fact]
    public void Delete_ExistingStudent_DeletesSuccessfully()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Delete(7)).Returns(true);

        // Act
        _service.Delete(7);

        // Assert
        _repositoryMock.Verify(r => r.Delete(7), Times.Once);
    }

    [Fact]
    public void Delete_NonExistingStudent_ThrowsEntityNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Delete(8)).Returns(false);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.Delete(8));
    }

    [Fact]
    public void CalculateFirstCourseNonLocalPercentage_MixedStudents_CalculatesCorrectPercentage()
    {
        // Arrange: 4 first course students (3 non-local, 1 Kyiv -> 75.0%), 1 second course
        var students = new List<StudentEntity>
        {
            new() { Id = 1, Course = 1, OriginCity = "Львів" },
            new() { Id = 2, Course = 1, OriginCity = "Одеса" },
            new() { Id = 3, Course = 1, OriginCity = "Київ" }, // Local
            new() { Id = 4, Course = 1, OriginCity = "Харків" },
            new() { Id = 5, Course = 2, OriginCity = "Полтава" } // Course 2, ignored
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(students);

        // Act
        double percentage = _service.CalculateFirstCourseNonLocalPercentage("Київ");

        // Assert
        Assert.Equal(75.0, percentage);
    }

    [Fact]
    public void CalculateFirstCourseNonLocalPercentage_NoFirstCourseStudents_ReturnsZero()
    {
        // Arrange
        var students = new List<StudentEntity>
        {
            new() { Id = 1, Course = 2, OriginCity = "Львів" },
            new() { Id = 2, Course = 3, OriginCity = "Одеса" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(students);

        // Act
        double percentage = _service.CalculateFirstCourseNonLocalPercentage("Київ");

        // Assert
        Assert.Equal(0.0, percentage);
    }

    [Fact]
    public void CalculateFirstCourseNonLocalPercentage_AllNonLocal_Returns100Percent()
    {
        // Arrange
        var students = new List<StudentEntity>
        {
            new() { Id = 1, Course = 1, OriginCity = "Львів" },
            new() { Id = 2, Course = 1, OriginCity = "Одеса" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(students);

        // Act
        double percentage = _service.CalculateFirstCourseNonLocalPercentage("Київ");

        // Assert
        Assert.Equal(100.0, percentage);
    }

    [Fact]
    public void CalculateFirstCourseNonLocalPercentage_AllLocal_ReturnsZeroPercent()
    {
        // Arrange
        var students = new List<StudentEntity>
        {
            new() { Id = 1, Course = 1, OriginCity = "Київ" },
            new() { Id = 2, Course = 1, OriginCity = " київ " } // Case-insensitive and trimmed
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(students);

        // Act
        double percentage = _service.CalculateFirstCourseNonLocalPercentage("Київ");

        // Assert
        Assert.Equal(0.0, percentage);
    }

    [Fact]
    public void GetFirstCourseNonLocalStudents_ReturnsOnlyFirstCourseOutOfTown()
    {
        // Arrange
        var students = new List<StudentEntity>
        {
            new() { Id = 1, FirstName = "Остап", LastName = "Вишня", Course = 1, OriginCity = "Полтава" },
            new() { Id = 2, FirstName = "Максим", LastName = "Рильський", Course = 1, OriginCity = "Київ" },
            new() { Id = 3, FirstName = "Микола", LastName = "Куліш", Course = 2, OriginCity = "Одеса" },
            new() { Id = 4, FirstName = "Василь", LastName = "Стус", Course = 1, OriginCity = "Донецьк" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(students);

        // Act
        var result = _service.GetFirstCourseNonLocalStudents("Київ");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.Equal(1, s.Course));
        Assert.Contains(result, s => s.LastName == "Вишня");
        Assert.Contains(result, s => s.LastName == "Стус");
    }

    [Fact]
    public void SaveToFile_ValidPath_CallsRepositorySave()
    {
        // Arrange
        const string path = "test_students.json";

        // Act
        _service.SaveToFile(path, "JSON");

        // Assert
        _repositoryMock.Verify(r => r.Save(path, _providerMock.Object), Times.Once);
    }

    [Fact]
    public void SaveToFile_UnknownFormat_ThrowsValidationException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _service.SaveToFile("test.xyz", "UNKNOWN_FORMAT"));
    }

    [Fact]
    public void LoadFromFile_ValidPath_CallsRepositoryLoad()
    {
        // Arrange
        const string path = "test_students.json";

        // Act
        _service.LoadFromFile(path, "JSON");

        // Assert
        _repositoryMock.Verify(r => r.Load(path, _providerMock.Object), Times.Once);
    }

    [Fact]
    public void Clear_CallsRepositoryClear()
    {
        // Arrange & Act
        _service.Clear();

        // Assert
        _repositoryMock.Verify(r => r.Clear(), Times.Once);
    }

    [Fact]
    public void SaveToFile_RepositoryThrows_WrapsInStorageException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Save(It.IsAny<string>(), It.IsAny<IDataProvider<StudentEntity>>()))
            .Throws(new IOException("Disk error"));

        // Act & Assert
        var ex = Assert.Throws<StorageException>(() => _service.SaveToFile("test.json", "JSON"));
        Assert.Contains("Помилка збереження", ex.Message);
    }

    [Fact]
    public void LoadFromFile_RepositoryThrows_WrapsInStorageException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.Load(It.IsAny<string>(), It.IsAny<IDataProvider<StudentEntity>>()))
            .Throws(new IOException("Disk read error"));

        // Act & Assert
        var ex = Assert.Throws<StorageException>(() => _service.LoadFromFile("test.json", "JSON"));
        Assert.Contains("Помилка завантаження", ex.Message);
    }
}
