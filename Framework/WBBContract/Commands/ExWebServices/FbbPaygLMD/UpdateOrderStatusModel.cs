using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WBBContract.Commands.ExWebServices.FbbPaygLMD
{
    public class UpdateOrderStatusCommand : UpdateOrderStatus
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }



    [DataContract]
    public class UpdateOrderStatus
    {
        [DataMember(Order = 1)]
        public string Status { get; set; }

        [DataMember(Order = 2)]
        public string Transaction_ID { get; set; }

        [DataMember(Order = 3)]
        public string Update_By { get; set; }

        [DataMember(Order = 4)]
        public string Update_Date { get; set; }
        [DataMember(Order = 5)]
        public List<Detail_Invoice> Invoice_List { get; set; }
    }
    public class Detail_Invoice
    {
        public string Invoice_no { get; set; }
    }

    public class UpdateOrderStatusResponse
    {
        public string Result_code { get; set; }
        public string Result_message { get; set; }
    }
}
