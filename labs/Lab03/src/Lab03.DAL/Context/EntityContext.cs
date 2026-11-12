using Lab03.DAL.Providers;

namespace Lab03.DAL.Context;

public class EntityContext<T> : IEntityContext<T>
{
    public IDataProvider<T> DataProvider { get; set; }

    public EntityContext(IDataProvider<T> dataProvider)
    {
        DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    }

    public void CreateOrOpenFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (!File.Exists(filePath))
        {
            using FileStream stream = File.Create(filePath);
        }
    }

    public void Save(string filePath, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        DataProvider.Serialize(filePath, data);
    }

    public T Load(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        return DataProvider.Deserialize(filePath);
    }

    public bool DeleteFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }

    public bool FileExists(string filePath)
    {
        return !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);
    }

    public long GetFileSize(string filePath)
    {
        if (FileExists(filePath))
        {
            return new FileInfo(filePath).Length;
        }
        return 0;
    }
}
