using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetRoomNumberQuery : IQuery<List<DropdownModel>>
    {
        public string language { get; set; }
        public string filteroom { get; set; }
        public string Netnumber { get; set; }
    }
}
