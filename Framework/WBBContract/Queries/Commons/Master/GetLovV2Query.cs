using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Masters
{
    public class GetLovV2Query : IQuery<List<LovValueModel>>
    {
        public string LovName { get; set; }
        public string LovType { get; set; }
        public string LovVal5 { get; set; }
        public string LovVal3 { get; set; }
    }
}
