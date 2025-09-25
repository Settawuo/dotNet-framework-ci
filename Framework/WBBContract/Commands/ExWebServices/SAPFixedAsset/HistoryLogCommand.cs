using System;

namespace WBBContract.Commands.ExWebServices.SAPFixedAsset
{
    public class HistoryLogCommand : CommandBase
    {
        public long HISTORY_ID { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string IN_FOA { get; set; }
        public string OUT_FOA { get; set; }
        public string INSTALLATION { get; set; }
        public string IN_SAP { get; set; }
        public string OUT_SAP { get; set; }
        public string REQUEST_STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
    }
}
