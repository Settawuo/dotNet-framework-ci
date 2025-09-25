namespace WBBContract.Commands
{
    public class SaveChangeStatusBuildingCommand
    {
        public SaveChangeStatusBuildingCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
            this.Return_UpdateDate = "";
        }
        public string ADDRESS_ID { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string FTTR_FLAG { get; set; }
        public string UPDATE_BY { get; set; }
        public string REASON { get; set; }
        public bool IS_SAVE_REASON { get; set; }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public string Return_UpdateDate { get; set; }
    }
}
