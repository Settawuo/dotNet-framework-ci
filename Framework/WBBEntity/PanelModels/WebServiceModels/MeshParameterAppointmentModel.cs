using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class MeshParameterAppointmentModel
    {
        public MeshParameterAppointmentModel()
        {
            if (RES_COMPLETE_CUR == null)
                RES_COMPLETE_CUR = new List<ParameterAppointment>();
        }

        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<ParameterAppointment> RES_COMPLETE_CUR { get; set; }
    }

    public class ParameterAppointment
    {
        public string INSTALLATION_DATE { get; set; }
        public string ACCESS_MODE { get; set; }
        public string PROD_SPEC_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string TASK_TYPE { get; set; }
        public string DAYS { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SUBDISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string PROVINCE { get; set; }
        public string POSTCODE { get; set; }
        public string SUB_ACCESS_MODE { get; set; }

        //R20.5 add by Aware : Atipon Wiparsmongkol
        public string LIST_SERVICE_LEVEL { get; set; }
        public string LIST_AREA_REGION { get; set; }
        public string LIST_EVENT_RULE { get; set; }
        public string LIST_SPECIAL_SKILL { get; set; }
        //R23.9
        public string TIME_SLOT_REGISTER_HR { get; set; }
        public string TIME_SLOT_REGISTER_FLOWACTION_NO { get; set; }

    }
}
