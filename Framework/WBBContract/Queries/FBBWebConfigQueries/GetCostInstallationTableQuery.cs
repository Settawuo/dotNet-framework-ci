//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetCostInstallationTableQuery : IQuery<ConfigurationCostInstallationView>
    {
        public string TB_NAME { get; set; }
        public string SUBCONTTYPE { get; set; }
        public string SUBCONTRACT_LOCATION { get; set; }
        public string COMPANY_NAME { get; set; }
        public string RULE_ID { get; set; }
        public string ORD_TYPE { get; set; }
        public string TECH_TYPE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string EXPDATE_FROM { get; set; }
        public string EXPDATE_TO { get; set; }
        public string PAGE_INDEX { get; set; }
        public string PAGE_SIZE { get; set; }
        public string ret_code { get; set; }
    }


}
