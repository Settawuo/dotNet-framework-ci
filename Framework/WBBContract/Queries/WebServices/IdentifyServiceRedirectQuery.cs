using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class IdentifyServiceRedirectQuery : IQuery<string>
    {
        public string CHANNEL { get; set; }
        public string SERVICE_PROVIDER_NAME { get; set; }
    }
}
