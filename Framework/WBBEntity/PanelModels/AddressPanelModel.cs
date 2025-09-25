namespace WBBEntity.PanelModels
{
    public class AddressPanelModel : PanelModelBase
    {
        public string L_PROVINCE { get; set; }
        public string L_TUMBOL { get; set; }
        public string L_AMPHUR { get; set; }
        public string L_MOOBAN { get; set; }
        public string L_HOME_NUMBER_1 { get; set; }
        public string L_MOO { get; set; }
        public string L_SOI { get; set; }
        public string L_ROAD { get; set; }
        public string L_ZIPCODE { get; set; }
        public string L_HOME_NUMBER_2 { get; set; }
        public string L_FLOOR { get; set; }
        public string L_ROOM { get; set; }
        public string L_BUILD_NAME { get; set; }
        public string L_BUILD_NAME_Hied { get; set; }
        public string L_BUILD_NO_Hied { get; set; }
        public string ZIPCODE_ID { get; set; }


        // Update 15.3
        public string AddressId { get; set; }

        // Update 16.1
        public string GIFT_VOUCHER { get; set; }
        public string SUB_LOCATION_ID { get; set; }
        public string SUB_CONTRACT_NAME { get; set; }
        public string INSTALL_STAFF_ID { get; set; }
        public string INSTALL_STAFF_NAME { get; set; }

        //20.5 Service Level
        public string Region { get; set; }
    }
}
