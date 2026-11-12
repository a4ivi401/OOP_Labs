using Lab01.Core.Models;
using Xunit;

namespace Lab01.Tests.Core;

public class ModelTests
{
    [Fact]
    public void Student_Initialization_SetsPropertiesCorrectly()
    {
        Student student = new("Тарас", "Шевченко", 18, 1, "КВ123456", "Львів", "МЕ123456");

        Assert.Equal("Тарас", student.FirstName);
        Assert.Equal("Шевченко", student.LastName);
        Assert.Equal(18, student.Age);
        Assert.Equal(1, student.Course);
        Assert.Equal("КВ123456", student.StudentId);
        Assert.Equal("Львів", student.CityOfOrigin);
        Assert.Equal("МЕ123456", student.Passport);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(-1)]
    public void Student_InvalidCourse_ThrowsArgumentOutOfRangeException(int invalidCourse)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Student("Тарас", "Шевченко", 18, invalidCourse, "КВ123456", "Львів", "МЕ123456"));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("ABC123456")]
    [InlineData("K123456")]
    public void Student_InvalidStudentId_ThrowsArgumentException(string invalidId)
    {
        Assert.Throws<ArgumentException>(() =>
            new Student("Тарас", "Шевченко", 18, 1, invalidId, "Львів", "МЕ123456"));
    }

    [Fact]
    public void Student_Study_ReturnsExpectedString()
    {
        Student student = new("Іван", "Франко", 19, 2, "КВ654321", "Дрогобич", "МЕ654321");
        string studyText = student.Study();

        Assert.Contains("Франко", studyText);
        Assert.Contains("2-му курсі", studyText);
    }

    [Fact]
    public void Polymorphism_IDanceable_DemonstratesPolymorphicBehavior()
    {
        Student student = new("Олександр", "Петренко", 18, 1, "КВ111111", "Київ", "МЕ111111");
        Acrobat acrobat = new("Олена", "Гончар", 24, "Національний Цирк", "Професіонал");

        IDanceable[] dancers = new IDanceable[] { student, acrobat };

        Assert.Equal(2, dancers.Length);
        Assert.Contains("вальсі", dancers[0].Dance());
        Assert.Contains("акробатичний танець", dancers[1].Dance());
    }

    [Fact]
    public void TaxiDriver_Drive_ReturnsCorrectDescription()
    {
        TaxiDriver driver = new("Василь", "Коваленко", 40, "ВХ123456", "Skoda Octavia", 12);
        string driveText = driver.Drive();

        Assert.Contains("Skoda Octavia", driveText);
        Assert.Contains("12 р.", driveText);
    }

    [Fact]
    public void Acrobat_PerformAcrobatics_ReturnsCorrectDescription()
    {
        Acrobat acrobat = new("Максим", "Кравченко", 22, "Arena", "Майстер спорту");
        string trickText = acrobat.PerformAcrobatics();

        Assert.Contains("сальто", trickText);
        Assert.Contains("Arena", trickText);
    }
}
