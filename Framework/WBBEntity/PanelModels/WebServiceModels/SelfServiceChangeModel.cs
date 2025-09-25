using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class SelfServiceChangeModel
    {
        public SelfServiceChangeModel()
        {
            if (RET_DATA == null)
            {
                RET_DATA = new List<ListTimeSlotModel>();
            }
        }
        public decimal RET_CODE { get; set; }
        public string RET_MSG { get; set; }
        public List<ListTimeSlotModel> RET_DATA { get; set; }
    }
    public class SetSessionSelf
    {
        public string session_name { get; set; }
        public string session_data { get; set; }
    }
    public class ListTimeSlotModel
    {
        public string ORDER_NO { get; set; }
        public string ID_CARD_NO { get; set; }
        public string TAX_ID { get; set; }
        public string ID_CARD_TYPE { get; set; }
        public string Mobile_No { get; set; }
        public string INSTALL_DATE { get; set; }
        public string TIME_SLOT { get; set; }
        public string INSTALLATION_CAPACITY { get; set; }
        public string INTERFACE_STATUS { get; set; }
        public string INTERFACE_RESULT { get; set; }
        public string ENG_FLAG { get; set; }
        public string DATE_NOW { get; set; }
        public string ACCESS_MODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string PROVINCE { get; set; }
        public string AMPHUR { get; set; }
        public string TUMBON { get; set; }
        public string ZIPCODE { get; set; }
        public string PRODUCTSPECCODE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string REGISTER_DATE { get; set; }
        public string BEFORE_DATE { get; set; }
        public string BEFORE_TIME { get; set; }
        public string LOCATION_CODE { get; set; }
        public string RESERVED_ID { get; set; }
        public string ASSIGN_RULE { get; set; }

        //sent model
        public string FBSSAPPOINTMENTDATE { get; set; }//2017-07-23
        public string FBSSTIMESLOT { get; set; } //10:00-12:00
        public string FBSSINSTALLATIONCAPACITY { get; set; } //17/19
        public string FBSSINSTALLDATE { get; set; } //23-07-2017
        public string FIBRENET_ID { get; set; }

        public string SFF_CA_NUMBER { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }

        public string L_MESSAGE_VALID_CHANGE_INSTALL { get; set; }
        public string L_MESSAGE_VALID_MOBILE { get; set; }

        public string CHANGE_INSTALL_COUNT { get; set; }
        public string TEST_DATE { get; set; }

        public string LIST_SERVICE_LEVEL { get; set; }
        public string LIST_AREA_REGION { get; set; }
        public string LIST_EVENT_RULE { get; set; }
    }
}
