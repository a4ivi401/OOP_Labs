using System.Xml.Serialization;

namespace Lab05.DAL.Providers;

public class XmlDataProvider<T> : IDataProvider<T>
{
    public string FormatName => "XML";

    public void SaveToFile(string filePath, IEnumerable<T> items)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(items);

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var list = items.ToList();
        var serializer = new XmlSerializer(typeof(List<T>));
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        serializer.Serialize(stream, list);
    }

    public List<T> LoadFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл даних не знайдено: '{filePath}'", filePath);
        }

        var serializer = new XmlSerializer(typeof(List<T>));
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var result = (List<T>?)serializer.Deserialize(stream);
        return result ?? [];
    }
}
