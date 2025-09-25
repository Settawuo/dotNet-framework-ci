using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;


namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class LastMileByDistanceBatchHandler : IQueryHandler<LastMileByDistanceBatchQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        public LastMileByDistanceBatchHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<string> Handle(LastMileByDistanceBatchQuery command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_OM010.p_read_file",
                    out paramOut,
                            new
                            {
                                //  return code
                                ret_code = ret_code
                            });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "1");

                return result;

            }
            catch (Exception ex)
            {
                command.ErrorMessage = "Error call service WBB.PKG_FBBPAYG_LOAD_OM010.p_read_file " + ex.Message;
                _logger.Info(ex.Message);
                return null;
            }

        }
    }
}
