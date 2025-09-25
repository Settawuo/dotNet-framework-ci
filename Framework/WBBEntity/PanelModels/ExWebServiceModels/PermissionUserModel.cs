using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class PermissionUserModel
    {
        public string ACTION { get; set; }
        public string TRANSACTION_NO { get; set; }
        public List<DETAIL_USER> USER_ARRAY { get; set; }
    }

    public class DETAIL_USER
    {
        public string USER_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public string ROLE { get; set; }
        public string ROLE_PAST { get; set; }
        public string PERIOD { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
    }
}
