using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GetConfigDropDownModel
    {
        public string symptom_group { get; set; }
        public string symptom_name { get; set; }
        public string province_th { get; set; }
        public string district_th { get; set; }
        public string sub_district_th { get; set; }
        public string main_pkg_group { get; set; }
        public string package_name { get; set; }
    }
}
