using Lab01.Core.Common;
using Lab01.Core.Models;

namespace Lab01.Core.Services;

/// <summary>
/// Результат аналітичного розрахунку.
/// </summary>
public class AnalyticsReport
{
    public int TotalStudents { get; set; }
    public int TotalFirstYearStudents { get; set; }
    public int OtherCitiesFirstYearStudents { get; set; }
    public double Percentage { get; set; }
    public string BaseCity { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Загалом студентів: {TotalStudents}\n" +
               $"Студентів 1-го курсу: {TotalFirstYearStudents}\n" +
               $"З інших міст (відмінних від {BaseCity}): {OtherCitiesFirstYearStudents}\n" +
               $"Відсоток іногородніх першокурсників: {Percentage:F2}%";
    }
}

/// <summary>
/// Сервіс для аналізу даних студентів (реалізація розрахункової частини).
/// Виконує обчислення виключно через операції з масивами Student[].
/// </summary>
public static class StudentAnalytics
{
    public const string DefaultBaseCity = "Київ";

    /// <summary>
    /// Обчислює відсоток студентів 1-го курсу, які приїхали з інших міст (Варіант 2).
    /// </summary>
    /// <param name="students">Масив усіх студентів.</param>
    /// <param name="baseCity">Місто університету (за замовчуванням "Київ"). Всі інші міста вважаються "іншими".</param>
    /// <returns>Звіт з кількісними показниками та відсотком.</returns>
    public static AnalyticsReport CalculateFirstYearFromOtherCities(
        Student[] students,
        string baseCity = DefaultBaseCity)
    {
        ArgumentNullException.ThrowIfNull(students);

        if (string.IsNullOrWhiteSpace(baseCity))
        {
            baseCity = DefaultBaseCity;
        }

        int totalStudents = students.Length;

        // Кількість першокурсників
        int totalFirstYear = ArrayHelper.Count(students, s => s.Course == 1);

        // Кількість першокурсників, які прибули з інших міст
        int otherCitiesFirstYear = ArrayHelper.Count(
            students,
            s => s.Course == 1 && !string.Equals(s.CityOfOrigin, baseCity, StringComparison.OrdinalIgnoreCase));

        // Обчислення відсотка: уникнення ділення на нуль
        double percentage = totalFirstYear > 0
            ? ((double)otherCitiesFirstYear / totalFirstYear) * 100.0
            : 0.0;

        return new AnalyticsReport
        {
            TotalStudents = totalStudents,
            TotalFirstYearStudents = totalFirstYear,
            OtherCitiesFirstYearStudents = otherCitiesFirstYear,
            Percentage = percentage,
            BaseCity = baseCity
        };
    }

    /// <summary>
    /// Отримує масив студентів 1-го курсу, які прибули з інших міст.
    /// </summary>
    public static Student[] GetFirstYearStudentsFromOtherCities(
        Student[] students,
        string baseCity = DefaultBaseCity)
    {
        ArgumentNullException.ThrowIfNull(students);

        if (string.IsNullOrWhiteSpace(baseCity))
        {
            baseCity = DefaultBaseCity;
        }

        return ArrayHelper.Filter(
            students,
            s => s.Course == 1 && !string.Equals(s.CityOfOrigin, baseCity, StringComparison.OrdinalIgnoreCase));
    }
}
