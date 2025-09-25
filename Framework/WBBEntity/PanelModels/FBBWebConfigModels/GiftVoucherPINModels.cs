using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GiftVoucherPINModels
    {
        public string PINCode { get; set; }
        public long PIN_Lot { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> ExpiredDate { get; set; }
    }
}
