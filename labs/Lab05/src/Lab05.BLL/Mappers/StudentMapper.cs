using Lab05.BLL.Models;
using Lab05.DAL.Entities;

namespace Lab05.BLL.Mappers;

public static class StudentMapper
{
    public static StudentEntity ToEntity(StudentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new StudentEntity
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Course = model.Course,
            StudentId = model.StudentId,
            OriginCity = model.OriginCity,
            Passport = model.Passport,
            IsSettledInDormitory = model.IsSettledInDormitory,
            DormitoryNumber = model.DormitoryNumber,
            RoomNumber = model.RoomNumber,
            SettlementDate = model.SettlementDate
        };
    }

    public static StudentModel ToModel(StudentEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new StudentModel
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Course = entity.Course,
            StudentId = entity.StudentId,
            OriginCity = entity.OriginCity,
            Passport = entity.Passport,
            IsSettledInDormitory = entity.IsSettledInDormitory,
            DormitoryNumber = entity.DormitoryNumber,
            RoomNumber = entity.RoomNumber,
            SettlementDate = entity.SettlementDate
        };
    }
}
