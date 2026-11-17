using Lab05.BLL.Interfaces;

namespace Lab05.BLL.Models;

public class AcrobatModel : PersonModel, IDancer
{
    public string CircusOrTroupe { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public bool SafetyCertificationValid { get; set; } = true;
    public int PerformancesCount { get; set; }

    public string Dance(string danceStyle = "Вальс")
    {
        string style = string.IsNullOrWhiteSpace(danceStyle) ? "Вальс" : danceStyle.Trim();
        return $"Акробат {FullName} ({CircusOrTroupe}, '{Specialization}') артистично танцює акробатичний '{style}' на манежі.";
    }

    public string PerformStunt(string stuntName)
    {
        if (!SafetyCertificationValid)
        {
            return $"Акробат {FullName} відсторонений від трюку '{stuntName}': протерміновано сертифікат техніки безпеки!";
        }

        PerformancesCount++;
        return $"Акробат {FullName} бездоганно виконав трюк '{stuntName}' у жанрі '{Specialization}'! (Виступів: {PerformancesCount})";
    }

    public override string ToString()
    {
        string cert = SafetyCertificationValid ? "Сертифікат дійсний" : "Без сертифіката";
        return $"{base.ToString()} | {CircusOrTroupe} | Спеціалізація: {Specialization} | {cert} | Виступів: {PerformancesCount}";
    }
}
