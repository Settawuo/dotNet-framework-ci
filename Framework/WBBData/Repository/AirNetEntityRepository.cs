using System;
using System.Collections.Generic;
using WBBData.DbIteration;

namespace WBBData.Repository
{
    public class AirNetEntityRepository<T> : EntityRepositoryBase<T>, IAirNetEntityRepository<T> where T : class
    {
        public AirNetEntityRepository(IAIRDbFactory dbFactory)
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
        public IEnumerable<T> Translate(Oracle.ManagedDataAccess.Client.OracleDataReader dataReader, string entitySetName)
        {
            throw new NotImplementedException();
        }
    }
}
