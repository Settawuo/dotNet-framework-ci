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
    class GetDataPopupQueryHandler : IQueryHandler<GetDataPopupQuery, List<ListPopupModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ListPopupModel> _objService;

        public GetDataPopupQueryHandler(ILogger logger, IEntityRepository<ListPopupModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<ListPopupModel> Handle(GetDataPopupQuery query)
        {
            try
            {
                var p_search_column = new OracleParameter();
                p_search_column.ParameterName = "p_search_column";
                p_search_column.OracleDbType = OracleDbType.Varchar2;
                p_search_column.Size = 2000;
                p_search_column.Direction = ParameterDirection.Input;
                p_search_column.Value = query.p_search_column;

                var p_value = new OracleParameter();
                p_value.ParameterName = "p_value";
                p_value.OracleDbType = OracleDbType.Varchar2;
                p_value.Size = 2000;
                p_value.Direction = ParameterDirection.Input;
                p_value.Value = query.p_value;

                var ret_code = new OracleParameter();
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
                    "WBB.PKG_FIXED_ASSET_CONFIG_INSTALL.p_get_subcontract_desc",
                     new
                     {
                         p_search_column,
                         p_value,
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
                query.ret_code = "PKG_FIXED_ASSET_CONFIG_INSTALL.p_get_subcontract_desc Error : " + ex.Message;

                return null;
            }
        }
    }
}
