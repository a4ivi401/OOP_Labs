using Lab05.BLL.Models;

namespace Lab05.BLL.Interfaces;

public interface IDriverLicenseService
{
    /// <summary>
    /// Варіант 2: Отримання водійського посвідчення при необхідних умовах.
    /// </summary>
    DriverLicenseResult ApplyForLicense(DriverLicenseApplication application);

    bool VerifyLicenseConditions(DriverLicenseApplication application, out List<string> rejectionReasons);
    string GenerateLicenseNumber();
}
