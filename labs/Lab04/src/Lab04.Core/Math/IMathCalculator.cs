using System;

namespace Lab04.Core.Math;

/// <summary>
/// Інтерфейс математичного компонента (клас багаторазового використання),
/// що надає базові та розширені математичні операції та подію цілочисельного ділення.
/// </summary>
public interface IMathCalculator
{
    /// <summary>
    /// Назва або ідентифікатор калькулятора.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Загальна кількість успішно виконаних операцій.
    /// </summary>
    long TotalOperationsCount { get; }

    /// <summary>
    /// Кількість виконаних операцій цілочисельного ділення.
    /// </summary>
    long IntegerDivisionsCount { get; }

    /// <summary>
    /// Стандартна подія .NET, що виникає при кожному виконанні цілочисельного ділення.
    /// </summary>
    event EventHandler<IntegerDivisionEventArgs>? IntegerDivision;

    /// <summary>
    /// Подія цілочисельного ділення, оголошена через власний кастомний делегат.
    /// </summary>
    event IntegerDivisionEventHandler? IntegerDivisionCustom;

    /// <summary>
    /// Додавання двох цілих чисел.
    /// </summary>
    long Add(long a, long b);

    /// <summary>
    /// Віднімання двох цілих чисел.
    /// </summary>
    long Subtract(long a, long b);

    /// <summary>
    /// Множення двох цілих чисел.
    /// </summary>
    long Multiply(long a, long b);

    /// <summary>
    /// Звичайне ділення дійсних чисел з плаваючою комою.
    /// </summary>
    double Divide(double a, double b);

    /// <summary>
    /// Цілочисельне ділення (повертає цілу частку і генерує подію IntegerDivision).
    /// </summary>
    long IntegerDivide(long dividend, long divisor, string note = "");

    /// <summary>
    /// Цілочисельне ділення з поверненням як частки, так і остачі (і генеруванням події).
    /// </summary>
    (long Quotient, long Remainder) IntegerDivideWithRemainder(long dividend, long divisor, string note = "");

    /// <summary>
    /// Обчислення остачі від ділення (modulo).
    /// </summary>
    long Modulo(long a, long b);

    /// <summary>
    /// Піднесення до степеня.
    /// </summary>
    double Power(double baseValue, double exponent);

    /// <summary>
    /// Обчислення факторіалу цілого числа (0..20).
    /// </summary>
    long Factorial(int n);

    /// <summary>
    /// Обчислення модуля числа (абсолютного значення).
    /// </summary>
    long Abs(long value);
}
