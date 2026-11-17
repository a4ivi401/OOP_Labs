namespace Lab05.BLL.Models;

public class DriverLicenseApplication
{
    public string CandidateName { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool HasMedicalCertificate { get; set; }
    public int TheoryScore { get; set; } // Max 20, passing is >= 18
    public bool PassedPracticalTest { get; set; }
    public string RequestedCategory { get; set; } = "B";
    public bool HasCourtBan { get; set; }
    public bool PaidStateFee { get; set; } = true;
}

public class DriverLicenseResult
{
    public bool IsApproved { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public List<string> RejectionReasons { get; set; } = [];

    public string Summary => IsApproved
        ? $"ПОСВІДЧЕННЯ ВОДІЯ ВИДАНО: {CandidateName} | № {LicenseNumber} (Кат. {Category}) | Дійсне до {ExpiryDate:dd.MM.yyyy}"
        : $"ВІДМОВА У ВИДАЧІ ВОДІЙСЬКОГО ПОСВІДЧЕННЯ: {CandidateName} | Причини: {string.Join("; ", RejectionReasons)}";
}
