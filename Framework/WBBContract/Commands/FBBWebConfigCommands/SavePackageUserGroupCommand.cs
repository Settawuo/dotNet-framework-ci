using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SavePackageUserGroupCommand
    {
        public SavePackageUserGroupCommand()
        {
            this.return_code = -1;
            this.return_msg = "";
        }

        public string service_option { get; set; }
        public string package_code { get; set; }

        private List<AirPackageUserArray> _AirPackageUserArray;
        public List<AirPackageUserArray> airPackageUserArray
        {
            get { return _AirPackageUserArray; }
            set { _AirPackageUserArray = value; }
        }

        // for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }
    }

    public class AirPackageUserArray
    {
        public string SERVICE_OPTION { get; set; }
        public string USER_GROUP { get; set; }
        public string UPD_BY { get; set; }
        public string EFFECTIVE_DTM { get; set; }
        public string EXPIRE_DTM { get; set; }
    }
}
