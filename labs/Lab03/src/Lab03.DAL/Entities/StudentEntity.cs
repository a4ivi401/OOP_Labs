namespace Lab03.DAL.Entities;

public class StudentEntity : PersonEntity
{
    public int Course { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public string Passport { get; set; } = string.Empty;

    public StudentEntity()
    {
    }

    public StudentEntity(string id, string firstName, string lastName, int course, string studentId, string originCity, string passport)
        : base(id, firstName, lastName)
    {
        Course = course;
        StudentId = studentId;
        OriginCity = originCity;
        Passport = passport;
    }
}
