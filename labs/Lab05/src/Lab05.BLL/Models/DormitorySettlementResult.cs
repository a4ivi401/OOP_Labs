namespace Lab05.BLL.Models;

public class DormitorySettlementResult
{
    public bool IsSuccess { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentFullName { get; set; } = string.Empty;
    public int? DormitoryNumber { get; set; }
    public int? RoomNumber { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? SettlementDate { get; set; }

    public static DormitorySettlementResult Success(
        string studentId,
        string fullName,
        int dormitoryNumber,
        int roomNumber,
        DateTime date) => new()
    {
        IsSuccess = true,
        StudentId = studentId,
        StudentFullName = fullName,
        DormitoryNumber = dormitoryNumber,
        RoomNumber = roomNumber,
        SettlementDate = date,
        Message = $"Студента {fullName} (квиток: {studentId}) успішно поселено в гуртожиток №{dormitoryNumber}, кімната {roomNumber}."
    };

    public static DormitorySettlementResult Failure(string studentId, string fullName, string reason) => new()
    {
        IsSuccess = false,
        StudentId = studentId,
        StudentFullName = fullName,
        Message = $"Поселення відхилено для студента {fullName} ({studentId}): {reason}"
    };
}
