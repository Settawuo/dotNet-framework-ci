using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WBBExternalAPI.Models.Request.FbbCpGwInterfaceController
{
    public class TransferFileToStorageRequest
    {
        public TransferFileToStorageRequest()
        {
            this.FileList = new List<RequestFileListData>();
        }

        public string UserName { get; set; }

        [Required]
        public string Option { get; set; }

        public string OrderNo { get; set; }

        public List<RequestFileListData> FileList { get; set; }
    }

    public class RequestFileListData
    {
        public string OrderNo { get; set; }
        public string Action { get; set; }
        public string FileName { get; set; }
        public string DataFile { get; set; }
    }
}