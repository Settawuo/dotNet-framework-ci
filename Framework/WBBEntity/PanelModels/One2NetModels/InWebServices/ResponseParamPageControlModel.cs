namespace WBBEntity.PanelModels.One2NetModels.InWebServices
{
    public class ResponseParamPageControlModel
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public int ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string ChannelRefID { get; set; }
        public string Command { get; set; }
        public string Msisdn { get; set; }
        public string AisRefDate { get; set; }
        public string AisRefNo { get; set; }
        public string BankRefNo { get; set; }
        public string PayBankFlag { get; set; }
        public string IssuerBankCode { get; set; }
        public string AcquirerBankCode { get; set; }
        public long CardNo { get; set; }
        public string CardType { get; set; }
        public string IssuerBankName { get; set; }
        public string ShopID { get; set; }
        public int PayTerm { get; set; }
        public long InstallmentRate { get; set; }
        public long AmountPerMonth { get; set; }
        public long TotalAmount { get; set; }
        public string BankResponseCode { get; set; }
        public string SessionID { get; set; }
        public decimal BankReturnAmount { get; set; }
        public decimal IpayReturnAmount { get; set; }

        public int ResultCode { get; set; }
    }
}
