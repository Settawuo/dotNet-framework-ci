using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWConfigurationAddressIDQueryHandler : IQueryHandler<GetAWConfigurationAddressIDQuery, List<ConfigurationAddressID>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ConfigurationAddressID> _ConfigurationAddressID;

        public GetAWConfigurationAddressIDQueryHandler(ILogger logger,
            IEntityRepository<ConfigurationAddressID> configurationAddressID)
        {
            _logger = logger;
            _ConfigurationAddressID = configurationAddressID;
        }

        public List<ConfigurationAddressID> Handle(GetAWConfigurationAddressIDQuery query)
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

                List<ConfigurationAddressID> executeResult = _ConfigurationAddressID.ExecuteReadStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_SEARCH_ADDRESS_ID",
                            new
                            {
                                p_region = query.Region,
                                p_user = query.User,
                                p_province = query.DormitoryProvince,
                                p_dormitory_name_th = query.DormitoryName,
                                p_flag = query.FibrenetIDFlag,

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
