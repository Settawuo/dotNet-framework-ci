using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetAPISendmailModel
    {
        public GetAPISendmailModel()
        {
            if (SEND_EMAIL == null)
                SEND_EMAIL = new List<APISendmail>();
        }
        public string RESULT_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<APISendmail> SEND_EMAIL { get; set; }
    }

    public class APISendmail
    {
        public string SEND_TO { get; set; }
        public string SEND_CC { get; set; }
        public string SEND_FROM { get; set; }
        public string IP_MAIL_SERVER { get; set; }
        public string SUBJECT_NAME { get; set; }
        public string DETAIL_1 { get; set; }
        public string DETAIL_2 { get; set; }
        public string DETAIL_3 { get; set; }
        public string DETAIL_4 { get; set; }
        public string DETAIL_5 { get; set; }
        public string CLOSE_EMAIL { get; set; }
    }
}
