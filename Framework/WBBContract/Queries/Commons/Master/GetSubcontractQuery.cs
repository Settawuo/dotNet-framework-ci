using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetSubcontractQuery : IQuery<List<DropdownModel>>
    {
        public string language { get; set; }
        public string netnumber { get; set; }
    }
}
