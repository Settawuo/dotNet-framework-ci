using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.CommandHandlers.WebServices
{
    public class UpdateFBBSaleportalPreRegisterByOrdermcCommandHandler : ICommandHandler<UpdateFBBSaleportalPreRegisterByOrdermcCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _obj;

        public UpdateFBBSaleportalPreRegisterByOrdermcCommandHandler(
            ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> obj
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _obj = obj;
        }

        public void Handle(UpdateFBBSaleportalPreRegisterByOrdermcCommand command)
        {
            command.ReturnMessage = "Failed";
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.RefferenceNo,
                    "UpdateFBBSaleportalPreRegisterByOrdermc", "UpdateFBBSaleportalPreRegisterByOrdermcCommandHandler", "", "FBB|" + "", "");
            try
            {
                var P_REFFERENCE_NO = new OracleParameter();
                P_REFFERENCE_NO.ParameterName = "p_refference_no";
                P_REFFERENCE_NO.Size = 2000;
                P_REFFERENCE_NO.OracleDbType = OracleDbType.Varchar2;
                P_REFFERENCE_NO.Direction = ParameterDirection.Input;
                P_REFFERENCE_NO.Value = command.RefferenceNo;

                var P_ORDER_MC = new OracleParameter();
                P_ORDER_MC.ParameterName = "p_order_mc";
                P_ORDER_MC.Size = 2000;
                P_ORDER_MC.OracleDbType = OracleDbType.Varchar2;
                P_ORDER_MC.Direction = ParameterDirection.Input;
                P_ORDER_MC.Value = command.OrderMC;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "return_code";
                RETURN_CODE.Size = 2000;
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "return_message";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var IM_LIST_INFO_BY_ORDERMC = new OracleParameter();
                IM_LIST_INFO_BY_ORDERMC.ParameterName = "im_list_info_by_ordermc";
                IM_LIST_INFO_BY_ORDERMC.OracleDbType = OracleDbType.RefCursor;
                IM_LIST_INFO_BY_ORDERMC.Direction = ParameterDirection.Output;

                var resultExecute = _obj.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_LIST_INFO_BY_ORDERMC",
                      new object[]
                      {
                          //Parameter Input
                          P_REFFERENCE_NO,
                          P_ORDER_MC,
                          //Parameter Output
                          RETURN_CODE,
                          RETURN_MESSAGE,
                          IM_LIST_INFO_BY_ORDERMC
                      });

                if (resultExecute != null)
                {
                    DataTable dtOrdServiceRentalRespones = (DataTable)resultExecute[2];
                    List<OrderMCModel> OrdServiceRentalList = dtOrdServiceRentalRespones.DataTableToList<OrderMCModel>();
                    if (OrdServiceRentalList != null && OrdServiceRentalList.Count > 0)
                    {
                        OrderMCModel OrderMCData = OrdServiceRentalList.FirstOrDefault();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, OrderMCData, log,
                                        "Success", "", "");

                        RestClient client = new RestClient(OrderMCData.API_URL);
                        RestRequest request = new RestRequest();
                        request.Method = Method.POST;
                        request.AddHeader("TRANSACTION-ID", OrderMCData.TRANSACTION_ID);
                        request.AddHeader("App-Source", OrderMCData.App_Source);
                        request.AddHeader("App-Destination", OrderMCData.App_Destination);
                        request.AddHeader("ContentType", OrderMCData.ContentType);
                        OrderMCBody orderMCBody = new OrderMCBody()
                        {
                            ORDER_NO = OrderMCData.ORDER_NO,
                            ORDER_CHANNEL = OrderMCData.ORDER_CHANNEL,
                            IS_CONTACT_CUSTOMER = OrderMCData.IS_CONTACT_CUSTOMER,
                            IS_IN_COVERAGE = OrderMCData.IS_IN_COVERAGE,
                            USER_ACTION = OrderMCData.USER_ACTION,
                            DATE_ACTION = OrderMCData.DATE_ACTION,
                            ORDER_PRE_REGISTER = OrderMCData.ORDER_PRE_REGISTER,
                            STATUS_ORDER = OrderMCData.STATUS_ORDER,
                            REMARK_NOTE = OrderMCData.REMARK_NOTE
                        };
                        string BodyStr = JsonConvert.SerializeObject(orderMCBody);
                        OrderMCData.BODY_PARAMETER_STR = BodyStr;
                        request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                        // execute the request
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        ServicePointManager.ServerCertificateValidationCallback =
                            (s, certificate, chain, sslPolicyErrors) => true;
                        InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, OrderMCData, command.RefferenceNo,
                            "UpdateFBBSaleportalPreRegisterByOrdermcCallAPI", "UpdateFBBSaleportalPreRegisterByOrdermcCommandHandler", "", "FBB|" + "", "");
                        try
                        {
                            var response = client.Execute(request);

                            var content = response.Content; // raw content as string

                            if (HttpStatusCode.OK.Equals(response.StatusCode))
                            {
                                OrderMCResponse result = JsonConvert.DeserializeObject<OrderMCResponse>(response.Content) ?? new OrderMCResponse();
                                if (result != null)
                                {
                                    if (result.RESULT_CODE == "20000")
                                    {
                                        command.ReturnCode = result.RESULT_CODE;
                                        command.ReturnMessage = "";
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log2,
                                        "Success", "", "");
                                    }
                                    else
                                    {
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log2,
                                                                    "Failed", "", "");
                                    }
                                }
                                else
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "DeserializeObject Failed", log2,
                                                                    "Failed", "", "");
                                }
                            }
                            else
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response, log2,
                                                                "Failed", "", "");
                            }
                        }
                        catch (Exception ex)
                        {
                            command.ReturnMessage = ex.Message;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log2,
                                        "Failed", "", "");
                        }
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "PROC_LIST_INFO_BY_ORDERMC Nodata.", log,
                            "Failed", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                command.ReturnMessage = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log,
                            "Failed", "", "");
            }
        }
    }
}
