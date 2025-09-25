using System.Collections.Generic;
using WBBContract.Queries.Commons;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class ZTEReservedTimeslotQuery : IQuery<ZTEReservedTimeslotRespModel>
    {
        public ZTEReservedTimeslotQuery()
        {
            ASSIGN_CONDITION_LIST = new List<ASSIGN_CONDITION_ATTR>();
        }

        public string APPOINTMENT_DATE { get; set; }
        public string TIME_SLOT { get; set; }
        public string ACCESS_MODE { get; set; }
        public string PROD_SPEC_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string SUBDISTRICT { get; set; }
        public string POSTAL_CODE { get; set; }
        public string ASSIGN_RULE { get; set; }
        public string TRANSACTION_ID { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }
        public string ID_CARD_NO { get; set; }

        // Update 18.8
        public string SUB_ACCESS_MODE { get; set; }

        public string TASK_TYPE { get; set; }

        //20.5 Service Level
        public List<ASSIGN_CONDITION_ATTR> ASSIGN_CONDITION_LIST { get; set; }
        public string AssignFlag { get; set; }
        public string AisAirNumber { get; set; }
        public string PlayBoxCountOld { get; set; }
        public string PlayBoxCountNew { get; set; }

        //R22.03 Topup Replace
        public string SymptonCodePBreplace { get; set; }
    }
}
