namespace Lab05.BLL.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class ValidationException : DomainException
{
    public string? PropertyName { get; }

    public ValidationException(string message, string? propertyName = null) : base(message)
    {
        PropertyName = propertyName;
    }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string message) : base(message)
    {
    }
}

public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string message) : base(message)
    {
    }
}

public class IneligibleForDormitoryException : DomainException
{
    public string StudentId { get; }
    public string OriginCity { get; }

    public IneligibleForDormitoryException(string studentId, string originCity, string message) : base(message)
    {
        StudentId = studentId;
        OriginCity = originCity;
    }
}

public class DriverLicenseDeniedException : DomainException
{
    public string Reason { get; }

    public DriverLicenseDeniedException(string reason, string message) : base(message)
    {
        Reason = reason;
    }
}

public class StorageException : DomainException
{
    public StorageException(string message, Exception? innerException = null)
        : base(message, innerException ?? new Exception(message))
    {
    }
}
