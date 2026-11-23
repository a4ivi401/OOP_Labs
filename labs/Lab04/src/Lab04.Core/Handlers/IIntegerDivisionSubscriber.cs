using System;
using Lab04.Core.Math;

namespace Lab04.Core.Handlers;

/// <summary>
/// Інтерфейс для класів-обробників події цілочисельного ділення.
/// Гарантує відповідність сигнатури стандартному патерну обробників подій у .NET:
/// void Handler(object? sender, IntegerDivisionEventArgs e).
/// </summary>
public interface IIntegerDivisionSubscriber
{
    /// <summary>
    /// Назва або ідентифікатор обробника події.
    /// </summary>
    string HandlerName { get; }

    /// <summary>
    /// Метод-обробник події, який отримує об'єкт-ініціатор (sender) та аргументи події (e).
    /// </summary>
    /// <param name="sender">Об'єкт, що згенерував подію (зазвичай IMathCalculator).</param>
    /// <param name="e">Параметри події (ділене, дільник, частка, остача, мітка часу тощо).</param>
    void OnIntegerDivision(object? sender, IntegerDivisionEventArgs e);

    /// <summary>
    /// Підписує даний обробник на подію цілочисельного ділення калькулятора (оператор +=).
    /// </summary>
    void Subscribe(IMathCalculator calculator);

    /// <summary>
    /// Відписує даний обробник від події цілочисельного ділення калькулятора (оператор -=).
    /// </summary>
    void Unsubscribe(IMathCalculator calculator);
}
