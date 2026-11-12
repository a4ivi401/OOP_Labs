using Lab01.Core.Validation;
using Xunit;

namespace Lab01.Tests.Validation;

public class ValidationHelperTests
{
    [Theory]
    [InlineData("КВ123456", true)]
    [InlineData("kb987654", true)]
    [InlineData("АБ000001", true)]
    [InlineData("123456", false)]
    [InlineData("КВ12345", false)]
    [InlineData("КВА123456", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidStudentId_ValidatesCorrectly(string? id, bool expected)
    {
        Assert.Equal(expected, ValidationHelper.IsValidStudentId(id));
    }

    [Theory]
    [InlineData("МЕ123456", true)]
    [InlineData("123456789", true)]
    [InlineData("me654321", true)]
    [InlineData("12345678", false)]
    [InlineData("М123456", false)]
    [InlineData("МЕЕ123456", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPassport_ValidatesCorrectly(string? passport, bool expected)
    {
        Assert.Equal(expected, ValidationHelper.IsValidPassport(passport));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(4, true)]
    [InlineData(6, true)]
    [InlineData(0, false)]
    [InlineData(7, false)]
    [InlineData(-2, false)]
    public void IsValidCourse_ValidatesRange1To6(int course, bool expected)
    {
        Assert.Equal(expected, ValidationHelper.IsValidCourse(course));
    }

    [Theory]
    [InlineData("Олександр", true)]
    [InlineData("Шевченко-Грушевський", true)]
    [InlineData("Д'Артаньян", true)]
    [InlineData("Іван", true)]
    [InlineData("A", false)]
    [InlineData("1234", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidName_ValidatesCorrectly(string? name, bool expected)
    {
        Assert.Equal(expected, ValidationHelper.IsValidName(name));
    }

    [Theory]
    [InlineData("Київ", true)]
    [InlineData("Івано-Франківськ", true)]
    [InlineData("Кам'янець-Подільський", true)]
    [InlineData("O", false)]
    [InlineData("", false)]
    public void IsValidCity_ValidatesCorrectly(string? city, bool expected)
    {
        Assert.Equal(expected, ValidationHelper.IsValidCity(city));
    }
}
