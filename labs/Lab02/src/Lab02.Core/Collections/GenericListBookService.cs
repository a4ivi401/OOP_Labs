using Lab02.Core.Models;

namespace Lab02.Core.Collections;

public class GenericListBookService : IBookCollectionOperations
{
    private readonly List<Book> _items;

    public GenericListBookService(Book[] initialItems)
    {
        _items = new List<Book>(initialItems);
    }

    public string CollectionTypeName => "Узагальнена колекція (List<Book>)";

    public int Count => _items.Count;

    public void Add(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        _items.Add(book);
    }

    public bool Remove(string serialNumber)
    {
        int index = _items.FindIndex(b => string.Equals(b.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase));
        if (index == -1)
        {
            return false;
        }

        _items.RemoveAt(index);
        return true;
    }

    public bool Update(string serialNumber, string newTitle, decimal newPrice, int newCopiesCount)
    {
        Book? book = _items.Find(b => string.Equals(b.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase));
        if (book is null)
        {
            return false;
        }

        book.Title = newTitle;
        book.Price = newPrice;
        book.CopiesCount = newCopiesCount;
        return true;
    }

    public Book? Find(string serialNumber)
    {
        return _items.Find(b => string.Equals(b.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase));
    }

    public Book[] GetAll()
    {
        return _items.ToArray();
    }
}
