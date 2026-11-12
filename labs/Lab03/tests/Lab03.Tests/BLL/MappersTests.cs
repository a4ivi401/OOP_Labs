using Lab03.BLL.Mappers;
using Lab03.BLL.Models;
using Lab03.DAL.Entities;

namespace Lab03.Tests.BLL;

public class MappersTests
{
    [Fact]
    public void StudentMapper_Bidirectional_ShouldPreserveValues()
    {
        StudentModel model = new("1", "Тарас", "Шевченко", 1, "KB112233", "Черкаси", "СН123456");
        StudentEntity entity = StudentMapper.ToEntity(model);

        Assert.Equal(model.Id, entity.Id);
        Assert.Equal(model.FirstName, entity.FirstName);
        Assert.Equal(model.LastName, entity.LastName);
        Assert.Equal(model.Course, entity.Course);
        Assert.Equal(model.StudentId, entity.StudentId);
        Assert.Equal(model.OriginCity, entity.OriginCity);
        Assert.Equal(model.Passport, entity.Passport);

        StudentModel restored = StudentMapper.ToModel(entity);
        Assert.Equal(model.FullName, restored.FullName);
        Assert.Equal(model.StudentId, restored.StudentId);
    }

    [Fact]
    public void BookMapper_Bidirectional_ShouldPreserveValues()
    {
        BookModel model = new("978-0132350884", "Clean Code", 2008, 450.00m, 1500);
        BookEntity entity = BookMapper.ToEntity(model);

        Assert.Equal(model.SerialNumber, entity.SerialNumber);
        Assert.Equal(model.Title, entity.Title);
        Assert.Equal(model.Price, entity.Price);

        BookModel restored = BookMapper.ToModel(entity);
        Assert.Equal(model.Title, restored.Title);
        Assert.Equal(model.TotalCirculationCost, restored.TotalCirculationCost);
    }

    [Fact]
    public void TaxiDriverMapper_Bidirectional_ShouldPreserveValues()
    {
        TaxiDriverModel model = new("1", "Василь", "Шофер", "Toyota Camry", "ВХА123456", 10);
        TaxiDriverEntity entity = TaxiDriverMapper.ToEntity(model);

        Assert.Equal(model.CarModel, entity.CarModel);
        Assert.Equal(model.LicenseNumber, entity.LicenseNumber);

        TaxiDriverModel restored = TaxiDriverMapper.ToModel(entity);
        Assert.Equal(model.DrivingExperienceYears, restored.DrivingExperienceYears);
    }

    [Fact]
    public void AcrobatMapper_Bidirectional_ShouldPreserveValues()
    {
        AcrobatModel model = new("1", "Анна", "Повітряна", "Національний Цирк", "Повітряне кільце");
        AcrobatEntity entity = AcrobatMapper.ToEntity(model);

        Assert.Equal(model.CircusOrTroupeName, entity.CircusOrTroupeName);
        Assert.Equal(model.Specialization, entity.Specialization);

        AcrobatModel restored = AcrobatMapper.ToModel(entity);
        Assert.Equal(model.Specialization, restored.Specialization);
    }
}
