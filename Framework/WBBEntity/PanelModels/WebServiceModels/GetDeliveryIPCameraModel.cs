using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetDeliveryIPCameraModel
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<GetdeliveryIPCameraList> RETURN_DELIVERY_CURROR { get; set; }
    }
    public class GetdeliveryIPCameraList
    {
        public string delivery_text_thai { get; set; }
        public string delivery_text_eng { get; set; }
    }

}
