using System;
using System.Collections.Generic;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FbbQueryStatusLeaveMessageInterface" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FbbQueryStatusLeaveMessageInterface.svc or FbbQueryStatusLeaveMessageInterface.svc.cs at the Solution Explorer and start debugging.
    public class FbbQueryStatusLeaveMessageInterface : IFbbQueryStatusLeaveMessageInterface
    {
        #region Properties

        private ILogger _logger;
        private IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        #endregion

        #region Constructor

        public FbbQueryStatusLeaveMessageInterface(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
        }

        #endregion

        #region Public Method

        public ListStatusLeaveMessageResponse LeaveMessageQuery(GetListStatusLeaveMessageQuery query)
        {
            InterfaceLogCommand log = null;
            log = StartInterface<GetListStatusLeaveMessageQuery>(query, "LeaveMessageQuery", "", "", "FBBOR021");
            _logger.Info("FbbQueryStatusLeaveMessageInterface LeaveMessageQuery Start");

            try
            {
                var resultStatusLeaveMessage = new ListStatusLeaveMessageResponse();

                if (query != null)
                {
                    var chkIsNullOrEmpty = checkParamStatusLeaveMsgIsNullOrEmptyAndLength(query);
                    if (chkIsNullOrEmpty.ReturnChkInputParam)
                    {
                        resultStatusLeaveMessage = _queryProcessor.Execute(query);
                    }
                    else
                    {
                        resultStatusLeaveMessage.ReturnCode = "-1";
                        resultStatusLeaveMessage.ReturnMessage = chkIsNullOrEmpty.ReturnMessageChk.ToSafeString();
                        resultStatusLeaveMessage.SearchStatusList = new List<StatusLeaveMessageList>();
                    }
                }
                else
                {
                    resultStatusLeaveMessage.ReturnCode = "-1";
                    resultStatusLeaveMessage.ReturnMessage = "Error Field.";
                    resultStatusLeaveMessage.SearchStatusList = new List<StatusLeaveMessageList>();
                }

                EndInterface<ListStatusLeaveMessageResponse>(resultStatusLeaveMessage, log, "", "Success", "");
                _logger.Info("End FbbQueryStatusLeaveMessageInterface LeaveMessageQuery output msg: " + resultStatusLeaveMessage.ReturnMessage.ToSafeString());
                return resultStatusLeaveMessage;
            }
            catch (Exception ex)
            {
                EndInterface<Exception>(ex, log, "", "Failed", "");
                _logger.Info("Error call Webservice FbbQueryStatusLeaveMessageInterface LeaveMessageQuery output msg: " + ex.Message);
                return new ListStatusLeaveMessageResponse();
            }
        }

        #endregion

        #region Private Method

        private ChkValidateLeaveMessageModel checkParamStatusLeaveMsgIsNullOrEmptyAndLength(GetListStatusLeaveMessageQuery query)
        {
            ChkValidateLeaveMessageModel chkValidateModel = new ChkValidateLeaveMessageModel();

            if (query.CustName == "?" || query.CustSurname == "?" || query.ContactMobile == "?" || query.RefferenceNo == "?")
            {
                chkValidateModel.ReturnChkInputParam = false;
                chkValidateModel.ReturnMessageChk = "Error Input : ?";
            }
            else if ((!(string.IsNullOrEmpty(query.CustName)) && !(string.IsNullOrEmpty(query.CustSurname)))
                && string.IsNullOrEmpty(query.ContactMobile) && string.IsNullOrEmpty(query.RefferenceNo))
            {
                // Input CustName and CustSurname
                if (query.CustName.Length <= 1000 && query.CustSurname.Length <= 1000)
                {
                    chkValidateModel.ReturnChkInputParam = true;
                    chkValidateModel.ReturnMessageChk = "Complete";
                }
                else
                {
                    chkValidateModel.ReturnChkInputParam = false;
                    chkValidateModel.ReturnMessageChk = "Error Input Maximun Field : CustName and CustSurname not more than 1000";
                }
            }
            else if ((string.IsNullOrEmpty(query.CustName) && string.IsNullOrEmpty(query.CustSurname))
                && !string.IsNullOrEmpty(query.ContactMobile) && string.IsNullOrEmpty(query.RefferenceNo))
            {
                // Input ContactMobile
                if (query.ContactMobile.Length <= 100)
                {
                    chkValidateModel.ReturnChkInputParam = true;
                    chkValidateModel.ReturnMessageChk = "Complete";
                }
                else
                {
                    chkValidateModel.ReturnChkInputParam = false;
                    chkValidateModel.ReturnMessageChk = "Error Input Maximun Field : ContactMobile not more than 100";
                }
            }
            else if ((string.IsNullOrEmpty(query.CustName) && string.IsNullOrEmpty(query.CustSurname))
               && string.IsNullOrEmpty(query.ContactMobile) && !string.IsNullOrEmpty(query.RefferenceNo))
            {
                // Input RefferenceNo
                if (query.RefferenceNo.Length <= 100)
                {
                    chkValidateModel.ReturnChkInputParam = true;
                    chkValidateModel.ReturnMessageChk = "Complete";
                }
                else
                {
                    chkValidateModel.ReturnChkInputParam = false;
                    chkValidateModel.ReturnMessageChk = "Error Input Maximun Field : RefferenceNo not more than 100";
                }
            }
            else
            {
                chkValidateModel.ReturnChkInputParam = false;

                if (!string.IsNullOrEmpty(query.CustName) && string.IsNullOrEmpty(query.CustSurname))
                    chkValidateModel.ReturnMessageChk = "Error Please Input Required Field : CustSurname";

                else if (string.IsNullOrEmpty(query.CustName) && !string.IsNullOrEmpty(query.CustSurname))
                    chkValidateModel.ReturnMessageChk = "Error Please Input Required Field : CustName";

                else if ((!(string.IsNullOrEmpty(query.CustName)) && !(string.IsNullOrEmpty(query.CustSurname)))
                        && !string.IsNullOrEmpty(query.ContactMobile) && !string.IsNullOrEmpty(query.RefferenceNo))
                    chkValidateModel.ReturnMessageChk = "Error Field.";

                else chkValidateModel.ReturnMessageChk = "Error Required Field.";
            }

            return chkValidateModel;
        }

        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo, string createdBy)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = query.GetType().Name,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = createdBy,
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd,
            string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogCommand.Handle(dbIntfCmd);
        }

        #endregion
    }
}
