using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdateNoteLastMileByDistanceCommandHandler : ICommandHandler<LastMileUpdateNoteCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        public UpdateNoteLastMileByDistanceCommandHandler(ILogger ILogger, IEntityRepository<string> objService)
        {
            _logger = ILogger;
            _objService = objService;
        }

        public void Handle(LastMileUpdateNoteCommand command)
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
                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE.p_update_note");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_update_note",
                    out paramOut,
                    new
                    {
                        command.p_ACCESS_NO,
                        command.p_ORDER_NO,
                        command.p_USER,
                        command.p_REMARK_FOR_SUB,

                        ret_code,
                        ret_msg
                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
                _logger.Info("EndPKG_FIXED_ASSET_LASTMILE.p_update_by_order" + ret_msg);

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call UpdateNoteLastMileByDistanceCommandHandler Handler: " + ex.GetErrorMessage();
            }

        }

    }
}
