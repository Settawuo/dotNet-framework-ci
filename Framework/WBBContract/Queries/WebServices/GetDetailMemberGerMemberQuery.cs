using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDetailMemberGerMemberQuery : IQuery<DetailMemberGetMember>
    {
        public string p_internet_no { get; set; }
        public string p_values1 { get; set; }
        public string p_ContactListInfo { get; set; }
        public string p_language { get; set; }
    }
}
