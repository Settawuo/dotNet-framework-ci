using System.Collections.Generic;

namespace WBBEntity.PanelModels.One2NetModels.InWebServices
{
    public class BatchNoRouterModel
    {
        public List<DetailSubjectRecList> P_RES_DATA1 { get; set; }
        public List<DetailForExcelList> P_RES_DATA2 { get; set; }

        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }

    }

    public class DetailSubjectRecList
    {
        public string P_IP_EMAIL_SERVER { get; set; }
        public string P_SEND_FROM { get; set; }
        public string P_SEND_TO { get; set; }
        public string P_SEND_CC { get; set; }
        public string P_SEND_BCC { get; set; }
        public string P_REGION { get; set; }
        public string P_SUBJECT_NAME { get; set; }
        public string P_BODY_DETAIL { get; set; }


    }

    public class DetailForExcelList
    {
        public string P_FIRST_NAME { get; set; }
        public string P_LAST_NAME { get; set; }
        public string P_CONTACT_PHONE { get; set; }
        public string P_ID_CARD_TYPE { get; set; }
        public string P_ID_CARD_NO { get; set; }
        public string P_DORMITORY_NAME { get; set; }
        public string P_BUILDING_NO { get; set; }
        public string P_FLOOR_NO { get; set; }
        public string P_ROOM_NO { get; set; }
        public string P_FIBRE_ID { get; set; }
        public string P_REGION { get; set; }
    }

    public class DetailResponseBatchList
    {
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
    }
}
