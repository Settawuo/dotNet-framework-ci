using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace FBBConfig.Controllers
namespace WBBContract.Commands
{
    public class SaveDSLAMModelCommand
    {
        public SaveDSLAMModelCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
        }

        // for return

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        public decimal DSLAMMODELID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string MODEL { get; set; }
        public string BRAND { get; set; }
        public string SH_BRAND { get; set; }
        public decimal SLOTSTARTINDEX { get; set; }
        public decimal MAXSLOT { get; set; }
        public string DATAONLY_FLANG { get; set; }
        public string ACTIVEFLAG { get; set; }
    }
}
