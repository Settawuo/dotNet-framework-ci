using System.Collections.Generic;
using WBBEntity.Models;
using WBBContract.Commands.ExWebServices.FbbCpGw;

namespace WBBExternalAPI.Models.Response.FbbMasterDataController
{
    public class GetDataFBBPreRegisterQueryResponse
    {
        public string RESULT_CODE { set; get; }
        public string RESULT_DESC { set; get; }
        public string TRANSACTION_ID { set; get; }
        public int COUNT { set; get; }
        public List<FBB_PRE_REGISTER> PRE_REGITER_LIST { set; get; } = new List<FBB_PRE_REGISTER>();
    }
}
