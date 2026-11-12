using Lab03.BLL.Exceptions;
using Lab03.BLL.Mappers;
using Lab03.BLL.Models;
using Lab03.DAL.Context;
using Lab03.DAL.Entities;

namespace Lab03.BLL.Services;

public class TaxiDriverService
{
    private readonly IEntityContext<TaxiDriverEntity[]> _context;

    public TaxiDriverService(IEntityContext<TaxiDriverEntity[]> context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public TaxiDriverModel[] GetAll(string filePath)
    {
        if (!_context.FileExists(filePath))
        {
            return Array.Empty<TaxiDriverModel>();
        }

        try
        {
            TaxiDriverEntity[] entities = _context.Load(filePath);
            return TaxiDriverMapper.ToModels(entities ?? Array.Empty<TaxiDriverEntity>());
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка завантаження водіїв таксі з файлу '{filePath}'", ex);
        }
    }

    public void SaveAll(string filePath, TaxiDriverModel[] drivers)
    {
        ArgumentNullException.ThrowIfNull(drivers);

        try
        {
            TaxiDriverEntity[] entities = TaxiDriverMapper.ToEntities(drivers);
            _context.Save(filePath, entities);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new StorageException($"Помилка збереження водіїв таксі у файл '{filePath}'", ex);
        }
    }

    public TaxiDriverModel Add(string filePath, TaxiDriverModel driver)
    {
        ArgumentNullException.ThrowIfNull(driver);

        TaxiDriverModel[] current = GetAll(filePath);
        for (int i = 0; i < current.Length; i++)
        {
            if (string.Equals(current[i].LicenseNumber, driver.LicenseNumber, StringComparison.OrdinalIgnoreCase))
            {
                throw new DuplicateEntityException(nameof(TaxiDriverModel), driver.LicenseNumber);
            }
        }

        TaxiDriverModel[] updated = new TaxiDriverModel[current.Length + 1];
        Array.Copy(current, updated, current.Length);
        updated[current.Length] = driver;

        SaveAll(filePath, updated);
        return driver;
    }
}
