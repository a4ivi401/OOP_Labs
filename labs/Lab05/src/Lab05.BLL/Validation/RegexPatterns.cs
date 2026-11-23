using System.Text.RegularExpressions;

namespace Lab05.BLL.Validation;

public static partial class RegexPatterns
{
    // Name: letters, hyphens, apostrophes (Ukrainian + Latin), length 2-50
    public const string NamePattern = @"^[A-Za-zА-Яа-яІіЇїЄєҐґ][A-Za-zА-Яа-яІіЇїЄєҐґ'\-’\s]{1,49}$";

    // Student ID: 2 letters + 6 to 8 digits (e.g. KB123456)
    public const string StudentIdPattern = @"^[A-Za-zА-Яа-яІіЇїЄєҐґ]{2}\d{6,8}$";

    // Passport: 2 letters + 6 digits (old book) OR 9 digits (modern ID card)
    public const string PassportPattern = @"^([A-Za-zА-Яа-яІіЇїЄєҐґ]{2}\d{6}|\d{9})$";

    // City: 2-50 characters
    public const string CityPattern = @"^[A-Za-zА-Яа-яІіЇїЄєҐґ][A-Za-zА-Яа-яІіЇїЄєҐґ'\-’\s.]{1,49}$";

    // Driver License: 3 letters + 6 digits (e.g. BTA123456)
    public const string DriverLicensePattern = @"^[A-Za-zА-Яа-яІіЇїЄєҐґ]{3}\d{6}$";

    public static readonly Regex NameRegex = new(NamePattern, RegexOptions.Compiled);
    public static readonly Regex StudentIdRegex = new(StudentIdPattern, RegexOptions.Compiled);
    public static readonly Regex PassportRegex = new(PassportPattern, RegexOptions.Compiled);
    public static readonly Regex CityRegex = new(CityPattern, RegexOptions.Compiled);
    public static readonly Regex DriverLicenseRegex = new(DriverLicensePattern, RegexOptions.Compiled);
}
