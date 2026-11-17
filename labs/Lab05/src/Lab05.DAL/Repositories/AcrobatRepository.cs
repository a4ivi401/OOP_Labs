using Lab05.DAL.Entities;
using Lab05.DAL.Providers;

namespace Lab05.DAL.Repositories;

public interface IAcrobatRepository : IRepository<AcrobatEntity>
{
}

public class AcrobatRepository : EntityRepository<AcrobatEntity>, IAcrobatRepository
{
    public AcrobatRepository(IDataProvider<AcrobatEntity> defaultProvider) : base(defaultProvider)
    {
    }
}
