using Lab02.Core.Models;

namespace Lab02.Tests.Models;

public class BookTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesPropertiesCorrectly()
    {
        var book = new Book("978-966-03-8201-5", "Кобзар", 1840, 350.50m, 5000);

        Assert.Equal("978-966-03-8201-5", book.SerialNumber);
        Assert.Equal("Кобзар", book.Title);
        Assert.Equal(1840, book.PublicationYear);
        Assert.Equal(350.50m, book.Price);
        Assert.Equal(5000, book.CopiesCount);
        Assert.Equal(1752500.00m, book.TotalCirculationCost);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SerialNumber_Invalid_ThrowsArgumentException(string invalidSerial)
    {
        Assert.Throws<ArgumentException>(() => new Book(invalidSerial, "Кобзар", 1840, 100, 10));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Title_Invalid_ThrowsArgumentException(string invalidTitle)
    {
        Assert.Throws<ArgumentException>(() => new Book("123", invalidTitle, 1840, 100, 10));
    }

    [Theory]
    [InlineData(1400)]
    [InlineData(3000)]
    public void PublicationYear_Invalid_ThrowsArgumentOutOfRangeException(int invalidYear)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Book("123", "Тест", invalidYear, 100, 10));
    }

    [Fact]
    public void Price_Negative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Book("123", "Тест", 2000, -5m, 10));
    }

    [Fact]
    public void CopiesCount_Negative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Book("123", "Тест", 2000, 100m, -1));
    }

    [Fact]
    public void IncreasePrice_ValidPercentage_IncreasesPriceCorrectly()
    {
        var book = new Book("123", "Кобзар", 1840, 200.00m, 1000);

        book.IncreasePrice(10);

        Assert.Equal(220.00m, book.Price);
        Assert.Equal(220000.00m, book.TotalCirculationCost);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void IncreasePrice_InvalidPercentage_ThrowsArgumentOutOfRangeException(decimal invalidPercentage)
    {
        var book = new Book("123", "Кобзар", 1840, 200.00m, 1000);

        Assert.Throws<ArgumentOutOfRangeException>(() => book.IncreasePrice(invalidPercentage));
    }

    [Fact]
    public void CompareTo_Generic_ComparesByTitleCorrectly()
    {
        var bookA = new Book("1", "Арсенал", 2000, 100, 10);
        var bookB = new Book("2", "Борисфен", 2000, 100, 10);

        Assert.True(bookA.CompareTo(bookB) < 0);
        Assert.True(bookB.CompareTo(bookA) > 0);
        Assert.Equal(0, bookA.CompareTo(new Book("1", "Арсенал", 2000, 100, 10)));
    }

    [Fact]
    public void Equals_SameSerialNumber_ReturnsTrue()
    {
        var book1 = new Book("ISBN-100", "Назва 1", 2001, 100, 10);
        var book2 = new Book("ISBN-100", "Інша Назва", 2010, 500, 50);

        Assert.Equal(book1, book2);
    }
}
