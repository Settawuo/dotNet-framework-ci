namespace FBBPAYG_LoadFile_3BBReport
{
    public class DownloadFileCopyModel
    {
        public byte[] Download { get; set; }
        public string msg { get; set; }
        public string filename { get; set; }
    }
    public class SendMailBatchReport
    {
        public string SEND_TO { get; set; }
        public string SEND_CC { get; set; }
        public string SEND_BCC { get; set; }
        public string SEND_FROM { get; set; }
        public string IP_MAIL_SERVER { get; set; }

    }
}
