using System.ComponentModel.DataAnnotations;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using WBBEntity.PanelModels.ExWebServiceModels;

    public class GetBlackListQuery : CpGateWayQueryBase, IQuery<evOMServiceIVRCheckBlackListModel>
    {
        [Required]
        public string IDCardNo { get; set; }
    }
}
