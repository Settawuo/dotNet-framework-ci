using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetMappingSbnOwnerProd : IQuery<string>
    {
        private List<FBSSAccessModeInfo> fbssAccessModeInfo;
        public List<FBSSAccessModeInfo> FBSSAccessModeInfo
        {
            get { return fbssAccessModeInfo; }
            set { fbssAccessModeInfo = value; }
        }

        public string IsPartner { get; set; }
        public string PartnerName { get; set; }
    }
}