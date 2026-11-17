using Lab05.BLL.Exceptions;
using Lab05.BLL.Validation;

namespace Lab05.Tests.BLL;

public class EntityValidatorTests
{
    private readonly EntityValidator _validator = new();

    [Theory]
    [InlineData("Іван")]
    [InlineData("Олександр")]
    [InlineData("Д'Артаньян")]
    [InlineData("Жан-Люк")]
    [InlineData("John")]
    public void ValidateName_ValidName_DoesNotThrow(string name)
    {
        // Arrange, Act & Assert
        _validator.ValidateName(name, "FirstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("I")] // Too short (< 2)
    [InlineData("Ivan@Code")]
    public void ValidateName_InvalidName_ThrowsValidationException(string name)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateName(name, "FirstName"));
    }

    [Theory]
    [InlineData("KB123456")]
    [InlineData("АА123456")]
    [InlineData("BC12345678")]
    public void ValidateStudentId_ValidTicket_DoesNotThrow(string ticket)
    {
        // Arrange, Act & Assert
        _validator.ValidateStudentId(ticket);
    }

    [Theory]
    [InlineData("")]
    [InlineData("K123456")] // Only 1 letter
    [InlineData("KBB123456")] // 3 letters
    [InlineData("KB12345")] // Only 5 digits
    [InlineData("12345678")] // No letters
    public void ValidateStudentId_InvalidTicket_ThrowsValidationException(string ticket)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateStudentId(ticket));
    }

    [Theory]
    [InlineData("СН123456")]
    [InlineData("МЕ654321")]
    [InlineData("123456789")] // 9-digit modern ID card
    public void ValidatePassport_ValidPassport_DoesNotThrow(string passport)
    {
        // Arrange, Act & Assert
        _validator.ValidatePassport(passport);
    }

    [Theory]
    [InlineData("")]
    [InlineData("С123456")] // 1 letter
    [InlineData("12345678")] // 8 digits (neither 6 nor 9)
    [InlineData("СН12345")] // 5 digits
    public void ValidatePassport_InvalidPassport_ThrowsValidationException(string passport)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidatePassport(passport));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(6)]
    public void ValidateCourse_ValidRange_DoesNotThrow(int course)
    {
        // Arrange, Act & Assert
        _validator.ValidateCourse(course);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(-1)]
    public void ValidateCourse_OutOfRange_ThrowsValidationException(int course)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateCourse(course));
    }

    [Theory]
    [InlineData("Київ")]
    [InlineData("Львів")]
    [InlineData("Івано-Франківськ")]
    [InlineData("Кам'янець-Подільський")]
    public void ValidateCity_ValidCity_DoesNotThrow(string city)
    {
        // Arrange, Act & Assert
        _validator.ValidateCity(city);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123City")]
    public void ValidateCity_InvalidCity_ThrowsValidationException(string city)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateCity(city));
    }

    [Theory]
    [InlineData("BTA123456")]
    [InlineData("CXR987654")]
    public void ValidateDriverLicense_Valid_DoesNotThrow(string license)
    {
        // Arrange, Act & Assert
        _validator.ValidateDriverLicense(license);
    }

    [Theory]
    [InlineData("")]
    [InlineData("BT123456")] // 2 letters
    [InlineData("BTA12345")] // 5 digits
    public void ValidateDriverLicense_Invalid_ThrowsValidationException(string license)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateDriverLicense(license));
    }

    [Fact]
    public void ValidateExperience_Valid_DoesNotThrow()
    {
        // Arrange, Act & Assert
        _validator.ValidateExperience(5, 25);
    }

    [Fact]
    public void ValidateExperience_Under18_ThrowsValidationException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateExperience(0, 17));
    }

    [Fact]
    public void ValidateExperience_ExperienceExceedsAge_ThrowsValidationException()
    {
        // Arrange, Act & Assert: age 20, max possible exp is 20-18=2. Exp 5 is impossible.
        Assert.Throws<ValidationException>(() => _validator.ValidateExperience(5, 20));
    }

    [Fact]
    public void ValidateExperience_NegativeExperience_ThrowsValidationException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateExperience(-1, 25));
    }

    [Theory]
    [InlineData("Skoda Octavia")]
    [InlineData("BMW X5")]
    public void ValidateCarModel_ValidModel_DoesNotThrow(string model)
    {
        // Arrange, Act & Assert
        _validator.ValidateCarModel(model);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")] // Length < 2
    public void ValidateCarModel_InvalidModel_ThrowsValidationException(string model)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateCarModel(model));
    }

    [Fact]
    public void ValidateAcrobatFields_Valid_DoesNotThrow()
    {
        // Arrange, Act & Assert
        _validator.ValidateAcrobatFields("Цирк Кобзов", "Повітряна акробатика");
    }

    [Theory]
    [InlineData("", "Спеціалізація")]
    [InlineData("Цирк", "")]
    public void ValidateAcrobatFields_EmptyFields_ThrowsValidationException(string circus, string spec)
    {
        // Arrange, Act & Assert
        Assert.Throws<ValidationException>(() => _validator.ValidateAcrobatFields(circus, spec));
    }
}
