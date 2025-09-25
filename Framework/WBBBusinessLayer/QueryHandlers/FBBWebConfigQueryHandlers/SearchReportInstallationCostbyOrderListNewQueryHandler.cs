using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SearchReportInstallationCostbyOrderListNewQueryHandler : IQueryHandler<SearchReportInstallationCostbyOrderListNewQuery, ReportInstallationCostbyOrderListReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReportInstallationCostbyOrderListModel_Binding> _objService;

        public SearchReportInstallationCostbyOrderListNewQueryHandler(ILogger logger, IEntityRepository<ReportInstallationCostbyOrderListModel_Binding> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public ReportInstallationCostbyOrderListReturn Handle(SearchReportInstallationCostbyOrderListNewQuery query)
        {
            var returnForm = new ReportInstallationCostbyOrderListReturn();
            try
            {
                var ret_code = new OracleParameter
                {
                    ParameterName = "ret_code",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };

                //var cur = new OracleParameter
                //{
                //    ParameterName = "cur",
                //    OracleDbType = OracleDbType.RefCursor,
                //    Direction = ParameterDirection.Output
                //}; 
                
                var install_cur = new OracleParameter
                {
                    ParameterName = "install_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                }; 
                
                var ma_cur = new OracleParameter
                {
                    ParameterName = "ma_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var ret_msgg = new OracleParameter
                {
                    ParameterName = "ret_msgg",
                    OracleDbType = OracleDbType.Clob,
                    Direction = ParameterDirection.Output
                };


                var p_ORDER_NO = new OracleParameter();
                p_ORDER_NO.ParameterName = "p_ORDER_NO";
                p_ORDER_NO.Size = 2000;
                p_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
                p_ORDER_NO.Direction = ParameterDirection.Input;
                p_ORDER_NO.Value = query.p_ORDER_NO;

                var p_ACCESS_NO = new OracleParameter();
                p_ACCESS_NO.ParameterName = "p_ACCESS_NO";
                p_ACCESS_NO.Size = 2000;
                p_ACCESS_NO.OracleDbType = OracleDbType.Varchar2;
                p_ACCESS_NO.Direction = ParameterDirection.Input;
                p_ACCESS_NO.Value = query.p_ACCESS_NO;

                var p_PRODUCT_NAME = new OracleParameter();
                p_PRODUCT_NAME.ParameterName = "p_PRODUCT_NAME";
                p_PRODUCT_NAME.Size = 2000;
                p_PRODUCT_NAME.OracleDbType = OracleDbType.Varchar2;
                p_PRODUCT_NAME.Direction = ParameterDirection.Input;
                p_PRODUCT_NAME.Value = query.p_PRODUCT_NAME;

                var p_SUBCONT_CODE = new OracleParameter();
                p_SUBCONT_CODE.ParameterName = "p_SUBCONT_CODE";
                p_SUBCONT_CODE.Size = 2000;
                p_SUBCONT_CODE.OracleDbType = OracleDbType.Varchar2;
                p_SUBCONT_CODE.Direction = ParameterDirection.Input;
                p_SUBCONT_CODE.Value = query.p_SUBCONT_CODE;

                var p_ORG_ID = new OracleParameter();
                p_ORG_ID.ParameterName = "p_ORG_ID";
                p_ORG_ID.Size = 2000;
                p_ORG_ID.OracleDbType = OracleDbType.Varchar2;
                p_ORG_ID.Direction = ParameterDirection.Input;
                p_ORG_ID.Value = query.p_ORG_ID;

                var p_SUBCONT_TYPE = new OracleParameter();
                p_SUBCONT_TYPE.ParameterName = "p_SUBCONT_TYPE";
                p_SUBCONT_TYPE.Size = 2000;
                p_SUBCONT_TYPE.OracleDbType = OracleDbType.Varchar2;
                p_SUBCONT_TYPE.Direction = ParameterDirection.Input;
                p_SUBCONT_TYPE.Value = query.p_SUBCONT_TYPE;

                var p_SUBCONT_SUB_TYPE = new OracleParameter();
                p_SUBCONT_SUB_TYPE.ParameterName = "p_SUBCONT_SUB_TYPE";
                p_SUBCONT_SUB_TYPE.Size = 2000;
                p_SUBCONT_SUB_TYPE.OracleDbType = OracleDbType.Varchar2;
                p_SUBCONT_SUB_TYPE.Direction = ParameterDirection.Input;
                p_SUBCONT_SUB_TYPE.Value = query.p_SUBCONT_SUB_TYPE;

                var p_IR_DOC = new OracleParameter();
                p_IR_DOC.ParameterName = "p_IR_DOC";
                p_IR_DOC.Size = 2000;
                p_IR_DOC.OracleDbType = OracleDbType.Varchar2;
                p_IR_DOC.Direction = ParameterDirection.Input;
                p_IR_DOC.Value = query.p_IR_DOC;

                var p_INVOICE_NO = new OracleParameter();
                p_INVOICE_NO.ParameterName = "p_INVOICE_NO";
                p_INVOICE_NO.Size = 2000;
                p_INVOICE_NO.OracleDbType = OracleDbType.Varchar2;
                p_INVOICE_NO.Direction = ParameterDirection.Input;
                p_INVOICE_NO.Value = query.p_INVOICE_NO;

                var p_WORK_STATUS = new OracleParameter();
                p_WORK_STATUS.ParameterName = "p_WORK_STATUS";
                p_WORK_STATUS.Size = 2000;
                p_WORK_STATUS.OracleDbType = OracleDbType.Varchar2;
                p_WORK_STATUS.Direction = ParameterDirection.Input;
                p_WORK_STATUS.Value = query.p_WORK_STATUS;

                var p_ORDER_STATUS = new OracleParameter();
                p_ORDER_STATUS.ParameterName = "p_ORDER_STATUS";
                p_ORDER_STATUS.Size = 2000;
                p_ORDER_STATUS.OracleDbType = OracleDbType.Varchar2;
                p_ORDER_STATUS.Direction = ParameterDirection.Input;
                p_ORDER_STATUS.Value = query.p_ORDER_STATUS;

                var p_ORDER_TYPE = new OracleParameter();
                p_ORDER_TYPE.ParameterName = "p_ORDER_TYPE";
                p_ORDER_TYPE.Size = 2000;
                p_ORDER_TYPE.OracleDbType = OracleDbType.Varchar2;
                p_ORDER_TYPE.Direction = ParameterDirection.Input;
                p_ORDER_TYPE.Value = query.p_ORDER_TYPE;

                var p_REGION = new OracleParameter();
                p_REGION.ParameterName = "p_REGION";
                p_REGION.Size = 2000;
                p_REGION.OracleDbType = OracleDbType.Varchar2;
                p_REGION.Direction = ParameterDirection.Input;
                p_REGION.Value = query.p_REGION;

                var p_FOA_FM = new OracleParameter();
                p_FOA_FM.ParameterName = "p_FOA_FM";
                p_FOA_FM.Size = 2000;
                p_FOA_FM.OracleDbType = OracleDbType.Varchar2;
                p_FOA_FM.Direction = ParameterDirection.Input;
                p_FOA_FM.Value = query.p_FOA_FM;

                var p_FOA_TO = new OracleParameter();
                p_FOA_TO.ParameterName = "p_FOA_TO";
                p_FOA_TO.Size = 2000;
                p_FOA_TO.OracleDbType = OracleDbType.Varchar2;
                p_FOA_TO.Direction = ParameterDirection.Input;
                p_FOA_TO.Value = query.p_FOA_TO;

                var p_APPROVE_FM = new OracleParameter();
                p_APPROVE_FM.ParameterName = "p_APPROVE_FM";
                p_APPROVE_FM.Size = 2000;
                p_APPROVE_FM.OracleDbType = OracleDbType.Varchar2;
                p_APPROVE_FM.Direction = ParameterDirection.Input;
                p_APPROVE_FM.Value = query.p_APPROVE_FM;

                var p_APPROVE_TO = new OracleParameter();
                p_APPROVE_TO.ParameterName = "p_APPROVE_TO";
                p_APPROVE_TO.Size = 2000;
                p_APPROVE_TO.OracleDbType = OracleDbType.Varchar2;
                p_APPROVE_TO.Direction = ParameterDirection.Input;
                p_APPROVE_TO.Value = query.p_APPROVE_TO;

                var p_PERIOD_FM = new OracleParameter();
                p_PERIOD_FM.ParameterName = "p_PERIOD_FM";
                p_PERIOD_FM.Size = 2000;
                p_PERIOD_FM.OracleDbType = OracleDbType.Varchar2;
                p_PERIOD_FM.Direction = ParameterDirection.Input;
                p_PERIOD_FM.Value = query.p_PERIOD_FM;

                var p_PERIOD_TO = new OracleParameter();
                p_PERIOD_TO.ParameterName = "p_PERIOD_TO";
                p_PERIOD_TO.Size = 2000;
                p_PERIOD_TO.OracleDbType = OracleDbType.Varchar2;
                p_PERIOD_TO.Direction = ParameterDirection.Input;
                p_PERIOD_TO.Value = query.p_PERIOD_TO;

                var p_TRANS_FM = new OracleParameter();
                p_TRANS_FM.ParameterName = "p_TRANS_FM";
                p_TRANS_FM.Size = 2000;
                p_TRANS_FM.OracleDbType = OracleDbType.Varchar2;
                p_TRANS_FM.Direction = ParameterDirection.Input;
                p_TRANS_FM.Value = query.p_TRANS_FM;

                var p_TRANS_TO = new OracleParameter();
                p_TRANS_TO.ParameterName = "p_TRANS_TO";
                p_TRANS_TO.Size = 2000;
                p_TRANS_TO.OracleDbType = OracleDbType.Varchar2;
                p_TRANS_TO.Direction = ParameterDirection.Input;
                p_TRANS_TO.Value = query.p_TRANS_TO;

                var p_PRODUCT_OWNER = new OracleParameter();
                p_PRODUCT_OWNER.ParameterName = "p_PRODUCT_OWNER";
                p_PRODUCT_OWNER.Size = 2000;
                p_PRODUCT_OWNER.OracleDbType = OracleDbType.Varchar2;
                p_PRODUCT_OWNER.Direction = ParameterDirection.Input;
                p_PRODUCT_OWNER.Value = query.p_PRODUCT_OWNER;

                var p_REPORT_TYPE = new OracleParameter();
                p_REPORT_TYPE.ParameterName = "p_REPORT_TYPE";
                p_REPORT_TYPE.Size = 2000;
                p_REPORT_TYPE.OracleDbType = OracleDbType.Varchar2;
                p_REPORT_TYPE.Direction = ParameterDirection.Input;
                p_REPORT_TYPE.Value = query.p_REPORT_TYPE;

                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE");


                
                    var executeResult = _objService.ExecuteStoredProcMultipleCursor(
                    //"WBB.PKG_FIXED_ASSET_LASTMILE.p_search_order_list",
                    "WBB.PKG_PAYG_INSTALL_COST_RPT.p_search_order",
                    new object[]
                    {
                        p_ORDER_NO,
                        p_ACCESS_NO,
                        p_PRODUCT_NAME,
                        p_SUBCONT_CODE,
                        p_ORG_ID,
                        p_SUBCONT_TYPE,
                        p_SUBCONT_SUB_TYPE,
                        p_IR_DOC,
                        p_INVOICE_NO,
                        p_WORK_STATUS,
                        p_ORDER_STATUS,
                        p_ORDER_TYPE,
                        p_REGION,
                        p_FOA_FM,
                        p_FOA_TO,
                        p_APPROVE_FM,
                        p_APPROVE_TO,
                        p_PERIOD_FM,
                        p_PERIOD_TO,
                        p_TRANS_FM,
                        p_TRANS_TO,
                        p_PRODUCT_OWNER,
                        p_REPORT_TYPE,

                        ret_code,
                        install_cur,
                        ma_cur,
                        ret_msgg
                    });
                //DataTable executeResult[1]resp = new DataTable();
                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                //returnForm.ret_msgg = ret_msgg.Value != null ? ret_msgg.Value.ToString() : "-1";
                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                //returnForm.cur = executeResult;
                string retmsg = "";
                string retcode = "";

                List<ReportInstallationCostbyOrderListModel_Binding> respList = new List<ReportInstallationCostbyOrderListModel_Binding>();
                if (executeResult == null || executeResult[0] == "1")
                {
                    returnForm.cur = respList;
                    return returnForm;
                }

                if (query.p_REPORT_TYPE == "INSTALL")
                {
                    respList = executeResult[1].Separate<ReportInstallationCostbyOrderListModel_Binding>();
                }
                else
                {
                    //resp = (DataTable)executeResult[2];
                    respList = executeResult[2].Separate<ReportInstallationCostbyOrderListModel_Binding>();
                }

               
                foreach (var item in respList)
                {
                    if(item.ACCESS_NUMBER != null)
                    {
                        item.ACCESS_NO = item.ACCESS_NUMBER;
                    }
                    else if(item.ACCESS_NO != null)
                    {
                        item.ACCESS_NUMBER = item.ACCESS_NO;
                    }
                }
                returnForm.cur = respList;
                return returnForm;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FIXED_ASSET_LASTMILE handles : " + ex.Message);

                //returnForm.ret_code = "-1";
                //returnForm.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_search_order_list Error : " + ex.Message;

                return null;
            }
        }
    }
}
