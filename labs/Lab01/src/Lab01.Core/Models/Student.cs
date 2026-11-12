using Lab01.Core.Validation;

namespace Lab01.Core.Models;

/// <summary>
/// Клас Студент, що успадковує клас Person та реалізує інтерфейс IDanceable.
/// Містить специфічні поля згідно з Варіантом 2:
/// курс, номер студентського квитка, місто прибуття, паспортні дані.
/// </summary>
public class Student : Person, IDanceable
{
    private int _course;
    private string _studentId = string.Empty;
    private string _cityOfOrigin = string.Empty;
    private string _passport = string.Empty;

    /// <summary>
    /// Курс навчання (від 1 до 6).
    /// </summary>
    public int Course
    {
        get => _course;
        set
        {
            if (!ValidationHelper.IsValidCourse(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Курс повинен бути в діапазоні від 1 до 6.");
            }
            _course = value;
        }
    }

    /// <summary>
    /// Номер студентського квитка (формат: 2 літери + 6 цифр, напр. КВ123456).
    /// </summary>
    public string StudentId
    {
        get => _studentId;
        set
        {
            if (!ValidationHelper.IsValidStudentId(value))
            {
                throw new ArgumentException("Некоректний формат студентського квитка (очікується 2 літери та 6 цифр).", nameof(value));
            }
            _studentId = value.Trim().ToUpperInvariant();
        }
    }

    /// <summary>
    /// Місто прибуття (звідки приїхав студент).
    /// </summary>
    public string CityOfOrigin
    {
        get => _cityOfOrigin;
        set
        {
            if (!ValidationHelper.IsValidCity(value))
            {
                throw new ArgumentException("Назва міста прибуття не може бути порожньою або містити неприпустимі символи.", nameof(value));
            }
            _cityOfOrigin = value.Trim();
        }
    }

    /// <summary>
    /// Серія та номер паспорту (або номер ID-картки).
    /// </summary>
    public string Passport
    {
        get => _passport;
        set
        {
            if (!ValidationHelper.IsValidPassport(value))
            {
                throw new ArgumentException("Некоректний формат паспорта (очікується 2 літери + 6 цифр або 9 цифр).", nameof(value));
            }
            _passport = value.Trim().ToUpperInvariant();
        }
    }

    /// <summary>
    /// Конструктор студента з обов'язковою валідацією полів.
    /// </summary>
    public Student(
        string firstName,
        string lastName,
        int age,
        int course,
        string studentId,
        string cityOfOrigin,
        string passport)
        : base(firstName, lastName, age)
    {
        Course = course;
        StudentId = studentId;
        CityOfOrigin = cityOfOrigin;
        Passport = passport;
    }

    /// <summary>
    /// Специфічний метод для студента - навчання.
    /// </summary>
    public string Study()
    {
        return $"Студент {LastName} {FirstName} (квиток: {StudentId}) старанно вивчає ООП на {Course}-му курсі.";
    }

    /// <summary>
    /// Реалізація інтерфейсу IDanceable.
    /// Демонструє поліморфну поведінку студента під час танцю.
    /// </summary>
    public string Dance()
    {
        return $"Студент {LastName} {FirstName} кружляє у святковому віденському вальсі на університетському балу!";
    }

    /// <summary>
    /// Перевизначення GetInfo() для повного представлення студента.
    /// </summary>
    public override string GetInfo()
    {
        return $"{base.GetInfo()}, Курс: {Course}, Квиток: {StudentId}, Місто: {CityOfOrigin}, Паспорт: {Passport}";
    }
}
