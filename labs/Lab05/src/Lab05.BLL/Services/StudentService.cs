using Lab05.BLL.Exceptions;
using Lab05.BLL.Interfaces;
using Lab05.BLL.Mappers;
using Lab05.BLL.Models;
using Lab05.BLL.Validation;
using Lab05.DAL.Entities;
using Lab05.DAL.Providers;
using Lab05.DAL.Repositories;

namespace Lab05.BLL.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly IEntityValidator _validator;
    private readonly IEnumerable<IDataProvider<StudentEntity>> _providers;

    public StudentService(
        IStudentRepository repository,
        IEntityValidator validator,
        IEnumerable<IDataProvider<StudentEntity>> providers)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public IReadOnlyList<StudentModel> GetAll()
    {
        return _repository.GetAll().Select(StudentMapper.ToModel).ToList().AsReadOnly();
    }

    public StudentModel GetById(int id)
    {
        var entity = _repository.GetById(id);
        if (entity is null)
        {
            throw new EntityNotFoundException($"Студента з ID={id} не знайдено.");
        }

        return StudentMapper.ToModel(entity);
    }

    public StudentModel GetByStudentId(string studentId)
    {
        _validator.ValidateStudentId(studentId);
        var entity = _repository.GetByStudentId(studentId);
        if (entity is null)
        {
            throw new EntityNotFoundException($"Студента зі студентським квитком '{studentId}' не знайдено.");
        }

        return StudentMapper.ToModel(entity);
    }

    public void Add(StudentModel student)
    {
        ArgumentNullException.ThrowIfNull(student);
        ValidateModel(student);

        var existing = _repository.GetByStudentId(student.StudentId);
        if (existing is not null)
        {
            throw new DuplicateEntityException(
                $"Студент зі студентським квитком '{student.StudentId}' вже зареєстрований у системі.");
        }

        var entity = StudentMapper.ToEntity(student);
        _repository.Add(entity);
        student.Id = entity.Id;
    }

    public void Update(StudentModel student)
    {
        ArgumentNullException.ThrowIfNull(student);
        ValidateModel(student);

        var existing = _repository.GetById(student.Id);
        if (existing is null)
        {
            throw new EntityNotFoundException($"Неможливо оновити: студента з ID={student.Id} не знайдено.");
        }

        // Check if another student has the same StudentId
        var duplicate = _repository.GetByStudentId(student.StudentId);
        if (duplicate is not null && duplicate.Id != student.Id)
        {
            throw new DuplicateEntityException(
                $"Інший студент уже має студентський квиток '{student.StudentId}'.");
        }

        var entity = StudentMapper.ToEntity(student);
        _repository.Update(entity);
    }

    public void Delete(int id)
    {
        if (!_repository.Delete(id))
        {
            throw new EntityNotFoundException($"Неможливо видалити: студента з ID={id} не знайдено.");
        }
    }

    public double CalculateFirstCourseNonLocalPercentage(string localCity = "Київ")
    {
        _validator.ValidateCity(localCity);
        var allStudents = _repository.GetAll();

        var firstCourse = allStudents.Where(s => s.Course == 1).ToList();
        if (firstCourse.Count == 0)
        {
            return 0.0;
        }

        int nonLocalCount = firstCourse.Count(s =>
            !string.IsNullOrWhiteSpace(s.OriginCity) &&
            !s.OriginCity.Trim().Equals(localCity.Trim(), StringComparison.OrdinalIgnoreCase));

        double percentage = (double)nonLocalCount / firstCourse.Count * 100.0;
        return Math.Round(percentage, 2);
    }

    public IReadOnlyList<StudentModel> GetFirstCourseNonLocalStudents(string localCity = "Київ")
    {
        _validator.ValidateCity(localCity);
        var allStudents = _repository.GetAll();

        return allStudents
            .Where(s => s.Course == 1 &&
                        !string.IsNullOrWhiteSpace(s.OriginCity) &&
                        !s.OriginCity.Trim().Equals(localCity.Trim(), StringComparison.OrdinalIgnoreCase))
            .Select(StudentMapper.ToModel)
            .ToList()
            .AsReadOnly();
    }

    public void SaveToFile(string filePath, string format = "JSON")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        var provider = ResolveProvider(format);

        try
        {
            _repository.Save(filePath, provider);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка збереження даних студентів у файл '{filePath}': {ex.Message}", ex);
        }
    }

    public void LoadFromFile(string filePath, string format = "JSON")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        var provider = ResolveProvider(format);

        try
        {
            _repository.Load(filePath, provider);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка завантаження даних студентів з файлу '{filePath}': {ex.Message}", ex);
        }
    }

    public void Clear()
    {
        _repository.Clear();
    }

    private void ValidateModel(StudentModel student)
    {
        _validator.ValidateName(student.FirstName, nameof(student.FirstName));
        _validator.ValidateName(student.LastName, nameof(student.LastName));
        _validator.ValidateCourse(student.Course);
        _validator.ValidateStudentId(student.StudentId);
        _validator.ValidateCity(student.OriginCity);
        _validator.ValidatePassport(student.Passport);
    }

    private IDataProvider<StudentEntity> ResolveProvider(string format)
    {
        var provider = _providers.FirstOrDefault(p =>
            p.FormatName.Contains(format, StringComparison.OrdinalIgnoreCase));

        if (provider is null)
        {
            throw new ValidationException(
                $"Невідомий формат серіалізації: '{format}'. Доступні: {string.Join(", ", _providers.Select(p => p.FormatName))}.");
        }

        return provider;
    }
}
