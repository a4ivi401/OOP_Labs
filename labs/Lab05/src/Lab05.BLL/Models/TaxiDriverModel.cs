using Lab05.BLL.Interfaces;

namespace Lab05.BLL.Models;

public class TaxiDriverModel : PersonModel, IDancer
{
    public string CarModel { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public int Age { get; set; } = 25;
    public bool HasMedicalCertificate { get; set; } = true;
    public bool HasActiveLicense { get; set; } = true;

    public string Dance(string danceStyle = "Вальс")
    {
        string style = string.IsNullOrWhiteSpace(danceStyle) ? "Вальс" : danceStyle.Trim();
        return $"Водій таксі {FullName} (авто: {CarModel}, стаж {ExperienceYears} р.) енергійно виконує '{style}' під час перерви на стоянці.";
    }

    public string Drive(string destination)
    {
        if (!HasActiveLicense)
        {
            return $"Водій {FullName} не може виконати замовлення: водійське посвідчення недійсне!";
        }

        return $"Водій {FullName} на автомобілі {CarModel} вирушив за маршрутом до: '{destination}'.";
    }

    public override string ToString()
    {
        string status = HasActiveLicense ? "Ліцензія активна" : "Без ліцензії";
        return $"{base.ToString()} | Авто: {CarModel} | Посвідчення: {LicenseNumber} | Стаж: {ExperienceYears} р. | Вік: {Age} | {status}";
    }
}
