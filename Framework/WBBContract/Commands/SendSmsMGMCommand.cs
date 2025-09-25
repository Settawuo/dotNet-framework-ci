using System.Collections.Generic;

namespace WBBContract.Commands
{
    public class SendSmsMGMCommand
    {
        public SendSmsMGMCommand()
        {
            this.return_code = -1;
            this.return_message = "";

            if (p_send_sms_mgm == null)
                p_send_sms_mgm = new List<SendSmsMGMModel>();
        }

        public string ClientIP { get; set; }
        public string FullUrl { get; set; }

        //in
        public string p_refference_no { get; set; }
        public string p_coverage_result { get; set; }
        public string p_mgm_flag { get; set; }
        public string p_language { get; set; }

        //out
        public decimal return_code { get; set; }
        public string return_message { get; set; }
        public List<SendSmsMGMModel> p_send_sms_mgm { get; set; }
    }

    public class SendSmsMGMModel
    {
        public string refference_no { get; set; }
        public string mobile { get; set; }
        public string message_sms { get; set; }
    }
}
