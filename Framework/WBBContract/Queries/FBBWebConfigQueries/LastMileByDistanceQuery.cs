using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class LastMileByDistanceQuery
    {
    }

    public class SearchLastMileByDistanceOrderListQuery : IQuery<LastMileByDistanceOrderListReturn>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_PRODUCT_NAME { get; set; }
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
    }


    public class SearchLastMileByDistanceOrderListNewQuery : IQuery<LastMileByDistanceOrderListReturn>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string p_PRODUCT_NAME { get; set; }
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

        // 3BB Integration Track OSS Multi Company
        public string p_PRODUCT_OWNER { get; set; }
    }
    public class GetLastMileByDistanceOrderListQuery : IQuery<List<LastMileByDistanceOrderReturn>>
    {
        public string p_Command { get; set; }
    }

    public class LastMileByDistanceOrderReturn
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

    public class GetLastMileByDistanceScmOrderListQuery : IQuery<List<ScmOrderListModel>>
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

    public class GetLastMileByDistanceFapoOrderListQuery : IQuery<List<FapoOrderListModel>>
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

    public class GetLastMileByDistanceAcctOrderListQuery : IQuery<List<AcctOrderListModel>>
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

    public class GetLastMileByDistanceOrderDetailQuery : IQuery<List<OrderDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetLastMileByDistanceOrderHistoryDetailQuery : IQuery<List<OrderHistoryDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetDistanceDetailQuery : IQuery<List<DistanceDetailModel>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetLastMileByDistancePostSapDetailQuery : IQuery<List<PostSapDetail>>
    {
        public string p_ORDER_NO { get; set; }
        public string p_ACCESS_NO { get; set; }
        public string ret_code { get; set; }
        public string cur { get; set; }
    }

    public class GetUserGroupQuery : IQuery<LastMileByDistanceUserGroupModel>
    {
        public string p_USER_NAME { get; set; }
    }
}
