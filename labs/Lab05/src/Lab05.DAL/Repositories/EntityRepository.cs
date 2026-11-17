using Lab05.DAL.Entities;
using Lab05.DAL.Providers;

namespace Lab05.DAL.Repositories;

public class EntityRepository<T> : IRepository<T> where T : PersonEntity
{
    private readonly List<T> _items = [];
    private readonly IDataProvider<T> _defaultProvider;

    public EntityRepository(IDataProvider<T> defaultProvider)
    {
        _defaultProvider = defaultProvider ?? throw new ArgumentNullException(nameof(defaultProvider));
    }

    public int Count => _items.Count;

    public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public void Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (entity.Id <= 0)
        {
            entity.Id = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1;
        }

        if (_items.Any(x => x.Id == entity.Id))
        {
            throw new InvalidOperationException($"Сутність з ID={entity.Id} вже існує у сховищі.");
        }

        _items.Add(entity);
    }

    public bool Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        int index = _items.FindIndex(x => x.Id == entity.Id);
        if (index == -1)
        {
            return false;
        }

        _items[index] = entity;
        return true;
    }

    public bool Delete(int id)
    {
        int index = _items.FindIndex(x => x.Id == id);
        if (index == -1)
        {
            return false;
        }

        _items.RemoveAt(index);
        return true;
    }

    public void Clear()
    {
        _items.Clear();
    }

    public void Save(string filePath, IDataProvider<T>? provider = null)
    {
        var activeProvider = provider ?? _defaultProvider;
        activeProvider.SaveToFile(filePath, _items);
    }

    public void Load(string filePath, IDataProvider<T>? provider = null)
    {
        var activeProvider = provider ?? _defaultProvider;
        var loaded = activeProvider.LoadFromFile(filePath);
        _items.Clear();
        _items.AddRange(loaded);
    }
}
