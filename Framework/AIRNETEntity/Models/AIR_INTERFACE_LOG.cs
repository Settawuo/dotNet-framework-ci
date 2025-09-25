using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_INTERFACE_LOG
    {
        public string ORDER_NO { get; set; }
        public string INTERFACE_EVENT { get; set; }
        public DateTime INTERFACE_DTM { get; set; }
        public string INTERFACE_BY { get; set; }
        public string INTERFACE_REQUEST { get; set; }
        public string INTERFACE_DATA { get; set; }
    }
}

