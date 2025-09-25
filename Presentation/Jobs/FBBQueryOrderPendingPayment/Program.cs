using FBBQueryOrderPendingPayment.CompositionRoot;
using FBBQueryOrderPendingPayment.Extension;
using FBBQueryOrderPendingPayment.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBQueryOrderPendingPayment
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBQueryOrderPendingPaymentJob>();
            job.LogMsg("FBBQueryOrderPendingPayment Start");
            job.StartWatching();
            //job.UpdatOrderPaymentStatus("N"); /// ForDev xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            job.LogMsg("CheckOrderPaymentStatus");



            SavePaymentLogModel savePaymentLogModel = null;

            if (!job.CheckOrderPaymentStatus())
            {
                try
                {
                    var lovData_Flag = job.GetLov("FBB_CONSTANT", "CALL_SUPER_DUPER_FLAG");
                    var resultSuperduperFlag = lovData_Flag.FirstOrDefault() ?? new LovValueModel();
                    var CALL_SUPER_DUPER_FLAG = resultSuperduperFlag.LovValue1;

                    job.LogMsg("CheckOrderPaymentStatus : OK");
                    job.UpdatOrderPaymentStatus("Y");
                    job.LogMsg("GetORDPendingPayment");

                    if (CALL_SUPER_DUPER_FLAG == "Y")
                    {
                        // TODO: New version
                        var result = job.CreateOrderPendingPayment();
                    }
                    else
                    {
                        #region Flow old

                        var lovData = job.GetLov();
                        var ImpersonateVar = lovData.Where(o => !string.IsNullOrEmpty(o.Type) && !string.IsNullOrEmpty(o.Name) && o.Type.Equals("FBB_CONSTANT") && o.Name.Equals("Impersonate_App")).FirstOrDefault();
                        string user = ImpersonateVar.LovValue1;
                        string pass = ImpersonateVar.LovValue2;
                        string ip = ImpersonateVar.LovValue3;
                        string Impersonate = ImpersonateVar.LovValue4;
                        var MsgSentData = lovData.Where(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR028") && l.Name.Equals("SMS_MESSAGE")).FirstOrDefault();
                        string MsgSentTH = MsgSentData.LovValue1;
                        string MsgSentEN = MsgSentData.LovValue2;

                        var MsgSentFailData = lovData.Where(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR028") && l.Name.Equals("SMS_MESSAGE_ERROR")).FirstOrDefault();
                        string MsgSentFailTH = MsgSentFailData.LovValue1;
                        string MsgSentFailEN = MsgSentFailData.LovValue2;

                        var resultQrcode = lovData.Where(l => !string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals("FBBOR028") && !string.IsNullOrEmpty(l.Name) && l.Name.Equals("RequestOrderQrcodeApi") && !string.IsNullOrEmpty(l.Text) && l.Text.Equals("ref5")).FirstOrDefault();
                        string paymentmethodQRCode = resultQrcode != null ? resultQrcode.LovValue1 : "147";


                        GetORDPendingPaymentModel data = new GetORDPendingPaymentModel()
                        {
                            GetORDPendingPaymentList = new List<GetORDPendingPayment>()
                        };


                        GetORDPendingPaymentModel dataQr = job.GetORDPendingPayment("147");
                        if (dataQr != null && dataQr.GetORDPendingPaymentList != null && dataQr.GetORDPendingPaymentList.Count > 0)
                        {
                            foreach (var itemQr in dataQr.GetORDPendingPaymentList)
                            {
                                data.GetORDPendingPaymentList.Add(itemQr);
                            }
                        }
                        GetORDPendingPaymentModel dataCreditDebit = job.GetORDPendingPayment("4");
                        if (dataCreditDebit != null && dataCreditDebit.GetORDPendingPaymentList != null && dataCreditDebit.GetORDPendingPaymentList.Count > 0)
                        {
                            foreach (var itemCreditDebit in dataCreditDebit.GetORDPendingPaymentList)
                            {
                                data.GetORDPendingPaymentList.Add(itemCreditDebit);
                            }
                        }



                        if (data != null && data.GetORDPendingPaymentList != null && data.GetORDPendingPaymentList.Count > 0)
                        {
                            var listLogSuccess = new List<paymentLog>();
                            foreach (var item in data.GetORDPendingPaymentList)
                            {
                                string errorMsg = "";
                                bool checkSaveJob = false;
                                job.LogMsg("SavePaymentLog order_id :" + item.order_id);
                                savePaymentLogModel = new SavePaymentLogModel()
                                {
                                    ACTION = "New",
                                    PROCESS_NAME = GetProcessName(item.command).Result,
                                    PAYMENT_ORDER_ID = item.order_id,
                                    ENDPOINT = item.url,
                                    REQ_PROJECT_CODE = item.project_code,
                                    REQ_COMMAND = item.command,
                                    REQ_MERCHANT_ID = item.merchant_id,
                                    REQ_ORDER_ID = item.order_id,
                                    REQ_PURCHASE_AMT = item.purchase_amt,
                                    REQ_TRAN_ID = item.order_id,

                                };
                                job.SavePaymentLog(savePaymentLogModel);
                                job.LogMsg("CheckPayment order_id :" + item.order_id);

                                paymentLog results = new paymentLog();
                                var successOrder = listLogSuccess.FirstOrDefault(x => x.PAYMENT_ORDER_ID == item.order_id && x.RESP_DETAIL1 == "SUCCESS");
                                if (successOrder == null)
                                {
                                    results = item.command == "qr-code-payments-inquiry"
                                        ? CheckPaymentQrCode(item).Result
                                        : CheckPayment(item).Result;

                                    job.LogMsg("UpdatePaymentLog order_id :" + item.order_id);
                                    if (results != null && results.RESP_DETAIL1 == "SUCCESS")
                                    {

                                        job.LogMsg("CheckPayment order_id :" + item.order_id + " IsOK");
                                        checkSaveJob = true;

                                        listLogSuccess.Add(results);

                                        savePaymentLogModel = new SavePaymentLogModel()
                                        {
                                            ACTION = "Modify",
                                            PROCESS_NAME = GetProcessName(item.command).Result,
                                            PAYMENT_ORDER_ID = results.PAYMENT_ORDER_ID,

                                            RESP_STATUS = results.RESP_STATUS,
                                            RESP_RESP_CODE = results.RESP_RESP_CODE,
                                            RESP_RESP_DESC = results.RESP_RESP_DESC,
                                            RESP_SALE_ID = results.RESP_SALE_ID,
                                            RESP_DETAIL1 = results.RESP_DETAIL1,

                                            //18.10 : QR Code
                                            RESP_QR_CODE_STR = results.RESP_QR_CODE_STR,
                                            RESP_TRAN_DTM = results.RESP_TRAN_DTM,
                                            RESP_TRAN_ID = results.RESP_TRAN_ID,
                                            RESP_SERVICE_ID = results.RESP_SERVICE_ID,
                                            RESP_TERMINAL_ID = results.RESP_TERMINAL_ID,
                                            RESP_LOCATION_NAME = results.RESP_LOCATION_NAME,
                                            RESP_AMOUNT = results.RESP_AMOUNT,
                                            RESP_SOF = results.RESP_SOF,
                                            RESP_QR_TYPE = results.RESP_QR_TYPE,
                                            RESP_REFUND_DT = results.RESP_REFUND_DT,
                                            RESP_DISPUTE_ID = results.RESP_DISPUTE_ID,
                                            RESP_DISPUT_STATUS = results.RESP_DISPUT_STATUS,
                                            RESP_DISPUT_REASON_ID = results.RESP_DISPUT_REASON_ID,
                                            RESP_REF1 = results.RESP_REF1,
                                            RESP_REF2 = results.RESP_REF2,
                                            RESP_REF3 = results.RESP_REF3,
                                            RESP_REF4 = results.RESP_REF4,
                                            RESP_REF5 = results.RESP_REF5,
                                        };
                                        job.SavePaymentLog(savePaymentLogModel);

                                    }
                                    else
                                    {
                                        job.LogMsg("CheckPayment order_id :" + item.order_id + " error");
                                        checkSaveJob = false;
                                        errorMsg = "error CheckPayment";

                                        results = results ?? new paymentLog();

                                        var updatePaymentLogModel = new SavePaymentLogModel()
                                        {
                                            ACTION = "Modify",
                                            PROCESS_NAME = GetProcessName(item.command).Result,
                                            PAYMENT_ORDER_ID = item.order_id,

                                            RESP_STATUS = results.RESP_STATUS,
                                            RESP_RESP_CODE = results.RESP_RESP_CODE,
                                            RESP_RESP_DESC = results.RESP_RESP_DESC,
                                            RESP_SALE_ID = "",
                                            RESP_DETAIL1 = "FAILED_DUP_NODATA"
                                        };

                                        job.SavePaymentLog(updatePaymentLogModel);

                                        continue;
                                    }
                                }
                                else
                                {
                                    //case duplicate order and fail

                                    var updatePaymentLogModel = new SavePaymentLogModel()
                                    {
                                        ACTION = "Modify",
                                        PROCESS_NAME = GetProcessName(item.command).Result,
                                        PAYMENT_ORDER_ID = item.order_id,

                                        RESP_STATUS = "",
                                        RESP_RESP_CODE = "",
                                        RESP_RESP_DESC = results.RESP_RESP_DESC,
                                        RESP_SALE_ID = "",
                                        RESP_DETAIL1 = "FAILED_DUP_NODATA"
                                    };

                                    job.SavePaymentLog(updatePaymentLogModel);

                                    //results = successOrder;

                                    job.LogMsg("CheckPayment order_id :" + item.order_id + " error");
                                    checkSaveJob = false;
                                    errorMsg = "error CheckPayment";
                                    continue;
                                }

                                //ก่อน Create Order check table fbb_register_pending_payment return_order != null

                                if (item.order_type.ToSafeString() == "SELL_ROUTER")
                                {
                                    /// Save For SCPE
                                    job.LogMsg("Save For SCPE");

                                    GetListORDDetailCreateModel dataORDDetailCreate = new GetListORDDetailCreateModel();
                                    job.LogMsg("GetListORDDetailCreate order_id :" + item.order_id);
                                    dataORDDetailCreate = job.GetListORDDetailCreate(item.order_id);
                                    if (dataORDDetailCreate != null && checkSaveJob)
                                    {
                                        string eng_flag = dataORDDetailCreate.ODRDetailCustomerList[0].eng_flag;
                                        bool isThai = true;
                                        if (eng_flag == "Y")
                                            isThai = false;

                                        /// Fix paymentmethod if not return CheckPayment
                                        dataORDDetailCreate.ODRDetailCustomerList[0].paymentmethod = item.command == "qr-code-payments-inquiry" ? paymentmethodQRCode : "4";
                                        dataORDDetailCreate.ODRDetailCustomerList[0].transactionid = results.RESP_TRAN_ID;
                                        job.LogMsg("GetListORDDetailCreate order_id :" + item.order_id + " IsOK");
                                        /// SaveOrder

                                        SaveOrderResp saveOrderResp = new SaveOrderResp();
                                        job.LogMsg("GetSaveOrderResp order_id :" + item.order_id);
                                        saveOrderResp = job.GetSaveOrderResp(dataORDDetailCreate);

                                        if (saveOrderResp != null && saveOrderResp.RETURN_ORDER_NO != null && saveOrderResp.RETURN_ORDER_NO != "")
                                        {
                                            dataORDDetailCreate.return_ia_no = saveOrderResp.RETURN_IA_NO;
                                            dataORDDetailCreate.return_order_no = saveOrderResp.RETURN_ORDER_NO;
                                            job.LogMsg("GetSaveOrderResp order_id :" + item.order_id + " IsOK");
                                            checkSaveJob = true;
                                        }
                                        else
                                        {
                                            dataORDDetailCreate.return_ia_no = "";
                                            dataORDDetailCreate.return_order_no = "";
                                            job.LogMsg("GetSaveOrderResp order_id :" + item.order_id + " IsOK");
                                            checkSaveJob = false;
                                            errorMsg = "error SaveOrder";
                                        }

                                        // register customer
                                        string customerRowID = "";

                                        if (checkSaveJob)
                                        {
                                            job.LogMsg("RegisterCustomer order_id :" + item.order_id);
                                            customerRowID = job.RegisterCustomer(dataORDDetailCreate);
                                            if (customerRowID == null || customerRowID == "")
                                            {
                                                job.LogMsg("RegisterCustomer order_id :" + item.order_id + " IsOK");
                                                checkSaveJob = false;
                                                errorMsg = "error RegisterCustomer";
                                            }
                                            else
                                            {
                                                job.LogMsg("RegisterCustomer order_id :" + item.order_id + " IsOK");
                                                checkSaveJob = true;

                                                // SendEmail

                                                string EmailAddress = "";
                                                if (!string.IsNullOrEmpty(dataORDDetailCreate.ODRDetailCustomerList[0].email_address))
                                                {
                                                    job.LogMsg("Have SendEmail order_id :" + item.order_id);
                                                    EmailAddress = dataORDDetailCreate.ODRDetailCustomerList[0].email_address;
                                                }
                                                job.LogMsg("GenPDFAndSendEmail order_id :" + item.order_id);
                                                job.GenPDFAndSendEmail(customerRowID, dataORDDetailCreate.ODRDetailCustomerList[0].id_card_no, saveOrderResp.RETURN_IA_NO, dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, EmailAddress, isThai, user, pass, ip, Impersonate);


                                                // SendSMS

                                                string mainCode = "";
                                                if (dataORDDetailCreate.ODRDetailPackageList.Count > 0 && checkSaveJob)
                                                {
                                                    for (int i = 0; i <= dataORDDetailCreate.ODRDetailPackageList.Count - 1; i++)
                                                    {
                                                        if (!string.IsNullOrEmpty(dataORDDetailCreate.ODRDetailPackageList[i].sff_promotion_code) && dataORDDetailCreate.ODRDetailPackageList[i].package_type == "Main")
                                                        {
                                                            mainCode += "|" + dataORDDetailCreate.ODRDetailPackageList[i].sff_promotion_code;
                                                        }
                                                    }
                                                }
                                                string MsgSent = "";
                                                if (isThai)
                                                    MsgSent = MsgSentTH;
                                                else
                                                    MsgSent = MsgSentEN;

                                                job.LogMsg("SendSMS order_id :" + item.order_id);
                                                //job.SendSMS(dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, mainCode, isThai, MsgSent);

                                                // R20.9 SentSMS SCPE
                                                if (job.CheckSMSFlag(item.order_id, "Success", "OrderSCPE", dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, item.url))
                                                {
                                                    var resultsms = job.SendSMS(dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, mainCode, isThai, MsgSent, item.internet_no);
                                                    if (resultsms == "Success")
                                                    {
                                                        job.UpdateSMSFlag(item.order_id, "Success", "OrderSCPE", dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, item.url);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    job.LogMsg("End Save For SCPE");
                                }
                                else if (item.order_type.ToSafeString() == "TOPUP_MESH")
                                {
                                    /// Save For Mesh

                                    job.LogMsg("Save For Mesh");

                                    job.LogMsg("GetMeshCustomerProfile");
                                    job.LogMsg("internet_no = " + item.internet_no);
                                    job.LogMsg("order_id = " + item.order_id);

                                    GetMeshCustomerProfileModel informationDataModel = job.GetMeshCustomerProfile(item.internet_no, item.order_id);

                                    if (informationDataModel != null && informationDataModel.contact_mobile != null && checkSaveJob)
                                    {
                                        job.LogMsg("GetMeshCustomerProfile Have Data");

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


                                        string ContactMobile = informationDataModel.contact_mobile;
                                        ContactMobile = ContactMobile.Replace("-", "");
                                        /// Getvalue for sent to sff
                                        job.LogMsg("GetOrderChangeService");
                                        GetOrderChangeServiceModel DataForSentToSff = job.GetOrderChangeService(item.internet_no, item.order_id);
                                        if (DataForSentToSff != null)
                                        {
                                            job.LogMsg("GetOrderChangeService OK");
                                            job.LogMsg("CreateOrderMeshPromotion");
                                            CreateOrderMeshPromotionResult result = null;
                                            result = job.CreateOrderMeshPromotion(DataForSentToSff, item.internet_no);
                                            if (result != null && result.order_no.ToSafeString() != "")
                                            {
                                                job.LogMsg("CreateOrderMeshPromotion OK");
                                                string customerRowID = "";

                                                /// Update CustRegister 

                                                string PaymentMethod = item.command == "qr-code-payments-inquiry" ? paymentmethodQRCode : "4";
                                                job.LogMsg("RegisterCustomer sffOrder : " + result.order_no.ToSafeString());
                                                job.LogMsg("RegisterCustomer PaymentID : " + item.order_id);
                                                job.LogMsg("RegisterCustomer Tran ID : " + results.RESP_TRAN_ID.ToSafeString());
                                                job.LogMsg("RegisterCustomer PaymentMethod : " + PaymentMethod);

                                                customerRowID = job.RegisterCustomerMesh(result.order_no.ToSafeString(),
                                                                                         item.order_id.ToSafeString(),
                                                                                         results.RESP_TRAN_ID.ToSafeString(),
                                                                                         PaymentMethod);

                                                /// Sent SMS
                                                job.LogMsg("MeshSendSMS");
                                                //job.MeshSendSMS(ContactMobile, msgTxts);

                                                // R20.9 SentSMS Mesh
                                                if (job.CheckSMSFlag(item.order_id, "Success", "OrderMesh", ContactMobile, item.url))
                                                {
                                                    var resultsms = job.MeshSendSMS(ContactMobile, msgTxts, item.internet_no);
                                                    if (resultsms == "Success")
                                                    {
                                                        job.UpdateSMSFlag(item.order_id, "Success", "OrderMesh", ContactMobile, item.url);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                /// Sent SMS
                                                job.LogMsg("MeshSendErrorSMS");
                                                //job.MeshSendSMS(ContactMobile, msgErrorTxts);

                                                // R20.9 SentSMS Mesh
                                                if (job.CheckSMSFlag(item.order_id, "Error", "OrderMesh", ContactMobile, item.url))
                                                {
                                                    var resultsms = job.MeshSendSMS(ContactMobile, msgErrorTxts, item.internet_no);
                                                    if (resultsms == "Success")
                                                    {
                                                        job.UpdateSMSFlag(item.order_id, "Error", "OrderMesh", ContactMobile, item.url);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            /// Sent SMS
                                            job.LogMsg("MeshSendErrorSMS");
                                            //job.MeshSendSMS(ContactMobile, msgErrorTxts);

                                            // R20.9 SentSMS Mesh
                                            if (job.CheckSMSFlag(item.order_id, "Error", "OrderMesh", ContactMobile, item.url))
                                            {
                                                var resultsms = job.MeshSendSMS(ContactMobile, msgErrorTxts, item.internet_no);
                                                if (resultsms == "Success")
                                                {
                                                    job.UpdateSMSFlag(item.order_id, "Error", "OrderMesh", ContactMobile, item.url);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        job.LogMsg("Order Nodata or not paid");
                                    }
                                    job.LogMsg("End Save For Mesh");
                                }
                            }
                        }


                        // GetJob TimeOut
                        job.LogMsg("GetORDPendingPaymentTimeOutModel");
                        GetORDPendingPaymentTimeOutModel ORDPendingPaymentTimeOutData = job.GetORDPendingPaymentTimeOut();
                        if (ORDPendingPaymentTimeOutData != null && ORDPendingPaymentTimeOutData.GetORDPendingPaymentTimeOutList != null && ORDPendingPaymentTimeOutData.GetORDPendingPaymentTimeOutList.Count > 0)
                        {
                            foreach (var item in ORDPendingPaymentTimeOutData.GetORDPendingPaymentTimeOutList)
                            {
                                if (item.order_type.ToSafeString() == "SELL_ROUTER")
                                {
                                    if (item.reserve_timeslot_id != null && item.reserve_timeslot_id != "")
                                    {
                                        job.LogMsg("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id);
                                        ReleaseTimeSlotModel releaseTimeSlotModel = job.ReleaseTimeSlot(item.reserve_timeslot_id, item.order_id);
                                        if (releaseTimeSlotModel.RESULT == "-1")
                                        {
                                            job.LogMsg("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "fail");
                                        }
                                        else
                                        {
                                            job.LogMsg("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "ok");
                                        }
                                    }
                                    if (item.reserve_port_id != null && item.reserve_port_id != "")
                                    {
                                        job.LogMsg("ResRelease reserve_port_id : " + item.reserve_port_id);
                                        ResReleaseModel resReleaseModel = job.ResRelease(item.reserve_port_id, item.order_id);
                                        if (resReleaseModel.RESULT == "-1")
                                        {
                                            job.LogMsg("ResRelease reserve_port_id : " + item.reserve_port_id + "fail");
                                        }
                                        else
                                        {
                                            job.LogMsg("ResRelease reserve_port_id : " + item.reserve_port_id + "ok");
                                        }
                                    }

                                    job.LogMsg("SendSMS order_id :" + item.order_id);
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

                                    //job.SendSMS(item.mobile_no, "", isThai, MsgSent);

                                    // R20.9 SentSMS SCPE
                                    if (job.CheckSMSFlag(item.order_id, "Error", "OrderSCPE", item.mobile_no, ""))
                                    {
                                        var resultsms = job.SendSMS(item.mobile_no, "", isThai, MsgSent, item.internet_no);
                                        if (resultsms == "Success")
                                        {
                                            job.UpdateSMSFlag(item.order_id, "Error", "OrderSCPE", item.mobile_no, "");
                                        }
                                    }
                                }
                                else if (item.order_type.ToSafeString() == "TOPUP_MESH")
                                {
                                    if (item.reserve_timeslot_id != null && item.reserve_timeslot_id != "")
                                    {
                                        job.LogMsg("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id);
                                        ReleaseTimeSlotModel releaseTimeSlotModel = job.ReleaseTimeSlot(item.reserve_timeslot_id, item.order_id);
                                        if (releaseTimeSlotModel.RESULT == "-1")
                                        {
                                            job.LogMsg("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "fail");
                                        }
                                        else
                                        {
                                            job.LogMsg("ReleaseTimeSlot reserve_timeslot_id : " + item.reserve_timeslot_id + "ok");
                                        }
                                    }

                                    GetMeshCustomerProfileModel informationDataModel = job.GetMeshCustomerProfile(item.internet_no, item.order_id);

                                    if (informationDataModel != null && informationDataModel.contact_mobile != null)
                                    {
                                        string ContactMobile = informationDataModel.contact_mobile.ToSafeString();
                                        ContactMobile = ContactMobile.Replace("-", "");

                                        string[] msgErrorTxts = new string[2];
                                        if (informationDataModel.language == "1")
                                        {
                                            msgErrorTxts[0] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_1")).LovValue1.ToSafeString();
                                            msgErrorTxts[1] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_2")).LovValue1.ToSafeString();
                                        }
                                        else
                                        {
                                            msgErrorTxts[0] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_1")).LovValue2.ToSafeString();
                                            msgErrorTxts[1] = lovData.FirstOrDefault(l => !string.IsNullOrEmpty(l.LovValue5) && !string.IsNullOrEmpty(l.Name) && l.LovValue5.Equals("FBBOR041") && l.Name.Equals("SMS_MESSAGE_ERROR_2")).LovValue2.ToSafeString();
                                        }

                                        /// Sent SMS
                                        job.LogMsg("MeshSendErrorSMS");
                                        //job.MeshSendSMS(ContactMobile, msgErrorTxts);

                                        // R20.9 SentSMS Mesh
                                        if (job.CheckSMSFlag(item.order_id, "Error", "OrderMesh", ContactMobile, ""))
                                        {
                                            var resultsms = job.MeshSendSMS(ContactMobile, msgErrorTxts, item.internet_no);
                                            if (resultsms == "Success")
                                            {
                                                job.UpdateSMSFlag(item.order_id, "Error", "OrderMesh", ContactMobile, "");
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        #endregion

                    }

                }
                catch (Exception ex)
                {
                    job.LogMsg("FBBQueryOrderPendingPayment Error : " + ex.GetBaseException());
                }
            }

            job.UpdatOrderPaymentStatus("N");
            job.StopWatching("FBBQueryOrderPendingPaymentJob");
            job.LogMsg("FBBQueryOrderPendingPayment End");
        }

        public static async Task<paymentLog> CheckPayment(GetORDPendingPayment Data)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBQueryOrderPendingPaymentJob>();
            response result = new response();
            paymentLog results = new paymentLog();

            try
            {

                var contents = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("projectCode", Data.project_code),
                        new KeyValuePair<string, string>("command", Data.command),
                        new KeyValuePair<string, string>("merchantId", Data.merchant_id),
                        new KeyValuePair<string, string>("orderId", Data.order_id),
                        new KeyValuePair<string, string>("purchaseAmt", Data.purchase_amt),
                        new KeyValuePair<string, string>("saleId", Data.sale_id)
                    });

                using (var client = new HttpClient())
                {

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsync(Data.url, contents);

                    response.EnsureSuccessStatusCode();

                    using (HttpContent content = response.Content)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        XmlSerializer serializer = new XmlSerializer(typeof(response), new XmlRootAttribute("response"));
                        StringReader stringReader = new StringReader(responseBody);
                        result = (response)serializer.Deserialize(stringReader);

                        paymentLog paymentLogItem = new paymentLog
                        {
                            PAYMENT_ORDER_ID = Data.order_id,
                            RESP_STATUS = result.status,
                            RESP_RESP_CODE = result.respCode,
                            RESP_RESP_DESC = result.respDesc,
                            RESP_SALE_ID = result.saleId,
                            RESP_DETAIL1 = result.paymentStatus,
                            RESP_TRAN_ID = result.tranId
                        };
                        results = paymentLogItem;
                    }

                }

            }
            catch (Exception ex)
            {
                paymentLog paymentLogItem = new paymentLog
                {
                    PAYMENT_ORDER_ID = Data.order_id,
                    RESP_STATUS = "",
                    RESP_RESP_CODE = "",
                    RESP_RESP_DESC = "",
                    RESP_SALE_ID = "",
                    RESP_DETAIL1 = "error",
                    RESP_TRAN_ID = ""
                };

                results = paymentLogItem;
            }

            return results;
        }

        public static async Task<paymentLog> CheckPaymentQrCode(GetORDPendingPayment data)
        {
            Bootstrapper.Bootstrap();
            Bootstrapper.GetInstance<FBBQueryOrderPendingPaymentJob>();
            paymentLog results;
            QrCodePaymentsInquiryResponse responseApi;

            try
            {
                //data.qrcommand = @"/mpay-unified-qr-code/payments/inquiry";

                var query = new QrCodePaymentsInquiryRequest
                {
                    orderId = data.order_id,
                    tranId = ""
                };

                var client = new RestClient(data.url); //"https://chillchill.ais.co.th:8002"
                // client.Authenticator = new HttpBasicAuthenticator(username, password);
                var request = new RestRequest(data.qrcommand, Method.POST) //"/mpay-unified-qr-code/qr"
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new BatchRestSharpJsonSerializer()

                };
                request.AddHeader("appId", data.appid);
                request.AddHeader("appSecret", data.appsecret);
                request.AddBody(query);

                // execute the request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseApi = JsonConvert.DeserializeObject<QrCodePaymentsInquiryResponse>(response.Content) ?? new QrCodePaymentsInquiryResponse();

                    var paymentLogItem = new paymentLog
                    {
                        PAYMENT_ORDER_ID = string.IsNullOrEmpty(responseApi.orderId)
                            ? data.order_id
                            : responseApi.orderId,
                        RESP_STATUS = responseApi.status,
                        RESP_RESP_CODE = responseApi.respCode,
                        RESP_RESP_DESC = responseApi.respDesc,
                        RESP_DETAIL1 = string.IsNullOrEmpty(responseApi.status)
                            ? ""
                            : responseApi.status.ToUpper(),

                        RESP_QR_CODE_STR = responseApi.qrCodeStr,
                        RESP_TRAN_DTM = responseApi.tranDtm,
                        RESP_TRAN_ID = responseApi.tranId,
                        RESP_SERVICE_ID = responseApi.serviceId,
                        RESP_TERMINAL_ID = responseApi.terminalId,
                        RESP_LOCATION_NAME = responseApi.locationName,
                        RESP_AMOUNT = responseApi.amount,
                        RESP_SOF = responseApi.sof,
                        RESP_QR_TYPE = responseApi.qrType,
                        RESP_REFUND_DT = responseApi.refundDt,
                        RESP_DISPUTE_ID = responseApi.disputeId,
                        RESP_DISPUT_STATUS = responseApi.disputeStatus,
                        RESP_DISPUT_REASON_ID = responseApi.disputeReasonId,
                        RESP_REF1 = responseApi.ref1,
                        RESP_REF2 = responseApi.ref2,
                        RESP_REF3 = responseApi.ref3,
                        RESP_REF4 = responseApi.ref4,
                        RESP_REF5 = responseApi.ref5,
                    };
                    results = paymentLogItem;
                }
                else
                {
                    var paymentLogItem = new paymentLog
                    {
                        PAYMENT_ORDER_ID = data.order_id,
                        RESP_STATUS = "",
                        RESP_RESP_CODE = "",
                        RESP_RESP_DESC = "",
                        RESP_SALE_ID = "",
                        RESP_DETAIL1 = "error",
                        RESP_TRAN_ID = ""
                    };

                    results = paymentLogItem;
                }
            }
            catch (Exception)
            {
                var paymentLogItem = new paymentLog
                {
                    PAYMENT_ORDER_ID = data.order_id,
                    RESP_STATUS = "",
                    RESP_RESP_CODE = "",
                    RESP_RESP_DESC = "",
                    RESP_SALE_ID = "",
                    RESP_DETAIL1 = "error",
                    RESP_TRAN_ID = ""
                };

                results = paymentLogItem;
            }

            return results;
        }

        public static async Task<string> GetProcessName(string command)
        {
            string result;
            try
            {
                result = !string.IsNullOrEmpty(command) && command == "qr-code-payments-inquiry"
                    ? command
                    : "InquiryApi";
            }
            catch (Exception)
            {
                result = "InquiryApi";
            }
            return result;
        }
    }

    [Serializable()]
    public class response
    {
        public string tranId { get; set; }
        public string saleId { get; set; }
        public string creditCardNo { get; set; }
        public string orderId { get; set; }
        public string shipProvince { get; set; }
        public string incCustomerFee { get; set; }
        public string shipName { get; set; }
        public string shipCountry { get; set; }
        public string exchangeRate { get; set; }
        public string orderExpireDate { get; set; }
        public string customerId { get; set; }
        public string currency { get; set; }
        public string paymentStatus { get; set; }
        public string respDesc { get; set; }
        public string amount { get; set; }
        public string shipZip { get; set; }
        public string integrityStr { get; set; }
        public string shipAddress { get; set; }
        public string excCustomerFee { get; set; }
        public string paymentCode { get; set; }
        public string custEmail { get; set; }
        public string purchaseAmt { get; set; }
        public string remark1 { get; set; }
        public string respCode { get; set; }
        public string status { get; set; }
        public string remark2 { get; set; }
    }

    public class paymentLog
    {
        public string PAYMENT_ORDER_ID { get; set; }
        public string RESP_STATUS { get; set; }
        public string RESP_RESP_CODE { get; set; }
        public string RESP_RESP_DESC { get; set; }
        public string RESP_SALE_ID { get; set; }
        public string RESP_DETAIL1 { get; set; }
        public string RESP_TRAN_ID { get; set; }
        //18.10 : QR Code
        public string RESP_TRAN_DTM { get; set; }
        public string RESP_SERVICE_ID { get; set; }
        public string RESP_TERMINAL_ID { get; set; }
        public string RESP_LOCATION_NAME { get; set; }
        public string RESP_AMOUNT { get; set; }
        public string RESP_SOF { get; set; }
        public string RESP_QR_CODE_STR { get; set; }
        public string RESP_QR_TYPE { get; set; }
        public string RESP_REFUND_DT { get; set; }
        public string RESP_DISPUTE_ID { get; set; }
        public string RESP_DISPUT_STATUS { get; set; }
        public string RESP_DISPUT_REASON_ID { get; set; }
        public string RESP_REF1 { get; set; }
        public string RESP_REF2 { get; set; }
        public string RESP_REF3 { get; set; }
        public string RESP_REF4 { get; set; }
        public string RESP_REF5 { get; set; }
    }
}
