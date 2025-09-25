using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices
{
    public class Quota_SubcontractCommandHandle : ICommandHandler<Quota_SubcontractCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public Quota_SubcontractCommandHandle(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(Quota_SubcontractCommand command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_TIME_SLOT.ROLLBACK_TIME_SLOT",
                out paramOut,
                    new
                    {
                        p_time_slot_id = command.TIME_SLOT_ID,
                        p_user = command.USER,

                        //return code
                        p_return_code = ret_code,

                    });

                command.ReturnCode = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call Quota_SubcontractCommand handles : " + ex.GetErrorMessage());
                command.ReturnCode = -1;
            }
        }
    }
}
