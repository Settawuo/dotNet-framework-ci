namespace WBBContract.Commands.ExWebServices.SAPFixedAsset
{
    public class SumitFOAErrorLogCommand
    {

        public string access_number { get; set; }
        public string in_xml_foa { get; set; }
        public string resend_status { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string updated_by { get; set; }
        public string updated_date { get; set; }
        public string updated_desc { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public string p_return_msg { get; set; }
    }
}
