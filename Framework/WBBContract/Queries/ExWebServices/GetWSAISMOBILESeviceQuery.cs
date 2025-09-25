namespace WBBContract.Queries.ExWebServices
{
    public class GetWSAISMOBILESeviceQuery : IQuery<string>
    {
        public string Username { get; set; }
        public string OrderRef { get; set; }
        public string OrderDesc { get; set; }
        public string Msisdn { get; set; }
        public string opt1 { get; set; }
        public string opt2 { get; set; }
        public string out_IsSuccess { get; set; }
        public string out_Code { get; set; }
        public string out_TransactionID { get; set; }
        public string out_Description { get; set; }
        public string ou_OrderRef { get; set; }
        public string LanguageSender { get; set; }
        public string User { get; set; }
        public string ResultID { get; set; }
        public string SffProfileLogID { get; set; }

    }


}
