using System.Collections;
using Lab02.Core.Models;

namespace Lab02.Core.Collections;

public class ArrayListBookService : IBookCollectionOperations
{
    private readonly ArrayList _items = new();

    public ArrayListBookService(Book[] initialItems)
    {
        for (int i = 0; i < initialItems.Length; i++)
        {
            _items.Add(initialItems[i]);
        }
    }

    public string CollectionTypeName => "Неузагальнена колекція (ArrayList)";

    public int Count => _items.Count;

    public void Add(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        _items.Add(book);
    }

    public bool Remove(string serialNumber)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] is Book book && string.Equals(book.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase))
            {
                _items.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public bool Update(string serialNumber, string newTitle, decimal newPrice, int newCopiesCount)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] is Book book && string.Equals(book.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase))
            {
                book.Title = newTitle;
                book.Price = newPrice;
                book.CopiesCount = newCopiesCount;
                return true;
            }
        }

        return false;
    }

    public Book? Find(string serialNumber)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] is Book book && string.Equals(book.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase))
            {
                return book;
            }
        }

        return null;
    }

    public Book[] GetAll()
    {
        var result = new Book[_items.Count];
        for (int i = 0; i < _items.Count; i++)
        {
            result[i] = (Book)_items[i]!;
        }

        return result;
    }
}
