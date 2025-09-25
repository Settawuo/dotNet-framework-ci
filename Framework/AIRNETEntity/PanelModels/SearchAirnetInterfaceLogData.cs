using System.Collections.Generic;

namespace AIRNETEntity.PanelModels
{
    public class SearchAirnetInterfaceLogData
    {
        public SearchAirnetInterfaceLogData()
        {
            this.AirInterfaceLogData = new List<SearchAirnetInterfaceLog>();
        }

        public List<SearchAirnetInterfaceLog> AirInterfaceLogData { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class SearchAirnetInterfaceLog
    {
        public string ORDER_INDEX { get; set; }
        public string ORDER_NO { get; set; }
        public string INTERFACE_EVENT { get; set; }
        public string INTERFACE_DTM { get; set; }
        public string INTERFACE_BY { get; set; }
        public string INTERFACE_REQUEST { get; set; }
        public string INTERFACE_DATA { get; set; }
        public decimal ALL_RECORDS { get; set; }
    }
}
