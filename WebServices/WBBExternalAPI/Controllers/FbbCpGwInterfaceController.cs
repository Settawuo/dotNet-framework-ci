using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WBBContract;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBEntity.Extensions;
using WBBExternalAPI.CompositionRoot;
using WBBExternalAPI.Models.Request.FbbCpGwInterfaceController;
using WBBExternalAPI.Models.Response.FbbCpGwInterfaceController;

namespace WBBExternalAPI.Controllers
{
    public class FbbCpGwInterfaceController : ApiController
    {
        #region Test Call

        [HttpGet]
        public string GetVersion()
        {
            return "External Web API R23.08_17082023_01";
        }

        [HttpPost]
        public string PostVersion([FromBody] string value)
        {
            return "External Web API R23.08_17082023_01 " + value;
        }

        #endregion Test Call


        #region R23 Call TransferFileToStorage On Prim

        [HttpPost]
        public TransferFileToStorageResponse TransferFileToStorage(TransferFileToStorageRequest request)
        {
            TransferFileToStorageResponse result = new TransferFileToStorageResponse();

            List<FileListData> requestFileListData = request.FileList.Select(d => new FileListData
            {
                OrderNo = d.OrderNo,
                Action = d.Action,
                FileName = d.FileName,
                DataFile = d.DataFile
            }).ToList();

            TransferFileToStorageCommand command = new TransferFileToStorageCommand()
            {
                UserName = !string.IsNullOrEmpty(request.UserName) ? request.UserName : "DOCNOTI",
                Option = request.Option,
                OrderNo = request.OrderNo,
                FileList = requestFileListData
            };

            try
            {
                if (request.Option == "2")
                {
                    if (request.FileList.Count > 0 && request.FileList != null)
                    {
                        if (request.FileList.getMoreThanOnceRepeated(z => new { z.OrderNo, z.Action, z.FileName }).ToList().Count > 0)
                        {
                            command.FileList = requestFileListData.getListNonDuplicated(z => new { z.OrderNo, z.Action, z.FileName }).ToList();
                        }

                        ExecuteCommand(command);

                        result.ResultCode = !string.IsNullOrEmpty(command.Return_code) ? command.Return_code : "50001";
                        result.ResultDesc = command.Return_message;
                        result.TRANSACTION_ID = command.Transaction_id;
                        result.OrderNo = null;
                        result.FileName = null;
                        result.DataFile = null;
                        result.FileList = command.FileList;
                    }
                }
                else if (request.Option == "12")
                {
                    if (request.OrderNo != "" || request.OrderNo != null)
                    {
                        ExecuteCommand(command);

                        result.ResultCode = !string.IsNullOrEmpty(command.Return_code) ? command.Return_code : "50001";
                        result.ResultDesc = command.Return_message;
                        result.TRANSACTION_ID = command.Transaction_id;
                        result.OrderNo = null;
                        result.FileName = null;
                        result.DataFile = null;
                        result.FileList = command.FileList;
                    }
                }
                else
                {
                    result.ResultCode = "40002";
                    result.ResultDesc = "Event not found";
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = "50001";
                result.ResultDesc = ex.Message;
            }

            return result;
        }

        #endregion R23 Call TransferFileToStorage On Prim


        #region Private Zone

        private object ExecuteCommand(dynamic command)
        {
            Type commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic commandHandler = Bootstrapper.GetInstance(commandHandlerType);

            commandHandler.Handle(command);

            // Instead of returning the output property of a command, we just return the complete command.
            // There is some overhead in this, but is of course much easier than returning a part of the command.
            return command;
        }

        private object ExecuteQuery(dynamic query)
        {
            Type queryType = query.GetType();
            Type resultType = GetQueryResultType(queryType);
            Type queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);

            dynamic queryHandler = Bootstrapper.GetInstance(queryHandlerType);

            return queryHandler.Handle(query);
        }

        private static Type GetQueryResultType(Type queryType)
        {
            return GetQueryInterface(queryType).GetGenericArguments()[0];
        }

        private static Type GetQueryInterface(Type type)
        {
            return (
                from @interface in type.GetInterfaces()
                where @interface.IsGenericType
                where typeof(IQuery<>).IsAssignableFrom(@interface.GetGenericTypeDefinition())
                select @interface)
                .SingleOrDefault();
        }

        #endregion Private Zone Excute
    }
}