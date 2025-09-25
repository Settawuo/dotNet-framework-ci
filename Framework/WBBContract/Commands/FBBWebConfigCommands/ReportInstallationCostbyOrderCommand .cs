using System.Collections.Generic;
namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class ReportInstallationCostbyOrderCommand
    {
    }
    public class ReportInstallation_FBB_access_list
    {
        public string ACCESS_NUMBER { get; set; }
    }
    public class UpdateReportInstallationCostbyOrderCommand
    {
        public List<ReportInstallation_FBB_access_list> p_ACCESS_list { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_INTERFACE { get; set; }
        public string p_USER { get; set; }
        public string p_STATUS { get; set; }
        public string p_INVOICE_NO { get; set; }
        public string p_INVOICE_DT { get; set; }
        public string p_IR_DOC { get; set; }
        public string p_VALIDATE_DIS { get; set; }
        public string p_REASON { get; set; }
        public string p_REMARK { get; set; }
        public string p_TRANSFER_DT { get; set; }
        public string p_REMARK_FOR_SUB { get; set; }
        public string p_CHECK_IRDOC { get; set; }
        public string p_PERIOD_DATE { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
    public class ReportInstallation_FixOrderListTmp
    {
        // public string ORDER_NO { get; set; }
        public string ACCESS_NUMBER { get; set; }
        //public string ORDER_TYPE { get; set; }
        //public string SUB_CONTRACTOR_CODE { get; set; }
        //public string SUB_CONTRACTOR_NAME { get; set; }
        //public string PAY_PERIOD_FROM { get; set; }
        //public string PAY_PERIOD_TO { get; set; }
        //public string ORDER_STATUS { get; set; }
    }

    public class SendMailReportInstallationCommand
    {
        public string p_USER { get; set; }
        public string p_period_from { get; set; }
        public string p_period_to { get; set; }
        public List<ReportInstallation_FixOrderListTmp> fixed_order { get; set; }
        public int ret_code { get; set; }
        public string ret_msg { get; set; }

    }

    public class ReportInstallationUpdateNoteCommand
    {
        public string p_ACCESS_NO { get; set; }
        public string p_ORDER_NO { get; set; }
        public string p_USER { get; set; }
        public string p_REMARK_FOR_SUB { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }

    public class ReportInstallationCostbyOrderUpdateByFileCommand
    {
        public string p_INTERFACE { get; set; }
        public string p_USER { get; set; }
        public string p_STATUS { get; set; }
        public string p_filename { get; set; }
        public List<ReportInstallation_FBB_update_file_list> p_file_list { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }

    public class ReportInstallation_FBB_update_file_list
    {
        public string ACC_NBR { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public string IR_DOC { get; set; }
        public string REMARK { get; set; }
        public string REMARK_FOR_SUB { get; set; }
        public decimal? VALIDATE_DIS { get; set; }
        public string REASON { get; set; }
        public string TRANSFER_DATE { get; set; }
    }
    public class SendMailReportInstallationNotificationCommand
    {
        public string ProcessName { get; set; }
        public string CreateUser { get; set; }
        public string SendTo { get; set; }
        public string SendCC { get; set; }
        public string SendBCC { get; set; }
        public string SendFrom { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string[] AttachFiles { get; set; }
        //public MemoryStream[] msAttachFiles { get;set; }
        //public string filename { get;set; }
        public List<MemoryStreamAttachFiles> msAttachFiles { get; set; }

        public string FromPassword { get; set; }
        public string Port { get; set; }
        public string Domaim { get; set; }
        public string IPMailServer { get; set; }

        public string ReturnMessage { get; set; }
    }
}
