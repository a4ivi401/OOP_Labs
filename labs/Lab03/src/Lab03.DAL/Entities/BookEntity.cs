namespace Lab03.DAL.Entities;

public class BookEntity
{
    public string SerialNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public decimal Price { get; set; }
    public int CopiesCount { get; set; }

    public BookEntity()
    {
    }

    public BookEntity(string serialNumber, string title, int publicationYear, decimal price, int copiesCount)
    {
        SerialNumber = serialNumber;
        Title = title;
        PublicationYear = publicationYear;
        Price = price;
        CopiesCount = copiesCount;
    }
}
