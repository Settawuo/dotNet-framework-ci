using System.Net;

namespace WBBContract.Commands.WebServices
{
    // R23.05.2023 Created: THOTST49
    public class InsertConsentLogNewRegisterCommand
    {
        public string InTransactionId { set; get; }

        public string ContactMobile { set; get; }

        public string IdentificationNo { set; get; }

        public bool ValueFlag { set; get; }

        public string RefOrderNo { set; get; }

        public string ClientIp { set; get; }


        public int ReturnCode { set; get; } = (int)HttpStatusCode.OK;

        public string ReturnMessage { set; get; } = HttpStatusCode.OK.ToString();
    }
}
