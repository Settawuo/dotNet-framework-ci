using WBBBusinessLayer.CommandHandlers;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers
{
    public static class InterfaceLogPayGServiceHelper
    {
        public static InterfaceLogPayGCommand StartInterfaceLogPayG<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_PAYG> interfaceLogPayG, T query,
         string transactionId, string methodName, string serviceName, string InIDCardNO, string InterfaceNode, string createBy)
        {
            var dbIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = InIDCardNO,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = InterfaceNode,
                CREATED_BY = createBy,
            };

            var log = InterfaceLogPayGHelper.Log(interfaceLogPayG, dbIntfCmd);
            //uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceLogPayG<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> interfaceLogPayG,
            T output, InterfaceLogPayGCommand dbIntfCmd,
            string result, string reason, string updateBy)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = reason.Length > 100 ? reason.Substring(0, 100) : reason;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();
            dbIntfCmd.UPDATED_BY = updateBy;

            InterfaceLogPayGHelper.Log(interfaceLogPayG, dbIntfCmd);
            //uow.Persist();
        }


        public static InterfaceLogPayGCommand StartInterfaceLogPayGRawXml(IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_PAYG> interfaceLogPayG, string query,
        string transactionId, string methodName, string serviceName, string InIDCardNO, string InterfaceNode, string createBy)
        {
            var dbIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = InIDCardNO,
                IN_XML_PARAM = query,
                INTERFACE_NODE = InterfaceNode,
                CREATED_BY = createBy,
            };

            var log = InterfaceLogPayGHelper.Log(interfaceLogPayG, dbIntfCmd);
            //uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceLogPayGRawXml(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> interfaceLogPayG,
            string output, InterfaceLogPayGCommand dbIntfCmd,
            string result, string reason, string updateBy)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = reason.Length > 100 ? reason.Substring(0, 100) : reason;
            dbIntfCmd.OUT_XML_PARAM = output;
            dbIntfCmd.UPDATED_BY = updateBy;

            InterfaceLogPayGHelper.Log(interfaceLogPayG, dbIntfCmd);
            //uow.Persist();
        }
    }
}
