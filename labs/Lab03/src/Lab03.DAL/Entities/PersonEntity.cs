using System.Xml.Serialization;

namespace Lab03.DAL.Entities;

[XmlInclude(typeof(StudentEntity))]
[XmlInclude(typeof(TaxiDriverEntity))]
[XmlInclude(typeof(AcrobatEntity))]
public abstract class PersonEntity
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    protected PersonEntity()
    {
    }

    protected PersonEntity(string id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
}
