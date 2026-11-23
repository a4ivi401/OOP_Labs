using Lab05.BLL.Models;

namespace Lab05.BLL.Interfaces;

public interface ITaxiDriverService
{
    IReadOnlyList<TaxiDriverModel> GetAll();
    TaxiDriverModel GetById(int id);
    TaxiDriverModel? GetByLicenseNumber(string licenseNumber);
    void Add(TaxiDriverModel driver);
    void Update(TaxiDriverModel driver);
    void Delete(int id);
    DriverLicenseResult UpgradeLicense(int driverId, string newCategory);
    void SaveToFile(string filePath, string format = "JSON");
    void LoadFromFile(string filePath, string format = "JSON");
}

public interface IAcrobatService
{
    IReadOnlyList<AcrobatModel> GetAll();
    AcrobatModel GetById(int id);
    void Add(AcrobatModel acrobat);
    void Update(AcrobatModel acrobat);
    void Delete(int id);
    string PerformSafetyAudit(int acrobatId, bool passAudit);
    void SaveToFile(string filePath, string format = "JSON");
    void LoadFromFile(string filePath, string format = "JSON");
}
