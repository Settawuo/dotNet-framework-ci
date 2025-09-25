using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class LovScreenValueModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Blob { get; set; }

        // displayvalue
        public string DisplayValueJing { get; set; }
        // lov1
        public string DisplayValue { get; set; }
        // lov5
        public string PageCode { get; set; }
        // lov3
        public string LovValue3 { get; set; }
        //ORDER_BY
        public decimal? OrderByPDF { get; set; }
        //Group_BY
        public string GroupByPDF { get; set; }
        //DefaultValue
        public string DefaultValue { get; set; }

    }

    public class DatAutoOntopPlayboxModel
    {
        public DatAutoOntopPlayboxModel()
        {
            if (Package == null)
                Package = new List<Package>();
        }
        public string DataName { get; set; }

        public string DataIconDes { get; set; }

        public string DetailHead1 { get; set; }

        public string DetailHead2 { get; set; }

        public string DetailDes { get; set; }

        public string IconPath { get; set; }

        public string LogoPath { get; set; }

        public string Group { get; set; }

        public List<Package> Package { get; set; }

        //17.9 Speed boost
        public string TopupGroup { get; set; }
        public string TopupGroupName { get; set; }
        public string GroupName { get; set; }
        public int GroupOrderBy { get; set; }
    }

    //18.1 FTTB Sell Router
    public class DatListRouterModel
    {
        public DatListRouterModel()
        {
            if (Package == null)
                Package = new List<Package>();
        }

        public string DataName { get; set; }

        public string DataIconDes { get; set; }

        public string IconPath { get; set; }

        public string Group { get; set; }

        public List<Package> Package { get; set; }

        public string TopupGroup { get; set; }

        public string TopupGroupName { get; set; }


        public string TopupPrice { get; set; }

        public string TopupPriceText { get; set; }
    }

    public class Package
    {
        public string Trial { get; set; }
        public string TopUp { get; set; }
    }
}
