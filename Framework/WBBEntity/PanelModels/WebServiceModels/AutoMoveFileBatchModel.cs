using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class AutoMoveFileBatchModel
    {
        public AutoMoveFileBatchModel()
        {
            if (RES_COMPLETE_CUR == null)
                RES_COMPLETE_CUR = new List<FilePathByOrderNo>();

            if (RES_CANCEL_WF_CUR == null)
                RES_CANCEL_WF_CUR = new List<FilePathByOrderNo>();

            if (RES_CANCEL_FBSS_CUR == null)
                RES_CANCEL_FBSS_CUR = new List<FilePathByOrderNo>();

            if (RES_OTHERS_CUR == null)
                RES_OTHERS_CUR = new List<FilePathByOrderNo>();

        }

        public static string DocumentFolder { get; set; }
        public static string CompletedFolder { get; set; }
        public static string CancelWfFolder { get; set; }
        public static string CancelFbbsFolder { get; set; }
        public static string OthersFolder { get; set; }
        public static string TextFilePath { get; set; }
        public static string Username { get; set; }
        public static string SourceSystem { get; set; }
        public static string DocCode { get; set; }

        public string RES_CODE { get; set; }
        public string RES_MESSAGE { get; set; }
        public List<FilePathByOrderNo> RES_COMPLETE_CUR { get; set; }
        public List<FilePathByOrderNo> RES_CANCEL_WF_CUR { get; set; }
        public List<FilePathByOrderNo> RES_CANCEL_FBSS_CUR { get; set; }

        public List<FilePathByOrderNo> RES_OTHERS_CUR { get; set; }
    }

    public class FilePathByOrderNo
    {
        public string ORDER_NO { get; set; }
        public string NON_MOBILE_NO { get; set; }
        public string FILE_PATH { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_STATUS { get; set; }

    }

    public class AutoMoveFileBatchLogInParam
    {
        public string RuningDateTime { get; set; }
    }

    public class AutoMoveFileBatchLogOutParam
    {
        public string RemoveFileSummary { get; set; }
        public string FileCompleted { get; set; }
        public string FileCancelWF { get; set; }
        public string FileCancelFBSS { get; set; }
        public string FileOthers { get; set; }
    }
}
