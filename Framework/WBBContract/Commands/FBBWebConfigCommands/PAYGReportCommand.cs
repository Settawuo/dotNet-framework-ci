using System;
using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class PAYGReportCommand
    {
    }

    public class UpdatePaidStatusDataCommand
    {
        public string FIBRENET_ID { get; set; }
        public string INVOICE_NO { get; set; }
        public string PO_NO { get; set; }
        public string DEVICE_TYPE { get; set; }
        public string PAID_ST { get; set; }
        public string REMARK { get; set; }

    }

    public class UpdatePaidStatusDataModel
    {
        public string FIBRENET_ID { get; set; }
        public bool CHECKED_IN { get; set; }
        public bool CHECKED_OUT { get; set; }
        public bool CHECKED_ONT { get; set; }
        public string INVOICE_NO_IN { get; set; }
        public string PO_NO_IN { get; set; }
        public string PAID_ST_IN { get; set; }
        public string REMARK_IN { get; set; }
        public string INVOICE_NO_OUT { get; set; }
        public string PO_NO_OUT { get; set; }
        public string PAID_ST_OUT { get; set; }
        public string REMARK_OUT { get; set; }
        public string INVOICE_NO_ONT { get; set; }
        public string PO_NO_ONT { get; set; }
        public string PAID_ST_ONT { get; set; }
        public string REMARK_ONT { get; set; }


    }

    public class ImportPaidStatusCommand
    {
        public string DEVICE_TYPE { get; set; }
        public string FIBRENET_ID { get; set; }
        public Nullable<DateTime> INVOICE_DT { get; set; }
        public string INVOICE_NO { get; set; }
        public string PAID_ST { get; set; }
        public string PO_NO { get; set; }
        public string REMARK { get; set; }
        public string Return_Code { get; set; }
        public string Return_Message { get; set; }
    }

    public class UpdateOLTStatusCommand
    {
        public string OLT_NAME { get; set; }
        public string OLT_STATUS { get; set; }
        public string Return_Code { get; set; }
        public string Return_Message { get; set; }
    }

    public class UpdateOLTStatusModel
    {
        public string OLT_NAME { get; set; }
        public string OLT_STATUS { get; set; }
    }

    public class SendMailDetailModel
    {
        public string Project { get; set; }
        public string Mode { get; set; }
        public string Result { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class SendMailBatchCommand
    {
        public List<SendMailDetailModel> SendMail { get; set; }
        public string Return_Message { get; set; }
    }

    public class ArchiveInterfaceLogCommand
    {
        public decimal p_date { get; set; }
        public string p_type { get; set; }

        //return value
        public string Return_Code { get; set; }
        public string Return_Message { get; set; }
    }

}

