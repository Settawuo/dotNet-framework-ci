using AIRNETEntity.Models.Mapping;
using System.Data.Entity;

namespace AIRNETEntity.Models
{
    public partial class Context : DbContext
    {
        static Context()
        {
            Database.SetInitializer<Context>(null);
        }

        public Context()
            : base("Name=AirNetContext")
        {
        }

        public DbSet<AIR_PACKAGE_MASTER> AIR_PACKAGE_MASTER { get; set; }
        public DbSet<AIR_SALE_ORD_CONTACT> AIR_SALE_ORD_CONTACT { get; set; }
        public DbSet<AIR_SALE_ORD_CUSTOMER> AIR_SALE_ORD_CUSTOMER { get; set; }
        public DbSet<AIR_SALE_ORD_FLOW> AIR_SALE_ORD_FLOW { get; set; }
        public DbSet<AIR_SALE_ORD_IA> AIR_SALE_ORD_IA { get; set; }
        public DbSet<AIR_SALE_ORD_IA_HIS> AIR_SALE_ORD_IA_HIS { get; set; }
        public DbSet<AIR_SALE_ORD_INTERFACE> AIR_SALE_ORD_INTERFACE { get; set; }
        public DbSet<AIR_SALE_ORD_NOTE> AIR_SALE_ORD_NOTE { get; set; }
        public DbSet<AIR_SALE_ORD_PACKAGE> AIR_SALE_ORD_PACKAGE { get; set; }
        public DbSet<AIR_SALE_ORDER> AIR_SALE_ORDER { get; set; }
        public DbSet<AIR_SFF_PROVINCE_MASTER> AIR_SFF_PROVINCE_MASTER { get; set; }
        public DbSet<AIR_SFF_ZIPCODE_MASTER> AIR_SFF_ZIPCODE_MASTER { get; set; }
        public DbSet<AIR_SFF_SERVICE_CODE> AIR_SFF_SERVICE_CODE { get; set; }
        public DbSet<AIR_NEW_PACKAGE_MASTER> AIR_NEW_PACKAGE_MASTER { get; set; }
        public DbSet<AIR_PACKAGE_MASTER_DETAIL> AIR_PACKAGE_MASTER_DETAIL { get; set; }
        public DbSet<AIR_PACKAGE_USER_GROUP> AIR_PACKAGE_USER_GROUP { get; set; }
        public DbSet<AIR_CHANNEL_GROUP> AIR_CHANNEL_GROUP { get; set; }
        public DbSet<AIR_PACKAGE_LOCATION> AIR_PACKAGE_LOCATION { get; set; }
        public DbSet<AIR_INTERFACE_LOG> AIR_INTERFACE_LOG { get; set; }
        public DbSet<AIR_PACKAGE_FTTR> AIR_PACKAGE_FTTR { get; set; }
        public DbSet<AIR_FBB_NEW_PACKAGE_MASTER> AIR_FBB_NEW_PACKAGE_MASTER { get; set; }
        public DbSet<AIR_FBB_PACKAGE_MASTER_DETAIL> AIR_FBB_PACKAGE_MASTER_DETAIL { get; set; }
        public DbSet<AIR_PACKAGE_SEQ_MASTER> AIR_PACKAGE_SEQ_MASTER { get; set; }
        public DbSet<AIR_FBB_PACKAGE_SERVICE_MASTER> AIR_FBB_PACKAGE_SERVICE_MASTER { get; set; }
        public DbSet<AIR_FBB_PACKAGE_MAPPING> AIR_FBB_PACKAGE_MAPPING { get; set; }
        public DbSet<AIR_PARAMETER> AIR_PARAMETER { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AIR_PACKAGE_MASTERMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_CONTACTMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_CUSTOMERMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_FLOWMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_IAMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_IA_HISMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_INTERFACEMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_NOTEMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORD_PACKAGEMap());
            modelBuilder.Configurations.Add(new AIR_SALE_ORDERMap());
            modelBuilder.Configurations.Add(new AIR_SFF_PROVINCE_MASTERMap());
            modelBuilder.Configurations.Add(new AIR_SFF_ZIPCODE_MASTERMap());
            modelBuilder.Configurations.Add(new AIR_SFF_SERVICE_CODEMap());
            modelBuilder.Configurations.Add(new AIR_NEW_PACKAGE_MASTERmap());
            modelBuilder.Configurations.Add(new AIR_PACKAGE_MASTER_DETAILmap());
            modelBuilder.Configurations.Add(new AIR_PACKAGE_USER_GROUPmap());
            modelBuilder.Configurations.Add(new AIR_CHANNEL_GROUPmap());
            modelBuilder.Configurations.Add(new AIR_PACKAGE_LOCATIONmap());
            modelBuilder.Configurations.Add(new AIR_INTERFACE_LOGmap());
            modelBuilder.Configurations.Add(new AIR_PACKAGE_FTTRMap());
            modelBuilder.Configurations.Add(new AIR_FBB_NEW_PACKAGE_MASTERMap());
            modelBuilder.Configurations.Add(new AIR_FBB_PACKAGE_MASTER_DETAILMap());
            modelBuilder.Configurations.Add(new AIR_PACKAGE_SEQ_MASTERMap());
            modelBuilder.Configurations.Add(new AIR_FBB_PACKAGE_SERVICE_MASTERmap());
            modelBuilder.Configurations.Add(new AIR_FBB_PACKAGE_MAPPINGMap());
            modelBuilder.Configurations.Add(new AIR_PARAMETERMap());



        }
    }
}
