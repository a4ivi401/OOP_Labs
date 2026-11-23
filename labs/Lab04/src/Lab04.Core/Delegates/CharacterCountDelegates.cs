using System;

namespace Lab04.Core.Delegates;

/// <summary>
/// Користувацький делегат для підрахунку кількості входжень заданого символу в рядок.
/// </summary>
/// <param name="source">Вхідний рядок для аналізу.</param>
/// <param name="targetChar">Символ, кількість входжень якого необхідно підрахувати.</param>
/// <returns>Кількість входжень символу в рядок.</returns>
public delegate int CharacterCountDelegate(string? source, char targetChar);

/// <summary>
/// Користувацький делегат для підрахунку кількості входжень символу з можливістю ігнорування регістру.
/// </summary>
/// <param name="source">Вхідний рядок для аналізу.</param>
/// <param name="targetChar">Шуканий символ.</param>
/// <param name="ignoreCase">Ознака ігнорування регістру (true — без урахування регістру).</param>
/// <returns>Кількість входжень символу в рядок.</returns>
public delegate int CharacterCountWithOptionsDelegate(string? source, char targetChar, bool ignoreCase);

/// <summary>
/// Узагальнений (generic) користувацький делегат для аналізу даних.
/// Демонструє контраваріантність (in) та коваріантність (out).
/// </summary>
/// <typeparam name="TSource">Тип джерела даних.</typeparam>
/// <typeparam name="TTarget">Тип шуканого елемента.</typeparam>
/// <typeparam name="TResult">Тип результату обчислення.</typeparam>
public delegate TResult GenericSearchDelegate<in TSource, in TTarget, out TResult>(TSource source, TTarget target);
