namespace Lab01.Core.Models;

/// <summary>
/// Базовий абстрактний клас, що представляє людину.
/// Демонструє принцип інкапсуляції (захист полів через властивості)
/// та спадкування (є спільним предком для Student, TaxiDriver, Acrobat).
/// </summary>
public abstract class Person
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private int _age;

    /// <summary>
    /// Ім'я людини.
    /// </summary>
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Ім'я не може бути порожнім.", nameof(value));
            }
            _firstName = value.Trim();
        }
    }

    /// <summary>
    /// Прізвище людини.
    /// </summary>
    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Прізвище не може бути порожнім.", nameof(value));
            }
            _lastName = value.Trim();
        }
    }

    /// <summary>
    /// Вік людини (має бути в діапазоні від 1 до 120 років).
    /// </summary>
    public int Age
    {
        get => _age;
        set
        {
            if (value is < 1 or > 120)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Вік повинен бути в межах від 1 до 120.");
            }
            _age = value;
        }
    }

    /// <summary>
    /// Захищений конструктор базового класу.
    /// </summary>
    protected Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    /// <summary>
    /// Віртуальний метод для отримання коротких відомостей про людину.
    /// Дочірні класи можуть перевизначати (override) цей метод.
    /// </summary>
    public virtual string GetInfo()
    {
        return $"{LastName} {FirstName}, вік: {Age}";
    }

    /// <summary>
    /// Перевизначення стандартного методу ToString() для зручного виводу.
    /// </summary>
    public override string ToString()
    {
        return GetInfo();
    }
}
