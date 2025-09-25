namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ArchiveInterfaceLogSendMailDetailModel
    {

    }

    public class ArchiveInterfaceLogSendMailDetailList
    {
        public int ret_code { get; set; }
        public string ret_msg { get; set; }

        public string p_subject { get; set; }
        public string p_body_h { get; set; }
        public string p_body_result { get; set; }
        public string p_body_summary { get; set; }
        public string p_body_signature { get; set; }
    }
}
