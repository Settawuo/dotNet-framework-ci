using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class DetailOrderFeeTopupReplaceModel
    {
        public DetailOrderFeeTopupReplaceModel()
        {
            if (RETURN_PRICE_CURROR == null)
                RETURN_PRICE_CURROR = new List<PriceATV>();
        }

        public int RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<PriceATV> RETURN_PRICE_CURROR { get; set; }
    }

    public class PriceATV
    {
        public string DISPLAY_ORDER_FEE { get; set; }
        public string PRICE_ORDER_FEE { get; set; }
        public string DISPLAY_ORDER_FEE_ENG { get; set; }
    }
}
