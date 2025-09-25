namespace WBBEntity.PanelModels
{
    public class LeavemessagePanelModel : PanelModelBase
    {
        public LeavemessagePanelModel()
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


        //20.2
        public bool WTTX_FULL { get; set; }
        public string WTTX_LOCATION { get; set; }

        //new leavemessage
        public string COMPANY_NAME { get; set; }

    }
}
