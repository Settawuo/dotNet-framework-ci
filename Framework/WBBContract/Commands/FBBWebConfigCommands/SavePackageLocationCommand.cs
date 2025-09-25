using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SavePackageLocationCommand
    {
        public SavePackageLocationCommand()
        {
            this.return_code = -1;
            this.return_msg = "";
        }

        public string service_option { get; set; }
        public string package_code { get; set; }
        public string user { get; set; }

        private List<AirPackageRegion> _AirPackageRegionArray;
        public List<AirPackageRegion> airPackageRegionArray
        {
            get { return _AirPackageRegionArray; }
            set { _AirPackageRegionArray = value; }
        }

        private List<AirPackageProvince> _AirPackageProvinceArray;
        public List<AirPackageProvince> airPackageProvinceArray
        {
            get { return _AirPackageProvinceArray; }
            set { _AirPackageProvinceArray = value; }
        }

        private List<AirPackageBuilding> _AirPackageBuildingArray;
        public List<AirPackageBuilding> airPackageBuildingArray
        {
            get { return _AirPackageBuildingArray; }
            set { _AirPackageBuildingArray = value; }
        }

        // for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }
    }

    public class AirPackageRegion
    {
        public string SERVICE_OPTION { get; set; }
        public string REGION { get; set; }
        public string EFFECTIVE_DTM { get; set; }
        public string EXPIRE_DTM { get; set; }
    }

    public class AirPackageProvince
    {
        public string SERVICE_OPTION { get; set; }
        public string REGION { get; set; }
        public string PROVINCE { get; set; }
        public string EFFECTIVE_DTM { get; set; }
        public string EXPIRE_DTM { get; set; }
    }

    public class AirPackageBuilding
    {
        public string SERVICE_OPTION { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ADDRESS_TYPE { get; set; }
        public string BUILDING_NAME { get; set; }
        public string BUILDING_NO { get; set; }
        public string BUILDING_NAME_E { get; set; }
        public string BUILDING_NO_E { get; set; }
        public string EFFECTIVE_DTM { get; set; }
        public string EXPIRE_DTM { get; set; }
    }
}
