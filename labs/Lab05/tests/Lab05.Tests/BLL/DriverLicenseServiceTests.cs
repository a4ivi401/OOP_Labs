using Lab05.BLL.Exceptions;
using Lab05.BLL.Models;
using Lab05.BLL.Services;
using Lab05.BLL.Validation;
using Moq;

namespace Lab05.Tests.BLL;

public class DriverLicenseServiceTests
{
    private readonly Mock<IEntityValidator> _validatorMock;
    private readonly DriverLicenseService _service;

    public DriverLicenseServiceTests()
    {
        _validatorMock = new Mock<IEntityValidator>();
        _service = new DriverLicenseService(_validatorMock.Object);
    }

    [Fact]
    public void Constructor_NullValidator_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DriverLicenseService(null!));
    }

    [Fact]
    public void ApplyForLicense_AllConditionsMet_IssuesDriverLicense()
    {
        // Arrange
        var app = new DriverLicenseApplication
        {
            CandidateName = "Олександр Коваленко",
            Age = 20,
            HasMedicalCertificate = true,
            TheoryScore = 19,
            PassedPracticalTest = true,
            RequestedCategory = "B",
            HasCourtBan = false,
            PaidStateFee = true
        };

        // Act
        var result = _service.ApplyForLicense(app);

        // Assert
        Assert.True(result.IsApproved);
        Assert.Equal("Олександр Коваленко", result.CandidateName);
        Assert.Equal("B", result.Category);
        Assert.False(string.IsNullOrWhiteSpace(result.LicenseNumber));
        Assert.NotNull(result.IssueDate);
        Assert.NotNull(result.ExpiryDate);
        Assert.True(result.ExpiryDate > result.IssueDate);
        Assert.Contains("ПОСВІДЧЕННЯ ВОДІЯ ВИДАНО", result.Summary);
    }

    [Fact]
    public void ApplyForLicense_AgeUnder18_ThrowsDriverLicenseDeniedException()
    {
        // Arrange
        var app = new DriverLicenseApplication
        {
            CandidateName = "Неповнолітній Кандидат",
            Age = 17, // Under 18
            HasMedicalCertificate = true,
            TheoryScore = 20,
            PassedPracticalTest = true,
            RequestedCategory = "B",
            HasCourtBan = false,
            PaidStateFee = true
        };

        // Act & Assert
        var ex = Assert.Throws<DriverLicenseDeniedException>(() => _service.ApplyForLicense(app));
        Assert.Contains("18 років", ex.Message);
    }

    [Fact]
    public void ApplyForLicense_NoMedicalCertificate_ThrowsDriverLicenseDeniedException()
    {
        // Arrange
        var app = new DriverLicenseApplication
        {
            CandidateName = "Кандидат Без Довідки",
            Age = 22,
            HasMedicalCertificate = false, // No medical cert
            TheoryScore = 19,
            PassedPracticalTest = true,
            RequestedCategory = "B",
            PaidStateFee = true
        };

        // Act & Assert
        var ex = Assert.Throws<DriverLicenseDeniedException>(() => _service.ApplyForLicense(app));
        Assert.Contains("медична довідка", ex.Message);
    }

    [Fact]
    public void ApplyForLicense_FailedTheoryTest_ThrowsDriverLicenseDeniedException()
    {
        // Arrange
        var app = new DriverLicenseApplication
        {
            CandidateName = "Кандидат Без Теорії",
            Age = 25,
            HasMedicalCertificate = true,
            TheoryScore = 16, // Failed: < 18
            PassedPracticalTest = true,
            PaidStateFee = true
        };

        // Act & Assert
        var ex = Assert.Throws<DriverLicenseDeniedException>(() => _service.ApplyForLicense(app));
        Assert.Contains("теоретичний іспит", ex.Message);
    }

    [Fact]
    public void ApplyForLicense_FailedPracticalTest_ThrowsDriverLicenseDeniedException()
    {
        // Arrange
        var app = new DriverLicenseApplication
        {
            CandidateName = "Кандидат Без Практики",
            Age = 25,
            HasMedicalCertificate = true,
            TheoryScore = 19,
            PassedPracticalTest = false, // Failed practical
            PaidStateFee = true
        };

        // Act & Assert
        var ex = Assert.Throws<DriverLicenseDeniedException>(() => _service.ApplyForLicense(app));
        Assert.Contains("практичний іспит", ex.Message);
    }

    [Fact]
    public void ApplyForLicense_HasCourtBan_ThrowsDriverLicenseDeniedException()
    {
        // Arrange
        var app = new DriverLicenseApplication
        {
            CandidateName = "Порушник Із Забороною",
            Age = 30,
            HasMedicalCertificate = true,
            TheoryScore = 20,
            PassedPracticalTest = true,
            HasCourtBan = true, // Court ban
            PaidStateFee = true
        };

        // Act & Assert
        var ex = Assert.Throws<DriverLicenseDeniedException>(() => _service.ApplyForLicense(app));
        Assert.Contains("судове рішення", ex.Message);
    }

    [Fact]
    public void ApplyForLicense_UnpaidFee_ThrowsDriverLicenseDeniedException()
    {
        // Arrange
        var app = new DriverLicenseApplication
        {
            CandidateName = "Кандидат Без Оплати",
            Age = 28,
            HasMedicalCertificate = true,
            TheoryScore = 20,
            PassedPracticalTest = true,
            PaidStateFee = false // Not paid
        };

        // Act & Assert
        var ex = Assert.Throws<DriverLicenseDeniedException>(() => _service.ApplyForLicense(app));
        Assert.Contains("державне мито", ex.Message);
    }

    [Fact]
    public void VerifyLicenseConditions_MultipleFailures_ListsAllReasons()
    {
        // Arrange: Age 16, no medical, failed theory, failed practice
        var app = new DriverLicenseApplication
        {
            CandidateName = "Кандидат",
            Age = 16,
            HasMedicalCertificate = false,
            TheoryScore = 12,
            PassedPracticalTest = false,
            HasCourtBan = true,
            PaidStateFee = false
        };

        // Act
        bool eligible = _service.VerifyLicenseConditions(app, out var reasons);

        // Assert
        Assert.False(eligible);
        Assert.Equal(6, reasons.Count);
    }

    [Fact]
    public void GenerateLicenseNumber_ReturnsValidFormat()
    {
        // Arrange & Act
        string number = _service.GenerateLicenseNumber();

        // Assert
        Assert.Equal(9, number.Length); // 3 letters + 6 digits
        Assert.Matches(@"^[A-Z]{3}\d{6}$", number);
    }
}
