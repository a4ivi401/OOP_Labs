using Lab03.BLL.Models;
using Lab03.DAL.Entities;

namespace Lab03.BLL.Mappers;

public static class TaxiDriverMapper
{
    public static TaxiDriverEntity ToEntity(TaxiDriverModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new TaxiDriverEntity(
            model.Id,
            model.FirstName,
            model.LastName,
            model.CarModel,
            model.LicenseNumber,
            model.DrivingExperienceYears
        );
    }

    public static TaxiDriverModel ToModel(TaxiDriverEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new TaxiDriverModel(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.CarModel,
            entity.LicenseNumber,
            entity.DrivingExperienceYears
        );
    }

    public static TaxiDriverEntity[] ToEntities(TaxiDriverModel[] models)
    {
        ArgumentNullException.ThrowIfNull(models);
        TaxiDriverEntity[] entities = new TaxiDriverEntity[models.Length];
        for (int i = 0; i < models.Length; i++)
        {
            entities[i] = ToEntity(models[i]);
        }
        return entities;
    }

    public static TaxiDriverModel[] ToModels(TaxiDriverEntity[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        TaxiDriverModel[] models = new TaxiDriverModel[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            models[i] = ToModel(entities[i]);
        }
        return models;
    }
}
