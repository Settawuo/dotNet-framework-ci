namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class SBNCheckCoverageResponse
    {
        public SBNCheckCoverageResponse()
        {
            this.RESULT_ID = -1;
            this.RETURN_CODE = -1;
            this.RETURN_DESC = "";
        }

        public decimal RESULT_ID { get; set; }
        public int RETURN_CODE { get; set; }
        public string RETURN_DESC { get; set; }

        private SBNCheckCoverageData _SBNCheckCoverageData;
        public SBNCheckCoverageData SBNCheckCoverageData
        {
            get { return _SBNCheckCoverageData ?? (_SBNCheckCoverageData = new SBNCheckCoverageData()); }
            set { _SBNCheckCoverageData = value; }
        }
    }

    public class SBNCheckCoverageData
    {
        public SBNCheckCoverageData()
        {
            this.AVALIABLE = "";
            this.NEWPORTID = -1;
            this.NEWPORTDESC = "";
            this.OWNER_PRODUCT = "";
            this.DATAPORTDESC = "";
            this.VOICEPORTDESC = "";
        }

        public string AVALIABLE { get; set; }
        public int NEWPORTID { get; set; }
        public string NEWPORTDESC { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string DATAPORTDESC { get; set; }
        public string VOICEPORTDESC { get; set; }
        public string NODEID { get; set; }
        public decimal MAXIMUMPORT { get; set; }

        //public class RESULT
        //{
        //    public RESULT()
        //   {
        //    this.AVALIABLE = "";
        //    this.OWNER_PRODUCT = "";
        //    }
        //     public string AVALIABLE { get; set; }
        //     public string OWNER_PRODUCT { get; set; }
        //}
    }


}
