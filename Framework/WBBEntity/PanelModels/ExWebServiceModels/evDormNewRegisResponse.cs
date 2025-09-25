namespace WBBEntity.PanelModels.ExWebServiceModels
{

    public class evDormNewRegisResponseBase
    {
        private string _result = "";

        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private string _errorCode = "";

        public string ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        private string _errorReason = "";

        public string ErrorReason
        {
            get { return _errorReason; }
            set { _errorReason = value; }
        }
    }

    public class RegisResponse : evDormNewRegisResponseBase { }

}
