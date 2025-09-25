using System.Collections.Generic;
using WBBContract.Commands.ExWebServices.FbbCpGw;

namespace WBBExternalAPI.Models.Response.FbbCpGwInterfaceController
{
    public class TransferFileToStorageResponse
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string OrderNo { get; set; }
        public string FileName { get; set; }
        public string DataFile { get; set; }
        public List<FileListData> FileList { get; set; }
    }
}