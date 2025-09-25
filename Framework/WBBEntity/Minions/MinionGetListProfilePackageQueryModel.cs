namespace WBBEntity.Minions
{
    public class MinionGetListProfilePackageQueryModel
    {
        public string ERROR_MSG { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_CLASS { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_GROUP { get; set; }
        public string PACKAGE_SUBTYPE { get; set; }
        public string PACKAGE_OWNER { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PACKAGE_STATUS { get; set; }
        public string PACKAGE_NAME { get; set; }
        public decimal? RECURRING_CHARGE { get; set; }
        public decimal? PRE_RECURRING_CHARGE { get; set; }
        public decimal? RECURRING_DISC_EXP { get; set; }
        public string RECURRING_START_DT { get; set; }
        public string RECURRING_END_DT { get; set; }
        public decimal? INITIATION_CHARGE { get; set; }
        public decimal? PRE_INITIATION_CHARGE { get; set; }
        public string PACKAGE_BILL_THA { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public string HOME_IP { get; set; }
        public string HOME_PORT { get; set; }
        public string IDD_FLAG { get; set; }
        public string FAX_FLAG { get; set; }
        public string NON_MOBILE_NO { get; set; }
        public string MOBILE_FORWARD { get; set; }
    }
}