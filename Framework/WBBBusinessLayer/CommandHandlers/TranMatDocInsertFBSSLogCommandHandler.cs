using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    //class TranMatDocInsertFBSSLogCommandHandler
    //{
    //}

    public class TranMatDocInsertFBSSLogCommandHandler : ICommandHandler<InsertTranMatDocLoadFileLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;

        public TranMatDocInsertFBSSLogCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
        }

        public void Handle(InsertTranMatDocLoadFileLogCommand command)
        {
            #region Parameter
            // Return Code
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Int32;
            ret_code.Direction = ParameterDirection.Output;

            // Return Message
            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            // Input
            var filename_log = new OracleParameter();
            filename_log.ParameterName = "filename_log";
            filename_log.Size = 2000;
            filename_log.OracleDbType = OracleDbType.Varchar2;
            filename_log.Direction = ParameterDirection.Input;
            filename_log.Value = command.filename;

            var filedate = new OracleParameter();
            filedate.ParameterName = "filedate";
            filedate.OracleDbType = OracleDbType.Date;
            filedate.Direction = ParameterDirection.Input;
            filedate.Value = command.filedate;

            var messege_log = new OracleParameter();
            messege_log.ParameterName = "messege_log";
            messege_log.Size = 2000;
            messege_log.OracleDbType = OracleDbType.Varchar2;
            messege_log.Direction = ParameterDirection.Input;
            messege_log.Value = command.message;

            var flag_type = new OracleParameter();
            flag_type.ParameterName = "flag_type";
            flag_type.Size = 3;
            flag_type.OracleDbType = OracleDbType.Varchar2;
            flag_type.Direction = ParameterDirection.Input;
            flag_type.Value = string.IsNullOrEmpty(command.flag_type) ? "N" : command.flag_type.ToUpper();

            #endregion

            #region Insert fbss_load_file_log
            try
            {
                var executeResults = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_SIM.p_insert_load_file_log",
                    new
                    {
                        // Parameter Input
                        filename_log,
                        filedate,
                        messege_log,
                        flag_type,
                        // Parameter Output
                        ret_code,
                        ret_msg
                    });

                //Return
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value == null ? "" : ret_msg.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_insert_load_file_log : " + ex.GetErrorMessage());
            }
            #endregion
        }
    }
}
