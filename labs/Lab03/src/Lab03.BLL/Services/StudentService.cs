using Lab03.BLL.Exceptions;
using Lab03.BLL.Mappers;
using Lab03.BLL.Models;
using Lab03.DAL.Context;
using Lab03.DAL.Entities;

namespace Lab03.BLL.Services;

public class StudentService
{
    private readonly IEntityContext<StudentEntity[]> _context;

    public StudentService(IEntityContext<StudentEntity[]> context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public StudentModel[] GetAll(string filePath)
    {
        if (!_context.FileExists(filePath))
        {
            return Array.Empty<StudentModel>();
        }

        try
        {
            StudentEntity[] entities = _context.Load(filePath);
            return StudentMapper.ToModels(entities ?? Array.Empty<StudentEntity>());
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка завантаження студентів з файлу '{filePath}'", ex);
        }
    }

    public void SaveAll(string filePath, StudentModel[] students)
    {
        ArgumentNullException.ThrowIfNull(students);

        try
        {
            StudentEntity[] entities = StudentMapper.ToEntities(students);
            _context.Save(filePath, entities);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка збереження студентів у файл '{filePath}'", ex);
        }
    }

    public StudentModel Add(string filePath, StudentModel student)
    {
        ArgumentNullException.ThrowIfNull(student);

        StudentModel[] current = GetAll(filePath);
        for (int i = 0; i < current.Length; i++)
        {
            if (string.Equals(current[i].StudentId, student.StudentId, StringComparison.OrdinalIgnoreCase))
            {
                throw new DuplicateEntityException(nameof(StudentModel), student.StudentId);
            }
        }

        StudentModel[] updated = new StudentModel[current.Length + 1];
        Array.Copy(current, updated, current.Length);
        updated[current.Length] = student;

        SaveAll(filePath, updated);
        return student;
    }

    public StudentModel Remove(string filePath, string studentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studentId);

        StudentModel[] current = GetAll(filePath);
        int targetIndex = -1;

        for (int i = 0; i < current.Length; i++)
        {
            if (string.Equals(current[i].StudentId, studentId, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(current[i].Id, studentId, StringComparison.OrdinalIgnoreCase))
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1)
        {
            throw new EntityNotFoundException(nameof(StudentModel), studentId);
        }

        StudentModel removed = current[targetIndex];
        StudentModel[] updated = new StudentModel[current.Length - 1];

        int dest = 0;
        for (int i = 0; i < current.Length; i++)
        {
            if (i != targetIndex)
            {
                updated[dest++] = current[i];
            }
        }

        SaveAll(filePath, updated);
        return removed;
    }

    public StudentModel? FindById(string filePath, string studentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studentId);

        StudentModel[] current = GetAll(filePath);
        for (int i = 0; i < current.Length; i++)
        {
            if (string.Equals(current[i].StudentId, studentId, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(current[i].Id, studentId, StringComparison.OrdinalIgnoreCase))
            {
                return current[i];
            }
        }

        return null;
    }

    public double CalculateFirstCourseNonLocalPercentage(StudentModel[] students, string localCity = "Київ")
    {
        ArgumentNullException.ThrowIfNull(students);

        int firstCourseTotal = 0;
        int nonLocalCount = 0;

        for (int i = 0; i < students.Length; i++)
        {
            StudentModel s = students[i];
            if (s.Course == 1)
            {
                firstCourseTotal++;
                if (!string.Equals(s.OriginCity, localCity, StringComparison.OrdinalIgnoreCase))
                {
                    nonLocalCount++;
                }
            }
        }

        if (firstCourseTotal == 0)
        {
            return 0.0;
        }

        return Math.Round((double)nonLocalCount / firstCourseTotal * 100.0, 2);
    }

    public StudentModel[] GetFirstCourseNonLocalStudents(StudentModel[] students, string localCity = "Київ")
    {
        ArgumentNullException.ThrowIfNull(students);

        int count = 0;
        for (int i = 0; i < students.Length; i++)
        {
            StudentModel s = students[i];
            if (s.Course == 1 && !string.Equals(s.OriginCity, localCity, StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }
        }

        StudentModel[] result = new StudentModel[count];
        int index = 0;
        for (int i = 0; i < students.Length; i++)
        {
            StudentModel s = students[i];
            if (s.Course == 1 && !string.Equals(s.OriginCity, localCity, StringComparison.OrdinalIgnoreCase))
            {
                result[index++] = s;
            }
        }

        return result;
    }
}
