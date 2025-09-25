using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.FBBShareplex
{
    public class RPAEquipmentReplenishmentToSubQuery : IQuery<RPAEquipmentReplenishmentToSubModel>
    {
        public string PathTempFile { get; set; }
        public string DomainTempFile { get; set; }
        public string UserTempFile { get; set; }
        public string PassTempFile { get; set; }
        public string TargetArchivePathFile { get; set; }
        public string TargetArchiveDomainFile { get; set; }
        public string TargetArchiveUserFile { get; set; }
        public string TargetArchivePassFile { get; set; }
    }
}
