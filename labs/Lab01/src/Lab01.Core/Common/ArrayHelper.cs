namespace Lab01.Core.Common;

/// <summary>
/// Допоміжний клас для роботи з фіксованими одновимірними масивами (T[]).
/// Замінює стандартні динамічні колекції (List&lt;T&gt;, ArrayList),
/// забезпечуючи суворе дотримання вимог лабораторної роботи щодо використання виключно масивів.
/// </summary>
public static class ArrayHelper
{
    /// <summary>
    /// Додає новий елемент до масиву шляхом ручного виділення нового масиву розміром N + 1
    /// та копіювання існуючих елементів через Array.Copy.
    /// </summary>
    public static T[] Add<T>(T[] source, T item)
    {
        ArgumentNullException.ThrowIfNull(source);

        T[] destination = new T[source.Length + 1];
        Array.Copy(source, destination, source.Length);
        destination[source.Length] = item;
        return destination;
    }

    /// <summary>
    /// Видаляє елемент за вказаним індексом та повертає новий масив розміром N - 1.
    /// </summary>
    public static T[] RemoveAt<T>(T[] source, int index)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (index < 0 || index >= source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Індекс знаходиться поза межами масиву.");
        }

        T[] destination = new T[source.Length - 1];

        // Копіюємо частину до видаленого елемента
        if (index > 0)
        {
            Array.Copy(source, 0, destination, 0, index);
        }

        // Копіюємо частину після видаленого елемента
        if (index < source.Length - 1)
        {
            Array.Copy(source, index + 1, destination, index, source.Length - index - 1);
        }

        return destination;
    }

    /// <summary>
    /// Підраховує кількість елементів у масиві, що задовольняють умову предиката.
    /// </summary>
    public static int Count<T>(T[] source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        int count = 0;
        for (int i = 0; i < source.Length; i++)
        {
            if (predicate(source[i]))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Фільтрує масив за предикатом без використання LINQ або динамічних колекцій.
    /// Працює у два проходи: підрахунок кількості -> виділення точного масиву -> заповнення.
    /// </summary>
    public static T[] Filter<T>(T[] source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        int matchCount = Count(source, predicate);
        T[] result = new T[matchCount];
        int writeIndex = 0;

        for (int i = 0; i < source.Length; i++)
        {
            if (predicate(source[i]))
            {
                result[writeIndex] = source[i];
                writeIndex++;
            }
        }

        return result;
    }

    /// <summary>
    /// Знаходить перший елемент, що відповідає умові, або повертає значення за замовчуванням (default).
    /// </summary>
    public static T? Find<T>(T[] source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < source.Length; i++)
        {
            if (predicate(source[i]))
            {
                return source[i];
            }
        }

        return default;
    }

    /// <summary>
    /// Знаходить індекс першого елемента, що відповідає умові. Якщо не знайдено — повертає -1.
    /// </summary>
    public static int FindIndex<T>(T[] source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < source.Length; i++)
        {
            if (predicate(source[i]))
            {
                return i;
            }
        }

        return -1;
    }
}
