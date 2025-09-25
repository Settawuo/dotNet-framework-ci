using System;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class UpdateAWCconfigCommand
    {
        public bool? FlagDup { get; set; }
        public decimal AP_ID { get; set; }
        public string AP_NAME { get; set; }
        public string SECTOR { get; set; }
        public decimal SITE_ID { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string user { get; set; }
    }
}
