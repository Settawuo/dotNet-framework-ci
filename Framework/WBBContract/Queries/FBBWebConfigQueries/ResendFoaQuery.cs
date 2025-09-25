using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class ResendFoaQuery : IQuery<List<ResendFoaModel>>
    {
        public string Acess_no { get; set; }
        public string resend_status { get; set; }

        public int flag { get; set; }
        //public string IN_XML_FOA { get; set; }
    }

    public class LostTranQuery : IQuery<List<LosttranModel>>
    {
        public string Acess_no { get; set; }
        public string Date { get; set; }


    }
}
