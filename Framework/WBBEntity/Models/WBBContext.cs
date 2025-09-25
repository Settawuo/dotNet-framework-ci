using System.Data.Common;
using System.Data.Entity;

namespace WBBEntity.Models
{
    public partial class Context : DbContext
    {
        private string _dbSchema = "AIRNET";

        public Context(DbConnection connection, string dbSchema)
            : base(connection, true)
        {
            _dbSchema = dbSchema;
            base.Configuration.AutoDetectChangesEnabled = false;
            //((IObjectContextAdapter)this).ObjectContext.Connection.Open();
            //Database.SetInitializer(new ContextInitializer());
            //Database.Initialize(force: true);    
            //base.Configuration.ProxyCreationEnabled = false;
        }

        public Context(string connectionString, string dbSchema)
            : base(connectionString)
        {
            _dbSchema = dbSchema;
            base.Configuration.AutoDetectChangesEnabled = false;
            //((IObjectContextAdapter)this).ObjectContext.Connection.Open();
            //Database.SetInitializer(new ContextInitializer());
            //Database.Initialize(force: true);    
            //base.Configuration.ProxyCreationEnabled = false;
        }
    }
}
