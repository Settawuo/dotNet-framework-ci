using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using WBBEntity.PanelModels;

    public class GetOrderDuplicateQuery : CpGateWayQueryBase, IQuery<List<OrderDupModel>>
    {
        [Required]
        public string IDCardNo { get; set; }

        [Required]
        public string Language { get; set; }
    }
}
