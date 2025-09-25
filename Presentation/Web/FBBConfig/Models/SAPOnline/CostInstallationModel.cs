using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Models.SAPOnline
{
    public class CostInstallationView
    {
        public List<CostInstallation> CostInstallation { get; set; }
        public IEnumerable<SelectListItem> Service { get; set; }
        public IEnumerable<SelectListItem> Customer { get; set; }
        public IEnumerable<SelectListItem> Customer_Name { get; set; }
        public IEnumerable<SelectListItem> OrderType { get; set; }
    }

    public class CostInstallationTable
    {
        public string SERVICE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public Nullable<decimal> INTERNET_RATE { get; set; }
        public Nullable<decimal> INTERNET_PLAYBOX { get; set; }
        public Nullable<decimal> INTERNET_VOIP { get; set; }
        public string ORDER_TYPE { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRE_DATE { get; set; }
        public string REMARK { get; set; }
        public Boolean CHK_PLAYBOX { get; set; }
    }

}