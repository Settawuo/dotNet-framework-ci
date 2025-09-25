using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class MeshParameterReserveModel
    {
        public MeshParameterReserveModel()
        {
            if (RES_COMPLETE_CUR == null)
                RES_COMPLETE_CUR = new List<ParameterReserve>();
        }

        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<ParameterReserve> RES_COMPLETE_CUR { get; set; }
    }

    public class ParameterReserve
    {
        public string APPOINTMENT_DATE { get; set; }
        public string TIME_SLOT { get; set; }
        public string ACCESS_MODE { get; set; }
        public string PROD_SPEC_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string SUBDISTRICT { get; set; }
        public string POSTCODE { get; set; }
        public string ASSIGN_RULE { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
        public string TASK_TYPE { get; set; }

        //R20.5 add by Aware : Atipon Wiparsmongkol
        public string LIST_SERVICE_LEVEL { get; set; }
        public string LIST_AREA_REGION { get; set; }
        public string LIST_EVENT_RULE { get; set; }
        public string LIST_SPECIAL_SKILL { get; set; }
    }
}
