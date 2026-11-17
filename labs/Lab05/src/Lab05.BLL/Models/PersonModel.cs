namespace Lab05.BLL.Models;

public abstract class PersonModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{LastName} {FirstName}".Trim();

    public override string ToString() => $"[ID:{Id}] {FullName}";
}
