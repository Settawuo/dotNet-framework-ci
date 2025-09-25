using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckChangePackageOntopQuery : IQuery<CheckChangePackageOntopModel>
    {
        public string p_non_mobile_no { get; set; }

        private List<AirChangePackage> _AirChangeOldPackageArray;
        public List<AirChangePackage> AirChangeOldPackageArray
        {
            get { return _AirChangeOldPackageArray; }
            set { _AirChangeOldPackageArray = value; }
        }


        private List<AirChangePackage> _AirChangeNewPackageArray;
        public List<AirChangePackage> AirChangeNewPackageArray
        {
            get { return _AirChangeNewPackageArray; }
            set { _AirChangeNewPackageArray = value; }

        }

        //return
        public string o_result { get; set; }
        public string o_errorreason { get; set; }

    }

    public class AirChangePackage
    {
        public string sff_promotion_code { get; set; }
        public string startdt { get; set; }
        public string enddt { get; set; }
        public string product_seq { get; set; }
    }

}
