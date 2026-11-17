using System;
using System.Collections.Generic;
using System.Linq;
using Lab04.Core.Math;

namespace Lab04.Core.Handlers;

/// <summary>
/// Запис журналу аудиту операції ділення.
/// </summary>
public sealed record AuditLogEntry(
    Guid OperationId,
    string SenderName,
    long Dividend,
    long Divisor,
    long Quotient,
    long Remainder,
    bool IsExact,
    DateTime TimestampUtc,
    string Note
);

/// <summary>
/// Обробник події, що виконує структурований аудит кожної операції цілочисельного ділення.
/// Зберігає хронологічний журнал усіх подій, що надійшли від одного або декількох калькуляторів.
/// </summary>
public class CalculationAuditLogger : IIntegerDivisionSubscriber
{
    private readonly List<AuditLogEntry> _auditLog = new();

    /// <inheritdoc />
    public string HandlerName { get; }

    /// <summary>
    /// Загальна кількість зафіксованих подій у журналі аудиту.
    /// </summary>
    public int TotalLoggedEntries => _auditLog.Count;

    /// <summary>
    /// Доступ до незмінного списку записів журналу аудиту.
    /// </summary>
    public IReadOnlyList<AuditLogEntry> Logs => _auditLog.AsReadOnly();

    /// <summary>
    /// Конструктор журналу аудиту.
    /// </summary>
    /// <param name="handlerName">Назва екземпляра аудитора.</param>
    public CalculationAuditLogger(string handlerName = "AuditLogger")
    {
        HandlerName = string.IsNullOrWhiteSpace(handlerName) ? "AuditLogger" : handlerName;
    }

    /// <inheritdoc />
    public void OnIntegerDivision(object? sender, IntegerDivisionEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        string senderName = sender is IMathCalculator calc ? calc.Name : (sender?.ToString() ?? "Unknown");

        var entry = new AuditLogEntry(
            OperationId: e.OperationId,
            SenderName: senderName,
            Dividend: e.Dividend,
            Divisor: e.Divisor,
            Quotient: e.Quotient,
            Remainder: e.Remainder,
            IsExact: e.IsExact,
            TimestampUtc: e.TimestampUtc,
            Note: e.Note
        );

        _auditLog.Add(entry);
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
    /// Повертає лише операції, виконані націло без остачі.
    /// </summary>
    public IReadOnlyList<AuditLogEntry> GetExactDivisions()
    {
        return _auditLog.Where(entry => entry.IsExact).ToList().AsReadOnly();
    }

    /// <summary>
    /// Повертає операції ділення, в яких була присутня ненульова остача.
    /// </summary>
    public IReadOnlyList<AuditLogEntry> GetDivisionsWithRemainder()
    {
        return _auditLog.Where(entry => !entry.IsExact).ToList().AsReadOnly();
    }

    /// <summary>
    /// Знаходить запис аудиту за ідентифікатором операції.
    /// </summary>
    public AuditLogEntry? FindById(Guid operationId)
    {
        return _auditLog.FirstOrDefault(e => e.OperationId == operationId);
    }

    /// <summary>
    /// Очищає журнал аудиту.
    /// </summary>
    public void Clear()
    {
        _auditLog.Clear();
    }
}
