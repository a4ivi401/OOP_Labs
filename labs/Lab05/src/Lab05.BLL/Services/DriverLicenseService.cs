using System.Security.Cryptography;
using Lab05.BLL.Exceptions;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Models;
using Lab05.BLL.Validation;

namespace Lab05.BLL.Services;

public class DriverLicenseService : IDriverLicenseService
{
    private readonly IEntityValidator _validator;

    public DriverLicenseService(IEntityValidator validator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public DriverLicenseResult ApplyForLicense(DriverLicenseApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);
        _validator.ValidateName(application.CandidateName, nameof(application.CandidateName));

        bool eligible = VerifyLicenseConditions(application, out var rejectionReasons);

        if (!eligible)
        {
            throw new DriverLicenseDeniedException(
                string.Join("; ", rejectionReasons),
                $"Кандидату '{application.CandidateName}' відмовлено у видачі посвідчення водія. Причини: {string.Join("; ", rejectionReasons)}");
        }

        DateTime issueDate = DateTime.Now;
        DateTime expiryDate = issueDate.AddYears(2); // First license in Ukraine is valid for 2 years
        string licenseNumber = GenerateLicenseNumber();

        return new DriverLicenseResult
        {
            IsApproved = true,
            CandidateName = application.CandidateName,
            LicenseNumber = licenseNumber,
            Category = string.IsNullOrWhiteSpace(application.RequestedCategory) ? "B" : application.RequestedCategory.ToUpperInvariant(),
            IssueDate = issueDate,
            ExpiryDate = expiryDate
        };
    }

    public bool VerifyLicenseConditions(DriverLicenseApplication application, out List<string> rejectionReasons)
    {
        ArgumentNullException.ThrowIfNull(application);
        rejectionReasons = [];

        if (application.Age < 18)
        {
            rejectionReasons.Add($"Вік кандидата ({application.Age} р.) є меншим за мінімально дозволений (18 років)");
        }

        if (!application.HasMedicalCertificate)
        {
            rejectionReasons.Add("Відсутня дійсна медична довідка про придатність до керування транспортним засобом (форма 083/о)");
        }

        if (application.TheoryScore < 18)
        {
            rejectionReasons.Add($"Не складено теоретичний іспит: набрано {application.TheoryScore} балів з 20 (прохідний бал: 18)");
        }

        if (!application.PassedPracticalTest)
        {
            rejectionReasons.Add("Не складено практичний іспит з керування транспортним засобом на автодромі або в місті");
        }

        if (application.HasCourtBan)
        {
            rejectionReasons.Add("Наявне судове рішення або обмеження щодо заборони права керування транспортними засобами");
        }

        if (!application.PaidStateFee)
        {
            rejectionReasons.Add("Не сплачено державне мито (адміністративний збір) за складання іспитів та видачу бланка посвідчення");
        }

        return rejectionReasons.Count == 0;
    }

    public string GenerateLicenseNumber()
    {
        // Format: 3 letters + 6 digits (e.g. BTA123456)
        string[] prefixes = ["BTA", "CXR", "AAM", "KIE", "ODS", "LVV", "DNP"];
        string prefix = prefixes[RandomNumberGenerator.GetInt32(prefixes.Length)];
        int number = RandomNumberGenerator.GetInt32(100000, 999999);
        return $"{prefix}{number}";
    }
}
