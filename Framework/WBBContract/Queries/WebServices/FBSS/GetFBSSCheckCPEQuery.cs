using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices.FBSS
{
    public class GetFBSSCheckCPEQuery : IQuery<List<FBSSCheckCPEModel>>
    {
        public string CPE { get; set; }
        public string playbox { get; set; }

        public string IN_ID_CARD_NO { get; set; }

        // Update 17.2
        public string Transaction_Id { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }

        //R21.2
        public string EndpointService { get; set; }
    }

}
