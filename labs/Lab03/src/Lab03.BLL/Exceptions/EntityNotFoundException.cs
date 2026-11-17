namespace Lab03.BLL.Exceptions;

public class EntityNotFoundException : DomainException
{
    public string EntityType { get; }
    public string EntityId { get; }

    public EntityNotFoundException(string entityType, string entityId)
        : base($"Сутність '{entityType}' з ідентифікатором '{entityId}' не знайдена.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
