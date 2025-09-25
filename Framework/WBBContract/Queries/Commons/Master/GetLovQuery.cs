using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Masters
{
    public class GetLovQuery : IQuery<List<LovValueModel>>
    {
        public string LovName { get; set; }
        public string LovType { get; set; }
        public bool IgonreFlag { get; set; }
    }
}
