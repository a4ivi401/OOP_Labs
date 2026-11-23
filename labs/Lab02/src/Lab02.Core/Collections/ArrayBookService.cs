using Lab02.Core.Models;

namespace Lab02.Core.Collections;

public class ArrayBookService : IBookCollectionOperations
{
    private Book[] _items;

    public ArrayBookService(Book[] initialItems)
    {
        _items = new Book[initialItems.Length];
        Array.Copy(initialItems, _items, initialItems.Length);
    }

    public string CollectionTypeName => "Фіксований масив (Book[])";

    public int Count => _items.Length;

    public void Add(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        var newArray = new Book[_items.Length + 1];
        Array.Copy(_items, newArray, _items.Length);
        newArray[^1] = book;
        _items = newArray;
    }

    public bool Remove(string serialNumber)
    {
        int index = FindIndex(serialNumber);
        if (index == -1)
        {
            return false;
        }

        var newArray = new Book[_items.Length - 1];
        Array.Copy(_items, 0, newArray, 0, index);
        Array.Copy(_items, index + 1, newArray, index, _items.Length - index - 1);
        _items = newArray;
        return true;
    }

    public bool Update(string serialNumber, string newTitle, decimal newPrice, int newCopiesCount)
    {
        int index = FindIndex(serialNumber);
        if (index == -1)
        {
            return false;
        }

        _items[index].Title = newTitle;
        _items[index].Price = newPrice;
        _items[index].CopiesCount = newCopiesCount;
        return true;
    }

    public Book? Find(string serialNumber)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (string.Equals(_items[i].SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase))
            {
                return _items[i];
            }
        }

        return null;
    }

    public Book[] GetAll()
    {
        var copy = new Book[_items.Length];
        Array.Copy(_items, copy, _items.Length);
        return copy;
    }

    private int FindIndex(string serialNumber)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (string.Equals(_items[i].SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }
}
