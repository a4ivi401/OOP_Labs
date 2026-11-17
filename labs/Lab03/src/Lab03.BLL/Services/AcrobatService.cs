using Lab03.BLL.Exceptions;
using Lab03.BLL.Mappers;
using Lab03.BLL.Models;
using Lab03.DAL.Context;
using Lab03.DAL.Entities;

namespace Lab03.BLL.Services;

public class AcrobatService
{
    private readonly IEntityContext<AcrobatEntity[]> _context;

    public AcrobatService(IEntityContext<AcrobatEntity[]> context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public AcrobatModel[] GetAll(string filePath)
    {
        if (!_context.FileExists(filePath))
        {
            return Array.Empty<AcrobatModel>();
        }

        try
        {
            AcrobatEntity[] entities = _context.Load(filePath);
            return AcrobatMapper.ToModels(entities ?? Array.Empty<AcrobatEntity>());
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка завантаження акробатів з файлу '{filePath}'", ex);
        }
    }

    public void SaveAll(string filePath, AcrobatModel[] acrobats)
    {
        ArgumentNullException.ThrowIfNull(acrobats);

        try
        {
            AcrobatEntity[] entities = AcrobatMapper.ToEntities(acrobats);
            _context.Save(filePath, entities);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка збереження акробатів у файл '{filePath}'", ex);
        }
    }

    public AcrobatModel Add(string filePath, AcrobatModel acrobat)
    {
        ArgumentNullException.ThrowIfNull(acrobat);

        AcrobatModel[] current = GetAll(filePath);
        for (int i = 0; i < current.Length; i++)
        {
            if (string.Equals(current[i].Id, acrobat.Id, StringComparison.OrdinalIgnoreCase))
            {
                throw new DuplicateEntityException(nameof(AcrobatModel), acrobat.Id);
            }
        }

        AcrobatModel[] updated = new AcrobatModel[current.Length + 1];
        Array.Copy(current, updated, current.Length);
        updated[current.Length] = acrobat;

        SaveAll(filePath, updated);
        return acrobat;
    }
}
