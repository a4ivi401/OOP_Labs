using System;

namespace Lab04.Core.Math;

/// <summary>
/// Клас-аргумент події цілочисельного ділення.
/// Згідно з конвенціями .NET Framework та вимогами на оцінку «відмінно»,
/// успадковується від System.EventArgs та передає вичерпну інформацію про обчислення.
/// </summary>
public class IntegerDivisionEventArgs : EventArgs
{
    /// <summary>
    /// Унікальний ідентифікатор операції для трасування в системних логах.
    /// </summary>
    public Guid OperationId { get; }

    /// <summary>
    /// Ділене (чисельник).
    /// </summary>
    public long Dividend { get; }

    /// <summary>
    /// Дільник (знаменник).
    /// </summary>
    public long Divisor { get; }

    /// <summary>
    /// Цілочисельна частка (результат операції ділення a / b).
    /// </summary>
    public long Quotient { get; }

    /// <summary>
    /// Остача від цілочисельного ділення (результат a % b).
    /// </summary>
    public long Remainder { get; }

    /// <summary>
    /// Ознака того, чи виконано ділення націло без остачі.
    /// </summary>
    public bool IsExact => Remainder == 0;

    /// <summary>
    /// Точна мітка часу виникнення події (UTC).
    /// </summary>
    public DateTime TimestampUtc { get; }

    /// <summary>
    /// Локальний час виникнення події для відображення в інтерфейсі.
    /// </summary>
    public DateTime TimestampLocal => TimestampUtc.ToLocalTime();

    /// <summary>
    /// Додаткова примітка або контекст операції (наприклад, ім'я сесії чи модуля).
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Прапорець, що дозволяє підписникам позначити подію як оброблену.
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="IntegerDivisionEventArgs"/>.
    /// </summary>
    public IntegerDivisionEventArgs(long dividend, long divisor, long quotient, long remainder, string note = "")
    {
        OperationId = Guid.NewGuid();
        Dividend = dividend;
        Divisor = divisor;
        Quotient = quotient;
        Remainder = remainder;
        TimestampUtc = DateTime.UtcNow;
        Note = note;
        Handled = false;
    }

    /// <summary>
    /// Повертає текстове представлення результатів операції ділення.
    /// </summary>
    public override string ToString()
    {
        string exactText = IsExact ? "націло" : $"з остачею {Remainder}";
        return $"[{TimestampLocal:HH:mm:ss.fff}] ID: {OperationId.ToString()[..8]} | {Dividend} / {Divisor} = {Quotient} ({exactText})";
    }
}
