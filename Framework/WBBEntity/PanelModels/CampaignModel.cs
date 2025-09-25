using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class CampaignModel
    {
        public CampaignModel()
        {
            FRIENDS = new List<FriendModel>();
        }
        //new
        public string EMP_NAME { get; set; }
        public string EMP_SURNAME { get; set; }
        public string EMP_CODE { get; set; }
        public string INTERNET_CODE { get; set; }
        public string MOBILE_CALLBACK { get; set; }
        public string TOTAL_FRIEND { get; set; }

        ////friends
        public List<FriendModel> FRIENDS { get; set; }

    }

    public class FriendModel
    {
        public string NAME { get; set; }
        public string SURNAME { get; set; }
        public string CONTACT_MOBILE { get; set; }
        public string MOBILE_IS_AIS { get; set; }
        public string EMAIL { get; set; }
        public string PROVINCE { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string POSTAL_CODE { get; set; }
        public string SELECT_ADDRESS_TYPE { get; set; }
        public string BUILDING { get; set; }
        public string HOUSE_NO { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string CONTACT_TIME { get; set; }
    }

}
