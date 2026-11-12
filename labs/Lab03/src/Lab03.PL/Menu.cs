using System.Text;
using Lab03.BLL.Exceptions;
using Lab03.BLL.Interfaces;
using Lab03.BLL.Models;
using Lab03.BLL.Services;
using Lab03.DAL.Context;
using Lab03.DAL.Entities;
using Lab03.DAL.Providers;

namespace Lab03.PL;

public class Menu
{
    private string _currentStorageDirectory = Path.Combine(AppContext.BaseDirectory, "storage");
    private string _currentFileName = "students.json";
    private int _currentProviderIndex = 0;

    private readonly IDataProvider<StudentEntity[]>[] _studentProviders = new IDataProvider<StudentEntity[]>[]
    {
        new JsonDataProvider<StudentEntity[]>(),
        new XmlDataProvider<StudentEntity[]>(),
        new BinaryDataProvider<StudentEntity[]>(),
        new CustomDataProvider<StudentEntity[]>()
    };

    private readonly IDataProvider<TaxiDriverEntity[]>[] _taxiProviders = new IDataProvider<TaxiDriverEntity[]>[]
    {
        new JsonDataProvider<TaxiDriverEntity[]>(),
        new XmlDataProvider<TaxiDriverEntity[]>(),
        new BinaryDataProvider<TaxiDriverEntity[]>(),
        new CustomDataProvider<TaxiDriverEntity[]>()
    };

    private readonly IDataProvider<AcrobatEntity[]>[] _acrobatProviders = new IDataProvider<AcrobatEntity[]>[]
    {
        new JsonDataProvider<AcrobatEntity[]>(),
        new XmlDataProvider<AcrobatEntity[]>(),
        new BinaryDataProvider<AcrobatEntity[]>(),
        new CustomDataProvider<AcrobatEntity[]>()
    };

    private StudentService _studentService = null!;
    private TaxiDriverService _taxiDriverService = null!;
    private AcrobatService _acrobatService = null!;
    private readonly SerializationComparisonService _comparisonService = new();

    public Menu()
    {
        UpdateServices();
    }

    private void UpdateServices()
    {
        EntityContext<StudentEntity[]> studentContext = new(_studentProviders[_currentProviderIndex]);
        EntityContext<TaxiDriverEntity[]> taxiContext = new(_taxiProviders[_currentProviderIndex]);
        EntityContext<AcrobatEntity[]> acrobatContext = new(_acrobatProviders[_currentProviderIndex]);

        _studentService = new StudentService(studentContext);
        _taxiDriverService = new TaxiDriverService(taxiContext);
        _acrobatService = new AcrobatService(acrobatContext);
    }

    private string CurrentStudentFilePath => Path.Combine(_currentStorageDirectory, _currentFileName);
    private string CurrentTaxiFilePath => Path.Combine(_currentStorageDirectory, $"taxi_drivers{_taxiProviders[_currentProviderIndex].DefaultExtension}");
    private string CurrentAcrobatFilePath => Path.Combine(_currentStorageDirectory, $"acrobats{_acrobatProviders[_currentProviderIndex].DefaultExtension}");

    public void MainMenu()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        EnsureInitialMockData();

        bool exit = false;
        while (!exit)
        {
            PrintHeader();
            Console.WriteLine("1. Частина 1: Дослідження серіалізації (Book: масив vs колекція, 4 формати)");
            Console.WriteLine("2. Частина 2: Переглянути всіх студентів з файлу");
            Console.WriteLine("3. Частина 2: Додати нового студента");
            Console.WriteLine("4. Частина 2: Знайти студента за квитком або ID");
            Console.WriteLine("5. Частина 2: Видалити студента");
            Console.WriteLine("6. Частина 2: Індивідуальне завдання (відсоток студентів 1 курсу не з Києва)");
            Console.WriteLine("7. Частина 2: Робота з додатковими сутностями (TaxiDriver, Acrobat)");
            Console.WriteLine("8. Частина 2: Демонстрація поліморфізму та вміння IDancer (Танцювати)");
            Console.WriteLine("9. Налаштування сховища (вибір провайдера та імені файлу)");
            Console.WriteLine("0. Вихід");
            Console.Write("\nОберіть опцію: ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    RunPart1SerializationStudy();
                    break;
                case "2":
                    ShowAllStudents();
                    break;
                case "3":
                    AddNewStudent();
                    break;
                case "4":
                    FindStudent();
                    break;
                case "5":
                    RemoveStudent();
                    break;
                case "6":
                    RunIndividualTask();
                    break;
                case "7":
                    ManageAdditionalEntities();
                    break;
                case "8":
                    DemonstrateDancingPolymorphism();
                    break;
                case "9":
                    ConfigureStorageSettings();
                    break;
                case "0":
                    exit = true;
                    Console.WriteLine("Завершення роботи застосунку.");
                    break;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("\nНатисніть Enter для продовження...");
                Console.ReadLine();
            }
        }
    }

    private void PrintHeader()
    {
        Console.Clear();
        Console.WriteLine("================================================================================");
        Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №3 (МОДУЛЬ 3) — ВАРІАНТ 2");
        Console.WriteLine("  Трирівнева архітектура (DAL / BLL / PL) та механізми серіалізації C#");
        Console.WriteLine("================================================================================");
        Console.WriteLine($"[Поточний провайдер DAL]: {_studentProviders[_currentProviderIndex].FormatName}");
        Console.WriteLine($"[Файл сховища студентів]: {CurrentStudentFilePath}");
        Console.WriteLine("================================================================================\n");
    }

    private void RunPart1SerializationStudy()
    {
        Console.WriteLine("--- ЧАСТИНА 1: ДОСЛІДЖЕННЯ МЕХАНІЗМІВ СЕРІАЛІЗАЦІЇ (КЛАС BOOK) ---");
        Console.WriteLine("Тестування масиву Book[] (4 об'єкти) та списку List<Book> через 4 провайдери:");

        try
        {
            string testDir = Path.Combine(_currentStorageDirectory, "part1_experiments");
            SerializationExperimentResult[] results = _comparisonService.RunFullComparison(testDir);

            Console.WriteLine("\n+-----------------------+--------------------+------------+----------+-----------+------------+");
            Console.WriteLine("| Провайдер             | Тип контейнера     | Розмір (Б) | Сер-я(мс)| Десер(мс) | Цілісність |");
            Console.WriteLine("+-----------------------+--------------------+------------+----------+-----------+------------+");

            foreach (SerializationExperimentResult r in results)
            {
                string status = r.DataIntegrityVerified ? "OK (100%)" : "FAIL";
                Console.WriteLine($"| {r.FormatName,-21} | {r.TargetType,-18} | {r.FileSizeBytes,10} | {r.SerializationTimeMs,8:F3} | {r.DeserializationTimeMs,9:F3} | {status,-10} |");
            }
            Console.WriteLine("+-----------------------+--------------------+------------+----------+-----------+------------+");

            Console.WriteLine("\nВисновки з дослідження:");
            Console.WriteLine("1. Binary: найбільш компактний та швидкий формат.");
            Console.WriteLine("2. JSON: баланс людиночитабельності, швидкості та стандартизованості.");
            Console.WriteLine("3. XML: надлишковий через теги, але строго типізований.");
            Console.WriteLine("4. Custom: демонстрація власного токенізованого формату збереження.");
        }
        catch (Exception ex)
        {
            PrintError("Помилка виконання експерименту серіалізації", ex);
        }
    }

    private void ShowAllStudents()
    {
        Console.WriteLine("--- СПИСОК СТУДЕНТІВ ІЗ СХОВИЩА ---");
        try
        {
            StudentModel[] students = _studentService.GetAll(CurrentStudentFilePath);
            PrintStudentTable(students);
        }
        catch (Exception ex)
        {
            PrintError("Помилка завантаження студентів", ex);
        }
    }

    private void AddNewStudent()
    {
        Console.WriteLine("--- ДОДАВАННЯ НОВОГО СТУДЕНТА ---");
        try
        {
            Console.Write("ID студента (наприклад, 10): ");
            string id = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Ім'я (FirstName): ");
            string firstName = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Прізвище (LastName): ");
            string lastName = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Курс (1-6): ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int course))
            {
                Console.WriteLine("Помилка: номер курсу повинен бути цілим числом.");
                return;
            }

            Console.Write("Студентський квиток (наприклад, KB123456): ");
            string studentId = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Місто прибуття (OriginCity, наприклад Львів, Одеса, Київ): ");
            string originCity = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Паспорт (серія та номер або 9 цифр ID-картки): ");
            string passport = Console.ReadLine()?.Trim() ?? "";

            StudentModel newStudent = new(id, firstName, lastName, course, studentId, originCity, passport);
            _studentService.Add(CurrentStudentFilePath, newStudent);

            Console.WriteLine($"\nСтудента '{newStudent.FullName}' успішно збережено у сховище ({_studentProviders[_currentProviderIndex].FormatName}).");
        }
        catch (DomainException ex)
        {
            PrintDomainError(ex);
        }
        catch (Exception ex)
        {
            PrintError("Непередбачена помилка", ex);
        }
    }

    private void FindStudent()
    {
        Console.WriteLine("--- ПОШУК СТУДЕНТА ---");
        Console.Write("Введіть студентський квиток або ID: ");
        string? query = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("Пошуковий запит не може бути порожнім.");
            return;
        }

        try
        {
            StudentModel? student = _studentService.FindById(CurrentStudentFilePath, query);
            if (student == null)
            {
                Console.WriteLine($"Студента за запитом '{query}' не знайдено.");
            }
            else
            {
                Console.WriteLine("\nЗнайдено студента:");
                PrintStudentTable(new[] { student });
            }
        }
        catch (Exception ex)
        {
            PrintError("Помилка пошуку", ex);
        }
    }

    private void RemoveStudent()
    {
        Console.WriteLine("--- ВИДАЛЕННЯ СТУДЕНТА ---");
        Console.Write("Введіть студентський квиток або ID для видалення: ");
        string? query = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("Запит не може бути порожнім.");
            return;
        }

        try
        {
            StudentModel removed = _studentService.Remove(CurrentStudentFilePath, query);
            Console.WriteLine($"Студента '{removed.FullName}' (квиток: {removed.StudentId}) успішно видалено.");
        }
        catch (DomainException ex)
        {
            PrintDomainError(ex);
        }
        catch (Exception ex)
        {
            PrintError("Помилка видалення", ex);
        }
    }

    private void RunIndividualTask()
    {
        Console.WriteLine("--- ІНДИВІДУАЛЬНЕ ЗАВДАННЯ ВАРІАНТА 2 ---");
        Console.WriteLine("Постановка: «Обчислити відсоток студентів 1-го курсу, які приїхали з інших міст. Отримати їх дані з файлу.»");
        Console.WriteLine("Базове місто: Київ\n");

        try
        {
            StudentModel[] allStudents = _studentService.GetAll(CurrentStudentFilePath);
            double percentage = _studentService.CalculateFirstCourseNonLocalPercentage(allStudents, "Київ");
            StudentModel[] nonLocal = _studentService.GetFirstCourseNonLocalStudents(allStudents, "Київ");

            Console.WriteLine($"Розрахований відсоток приїжджих 1-курсників: {percentage:F2}%\n");
            Console.WriteLine("Список студентів 1-го курсу, що приїхали з інших міст:");
            PrintStudentTable(nonLocal);
        }
        catch (Exception ex)
        {
            PrintError("Помилка виконання індивідуального завдання", ex);
        }
    }

    private void ManageAdditionalEntities()
    {
        Console.WriteLine("--- ДОДАТКОВІ СУТНОСТІ ВАРІАНТА 2 (TAXIDRIVER, ACROBAT) ---");
        Console.WriteLine("1. Переглянути водіїв таксі (TaxiDriver)");
        Console.WriteLine("2. Переглянути акробатів (Acrobat)");
        Console.WriteLine("3. Додати тестових водіїв таксі та акробатів");
        Console.Write("Ваш вибір: ");

        string? subChoice = Console.ReadLine()?.Trim();
        Console.WriteLine();

        try
        {
            switch (subChoice)
            {
                case "1":
                    TaxiDriverModel[] drivers = _taxiDriverService.GetAll(CurrentTaxiFilePath);
                    PrintTaxiTable(drivers);
                    break;
                case "2":
                    AcrobatModel[] acrobats = _acrobatService.GetAll(CurrentAcrobatFilePath);
                    PrintAcrobatTable(acrobats);
                    break;
                case "3":
                    SeedAdditionalEntities();
                    Console.WriteLine("Тестові водії таксі та акробати додані у сховище.");
                    break;
                default:
                    Console.WriteLine("Невірний пункт.");
                    break;
            }
        }
        catch (Exception ex)
        {
            PrintError("Помилка роботи з додатковими сутностями", ex);
        }
    }

    private void DemonstrateDancingPolymorphism()
    {
        Console.WriteLine("--- ПОЛІМОРФІЗМ ТА ДОДАТКОВЕ ВМІННЯ (IDANCER: ТАНЦЮВАТИ) ---");
        Console.WriteLine("Виклик методу Dance() через спільний інтерфейс IDancer для всіх сутностей варіанта 2:\n");

        try
        {
            StudentModel[] students = _studentService.GetAll(CurrentStudentFilePath);
            TaxiDriverModel[] drivers = _taxiDriverService.GetAll(CurrentTaxiFilePath);
            AcrobatModel[] acrobats = _acrobatService.GetAll(CurrentAcrobatFilePath);

            List<IDancer> dancers = new();
            foreach (StudentModel s in students) dancers.Add(s);
            foreach (TaxiDriverModel d in drivers) dancers.Add(d);
            foreach (AcrobatModel a in acrobats) dancers.Add(a);

            if (dancers.Count == 0)
            {
                Console.WriteLine("Сховище порожнє. Додайте сутності для демонстрації.");
                return;
            }

            string[] styles = { "Вальс", "Танго", "Брейк-данс", "Хіп-хоп", "Сальса", "Фокстрот" };
            int i = 0;
            foreach (IDancer dancer in dancers)
            {
                string style = styles[i % styles.Length];
                Console.WriteLine($"💃 {dancer.Dance(style)}");
                i++;
            }
        }
        catch (Exception ex)
        {
            PrintError("Помилка демонстрації поліморфізму", ex);
        }
    }

    private void ConfigureStorageSettings()
    {
        Console.WriteLine("--- НАЛАШТУВАННЯ СХОВИЩА ТА СЕРІАЛІЗАЦІЇ ---");
        Console.WriteLine($"Поточний провайдер: {_studentProviders[_currentProviderIndex].FormatName}");
        Console.WriteLine($"Поточний файл студентів: {_currentFileName}");
        Console.WriteLine($"Каталог сховища: {_currentStorageDirectory}\n");

        Console.WriteLine("1. Змінити провайдер серіалізації (JSON / XML / Binary / Custom)");
        Console.WriteLine("2. Змінити ім'я файлу студентів");
        Console.WriteLine("3. Змінити каталог сховища");
        Console.Write("Ваш вибір: ");

        string? sub = Console.ReadLine()?.Trim();
        switch (sub)
        {
            case "1":
                Console.WriteLine("\nОберіть провайдер:");
                for (int i = 0; i < _studentProviders.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {_studentProviders[i].FormatName} ({_studentProviders[i].DefaultExtension})");
                }
                Console.Write("Номер: ");
                if (int.TryParse(Console.ReadLine()?.Trim(), out int idx) && idx >= 1 && idx <= _studentProviders.Length)
                {
                    _currentProviderIndex = idx - 1;
                    string ext = _studentProviders[_currentProviderIndex].DefaultExtension;
                    _currentFileName = Path.ChangeExtension(_currentFileName, ext);
                    UpdateServices();
                    EnsureInitialMockData();
                    Console.WriteLine($"Провайдер змінено на: {_studentProviders[_currentProviderIndex].FormatName}. Ім'я файлу: {_currentFileName}");
                }
                break;
            case "2":
                Console.Write("Введіть нове ім'я файлу: ");
                string? newName = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    _currentFileName = newName;
                    EnsureInitialMockData();
                    Console.WriteLine($"Ім'я файлу оновлено: {_currentFileName}");
                }
                break;
            case "3":
                Console.Write("Введіть новий шлях до каталогу: ");
                string? newPath = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    _currentStorageDirectory = newPath;
                    EnsureInitialMockData();
                    Console.WriteLine($"Каталог оновлено: {_currentStorageDirectory}");
                }
                break;
        }
    }

    private void PrintStudentTable(StudentModel[] students)
    {
        if (students.Length == 0)
        {
            Console.WriteLine("Немає записів для відображення.");
            return;
        }

        Console.WriteLine("+----+----------------------+------+-------------+-----------------+-------------+");
        Console.WriteLine("| ID | ПІБ                  | Курс | Квиток      | Місто           | Паспорт     |");
        Console.WriteLine("+----+----------------------+------+-------------+-----------------+-------------+");

        foreach (StudentModel s in students)
        {
            Console.WriteLine($"| {s.Id,-2} | {s.FullName,-20} | {s.Course,4} | {s.StudentId,-11} | {s.OriginCity,-15} | {s.Passport,-11} |");
        }
        Console.WriteLine("+----+----------------------+------+-------------+-----------------+-------------+");
        Console.WriteLine($"Всього студентів: {students.Length}");
    }

    private void PrintTaxiTable(TaxiDriverModel[] drivers)
    {
        if (drivers.Length == 0)
        {
            Console.WriteLine("Немає записів водіїв таксі.");
            return;
        }

        Console.WriteLine("+----+----------------------+--------------------+-------------+-------+");
        Console.WriteLine("| ID | ПІБ                  | Автомобіль         | Посвідчення | Стаж  |");
        Console.WriteLine("+----+----------------------+--------------------+-------------+-------+");
        foreach (TaxiDriverModel d in drivers)
        {
            Console.WriteLine($"| {d.Id,-2} | {d.FullName,-20} | {d.CarModel,-18} | {d.LicenseNumber,-11} | {d.DrivingExperienceYears,3} р.|");
        }
        Console.WriteLine("+----+----------------------+--------------------+-------------+-------+");
    }

    private void PrintAcrobatTable(AcrobatModel[] acrobats)
    {
        if (acrobats.Length == 0)
        {
            Console.WriteLine("Немає записів акробатів.");
            return;
        }

        Console.WriteLine("+----+----------------------+-------------------------+-------------------------+");
        Console.WriteLine("| ID | ПІБ                  | Цирк / Трупа            | Спеціалізація           |");
        Console.WriteLine("+----+----------------------+-------------------------+-------------------------+");
        foreach (AcrobatModel a in acrobats)
        {
            Console.WriteLine($"| {a.Id,-2} | {a.FullName,-20} | {a.CircusOrTroupeName,-23} | {a.Specialization,-23} |");
        }
        Console.WriteLine("+----+----------------------+-------------------------+-------------------------+");
    }

    private void EnsureInitialMockData()
    {
        if (!_studentService.GetAll(CurrentStudentFilePath).Any())
        {
            StudentModel[] initial = new StudentModel[]
            {
                new("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456"),
                new("2", "Леся", "Українка", 1, "KB223344", "Новоград", "МЕ654321"),
                new("3", "Іван", "Франко", 1, "KB334455", "Київ", "АН778899"),
                new("4", "Михайло", "Грушевський", 2, "KB445566", "Холм", "ВВ332211"),
                new("5", "Григорій", "Сковорода", 1, "KB556677", "Полтава", "123456789"),
                new("6", "Богдан", "Хмельницький", 3, "KB667788", "Київ", "СН998877")
            };
            _studentService.SaveAll(CurrentStudentFilePath, initial);
        }

        if (!_taxiDriverService.GetAll(CurrentTaxiFilePath).Any())
        {
            TaxiDriverModel[] drivers = new TaxiDriverModel[]
            {
                new("T1", "Василь", "Шофер", "Toyota Camry", "ВХА123456", 12),
                new("T2", "Петро", "Керманич", "Skoda Octavia", "ААВ987654", 6)
            };
            _taxiDriverService.SaveAll(CurrentTaxiFilePath, drivers);
        }

        if (!_acrobatService.GetAll(CurrentAcrobatFilePath).Any())
        {
            AcrobatModel[] acrobats = new AcrobatModel[]
            {
                new("A1", "Анна", "Повітряна", "Національний Цирк", "Повітряне кільце"),
                new("A2", "Максим", "Сальто", "Cirque du Soleil", "Батутна акробатика")
            };
            _acrobatService.SaveAll(CurrentAcrobatFilePath, acrobats);
        }
    }

    private void SeedAdditionalEntities()
    {
        TaxiDriverModel[] drivers = new TaxiDriverModel[]
        {
            new("T1", "Василь", "Шофер", "Toyota Camry", "ВХА123456", 12),
            new("T2", "Петро", "Керманич", "Skoda Octavia", "ААВ987654", 6),
            new("T3", "Олег", "Драйвер", "Volkswagen Passat", "АІК456789", 15)
        };
        _taxiDriverService.SaveAll(CurrentTaxiFilePath, drivers);

        AcrobatModel[] acrobats = new AcrobatModel[]
        {
            new("A1", "Анна", "Повітряна", "Національний Цирк", "Повітряне кільце"),
            new("A2", "Максим", "Сальто", "Cirque du Soleil", "Батутна акробатика"),
            new("A3", "Олена", "Гнучка", "Київський Театр Трюків", "Каучук і жонглювання")
        };
        _acrobatService.SaveAll(CurrentAcrobatFilePath, acrobats);
    }

    private static void PrintDomainError(DomainException ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[Бізнес-помилка]: {ex.Message}");
        Console.ResetColor();
    }

    private static void PrintError(string context, Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Помилка ({context})]: {ex.Message}");
        Console.ResetColor();
    }
}
