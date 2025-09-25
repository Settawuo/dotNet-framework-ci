using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.QueryModels.FBSS
{
    public class GetBuildingByBuildingNameAndNoQueryModel
    {
        public string Partner { get; set; }
        public string Address_id { get; set; }
        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string Exclusive_3bb { get; set; }
    }
}
