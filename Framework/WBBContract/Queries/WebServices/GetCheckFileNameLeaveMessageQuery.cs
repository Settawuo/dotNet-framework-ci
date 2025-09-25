using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCheckFileNameLeaveMessageQuery : IQuery<CheckFileNameLeaveMessageModel>
    {
        public string p_file_name { get; set; }
        public string p_user_name { get; set; }

        public decimal return_code { get; set; }
        public string return_message { get; set; }
    }
}
