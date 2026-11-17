using Lab05.BLL.Interfaces;
using Lab05.BLL.Models;

namespace Lab05.Tests.BLL;

public class PolymorphismAndDancerTests
{
    [Fact]
    public void Student_ImplementsIDancer_DancesSuccessfully()
    {
        // Arrange
        IDancer student = new StudentModel
        {
            FirstName = "Тарас",
            LastName = "Шевченко",
            Course = 1,
            StudentId = "KB102030"
        };

        // Act
        string result = student.Dance("Полонез");

        // Assert
        Assert.Contains("Студент Шевченко Тарас", result);
        Assert.Contains("Полонез", result);
    }

    [Fact]
    public void TaxiDriver_ImplementsIDancer_DancesSuccessfully()
    {
        // Arrange
        IDancer driver = new TaxiDriverModel
        {
            FirstName = "Василь",
            LastName = "Петренко",
            CarModel = "Skoda Octavia",
            ExperienceYears = 8
        };

        // Act
        string result = driver.Dance("Танго");

        // Assert
        Assert.Contains("Водій таксі Петренко Василь", result);
        Assert.Contains("Skoda Octavia", result);
        Assert.Contains("Танго", result);
    }

    [Fact]
    public void Acrobat_ImplementsIDancer_DancesSuccessfully()
    {
        // Arrange
        IDancer acrobat = new AcrobatModel
        {
            FirstName = "Вікторія",
            LastName = "Лисенко",
            CircusOrTroupe = "Шапіто",
            Specialization = "Повітряна акробатика"
        };

        // Act
        string result = acrobat.Dance("Вальс");

        // Assert
        Assert.Contains("Акробат Лисенко Вікторія", result);
        Assert.Contains("Шапіто", result);
        Assert.Contains("Вальс", result);
    }

    [Fact]
    public void PolymorphicDancerList_ExecutesAllDancersCleanly()
    {
        // Arrange
        var dancers = new List<IDancer>
        {
            new StudentModel { FirstName = "Олег", LastName = "Ткач", StudentId = "KB111111" },
            new TaxiDriverModel { FirstName = "Іван", LastName = "Сірко", CarModel = "Lanos" },
            new AcrobatModel { FirstName = "Марія", LastName = "Заньковецька", CircusOrTroupe = "Цирк" }
        };

        // Act
        var results = dancers.Select(d => d.Dance("Брейк-данс")).ToList();

        // Assert
        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.Contains("Брейк-данс", r));
    }
}
