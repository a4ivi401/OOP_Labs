namespace Lab01.Core.Models;

/// <summary>
/// Інтерфейс, що описує здатність сутності танцювати.
/// Демонструє поліморфізм через інтерфейси: різні сутності (Student, Acrobat)
/// реалізують один і той самий метод Dance() по-своєму.
/// </summary>
public interface IDanceable
{
    /// <summary>
    /// Виконує танець та повертає опис дії.
    /// </summary>
    string Dance();
}
