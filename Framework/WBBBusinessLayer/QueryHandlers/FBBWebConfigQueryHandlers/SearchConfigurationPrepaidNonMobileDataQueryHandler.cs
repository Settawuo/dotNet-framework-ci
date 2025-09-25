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
    public class SearchConfigurationPrepaidNonMobileDataQueryHandler : IQueryHandler<SearchConfigurationPrepaidNonMobileDataQuery, List<ConfigurationPrepaidNonMobileData>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ConfigurationPrepaidNonMobileData> _ConfigurationPrepaidNonMobileData;

        public SearchConfigurationPrepaidNonMobileDataQueryHandler(ILogger logger, IEntityRepository<ConfigurationPrepaidNonMobileData> ConfigurationPrepaidNonMobileData)
        {
            _logger = logger;
            _ConfigurationPrepaidNonMobileData = ConfigurationPrepaidNonMobileData;
        }
        public List<ConfigurationPrepaidNonMobileData> Handle(SearchConfigurationPrepaidNonMobileDataQuery query)
        {
            try
            {

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "p_return_code";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "p_return_message";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "p_res_data";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;

                List<ConfigurationPrepaidNonMobileData> executeResult = _ConfigurationPrepaidNonMobileData.ExecuteReadStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_SEARCH_MOBILE",
                            new
                            {
                                p_fibre = query.FibrenetID,
                                p_region = query.Region,
                                p_province = query.DormitoryProvince,
                                p_dormitory_name_th = query.DormitoryName,
                                p_building = query.BuildingNo,
                                p_floor = query.FloorNo,
                                p_room = query.RoomNo,
                                p_status = query.Stutus,
                                p_user = query.User,
                                /// return //////
                                p_return_code = p_return_code,
                                p_return_message = p_return_message,

                                p_res_data = p_res_data

                            }).ToList();

                var Return_Code = p_return_code.Value != null ? p_return_code.Value : "-1";
                var Return_Message = p_return_message.Value != null ? p_return_message.Value : "error";

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
