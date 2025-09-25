namespace WBBEntity.PanelModels.WebServiceModels
{
    public class RequestsGSSOServiceModel
    {
        public string GSSOContent { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class SendOneTimePWModel
    {
        public SendOneTimePW sendOneTimePW { get; set; }
    }

    public class SendOneTimePW
    {
        public string msisdn { get; set; }
        public string otpChannel { get; set; }
        public string service { get; set; }
        public string accountType { get; set; }
        public string lifeTimeoutMins { get; set; }
        public string waitDR { get; set; }
        public string otpDigit { get; set; }
        public string refDigit { get; set; }
    }

    public class SendOneTimePWResponseModel
    {
        public SendOneTimePWResponse sendOneTimePWResponse { get; set; }
    }

    public class SendOneTimePWResponse
    {
        public string code { get; set; }
        public string description { get; set; }
        public string isSuccess { get; set; }
        public string orderRef { get; set; }
        public string transactionID { get; set; }
        public string referenceNumber { get; set; }
        public string operName { get; set; }
        public string lifeTimeoutMins { get; set; }
        public string expirePassword { get; set; }
        public string oneTimePassword { get; set; }
    }

    public class ConfirmOneTimePWModel
    {
        public ConfirmOneTimePW confirmOneTimePW { get; set; }
    }

    public class ConfirmOneTimePW
    {
        public string msisdn { get; set; }
        public string pwd { get; set; }
        public string transactionID { get; set; }
        public string service { get; set; }
    }

    public class ConfirmOneTimePWResponseModel
    {
        public ConfirmOneTimePWResponse confirmOneTimePWResponse { get; set; }
    }

    public class ConfirmOneTimePWResponse
    {
        public string code { get; set; }
        public string description { get; set; }
        public string isSuccess { get; set; }
        public string orderRef { get; set; }
        public string operName { get; set; }
        public string expirePassword { get; set; }
        public string transactionID { get; set; }
    }
}
