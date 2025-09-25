namespace FBBQueryOrderPendingPayment.Model
{
    public class QrCodePaymentsInquiryResponse
    {
        public string respCode { get; set; }
        public string respDesc { get; set; }
        public string orderId { get; set; }
        public string tranDtm { get; set; }
        public string tranId { get; set; }
        public string serviceId { get; set; }
        public string qrCodeStr { get; set; }
        public string terminalId { get; set; }
        public string locationName { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
        public string sof { get; set; }
        public string qrType { get; set; }
        public string refundDt { get; set; }
        public string disputeId { get; set; }
        public string disputeStatus { get; set; }
        public string disputeReasonId { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ref3 { get; set; }
        public string ref4 { get; set; }
        public string ref5 { get; set; }
    }
}
