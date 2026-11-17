using Lab05.BLL.Models;

namespace Lab05.BLL.Interfaces;

public interface IStudentService
{
    IReadOnlyList<StudentModel> GetAll();
    StudentModel GetById(int id);
    StudentModel GetByStudentId(string studentId);
    void Add(StudentModel student);
    void Update(StudentModel student);
    void Delete(int id);

    /// <summary>
    /// Варіант 2: Обчислити відсоток студентів 1-го курсу, які приїхали з інших міст.
    /// </summary>
    double CalculateFirstCourseNonLocalPercentage(string localCity = "Київ");

    /// <summary>
    /// Варіант 2: Отримати дані студентів 1-го курсу, які приїхали з інших міст.
    /// </summary>
    IReadOnlyList<StudentModel> GetFirstCourseNonLocalStudents(string localCity = "Київ");

    void SaveToFile(string filePath, string format = "JSON");
    void LoadFromFile(string filePath, string format = "JSON");
    void Clear();
}
