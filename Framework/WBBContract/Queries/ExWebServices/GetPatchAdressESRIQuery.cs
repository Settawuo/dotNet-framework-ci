using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetPatchAdressESRIQuery : IQuery<PatchAdressESRIModel>
    {
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Lang { get; set; }
    }
}
