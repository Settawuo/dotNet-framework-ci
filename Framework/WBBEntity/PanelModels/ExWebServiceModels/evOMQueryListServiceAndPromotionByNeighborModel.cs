using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class evOMQueryListServiceAndPromotionByNeighborModel
    {
        public evOMQueryListServiceAndPromotionByNeighborModel()
        {
            if (productCDContent == null)
                productCDContent = new List<string>();
        }
        public string resultFlag { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> productCDContent { get; set; }

    }
}
