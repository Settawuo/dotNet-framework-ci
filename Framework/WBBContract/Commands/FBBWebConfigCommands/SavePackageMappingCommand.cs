using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SavePackageMappingCommand
    {
        public SavePackageMappingCommand()
        {
            this.return_code = -1;
            this.return_msg = "";
        }

        public string service_option { get; set; }
        public string package_code { get; set; }

        private List<AirPackageMapping> _AirPackageMappingArray;
        public List<AirPackageMapping> airPackageMappingArray
        {
            get { return _AirPackageMappingArray; }
            set { _AirPackageMappingArray = value; }
        }

        // for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }
    }

    public class AirPackageMapping
    {
        public string SERVICE_OPTION { get; set; }
        public string MAPPING_CODE { get; set; }
        public string MAPPING_PRODUCT { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string UPD_BY { get; set; }
        public string EFFECTIVE_DTM { get; set; }
        public string EXPIRE_DTM { get; set; }

    }
}
