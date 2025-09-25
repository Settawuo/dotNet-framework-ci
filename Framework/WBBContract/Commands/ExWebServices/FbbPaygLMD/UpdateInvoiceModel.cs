using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WBBContract.Commands.ExWebServices.FbbPaygLMD
{
    #region MyRegion

    public class UpdateInvoiceCommand : UpdateInvoice
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
    [DataContract]
    public class UpdateInvoice
    {

        [DataMember(Order = 1)]
        [Required]
        public string Action { get; set; }
        [DataMember(Order = 2)]
        [Required]
        public string Transaction_ID { get; set; }
        [DataMember(Order = 3)]
        public string Invoice_no_Old { get; set; }
        [DataMember(Order = 4)]
        public string Invoice_no_New { get; set; }
        [DataMember(Order = 5)]
        [Required]
        public string Invoice_date { get; set; }
        [DataMember(Order = 6)]
        [Required]
        public string Create_By { get; set; }
        [DataMember(Order = 7)]
        [Required]
        public string Create_Date { get; set; }
        [DataMember(Order = 8)]
        [Required]
        public List<Detail_order> Order_list { get; set; }

    }
    public class Detail_order
    {
        public string Access_No { get; set; }
        public string Order_no { get; set; }
    }
    #endregion
    public class UpdateInvoiceResponse
    {
        public string Result_code { get; set; }
        public string Result_message { get; set; }
    }




}


