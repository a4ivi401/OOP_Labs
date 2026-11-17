using System.Xml;
using System.Xml.Serialization;

namespace Lab03.DAL.Providers;

public class XmlDataProvider<T> : IDataProvider<T>
{
    private static readonly XmlSerializer Serializer = new(typeof(T));

    public string FormatName => "XML";
    public string DefaultExtension => ".xml";

    public void Serialize(string filePath, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        EnsureDirectory(filePath);

        XmlWriterSettings settings = new()
        {
            Indent = true,
            OmitXmlDeclaration = false,
            Encoding = System.Text.Encoding.UTF8
        };

        using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using XmlWriter writer = XmlWriter.Create(stream, settings);
        Serializer.Serialize(writer, data);
    }

    public T Deserialize(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не знайдено: {filePath}", filePath);
        }

        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using XmlReader reader = XmlReader.Create(stream);
        object? result = Serializer.Deserialize(reader);
        return result is T typed ? typed : throw new InvalidDataException($"Не вдалося десеріалізувати XML з файлу {filePath}");
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
