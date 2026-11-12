using Lab03.BLL.Models;
using Lab03.DAL.Entities;

namespace Lab03.BLL.Mappers;

public static class BookMapper
{
    public static BookEntity ToEntity(BookModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new BookEntity(
            model.SerialNumber,
            model.Title,
            model.PublicationYear,
            model.Price,
            model.CopiesCount
        );
    }

    public static BookModel ToModel(BookEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new BookModel(
            entity.SerialNumber,
            entity.Title,
            entity.PublicationYear,
            entity.Price,
            entity.CopiesCount
        );
    }

    public static BookEntity[] ToEntities(BookModel[] models)
    {
        ArgumentNullException.ThrowIfNull(models);
        BookEntity[] entities = new BookEntity[models.Length];
        for (int i = 0; i < models.Length; i++)
        {
            entities[i] = ToEntity(models[i]);
        }
        return entities;
    }

    public static BookModel[] ToModels(BookEntity[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        BookModel[] models = new BookModel[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            models[i] = ToModel(entities[i]);
        }
        return models;
    }
}
