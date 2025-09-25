using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using WBBData.DbIteration;

namespace WBBData.Repository
{
    public class FBBShareplexEntityRepository<T> : EntityRepositoryBase<T>, IFBBShareplexEntityRepository<T> where T : class
    {
        public FBBShareplexEntityRepository(IFBBShareplexDbFactory dbFactory)
            : base(dbFactory)
        { }

        //public EntityRepository(DbContext dbContext)
        //    : base(dbContext)
        //{ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="entitySetName"></param>
        /// <returns></returns>
        public IEnumerable<T> Translate(OracleDataReader dataReader, string entitySetName)
        {
            return DbContext != null
                ? ((IObjectContextAdapter)DbContext).ObjectContext.Translate<T>(dataReader, entitySetName, (System.Data.Entity.Core.Objects.MergeOption)MergeOption.AppendOnly).ToList()
                : new List<T>();
        }
    }
}
