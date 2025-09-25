using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    //public class AddTopupViewModel
    //{
    //    public string[] promotion_code { get; set; }
    //}

    public partial class ProcessController : WBBController
    {
        private Object thisLock = new Object();
        public ActionResult Submit(string ProSubType = "")
        {
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                ViewBag.LanguagePage = "1";
            }
            else
            {
                ViewBag.LanguagePage = "2";
            }

            var data = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"));
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LCLOSE = data.FirstOrDefault().LovValue1; }
            else
            { ViewBag.LCLOSE = data.FirstOrDefault().LovValue2; }

            data = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_SAVE"));
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LSAVE = data.FirstOrDefault().LovValue1; }
            else
            { ViewBag.LSAVE = data.FirstOrDefault().LovValue2; }

            data = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("L_POPUP_SAVE"));
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(data.FirstOrDefault().LovValue1); }
            else
            { ViewBag.LPOPUPSAVE = HttpUtility.HtmlEncode(data.FirstOrDefault().LovValue2); }

            ViewBag.ProSubType = ProSubType;
            return View();
        }

        public JsonResult SendSMS(string mobileNo, string mainCode, string ontopCode, string AisAirNumber, string Entrancefee)
        {
            string sessionId = "";
            try
            {
                sessionId = System.Web.HttpContext.Current.Session.SessionID;
                if(!LimitSend(sessionId))
                {
                    return Json(
                       new
                       {
                           data = new
                           {
                               return_status = "Beyond the limit"
                           }
                       }, JsonRequestBehavior.AllowGet);
                }
            }
            catch { }           

            string FullUrl = "";
            if (Session["FullUrl"] != null)
            {
                FullUrl = Session["FullUrl"].ToSafeString();
            }

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = AisAirNumber + ipAddress;

            #endregion

            string strOntopCode = "";
            string strMainCode = "";
            if (!string.IsNullOrEmpty(ontopCode))
            {
                strOntopCode = ontopCode;
            }
            if (!string.IsNullOrEmpty(mainCode))
            {
                strMainCode = mainCode;
            }
            lock (thisLock)
            {
                string msgtxt1 = "";
                string msgtxt2 = "";
                string msgtxt3 = "";
                string msgtxt4 = "";
                string[] msgtxt;

                string lov_name_sms_message = "SMS_MESSAGE";
                var model = new QuickWinPanelModel();
                if (Session["OfficerModel"] != null)
                {
                    var officerModel = Session["OfficerModel"] as QuickWinPanelModel;
                    model.TopUp = "5";
                    model.SummaryPanelModel.TOPUP = "5";
                    model.CustomerRegisterPanelModel.L_ASC_CODE = officerModel.CustomerRegisterPanelModel.L_ASC_CODE;
                    model.CustomerRegisterPanelModel.L_LOC_CODE = officerModel.CustomerRegisterPanelModel.L_LOC_CODE;
                    model.CustomerRegisterPanelModel.outType = officerModel.CustomerRegisterPanelModel.outType;
                    model.CustomerRegisterPanelModel.outSubType = officerModel.CustomerRegisterPanelModel.outSubType;
                    model.PlugAndPlayFlow = officerModel.PlugAndPlayFlow;
                }

                if (model.PlugAndPlayFlow == "Y") lov_name_sms_message = "SMS_MESSAGE_PLUG";

                var data = base.LovData
                    .Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals(lov_name_sms_message))
                    .FirstOrDefault();

                var smsEntryFee = base.LovData
                    .Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("SMS_ENTRY_FEE"))
                    .FirstOrDefault();

                if (data != null)
                {
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        msgtxt1 = data.LovValue1;
                        msgtxt2 = data.LovValue3;
                        if (Entrancefee != "" && Entrancefee.ToSafeInteger() > 0)
                            msgtxt2 = msgtxt2.Replace("{EntryFee} ", smsEntryFee.LovValue1.Replace("{Value} ", Entrancefee));
                        else
                            msgtxt2 = msgtxt2.Replace("{EntryFee} ", "");

                        msgtxt = msgtxt2.Split(',');
                        if (!String.IsNullOrEmpty(msgtxt[0]))
                        {
                            msgtxt3 = msgtxt[0].ToSafeString();
                        }
                        if (!String.IsNullOrEmpty(msgtxt[1]))
                        {
                            msgtxt4 = msgtxt[1].ToSafeString();
                        }
                    }
                    else
                    {
                        msgtxt1 = data.LovValue2;
                        msgtxt3 = data.LovValue4;
                        msgtxt4 = data.LovValue5;
                        if (Entrancefee != "" && Entrancefee.ToSafeInteger() > 0)
                            msgtxt4 = msgtxt4.Replace("{EntryFee} ", smsEntryFee.LovValue2.Replace("{Value} ", Entrancefee));
                        else
                            msgtxt4 = msgtxt4.Replace("{EntryFee} ", "");
                    }

                    if (strMainCode != "")
                    {
                        msgtxt1 = msgtxt1.Replace("{Package}", strMainCode);
                    }
                    else
                    {
                        msgtxt1 = msgtxt1.Replace("{Package}", "");
                    }

                    if (strOntopCode != "")
                    {
                        msgtxt1 = msgtxt1.Replace("{Special Offer}", strOntopCode);
                    }
                    else
                    {
                        msgtxt1 = msgtxt1.Replace("{Special Offer}", "");
                    }
                }

                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                var command = new SendSmsCommand();
                command.FullUrl = FullUrl;
                command.Source_Addr = "AISFIBRE";
                command.Destination_Addr = mobileNo;
                // Update 17.2
                command.Transaction_Id = transactionId;
                command.Message_Text = msgtxt1;
                _sendSmsCommand.Handle(command);

                for (int i = 0; i <= 1; i++)
                {
                    switch (i)
                    {
                        case 0:
                            command.Message_Text = msgtxt3;
                            break;
                        case 1:
                            command.Message_Text = msgtxt4;
                            break;
                    }

                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _sendSmsCommand.Handle(command);
                    }

                    command.Message_Text = "";


                }

                return Json(
                       new
                       {
                           data = new
                           {
                               return_status = command.return_status
                           }
                       }, JsonRequestBehavior.AllowGet);
            }

        }

        private bool LimitSend(string sessionId = "")
        {
            bool isLimitSend = false;
            int sessionIdCount = 0;
            try
            {
                if (sessionId != "")
                {
                    if (Session["LimitSend" + sessionId] == null)
                    {
                        Session["LimitSend" + sessionId] = 1;
                        sessionIdCount = 1;
                    }
                    else
                    {
                        sessionIdCount = (int)Session["LimitSend" + sessionId];
                        Session["LimitSend" + sessionId] = sessionIdCount + 1;
                    }

                    if (sessionIdCount <= 3)
                    {
                        isLimitSend = true;
                    }
                }
            }
            catch
            { }
            return isLimitSend;
        }

        public string CheckShowMenuChangeService(string InternetNo = "", string IDCardNo = "", string FullUrl = "")
        {
            List<string> listErrorCode = new List<string>();
            listErrorCode.Add("005");
            listErrorCode.Add("006");
            listErrorCode.Add("007");
            listErrorCode.Add("008");
            listErrorCode.Add("013");
            listErrorCode.Add("014");
            listErrorCode.Add("015");
            listErrorCode.Add("016");
            //R24.01 Fix IP Camera AR order by Max kunlp885
            /*for (int i = 3; i < 17; i++)
            {
                listErrorCode.Add(i.ToString("D3"));
            }*/
            string result = "N";
            evOMServiceCheckChangeServiceQuery query = new evOMServiceCheckChangeServiceQuery()
            {
                InternetNo = InternetNo,
                IDCardNo = IDCardNo,
                TransactionID = InternetNo,
                FullUrl = FullUrl
            };
            var queryData = _queryProcessor.Execute(query);
            if (queryData != null)
            {
                if (listErrorCode.IndexOf(queryData.ReturnCode) == -1)
                {
                    result = "Y|";
                }
                else
                {
                    result = "N|" + queryData.ReturnMessage.ToSafeString();
                }
            }
            else
            {
                result += "|No Data.";
            }
            return result;
        }

        public string CheckPendingOrder(string AisNonMobile = "", string RegisterType = "", string FullUrl = "")
        {
            if (FullUrl == "")
            {
                if (Session["FullUrl"] != null)
                    FullUrl = Session["FullUrl"].ToSafeString();
            }
            string result = "Y";
            CheckPendingOrderQuery query = new CheckPendingOrderQuery()
            {
                AisNonMobile = AisNonMobile,
                RegisterType = RegisterType,
                FullUrl = FullUrl
            };
            var queryData = _queryProcessor.Execute(query);
            if (queryData != null)
            {
                result = "N";
                if (queryData.IsPendingOrder)
                {
                    result = "Y";
                }
            }
            return result;
        }

        private decimal InsertMailLog(string customerId)
        {
            var command = new MailLogCommand
            {
                CustomerId = customerId,
            };

            _mailLogCommand.Handle(command);

            return command.RunningNo;
        }

        private void SendEmail(string custRowId, decimal runningNo, string mailTo, string filePath, string filePathApp)
        {

            var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT_WEB" && l.Name == "Impersonate_App").SingleOrDefault();
            var imagepathimer = @ImpersonateVar.LovValue4;
            string user = ImpersonateVar.LovValue1;
            string pass = ImpersonateVar.LovValue2;
            string ip = ImpersonateVar.LovValue3;
            //14.05.23 e-app first
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(filePathApp))
            {

                //string filePathAppNASTemp = filePath.Substring(2);
                //string filePathAppNAS = "\\\\" + ip + filePathAppNASTemp.Replace(filePathAppNASTemp.Split('\\')[0], "");
                //string filePathAppNASTemp2 = filePathApp.Substring(2);
                //string filePathAppNAS2 = "\\\\" + ip + filePathAppNASTemp2.Replace(filePathAppNASTemp2.Split('\\')[0], "");

                string filePathAppNAS = ConvertAdressPath(filePath, ip);
                string filePathAppNAS2 = ConvertAdressPath(filePathApp, ip);

                var command = new NotificationCommand
                {
                    CustomerId = custRowId,
                    CurrentCulture = SiteSession.CurrentUICulture,
                    RunningNo = runningNo,
                    EmailModel = new EmailModel
                    {
                        MailTo = mailTo,
                        FilePath = filePathAppNAS,
                        FilePath2 = filePathAppNAS2,
                    },
                    ImpersonateUser = user,
                    ImpersonatePass = pass,
                    ImpersonateIP = ip
                };

                SendEmailHandler(command);
            }
        }
        private string ConvertAdressPath(string path, string ip)
        {
            string lowerChar = path.ToLower();
            Match firstMatch = Regex.Match(lowerChar, "[a-z]");
            string firstCharevter = "";
            if (firstMatch.Success)
                firstCharevter = firstMatch.Value;
            else
                throw new Exception("No Path character from a-z found.");

            int indexStart = lowerChar.IndexOf(firstCharevter);
            int indexEnd = path.Length - indexStart;
            string pathSubtring = path.Substring(indexStart, indexEnd);
            string backslash = "\\\\";
            string text = Path.Combine(backslash, ip, pathSubtring);
            string convertPath = Path.Combine(backslash, ip, pathSubtring);
            return convertPath;
        }

        public void SendEmailHandler(NotificationCommand command)
        {

            _noticeCommand.Handle(command);
        }

        public string getErrorCodeOTP(string Error_Code = "", string Mobile_No = "")
        {
            try
            {
                var Massage = "";
                var Error_Massage = base.LovData.Where(l => l.Name == "L_ERROR_OTP" && l.Text == Error_Code);
                if (Error_Massage.Any())
                {
                    var tmp = Error_Massage.FirstOrDefault();
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        Massage = tmp.LovValue1;
                        if (Massage.IndexOf("{MobileNo}") > 0)
                        {
                            if (Mobile_No == "SESSION")
                                Massage = Massage.Replace("{MobileNo}", Session["CONTRACTMOBILENO"] as string);
                            else
                                Massage = Massage.Replace("{MobileNo}", Mobile_No.Substring(0, 3) + "XXX" + Mobile_No.Substring(Mobile_No.Length - 4));
                        }
                    }
                    else
                    {
                        Massage = tmp.LovValue2;
                        if (Massage.IndexOf("{MobileNo}") > 0)
                        {
                            if (Mobile_No == "SESSION")
                                Massage = Massage.Replace("{MobileNo}", Session["CONTRACTMOBILENO"] as string);
                            else
                                Massage = Massage.Replace("{MobileNo}", Mobile_No.Substring(0, 3) + "XXX" + Mobile_No.Substring(Mobile_No.Length - 4));
                        }
                    }
                }
                return Massage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult CheckOfficerTimeOut()
        {
            bool result = false;
            if (Session["OfficerModel"] != null)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenQuestion(string MobileNo = "", string Technology = "")
        {
            var Questions = GetQuestionByChannel(MobileNo, "FBBWF", "New Registration", Technology);
            string result = "";
            List<string> AnswerDllList = new List<string>();
            List<string> SubAnswerDllList = new List<string>();
            if (Questions != null && Questions.questionDatas != null && Questions.questionDatas.Count > 0)
            {
                List<QuestionData> QuestionGroups = Questions.questionDatas.DistinctBy(t => new { t.GROUP_ID, t.GROUP_NAME_EN, t.GROUP_NAME_TH }).ToList();
                foreach (var itemGroup in QuestionGroups)
                {
                    string GroupName = "";
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        GroupName = itemGroup.GROUP_NAME_TH;
                    }
                    else
                    {
                        GroupName = itemGroup.GROUP_NAME_EN;
                    }
                    result += "<div class='col-md-12  col-sm-12'><strong>" + GroupName + "</strong></div>";
                    result += "<div class='col-md-12  col-sm-12'>";
                    List<QuestionData> QuestionInGroups = Questions.questionDatas.Where(t => t.GROUP_ID == itemGroup.GROUP_ID).ToList();
                    foreach (var item in QuestionInGroups)
                    {
                        string temSubAnswer = "";
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        {
                            result += "<div id='QuestionAIS" + item.QUESTION_ID + "' class='QuestionAIS'><div class='col-md-3 col-sm-12' style='padding-left: 15px;'>" + item.QUESTION_TH + "</div>";
                        }
                        else
                        {
                            result += "<div id='QuestionAIS" + item.QUESTION_ID + "' class='QuestionAIS'><div class='col-md-3 col-sm-12' style='padding-left: 15px;'>" + item.QUESTION_EN + "</div>";
                        }

                        var Answers = Questions.answerDatas.Where(t => t.QUESTION_ID == item.QUESTION_ID && t.PARENT_ANSWER_ID == "").ToList();
                        var AnswerHaveParents = Questions.answerDatas.Where(t => t.QUESTION_ID == item.QUESTION_ID && t.PARENT_ANSWER_ID != "").ToList();

                        result += "<div class='col-md-9  col-sm-12'>";
                        foreach (var subItem in Answers)
                        {
                            var UseSubAnswer = "";
                            List<AnswerData> AnswerHaveParent = AnswerHaveParents.Where(t => t.PARENT_ANSWER_ID == subItem.ANSWER_ID).ToList();
                            List<SubAnswerData> subAnswerData = new List<SubAnswerData>();
                            if (AnswerHaveParent != null && AnswerHaveParent.Count > 0)
                            {
                                foreach (var SubAnswerItem in AnswerHaveParent)
                                {
                                    subAnswerData = Questions.subAnswerDatas.Where(t => t.ANSWER_ID == SubAnswerItem.ANSWER_ID).ToList();
                                    if (subAnswerData != null && subAnswerData.Count > 0)
                                    {
                                        string SubAnswerName = "";
                                        bool DefaultValue = true;

                                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                                        {
                                            SubAnswerName = SubAnswerItem.ANSWER_TH;
                                        }
                                        else
                                        {
                                            SubAnswerName = SubAnswerItem.ANSWER_EN;
                                        }
                                        if (SubAnswerItem.DISPLAY_TYPE == "Drop Down List")
                                        {
                                            temSubAnswer += "<div id='divDll" + subItem.ANSWER_ID + "' class='SubAnswer" + item.QUESTION_ID + "' style='display: none;'><div class='col-md-3  col-sm-12'>" + SubAnswerName + "</div>";
                                            temSubAnswer += "<div class='col-md-5  col-sm-12'><select class='select' style='width: 100%' id='dll_" + subItem.ANSWER_ID + "' title='" + SubAnswerName + "'>";
                                            foreach (var itemSubAnswer in subAnswerData)
                                            {
                                                string AnswerDisplay = "";

                                                if (SiteSession.CurrentUICulture.IsThaiCulture())
                                                {
                                                    AnswerDisplay = itemSubAnswer.ANSWER_VALUE_TH;
                                                }
                                                else
                                                {
                                                    AnswerDisplay = itemSubAnswer.ANSWER_VALUE_EN;
                                                }
                                                if (DefaultValue)
                                                {
                                                    temSubAnswer += "<option value='" + itemSubAnswer.ANSWER_ID + "' selected>" + AnswerDisplay + "</option>";
                                                    DefaultValue = false;
                                                }
                                                else
                                                {
                                                    temSubAnswer += "<option value='" + itemSubAnswer.ANSWER_ID + "'>" + AnswerDisplay + "</option>";
                                                }
                                            }
                                            temSubAnswer += "</select></div><div class='col-md-4 hidden-md-down'></div><p class='clearfix'></p></div>";
                                            SubAnswerDllList.Add("dll_" + subItem.ANSWER_ID);
                                        }
                                        else if (SubAnswerItem.DISPLAY_TYPE == "Radio Button")
                                        {

                                        }
                                        else if (SubAnswerItem.DISPLAY_TYPE == "Check Box")
                                        {

                                        }
                                        else if (SubAnswerItem.DISPLAY_TYPE == "Textbox")
                                        {

                                        }

                                        UseSubAnswer = @"UseSubAnswer(""" + item.QUESTION_ID + @""",""" + subItem.ANSWER_ID + @""");";
                                    }
                                }
                            }
                            if (UseSubAnswer == "" && AnswerHaveParents != null && AnswerHaveParents.Count > 0)
                            {
                                UseSubAnswer = @"UseSubAnswer(""" + item.QUESTION_ID + @""",);";
                            }

                            string AnswerName = "";
                            if (SiteSession.CurrentUICulture.IsThaiCulture())
                            {
                                AnswerName = subItem.ANSWER_TH;
                            }
                            else
                            {
                                AnswerName = subItem.ANSWER_EN;
                            }


                            var QuestionHide = "";
                            if (item.CHECK_ACTION_FLAG == "Y")
                            {
                                if (subItem.ACTION == "SKIP")
                                {
                                    QuestionHide = @"QuestionHide("""",""" + subItem.VALUE + @""");";
                                }
                                else if (subItem.ACTION == "END")
                                {
                                    QuestionHide = @"QuestionHide(""" + item.QUESTION_ID + @""",""END"");";
                                }
                                else
                                {
                                    var checkSkip = Answers.Where(t => t.ACTION == "SKIP").ToList();
                                    var checkEnd = Answers.Where(t => t.ACTION == "END").ToList();
                                    if (checkSkip != null && checkSkip.Count > 0)
                                    {
                                        QuestionHide = @"QuestionShow("""",""" + checkSkip.FirstOrDefault().VALUE + @""");";
                                    }
                                    if (checkEnd != null && checkEnd.Count > 0)
                                    {
                                        QuestionHide = @"QuestionShow(""" + item.QUESTION_ID + @""",""END"");";
                                    }
                                }
                            }

                            if (subItem.DISPLAY_TYPE == "Radio Button")
                            {
                                result += "<label id='lbl_" + subItem.ANSWER_ID + "' for='" + subItem.ANSWER_ID + "' class='radio-inline' style='padding-right: 15px; font-weight: normal !important;'>";
                                result += "<input type='radio' id='" + subItem.ANSWER_ID + "'  value='" + subItem.ANSWER_ID + "' name='select" + subItem.QUESTION_ID + "' onchange='" + UseSubAnswer + " " + QuestionHide + "' />";
                                result += AnswerName;
                                result += "</label>";
                            }
                            else if (subItem.DISPLAY_TYPE == "Check Box")
                            {
                                result += "<label><input name='check" + subItem.QUESTION_ID + "' id='" + subItem.ANSWER_ID + "' value='" + subItem.ANSWER_ID + "' onclick='" + UseSubAnswer + " " + QuestionHide + "' type='checkbox'>";
                                result += AnswerName + " </label>";
                            }
                            else if (subItem.DISPLAY_TYPE == "Drop Down List")
                            {
                                subAnswerData = Questions.subAnswerDatas.Where(t => t.ANSWER_ID == subItem.ANSWER_ID).ToList();
                                if (subAnswerData != null && subAnswerData.Count > 0)
                                {
                                    bool DefaultValue = true;
                                    result += "<select class='select' style='width: 100%' id='dll_" + subItem.ANSWER_ID + "' title='" + AnswerName + "'>";
                                    foreach (var itemSubAnswer in subAnswerData)
                                    {
                                        string AnswerDisplay = "";

                                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                                        {
                                            AnswerDisplay = itemSubAnswer.ANSWER_VALUE_TH;
                                        }
                                        else
                                        {
                                            AnswerDisplay = itemSubAnswer.ANSWER_VALUE_EN;
                                        }
                                        if (DefaultValue)
                                        {
                                            result += "<option value='" + itemSubAnswer.ANSWER_ID + "' selected>" + AnswerDisplay + "</option>";
                                            DefaultValue = false;
                                        }
                                        else
                                        {
                                            result += "<option value='" + itemSubAnswer.ANSWER_ID + "'>" + AnswerDisplay + "</option>";
                                        }
                                    }
                                    result += "</select>";
                                    AnswerDllList.Add("dll_" + subItem.ANSWER_ID);
                                }
                            }
                            else if (subItem.DISPLAY_TYPE == "Textbox")
                            {

                            }
                        }
                        result += "</div>";
                        result += "<p class='clearfix'></p></div>";
                        if (temSubAnswer != "")
                        {
                            result += temSubAnswer;
                        }
                        string QuestionDesc = "";
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        {
                            QuestionDesc = item.QUESTION_DESC_TH.ToSafeString();
                        }
                        else
                        {
                            QuestionDesc = item.QUESTION_DESC_EN.ToSafeString();
                        }
                        if (QuestionDesc != "")
                        {
                            result += "<div class='col-md-12 col-sm-12'>";
                            result += QuestionDesc;
                            result += "<p class='clearfix'></p></div>";
                        }
                    }
                    result += "</p></div>";
                }
                result += "<div id='listCustomerInsight'></div>";
            }
            // REMARK_FOR_SUB
            string L_REMARK_FOR_SUB = "";
            var lovValue = base.LovData.FirstOrDefault(t => t.Name == "L_REMARK_FOR_SUB" && t.Type == "SCREEN" && t.LovValue5 == "FBBOR003");
            if (lovValue != null)
            {
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    L_REMARK_FOR_SUB = lovValue.LovValue1.ToSafeString();
                }
                else
                {
                    L_REMARK_FOR_SUB = lovValue.LovValue2.ToSafeString();
                }
                if (L_REMARK_FOR_SUB != "")
                {
                    result += "<p class='clearfix'></p><div class='col-md-12 col-sm-12'>" + L_REMARK_FOR_SUB + "</div>";
                    result += "<div class='col-ms-12 col-sm-12'><textarea class='form-control' cols='20' id='Remark_For_Subcontract' name='CustomerRegisterPanelModel.Remark_For_Subcontract' maxlength='500' rows='3' title='" + L_REMARK_FOR_SUB + "'></textarea><p class='clearfix'></p><p class='clearfix'></div>";
                }
            }
            return Json(new { resultHtml = result, AnswerIDs = AnswerDllList, SubAnswerIDs = SubAnswerDllList, QuestionList = Questions }, JsonRequestBehavior.AllowGet);
        }

        private GetQuestionByChannelModel GetQuestionByChannel(string MobileNo = "", string Channel = "", string OrderType = "", string Technology = "")
        {
            GetQuestionByChannelQuery query = new GetQuestionByChannelQuery()
            {
                MobileNo = MobileNo,
                p_channel = Channel,
                p_order_type = OrderType,
                p_technology = Technology
            };
            GetQuestionByChannelModel queryData = _queryProcessor.Execute(query);
            return queryData;
        }
    }
}