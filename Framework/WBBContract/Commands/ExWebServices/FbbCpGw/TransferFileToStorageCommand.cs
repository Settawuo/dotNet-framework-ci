using System.Collections.Generic;

namespace WBBContract.Commands.ExWebServices.FbbCpGw
{
    public class TransferFileToStorageCommand
    {
        //input
        public string Transaction_id { get; set; }
        public string Channel { get; set; }
        public string UserName { get; set; }
        public string Option { get; set; }
        public string OrderNo { get; set; }
        public string FileName { get; set; }
        public List<FileListData> FileList { get; set; }

        //output
        public string Return_code { get; set; }
        public string Return_message { get; set; }
    }

    public class FileListData
    {
        public string OrderNo { get; set; }
        public string Action { get; set; }
        public string FileName { get; set; }
        public string DataFile { get; set; }
        public string Result { get; set; }
        public string ErrorDesc { get; set; }
    }

    public class tbFileListData
    {
        public string OrderNo { get; set; }
        public string Action { get; set; }
        public string Filename { get; set; }
        public string Result { get; set; }
        public string ErrorDesc { get; set; }
    }

    public class tbRETURN_DETAIL_FILE_CUR
    {
        public string OrderNo { get; set; }
        public string Action { get; set; }
        public string Filename { get; set; }
    }
}
