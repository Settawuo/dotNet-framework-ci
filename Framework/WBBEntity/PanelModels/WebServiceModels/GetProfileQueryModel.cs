namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetProfileQueryModel
    {
        public string username { get; set; }
        public string sub { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string location_code { get; set; }
        public string tax_id { get; set; }
        public string pincode { get; set; }
        public string asc_code { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }

        public string channel_salescode { get; set; }
        public string is_special { get; set; }
    }
}
