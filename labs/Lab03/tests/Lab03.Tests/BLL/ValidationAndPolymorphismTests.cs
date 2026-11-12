using Lab03.BLL.Exceptions;
using Lab03.BLL.Interfaces;
using Lab03.BLL.Models;

namespace Lab03.Tests.BLL;

public class ValidationAndPolymorphismTests
{
    [Fact]
    public void StudentModel_InvalidCourse_ShouldThrowValidationException()
    {
        Assert.Throws<ValidationException>(() => new StudentModel("1", "Іван", "Франко", 0, "KB123456", "Львів", "СН123456"));
        Assert.Throws<ValidationException>(() => new StudentModel("1", "Іван", "Франко", 7, "KB123456", "Львів", "СН123456"));
    }

    [Fact]
    public void StudentModel_InvalidStudentId_ShouldThrowValidationException()
    {
        Assert.Throws<ValidationException>(() => new StudentModel("1", "Іван", "Франко", 1, "K123456", "Львів", "СН123456")); // 1 letter
        Assert.Throws<ValidationException>(() => new StudentModel("1", "Іван", "Франко", 1, "KB12", "Львів", "СН123456"));     // too short digits
    }

    [Fact]
    public void StudentModel_InvalidPassport_ShouldThrowValidationException()
    {
        Assert.Throws<ValidationException>(() => new StudentModel("1", "Іван", "Франко", 1, "KB123456", "Львів", "123")); // invalid passport
    }

    [Fact]
    public void TaxiDriverModel_InvalidExperience_ShouldThrowValidationException()
    {
        Assert.Throws<ValidationException>(() => new TaxiDriverModel("1", "Василь", "Шофер", "Toyota", "ВХА123456", -1));
        Assert.Throws<ValidationException>(() => new TaxiDriverModel("1", "Василь", "Шофер", "Toyota", "ВХА123456", 80));
    }

    [Fact]
    public void AcrobatModel_EmptyTroupeOrSpecialization_ShouldThrowValidationException()
    {
        Assert.Throws<ValidationException>(() => new AcrobatModel("1", "Анна", "Повітряна", "", "Повітряне кільце"));
        Assert.Throws<ValidationException>(() => new AcrobatModel("1", "Анна", "Повітряна", "Цирк", ""));
    }

    [Fact]
    public void IDancer_PolymorphicDanceCalls_ShouldSucceedForAllEntities()
    {
        List<IDancer> dancers = new()
        {
            new StudentModel("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456"),
            new TaxiDriverModel("2", "Василь", "Шофер", "Toyota Camry", "ВХА123456", 10),
            new AcrobatModel("3", "Анна", "Повітряна", "Національний Цирк", "Повітряне кільце")
        };

        foreach (IDancer dancer in dancers)
        {
            string danceResult = dancer.Dance("Танго");
            Assert.Contains("Танго", danceResult);
            Assert.False(string.IsNullOrWhiteSpace(danceResult));
        }
    }
}
