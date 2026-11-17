using System;

namespace Lab04.Core.Math;

/// <summary>
/// Компонент багаторазового використання для виконання математичних операцій
/// (додавання, віднімання, множення, ділення, факторіал тощо).
/// Містить подію «Цілочисельне ділення» згідно з Варіантом 2 Лабораторної роботи 3.4.
/// Реалізує стандартний шаблон проектування подій у .NET:
/// - Клас-аргумент події успадковує EventArgs
/// - Оголошення події через EventHandler&lt;T&gt; та кастомний делегат
/// - Віртуальний метод OnIntegerDivision(...) для безпечного виклику підписників.
/// </summary>
public class MathCalculator : IMathCalculator
{
    private long _totalOperationsCount;
    private long _integerDivisionsCount;

    /// <summary>
    /// Назва екземпляра калькулятора.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    public long TotalOperationsCount => _totalOperationsCount;

    /// <inheritdoc />
    public long IntegerDivisionsCount => _integerDivisionsCount;

    /// <summary>
    /// Стандартна подія .NET Framework, що викликається при кожному виконанні цілочисельного ділення.
    /// </summary>
    public event EventHandler<IntegerDivisionEventArgs>? IntegerDivision;

    /// <summary>
    /// Альтернативна подія цілочисельного ділення через кастомний делегат IntegerDivisionEventHandler.
    /// </summary>
    public event IntegerDivisionEventHandler? IntegerDivisionCustom;

    /// <summary>
    /// Конструктор математичного калькулятора.
    /// </summary>
    /// <param name="name">Назва калькулятора (за замовчуванням "DefaultCalculator").</param>
    public MathCalculator(string name = "DefaultCalculator")
    {
        Name = string.IsNullOrWhiteSpace(name) ? "DefaultCalculator" : name;
    }

    /// <inheritdoc />
    public long Add(long a, long b)
    {
        checked
        {
            long result = a + b;
            _totalOperationsCount++;
            return result;
        }
    }

    /// <inheritdoc />
    public long Subtract(long a, long b)
    {
        checked
        {
            long result = a - b;
            _totalOperationsCount++;
            return result;
        }
    }

    /// <inheritdoc />
    public long Multiply(long a, long b)
    {
        checked
        {
            long result = a * b;
            _totalOperationsCount++;
            return result;
        }
    }

    /// <inheritdoc />
    public double Divide(double a, double b)
    {
        if (System.Math.Abs(b) < double.Epsilon)
        {
            throw new DivideByZeroException("Дільник не може дорівнювати нулю при діленні.");
        }

        double result = a / b;
        _totalOperationsCount++;
        return result;
    }

    /// <inheritdoc />
    public long IntegerDivide(long dividend, long divisor, string note = "")
    {
        var (quotient, _) = IntegerDivideWithRemainder(dividend, divisor, note);
        return quotient;
    }

    /// <inheritdoc />
    public (long Quotient, long Remainder) IntegerDivideWithRemainder(long dividend, long divisor, string note = "")
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException("Ділення на нуль неможливе в цілочисельній арифметиці.");
        }

        long quotient = dividend / divisor;
        long remainder = dividend % divisor;

        _totalOperationsCount++;
        _integerDivisionsCount++;

        // Створення аргументу події
        var eventArgs = new IntegerDivisionEventArgs(dividend, divisor, quotient, remainder, note);

        // Генерація події через захищений віртуальний метод
        OnIntegerDivision(eventArgs);

        return (quotient, remainder);
    }

    /// <inheritdoc />
    public long Modulo(long a, long b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Дільник не може дорівнювати нулю при взятті за модулем.");
        }

        long result = a % b;
        _totalOperationsCount++;
        return result;
    }

    /// <inheritdoc />
    public double Power(double baseValue, double exponent)
    {
        double result = System.Math.Pow(baseValue, exponent);
        _totalOperationsCount++;
        return result;
    }

    /// <inheritdoc />
    public long Factorial(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Факторіал визначений лише для невід'ємних цілих чисел (n >= 0).");
        }

        if (n > 20)
        {
            throw new OverflowException("Обчислення факторіалу для чисел більше 20 призводить до переповнення типу long (64-bit).");
        }

        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }

        _totalOperationsCount++;
        return result;
    }

    /// <inheritdoc />
    public long Abs(long value)
    {
        long result = value < 0 ? -value : value;
        _totalOperationsCount++;
        return result;
    }

    /// <summary>
    /// Захищений віртуальний метод генерації події (Event Invocation Pattern у .NET).
    /// Дозволяє похідним класам перевизначати або розширювати поведінку при виклику події
    /// без необхідності дублювати логіку перевірки підписників на null.
    /// Безпечний до багатопоточності виклик: ?.Invoke(this, e) копіює посилання на делегат перед перевіркою.
    /// </summary>
    /// <param name="e">Аргументи події цілочисельного ділення.</param>
    protected virtual void OnIntegerDivision(IntegerDivisionEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        // 1. Сповіщення підписників стандартної події .NET
        IntegerDivision?.Invoke(this, e);

        // 2. Сповіщення підписників кастомної події з кастомним делегатом
        IntegerDivisionCustom?.Invoke(this, e);
    }
}
