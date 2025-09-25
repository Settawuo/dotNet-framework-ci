using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetRegisterPendingPaymentByTransactionIDInModel
    {
        public string resultcode { get; set; }
        public List<RegisterPendingPaymentData> RegisterPendingPaymentList { get; set; }
    }

    public class RegisterPendingPaymentData
    {
        public string ais_non_mobile { get; set; }
        public string contact_mobile_phone1 { get; set; }
        public string payment_method { get; set; }
        public string payment_status { get; set; }
        public DateTime created { get; set; }
    }
}
