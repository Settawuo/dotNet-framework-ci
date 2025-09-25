using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FBBDeleteFileOrderCancelModel
    {
        public string RES_CODE { get; set; }
        public string RES_MESSAGE { get; set; }

        public List<FilePathByOrderNo> RES_CANCEL_WF_CUR { get; set; }
    }

    public class FBBDeleteFileOrderCancelLogInParam
    {
        public string RuningDateTime { get; set; }
    }

    public class FBBDeleteFileOrderCancelLogOutParam
    {
        public string DeleteFileSummary { get; set; }
    }
}
