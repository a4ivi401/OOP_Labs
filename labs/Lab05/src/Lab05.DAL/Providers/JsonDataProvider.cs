using System.Text.Encodings.Web;
using System.Text.Json;

namespace Lab05.DAL.Providers;

public class JsonDataProvider<T> : IDataProvider<T>
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true
    };

    public string FormatName => "JSON";

    public void SaveToFile(string filePath, IEnumerable<T> items)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(items);

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, items, SerializerOptions);
    }

    public List<T> LoadFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл даних не знайдено: '{filePath}'", filePath);
        }

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var result = JsonSerializer.Deserialize<List<T>>(stream, SerializerOptions);
        return result ?? [];
    }
}
