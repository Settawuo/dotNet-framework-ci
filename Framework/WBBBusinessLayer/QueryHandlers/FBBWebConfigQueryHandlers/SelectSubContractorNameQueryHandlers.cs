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
    public class SelectSubContractorNameQueryHandlers : IQueryHandler<SelectSubContractorNameQuery, List<SelectSubContractorModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SelectSubContractorModel> _objService;
        public SelectSubContractorNameQueryHandlers(ILogger logger, IEntityRepository<SelectSubContractorModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<SelectSubContractorModel> Handle(SelectSubContractorNameQuery query)
        {
            try
            {
                var p_sub_list = new OracleParameter
                {
                    ParameterName = "p_sub_list",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_FIXED_ASSET_LASTMILE.p_get_subcontract_name",
                    new
                    {
                        query.p_code,
                        query.p_name,
                        p_sub_list
                    }).ToList();


                if (query.p_code_distinct)
                {
                    executeResult = executeResult.GroupBy(p => p.SUB_CONTRACTOR_CODE).Select(g => g.First()).ToList();
                }

                return executeResult;
            }
            catch (Exception)
            {
                _logger.Info("Error SelectSubContractorNameQueryHandlers");
                return null;
            }
        }
    }
}
