using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class CoverageSitePanelModel
    {
        public string TotalSite { get; set; }
        public int TotalCoverage { get; set; }
        public int TotalPort { get; set; }
        public int Available { get; set; }
        public int Active { get; set; }
        public int Reserve { get; set; }
        public int OutOfService { get; set; }
        public int PendingTerminate { get; set; }
        public CoverageAreaPanel Coverage { get; set; }
        public List<BuildingPanel> Building { get; set; }
        public List<CoverageAreaPanel> CoverageAreaPanel { get; set; }
    }

    public class CoverageExcelModel
    {
        public string REGION { get; set; }
        public string IPRAN_CODE { get; set; }
        public string LOCATIONCODE { get; set; }
        public string NODESTATUS { get; set; }
        public string ONTARGET_DATE_IN { get; set; }
        public string ONTARGET_DATE_EX { get; set; }
        public string NODETYPE { get; set; }
        public string NODENAME_TH { get; set; }
        public string NODENAME_EN { get; set; }
        public string CONTACT_NUMBER { get; set; }
        public string MOO { get; set; }
        public string SOI_TH { get; set; }
        public string ROAD_TH { get; set; }
        public string ZIPCODE { get; set; }
        public string TUMBON { get; set; }
        public string AMPHUR { get; set; }
        public string PROVINCE { get; set; }

    }

    public class PortUtillzationPanel
    {
        public decimal GroupData { get; set; }
        public decimal CountData { get; set; }
    }

    public class CoverageAreaPanel
    {
        [DisplayName("Node Type")]
        public string NodeType { get; set; }
        [DisplayName("Node Name TH")]
        public string NodeNameTH { get; set; }
        [DisplayName("Node Name EN")]
        public string NodeNameEN { get; set; }
        [DisplayName("ContactNumber")]
        public string ContactNumber { get; set; }
        [DisplayName("FaxNumber")]
        public string FaxNumber { get; set; }
        [DisplayName("จังหวัด")]
        public string ProvinceTH { get; set; }
        [DisplayName("อำเภอ")]
        public string AmphurTH { get; set; }
        [DisplayName("ตำบล")]
        public string TumbonTH { get; set; }
        [DisplayName("หมู่")]
        public decimal? MooTH { get; set; }
        [DisplayName("ซอย")]
        public string SoiTH { get; set; }
        [DisplayName("ถนน")]
        public string RoadTH { get; set; }
        [DisplayName("รหัสไปรษณีย์")]
        public string ZipCodeTH { get; set; }
        [DisplayName("Province")]
        public string ProvinceEN { get; set; }
        [DisplayName("Amphur")]
        public string AmphurEN { get; set; }
        [DisplayName("Tumbon")]
        public string TumbonEN { get; set; }
        [DisplayName("Moo")]
        public decimal? MooEN { get; set; }
        [DisplayName("Soi")]
        public string SoiEN { get; set; }
        [DisplayName("Road")]
        public string RoadEN { get; set; }
        [DisplayName("Postcode")]
        public string ZipCodeEN { get; set; }
        [DisplayName("IPRAN Site Code")]
        public string IpRanSiteCode { get; set; }
        [DisplayName("Condo Code")]
        public string CondoCode { get; set; }
        [DisplayName("Lat")]
        public string Lat { get; set; }
        [DisplayName("Long")]
        public string Long { get; set; }
        [DisplayName("Building Code")]
        public string BuildingCode { get; set; }
        [DisplayName("On Target Date Internal")]
        public Nullable<System.DateTime> OnTargetDateIn { get; set; }
        [DisplayName("On Target Date External")]
        public Nullable<System.DateTime> OnTargetDateEx { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }
        public string StatusValue { get; set; }
        [DisplayName("Config Complete")]
        public bool ConfigComplete { get; set; }
        [DisplayName("TIE")]
        public bool TieFlag { get; set; }

        public decimal CVRId { get; set; }
        public decimal? ContactId { get; set; }
        [DisplayName("Region Code")]
        public string RegionCode { get; set; }

        public string CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public int UpdateCoverageType { get; set; }
    }

    public class DslamInfoPanel
    {
        public decimal DSLAMID { get; set; }
        public decimal DSLAMNUMBER { get; set; }
        public decimal DSLAMMODELID { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string NODEID { get; set; }
    }

    public class CardPanel
    {
        public decimal Number { get; set; }
        public string Model { get; set; }
        public string CardType { get; set; }
        public string Reserve { get; set; }
        public string NodeId { get; set; }
        public decimal CardId { get; set; }


    }

    public class PortPanel
    {
        public int PortNumber { get; set; }
        public string PortStatus { get; set; }
        public string PortType { get; set; }
    }

    public class DslamPanel
    {
        public decimal DSLAMID { get; set; }
        public string NodeId { get; set; }
        public decimal Number { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingUse { get; set; }
        public int CurrentPort { get; set; }
        public string Code { get; set; }
        public string DLRuningNumber { get; set; }
        public string MC { get; set; }
        public string IPRANPort { get; set; }
        public string DSLAMModel { get; set; }
        public string Brand { get; set; }

        public string RegionDSLAM { get; set; }
        public string Lot { get; set; }
        public decimal CVRRelationID { get; set; }
    }

    public class GridDSLAMRestockModel
    {
        public decimal DSLAMID { get; set; }
        public string NodeID { get; set; }
        public decimal DSALMNumber { get; set; }
        public string RegionCode { get; set; }
        public string LotNumber { get; set; }
    }

    public class BuildingPanel
    {
        public decimal ContactId { get; set; }
        public string Tower { get; set; }
        public string TowerTH { get; set; }
        public string TowerEN { get; set; }
        public string InstallNote { get; set; }

        public string Text { get; set; }
        public string Value { get; set; }

        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

    public class CoverageAreaModel
    {
        public decimal CVRID { get; set; }
        public decimal? ContactID { get; set; }
        public string Region { get; set; }
        public string RegionCode { get; set; }
        public string RegionLOV { get; set; }
        public string BuildingCode { get; set; }
        public string CondoCode { get; set; }
        public string IPRANCode { get; set; }
    }

}
