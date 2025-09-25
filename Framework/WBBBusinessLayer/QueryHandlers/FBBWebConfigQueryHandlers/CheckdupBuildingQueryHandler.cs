using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class CheckdupBuildingQueryHandler : IQueryHandler<CheckdupBuildingQuery, ConfigurationDormitoryBuildingData>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<ConfigurationDormitoryBuildingData> _ConfigurationDormitoryBuildingData;

        public CheckdupBuildingQueryHandler(ILogger logger, IEntityRepository<ConfigurationDormitoryBuildingData> configurationDormitoryBuildingData)
        {
            _logger = logger;
            //   _objService = objService;
            _ConfigurationDormitoryBuildingData = configurationDormitoryBuildingData;

        }
        public ConfigurationDormitoryBuildingData Handle(CheckdupBuildingQuery query)
        {
            ConfigurationDormitoryBuildingData searchresult = new ConfigurationDormitoryBuildingData();
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


                var executeResult = _ConfigurationDormitoryBuildingData.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_CHECK_DUP_BUILDING",
                  new
                  {
                      p_dormitory_id = query.p_dormitory_id,
                      p_building_th = query.p_building_th,
                      p_building_en = query.p_building_en,
                      p_room_amount = query.p_room_amount,

                      /// return //////
                      p_return_code = p_return_code,
                      p_return_message = p_return_message

                  });
                query.p_return_code = p_return_code.Value != null ? Convert.ToInt32(p_return_code.Value.ToSafeString()) : -1;
                searchresult.result = query.p_return_code;
                return searchresult;

            }
            catch (Exception ex)
            {
                searchresult.result = query.p_return_code;
                return searchresult;
            }
        }

    }
}