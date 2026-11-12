namespace Lab03.BLL.Exceptions;

public class ValidationException : DomainException
{
    public string PropertyName { get; }

    public ValidationException(string propertyName, string message)
        : base($"Помилка валідації для '{propertyName}': {message}")
    {
        PropertyName = propertyName;
    }
}
