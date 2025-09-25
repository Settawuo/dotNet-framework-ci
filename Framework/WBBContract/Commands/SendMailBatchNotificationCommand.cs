using System.Collections.Generic;
using System.IO;

namespace WBBContract.Commands
{
    public class SendMailBatchNotificationCommand
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


    public class SendMailBatchPatchDataCommand
    {
        public string ProcessName { get; set; }
        public string SendTo { get; set; }
        public string SendCC { get; set; }
        public string SendBCC { get; set; }
        public string SendFrom { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        //public string[] AttachFiles { get; set; }
        //public List<MemoryStreamAttachFiles> msAttachFiles { get; set; }

        public string FromPassword { get; set; }
        public string Port { get; set; }
        public string Domaim { get; set; }
        public string IPMailServer { get; set; }
        public string FileName { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class SendMailBatchPatchDataList
    {
        public List<SendMailBatchPatchList> sendMailBatchPatchLists { get; set; }
        public string ReturnMessage { get; set; }
    }
    public class SendMailBatchPatchList
    {
        public string ProcessName { get; set; }
        public string SendTo { get; set; }
        public string SendCC { get; set; }
        public string SendBCC { get; set; }
        public string SendFrom { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        //public string[] AttachFiles { get; set; }
        //public List<MemoryStreamAttachFiles> msAttachFiles { get; set; }

        public string FromPassword { get; set; }
        public string Port { get; set; }
        public string Domaim { get; set; }
        public string IPMailServer { get; set; }
        public string FileName { get; set; }
    }

    public class MemoryStreamAttachFiles
    {
        public MemoryStream file;
        public string fileName;
    }
}
