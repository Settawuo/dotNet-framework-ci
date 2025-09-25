using AIRNETEntity.Extensions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CreateOrderPendingPaymentHandler : IQueryHandler<CreateOrderPendingPaymentQuery, CreateOrderPendingPaymentModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<OrderPendingPaymentModel> _objService;
        private readonly IQueryHandler<GetPaymentEnquiryToSuperDuperQuery, GetPaymentEnquiryToSuperDuperModel> _queryHandlerEnquiry;
        private readonly IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> _queryHandlerConfigEnquiry;
        private readonly IQueryHandler<CreateOrderMeshQuery, CreateOrderMeshModel> _createOrderMeshHandler;
        private readonly IQueryHandler<CreateOrderSCPEQuery, CreateOrderSCPEModel> _createOrderScpeHandler;
        private readonly IQueryHandler<CreateOrderNewRegisterWTTxQuery, CreateOrderNewRegisterWTTxModel> _createOrderNewRegisterWTTxHandler;
        private readonly IEntityRepository<string> _objServiceLog;
        private readonly IQueryHandler<GetORDPendingPaymentTimeOutQuery, GetORDPendingPaymentTimeOutModel> _queryTimeoutHandler;
        private readonly IQueryHandler<ReleaseTimeSlotQuery, ReleaseTimeSlotModel> _queryReleaseTimeslotHandler;
        private readonly IQueryHandler<ResReleaseQuery, ResReleaseModel> _queryReleaseHandler;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IQueryHandler<GetListShortNamePackageQuery, List<ListShortNameModel>> _GetListShortNamePackage;
        private readonly ICommandHandler<SendSmsCommand> _SendSms;
        private readonly IQueryHandler<GetMeshCustomerProfileQuery, GetMeshCustomerProfileModel> _GetMeshCustomerProfile;
        private readonly IQueryHandler<SMSFlagRegisterPendingQuery, SMSFlagRegisterPendingModel> _smsFlagHandler;

        private string UpdateBy;
        public CreateOrderPendingPaymentHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<OrderPendingPaymentModel> objService,
            IQueryHandler<GetPaymentEnquiryToSuperDuperQuery, GetPaymentEnquiryToSuperDuperModel> queryHandlerEnquiry,
            IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> queryHandlerConfigEnquiry,
            IQueryHandler<CreateOrderMeshQuery, CreateOrderMeshModel> createOrderMeshHandler,
            IQueryHandler<CreateOrderSCPEQuery, CreateOrderSCPEModel> createOrderScpeHandler,
            IQueryHandler<CreateOrderNewRegisterWTTxQuery, CreateOrderNewRegisterWTTxModel> createOrderNewRegisterWTTxHandler,
            IEntityRepository<string> objServiceLog,
            IQueryHandler<GetORDPendingPaymentTimeOutQuery, GetORDPendingPaymentTimeOutModel> queryTimeoutHandler,
            IQueryHandler<ReleaseTimeSlotQuery, ReleaseTimeSlotModel> queryReleaseTimeslotHandler,
            IQueryHandler<ResReleaseQuery, ResReleaseModel> queryReleaseHandler,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IQueryHandler<GetListShortNamePackageQuery, List<ListShortNameModel>> getListShortNamePackage,
            ICommandHandler<SendSmsCommand> sendSms,
            IQueryHandler<GetMeshCustomerProfileQuery, GetMeshCustomerProfileModel> getMeshCustomerProfile,
            IQueryHandler<SMSFlagRegisterPendingQuery, SMSFlagRegisterPendingModel> smsFlagHandler)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
            _queryHandlerEnquiry = queryHandlerEnquiry;
            _queryHandlerConfigEnquiry = queryHandlerConfigEnquiry;
            _createOrderMeshHandler = createOrderMeshHandler;
            _createOrderScpeHandler = createOrderScpeHandler;
            _createOrderNewRegisterWTTxHandler = createOrderNewRegisterWTTxHandler;
            _objServiceLog = objServiceLog;
            _queryTimeoutHandler = queryTimeoutHandler;
            _queryReleaseTimeslotHandler = queryReleaseTimeslotHandler;
            _queryReleaseHandler = queryReleaseHandler;
            _lovService = lovService;
            _GetListShortNamePackage = getListShortNamePackage;
            _SendSms = sendSms;
            _GetMeshCustomerProfile = getMeshCustomerProfile;
            _smsFlagHandler = smsFlagHandler;
        }

        public CreateOrderPendingPaymentModel Handle(CreateOrderPendingPaymentQuery query)
        {
            var res = new CreateOrderPendingPaymentModel();
            InterfaceLogCommand log = null;
            try
            {
                UpdateBy = query.UpdateBy;
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "CreateOrderPendingPayment", "CreateOrderPendingPaymentHandler", "", UpdateBy, "");
                //get all order pending
                var orderPendingList = GetPendingOrderPayment("");

                var orderPayCompleteList = new List<string>();


                foreach (var order in orderPendingList)
                {


                    try
                    {
                        var checkDuplicateOrder = orderPayCompleteList.Where(item => item == order.order_id);
                        if (checkDuplicateOrder.Count() > 0) continue;

                        GetConfigReqPaymentModel configEnquiry = null;

                        if (order.product_name == "WTTx")
                        {
                            configEnquiry = GetConfigEnquiry(order.product_name.ToSafeString());
                        }
                        else
                        {
                            configEnquiry = GetConfigEnquiry();
                        }

                        if (configEnquiry != null && configEnquiry.list_config_req_payment != null && configEnquiry.list_config_req_payment.Count > 0)
                        {
                            var queryEnquiry = SetRequestEnquiry(configEnquiry, order);
                            GetPaymentEnquiryToSuperDuperModel resultEnquiry = new GetPaymentEnquiryToSuperDuperModel();
                            resultEnquiry.channel_type = "WEBREGISTER"; // for test
                            resultEnquiry = _queryHandlerEnquiry.Handle(queryEnquiry);

                            if (resultEnquiry != null && resultEnquiry.status == "SUCCESS")
                            {
                                orderPayCompleteList.Add(order.order_id);

                                CreateOrder(order, resultEnquiry);

                            }
                        }
                        else
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", "Not Config Enquiry", "");
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: Log interface error and continue create next order

                        var logerr = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, order, order.txn_id, "CreateByOrder", "CreateOrderPendingPaymentHandler", "", UpdateBy, "");
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logerr, "Error", ex.StackTrace, "");
                    }

                    //ก่อน Create Order check table fbb_register_pending_payment return_order != null
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, orderPendingList, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");
            }
            finally
            {
                GetOrderTimeOut();
            }

            return res;
        }

        private List<OrderPendingPaymentModel> GetPendingOrderPayment(string txn_id)
        {
            InterfaceLogCommand log = null;
            var data = new List<OrderPendingPaymentModel>();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, txn_id, txn_id, "GetPendingOrderPayment", "CreateOrderPendingPaymentHandler", "", UpdateBy, "");

                var p_transaction_id = new OracleParameter();
                p_transaction_id.ParameterName = "p_transaction_id";
                p_transaction_id.OracleDbType = OracleDbType.Varchar2;
                p_transaction_id.Direction = ParameterDirection.Input;
                p_transaction_id.Value = txn_id;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msgerr = new OracleParameter();
                ret_msgerr.ParameterName = "ret_message";
                ret_msgerr.OracleDbType = OracleDbType.Varchar2;
                ret_msgerr.Size = 2000;
                ret_msgerr.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "list_order_pending_spdp";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<OrderPendingPaymentModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_ORD_PENDING_SPDP",
                    new
                    {
                        p_transaction_id = p_transaction_id,
                        //  return code
                        ret_code = ret_code,
                        ret_message = ret_msgerr,

                        list_order_paending_payment = ioResults
                    }).ToList();

                if (ret_code.Value.ToSafeString() != "0")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ret_code.Value.ToSafeString(), "");
                }
                else
                {
                    if (executeResult != null && executeResult.Count > 0)
                    {
                        data = executeResult;
                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("GetPendingOrderPayment : Error.");
                _logger.Info(ex.Message);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");

                throw;
            }

            return data;
        }

        private GetConfigReqPaymentModel GetConfigEnquiry(string p_product_name = "")
        {
            string p_product_name_tmp = "ENQUIRY";
            if (p_product_name != "")
            {
                p_product_name_tmp = p_product_name;
            }
            var resultConfig = _queryHandlerConfigEnquiry.Handle(new GetConfigReqPaymentQuery()
            {
                p_product_name = p_product_name_tmp,
                p_service_name = "Enquiry"
            });

            return resultConfig;
        }

        private GetPaymentEnquiryToSuperDuperQuery SetRequestEnquiry(GetConfigReqPaymentModel config, OrderPendingPaymentModel order)
        {
            var request = new GetPaymentEnquiryToSuperDuperQuery()
            {
                p_order_id = order.order_id,
                User = UpdateBy,
                Url = config.list_config_req_payment.FirstOrDefault(item => item.attr_name == "endpoint").attr_value,
                MerchantID = config.list_config_req_payment.FirstOrDefault(item => item.attr_name == "X-sdpg-merchant-id").attr_value,
                Secret = config.list_config_req_payment.FirstOrDefault(item => item.attr_name == "channel_secret").attr_value,
                ContentType = "application/json",
                Nonce = Guid.NewGuid().ToString(),
                Body = new PaymentEnquiryToSuperDuperBody()
                {
                    txn_id = order.txn_id
                }
            };
            return request;
        }

        private void CreateOrder(OrderPendingPaymentModel order, GetPaymentEnquiryToSuperDuperModel resultEnquiry)
        {
            if (order.product_name == "TOPUP_MESH")
            {
                var resultMesh = _createOrderMeshHandler.Handle(new CreateOrderMeshQuery()
                {
                    //20.8 change channel -> channel_type
                    Channel = resultEnquiry.channel_type,
                    InternetNo = order.non_mobile_no,
                    OrderId = order.order_id,
                    txn_id = order.txn_id,
                    FullUrl = UpdateBy
                });

                if (resultMesh.RESULT_CODE != "0")
                {
                    //TODO: Rollback payment status
                    var commandRollback = new SavePaymentSPDPLogCommand()
                    {
                        p_action = "Rollback",
                        p_service_name = "BATCH",
                        p_user_name = UpdateBy,
                        p_order_id = order.order_id
                    };
                    InterfaceLogServiceHelper.SavePaymentSPDPLog(_objServiceLog, commandRollback, string.Empty);

                }
            }
            else if (order.product_name == "SELL_ROUTER")
            {
                var resultScpe = _createOrderScpeHandler.Handle(new CreateOrderSCPEQuery()
                {
                    //20.8 change channel -> channel_type
                    Channel = resultEnquiry.channel_type,
                    OrderId = order.order_id,
                    txn_id = order.txn_id,
                    FullUrl = UpdateBy
                });

                if (resultScpe.RESULT_CODE != "0")
                {
                    //TODO: Rollback payment status
                    var commandRollback = new SavePaymentSPDPLogCommand()
                    {
                        p_action = "Rollback",
                        p_service_name = "BATCH",
                        p_user_name = UpdateBy,
                        p_order_id = order.order_id
                    };
                    InterfaceLogServiceHelper.SavePaymentSPDPLog(_objServiceLog, commandRollback, string.Empty);
                }
            }
            else if (order.product_name == "WTTx")
            {
                CreateOrderNewRegisterWTTxModel resultWttx = _createOrderNewRegisterWTTxHandler.Handle(new CreateOrderNewRegisterWTTxQuery()
                {
                    Channel = resultEnquiry.channel_type,
                    OrderId = order.order_id,
                    txn_id = order.txn_id,
                    FullUrl = UpdateBy,
                    address_id = order.address_id,
                    payment_method = order.payment_method,
                    payment_transaction_date = order.payment_transaction_date,
                    access_mode = order.product_name
                });

                if (resultWttx.RESULT_CODE != "0")
                {
                    //TODO: Rollback payment status
                    var commandRollback = new SavePaymentSPDPLogCommand()
                    {
                        p_action = "Rollback",
                        p_service_name = "BATCH",
                        p_user_name = UpdateBy,
                        p_order_id = order.order_id
                    };
                    InterfaceLogServiceHelper.SavePaymentSPDPLog(_objServiceLog, commandRollback, string.Empty);
                }
            }

        }



        private void GetOrderTimeOut()
        {
            try
            {
                var lovData028 = GetLov("FBBOR028");
                var lovData041 = GetLov("FBBOR041");

                var MsgSentFailData = lovData028.Where(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR028") && l.LOV_NAME.Equals("SMS_MESSAGE_ERROR")).FirstOrDefault();
                string MsgSentFailTH = MsgSentFailData.LOV_VAL1;
                string MsgSentFailEN = MsgSentFailData.LOV_VAL2;

                //R24.10 Inactive Sent SMS TOPUP_MESH
                string flagTopupMeshSMSError = _lovService.Get(x => x.LOV_TYPE == "CONFIG" && x.LOV_NAME == "FLAG_TOPUP_MESH_SMS_ERROR" && x.ACTIVEFLAG == "Y").FirstOrDefault()?.LOV_VAL1 ?? "N";

                // GetJob TimeOut
                _logger.Info("GetORDPendingPaymentTimeOutModel");
                var ORDPendingPaymentTimeOutData = GetORDPendingPaymentTimeOut();
                if (ORDPendingPaymentTimeOutData != null && ORDPendingPaymentTimeOutData.GetORDPendingPaymentTimeOutList != null && ORDPendingPaymentTimeOutData.GetORDPendingPaymentTimeOutList.Count > 0)
                {
                    foreach (var item in ORDPendingPaymentTimeOutData.GetORDPendingPaymentTimeOutList)
                    {
                        if (item.order_type.ToSafeString() == "SELL_ROUTER")
                        {
                            if (item.reserve_timeslot_id != null && item.reserve_timeslot_id != "")
                            {
                                _logger.Info("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id);
                                ReleaseTimeSlotModel releaseTimeSlotModel = ReleaseTimeSlot(item.reserve_timeslot_id, item.order_id);
                                if (releaseTimeSlotModel.RESULT == "-1")
                                {
                                    _logger.Info("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "fail");
                                }
                                else
                                {
                                    _logger.Info("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "ok");
                                }
                            }
                            if (item.reserve_port_id != null && item.reserve_port_id != "")
                            {
                                _logger.Info("ResRelease reserve_port_id : " + item.reserve_port_id);
                                ResReleaseModel resReleaseModel = ResRelease(item.reserve_port_id, item.order_id);
                                if (resReleaseModel.RESULT == "-1")
                                {
                                    _logger.Info("ResRelease reserve_port_id : " + item.reserve_port_id + "fail");
                                }
                                else
                                {
                                    _logger.Info("ResRelease reserve_port_id : " + item.reserve_port_id + "ok");
                                }
                            }

                            _logger.Info("SendSMS order_id :" + item.order_id);
                            string MsgSent = "";
                            bool isThai = false;
                            if (item.eng_flag == "N")
                            {
                                isThai = true;
                                MsgSent = MsgSentFailTH;
                            }
                            else
                            {
                                isThai = false;
                                MsgSent = MsgSentFailEN;
                            }

                            if (CheckSMSFlag(item.order_id, "Error", item.mobile_no, UpdateBy))
                            {
                                var resultsms = SendSMS(item.mobile_no, "", isThai, MsgSent);
                                if (resultsms == "Success")
                                {
                                    UpdateSMSFlag(item.order_id, "Error", item.mobile_no, UpdateBy);
                                }
                            }
                        }
                        else if (item.order_type.ToSafeString() == "TOPUP_MESH")
                        {
                            if (item.reserve_timeslot_id != null && item.reserve_timeslot_id != "")
                            {
                                _logger.Info("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id);
                                ReleaseTimeSlotModel releaseTimeSlotModel = ReleaseTimeSlot(item.reserve_timeslot_id, item.order_id);
                                if (releaseTimeSlotModel.RESULT == "-1")
                                {
                                    _logger.Info("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "fail");
                                }
                                else
                                {
                                    _logger.Info("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "ok");
                                }
                            }

                            //R24.10 Inactive Sent SMS TOPUP_MESH
                            #region R24.10 Inactive Sent SMS TOPUP_MESH
                            if (flagTopupMeshSMSError == "Y")
                            {
                                GetMeshCustomerProfileModel informationDataModel = GetMeshCustomerProfile(item.internet_no, item.order_id);

                                if (informationDataModel != null && informationDataModel.contact_mobile != null)
                                {
                                    string ContactMobile = informationDataModel.contact_mobile.ToSafeString();
                                    ContactMobile = ContactMobile.Replace("-", "");

                                    string[] msgErrorTxts = new string[2];
                                    if (informationDataModel.language == "1")
                                    {
                                        msgErrorTxts[0] = lovData041.FirstOrDefault(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_ERROR_1")).LOV_VAL1.ToSafeString();
                                        msgErrorTxts[1] = lovData041.FirstOrDefault(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_ERROR_2")).LOV_VAL1.ToSafeString();
                                    }
                                    else
                                    {
                                        msgErrorTxts[0] = lovData041.FirstOrDefault(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_ERROR_1")).LOV_VAL2.ToSafeString();
                                        msgErrorTxts[1] = lovData041.FirstOrDefault(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_ERROR_2")).LOV_VAL2.ToSafeString();
                                    }

                                    /// Sent SMS
                                    _logger.Info("MeshSendErrorSMS");
                                    if (CheckSMSFlag(item.order_id, "Error", ContactMobile, UpdateBy))
                                    {
                                        var resultsms = MeshSendSMS(ContactMobile, msgErrorTxts, item.internet_no);
                                        if (resultsms == "Success")
                                        {
                                            UpdateSMSFlag(item.order_id, "Error", ContactMobile, UpdateBy);
                                        }
                                    }
                                }
                            }
                            #endregion

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info("CreateOrderPendingPaymentHandler.GetOrderTimeOut Error : " + ex.StackTrace);

            }
        }

        private GetORDPendingPaymentTimeOutModel GetORDPendingPaymentTimeOut()
        {
            var Results = new GetORDPendingPaymentTimeOutModel();

            try
            {
                Results = _queryTimeoutHandler.Handle(new GetORDPendingPaymentTimeOutQuery());
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetORDPendingPaymentTimeOut: " + ex.StackTrace);
            }

            return Results;
        }

        private ReleaseTimeSlotModel ReleaseTimeSlot(string RESERVED_ID, string ORDER_ID)
        {
            ReleaseTimeSlotQuery query = new ReleaseTimeSlotQuery
            {
                RESERVED_ID = RESERVED_ID,
                ORDER_ID = ORDER_ID
            };
            return _queryReleaseTimeslotHandler.Handle(query);
        }

        private ResReleaseModel ResRelease(string RES_RESERVATION_ID, string ORDER_ID)
        {
            ResReleaseQuery query = new ResReleaseQuery
            {
                RES_RESERVATION_ID = RES_RESERVATION_ID,
                ORDER_ID = ORDER_ID
            };
            return _queryReleaseHandler.Handle(query);
        }

        private Object thisLock = new Object();

        private string SendSMS(string mobileNo, string mainCode, bool IsThaiCulture, string msgtxt)
        {
            var resultsms = string.Empty;
            var tempMobileNo = mobileNo;
            string[] strMainCode = null;
            if (!string.IsNullOrEmpty(mainCode))
            {
                strMainCode = mainCode.Split('|');
            }
            lock (thisLock)
            {
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                List<string> promotionCode = new List<string>();
                if (strMainCode != null)
                {
                    foreach (string mCode in strMainCode)
                    {
                        if (!string.IsNullOrEmpty(mCode))
                        {
                            promotionCode.Add(mCode);
                        }

                    }
                }

                var air_old_list = new List<AIR_CHANGE_OLD_PACKAGE_ARRAY>();
                foreach (var code in promotionCode)
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        var air = new AIR_CHANGE_OLD_PACKAGE_ARRAY()
                        {
                            enddt = "",
                            productSeq = "",
                            sffPromotionCode = code,
                            startdt = ""
                        };
                        air_old_list.Add(air);
                    }
                }

                List<ListShortNameModel> result = new List<ListShortNameModel>();

                if (air_old_list != null && air_old_list.Count > 0)
                {

                    var query = new GetListShortNamePackageQuery()
                    {
                        id_card_no = "",
                        airChangePromotionCode_List = air_old_list,
                        transaction_id = "",
                        FullUrl = ""
                    };
                    result = _GetListShortNamePackage.Handle(query);
                }

                var source_addr_list = GetLovByTypeName("CONFIG", "SENDER_SMS");
                var source_addr = "";

                if (source_addr_list != null && source_addr_list.Any())
                {
                    source_addr = source_addr_list.FirstOrDefault().LovValue1;
                }
                else
                {
                    source_addr = "AISFIBRE";
                }

                var command = new SendSmsCommand();
                command.FullUrl = "";
                command.Source_Addr = source_addr;
                command.Destination_Addr = mobileNo;
                // Update 17.2
                command.Transaction_Id = tempMobileNo;
                var packageMainName = "";

                if (result != null && result.Count > 0)
                {
                    foreach (var name in result)
                    {
                        if (name.package_class == "MAIN")  //Main
                        {
                            if (IsThaiCulture)
                            {
                                packageMainName = name.package_Short_Name_TH;
                            }
                            else
                            {
                                packageMainName = name.package_Short_Name_EN;
                            }

                            msgtxt = msgtxt.Replace("{Package}", packageMainName);

                            command.Message_Text = msgtxt;
                            if (!string.IsNullOrEmpty(command.Message_Text))
                            {
                                _SendSms.Handle(command);
                                //Thread.Sleep(15000);
                            }
                            command.Message_Text = "";
                            msgtxt = "";

                            if (string.IsNullOrEmpty(resultsms))
                            {
                                resultsms = command.return_status;
                            }
                        }
                    }

                }
                else
                {
                    command.Message_Text = msgtxt;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _SendSms.Handle(command);
                        //Thread.Sleep(15000);
                    }
                    command.Message_Text = "";
                    msgtxt = "";

                    if (string.IsNullOrEmpty(resultsms))
                    {
                        resultsms = command.return_status;
                    }

                }



            }

            return resultsms;
        }

        private GetMeshCustomerProfileModel GetMeshCustomerProfile(string internetNo, string paymentOrderID)
        {
            GetMeshCustomerProfileModel result = null;
            GetMeshCustomerProfileQuery query = new GetMeshCustomerProfileQuery()
            {
                p_internet_no = internetNo,
                p_payment_order_id = paymentOrderID
            };
            result = _GetMeshCustomerProfile.Handle(query);
            return result;
        }

        private Object MeshthisLock = new Object();

        private string MeshSendSMS(string mobileNo, string[] msgtxts, string internetNo)
        {
            var resultsms = string.Empty;

            string FullUrl = "";

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = "";

            transactionId = ipAddress + "|";

            #endregion

            lock (MeshthisLock)
            {
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                var source_addr_list = GetLovByTypeName("CONFIG", "SENDER_SMS");
                var source_addr = "";

                if (source_addr_list != null)
                {
                    source_addr = source_addr_list.Any() ? source_addr_list.FirstOrDefault().LovValue1 : "AISFIBRE";
                }
                else
                {
                    source_addr = "AISFIBRE";
                }

                foreach (var item in msgtxts)
                {
                    var command = new SendSmsCommand();
                    command.FullUrl = FullUrl;
                    command.Source_Addr = source_addr;
                    command.Destination_Addr = mobileNo;
                    command.Transaction_Id = internetNo;

                    command.Message_Text = item;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _SendSms.Handle(command);
                    }
                    command.Message_Text = "";

                    if (string.IsNullOrEmpty(resultsms))
                    {
                        resultsms = command.return_status;
                    }
                }
            }

            return resultsms;
        }

        private bool CheckSMSFlag(string orderId, string status, string ContactMobile, string fullUrl)
        {
            var result = _smsFlagHandler.Handle(new SMSFlagRegisterPendingQuery()
            {
                Action = "Check",
                OrderId = orderId,
                Status = status,
                Mobile_No = ContactMobile,
                FullUrl = fullUrl
            });

            if (result != null && result.SendSMS_Flag != "Y")
            {
                return true;
            }

            return false;
        }

        private void UpdateSMSFlag(string orderId, string status, string ContactMobile, string fullUrl)
        {
            _smsFlagHandler.Handle(new SMSFlagRegisterPendingQuery()
            {
                Action = "Update",
                OrderId = orderId,
                Status = status,
                SMS_Flag = "Y",
                Mobile_No = ContactMobile,
                FullUrl = fullUrl
            });
        }

        private List<FBB_CFG_LOV> GetLov(string lov_val5 = "")
        {
            List<FBB_CFG_LOV> loveList = null;
            if (!string.IsNullOrEmpty(lov_val5))
            {
                loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_VAL5 == lov_val5)
                     .OrderBy(o => o.ORDER_BY).ToList();
            }
            return loveList;
        }

        private List<LovValueModel> GetLovByTypeName(string type = "", string name = "")
        {
            List<FBB_CFG_LOV> loveList = null;
            if (!string.IsNullOrEmpty(type))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == type && lov.LOV_NAME == name)
                     .OrderBy(o => o.ORDER_BY).ToList();
                }
                else
                {
                    loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == type)
                     .OrderBy(o => o.ORDER_BY).ToList();
                }

            }
            else if (!string.IsNullOrEmpty(name))
            {
                loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME == name)
                     .OrderBy(o => o.ORDER_BY).ToList();
            }

            var loveValueModelList = loveList.Select(l => new LovValueModel
            {
                Text = l.DISPLAY_VAL,
                Type = l.LOV_TYPE,
                Name = l.LOV_NAME,
                Id = l.LOV_ID,
                ParId = l.PAR_LOV_ID.ToSafeDecimal(),
                LovValue1 = l.LOV_VAL1,
                LovValue2 = l.LOV_VAL2,
                LovValue3 = l.LOV_VAL3,
                LovValue4 = l.LOV_VAL4,
                LovValue5 = l.LOV_VAL5,
                OrderBy = l.ORDER_BY,
                DefaultValue = l.DEFAULT_VALUE,
            }).ToList();

            return loveValueModelList;
        }
    }
}
