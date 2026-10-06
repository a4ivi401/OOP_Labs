using System.Text;
using System.Text.RegularExpressions;
using Lab01.Core.Common;
using Lab01.Core.Models;

namespace Lab01.Core.IO;

/// <summary>
/// Сервіс для збереження та завантаження сутностей у спеціальному текстовому форматі
/// згідно з методичними вказівками лабораторної роботи:
/// TypeName ObjectName
/// { "attr1": "val1",
/// "attr2": "val2" };
///
/// Використовує низькорівневі потоки введення/виведення: FileStream, StreamReader, StreamWriter.
/// Працює виключно з масивами T[] без використання стандартних колекцій.
/// </summary>
public static class CustomFileService
{
    private static readonly Regex KeyValueRegex = new(
        @"\""([^\""]+)\""\s*:\s*\""([^\""]*)\""",
        RegexOptions.Compiled);

    /// <summary>
    /// Зберігає масив студентів у файл у заданому дужковому форматі за допомогою FileStream та StreamWriter.
    /// </summary>
    /// <param name="filePath">Шлях до файлу для збереження.</param>
    /// <param name="students">Масив студентів.</param>
    public static void SaveStudents(string filePath, Student[] students)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(students);

        // Створюємо або перезаписуємо файл, використовуючи FileStream
        using FileStream fileStream = new(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        // Обгортаємо потік у StreamWriter з кодуванням UTF-8 для коректного збереження кирилиці
        using StreamWriter writer = new(fileStream, Encoding.UTF8);

        for (int i = 0; i < students.Length; i++)
        {
            Student s = students[i];
            string objectName = $"Student_{i + 1}";

            writer.WriteLine($"Student {objectName}");
            writer.WriteLine("{");
            writer.WriteLine($"  \"FirstName\": \"{EscapeString(s.FirstName)}\",");
            writer.WriteLine($"  \"LastName\": \"{EscapeString(s.LastName)}\",");
            writer.WriteLine($"  \"Age\": \"{s.Age}\",");
            writer.WriteLine($"  \"Course\": \"{s.Course}\",");
            writer.WriteLine($"  \"StudentId\": \"{EscapeString(s.StudentId)}\",");
            writer.WriteLine($"  \"CityOfOrigin\": \"{EscapeString(s.CityOfOrigin)}\",");
            writer.WriteLine($"  \"Passport\": \"{EscapeString(s.Passport)}\"");
            writer.WriteLine("};");

            // Розділяємо об'єкти порожнім рядком для зручності читання
            if (i < students.Length - 1)
            {
                writer.WriteLine();
            }
        }
    }

    /// <summary>
    /// Зчитує студентів з текстового файлу за допомогою FileStream та StreamReader.
    /// Повертає звичайний фіксований масив Student[].
    /// </summary>
    /// <param name="filePath">Шлях до файлу для читання.</param>
    /// <returns>Масив відновлених об'єктів Student.</returns>
    public static Student[] LoadStudents(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл за шляхом '{filePath}' не знайдено.");
        }

        Student[] result = Array.Empty<Student>();

        // Відкриваємо файл на читання через FileStream
        using FileStream fileStream = new(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        // Використовуємо StreamReader для читання текстового потоку
        using StreamReader reader = new(fileStream, Encoding.UTF8);

        string? line;
        bool insideBlock = false;

        string firstName = string.Empty;
        string lastName = string.Empty;
        int age = 18;
        int course = 1;
        string studentId = string.Empty;
        string cityOfOrigin = string.Empty;
        string passport = string.Empty;

        while ((line = reader.ReadLine()) != null)
        {
            string trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            // Початок блоку об'єкта
            if (trimmed.StartsWith('{'))
            {
                insideBlock = true;
                continue;
            }

            // Кінець блоку об'єкта
            if (trimmed.StartsWith("};") || trimmed == "}")
            {
                insideBlock = false;

                // Перевіряємо наявність обов'язкових полів перед створенням студента
                if (!string.IsNullOrEmpty(firstName) &&
                    !string.IsNullOrEmpty(lastName) &&
                    !string.IsNullOrEmpty(studentId) &&
                    !string.IsNullOrEmpty(cityOfOrigin) &&
                    !string.IsNullOrEmpty(passport))
                {
                    Student student = new(
                        firstName,
                        lastName,
                        age,
                        course,
                        studentId,
                        cityOfOrigin,
                        passport);

                    result = ArrayHelper.Add(result, student);
                }

                // Скидаємо тимчасові змінні для наступного об'єкта
                firstName = string.Empty;
                lastName = string.Empty;
                age = 18;
                course = 1;
                studentId = string.Empty;
                cityOfOrigin = string.Empty;
                passport = string.Empty;

                continue;
            }

            // Якщо ми всередині фігурних дужок - парсимо пари "ключ": "значення"
            if (insideBlock)
            {
                Match match = KeyValueRegex.Match(trimmed);
                if (match.Success)
                {
                    string key = match.Groups[1].Value.Trim();
                    string value = match.Groups[2].Value.Trim();

                    switch (key)
                    {
                        case "FirstName":
                            firstName = UnescapeString(value);
                            break;
                        case "LastName":
                            lastName = UnescapeString(value);
                            break;
                        case "Age":
                            int.TryParse(value, out age);
                            break;
                        case "Course":
                            int.TryParse(value, out course);
                            break;
                        case "StudentId":
                            studentId = UnescapeString(value);
                            break;
                        case "CityOfOrigin":
                            cityOfOrigin = UnescapeString(value);
                            break;
                        case "Passport":
                            passport = UnescapeString(value);
                            break;
                    }
                }
            }
        }

        return result;
    }

    private static string EscapeString(string value)
    {
        return value.Replace("\"", "\\\"");
    }

    private static string UnescapeString(string value)
    {
        return value.Replace("\\\"", "\"");
    }
}
