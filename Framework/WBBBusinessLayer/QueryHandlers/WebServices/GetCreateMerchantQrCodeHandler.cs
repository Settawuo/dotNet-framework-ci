using Inetlab.SMPP;
using Inetlab.SMPP.Common;
using Inetlab.SMPP.PDU;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetCreateMerchantQrCodeHandler : IQueryHandler<GetCreateMerchantQrCodeQuery, GetCreateMerchantQrCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        //private readonly IEntityRepository<FBB_CFG_LOV> _fbbCfgLov;

        public GetCreateMerchantQrCodeHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog
            //,IEntityRepository<FBB_CFG_LOV> fbbCfgLov
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            //_fbbCfgLov = fbbCfgLov;
        }

        public GetCreateMerchantQrCodeModel Handle(GetCreateMerchantQrCodeQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new GetCreateMerchantQrCodeModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Body.orderId,
                    "GetCreateMerchantQrCodeApi", "GetCreateMerchantQrCodeHandler", "", "FBB", "");

                var client = new RestClient(query.Url); //"https://chillchill.ais.co.th:8002"
                // client.Authenticator = new HttpBasicAuthenticator(username, password);
                var request = new RestRequest(query.Method, Method.POST) //"/mpay-unified-qr-code/qr"
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new RestSharpJsonSerializer()

                };
                request.AddHeader("appId", query.AppId);
                request.AddHeader("appSecret", query.AppSecret);
                request.AddBody(query.Body);

                // execute the request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);
                var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<GetCreateMerchantQrCodeModel>(response.Content) ?? new GetCreateMerchantQrCodeModel();

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                        result.RespCode == "0000" ? "Success" : "Failed", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", response.ErrorMessage, "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

    }

    public class GetValueQrCodeHandler : IQueryHandler<GetValueQrCodeQuery, GetValueQrCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _obj;

        public GetValueQrCodeHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> obj
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _obj = obj;
        }

        public GetValueQrCodeModel Handle(GetValueQrCodeQuery query)
        {
            InterfaceLogCommand log = null;
            GetValueQrCodeModel result = new GetValueQrCodeModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.InternetNo,
                    "GetValueQrCode", "GetValueQrCodeHandler", "", "FBB", "");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var ret_payment_order_id = new OracleParameter();
                ret_payment_order_id.ParameterName = "ret_payment_order_id";
                ret_payment_order_id.Size = 2000;
                ret_payment_order_id.OracleDbType = OracleDbType.Varchar2;
                ret_payment_order_id.Direction = ParameterDirection.Output;

                var ret_purchase_amt = new OracleParameter();
                ret_purchase_amt.ParameterName = "ret_purchase_amt";
                ret_purchase_amt.Size = 2000;
                ret_purchase_amt.OracleDbType = OracleDbType.Varchar2;
                ret_purchase_amt.Direction = ParameterDirection.Output;

                var executeResult = _obj.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_GET_VALUE_QR_CODE",
                            new
                            {
                                p_internet_no = query.InternetNo,
                                /// return //////
                                ret_code = ret_code,
                                ret_message = ret_message,
                                ret_payment_order_id = ret_payment_order_id,
                                ret_purchase_amt = ret_purchase_amt
                            });

                var Return_Code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                var Return_Message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "error";

                if (Return_Code != "-1")
                {
                    result = new GetValueQrCodeModel
                    {
                        PaymentOrderID = ret_payment_order_id.Value != null ? ret_payment_order_id.Value.ToSafeString() : "",
                        PurchaseAmt = ret_purchase_amt.Value != null ? ret_purchase_amt.Value.ToSafeString() : "",
                    };

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log,
                        result.RespCode == "0000" ? "Success" : "Failed", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", Return_Message, "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

    }

    public class GetOrderChangeServiceHandler : IQueryHandler<GetOrderChangeServiceQuery, GetOrderChangeServiceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _obj;

        public GetOrderChangeServiceHandler(ILogger logger,
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

        public GetOrderChangeServiceModel Handle(GetOrderChangeServiceQuery query)
        {
            InterfaceLogCommand log = null;
            GetOrderChangeServiceModel result = new GetOrderChangeServiceModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_internet_no,
                    "GetOrderChangeService", "GetOrderChangeServiceHandler", "", "FBB", "");

                var p_internet_no = new OracleParameter();
                p_internet_no.ParameterName = "p_internet_no";
                p_internet_no.OracleDbType = OracleDbType.Varchar2;
                p_internet_no.Size = 2000;
                p_internet_no.Direction = ParameterDirection.Input;
                p_internet_no.Value = query.p_internet_no;

                var p_payment_order_id = new OracleParameter();
                p_payment_order_id.ParameterName = "p_payment_order_id";
                p_payment_order_id.OracleDbType = OracleDbType.Varchar2;
                p_payment_order_id.Size = 2000;
                p_payment_order_id.Direction = ParameterDirection.Input;
                p_payment_order_id.Value = query.p_payment_order_id;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var list_ord_change_service = new OracleParameter();
                list_ord_change_service.ParameterName = "list_ord_change_service";
                list_ord_change_service.OracleDbType = OracleDbType.RefCursor;
                list_ord_change_service.Direction = ParameterDirection.Output;

                var list_ord_service_attribute = new OracleParameter();
                list_ord_service_attribute.ParameterName = "list_ord_service_attribute";
                list_ord_service_attribute.OracleDbType = OracleDbType.RefCursor;
                list_ord_service_attribute.Direction = ParameterDirection.Output;

                var list_ord_fee = new OracleParameter();
                list_ord_fee.ParameterName = "list_ord_fee";
                list_ord_fee.OracleDbType = OracleDbType.RefCursor;
                list_ord_fee.Direction = ParameterDirection.Output;

                var resultExecute = _obj.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.CREATE_ORDER_CHANGE_SERVICE",
                      new object[]
                      {
                          //Parameter Input
                          p_internet_no,
                          p_payment_order_id,
                          //Parameter Output
                          ret_code,
                          ret_message,
                          list_ord_change_service,
                          list_ord_service_attribute,
                          list_ord_fee
                      });

                if (resultExecute != null)
                {
                    //result.RespCode = ret_code != null ? ret_code.Value.ToString() : "-1";
                    //result.RespDesc = ret_message != null ? ret_message.Value.ToString() : "";

                    result.RespCode = resultExecute[0] != null ? resultExecute[0].ToString() : "-1";
                    result.RespDesc = resultExecute[1] != null ? resultExecute[1].ToString() : "";

                    DataTable dtOrdChangeServiceRespones = (DataTable)resultExecute[2];
                    List<OrdChangeService> OrdChangeServiceList = dtOrdChangeServiceRespones.DataTableToList<OrdChangeService>();
                    result.OrdChangeServiceList = OrdChangeServiceList;

                    DataTable dtOrdServiceAttributeRespones = (DataTable)resultExecute[3];
                    List<OrdServiceAttribute> OrdServiceAttributeList = dtOrdServiceAttributeRespones.DataTableToList<OrdServiceAttribute>();
                    result.OrdServiceAttributeList = OrdServiceAttributeList;

                    DataTable dtOrdFeeRespones = (DataTable)resultExecute[4];
                    List<OrdFee> OrdFeeList = dtOrdFeeRespones.DataTableToList<OrdFee>();
                    result.OrdFeeList = OrdFeeList;

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultExecute, log, "Success", "", "");
                }
                else
                {
                    result.RespCode = "-1";
                    result.RespDesc = "resultExecute is null";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", "resultExecute is null", "");
                }

            }
            catch (Exception ex)
            {
                result.RespCode = "-1";
                result.RespDesc = ex.Message.ToSafeString();
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

    }

    public class GetMeshCustomerProfileHandler : IQueryHandler<GetMeshCustomerProfileQuery, GetMeshCustomerProfileModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _obj;

        public GetMeshCustomerProfileHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> obj
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _obj = obj;
        }

        public GetMeshCustomerProfileModel Handle(GetMeshCustomerProfileQuery query)
        {
            InterfaceLogCommand log = null;
            GetMeshCustomerProfileModel result = new GetMeshCustomerProfileModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_internet_no,
                    "GetMeshCustomerProfile", "GetMeshCustomerProfileHandler", "", "FBB", "");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var o_order_list = new OracleParameter();
                o_order_list.ParameterName = "o_order_list";
                o_order_list.Size = 2000;
                o_order_list.OracleDbType = OracleDbType.Varchar2;
                o_order_list.Direction = ParameterDirection.Output;

                var o_amount = new OracleParameter();
                o_amount.ParameterName = "o_amount";
                o_amount.Size = 2000;
                o_amount.OracleDbType = OracleDbType.Varchar2;
                o_amount.Direction = ParameterDirection.Output;

                var o_purchase_amt = new OracleParameter();
                o_purchase_amt.ParameterName = "o_purchase_amt";
                o_purchase_amt.Size = 2000;
                o_purchase_amt.OracleDbType = OracleDbType.Varchar2;
                o_purchase_amt.Direction = ParameterDirection.Output;

                var o_tran_id = new OracleParameter();
                o_tran_id.ParameterName = "o_tran_id";
                o_tran_id.Size = 2000;
                o_tran_id.OracleDbType = OracleDbType.Varchar2;
                o_tran_id.Direction = ParameterDirection.Output;

                var o_order_date = new OracleParameter();
                o_order_date.ParameterName = "o_order_date";
                o_order_date.Size = 2000;
                o_order_date.OracleDbType = OracleDbType.Varchar2;
                o_order_date.Direction = ParameterDirection.Output;

                var o_customer_name = new OracleParameter();
                o_customer_name.ParameterName = "o_customer_name";
                o_customer_name.Size = 2000;
                o_customer_name.OracleDbType = OracleDbType.Varchar2;
                o_customer_name.Direction = ParameterDirection.Output;

                var o_non_mobile_no = new OracleParameter();
                o_non_mobile_no.ParameterName = "o_non_mobile_no";
                o_non_mobile_no.Size = 2000;
                o_non_mobile_no.OracleDbType = OracleDbType.Varchar2;
                o_non_mobile_no.Direction = ParameterDirection.Output;

                var o_contact_mobile = new OracleParameter();
                o_contact_mobile.ParameterName = "o_contact_mobile";
                o_contact_mobile.Size = 2000;
                o_contact_mobile.OracleDbType = OracleDbType.Varchar2;
                o_contact_mobile.Direction = ParameterDirection.Output;

                var o_install_date = new OracleParameter();
                o_install_date.ParameterName = "o_install_date";
                o_install_date.Size = 2000;
                o_install_date.OracleDbType = OracleDbType.Varchar2;
                o_install_date.Direction = ParameterDirection.Output;

                var o_language = new OracleParameter();
                o_language.ParameterName = "o_language";
                o_language.Size = 2000;
                o_language.OracleDbType = OracleDbType.Varchar2;
                o_language.Direction = ParameterDirection.Output;

                var o_sff_promotion_code = new OracleParameter();
                o_sff_promotion_code.ParameterName = "o_sff_promotion_code";
                o_sff_promotion_code.Size = 2000;
                o_sff_promotion_code.OracleDbType = OracleDbType.Varchar2;
                o_sff_promotion_code.Direction = ParameterDirection.Output;

                //R20.10 ref1 = ba
                var o_ba = new OracleParameter();
                o_ba.ParameterName = "o_sff_promotion_code";
                o_ba.Size = 2000;
                o_ba.OracleDbType = OracleDbType.Varchar2;
                o_ba.Direction = ParameterDirection.Output;

                var executeResult = _obj.ExecuteStoredProc("WBB.PKG_FBBOR041.QUERY_MESH_CUSTOMER_PROFILE",
                            new
                            {
                                p_internet_no = query.p_internet_no,
                                p_payment_order_id = query.p_payment_order_id,
                                /// return //////
                                ret_code = ret_code,
                                ret_message = ret_message,
                                o_order_list = o_order_list,
                                o_amount = o_amount,
                                o_purchase_amt = o_purchase_amt,
                                o_tran_id = o_tran_id,
                                o_order_date = o_order_date,
                                o_customer_name = o_customer_name,
                                o_non_mobile_no = o_non_mobile_no,
                                o_contact_mobile = o_contact_mobile,
                                o_install_date = o_install_date,
                                o_language = o_language,
                                o_sff_promotion_code = o_sff_promotion_code,
                                o_ba = o_ba
                            });

                var Return_Code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                var Return_Message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "error";

                if (Return_Code != "-1")
                {
                    result = new GetMeshCustomerProfileModel
                    {
                        order_list = o_order_list.Value != null ? o_order_list.Value.ToSafeString() : "",
                        amount = o_amount.Value != null ? o_amount.Value.ToSafeString() : "",
                        purchase_amt = o_purchase_amt.Value != null ? o_purchase_amt.Value.ToSafeString() : "",
                        tran_id = o_tran_id.Value != null ? o_tran_id.Value.ToSafeString() : "",
                        order_date = o_order_date.Value != null ? o_order_date.Value.ToSafeString() : "",
                        customer_name = o_customer_name.Value != null ? o_customer_name.Value.ToSafeString() : "",
                        non_mobile_no = o_non_mobile_no.Value != null ? o_non_mobile_no.Value.ToSafeString() : "",
                        contact_mobile = o_contact_mobile.Value != null ? o_contact_mobile.Value.ToSafeString() : "",
                        install_date = o_install_date.Value != null ? o_install_date.Value.ToSafeString() : "",
                        language = o_language.Value != null ? o_language.Value.ToSafeString() : "",
                        sff_promotion_code = o_sff_promotion_code.Value != null ? o_sff_promotion_code.Value.ToSafeString() : "",
                        ba = o_ba.Value != null ? o_ba.Value.ToSafeString() : ""
                    };
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", Return_Message, "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

    }

    public class MeshSmsHandler : IQueryHandler<MeshSmsQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

        public MeshSmsHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
        {
            _logger = logger;
            _lov = lov;
            _uow = uow;

            _interfaceLog = interfaceLog;
        }

        public string Handle(MeshSmsQuery query)
        {
            string result = "N";
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, query, query.Transaction_Id, "MeshSmsCommand", "MeshSms", null, "FBB|" + query.FullUrl, "SMS");

            try
            {
                var SmsConfig = _lov.Get(l => l.LOV_TYPE.Equals("FBB_CONSTANT") && l.LOV_NAME.Equals("SMPP_CONFIGURATION"));
                string Host = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("HOST")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("HOST")).FirstOrDefault().LOV_VAL1;
                string Port = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PORT")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PORT")).FirstOrDefault().LOV_VAL1;
                string System_id = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMID")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMID")).FirstOrDefault().LOV_VAL1;
                string Password = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PASSWORD")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PASSWORD")).FirstOrDefault().LOV_VAL1;
                string System_type = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMTYPE")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMTYPE")).FirstOrDefault().LOV_VAL1;
                string Addr_Ton = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRNPI")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRNPI")).FirstOrDefault().LOV_VAL1;
                string Addr_Npi = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRTON")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRTON")).FirstOrDefault().LOV_VAL1;

                string[] msgtxts = null;

                //chist699 20/02/2024 senderSMS
                if (query.Source_Addr.ToUpper() == "AISFIBRE")
                {
                    var GetsenderName = _lov.Get(l => l.LOV_TYPE.Equals("CONFIG") && l.LOV_NAME.Equals("SENDER_SMS")).FirstOrDefault().LOV_VAL1;
                    string senderName = !string.IsNullOrEmpty(GetsenderName) ? GetsenderName : "AISFIBRE3";
                    query.Source_Addr = senderName;
                }

                if (query.mobileNo.Substring(0, 2) != "66")
                {
                    if (query.mobileNo.Substring(0, 1) == "0")
                    {
                        query.mobileNo = "66" + query.mobileNo.Substring(1);
                    }
                }

                if (query.MsgWay == "1")
                {
                    msgtxts = new string[3];

                    var data1 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_SUCCESS_1") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();
                    var data2 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_SUCCESS_2") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();
                    var data3 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_SUCCESS_3") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();

                    if (data1 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[0] = data1.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[0] = data1.LOV_VAL2;
                        }
                        msgtxts[0] = msgtxts[0].Replace("[PurchaseAmt]", query.PurchaseAmt);
                        msgtxts[0] = msgtxts[0].Replace("[InstallDate]", query.InstallDate);
                        msgtxts[0] = msgtxts[0].Replace("[NonMobileNo]", query.NonMobileNo);
                        msgtxts[0] = msgtxts[0].Replace("[TranID]", query.TranID);

                    }
                    if (data2 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[1] = data2.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[1] = data2.LOV_VAL2;
                        }
                        msgtxts[1] = msgtxts[1].Replace("[PurchaseAmt]", query.PurchaseAmt);
                        msgtxts[1] = msgtxts[1].Replace("[InstallDate]", query.InstallDate);
                        msgtxts[1] = msgtxts[1].Replace("[NonMobileNo]", query.NonMobileNo);
                        msgtxts[1] = msgtxts[1].Replace("[TranID]", query.TranID);
                    }
                    if (data3 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[2] = data3.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[2] = data3.LOV_VAL2;
                        }
                        msgtxts[2] = msgtxts[2].Replace("[PurchaseAmt]", query.PurchaseAmt);
                        msgtxts[2] = msgtxts[2].Replace("[InstallDate]", query.InstallDate);
                        msgtxts[2] = msgtxts[2].Replace("[NonMobileNo]", query.NonMobileNo);
                        msgtxts[2] = msgtxts[2].Replace("[TranID]", query.TranID);
                    }

                }
                else if (query.MsgWay == "2")
                {
                    msgtxts = new string[2];
                    var data1 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_ERROR_1") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();
                    var data2 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_ERROR_2") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();

                    if (data1 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[0] = data1.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[0] = data1.LOV_VAL2;
                        }
                    }
                    if (data2 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[1] = data2.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[1] = data2.LOV_VAL2;
                        }
                    }

                }
                else if (query.MsgWay == "3")
                {

                    string UrlForSentSMS = query.UrlForSentSMS;

                    msgtxts = new string[3];
                    var data1 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_PAYMENT_1") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();
                    var data2 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_PAYMENT_2") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();
                    var data3 = _lov
                        .Get(l => !string.IsNullOrEmpty(l.LOV_VAL5) && !string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_VAL5.Equals("FBBOR041") && l.LOV_NAME.Equals("SMS_MESSAGE_PAYMENT_3") && l.ACTIVEFLAG == "Y")
                        .FirstOrDefault();

                    if (data1 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[0] = data1.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[0] = data1.LOV_VAL2;
                        }
                        msgtxts[0] = msgtxts[0].Replace("[PaymentURL]", query.UrlForSentSMS);
                    }
                    if (data2 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[1] = data2.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[1] = data2.LOV_VAL2;
                        }
                        msgtxts[1] = msgtxts[1].Replace("[PaymentURL]", query.UrlForSentSMS);
                    }
                    if (data3 != null)
                    {
                        if (query.IsThaiCulture)
                        {
                            msgtxts[2] = data3.LOV_VAL1;
                        }
                        else
                        {
                            msgtxts[2] = data3.LOV_VAL2;
                        }
                        msgtxts[2] = msgtxts[2].Replace("[PaymentURL]", query.UrlForSentSMS);
                    }


                }
                string msgForSendSMS = "";
                foreach (var itemSMS in msgtxts)
                {
                    msgForSendSMS += itemSMS;
                }
                if (msgForSendSMS != "")
                {
                    using (SmppClient client = new SmppClient())
                    {
                        //client.AddrNpi = Convert.ToByte(Addr_Npi);
                        //client.AddrTon = Convert.ToByte(Addr_Ton);
                        //client.SystemType = System_type;
                        if (client.Connect(Host, int.Parse(Port)))
                        {
                            BindResp bindResp = client.Bind(System_id, Password);

                            if (bindResp.Status == CommandStatus.ESME_ROK)
                            {
                                var submitResp = client.Submit(
                                    SMS.ForSubmit()
                                        .From(query.Source_Addr)
                                        .To(query.mobileNo)
                                        .Coding(DataCodings.UCS2)
                                        .Text(msgForSendSMS));
                                if (submitResp.All(x => x.Status == CommandStatus.ESME_ROK))
                                {
                                    //Success case
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, query, log, "Success", "", "SMS");
                                    result = "Y";
                                }
                                else
                                {
                                    //Fail case
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, query, log, "Failed", "Send Failed", "SMS");
                                }
                            }

                            client.Disconnect();
                        }
                        else
                        {
                            //Fail case
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, query, log, "Failed", "Connection Status is not open", "SMS");
                        }
                    }
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, query, log, "Failed", "MSG is null", "SMS");
                }
            }
            catch (Exception ex)
            {
                query.return_status = "";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, "", log, "Failed", ex.Message, "SMS");
            }

            return result;
        }

        public bool isThai(String msg)
        {
            if (msg != null)
            {
                for (int i = 0; i < msg.Length; i++)
                {
                    char code = msg[i];
                    if ((161 <= code) && (code <= 251) || (3585 <= code)
                            && (code <= 3675))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}