using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetFloorsQuery : IQuery<List<DropdownModel>>
    {
        public string language { get; set; }
        public string DormID { get; set; }
        public string DormName { get; set; }
        public string DormNO { get; set; }
        public string Netnumber { get; set; }
    }

}
