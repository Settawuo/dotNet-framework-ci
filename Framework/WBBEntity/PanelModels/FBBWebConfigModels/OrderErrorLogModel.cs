using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{

    public class OrderErrorLogModel
    {
        public int P_PAGE_COUNT { get; set; }
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
        public List<OrderErrorLogList> P_RES_DATA { get; set; }
    }

    public class OrderErrorLogList
    {

        public string IN_ID_CARD_NO { get; set; }
        public string IN_XML_PARAM { get; set; }
        public string REQUEST_STATUS { get; set; }
        public string CREATED_DATE { get; set; }
        public string UPDATED_DATE { get; set; }
        public int NO_OF_SEND { get; set; }
        public int RowNumber { get; set; }
    }

    public class StatusLogDropdown
    {

        public string TEXT { get; set; }
        public string VALUE { get; set; }

    }

    public partial class FB_Interfce_log_byServiceModel
    {

        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
        public List<GetXML_PARAM> P_IN_XML_PARAM { get; set; }
    }

    public class GetXML_PARAM
    {
        public string IN_XML_PARAM { get; set; }

    }
}

