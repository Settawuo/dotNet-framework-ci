using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Masters
{
    public class GetLovWithParamsQuery : IQuery<List<LovValueModel>>
    {
        public string LovName { get; set; }
        public string LovType { get; set; }
        public string LovValue1 { get; set; }
        public string LovValue2 { get; set; }
        public string LovValue3 { get; set; }
        public string LovValue4 { get; set; }
        public string LovValue5 { get; set; }
    }
}
