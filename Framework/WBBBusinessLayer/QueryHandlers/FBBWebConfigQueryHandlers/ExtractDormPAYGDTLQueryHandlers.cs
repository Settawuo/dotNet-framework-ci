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
    public class ExtractDormPAYGDTLQueryHandlers : IQueryHandler<GetExtractDormitoryDTL, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public ExtractDormPAYGDTLQueryHandlers(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<string> Handle(GetExtractDormitoryDTL command)
        {
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var result = new List<string>();
            var outp = new List<object>();
            var paramOut = outp.ToArray();

            try
            {
                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBDORM_REPORT_DTL.FBBDORM_REPORT_DTL",
                    out paramOut,
                            new
                            {
                                date_to = System.DateTime.Today.Date.ToString("ddMMyyyy"),
                                //  return code
                                ret_code = ret_code,
                                ret_msg = ret_msg

                            });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "1");
                result.Add(ret_msg.Value.ToSafeString());

                return result;

            }
            catch (Exception ex)
            {
                command.ErrorMessage = "Error call service WBB.PKG_FBBDORM_REPORT_DTL.FBBDORM_REPORT_DTL " + ex.Message;
                _logger.Info(ex.Message);

                result.Add("-1");
                result.Add("Error call service WBB.PKG_FBBDORM_REPORT_DTL.FBBDORM_REPORT_DTL " + ex.Message);

                return result;
            }

        }
    }
}
