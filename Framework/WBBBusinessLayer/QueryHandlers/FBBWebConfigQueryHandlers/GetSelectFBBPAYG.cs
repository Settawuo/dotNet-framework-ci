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
    class GetSelectFBBPAYGQueryHandler : IQueryHandler<FBBPAYG_SUMLASTMILEQuery, List<FBBPAYG_Dropdown>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPAYG_Dropdown> _objService;

        public GetSelectFBBPAYGQueryHandler(ILogger logger, IEntityRepository<FBBPAYG_Dropdown> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<FBBPAYG_Dropdown> Handle(FBBPAYG_SUMLASTMILEQuery query)
        {
            try
            {
                var cur = new OracleParameter();
                cur.ParameterName = "cur";
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                if (query.ProcName == "p_get_address_id")
                {
                    string pack_fullname = "WBB.PKG_FBBPAYG_SUMLASTMILE." + query.ProcName;
                    List<FBBPAYG_Dropdown> executeResult = _objService.ExecuteReadStoredProc(pack_fullname,
                    new
                    {
                        p_region = query.Region,
                        cur = cur
                    }).ToList();
                    return executeResult;
                }
                else
                {
                    string pack_fullname = "WBB.PKG_FBBPAYG_SUMLASTMILE." + query.ProcName;
                    List<FBBPAYG_Dropdown> executeResult = _objService.ExecuteReadStoredProc(pack_fullname,
                    new
                    {
                        cur = cur
                    }).ToList();
                    return executeResult;
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                return null;
            }
        }
    }
}
