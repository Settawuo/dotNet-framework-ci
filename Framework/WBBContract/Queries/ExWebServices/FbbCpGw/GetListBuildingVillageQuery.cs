using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetListBuildingVillageQuery : CpGateWayQueryBase, IQuery<List<ListBuildingVillageModel>>
    {
        [Required]
        public string Language { get; set; }
        [Required]
        public string AddressType { get; set; }
        public string AddressId { get; set; }
    }
}