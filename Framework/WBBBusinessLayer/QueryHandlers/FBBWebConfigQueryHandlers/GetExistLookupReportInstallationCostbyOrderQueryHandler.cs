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
    public class GetExistLookupReportInstallationCostbyOrderQueryHandler : IQueryHandler<GetExistLookupReportInstallationCostbyOrderQuery, ExistReportInstallationCostbyOrderReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ExistCurReportInstallationCostbyOrderListModel> _objService;

        public GetExistLookupReportInstallationCostbyOrderQueryHandler(ILogger logger, IEntityRepository<ExistCurReportInstallationCostbyOrderListModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public ExistReportInstallationCostbyOrderReturn Handle(GetExistLookupReportInstallationCostbyOrderQuery query)
        {
            var returnForm = new ExistReportInstallationCostbyOrderReturn();
            try
            {
                var p_ORDER_NO = new OracleParameter();
                {
                    p_ORDER_NO.ParameterName = "p_ORDER_NO";
                    p_ORDER_NO.Size = 2000;
                    p_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
                    p_ORDER_NO.Direction = ParameterDirection.Input;
                    p_ORDER_NO.Value = query.p_ORDER_NO;
                };

                var p_ACCESS_NUMBER = new OracleParameter();
                {
                    p_ACCESS_NUMBER.ParameterName = "p_ACCESS_NUMBER";
                    p_ACCESS_NUMBER.Size = 2000;
                    p_ACCESS_NUMBER.OracleDbType = OracleDbType.Varchar2;
                    p_ACCESS_NUMBER.Direction = ParameterDirection.Input;
                    p_ACCESS_NUMBER.Value = query.p_ACCESS_NO;
                };

                var p_FOA_SUBMIT_DATE = new OracleParameter();
                {
                    p_FOA_SUBMIT_DATE.ParameterName = "p_FOA_SUBMIT_DATE";
                    p_FOA_SUBMIT_DATE.Size = 2000;
                    p_FOA_SUBMIT_DATE.OracleDbType = OracleDbType.Varchar2;
                    p_FOA_SUBMIT_DATE.Direction = ParameterDirection.Input;
                    p_FOA_SUBMIT_DATE.Value = query.p_FOA_SUBMIT_DATE;
                };

                var ret_over_cost = new OracleParameter();
                {
                    ret_over_cost.ParameterName = "ret_over_cost";
                    ret_over_cost.Size = 2000;
                    ret_over_cost.OracleDbType = OracleDbType.Int32;
                    ret_over_cost.Direction = ParameterDirection.Output;
                };

                var ret_total_price = new OracleParameter();
                {
                    ret_total_price.ParameterName = "ret_total_price";
                    ret_total_price.Size = 2000;
                    ret_total_price.OracleDbType = OracleDbType.Int32;
                    ret_total_price.Direction = ParameterDirection.Output;
                };

                var ret_code = new OracleParameter();
                {
                    ret_code.ParameterName = "ret_code";
                    ret_code.Size = 2000;
                    ret_code.OracleDbType = OracleDbType.Int32;
                    ret_code.Direction = ParameterDirection.Output;
                };

                var ret_msg = new OracleParameter();
                {
                    ret_msg.ParameterName = "ret_msg";
                    ret_msg.Size = 2000;
                    ret_msg.OracleDbType = OracleDbType.Varchar2;
                    ret_msg.Direction = ParameterDirection.Output;
                };

                var return_exists_lookup_cur = new OracleParameter();
                {
                    return_exists_lookup_cur.ParameterName = "return_exists_lookup_cur";
                    p_FOA_SUBMIT_DATE.Size = 2000;
                    return_exists_lookup_cur.OracleDbType = OracleDbType.RefCursor;
                    return_exists_lookup_cur.Direction = ParameterDirection.Output;
                };

                var return_main_lookup_cur = new OracleParameter();
                {
                    return_main_lookup_cur.ParameterName = "return_main_lookup_cur";
                    return_main_lookup_cur.Size = 2000;
                    return_main_lookup_cur.OracleDbType = OracleDbType.RefCursor;
                    return_main_lookup_cur.Direction = ParameterDirection.Output;
                };





                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE");

                var executeResult = _objService.ExecuteStoredProcMultipleCursor(
                    "WBB.PKG_PAYG_INSTALL_COST_RPT.p_get_exist_lookup",
                    new object[]
                    {
                        p_ORDER_NO,
                        p_ACCESS_NUMBER,
                        p_FOA_SUBMIT_DATE,
                        ret_over_cost,
                        ret_total_price,
                        ret_code,
                        ret_msg,
                        return_exists_lookup_cur,
                        return_main_lookup_cur
                    }).ToList();

                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                DataTable resp = new DataTable();
                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                //returnForm.ret_msgg = ret_msgg.Value != null ? ret_msgg.Value.ToString() : "-1";
                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                //returnForm.cur = executeResult;
                List<ExistCurReportInstallationCostbyOrderListModel> respList = new List<ExistCurReportInstallationCostbyOrderListModel>();
            
                resp = (DataTable)executeResult[4];

                if (executeResult[4] != null)
                {
                    resp = (DataTable)executeResult[4];
                    respList = resp.DataTableToList<ExistCurReportInstallationCostbyOrderListModel>();
                    returnForm.RETURN_EXISTS_LOOKUP_CUR = respList;
                }

                if (executeResult[5] != null)
                {
                    resp = (DataTable)executeResult[5];
                    respList = resp.DataTableToList<ExistCurReportInstallationCostbyOrderListModel>();
                    returnForm.RETURN_MAIN_LOOKUP_CUR = respList;
                }


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
