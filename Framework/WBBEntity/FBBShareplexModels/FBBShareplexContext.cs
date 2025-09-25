using System.Data.Common;
using System.Data.Entity;
using WBBEntity.FBBShareplexModels.Mapping;

namespace WBBEntity.FBBShareplexModels
{
    public partial class FBBShareplexContext : DbContext
    {
        static FBBShareplexContext()
        {
            Database.SetInitializer<FBBShareplexContext>(null);
        }

        public FBBShareplexContext()
            : base("Name=ContextFBBShareplex")
        {
        }

        private string _dbSchema = "AIRNET";

        public FBBShareplexContext(DbConnection connection, string dbSchema)
            : base(connection, true)
        {
            _dbSchema = dbSchema;
            base.Configuration.AutoDetectChangesEnabled = false;
            //((IObjectContextAdapter)this).ObjectContext.Connection.Open();
            //Database.SetInitializer(new ContextInitializer());
            //Database.Initialize(force: true);    
            //base.Configuration.ProxyCreationEnabled = false;
        }

        public FBBShareplexContext(string connectionString, string dbSchema)
            : base(connectionString)
        {
            _dbSchema = dbSchema;
            base.Configuration.AutoDetectChangesEnabled = false;
            //((IObjectContextAdapter)this).ObjectContext.Connection.Open();
            //Database.SetInitializer(new ContextInitializer());
            //Database.Initialize(force: true);    
            //base.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<WFS_TEAM_ATTR> WFS_TEAM_ATTR { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new WFS_TEAM_ATTRMap());
        }
    }
}
