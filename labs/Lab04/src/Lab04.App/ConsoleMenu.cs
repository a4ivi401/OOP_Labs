using System;
using System.Text;
using Lab04.Core.Analysis;
using Lab04.Core.Delegates;
using Lab04.Core.Handlers;
using Lab04.Core.Math;

namespace Lab04.App;

/// <summary>
/// Інтерактивне консольне меню для демонстрації можливостей Лабораторної роботи 3.4 (Варіант 2).
/// Демонструє:
/// 1. Роботу з делегатами (іменований метод, анонімний метод, лямбда-вираз, FCL Func).
/// 2. Компонент математичних операцій MathCalculator.
/// 3. Подію цілочисельного ділення IntegerDivision та класи-підписники.
/// 4. Підписку (+-) та відписку (-=) у мультикаст-делегатах.
/// </summary>
public class ConsoleMenu
{
    private readonly MathCalculator _calculator;
    private readonly ConsoleNotificationHandler _notificationHandler;
    private readonly CalculationAuditLogger _auditLogger;
    private readonly DivisionStatisticsTracker _statsTracker;

    private bool _isNotificationSubscribed = true;
    private bool _isAuditSubscribed = true;
    private bool _isStatsSubscribed = true;

    public ConsoleMenu()
    {
        _calculator = new MathCalculator("EnterpriseMathCore");
        _notificationHandler = new ConsoleNotificationHandler("ConsoleNotifier", Console.WriteLine);
        _auditLogger = new CalculationAuditLogger("CentralAuditLogger");
        _statsTracker = new DivisionStatisticsTracker("MetricsTracker");

        // Початкова підписка всіх обробників (мультикаст-делегат)
        _notificationHandler.Subscribe(_calculator);
        _auditLogger.Subscribe(_calculator);
        _statsTracker.Subscribe(_calculator);
    }

    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        bool running = true;
        while (running)
        {
            PrintHeader();
            Console.WriteLine("1. 🔍 Частина 1: Пошук кількості входжень символу (Делегати, Анонімні методи, Лямбди)");
            Console.WriteLine("2. ➕ Частина 2: Стандартні математичні операції (Додавання, Віднімання, Множення, тощо)");
            Console.WriteLine("3. ➗ Частина 2 & 3: Цілочисельне ділення (Генерація події IntegerDivision)");
            Console.WriteLine("4. 🔔 Керування підписниками події (+- / -= мультикаст-делегат)");
            Console.WriteLine("5. 📜 Перегляд журналу аудиту операцій ділення (CalculationAuditLogger)");
            Console.WriteLine("6. 📊 Перегляд статистики та аналітики ділення (DivisionStatisticsTracker)");
            Console.WriteLine("7. 🚀 Автоматичний демонстраційний сценарій (Showcase)");
            Console.WriteLine("0. 🚪 Вихід із програми");
            Console.WriteLine("===================================================================================");
            Console.Write("Оберіть пункт меню (0-7): ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        RunCharacterCountDemo();
                        break;
                    case "2":
                        RunStandardMathOperations();
                        break;
                    case "3":
                        RunIntegerDivisionWithEvent();
                        break;
                    case "4":
                        ManageSubscribers();
                        break;
                    case "5":
                        DisplayAuditLogs();
                        break;
                    case "6":
                        DisplayStatistics();
                        break;
                    case "7":
                        RunAutomatedShowcase();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Дякуємо за роботу з програмою! До побачення.");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Невірний вибір. Будь ласка, введіть число від 0 до 7.");
                        Console.ResetColor();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ПОМИЛКА]: {ex.Message}");
                Console.ResetColor();
            }

            if (running)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Натисніть Enter для продовження...");
                Console.ResetColor();
                Console.ReadLine();
            }
        }
    }

    private static void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("===================================================================================");
        Console.WriteLine("  ЛАБОРАТОРНА РОБОТА 3.4 (Варіант 2) | C# .NET 8");
        Console.WriteLine("  Тема: Створення та використання делегатів, подій та лямбда-виразів");
        Console.WriteLine("  Студент: Варіант 2 | Делегати пошуку символу & Подія цілочисельного ділення");
        Console.WriteLine("===================================================================================");
        Console.ResetColor();
    }

    private void RunCharacterCountDemo()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- ЧАСТИНА 1: ПОШУК СИМВОЛУ ЧЕРЕЗ ДЕЛЕГАТИ ---");
        Console.ResetColor();
        Console.WriteLine("1. Ввести власний рядок та символ");
        Console.WriteLine("2. Використати демонстраційний приклад за замовчуванням");
        Console.Write("Ваш вибір (1-2): ");

        string? subChoice = Console.ReadLine()?.Trim();
        string text;
        char target;
        bool ignoreCase = false;

        if (subChoice == "1")
        {
            Console.Write("Введіть рядок для аналізу: ");
            text = Console.ReadLine() ?? string.Empty;

            Console.Write("Введіть шуканий символ: ");
            string? charInput = Console.ReadLine();
            if (string.IsNullOrEmpty(charInput))
            {
                Console.WriteLine("Помилка: символ не введено.");
                return;
            }
            target = charInput[0];

            Console.Write("Ігнорувати регістр? (y/n, за замовчуванням n): ");
            string? ignoreInput = Console.ReadLine()?.Trim().ToLowerInvariant();
            ignoreCase = ignoreInput is "y" or "yes" or "так" or "т";
        }
        else
        {
            text = "Делегати, анонімні методи та лямбда-вирази у мові програмування C# 8.0";
            target = 'а';
            ignoreCase = true;
            Console.WriteLine($"\n[ДЕМО-РЯДОК]: \"{text}\"");
            Console.WriteLine($"[ДЕМО-СИМВОЛ]: '{target}' (ігнорування регістру: {ignoreCase})");
        }

        Console.WriteLine("\nОбчислення результатів через 4 механізми виклику:");

        // 1. Іменований метод
        CharacterCountWithOptionsDelegate namedDel = CharacterCounter.CountOccurrencesNamedWithOptions;
        int namedCount = namedDel(text, target, ignoreCase);

        // 2. Анонімний метод
        CharacterCountWithOptionsDelegate anonDel = CharacterCounter.CreateAnonymousMethodWithOptions();
        int anonCount = anonDel(text, target, ignoreCase);

        // 3. Лямбда-вираз
        CharacterCountWithOptionsDelegate lambdaDel = CharacterCounter.CreateLambdaExpressionWithOptions();
        int lambdaCount = lambdaDel(text, target, ignoreCase);

        // 4. FCL Func
        Func<string?, char, bool, int> funcDel = CharacterCounter.CreateFuncWithOptionsDelegate();
        int funcCount = funcDel(text, target, ignoreCase);

        StringAnalysisResult comparison = CharacterCounter.CompareAllApproaches(text, target, ignoreCase);

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"┌────────────────────────────────────────────────────────┬──────────┐");
        Console.WriteLine($"│ Механізм виклику через делегат                         │ Кількість│");
        Console.WriteLine($"├────────────────────────────────────────────────────────┼──────────┤");
        Console.WriteLine($"│ 1. Іменований метод (Named Method)                     │ {namedCount,8} │");
        Console.WriteLine($"│ 2. Анонімний метод (Anonymous Method, delegate {{ ... }}) │ {anonCount,8} │");
        Console.WriteLine($"│ 3. Лямбда-вираз (Lambda Expression, (...) => ...)       │ {lambdaCount,8} │");
        Console.WriteLine($"│ 4. Системний generic делегат Func<...>                 │ {funcCount,8} │");
        Console.WriteLine($"└────────────────────────────────────────────────────────┴──────────┘");
        Console.ResetColor();

        if (comparison.AllResultsMatch)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Успіх: Усі 4 механізми повернули ідентичний результат!");
            Console.ResetColor();
        }
    }

    private void RunStandardMathOperations()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- ЧАСТИНА 2: СТАНДАРТНІ МАТЕМАТИЧНІ ОПЕРАЦІЇ ---");
        Console.ResetColor();

        Console.WriteLine("1. Додавання (a + b)");
        Console.WriteLine("2. Віднімання (a - b)");
        Console.WriteLine("3. Множення (a * b)");
        Console.WriteLine("4. Дійсне ділення з плаваючою комою (double: a / b)");
        Console.WriteLine("5. Остача від ділення / Modulo (a % b)");
        Console.WriteLine("6. Піднесення до степеня (a ^ b)");
        Console.WriteLine("7. Факторіал числа (n!)");
        Console.WriteLine("8. Модуль числа (Abs(x))");
        Console.Write("Оберіть операцію (1-8): ");

        string? op = Console.ReadLine()?.Trim();
        switch (op)
        {
            case "1":
                long a1 = ReadLong("Введіть перше число a: ");
                long b1 = ReadLong("Введіть друге число b: ");
                Console.WriteLine($"Результат: {a1} + {b1} = {_calculator.Add(a1, b1)}");
                break;
            case "2":
                long a2 = ReadLong("Введіть перше число a: ");
                long b2 = ReadLong("Введіть друге число b: ");
                Console.WriteLine($"Результат: {a2} - {b2} = {_calculator.Subtract(a2, b2)}");
                break;
            case "3":
                long a3 = ReadLong("Введіть перше число a: ");
                long b3 = ReadLong("Введіть друге число b: ");
                Console.WriteLine($"Результат: {a3} * {b3} = {_calculator.Multiply(a3, b3)}");
                break;
            case "4":
                double da = ReadDouble("Введіть ділене (double): ");
                double db = ReadDouble("Введіть дільник (double): ");
                Console.WriteLine($"Результат: {da} / {db} = {_calculator.Divide(da, db):F4}");
                break;
            case "5":
                long ma = ReadLong("Введіть число a: ");
                long mb = ReadLong("Введіть модуль b: ");
                Console.WriteLine($"Результат: {ma} % {mb} = {_calculator.Modulo(ma, mb)}");
                break;
            case "6":
                double baseVal = ReadDouble("Введіть основу: ");
                double expVal = ReadDouble("Введіть показник степеня: ");
                Console.WriteLine($"Результат: {baseVal} ^ {expVal} = {_calculator.Power(baseVal, expVal):F4}");
                break;
            case "7":
                int n = (int)ReadLong("Введіть невід'ємне ціле число n (0..20): ");
                Console.WriteLine($"Результат: {n}! = {_calculator.Factorial(n)}");
                break;
            case "8":
                long absVal = ReadLong("Введіть число: ");
                Console.WriteLine($"Результат: |{absVal}| = {_calculator.Abs(absVal)}");
                break;
            default:
                Console.WriteLine("Невідома операція.");
                break;
        }
    }

    private void RunIntegerDivisionWithEvent()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- ЧАСТИНА 2 & 3: ЦІЛОЧИСЕЛЬНЕ ДІЛЕННЯ ТА ГЕНЕРАЦІЯ ПОДІЇ ---");
        Console.ResetColor();

        Console.WriteLine("При виконанні цієї операції калькулятор викликає подію IntegerDivision.");
        Console.WriteLine("Усі активні підписники (мультикаст-делегат) автоматично отримають сповіщення!\n");

        long dividend = ReadLong("Введіть ділене (чисельник): ");
        long divisor = ReadLong("Введіть дільник (знаменник): ");

        Console.Write("Введіть примітку/контекст операції (наприклад, 'Сесія #1'): ");
        string note = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.WriteLine("\n[ВИКОНАННЯ ОПЕРАЦІЇ]: Calculator.IntegerDivideWithRemainder(...)");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("--- Початок ланцюжка виклику підписників події ---");
        Console.ResetColor();

        var (quotient, remainder) = _calculator.IntegerDivideWithRemainder(dividend, divisor, note);

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("--- Кінець ланцюжка виклику підписників події ---\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Прямий результат виклику методу:");
        Console.WriteLine($"  Частка (Quotient):  {quotient}");
        Console.WriteLine($"  Остача (Remainder): {remainder}");
        Console.WriteLine($"  Ділення націло:     {(remainder == 0 ? "ТАК" : "НІ")}");
        Console.ResetColor();
    }

    private void ManageSubscribers()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- КЕРУВАННЯ ПІДПИСНИКАМИ (МУЛЬТИКАСТ-ДЕЛЕГАТ) ---");
        Console.ResetColor();

        Console.WriteLine($"1. ConsoleNotificationHandler: [{( _isNotificationSubscribed ? "ПІДПИСАНО (+)" : "ВІДПИСАНО (-)" )}] (Перемкнути)");
        Console.WriteLine($"2. CalculationAuditLogger:     [{( _isAuditSubscribed ? "ПІДПИСАНО (+)" : "ВІДПИСАНО (-)" )}] (Перемкнути)");
        Console.WriteLine($"3. DivisionStatisticsTracker:  [{( _isStatsSubscribed ? "ПІДПИСАНО (+)" : "ВІДПИСАНО (-)" )}] (Перемкнути)");
        Console.Write("Оберіть номер обробника для зміни стану підписки (1-3): ");

        string? choice = Console.ReadLine()?.Trim();
        switch (choice)
        {
            case "1":
                if (_isNotificationSubscribed)
                {
                    _notificationHandler.Unsubscribe(_calculator);
                    _isNotificationSubscribed = false;
                    Console.WriteLine("Обробник ConsoleNotificationHandler відписано від події (-=).");
                }
                else
                {
                    _notificationHandler.Subscribe(_calculator);
                    _isNotificationSubscribed = true;
                    Console.WriteLine("Обробник ConsoleNotificationHandler підписано на подію (+=).");
                }
                break;
            case "2":
                if (_isAuditSubscribed)
                {
                    _auditLogger.Unsubscribe(_calculator);
                    _isAuditSubscribed = false;
                    Console.WriteLine("Обробник CalculationAuditLogger відписано від події (-=).");
                }
                else
                {
                    _auditLogger.Subscribe(_calculator);
                    _isAuditSubscribed = true;
                    Console.WriteLine("Обробник CalculationAuditLogger підписано на подію (+=).");
                }
                break;
            case "3":
                if (_isStatsSubscribed)
                {
                    _statsTracker.Unsubscribe(_calculator);
                    _isStatsSubscribed = false;
                    Console.WriteLine("Обробник DivisionStatisticsTracker відписано від події (-=).");
                }
                else
                {
                    _statsTracker.Subscribe(_calculator);
                    _isStatsSubscribed = true;
                    Console.WriteLine("Обробник DivisionStatisticsTracker підписано на подію (+=).");
                }
                break;
            default:
                Console.WriteLine("Невірний вибір.");
                break;
        }
    }

    private void DisplayAuditLogs()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- ЖУРНАЛ АУДИТУ (CalculationAuditLogger) ---");
        Console.ResetColor();

        Console.WriteLine($"Загальна кількість записів у журналі: {_auditLogger.TotalLoggedEntries}");
        if (_auditLogger.TotalLoggedEntries == 0)
        {
            Console.WriteLine("Журнал порожній. Виконайте операції цілочисельного ділення для наповнення.");
            return;
        }

        Console.WriteLine("1. Показати всі записи");
        Console.WriteLine("2. Показати лише ділення націло");
        Console.WriteLine("3. Показати лише ділення з остачею");
        Console.WriteLine("4. Очистити журнал");
        Console.Write("Ваш вибір (1-4): ");

        string? opt = Console.ReadLine()?.Trim();
        var logs = opt switch
        {
            "2" => _auditLogger.GetExactDivisions(),
            "3" => _auditLogger.GetDivisionsWithRemainder(),
            "4" => ClearAuditLog(),
            _ => _auditLogger.Logs
        };

        if (opt == "4")
        {
            return;
        }

        Console.WriteLine($"\nЗнайдено записів: {logs.Count}");
        foreach (var entry in logs)
        {
            string exactMark = entry.IsExact ? "✔ Націло" : $" Остача {entry.Remainder}";
            Console.WriteLine($"[{entry.TimestampUtc:HH:mm:ss}] {entry.SenderName} | {entry.Dividend} / {entry.Divisor} = {entry.Quotient} ({exactMark}) | Контекст: '{entry.Note}' | ID: {entry.OperationId.ToString()[..8]}");
        }
    }

    private IReadOnlyList<AuditLogEntry> ClearAuditLog()
    {
        _auditLogger.Clear();
        Console.WriteLine("Журнал аудиту успішно очищено.");
        return Array.Empty<AuditLogEntry>();
    }

    private void DisplayStatistics()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--- СТАТИСТИКА ДІЛЕННЯ (DivisionStatisticsTracker) ---");
        Console.ResetColor();

        Console.WriteLine(_statsTracker.ToString());
        Console.WriteLine();
        Console.WriteLine($"• Усього операцій ділення:       {_statsTracker.TotalDivisions}");
        Console.WriteLine($"• Ділень націло (без остачі):    {_statsTracker.ExactDivisionsCount}");
        Console.WriteLine($"• Ділень з ненульовою остачею:   {_statsTracker.NonExactDivisionsCount}");
        Console.WriteLine($"• Відсоток ділень націло:        {_statsTracker.ExactDivisionPercentage:F2}%");
        Console.WriteLine($"• Сума всіх часток:              {_statsTracker.SumOfQuotients}");
        Console.WriteLine($"• Середня частка:                {_statsTracker.AverageQuotient:F2}");
        Console.WriteLine($"• Максимальне ділене:            {_statsTracker.MaxDividend?.ToString() ?? "N/A"}");
        Console.WriteLine($"• Мінімальне ділене:             {_statsTracker.MinDividend?.ToString() ?? "N/A"}");
    }

    private void RunAutomatedShowcase()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== АВТОМАТИЧНИЙ ДЕМОНСТРАЦІЙНИЙ СЦЕНАРІЙ ===");
        Console.ResetColor();

        Console.WriteLine("1. Перевірка аналізу рядка через 4 механізми делегатів:");
        var analysis = CharacterCounter.CompareAllApproaches("Події та делегати у C#", 'е', true);
        Console.WriteLine($"   Рядок: \"Події та делегати у C#\", символ: 'е' (ігнорування регістру: true)");
        Console.WriteLine($"   Результат: {analysis.NamedMethodCount} входжень (AllMatch: {analysis.AllResultsMatch})");

        Console.WriteLine("\n2. Виконання серії з 4 цілочисельних ділень із генерацією події:");
        _calculator.IntegerDivideWithRemainder(100, 20, "Демо націло 1");
        _calculator.IntegerDivideWithRemainder(77, 6, "Демо з остачею 1");
        _calculator.IntegerDivideWithRemainder(50, 10, "Демо націло 2");
        _calculator.IntegerDivideWithRemainder(13, 4, "Демо з остачею 2");

        Console.WriteLine("\n3. Демонстрація відписки (-=) ConsoleNotificationHandler:");
        _notificationHandler.Unsubscribe(_calculator);
        Console.WriteLine("   [!] ConsoleNotificationHandler відписано. Виконуємо: 42 / 7");
        _calculator.IntegerDivideWithRemainder(42, 7, "Після відписки сповіщувача");
        Console.WriteLine("   Повідомлення в консоль не виведено, але аудит та статистика оновлені!");

        Console.WriteLine("\n4. Відновлення підписки (+=) ConsoleNotificationHandler:");
        _notificationHandler.Subscribe(_calculator);
        Console.WriteLine("   [!] ConsoleNotificationHandler знову підписано.");

        Console.WriteLine("\n5. Поточний звіт статистики:");
        Console.WriteLine($"   {_statsTracker}");
    }

    private static long ReadLong(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (long.TryParse(input, out long value))
            {
                return value;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Будь ласка, введіть коректне ціле число.");
            Console.ResetColor();
        }
    }

    private static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (double.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, out double value) ||
                double.TryParse(input, out value))
            {
                return value;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Будь ласка, введіть коректне дійсне число.");
            Console.ResetColor();
        }
    }
}
