using Lab02.Core.Models;

namespace Lab02.Core.Comparers;

public class BookPriceComparer : IComparer<Book>
{
    public int Compare(Book? x, Book? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        int priceComparison = x.Price.CompareTo(y.Price);
        return priceComparison != 0 ? priceComparison : x.CompareTo(y);
    }
}

public class BookTotalCostComparer : IComparer<Book>
{
    public int Compare(Book? x, Book? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        int costComparison = x.TotalCirculationCost.CompareTo(y.TotalCirculationCost);
        return costComparison != 0 ? costComparison : x.CompareTo(y);
    }
}

public class BookYearComparer : IComparer<Book>
{
    public int Compare(Book? x, Book? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        int yearComparison = x.PublicationYear.CompareTo(y.PublicationYear);
        return yearComparison != 0 ? yearComparison : x.CompareTo(y);
    }
}

public class BookTitleComparer : IComparer<Book>
{
    public int Compare(Book? x, Book? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
    }
}
