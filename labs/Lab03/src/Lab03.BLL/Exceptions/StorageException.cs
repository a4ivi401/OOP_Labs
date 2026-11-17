namespace Lab03.BLL.Exceptions;

public class StorageException : DomainException
{
    public StorageException(string message) : base(message)
    {
    }

    public StorageException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
