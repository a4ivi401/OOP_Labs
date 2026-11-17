using Lab05.BLL.Interfaces;

namespace Lab05.BLL.Models;

public class StudentModel : PersonModel, IDancer
{
    public int Course { get; set; } = 1;
    public string StudentId { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public string Passport { get; set; } = string.Empty;
    public bool IsSettledInDormitory { get; set; }
    public int? DormitoryNumber { get; set; }
    public int? RoomNumber { get; set; }
    public DateTime? SettlementDate { get; set; }

    public bool IsFromAnotherCity(string localCity = "Київ")
    {
        return !string.IsNullOrWhiteSpace(OriginCity) &&
               !OriginCity.Trim().Equals(localCity.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public bool IsFirstCourseNonLocal(string localCity = "Київ")
    {
        return Course == 1 && IsFromAnotherCity(localCity);
    }

    public string Dance(string danceStyle = "Вальс")
    {
        string style = string.IsNullOrWhiteSpace(danceStyle) ? "Вальс" : danceStyle.Trim();
        return $"Студент {FullName} ({Course}-й курс, квиток {StudentId}) танцює '{style}' на студентському балу.";
    }

    public string GetDormitoryInfo()
    {
        if (!IsSettledInDormitory)
        {
            return "Не проживає в гуртожитку";
        }

        string dateStr = SettlementDate.HasValue ? SettlementDate.Value.ToString("dd.MM.yyyy") : "не вказано";
        return $"Гуртожиток №{DormitoryNumber}, кімната {RoomNumber} (поселено: {dateStr})";
    }

    public override string ToString()
    {
        return $"{base.ToString()} | {Course} курс | Квиток: {StudentId} | З міста: {OriginCity} | Паспорт: {Passport} | {GetDormitoryInfo()}";
    }
}
