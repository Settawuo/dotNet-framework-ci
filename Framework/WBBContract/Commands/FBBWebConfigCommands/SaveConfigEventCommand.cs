using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveConfigEventCommand
    {
        public SaveConfigEventCommand()
        {
            this.return_code = -1;
            this.return_msg = "";
        }

        public string service_option { get; set; }
        public string event_code { get; set; }
        public string effective_date { get; set; }
        public string expire_date { get; set; }
        public string user { get; set; }
        public string technology { get; set; }
        public string provice { get; set; }
        public string amphur { get; set; }
        public string tumbon { get; set; }
        public string zipcode { get; set; }
        public string plug_and_play_flag { get; set; }

        private List<FbbEventSub> _FbbEventSubArray;
        public List<FbbEventSub> fbbEventSubArray
        {
            get { return _FbbEventSubArray; }
            set { _FbbEventSubArray = value; }
        }

        // for return
        public decimal return_code { get; set; }
        public string return_msg { get; set; }
    }

    public class FbbEventSub
    {
        public string SERVICE_OPTION { get; set; }
        public string EVENT_CODE { get; set; }
        public string SUB_LOCATION_ID { get; set; }
        public string SUB_CONTRACT_NAME { get; set; }
        public string SUB_TEAM_ID { get; set; }
        public string SUB_TEAM_NAME { get; set; }
        public string INSTALL_STAFF_ID { get; set; }
        public string INSTALL_STAFF_NAME { get; set; }
        public string EVENT_START_DATE { get; set; }
        public string EVENT_END_DATE { get; set; }
        public string SUB_ROW_ID { get; set; }
    }
}
