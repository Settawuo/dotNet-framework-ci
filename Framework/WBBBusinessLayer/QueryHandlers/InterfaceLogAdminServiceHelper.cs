using WBBBusinessLayer.CommandHandlers;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers
{
    public static class InterfaceLogAdminServiceHelper
    {
        public static InterfaceLogAdminCommand StartInterfaceAdminLog<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog, T query,
         string transactionId, string methodName, string serviceName, string InIDCardNO)
        {
            var dbIntfCmd = new InterfaceLogAdminCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = InIDCardNO,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "ADMIN",
            };

            var log = InterfaceLogAdminHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceAdminLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog,
            T output, InterfaceLogAdminCommand dbIntfCmd,
            string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            InterfaceLogAdminHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();
        }
    }
}
