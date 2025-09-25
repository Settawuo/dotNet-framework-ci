using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers
{
    public class ReconcileStandardAddressCommandHandlers : ICommandHandler<ReconcileStandardAddressCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<string> _objService;

        public ReconcileStandardAddressCommandHandlers(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
        }

        public void Handle(ReconcileStandardAddressCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPROC_RECONCILE_STANDARD_ADDRESS");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBB_RECONCILE_STD_ADDRESS.p_get_row_table",
                out paramOut,
                  new
                  {
                      //result = result,
                      ret_code = ret_code,
                      ret_message = ret_message

                  });

                command.Return_Code = ret_code.Value.ToSafeString().ToSafeDecimal();
                command.Return_Message = ret_message.Value.ToSafeString();

                _logger.Info("StartPROC_RECONCILE_STANDARD_ADDRESS");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }
        }


    }
}
