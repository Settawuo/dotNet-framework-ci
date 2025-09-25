using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetConfigDropDownQueryHandler : IQueryHandler<GetConfigDropDownQuery, List<GetConfigDropDownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetConfigDropDownModel> _objService;

        public GetConfigDropDownQueryHandler(ILogger logger, IEntityRepository<GetConfigDropDownModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<GetConfigDropDownModel> Handle(GetConfigDropDownQuery query)
        {
            try
            {
                var p_config_name = new OracleParameter();
                p_config_name.ParameterName = "p_config_name";
                p_config_name.Size = 2000;
                p_config_name.OracleDbType = OracleDbType.Varchar2;
                p_config_name.Direction = ParameterDirection.Input;
                p_config_name.Value = query.config_name;

                var p_symptom_group = new OracleParameter();
                p_symptom_group.ParameterName = "p_symptom_group";
                p_symptom_group.Size = 2000;
                p_symptom_group.OracleDbType = OracleDbType.Varchar2;
                p_symptom_group.Direction = ParameterDirection.Input;
                p_symptom_group.Value = query.symptom_group;

                var p_province_th = new OracleParameter();
                p_province_th.ParameterName = "p_province_th";
                p_province_th.Size = 2000;
                p_province_th.OracleDbType = OracleDbType.Varchar2;
                p_province_th.Direction = ParameterDirection.Input;
                p_province_th.Value = query.province_th;

                var p_district_th = new OracleParameter();
                p_district_th.ParameterName = "p_district_th";
                p_district_th.Size = 2000;
                p_district_th.OracleDbType = OracleDbType.Varchar2;
                p_district_th.Direction = ParameterDirection.Input;
                p_district_th.Value = query.district_th;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.BinaryFloat;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var result_config_dropdown_cur = new OracleParameter
                {
                    ParameterName = "result_config_dropdown_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc(
                    "wbb.pkg_fixed_asset_prioritylookup.p_get_config_dropdown",
                    new
                    {
                        p_config_name,
                        p_symptom_group,
                        p_province_th,
                        p_district_th,
                        return_code,
                        return_msg,
                        result_config_dropdown_cur
                    }).ToList();

                return executeResult;
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
