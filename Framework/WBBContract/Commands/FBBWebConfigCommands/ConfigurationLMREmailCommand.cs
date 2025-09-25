using System;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class ConfigurationLMREmailCommand
    {
        public string Text { get; set; }
        public decimal Id { get; set; }
        public string LovValue1 { get; set; }
        public string updated_by { get; set; }
        public DateTime? updated_date { get; set; }
        // respornt
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }
}
