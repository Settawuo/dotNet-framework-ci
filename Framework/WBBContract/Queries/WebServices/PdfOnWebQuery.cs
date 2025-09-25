using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class PdfOnWebQuery : IQuery<PdfOnWebModel>
    {
        public string orderNo { get; set; }
        public string Language { get; set; }
        public string isShop { get; set; }
    }
}
