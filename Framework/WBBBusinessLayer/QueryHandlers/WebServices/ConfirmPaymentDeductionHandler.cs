using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBEntity.PanelModels.WebServices;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class ConfirmPaymentDeductionHandler : IQueryHandler<ConfirmPaymentDeductionQuery, ConfirmPaymentDeductionModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<GetPendingDeductionQuery, GetPendingDeductionModel> _getPendingDeduction;
        private readonly IQueryHandler<ConfPMPayQuery, ConfPMPayResponse> _confPMPayResp;
        private readonly IQueryHandler<evESeServiceQueryMassCommonAccountInfoQuery, evESeServiceQueryMassCommonAccountInfoModel> _evESeServiceQueryMassCommonAccountInfo;
        private readonly IQueryHandler<GetSendSMSDeductionPaymentQuery, GetSendSMSDeductionPaymentModel> _getSendSMSDeductionPayment;
        private readonly IQueryHandler<EncryptIntraAISServiceQuery, EncryptIntraAISServiceModel> _EncryptIntraAISService;
        private readonly IQueryHandler<confirmPaymenQuery, ConfPMPayResponse> _confirmPaymen;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IQueryHandler<DeductionUpddateSendSMSFlagQuery, string> _deductionUpddateSendSMSFlag;
        private readonly IQueryHandler<D_LstPMETaxRcptQuery, D_LstPMETaxRcptModel> _eReceiptHandler;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        public ConfirmPaymentDeductionHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IQueryHandler<GetPendingDeductionQuery, GetPendingDeductionModel> getPendingDeduction, IQueryHandler<ConfPMPayQuery, ConfPMPayResponse> confPMPayResp, IQueryHandler<evESeServiceQueryMassCommonAccountInfoQuery, evESeServiceQueryMassCommonAccountInfoModel> evESeServiceQueryMassCommonAccountInfo, IQueryHandler<GetSendSMSDeductionPaymentQuery, GetSendSMSDeductionPaymentModel> getSendSMSDeductionPayment, IQueryHandler<EncryptIntraAISServiceQuery, EncryptIntraAISServiceModel> encryptIntraAISService, ICommandHandler<SendSmsCommand> sendSmsCommand, IQueryHandler<DeductionUpddateSendSMSFlagQuery, string> deductionUpddateSendSMSFlag, IQueryHandler<D_LstPMETaxRcptQuery, D_LstPMETaxRcptModel> eReceiptHandler, IEntityRepository<FBB_CFG_LOV> lovService, IQueryHandler<confirmPaymenQuery, ConfPMPayResponse> confirmPaymen)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _getPendingDeduction = getPendingDeduction;
            _confPMPayResp = confPMPayResp;
            _evESeServiceQueryMassCommonAccountInfo = evESeServiceQueryMassCommonAccountInfo;
            _getSendSMSDeductionPayment = getSendSMSDeductionPayment;
            _EncryptIntraAISService = encryptIntraAISService;
            _sendSmsCommand = sendSmsCommand;
            _deductionUpddateSendSMSFlag = deductionUpddateSendSMSFlag;
            _eReceiptHandler = eReceiptHandler;
            _lovService = lovService;
            _confirmPaymen = confirmPaymen;
        }

        public ConfirmPaymentDeductionModel Handle(ConfirmPaymentDeductionQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new ConfirmPaymentDeductionModel
            {
                RESULT_CODE = "-1",
                RESULT_DESC = "FAIL"
            };

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.non_mobile_no, "ConfirmPaymentDeduction", "ConfirmPaymentDeductionHandler", "", "FBB|" + query.FullUrl, "");
                var sourceAddr = GetSourceAddr();
                var getPendingDeductionQuery = new GetPendingDeductionQuery()
                {
                    p_transaction_id = query.txn_id,
                    p_mobile_no = query.non_mobile_no
                };

                GetPendingDeductionModel GetPendingDeductionResults = GetPendingDeduction(getPendingDeductionQuery);
                if (GetPendingDeductionResults != null && GetPendingDeductionResults.orderPaendingDeductionDatas != null && GetPendingDeductionResults.orderPaendingDeductionDatas.Count > 0)
                {
                    _logger.Info("GetPendingDeduction HaveData.");
                    foreach (var orderPaendingDeductionData in GetPendingDeductionResults.orderPaendingDeductionDatas)
                    {
                        _logger.Info("OrderPaendingDeduction tran_id : " + orderPaendingDeductionData.pm_tran_id);
                        string[] PM_STATUS_CD = new string[1];
                        PM_STATUS_CD[0] = "";
                        double PM_PAID_AMT = 0;
                        double PM_TAX_AMT = 0;
                        long PM_RECEIPT_LOCATION = 0;
                        long PM_PAYMENT_CHANNEL_ID = 0;
                        long PM_SHIFT_NUM = 0;
                        double PM_PAYMENT_METHOD_ID = 0;
                        long PM_PRINT_FLAG = 0;

                        double.TryParse(orderPaendingDeductionData.pm_paid_amt, out PM_PAID_AMT);
                        long.TryParse(orderPaendingDeductionData.pm_receipt_location, out PM_RECEIPT_LOCATION);
                        long.TryParse(orderPaendingDeductionData.pm_payment_channel_id, out PM_PAYMENT_CHANNEL_ID);
                        double.TryParse(orderPaendingDeductionData.pm_payment_method_id, out PM_PAYMENT_METHOD_ID);
                        long.TryParse(orderPaendingDeductionData.pm_print_flag, out PM_PRINT_FLAG);
                        ConfPMPayResponse ConfPMPayResults = null;

                        var lovList = _lovService.Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == "CONFRIMPAYMENT" && lov.ACTIVEFLAG == "Y");
                        var lovFlag = lovList.FirstOrDefault(i=> i.LOV_NAME == "CONFRIMPAYMENT_FLAG").LOV_VAL1;
                        var channelGroupId = lovList.FirstOrDefault(i => i.LOV_NAME == "channelGroupId").LOV_VAL1;
                        var debtType = lovList.FirstOrDefault(i => i.LOV_NAME == "debtType").LOV_VAL1;
                        var keyRec1Desc = lovList.FirstOrDefault(i => i.LOV_NAME == "keyRec1Desc").LOV_VAL1;
                        var settleCompany = lovList.FirstOrDefault(i => i.LOV_NAME == "settleCompany").LOV_VAL1;
                        var settleSystemId = lovList.FirstOrDefault(i => i.LOV_NAME == "settleSystemId").LOV_VAL1;
                        var wtAmt = lovList.FirstOrDefault(i => i.LOV_NAME == "wtAmt").LOV_VAL1;

                        if (lovFlag == "Y")
                        {

                            List<payInfo> pays = new List<payInfo>();
                            pays.Add(new payInfo()
                            {
                                baNum = orderPaendingDeductionData.pm_billing_acc_num,
                                mobileNum = orderPaendingDeductionData.pm_moblie_num,
                                paidAmt = PM_PAID_AMT,
                                debtType = debtType,

                            });
                            List<method> methods = new List<method>();
                            methods.Add(new method()
                            {
                                methodId = PM_PAYMENT_METHOD_ID.ToString(),
                                paidAmt = PM_PAID_AMT,
                                bankCode = Int32.Parse(orderPaendingDeductionData.pm_bank_code),
                            });

                            confirmPaymenQuery confirmPaymen = new confirmPaymenQuery()
                            {
                                User = "FBBQueryOrderPendingDeduction",
                                Url = orderPaendingDeductionData.endpoint,
                                username = orderPaendingDeductionData.user_name,
                                password = orderPaendingDeductionData.password,
                                fullUrl = query.FullUrl,
                                transaction_id = query.order_transaction_id,

                                bodyQuery = new confirmPaymenBodyQuery()
                                {
                                    location = PM_RECEIPT_LOCATION,
                                    paymentChannel = PM_PAYMENT_CHANNEL_ID,
                                    tranId = orderPaendingDeductionData.pm_tran_id,
                                    channelGroupId = long.Parse(channelGroupId),
                                    settleSystemId = long.Parse(settleSystemId),
                                    settleCompany = settleCompany,
                                    subChannel = orderPaendingDeductionData.pm_sub_channel,
                                    paidAmt = PM_PAID_AMT,
                                    shiftNum = PM_SHIFT_NUM,
                                    wtAmt = long.Parse(wtAmt),
                                    keyRec1 = orderPaendingDeductionData.pm_tran_id,
                                    keyRec1Desc = keyRec1Desc,
                                    terminalId = orderPaendingDeductionData.pm_terminal_id,
                                    userId = orderPaendingDeductionData.pm_user_id,
                                    payInfo = pays,
                                    method = methods
                                }
                            };
                            ConfPMPayResults = ConfPMPayNew(confirmPaymen);

                            //List<payInfo> pays = new List<payInfo>();
                            //pays.Add(new payInfo()
                            //{
                            //    baNum = "32200050401704",//H
                            //    mobileNum = "8850099315",//H
                            //    paidAmt = 693.64,
                            //    debtType = debtType,

                            //});
                            //List<method> methods = new List<method>();
                            //methods.Add(new method()
                            //{
                            //    methodId = "158",//H
                            //    paidAmt = 693.64,
                            //    bankCode = 998,//H
                            //});

                            //confirmPaymenQuery confirmPaymen = new confirmPaymenQuery()
                            //{
                            //    User = "FBBQueryOrderPendingDeduction",
                            //    Url = "https://sit-collection-api.ais.co.th/CollectionService/gen-receipt",
                            //    username = "U_FBBWEBTEST",
                            //    password = "P_FBBWEBTEST",
                            //    fullUrl = "https://sit-collection-api.ais.co.th/CollectionService/gen-receipt",
                            //    transaction_id = "Test1000000001",

                            //    bodyQuery = new confirmPaymenBodyQuery()
                            //    {
                            //        location = 1047,//H
                            //        paymentChannel = 4,
                            //        tranId = "T2232517093110216938",
                            //        channelGroupId = long.Parse(channelGroupId),
                            //        settleSystemId = long.Parse(settleSystemId),
                            //        settleCompany = settleCompany,
                            //        subChannel = "FBBPay",               
                            //        paidAmt = 693.64,
                            //        shiftNum = 0,
                            //        wtAmt = long.Parse(wtAmt),
                            //        keyRec1 = "T2232517093110216938",
                            //        keyRec1Desc = keyRec1Desc,
                            //        terminalId = "0",
                            //        userId = "PY-ACD",
                            //        payInfo = pays,
                            //        method = methods
                            //    }
                            //};
                            //ConfPMPayResults = ConfPMPayNew(confirmPaymen);
                        }
                        else
                        {
                            ConfPMPayQuery confPMPayQuery = new ConfPMPayQuery()
                            {
                                User = "FBBQueryOrderPendingDeduction",
                                Url = orderPaendingDeductionData.endpoint,
                                username = orderPaendingDeductionData.user_name,
                                password = orderPaendingDeductionData.password,
                                PM_MOBLIE_NUM = orderPaendingDeductionData.pm_moblie_num,
                                PM_BILLING_ACC_NUM = orderPaendingDeductionData.pm_billing_acc_num,
                                PM_STATUS_CD = PM_STATUS_CD,
                                PM_PAID_AMT = PM_PAID_AMT,
                                PM_TAX_AMT = PM_TAX_AMT,
                                PM_RECEIPT_LOCATION = PM_RECEIPT_LOCATION,
                                PM_PAYMENT_CHANNEL_ID = PM_PAYMENT_CHANNEL_ID,
                                PM_SHIFT_NUM = PM_SHIFT_NUM,
                                PM_TERMINAL_ID = orderPaendingDeductionData.pm_terminal_id,
                                PM_USER_ID = orderPaendingDeductionData.pm_user_id,
                                PM_NEXT_BILL_FLAG = "",
                                PM_PAYMENT_METHOD_ID = PM_PAYMENT_METHOD_ID,
                                PM_PRINT_FLAG = PM_PRINT_FLAG,
                                PM_TRAN_ID = orderPaendingDeductionData.pm_tran_id,
                                PM_BANK_CODE = orderPaendingDeductionData.pm_bank_code,
                                PM_SUB_CHANNEL = orderPaendingDeductionData.pm_sub_channel,
                                PM_ORDER_TRANSACTION_ID = query.order_transaction_id,
                                FullUrl = query.FullUrl,
                            };
                             ConfPMPayResults = ConfPMPay(confPMPayQuery);
                        }
                        






                        if (ConfPMPayResults != null && ConfPMPayResults.PM_TUX_MSG != null && ConfPMPayResults.PM_TUX_MSG == "Success")
                        {
                            _logger.Info("ConfPMPay Success.");
                            string MobileNo = GetMobile(orderPaendingDeductionData.pm_moblie_num);
                            if (MobileNo != "")
                            {
                                GetSendSMSDeductionPaymentModel GetSendSMSDeductionPaymentResults = GetSendSMSDeductionPayment(orderPaendingDeductionData.pm_tran_id, orderPaendingDeductionData.pm_moblie_num);
                                if (GetSendSMSDeductionPaymentResults != null && GetSendSMSDeductionPaymentResults.list_order_send_sms_payment != null && GetSendSMSDeductionPaymentResults.list_order_send_sms_payment.Count > 0)
                                {
                                    _logger.Info("GetSendSMSDeductionPayment Success.");

                                    //TODO: Check eReceipt before encrypt
                                    if (CheckeReceipt(ConfPMPayResults, query.txn_id, query.FullUrl, query.non_mobile_no))
                                    {
                                        // Has eReceipt
                                        EncryptIntraAISServiceBody encryptIntraAISServiceBody = new EncryptIntraAISServiceBody()
                                        {
                                            ssid = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].ssid.ToSafeString(),
                                            command = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].command.ToSafeString(),
                                            input = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].input.ToSafeString(),
                                        };
                                        EncryptIntraAISServiceQuery encryptIntraAISServiceQuery = new EncryptIntraAISServiceQuery()
                                        {
                                            p_transaction_id = orderPaendingDeductionData.pm_tran_id,
                                            Url = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].endpoint,
                                            body = encryptIntraAISServiceBody,
                                            p_non_mobile_no = query.non_mobile_no
                                        };
                                        EncryptIntraAISServiceModel EncryptIntraAISServiceResults = EncryptIntraAISService(encryptIntraAISServiceQuery);
                                        if (EncryptIntraAISServiceResults != null && EncryptIntraAISServiceResults.resultcode == "1")
                                        {
                                            _logger.Info("EncryptIntraAISService Success.");
                                            string tmpUrl = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].receipt_url + EncryptIntraAISServiceResults.output;
                                            string tmpMsgTH = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_th;
                                            string tmpMsgEN = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_en;
                                            string tmpMsgeReceiptTH = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_eReceipt_th;
                                            string tmpMsgeReceiptEN = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_eReceipt_en;
                                            tmpMsgTH += tmpMsgeReceiptTH.Replace("{URL}", tmpUrl);
                                            tmpMsgEN += tmpMsgeReceiptEN.Replace("{URL}", tmpUrl);
                                            SendSMS(orderPaendingDeductionData.pm_tran_id, orderPaendingDeductionData.pm_moblie_num, MobileNo, tmpMsgTH, tmpMsgEN, sourceAddr, query.FullUrl, query.update_by);

                                            result.RESULT_CODE = "0";
                                            result.RESULT_DESC = "Success";
                                        }
                                        else
                                        {
                                            _logger.Info("EncryptIntraAISService Nodata.");

                                            string tmpMsgTH = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_th;
                                            string tmpMsgEN = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_en;
                                            SendSMS(orderPaendingDeductionData.pm_tran_id, orderPaendingDeductionData.pm_moblie_num, MobileNo, tmpMsgTH, tmpMsgEN, sourceAddr, query.FullUrl, query.update_by);

                                            result.RESULT_CODE = "-1";
                                            result.RESULT_DESC = "EncryptIntraAISService Nodata.";
                                        }
                                    }
                                    else
                                    {
                                        // No eReceipt
                                        string tmpMsgTH = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_th;
                                        string tmpMsgEN = GetSendSMSDeductionPaymentResults.list_order_send_sms_payment[0].message_en;
                                        SendSMS(orderPaendingDeductionData.pm_tran_id, orderPaendingDeductionData.pm_moblie_num, MobileNo, tmpMsgTH, tmpMsgEN, sourceAddr, query.FullUrl, query.update_by);
                                    }

                                }
                                else
                                {
                                    _logger.Info("GetSendSMSDeductionPayment Nodata.");

                                    result.RESULT_CODE = "-1";
                                    result.RESULT_DESC = "GetSendSMSDeductionPayment Nodata.";
                                }
                            }
                            else
                            {
                                _logger.Info("No MobileNo.");

                                result.RESULT_CODE = "-1";
                                result.RESULT_DESC = "No MobileNo.";
                            }
                        }
                        else
                        {
                            _logger.Info("ConfPMPay FAIL.");

                            result.RESULT_CODE = "-1";
                            result.RESULT_DESC = "ConfPMPay FAIL.";
                        }
                    }
                }
                else
                {
                    _logger.Info("GetPendingDeduction Nodata.");

                    result.RESULT_CODE = "-1";
                    result.RESULT_DESC = "GetPendingDeduction Nodata.";
                }

                return result;
            }
            catch (Exception ex)
            {
                result.RESULT_CODE = "-1";
                result.RESULT_DESC = ex.GetErrorMessage();
                _logger.Info("Error call ConfirmPaymentDeductionHandler : " + ex.GetErrorMessage());
                return result;
            }
            finally
            {
                var resultLog = (result ?? new ConfirmPaymentDeductionModel()).RESULT_CODE == "0" ? "Success" : "Failed";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, resultLog, result.RESULT_DESC, "");
            }
        }

        private GetPendingDeductionModel GetPendingDeduction(GetPendingDeductionQuery query)
        {
            GetPendingDeductionModel Results = new GetPendingDeductionModel();
            try
            {
                Results = _getPendingDeduction.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
            return Results;
        }

        private ConfPMPayResponse ConfPMPay(ConfPMPayQuery query)
        {
            ConfPMPayResponse Results = new ConfPMPayResponse();
            try
            {
                Results = _confPMPayResp.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
            return Results;
        }
        private ConfPMPayResponse ConfPMPayNew(confirmPaymenQuery query)
        {
            ConfPMPayResponse Results = new ConfPMPayResponse();
            try
            {
                Results = _confirmPaymen.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
            return Results;
        }

        private string GetMobile(string nonMobileNo)
        {
            string Results = "";
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "4",
                inMobileNo = nonMobileNo,
                inCardNo = "",
                inCardType = "",
                Page = "FBBQueryOrderPendingDeduction",
                Username = "FBBQueryOrderPendingDeduction",
                ClientIP = "",
                FullUrl = ""
            };
            var a = _evESeServiceQueryMassCommonAccountInfo.Handle(query);
            if (a != null && a.outServiceMobileNo != null)
            {
                Results = a.outServiceMobileNo;
            }
            return Results;
        }

        private GetSendSMSDeductionPaymentModel GetSendSMSDeductionPayment(string p_transaction_id, string p_mobile_no)
        {
            GetSendSMSDeductionPaymentQuery query = new GetSendSMSDeductionPaymentQuery()
            {
                p_transaction_id = p_transaction_id,
                p_mobile_no = p_mobile_no
            };
            GetSendSMSDeductionPaymentModel Results = new GetSendSMSDeductionPaymentModel();
            try
            {
                Results = _getSendSMSDeductionPayment.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
            return Results;
        }

        private EncryptIntraAISServiceModel EncryptIntraAISService(EncryptIntraAISServiceQuery query)
        {
            EncryptIntraAISServiceModel Results = new EncryptIntraAISServiceModel();
            try
            {
                Results = _EncryptIntraAISService.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
            return Results;
        }

        private Object thisLock = new Object();

        private void SendSMS(string p_transaction_id, string nonmobileNo, string mobileNo, string msgtxtTH, string msgtxtEN, string sourceAddr, string fullUrl, string p_update_by)
        {
            lock (thisLock)
            {
                _logger.Info("SendSMS");
                var tempMobileNo = mobileNo;
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                SendSmsCommand command1 = new SendSmsCommand()
                {
                    FullUrl = fullUrl, //"",
                    Source_Addr = sourceAddr, //"AISFIBRE",
                    Destination_Addr = mobileNo,
                    Message_Text = msgtxtTH,
                    Transaction_Id = tempMobileNo
                };
                _sendSmsCommand.Handle(command1);
                SendSmsCommand command2 = new SendSmsCommand()
                {
                    FullUrl = fullUrl, //"",
                    Source_Addr = sourceAddr, //"AISFIBRE",
                    Destination_Addr = mobileNo,
                    Message_Text = msgtxtEN,
                    Transaction_Id = tempMobileNo
                };
                _sendSmsCommand.Handle(command2);
                DeductionUpddateSendSMSFlag(p_transaction_id, nonmobileNo, p_update_by);
                _logger.Info("SendSMS Success.");
            }
        }

        private void DeductionUpddateSendSMSFlag(string p_transaction_id, string p_nonmobile_no, string p_update_by)
        {
            DeductionUpddateSendSMSFlagQuery query = new DeductionUpddateSendSMSFlagQuery()
            {
                p_transaction_id = p_transaction_id,
                p_nonmobile_no = p_nonmobile_no,
                p_sms_flag = "Y",
                p_update_by = p_update_by
            };
            string Results = "";
            try
            {
                Results = _deductionUpddateSendSMSFlag.Handle(query);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
        }

        private bool CheckeReceipt(ConfPMPayResponse confPMPayResults, string txn_id, string fullUrl, string non_mobile_no)
        {
            var arrReceiptid = confPMPayResults?.PM_RECEIPT_ID?.Select(x => x.ToSafeString()).ToArray();
            var result = _eReceiptHandler.Handle(new D_LstPMETaxRcptQuery()
            {
                InternetNo = non_mobile_no,
                TransactionId = txn_id,
                PM_RECEIPT_ID = arrReceiptid,
                FullUrl = fullUrl
            });

            var eReceipt = result?.PM_ETAX_FLAG?.FirstOrDefault();
            return (eReceipt == "Y");
        }

        private string GetSourceAddr()
        {
            var sourceAddr = "AISFIBRE";

            var lovList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == "CONFIG" && lov.LOV_NAME == "SENDER_SMS")
                     .OrderBy(o => o.ORDER_BY);

            if (lovList.Any())
            {
                var lovSource = lovList.FirstOrDefault();
                if (lovSource != null && !string.IsNullOrEmpty(lovSource.LOV_VAL1))
                {
                    sourceAddr = lovSource.LOV_VAL1;
                }
            }

            return sourceAddr;
        }
    }
}