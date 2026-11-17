using Lab05.BLL.Exceptions;

namespace Lab05.BLL.Validation;

public interface IEntityValidator
{
    void ValidateName(string name, string fieldName);
    void ValidateStudentId(string studentId);
    void ValidatePassport(string passport);
    void ValidateCourse(int course);
    void ValidateCity(string city);
    void ValidateDriverLicense(string licenseNumber);
    void ValidateExperience(int experienceYears, int age);
    void ValidateCarModel(string carModel);
    void ValidateAcrobatFields(string circusOrTroupe, string specialization);
}

public class EntityValidator : IEntityValidator
{
    public void ValidateName(string name, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException($"Поле '{fieldName}' не може бути порожнім.", fieldName);
        }

        string trimmed = name.Trim();
        if (!RegexPatterns.NameRegex.IsMatch(trimmed))
        {
            throw new ValidationException(
                $"Поле '{fieldName}' містить неприпустимі символи або має некоректну довжину ('{name}').",
                fieldName);
        }
    }

    public void ValidateStudentId(string studentId)
    {
        if (string.IsNullOrWhiteSpace(studentId))
        {
            throw new ValidationException("Номер студентського квитка не може бути порожнім.", nameof(studentId));
        }

        string trimmed = studentId.Trim();
        if (!RegexPatterns.StudentIdRegex.IsMatch(trimmed))
        {
            throw new ValidationException(
                $"Номер студентського квитка повинен містити 2 літери та 6-8 цифр (наприклад, KB123456). Отримано: '{studentId}'.",
                nameof(studentId));
        }
    }

    public void ValidatePassport(string passport)
    {
        if (string.IsNullOrWhiteSpace(passport))
        {
            throw new ValidationException("Серія/номер паспорту не може бути порожнім.", nameof(passport));
        }

        string trimmed = passport.Trim();
        if (!RegexPatterns.PassportRegex.IsMatch(trimmed))
        {
            throw new ValidationException(
                $"Паспорт повинен містити 2 літери та 6 цифр (книжечка) або 9 цифр (ID-картка). Отримано: '{passport}'.",
                nameof(passport));
        }
    }

    public void ValidateCourse(int course)
    {
        if (course is < 1 or > 6)
        {
            throw new ValidationException($"Курс повинен бути в діапазоні від 1 до 6. Отримано: {course}.", nameof(course));
        }
    }

    public void ValidateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ValidationException("Місто не може бути порожнім.", nameof(city));
        }

        string trimmed = city.Trim();
        if (!RegexPatterns.CityRegex.IsMatch(trimmed))
        {
            throw new ValidationException($"Назва міста містить неприпустимі символи: '{city}'.", nameof(city));
        }
    }

    public void ValidateDriverLicense(string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
        {
            throw new ValidationException("Номер посвідчення водія не може бути порожнім.", nameof(licenseNumber));
        }

        string trimmed = licenseNumber.Trim();
        if (!RegexPatterns.DriverLicenseRegex.IsMatch(trimmed))
        {
            throw new ValidationException(
                $"Номер посвідчення водія повинен містити 3 літери та 6 цифр (наприклад, BTA123456). Отримано: '{licenseNumber}'.",
                nameof(licenseNumber));
        }
    }

    public void ValidateExperience(int experienceYears, int age)
    {
        if (experienceYears < 0)
        {
            throw new ValidationException("Стаж водіння не може бути від'ємним.", nameof(experienceYears));
        }

        if (age < 18)
        {
            throw new ValidationException("Вік водія не може бути менше 18 років.", nameof(age));
        }

        if (experienceYears > age - 18)
        {
            throw new ValidationException(
                $"Стаж водіння ({experienceYears} р.) не може перевищувати вік після повноліття ({age - 18} р.).",
                nameof(experienceYears));
        }
    }

    public void ValidateCarModel(string carModel)
    {
        if (string.IsNullOrWhiteSpace(carModel))
        {
            throw new ValidationException("Модель автомобіля не може бути порожньою.", nameof(carModel));
        }

        if (carModel.Trim().Length < 2)
        {
            throw new ValidationException("Назва моделі авто повинна містити щонайменше 2 символи.", nameof(carModel));
        }
    }

    public void ValidateAcrobatFields(string circusOrTroupe, string specialization)
    {
        if (string.IsNullOrWhiteSpace(circusOrTroupe))
        {
            throw new ValidationException("Назва цирку чи трупи не може бути порожньою.", nameof(circusOrTroupe));
        }

        if (string.IsNullOrWhiteSpace(specialization))
        {
            throw new ValidationException("Спеціалізація акробата не може бути порожньою.", nameof(specialization));
        }
    }
}
