using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_CHANNEL_GROUP
    {
        public string PARTNER_TYPE { get; set; }
        public string PARTNER_SUBTYPE { get; set; }
        public string CATALOG_AUTHORIZE { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string UPD_BY { get; set; }
        public DateTime UPD_DTM { get; set; }
    }
}
