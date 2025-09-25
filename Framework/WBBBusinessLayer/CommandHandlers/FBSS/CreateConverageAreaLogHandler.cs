using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBSS;
using WBBData.Repository;
using WBBEntity.Extensions;
namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class CreateConverageAreaLogHandler : ICommandHandler<CoverageAreaLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public CreateConverageAreaLogHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(CoverageAreaLogCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPROC_CREATE_COVERAGE_AREA_LOG");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBB_COVERAGEAREA_LOG.p_fetch_cov_log",
                out paramOut,
                  new
                  {
                      //result = result,                     
                      ret_code = ret_code,
                      ret_msg = ret_msg

                  });

                command.Return_Code = ret_code.Value.ToSafeString();
                command.Return_Message = ret_msg.Value.ToSafeString();

                _logger.Info("EndPROC_CREATE_COVERAGE_AREA_LOG");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.Return_Code = "1";
                command.Return_Message = ex.GetErrorMessage();
            }
        }

    }
}
