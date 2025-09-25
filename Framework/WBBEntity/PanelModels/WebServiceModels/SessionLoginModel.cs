using System;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class SessionLoginModel
    {
        public string CustInternetNum { get; set; }
        public string SessionId { get; set; }
        public DateTime? LoginDate { get; set; }
    }
}
