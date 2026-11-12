namespace Lab03.DAL.Providers;

public interface IDataProvider<T>
{
    string FormatName { get; }
    string DefaultExtension { get; }
    void Serialize(string filePath, T data);
    T Deserialize(string filePath);
}
