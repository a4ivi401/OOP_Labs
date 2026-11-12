using Lab01.Core.Common;
using Lab01.Core.IO;
using Lab01.Core.Models;
using Lab01.Core.Services;
using Lab01.Core.Validation;

namespace Lab01.App;

/// <summary>
/// Консольний інтерфейс користувача (CLI) для демонстрації можливостей Лабораторної роботи №1.
/// Підтримує меню, валідацію вводу, збереження/завантаження файлів та демонстрацію поліморфізму.
/// </summary>
public class ConsoleMenu
{
    private Student[] _students;
    private const string DefaultFileName = "students.txt";

    public ConsoleMenu()
    {
        _students = GetInitialMockData();
    }

    /// <summary>
    /// Запуск головного циклу консольного меню.
    /// </summary>
    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        bool exit = false;
        while (!exit)
        {
            PrintHeader();
            Console.WriteLine("1. Показати список усіх студентів");
            Console.WriteLine("2. Додати нового студента (з валідацією полів)");
            Console.WriteLine("3. Знайти студента за прізвищем або студентським квитком");
            Console.WriteLine("4. Видалити студента за номером студентського квитка");
            Console.WriteLine("5. Обчислити відсоток студентів 1-го курсу з інших міст");
            Console.WriteLine("6. Демонстрація IDanceable, Спадкування та поліморфних викликів");
            Console.WriteLine("7. Зберегти студентів у файл (формат згідно з ТЗ)");
            Console.WriteLine("8. Завантажити студентів із файлу");
            Console.WriteLine("9. Скинути список до початкових демо-даних");
            Console.WriteLine("0. Вихід");
            Console.WriteLine("====================================================================");
            Console.Write("Оберіть пункт меню (0-9): ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ShowAllStudents();
                    break;
                case "2":
                    AddNewStudent();
                    break;
                case "3":
                    SearchStudent();
                    break;
                case "4":
                    DeleteStudent();
                    break;
                case "5":
                    CalculateVariant2();
                    break;
                case "6":
                    DemonstratePolymorphism();
                    break;
                case "7":
                    SaveToFile();
                    break;
                case "8":
                    LoadFromFile();
                    break;
                case "9":
                    ResetToMockData();
                    break;
                case "0":
                    exit = true;
                    Console.WriteLine("Роботу програми завершено. До зустрічі!");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    Console.ResetColor();
                    break;
            }

            if (!exit)
            {
                Console.WriteLine();
                Console.Write("Натисніть Enter для повернення в меню...");
                Console.ReadLine();
            }
        }
    }

    private static void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================================================");
        Console.WriteLine("   ЛАБОРАТОРНА РОБОТА №1 | ВАРІАНТ 2   ");
        Console.WriteLine("   Тема: Спадкування, інтерфейси (IDanceable), масиви та файлові потоки");
        Console.WriteLine("====================================================================");
        Console.ResetColor();
    }

    private void ShowAllStudents()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Список зареєстрованих студентів ===");
        Console.ResetColor();

        if (_students.Length == 0)
        {
            Console.WriteLine("Список порожній. Додайте студентів або завантажте з файлу.");
            return;
        }

        Console.WriteLine($"Усього студентів у системі: {_students.Length}\n");
        for (int i = 0; i < _students.Length; i++)
        {
            Student s = _students[i];
            Console.WriteLine($"[{i + 1}] {s.GetInfo()}");
        }
    }

    private void AddNewStudent()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Додавання нового студента (покрокова валідація) ===");
        Console.ResetColor();

        string lastName = PromptValidatedInput(
            "Введіть прізвище: ",
            ValidationHelper.IsValidName,
            "Помилка: Прізвище має містити від 2 до 50 літер.");

        string firstName = PromptValidatedInput(
            "Введіть ім'я: ",
            ValidationHelper.IsValidName,
            "Помилка: Ім'я має містити від 2 до 50 літер.");

        int age = PromptValidatedInt(
            "Введіть вік (15-90): ",
            ValidationHelper.IsValidStudentAge,
            "Помилка: Вік студента повинен бути у межах від 15 до 90 років.");

        int course = PromptValidatedInt(
            "Введіть курс (1-6): ",
            ValidationHelper.IsValidCourse,
            "Помилка: Курс має бути цілим числом від 1 до 6.");

        string studentId = PromptValidatedInput(
            "Введіть номер студентського квитка (напр., КВ123456): ",
            s => ValidationHelper.IsValidStudentId(s) && !IsStudentIdTaken(s),
            "Помилка: Формат квитка повинен містити 2 літери і 6 цифр, та бути унікальним.");

        string city = PromptValidatedInput(
            "Введіть місто прибуття (напр., Львів, Київ): ",
            ValidationHelper.IsValidCity,
            "Помилка: Некоректна назва міста.");

        string passport = PromptValidatedInput(
            "Введіть серію і номер паспорта (напр., МЕ123456 або 001234567): ",
            ValidationHelper.IsValidPassport,
            "Помилка: Некоректний паспорт (очікується 2 літери + 6 цифр або 9 цифр ID).");

        try
        {
            Student newStudent = new(firstName, lastName, age, course, studentId, city, passport);
            _students = ArrayHelper.Add(_students, newStudent);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nСтудента {lastName} {firstName} успішно додано до бази даних!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nПомилка створення студента: {ex.Message}");
            Console.ResetColor();
        }
    }

    private void SearchStudent()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Пошук студента ===");
        Console.ResetColor();

        Console.Write("Введіть прізвище або номер студентського квитка для пошуку: ");
        string? query = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("Пошуковий запит не може бути порожнім.");
            return;
        }

        Student[] matches = ArrayHelper.Filter(_students, s =>
            s.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(s.StudentId, query, StringComparison.OrdinalIgnoreCase));

        if (matches.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Студентів за запитом '{query}' не знайдено.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Знайдено співпадінь: {matches.Length}\n");
        Console.ResetColor();

        for (int i = 0; i < matches.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {matches[i].GetInfo()}");
        }
    }

    private void DeleteStudent()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Видалення студента ===");
        Console.ResetColor();

        Console.Write("Введіть номер студентського квитка для видалення: ");
        string? studentId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(studentId))
        {
            Console.WriteLine("Номер квитка не введено.");
            return;
        }

        int index = ArrayHelper.FindIndex(_students, s =>
            string.Equals(s.StudentId, studentId, StringComparison.OrdinalIgnoreCase));

        if (index == -1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Студента з квитком '{studentId}' не знайдено.");
            Console.ResetColor();
            return;
        }

        Student toRemove = _students[index];
        _students = ArrayHelper.RemoveAt(_students, index);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Студента {toRemove.LastName} {toRemove.FirstName} (квиток: {toRemove.StudentId}) успішно видалено.");
        Console.ResetColor();
    }

    private void CalculateVariant2()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================================================");
        Console.WriteLine("  РОЗРАХУНКОВА ЧАСТИНА:");
        Console.WriteLine("  'Обчислити відсоток студентів 1-го курсу, які приїхали з інших міст'");
        Console.WriteLine("====================================================================");
        Console.ResetColor();

        Console.Write($"Введіть базове місто університету (Enter для '{StudentAnalytics.DefaultBaseCity}'): ");
        string? inputCity = Console.ReadLine()?.Trim();
        string baseCity = string.IsNullOrWhiteSpace(inputCity) ? StudentAnalytics.DefaultBaseCity : inputCity;

        AnalyticsReport report = StudentAnalytics.CalculateFirstYearFromOtherCities(_students, baseCity);
        Student[] otherCitiesStudents = StudentAnalytics.GetFirstYearStudentsFromOtherCities(_students, baseCity);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(report.ToString());
        Console.ResetColor();

        if (otherCitiesStudents.Length > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nСписок студентів 1-го курсу з інших міст ({otherCitiesStudents.Length}):");
            Console.ResetColor();

            for (int i = 0; i < otherCitiesStudents.Length; i++)
            {
                Student s = otherCitiesStudents[i];
                Console.WriteLine($"  {i + 1}. {s.LastName} {s.FirstName} | Місто: {s.CityOfOrigin} | Квиток: {s.StudentId}");
            }
        }
        else
        {
            Console.WriteLine("\nІногородніх першокурсників у поточній вибірці немає.");
        }
    }

    private void DemonstratePolymorphism()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("====================================================================");
        Console.WriteLine("  ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ, СПАДКУВАННЯ ТА ІНТЕРФЕЙСІВ");
        Console.WriteLine("====================================================================");
        Console.ResetColor();

        // 1. Створення об'єктів різних класів
        Student student = new("Тарас", "Шевченко", 19, 2, "КВ112233", "Канів", "МЕ112233");
        TaxiDriver driver = new("Василь", "Коваленко", 42, "ВХ998877", "Toyota Camry", 15);
        Acrobat acrobat = new("Олена", "Гончар", 23, "Cirque du Soleil", "Майстер спорту");

        // 2. Демонстрація спадкування через базовий клас Person
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Спадкування від базового класу Person (масив Person[]):");
        Console.ResetColor();

        Person[] people = new Person[] { student, driver, acrobat };
        for (int i = 0; i < people.Length; i++)
        {
            // Поліморфний виклик GetInfo()
            Console.WriteLine($"  [{i + 1}] {people[i].GetInfo()}");
        }

        // 3. Демонстрація поліморфізму через інтерфейс IDanceable
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n2. Поліморфізм через інтерфейс IDanceable (масив IDanceable[]):");
        Console.WriteLine("   (Клас TaxiDriver не реалізує IDanceable, тому сюди входять лише Student та Acrobat)");
        Console.ResetColor();

        IDanceable[] dancers = new IDanceable[] { student, acrobat };
        for (int i = 0; i < dancers.Length; i++)
        {
            // Поліморфний виклик Dance()
            Console.WriteLine($"  [{i + 1}] {dancers[i].Dance()}");
        }

        // 4. Демонстрація специфічних методів кожного класу
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n3. Специфічні методи відповідних класів:");
        Console.ResetColor();

        Console.WriteLine($"  - Student.Study(): {student.Study()}");
        Console.WriteLine($"  - TaxiDriver.Drive(): {driver.Drive()}");
        Console.WriteLine($"  - Acrobat.PerformAcrobatics(): {acrobat.PerformAcrobatics()}");
    }

    private void SaveToFile()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Збереження даних у файл (FileStream + StreamWriter) ===");
        Console.ResetColor();

        Console.Write($"Введіть шлях/ім'я файлу (Enter для '{DefaultFileName}'): ");
        string? input = Console.ReadLine()?.Trim();
        string fileName = string.IsNullOrWhiteSpace(input) ? DefaultFileName : input;

        try
        {
            CustomFileService.SaveStudents(fileName, _students);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Дані успішно збережено у файл '{fileName}'. Записано об'єктів: {_students.Length}");
            Console.ResetColor();

            // Показуємо фрагмент записаного файлу
            if (File.Exists(fileName))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n--- Прев'ю формату збереженого файлу (перші 12 рядків): ---");
                string[] lines = File.ReadAllLines(fileName);
                for (int i = 0; i < Math.Min(12, lines.Length); i++)
                {
                    Console.WriteLine(lines[i]);
                }
                Console.WriteLine("---------------------------------------------------------");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка при збереженні файлу: {ex.Message}");
            Console.ResetColor();
        }
    }

    private void LoadFromFile()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Завантаження даних із файлу (FileStream + StreamReader) ===");
        Console.ResetColor();

        Console.Write($"Введіть шлях/ім'я файлу (Enter для '{DefaultFileName}'): ");
        string? input = Console.ReadLine()?.Trim();
        string fileName = string.IsNullOrWhiteSpace(input) ? DefaultFileName : input;

        try
        {
            Student[] loaded = CustomFileService.LoadStudents(fileName);
            _students = loaded;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Дані успішно зчитано з файлу '{fileName}'. Завантажено студентів: {_students.Length}");
            Console.ResetColor();
        }
        catch (FileNotFoundException)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Файл '{fileName}' ще не створено. Спочатку збережіть дані через пункт 7.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка при завантаженні файлу: {ex.Message}");
            Console.ResetColor();
        }
    }

    private void ResetToMockData()
    {
        _students = GetInitialMockData();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Список студентів скинуто до початкових тестових даних ({_students.Length} осіб).");
        Console.ResetColor();
    }

    private bool IsStudentIdTaken(string studentId)
    {
        return ArrayHelper.Find(_students, s =>
            string.Equals(s.StudentId, studentId, StringComparison.OrdinalIgnoreCase)) != null;
    }

    private static string PromptValidatedInput(string prompt, Func<string, bool> validator, string errorMessage)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(input) && validator(input))
            {
                return input;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }
    }

    private static int PromptValidatedInt(string prompt, Func<int, bool> validator, string errorMessage)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out int value) && validator(value))
            {
                return value;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Генерує початковий тестовий набір студентів з різними курсами та містами
    /// для наочної перевірки розрахунку Варіанту 2.
    /// </summary>
    private static Student[] GetInitialMockData()
    {
        return new Student[]
        {
            new("Олександр", "Шевченко", 17, 1, "КВ101010", "Львів", "МЕ101010"),
            new("Марія", "Коваленко", 18, 1, "КВ202020", "Київ", "МЕ202020"),
            new("Дмитро", "Бондаренко", 17, 1, "КВ303030", "Одеса", "МЕ303030"),
            new("Анна", "Мельник", 18, 1, "КВ404040", "Харків", "МЕ404040"),
            new("Богдан", "Ткаченко", 19, 2, "КВ505050", "Полтава", "МЕ505050"),
            new("Софія", "Кравченко", 20, 3, "КВ606060", "Київ", "МЕ606060")
        };
    }
}
