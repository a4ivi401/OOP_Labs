using System.Text.RegularExpressions;

namespace Lab01.Core.Validation;

/// <summary>
/// Статичний клас для валідації вхідних даних за допомогою регулярних виразів (Regex).
/// Забезпечує цілісність даних та відповідність вимогам завдання.
/// </summary>
public static class ValidationHelper
{
    // Шаблон для імен, прізвищ та назв міст (підтримує українські та латинські літери, дефіс, апостроф)
    private static readonly Regex TextNameRegex = new(
        @"^[A-Za-zА-Яа-яІіЇїЄєҐґ'\- ]{2,50}$",
        RegexOptions.Compiled);

    // Шаблон для студентського квитка: 2 літери + 6 цифр (наприклад, КВ123456 або KB123456)
    private static readonly Regex StudentIdRegex = new(
        @"^[A-Za-zА-Яа-яІіЇїЄєҐґ]{2}\d{6}$",
        RegexOptions.Compiled);

    // Шаблон для паспорта: старий зразок (2 літери + 6 цифр) або ID-картка (9 цифр)
    private static readonly Regex PassportRegex = new(
        @"^([A-Za-zА-Яа-яІіЇїЄєҐґ]{2}\d{6}|\d{9})$",
        RegexOptions.Compiled);

    /// <summary>
    /// Перевіряє коректність імені або прізвища.
    /// </summary>
    public static bool IsValidName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && TextNameRegex.IsMatch(name.Trim());
    }

    /// <summary>
    /// Перевіряє коректність назви міста.
    /// </summary>
    public static bool IsValidCity(string? city)
    {
        return !string.IsNullOrWhiteSpace(city) && TextNameRegex.IsMatch(city.Trim());
    }

    /// <summary>
    /// Перевіряє номер студентського квитка (формат: 2 літери + 6 цифр).
    /// </summary>
    public static bool IsValidStudentId(string? studentId)
    {
        return !string.IsNullOrWhiteSpace(studentId) && StudentIdRegex.IsMatch(studentId.Trim());
    }

    /// <summary>
    /// Перевіряє паспортні дані (серія і номер або ID-картка з 9 цифр).
    /// </summary>
    public static bool IsValidPassport(string? passport)
    {
        return !string.IsNullOrWhiteSpace(passport) && PassportRegex.IsMatch(passport.Trim());
    }

    /// <summary>
    /// Перевіряє номер курсу (від 1 до 6).
    /// </summary>
    public static bool IsValidCourse(int course)
    {
        return course is >= 1 and <= 6;
    }

    /// <summary>
    /// Перевіряє вік студента (від 15 до 90 років для вищої освіти).
    /// </summary>
    public static bool IsValidStudentAge(int age)
    {
        return age is >= 15 and <= 90;
    }
}
