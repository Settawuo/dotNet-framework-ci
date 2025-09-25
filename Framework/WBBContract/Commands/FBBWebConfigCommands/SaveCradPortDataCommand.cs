using System;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveCradPortDataCommand
    {

        public decimal CradNumber { get; set; }
        public string CardModel { get; set; }
        public string CardType { get; set; }
        public string RESERVE_TECHNOLOGY { get; set; }
        public decimal CardModelID { get; set; }
        public decimal CARDID { get; set; }
        public decimal DSalamModelID { get; set; }
        public DateTime CRATE_DATE { get; set; }
        public string UPDATEBY { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string Return_Desc { get; set; }
        public decimal Return_Code { get; set; }
        public string ResultCommand { get; set; }
        public string Node_ID { get; set; }
        public bool? FlagDup { get; set; }
        public bool? FlagUpdate { get; set; }
        public string Building { get; set; }

    }




}
