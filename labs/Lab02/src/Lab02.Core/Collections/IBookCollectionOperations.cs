using Lab02.Core.Models;

namespace Lab02.Core.Collections;

public interface IBookCollectionOperations
{
    string CollectionTypeName { get; }
    int Count { get; }
    void Add(Book book);
    bool Remove(string serialNumber);
    bool Update(string serialNumber, string newTitle, decimal newPrice, int newCopiesCount);
    Book? Find(string serialNumber);
    Book[] GetAll();
}
