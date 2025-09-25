using System.Collections.Generic;

namespace WBBContract.Commands.ExWebServices
{
    public class UpdateDocumentOrCSNoteByNotifyOrderCommand
    {
        public string TransactionID { get; set; }
        public string OrderNo { get; set; }
        public string CSNote { get; set; }
        public List<FileData> ListFile { get; set; }
        public string UserName { get; set; }
        public string CustomerPurge { get; set; }
        public string ExceptEntryFee { get; set; }
        public string SecondInstallation { get; set; }
    }

    public class FileData
    {
        public string FileName { get; set; }
    }
}
