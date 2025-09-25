using WBBData.DbIteration;

namespace WBBData.Repository
{
    public interface IEntityRepository<T> : IEntityRepositoryBase<T> where T : class
    {
    }
}
