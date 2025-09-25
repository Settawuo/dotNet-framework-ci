using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBSS
{
    public class GetbuildingChangeQuery : IQuery<List<DropdownModel>>
    {
        public string Province { get; set; }
        public string Aumphur { get; set; }
        public string Tumbon { get; set; }
        public string Typeaddress { get; set; }
        public string Language { get; set; }
        public string AccessMode { get; set; }
        public string LOC_CODE { get; set; }
        public string ReloadCache { get; set; }
    }

    public class GetbuildingAllQuery : IQuery<List<DropdownModel>>
    {
        public string Typeaddress { get; set; }
        public string Language { get; set; }
    }
}
