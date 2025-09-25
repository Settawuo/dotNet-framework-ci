namespace WBBContract.Commands
{
    public class InsertAppointmentTrackingCommand
    {
        public string order_no { get; set; }
        public string id_card_no { get; set; }
        public string non_mobile_no { get; set; }
        public string create_date_zte { get; set; }
        public string appointment_date { get; set; }
        public string appointment_timeslot { get; set; }
        public string client_ip { get; set; }
        public string FullUrl { get; set; }

        public decimal output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
