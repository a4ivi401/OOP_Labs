using Lab03.BLL.Models;
using Lab03.BLL.Services;

namespace Lab03.Tests.BLL;

public class SerializationComparisonServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly SerializationComparisonService _service = new();

    public SerializationComparisonServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "ComparisonTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Fact]
    public void RunFullComparison_ShouldProduce8SuccessfulExperiments()
    {
        SerializationExperimentResult[] results = _service.RunFullComparison(_tempDir);

        Assert.Equal(8, results.Length); // 4 formats x 2 (Array and List)

        foreach (SerializationExperimentResult res in results)
        {
            Assert.True(res.DataIntegrityVerified, $"Формат {res.FormatName} ({res.TargetType}) не пройшов верифікацію цілісності.");
            Assert.True(res.FileSizeBytes > 0);
            Assert.Equal(4, res.ItemCount);
        }
    }
}
