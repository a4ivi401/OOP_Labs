using Lab05.DAL.Entities;
using Lab05.DAL.Providers;

namespace Lab05.DAL.Repositories;

public interface IRepository<T> where T : PersonEntity
{
    IReadOnlyList<T> GetAll();
    T? GetById(int id);
    void Add(T entity);
    bool Update(T entity);
    bool Delete(int id);
    int Count { get; }
    void Clear();
    void Save(string filePath, IDataProvider<T>? provider = null);
    void Load(string filePath, IDataProvider<T>? provider = null);
}
