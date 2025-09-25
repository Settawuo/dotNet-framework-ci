using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    class GetListLookupNameQueryHandler : IQueryHandler<GetListLookupNameQuery, List<GetListLookupNameModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetListLookupNameModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetListLookupNameQueryHandler(ILogger logger, IEntityRepository<GetListLookupNameModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public List<GetListLookupNameModel> Handle(GetListLookupNameQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GetListLookupNameQueryHandler", "GetListLookupNameQueryHandler", "", "FBB", "WEB_CONFIG");

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.BinaryFloat;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var result_lookup_name_cur = new OracleParameter
                {
                    ParameterName = "result_lookup_name_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                //p_get_lookup_name
                var executeResult = _objService.ExecuteReadStoredProc(
                    "wbb.pkg_fixed_asset_prioritylookup.p_search_lookup_name",
                    new
                    {
                        return_code,
                        return_msg,
                        result_lookup_name_cur
                    }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, return_code, log, "Success", "", "");
                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                //query.ret_code = "-1";
                //query.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_get_rule_id Error : " + ex.Message;

                return null;
            }
        }
    }
}
