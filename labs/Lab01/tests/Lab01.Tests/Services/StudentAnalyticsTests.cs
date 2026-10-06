using Lab01.Core.Models;
using Lab01.Core.Services;
using Xunit;

namespace Lab01.Tests.Services;

public class StudentAnalyticsTests
{
    [Fact]
    public void CalculateFirstYearFromOtherCities_CalculatesCorrectPercentage()
    {
        // 4 першокурсники: 3 з інших міст (Львів, Одеса, Харків) та 1 з Києва
        // Очікуваний відсоток: 3 / 4 * 100 = 75.0%
        Student[] students = new Student[]
        {
            new("Олександр", "Петренко", 17, 1, "КВ111111", "Львів", "МЕ111111"),
            new("Марія", "Коваленко", 18, 1, "КВ222222", "Київ", "МЕ222222"),
            new("Дмитро", "Сидоренко", 17, 1, "КВ333333", "Одеса", "МЕ333333"),
            new("Анна", "Ткач", 18, 1, "КВ444444", "Харків", "МЕ444444"),
            new("Богдан", "Мельник", 19, 2, "КВ555555", "Полтава", "МЕ555555"), // 2 курс - не враховується
            new("Ірина", "Кравчук", 20, 3, "КВ666666", "Тернопіль", "МЕ666666")  // 3 курс - не враховується
        };

        AnalyticsReport report = StudentAnalytics.CalculateFirstYearFromOtherCities(students, "Київ");

        Assert.Equal(6, report.TotalStudents);
        Assert.Equal(4, report.TotalFirstYearStudents);
        Assert.Equal(3, report.OtherCitiesFirstYearStudents);
        Assert.Equal(75.0, report.Percentage, precision: 2);
    }

    [Fact]
    public void CalculateFirstYearFromOtherCities_WhenNoFirstYearStudents_ReturnsZeroPercentage()
    {
        Student[] students = new Student[]
        {
            new("Богдан", "Мельник", 19, 2, "КВ555555", "Полтава", "МЕ555555"),
            new("Ірина", "Кравчук", 20, 3, "КВ666666", "Тернопіль", "МЕ666666")
        };

        AnalyticsReport report = StudentAnalytics.CalculateFirstYearFromOtherCities(students, "Київ");

        Assert.Equal(2, report.TotalStudents);
        Assert.Equal(0, report.TotalFirstYearStudents);
        Assert.Equal(0, report.OtherCitiesFirstYearStudents);
        Assert.Equal(0.0, report.Percentage);
    }

    [Fact]
    public void CalculateFirstYearFromOtherCities_WhenAllFromBaseCity_ReturnsZeroPercentage()
    {
        Student[] students = new Student[]
        {
            new("Олександр", "Петренко", 17, 1, "КВ111111", "Київ", "МЕ111111"),
            new("Марія", "Коваленко", 18, 1, "КВ222222", "київ", "МЕ222222") // Перевірка регістронезалежності
        };

        AnalyticsReport report = StudentAnalytics.CalculateFirstYearFromOtherCities(students, "Київ");

        Assert.Equal(2, report.TotalFirstYearStudents);
        Assert.Equal(0, report.OtherCitiesFirstYearStudents);
        Assert.Equal(0.0, report.Percentage);
    }

    [Fact]
    public void GetFirstYearStudentsFromOtherCities_ReturnsFilteredArray()
    {
        Student[] students = new Student[]
        {
            new("Олександр", "Петренко", 17, 1, "КВ111111", "Львів", "МЕ111111"),
            new("Марія", "Коваленко", 18, 1, "КВ222222", "Київ", "МЕ222222"),
            new("Богдан", "Мельник", 19, 2, "КВ555555", "Одеса", "МЕ555555")
        };

        Student[] result = StudentAnalytics.GetFirstYearStudentsFromOtherCities(students, "Київ");

        Assert.Single(result);
        Assert.Equal("Петренко", result[0].LastName);
        Assert.Equal("Львів", result[0].CityOfOrigin);
    }
}
