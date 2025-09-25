using System;

namespace WBBContract.Commands
{
    public class SaveCardModelCommand
    {
        public SaveCardModelCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
        }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public decimal CARDMODELID { get; set; }
        public decimal POSTSTARTINDEX { get; set; }
        public decimal MAXSLOT { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public decimal RESERVEPORTSPARE { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string MODEL { get; set; }
        public string BRAND { get; set; }
        public decimal RESERVE { get; set; }
        public string ResultCommand { get; set; }
        public string DATAONLY_FLANG { get; set; }



    }
}
