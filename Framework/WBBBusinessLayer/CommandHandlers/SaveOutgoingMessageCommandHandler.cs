using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SaveOutgoingMessageCommandHandler : ICommandHandler<SaveOutgoingMessageCommand>
    {
        private readonly IEntityRepository<object> _objService;
        private readonly ILogger _logger;

        public SaveOutgoingMessageCommandHandler(IEntityRepository<object> objService, ILogger logger)
        {
            _objService = objService;
            _logger = logger;
        }

        public void Handle(SaveOutgoingMessageCommand command)
        {

            try
            {
                //Verify methodName
                var resultMethodName =
                    (Constants.SbnWebService)Enum.Parse(typeof(Constants.SbnWebService), command.MethodName.ToUpper());

                int start;
                int end;
                string airOrderNo;
                switch (command.Action)
                {
                    case ActionType.Insert:

                        //command.OrderRowId = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                        string mobileNo;
                        SaveOutgoingMessageModel model;
                        string orderRowId;
                        switch (resultMethodName)
                        {
                            case Constants.SbnWebService.SAVEORDERNEW:

                                model = new SaveOutgoingMessageModel
                                {
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    PCustRowId = "",
                                    POrderNo = "",
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = command.SoapXml,
                                    POutXmlParam = "",
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PStatus = "OPENING"
                                };

                                SaveData(model, out orderRowId);


                                break;

                            case Constants.SbnWebService.LISTPACKAGEBYSERVICE:


                                model = new SaveOutgoingMessageModel
                                {
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    PCustRowId = "",
                                    POrderNo = "",
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = command.SoapXml,
                                    POutXmlParam = "",
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PStatus = "OPENING"
                                };

                                SaveData(model, out orderRowId);

                                break;
                            case Constants.SbnWebService.CUSTREGISTERCOMMAND:

                                airOrderNo = command.AirOrderNo;

                                model = new SaveOutgoingMessageModel
                                {
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    PCustRowId = "",
                                    POrderNo = "",
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = command.SoapXml,
                                    POutXmlParam = "",
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PStatus = "OPENING"
                                };

                                SaveData(model, out orderRowId);

                                break;

                            case Constants.SbnWebService.GETLISTPACKAGEBYSERVICE:

                                airOrderNo = command.AirOrderNo;

                                model = new SaveOutgoingMessageModel
                                {
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    PCustRowId = "",
                                    POrderNo = "",
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = command.SoapXml,
                                    POutXmlParam = "",
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PStatus = "OPENING"
                                };

                                SaveData(model, out orderRowId);

                                break;
                        }

                        break;

                    case ActionType.Update:
                        switch (resultMethodName)
                        {
                            case Constants.SbnWebService.SAVEORDERNEW:


                                model = new SaveOutgoingMessageModel
                                {
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    POrderNo = "",
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = "",
                                    POutXmlParam = command.SoapXml,
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PStatus = "SUCCESS"
                                };

                                SaveData(model, out orderRowId);

                                break;

                            case Constants.SbnWebService.LISTPACKAGEBYSERVICE:
                                model = new SaveOutgoingMessageModel
                                {
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    POrderNo = "",
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = "",
                                    POutXmlParam = command.SoapXml,
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PStatus = "SUCCESS"
                                };

                                SaveData(model, out orderRowId);

                                break;

                            case Constants.SbnWebService.CUSTREGISTERCOMMAND:

                                airOrderNo = command.AirOrderNo;

                                model = new SaveOutgoingMessageModel
                                {
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    PCustRowId = command.MobileNo,
                                    POrderNo = airOrderNo,
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = "",
                                    POutXmlParam = "",
                                    PStatus = "SUCCESS"
                                };

                                SaveData(model, out orderRowId);

                                break;

                            case Constants.SbnWebService.GETLISTPACKAGEBYSERVICE:

                                airOrderNo = command.AirOrderNo;

                                model = new SaveOutgoingMessageModel
                                {
                                    PTransactionId = command.TransactionId.ToSafeString(),
                                    PAction = command.Action.ToSafeString().ToUpper(),
                                    PLogRowId = "",
                                    PCustRowId = command.MobileNo,
                                    POrderNo = airOrderNo,
                                    PServiceName = resultMethodName.ToSafeString(),
                                    PInXmlParam = "",
                                    POutXmlParam = "",
                                    PStatus = "SUCCESS"
                                };

                                SaveData(model, out orderRowId);

                                break;
                        }

                        break;
                }
            }
            catch (ArgumentException argumentException)
            {
                _logger.Info(string.Format("Warning : {0} , Action({1}), description = {2}", command.MethodName.ToSafeString().ToUpper(), command.Action.ToSafeString().ToUpper(), argumentException.Message));

            }
            catch (Exception ex)
            {
                _logger.Info(string.Format("ERROR : {0} , Action({1}), description = {2}", command.MethodName.ToSafeString().ToUpper(), command.Action.ToSafeString().ToUpper(), ex.Message));
            }
        }

        private void SaveData(SaveOutgoingMessageModel model, out string orderRowId)
        {

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var returnCode = new OracleParameter
            {
                OracleDbType = OracleDbType.Varchar2,
                Size = 2000,
                Direction = ParameterDirection.Output
            };

            var returnMessage = new OracleParameter
            {
                OracleDbType = OracleDbType.Varchar2,
                Size = 2000,
                Direction = ParameterDirection.Output
            };

            //var executeResult =
            //                   _objService.ExecuteStoredProc(
            //                       "WBB.PKG_FBB_CHANGE_PRO_FIELDWORK.ADD_SERVICELOG",
            //                       out paramOut,
            //                       new
            //                       {
            //                           p_action = model.PAction,
            //                           p_log_row_id = model.PLogRowId,
            //                           p_cust_row_id = model.PCustRowId,
            //                           p_order_no = model.POrderNo,
            //                           p_service_name = model.PServiceName,
            //                           p_in_xml_param = model.PInXmlParam,
            //                           p_out_xml_param = model.POutXmlParam,
            //                           p_status = model.PStatus,
            //                           p_transaction_id = model.PTransactionId.ToSafeString(),

            //                           return_code = returnCode,
            //                           return_message = returnMessage
            //                       });

            if (returnCode.Value != null && returnCode.Value.ToSafeString() == "0")
            {
                orderRowId = returnMessage.Value.ToSafeString();
            }
            else
            {
                orderRowId = "0";
            }
        }
    }
}
