using Lab05.BLL.Models;

namespace Lab05.BLL.Interfaces;

public interface IDormitoryService
{
    /// <summary>
    /// Варіант 2: Поселення в гуртожиток при умові приїзду з іншого міста.
    /// </summary>
    DormitorySettlementResult SettleInDormitory(string studentId, int dormitoryNumber, int roomNumber, string localCity = "Київ");

    bool EvictFromDormitory(string studentId);
    IReadOnlyList<StudentModel> GetSettledStudents();
    bool IsEligibleForDormitory(string studentId, string localCity, out string reason);
}
