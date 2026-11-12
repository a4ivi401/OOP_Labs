namespace Lab01.Core.Models;

/// <summary>
/// Клас Водій таксі, що успадковує клас Person.
/// Демонструє розширення базового класу унікальними властивостями та методами таксиста.
/// </summary>
public class TaxiDriver : Person
{
    private string _driverLicense = string.Empty;
    private string _carModel = string.Empty;
    private int _experienceYears;

    /// <summary>
    /// Номер посвідчення водія.
    /// </summary>
    public string DriverLicense
    {
        get => _driverLicense;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Номер посвідчення водія не може бути порожнім.", nameof(value));
            }
            _driverLicense = value.Trim().ToUpperInvariant();
        }
    }

    /// <summary>
    /// Модель автомобіля таксі.
    /// </summary>
    public string CarModel
    {
        get => _carModel;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Модель авто не може бути порожньою.", nameof(value));
            }
            _carModel = value.Trim();
        }
    }

    /// <summary>
    /// Стаж водіння (у роках).
    /// </summary>
    public int ExperienceYears
    {
        get => _experienceYears;
        set
        {
            if (value < 0 || value > 70)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Стаж водіння має бути від 0 до 70 років.");
            }
            _experienceYears = value;
        }
    }

    /// <summary>
    /// Конструктор для водія таксі.
    /// </summary>
    public TaxiDriver(
        string firstName,
        string lastName,
        int age,
        string driverLicense,
        string carModel,
        int experienceYears)
        : base(firstName, lastName, age)
    {
        DriverLicense = driverLicense;
        CarModel = carModel;
        ExperienceYears = experienceYears;
    }

    /// <summary>
    /// Специфічний метод професійної діяльності таксиста.
    /// </summary>
    public string Drive()
    {
        return $"Водій {LastName} {FirstName} веде авто {CarModel} (стаж: {ExperienceYears} р., права: {DriverLicense}) за викликом пасажира.";
    }

    /// <summary>
    /// Перевизначення інформації про особу.
    /// </summary>
    public override string GetInfo()
    {
        return $"{base.GetInfo()}, Таксист, Авто: {CarModel}, Стаж: {ExperienceYears} р., Посвідчення: {DriverLicense}";
    }
}
