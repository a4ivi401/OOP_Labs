using System;
using Lab04.Core.Math;

namespace Lab04.Core.Handlers;

/// <summary>
/// Обробник події, що акумулює статистичні показники виконання цілочисельного ділення.
/// Демонструє підтримку кількох незалежних підписників на одну подію (мультикаст-делегат).
/// </summary>
public class DivisionStatisticsTracker : IIntegerDivisionSubscriber
{
    private long _totalDivisions;
    private long _exactDivisionsCount;
    private long _nonExactDivisionsCount;
    private long _sumOfQuotients;
    private long? _maxDividend;
    private long? _minDividend;

    /// <inheritdoc />
    public string HandlerName { get; }

    /// <summary>
    /// Загальна кількість зафіксованих операцій цілочисельного ділення.
    /// </summary>
    public long TotalDivisions => _totalDivisions;

    /// <summary>
    /// Кількість операцій ділення націло без остачі.
    /// </summary>
    public long ExactDivisionsCount => _exactDivisionsCount;

    /// <summary>
    /// Кількість операцій ділення з ненульовою остачею.
    /// </summary>
    public long NonExactDivisionsCount => _nonExactDivisionsCount;

    /// <summary>
    /// Сума всіх отриманих часток.
    /// </summary>
    public long SumOfQuotients => _sumOfQuotients;

    /// <summary>
    /// Середнє значення частки.
    /// </summary>
    public double AverageQuotient => _totalDivisions > 0 ? (double)_sumOfQuotients / _totalDivisions : 0.0;

    /// <summary>
    /// Відсоток ділень націло від загальної кількості.
    /// </summary>
    public double ExactDivisionPercentage => _totalDivisions > 0
        ? (double)_exactDivisionsCount / _totalDivisions * 100.0
        : 0.0;

    /// <summary>
    /// Найбільше ділене серед усіх оброблених операцій.
    /// </summary>
    public long? MaxDividend => _maxDividend;

    /// <summary>
    /// Найменше ділене серед усіх оброблених операцій.
    /// </summary>
    public long? MinDividend => _minDividend;

    /// <summary>
    /// Конструктор трекера статистики.
    /// </summary>
    /// <param name="handlerName">Назва екземпляра трекера.</param>
    public DivisionStatisticsTracker(string handlerName = "StatsTracker")
    {
        HandlerName = string.IsNullOrWhiteSpace(handlerName) ? "StatsTracker" : handlerName;
    }

    /// <inheritdoc />
    public void OnIntegerDivision(object? sender, IntegerDivisionEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        _totalDivisions++;
        _sumOfQuotients += e.Quotient;

        if (e.IsExact)
        {
            _exactDivisionsCount++;
        }
        else
        {
            _nonExactDivisionsCount++;
        }

        if (!_maxDividend.HasValue || e.Dividend > _maxDividend.Value)
        {
            _maxDividend = e.Dividend;
        }

        if (!_minDividend.HasValue || e.Dividend < _minDividend.Value)
        {
            _minDividend = e.Dividend;
        }
    }

    /// <summary>
    /// Альтернативний обробник для демонстрації роботи з кастомним делегатом IntegerDivisionEventHandler.
    /// </summary>
    public void OnIntegerDivisionCustom(object? sender, IntegerDivisionEventArgs e)
    {
        OnIntegerDivision(sender, e);
    }

    /// <inheritdoc />
    public void Subscribe(IMathCalculator calculator)
    {
        ArgumentNullException.ThrowIfNull(calculator);
        calculator.IntegerDivision += OnIntegerDivision;
    }

    /// <inheritdoc />
    public void Unsubscribe(IMathCalculator calculator)
    {
        ArgumentNullException.ThrowIfNull(calculator);
        calculator.IntegerDivision -= OnIntegerDivision;
    }

    /// <summary>
    /// Скидає всі накопичені статистичні показники.
    /// </summary>
    public void Reset()
    {
        _totalDivisions = 0;
        _exactDivisionsCount = 0;
        _nonExactDivisionsCount = 0;
        _sumOfQuotients = 0;
        _maxDividend = null;
        _minDividend = null;
    }

    /// <summary>
    /// Формує текстовий підсумок поточної статистики.
    /// </summary>
    public override string ToString()
    {
        return $"[СТАТИСТИКА: {HandlerName}] Усього операцій ділення: {TotalDivisions} | " +
               $"Націло: {ExactDivisionsCount} ({ExactDivisionPercentage:F1}%) | " +
               $"З остачею: {NonExactDivisionsCount} | " +
               $"Сер. частка: {AverageQuotient:F2} | " +
               $"Діапазон діленого: [{MinDividend ?? 0} .. {MaxDividend ?? 0}]";
    }
}
