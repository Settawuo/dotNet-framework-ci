namespace WBBContract.Commands
{
    public class CoverageResultCommand : CommandBase
    {
        public CoverageResultCommand()
        {
            this.RESULT_ID = -1;
            this.CVRID = -1;
            this.FLOOR = -1;
            this.MOO = -1;
        }

        public decimal RESULT_ID { get; set; }
        public decimal CVRID { get; set; }
        public string NODENAME { get; set; }
        public string TOWER { get; set; }
        public decimal FLOOR { get; set; }
        public string ISONLINENUMBER { get; set; }
        public string ADDRESS_NO { get; set; }
        public decimal MOO { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string COVERAGETYPE { get; set; }
        public string COVERAGERESULT { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string ZIPCODE_ROWID { get; set; }
        public string OWNER { get; set; }
        public string TRANSACTION_ID { get; set; }

        // for update
        public string PREFIXNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string CONTACTNUMBER { get; set; }

        // for airnet return response

        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string ReturnOrder { get; set; }
    }
}
