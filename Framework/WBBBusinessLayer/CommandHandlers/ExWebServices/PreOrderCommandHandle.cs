using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices
{
    public class PreOrderCommandHandle : ICommandHandler<PreOrderCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_PRE_REGISTER> _preRegister;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommandHandler;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<MemberGetMemberStatus> _objServiceMgm;

        public PreOrderCommandHandle(ILogger logger, IEntityRepository<FBB_PRE_REGISTER> preRegister, IEntityRepository<FBB_CFG_LOV> lov, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, ICommandHandler<SendSmsCommand> sendSmsCommandHandler, IEntityRepository<string> objService, IEntityRepository<MemberGetMemberStatus> objServiceMgm)
        {
            _logger = logger;
            _preRegister = preRegister;
            _lov = lov;
            _intfLog = intfLog;
            _uow = uow;
            _sendSmsCommandHandler = sendSmsCommandHandler;
            _objService = objService;
            _objServiceMgm = objServiceMgm;
        }

        public void Handle(PreOrderCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.PreOrderNo, "UpdateStatusPreRegister", "PreOrderCommandHandle", null, "FBB", "");

                // Update status Call package
                var retCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };

                var retMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 200,
                    Direction = ParameterDirection.Output
                };

                object[] paramOut;

                _objService.ExecuteStoredProc("WBB.PKG_FBBOR021.PROC_UPD_ORDER_STATUS",
                    out paramOut,
                    new
                    {
                        p_refference_no = command.PreOrderNo,
                        p_status = command.Status,

                        //return code
                        return_code = retCode,
                        return_message = retMessage
                    });

                command.ReturnCode = retCode.Value != null ? Convert.ToInt32(retCode.Value.ToSafeString()) : -1;
                command.ReturnMessage = retMessage.Value.ToSafeString();

                #region Check count register

                var overMaxRegister = false;
                if (command.ReturnCode == 0)
                {
                    var returnCode = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Decimal,
                        ParameterName = "return_code",
                        Direction = ParameterDirection.Output
                    };

                    var returnMessage = new OracleParameter
                    {
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "return_message",
                        Size = 2000,
                        Direction = ParameterDirection.Output
                    };

                    var ioResults = new OracleParameter
                    {
                        ParameterName = "ioResults",
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };

                    List<MemberGetMemberStatus> executeResult = _objServiceMgm.ExecuteReadStoredProc("WBB.PKG_FBBOR021.PROC_LIST_BY_ORDER_NO",
                       new
                       {
                           //in 
                           p_refference_no = command.PreOrderNo,
                           p_status = "ACTIVE",

                           /// Out
                           return_code = returnCode,
                           return_message = returnMessage,
                           ioResults = ioResults

                       }).ToList();

                    var lovMax =
                        (from lov in _lov.Get() where lov.LOV_NAME == "NEIGHBOR_TOTAL" select lov).SingleOrDefault() ??
                        new FBB_CFG_LOV();

                    var lovMaxCount = !string.IsNullOrEmpty(lovMax.DISPLAY_VAL)
                        ? Convert.ToInt16(lovMax.DISPLAY_VAL)
                        : 0;

                    overMaxRegister = executeResult.Count > lovMaxCount;

                }

                #endregion

                #region Send SMS

                //if (command.ReturnCode == 0 && command.Status.ToUpper() == "ACTIVE" && !overMaxRegister)
                //{

                //    //get mobile
                //    var preRegister =
                //        (from pRegister in _preRegister.Get()
                //            where pRegister.REFFERENCE_NO == command.PreOrderNo
                //            select pRegister).SingleOrDefault() ?? new FBB_PRE_REGISTER();
                //    var mobileNo = preRegister.CONTACT_MOBILE_NO; 
                //    if (!string.IsNullOrEmpty(mobileNo) && mobileNo.Length > 2)
                //    {
                //        if (mobileNo.Substring(0, 2) != "66")
                //        {
                //            if (mobileNo.Substring(0, 1) == "0")
                //            {
                //                mobileNo = "66" + mobileNo.Substring(1);
                //            }
                //        }
                //    }

                //    var lovSms =
                //        (from lov in _lov.Get() where lov.LOV_NAME == "SMS_INSTALL_ACTIVE" select lov).SingleOrDefault() ??
                //        new FBB_CFG_LOV();
                //    var spSms = preRegister.LANGUAGE == "E" ? lovSms.LOV_VAL2.Split(',') : lovSms.LOV_VAL1.Split(',');
                //    foreach (
                //        var sendSmsCommand in
                //            spSms.Select(
                //                sms =>
                //                    new SendSmsCommand
                //                    {
                //                        Destination_Addr = mobileNo,
                //                        Source_Addr = "AISFIBRE",
                //                        Message_Text = sms,
                //                        Transaction_Id = command.PreOrderNo
                //                    }))
                //    {
                //        _sendSmsCommandHandler.Handle(sendSmsCommand);
                //    }
                //}

                #endregion

                if (command.ReturnCode == 0)
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                else
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.ReturnMessage, "");
            }
            catch (Exception ex)
            {
                command.ReturnMessage = "Failed, + " + ex.GetErrorMessage();

                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }
        }
    }
}
