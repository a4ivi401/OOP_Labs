using Lab02.Core.Comparers;
using Lab02.Core.Data;
using Lab02.Core.Models;
using Lab02.Core.Services;

namespace Lab02.Tests.Comparers;

public class ComparersAndSortingTests
{
    [Fact]
    public void SortByNaturalOrder_SortsByTitleAscending()
    {
        var books = BookDataSample.CreateInitialBooks();

        var sorted = BookSortingService.SortByNaturalOrder(books);

        Assert.Equal("Захар Беркут", sorted[0].Title);
        Assert.Equal("Кобзар", sorted[1].Title);
        Assert.Equal("Місто", sorted[2].Title);
        Assert.Equal("Тіні забутих предків", sorted[3].Title);
    }

    [Fact]
    public void SortByPrice_SortsByPriceAscending()
    {
        var books = BookDataSample.CreateInitialBooks();

        var sorted = BookSortingService.SortByPrice(books);

        Assert.Equal(195.00m, sorted[0].Price);
        Assert.Equal(220.00m, sorted[1].Price);
        Assert.Equal(280.00m, sorted[2].Price);
        Assert.Equal(350.00m, sorted[3].Price);
    }

    [Fact]
    public void SortByYear_SortsByPublicationYearAscending()
    {
        var books = BookDataSample.CreateInitialBooks();

        var sorted = BookSortingService.SortByYear(books);

        Assert.Equal(1840, sorted[0].PublicationYear);
        Assert.Equal(1883, sorted[1].PublicationYear);
        Assert.Equal(1911, sorted[2].PublicationYear);
        Assert.Equal(1928, sorted[3].PublicationYear);
    }

    [Fact]
    public void SortByTotalCost_SortsByCirculationCostAscending()
    {
        var books = BookDataSample.CreateInitialBooks();

        var sorted = BookSortingService.SortByTotalCost(books);

        Assert.True(sorted[0].TotalCirculationCost <= sorted[1].TotalCirculationCost);
        Assert.True(sorted[1].TotalCirculationCost <= sorted[2].TotalCirculationCost);
        Assert.True(sorted[2].TotalCirculationCost <= sorted[3].TotalCirculationCost);
    }

    [Fact]
    public void SortUsingTree_WithPriceComparer_ReturnsSortedOrder()
    {
        var books = BookDataSample.CreateInitialBooks();

        var sorted = BookSortingService.SortUsingTree(books, new BookPriceComparer());

        Assert.Equal(195.00m, sorted[0].Price);
        Assert.Equal(350.00m, sorted[3].Price);
    }
}
