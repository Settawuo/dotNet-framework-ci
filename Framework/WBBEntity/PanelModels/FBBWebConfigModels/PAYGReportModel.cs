using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class PAYGReportModel
    {


    }

    public class LastmileAndCPEReportModel
    {
        public string OLTBrand { get; set; }
        public string Phase { get; set; }
        public string Region { get; set; }
        public string product { get; set; }
        public string addressid { get; set; }
        public int P_PAGE_INDEX { get; set; }
        public int P_PAGE_SIZE { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class LastmileAndCPEReportList
    {
        public string DEVICE_VENDOR { get; set; }
        public string PHASE_PO { get; set; }
        public string REGION { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string OLT_NAME { get; set; }
        public DateTime? REGISTER_DT { get; set; }
        public decimal? TO_BE_APPROVE { get; set; }
        public decimal? APPROVE { get; set; }
        public decimal? PAID_LASTMILE_IND { get; set; }
        public decimal? PAID_LASTMILE_OUT { get; set; }
        public decimal? NOT_PAID_LASTMILE_IND { get; set; }
        public decimal? NOT_PAID_LASTMILE_OUT { get; set; }
        public decimal? REM_PAID_LASTMILE_IND { get; set; }
        public decimal? REM_PAID_LASTMILE_OUT { get; set; }
        public decimal? PAID_ONT { get; set; }
        public decimal? NOT_PAID_ONT { get; set; }
        public decimal? REM_PAID_ONT { get; set; }
    }

    public class DetailLastmileAndCPEReportList
    {
        public string CUST_REQUST_DT { get; set; }
        public string CUST_REGISTER_DT { get; set; }
        public string CS_APPROVE_DT { get; set; }
        //public DateTime? REGISTER_DT { get; set; }
        public string ACTIVITY { get; set; }
        public string FIBRENET_ID { get; set; }
        public string CUST_NAME { get; set; }
        public string CUST_STATUS { get; set; }
        public string CUST_STATUS_DT { get; set; }
        //Add new column
        public string PACKAGE_NAME { get; set; }
        public string PACKAGE_CHANGE_DT { get; set; }

        public string ADDRESS_ID { get; set; }
        public string BUILDING_NAME { get; set; }
        public string BUSINESS_MODEL { get; set; }
        public string PRODUCT { get; set; }

        public string SERVICE { get; set; }

        public string SUBCONTRACTOR_NAME { get; set; }

        public string REGION { get; set; }
        public string PHASE { get; set; }
        public string OLT_VENDOR { get; set; }
        public string OLT_NAME { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string SITE_NAME { get; set; }
        public string SITE_NO { get; set; }
        public string SPLITTER1 { get; set; }
        public string SPLITTER2 { get; set; }
        public string INDOOR_INVOICE { get; set; }
        public string INDOOR_ONT_INVOICE_DATE { get; set; }
        public string INDOOR_ONT_PO { get; set; }
        public string INDOOR_ONT_STATUS { get; set; }
        public string OUTDOOR_INVOICE { get; set; }
        public string OUTDOO_INVOICE_DATE { get; set; }
        public string OUTDOO_PO { get; set; }
        public string OUTDOO_STATUS { get; set; }
        public string SUBCONTRACT_IN { get; set; }
        //public string ONT_VENDOR { get; set; }
        //public string ONT_MODEL { get; set; }
        //public string ONT_SERIAL_NO { get; set; }
        public string CPE_SN { get; set; }
        public string WIFI_ROUTER_SN { get; set; }
        public string ATA_SN { get; set; }
        public string STB_SN1 { get; set; }
        public string STB_SN2 { get; set; }
        public string STB_SN3 { get; set; }
        public string ONT_INVOICE { get; set; }
        public string ONT_INVOICE_DATE { get; set; }
        public string ONT_PO { get; set; }
        public string ONT_STATUS { get; set; }
        public decimal CNT { get; set; }
    }

    public class DetailLastmileAndCPEReportGridList
    {
        public string CUST_REQUST_DT { get; set; }
        public string CUST_REGISTER_DT { get; set; }
        public string CS_APPROVE_DT { get; set; }
        public string ACTIVITY { get; set; }
        public string FIBRENET_ID { get; set; }
        public string CUST_NAME { get; set; }
        public string CUST_STATUS { get; set; }
        public string CUST_STATUS_DT { get; set; }
        public string PACKAGE_NAME { get; set; }
        public string PACKAGE_CHANGE_DT { get; set; }
        public string ADDRESS_ID { get; set; }
        public string BUILDING_NAME { get; set; }
        public string BUSINESS_MODEL { get; set; }
        public string PRODUCT { get; set; }
        public string SERVICE { get; set; }
        public string SUBCONTRACTOR_NAME { get; set; }
        public string REGION { get; set; }
        public string PHASE { get; set; }
        public string OLT_VENDOR { get; set; }
        public string OLT_NAME { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string SITE_NAME { get; set; }
        public string SITE_NO { get; set; }
        public string SPLITTER1 { get; set; }
        public string SPLITTER2 { get; set; }
        public string INDOOR_INVOICE { get; set; }
        public string INDOOR_ONT_INVOICE_DATE { get; set; }
        public string INDOOR_ONT_PO { get; set; }
        public string INDOOR_ONT_STATUS { get; set; }
        public string OUTDOOR_INVOICE { get; set; }
        public string OUTDOO_INVOICE_DATE { get; set; }
        public string OUTDOO_PO { get; set; }
        public string OUTDOO_STATUS { get; set; }
        public string SUBCONTRACT_IN { get; set; }
        //public string ONT_VENDOR { get; set; }
        //public string ONT_MODEL { get; set; }
        //public string ONT_SERIAL_NO { get; set; }
        public string CPE_SN { get; set; }
        public string WIFI_ROUTER_SN { get; set; }
        public string ATA_SN { get; set; }
        public string STB_SN1 { get; set; }
        public string STB_SN2 { get; set; }
        public string STB_SN3 { get; set; }

        public string ONT_INVOICE { get; set; }
        public string ONT_INVOICE_DATE { get; set; }
        public string ONT_PO { get; set; }
        public string ONT_STATUS { get; set; }
    }

    public class SupContractorReportList
    {
        public decimal? ORG_ID { get; set; }
        public string SUB_CONTRACTOR_NAME_TH { get; set; }
        public string SUB_CONTRACTOR_NAME_EN { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SUBCONTRACT_EMAIL { get; set; }
        public string PHASE { get; set; }
        public string SUB_CONTRACTOR_CODE { get; set; }
        public string SUB_CONTRACTOR_FOR_MAIL { get; set; }
    }
    public class OLTList
    {
        public string OLT_VENDOR { get; set; }
        public string PHASE { get; set; }
        public string REGION { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string OLT_NAME { get; set; }
        public DateTime? REGISTER_DT { get; set; }
        public decimal? SERVICE_USED { get; set; }
        public decimal? PORT_QUAN { get; set; }
        public string OLT_STATUS { get; set; }
        public decimal? MAX_PORT { get; set; }
        public DateTime? MAX_DATE { get; set; }
        public decimal? LAST_PERIOD { get; set; }
        public decimal? CURR_PERIOD { get; set; }
        public decimal? DIFF_CURR_MAX { get; set; }

    }

    public class OSPList
    {
        public string sp_no_1 { get; set; }
        public string sp_no_2 { get; set; }
        public string invoice_no { get; set; }
        public DateTime? invoice_dt { get; set; }
        public string po_no { get; set; }
        public string paid_st { get; set; }
        public string REGION { get; set; }
        public string PHASE { get; set; }
        public string device_vendor { get; set; }
        public string OLT_NAME { get; set; }
        public string SITE_NAME { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string spilter_1_site_code { get; set; }
        public string fibrenet_id { get; set; }
        public DateTime? REGISTER_DT { get; set; }
        public string Order_Status { get; set; }
        public string fibrenet_status { get; set; }
        public DateTime? CUSTOMER_STATE_DATE { get; set; }
    }

    public class UpDateScreenModel
    {
        public string LastMileIndoor { get; set; }
        public string LastMileOutdoor { get; set; }
        public string CPE { get; set; }
        public string InternatNo { get; set; }
        public string PO { get; set; }
        public string Invoice { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public decimal CHKINDOOR { get; set; }
        public decimal CHKOUTDOOR { get; set; }
        public decimal CHKONT { get; set; }
    }

    public class UpdateScreenList
    {
        public DateTime? REGISTER_DT { get; set; }
        public string ACTIVITY { get; set; }
        public string FIBRENET_ID { get; set; }
        public string CUST_NAME { get; set; }
        public string PHASE_PO { get; set; }
        public string OLT_VENDOR { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string Subcon_Name_lastmile_indoor { get; set; }
        public string Subcon_Name_lastmile_outdoor { get; set; }
        public string INVOICE_NO_IN { get; set; }
        public DateTime? INVOICE_DT_IN { get; set; }
        public string PO_NO_IN { get; set; }
        public string PAID_ST_IN { get; set; }
        public string REMARK_IN { get; set; }
        public string INVOICE_NO_OUT { get; set; }
        public DateTime? INVOICE_DT_OUT { get; set; }
        public string PO_NO_OUT { get; set; }
        public string PAID_ST_OUT { get; set; }
        public string REMARK_OUT { get; set; }
        public string ONT_VENDOR { get; set; }
        public string INVOICE_NO_ONT { get; set; }
        public DateTime? INVOICE_DT_ONT { get; set; }
        public string PO_NO_ONT { get; set; }
        public string PAID_ST_ONT { get; set; }
        public string REMARK_ONT { get; set; }

        public string PAID_ST_IN_OLD { get; set; }
        public string PAID_ST_OUT_OLD { get; set; }
        public string PAID_ST_ONT_OLD { get; set; }
    }

    public class CriteriaModel
    {
        public string REPORT { get; set; }
        public string CRITERIA { get; set; }
        public string REPORT_DATE { get; set; }
    }

    public class CriteriaScreenModel
    {
        public string REPORT { get; set; }
        public string PAIDFOR { get; set; }
        public string REPORT_DATE { get; set; }
    }

    public class PAYGSearchDateModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class StandardFullConModel
    {
        public string Region { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class StandardFullConList
    {
        public string REGION { get; set; }
        public string OLT_NO { get; set; }
        public string OLT_PORT_OUT { get; set; }
        public string ODF_NO { get; set; }
        public string ODF_PORT_OUT { get; set; }
        public string SITE_NO { get; set; }
        public string SP1_NO { get; set; }
        public string SP1_PORT_OUT { get; set; }
        public string LATITUDE_SP1 { get; set; }
        public string LONGITUDE_SP1 { get; set; }
        public string SP2_NO { get; set; }
        public decimal AVAILABLE_PORT { get; set; }
        public decimal USED_PORT { get; set; }
        public string SP2_ALIAS { get; set; }
        public string LATITUDE_SP2 { get; set; }
        public string LONGITUDE_SP2 { get; set; }
        public string ADDR_NAME_EN { get; set; }
        public string ADDR_NAME_TH { get; set; }
    }

    public class AddressidListQueryModel
    {
        public string ADDRESS_ID { get; set; }
    }
}
