namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class FTTxCommand
    {
        public string Action { get; set; }

        public string OwnerProduct { get; set; }
        public string OwnerType { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string GroupAmphur { get; set; }

        public string Username { get; set; }
        public string Region { get; set; }
        public bool? FlagDup { get; set; }
        public decimal Fttx_id { get; set; }
        public string OldOwnerProduct { get; set; }
        public string OldOwnerType { get; set; }
        public string tower_th { get; set; }
        public string tower_en { get; set; }
        public string Service_Type { get; set; }
        public string OnTarget_date { get; set; }

        /// new 
        public string Tumbon { get; set; }
        public string ProvinceEN { get; set; }
        public string AmphurEN { get; set; }
        public string TumbonEN { get; set; }
        public string tagetdate_ex { get; set; }
        public string targetdate_in { get; set; }
        public string status { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string zipcode { get; set; }

    }
}
