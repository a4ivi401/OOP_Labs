using System;
using Lab04.Core.Analysis;

namespace Lab04.Core.Delegates;

/// <summary>
/// Сервіс для демонстрації роботи з делегатами при розв'язанні завдання Варіанту 2:
/// «Пошук у рядку кількості використань заданого символу».
/// Демонструє 4 підходи до виклику коду через делегат:
/// 1. Звичайний іменований метод.
/// 2. Анонімний метод (синтаксис delegate (...) { ... }).
/// 3. Лямбда-вираз (синтаксис (...) => ...).
/// 4. Стандартний FCL-делегат Func&lt;string?, char, int&gt;.
/// </summary>
public static class CharacterCounter
{
    // =========================================================================
    // 1. ІМЕНОВАНІ МЕТОДИ (Named Methods)
    // =========================================================================

    /// <summary>
    /// Іменований метод для підрахунку входжень символу (з урахуванням регістру).
    /// Сигнатура повністю відповідає делегату CharacterCountDelegate.
    /// </summary>
    public static int CountOccurrencesNamed(string? source, char targetChar)
    {
        if (string.IsNullOrEmpty(source))
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] == targetChar)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Іменований метод для підрахунку входжень символу з можливістю ігнорування регістру.
    /// Сигнатура відповідає делегату CharacterCountWithOptionsDelegate.
    /// </summary>
    public static int CountOccurrencesNamedWithOptions(string? source, char targetChar, bool ignoreCase)
    {
        if (string.IsNullOrEmpty(source))
        {
            return 0;
        }

        if (!ignoreCase)
        {
            return CountOccurrencesNamed(source, targetChar);
        }

        char normalizedTarget = char.ToUpperInvariant(targetChar);
        int count = 0;
        for (int i = 0; i < source.Length; i++)
        {
            if (char.ToUpperInvariant(source[i]) == normalizedTarget)
            {
                count++;
            }
        }

        return count;
    }

    // =========================================================================
    // 2. АНОНІМНІ МЕТОДИ (Anonymous Methods — C# 2.0 syntax)
    // =========================================================================

    /// <summary>
    /// Створює екземпляр делегата CharacterCountDelegate на основі анонімного методу (delegate keyword).
    /// </summary>
    public static CharacterCountDelegate CreateAnonymousMethod()
    {
        return delegate (string? text, char target)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int matches = 0;
            foreach (char ch in text)
            {
                if (ch == target)
                {
                    matches++;
                }
            }

            return matches;
        };
    }

    /// <summary>
    /// Створює екземпляр делегата CharacterCountWithOptionsDelegate на основі анонімного методу.
    /// </summary>
    public static CharacterCountWithOptionsDelegate CreateAnonymousMethodWithOptions()
    {
        return delegate (string? text, char target, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int matches = 0;
            char targetComparison = ignoreCase ? char.ToUpperInvariant(target) : target;

            foreach (char ch in text)
            {
                char current = ignoreCase ? char.ToUpperInvariant(ch) : ch;
                if (current == targetComparison)
                {
                    matches++;
                }
            }

            return matches;
        };
    }

    // =========================================================================
    // 3. ЛЯМБДА-ВИРАЗИ (Lambda Expressions — C# 3.0+ syntax)
    // =========================================================================

    /// <summary>
    /// Створює екземпляр делегата CharacterCountDelegate на основі блочного лямбда-виразу.
    /// </summary>
    public static CharacterCountDelegate CreateLambdaExpression()
    {
        return (source, targetChar) =>
        {
            if (string.IsNullOrEmpty(source))
            {
                return 0;
            }

            int occurrences = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == targetChar)
                {
                    occurrences++;
                }
            }

            return occurrences;
        };
    }

    /// <summary>
    /// Створює екземпляр делегата CharacterCountDelegate на основі лаконічного виразного лямбда-виразу.
    /// </summary>
    public static CharacterCountDelegate CreateExpressionBodiedLambda()
    {
        return (source, targetChar) => string.IsNullOrEmpty(source) ? 0 : CountOccurrencesNamed(source, targetChar);
    }

    /// <summary>
    /// Створює екземпляр делегата CharacterCountWithOptionsDelegate на основі лямбда-виразу.
    /// </summary>
    public static CharacterCountWithOptionsDelegate CreateLambdaExpressionWithOptions()
    {
        return (source, targetChar, ignoreCase) =>
        {
            if (string.IsNullOrEmpty(source))
            {
                return 0;
            }

            int total = 0;
            char normalizedTarget = ignoreCase ? char.ToUpperInvariant(targetChar) : targetChar;

            for (int i = 0; i < source.Length; i++)
            {
                char current = ignoreCase ? char.ToUpperInvariant(source[i]) : source[i];
                if (current == normalizedTarget)
                {
                    total++;
                }
            }

            return total;
        };
    }

    // =========================================================================
    // 4. СТАНДАРТНІ FCL ДЕЛЕГАТИ (System.Func<...>)
    // =========================================================================

    /// <summary>
    /// Створює стандартний делегат бібліотеки базових класів .NET (Func&lt;string?, char, int&gt;).
    /// </summary>
    public static Func<string?, char, int> CreateFuncDelegate()
    {
        return (str, chr) => CountOccurrencesNamed(str, chr);
    }

    /// <summary>
    /// Створює Func з підтримкою налаштування ігнорування регістру.
    /// </summary>
    public static Func<string?, char, bool, int> CreateFuncWithOptionsDelegate()
    {
        return (str, chr, ignoreCase) => CountOccurrencesNamedWithOptions(str, chr, ignoreCase);
    }

    // =========================================================================
    // 5. ВИСОКОРІВНЕВЕ ВИКОНАННЯ ТА ПОРІВНЯННЯ РЕЗУЛЬТАТІВ
    // =========================================================================

    /// <summary>
    /// Викликає алгоритм через переданий користувацький делегат CharacterCountDelegate.
    /// </summary>
    public static int ExecuteCount(string? source, char targetChar, CharacterCountDelegate countDelegate)
    {
        ArgumentNullException.ThrowIfNull(countDelegate);
        return countDelegate(source, targetChar);
    }

    /// <summary>
    /// Порівнює роботу всіх 4 реалізацій (іменований метод, анонімний метод, лямбда, Func)
    /// на однакових вхідних даних для підтвердження ідентичності семантики.
    /// </summary>
    public static StringAnalysisResult CompareAllApproaches(string? source, char targetChar, bool ignoreCase = false)
    {
        string safeText = source ?? string.Empty;

        // 1. Іменований метод
        CharacterCountWithOptionsDelegate namedDelegate = CountOccurrencesNamedWithOptions;
        int namedResult = namedDelegate(safeText, targetChar, ignoreCase);

        // 2. Анонімний метод
        CharacterCountWithOptionsDelegate anonymousDelegate = CreateAnonymousMethodWithOptions();
        int anonymousResult = anonymousDelegate(safeText, targetChar, ignoreCase);

        // 3. Лямбда-вираз
        CharacterCountWithOptionsDelegate lambdaDelegate = CreateLambdaExpressionWithOptions();
        int lambdaResult = lambdaDelegate(safeText, targetChar, ignoreCase);

        // 4. FCL Func делегат
        Func<string?, char, bool, int> funcDelegate = CreateFuncWithOptionsDelegate();
        int funcResult = funcDelegate(safeText, targetChar, ignoreCase);

        return new StringAnalysisResult
        {
            SourceText = safeText,
            TargetChar = targetChar,
            IgnoreCase = ignoreCase,
            NamedMethodCount = namedResult,
            AnonymousMethodCount = anonymousResult,
            LambdaExpressionCount = lambdaResult,
            FuncDelegateCount = funcResult
        };
    }
}
