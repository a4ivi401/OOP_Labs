using System.Text.RegularExpressions;
using Lab03.BLL.Exceptions;
using Lab03.BLL.Interfaces;

namespace Lab03.BLL.Models;

public class StudentModel : PersonModel, IDancer
{
    private static readonly Regex StudentIdRegex = new(@"^[A-Za-zА-Яа-яІіЇїЄєҐґ]{2}\d{6,8}$", RegexOptions.Compiled);
    private static readonly Regex PassportRegex = new(@"^([A-Za-zА-Яа-яІіЇїЄєҐґ]{2}\d{6}|\d{9})$", RegexOptions.Compiled);

    private int _course;
    private string _studentId = string.Empty;
    private string _originCity = string.Empty;
    private string _passport = string.Empty;

    public int Course
    {
        get => _course;
        set
        {
            if (value < 1 || value > 6)
            {
                throw new ValidationException(nameof(Course), "Курс має бути в межах від 1 до 6.");
            }
            _course = value;
        }
    }

    public string StudentId
    {
        get => _studentId;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !StudentIdRegex.IsMatch(value.Trim()))
            {
                throw new ValidationException(nameof(StudentId), "Студентський квиток має містити 2 літери та 6-8 цифр (наприклад, KB123456).");
            }
            _studentId = value.Trim().ToUpperInvariant();
        }
    }

    public string OriginCity
    {
        get => _originCity;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(OriginCity), "Місто прибуття не може бути порожнім.");
            }
            _originCity = value.Trim();
        }
    }

    public string Passport
    {
        get => _passport;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !PassportRegex.IsMatch(value.Trim()))
            {
                throw new ValidationException(nameof(Passport), "Паспорт має бути серії з 2 літер і 6 цифр або 9-значним номером ID-картки.");
            }
            _passport = value.Trim().ToUpperInvariant();
        }
    }

    public StudentModel(string id, string firstName, string lastName, int course, string studentId, string originCity, string passport)
        : base(id, firstName, lastName)
    {
        Course = course;
        StudentId = studentId;
        OriginCity = originCity;
        Passport = passport;
    }

    public string Dance(string danceStyle = "Вальс")
    {
        return $"Студент {FullName} ({Course} курс) танцює {danceStyle}.";
    }

    public override string ToString()
    {
        return $"[Студент] {FullName}, Курс: {Course}, Квиток: {StudentId}, Місто: {OriginCity}, Паспорт: {Passport}";
    }
}
