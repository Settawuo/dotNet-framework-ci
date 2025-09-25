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
    public class PAYGLoadFileToTableQueryHandler : IQueryHandler<PAYGLoadFilToTableQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public PAYGLoadFileToTableQueryHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<string> Handle(PAYGLoadFilToTableQuery command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOADFILE_TO_TABLE.p_read_file_all",
                    out paramOut,
                            new
                            {
                                //  return code
                                ret_code = ret_code
                            });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "1");
                _logger.Info("Package SUCCESS:" + ret_code.Value.ToString());
                return result;

            }
            catch (Exception ex)
            {
                _logger.Info("Package FAIL");
                command.ErrorMessage = "Error call service WBB.PKG_FBBPAYG_LOADFILE_TO_TABLE.p_read_file_all " + ex.Message;
                _logger.Info(ex.Message);
                return null;
            }

        }
    }
}
