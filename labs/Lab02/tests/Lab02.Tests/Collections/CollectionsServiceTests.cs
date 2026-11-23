using Lab02.Core.Collections;
using Lab02.Core.Data;
using Lab02.Core.Models;

namespace Lab02.Tests.Collections;

public class CollectionsServiceTests
{
    public static IEnumerable<object[]> GetCollectionServices()
    {
        yield return new object[] { new ArrayBookService(BookDataSample.CreateInitialBooks()) };
        yield return new object[] { new ArrayListBookService(BookDataSample.CreateInitialBooks()) };
        yield return new object[] { new GenericListBookService(BookDataSample.CreateInitialBooks()) };
    }

    [Theory]
    [MemberData(nameof(GetCollectionServices))]
    public void InitialCount_IsFour(IBookCollectionOperations service)
    {
        Assert.Equal(4, service.Count);
        Assert.Equal(4, service.GetAll().Length);
    }

    [Theory]
    [MemberData(nameof(GetCollectionServices))]
    public void Add_IncreasesCountAndItemIsPresent(IBookCollectionOperations service)
    {
        var newBook = new Book("978-000", "Нова книга", 2020, 150m, 500);

        service.Add(newBook);

        Assert.Equal(5, service.Count);
        Assert.NotNull(service.Find("978-000"));
        Assert.Equal("Нова книга", service.Find("978-000")?.Title);
    }

    [Theory]
    [MemberData(nameof(GetCollectionServices))]
    public void Find_ExistingAndNonExisting(IBookCollectionOperations service)
    {
        Book? found = service.Find("978-966-03-8201-5");
        Assert.NotNull(found);
        Assert.Equal("Кобзар", found.Title);

        Book? notFound = service.Find("non-existent");
        Assert.Null(notFound);
    }

    [Theory]
    [MemberData(nameof(GetCollectionServices))]
    public void Update_ExistingItem_ModifiesFields(IBookCollectionOperations service)
    {
        bool updated = service.Update("978-966-03-8201-5", "Кобзар (Ювілейний)", 500m, 10000);

        Assert.True(updated);
        Book? modified = service.Find("978-966-03-8201-5");
        Assert.NotNull(modified);
        Assert.Equal("Кобзар (Ювілейний)", modified.Title);
        Assert.Equal(500m, modified.Price);
        Assert.Equal(10000, modified.CopiesCount);
    }

    [Theory]
    [MemberData(nameof(GetCollectionServices))]
    public void Update_NonExistingItem_ReturnsFalse(IBookCollectionOperations service)
    {
        bool updated = service.Update("unknown", "Тест", 100m, 10);
        Assert.False(updated);
    }

    [Theory]
    [MemberData(nameof(GetCollectionServices))]
    public void Remove_ExistingItem_DecreasesCount(IBookCollectionOperations service)
    {
        bool removed = service.Remove("978-966-03-8201-5");

        Assert.True(removed);
        Assert.Equal(3, service.Count);
        Assert.Null(service.Find("978-966-03-8201-5"));
    }

    [Theory]
    [MemberData(nameof(GetCollectionServices))]
    public void Remove_NonExistingItem_ReturnsFalse(IBookCollectionOperations service)
    {
        bool removed = service.Remove("non-existent");

        Assert.False(removed);
        Assert.Equal(4, service.Count);
    }
}
