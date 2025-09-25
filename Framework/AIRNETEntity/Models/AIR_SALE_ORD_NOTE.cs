using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_SALE_ORD_NOTE
    {
        public string ORDER_NO { get; set; }
        public Nullable<DateTime> NOTE_DATE { get; set; }
        public string NOTE_USER_NAME { get; set; }
        public string NOTE_DETAIL { get; set; }
        public Nullable<DateTime> UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
    }
}
