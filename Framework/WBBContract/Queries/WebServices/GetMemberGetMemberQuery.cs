using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMemberGetMemberQuery : IQuery<List<MemberGetMemberStatus>>
    {
        public string Language { get; set; }
        public string InternetNo { get; set; }
    }
}
