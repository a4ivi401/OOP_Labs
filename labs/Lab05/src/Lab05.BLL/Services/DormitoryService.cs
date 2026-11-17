using Lab05.BLL.Exceptions;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Mappers;
using Lab05.BLL.Models;
using Lab05.BLL.Validation;
using Lab05.DAL.Repositories;

namespace Lab05.BLL.Services;

public class DormitoryService : IDormitoryService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEntityValidator _validator;

    public DormitoryService(IStudentRepository studentRepository, IEntityValidator validator)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public DormitorySettlementResult SettleInDormitory(
        string studentId,
        int dormitoryNumber,
        int roomNumber,
        string localCity = "Київ")
    {
        _validator.ValidateStudentId(studentId);
        _validator.ValidateCity(localCity);

        if (dormitoryNumber is < 1 or > 50)
        {
            throw new ValidationException(
                $"Номер гуртожитку повинен бути в межах від 1 до 50. Отримано: {dormitoryNumber}.",
                nameof(dormitoryNumber));
        }

        if (roomNumber is < 1 or > 999)
        {
            throw new ValidationException(
                $"Номер кімнати повинен бути в межах від 1 до 999. Отримано: {roomNumber}.",
                nameof(roomNumber));
        }

        var student = _studentRepository.GetByStudentId(studentId);
        if (student is null)
        {
            throw new EntityNotFoundException(
                $"Студента зі студентським квитком '{studentId}' не знайдено для поселення в гуртожиток.");
        }

        string fullName = $"{student.LastName} {student.FirstName}".Trim();

        // Rule: Only students from other cities are eligible!
        if (student.OriginCity.Trim().Equals(localCity.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new IneligibleForDormitoryException(
                studentId,
                student.OriginCity,
                $"Студент {fullName} проживає у місті {localCity} (місцевий житель) і не має права на поселення в гуртожиток університету.");
        }

        // Rule: Check if already settled
        if (student.IsSettledInDormitory)
        {
            throw new InvalidOperationException(
                $"Студент {fullName} ({studentId}) вже поселений у гуртожиток №{student.DormitoryNumber}, кімната {student.RoomNumber}.");
        }

        // Perform settlement
        DateTime settlementDate = DateTime.Now;
        student.IsSettledInDormitory = true;
        student.DormitoryNumber = dormitoryNumber;
        student.RoomNumber = roomNumber;
        student.SettlementDate = settlementDate;

        _studentRepository.Update(student);

        return DormitorySettlementResult.Success(studentId, fullName, dormitoryNumber, roomNumber, settlementDate);
    }

    public bool EvictFromDormitory(string studentId)
    {
        _validator.ValidateStudentId(studentId);

        var student = _studentRepository.GetByStudentId(studentId);
        if (student is null)
        {
            throw new EntityNotFoundException($"Студента зі студентським квитком '{studentId}' не знайдено.");
        }

        if (!student.IsSettledInDormitory)
        {
            return false;
        }

        student.IsSettledInDormitory = false;
        student.DormitoryNumber = null;
        student.RoomNumber = null;
        student.SettlementDate = null;

        _studentRepository.Update(student);
        return true;
    }

    public IReadOnlyList<StudentModel> GetSettledStudents()
    {
        return _studentRepository.GetAll()
            .Where(s => s.IsSettledInDormitory)
            .Select(StudentMapper.ToModel)
            .ToList()
            .AsReadOnly();
    }

    public bool IsEligibleForDormitory(string studentId, string localCity, out string reason)
    {
        _validator.ValidateStudentId(studentId);
        _validator.ValidateCity(localCity);

        var student = _studentRepository.GetByStudentId(studentId);
        if (student is null)
        {
            reason = "Студента не знайдено в базі даних.";
            return false;
        }

        if (student.OriginCity.Trim().Equals(localCity.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            reason = $"Студент є місцевим жителем ({localCity}).";
            return false;
        }

        if (student.IsSettledInDormitory)
        {
            reason = $"Студент вже поселений у гуртожиток №{student.DormitoryNumber}, кімната {student.RoomNumber}.";
            return false;
        }

        reason = "Студент має право на отримання місця в гуртожитку.";
        return true;
    }
}
