using Lab03.BLL.Models;
using Lab03.DAL.Entities;

namespace Lab03.BLL.Mappers;

public static class StudentMapper
{
    public static StudentEntity ToEntity(StudentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new StudentEntity(
            model.Id,
            model.FirstName,
            model.LastName,
            model.Course,
            model.StudentId,
            model.OriginCity,
            model.Passport
        );
    }

    public static StudentModel ToModel(StudentEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new StudentModel(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Course,
            entity.StudentId,
            entity.OriginCity,
            entity.Passport
        );
    }

    public static StudentEntity[] ToEntities(StudentModel[] models)
    {
        ArgumentNullException.ThrowIfNull(models);
        StudentEntity[] entities = new StudentEntity[models.Length];
        for (int i = 0; i < models.Length; i++)
        {
            entities[i] = ToEntity(models[i]);
        }
        return entities;
    }

    public static StudentModel[] ToModels(StudentEntity[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        StudentModel[] models = new StudentModel[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            models[i] = ToModel(entities[i]);
        }
        return models;
    }
}
