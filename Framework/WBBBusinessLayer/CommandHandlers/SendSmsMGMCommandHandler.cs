using Inetlab.SMPP;
using Inetlab.SMPP.Common;
using Inetlab.SMPP.PDU;
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
    public class SendSmsMGMCommandHandler : ICommandHandler<SendSmsMGMCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommandHandler;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly Object _thisLock = new Object();

        public SendSmsMGMCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            ICommandHandler<SendSmsCommand> sendSmsCommandHandler,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
            _sendSmsCommandHandler = sendSmsCommandHandler;
            _lov = lov;
        }

        public void Handle(SendSmsMGMCommand command)
        {
            //R22.06.14062022

            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;
            SendSmsCommand sendSmsCommand = new SendSmsCommand();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_refference_no, "GetDataSendSmsMGM", "SendSmsMGMCommandHandler", command.p_refference_no, "FBB|" + command.FullUrl, "WEB");

                var p_refference_no = new OracleParameter();
                p_refference_no.ParameterName = "p_refference_no";
                p_refference_no.OracleDbType = OracleDbType.Varchar2;
                p_refference_no.Size = 2000;
                p_refference_no.Direction = ParameterDirection.Input;
                p_refference_no.Value = command.p_refference_no;

                var p_coverage_result = new OracleParameter();
                p_coverage_result.ParameterName = "p_coverage_result";
                p_coverage_result.OracleDbType = OracleDbType.Varchar2;
                p_coverage_result.Size = 2000;
                p_coverage_result.Direction = ParameterDirection.Input;
                p_coverage_result.Value = command.p_coverage_result;

                var p_mgm_flag = new OracleParameter();
                p_mgm_flag.ParameterName = "p_mgm_flag";
                p_mgm_flag.OracleDbType = OracleDbType.Varchar2;
                p_mgm_flag.Size = 2000;
                p_mgm_flag.Direction = ParameterDirection.Input;
                p_mgm_flag.Value = command.p_mgm_flag;

                var p_language = new OracleParameter();
                p_language.ParameterName = "p_language";
                p_language.OracleDbType = OracleDbType.Varchar2;
                p_language.Size = 2000;
                p_language.Direction = ParameterDirection.Input;
                p_language.Value = command.p_language;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.Decimal;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.ParameterName = "return_message";
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var p_send_sms_mgm = new OracleParameter();
                p_send_sms_mgm.ParameterName = "p_send_sms_mgm";
                p_send_sms_mgm.OracleDbType = OracleDbType.RefCursor;
                p_send_sms_mgm.Direction = ParameterDirection.Output;

                var resultExecute = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR021.PROC_SEND_SMS_MGM",
                      new object[]
                      {
                          //Parameter Input
                          p_refference_no,
                          p_coverage_result,
                          p_mgm_flag,
                          p_language,
                          //Parameter Output
                          return_code,
                          return_message,
                          p_send_sms_mgm
                      });

                if (resultExecute != null)
                {

                    command.return_code = resultExecute[0] != null ? Convert.ToInt16(resultExecute[0].ToSafeString()) : -1;
                    command.return_message = resultExecute[1] != null ? resultExecute[1].ToString() : "";

                    DataTable dtSendSmsMGMRespones = (DataTable)resultExecute[2];
                    List<SendSmsMGMModel> sendSmsMGMlList = dtSendSmsMGMRespones.DataTableToList<SendSmsMGMModel>();
                    command.p_send_sms_mgm = sendSmsMGMlList;

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");

                    if (command.return_code.ToSafeString() == "0")
                    {
                        #region Send SMS

                        lock (_thisLock)
                        {
                            foreach (var item in command.p_send_sms_mgm)
                            {
                                var mobileNo = item.mobile;
                                if (!string.IsNullOrEmpty(mobileNo) && mobileNo.Length > 2)
                                {
                                    if (mobileNo.Substring(0, 2) != "66")
                                    {
                                        if (mobileNo.Substring(0, 1) == "0")
                                        {
                                            mobileNo = "66" + mobileNo.Substring(1);
                                        }
                                    }
                                }
                                var messageText = item.message_sms.ToSafeString();

                                sendSmsCommand = new SendSmsCommand
                                {
                                    Destination_Addr = mobileNo.ToString(),
                                    Source_Addr = "AISFIBRE",
                                    Message_Text = messageText.ToString(),
                                    Transaction_Id = item.mobile.ToString() + command.ClientIP.ToString(),
                                    FullUrl = command.FullUrl.ToString()
                                };

                                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, sendSmsCommand, item.mobile, "SendSmsMGM", "SendSmsMGMCommandHandler", item.refference_no, "FBB|" + command.FullUrl, "SMS");

                                try
                                {
                                    var SmsConfig = _lov.Get(l => l.LOV_TYPE.Equals("FBB_CONSTANT") && l.LOV_NAME.Equals("SMPP_CONFIGURATION"));
                                    string Host = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("HOST")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("HOST")).FirstOrDefault().LOV_VAL1;
                                    string Port = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PORT")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PORT")).FirstOrDefault().LOV_VAL1;
                                    string System_id = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMID")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMID")).FirstOrDefault().LOV_VAL1;
                                    string Password = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PASSWORD")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PASSWORD")).FirstOrDefault().LOV_VAL1;
                                    string System_type = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMTYPE")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMTYPE")).FirstOrDefault().LOV_VAL1;
                                    string Addr_Ton = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRNPI")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRNPI")).FirstOrDefault().LOV_VAL1;
                                    string Addr_Npi = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRTON")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRTON")).FirstOrDefault().LOV_VAL1;

                                    //chist699 20/02/2024 senderSMS
                                    if (sendSmsCommand.Source_Addr.ToUpper() == "AISFIBRE")
                                    {
                                        var GetsenderName = _lov.Get(l => l.LOV_TYPE.Equals("CONFIG") && l.LOV_NAME.Equals("SENDER_SMS")).FirstOrDefault().LOV_VAL1;
                                        string senderName = !string.IsNullOrEmpty(GetsenderName) ? GetsenderName : "AISFIBRE3";
                                        sendSmsCommand.Source_Addr = senderName;
                                    }

                                    using (SmppClient client = new SmppClient())
                                    {
                                        if (client.Connect(Host, int.Parse(Port)))
                                        {
                                            BindResp bindResp = client.Bind(System_id, Password);

                                            if (bindResp.Status == CommandStatus.ESME_ROK)
                                            {
                                                var submitResp = client.Submit(
                                                    SMS.ForSubmit()
                                                        .From(sendSmsCommand.Source_Addr)
                                                        .To(sendSmsCommand.Destination_Addr)
                                                        .Coding(DataCodings.UCS2)
                                                        .Text(sendSmsCommand.Message_Text));
                                                if (submitResp.All(x => x.Status == CommandStatus.ESME_ROK))
                                                {
                                                    sendSmsCommand.return_status = "Success";
                                                    //Success case
                                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, sendSmsCommand, log2, "Success", "", "SMS");
                                                }
                                                else
                                                {
                                                    sendSmsCommand.return_status = "Failed";
                                                    //Fail case
                                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, sendSmsCommand, log2, "Failed", "Connection Status is not open", "SMS");
                                                }
                                            }

                                            client.Disconnect();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sendSmsCommand.return_status = "";
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log2, "Failed", ex.Message, "SMS");
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command.return_code, log, "Failed", "Error", "");
                    }
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, return_code, log, "Failed", "Error", "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.return_code = -1;
                command.return_message = "Error save Campaign service " + ex.Message;
            }
        }
    }
}
