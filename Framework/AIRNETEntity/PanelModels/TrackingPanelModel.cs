using AIRNETEntity.StoredProc;
using System.Collections.Generic;

namespace AIRNETEntity.PanelModel
{
    public class TrackingPanelModel
    {

        public IEnumerable<TrackingModel> ienumerableTrackingPanelModel { get; set; }
        public TrackingLogCommand trackingLogCommand { get; set; }
    }

    public class TrackingLogCommand
    {
        public string IDCard { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ResultCode { get; set; }
        public string ReturnOrder { get; set; }
        public string CreatedBy { get; set; }
    }

    public class LovScreenValueModel
    {
        public string Type { get; set; }
        public string Name { get; set; }

        // lov1
        public string DisplayValue { get; set; }
        // lov5
        public string PageCode { get; set; }
        //ORDER_BY
        public decimal? OrderByPDF { get; set; }
        //Group_BY
        public string GroupByPDF { get; set; }
    }


}
