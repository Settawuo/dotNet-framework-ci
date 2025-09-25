using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetAPISendmailQuery : IQuery<GetAPISendmailModel>
    {
        //R22.07 APISendmail 
        public string TransactionID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string corporate_name { get; set; }
        public string message { get; set; }
    }
}
