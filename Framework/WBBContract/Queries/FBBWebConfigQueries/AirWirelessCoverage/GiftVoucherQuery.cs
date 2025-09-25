using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GiftVoucherQuery : IQuery<List<GiftVoucherPINModels>>
    {
        public int pin_length { get; set; }
        public int pin_type { get; set; }
        public long pin_lot { get; set; }
        public string exceptedChar { get; set; }
        public string fixedChar { get; set; }
        public int fixedPosition { get; set; }

        public Nullable<DateTime> start_date { get; set; }
        public Nullable<DateTime> expired_date { get; set; }

        public int AmountPINs { get; set; }
    }
}
