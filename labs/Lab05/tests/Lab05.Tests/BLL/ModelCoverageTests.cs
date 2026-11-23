using Lab05.BLL.Exceptions;
using Lab05.BLL.Models;

namespace Lab05.Tests.BLL;

public class ModelCoverageTests
{
    [Fact]
    public void StudentModel_IsFromAnotherCity_ReturnsExpectedResults()
    {
        // Arrange
        var local = new StudentModel { OriginCity = "Київ" };
        var nonLocal = new StudentModel { OriginCity = "Львів" };
        var empty = new StudentModel { OriginCity = "" };

        // Act & Assert
        Assert.False(local.IsFromAnotherCity("Київ"));
        Assert.True(nonLocal.IsFromAnotherCity("Київ"));
        Assert.False(empty.IsFromAnotherCity("Київ"));
    }

    [Fact]
    public void StudentModel_IsFirstCourseNonLocal_ChecksCourseAndCity()
    {
        // Arrange
        var s1 = new StudentModel { Course = 1, OriginCity = "Одеса" };
        var s2 = new StudentModel { Course = 2, OriginCity = "Одеса" };
        var s3 = new StudentModel { Course = 1, OriginCity = "Київ" };

        // Act & Assert
        Assert.True(s1.IsFirstCourseNonLocal("Київ"));
        Assert.False(s2.IsFirstCourseNonLocal("Київ"));
        Assert.False(s3.IsFirstCourseNonLocal("Київ"));
    }

    [Fact]
    public void StudentModel_GetDormitoryInfo_FormatsCorrectly()
    {
        // Arrange
        var notSettled = new StudentModel { IsSettledInDormitory = false };
        var settled = new StudentModel
        {
            IsSettledInDormitory = true,
            DormitoryNumber = 4,
            RoomNumber = 301,
            SettlementDate = new DateTime(2026, 9, 1)
        };
        var settledNoDate = new StudentModel
        {
            IsSettledInDormitory = true,
            DormitoryNumber = 2,
            RoomNumber = 105
        };

        // Act & Assert
        Assert.Contains("Не проживає", notSettled.GetDormitoryInfo());
        Assert.Contains("Гуртожиток №4, кімната 301", settled.GetDormitoryInfo());
        Assert.Contains("01.09.2026", settled.GetDormitoryInfo());
        Assert.Contains("не вказано", settledNoDate.GetDormitoryInfo());
    }

    [Fact]
    public void StudentModel_ToString_ContainsFullDetails()
    {
        // Arrange
        var s = new StudentModel
        {
            Id = 5,
            FirstName = "Тарас",
            LastName = "Шевченко",
            Course = 1,
            StudentId = "KB102030",
            OriginCity = "Канів",
            Passport = "СН123456"
        };

        // Act
        string str = s.ToString();

        // Assert
        Assert.Contains("Шевченко Тарас", str);
        Assert.Contains("KB102030", str);
        Assert.Contains("Канів", str);
    }

    [Fact]
    public void TaxiDriverModel_ToString_ContainsDetails()
    {
        // Arrange
        var driver = new TaxiDriverModel
        {
            Id = 1,
            FirstName = "Олег",
            LastName = "Бондар",
            CarModel = "Skoda Octavia",
            LicenseNumber = "BTA123456",
            ExperienceYears = 5,
            Age = 28,
            HasActiveLicense = true
        };

        // Act
        string str = driver.ToString();

        // Assert
        Assert.Contains("Skoda Octavia", str);
        Assert.Contains("BTA123456", str);
        Assert.Contains("Ліцензія активна", str);
    }

    [Fact]
    public void AcrobatModel_ToString_ContainsDetails()
    {
        // Arrange
        var acrobat = new AcrobatModel
        {
            Id = 1,
            FirstName = "Ольга",
            LastName = "Харлан",
            CircusOrTroupe = "Кобзов",
            Specialization = "Партер",
            SafetyCertificationValid = true,
            PerformancesCount = 15
        };

        // Act
        string str = acrobat.ToString();

        // Assert
        Assert.Contains("Кобзов", str);
        Assert.Contains("Партер", str);
        Assert.Contains("Виступів: 15", str);
    }

    [Fact]
    public void DormitorySettlementResult_Failure_CreatesFailedResult()
    {
        // Arrange & Act
        var res = DormitorySettlementResult.Failure("KB123456", "Іван Іванов", "Місцевий житель");

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Equal("KB123456", res.StudentId);
        Assert.Contains("відхилено", res.Message);
    }

    [Fact]
    public void DomainExceptions_Constructors_SetProperties()
    {
        // Arrange & Act
        var inner = new Exception("Inner error");
        var domainEx = new DomainException("Domain error", inner);
        var validationEx = new ValidationException("Validation error", "Prop");
        var storageEx = new StorageException("Storage error", inner);
        var storageExNoInner = new StorageException("Storage error single");
        var deniedEx = new DriverLicenseDeniedException("Reason text", "Denied error");

        // Assert
        Assert.Equal("Domain error", domainEx.Message);
        Assert.Same(inner, domainEx.InnerException);
        Assert.Equal("Prop", validationEx.PropertyName);
        Assert.Equal("Storage error", storageEx.Message);
        Assert.NotNull(storageExNoInner.InnerException);
        Assert.Equal("Reason text", deniedEx.Reason);
    }
}
