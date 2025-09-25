using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CreateOrderMeshHandler : IQueryHandler<CreateOrderMeshQuery, CreateOrderMeshModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly ICommandHandler<CustRegisterJobCommand> _custRegJobCommand;
        private readonly ICommandHandler<SendSmsCommand> _SendSms;
        private readonly IQueryHandler<GetMeshCustomerProfileQuery, GetMeshCustomerProfileModel> _GetMeshCustomerProfile;
        private readonly IQueryHandler<GetOrderChangeServiceQuery, GetOrderChangeServiceModel> _GetOrderChangeService;
        private readonly ICommandHandler<CreateOrderMeshPromotionCommand> _CreateOrderMeshPromotion;
        private readonly IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> _GetConfigReqPayment;
        private readonly IQueryHandler<CheckOrderPendingCreateQuery, CheckOrderPendingCreateModel> _queryCheckOrderCreateHandler;
        private readonly IQueryHandler<SMSFlagRegisterPendingQuery, SMSFlagRegisterPendingModel> _smsFlagHandler;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        public CreateOrderMeshHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, ICommandHandler<CustRegisterJobCommand> custRegJobCommand, ICommandHandler<SendSmsCommand> sendSms, IQueryHandler<GetMeshCustomerProfileQuery, GetMeshCustomerProfileModel> getMeshCustomerProfile, IQueryHandler<GetOrderChangeServiceQuery, GetOrderChangeServiceModel> getOrderChangeService, ICommandHandler<CreateOrderMeshPromotionCommand> createOrderMeshPromotion, IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> getConfigReqPayment, IQueryHandler<CheckOrderPendingCreateQuery, CheckOrderPendingCreateModel> queryCheckOrderCreateHandler, IQueryHandler<SMSFlagRegisterPendingQuery, SMSFlagRegisterPendingModel> smsFlagHandler, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _custRegJobCommand = custRegJobCommand;
            _SendSms = sendSms;
            _GetMeshCustomerProfile = getMeshCustomerProfile;
            _GetOrderChangeService = getOrderChangeService;
            _CreateOrderMeshPromotion = createOrderMeshPromotion;
            _GetConfigReqPayment = getConfigReqPayment;
            _queryCheckOrderCreateHandler = queryCheckOrderCreateHandler;
            _smsFlagHandler = smsFlagHandler;
            _lovService = lovService;
        }

        public CreateOrderMeshModel Handle(CreateOrderMeshQuery query)
        {
            InterfaceLogCommand log = null;
            var createOrderMeshModel = new CreateOrderMeshModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.InternetNo, "CreateOrderMesh", "CreateOrderMeshHandler", "", "FBB|" + query.FullUrl, "");

                //Check Order create ?
                if (CheckOrderCreated(query))
                {
                    createOrderMeshModel.RESULT_CODE = "0";
                    createOrderMeshModel.RESULT_DESC = "The order has been created.";

                    return createOrderMeshModel;
                }

                //1.Get data จาก Transaction ID (TXN_ID) 
                //var resSpDpLog = GetPaymentOldDataSuperDuper(query);
                //var item = data.GetORDPendingPaymentList.FirstOrDefault();

                var lovData = GetLov("FBBOR041");

                //TODO: get method payment table 
                //select p.* from fbb_cfg_payment p where p.product_name = 'MESH' and p.service_name = 'Generate QR' and p.attr_name = 'ref_5'
                //PROC_LIST_CONFIG_REQ_PAYMENT p_product_name = '',p_service_name = query.channel , p_transaction_id = query.txn_id
                //var paymentMethod = "147";
                var paymentMethod = GetConfigReqPayment(query);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                _logger.Info("Save For Mesh");
                _logger.Info("GetMeshCustomerProfile");
                _logger.Info("internet_no = " + query.InternetNo);
                _logger.Info("order_id = " + query.OrderId);

                GetMeshCustomerProfileModel informationDataModel = GetMeshCustomerProfile(query.InternetNo, query.OrderId);

                if (informationDataModel != null && informationDataModel.contact_mobile != null)
                {
                    _logger.Info("GetMeshCustomerProfile Have Data");

                    informationDataModel.tran_id = query.txn_id.ToSafeString();
                    string[] msgTxts = new string[3];
                    string[] msgErrorTxts = new string[2];
                    if (informationDataModel.language == "1")
                    {
                        msgTxts[0] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_SUCCESS_1")).LovValue1.ToSafeString();
                        msgTxts[1] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_SUCCESS_2")).LovValue1.ToSafeString();
                        msgTxts[2] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_SUCCESS_3")).LovValue1.ToSafeString();

                        msgErrorTxts[0] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_1")).LovValue1.ToSafeString();
                        msgErrorTxts[1] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_2")).LovValue1.ToSafeString();
                    }
                    else
                    {
                        msgTxts[0] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_SUCCESS_1")).LovValue2.ToSafeString();
                        msgTxts[1] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_SUCCESS_2")).LovValue2.ToSafeString();
                        msgTxts[2] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_SUCCESS_3")).LovValue2.ToSafeString();

                        msgErrorTxts[0] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_1")).LovValue2.ToSafeString();
                        msgErrorTxts[1] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_2")).LovValue2.ToSafeString();
                    }

                    int indexMsgTxt = 0;
                    foreach (var msgTxt in msgTxts)
                    {
                        string tmpmsgTxt = msgTxt;
                        tmpmsgTxt = tmpmsgTxt.Replace("[PurchaseAmt]", informationDataModel.purchase_amt);
                        tmpmsgTxt = tmpmsgTxt.Replace("[InstallDate]", informationDataModel.install_date);
                        tmpmsgTxt = tmpmsgTxt.Replace("[NonMobileNo]", informationDataModel.non_mobile_no);
                        tmpmsgTxt = tmpmsgTxt.Replace("[TranID]", informationDataModel.tran_id);
                        msgTxts[indexMsgTxt] = tmpmsgTxt;
                        indexMsgTxt++;
                    }

                    var delaySms = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("DELAY_SEND_MESH_SMS")).LovValue1.ToSafeInteger();

                    string ContactMobile = informationDataModel.contact_mobile;
                    ContactMobile = ContactMobile.Replace("-", "");
                    /// Getvalue for sent to sff
                    _logger.Info("GetOrderChangeService");
                    GetOrderChangeServiceModel DataForSentToSff = GetOrderChangeService(query.InternetNo, query.OrderId);
                    if (DataForSentToSff != null)
                    {
                        _logger.Info("GetOrderChangeService OK");
                        _logger.Info("CreateOrderMeshPromotion");

                        //DataForSentToSff txn_id
                        DataForSentToSff = SetDataTxnToSff(DataForSentToSff, query.txn_id.ToSafeString());

                        CreateOrderMeshPromotionResult result = null;
                        result = CreateOrderMeshPromotion(DataForSentToSff, query.InternetNo);
                        if (result != null && result.order_no.ToSafeString() != "")
                        {
                            _logger.Info("CreateOrderMeshPromotion OK");
                            string customerRowID = "";

                            /// Update CustRegister 

                            _logger.Info("RegisterCustomer sffOrder : " + result.order_no.ToSafeString());
                            _logger.Info("RegisterCustomer PaymentID : " + query.OrderId);
                            _logger.Info("RegisterCustomer Tran ID : " + query.txn_id.ToSafeString());
                            _logger.Info("RegisterCustomer PaymentMethod : " + paymentMethod);

                            customerRowID = RegisterCustomerMesh(result.order_no.ToSafeString(),
                                                                     query.OrderId.ToSafeString(),
                                                                     query.txn_id.ToSafeString(),
                                                                     paymentMethod,
                                                                     query.InternetNo);

                            /// Sent SMS
                            _logger.Info("MeshSendSMS");
                            if (CheckSMSFlag(query.OrderId, "Success", ContactMobile, query.FullUrl, query.InternetNo))
                            {
                                var resultsms = MeshSendSMS(ContactMobile, msgTxts, query.InternetNo, delaySms);
                                if (resultsms == "Success")
                                {
                                    UpdateSMSFlag(query.OrderId, "Success", ContactMobile, query.FullUrl, query.InternetNo);
                                }
                            }

                            createOrderMeshModel.RESULT_CODE = "0";
                            createOrderMeshModel.RESULT_DESC = "Success";
                        }
                        else
                        {

                            /// Sent SMS
                            _logger.Info("MeshSendErrorSMS");
                            if (CheckSMSFlag(query.OrderId, "Error", ContactMobile, query.FullUrl, query.InternetNo))
                            {
                                var resultsms = MeshSendSMS(ContactMobile, msgErrorTxts, query.InternetNo, delaySms);
                                if (resultsms == "Success")
                                {
                                    UpdateSMSFlag(query.OrderId, "Error", ContactMobile, query.FullUrl, query.InternetNo);
                                }
                            }

                            createOrderMeshModel.RESULT_CODE = "-1";
                            createOrderMeshModel.RESULT_DESC = "CreateOrderMeshPromotion faild";
                        }
                    }
                    else
                    {
                        /// Sent SMS
                        _logger.Info("MeshSendErrorSMS");
                        if (CheckSMSFlag(query.OrderId, "Error", ContactMobile, query.FullUrl, query.InternetNo))
                        {
                            var resultsms = MeshSendSMS(ContactMobile, msgErrorTxts, query.InternetNo, delaySms);
                            if (resultsms == "Success")
                            {
                                UpdateSMSFlag(query.OrderId, "Error", ContactMobile, query.FullUrl, query.InternetNo);
                            }
                        }

                        createOrderMeshModel.RESULT_CODE = "-1";
                        createOrderMeshModel.RESULT_DESC = "GetOrderChangeService faild";
                    }
                }
                else
                {
                    _logger.Info("Order Nodata or not paid");
                    createOrderMeshModel.RESULT_CODE = "-1";
                    createOrderMeshModel.RESULT_DESC = "Order Nodata or not paid";
                }
                _logger.Info("End Save For Mesh");

                return createOrderMeshModel;
            }
            catch (Exception ex)
            {
                createOrderMeshModel.RESULT_CODE = "-1";
                createOrderMeshModel.RESULT_DESC = ex.GetErrorMessage();
                _logger.Info("Error call CreateOrderMeshHandler : " + ex.GetErrorMessage());
                return createOrderMeshModel;
            }
            finally
            {
                var resultLog = (createOrderMeshModel ?? new CreateOrderMeshModel()).RESULT_CODE == "0" ? "Success" : "Failed";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, createOrderMeshModel, log, resultLog, createOrderMeshModel.RESULT_DESC, "");
            }
        }


        private bool CheckOrderCreated(CreateOrderMeshQuery query)
        {
            var checkOrderCreate = _queryCheckOrderCreateHandler.Handle(new CheckOrderPendingCreateQuery() { OrderId = query.OrderId, UpdateBy = query.FullUrl, InternetNo = query.InternetNo });
            if (checkOrderCreate != null && checkOrderCreate.OrderCreated == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private List<LovValueModel> GetLov(string lov_val5 = "")
        {
            List<FBB_CFG_LOV> loveList = null;
            if (!string.IsNullOrEmpty(lov_val5))
            {
                loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_VAL5 == lov_val5)
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

        public GetMeshCustomerProfileModel GetMeshCustomerProfile(string internetNo, string paymentOrderID)
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

        public GetOrderChangeServiceModel GetOrderChangeService(string internetNo, string paymentOrderID)
        {
            GetOrderChangeServiceQuery changeServiceQuery = new GetOrderChangeServiceQuery()
            {
                p_internet_no = internetNo,
                p_payment_order_id = paymentOrderID
            };

            GetOrderChangeServiceModel result = _GetOrderChangeService.Handle(changeServiceQuery);
            return result;
        }

        public CreateOrderMeshPromotionResult CreateOrderMeshPromotion(GetOrderChangeServiceModel data, string NonMobileNo)
        {
            CreateOrderMeshPromotionResult result = new CreateOrderMeshPromotionResult();
            string FullUrl = "";

            // Get IP Address
            string ipAddress = "";

            CreateOrderMeshPromotionCommand createOrderMeshPromotionCommand = new CreateOrderMeshPromotionCommand()
            {
                GetOrderChangeService = data,
                NonMobileNo = NonMobileNo,
                FullUrl = FullUrl,
                client_ip = ipAddress
            };

            _CreateOrderMeshPromotion.Handle(createOrderMeshPromotionCommand);

            if (createOrderMeshPromotionCommand.ERROR_MSG == "")
            {
                result.ret_code = createOrderMeshPromotionCommand.VALIDATE_FLAG.ToSafeString();
                result.ret_message = createOrderMeshPromotionCommand.ERROR_MSG.ToSafeString();
                result.order_no = createOrderMeshPromotionCommand.sffOrderNo.ToSafeString();
            }

            return result;
        }

        public string RegisterCustomerMesh(string SffOrder, string oderID, string transactionID, string paymentMethod, string internetNo)
        {
            CustRegisterJobCommand command = new CustRegisterJobCommand()
            {
                RETURN_IA_NO = SffOrder,
                RETURN_ORDER_NO = SffOrder,
                TRANSACTIONID_IN = oderID,
                TRANSACTIONID = transactionID,
                PAYMENTMETHOD = paymentMethod,
                PLUG_AND_PLAY_FLAG = "",
                REGISTER_TYPE = "TOPUP_MESH",
                ClientIP = "",
                FullUrl = "",
                INTERNET_NO = internetNo
            };
            _custRegJobCommand.Handle(command);

            return command.CUSTOMERID;
        }

        private Object MeshthisLock = new Object();

        public string MeshSendSMS(string mobileNo, string[] msgtxts, string internetNo, int delaySms)
        {
            var resultsms = string.Empty;
            var tempMobileNo = mobileNo;
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
                    command.Transaction_Id = tempMobileNo;

                    command.Message_Text = item;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _SendSms.Handle(command);

                        if (delaySms > 0)
                        {
                            Thread.Sleep(new TimeSpan(0, 0, delaySms));
                        }
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

        private string GetConfigReqPayment(CreateOrderMeshQuery query)
        {
            var payment_method_id = "";

            GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
            {
                p_product_name = "",
                p_service_name = query.Channel,
                p_transaction_id = query.txn_id,
                p_non_mobile_no = query.InternetNo
            };

            GetConfigReqPaymentModel getConfigReqPaymentData = _GetConfigReqPayment.Handle(getConfigReqPaymentQuery);

            if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
            {
                List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;

                payment_method_id = (configReqPaymentDatas.FirstOrDefault(item => item.attr_name == "payment_method_id") ?? new ConfigReqPaymentData()).attr_value;
            }

            return payment_method_id;
        }

        private bool CheckSMSFlag(string orderId, string status, string ContactMobile, string fullUrl, string internetNo)
        {
            var result = _smsFlagHandler.Handle(new SMSFlagRegisterPendingQuery()
            {
                Action = "Check",
                OrderId = orderId,
                Status = status,
                Mobile_No = ContactMobile,
                FullUrl = fullUrl,
                InternetNo = internetNo
            });

            if (result != null && result.SendSMS_Flag != "Y")
            {
                return true;
            }

            return false;
        }

        private void UpdateSMSFlag(string orderId, string status, string ContactMobile, string fullUrl, string internetNo)
        {
            _smsFlagHandler.Handle(new SMSFlagRegisterPendingQuery()
            {
                Action = "Update",
                OrderId = orderId,
                Status = status,
                SMS_Flag = "Y",
                Mobile_No = ContactMobile,
                FullUrl = fullUrl,
                InternetNo = internetNo
            });
        }

        private GetOrderChangeServiceModel SetDataTxnToSff(GetOrderChangeServiceModel DataForSentToSff, string txn_id)
        {
            if (DataForSentToSff != null && DataForSentToSff.OrdServiceAttributeList != null)
            {
                foreach (var item in from item in DataForSentToSff.OrdServiceAttributeList
                                     where item.serviceAttributeName == "transactionId"
                                     select item)
                {
                    item.serviceAttributeValue = txn_id.ToSafeString();
                }
            }
            return DataForSentToSff;
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