namespace WBBContract.Queries.WebServices
{
    //R23.05 Max E-app term & condition log
    public class InsertFbbConsentLogQuery : IQuery<string>
    {
        public string internet_no { get; set; }
        public string contact_mobile { get; set; }

        public string location_code { get; set; }
        public string asc_code { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string ip_address { get; set; }
        public string type_flag { get; set; }
    }
    //end R23.05 Max E-app term & condition log
}
