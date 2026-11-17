namespace Lab03.BLL.Models;

public class SerializationExperimentResult
{
    public string FormatName { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty; // "Array" vs "List"
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public double SerializationTimeMs { get; set; }
    public double DeserializationTimeMs { get; set; }
    public bool DataIntegrityVerified { get; set; }
    public int ItemCount { get; set; }
}
