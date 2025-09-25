using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using WBBEntity.Models.Mapping;

namespace WBBEntity.Models
{
    public partial class Context : DbContext
    {
        static Context()
        {
            Database.SetInitializer<Context>(null);
        }

        public Context()
            : base("Name=Context")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
        }

        public DbSet<FBB_ADDRESS> FBB_ADDRESS { get; set; }
        public DbSet<FBB_ALERT_MESSAGE> FBB_ALERT_MESSAGE { get; set; }
        public DbSet<FBB_APCOVERAGE> FBB_APCOVERAGE { get; set; }
        public DbSet<FBB_CARD_INFO> FBB_CARD_INFO { get; set; }
        public DbSet<FBB_CARDMODEL> FBB_CARDMODEL { get; set; }
        public DbSet<FBB_CFG_LOV> FBB_CFG_LOV { get; set; }
        public DbSet<FBB_CFG_IDS> FBB_CFG_IDS { get; set; }
        public DbSet<FBB_CFG_REPORT> FBB_CFG_REPORT { get; set; }
        public DbSet<FBB_COVERAGEAREA> FBB_COVERAGEAREA { get; set; }
        public DbSet<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION { get; set; }
        public DbSet<FBB_COVERAGEAREA_RESULT> FBB_COVERAGEAREA_RESULT { get; set; }
        public DbSet<FBB_DSLAM_INFO> FBB_DSLAM_INFO { get; set; }
        public DbSet<FBB_DSLAMMODEL> FBB_DSLAMMODEL { get; set; }
        public DbSet<FBB_PORT_INFO> FBB_PORT_INFO { get; set; }
        public DbSet<FBB_PORT_NOTE> FBB_PORT_NOTE { get; set; }
        public DbSet<FBB_REGISTER> FBB_REGISTER { get; set; }
        public DbSet<FBB_SENDMAIL_LOG> FBB_SENDMAIL_LOG { get; set; }
        public DbSet<FBB_ZIPCODE> FBB_ZIPCODE { get; set; }
        public DbSet<FBB_EMAIL_PROCESSING> FBB_EMAIL_PROCESSING { get; set; }
        public DbSet<FBB_EMAIL_TRANSACTION> FBB_EMAIL_TRANSACTION { get; set; }
        public DbSet<FBB_TITLE_MASTER> FBB_TITLE_MASTER { get; set; }
        public DbSet<FBB_NATIONALITY_MASTER> FBB_NATIONALITY_MASTER { get; set; }
        public DbSet<FBB_COVERAGE_ZIPCODE> FBB_COVERAGE_ZIPCODE { get; set; }
        public DbSet<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION { get; set; }
        public DbSet<FBB_USER> FBB_USER { get; set; }
        public DbSet<FBB_PROGRAM> FBB_PROGRAM { get; set; }
        public DbSet<FBB_PROGRAM_PERMISSION> FBB_PROGRAM_PERMISSION { get; set; }
        public DbSet<FBB_GROUP> FBB_GROUP { get; set; }
        public DbSet<FBB_GROUP_PERMISSION> FBB_GROUP_PERMISSION { get; set; }
        public DbSet<FBB_RPT_LOG> FBB_RPT_LOG { get; set; }
        public DbSet<FBB_TRACKING_IMPLEMENT_LOG> FBB_TRACKING_IMPLEMENT_LOG { get; set; }
        public DbSet<FBB_IMPLEMENT_REGIS_RABBIT> FBB_IMPLEMENT_REGIS_RABBIT { get; set; }
        public DbSet<FBB_CUST_PROFILE> FBB_CUST_PROFILE { get; set; }
        public DbSet<FBB_CUST_CONTACT> FBB_CUST_CONTACT { get; set; }
        public DbSet<FBB_COVERAGEAREA_BUILDING> FBB_COVERAGEAREA_BUILDING { get; set; }
        public DbSet<FBB_HISTORY_LOG> FBB_HISTORY_LOG { get; set; }
        public DbSet<FBSS_HISTORY_LOG> FBSS_HISTORY_LOG { get; set; }
        public DbSet<FBB_COMPONENT> FBB_COMPONENT { get; set; }
        public DbSet<FBB_COMPONENT_PERMISSION> FBB_COMPONENT_PERMISSION { get; set; }
        public DbSet<FBB_AP_INFO> FBB_AP_INFO { get; set; }
        public DbSet<FBB_MESSAGE_LOG> FBB_MESSAGE_LOG { get; set; }
        public DbSet<FBB_SFF_CHKPROFILE_LOG> FBB_SFF_CHKPROFILE_LOG { get; set; }
        public DbSet<FBB_CUST_PACKAGE> FBB_CUST_PACKAGE { get; set; }
        public DbSet<FBB_VISIT_LOG> FBB_VISIT_LOG { get; set; }
        public DbSet<FBB_VSMP_LOG> FBB_VSMP_LOG { get; set; }
        public DbSet<FBB_INTERFACE_LOG> FBB_INTERFACE_LOG { get; set; }
        public DbSet<FBB_FBSS_LISTBV> FBB_FBSS_LISTBV { get; set; }
        public DbSet<FBB_FBSS_COVERAGEAREA_RESULT> FBB_FBSS_COVERAGEAREA_RESULT { get; set; }
        public DbSet<FBB_EVENT_CODE> FBB_EVENT_CODE { get; set; }
        public DbSet<FBB_PACKAGE_TRAN> FBB_PACKAGE_TRAN { get; set; }
        public DbSet<FBBDORM_DORMITORY_MASTER> FBBDORM_DORMITORY_MASTER { get; set; }
        public DbSet<FBBDORM_DORMITORY_DTL> FBBDORM_DORMITORY_DTL { get; set; }
        public DbSet<FBBDORM_ORDER_INTERFACE> FBBDORM_ORDER_INTERFACE { get; set; }
        public DbSet<FBBDORM_DORMITORY_TRN_IPAY> FBBDORM_DORMITORY_TRN_IPAY { get; set; }
        public DbSet<FBB_VOUCHER_PIN> FBB_VOUCHER_PIN { get; set; }
        public DbSet<FBB_VOUCHER_MASTER> FBB_VOUCHER_MASTER { get; set; }
        public DbSet<FBB_INTERFACE_LOG_ADMIN> FBB_INTERFACE_LOG_ADMIN { get; set; }
        public DbSet<FBB_EVENT_SUBCONTRACT> FBB_EVENT_SUBCONTRACT { get; set; }
        public DbSet<FBB_ORD_CHANGE_PACKAGE> FBB_ORD_CHANGE_PACKAGE { get; set; }
        public DbSet<FBB_LISTBV_LOCATION_GROUP> FBB_LISTBV_LOCATION_GROUP { get; set; }
        public DbSet<FBB_LISTBV_BY_LOCATION> FBB_LISTBV_BY_LOCATION { get; set; }
        public DbSet<FBB_SFF_PROMOTION_CONFIG> FBB_SFF_PROMOTION_CONFIG { get; set; }
        public DbSet<FBB_SFF_PROMOTION_MAPPING> FBB_SFF_PROMOTION_MAPPING { get; set; }
        public DbSet<FBB_PRE_REGISTER> FBB_PRE_REGISTER { get; set; }
        public DbSet<FBSS_INSTALLATION_COST> FBSS_INSTALLATION_COST { get; set; }
        public DbSet<FBSS_CONFIG_TBL> FBSS_CONFIG_TBL { get; set; }
        public DbSet<FBSS_FIXED_ASSET_REASON> FBSS_FIXED_ASSET_REASON { get; set; }
        public DbSet<FBSS_FIXED_ASSET_SYMPTOM> FBSS_FIXED_ASSET_SYMPTOM { get; set; }
        public DbSet<FBSS_FOA_SYMPTOM> FBSS_FOA_SYMPTOM { get; set; }
        public DbSet<FBB_SUBMIT_FOA_ERROR_LOG> FBB_SUBMIT_FOA_ERROR_LOG { get; set; }
        public DbSet<FBSS_FIXED_ASSET_TRAN_LOG> FBSS_FIXED_ASSET_TRAN_LOG { get; set; }
        public DbSet<FBSS_FOA_SUBMIT_ORDER> FBSS_FOA_SUBMIT_ORDER { get; set; }
        public DbSet<FBSS_FOA_SUBMIT_ORDER_DTL> FBSS_FOA_SUBMIT_ORDER_DTL { get; set; }
        public DbSet<FBB_FIXED_ASSET_HISTORY_LOG> FBB_FIXED_ASSET_HISTORY_LOG { get; set; }
        public DbSet<FBSS_FIXED_ASSET_CONFIG> FBSS_FIXED_ASSET_CONFIG { get; set; }
        public DbSet<FBB_INTERFACE_LOG_PAYG> FBB_INTERFACE_LOG_PAYG { get; set; }
        public DbSet<FBBPAYG_STANDARD_ADDRESS> FBBPAYG_STANDARD_ADDRESS { get; set; }
        public DbSet<FBBPAYG_WFM_SUBCONTRACTOR> FBBPAYG_WFM_SUBCONTRACTOR { get; set; }
        public DbSet<FBBPAYG_VENDOR> FBBPAYG_VENDOR { get; set; }
        public DbSet<FBSS_FOA_VENDOR_CODE> FBSS_FOA_VENDOR_CODE { get; set; }
        public DbSet<FBB_MINION_SERVICE> FBB_MINION_SERVICE { get; set; }
        public DbSet<FBB_EMPLOYEE> FBB_EMPLOYEE { get; set; }
        //public DbSet<XML_TEST> XML_TEST { get; set; }
        public DbSet<FBSS_FIXED_ASSET_SN_ACT> FBSS_FIXED_ASSET_SN_ACT { get; set; }
        public DbSet<FBB_LOAD_CONFIG_LOV> FBB_LOAD_CONFIG_LOV { get; set; }
        public DbSet<FBB_REGISTER_PENDING_PAYMENT> FBB_REGISTER_PENDING_PAYMENT { get; set; }
        public DbSet<FBB_REGISTER_PENDING_DEDUCTION> FBB_REGISTER_PENDING_DEDUCTION { get; set; }
        public DbSet<FBB_CFG_PAYMENT> FBB_CFG_PAYMENTMap { get; set; }
        public DbSet<FBB_REGISTER_PAYMENT_LOG_SPDP> FBB_REGISTER_PAYMENT_LOG_SPDP { get; set; }
        public DbSet<FBSS_FIXED_OM010_RPT> FBSS_FIXED_OM010_RPT { get; set; }
        public DbSet<FBBPAYG_SIM_SLOC_TEMP> FBBPAYG_SIM_SLOC_TEMP { get; set; }
        // -------------Create new Table --------------------
        public DbSet<FBB_CFG_QUERY_REPORT> fBB_CFG_QUERY_REPORT { get; set; }
        public DbSet<FBBPAYG_PATCH_SN_SENDMAIL_LOG> FBBPAYG_PATCH_SN_SENDMAIL_LOG { get; set; }
        public DbSet<FBB_INTERFACE_LOG_3BB> FBB_INTERFACE_LOG_3BB { get; set; }
        public DbSet<FBBPAYG_ORDER_PACKAGE> FBBPAYG_ORDER_PACKAGE { get; set; }
        public DbSet<FBBPAYG_ORDER_FEE> FBBPAYG_ORDER_FEE { get; set; }

        // public DbSet<FBB_CONSENT_LOG> FBB_CONSENT_LOG { get; set; }\

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FBB_ADDRESSMap());
            modelBuilder.Configurations.Add(new FBB_ALERT_MESSAGEMap());
            modelBuilder.Configurations.Add(new FBB_APCOVERAGEMap());
            modelBuilder.Configurations.Add(new FBB_CARD_INFOMap());
            modelBuilder.Configurations.Add(new FBB_CARDMODELMap());
            modelBuilder.Configurations.Add(new FBB_CFG_LOVMap());
            modelBuilder.Configurations.Add(new FBB_CFG_IDSMap());
            modelBuilder.Configurations.Add(new FBB_CFG_REPORTMap());
            modelBuilder.Configurations.Add(new FBB_COVERAGEAREAMap());
            modelBuilder.Configurations.Add(new FBB_COVERAGEAREA_RELATIONMap());
            modelBuilder.Configurations.Add(new FBB_COVERAGEAREA_RESULTMap());
            modelBuilder.Configurations.Add(new FBB_DSLAM_INFOMap());
            modelBuilder.Configurations.Add(new FBB_DSLAMMODELMap());
            modelBuilder.Configurations.Add(new FBB_PORT_INFOMap());
            modelBuilder.Configurations.Add(new FBB_PORT_NOTEMap());
            modelBuilder.Configurations.Add(new FBB_REGISTERMap());
            modelBuilder.Configurations.Add(new FBB_SENDMAIL_LOGMap());
            modelBuilder.Configurations.Add(new FBB_ZIPCODEMap());
            modelBuilder.Configurations.Add(new FBB_EMAIL_PROCESSINGMap());
            modelBuilder.Configurations.Add(new FBB_EMAIL_TRANSACTIONMap());
            modelBuilder.Configurations.Add(new FBB_TITLE_MASTERMap());
            modelBuilder.Configurations.Add(new FBB_NATIONALITY_MASTERMap());
            modelBuilder.Configurations.Add(new FBB_COVERAGE_ZIPCODEMap());
            modelBuilder.Configurations.Add(new FBB_COVERAGE_REGIONMap());
            modelBuilder.Configurations.Add(new FBB_USERMap());
            modelBuilder.Configurations.Add(new FBB_PROGRAMMap());
            modelBuilder.Configurations.Add(new FBB_PROGRAM_PERMISSIONMap());
            modelBuilder.Configurations.Add(new FBB_GROUPMap());
            modelBuilder.Configurations.Add(new FBB_GROUP_PERMISSIONMap());
            modelBuilder.Configurations.Add(new FBB_RPT_LOGMap());
            modelBuilder.Configurations.Add(new FBB_TRACKING_IMPLEMENT_LOGMap());
            modelBuilder.Configurations.Add(new FBB_IMPLEMENT_REGIS_RABBITMap());
            modelBuilder.Configurations.Add(new FBB_CUST_PROFILEMap());
            modelBuilder.Configurations.Add(new FBB_CUST_CONTACTMap());
            modelBuilder.Configurations.Add(new FBB_AP_INFOMap());
            modelBuilder.Configurations.Add(new FBB_COVERAGEAREA_BUILDINGMap());
            modelBuilder.Configurations.Add(new FBB_HISTORY_LOGmap());
            modelBuilder.Configurations.Add(new FBSS_HISTORY_LOGmap());
            modelBuilder.Configurations.Add(new FBB_COMPONENTMap());
            modelBuilder.Configurations.Add(new FBB_COMPONENT_PERMISSIONMap());
            modelBuilder.Configurations.Add(new FBB_MESSAGE_LOGMap());
            modelBuilder.Configurations.Add(new FBB_SFF_CHKPROFILE_LOGMap());
            modelBuilder.Configurations.Add(new FBB_CUST_PACKAGEMap());
            modelBuilder.Configurations.Add(new FBB_VISIT_LOGMap());
            modelBuilder.Configurations.Add(new FBB_INTERFACE_LOGMap());
            modelBuilder.Configurations.Add(new FBB_VSMP_LOGMap());
            modelBuilder.Configurations.Add(new FBB_FBSS_LISTBVMap());
            modelBuilder.Configurations.Add(new FBB_FBSS_COVERAGEAREA_RESULTMap());
            modelBuilder.Configurations.Add(new FBB_EVENT_CODEMap());
            modelBuilder.Configurations.Add(new FBB_PACKAGE_TRANMap());
            modelBuilder.Configurations.Add(new FBBDORM_DORMITORY_MASTERMap());
            modelBuilder.Configurations.Add(new FBBDORM_DORMITORY_DTLMap());
            modelBuilder.Configurations.Add(new FBBDORM_ORDER_INTERFACEMap());
            modelBuilder.Configurations.Add(new FBBDORM_DORMITORY_TRN_IPAYMap());
            modelBuilder.Configurations.Add(new FBB_VOUCHER_PINMap());
            modelBuilder.Configurations.Add(new FBB_VOUCHER_MASTERMap());
            modelBuilder.Configurations.Add(new FBB_INTERFACE_LOG_ADMINMap());
            modelBuilder.Configurations.Add(new FBB_EVENT_SUBCONTRACTMap());
            modelBuilder.Configurations.Add(new FBB_ORD_CHANGE_PACKAGEMap());
            modelBuilder.Configurations.Add(new FBB_LISTBV_LOCATION_GROUPMap());
            modelBuilder.Configurations.Add(new FBB_LISTBV_BY_LOCATIONMap());
            modelBuilder.Configurations.Add(new FBB_SFF_PROMOTION_CONFIGMap());
            modelBuilder.Configurations.Add(new FBB_SFF_PROMOTION_MAPPINGMap());
            modelBuilder.Configurations.Add(new FBB_PRE_REGISTERMap());
            modelBuilder.Configurations.Add(new FBSS_INSTALLATION_COSTMap());
            modelBuilder.Configurations.Add(new FBSS_CONFIG_TBLMap());
            modelBuilder.Configurations.Add(new FBSS_FIXED_ASSET_SYMPTOMMap());
            modelBuilder.Configurations.Add(new FBSS_FIXED_ASSET_REASONMap());
            modelBuilder.Configurations.Add(new FBSS_FOA_SYMPTOMMap());
            modelBuilder.Configurations.Add(new FBB_SUBMIT_FOA_ERROR_LOGMap());
            modelBuilder.Configurations.Add(new FBSS_FIXED_ASSET_TRAN_LOGMap());
            modelBuilder.Configurations.Add(new FBSS_FOA_SUBMIT_ORDERMap());
            modelBuilder.Configurations.Add(new FBSS_FOA_SUBMIT_ORDER_DTLMap());
            modelBuilder.Configurations.Add(new FBB_FIXED_ASSET_HISTORY_LOGMap());
            modelBuilder.Configurations.Add(new FBSS_FIXED_ASSET_CONFIGMap());
            modelBuilder.Configurations.Add(new FBB_INTERFACE_LOG_PAYGMap());
            modelBuilder.Configurations.Add(new FBBPAYG_STANDARD_ADDRESSMap());
            modelBuilder.Configurations.Add(new FBBPAYG_WFM_SUBCONTRACTORMap());
            modelBuilder.Configurations.Add(new FBBPAYG_VENDORMap());
            modelBuilder.Configurations.Add(new FBSS_FOA_VENDOR_CODEMap());
            modelBuilder.Configurations.Add(new FBB_MINION_SERVICEMap());
            modelBuilder.Configurations.Add(new FBB_EMPLOYEEMap());
            //modelBuilder.Configurations.Add(new XML_TESTMap());
            modelBuilder.Configurations.Add(new FBSS_FIXED_ASSET_SN_ACTMap());
            modelBuilder.Configurations.Add(new FBB_LOAD_CONFIG_LOVMap());
            modelBuilder.Configurations.Add(new FBB_REGISTER_PENDING_PAYMENTMap());
            modelBuilder.Configurations.Add(new FBB_REGISTER_PENDING_DEDUCTIONMap());
            modelBuilder.Configurations.Add(new FBB_CFG_PAYMENTMap());
            modelBuilder.Configurations.Add(new FBB_REGISTER_PAYMENT_LOG_SPDPMap());
            modelBuilder.Configurations.Add(new FBSS_FIXED_OM010_RPTMap());
            modelBuilder.Configurations.Add(new FBB_CFG_QUERY_REPORTMap());
            modelBuilder.Configurations.Add(new FBBPAYG_SIM_SLOC_TEMPMap());
            modelBuilder.Configurations.Add(new FBBPAYG_PATCH_SN_SENDMAIL_LOGMap());
            modelBuilder.Configurations.Add(new FBB_INTERFACE_LOG_3BBMap());
            modelBuilder.Configurations.Add(new FBB_SPECIALISTMap());
            modelBuilder.Configurations.Add(new FBBPAYG_ORDER_FEEMap());
            modelBuilder.Configurations.Add(new FBBPAYG_ORDER_PACKAGEMap());
        }
    }
}
