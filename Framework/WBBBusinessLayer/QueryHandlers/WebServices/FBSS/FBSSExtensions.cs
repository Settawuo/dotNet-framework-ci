using WBBBusinessLayer.CommandHandlers;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices.FBSS
{
    public static class FBSSExtensions
    {
        public static InterfaceLogCommand StartInterfaceFBSSLog<T>(IWBBUnitOfWork uow,
           IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
           T query, string transactionId, string serviceName,
           string methodName)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "FbbFbssInterface",
            };

            var log = InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        public static void EndInterfaceFBSSLog<T>(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            T output, InterfaceLogCommand dbIntfCmd,
            string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (reason.Length > 100 ? reason.Substring(0, 100) : reason);
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            InterfaceLogHelper.Log(interfaceLog, dbIntfCmd);
            uow.Persist();
        }
    }
}
