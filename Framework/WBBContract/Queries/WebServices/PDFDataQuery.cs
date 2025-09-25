using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class PDFDataQuery : IQuery<PDFData>
    {
        public string orderNo { get; set; }
        public string Language { get; set; }
        public string isShop { get; set; }
        public bool isEApp { get; set; }
        public int pageNo { get; set; }
    }
}
