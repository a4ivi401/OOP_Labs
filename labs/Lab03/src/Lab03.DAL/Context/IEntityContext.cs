using Lab03.DAL.Providers;

namespace Lab03.DAL.Context;

public interface IEntityContext<T>
{
    IDataProvider<T> DataProvider { get; set; }
    void CreateOrOpenFile(string filePath);
    void Save(string filePath, T data);
    T Load(string filePath);
    bool DeleteFile(string filePath);
    bool FileExists(string filePath);
    long GetFileSize(string filePath);
}
