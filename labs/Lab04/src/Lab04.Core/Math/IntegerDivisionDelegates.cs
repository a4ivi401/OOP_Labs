using System;

namespace Lab04.Core.Math;

/// <summary>
/// Кастомний делегат для опису події цілочисельного ділення.
/// Відповідає стандартній сигнатурі подій .NET: (object? sender, EventArgs e).
/// </summary>
/// <param name="sender">Об'єкт-ініціатор виникнення події (MathCalculator).</param>
/// <param name="e">Об'єкт-аргумент події, що містить параметри та результати ділення.</param>
public delegate void IntegerDivisionEventHandler(object? sender, IntegerDivisionEventArgs e);
