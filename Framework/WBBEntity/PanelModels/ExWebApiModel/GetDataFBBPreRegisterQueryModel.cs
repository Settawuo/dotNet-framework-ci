using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.Models;

namespace WBBEntity.PanelModels.ExWebApiModel
{
    public class GetDataFBBPreRegisterQueryModel
    {
        public string RESULT_CODE { set; get; } // 0 ,-1
        public string RESULT_DESC { set; get; } //FAIL , Success
        public string TRANSACTION_ID { set; get; }
        public int COUNT { set; get; }
        public List<FBB_PRE_REGISTER> PRE_REGITER_LIST { set; get; } = new List<FBB_PRE_REGISTER>();
    }

    public class GetDataFBBPreRegisterQueryLog
    {
        public string RESULT_CODE { set; get; } // 0 ,-1
        public string RESULT_DESC { set; get; } //FAIL , Success
        public string TRANSACTION_ID { set; get; }
        public int COUNT { set; get; }
        public List<FBB_PRE_REGISTER_LOG> PRE_REGITER_LIST { set; get; } = new List<FBB_PRE_REGISTER_LOG>();
    }

    public class FBB_PRE_REGISTER_LOG
    {
        public long PRE_REG_ID { get; set; }
    }
}
