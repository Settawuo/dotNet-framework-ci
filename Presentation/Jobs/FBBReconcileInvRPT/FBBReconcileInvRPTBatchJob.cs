using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBReconcileInvRPT
{
    public class FBBReconcileInvRPTBatchJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssConfigTblCommand> _intfLogCommand;
        private string _outErrorResult = string.Empty;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private string fullpath_output = "";
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("FBBReconcileInvRPT : " + _timer.Elapsed);
        }
        public FBBReconcileInvRPTBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
             ICommandHandler<UpdateFbssConfigTblCommand> intfLogCommand,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail,
              ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _sendMail = sendMail;
            _sendSmsCommand = SendSmsCommand;

        }
        public FBBReconcileInvRPTBatchModel GetReconcileBatch()
        {
            try
            {

                var model = new FBBReconcileInvRPTBatchQuery()
                {

                };
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetReconcileBatch : " + ex.Message);
                SendSms();
                return new FBBReconcileInvRPTBatchModel();
            }
        }

        public string GenBodyEmailTableAutoResend()
        {
            var result = GetReconcileBatch();

            string body = "ret code = " + result.ret_code + " <br/> ret msg = " + result.ret_msg;
            body += "<br/><br/>";
            body += "not found FOA = " + result.cur.Count + " record <br/> not found OM010 = " + result.cur2.Count + " record";
            //cur
            body += "<br/><br/>";
            body += "<table border='1px solid #ddd' width='100%' cellpadding='0' cellspacing='0'>";
            body += "<thead>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>not found FOA</th>";
            body += "</thead>";
            body += "<tbody>";
            foreach (var item in result.cur)
            {
                body += "<tr>";
                body += "<td style='vertical-align: top;'>" + item.ord_no + "</td>";
                body += "</tr>";
            }
            body += "</tbody>";
            body += "</table>";

            //cur 2
            body += "<br/><br/>";
            body += "<table border='1px solid #ddd' width='100%' cellpadding='0' cellspacing='0'>";
            body += "<thead>";
            body += "<th style='background-color:#58ACFA;text-alight:center;width:150px'>not found OM010</th>";
            body += "</thead>";
            body += "<tbody>";
            foreach (var i in result.cur2)
            {
                body += "<tr>";
                body += "<td style='vertical-align: top;'>" + i.ACCESS_NUMBER + "</td>";
                body += "</tr>";
            }
            body += "</tbody>";
            body += "</table>";

            string o_fileName = "FBBReconcileInvRPTBatch" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx";
            fullpath_output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"output\", o_fileName);

            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;

                //*** Sheet 1
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = "not found FOA";
                worksheet.Cells["B1"].Value = "not found OM010";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["B1"].Style.Font.Bold = true;
                worksheet.Cells["A1,B1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A1,B1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFE333"));
                worksheet.Cells["A1,B1"].AutoFitColumns();


                int row = 2;
                for (int i = 0; i < result.cur.Count; i++)
                {
                    worksheet.Cells["A" + row++].Value = result.cur[i].ord_no;
                }
                int row2 = 2;
                for (int i = 0; i < result.cur2.Count; i++)
                {

                    worksheet.Cells["B" + row2++].Value = result.cur2[i].ACCESS_NUMBER;
                }
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"output\")))
                {
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"output\"));
                }
                package.SaveAs(new FileInfo(fullpath_output));
            }

            //Excel excel = new Excel(path, 1);
            //excel.WriteToCell(0, 0, "CUR");
            //excel.WriteToCell(0, 1, "CUR2");
            //int row = 1;
            //for (int i = 0; i < result.cur.Count; i++)
            //{

            //    excel.WriteToCell(row++, 0, result.cur[i].ord_no);
            //}
            //int row2 = 1;
            //for (int i = 0; i < result.cur2.Count; i++)
            //{

            //    excel.WriteToCell(row2++, 1, result.cur2[i].ACCESS_NUMBER);
            //}
            //excel.SaveAs(fullpath_output);
            //excel.Close();

            return body;
        }
        public void SendMail()
        {
            var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("FBB_RECONCILEINVRPT").FirstOrDefault();

            if (resultFixAssConfig == null)
            {
                _logger.Info("program haven't lov ,please  set config");
            }
            else
            {

                if (resultFixAssConfig.DISPLAY_VAL == "Y")
                {
                    _logger.Info("COM_CODE = 'Y'");
                    //TODO: GET Subject and detail 
                    string strFrom = "";
                    string strTo = "";
                    string strCC = "";
                    string strBCC = "";
                    string IPMailServer = "";
                    string FromPass = "";// Fixed Code scan : string FromPassword = "";
                    string Port = "";
                    string Domaim = "";
                    string strSubject = "FBBReconcileInvRPTBatch [" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "]";
                    string[] att = new string[1];
                    try
                    {

                        string strBody = "Dear All<br>" +
                           GenBodyEmailTableAutoResend();



                        att[0] = fullpath_output;

                        string[] sendResult = Sendmail("SEND_EMAIL_FBBRECONCILEINVRPT_BATCH", "BATCH", strFrom, strTo, strCC,
                            strBCC, strSubject, strBody, IPMailServer, FromPass, Port, Domaim, att);

                        _logger.Info("SendEmail Success");
                    }
                    catch (Exception ex)
                    {
                        SendSms();
                        _logger.Info(" Error QueryDataToSendMail :" + ex.Message);
                    }
                }
                else
                {
                    _logger.Info("COM_CODE = 'N'");
                }

            }

        }

        public string[] Sendmail(string processname, string createuser, string sendfrom, string sendto, string sendcc, string sendbcc
            , string subject, string body, string ip_mail_server, string frompass, string port, string domain, string[] attachfile)
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
                    IPMailServer = ip_mail_server,
                    AttachFiles = attachfile
                };


                _sendMail.Handle(command);

                _logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                StopWatching();

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
                                      ex.Message);
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.Message));
                _logger.Info(ex.Message);

                StopWatching();
            }

            return result;
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
        public List<LovModel> GET_FBSS_FIXED_ASSET_CONFIG(string product_name)
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = product_name
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
                            command.FullUrl = "FBBReconcileInvRPTBatch";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBReconcileInvRPTBatch Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
