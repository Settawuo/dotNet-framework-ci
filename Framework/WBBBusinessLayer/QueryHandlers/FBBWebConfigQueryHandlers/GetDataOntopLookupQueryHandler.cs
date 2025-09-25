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
    public class GetDataOntopLookupQueryHandler : IQueryHandler<GetDataOntopLookupQuery, List<GetListDataOntopLookupModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetListDataOntopLookupModel> _objService;

        public GetDataOntopLookupQueryHandler(ILogger logger, IEntityRepository<GetListDataOntopLookupModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<GetListDataOntopLookupModel> Handle(GetDataOntopLookupQuery query)
        {
            try
            {
                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.BinaryFloat;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var result_ontop_lookup_cur = new OracleParameter
                {
                    ParameterName = "result_ontop_lookup_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc(
                    "wbb.pkg_fixed_asset_prioritylookup.p_get_ontop_lookupname",
                    new
                    {
                        return_code,
                        return_msg,
                        result_ontop_lookup_cur
                    }).ToList();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);


                return null;
            }
        }
    }
}
