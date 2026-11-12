using Lab03.BLL.Models;
using Lab03.DAL.Entities;

namespace Lab03.BLL.Mappers;

public static class AcrobatMapper
{
    public static AcrobatEntity ToEntity(AcrobatModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new AcrobatEntity(
            model.Id,
            model.FirstName,
            model.LastName,
            model.CircusOrTroupeName,
            model.Specialization
        );
    }

    public static AcrobatModel ToModel(AcrobatEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new AcrobatModel(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.CircusOrTroupeName,
            entity.Specialization
        );
    }

    public static AcrobatEntity[] ToEntities(AcrobatModel[] models)
    {
        ArgumentNullException.ThrowIfNull(models);
        AcrobatEntity[] entities = new AcrobatEntity[models.Length];
        for (int i = 0; i < models.Length; i++)
        {
            entities[i] = ToEntity(models[i]);
        }
        return entities;
    }

    public static AcrobatModel[] ToModels(AcrobatEntity[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        AcrobatModel[] models = new AcrobatModel[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            models[i] = ToModel(entities[i]);
        }
        return models;
    }
}
