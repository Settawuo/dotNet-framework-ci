using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class SubmitFOAResendOrderNewQuery : IQuery<ResendOrderGetData>
    {
        public List<SubmitFOAResenorderdata> list_p_get_oder { get; set; }
        public string tab_name { get; set; }

        //return value
        public string Return_Code { get; set; }
        public string Return_Message { get; set; }
        public string Return_Transactions { get; set; }
        public List<string> item_json { get; set; }
    }
}
