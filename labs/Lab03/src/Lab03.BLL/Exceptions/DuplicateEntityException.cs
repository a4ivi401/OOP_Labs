namespace Lab03.BLL.Exceptions;

public class DuplicateEntityException : DomainException
{
    public string EntityType { get; }
    public string EntityId { get; }

    public DuplicateEntityException(string entityType, string entityId)
        : base($"Сутність '{entityType}' з ідентифікатором '{entityId}' вже існує в системі.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
