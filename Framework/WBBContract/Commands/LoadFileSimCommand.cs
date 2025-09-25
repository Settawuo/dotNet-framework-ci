namespace WBBContract.Commands
{
    public class TruncateSIMSlocTempCommand
    {
        public string SHIP_ID { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public int Total { get; set; }

        //Return 
        public string RET_CODE { get; set; }
        public string RET_MSG { get; set; }
    }
}
