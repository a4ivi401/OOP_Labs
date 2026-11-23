namespace Lab05.DAL.Entities;

public class AcrobatEntity : PersonEntity
{
    public string CircusOrTroupe { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public bool SafetyCertificationValid { get; set; }
    public int PerformancesCount { get; set; }
}
