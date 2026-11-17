namespace Lab03.DAL.Entities;

public class TaxiDriverEntity : PersonEntity
{
    public string CarModel { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int DrivingExperienceYears { get; set; }

    public TaxiDriverEntity()
    {
    }

    public TaxiDriverEntity(string id, string firstName, string lastName, string carModel, string licenseNumber, int drivingExperienceYears)
        : base(id, firstName, lastName)
    {
        CarModel = carModel;
        LicenseNumber = licenseNumber;
        DrivingExperienceYears = drivingExperienceYears;
    }
}
