using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WBBEntity.PanelModels
{
    public class CoveragePanelModel : PanelModelBase
    {
        public CoveragePanelModel()
        {
            this.Address = new AddressPanelModel();
            this.CoverageMemberGetMember = new CoverageMemberGetMemberModel();
        }

        public string H_FBB001 { get; set; }
        public string L_SPECIFIC_ADDR_1 { get; set; }
        public string L_SPECIFIC_BUILD { get; set; }
        public string L_BUILD_NAME { get; set; }
        public string L_BUILD_NO { get; set; }
        public string L_FLOOR_CONDO { get; set; }
        public string L_FLOOR_VILLAGE { get; set; }
        public string L_HAVE_FIXED_LINE { get; set; }
        public string L_SPECIFIC_ADDR_2 { get; set; }
        public string L_LAT_LONG { get; set; }
        public string L_HOW_LAT_LONG { get; set; }
        public string L_CLICK { get; set; }
        public string L_LAT { get; set; }
        public string L_LONG { get; set; }
        public string L_LAT2 { get; set; }
        public string L_LONG2 { get; set; }
        public string L_DECIMAL { get; set; }
        public string L_SPECIFIC_LOC { get; set; }
        public string L_HOW_TO { get; set; }
        public string B_CHECKING { get; set; }
        public string RESULT_ID { get; set; }
        public string L_RESULT { get; set; }
        public string L_RESULT_DETAIL { get; set; }
        public string L_FIRST_LAST { get; set; }
        public string L_CONTACT_PHONE { get; set; }
        public AddressPanelModel Address { get; set; }
        public string BuildingType { get; set; }

        public string CVRID { get; set; }
        public string CVR_NODE { get; set; }
        public string CVR_TOWER { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }

        public string L_FIRST_NAME { get; set; }
        public string L_LAST_NAME { get; set; }
        public string P_MOBILE { get; set; }
        public string CA_ID { get; set; }
        public string SA_ID { get; set; }
        public string BA_ID { get; set; }
        public string SffProductName { get; set; }
        public string SffServiceYear { get; set; }
        public string P_FTTX_VENDOR { get; set; }
        public string BillingSystem { get; set; }
        public string CoverageResult { get; set; }

        // Update 15.3
        [Required]
        public string AccessMode { get; set; }
        [Required]
        public string ServiceCode { get; set; }

        // Update 15.7
        public string OneBill { get; set; }

        // Update 16.2
        public string L_CONTACT_EMAIL { get; set; }
        public string L_CONTACT_LINE_ID { get; set; }
        public List<SPLITTER_INFO> splitter { get; set; }
        public List<SPLITTER_3BB_INFO> splitter3bb { get; set; }

        // Update 16.3
        public string MobileSegment { get; set; }
        public string ChargeType { get; set; }
        public string BundlingMainFlag { get; set; }
        public string BundlingSpecialFlag { get; set; }
        public string Serenade_Flag { get; set; }
        public string FMC_SPECIAL_FLAG { get; set; }
        //public string existing_mobile_flag { get; set; }
        public string FMPA_FLAG { get; set; }
        public string CVM_FLAG { get; set; }
        public string P_PrePostpiad { get; set; }

        public string Mobile_Segment { get; set; }
        public string NetworkType { get; set; }
        public string ARPU { get; set; }

        //Update 17.7
        public CoverageMemberGetMemberModel CoverageMemberGetMember { get; set; }

        //Update 18.4
        public string LOCATION { get; set; }

        //20.1 For SaveOrder
        public string SAVEORDER_SALE_CHANNEL { get; set; }
        public string SAVEORDER_OWNER_PRODUCT { get; set; }
        public string SAVEORDER_PACKAGE_FOR { get; set; }
        public string SAVEORDER_REGION { get; set; }
        public string SAVEORDER_PROVINCE { get; set; }
        public string SAVEORDER_DISTRICT { get; set; }
        public string SAVEORDER_SUB_DISTRICT { get; set; }
        public string SAVEORDER_SERENADE_FLAG { get; set; }
        public string SAVEORDER_FMPA_FLAG { get; set; }
        public string SAVEORDER_CVM_FLAG { get; set; }
        public string SAVEORDER_FMC_SPECIAL_FLAG { get; set; }
        public string SAVEORDER_MOU_FLAG { get; set; } //21.10 MOU

        //20.2
        public string GRID_ID { get; set; }
        public string WTTX_MAXBANDWITH { get; set; }
        public string WTTX_COVERAGE_RESULT { get; set; }
        public string WTTX_MOBILE_NO { get; set; }
        //21.4 Out Of Coverage Phase 2
        public string ADDRESS_TYPE_DTL { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PROJECTNAME { get; set; }
        public string REMARK { get; set; }

        public string EXISTING_MOBILE_FLAG { get; set; }
        public string USE_INPUT_MOBILE_ONLINE_QUERY { get; set; }
    }

    public class CoverageValueModel
    {
        public decimal CvrId { get; set; }
        public string SiteCode { get; set; }
        public string NodeNameEn { get; set; }
        public string NodeNameTh { get; set; }
        public string NodeType { get; set; }
        public string NodeStatus { get; set; }

        // node address.
        public string Moo { get; set; }
        public string Soi_Th { get; set; }
        public string Road_Th { get; set; }
        public string Soi_En { get; set; }
        public string Road_En { get; set; }
        public string Zipcode { get; set; }

        public DateTime? ONTARGET_DATE_EX { get; set; }
        public DateTime? ONTARGET_DATE_IN { get; set; }
    }

    public class CoverageRelValueModel
    {
        public decimal CvrId { get; set; }
        public decimal DslamId { get; set; }
        public string TowerNameEn { get; set; }
        public string TowerNameTh { get; set; }
        public string Latitute { get; set; }
        public string Longitude { get; set; }
    }

    public class CoverageZipCodeModel
    {
        public CoverageZipCodeModel()
        {
            this.CoverageValueModels = new List<CoverageValueModel>();
            this.ZipCodeModels = new List<ZipCodeModel>();
        }

        public List<CoverageValueModel> CoverageValueModels { get; set; }
        public List<ZipCodeModel> ZipCodeModels { get; set; }
    }

    public class ImageResultModel
    {
        public ImageResultModel()
        {
            this.PicList = new List<ListImageLink>();

        }

        public decimal ReturnCode { get; set; }
        public string ReturnMassage { get; set; }
        public List<ListImageLink> PicList { get; set; }
    }

    public class ListImageLink
    {
        public string AXIS { get; set; }
        public string PICTURE_PATH { get; set; }
    }

    public class CoverageMemberGetMemberModel
    {
        public string RefferenceNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerMobileNo { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerLineId { get; set; }
        public string VoucherDesc { get; set; }
        public string CampaignProjectName { get; set; }
        public string VoucherPin { get; set; }

        // node address.
        public string Language { get; set; }
        public string BuildingName { get; set; }
        public string VillageName { get; set; }
        public string Province { get; set; }
        public string Distrinct { get; set; }
        public string SubDistrict { get; set; }
        public string PostCode { get; set; }
        public string HouseNo { get; set; }
        public string Soi { get; set; }
        public string Road { get; set; }
        public string AddressType { get; set; }
        public string FULL_ADDRESS { get; set; }
        public bool IM_ORDER { get; set; }
        public string SERVICE_CASE_ID { get; set; }
        public string FLOOR { get; set; }
        public string MOO { get; set; }
        public string BUILDING_NO { get; set; }
        public string IS_CHANNEL_IM { get; set; }
        public string CASE_ID { get; set; }
        public string ASSET_NUMBER { get; set; }
        public string RELATE_MOBILE_NO { get; set; }
        public string FBB_PERCENT_DISCOUNT { get; set; }
    }

}
