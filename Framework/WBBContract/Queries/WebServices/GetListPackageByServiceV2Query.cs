using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetListPackageByServiceV2Query : IQuery<PackageDataV2Model>
    {
        public string SessionId { get; set; }
        public string TransactionID { get; set; }
        public string FullUrl { get; set; }
        public string P_SALE_CHANNEL { get; set; }
        public string P_OWNER_PRODUCT { get; set; }
        public string P_PACKAGE_FOR { get; set; }
        public string P_SFF_PROMOTION_CODE { get; set; }
        public string P_Customer_Type { get; set; }
        public string P_Customer_subtype { get; set; }
        public string P_Partner_Type { get; set; }
        public string P_Partner_SubType { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; }
        public string P_Location_Code { get; set; }
        public string P_ASC_CODE { get; set; }
        public string P_EMPLOYEE_ID { get; set; }
        public string P_Region { get; set; }
        public string P_Province { get; set; }
        public string P_District { get; set; }
        public string P_Sub_District { get; set; }
        public string P_Address_Id { get; set; }
        public string P_Serenade_Flag { get; set; }
        public string P_FMPA_Flag { get; set; }
        public string P_CVM_FLAG { get; set; }
        public string P_MOBILE_PRICE { get; set; }
        public string P_EXISTING_MOBILE { get; set; }
        public string P_FMC_SPECIAL_FLAG { get; set; }
        public string P_NON_RES_FLAG { get; set; }
        public string P_DistChn { get; set; }
        public string P_ChnSales { get; set; }
        public string P_OperatorClass { get; set; }
        public string P_LocationProvince { get; set; }
        //21.10 MOU
        public string P_MOU_FLAG { get; set; }
    }
}
