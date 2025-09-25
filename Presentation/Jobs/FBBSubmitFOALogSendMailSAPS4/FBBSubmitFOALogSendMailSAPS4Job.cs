using System;
using System.Collections.Generic;

namespace FBBSubmitFOALogSendMailSAPS4
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using WBBBusinessLayer;
    using WBBBusinessLayer.CommandHandlers;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.ExWebServices.SAPFixedAsset;
    using WBBContract.Commands.FBBWebConfigCommands;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBContract.Queries.WebServices;
    using WBBEntity.Extensions;
    //using CompositionRoot;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using WBBEntity.PanelModels.WebServiceModels;

    public class FBBSubmitFOALogSendMailSAPS4Job
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private string _outErrorResult;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogCommand;
        private readonly IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> _queryProcessorGoodsMovementHandler;
        private readonly ICommandHandler<UpdateSubmitFoaErrorLogCommand> _UpdateSubmitFoaError;
        private readonly ICommandHandler<UpdateFoaResendEditCommand> _intFoaResendEditCommand;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }
        public FBBSubmitFOALogSendMailSAPS4Job(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateSubmitFoaErrorLogCommand> UpdateSubmitFoaError,
             ICommandHandler<SendSmsCommand> SendSmsCommand,
             ICommandHandler<SendMailBatchNotificationCommand> sendMail,
            ICommandHandler<UpdateFbssFOAConfigTblCommand> intfLogCommand
            , IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> queryProcessorGoodsMovementHandle
            , ICommandHandler<UpdateFoaResendEditCommand> intFoaResendEditCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _UpdateSubmitFoaError = UpdateSubmitFoaError;
            _sendMail = sendMail;
            _sendSmsCommand = SendSmsCommand;
            _queryProcessorGoodsMovementHandler = queryProcessorGoodsMovementHandle;
            _intFoaResendEditCommand = intFoaResendEditCommand;
        }
        public void FBBSubmitFOALogSendMailSAPS4()
        {
            try
            {

                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH", "DATE_START").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH", "DATE_TO").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var date_diff = Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH", "DATE_DIFF").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var Check_DateDiff = date_diff.DISPLAY_VAL.ToSafeString() == "Y" ? date_diff.VAL1.ToSafeInteger() : 1;

                _logger.Info("DATE_START : DISPLAY_VAL :: " + date_start.DISPLAY_VAL + " VAL_1 :: " + date_start.VAL1);
                _logger.Info("DATE_TO : DISPLAY_VAL :: " + date_to.DISPLAY_VAL + " VAL_1 :: " + date_to.VAL1);
                _logger.Info("DATE_DIFF : DISPLAY_VAL :: " + date_diff.DISPLAY_VAL + " VAL_1 :: " + date_diff.VAL1);

                try
                {

                    ///////////////  DATE_TO
                    string c_DATE_TO = string.Empty;
                    DateTime parsedDateTo = DateTime.Now;
                    if (date_to.DISPLAY_VAL == "Y")
                    {
                        c_DATE_TO = date_to.VAL1;
                        DateTime.TryParseExact(date_to.VAL1, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out parsedDateTo);
                    }
                    else
                    {
                        c_DATE_TO = parsedDateTo.ToString("ddMMyyyy");
                    }


                    ///////////////  DATE_START
                    DateTime parsedDateStart;
                    string c_DATE_START = string.Empty;
                    if (date_start.DISPLAY_VAL == "Y")
                    {
                        c_DATE_START = date_start.VAL1;
                    }
                    else
                    {
                        parsedDateStart = parsedDateTo.AddDays(-Check_DateDiff);
                        c_DATE_START = parsedDateStart.ToString("ddMMyyyy");
                    }



                    _logger.Info("DATE_START :: " + c_DATE_START);
                    _logger.Info("DATE_TO :: " + c_DATE_TO);


                    ////////////////// UPDATE DISPLAY_VAL DATE_START เป็นค่า Y
                    var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH",
                        con_name = "DATE_START",
                        display_val = "Y",
                        val1 = c_DATE_START,
                        flag = "EQUIP",
                        updated_by = "SUBMITFOALOGSENDMAILSAPS4"
                    };
                    //UpdateFbssFOAConfigTblCommand
                    _intfLogCommand.Handle(queryUpdateDate);
                    _logger.Info("Update DISPLAY_VAL DATE_START to Y");



                    _logger.Info("Call P_FBB_GET_RESEND_ERROR");

                    var ResultModelSendmailData = new List<SubmitFOAEquipment>();
                    var query = new SubmitFOASendmailDataNewQuery()
                    {
                        dateFrom = c_DATE_START,
                        dateTo = c_DATE_TO                     
                    };

                    ResultModelSendmailData = _queryProcessor.Execute(query);

                    var all_result = new List<SubmitFOAEquipment>();
                    var new_result = new List<SubmitFOAEquipment>();

                    if (ResultModelSendmailData.Count > 0)
                    {

                        var ErrList = Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH", "S4_AUTO_RESEND").Where(record => record.ACTIVEFLAG == "Y").ToList();
                        var errPatterns = ErrList.Select(x => x.VAL1?.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
                        var ErrList_in = Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH", "S4_AUTO_RESEND_INS").Where(record => record.ACTIVEFLAG == "Y").ToList();
                        var errPatterns_in = ErrList_in.Select(x => x.VAL1?.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();

                        var ErrList_edit = Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH", "S4_AUTO_RESEND_EDIT").Where(record => record.ACTIVEFLAG == "Y").ToList();
                        var errPatterns_edit = ErrList_edit.Select(x => x.VAL1?.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();

                        var errPatternsRegex = errPatterns
                            .Select(p => "^" + Regex.Escape(p).Replace("%", ".*") + "$")
                            .Select(p => new Regex(p, RegexOptions.IgnoreCase))
                            .ToList();

                        var errPatternsRegex_IN = errPatterns_in
                           .Select(p => "^" + Regex.Escape(p).Replace("%", ".*") + "$")
                           .Select(p => new Regex(p, RegexOptions.IgnoreCase))
                           .ToList();

                        //string postingPeriodText = errPatterns_in.FirstOrDefault() ?? "";
                       
                        var errPatternsRegex_EDIT = errPatterns_edit
                           .Select(p => "^" + Regex.Escape(p).Replace("%", ".*") + "$")
                           .Select(p => new Regex(p, RegexOptions.IgnoreCase))
                           .ToList();

                        //var errPatternsRegex_EDIT_New = errPatterns_edit
                        //   .Select(BuildRegexFromTemplate)
                        //   .ToList();
                   
                        foreach (var obj in ResultModelSendmailData)
                        {
                            try
                            {
                                var errMsgs = obj.ERR_MSG.ToSafeString().Split('|');
                                var lastErrMsg = errMsgs.LastOrDefault()?.Trim();
                                var recType = obj.REC_TYPE?.Trim().ToUpper();
                                // กรองเฉพาะที่ match ตามเดิม
                                bool isMatch = false;
                                var allowedTypes = new[] { "A", "B", "C" };


                                var temp = new SubmitFOAEquipment()
                                {
                                    MATERIAL_CODE = obj.MATERIAL_CODE,
                                    STORAGE_LOCATION = obj.STORAGE_LOCATION,
                                    SERIAL_NUMBER = obj.SN,
                                    PLANT = obj.PLANT,
                                    COMPANY_CODE = obj.COMPANY_CODE,
                                    TRANS_ID = obj.TRANS_ID,
                                    ACCESS_NUMBER = obj.ACCESS_NUMBER,
                                    SUBCONTRACT_NAME = obj.SUBCONTRACT_NAME,
                                    ERR_MSG = lastErrMsg,
                                    REC_TYPE = recType

                                };

                                all_result.Add(temp); // เก็บทั้งหมด


                                if (recType == "A" || recType == "C")
                                {

                                    if (errPatterns_edit != null || errPatterns != null)
                                    {

                                        //isMatch = errMsgs.Any(msg => errPatternsRegex_EDIT.Any(rx => rx.IsMatch(msg.Trim())));
                                        isMatch = errPatternsRegex_EDIT.Any(rx => rx.IsMatch(lastErrMsg ?? ""));
                                        if (isMatch)
                                        {
                                            
                                            var matchSerial = Regex.Match(obj.ERR_MSG, @"(?<=Stock data of serial number\s)(.+?)(?=\snot suitable for movement)");
                                            var matchMaterial = Regex.Match(obj.ERR_MSG, @"(?<=Material\s)(.+?)(?=\sStatus ESTO in)");
                                            var matchPlant = Regex.Match(obj.ERR_MSG, @"(?<=Status ESTO in\s)([^/]+)(?=/)");
                                            var match_Sloc = Regex.Match(obj.ERR_MSG, @"(?<=\/)([^ ]+?)(?=\sBatch)");
                                            var macthBatch = Regex.Match(obj.ERR_MSG,@"\bBatch\s+(.*?)\s+Stock\s+Type\s+01\b",RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                            string batch = macthBatch.Success ? macthBatch.Groups[1].Value.Trim() : string.Empty;
                                            string[] percentKeysInOrder =
                                            {
                                                "serial_no",
                                                "serial",
                                                "material_code",
                                                "plant",
                                                "storage_location",
                                                "batch"
                                            };

                                            // สร้าง map สำหรับแทนค่า
                                            var map = new Dictionary<string, string>
                                            {
                                                ["serial_no"] = matchSerial.Value,
                                                ["serial"] = matchSerial.Value,
                                                ["material_code"] = matchMaterial.Value,
                                                ["plant"] = matchPlant.Value,
                                                ["storage_location"] = match_Sloc.Value,
                                                ["batch"] = batch
                                            };

                                            string pattern = errPatterns_edit.FirstOrDefault();
                                            string result = pattern;

                                            foreach (var key in percentKeysInOrder)
                                            {
                                                if (map.TryGetValue(key, out var value))
                                                {
                                                    // ReplaceFirst คือ method ช่วย ให้แทนที่ % ตัวแรกทีละรอบ
                                                    result = ReplaceFirst(result, "%", value);
                                                }
                                            }

                                            temp.ERR_MSG = result;
                                            temp.ERR_DESC = "Resend Edit";

                                            var queryUpdateFoaResendEdit = new UpdateFoaResendEditCommand()
                                            {
                                                p_serial_no = matchSerial.Value,
                                                p_order_no = obj.ORDER_NO,
                                                p_trans_id = obj.TRANS_ID,
                                                p_plant = matchPlant.Value,
                                                p_storage_location = match_Sloc.Value,
                                                p_access_number = obj.ACCESS_NUMBER
                                            };

                                            _logger.Info("Plant : " + matchPlant.Value + " Storage location : " + match_Sloc.Value);
                                            _logger.Info("== Start Update Plant and Storage_location  ==");

                                            _intFoaResendEditCommand.Handle(queryUpdateFoaResendEdit);

                                            _logger.Info("== End Update Plant and Storage location ==");
                                        }

                                        else
                                        {
                                            isMatch = errMsgs.Any(msg => errPatternsRegex.Any(rx => rx.IsMatch(msg.Trim())));
                                            if (isMatch)
                                            {
                                                temp.ERR_DESC = "Resend";
                                            }
                                        }

                                    }
                                }
                                

                                else if (recType == "B" && errPatterns_in != null)
                                {
                                    //isMatch = errMsgs.Any(msg => msg.IndexOf(postingPeriodText, StringComparison.OrdinalIgnoreCase) >= 0);
                                    isMatch = errMsgs.Any(msg => errPatternsRegex_IN.Any(rx => rx.IsMatch(msg.Trim())));
                                    if (isMatch)
                                    {
                                        temp.ERR_DESC = "Resend";
                                    }
                                }

                                if (isMatch && allowedTypes.Contains(recType))
                                {
                                    //temp.ERR_DESC = "Resend";
                                    new_result.Add(temp); // เก็บเฉพาะ match
                                }
                                else
                                {
                                    temp.ERR_DESC = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("Exception in Loop : " + ex.Message);
                            }
                        }


                        var recTypeGroups = new_result
                       .Where(e => !string.IsNullOrWhiteSpace(e.REC_TYPE))
                       .GroupBy(e =>
                       {
                           var type = e.REC_TYPE.Trim().ToUpper();
                           return (type == "B") ? "INSTALLATION" : "EQUIPMENT";
                       });

                        foreach (var group in recTypeGroups)
                        {
                            var tabName = group.Key;

                            var masterList = group.Select(e => new SubmitFOAResenorderdata
                            {
                                MaterialCode = e.MATERIAL_CODE,
                                StorageLocation = e.STORAGE_LOCATION,
                                SerialNumber = e.SERIAL_NUMBER,
                                Plant = e.PLANT,
                                CompanyCode = e.COMPANY_CODE,
                                trans_id = e.TRANS_ID
                            }).ToList();


                            var querys = new SubmitFOAResendOrderNewQuery()
                            {
                                tab_name = tabName,
                                list_p_get_oder = masterList
                            };

                            var ResultModelResendOrderGetData = new ResendOrderGetData();
                            ResultModelResendOrderGetData = _queryProcessor.Execute(querys);
                            var transIds = string.Join(", ", masterList.Select(x => x.trans_id));

                            _logger.Info($"Submit {tabName} → {masterList.Count} records trans id: {transIds}");
                        }

                    }
                    else
                    {
                        _logger.Info("No Data");
                    }



                    ////////////////// UPDATE DISPLAY_VAL DATE_START, DATE_TO เป็นค่า N
                    var updateDateStart_L = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH",
                        con_name = "DATE_START",
                        display_val = "N",
                        val1 = DateTime.Now.ToString("ddMMyyyy"),
                        flag = "EQUIP",
                        updated_by = "SUBMITFOALOGSENDMAILSAPS4"
                    };
                    _intfLogCommand.Handle(updateDateStart_L);
                    _logger.Info("Update DISPLAY_VAL DATE_START to N");


                    var updateDateTo_L = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH",
                        con_name = "DATE_TO",
                        display_val = "N",
                        val1 = DateTime.Now.ToString("ddMMyyyy"),
                        flag = "EQUIP",
                        updated_by = "SUBMITFOALOGSENDMAILSAPS4"
                    };
                    _intfLogCommand.Handle(updateDateTo_L);
                    _logger.Info("Update DISPLAY_VAL DATE_TO to N");


                    if (all_result.Any())
                    {
                        var result = new List<SubmitFOAEquipment>();

                        var result_sendmail = (result ?? new List<SubmitFOAEquipment>())
                       .Except(new_result ?? new List<SubmitFOAEquipment>())
                       .ToList();

                        //TODO: GET Subject and detail 
                        string strFrom = "";
                        string strTo = "";
                        string strCC = "";
                        string strBCC = "";
                        string IPMailServer = "";
                        string FromPassword = "";
                        string Port = "";
                        string Domaim = "";
                        string strSubject = "Summary Error Message FOA [" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "]  จำนวน " + new_result.Count + " record";

                        try
                        {


                            string strBody = "Dear All<br>" +
                                   GenBodyEmailTableAutoResend(all_result);

                            string[] sendResult = Sendmail("SEND_EMAIL_FIXED_ASSET", "BATCH", strFrom, strTo, strCC,
                                strBCC, strSubject, strBody, IPMailServer, FromPassword, Port, Domaim);

                            _logger.Info("SendEmail Success");
                        }
                        catch (Exception ex)
                        {
                            //SendSms();
                            _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());
                        }
                    }
                    else
                    {
                        _logger.Info(" Unable to send email");
                    }



                }


                catch (Exception ex)
                {
                    _logger.Info("Exception at FBBSubmitFOALogSendMailSAPS4Job ERROR : " + ex.Message);
                }


            }
            catch (Exception ex)
            {
                _logger.Info("Exception at FBBSubmitFOALogSendMailSAPS4Job Error: " + ex.Message);
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







        public string GenBodyEmailTableAutoResend(List<SubmitFOAEquipment> list)
        {
            string body = "พบ Error ในการตัด Stock จำนวน " + list.Count + " record";
            body += "<br/><br/>";
            body += "<table border='1px solid #ddd' width='100%' cellpadding='0' cellspacing='0'>";
            body += "<thead>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Tarns Id</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Internet Number</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>Sub Contract Name</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>S/N</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:300px'>Error Message</th>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:300px'>Error Description</th>";
            body += "</thead>";
            body += "<tbody>";
            foreach (var item in list)
            {
                body += "<tr>";
                body += "<td style='vertical-align: top;'>" + item.TRANS_ID + "</td>";
                body += "<td style='vertical-align: top;'>" + item.ACCESS_NUMBER + "</td>";
                body += "<td style='vertical-align: top;'>" + item.SUBCONTRACT_NAME + "</td>";
                body += "<td style='vertical-align: top;'>" + item.SERIAL_NUMBER + "</td>";
                body += "<td style='vertical-align: top;'>" + item.ERR_MSG + "</td>";
                body += "<td style='vertical-align: top;'>" + item.ERR_DESC + "</td>";
                body += "</tr>";
            }
            body += "</tbody>";
            body += "</table>";
            return body;
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

        public List<LovModel> GET_FBSS_FIXED_ASSET_CONFIG(string product_name)
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = product_name
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
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


        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        //Add New 21/08/2568 สำหรับกรณีดักแค่ Serial ค่าต้องไม่เป็น Null หรือว่างมา
        //public static Regex BuildRegexFromTemplate(string template)
        //{
        //    // 1) หนีอักขระทั้งหมดก่อน
        //    var escaped = Regex.Escape(template);

        //    // 2) บังคับ "ต้องไม่ว่าง" เฉพาะ % หลัง 'serial number' และหลัง 'Serial'
        //    escaped = Regex.Replace(
        //        escaped,
        //        @"(?<=serial number\s)\\%",
        //        @"(?<serial_no>.+?)",
        //        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        //    escaped = Regex.Replace(
        //        escaped,
        //        @"(?<=Serial\s)\\%",
        //        @"(?<serial>.+?)",
        //        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        //    // 3) ที่เหลือทั้งหมดของ \% = ว่างได้ (.*?)
        //    escaped = Regex.Replace(escaped, @"\\%", @".*?");

        //    // 4) ใส่ anchor และ option
        //    return new Regex("^" + escaped + "$",
        //        RegexOptions.IgnoreCase |
        //        RegexOptions.CultureInvariant |
        //        RegexOptions.Compiled |
        //        RegexOptions.Singleline);
        //}

    }
}
