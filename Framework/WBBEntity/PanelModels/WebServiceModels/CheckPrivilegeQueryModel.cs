using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class CheckPrivilegeQueryModel
    {
        public string TransactionID { get; set; }
        public string HttpStatus { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Msg { get; set; }
    }

    public class CheckPrivilegePointQueryModel
    {
        public string TransactionID { get; set; }
        public string HttpStatus { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<PrivilegePoint> PrivilegePointList { get; set; }
    }

    public class PrivilegePoint
    {
        public string msisdn { get; set; }
        public decimal points { get; set; }
        public decimal pointsBonus { get; set; }
    }

    public class PrivilegeRedeemPointQueryModel
    {
        public string TransactionID { get; set; }
        public string HttpStatus { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string MsgBarcode { get; set; }
        public string Msg { get; set; }
    }
}
