using System.Text;
using Lab05.BLL.Exceptions;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Models;

namespace Lab05.PL;

public class ConsoleMenu
{
    private readonly IStudentService _studentService;
    private readonly ITaxiDriverService _taxiService;
    private readonly IAcrobatService _acrobatService;
    private readonly IDormitoryService _dormitoryService;
    private readonly IDriverLicenseService _licenseService;

    public ConsoleMenu(
        IStudentService studentService,
        ITaxiDriverService taxiService,
        IAcrobatService acrobatService,
        IDormitoryService dormitoryService,
        IDriverLicenseService licenseService)
    {
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _taxiService = taxiService ?? throw new ArgumentNullException(nameof(taxiService));
        _acrobatService = acrobatService ?? throw new ArgumentNullException(nameof(acrobatService));
        _dormitoryService = dormitoryService ?? throw new ArgumentNullException(nameof(dormitoryService));
        _licenseService = licenseService ?? throw new ArgumentNullException(nameof(licenseService));
    }

    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        SampleDataSeeder.SeedDemoData(_studentService, _taxiService, _acrobatService, _dormitoryService);

        bool running = true;
        while (running)
        {
            PrintHeader();
            PrintMenuItems();

            Console.Write("\nВаш вибір [0-12]: ");
            string? choice = Console.ReadLine()?.Trim();

            Console.Clear();
            try
            {
                switch (choice)
                {
                    case "1":
                        ShowAllStudents();
                        break;
                    case "2":
                        CalculateFirstCourseNonLocalPercentage();
                        break;
                    case "3":
                        ShowFirstCourseNonLocalStudents();
                        break;
                    case "4":
                        SettleStudentInDormitory();
                        break;
                    case "5":
                        ApplyForDriverLicense();
                        break;
                    case "6":
                        DemonstrateDancing();
                        break;
                    case "7":
                        ManageTaxiDrivers();
                        break;
                    case "8":
                        ManageAcrobats();
                        break;
                    case "9":
                        AddNewStudent();
                        break;
                    case "10":
                        SaveDataToFile();
                        break;
                    case "11":
                        LoadDataFromFile();
                        break;
                    case "12":
                        SampleDataSeeder.SeedDemoData(_studentService, _taxiService, _acrobatService, _dormitoryService);
                        PrintSuccess("Демонстраційні дані успішно переініціалізовано.");
                        break;
                    case "0":
                        running = false;
                        PrintInfo("Роботу застосунку завершено. До зустрічі!");
                        continue;
                    default:
                        PrintWarning("Невідомий пункт меню. Будь ласка, введіть число від 0 до 12.");
                        break;
                }
            }
            catch (DomainException ex)
            {
                PrintError($"Помилка бізнес-логіки: {ex.Message}");
            }
            catch (Exception ex)
            {
                PrintError($"Непередбачена помилка: {ex.Message}");
            }

            if (running)
            {
                Console.WriteLine("\nНатисніть Enter для повернення в головне меню...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    private static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            ЛАБОРАТОРНА РОБОТА №5 (МОДУЛЬ 3, ЛАБ 3.5) — ВАРІАНТ 2               ║");
        Console.WriteLine("║            ТЕМА: ДОСЛІДЖЕННЯ МОДУЛЬНОГО ТЕСТУВАННЯ ТА DEPENDENCY INJECTION     ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    private static void PrintMenuItems()
    {
        Console.WriteLine(" ОСНОВНІ ОПЕРАЦІЇ ТА ВАРІАНТ 2:");
        Console.WriteLine("  1.  👥 Список усіх студентів");
        Console.WriteLine("  2.  📊 [ВАРІАНТ 2] Обчислити відсоток студентів 1-го курсу з інших міст");
        Console.WriteLine("  3.  📋 [ВАРІАНТ 2] Список студентів 1-го курсу, які приїхали з інших міст");
        Console.WriteLine("  4.  🏠 [ВАРІАНТ 2] Поселення в гуртожиток при умові приїзду з іншого міста");
        Console.WriteLine("  5.  🚗 [ВАРІАНТ 2] Отримання водійського посвідчення при необхідних умовах");
        Console.WriteLine("  6.  💃 [ДОДАТКОВІ ВМІННЯ] Демонстрація здатності танцювати (інтерфейс IDancer)");
        Console.WriteLine();
        Console.WriteLine(" ДОДАТКОВІ СУТНОСТІ ТА КЕРУВАННЯ:");
        Console.WriteLine("  7.  🚕 Водії таксі (TaxiDriver) — перегляд, додавання, підвищення категорії");
        Console.WriteLine("  8.  🎪 Акробати (Acrobat) — перегляд, додавання, трюки, аудит безпеки");
        Console.WriteLine("  9.  ➕ Додати нового студента");
        Console.WriteLine(" 10.  💾 Зберегти дані у файл (JSON / Stream Custom / XML)");
        Console.WriteLine(" 11.  📂 Завантажити дані з файлу (JSON / Stream Custom / XML)");
        Console.WriteLine(" 12.  🧪 Перезавантажити демо-дані");
        Console.WriteLine("  0.  ❌ Вихід");
    }

    private void ShowAllStudents()
    {
        var students = _studentService.GetAll();
        PrintSectionTitle($"СПИСОК СТУДЕНТІВ (ВСЬОГО: {students.Count})");

        if (students.Count == 0)
        {
            PrintWarning("У системі немає зареєстрованих студентів.");
            return;
        }

        PrintStudentTable(students);
    }

    private void CalculateFirstCourseNonLocalPercentage()
    {
        PrintSectionTitle("ОБЧИСЛЕННЯ ВІДСОТКА СТУДЕНТІВ 1-ГО КУРСУ З ІНШИХ МІСТ");

        Console.Write("Введіть базове місцеве місто [Enter за замовчуванням 'Київ']: ");
        string? inputCity = Console.ReadLine()?.Trim();
        string localCity = string.IsNullOrWhiteSpace(inputCity) ? "Київ" : inputCity;

        var allStudents = _studentService.GetAll();
        var firstCourseAll = allStudents.Where(s => s.Course == 1).ToList();
        var firstCourseNonLocal = _studentService.GetFirstCourseNonLocalStudents(localCity);

        double percentage = _studentService.CalculateFirstCourseNonLocalPercentage(localCity);

        Console.WriteLine($"\n  Базове місцеве місто:           {localCity}");
        Console.WriteLine($"  Всього студентів 1-го курсу:    {firstCourseAll.Count}");
        Console.WriteLine($"  З них приїхали з інших міст:    {firstCourseNonLocal.Count}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ➡️ ВІДСОТОК НЕ МІСЦЕВИХ:         {percentage:F2}%\n");
        Console.ResetColor();

        if (firstCourseAll.Count == 0)
        {
            PrintInfo("Примітка: студентів 1-го курсу наразі немає, тому відсоток становить 0.00%.");
        }
    }

    private void ShowFirstCourseNonLocalStudents()
    {
        PrintSectionTitle("СТУДЕНТИ 1-ГО КУРСУ, ЯКІ ПРИЇХАЛИ З ІНШИХ МІСТ");

        Console.Write("Введіть базове місцеве місто [Enter за замовчуванням 'Київ']: ");
        string? inputCity = Console.ReadLine()?.Trim();
        string localCity = string.IsNullOrWhiteSpace(inputCity) ? "Київ" : inputCity;

        var nonLocal = _studentService.GetFirstCourseNonLocalStudents(localCity);
        if (nonLocal.Count == 0)
        {
            PrintWarning($"Не знайдено студентів 1-го курсу, які приїхали з інших міст (крім {localCity}).");
            return;
        }

        PrintStudentTable(nonLocal);
    }

    private void SettleStudentInDormitory()
    {
        PrintSectionTitle("ПОСЕЛЕННЯ В ГУРТОЖИТОК (УМОВА: ПРИЇЗД З ІНШОГО МІСТА)");

        Console.Write("Введіть номер студентського квитка (наприклад, KB102030): ");
        string? studentId = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(studentId))
        {
            PrintWarning("Номер студентського квитка обов'язковий.");
            return;
        }

        Console.Write("Введіть базове місцеве місто [Enter за замовчуванням 'Київ']: ");
        string? inputCity = Console.ReadLine()?.Trim();
        string localCity = string.IsNullOrWhiteSpace(inputCity) ? "Київ" : inputCity;

        if (!_dormitoryService.IsEligibleForDormitory(studentId, localCity, out string checkReason))
        {
            PrintWarning($"\nПеревірка права на гуртожиток: {checkReason}");
        }

        Console.Write("Введіть номер гуртожитку (1-50): ");
        if (!int.TryParse(Console.ReadLine(), out int dormNum))
        {
            PrintWarning("Некоректний номер гуртожитку.");
            return;
        }

        Console.Write("Введіть номер кімнати (1-999): ");
        if (!int.TryParse(Console.ReadLine(), out int roomNum))
        {
            PrintWarning("Некоректний номер кімнати.");
            return;
        }

        var result = _dormitoryService.SettleInDormitory(studentId, dormNum, roomNum, localCity);
        PrintSuccess($"\n{result.Message}");
    }

    private void ApplyForDriverLicense()
    {
        PrintSectionTitle("ОТРИМАННЯ ВОДІЙСЬКОГО ПОСВІДЧЕННЯ ПРИ НЕОБХІДНИХ УМОВАХ");
        Console.WriteLine("Умови: Вік >= 18 р., дійсна медична довідка, теорія >= 18/20, практика складена, відсутність заборон.\n");

        Console.Write("ПІБ кандидата: ");
        string? name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            PrintWarning("Ім'я не може бути порожнім.");
            return;
        }

        Console.Write("Вік (років): ");
        if (!int.TryParse(Console.ReadLine(), out int age))
        {
            PrintWarning("Некоректний вік.");
            return;
        }

        Console.Write("Наявність медичної довідки 083/о (т/н): ");
        bool hasMed = Console.ReadLine()?.Trim().ToLower().StartsWith("т") ?? false;

        Console.Write("Результат теоретичного іспиту (0-20 балів, прохідний >= 18): ");
        if (!int.TryParse(Console.ReadLine(), out int theoryScore))
        {
            PrintWarning("Некоректний бал.");
            return;
        }

        Console.Write("Практичний іспит складено (т/н): ");
        bool passedPractical = Console.ReadLine()?.Trim().ToLower().StartsWith("т") ?? false;

        Console.Write("Категорія (за замовчуванням 'B'): ");
        string? category = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(category)) category = "B";

        var app = new DriverLicenseApplication
        {
            CandidateName = name,
            Age = age,
            HasMedicalCertificate = hasMed,
            TheoryScore = theoryScore,
            PassedPracticalTest = passedPractical,
            RequestedCategory = category,
            HasCourtBan = false,
            PaidStateFee = true
        };

        var result = _licenseService.ApplyForLicense(app);
        PrintSuccess($"\n{result.Summary}");
    }

    private void DemonstrateDancing()
    {
        PrintSectionTitle("ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ: ДОДАТКОВЕ ВМІННЯ 'ТАНЦЮВАТИ' (IDancer)");
        Console.WriteLine("Інтерфейс IDancer реалізовано трьома різними сутностями системи:\n");

        List<IDancer> dancers = [];

        var student = _studentService.GetAll().FirstOrDefault();
        if (student is not null) dancers.Add(student);

        var taxi = _taxiService.GetAll().FirstOrDefault();
        if (taxi is not null) dancers.Add(taxi);

        var acrobat = _acrobatService.GetAll().FirstOrDefault();
        if (acrobat is not null) dancers.Add(acrobat);

        if (dancers.Count == 0)
        {
            PrintWarning("Не знайдено об'єктів для танцювальної демонстрації.");
            return;
        }

        string[] styles = ["Танго", "Вальс", "Брейк-данс", "Сальса", "Гопак"];
        int index = 0;
        foreach (var dancer in dancers)
        {
            string style = styles[index % styles.Length];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"🎭 [{dancer.GetType().Name}]:");
            Console.ResetColor();
            Console.WriteLine($"   {dancer.Dance(style)}\n");
            index++;
        }
    }

    private void ManageTaxiDrivers()
    {
        PrintSectionTitle("ВОДІЇ ТАКСІ (TaxiDriver)");
        var drivers = _taxiService.GetAll();

        Console.WriteLine($"Всього водіїв: {drivers.Count}\n");
        foreach (var d in drivers)
        {
            Console.WriteLine($" • {d}");
        }

        Console.WriteLine("\nОпції: [1] Додати нового водія | [2] Підвищити категорію посвідчення | [Enter] Назад");
        string? subChoice = Console.ReadLine()?.Trim();
        if (subChoice == "1")
        {
            Console.Write("Ім'я: ");
            string? fn = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Прізвище: ");
            string? ln = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Модель авто: ");
            string? car = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Номер посвідчення (3 літери + 6 цифр, напр. BTA123456): ");
            string? lic = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Вік: ");
            int.TryParse(Console.ReadLine(), out int age);
            Console.Write("Стаж водіння (років): ");
            int.TryParse(Console.ReadLine(), out int exp);

            _taxiService.Add(new TaxiDriverModel
            {
                FirstName = fn,
                LastName = ln,
                CarModel = car,
                LicenseNumber = lic,
                Age = age,
                ExperienceYears = exp,
                HasMedicalCertificate = true,
                HasActiveLicense = true
            });
            PrintSuccess("Водія таксі успішно додано!");
        }
        else if (subChoice == "2")
        {
            Console.Write("Введіть ID водія для підвищення категорії: ");
            if (int.TryParse(Console.ReadLine(), out int dId))
            {
                Console.Write("Нова категорія (напр. 'D' або 'C'): ");
                string newCat = Console.ReadLine()?.Trim() ?? "C";
                var res = _taxiService.UpgradeLicense(dId, newCat);
                PrintSuccess($"\n{res.Summary}");
            }
        }
    }

    private void ManageAcrobats()
    {
        PrintSectionTitle("АКРОБАТИ (Acrobat)");
        var acrobats = _acrobatService.GetAll();

        Console.WriteLine($"Всього акробатів: {acrobats.Count}\n");
        foreach (var a in acrobats)
        {
            Console.WriteLine($" • {a}");
        }

        Console.WriteLine("\nОпції: [1] Додати акробата | [2] Виконати трюк | [3] Провести аудит безпеки | [Enter] Назад");
        string? subChoice = Console.ReadLine()?.Trim();
        if (subChoice == "1")
        {
            Console.Write("Ім'я: ");
            string? fn = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Прізвище: ");
            string? ln = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Цирк / трупа: ");
            string? circus = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Спеціалізація: ");
            string? spec = Console.ReadLine()?.Trim() ?? string.Empty;

            _acrobatService.Add(new AcrobatModel
            {
                FirstName = fn,
                LastName = ln,
                CircusOrTroupe = circus,
                Specialization = spec,
                SafetyCertificationValid = true,
                PerformancesCount = 0
            });
            PrintSuccess("Акробата успішно додано!");
        }
        else if (subChoice == "2")
        {
            Console.Write("Введіть ID акробата: ");
            if (int.TryParse(Console.ReadLine(), out int aId))
            {
                var a = _acrobatService.GetById(aId);
                Console.Write("Назва трюку (напр. 'Потрійне сальто-мортале'): ");
                string stunt = Console.ReadLine()?.Trim() ?? "Сальто";
                string res = a.PerformStunt(stunt);
                _acrobatService.Update(a);
                PrintSuccess(res);
            }
        }
        else if (subChoice == "3")
        {
            Console.Write("Введіть ID акробата: ");
            if (int.TryParse(Console.ReadLine(), out int aId))
            {
                Console.Write("Аудит пройдено успішно? (т/н): ");
                bool pass = Console.ReadLine()?.Trim().ToLower().StartsWith("т") ?? false;
                string auditRes = _acrobatService.PerformSafetyAudit(aId, pass);
                PrintInfo(auditRes);
            }
        }
    }

    private void AddNewStudent()
    {
        PrintSectionTitle("ДОДАВАННЯ НОВОГО СТУДЕНТА");

        Console.Write("Ім'я: ");
        string fn = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Прізвище: ");
        string ln = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Курс (1-6): ");
        int.TryParse(Console.ReadLine(), out int course);

        Console.Write("Студентський квиток (напр. KB123456): ");
        string sId = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Місто прибуття (напр. Одеса або Київ): ");
        string city = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Паспорт (2 букви + 6 цифр або 9 цифр): ");
        string passport = Console.ReadLine()?.Trim() ?? string.Empty;

        var student = new StudentModel
        {
            FirstName = fn,
            LastName = ln,
            Course = course,
            StudentId = sId,
            OriginCity = city,
            Passport = passport
        };

        _studentService.Add(student);
        PrintSuccess($"Студента '{student.FullName}' успішно додано з ID={student.Id}!");
    }

    private void SaveDataToFile()
    {
        PrintSectionTitle("ЗБЕРЕЖЕННЯ ДАНИХ У ФАЙЛ");
        Console.WriteLine("Оберіть формат збереження:");
        Console.WriteLine(" 1. JSON (.json)");
        Console.WriteLine(" 2. Stream Custom Text (.txt)");
        Console.WriteLine(" 3. XML (.xml)");
        Console.Write("Ваш вибір [1-3]: ");

        string? fmtChoice = Console.ReadLine()?.Trim();
        string format = fmtChoice switch
        {
            "2" => "Stream",
            "3" => "XML",
            _ => "JSON"
        };

        string ext = fmtChoice switch
        {
            "2" => ".txt",
            "3" => ".xml",
            _ => ".json"
        };

        string defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "data", $"students{ext}");
        Console.Write($"Введіть шлях до файлу [Enter для '{defaultPath}']: ");
        string? inputPath = Console.ReadLine()?.Trim();
        string filePath = string.IsNullOrWhiteSpace(inputPath) ? defaultPath : inputPath;

        _studentService.SaveToFile(filePath, format);
        PrintSuccess($"Дані студентів успішно збережено у форматі {format}: {filePath}");
    }

    private void LoadDataFromFile()
    {
        PrintSectionTitle("ЗАВАНТАЖЕННЯ ДАНИХ З ФАЙЛУ");
        Console.WriteLine("Оберіть формат файлу:");
        Console.WriteLine(" 1. JSON (.json)");
        Console.WriteLine(" 2. Stream Custom Text (.txt)");
        Console.WriteLine(" 3. XML (.xml)");
        Console.Write("Ваш вибір [1-3]: ");

        string? fmtChoice = Console.ReadLine()?.Trim();
        string format = fmtChoice switch
        {
            "2" => "Stream",
            "3" => "XML",
            _ => "JSON"
        };

        string ext = fmtChoice switch
        {
            "2" => ".txt",
            "3" => ".xml",
            _ => ".json"
        };

        string defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "data", $"students{ext}");
        Console.Write($"Введіть шлях до файлу [Enter для '{defaultPath}']: ");
        string? inputPath = Console.ReadLine()?.Trim();
        string filePath = string.IsNullOrWhiteSpace(inputPath) ? defaultPath : inputPath;

        _studentService.LoadFromFile(filePath, format);
        PrintSuccess($"Дані успішно завантажено. Поточна кількість студентів: {_studentService.GetAll().Count}");
    }

    private static void PrintStudentTable(IReadOnlyList<StudentModel> students)
    {
        string header = $"| {"ID",-4} | {"ПІБ",-24} | {"Курс",-5} | {"Квиток",-10} | {"Місто",-12} | {"Паспорт",-12} | {"Гуртожиток",-28} |";
        string separator = new('-', header.Length);

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(separator);
        Console.WriteLine(header);
        Console.WriteLine(separator);
        Console.ResetColor();

        foreach (var s in students)
        {
            string dormInfo = s.IsSettledInDormitory
                ? $"Гуртожиток {s.DormitoryNumber}, кімн. {s.RoomNumber}"
                : "Ні";

            Console.WriteLine($"| {s.Id,-4} | {s.FullName,-24} | {s.Course,-5} | {s.StudentId,-10} | {s.OriginCity,-12} | {s.Passport,-12} | {dormInfo,-28} |");
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(separator);
        Console.ResetColor();
    }

    private static void PrintSectionTitle(string title)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n═══ {title} ═══\n");
        Console.ResetColor();
    }

    private static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[УСПІХ] {message}");
        Console.ResetColor();
    }

    private static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[ПОПЕРЕДЖЕННЯ] {message}");
        Console.ResetColor();
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ПОМИЛКА] {message}");
        Console.ResetColor();
    }

    private static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[ІНФО] {message}");
        Console.ResetColor();
    }
}
