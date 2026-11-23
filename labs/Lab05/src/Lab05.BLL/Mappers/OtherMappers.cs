using Lab05.BLL.Models;
using Lab05.DAL.Entities;

namespace Lab05.BLL.Mappers;

public static class TaxiDriverMapper
{
    public static TaxiDriverEntity ToEntity(TaxiDriverModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new TaxiDriverEntity
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CarModel = model.CarModel,
            LicenseNumber = model.LicenseNumber,
            ExperienceYears = model.ExperienceYears,
            Age = model.Age,
            HasMedicalCertificate = model.HasMedicalCertificate,
            HasActiveLicense = model.HasActiveLicense
        };
    }

    public static TaxiDriverModel ToModel(TaxiDriverEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new TaxiDriverModel
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CarModel = entity.CarModel,
            LicenseNumber = entity.LicenseNumber,
            ExperienceYears = entity.ExperienceYears,
            Age = entity.Age,
            HasMedicalCertificate = entity.HasMedicalCertificate,
            HasActiveLicense = entity.HasActiveLicense
        };
    }
}

public static class AcrobatMapper
{
    public static AcrobatEntity ToEntity(AcrobatModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new AcrobatEntity
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CircusOrTroupe = model.CircusOrTroupe,
            Specialization = model.Specialization,
            SafetyCertificationValid = model.SafetyCertificationValid,
            PerformancesCount = model.PerformancesCount
        };
    }

    public static AcrobatModel ToModel(AcrobatEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new AcrobatModel
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CircusOrTroupe = entity.CircusOrTroupe,
            Specialization = entity.Specialization,
            SafetyCertificationValid = entity.SafetyCertificationValid,
            PerformancesCount = entity.PerformancesCount
        };
    }
}
