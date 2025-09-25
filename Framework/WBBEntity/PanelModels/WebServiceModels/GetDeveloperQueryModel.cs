using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetDeveloperQueryModel
    {
        public GetDeveloperQueryModel()
        {
            if (P_RES_DATA == null)
                P_RES_DATA = new List<DETAIL_DEV_CUR>();
        }
        public string P_RETURN_CODE { get; set; }

        public string P_RETURN_MESSAGE { get; set; }
        public List<DETAIL_DEV_CUR> P_RES_DATA { get; set; }
    }
    public class DETAIL_DEV_CUR
    {
        public decimal ID { get; set; }
        public string LOV_VAL_TH { get; set; }
        public string LOV_VAL_EN { get; set; }
    }
}
