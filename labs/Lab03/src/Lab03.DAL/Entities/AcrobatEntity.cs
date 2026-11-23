namespace Lab03.DAL.Entities;

public class AcrobatEntity : PersonEntity
{
    public string CircusOrTroupeName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;

    public AcrobatEntity()
    {
    }

    public AcrobatEntity(string id, string firstName, string lastName, string circusOrTroupeName, string specialization)
        : base(id, firstName, lastName)
    {
        CircusOrTroupeName = circusOrTroupeName;
        Specialization = specialization;
    }
}
