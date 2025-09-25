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
    class GetSelectFBBPAYGSubSontractHandler : IQueryHandler<FBBPAYG_DropdownSUBSONTRACTQuery, List<FBBPAYG_Dropdown>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPAYG_Dropdown> _objService;

        public GetSelectFBBPAYGSubSontractHandler(ILogger logger, IEntityRepository<FBBPAYG_Dropdown> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<FBBPAYG_Dropdown> Handle(FBBPAYG_DropdownSUBSONTRACTQuery query)
        {
            try
            {
                var cur = new OracleParameter();
                cur.ParameterName = query.CurName;
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                string pack_fullname = query.PackageName + "." + query.ProcName;
                List<FBBPAYG_Dropdown> executeResult = _objService.ExecuteReadStoredProc(pack_fullname,
                new
                {
                    cur = cur
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
