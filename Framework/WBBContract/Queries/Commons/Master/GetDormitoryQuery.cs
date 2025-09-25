using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetDormitoryQuery : IQuery<List<DomitoryModel>>
    {
        public string language { get; set; }
        public string netnumber { get; set; }
        public string DormID { get; set; }
        public string DormName { get; set; }
        public string DormNO { get; set; }
        public string FloorNo { get; set; }
        public string Subcontract { get; set; }
        public string SubcontractTH { get; set; }
        public string SubcontractEN { get; set; }
    }
}
