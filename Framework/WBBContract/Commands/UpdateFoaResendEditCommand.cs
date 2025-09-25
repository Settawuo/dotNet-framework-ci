namespace WBBContract.Commands
{
    public class UpdateFoaResendEditCommand
    {
        public string p_serial_no { get; set; }
        public string p_order_no { get; set; }
        public string p_trans_id { get; set; }
        public string p_plant { get; set; }
        public string p_storage_location { get; set; }
        public string p_access_number { get; set; }

        public int return_code { get; set; }
        public string return_message { get; set; }

    }
}
