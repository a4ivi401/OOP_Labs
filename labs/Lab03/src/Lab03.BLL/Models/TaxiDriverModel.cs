using Lab03.BLL.Exceptions;
using Lab03.BLL.Interfaces;

namespace Lab03.BLL.Models;

public class TaxiDriverModel : PersonModel, IDancer
{
    private string _carModel = string.Empty;
    private string _licenseNumber = string.Empty;
    private int _drivingExperienceYears;

    public string CarModel
    {
        get => _carModel;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(CarModel), "Модель авто не може бути порожньою.");
            }
            _carModel = value.Trim();
        }
    }

    public string LicenseNumber
    {
        get => _licenseNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(LicenseNumber), "Номер посвідчення не може бути порожнім.");
            }
            _licenseNumber = value.Trim().ToUpperInvariant();
        }
    }

    public int DrivingExperienceYears
    {
        get => _drivingExperienceYears;
        set
        {
            if (value < 0 || value > 70)
            {
                throw new ValidationException(nameof(DrivingExperienceYears), "Стаж водіння має бути від 0 до 70 років.");
            }
            _drivingExperienceYears = value;
        }
    }

    public TaxiDriverModel(string id, string firstName, string lastName, string carModel, string licenseNumber, int drivingExperienceYears)
        : base(id, firstName, lastName)
    {
        CarModel = carModel;
        LicenseNumber = licenseNumber;
        DrivingExperienceYears = drivingExperienceYears;
    }

    public string Dance(string danceStyle = "Вальс")
    {
        return $"Водій таксі {FullName} ({CarModel}, стаж {DrivingExperienceYears} р.) танцює {danceStyle}.";
    }

    public override string ToString()
    {
        return $"[Таксист] {FullName}, Авто: {CarModel}, Посвідчення: {LicenseNumber}, Стаж: {DrivingExperienceYears} р.";
    }
}
