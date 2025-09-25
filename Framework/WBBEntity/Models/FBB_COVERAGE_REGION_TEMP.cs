using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.Models
{
    public partial class FBB_COVERAGE_REGION_TEMP
    {


        public decimal? FTTX_ID { get; set; }
        public string GROUP_AMPHUR { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string OWNER_TYPE { get; set; }
        public string TOWER_TH { get; set; }
        public string TOWER_EN { get; set; }
        public DateTime? ONTARGET_DATE { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
