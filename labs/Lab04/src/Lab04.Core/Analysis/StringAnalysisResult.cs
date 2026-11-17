using System;

namespace Lab04.Core.Analysis;

/// <summary>
/// Модель результату порівняльного аналізу виконання підрахунку різними механізмами делегатів.
/// </summary>
public sealed record StringAnalysisResult
{
    /// <summary>
    /// Вхідний рядок, який аналізувався.
    /// </summary>
    public string SourceText { get; init; } = string.Empty;

    /// <summary>
    /// Шуканий символ.
    /// </summary>
    public char TargetChar { get; init; }

    /// <summary>
    /// Чи враховувався регістр символів.
    /// </summary>
    public bool IgnoreCase { get; init; }

    /// <summary>
    /// Результат виконання через іменований метод.
    /// </summary>
    public int NamedMethodCount { get; init; }

    /// <summary>
    /// Результат виконання через анонімний метод (delegate keyword).
    /// </summary>
    public int AnonymousMethodCount { get; init; }

    /// <summary>
    /// Результат виконання через лямбда-вираз.
    /// </summary>
    public int LambdaExpressionCount { get; init; }

    /// <summary>
    /// Результат виконання через узагальнений системний делегат Func&lt;string, char, int&gt;.
    /// </summary>
    public int FuncDelegateCount { get; init; }

    /// <summary>
    /// Ознака того, що всі 4 механізми повернули ідентичний результат.
    /// </summary>
    public bool AllResultsMatch =>
        NamedMethodCount == AnonymousMethodCount &&
        AnonymousMethodCount == LambdaExpressionCount &&
        LambdaExpressionCount == FuncDelegateCount;
}
