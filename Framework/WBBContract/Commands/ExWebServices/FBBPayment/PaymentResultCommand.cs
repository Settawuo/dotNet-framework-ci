namespace WBBContract.Commands.ExWebServices.FBBPayment
{
    public class PaymentResultCommand
    {
        private string status { get; set; }
        private string respCode { get; set; }
        private string respDesc { get; set; }
        private string tranId { get; set; }
        private string saleId { get; set; }
        private string orderId { get; set; }
        private string currency { get; set; }
        private string exchangeRate { get; set; }
        private string purchaseAmt { get; set; }
        private string amount { get; set; }
        private string incCustomerFee { get; set; }
        private string excCustomerFee { get; set; }
        private string paymentStatus { get; set; }
        private string paymentCode { get; set; }
        private string orderExpireDate { get; set; }
        private string custEmail { get; set; }
        private string shipName { get; set; }
        private string shipAddress { get; set; }
        private string shipProvince { get; set; }
        private string shipZip { get; set; }
        private string shipCountry { get; set; }
        private string remark1 { get; set; }
        private string remark2 { get; set; }
        private string integrityStr { get; set; }
        private string customerId { get; set; }
    }
}
