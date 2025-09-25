using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WBBBusinessLayer.QueryHandlers;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;

namespace FBBFBSSToPAYG
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.WebServiceModels;
    //using CompositionRoot;
    using System.Net.Mail;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBContract.Queries.ExWebServices;
    using WBBEntity.PanelModels.ExWebServiceModels;
    using WBBContract.Queries.FBBWebConfigQueries;
    using System.Threading;
    public class FBBFBSSToPAYGBatchJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private string _outErrorResult = string.Empty;
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }
        public FBBFBSSToPAYGBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail,
               ICommandHandler<SendSmsCommand> SendSmsCommand
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendMail = sendMail;
            _sendSmsCommand = SendSmsCommand;
        }
        public string GenBodyEmailTable()
        {
            var list = GetDataFOALogError();
            string body = "พบ Error ในการตัด Stock จำนวน " + list.Count + " record";
            body += "<br/><br/>";
            body += "<table border='1px solid #ddd' width='100%' cellpadding='0' cellspacing='0'>";
            body += "<thead>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Internet Number</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Sub Contract Name</th>";
            //body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>mat Code</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>S/N</th>";
            //body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Sto Loc. FBSS</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:300px'>Error Message</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:300px'>Error Description</th>";
            body += "</thead>";
            body += "<tbody>";
            foreach (var item in list)
            {
                body += "<tr>";
                body += "<td style='vertical-align: top;'>" + item.ACCESS_NUMBER + "</td>";
                body += "<td style='vertical-align: top;'>" + item.SUBCONTRACT_NAME + "</td>";
                //body += "<td>"+item.MATERIAL_CODE+"</td>";
                body += "<td style='vertical-align: top;'>" + item.SN + "</td>";
                //body += "<td>"+item.STORAGE_LOCATION+"</td>";
                body += "<td style='vertical-align: top;'>" + item.ERR_MSG + "</td>";
                body += "<td style='vertical-align: top;'>" + item.ERR_DESC + "</td>";
                body += "</tr>";
            }
            body += "</tbody>";
            body += "</table>";
            return body;
        }
        public List<SubmitFOAEquipment> GetDataFOALogError()
        {
            SubmitFOASendmailDataQuery searchModel = new SubmitFOASendmailDataQuery();
            searchModel.dateFrom = DateTime.Now.ToString("dd/MM/yyyy");  //
            searchModel.dateTo = DateTime.Now.ToString("dd/MM/yyyy");
            searchModel.status = "ERROR";
            searchModel.serviceName = "";
            searchModel.companyCode = "ALL";
            searchModel.orderType = "ALL";
            searchModel.subcontractorCode = "ALL";
            searchModel.materialCode = "ALL";
            searchModel.plant = "ALL";
            searchModel.orderNo = "ALL";
            searchModel.productName = "ALL";
            searchModel.storLocation = "ALL";
            searchModel.internetNo = "ALL";
            var resultEquip = this.GetSubmitFOAEquipment(searchModel);

            //เรียงข้อมูลใหม่ ให้เป็น group// begin
            List<SubmitFOAEquipment> newResultList = new List<SubmitFOAEquipment>();
            foreach (var item in resultEquip)
            {
                SubmitFOAEquipment newResult = new SubmitFOAEquipment();
                SubmitFOAGetErrorDescQuery model = new SubmitFOAGetErrorDescQuery();
                model.P_LOCATION = item.STORAGE_LOCATION;
                model.P_MATERIAL = item.MATERIAL_CODE;
                model.P_PLANT = item.PLANT;
                model.P_SERIAL = item.SN;
                model.P_ERR_MSG = item.ERR_MSG;
                var desc = GetErrorDesc(model);
                item.ERR_DESC = desc.V_ERR_MSG;
                var checkList = newResultList.Any(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                    && x.SUBCONTRACT_CODE == item.SUBCONTRACT_CODE && x.SUBCONTRACT_NAME == item.SUBCONTRACT_NAME
                    && x.PRODUCT_NAME == item.PRODUCT_NAME && x.ORDER_TYPE == item.ORDER_TYPE && x.SUBMIT_DATE == item.SUBMIT_DATE
                    && x.STATUS == item.STATUS);

                if (!checkList)
                {
                    newResult.ACCESS_NUMBER = item.ACCESS_NUMBER;
                    newResult.ORDER_NO = item.ORDER_NO;
                    newResult.SUBCONTRACT_CODE = item.SUBCONTRACT_CODE;
                    newResult.SUBCONTRACT_NAME = item.SUBCONTRACT_NAME;
                    newResult.PRODUCT_NAME = item.PRODUCT_NAME;
                    newResult.ORDER_TYPE = item.ORDER_TYPE;
                    newResult.SUBMIT_DATE = item.SUBMIT_DATE;
                    newResult.SN = item.SN == "" || item.SN == null ? "&nbsp;" : item.SN;
                    newResult.MATERIAL_CODE = item.MATERIAL_CODE == "" || item.MATERIAL_CODE == null ? "&nbsp;" : item.MATERIAL_CODE;
                    newResult.COMPANY_CODE = item.COMPANY_CODE == "" || item.COMPANY_CODE == null ? "&nbsp;" : item.COMPANY_CODE;
                    newResult.PLANT = item.PLANT == "" || item.PLANT == null ? "&nbsp;" : item.PLANT;
                    newResult.STORAGE_LOCATION = item.STORAGE_LOCATION == "" || item.STORAGE_LOCATION == null ? "&nbsp;" : item.STORAGE_LOCATION;
                    newResult.SN_TYPE = item.SN_TYPE == "" || item.SN_TYPE == null ? "&nbsp;" : item.SN_TYPE;
                    newResult.MOVEMENT_TYPE = item.MOVEMENT_TYPE == "" || item.MOVEMENT_TYPE == null ? "&nbsp;" : item.MOVEMENT_TYPE;
                    newResult.ERR_CODE = item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE;
                    newResult.ERR_MSG = item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG;
                    newResult.MOVEMENT_TYPE_OUT = item.MOVEMENT_TYPE_OUT == "" || item.MOVEMENT_TYPE_OUT == null ? "&nbsp;" : item.MOVEMENT_TYPE_OUT;
                    newResult.MOVEMENT_TYPE_IN = item.MOVEMENT_TYPE_IN == "" || item.MOVEMENT_TYPE_IN == null ? "&nbsp;" : item.MOVEMENT_TYPE_IN;
                    newResult.STATUS = item.STATUS;
                    newResult.SERVICE_NAME = item.SERVICE_NAME;
                    newResult.REJECT_REASON = item.REJECT_REASON;
                    newResult.OLT_NAME = item.OLT_NAME;
                    newResult.BUILDING_NAME = item.BUILDING_NAME;
                    newResult.MOBILE_CONTACT = item.MOBILE_CONTACT;
                    newResult.ERR_DESC = item.ERR_DESC;
                    newResultList.Add(newResult);
                }
                else
                {
                    var whereResult = newResultList.First(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                    && x.SUBCONTRACT_CODE == item.SUBCONTRACT_CODE && x.SUBCONTRACT_NAME == item.SUBCONTRACT_NAME
                    && x.PRODUCT_NAME == item.PRODUCT_NAME && x.ORDER_TYPE == item.ORDER_TYPE && x.SUBMIT_DATE == item.SUBMIT_DATE
                    && x.STATUS == item.STATUS);

                    whereResult.SN = whereResult.SN + "<br/>" + (item.SN == "" || item.SN == null ? "&nbsp;" : item.SN);
                    whereResult.MATERIAL_CODE = whereResult.MATERIAL_CODE + "<br/>" + (item.MATERIAL_CODE == "" || item.MATERIAL_CODE == null ? "&nbsp;" : item.MATERIAL_CODE);
                    whereResult.COMPANY_CODE = whereResult.COMPANY_CODE + "<br/>" + (item.COMPANY_CODE == "" || item.COMPANY_CODE == null ? "&nbsp;" : item.COMPANY_CODE);
                    whereResult.PLANT = whereResult.PLANT + "<br/>" + (item.PLANT == "" || item.PLANT == null ? "&nbsp;" : item.PLANT);
                    whereResult.STORAGE_LOCATION = whereResult.STORAGE_LOCATION + "<br/>" + (item.STORAGE_LOCATION == "" || item.STORAGE_LOCATION == null ? "&nbsp;" : item.STORAGE_LOCATION);
                    whereResult.SN_TYPE = whereResult.SN_TYPE + "<br/>" + (item.SN_TYPE == "" || item.SN_TYPE == null ? "&nbsp;" : item.SN_TYPE);
                    whereResult.MOVEMENT_TYPE = whereResult.MOVEMENT_TYPE + "<br/>" + (item.MOVEMENT_TYPE == "" || item.MOVEMENT_TYPE == null ? "&nbsp;" : item.MOVEMENT_TYPE);
                    whereResult.ERR_CODE = whereResult.ERR_CODE + "<br/>" + (item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE);
                    whereResult.ERR_MSG = whereResult.ERR_MSG + "<br/>" + (item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG);
                    whereResult.SERVICE_NAME = whereResult.SERVICE_NAME + "<br/>" + (item.SERVICE_NAME == "" || item.SERVICE_NAME == null ? "&nbsp;" : item.SERVICE_NAME);
                    whereResult.REJECT_REASON = whereResult.REJECT_REASON + "<br/>" + (item.REJECT_REASON == "" || item.REJECT_REASON == null ? "&nbsp;" : item.REJECT_REASON);
                    whereResult.OLT_NAME = whereResult.OLT_NAME + "<br/>" + (item.OLT_NAME == "" || item.OLT_NAME == null ? "&nbsp;" : item.OLT_NAME);
                    whereResult.BUILDING_NAME = whereResult.BUILDING_NAME + "<br/>" + (item.BUILDING_NAME == "" || item.BUILDING_NAME == null ? "&nbsp;" : item.BUILDING_NAME);
                    whereResult.MOBILE_CONTACT = whereResult.MOBILE_CONTACT + "<br/>" + (item.MOBILE_CONTACT == "" || item.MOBILE_CONTACT == null ? "&nbsp;" : item.MOBILE_CONTACT);
                    whereResult.MOVEMENT_TYPE_OUT = whereResult.MOVEMENT_TYPE_OUT + "<br/>" + (item.MOVEMENT_TYPE_OUT == "" || item.MOVEMENT_TYPE_OUT == null ? "&nbsp;" : item.MOVEMENT_TYPE_OUT);
                    whereResult.MOVEMENT_TYPE_IN = whereResult.MOVEMENT_TYPE_IN + "<br/>" + (item.MOVEMENT_TYPE_IN == "" || item.MOVEMENT_TYPE_IN == null ? "&nbsp;" : item.MOVEMENT_TYPE_IN);
                    whereResult.ERR_DESC = whereResult.ERR_DESC + "<br/>" + (item.ERR_DESC == "" || item.ERR_DESC == null ? "&nbsp;" : item.ERR_DESC);
                }
            }

            return newResultList;
        }
        public List<SubmitFOAEquipment> GetSubmitFOAEquipment(SubmitFOASendmailDataQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                SendSms();
                _logger.Info("Error GetSubmitFOAEquipment : " + ex.GetErrorMessage());
                return new List<SubmitFOAEquipment>();
            }
        }
        public List<LovModel> GetConfig(GetFixedAssetConfigQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                SendSms();
                _logger.Info("Error GetConfig : " + ex.GetErrorMessage());
                return new List<LovModel>();
            }
        }
        public SubmitFOAGetErrorDesc GetErrorDesc(SubmitFOAGetErrorDescQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                SendSms();
                _logger.Info("Error GetErrorDesc : " + ex.GetErrorMessage());
                return new SubmitFOAGetErrorDesc();
            }
        }
        #region Send E-mail

        public void QueryDataToSendMail()
        {
            var list = GetDataFOALogError();
            //TODO: GET Subject and detail 
            string strFrom = "";
            string strTo = "";
            string strCC = "";
            string strBCC = "";
            string IPMailServer = "";
            string FromPassword = "";
            string Port = "";
            string Domaim = "";
            string strSubject = "Summary Error Message FOA [" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "]  จำนวน " + list.Count + " record";

            try
            {

                string strBody = "Dear All<br>" +
                   GenBodyEmailTable();

                string[] sendResult = Sendmail("SEND_EMAIL_FIXED_ASSET", "BATCH", strFrom, strTo, strCC,
                    strBCC, strSubject, strBody, IPMailServer, FromPassword, Port, Domaim);

                _logger.Info("SendEmail Success");
            }
            catch (Exception ex)
            {
                SendSms();
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
                    SendFrom = sendfrom,
                    Subject = subject,
                    Body = body,
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
                SendSms();
                _outErrorResult = "Sending an Email" + string.Format(" is error on execute : {0}.",
                                      ex.GetErrorMessage());
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage());

                StopWatching("Sending an Email");
            }

            return result;
        }

        #endregion

        //21.12.2017 
        public bool AutoResendOrder(string sendmail = "")
        {
            bool retCode = false;
            try
            {
                var  process= CheckProcessBatch();
                SubmitFOASendmailDataQuery searchModel = new SubmitFOASendmailDataQuery();
                searchModel.dateFrom = DateTime.Now.ToString("dd/MM/yyyy");  //
                searchModel.dateTo = DateTime.Now.ToString("dd/MM/yyyy");
                searchModel.status = "ERROR";
                searchModel.serviceName = "";
                searchModel.companyCode = "ALL";
                searchModel.orderType = "ALL";
                searchModel.subcontractorCode = "ALL";
                searchModel.materialCode = "ALL";
                searchModel.plant = "ALL";
                searchModel.orderNo = "ALL";
                searchModel.productName = "ALL";
                searchModel.storLocation = "ALL";
                searchModel.internetNo = "ALL";
                var result = this.GetSubmitFOAEquipment(searchModel);
                GetFixedAssetConfigQuery model = new GetFixedAssetConfigQuery();
                model.Program = "AUTO_RESEND";
                var configfbb = GetConfig(model).Where(x => x.LOV_NAME != "999").Select(x => x.LOV_NAME);
                var resend = result.Where(x => x.ERR_CODE.Split('|').ToList().Intersect(configfbb).Count() > 0 // configfbb.Contains(x.ERR_CODE)
               || (x.ERR_CODE == "999" && x.ERR_MSG == "Goods issue error in other line item.")).ToList();

                if (resend != null && resend.Count > 0)
                {
                    ResendToSelectSubmitFOA(resend);
                    retCode = true;
                    //resend
                }

                if (sendmail == "")
                {
                    var result_sendmail = result.Except(resend).ToList();
                    var setResultMail = MergeResult(result_sendmail);

                    //TODO: GET Subject and detail 
                    string strFrom = "";
                    string strTo = "";
                    string strCC = "";
                    string strBCC = "";
                    string IPMailServer = "";
                    string FromPassword = "";
                    string Port = "";
                    string Domaim = "";
                    string strSubject = "Summary Error Message FOA [" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "]  จำนวน " + setResultMail.Count + " record";

                    try
                    {

                        string strBody = "Dear All<br>" +
                           GenBodyEmailTableAutoResend(setResultMail);

                        string[] sendResult = Sendmail("SEND_EMAIL_FIXED_ASSET", "BATCH", strFrom, strTo, strCC,
                            strBCC, strSubject, strBody, IPMailServer, FromPassword, Port, Domaim);

                        _logger.Info("SendEmail Success");
                    }
                    catch (Exception ex)
                    {
                        SendSms();
                        _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());
                    }
                }
                return retCode;

            }
            catch (Exception ex)
            {
                SendSms();
                _logger.Error(ex.Message);
                return retCode;
            }
        }
        public List<SubmitFOAEquipment> MergeResult(List<SubmitFOAEquipment> resultEquip)
        {
            //เรียงข้อมูลใหม่ ให้เป็น group// begin
            List<SubmitFOAEquipment> newResultList = new List<SubmitFOAEquipment>();
            foreach (var item in resultEquip)
            {
                SubmitFOAEquipment newResult = new SubmitFOAEquipment();
                SubmitFOAGetErrorDescQuery model = new SubmitFOAGetErrorDescQuery();
                model.P_LOCATION = item.STORAGE_LOCATION;
                model.P_MATERIAL = item.MATERIAL_CODE;
                model.P_PLANT = item.PLANT;
                model.P_SERIAL = item.SN;
                model.P_ERR_MSG = item.ERR_MSG;
                var desc = GetErrorDesc(model);
                item.ERR_DESC = desc.V_ERR_MSG;
                var checkList = newResultList.Any(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                    && x.SUBCONTRACT_CODE == item.SUBCONTRACT_CODE && x.SUBCONTRACT_NAME == item.SUBCONTRACT_NAME
                    && x.PRODUCT_NAME == item.PRODUCT_NAME && x.ORDER_TYPE == item.ORDER_TYPE && x.SUBMIT_DATE == item.SUBMIT_DATE
                    && x.STATUS == item.STATUS);

                if (!checkList)
                {
                    newResult.ACCESS_NUMBER = item.ACCESS_NUMBER;
                    newResult.ORDER_NO = item.ORDER_NO;
                    newResult.SUBCONTRACT_CODE = item.SUBCONTRACT_CODE;
                    newResult.SUBCONTRACT_NAME = item.SUBCONTRACT_NAME;
                    newResult.PRODUCT_NAME = item.PRODUCT_NAME;
                    newResult.ORDER_TYPE = item.ORDER_TYPE;
                    newResult.SUBMIT_DATE = item.SUBMIT_DATE;
                    newResult.SN = item.SN == "" || item.SN == null ? "&nbsp;" : item.SN;
                    newResult.MATERIAL_CODE = item.MATERIAL_CODE == "" || item.MATERIAL_CODE == null ? "&nbsp;" : item.MATERIAL_CODE;
                    newResult.COMPANY_CODE = item.COMPANY_CODE == "" || item.COMPANY_CODE == null ? "&nbsp;" : item.COMPANY_CODE;
                    newResult.PLANT = item.PLANT == "" || item.PLANT == null ? "&nbsp;" : item.PLANT;
                    newResult.STORAGE_LOCATION = item.STORAGE_LOCATION == "" || item.STORAGE_LOCATION == null ? "&nbsp;" : item.STORAGE_LOCATION;
                    newResult.SN_TYPE = item.SN_TYPE == "" || item.SN_TYPE == null ? "&nbsp;" : item.SN_TYPE;
                    newResult.MOVEMENT_TYPE = item.MOVEMENT_TYPE == "" || item.MOVEMENT_TYPE == null ? "&nbsp;" : item.MOVEMENT_TYPE;
                    newResult.ERR_CODE = item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE;
                    newResult.ERR_MSG = item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG;
                    newResult.MOVEMENT_TYPE_OUT = item.MOVEMENT_TYPE_OUT == "" || item.MOVEMENT_TYPE_OUT == null ? "&nbsp;" : item.MOVEMENT_TYPE_OUT;
                    newResult.MOVEMENT_TYPE_IN = item.MOVEMENT_TYPE_IN == "" || item.MOVEMENT_TYPE_IN == null ? "&nbsp;" : item.MOVEMENT_TYPE_IN;
                    newResult.STATUS = item.STATUS;
                    newResult.SERVICE_NAME = item.SERVICE_NAME;
                    newResult.REJECT_REASON = item.REJECT_REASON;
                    newResult.OLT_NAME = item.OLT_NAME;
                    newResult.BUILDING_NAME = item.BUILDING_NAME;
                    newResult.MOBILE_CONTACT = item.MOBILE_CONTACT;
                    newResult.ERR_DESC = item.ERR_DESC;
                    newResultList.Add(newResult);
                }
                else
                {
                    var whereResult = newResultList.First(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                    && x.SUBCONTRACT_CODE == item.SUBCONTRACT_CODE && x.SUBCONTRACT_NAME == item.SUBCONTRACT_NAME
                    && x.PRODUCT_NAME == item.PRODUCT_NAME && x.ORDER_TYPE == item.ORDER_TYPE && x.SUBMIT_DATE == item.SUBMIT_DATE
                    && x.STATUS == item.STATUS);

                    whereResult.SN = whereResult.SN + "<br/>" + (item.SN == "" || item.SN == null ? "&nbsp;" : item.SN);
                    whereResult.MATERIAL_CODE = whereResult.MATERIAL_CODE + "<br/>" + (item.MATERIAL_CODE == "" || item.MATERIAL_CODE == null ? "&nbsp;" : item.MATERIAL_CODE);
                    whereResult.COMPANY_CODE = whereResult.COMPANY_CODE + "<br/>" + (item.COMPANY_CODE == "" || item.COMPANY_CODE == null ? "&nbsp;" : item.COMPANY_CODE);
                    whereResult.PLANT = whereResult.PLANT + "<br/>" + (item.PLANT == "" || item.PLANT == null ? "&nbsp;" : item.PLANT);
                    whereResult.STORAGE_LOCATION = whereResult.STORAGE_LOCATION + "<br/>" + (item.STORAGE_LOCATION == "" || item.STORAGE_LOCATION == null ? "&nbsp;" : item.STORAGE_LOCATION);
                    whereResult.SN_TYPE = whereResult.SN_TYPE + "<br/>" + (item.SN_TYPE == "" || item.SN_TYPE == null ? "&nbsp;" : item.SN_TYPE);
                    whereResult.MOVEMENT_TYPE = whereResult.MOVEMENT_TYPE + "<br/>" + (item.MOVEMENT_TYPE == "" || item.MOVEMENT_TYPE == null ? "&nbsp;" : item.MOVEMENT_TYPE);
                    whereResult.ERR_CODE = whereResult.ERR_CODE + "<br/>" + (item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE);
                    whereResult.ERR_MSG = whereResult.ERR_MSG + "<br/>" + (item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG);
                    whereResult.SERVICE_NAME = whereResult.SERVICE_NAME + "<br/>" + (item.SERVICE_NAME == "" || item.SERVICE_NAME == null ? "&nbsp;" : item.SERVICE_NAME);
                    whereResult.REJECT_REASON = whereResult.REJECT_REASON + "<br/>" + (item.REJECT_REASON == "" || item.REJECT_REASON == null ? "&nbsp;" : item.REJECT_REASON);
                    whereResult.OLT_NAME = whereResult.OLT_NAME + "<br/>" + (item.OLT_NAME == "" || item.OLT_NAME == null ? "&nbsp;" : item.OLT_NAME);
                    whereResult.BUILDING_NAME = whereResult.BUILDING_NAME + "<br/>" + (item.BUILDING_NAME == "" || item.BUILDING_NAME == null ? "&nbsp;" : item.BUILDING_NAME);
                    whereResult.MOBILE_CONTACT = whereResult.MOBILE_CONTACT + "<br/>" + (item.MOBILE_CONTACT == "" || item.MOBILE_CONTACT == null ? "&nbsp;" : item.MOBILE_CONTACT);
                    whereResult.MOVEMENT_TYPE_OUT = whereResult.MOVEMENT_TYPE_OUT + "<br/>" + (item.MOVEMENT_TYPE_OUT == "" || item.MOVEMENT_TYPE_OUT == null ? "&nbsp;" : item.MOVEMENT_TYPE_OUT);
                    whereResult.MOVEMENT_TYPE_IN = whereResult.MOVEMENT_TYPE_IN + "<br/>" + (item.MOVEMENT_TYPE_IN == "" || item.MOVEMENT_TYPE_IN == null ? "&nbsp;" : item.MOVEMENT_TYPE_IN);
                    whereResult.ERR_DESC = whereResult.ERR_DESC + "<br/>" + (item.ERR_DESC == "" || item.ERR_DESC == null ? "&nbsp;" : item.ERR_DESC);
                }
            }

            return newResultList;
        }
        public string ResendToSelectSubmitFOA(List<SubmitFOAEquipment> model)
        {

            var groupEquipmentModel = GroupEquipment(model);

            string msg = "";
            int indexSuccess = 0;
            int indexError = 0;
            string orderno = "", accessnum = "";
            if (groupEquipmentModel.Any())
            {
                var _mainList = new List<NewRegistForSubmitFOAQuery>();
                foreach (var resultlist in groupEquipmentModel)
                {
                    // 2.GetProductList
                    var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                    var resultProductList = _queryProcessor.Execute(queryProductList);

                    var _main = new NewRegistForSubmitFOAQuery();
                    var _product = new List<NewRegistFOAProductList>();

                    // 3.ResendSubmitFOA
                    //if (resultlist.STATUS == "Pending")
                    //{
                    if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                    {
                        _main = new NewRegistForSubmitFOAQuery()
                        {
                            Access_No = resultProductList.AccessNo.ToSafeString(),
                            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                            SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                            ProductName = resultProductList.ProductName.ToSafeString(),
                            OrderType = resultProductList.OrderType.ToSafeString(),
                            SubmitFlag = "BATCH_SERVER_ERROR",
                            RejectReason = resultProductList.RejectReason.ToSafeString(),
                            FOA_Submit_date = resultProductList.FOA_Submit_date.ToSafeString(),
                            OLT_NAME = resultProductList.OLT_NAME.ToSafeString(),
                            BUILDING_NAME = resultProductList.BUILDING_NAME.ToSafeString(),
                            Mobile_Contact = resultProductList.Mobile_Contact.ToSafeString(),

                            Post_Date = DateTime.Now.ToString("dd/MM/yyyy"),
                            Address_ID = resultProductList.ADDRESS_ID.ToSafeString(),
                            ORG_ID = resultProductList.ORG_ID.ToSafeString(),
                            Reuse_Flag = resultProductList.REUSE_FLAG.ToSafeString(),
                            Event_Flow_Flag = resultProductList.EVENT_FLOW_FLAG.ToSafeString(),

                            Subcontract_Type = resultProductList.SUBCONTRACT_TYPE.ToSafeString(),
                            Subcontract_Sub_Type = resultProductList.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                            Request_Sub_Flag = resultProductList.REQUEST_SUB_FLAG.ToSafeString(),
                            Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString(),
                            //18.07.02
                            UserName = "FBBSubmitFOALogSendEmailBatch"
                            //
                        };

                        _product = resultProductList.ProductList.Select(p =>
                        {
                            return new NewRegistFOAProductList()
                            {
                                SerialNumber = p.SerialNumber.ToSafeString(),
                                MaterialCode = p.MaterialCode.ToSafeString(),
                                CompanyCode = p.CompanyCode.ToSafeString(),
                                Plant = p.Plant.ToSafeString(),
                                StorageLocation = p.StorageLocation.ToSafeString(),
                                SNPattern = p.SNPattern.ToSafeString(),
                                MovementType = p.MovementType.ToSafeString()
                            };
                        }).ToList();
                        //}

                        _main.ProductList = _product;

                        var _services = new NewRegistFOAServiceList()
                        {
                            ServiceName = resultProductList.ServiceName != null ? resultProductList.ServiceName : ""
                        };

                        List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                        var subStr = _services.ServiceName.Split(',');
                        foreach (var service in subStr)
                        {
                            _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                        }
                        _main.ServiceList = _service;

                        orderno = resultlist.ORDER_NO;
                        accessnum = resultlist.ACCESS_NUMBER;

                        NewRegistForSubmitFOAResponse resultNewRegist = _queryProcessor.Execute(_main);
                        if (resultNewRegist.result != "") indexSuccess += 1;
                        else indexError += 1;

                        _logger.Info("Access No: " + _main.Access_No);
                    }
                }
                #region comment 30042019
                //foreach (var resultlist in groupEquipmentModel)
                //{
                //    // 2.GetProductList
                //    var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                //    var resultProductList = _queryProcessor.Execute(queryProductList);


                //    var _main = new NewRegistForSubmitFOAQuery();
                //    var _product = new List<NewRegistFOAProductList>();

                //    // 3.ResendSubmitFOA (STATUS = ERROR Only)
                //    if (resultlist.STATUS == "ERROR")
                //    {
                //        // Set _main
                //        _main = new NewRegistForSubmitFOAQuery()
                //        {
                //            Access_No = resultProductList.AccessNo.ToSafeString(),
                //            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                //            SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                //            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                //            ProductName = resultProductList.ProductName.ToSafeString(),
                //            OrderType = resultProductList.OrderType.ToSafeString(),
                //            SubmitFlag = "RESEND",
                //            RejectReason = resultProductList.RejectReason.ToSafeString(),
                //            FOA_Submit_date = resultProductList.FOA_Submit_date.ToSafeString(),
                //            OLT_NAME = resultProductList.OLT_NAME.ToSafeString(),
                //            BUILDING_NAME = resultProductList.BUILDING_NAME.ToSafeString(),
                //            Mobile_Contact = resultProductList.Mobile_Contact.ToSafeString(),

                //            Post_Date = null,
                //            Address_ID = resultProductList.ADDRESS_ID.ToSafeString(),
                //            ORG_ID = resultProductList.ORG_ID.ToSafeString(),
                //            Reuse_Flag = resultProductList.REUSE_FLAG.ToSafeString(),
                //            Event_Flow_Flag = resultProductList.EVENT_FLOW_FLAG.ToSafeString(),

                //            //18.07.02
                //            UserName = "FBBSubmitFOALogSendEmailBatch"
                //            //
                //        };


                //        // Set _product
                //        foreach (var equipmentlist in resultProductList.ProductList)
                //        {
                //            var equipmentListFormView = new List<SubmitFOAEquipment>();

                //            try
                //            {
                //                // Get Equipment from View Edit
                //                equipmentListFormView = resultlist.ListEquipment.Where(x => x.SN == equipmentlist.SerialNumber).Select(p =>
                //                {
                //                    return new SubmitFOAEquipment()
                //                    {
                //                        SN = p.SN.ToSafeString(),
                //                        MATERIAL_CODE = p.MATERIAL_CODE.ToSafeString(),
                //                        COMPANY_CODE = p.COMPANY_CODE.ToSafeString(),
                //                        PLANT = p.PLANT.ToSafeString(),
                //                        STORAGE_LOCATION = p.STORAGE_LOCATION.ToSafeString(),
                //                        SN_TYPE = p.SN_TYPE.ToSafeString(),
                //                        MOVEMENT_TYPE = p.MOVEMENT_TYPE.ToSafeString()
                //                    };
                //                }).ToList();
                //            }
                //            catch { SendSms(); }

                //            if (equipmentListFormView.Any())
                //            {
                //                // Assign Value from View Edit
                //                var equipment = equipmentListFormView.Select(e =>
                //                {
                //                    return new NewRegistFOAProductList()
                //                    {
                //                        SerialNumber = e.SN,
                //                        MaterialCode = e.MATERIAL_CODE,
                //                        CompanyCode = e.COMPANY_CODE,
                //                        Plant = e.PLANT,
                //                        StorageLocation = e.STORAGE_LOCATION,
                //                        SNPattern = e.SN_TYPE,
                //                        MovementType = e.MOVEMENT_TYPE
                //                    };
                //                }).First();

                //                _product.Add(new NewRegistFOAProductList()
                //                {
                //                    SerialNumber = equipment.SerialNumber,
                //                    MaterialCode = equipment.MaterialCode,
                //                    CompanyCode = equipment.CompanyCode,
                //                    Plant = equipment.Plant,
                //                    StorageLocation = equipment.StorageLocation,
                //                    SNPattern = equipment.SNPattern,
                //                    MovementType = equipment.MovementType
                //                });
                //            }
                //            else
                //            {
                //                _product.Add(new NewRegistFOAProductList()
                //                {
                //                    SerialNumber = equipmentlist.SerialNumber,
                //                    MaterialCode = equipmentlist.MaterialCode,
                //                    CompanyCode = equipmentlist.CompanyCode,
                //                    Plant = equipmentlist.Plant,
                //                    StorageLocation = equipmentlist.StorageLocation,
                //                    SNPattern = equipmentlist.SNPattern,
                //                    MovementType = equipmentlist.MovementType
                //                });
                //            }
                //        }

                //        _main.ProductList = _product;

                //        var _services = new NewRegistFOAServiceList()
                //        {
                //            ServiceName = resultProductList.ServiceName != null ? resultProductList.ServiceName : ""
                //        };

                //        List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                //        var subStr = _services.ServiceName.Split(',');
                //        foreach (var service in subStr)
                //        {
                //            _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                //        }
                //        _main.ServiceList = _service;

                //        NewRegistForSubmitFOAResponse resultNewRegist = new NewRegistForSubmitFOAResponse();
                //        resultNewRegist = _queryProcessor.Execute(_main);
                //        //if (resultNewRegist.result == "0" || resultNewRegist.result == "000" || resultNewRegist.result == "") indexSuccess += 1;
                //        //else indexError += 1;
                //        if (resultNewRegist.result != "") indexSuccess += 1;
                //        else indexError += 1;
                //    }

                //}
                #endregion comment 30042019
            }

            if (indexSuccess > 0 || indexError > 0)
            {
                msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
            }

            return msg;
        }

        public List<SubmitFOAEquipmentReport> GroupEquipment(List<SubmitFOAEquipment> dataS)
        {
            var result = new List<SubmitFOAEquipmentReport>();


            string tmpAccessNo = "";

            if (dataS.Count > 0)
            {
                foreach (var item in dataS)
                {
                    var EquipList = new SubmitFOAEquipmentReport();
                    try
                    {
                        // Group By => Show Multi
                        if (item.ACCESS_NUMBER != tmpAccessNo)
                        {
                            tmpAccessNo = item.ACCESS_NUMBER;

                            var SubmitFOAEquipmentData = dataS.Where(p => p.ACCESS_NUMBER == item.ACCESS_NUMBER && p.ORDER_NO == item.ORDER_NO)
                                                               .Select(p =>
                                                               {
                                                                   return new SubmitFOAEquipment
                                                                   {
                                                                       ACCESS_NUMBER = item.ACCESS_NUMBER,
                                                                       ORDER_NO = item.ORDER_NO,
                                                                       ORDER_TYPE = item.ORDER_TYPE,
                                                                       SUBCONTRACT_CODE = item.SUBCONTRACT_CODE,
                                                                       SUBCONTRACT_NAME = item.SUBCONTRACT_NAME,
                                                                       PRODUCT_NAME = item.PRODUCT_NAME,
                                                                       SERVICE_NAME = item.SERVICE_NAME,
                                                                       SUBMIT_FLAG = item.SUBMIT_FLAG,
                                                                       REJECT_REASON = item.REJECT_REASON,
                                                                       SUBMIT_DATE = item.SUBMIT_DATE,
                                                                       OLT_NAME = item.OLT_NAME,
                                                                       BUILDING_NAME = item.BUILDING_NAME,
                                                                       MOBILE_CONTACT = item.MATERIAL_CODE,
                                                                       SN = item.SN,
                                                                       MATERIAL_CODE = item.MATERIAL_CODE,
                                                                       COMPANY_CODE = item.COMPANY_CODE,
                                                                       PLANT = item.PLANT,
                                                                       STORAGE_LOCATION = item.STORAGE_LOCATION,
                                                                       SN_TYPE = item.SN_TYPE,
                                                                       MOVEMENT_TYPE = item.MOVEMENT_TYPE,
                                                                       MOVEMENT_TYPE_OUT = item.MOVEMENT_TYPE_OUT,
                                                                       MOVEMENT_TYPE_IN = item.MOVEMENT_TYPE_IN,
                                                                       STATUS = item.STATUS,
                                                                       ERR_CODE = item.ERR_CODE,
                                                                       ERR_MSG = item.ERR_MSG
                                                                   };
                                                               }).ToList();

                            EquipList.ACCESS_NUMBER = item.ACCESS_NUMBER;
                            EquipList.ORDER_NO = item.ORDER_NO;
                            EquipList.ORDER_TYPE = item.ORDER_TYPE;
                            EquipList.SUBCONTRACT_CODE = item.SUBCONTRACT_CODE;
                            EquipList.SUBCONTRACT_NAME = item.SUBCONTRACT_NAME;
                            EquipList.PRODUCT_NAME = item.PRODUCT_NAME;
                            EquipList.SERVICE_NAME = item.SERVICE_NAME;
                            EquipList.SUBMIT_FLAG = item.SUBMIT_FLAG;
                            EquipList.REJECT_REASON = item.REJECT_REASON;
                            EquipList.SUBMIT_DATE = item.SUBMIT_DATE;
                            EquipList.OLT_NAME = item.OLT_NAME;
                            EquipList.BUILDING_NAME = item.BUILDING_NAME;
                            EquipList.MOBILE_CONTACT = item.MATERIAL_CODE;
                            EquipList.SN = item.SN;
                            EquipList.MATERIAL_CODE = item.MATERIAL_CODE;
                            EquipList.COMPANY_CODE = item.COMPANY_CODE;
                            EquipList.PLANT = item.PLANT;
                            EquipList.STORAGE_LOCATION = item.STORAGE_LOCATION;
                            EquipList.SN_TYPE = item.SN_TYPE;
                            EquipList.MOVEMENT_TYPE = item.MOVEMENT_TYPE;
                            EquipList.STATUS = item.STATUS;
                            EquipList.ERR_CODE = item.ERR_CODE;
                            EquipList.ERR_MSG = item.ERR_MSG;
                            EquipList.ListEquipment = SubmitFOAEquipmentData;
                            EquipList.MOVEMENT_TYPE_OUT = item.MOVEMENT_TYPE_OUT;
                            EquipList.MOVEMENT_TYPE_IN = item.MOVEMENT_TYPE_IN;
                            result.Add(EquipList);
                        }
                    }
                    catch
                    {
                        SendSms();
                    }
                }
            }

            return result;
        }

        public string GenBodyEmailTableAutoResend(List<SubmitFOAEquipment> list)
        {
            string body = "พบ Error ในการตัด Stock จำนวน " + list.Count + " record";
            body += "<br/><br/>";
            body += "<table border='1px solid #ddd' width='100%' cellpadding='0' cellspacing='0'>";
            body += "<thead>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Internet Number</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Sub Contract Name</th>";
            //body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>mat Code</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>S/N</th>";
            //body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Sto Loc. FBSS</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:300px'>Error Message</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:300px'>Error Description</th>";
            body += "</thead>";
            body += "<tbody>";
            foreach (var item in list)
            {
                body += "<tr>";
                body += "<td style='vertical-align: top;'>" + item.ACCESS_NUMBER + "</td>";
                body += "<td style='vertical-align: top;'>" + item.SUBCONTRACT_NAME + "</td>";
                //body += "<td>"+item.MATERIAL_CODE+"</td>";
                body += "<td style='vertical-align: top;'>" + item.SN + "</td>";
                //body += "<td>"+item.STORAGE_LOCATION+"</td>";
                body += "<td style='vertical-align: top;'>" + item.ERR_MSG + "</td>";
                body += "<td style='vertical-align: top;'>" + item.ERR_DESC + "</td>";
                body += "</tr>";
            }
            body += "</tbody>";
            body += "</table>";
            return body;
        }
        public bool CheckProcessBatch()
        {
            bool result = false;
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAIL_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                _logger.Info("PROGRAM_PROCESS: " + program_process.DISPLAY_VAL);
                if (program_process.DISPLAY_VAL == "Y")
                {
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckProcessBatch : " + ex.Message);
                return result;
            }
            
        }
        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }
        public void SendSms()
        {
            var getMobile = Get_FBSS_CONFIG_TBL_LOV("FBB_MOBILE_ERROR_BATCH", "MOBILE_SMS").FirstOrDefault();
            if (getMobile != null)
            {
                if (!string.IsNullOrEmpty(getMobile.VAL1) && getMobile.DISPLAY_VAL == "Y")
                {
                    var mobile = getMobile.VAL1.Split(',');

                    foreach (var item in mobile)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var command = new SendSmsCommand();
                            command.FullUrl = "FBBSubmitFOALogSendMail";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBSubmitFOALogSendMail Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
