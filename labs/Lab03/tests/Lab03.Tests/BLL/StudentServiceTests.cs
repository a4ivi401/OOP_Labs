using Lab03.BLL.Exceptions;
using Lab03.BLL.Models;
using Lab03.BLL.Services;
using Lab03.DAL.Context;
using Lab03.DAL.Entities;
using Lab03.DAL.Providers;

namespace Lab03.Tests.BLL;

public class StudentServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _filePath;
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "StudentServiceTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        _filePath = Path.Combine(_tempDir, "students.json");

        EntityContext<StudentEntity[]> context = new(new JsonDataProvider<StudentEntity[]>());
        _service = new StudentService(context);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Fact]
    public void Add_ShouldPersistStudentAndReturnIt()
    {
        StudentModel student = new("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456");

        _service.Add(_filePath, student);
        StudentModel[] all = _service.GetAll(_filePath);

        Assert.Single(all);
        Assert.Equal("KB112233", all[0].StudentId);
    }

    [Fact]
    public void Add_DuplicateStudentId_ShouldThrowDuplicateEntityException()
    {
        StudentModel student1 = new("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456");
        StudentModel student2 = new("2", "Іван", "Франко", 1, "KB112233", "Львів", "АН654321");

        _service.Add(_filePath, student1);

        Assert.Throws<DuplicateEntityException>(() => _service.Add(_filePath, student2));
    }

    [Fact]
    public void Remove_ExistingStudent_ShouldRemoveAndReturnStudent()
    {
        StudentModel student = new("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456");
        _service.Add(_filePath, student);

        StudentModel removed = _service.Remove(_filePath, "KB112233");

        Assert.Equal("KB112233", removed.StudentId);
        Assert.Empty(_service.GetAll(_filePath));
    }

    [Fact]
    public void Remove_NonExistentStudent_ShouldThrowEntityNotFoundException()
    {
        Assert.Throws<EntityNotFoundException>(() => _service.Remove(_filePath, "KB999999"));
    }

    [Fact]
    public void FindById_ShouldReturnStudent_WhenFound()
    {
        StudentModel s1 = new("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456");
        StudentModel s2 = new("2", "Леся", "Українка", 2, "KB223344", "Новоград", "МЕ654321");

        _service.Add(_filePath, s1);
        _service.Add(_filePath, s2);

        StudentModel? found = _service.FindById(_filePath, "KB223344");
        Assert.NotNull(found);
        Assert.Equal("Леся", found.FirstName);

        StudentModel? byId = _service.FindById(_filePath, "1");
        Assert.NotNull(byId);
        Assert.Equal("Тарас", byId.FirstName);
    }

    [Fact]
    public void CalculateFirstCourseNonLocalPercentage_ShouldCalculateAccurately()
    {
        StudentModel[] students = new StudentModel[]
        {
            new("1", "Студент1", "Прізвище1", 1, "KB111111", "Одеса", "СН111111"),   // 1 курс, не Київ
            new("2", "Студент2", "Прізвище2", 1, "KB222222", "Львів", "СН222222"),   // 1 курс, не Київ
            new("3", "Студент3", "Прізвище3", 1, "KB333333", "Харків", "СН333333"),  // 1 курс, не Київ
            new("4", "Студент4", "Прізвище4", 1, "KB444444", "Київ", "СН444444"),    // 1 курс, Київ
            new("5", "Студент5", "Прізвище5", 2, "KB555555", "Одеса", "СН555555"),   // 2 курс (ігнорується)
            new("6", "Студент6", "Прізвище6", 3, "KB666666", "Київ", "СН666666")     // 3 курс (ігнорується)
        };

        // Всього на 1 курсі: 4 студенти. Не з Києва: 3 студенти. Відсоток: 3 / 4 * 100 = 75.0%
        double percentage = _service.CalculateFirstCourseNonLocalPercentage(students, "Київ");
        Assert.Equal(75.0, percentage);
    }

    [Fact]
    public void CalculateFirstCourseNonLocalPercentage_ZeroFirstCourseStudents_ShouldReturnZero()
    {
        StudentModel[] students = new StudentModel[]
        {
            new("1", "Студент1", "Прізвище1", 2, "KB111111", "Одеса", "СН111111"),
            new("2", "Студент2", "Прізвище2", 3, "KB222222", "Львів", "СН222222")
        };

        double percentage = _service.CalculateFirstCourseNonLocalPercentage(students, "Київ");
        Assert.Equal(0.0, percentage);
    }

    [Fact]
    public void CalculateFirstCourseNonLocalPercentage_AllLocal_ShouldReturnZero()
    {
        StudentModel[] students = new StudentModel[]
        {
            new("1", "Студент1", "Прізвище1", 1, "KB111111", "Київ", "СН111111"),
            new("2", "Студент2", "Прізвище2", 1, "KB222222", "Київ", "СН222222")
        };

        double percentage = _service.CalculateFirstCourseNonLocalPercentage(students, "Київ");
        Assert.Equal(0.0, percentage);
    }

    [Fact]
    public void GetFirstCourseNonLocalStudents_ShouldReturnOnlyMatchingStudents()
    {
        StudentModel[] students = new StudentModel[]
        {
            new("1", "Студент1", "Прізвище1", 1, "KB111111", "Одеса", "СН111111"),
            new("2", "Студент2", "Прізвище2", 1, "KB222222", "Київ", "СН222222"),
            new("3", "Студент3", "Прізвище3", 2, "KB333333", "Львів", "СН333333"),
            new("4", "Студент4", "Прізвище4", 1, "KB444444", "Полтава", "СН444444")
        };

        StudentModel[] result = _service.GetFirstCourseNonLocalStudents(students, "Київ");

        Assert.Equal(2, result.Length);
        Assert.Contains(result, s => s.StudentId == "KB111111");
        Assert.Contains(result, s => s.StudentId == "KB444444");
    }
}
