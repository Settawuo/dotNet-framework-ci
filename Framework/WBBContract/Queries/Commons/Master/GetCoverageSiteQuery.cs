using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetCoverageSiteQuery : IQuery<List<CoverageAreaPanel>>
    {
        public string Area { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string Staus { get; set; }
        public string ProjectName { get; set; }        
        public string Port { get; set; }
        public string LocationCode { get; set; }
    }   

}
