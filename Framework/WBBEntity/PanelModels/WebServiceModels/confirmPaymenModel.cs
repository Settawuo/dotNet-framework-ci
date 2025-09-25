using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.WebServiceModels
{
     public class confirmPaymenModel
    {
        public string resp_code { get; set; }
        public string resp_desc { get; set; }
        public List<respData> resp_data { get; set; }
    }


    public class respData
    {
        public string baNum { get; set; }
        public string mobileNum { get; set; }

        public double receiptId { get; set; }

        public string receiptNum { get; set; }

        public double receiptTotMny { get; set; }

        public double taxMny { get; set; }

        public string payForCode { get; set; }

        public string caNum { get; set; }

        public double vatRate { get; set; }

        public double vatMny { get; set; }

        public double nonVatMny { get; set; }
        public double excVatMny { get; set; }
        public string eReceiptType { get; set; }
        public List<receiptDetail> receiptDetail { get; set; }

    }
    public class receiptDetail
    {
        public string docNum { get; set; }
        public double totalMny { get; set; }
    }
}
