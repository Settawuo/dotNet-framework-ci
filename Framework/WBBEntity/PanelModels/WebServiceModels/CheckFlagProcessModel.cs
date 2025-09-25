namespace WBBEntity.PanelModels.WebServiceModels
{
    public class CheckFlagProcessModel
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public CheckFlagProcessData checkFlagProcessData { get; set; }
    }

    public class CheckFlagProcessData
    {
        public string P_FLAG_DOPA { get; set; }
        public string P_FLAG_FACE_RECOGNITION { get; set; }
        public string P_FLAG_TAKE_PHOTO { get; set; }
        public string P_FLAG_BROWSE_FILE { get; set; }
        public string P_FLAG_VARIFY_DOCUMENTS { get; set; }

    }
}
