
using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract.Queries.WebServices;

namespace FBBBulkCorpBatchService
{
    // using One2NetBusinessLayer.QueryHandlers.Common;
    // using One2NetContract.Queries.InWebServices;
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.ExWebServices;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.ExWebServiceModels;
    using WBBEntity.PanelModels.WebServiceModels;
    // using One2NetContract.Commands.InWebServices;

    public class FBBBulkCorpBatchServiceJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly ICommandHandler<UpdateBulkCorpCommand> _UpdateBulkCorpCommand;
        private readonly ICommandHandler<BatchUpdateMailStatusCommand> _UpdateStatusMailCommand;

        private string username = "BulkCorpBatch";

        private string errorMsg = string.Empty;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        public FBBBulkCorpBatchServiceJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<MailLogCommand> mailLogCommand,
             ICommandHandler<SendMailBatchNotificationCommand> sendMail,
            ICommandHandler<UpdateBulkCorpCommand> updateBulkCorpCommand,
             ICommandHandler<BatchUpdateMailStatusCommand> updateStatusMailCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _mailLogCommand = mailLogCommand;
            _sendMail = sendMail;
            _UpdateBulkCorpCommand = updateBulkCorpCommand;
            _UpdateStatusMailCommand = updateStatusMailCommand;

        }

        #region WebService WorkFlow

        public List<DetialBlukCorpRegister> ExecuteGetBulkCorpRegister()
        {
            GetBlukCorpRegisterModel result = new GetBlukCorpRegisterModel();
            var query = new GetBatchBulkCorpRegister()
            {
                p_test_parameter = "test"
            };
            result = _queryProcessor.Execute(query);
            if (result == null)
                return null;

            return result.P_RES_DATA;

        }

        public void ExecuteWorkFlowService()
        {
            _logger.Info("Start ExecuteWorkFlowService.");
            StartWatching();

            BatchBulkCorpModel result = new BatchBulkCorpModel();
            try
            {
                var getRegister = ExecuteGetBulkCorpRegister();
                if (getRegister != null && getRegister.Count > 0)
                {
                    foreach (var item in getRegister)
                    {
                        #region WF
                        // DetialBlukCorpRegister item = new DetialBlukCorpRegister();
                        //  item.BULK_NO = "B20170518000878";//"B20170515000740";
                        //  item.ORDER_NO = "AIR-BC-201705-001057";//"AIR-BC-201705-000800";

                        var query = new GetBatchBulkCorpQuery()
                        {
                            P_BULK_NUMBER = item.BULK_NO
                           ,
                            P_ORDER_NUMBER = item.ORDER_NO

                        };

                        result = _queryProcessor.Execute(query);
                        if (result != null)
                        {
                            string bulkno = String.Format("{0}|{1}", item.BULK_NO, item.ORDER_NO);
                            _logger.Info("execute GetBatchBulkCorpQuery :" + result.OUTPUT_RETURN_MESSAGE + bulkno);

                            if (result.OUTPUT_RETURN_CODE == "0")
                            {
                                SaveOrderResp resultBulk = SetOrderBulkCorpRespWF(result);
                                if (resultBulk != null)
                                {

                                    Console.WriteLine("WF Order No :" + item.ORDER_NO);

                                    if (resultBulk.RETURN_CODE == 0)
                                    {
                                        UpdateStatus(username, item.BULK_NO, item.ORDER_NO, "WF", "0", "");
                                        if (string.IsNullOrEmpty(item.STATUS_SEND_SFF))
                                        {
                                            ExecuteSFFService(item.BULK_NO, item.ORDER_NO);
                                        }
                                    }
                                    else
                                    {
                                        // UpdateStatus(username, item.BULK_NO, item.ORDER_NO, "WF", "-1", resultBulk.RETURN_MESSAGE);
                                    }
                                }
                                else
                                {
                                    // UpdateStatus(username, item.BULK_NO, item.ORDER_NO, "WF", "-1", resultBulk.RETURN_MESSAGE);
                                    _logger.Info("execute SetOrderBulkCorpRespWF error :" + resultBulk.RETURN_MESSAGE + bulkno);
                                }

                            }
                            else
                            {
                                //  UpdateStatus(username, item.BULK_NO, item.ORDER_NO, "WF", "-1", "PROC_DETAIL_WORKFLOW1 data null.");
                                bulkno = String.Format("{0}|{1}", item.BULK_NO, item.ORDER_NO);
                                _logger.Info("execute SetOrderBulkCorpRespWF error :" + "PROC_DETAIL_WORKFLOW1 data null." + bulkno);
                            }

                        }
                        else
                        {
                            UpdateStatus(username, item.BULK_NO, item.ORDER_NO, "WF", "-1", "PROC_DETAIL_WORKFLOW1 data null.");
                            string bulkno = String.Format("{0}|{1}", item.BULK_NO, item.ORDER_NO);
                            _logger.Info("execute SetOrderBulkCorpRespWF error :" + "PROC_DETAIL_WORKFLOW1 data null." + bulkno);
                        }


                        #endregion
                    }
                }

                StopWatching("end ExecuteWorkFlowService");
            }
            catch (Exception ex)
            {
                _logger.Info("Error ExecuteWorkFlowService : " + ex.GetErrorMessage());
                StopWatching("End ExecuteWorkFlowService");
            }

        }

        //call webservice Workflow
        private SaveOrderResp SetOrderBulkCorpRespWF(BatchBulkCorpModel model)
        {
            SaveOrderResp result = new SaveOrderResp();
            var query = new GetSaveBulkCorpOrderNewQuery
            {
                GetBulkCorpRegister = model,
            };
            result = _queryProcessor.Execute(query);

            return result;
        }

        #endregion

        #region WebService SFF
        public void ExecuteSFFService(string bulk, string order)
        {
            try
            {
                var query = new GetBatchBulkCorpSFFQuery()
                {
                    P_BULK_NUMBER = bulk
                   ,
                    P_ORDER_NUMBER = order
                };

                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    string bulkno = String.Format("{0}|{1}", bulk, order);
                    _logger.Info("execute GetBatchBulkCorpSFFQuery Error :" + result.OUTPUT_return_message + bulkno);
                    if (result.OUTPUT_return_code == "0")
                    {
                        List<DetailBulkCorpSFF> bulkregis = result.P_CALL_SFF;
                        List<DetailBulkCorpListServiceVdsl> bulkvdsl = result.P_LIST_SERVICE_VDSL;
                        List<DetailBulkCorpListServiceVdslRouter> bulkvdslrouter = result.P_LIST_SERVICE_VDSL_ROUTER;
                        List<DetailBulkCorpSffPromotionCur> bulksffpromotioncur = result.P_SFF_PROMOTION_CUR;
                        List<DetailBulkCorpSffPromotionOntopCur> bulksffpromotionontopcur = result.P_SFF_PROMOTION_ONTOP_CUR;
                        List<DetailBulkCorpListInstanceCur> bulklistinstancecur = result.P_LIST_INSTANCE_CUR;
                        if (bulkregis.Count > 0)
                        {
                            foreach (DetailBulkCorpSFF model in bulkregis)
                            {
                                var sff = SetOrderBulkCorpRespSFF(model, bulkvdsl, bulkvdslrouter, bulksffpromotioncur,
                                         bulksffpromotionontopcur, bulklistinstancecur);
                                if (sff != null)
                                {
                                    Console.WriteLine("SF Order No :" + order);

                                    if (sff.ReturnCode == "0")
                                    {
                                        UpdateStatus(username, bulk, order, "SFF", sff.ReturnCode, sff.ReturnMessage);
                                    }
                                    else
                                    {
                                        UpdateStatus(username, bulk, order, "SFF", sff.ReturnCode, sff.ReturnMessage);
                                    }
                                }
                                else
                                {
                                    UpdateStatus(username, bulk, order, "SFF", "-1", sff.ReturnMessage);
                                    bulkno = String.Format("{0}|{1}", bulk, order);
                                    _logger.Info("execute SetOrderBulkCorpRespSFF error :" + sff.ReturnMessage + bulkno);
                                }
                            }

                        }

                    }
                    else
                    {
                        UpdateStatus(username, bulk, order, "SFF", "-1", result.OUTPUT_return_message);
                        bulkno = String.Format("{0}|{1}", bulk, order);
                        _logger.Info("execute SetOrderBulkCorpRespSFF error :" + result.OUTPUT_return_message + bulkno);
                    }
                }
                else
                {
                    UpdateStatus(username, bulk, order, "SFF", "-1", "PROC_DETAIL_SFF1 is " + result.OUTPUT_return_message);
                    string bulkno = String.Format("{0}|{1}", bulk, order);
                    _logger.Info("execute SetOrderBulkCorpRespSFF error :" + result.OUTPUT_return_message + bulkno);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error ExecuteSFFService : " + ex.GetErrorMessage());
                UpdateStatus(username, bulk, order, "SFF", "-1", ex.GetErrorMessage());
            }

        }

        private evOMNewRegisMultiInstanceModel SetOrderBulkCorpRespSFF(DetailBulkCorpSFF model
            , List<DetailBulkCorpListServiceVdsl> bulkvdsl
            , List<DetailBulkCorpListServiceVdslRouter> bulkvdslrouter
            , List<DetailBulkCorpSffPromotionCur> bulksffpromotioncur
            , List<DetailBulkCorpSffPromotionOntopCur> bulksffpromotionontopcur
            , List<DetailBulkCorpListInstanceCur> bulklistinstancecur)
        {
            var query = new evOMNewRegisMultiInstanceQuery
            {
                bulkvdsl = bulkvdsl,
                bulkvdslrouter = bulkvdslrouter,
                bulksffpromotioncur = bulksffpromotioncur,
                bulksffpromotionontopcur = bulksffpromotionontopcur,
                bulklistinstancecur = bulklistinstancecur,


                referenceNo = model.p_referenceNo,//"FBB-2017-04-12-1058",//
                accountCat = model.P_accountCat.ToSafeString(),
                accountSubCat = model.P_accountSubCat.ToSafeString(), //"SME",
                idCardType = model.P_idCardType.ToSafeString(),
                idCardNo = model.P_idCardNo.ToSafeString(),
                titleName = model.P_titleName.ToSafeString(),
                firstName = model.P_firstName.ToSafeString(),
                lastName = model.P_lastName.ToSafeString(),
                saName = model.P_saName.ToSafeString(),
                baName = model.P_baName.ToSafeString(),
                caNumber = model.P_caNumber.ToSafeString(),
                baNumber = model.P_baNumber.ToSafeString(),
                saNumber = model.P_saNumber.ToSafeString(),
                birthdate = model.P_birthdate.ToSafeString(),
                gender = model.P_gender.ToSafeString(),
                billName = model.P_billName.ToSafeString(),
                billCycle = model.P_billCycle.ToSafeString(),
                billLanguage = model.P_billLanguage.ToSafeString(),
                engFlag = model.P_engFlag.ToSafeString(),
                accHomeNo = model.P_accHomeNo.ToSafeString(),
                accBuildingName = model.P_accBuildingName.ToSafeString(),
                accFloor = model.P_accFloor.ToSafeString(),
                accRoom = model.P_accRoom.ToSafeString(),
                accMoo = model.P_accMoo.ToSafeString(),
                accMooBan = model.P_accMooBan.ToSafeString(),
                accSoi = model.P_accSoi.ToSafeString(),
                accStreet = model.P_accStreet.ToSafeString(),
                accTumbol = model.P_accTumbol.ToSafeString(),
                accAmphur = model.P_accAmphur.ToSafeString(),
                accProvince = model.P_accProvince.ToSafeString(),
                accZipCode = model.P_accZipCode.ToSafeString(),
                billHomeNo = model.P_billHomeNo.ToSafeString(),
                billBuildingName = model.P_billBuildingName.ToSafeString(),
                billFloor = model.P_billFloor.ToSafeString(),
                billRoom = model.P_billRoom.ToSafeString(),
                billMoo = model.P_billMoo.ToSafeString(),
                billMooBan = model.P_billMooBan.ToSafeString(),
                billSoi = model.P_billSoi.ToSafeString(),
                billStreet = model.P_billStreet.ToSafeString(),
                billTumbol = model.P_billTumbol.ToSafeString(),
                billAmphur = model.P_billAmphur.ToSafeString(),
                billProvince = model.P_billProvince.ToSafeString(),
                billZipCode = model.P_billZipCode.ToSafeString(),
                userId = model.P_userId.ToSafeString(),
                dealerLocationCode = model.P_dealerLocationCode.ToSafeString(),
                ascCode = model.P_ascCode.ToSafeString(),
                orderReason = model.P_orderReason.ToSafeString(),
                remark = model.P_remark.ToSafeString(),
                saVatName = model.P_saVatName.ToSafeString(),
                saVatAddress1 = model.P_saVatAddress1.ToSafeString(),
                saVatAddress2 = model.P_saVatAddress2.ToSafeString(),
                saVatAddress3 = model.P_saVatAddress3.ToSafeString(),
                saVatAddress4 = model.P_saVatAddress4.ToSafeString(),
                saVatAddress5 = model.P_saVatAddress5.ToSafeString(),
                saVatAddress6 = model.P_saVatAddress6.ToSafeString(),
                contactFirstName = model.P_contactFirstName.ToSafeString(),
                contactLastName = model.P_contactLastName.ToSafeString(),
                contactTitle = model.P_contactTitle.ToSafeString(), //"คุณ",// model.CONTACT_TITLE_CODE.ToSafeString(),คุณ
                mobileNumberContact = model.P_mobileNumberContact.ToSafeString(),
                phoneNumberContact = model.P_phoneNumberContact.ToSafeString(),
                emailAddress = model.P_emailAddress.ToSafeString(),
                saHomeNo = model.P_saHomeNo.ToSafeString(),
                saBuildingName = model.P_saBuildingName.ToSafeString(),
                saFloor = model.P_saFloor.ToSafeString(),
                saRoom = model.P_saRoom.ToSafeString(),
                saMoo = model.P_saMoo.ToSafeString(),
                saMooBan = model.P_saMooBan.ToSafeString(),
                saSoi = model.P_saSoi.ToSafeString(),
                saStreet = model.P_saStreet.ToSafeString(),
                saTumbol = model.P_saTumbol.ToSafeString(),
                saAmphur = model.P_saAmphur.ToSafeString(),
                saProvince = model.P_saProvince.ToSafeString(),
                saZipCode = model.P_saZipCode.ToSafeString(),
                orderType = model.P_orderType.ToSafeString(),
                channel = model.P_channel.ToSafeString(),
                projectName = model.P_projectName.ToSafeString(),
                caBranchNo = model.P_caBranchNo.ToSafeString(),
                saBranchNo = model.P_saBranchNo.ToSafeString(),
                chargeType = model.P_chargeType.ToSafeString(),
                sourceSystem = model.P_sourceSystem.ToSafeString(),
                subcontractor = model.P_subcontractor.ToSafeString(),
                installStaffID = model.P_installStaffID.ToSafeString(),
                employeeID = model.P_employeeID.ToSafeString(),
                billMedia = model.P_billmedia.ToSafeString()

            };

            var result = _queryProcessor.Execute(query);
            return result;
        }

        #endregion

        #region Update Status

        private bool UpdateStatus(string user, string bulkNo, string orderNo
            , string statustype, string status, string error_msg)
        {
            var command = new UpdateBulkCorpCommand()
            {
                P_USER = user,
                P_BULK_NO = bulkNo,
                P_ORDER_NO = orderNo,
                P_STATUS_TYPE = statustype,
                P_STATUS = status,
                P_MSG_ERROR = error_msg

            };

            _UpdateBulkCorpCommand.Handle(command);

            string result = command.P_RETURN_MESSAGE;
            if (result == "Success")
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Send Email
        public void QueryDataToSendMail()
        {
            try
            {
                string strUser = "";
                string FromPass = "";// Fixed Code scan : string FromPassword = "";
                string Port = "";
                string Domaim = "";
                string IPMailServer = "";
                string strFrom = "";
                string strTo = "";
                string strCC = "";
                string strBCC = "";
                string strSubject = "";
                string strBody = "";

                GetEmailStatusModel result = new GetEmailStatusModel();
                var query = new GetBatchMailStatus() { };
                if (query != null)
                { }

                result = _queryProcessor.Execute(query);
                if (result != null)
                {
                    if (result.P_RETURN_CODE == "0")
                    {
                        foreach (var item in result.P_RES_DATA)
                        {
                            strTo = item.EMAIL;

                            var command = new BatchUpdateMailStatusCommand()
                            {
                                P_BULK_NO = item.BULK_NO
                            };

                            _UpdateStatusMailCommand.Handle(command);

                            string resultCode = command.P_RETURN_CODE;
                            if (resultCode == "0")
                            {
                                var objFrom = GetLovList("BATCH", "L_MAIL_FROM");
                                if (objFrom != null)
                                {
                                    if (objFrom.Count > 0)
                                    {
                                        strFrom = objFrom[0].LovValue1;
                                    }
                                }

                                var objSubj = GetLovList("BATCH", "L_MAIL_SUBJECT");
                                if (objSubj != null)
                                {
                                    if (objSubj.Count > 0)
                                    {
                                        strSubject = objSubj[0].LovValue1;
                                    }
                                }

                                var objBody = GetLovList("BATCH", "L_MAIL_BODY");
                                if (objBody != null)
                                {
                                    if (objBody.Count > 0)
                                    {
                                        strBody = objBody[0].LovValue1;
                                    }
                                }

                                // Mail server
                                var vMailServer = GetLovList("FBBDORM_CONSTANT", "").Where(p => p.Name.Contains("VAR") && p.LovValue5 == "FBBDORM010").ToList();
                                if (vMailServer != null)
                                {
                                    if (vMailServer.Count > 0)
                                    {
                                        foreach (var key in vMailServer)
                                        {
                                            if (key.Name.Trim() == "VAR_FROM_PASSWORD")
                                            {
                                                FromPass = key.LovValue1;
                                            }
                                            else if (key.Name.Trim() == "VAR_HOST")
                                            {
                                                IPMailServer = key.LovValue1;
                                            }
                                            else if (key.Name.Trim() == "VAR_PORT")
                                            {
                                                Port = key.LovValue1;
                                            }
                                            else if (key.Name.Trim() == "VAR_DOMAIN")
                                            {
                                                Domaim = key.LovValue1;
                                            }
                                        }
                                    }

                                    string[] sendResult = Sendmail("BulkBatch", "BATCH", strFrom, strTo, strCC,
                                                                   strBCC, strSubject, strBody, IPMailServer, FromPass, Port, Domaim);
                                    if (sendResult != null)
                                    {
                                        _logger.Info("Send mail not Success");
                                    }

                                }
                            }

                        }
                    }
                }



            }
            catch (Exception ex)
            {
                _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());

            }
        }

        public string[] Sendmail(string processname, string createuser, string sendfrom, string sendto, string sendcc, string sendbcc
        , string subject, string body, string ip_mail_server, string frompass, string port, string domain)
        {
            _logger.Info("Sending an Email.");

            string[] result = new string[2];

            StartWatching();
            try
            {
                var command = new SendMailBatchNotificationCommand
                {

                    ProcessName = processname,
                    CreateUser = createuser,
                    SendTo = sendto,
                    SendCC = sendcc,
                    SendBCC = sendbcc,
                    SendFrom = sendfrom,
                    Subject = subject,
                    Body = body,
                    // AttachFiles = attach,
                    FromPassword = frompass,
                    Port = port,
                    Domaim = domain,
                    IPMailServer = ip_mail_server
                };


                _sendMail.Handle(command);

                _logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                StopWatching("Sending an Email");

                if (command.ReturnMessage == "Success.")
                {
                    result[0] = "0";
                    result[1] = "";
                }
                else
                {
                    result[0] = "-1";
                    result[1] = command.ReturnMessage;
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage());

                StopWatching("Sending an Email");
                //throw ex;

            }

            return result;
        }

        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetLovList : " + ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        #endregion

        #region Update BA CA SA No
        public void SffReturn()
        {
            _logger.Info("Start SffReturn.");
            StartWatching();

            try
            {
                string sffUpdate;
                BatchBulkCorpSFFReturnModel result = new BatchBulkCorpSFFReturnModel();
                var detail = GetWfAndSffStatus();
                if (detail != null)
                {
                    foreach (var i in detail)
                    {
                        Console.WriteLine("Return SFF Order No(" + detail.Count.ToString() + ") :" + i.ORDER_NO);

                        if (i.STATUS_SEND_WORKFLOW == "Completed" && i.STATUS_SEND_SFF == "Completed")
                        {
                            if (string.IsNullOrEmpty(i.FIBRE_NET_ID))
                            {
                                var query = new GetBatchBulkCorpSFFReturn()
                                {
                                    P_BULK_NUMBER = i.ORDER_NO
                                };

                                result = _queryProcessor.Execute(query);
                                if (result != null)
                                {
                                    if (result.OUTPUT_return_code == "0")
                                    {
                                        if (result.P_OUTPUT_SFF_DETAIL != null && result.P_OUTPUT_SFF_DETAIL.Count > 0)
                                        {
                                            foreach (var item in result.P_OUTPUT_SFF_DETAIL)
                                            {
                                                sffUpdate = UpdateSffReturn(item);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string orderno = String.Format("{0}", i.ORDER_NO);
                                    _logger.Info("execute GetBatchBulkCorpSFFReturn error :" + result.OUTPUT_return_message + orderno);
                                }
                            }
                        }

                    }
                }

                StopWatching("end SffReturn");
            }
            catch (Exception ex)
            {
                _logger.Info("Error SffReturn : " + ex.GetErrorMessage());
                StopWatching("end SffReturn");

            }
        }

        public string UpdateSffReturn(DetailSffReturn model)
        {
            BatchBulkCorpUpdateSFFReturnModel result = new BatchBulkCorpUpdateSFFReturnModel();
            var query = new GetBatchBulkCorpUpdateSFFReturn()
            {
                P_ORDER_NO = model.P_ORDER_NO,
                P_BA_NO = model.P_BA_NO,
                P_CA_NO = model.P_CA_NO,
                P_SA_NO = model.P_SA_NO,
                P_MOBILE_NO = model.P_MOBILE_NO,
                P_USER = username,
                p_error_reason = model.p_error_reason,
                p_interface_result = model.p_interface_result

            };
            result = _queryProcessor.Execute(query);
            _logger.Info("execute UpdateSffReturn Return Code :" + result.P_RETURN_CODE);
            return result.P_RETURN_CODE;
        }

        public List<DetailWFAndSFFStatus> GetWfAndSffStatus()
        {
            GetWFAndSFFStatus result = new GetWFAndSFFStatus();
            var query = new GetBatchBulkCorpWfAndSffStatus() { };

            result = _queryProcessor.Execute(query);

            if (result == null)
                return null;

            return result.P_RES_DATA;
        }
        #endregion


    }
}
