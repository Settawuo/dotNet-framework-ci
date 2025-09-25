using System.Collections.Generic;
using WBBContract.Queries.ExWebServices;

namespace WBBContract.Commands
{
    public class AssignVOIPIPCommand
    {

        public AssignVOIPIPCommand()
        {
            this.RETURN_CODE = -1;
            this.RETURN_DESC = "";
        }


        public List<GetAssign_VOIP_IPListQuery> SBNResponse { get; set; }

        public string REFF_USER { get; set; }
        public int RETURN_CODE { get; set; }
        public string RETURN_DESC { get; set; }
        public string REFF_KEY { get; set; }

        public class RESULTModel
        {
            public string IP { get; set; }
            public string PORT { get; set; }
        }

        public RESULTModel RESULT { get; set; }
    }

}

