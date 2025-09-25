namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class evDormUpdateStatusModel
    {
        private string ret_code;

        public string retcode
        {
            get { return ret_code; }
            set { ret_code = value; }
        }

        private string ret_msg = "";

        public string retmsg
        {
            get { return ret_msg; }
            set { ret_msg = value; }
        }


    }

    public class UpdateStatus : evDormUpdateStatusModel { }
}
