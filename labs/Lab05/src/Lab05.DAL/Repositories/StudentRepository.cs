using Lab05.DAL.Entities;
using Lab05.DAL.Providers;

namespace Lab05.DAL.Repositories;

public interface IStudentRepository : IRepository<StudentEntity>
{
    StudentEntity? GetByStudentId(string studentId);
}

public class StudentRepository : EntityRepository<StudentEntity>, IStudentRepository
{
    public StudentRepository(IDataProvider<StudentEntity> defaultProvider) : base(defaultProvider)
    {
    }

    public StudentEntity? GetByStudentId(string studentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(studentId);
        return GetAll().FirstOrDefault(s => s.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase));
    }
}
