using Lab03.BLL.Exceptions;
using Lab03.BLL.Models;

namespace Lab03.Tests.BLL;

public class BookModelTests
{
    [Fact]
    public void Constructor_ValidArguments_ShouldInitializeProperties()
    {
        BookModel book = new("978-0132350884", "Clean Code", 2008, 450.00m, 1500);

        Assert.Equal("978-0132350884", book.SerialNumber);
        Assert.Equal("Clean Code", book.Title);
        Assert.Equal(2008, book.PublicationYear);
        Assert.Equal(450.00m, book.Price);
        Assert.Equal(1500, book.CopiesCount);
        Assert.Equal(675000.00m, book.TotalCirculationCost);
    }

    [Fact]
    public void IncreasePrice_ValidPercentage_ShouldIncreaseCorrectly()
    {
        BookModel book = new("978-0132350884", "Clean Code", 2008, 100.00m, 10);

        book.IncreasePrice(15); // +15% -> 115.00

        Assert.Equal(115.00m, book.Price);
        Assert.Equal(1150.00m, book.TotalCirculationCost);
    }

    [Fact]
    public void IncreasePrice_ZeroOrNegativePercentage_ShouldThrowValidationException()
    {
        BookModel book = new("978-0132350884", "Clean Code", 2008, 100.00m, 10);

        Assert.Throws<ValidationException>(() => book.IncreasePrice(0));
        Assert.Throws<ValidationException>(() => book.IncreasePrice(-10));
    }

    [Fact]
    public void Setters_InvalidData_ShouldThrowValidationException()
    {
        Assert.Throws<ValidationException>(() => new BookModel("", "Title", 2020, 100m, 10));
        Assert.Throws<ValidationException>(() => new BookModel("ISBN", "", 2020, 100m, 10));
        Assert.Throws<ValidationException>(() => new BookModel("ISBN", "Title", 1200, 100m, 10));
        Assert.Throws<ValidationException>(() => new BookModel("ISBN", "Title", 2020, -50m, 10));
        Assert.Throws<ValidationException>(() => new BookModel("ISBN", "Title", 2020, 100m, -5));
    }
}
