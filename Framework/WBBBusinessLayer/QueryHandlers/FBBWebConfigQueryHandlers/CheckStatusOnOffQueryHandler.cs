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
    public class CheckStatusOnOffQueryHandler : IQueryHandler<CheckStatusOnOffQuery, ConfigurationOnOffServices>
    {
        private readonly ILogger _logger;
        //  private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<ConfigurationOnOffServices> _ConfigurationOnOffServices;


        public CheckStatusOnOffQueryHandler(ILogger logger, IEntityRepository<ConfigurationOnOffServices> configurationOnOffServices)
        {
            _logger = logger;
            //   _objService = objService;
            _ConfigurationOnOffServices = configurationOnOffServices;

        }

        public ConfigurationOnOffServices Handle(CheckStatusOnOffQuery query)
        {
            ConfigurationOnOffServices searchresult = new ConfigurationOnOffServices();
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


                var executeResult = _ConfigurationOnOffServices.ExecuteStoredProc("WBB.PKG_FBBDORM_ADMIN001.PROC_CHECK_ON_WEB_DORMITORY",
                  new
                  {
                      p_dormitory_name = query.p_dormitory_name.ToSafeString(),
                      p_building = query.p_building.ToSafeString(),

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
                searchresult.result = -1;
                return searchresult;
            }
        }

    }
}