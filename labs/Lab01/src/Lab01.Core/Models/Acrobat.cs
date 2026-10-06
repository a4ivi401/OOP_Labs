namespace Lab01.Core.Models;

/// <summary>
/// Клас Акробат, що успадковує Person та реалізує інтерфейс IDanceable.
/// Демонструє поліморфізм: акробат і студент танцюють по-різному через інтерфейс IDanceable.
/// </summary>
public class Acrobat : Person, IDanceable
{
    private string _circusName = string.Empty;
    private string _skillLevel = string.Empty;

    /// <summary>
    /// Назва цирку чи трупи, де виступає акробат.
    /// </summary>
    public string CircusName
    {
        get => _circusName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Назва цирку не може бути порожньою.", nameof(value));
            }
            _circusName = value.Trim();
        }
    }

    /// <summary>
    /// Рівень майстерності (наприклад, "Майстер спорту", "Професіонал", "Початківець").
    /// </summary>
    public string SkillLevel
    {
        get => _skillLevel;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Рівень майстерності не може бути порожнім.", nameof(value));
            }
            _skillLevel = value.Trim();
        }
    }

    /// <summary>
    /// Конструктор для акробата.
    /// </summary>
    public Acrobat(
        string firstName,
        string lastName,
        int age,
        string circusName,
        string skillLevel)
        : base(firstName, lastName, age)
    {
        CircusName = circusName;
        SkillLevel = skillLevel;
    }

    /// <summary>
    /// Специфічний акробатичний трюк.
    /// </summary>
    public string PerformAcrobatics()
    {
        return $"Акробат {LastName} {FirstName} ({SkillLevel}) віртуозно виконує потрійне сальто під куполом цирку '{CircusName}'!";
    }

    /// <summary>
    /// Реалізація інтерфейсу IDanceable.
    /// </summary>
    public string Dance()
    {
        return $"Акробат {LastName} {FirstName} виконує експресивний акробатичний танець з елементами гімнастики на арені цирку '{CircusName}'!";
    }

    /// <summary>
    /// Перевизначення інформації про особу.
    /// </summary>
    public override string GetInfo()
    {
        return $"{base.GetInfo()}, Акробат, Цирк: '{CircusName}', Рівень: {SkillLevel}";
    }
}
