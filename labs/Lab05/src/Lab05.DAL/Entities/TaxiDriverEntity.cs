namespace Lab05.DAL.Entities;

public class TaxiDriverEntity : PersonEntity
{
    public string CarModel { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public int Age { get; set; }
    public bool HasMedicalCertificate { get; set; }
    public bool HasActiveLicense { get; set; }
}
