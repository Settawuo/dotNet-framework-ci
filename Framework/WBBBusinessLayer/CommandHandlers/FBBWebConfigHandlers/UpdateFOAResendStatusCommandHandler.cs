using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdateFOAResendStatusCommandHandler : ICommandHandler<UpdateFOAResendStatusCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> _tran;

        public UpdateFOAResendStatusCommandHandler(ILogger ILogger
            , IEntityRepository<string> objService
            , IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> tran)
        {
            _logger = ILogger;
            _objService = objService;
            _tran = tran;
        }
        public void Handle(UpdateFOAResendStatusCommand command)
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

            try
            {
                _logger.Info("Start PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_foa_resend_status");

                command.P_TRANS_ID = getTran(command.P_SERIAL_NO, command.P_INTERNET_NO);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_foa_resend_status",
                       out paramOut,
                       new
                       {
                           command.P_TRANS_ID,
                           command.P_ORDER_NO,
                           command.P_SERIAL_NO,
                           command.P_INTERNET_NO,
                           command.P_SUBNUMBER,
                           command.P_ASSET_CODE,
                           command.P_MATERIAL_DOC,
                           command.P_DOC_YEAR,
                           command.P_COM_CODE,
                           command.P_ERR_CODE,
                           command.P_ERR_MSG,
                           command.P_REMARK,
                           ret_code,
                           ret_msg

                       });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
                _logger.Info("End PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_foa_resend_status" + ret_msg);

            }
            catch (Exception e)
            {
                _logger.Info(e.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call FOA Resend  By ORder CommandHandler : " + e.GetErrorMessage();
            }
        }

        public string getTran(string serial_no, string internet_no)
        {
            string _tranID = string.Empty;


            var _hgroup = (from c in _tran.Get() select c);

            _hgroup = (from c in _hgroup
                       where (c.INTERNET_NO == internet_no && c.SERIAL_NO == serial_no)
                       orderby c.MODIFY_DATE descending
                       select c);


            var Tranresult = (from c in _hgroup
                              select new GetTRANSID
                              {
                                  TRANSID = c.TRANS_ID,

                              }).FirstOrDefault();

            if (Tranresult != null)
            {


                _tranID = Tranresult.TRANSID.ToSafeString();
            }
            else

            {
                _tranID = "";
            }
            return _tranID;
        }

    }
    public class GetTRANSID
    {
        public string TRANSID { get; set; }
    }
}
