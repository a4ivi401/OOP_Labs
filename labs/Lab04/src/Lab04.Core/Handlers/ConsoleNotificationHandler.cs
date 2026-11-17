using System;
using System.Collections.Generic;
using Lab04.Core.Math;

namespace Lab04.Core.Handlers;

/// <summary>
/// Обробник події цілочисельного ділення, що формує та надсилає сповіщення
/// про виникнення події для користувача.
/// Демонструє отримання інформації про об'єкт-ініціатор (sender) та аргументи події (e).
/// </summary>
public class ConsoleNotificationHandler : IIntegerDivisionSubscriber
{
    private readonly List<string> _renderedMessages = new();
    private readonly Action<string>? _messageSink;

    /// <inheritdoc />
    public string HandlerName { get; }

    /// <summary>
    /// Кількість сформованих та надісланих сповіщень.
    /// </summary>
    public int NotificationsSentCount => _renderedMessages.Count;

    /// <summary>
    /// Історія сформованих повідомлень для верифікації в тестах або відображення.
    /// </summary>
    public IReadOnlyList<string> RenderedMessages => _renderedMessages.AsReadOnly();

    /// <summary>
    /// Конструктор обробника сповіщень.
    /// </summary>
    /// <param name="handlerName">Назва обробника.</param>
    /// <param name="messageSink">Опціональний делегат для перенаправлення виводу (наприклад, Console.WriteLine).</param>
    public ConsoleNotificationHandler(string handlerName = "NotificationService", Action<string>? messageSink = null)
    {
        HandlerName = string.IsNullOrWhiteSpace(handlerName) ? "NotificationService" : handlerName;
        _messageSink = messageSink;
    }

    /// <inheritdoc />
    public void OnIntegerDivision(object? sender, IntegerDivisionEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        string senderInfo = sender switch
        {
            IMathCalculator calc => $"Калькулятор '{calc.Name}' (усього операцій: {calc.TotalOperationsCount})",
            null => "Невідомий ініціатор (null)",
            _ => $"Об'єкт типу {sender.GetType().FullName}"
        };

        string divisionType = e.IsExact
            ? "Ділення націло (без остачі)"
            : $"Ділення з остачею (остача = {e.Remainder})";

        string message =
            $"[СПОСТЕРІГАЧ: {HandlerName}] Подія цілочисельного ділення!\n" +
            $"   Ініціатор: {senderInfo}\n" +
            $"   Операція: {e.Dividend} / {e.Divisor} = {e.Quotient} ({divisionType})\n" +
            $"   Час (UTC): {e.TimestampUtc:yyyy-MM-dd HH:mm:ss.fff}\n" +
            $"   ID операції: {e.OperationId}\n" +
            (string.IsNullOrWhiteSpace(e.Note) ? string.Empty : $"   Контекст: {e.Note}\n");

        _renderedMessages.Add(message);
        _messageSink?.Invoke(message);

        // Позначаємо як оброблене
        e.Handled = true;
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
    /// Очищає накопичений список сформованих повідомлень.
    /// </summary>
    public void ClearMessages()
    {
        _renderedMessages.Clear();
    }
}
