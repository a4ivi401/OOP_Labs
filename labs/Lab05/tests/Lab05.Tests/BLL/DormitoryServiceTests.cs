using Lab05.BLL.Exceptions;
using Lab05.BLL.Models;
using Lab05.BLL.Services;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Repositories;
using Moq;

namespace Lab05.Tests.BLL;

public class DormitoryServiceTests
{
    private readonly Mock<IStudentRepository> _studentRepoMock;
    private readonly Mock<IEntityValidator> _validatorMock;
    private readonly DormitoryService _service;

    public DormitoryServiceTests()
    {
        _studentRepoMock = new Mock<IStudentRepository>();
        _validatorMock = new Mock<IEntityValidator>();
        _service = new DormitoryService(_studentRepoMock.Object, _validatorMock.Object);
    }

    [Fact]
    public void Constructor_NullArguments_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DormitoryService(null!, _validatorMock.Object));
        Assert.Throws<ArgumentNullException>(() => new DormitoryService(_studentRepoMock.Object, null!));
    }

    [Fact]
    public void SettleInDormitory_EligibleNonLocalStudent_SettlesSuccessfully()
    {
        // Arrange: Student from Lviv (non-local when base city is Kyiv)
        const string ticket = "KB102030";
        var entity = new StudentEntity
        {
            Id = 1,
            FirstName = "Тарас",
            LastName = "Шевченко",
            StudentId = ticket,
            Course = 1,
            OriginCity = "Львів",
            IsSettledInDormitory = false
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act
        var result = _service.SettleInDormitory(ticket, 4, 305, "Київ");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.DormitoryNumber);
        Assert.Equal(305, result.RoomNumber);
        Assert.True(entity.IsSettledInDormitory);
        Assert.Equal(4, entity.DormitoryNumber);
        Assert.Equal(305, entity.RoomNumber);
        Assert.NotNull(entity.SettlementDate);
        _studentRepoMock.Verify(r => r.Update(entity), Times.Once);
    }

    [Fact]
    public void SettleInDormitory_LocalStudent_ThrowsIneligibleForDormitoryException()
    {
        // Arrange: Student from Kyiv (local)
        const string ticket = "KB102031";
        var entity = new StudentEntity
        {
            Id = 2,
            FirstName = "Іван",
            LastName = "Франко",
            StudentId = ticket,
            Course = 1,
            OriginCity = "Київ",
            IsSettledInDormitory = false
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act & Assert
        var ex = Assert.Throws<IneligibleForDormitoryException>(() =>
            _service.SettleInDormitory(ticket, 3, 201, "Київ"));

        Assert.Equal(ticket, ex.StudentId);
        Assert.Equal("Київ", ex.OriginCity);
        _studentRepoMock.Verify(r => r.Update(It.IsAny<StudentEntity>()), Times.Never);
    }

    [Fact]
    public void SettleInDormitory_AlreadySettledStudent_ThrowsInvalidOperationException()
    {
        // Arrange
        const string ticket = "KB102032";
        var entity = new StudentEntity
        {
            Id = 3,
            FirstName = "Леся",
            LastName = "Українка",
            StudentId = ticket,
            Course = 2,
            OriginCity = "Луцьк",
            IsSettledInDormitory = true,
            DormitoryNumber = 1,
            RoomNumber = 101
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _service.SettleInDormitory(ticket, 2, 202, "Київ"));

        Assert.Contains("вже поселений", ex.Message);
        _studentRepoMock.Verify(r => r.Update(It.IsAny<StudentEntity>()), Times.Never);
    }

    [Fact]
    public void SettleInDormitory_NonExistingStudent_ThrowsEntityNotFoundException()
    {
        // Arrange
        const string ticket = "KB999999";
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns((StudentEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() =>
            _service.SettleInDormitory(ticket, 1, 101, "Київ"));
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(51, 100)]
    public void SettleInDormitory_InvalidDormitoryNumber_ThrowsValidationException(int dormNum, int roomNum)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() =>
            _service.SettleInDormitory("KB123456", dormNum, roomNum, "Київ"));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, 1000)]
    public void SettleInDormitory_InvalidRoomNumber_ThrowsValidationException(int dormNum, int roomNum)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() =>
            _service.SettleInDormitory("KB123456", dormNum, roomNum, "Київ"));
    }

    [Fact]
    public void EvictFromDormitory_SettledStudent_EvictsSuccessfully()
    {
        // Arrange
        const string ticket = "KB102030";
        var entity = new StudentEntity
        {
            Id = 1,
            StudentId = ticket,
            IsSettledInDormitory = true,
            DormitoryNumber = 4,
            RoomNumber = 305,
            SettlementDate = DateTime.Now
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act
        bool result = _service.EvictFromDormitory(ticket);

        // Assert
        Assert.True(result);
        Assert.False(entity.IsSettledInDormitory);
        Assert.Null(entity.DormitoryNumber);
        Assert.Null(entity.RoomNumber);
        Assert.Null(entity.SettlementDate);
        _studentRepoMock.Verify(r => r.Update(entity), Times.Once);
    }

    [Fact]
    public void EvictFromDormitory_NotSettledStudent_ReturnsFalse()
    {
        // Arrange
        const string ticket = "KB102030";
        var entity = new StudentEntity
        {
            Id = 1,
            StudentId = ticket,
            IsSettledInDormitory = false
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act
        bool result = _service.EvictFromDormitory(ticket);

        // Assert
        Assert.False(result);
        _studentRepoMock.Verify(r => r.Update(It.IsAny<StudentEntity>()), Times.Never);
    }

    [Fact]
    public void GetSettledStudents_ReturnsOnlySettledStudents()
    {
        // Arrange
        var entities = new List<StudentEntity>
        {
            new() { Id = 1, StudentId = "KB111111", IsSettledInDormitory = true },
            new() { Id = 2, StudentId = "KB222222", IsSettledInDormitory = false },
            new() { Id = 3, StudentId = "KB333333", IsSettledInDormitory = true }
        };
        _studentRepoMock.Setup(r => r.GetAll()).Returns(entities);

        // Act
        var result = _service.GetSettledStudents();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.True(s.IsSettledInDormitory));
    }

    [Fact]
    public void IsEligibleForDormitory_Eligible_ReturnsTrue()
    {
        // Arrange
        const string ticket = "KB102030";
        var entity = new StudentEntity
        {
            Id = 1,
            StudentId = ticket,
            OriginCity = "Одеса",
            IsSettledInDormitory = false
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act
        bool eligible = _service.IsEligibleForDormitory(ticket, "Київ", out string reason);

        // Assert
        Assert.True(eligible);
        Assert.Contains("має право", reason);
    }

    [Fact]
    public void IsEligibleForDormitory_LocalStudent_ReturnsFalse()
    {
        // Arrange
        const string ticket = "KB102030";
        var entity = new StudentEntity
        {
            Id = 1,
            StudentId = ticket,
            OriginCity = "Київ",
            IsSettledInDormitory = false
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act
        bool eligible = _service.IsEligibleForDormitory(ticket, "Київ", out string reason);

        // Assert
        Assert.False(eligible);
        Assert.Contains("місцевим жителем", reason);
    }

    [Fact]
    public void EvictFromDormitory_NonExistingStudent_ThrowsEntityNotFoundException()
    {
        // Arrange
        const string ticket = "KB999999";
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns((StudentEntity?)null);

        // Act & Assert
        Assert.Throws<EntityNotFoundException>(() => _service.EvictFromDormitory(ticket));
    }

    [Fact]
    public void IsEligibleForDormitory_NonExistingStudent_ReturnsFalse()
    {
        // Arrange
        const string ticket = "KB999999";
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns((StudentEntity?)null);

        // Act
        bool eligible = _service.IsEligibleForDormitory(ticket, "Київ", out string reason);

        // Assert
        Assert.False(eligible);
        Assert.Contains("не знайдено", reason);
    }

    [Fact]
    public void IsEligibleForDormitory_AlreadySettledStudent_ReturnsFalse()
    {
        // Arrange
        const string ticket = "KB102030";
        var entity = new StudentEntity
        {
            Id = 1,
            StudentId = ticket,
            OriginCity = "Львів",
            IsSettledInDormitory = true,
            DormitoryNumber = 4,
            RoomNumber = 301
        };
        _studentRepoMock.Setup(r => r.GetByStudentId(ticket)).Returns(entity);

        // Act
        bool eligible = _service.IsEligibleForDormitory(ticket, "Київ", out string reason);

        // Assert
        Assert.False(eligible);
        Assert.Contains("вже поселений", reason);
    }
}
