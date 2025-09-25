using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GenerateTokenQuery : IQuery<GenerateTokenModel>
    {
        public string contents { get; set; }
        public string jsonStrForlog { get; set; } = "";
        public string url_privitege { get; set; } = "";
        public string flag { get; set; } = "";
        public string accessTokenName { get; set; }
        public string fullURL { get; set; } = "";
        public string mobileNo { get; set; }
        public ParametertInterfaceLog parameter { get; set; } = new ParametertInterfaceLog();
        public GenerateTokenParam genTokenParam { get; set; }

    }

    public class ParametertInterfaceLog
    {

        public string transactionId { get; set; } = "";
        public string methodName { get; set; } = "";
        public string serviceName { get; set; } = "";
        public string InIDCardNO { get; set; } = "";
        public string InterfaceNode { get; set; } = "";
        public string createBy { get; set; } = "";
    }

    public class GenerateTokenParam
    {
        public string user { get; set; } = "";
        public string pass { get; set; } = "";
    }

}
