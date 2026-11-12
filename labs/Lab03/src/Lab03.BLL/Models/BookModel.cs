using Lab03.BLL.Exceptions;

namespace Lab03.BLL.Models;

public class BookModel
{
    private string _serialNumber = string.Empty;
    private string _title = string.Empty;
    private int _publicationYear;
    private decimal _price;
    private int _copiesCount;

    public string SerialNumber
    {
        get => _serialNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(SerialNumber), "Серійний номер книги не може бути порожнім.");
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
                throw new ValidationException(nameof(Title), "Назва книги не може бути порожньою.");
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
                throw new ValidationException(nameof(PublicationYear), "Рік видання має бути між 1450 та наступним роком.");
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
                throw new ValidationException(nameof(Price), "Вартість примірника не може бути від'ємною.");
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
                throw new ValidationException(nameof(CopiesCount), "Кількість примірників не може бути від'ємною.");
            }
            _copiesCount = value;
        }
    }

    public decimal TotalCirculationCost => Price * CopiesCount;

    public BookModel(string serialNumber, string title, int publicationYear, decimal price, int copiesCount)
    {
        SerialNumber = serialNumber;
        Title = title;
        PublicationYear = publicationYear;
        Price = price;
        CopiesCount = copiesCount;
    }

    public void IncreasePrice(decimal percentage)
    {
        if (percentage <= 0)
        {
            throw new ValidationException(nameof(percentage), "Відсоток збільшення має бути додатним.");
        }
        Price += Price * (percentage / 100m);
    }

    public override string ToString()
    {
        return $"[Книга] ISBN: {SerialNumber}, «{Title}» ({PublicationYear}), Ціна: {Price:C}, Наклад: {CopiesCount} прим., Тираж: {TotalCirculationCost:C}";
    }
}
