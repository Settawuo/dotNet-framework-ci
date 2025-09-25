using System;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class GenerateGiftVoucherPINsCommand
    {
        public int pin_length { get; set; }
        public int pin_type { get; set; }
        public long voucher_master_id { get; set; }
        public long lot { get; set; }
        public string exceptedChar { get; set; }
        public string fixedChar { get; set; }

        public Nullable<DateTime> start_date { get; set; }
        public Nullable<DateTime> expired_date { get; set; }

        public int AmountPINs { get; set; }
    }
}
