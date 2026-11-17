using Lab03.BLL.Exceptions;

namespace Lab03.BLL.Models;

public abstract class PersonModel
{
    private string _id = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;

    public string Id
    {
        get => _id;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(Id), "Ідентифікатор не може бути порожнім.");
            }
            _id = value.Trim();
        }
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(FirstName), "Ім'я не може бути порожнім.");
            }
            _firstName = value.Trim();
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(LastName), "Прізвище не може бути порожнім.");
            }
            _lastName = value.Trim();
        }
    }

    public string FullName => $"{FirstName} {LastName}";

    protected PersonModel(string id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
}
