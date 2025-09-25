using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class SubmitFOAReportQuery : IQuery<List<SubmitFOAReport>>
    {
        public string accessNo { get; set; }
        public string orderNo { get; set; }
        public string status { get; set; }
        public string productName { get; set; }
        public string orderType { get; set; }
        public string companyCode { get; set; }
        public string serviceName { get; set; }
        public string subcontractorCode { get; set; }
        public string plant { get; set; }
        public string materialCode { get; set; }
        public string storLocation { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public string productOwner { get; set; }
    }

    public class GetProductListQuery : IQuery<SubmitFOAResend>
    {
        public string OrderNo { get; set; }
        public string AccessNo { get; set; }
    }
    public class GetProductListByPackageQuery : IQuery<SubmitFOAResend>
    {
        public string OrderNo { get; set; }
        public string AccessNo { get; set; }
        //R21.03.2021
        public string flag_auto_resend { get; set; }
    }

    public class ResendToSelectQuery : IQuery<SubmitFOAResend>
    {
        public string AccessNumber { get; set; }
        public string OrderNo { get; set; }
        public List<SubmitFOAEquipment> ListEquipment { get; set; }
    }

    public class GetMaterialCodeQuery : IQuery<List<LovModel>>
    {

    }

    public class GetListFbssFoaVendorCodeQuery : IQuery<List<LovModel>>
    {

    }

    public class GetListFbssFixedAssetConfigQuery : IQuery<List<LovModel>>
    {
        public string DDLName { get; set; }
        public string Param1 { get; set; }
    }
}
