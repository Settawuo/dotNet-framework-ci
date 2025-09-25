namespace WBBEntity.PanelModels
{
    public class ZTEResQueryModel
    {
        public string RETURN_CASECODE { get; set; }
        public ResultSplitList[] RESULT_SPLITTERLIST { get; set; }
        public ResultDslamList[] RESULT_DSLAMLIST { get; set; }
    }

    public class ResultSplitList
    {
        public string SPLITTER_NO { get; set; }
        public string RESULT_CODE { get; set; }
        public string RESULT_DESCRIPTION { get; set; }
    }

    public class ResultDslamList
    {
        public string DSLAM_NO { get; set; }
        public string RESULT_CODE { get; set; }
        public string RESULT_DESCRIPTION { get; set; }
    }
}
