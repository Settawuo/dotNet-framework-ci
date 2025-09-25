using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{


    public class ReverseAssetQuery : IQuery<List<ReverseAssetModel>>
    {


        public string p_ACCESS_NO { get; set; }
        public string p_ASSET_CODE { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }


    public class ReverseAssetSapQuery : IQuery<ReturnReverseSapModel>
    {
        public List<ReverseAssetAcessNumber> p_ACCESS_list { get; set; }
        public string p_USER_CODE { get; set; }
        // out put
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }
}

