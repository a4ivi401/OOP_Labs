namespace Lab05.DAL.Providers;

public interface IDataProvider<T>
{
    string FormatName { get; }
    void SaveToFile(string filePath, IEnumerable<T> items);
    List<T> LoadFromFile(string filePath);
}
