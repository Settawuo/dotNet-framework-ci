using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetListGroupMailSpecialAccountModel
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }

        public List<GroupMailSpecialAccount> LIST_MAIL_SPECIAL_ACCOUNT { get; set; }
    }

    public class GroupMailSpecialAccount
    {
        public string DISPLAY_VAL { get; set; }
        public string LOV_VAL1 { get; set; }
        public string LOV_VAL2 { get; set; }

    }
}
