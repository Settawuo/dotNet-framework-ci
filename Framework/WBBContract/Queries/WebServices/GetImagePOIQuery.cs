using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetImagePOIQuery : IQuery<ImageResultModel>
    {
        public CoveragePictureModel model { get; set; }

    }

}
