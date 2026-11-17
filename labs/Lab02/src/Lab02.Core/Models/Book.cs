namespace Lab02.Core.Models;

public class Book : IComparable<Book>, IComparable, IEquatable<Book>
{
    private string _serialNumber = string.Empty;
    private string _title = string.Empty;
    private int _publicationYear;
    private decimal _price;
    private int _copiesCount;

    public Book(string serialNumber, string title, int publicationYear, decimal price, int copiesCount)
    {
        SerialNumber = serialNumber;
        Title = title;
        PublicationYear = publicationYear;
        Price = price;
        CopiesCount = copiesCount;
    }

    public string SerialNumber
    {
        get => _serialNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Серійний номер не може бути порожнім.", nameof(value));
            }

            _serialNumber = value.Trim();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Назва книги не може бути порожньою.", nameof(value));
            }

            _title = value.Trim();
        }
    }

    public int PublicationYear
    {
        get => _publicationYear;
        set
        {
            if (value < 1450 || value > DateTime.Now.Year + 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Рік видання має бути в межах від 1450 до наступного року.");
            }

            _publicationYear = value;
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Вартість примірника не може бути від'ємною.");
            }

            _price = Math.Round(value, 2);
        }
    }

    public int CopiesCount
    {
        get => _copiesCount;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Кількість примірників не може бути від'ємною.");
            }

            _copiesCount = value;
        }
    }

    public decimal TotalCirculationCost => Price * CopiesCount;

    public void IncreasePrice(decimal percentage)
    {
        if (percentage <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage), "Відсоток збільшення має бути більше нуля.");
        }

        Price += Price * (percentage / 100m);
    }

    public int CompareTo(Book? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        int titleComparison = string.Compare(Title, other.Title, StringComparison.OrdinalIgnoreCase);
        if (titleComparison != 0)
        {
            return titleComparison;
        }

        int yearComparison = PublicationYear.CompareTo(other.PublicationYear);
        if (yearComparison != 0)
        {
            return yearComparison;
        }

        return string.Compare(SerialNumber, other.SerialNumber, StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is Book otherBook)
        {
            return CompareTo(otherBook);
        }

        throw new ArgumentException("Об'єкт повинен мати тип Book.", nameof(obj));
    }

    public bool Equals(Book? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(SerialNumber, other.SerialNumber, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Book);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(SerialNumber);
    }

    public override string ToString()
    {
        return $"[{SerialNumber}] \"{Title}\" ({PublicationYear} р.) - {Price:F2} грн, тираж: {CopiesCount} шт. (разом: {TotalCirculationCost:F2} грн)";
    }
}
