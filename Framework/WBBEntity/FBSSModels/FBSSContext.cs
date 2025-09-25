using System.Data.Common;
using System.Data.Entity;

namespace WBBEntity.FBSSModels
{
    public partial class FBSSContext : DbContext
    {
        static FBSSContext()
        {
            Database.SetInitializer<FBSSContext>(null);
        }

        public FBSSContext()
            : base("Name=ContextFBSS")
        {
        }

        private string _dbSchema = "AIRNET";

        public FBSSContext(DbConnection connection, string dbSchema)
            : base(connection, true)
        {
            _dbSchema = dbSchema;
            base.Configuration.AutoDetectChangesEnabled = false;
            //((IObjectContextAdapter)this).ObjectContext.Connection.Open();
            //Database.SetInitializer(new ContextInitializer());
            //Database.Initialize(force: true);    
            //base.Configuration.ProxyCreationEnabled = false;
        }

        public FBSSContext(string connectionString, string dbSchema)
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
