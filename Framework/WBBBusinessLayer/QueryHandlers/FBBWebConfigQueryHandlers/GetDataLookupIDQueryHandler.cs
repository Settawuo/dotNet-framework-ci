using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetDataLookupIDQueryHandler : IQueryHandler<GetDataLookupIDQuery, List<GetListDataLookupIDModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetListDataLookupIDModel> _objService;

        public GetDataLookupIDQueryHandler(ILogger logger, IEntityRepository<GetListDataLookupIDModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<GetListDataLookupIDModel> Handle(GetDataLookupIDQuery query)
        {
            try
            {
                var p_lookup_name = new OracleParameter();
                p_lookup_name.ParameterName = "p_lookup_name";
                p_lookup_name.Size = 2000;
                p_lookup_name.OracleDbType = OracleDbType.Varchar2;
                p_lookup_name.Direction = ParameterDirection.Input;
                p_lookup_name.Value = query.LOOKUP_NAME;

                var return_param_name = new OracleParameter();
                return_param_name.ParameterName = "return_param_name";
                return_param_name.OracleDbType = OracleDbType.Varchar2;
                return_param_name.Size = 2000;
                return_param_name.Direction = ParameterDirection.Output;

                var return_ontop_flag = new OracleParameter();
                return_ontop_flag.ParameterName = "return_ontop_flag";
                return_ontop_flag.OracleDbType = OracleDbType.Varchar2;
                return_ontop_flag.Size = 2000;
                return_ontop_flag.Direction = ParameterDirection.Output;

                var return_ontop_lookup = new OracleParameter();
                return_ontop_lookup.ParameterName = "return_ontop_lookup";
                return_ontop_lookup.OracleDbType = OracleDbType.Varchar2;
                return_ontop_lookup.Size = 2000;
                return_ontop_lookup.Direction = ParameterDirection.Output;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.BinaryFloat;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var p_page_index = new OracleParameter();
                p_page_index.ParameterName = "p_page_index";
                p_page_index.OracleDbType = OracleDbType.Int32;
                p_page_index.Direction = ParameterDirection.Input;
                p_page_index.Value = 1;

                var p_page_size = new OracleParameter();
                p_page_size.ParameterName = "p_page_size";
                p_page_size.OracleDbType = OracleDbType.Int32;
                p_page_size.Direction = ParameterDirection.Input;
                p_page_size.Value = 99999;

                var result_lookup_id_cur = new OracleParameter
                {
                    ParameterName = "result_lookup_id_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteStoredProcMultipleCursor(
                       "wbb.pkg_fixed_asset_prioritylookup.p_get_lookup_id",
                       new object[]
                       {
                            p_lookup_name,
                            p_page_index,
                            p_page_size,
                            //out
                            return_param_name,
                            return_ontop_flag,
                            return_ontop_lookup,
                            return_code,
                            return_msg,
                            result_lookup_id_cur

                       });
                List<GetListDataLookupIDModel> getListDataLookupIDModelList = new List<GetListDataLookupIDModel>();
                GetListDataLookupIDModel getListDataLookupIDModel = new GetListDataLookupIDModel();
                if (return_code.Value.ToString() == "0")
                {
                    DataTable dtTableRespones = (DataTable)executeResult[5];
                    List<result_lookup_id_cur> result_Lookup_Id_Cur_List = new List<result_lookup_id_cur>();
                    result_Lookup_Id_Cur_List = dtTableRespones.AsEnumerable()
                                                .Select(s => new result_lookup_id_cur()
                                                {
                                                    LOOKUP_ID = dtTableRespones.Columns.IndexOf("LOOKUP_ID") >= 0 ? Convert.ToString(s["LOOKUP_ID"]) : string.Empty,
                                                    LOOKUP_NAME = dtTableRespones.Columns.IndexOf("LOOKUP_NAME") >= 0 ? Convert.ToString(s["LOOKUP_NAME"]) : string.Empty,
                                                    base_price = dtTableRespones.Columns.IndexOf("base_price") >= 0 ? Convert.ToString(s["base_price"]) : string.Empty,
                                                    effective_date_start = dtTableRespones.Columns.IndexOf("effective_date_start") >= 0 ? Convert.ToString(s["effective_date_start"]) : string.Empty,
                                                    effective_date_to = dtTableRespones.Columns.IndexOf("effective_date_to") >= 0 ? Convert.ToString(s["effective_date_to"]) : string.Empty,
                                                    p_main_promo_code = dtTableRespones.Columns.IndexOf("p_main_promo_code") >= 0 ? Convert.ToString(s["p_main_promo_code"]) : string.Empty,

                                                    p_ORDER_TYPE = dtTableRespones.Columns.IndexOf("p_ORDER_TYPE") >= 0 ? Convert.ToString(s["p_ORDER_TYPE"]) : string.Empty,
                                                    p_PRODUCT_NAME = dtTableRespones.Columns.IndexOf("p_PRODUCT_NAME") >= 0 ? Convert.ToString(s["p_PRODUCT_NAME"]) : string.Empty,
                                                    p_Reject_reason = dtTableRespones.Columns.IndexOf("p_Reject_reason") >= 0 ? Convert.ToString(s["p_Reject_reason"]) : string.Empty,
                                                    p_product_owner = dtTableRespones.Columns.IndexOf("p_product_owner") >= 0 ? Convert.ToString(s["p_product_owner"]) : string.Empty,
                                                    v_same_day = dtTableRespones.Columns.IndexOf("v_same_day") >= 0 ? Convert.ToString(s["v_same_day"]) : string.Empty,
                                                    p_event_flow_flag = dtTableRespones.Columns.IndexOf("p_event_flow_flag") >= 0 ? Convert.ToString(s["p_event_flow_flag"]) : string.Empty,
                                                    p_request_sub_flag = dtTableRespones.Columns.IndexOf("p_request_sub_flag") >= 0 ? Convert.ToString(s["p_request_sub_flag"]) : string.Empty,

                                                    v_province = dtTableRespones.Columns.IndexOf("v_province") >= 0 ? Convert.ToString(s["v_province"]) : string.Empty,
                                                    v_district = dtTableRespones.Columns.IndexOf("v_district") >= 0 ? Convert.ToString(s["v_district"]) : string.Empty,
                                                    v_subdistrict = dtTableRespones.Columns.IndexOf("v_subdistrict") >= 0 ? Convert.ToString(s["v_subdistrict"]) : string.Empty,
                                                    p_addess_id = dtTableRespones.Columns.IndexOf("p_addess_id") >= 0 ? Convert.ToString(s["p_addess_id"]) : string.Empty,
                                                    v_fttr_flag = dtTableRespones.Columns.IndexOf("v_fttr_flag") >= 0 ? Convert.ToString(s["v_fttr_flag"]) : string.Empty,
                                                    p_subcontract_type = dtTableRespones.Columns.IndexOf("p_subcontract_type") >= 0 ? Convert.ToString(s["p_subcontract_type"]) : string.Empty,
                                                    p_subcontract_sub_type = dtTableRespones.Columns.IndexOf("p_subcontract_sub_type") >= 0 ? Convert.ToString(s["p_subcontract_sub_type"]) : string.Empty,
                                                    v_region = dtTableRespones.Columns.IndexOf("v_region") >= 0 ? Convert.ToString(s["v_region"]) : string.Empty,
                                                    p_org_id = dtTableRespones.Columns.IndexOf("p_org_id") >= 0 ? Convert.ToString(s["p_org_id"]) : string.Empty,

                                                    p_SUBCONTRACT_CODE = dtTableRespones.Columns.IndexOf("p_SUBCONTRACT_CODE") >= 0 ? Convert.ToString(s["p_SUBCONTRACT_CODE"]) : string.Empty,
                                                    p_SUBCONTRACT_NAME = dtTableRespones.Columns.IndexOf("p_SUBCONTRACT_NAME") >= 0 ? Convert.ToString(s["p_SUBCONTRACT_NAME"]) : string.Empty,
                                                    v_reused_flag = dtTableRespones.Columns.IndexOf("v_reused_flag") >= 0 ? Convert.ToString(s["v_reused_flag"]) : string.Empty,
                                                    distance_from = dtTableRespones.Columns.IndexOf("distance_from") >= 0 ? Convert.ToString(s["distance_from"]) : string.Empty,
                                                    distance_to = dtTableRespones.Columns.IndexOf("distance_to") >= 0 ? Convert.ToString(s["distance_to"]) : string.Empty,
                                                    v_subcontract_location = dtTableRespones.Columns.IndexOf("v_subcontract_location") >= 0 ? Convert.ToString(s["v_subcontract_location"]) : string.Empty,
                                                    indoor_cost = dtTableRespones.Columns.IndexOf("indoor_cost") >= 0 ? Convert.ToString(s["indoor_cost"]) : string.Empty,
                                                    outdoor_cost = dtTableRespones.Columns.IndexOf("outdoor_cost") >= 0 ? Convert.ToString(s["outdoor_cost"]) : string.Empty,
                                                    v_over_cost_pm = dtTableRespones.Columns.IndexOf("v_over_cost_pm") >= 0 ? Convert.ToString(s["v_over_cost_pm"]) : string.Empty,
                                                    v_max_distance = dtTableRespones.Columns.IndexOf("v_max_distance") >= 0 ? Convert.ToString(s["v_max_distance"]) : string.Empty,
                                                    v_symptom_group = dtTableRespones.Columns.IndexOf("v_symptom_group") >= 0 ? Convert.ToString(s["v_symptom_group"]) : string.Empty,
                                                    v_same_subs = dtTableRespones.Columns.IndexOf("v_same_subs") >= 0 ? Convert.ToString(s["v_same_subs"]) : string.Empty,
                                                    v_same_team = dtTableRespones.Columns.IndexOf("v_same_team") >= 0 ? Convert.ToString(s["v_same_team"]) : string.Empty,
                                                }).ToList();
                    getListDataLookupIDModel.return_code = return_code.Value.ToString();
                    getListDataLookupIDModel.return_msg = return_msg.Value.ToString();
                    getListDataLookupIDModel.result_lookup_id_cur_data = result_Lookup_Id_Cur_List;
                    getListDataLookupIDModel.return_param_name = return_param_name.Value.ToString();
                    getListDataLookupIDModel.return_ontop_flag = return_ontop_flag.Value.ToString();
                    getListDataLookupIDModel.return_ontop_lookup = return_ontop_lookup.Value.ToString();
                }
                getListDataLookupIDModelList.Add(getListDataLookupIDModel);

                return getListDataLookupIDModelList;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                //query.ret_code = "-1";
                //query.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_get_rule_id Error : " + ex.Message;

                return null;
            }
        }
    }
}
