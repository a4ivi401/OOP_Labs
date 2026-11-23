namespace Lab05.DAL.Entities;

public class StudentEntity : PersonEntity
{
    public int Course { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public string Passport { get; set; } = string.Empty;
    public bool IsSettledInDormitory { get; set; }
    public int? DormitoryNumber { get; set; }
    public int? RoomNumber { get; set; }
    public DateTime? SettlementDate { get; set; }
}
