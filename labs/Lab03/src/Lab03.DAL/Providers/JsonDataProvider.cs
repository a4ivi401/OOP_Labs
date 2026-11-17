using System.Text.Encodings.Web;
using System.Text.Json;

namespace Lab03.DAL.Providers;

public class JsonDataProvider<T> : IDataProvider<T>
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true
    };

    public string FormatName => "JSON";
    public string DefaultExtension => ".json";

    public void Serialize(string filePath, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        EnsureDirectory(filePath);

        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, data, _options);
    }

    public T Deserialize(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не знайдено: {filePath}", filePath);
        }

        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        T? result = JsonSerializer.Deserialize<T>(stream, _options);
        return result ?? throw new InvalidDataException($"Не вдалося десеріалізувати дані з файлу {filePath}");
    }

    private static void EnsureDirectory(string filePath)
    {
        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}
