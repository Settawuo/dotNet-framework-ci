using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetListFBSSBuildingQuery : CpGateWayQueryBase, IQuery<List<FBSSBuildingModel>>
    {
        [Required]
        public string PostalCode { get; set; }

        public string SubDistrict { get; set; }
    }
}