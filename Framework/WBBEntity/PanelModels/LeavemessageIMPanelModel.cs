namespace WBBEntity.PanelModels
{
    public class LeavemessageIMPanelModel : PanelModelBase
    {
        public LeavemessageIMPanelModel()
        {
            this.CustomerRegisterPanelModel = new CustomerRegisterPanelModel();
        }
        public CustomerRegisterPanelModel CustomerRegisterPanelModel { get; set; }

        public string SELECT_SERVICE { get; set; }
        public string NAME { get; set; }
        public string SURNAME { get; set; }
        public string CONTACT_MOBILE { get; set; }
        public string MOBILE_IS_AIS { get; set; }
        public string EMAIL { get; set; }
        public string INSTALL_ADDRESS { get; set; }
        public string PROVINCE { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string POSTAL_CODE { get; set; }
        public string SELECT_ADDRESS_TYPE { get; set; }
        public string BUILDING { get; set; }
        public string VILLAGE { get; set; }
        public string HOUSE_NO { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string CONTACT_TIME { get; set; }
        public string INTERNET_NO { get; set; }
        public string SELECT_TYPE_BUILD { get; set; }
        public string LOCATION { get; set; }
        public string FULL_ADDRESS { get; set; }

        public string LOCATION_CODE { get; set; }
        public string ASC_CODE { get; set; }
        public string EMP_ID { get; set; }
        public string SALES_REP { get; set; }
        public string ASSET_NUMBER { get; set; }
        public string SERVICE_CASE_ID { get; set; }
        public string PRODUCT_TYPE_BUILDING { get; set; }
        public string PRODUCT_TYPE_HOME { get; set; }
        public string BUILDING_NO { get; set; }
        public string FLOOR { get; set; }
        public string MOO { get; set; }


    }
}
