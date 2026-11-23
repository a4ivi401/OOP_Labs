using Lab03.BLL.Exceptions;
using Lab03.BLL.Interfaces;

namespace Lab03.BLL.Models;

public class AcrobatModel : PersonModel, IDancer
{
    private string _circusOrTroupeName = string.Empty;
    private string _specialization = string.Empty;

    public string CircusOrTroupeName
    {
        get => _circusOrTroupeName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(CircusOrTroupeName), "Назва цирку чи трупи не може бути порожньою.");
            }
            _circusOrTroupeName = value.Trim();
        }
    }

    public string Specialization
    {
        get => _specialization;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(Specialization), "Спеціалізація не може бути порожньою.");
            }
            _specialization = value.Trim();
        }
    }

    public AcrobatModel(string id, string firstName, string lastName, string circusOrTroupeName, string specialization)
        : base(id, firstName, lastName)
    {
        CircusOrTroupeName = circusOrTroupeName;
        Specialization = specialization;
    }

    public string Dance(string danceStyle = "Вальс")
    {
        return $"Акробат {FullName} ({Specialization}, {CircusOrTroupeName}) віртуозно виконує танець {danceStyle}.";
    }

    public override string ToString()
    {
        return $"[Акробат] {FullName}, Трупа: {CircusOrTroupeName}, Спеціалізація: {Specialization}";
    }
}
