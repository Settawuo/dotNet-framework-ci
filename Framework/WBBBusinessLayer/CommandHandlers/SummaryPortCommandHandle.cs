using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SummaryPortCommandHandle : ICommandHandler<SummaryPortCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public SummaryPortCommandHandle(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(SummaryPortCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_RPTPORT000.FBB_RPTPORT000",
                out paramOut,
                  new
                  {
                      p_create_by = command.Create_By.ToSafeString(),
                      p_report_code = command.Report_Code.ToSafeString(),
                      p_report_parameter = command.ReportParam.ToSafeString(),

                      //  return code
                      ret_code = ret_code,
                      ret_msg = v_error_msg
                  });

                command.Return_Code = 1;
                command.Return_Desc = "Success";
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.Return_Code = -1;
                command.Return_Desc = "Error call Summary handles : " + ex.GetErrorMessage();
            }
        }
    }
}
