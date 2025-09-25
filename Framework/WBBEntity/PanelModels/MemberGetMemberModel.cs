using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class MemberGetMemberModel
    {
        public MemberGetMemberModel()
        {
            CustomerNeighborList = new List<CustomerNeighbor>();
        }

        public string NeighborName { get; set; }
        public string NeighborSurname { get; set; }
        public string InternetNo { get; set; }
        public string MobileNo { get; set; }
        public string TotalFriend { get; set; }
        public string Language { get; set; }
        public List<CustomerNeighbor> CustomerNeighborList { get; set; }

    }

    public class CustomerNeighbor
    {
        ////friend1
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ContactNo { get; set; }
        public string IsAisMobile { get; set; }
        public string Email { get; set; }
        public string SelectAddressType { get; set; }
        public string Building { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string PostalCode { get; set; }
        public string InstallAddress { get; set; }
        public string HouseNo { get; set; }
        public string Soi { get; set; }
        public string Road { get; set; }
        public string ContactTime { get; set; }
        public string LineId { get; set; }
        public string Rewards { get; set; }
        public string FullAddress { get; set; }

        public string OrderDate { get; set; }
        public string CampaignPeriod { get; set; }
        public string Status { get; set; }
    }

    public class MemberGetMemberStatus
    {
        public string refference_no { get; set; }
        public string referral_name { get; set; }
        public string referral_surname { get; set; }
        public string referral_contact_mobile_no { get; set; }
        public string member_name { get; set; }
        public string member_contact_mobile_no { get; set; }
        public string voucher_desc { get; set; }
        public string created_date { get; set; }
        public string expired_date { get; set; }
        public string status { get; set; }
    }

    public class DetailMemberGetMember
    {
        //R22.06.14062022
        public string internet_no { get; set; }
        public string detail { get; set; }
    }
}
