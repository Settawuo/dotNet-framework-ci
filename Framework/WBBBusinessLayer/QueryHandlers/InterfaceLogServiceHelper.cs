using WBBBusinessLayer.CommandHandlers;
using WBBBusinessLayer.CommandHandlers.WebServices;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers
{
    public static class InterfaceLogServiceHelper
    {
        #region FBB

        public static InterfaceLogCommand StartInterfaceLog<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> interfaceLog, T query,
         string transactionId, string methodName, string serviceName, string InIDCardNO, string InterfaceNode, string createBy)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId.ToSafeString(),
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = InIDCardNO,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = InterfaceNode,
                CREATED_BY = createBy,
            };

            var log = InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            //uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T output, InterfaceLogCommand dbIntfCmd,
            string result, string reason, string updateBy)
        {
            dbIntfCmd.ActionType = ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = reason.Length > 100 ? reason.Substring(0, 100) : reason;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();
            dbIntfCmd.UPDATED_BY = updateBy;

            InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            //uow.Persist();
        }

        public static InterfaceLogCommand StartInterfaceLogRawXml(IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> interfaceLog, string query,
        string transactionId, string methodName, string serviceName, string InIDCardNO, string InterfaceNode, string createBy)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = InIDCardNO,
                IN_XML_PARAM = query,
                INTERFACE_NODE = InterfaceNode,
                CREATED_BY = createBy,
            };

            var log = InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            //uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceLogRawXml(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            string output, InterfaceLogCommand dbIntfCmd,
            string result, string reason, string updateBy)
        {
            dbIntfCmd.ActionType = ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = reason.Length > 100 ? reason.Substring(0, 100) : reason;
            dbIntfCmd.OUT_XML_PARAM = output;
            dbIntfCmd.UPDATED_BY = updateBy;

            InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            //uow.Persist();
        }

        public static void DeductionLog<T>(IEntityRepository<string> objService, SaveDeductionLogCommand command, T query)
        {
            string p_xml_param = query.DumpToXml();
            if (command.p_action == "New" && p_xml_param != "")
            {
                command.p_req_xml_param = p_xml_param;
            }
            else if (command.p_action == "Update" && p_xml_param != "")
            {
                command.p_res_xml_param = p_xml_param;
            }
            DeductionLogHelper.Log(objService, command);
        }

        public static void SavePaymentSPDPLog<T>(IEntityRepository<string> objService, SavePaymentSPDPLogCommand command, T query)
        {
            string p_xml_param = query.DumpToXml();
            if (command.p_action == "New" && p_xml_param != "")
            {
                command.p_req_xml_param = p_xml_param;
            }
            else if (command.p_action == "Update" && p_xml_param != "")
            {
                command.p_res_xml_param = p_xml_param;
            }
            SavePaymentSPDPLogHelper.Log(objService, command);
        }

        #endregion

        #region 3BB

        public static InterfaceLog3BBCommand StartInterfaceLog3BB<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_3BB> interfaceLog, T query,
         string transactionId, string methodName, string serviceName, string InIDCardNO, string InterfaceNode, string createBy)
        {
            var dbIntfCmd = new InterfaceLog3BBCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = transactionId.ToSafeString(),
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = InIDCardNO,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = InterfaceNode,
                CREATED_BY = createBy,
            };

            var log = InterfaceLog3BBHelper.Log(interfaceLog, dbIntfCmd);
            //uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceLog3BB<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG_3BB> interfaceLog,
            T output, InterfaceLog3BBCommand dbIntfCmd,
            string result, string reason, string updateBy)
        {
            dbIntfCmd.ActionType = ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = reason.Length > 100 ? reason.Substring(0, 100) : reason;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();
            dbIntfCmd.UPDATED_BY = updateBy;

            InterfaceLog3BBHelper.Log(interfaceLog, dbIntfCmd);
            //uow.Persist();
        }

        #endregion
    }
}

