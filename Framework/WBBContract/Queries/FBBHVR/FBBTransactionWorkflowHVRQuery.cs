using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.FBBHVR
{
    public class FBBTransactionWorkflowHVRQuery : IQuery<FBBTransactionWorkflowModel>
    {
        public string DomainHost { get; set; }
        public string UserHost { get; set; }
        public string PassHost { get; set; }
        public string TargetArchivePath { get; set; }
        public string TargetDomainPath { get; set; }
    }
}
