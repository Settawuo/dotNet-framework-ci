using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class ReportInstallationCostbyOrderQuery
    {
    }

    public class SearchReportInstallationCostbyOrderListQuery : IQuery<ReportInstallationCostbyOrderListReturn>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_PRODUCT_NAME { get; set; }
        public string p_PRODUCT_OWNER { get; set; }
        public string p_REPORT_TYPE { get; set; }
        public string p_SUBCONT_CODE { get; set; }
        public string p_ORG_ID { get; set; }
        //public string p_SUBCONT_NAME { get; set; }
        public string p_IR_DOC { get; set; }
        public string p_INVOICE_NO { get; set; }
        public string p_WORK_STATUS { get; set; }
        public string p_ORDER_STATUS { get; set; }
        public string p_ORD_STATUS { get; set; }
        public string p_ORDER_TYPE { get; set; }
        public string p_REGION { get; set; }

        public string p_SUBCONT_TYPE { get; set; }
        public string p_SUBCONT_SUB_TYPE { get; set; }
        public string p_FOA_FM { get; set; }
        public string p_FOA_TO { get; set; }
        public string p_APPROVE_FM { get; set; }
        public string p_APPROVE_TO { get; set; }
        public string p_PERIOD_FM { get; set; }
        public string p_PERIOD_TO { get; set; }
        public string p_TRANS_FM { get; set; }
        public string p_TRANS_TO { get; set; }
        public string p_UPDATE_BY { get; set; }
        public decimal P_PAGE_INDEX { get; set; }
        public decimal P_PAGE_SIZE { get; set; }
        //public string install_cur { get; set; }
        //public string ma_cur { get; set; }
    }


    public class SearchReportInstallationCostbyOrderListNewQuery : IQuery<ReportInstallationCostbyOrderListReturn>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_PRODUCT_NAME { get; set; }
        public string p_PRODUCT_OWNER { get; set; }
        public string p_SUBCONT_CODE { get; set; }
        public string p_ORG_ID { get; set; }
        //public string p_SUBCONT_NAME { get; set; }
        public string p_IR_DOC { get; set; }
        public string p_INVOICE_NO { get; set; }
        public string p_WORK_STATUS { get; set; }
        public string p_ORDER_STATUS { get; set; }
        public string p_ORD_STATUS { get; set; }
        public string p_ORDER_TYPE { get; set; }
        public string p_REGION { get; set; }

        public string p_SUBCONT_TYPE { get; set; }
        public string p_SUBCONT_SUB_TYPE { get; set; }
        public string p_FOA_FM { get; set; }
        public string p_FOA_TO { get; set; }
        public string p_APPROVE_FM { get; set; }
        public string p_APPROVE_TO { get; set; }
        public string p_PERIOD_FM { get; set; }
        public string p_PERIOD_TO { get; set; }
        public string p_TRANS_FM { get; set; }
        public string p_TRANS_TO { get; set; }
        public string p_UPDATE_BY { get; set; }
        public decimal P_PAGE_INDEX { get; set; }
        public decimal P_PAGE_SIZE { get; set; }
        public string p_REPORT_TYPE { get; set; }
    }
    public class GetReportInstallationCostbyOrderListQuery : IQuery<List<ReportInstallationCostbyOrderReturn>>
    {
        public string p_Command { get; set; }
    }

    public class ReportInstallationCostbyOrderReturn
    {
        public string PERIOD { get; set; }
        public Nullable<System.DateTime> ORDER_STATUS_DT { get; set; }
        public string ACCESS_NUMBER { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public Nullable<System.DateTime> APPOINTMENNT_DT { get; set; }
        public string PROMOTION_NAME { get; set; }
        public string SBC_CPY { get; set; }
        public decimal? DISTANCE_TOTAL { get; set; }
        public decimal? TOTAL_PAID { get; set; }
        public decimal? ENTRY_FEE { get; set; }
        public string ORG_ID { get; set; }
        public string ORDER_STATUS { get; set; }
        public string subcontract_email { get; set; }
    }

    public class GetReportInstallationCostScmOrderListQuery : IQuery<List<reportInstallScmOrderListModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_PRODUCT_NAME { get; set; }
        public string p_SUBCONT_CODE { get; set; }
        public string p_SUBCONT_NAME { get; set; }
        public string p_IR_DOC { get; set; }
        public string p_INVOICE_NO { get; set; }
        public string p_ORDER_STATUS { get; set; }
        public string p_REGION { get; set; }
        public string p_APPROVE_FM { get; set; }
        public string p_APPROVE_TO { get; set; }

        public string p_PERIOD_FM { get; set; }
        public string p_PERIOD_TO { get; set; }
        public string p_UPDATE_DT { get; set; }
        public string p_UPDATE_BY { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetReportInstallationFapoOrderListQuery : IQuery<List<reportInstallFapoOrderListModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_PRODUCT_NAME { get; set; }
        public string p_SUBCONT_CODE { get; set; }
        public string p_SUBCONT_NAME { get; set; }
        public string p_IR_DOC { get; set; }
        public string p_INVOICE_NO { get; set; }
        public string p_ORDER_STATUS { get; set; }
        public string p_REGION { get; set; }
        public string p_APPROVE_FM { get; set; }
        public string p_APPROVE_TO { get; set; }
        public string p_PERIOD_FM { get; set; }
        public string p_PERIOD_TO { get; set; }
        public string p_UPDATE_DT { get; set; }
        public string p_UPDATE_BY { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetReportInstallationAcctOrderListQuery : IQuery<List<reportInstallAcctOrderListModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_PRODUCT_NAME { get; set; }
        public string p_SUBCONT_CODE { get; set; }
        public string p_SUBCONT_NAME { get; set; }
        public string p_IR_DOC { get; set; }
        public string p_INVOICE_NO { get; set; }
        public string p_REGION { get; set; }
        public string p_ORDER_STATUS { get; set; }
        public string p_APPROVE_FM { get; set; }
        public string p_APPROVE_TO { get; set; }
        public string p_PERIOD_FM { get; set; }
        public string p_PERIOD_TO { get; set; }
        public string p_UPDATE_DT { get; set; }
        public string p_UPDATE_BY { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetReportInstallationOrderDetailQuery : IQuery<List<reportInstallOrderDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetReportInstallationCostbyOrderHistoryDetailQuery : IQuery<List<reportInstallOrderHistoryDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetReportInstallationDistanceDetailQuery : IQuery<List<reportInstallDistanceDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetReportInstallationCostbyOrderPostSapDetailQuery : IQuery<List<reportInstallPostSapDetail>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetUserReportInstallationCostbyOrderGroupQuery : IQuery<ReportInstallationCostbyOrderUserGroupModel>
    {
        public string p_USER_NAME { get; set; }
    }




    public class GetExistLookupReportInstallationCostbyOrderQuery : IQuery<ExistReportInstallationCostbyOrderReturn>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_FOA_SUBMIT_DATE { get; set; }

    }


    public class GetSelectedLookupReportInstallationCostbyOrderQuery : IQuery<SelectedLookupReportInstallationCostbyOrderReturn>
    {
        public string p_FOA_SUBMIT_DATE { get; set; }
        public string p_LOOKUP_NAME { get; set; }

    }

    public class GetReportInstallationCostbyOrderPackageDetailQuery : IQuery<List<reportInstallOrderPackageDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_cursor { get; set; }
        public string ret_code { get; set; }
    }

    public class GetReportInstallationCostbyOrderFeeDetailQuery : IQuery<List<reportInstallOrderFeeDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_cursor { get; set; }
        public string ret_code { get; set; }
    }
}
