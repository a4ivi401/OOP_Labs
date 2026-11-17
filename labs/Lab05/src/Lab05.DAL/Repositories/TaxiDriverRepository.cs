using Lab05.DAL.Entities;
using Lab05.DAL.Providers;

namespace Lab05.DAL.Repositories;

public interface ITaxiDriverRepository : IRepository<TaxiDriverEntity>
{
    TaxiDriverEntity? GetByLicenseNumber(string licenseNumber);
}

public class TaxiDriverRepository : EntityRepository<TaxiDriverEntity>, ITaxiDriverRepository
{
    public TaxiDriverRepository(IDataProvider<TaxiDriverEntity> defaultProvider) : base(defaultProvider)
    {
    }

    public TaxiDriverEntity? GetByLicenseNumber(string licenseNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(licenseNumber);
        return GetAll().FirstOrDefault(t => t.LicenseNumber.Equals(licenseNumber, StringComparison.OrdinalIgnoreCase));
    }
}
