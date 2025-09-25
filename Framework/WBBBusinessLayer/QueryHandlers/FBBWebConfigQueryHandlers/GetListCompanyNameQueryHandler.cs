using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandler
{
    class GetListCompanyNameQueryHandler : IQueryHandler<GetListSubCompanyNameQuery, List<ListSubCompanyNameModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ListSubCompanyNameModel> _objService;

        public GetListCompanyNameQueryHandler(ILogger logger, IEntityRepository<ListSubCompanyNameModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<ListSubCompanyNameModel> Handle(GetListSubCompanyNameQuery query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var cur = new OracleParameter
                {
                    ParameterName = "cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_FIXED_ASSET_CONFIG_INSTALL.p_get_company_name",
                    new
                    {
                        ret_code,
                        ret_msg,
                        cur
                    }).ToList();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ret_code = "-1";
                query.ret_code = "PKG_FIXED_ASSET_CONFIG_INSTALL.p_get_company_name Error : " + ex.Message;

                return null;
            }
        }
    }
}
