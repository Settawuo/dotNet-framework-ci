using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.ExWebApiModel;

namespace WBBContract.Queries.ExWebApi
{
    public class GetDataFBBPreRegisterQuery : IQuery<GetDataFBBPreRegisterQueryModel>
    {
        public string TRANSACTION_ID { set; get; }
    }
}
