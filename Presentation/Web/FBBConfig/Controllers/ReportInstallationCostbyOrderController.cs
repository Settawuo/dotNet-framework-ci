using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using WBBBusinessLayer;
using WBBBusinessLayer.QueryHandlers.Commons.Master;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WebGrease.Activities;

namespace FBBConfig.Controllers
{
    public partial class ReportInstallationCostbyOrderController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendMailReportInstallationNotificationCommand> _sendMail;//
        private readonly ICommandHandler<SendMailReportInstallationCommand> _SendReportInstallationExp;//
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLogAPICommand;

        private readonly ICommandHandler<UpdateReportInstallationCostbyOrderCommand>
            _UpdateReportInstallationCostbyOrderCommand;//

        private readonly ICommandHandler<ReportInstallationUpdateNoteCommand> _ReportInstallationUpdateNoteCommand;//
        private readonly ICommandHandler<ReportInstallationCostbyOrderUpdateByFileCommand> _ReportInstallationCostbyOrderUpdateByFileCommand;//

        //R19.03
        private readonly ICommandHandler<ReportInstallationRecalByOrderCommand> _ReportInstallationRecalByOrderCommand;//
        private readonly ICommandHandler<ReportInstallationRecalByRevampCommand> _ReportInstallationRecalByRevampCommand;//
        private readonly ICommandHandler<ReportInstallationRecalByFileCommand> _ReportInstallationRecalByFileCommand;//
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        static ReportInstallationCostbyOrderListReturn resultInstall;
        static IEnumerable<ReportInstallationCostbyOrderListModel_Binding> newResultList;

        public ReportInstallationCostbyOrderController(ILogger logger, IQueryProcessor queryProcessor
            , ICommandHandler<SendMailReportInstallationNotificationCommand> sendMail //
            , ICommandHandler<SendMailReportInstallationCommand> SendReportInstallationExp//
            , ICommandHandler<UpdateReportInstallationCostbyOrderCommand> UpdateReportInstallationCostbyOrderCommand//
            , ICommandHandler<ReportInstallationUpdateNoteCommand> ReportInstallationUpdateNoteCommand //
            , ICommandHandler<ReportInstallationCostbyOrderUpdateByFileCommand> ReportInstallationCostbyOrderUpdateByFileCommand//
            , ICommandHandler<ReportInstallationRecalByOrderCommand> ReportInstallationRecalByOrderCommand//
            , ICommandHandler<ReportInstallationRecalByRevampCommand> ReportInstallationRecalByRevampCommand//
            , ICommandHandler<ReportInstallationRecalByFileCommand> ReportInstallationRecalByFileCommand//
            , ICommandHandler<InterfaceLogCommand> intfLogCommand
            , ICommandHandler<InterfaceLogPayGCommand> intfLogAPICommand

            )
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _sendMail = sendMail;
            _SendReportInstallationExp = SendReportInstallationExp;//
            _UpdateReportInstallationCostbyOrderCommand = UpdateReportInstallationCostbyOrderCommand;//
            _ReportInstallationUpdateNoteCommand = ReportInstallationUpdateNoteCommand;//
            _ReportInstallationCostbyOrderUpdateByFileCommand = ReportInstallationCostbyOrderUpdateByFileCommand;//
            _ReportInstallationRecalByOrderCommand = ReportInstallationRecalByOrderCommand;//
            _ReportInstallationRecalByRevampCommand = ReportInstallationRecalByRevampCommand;//
            _ReportInstallationRecalByFileCommand = ReportInstallationRecalByFileCommand;//
            _intfLogCommand = intfLogCommand;
            _intfLogAPICommand = intfLogAPICommand;
        }


        // GET: /ReportInstallationCostbyOrder/
        public ActionResult Index()
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            //  CurrentUser.E
            ViewBag.User = CurrentUser;
            ViewBag.UserGroup = GetUserGroup();

            SetViewBagLovV2("L_SEARCH_REPORT_INSTALLATION", "L_SEARCH_REPORT_INSTALLATION");
            SetViewBagLovV2("L_ORD_LIST_SCM_REPORT_INSTALLATION", "L_ORD_LIST_SCM_REPORT_INSTALLATION");
            SetViewBagLovV2("L_ORD_LIST_FAPO_REPORT_INSTALLATION", "L_ORD_LIST_FAPO_REPORT_INSTALLATION");
            SetViewBagLovV2("L_ORD_LIST_ACCT_REPORT_INSTALLATION", "L_ORD_LIST_ACCT_REPORT_INSTALLATION");

            SetViewBagLovV2("L_ORD_DETAIL_REPORT_INSTALLATION", "L_ORD_DETAIL_REPORT_INSTALLATION");
            SetViewBagLovV2("L_DIST_DETAIL_REPORT_INSTALLATION", "L_DIST_DETAIL_REPORT_INSTALLATION");
            SetViewBagLovV2("L_ORD_HIS_REPORT_INSTALLATION", "L_ORD_HIS_REPORT_INSTALLATION");
            SetViewBagLovV2("L_POST_SAP_DETAIL_REPORT_INSTALLATION", "L_POST_SAP_DETAIL_REPORT_INSTALLATION");
            SetViewBagLovV2("L_ORD_PACKAGE_REPORT_INSTALLATION", "L_ORD_PACKAGE_REPORT_INSTALLATION");
            SetViewBagLovV2("L_ORD_FEE_REPORT_INSTALLATION", "L_ORD_FEE_REPORT_INSTALLATION");


            var model = new ReportInstallationCostbyOrderModel();
            Session["TempSearchCriteria"] = null;
            Session["TempTotalPrice"] = null;
            return View(model);
        }

        private string GetUserGroup()
        {
            string ReSult = "";
            var query = new GetUserReportInstallationCostbyOrderGroupQuery()
            {
                p_USER_NAME = CurrentUser.UserName
            };

            var result = _queryProcessor.Execute(query);

            if (result != null)
            {
                ReSult = result.GROUP_NAME.ToSafeString();
            }

            return ReSult;
        }

        private void SetViewBagLovV2(string screenType, string type)
        {
            var query = new GetLovQuery()
            {
                LovType = screenType
            };

            var LovDataScreen = _queryProcessor.Execute(query).ToList();

            if (type == "L_SEARCH_REPORT_INSTALLATION")
            {
                ViewBag.SearchListScreen = LovDataScreen;
            }

            else if (type == "L_ORD_LIST_SCM_REPORT_INSTALLATION")
            {
                ViewBag.SCMOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_FAPO_REPORT_INSTALLATION")
            {
                ViewBag.FAPOOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_ACCT_REPORT_INSTALLATION")
            {
                ViewBag.ACCOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_DETAIL_REPORT_INSTALLATION")
            {
                ViewBag.OrderDetailListScreen = LovDataScreen;
            }
            else if (type == "L_DIST_DETAIL_REPORT_INSTALLATION")
            {
                ViewBag.DisDetailListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_HIS_REPORT_INSTALLATION")
            {
                ViewBag.OrderHisListScreen = LovDataScreen;
            }
            else if (type == "L_POST_SAP_DETAIL_REPORT_INSTALLATION")
            {
                ViewBag.POSTSAPListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_PACKAGE_REPORT_INSTALLATION")
            {
                ViewBag.OrderPackageListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_FEE_REPORT_INSTALLATION")
            {
                ViewBag.OrderFeeListScreen = LovDataScreen;
            }

        }

        private void SetViewBagLov(string screenType, string type)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType).ToList();
            if (type == "L_SEARCH_REPORT_INSTALLATION")
            {
                ViewBag.SearchListScreen = LovDataScreen;
            }

            else if (type == "L_ORD_LIST_SCM_REPORT_INSTALLATION")
            {
                ViewBag.SCMOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_FAPO_REPORT_INSTALLATION")
            {
                ViewBag.FAPOOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_ACCT_REPORT_INSTALLATION")
            {
                ViewBag.ACCOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_DETAIL_REPORT_INSTALLATION")
            {
                ViewBag.OrderDetailListScreen = LovDataScreen;
            }
            else if (type == "L_DIST_DETAIL_REPORT_INSTALLATION")
            {
                ViewBag.DisDetailListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_HIS_REPORT_INSTALLATION")
            {
                ViewBag.OrderHisListScreen = LovDataScreen;
            }
            else if (type == "L_POST_SAP_DETAIL_REPORT_INSTALLATION")
            {
                ViewBag.POSTSAPListScreen = LovDataScreen;
            }

        }
        #region intetfacelog
        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo, string INTERFACE_NODE, string reason)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = query.GetType().Name,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                OUT_RESULT = reason,
                INTERFACE_NODE = INTERFACE_NODE,
                CREATED_BY = "FBBCONFIGWEB",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }


        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd,
            string transactionId, string result, string reason, string intranID, string UpdateBy)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.IN_TRANSACTION_ID = intranID;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = reason;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;
            dbIntfCmd.UPDATED_BY = UpdateBy;
            _intfLogCommand.Handle(dbIntfCmd);
            ///  intfLogCommand.Handle(dbIntfCmd);
        }


        #endregion
        #region Export Excel File

        private string rptCriteria = " ACCESSNO: {0}  ORDERNO: {1}";
        private string rptName = "ReportInstallationCostbyOrder";

        public ActionResult ExportExcel(string dataS = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");
            var result = GetReportInstallationOrderListByPage(searchModel, 1, Decimal.MaxValue);
            decimal? sumtotal = 0; decimal? sumvat = 0; decimal? sumincvat = 0;
            var items = result.cur.FirstOrDefault();
            sumtotal = items.TOTAL_PAID_AMOUNT;
            sumvat = items.TOTAL_VAT;
            sumincvat = items.TOTAL_AMOUNT;

            string filename = "REPORTINSTALLATIONCostbyOrder";
            var bytes = GenerateEntitytoExcel(result.cur, filename, sumtotal, sumvat, sumincvat);

            return File(bytes, "application/octet-stream", filename + ".xlsx");
        }

        public string EmailTemplate(string fileName, ReportInstallationCostbyOrderModel searchModel)
        {

            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                StringBuilder tempBody = new StringBuilder();

                string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
                string orderStatus = searchModel.ORDER_STATUS == null ? "" : string.Join(",", searchModel.ORDER_STATUS);
                string workStatus = searchModel.WORK_STATUS == null ? "" : string.Join(",", searchModel.WORK_STATUS);


                #region tempBody

                tempBody.Append("<p style='font-weight:bolder;'>เรียน..." + CurrentUser.UserFullNameInThai + "</p>");
                tempBody.Append("<br/>");


                tempBody.Append("<span> ExportDate:" + DateNow.ToSafeString());
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<span>File Export Report Installation Cost by Order available for Download :" + fileName);
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                tempBody.Append("<a style='font-weight:bolder;'> Search Criteria By</a>");
                tempBody.Append("<br/>");
                if (searchModel.ACCESS_NO != "")
                {
                    tempBody.Append("<span>AccessNo :" + searchModel.ACCESS_NO);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");

                }
                if (searchModel.ORDER_NO != "")
                {
                    tempBody.Append("<span>OrderNo :" + searchModel.ORDER_NO);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.REGION != "")
                {
                    tempBody.Append("<span>Region :" + searchModel.REGION);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (productName != "")
                {
                    tempBody.Append("<span>Product Name :" + productName);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.ORG_ID != "")
                {
                    tempBody.Append("<span>ORG ID:" + searchModel.ORG_ID);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.SUBCONT_CODE != "")
                {
                    tempBody.Append("<span>Sub Contract Code:" + searchModel.SUBCONT_CODE);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.SUBCONT_TYPE != "")
                {
                    tempBody.Append("<span>Sub Contract Type:" + searchModel.SUBCONT_TYPE);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.SUBCONTSUB_TYPE != "")
                {
                    tempBody.Append("<span>Sub Contract Sub Type:" + searchModel.SUBCONTSUB_TYPE);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.ORD_TYPE != "")
                {
                    tempBody.Append("<span>Order Type:" + searchModel.ORD_TYPE);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.IR_DOC != "")
                {
                    tempBody.Append("<span>IR DOC Y/N:" + searchModel.IR_DOC);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.INVOICE_NO != "")
                {
                    tempBody.Append("<span>Invoice No:" + searchModel.INVOICE_NO);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (workStatus != "")
                {
                    tempBody.Append("<span>Work Flow Status:" + workStatus);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.ORD_STATUS != "")
                {
                    tempBody.Append("<span>Order Status:" + searchModel.ORD_STATUS);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.FOA_FM != "" && searchModel.FOA_TO != "")
                {
                    tempBody.Append("<span>FOA Date From :" + searchModel.FOA_FM + "-" + searchModel.FOA_TO);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.APPROVE_FM != "" && searchModel.APPROVE_TO != "")
                {
                    tempBody.Append("<span >CS Approve Date:" + searchModel.APPROVE_FM + "-" + searchModel.APPROVE_TO);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.PERIOD_FM != "" && searchModel.PERIOD_TO != "")
                {
                    tempBody.Append("<span>PERIOD Date:" + searchModel.PERIOD_FM + "-" + searchModel.PERIOD_TO);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                if (searchModel.TRANS_FM != "" && searchModel.TRANS_TO != "")
                {
                    tempBody.Append("<span>Tranfer Date:" + searchModel.TRANS_FM + "-" + searchModel.TRANS_FM);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                }
                tempBody.Append("<br/>");
                tempBody.Append("<span>To download your file");
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                var url = $"{Request.Url.GetLeftPart(UriPartial.Authority)}{Url.Content("~/")}";
                string urlName = url + "ReportInstallationCostbyOrder/DownloadFileExport?fileName=" + fileName;
                tempBody.Append("<a href='" + urlName + "'>Download Here</a>");

                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                tempBody.Append("<span>Thanks.");
                tempBody.Append("</span>");


                #endregion
                string body = "";
                body = tempBody.ToSafeString();

                return body;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error ExportFile: " + ex.GetErrorMessage());
                return ex.GetErrorMessage();
            }
        }




        public ActionResult DownloadFileExport(string fileName)
        {
            string tempPath = Path.GetTempPath();
            var filepath = Path.Combine(tempPath, fileName);
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception Ex)
            {
                _Logger.Info("DownLoad: " + Ex.GetErrorMessage());
                return null;
            }
            //finally
            //{
            //    System.IO.File.Delete(filepath);
            //}
        }


        public async Task ExportByEmail(string Email = "", string tab = "", string visibleColumn = "")
        {
            //
            ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");
            InterfaceLogCommand log = null;
            List<string> filteredColumns = null;
            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                _Logger.Info("ExportExcelByAsync" + DateNow.ToSafeString());
                if (!string.IsNullOrEmpty(visibleColumn))
                {
                    try
                    {
                        var visibleColumns = JsonConvert.DeserializeObject<List<string>>(visibleColumn);  // Declare once here

                        HashSet<string> uniqueColumns;

                        if (tab == "INSTALL")
                        {
                            uniqueColumns = new HashSet<string>(
                                visibleColumns.Where(c => !string.IsNullOrEmpty(c) &&
                                                          !string.Equals(c, "ACCESS_NO", StringComparison.OrdinalIgnoreCase) &&
                                                          !string.Equals(c, "ORDER_NO", StringComparison.OrdinalIgnoreCase)),
                                StringComparer.OrdinalIgnoreCase
                            );
                        }
                        else
                        {
                            uniqueColumns = new HashSet<string>(
                                visibleColumns.Where(c => !string.IsNullOrEmpty(c) &&
                                                          !string.Equals(c, "ACCESS_NUMBER", StringComparison.OrdinalIgnoreCase) &&
                                                          !string.Equals(c, "ORDER_NO", StringComparison.OrdinalIgnoreCase)),
                                StringComparer.OrdinalIgnoreCase
                            );
                        }

                        filteredColumns = uniqueColumns.ToList();
                    }
                    catch (JsonException jsonEx)
                    {
                        _Logger.Error("Failed to deserialize visibleColumn: " + jsonEx.Message);
                        filteredColumns = new List<string>();
                    }
                }

                Task<string> Logresult = null;
                log = StartInterface(searchModel, "EmailExport", "", null, "EmailExport", "Start EmailExport");

                ReportInstallationCostbyOrderListReturn result = await getAllRecord();
                decimal? sumtotal = 0; decimal? sumvat = 0; decimal? sumincvat = 0;

                var items = result.cur.FirstOrDefault();
                sumtotal = items.TOTAL_PAID_AMOUNT;
                sumvat = items.TOTAL_VAT;
                sumincvat = items.TOTAL_AMOUNT;
                string filename = "ReportInstallationCostbyOrder_" + CurrentUser.UserName.ToSafeString() + DateNow;

                GenerateEntitytoExcelAsync(result.cur, filename, sumtotal, sumvat, sumincvat, searchModel, Email, log, tab, filteredColumns);
                //HttpContext.Session["ExcelFileData"] = fileData;
                EndInterface(searchModel, log, null, "Success", "Send EmailComplete", "TotalRecord" + result.cur.Count.ToSafeString(), CurrentUser.UserName.ToSafeString());
                _Logger.Info("SuccessExportExcelByAsync" + DateNow.ToSafeString());
            }
            catch (Exception Ex)
            {
                EndInterface(searchModel, log, null, "Error", Ex.Message.ToSafeString(), null, "FBBConfig");
                _Logger.Info("Fail:" + Ex.Message.ToSafeString());
            }
        }

        private async Task<string> sendEmailExport(string fileName, ReportInstallationCostbyOrderModel searchModel, string Email, InterfaceLogCommand log)
        {
            try
            {
                _Logger.Info("Start SendEmail");
                string result = "";
                string body = "";

                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                body = EmailTemplate(fileName, searchModel);

                string sendto = string.Empty;
                if (!string.IsNullOrEmpty(Email))
                {
                    sendto = Email;
                }
                else
                {
                    sendto = CurrentUser.Email.ToSafeString();
                }
                var command = new SendMailReportInstallationNotificationCommand
                {
                    ProcessName = "SEND_EMAIL_FIXED_REPORTINSTALLATION",
                    Subject = "ExportFile Report Installation Success:" + DateNow,
                    Body = body,
                    SendTo = sendto
                };
                _sendMail.Handle(command);

                _Logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                if (command.ReturnMessage == "Success.")
                {
                    result = "Success";
                }
                else
                {
                    result = command.ReturnMessage;
                }

                return result;

            }
            catch (Exception Ex)
            {
                EndInterface(searchModel, log, null, "ERROR", Ex.Message.ToSafeString(), null, CurrentUser.UserName.ToSafeString());
                _Logger.Info("Fail:" + Ex.Message.ToSafeString());
                return "Email Send Fail";
            }
        }

        //Download File Excel
        [HttpGet]
        public FileResult DownloadExcel(string tab = "", string visibleColumn = "")
        {
            //ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");
            //InterfaceLogCommand log = null;
            //List<string> filteredColumns = null;
            try
            {
                //string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                //string fileName = "ReportInstallationCostbyOrder_" + CurrentUser.UserName.ToSafeString() + DateNow;
                ////// Generate Excel and get it as a byte array
                //byte[] fileData = HttpContext.Session["ExcelFileData"] as byte[]; //ตรงนี้จะมีโอกาสเกิด outoffmemory สูงมาก
                //return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
                string tempPath = Path.GetTempPath();
                var filepath = Session["ExcelFilePath"]as string;
            
                var fileName = Path.GetFileName(filepath);

                //if (!System.IO.File.Exists(filepath))
                //{
                //    return 
                //}

                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);


                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                _Logger.Error("DownloadExcel failed: " + ex.Message);
                return null; // Handle error more gracefully if needed
            }
        }


        private int getSheetPage(int Val)
        {
            if (Val == 0)
            {
                int sheet = 1;
                return (sheet);
            }
            else
            {
                string strVal = string.Format("{0:#,0.00}", Val); //.ToSafeString(),
                                                                  //string strVal = "32.11"; // will return 33
                                                                  // string strVal = "32.00" // returns 32
                                                                  // string strVal = "32.98" // returns 33
                string[] valStr = strVal.Split('.');

                int leftSide = Convert.ToInt32(valStr[0]);
                int rightSide = Convert.ToInt32(valStr[1]);

                if (rightSide > 0)
                    leftSide = leftSide + 1;


                return (leftSide);
            }

        }

        private static List<DataTable> SplitTable(DataTable originalTable, int batchSize)
        {
            List<DataTable> tables = new List<DataTable>();
            int i = 0;
            int j = 1;
            DataTable newDt = originalTable.Clone();
            newDt.TableName = "Table_" + j;
            newDt.Clear();
            foreach (DataRow row in originalTable.Rows)
            {
                DataRow newRow = newDt.NewRow();
                newRow.ItemArray = row.ItemArray;
                newDt.Rows.Add(newRow);
                i++;
                if (i == batchSize)
                {
                    tables.Add(newDt);
                    j++;
                    newDt = originalTable.Clone();
                    newDt.TableName = "Table_" + j;
                    newDt.Clear();
                    i = 0;
                }
            }
            if (newDt.Rows.Count > 0)
            {
                tables.Add(newDt);
                j++;
                newDt = originalTable.Clone();
                newDt.TableName = "Table_" + j;
                newDt.Clear();

            }

            return tables;
        }
        private async Task<string> GenerateEntitytoExcelAsync<T>(List<T> data, string fileName, decimal? sumtotal, decimal? sumvat, decimal? sumincvat, ReportInstallationCostbyOrderModel searchModel, string Email, InterfaceLogCommand log, string tab, List<string> filteredColumns)
        {
            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                _Logger.Info("StartGenerateEntitytoExcelAsync" + DateNow.ToSafeString());
                //_Logger.Info("Start GenerateSuccess");
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
                DataTable table = new DataTable();
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name);
                }

                object[] values = new object[props.Count];
                foreach (T item in data)
                {

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);


                    }

                    table.Rows.Add(values);

                }

                string _userGroup = GetUserGroup();
                int rowperSheet = 0;

                var resData = SelectFbbCfgLov("EMAILEXPORT_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                    .Where(d => d.LOV_VAL1 != null).ToList();
                if (resData != null)
                {
                    var sheetData = resData.FirstOrDefault();
                    if (sheetData.LOV_VAL1.ToSafeString() != "" || sheetData.LOV_VAL1.ToSafeString() != null)
                    {
                        rowperSheet = Convert.ToInt32(sheetData.LOV_VAL1.ToSafeString());
                    }
                    else
                    {
                        rowperSheet = 10000;
                    }
                }
                else
                {
                    rowperSheet = 10000;
                }

                table.Columns.Remove("CNT");

                if (_userGroup == "SCM")
                {
                    table = ScmOrderExcelColumnWithtab(table, tab, filteredColumns);
                }
                else if (_userGroup == "FAPO")
                {

                    table = FapoOrderExcelColumnWithtab(table, tab, filteredColumns);

                }
                else if (_userGroup == "ACCT")
                {
                    //DataRow dr;
                    //dr = table.NewRow();
                    //dr["RECAL_RATE"] = "Total Amount:";
                    //dr["TOTAL_PAID"] = string.Format("{0:#,0.00}", sumtotal);
                    //dr["INVOICE_AMOUNT_VAT"] = string.Format("{0:#,0.00}", sumvat);
                    //dr["TOTAL_INVOICE_AMOUNT_VAT"] = string.Format("{0:#,0.00}", sumincvat);
                    //table.Rows.Add(dr);
                    table = AcctOrderExcelColumnWithtab(table, tab, filteredColumns);
                }

                string tempPath = Path.GetTempPath();


                string tempFilePath = Path.Combine(tempPath, fileName + ".csv");

                Session["ExcelFilePath"] = tempFilePath;

                //Delete existing file with same file name.
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }

                var newFile = new FileInfo(tempFilePath);
                reportInstallScmOrderListModelBySendMail scmOrderListModelBySendMail = new reportInstallScmOrderListModelBySendMail();

                int iRow;
                int iHeaderRow;
                string strRow;
                string strColumn1 = string.Empty;
                int iCol = table.Columns.Count;
                _Logger.Info("StartSplitTable" + DateNow.ToSafeString());
                List<DataTable> splitdt = SplitTable(table, rowperSheet);
                _Logger.Info("EndSplitTable" + DateNow.ToSafeString());
                //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
                _Logger.Info("StartSplitSheet" + DateNow.ToSafeString());

                //using (var package = new ExcelPackage())
                //{


                for (int i = 0; i < splitdt.Count; i++)
                {


                    var data_byte = GenerateCSV(splitdt[i], tempFilePath);

                    //using (FileStream fs = new FileStream(tempPath.Trim() + splitdt[i].TableName +"_"+ fileName.Trim(), FileMode.OpenOrCreate))
                    //{
                    //    new MemoryStream(data_byte).CopyTo(fs);
                    //    fs.Flush();
                    //}
                    //int page = 0;
                    //page = i + 1;
                    ////Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                    //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet" + page);
                    //iRow = 1;
                    //iHeaderRow = iRow + 1;
                    //strRow = iRow.ToSafeString();

                    //ExcelRange rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                    //rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //rangeHeader.Style.Font.Bold = true;
                    //rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                    //rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //worksheet.View.FreezePanes(iHeaderRow, 1);
                    //strColumn1 = string.Format("A{0}", strRow);
                    //// Add a new worksheet to ExcelPackage object and give a suitable name
                    ////ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet" + page);

                    ////worksheet.Cells.LoadFromDataTable(splitdt[i], true);
                    ////worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    //worksheet.Cells[strColumn1].LoadFromDataTable(splitdt[i], true, TableStyles.None);
                    //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();


                    //package.Workbook.Properties.Title = "FBB Config";
                    //package.Workbook.Properties.Author = "FBB";
                    //package.Workbook.Properties.Subject = "Sheet";
                }

                //scmOrderListModelBySendMail.FileData = package.GetAsByteArray();
                //HttpContext.Session["ExcelFileData"] = scmOrderListModelBySendMail.FileData;

                //// Set file properties
                //package.Workbook.Properties.Title = "FBB Config";
                //package.Workbook.Properties.Author = "FBB";
                //package.Workbook.Properties.Subject = "Sheet";
                //using (FileStream fs = new FileStream(tempPath.Trim() + fileName.Trim(), FileMode.OpenOrCreate))
                //{
                //    new MemoryStream(scmOrderListModelBySendMail.FileData).CopyTo(fs);
                //    fs.Flush();
                //}

                // Return the generated Excel as a byte array
                _Logger.Info("EndFileStream" + DateNow.ToSafeString());
                _Logger.Info("GenerateSuccess");
                _Logger.Info("StartSendEmail" + DateNow.ToSafeString());
                sendEmailExport(fileName + ".csv", searchModel, Email, log);


                //}
                //scmOrderListModelBySendMail.msExcel = new MemoryStream(scmOrderListModelBySendMail.FileData);
                // _Logger.Info("EndSplitSheet" + DateNow.ToSafeString());
                //_Logger.Info("StartFileStream" + DateNow.ToSafeString());

                return "Success";
            }
            catch (Exception Ex)
            {
                EndInterface(searchModel, log, null, "ERROR", Ex.Message.ToSafeString(), null, CurrentUser.UserName.ToSafeString());
                _Logger.Info("Error  GenerateFail" + Ex.GetBaseException());
                return "Fail";
            }
        }

        public byte[] GenerateEntitytoExcel<T>(List<T> data, string fileName, decimal? sumtotal, decimal? sumvat, decimal? sumincvat)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name);
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            string _userGroup = GetUserGroup();
            table.Columns.Remove("CNT");

            if (_userGroup == "SCM")
            {
                table = ScmOrderExcelColumn(table);
            }
            else if (_userGroup == "FAPO")
            {

                table = FapoOrderExcelColumn(table);

            }
            else if (_userGroup == "ACCT")
            {
                DataRow dr;
                dr = table.NewRow();
                dr["RECAL_RATE"] = "Total Amount:";
                dr["TOTAL_PAID"] = string.Format("{0:#,0.00}", sumtotal);
                dr["INVOICE_AMOUNT_VAT"] = string.Format("{0:#,0.00}", sumvat);
                dr["TOTAL_INVOICE_AMOUNT_VAT"] = string.Format("{0:#,0.00}", sumincvat);
                // dr["TOTAL_PAID"] = quantity;
                table.Rows.Add(dr);
                table = AcctOrderExcelColumn(table);
                // table = ScmOrderExcelColumn(table);
            }

            string tempPath = Path.GetTempPath();

            //var data_ = GenerateExcel(table, "WorkSheet", tempPath, fileName);
            var data_ = GenerateCSV(table, tempPath);
            return data_;
        }


        private DataTable ScmOrderExcelColumn(DataTable table)
        {
            table.Columns.Remove("RowNumber");
            table.Columns.Remove("DISTANCE_LASTMILE_APP");
            table.Columns.Remove("DISTANCE_ESRI");
            table.Columns.Remove("DISTANCE_GMAP");
            table.Columns.Remove("DISTANCE_STRAIT");
            table.Columns.Remove("DISTANCE_VALIDATE");
            table.Columns.Remove("DISTANCE_TOTAL");
            table.Columns.Remove("Distance_MORE_325");
            table.Columns.Remove("LASTMILE_PRICE");
            table.Columns.Remove("ORDER_FEE");
            table.Columns.Remove("LAST_UPDATE_DATE");
            table.Columns.Remove("LAST_UPDATE_BY");
            table.Columns.Remove("PERIOD");
            table.Columns.Remove("ORDER_STATUS_DT");
            table.Columns.Remove("APPOINTMENNT_DT");
            table.Columns.Remove("EFFECTIVE_END_DT");
            table.Columns.Remove("CREAETED_BY");
            table.Columns.Remove("LAST_UPDATED_BY");
            table.Columns.Remove("INVOICE_DATE");
            table.Columns.Remove("CS_APPROVE_DATE");
            table.Columns.Remove("LAST_UPDATE_DATE_TEXT");
            table.Columns.Remove("EFFECTIVE_END_DT_TEXT");
            table.Columns.Remove("SUB_MAIL");
            table.Columns.Remove("SENDMAIL_LIST");
            table.Columns.Remove("SFF_ACTIVE_DATE");
            table.Columns.Remove("SFF_SUBMITTED_DATE");
            //     table.Columns.Remove("REUSE_FLAG");

            table.Columns.Remove("DISPUTE_DISTANCE");
            table.Columns.Remove("TOTAL_DISTANCE");
            table.Columns.Remove("DIFF_DISTANCE");

            table.Columns.Remove("DISTANCE_PAID");
            table.Columns.Remove("SUBCONTRACT_NAME");
            table.Columns.Remove("PAY_PERIOD");
            // table.Columns.Remove("INVOICE_NO");
            table.Columns.Remove("INVOICE_DATE_TEXT");
            //table.Columns.Remove("IR_DOC");
            table.Columns.Remove("LENGTH_DISTANCE");
            //  table.Columns.Remove("TOTAL_COST");
            table.Columns.Remove("ORG_ID");
            table.Columns.Remove("ADDR_NAME_TH");
            table.Columns.Remove("FOA_SUBMIT_DATE");
            table.Columns.Remove("INVOICE_AMOUNT_BFVAT");
            table.Columns.Remove("INVOICE_AMOUNT_VAT");
            table.Columns.Remove("TOTAL_INVOICE_AMOUNT_VAT");
            table.Columns.Remove("FOA_SUBMIT_DATE_TEXT");
            table.Columns.Remove("PAID_DATE");
            table.Columns.Remove("PAID_DATE_TEXT");
            table.Columns.Remove("APPROVE_FLAG");
            table.Columns.Remove("TOTAL_PAID_AMOUNT");
            table.Columns.Remove("TOTAL_VAT");
            table.Columns.Remove("TOTAL_AMOUNT");
            table.Columns.Remove("NOTE");
            table.Columns.Remove("USER_ID");
            table.Columns.Remove("INV_GRP");
            table.Columns.Remove("ACCESS_NUMBER");
            SetColumnsOrder(table,
                "ACCESS_NUMBER_MASKING",
                "ACCOUNT_NAME",
                "SBC_CPY",
                "PRODUCT_NAME",
                "ON_TOP1",
                "ON_TOP2",
                "VOIP_NUMBER",
                "PROMOTION_NAME",
                "ORDER_NO",
                "ORD_TYPE",
                "ORDER_SFF",
                "APPOINTMENNT_DT_TEXT",
                "CS_APPROVE_DATE_TEXT",
                "ORDER_STATUS_DT_TEXT",

                "REJECT_REASON",
                "MATERIAL_CODE_CPESN",
                "CPE_SN",
                "CPE_MODE",
                "MATERIAL_CODE_STBSN",
                "STB_SN",
                "MATERIAL_CODE_ATASN",
                "ATA_SN",
                "MATERIAL_CODE_WIFIROUTESN",
                "WIFI_ROUTER_SN",
                "STO_LOCATION",
                "SUBCONTRACT_CODE",
                "FOA_REJECT_REASON",
                "RE_APPOINTMENT_REASON",
                "PHASE_PO",
                  "SFF_ACTIVE_DATE_TEXT",
                "SFF_SUBMITTED_DATE_TEXT",
                "EVENT_CODE",
                "REGION",
                "ENTRY_FEE",
                "FEE_CODE",
                "ADDR_ID",
            //  "ADDR_NAME_TH",
            "INSTALLATION_ADDRESS",
            "SUBCONTRACT_TYPE",
            "SUBCONTRACT_SUB_TYPE",
            "REQUEST_DISTANCE",
            "APPROVE_DISTANCE",
            "APPROVE_STAFF",
            "APPROVE_STATUS",
            "REUSED_FLAG",
            "REQUEST_SUB_FLAG",
                // "DISTANCE_PAID",
                "OUTDOOR_COST",
                "INDOOR_COST",
                  "MAPPING_COST",
            "OVER_LENGTH",
            "OVER_COST",
            "TOTAL_COST",
            "RECAL_DIS",
                "RECAL_OVER_LENGTH",
                "RECAL_OVER_COST",
                "RECAL_MAPPING_COST",
                "RECAL_RATE",
                 "MAX_LENGTH",
                "TOTAL_PAID",

                "ORDER_STATUS",
                "RULE_ID",
                "REMARK",
                  "OM_ORDER_STATUS",
                "IR_DOC",
                "INVOICE_NO",
                "SUBCONTRACT_LOCATION"
                );


            GetScmOrderExcelCaption(table);

            return table;
        }
        private DataTable FapoOrderExcelColumn(DataTable table)
        {
            table.Columns.Remove("RowNumber");
            //  table.Columns.Remove("ORDER_NO");
            //   table.Columns.Remove("REUSE_FLAG");
            table.Columns.Remove("SUBCONTRACT_CODE");
            table.Columns.Remove("PAY_PERIOD");
            table.Columns.Remove("CS_APPROVE_DATE");
            table.Columns.Remove("INVOICE_NO");
            table.Columns.Remove("INVOICE_DATE");
            table.Columns.Remove("IR_DOC");
            //  table.Columns.Remove("REMARK");
            table.Columns.Remove("NOTE");
            table.Columns.Remove("LAST_UPDATED_BY");
            table.Columns.Remove("PERIOD");
            table.Columns.Remove("ORDER_STATUS_DT");
            table.Columns.Remove("ACCOUNT_NAME");
            table.Columns.Remove("APPOINTMENNT_DT");
            table.Columns.Remove("PROMOTION_NAME");
            table.Columns.Remove("LENGTH_DISTANCE");
            // table.Columns.Remove("OUTDOOR_COST");
            //  table.Columns.Remove("INDOOR_COST");
            //  table.Columns.Remove("TOTAL_COST");

            table.Columns.Remove("ORDER_FEE");
            table.Columns.Remove("PRODUCT_NAME");
            table.Columns.Remove("EFFECTIVE_END_DT");
            table.Columns.Remove("CREAETED_BY");
            //  table.Columns.Remove("LAST_UPDATED_DATE");
            table.Columns.Remove("CS_APPROVE_DATE_TEXT");
            table.Columns.Remove("INVOICE_DATE_TEXT");
            table.Columns.Remove("ORDER_STATUS_DT_TEXT");
            table.Columns.Remove("APPOINTMENNT_DT_TEXT");
            table.Columns.Remove("EFFECTIVE_END_DT_TEXT");
            table.Columns.Remove("SUB_MAIL");
            table.Columns.Remove("SENDMAIL_LIST");
            table.Columns.Remove("SBC_CPY");
            table.Columns.Remove("ON_TOP1");
            table.Columns.Remove("ON_TOP2");
            table.Columns.Remove("VOIP_NUMBER");
            table.Columns.Remove("ORD_TYPE");
            table.Columns.Remove("ORDER_SFF");
            table.Columns.Remove("SFF_ACTIVE_DATE");
            table.Columns.Remove("SFF_ACTIVE_DATE_TEXT");
            //   table.Columns.Remove("OM_ORDER_STATUS");
            table.Columns.Remove("REJECT_REASON");
            table.Columns.Remove("MATERIAL_CODE_CPESN");
            table.Columns.Remove("CPE_SN");
            table.Columns.Remove("CPE_MODE");
            table.Columns.Remove("MATERIAL_CODE_STBSN");
            table.Columns.Remove("STB_SN");
            table.Columns.Remove("MATERIAL_CODE_ATASN");
            table.Columns.Remove("ATA_SN");
            table.Columns.Remove("MATERIAL_CODE_WIFIROUTESN");
            table.Columns.Remove("WIFI_ROUTER_SN");
            table.Columns.Remove("STO_LOCATION");
            table.Columns.Remove("FOA_REJECT_REASON");
            table.Columns.Remove("RE_APPOINTMENT_REASON");
            table.Columns.Remove("PHASE_PO");
            table.Columns.Remove("SFF_SUBMITTED_DATE");
            table.Columns.Remove("SFF_SUBMITTED_DATE_TEXT");
            table.Columns.Remove("EVENT_CODE");
            table.Columns.Remove("REGION");
            table.Columns.Remove("FEE_CODE");
            table.Columns.Remove("ADDR_ID");
            table.Columns.Remove("ADDR_NAME_TH");
            table.Columns.Remove("ORG_ID");
            table.Columns.Remove("DIFF_DISTANCE");
            //  table.Columns.Remove("DISPUTE_DISTANCE");
            table.Columns.Remove("TOTAL_DISTANCE");
            //table.Columns.Remove("FOA_SUBMIT_DATE");
            table.Columns.Remove("LAST_UPDATE_DATE");
            table.Columns.Remove("DISTANCE_PAID");
            table.Columns.Remove("Distance_MORE_325");
            table.Columns.Remove("INSTALLATION_ADDRESS");
            table.Columns.Remove("LASTMILE_PRICE");
            //  table.Columns.Remove("RECAL_MAPPING_COST");
            table.Columns.Remove("MAX_LENGTH");
            table.Columns.Remove("INVOICE_AMOUNT_BFVAT");
            table.Columns.Remove("INVOICE_AMOUNT_VAT");
            table.Columns.Remove("TOTAL_INVOICE_AMOUNT_VAT");
            table.Columns.Remove("FOA_SUBMIT_DATE_TEXT");
            table.Columns.Remove("PAID_DATE");
            table.Columns.Remove("PAID_DATE_TEXT");
            table.Columns.Remove("APPROVE_FLAG");
            table.Columns.Remove("TOTAL_PAID_AMOUNT");
            table.Columns.Remove("TOTAL_VAT");
            table.Columns.Remove("TOTAL_AMOUNT");
            table.Columns.Remove("INV_GRP");
            table.Columns.Remove("ACCESS_NUMBER");

            // new remove
            table.Columns.Remove("DISTANCE_LASTMILE_APP");
            table.Columns.Remove("DISTANCE_ESRI");
            table.Columns.Remove("DISTANCE_GMAP");
            table.Columns.Remove("DISTANCE_STRAIT");
            table.Columns.Remove("DISPUTE_DISTANCE");
            table.Columns.Remove("DISTANCE_VALIDATE");
            table.Columns.Remove("REQUEST_DISTANCE");
            table.Columns.Remove("APPROVE_DISTANCE");
            table.Columns.Remove("APPROVE_STAFF");
            table.Columns.Remove("APPROVE_STATUS");
            table.Columns.Remove("TOTAL_COST");
            table.Columns.Remove("RECAL_DIS");
            table.Columns.Remove("RECAL_RATE");
            table.Columns.Remove("RECAL_MAPPING_COST");
            table.Columns.Remove("TOTAL_PAID");
            table.Columns.Remove("RULE_ID");
            table.Columns.Remove("OM_ORDER_STATUS");
            table.Columns.Remove("REMARK");
            table.Columns.Remove("USER_ID");
            table.Columns.Remove("SUBCONTRACT_LOCATION");
            table.Columns.Remove("MAPPING_COST");
            table.Columns.Remove("OUTDOOR_COST");
            table.Columns.Remove("INDOOR_COST");
            table.Columns.Remove("ENTRY_FEE");
            table.Columns.Remove("COMPLETE_DATE");
            SetColumnsOrder(table,
                "ORDER_STATUS",
                "ACCESS_NUMBER_MASKING",
                "ORDER_NO",//ORDER_NO
                "ORDER_TYPE",//ORDER_TYPE
                "FOA_SUBMIT_DATE",//COMPLETE_DATE
                "SUBCONTRACT_NAME",
                "SUBCONTRACT_TYPE",
                "SUBCONTRACT_SUB_TYPE",
                "LOOKUP_ID",//
                "LOOKUP_NAME",//
                "ONTOP_LOOKUP_ID",//
                "ONTOP_LOOKUP_NAME",//
                "DISTANCE_TOTAL",
                "BASE_COST",//
                "OVER_LENGTH",
                "OVER_COST",
                "TOTAL_SOA",//
                "RECAL_LOOKUP_ID",
                "RECAL_ONTOP_LOOKUP_ID",//
                "RECAL_DISTANCE",//
                "RECAL_COST",//
                "RECAL_OVER_LENGTH",
                "RECAL_OVER_COST",
                "TOTAL_RECAL",//
                "TOTAL_DISPUTE",//
                "REUSED_FLAG",
                "REQUEST_SUB_FLAG",
                "TOTAL_FEE",//
                "LAST_UPDATE_DATE_TEXT",
                "LAST_UPDATE_BY"
                );

            GetFapoOrderExcelCaption(table);

            return table;
        }
        private DataTable AcctOrderExcelColumn(DataTable table)
        {
            // table.Columns.Remove("RowNumber");
            // table.Columns.Remove("RowNumber");
            table.Columns.Remove("USER_ID");
            //table.Columns.Remove("REUSED_FLAG");
            table.Columns.Remove("REQUEST_SUB_FLAG");
            table.Columns.Remove("INVOICE_DATE");
            table.Columns.Remove("IR_DOC");
            table.Columns.Remove("NOTE");
            table.Columns.Remove("DISTANCE_LASTMILE_APP");
            table.Columns.Remove("DISTANCE_ESRI");
            table.Columns.Remove("DISTANCE_GMAP");
            table.Columns.Remove("DISTANCE_STRAIT");
            table.Columns.Remove("DISTANCE_VALIDATE");
            //table.Columns.Remove("DISTANCE_TOTAL");
            table.Columns.Remove("DISTANCE_PAID");
            table.Columns.Remove("Distance_MORE_325");
            table.Columns.Remove("LASTMILE_PRICE");
            //      table.Columns.Remove("ORDER_FEE");
            table.Columns.Remove("LAST_UPDATE_DATE");
            table.Columns.Remove("LAST_UPDATE_BY");
            table.Columns.Remove("PERIOD");
            // table.Columns.Remove("TOTAL_COST");
            table.Columns.Remove("EFFECTIVE_END_DT");
            table.Columns.Remove("CREAETED_BY");
            table.Columns.Remove("LAST_UPDATED_BY");
            //      table.Columns.Remove("ORDER_STATUS_DT");
            table.Columns.Remove("APPOINTMENNT_DT");

            table.Columns.Remove("LAST_UPDATE_DATE_TEXT");
            table.Columns.Remove("EFFECTIVE_END_DT_TEXT");
            table.Columns.Remove("SUB_MAIL");
            table.Columns.Remove("SENDMAIL_LIST");
            table.Columns.Remove("SBC_CPY");
            table.Columns.Remove("ON_TOP1");
            table.Columns.Remove("ON_TOP2");
            table.Columns.Remove("VOIP_NUMBER");
            table.Columns.Remove("ORD_TYPE");

            table.Columns.Remove("SFF_ACTIVE_DATE");
            table.Columns.Remove("SFF_ACTIVE_DATE_TEXT");

            table.Columns.Remove("REJECT_REASON");
            table.Columns.Remove("MATERIAL_CODE_CPESN");
            table.Columns.Remove("CPE_SN");
            table.Columns.Remove("CPE_MODE");
            table.Columns.Remove("MATERIAL_CODE_STBSN");
            table.Columns.Remove("STB_SN");
            table.Columns.Remove("MATERIAL_CODE_ATASN");
            table.Columns.Remove("ATA_SN");
            table.Columns.Remove("MATERIAL_CODE_WIFIROUTESN");
            table.Columns.Remove("WIFI_ROUTER_SN");
            table.Columns.Remove("STO_LOCATION");
            table.Columns.Remove("FOA_REJECT_REASON");
            table.Columns.Remove("RE_APPOINTMENT_REASON");
            //table.Columns.Remove("PHASE_PO");
            table.Columns.Remove("SFF_SUBMITTED_DATE");
            table.Columns.Remove("SFF_SUBMITTED_DATE_TEXT");

            table.Columns.Remove("REGION");
            table.Columns.Remove("FEE_CODE");
            // table.Columns.Remove("ADDR_ID");
            table.Columns.Remove("ADDR_NAME_TH");
            table.Columns.Remove("ORG_ID");

            table.Columns.Remove("MAX_LENGTH");
            table.Columns.Remove("REQUEST_DISTANCE");

            table.Columns.Remove("APPROVE_DISTANCE");
            table.Columns.Remove("APPROVE_STAFF");

            table.Columns.Remove("APPROVE_STATUS");
            table.Columns.Remove("CS_APPROVE_DATE");

            table.Columns.Remove("ORDER_FEE");
            table.Columns.Remove("DISPUTE_DISTANCE");
            table.Columns.Remove("TOTAL_DISTANCE");
            table.Columns.Remove("ORDER_STATUS_DT");

            table.Columns.Remove("APPOINTMENNT_DT_TEXT");
            table.Columns.Remove("FOA_SUBMIT_DATE_TEXT");
            table.Columns.Remove("DIFF_DISTANCE");
            table.Columns.Remove("RECAL_DIS");

            // table.Columns.Remove("ORDER_STATUS");
            table.Columns.Remove("PAID_DATE");
            table.Columns.Remove("APPROVE_FLAG");
            table.Columns.Remove("TOTAL_PAID_AMOUNT");
            table.Columns.Remove("TOTAL_VAT");
            table.Columns.Remove("TOTAL_AMOUNT");
            // table.Columns.Remove("RowNumber");

            table.Columns.Remove("INVOICE_AMOUNT_BFVAT");
            table.Columns.Remove("ACCESS_NUMBER");
            //SetColumnsOrder(table,
            //    "RowNumber",

            //   "ORDER_STATUS",
            //    "PAY_PERIOD",
            //     "ORDER_STATUS_DT_TEXT",
            //     "ACCESS_NUMBER_MASKING",
            //     "ACCOUNT_NAME",
            //     "PROMOTION_NAME",
            //     "SUBCONTRACT_NAME",
            //     "LENGTH_DISTANCE",
            //     "OUTDOOR_COST",
            //     "INDOOR_COST",
            //     "MAPPING_COST",
            //     "OVER_LENGTH",
            //     "OVER_COST",
            //     "TOTAL_COST",
            //     "ENTRY_FEE",
            //     "ORDER_NO",
            //     "PRODUCT_NAME",
            //     "SUBCONTRACT_CODE",
            //     "INVOICE_DATE_TEXT",
            //     "INVOICE_NO",
            //     "REMARK",
            //     "PAID_DATE_TEXT",
            //     "OM_ORDER_STATUS",
            //     "ORDER_SFF",
            //     "FOA_SUBMIT_DATE",
            //     "CS_APPROVE_DATE_TEXT",
            //     "EVENT_CODE",
            //     "INSTALLATION_ADDRESS",
            //     "SUBCONTRACT_TYPE",
            //     "SUBCONTRACT_SUB_TYPE",
            //     "RECAL_OVER_LENGTH",
            //     "RECAL_OVER_COST",
            //     "RECAL_MAPPING_COST",
            //     "RECAL_RATE",
            //    "TOTAL_PAID",
            //    // "INVOICE_AMOUNT_BFVAT",   //BEVAT
            //    "INVOICE_AMOUNT_VAT", //Vat7%
            //    "TOTAL_INVOICE_AMOUNT_VAT", //IncludeVat
            //    "RULE_ID",
            //      "INV_GRP",
            //   "ADDR_ID",
            //   "PHASE_PO",

            //   "SUBCONTRACT_LOCATION"


            //    );

            SetColumnsOrder(table,
                "RowNumber",

               "ORDER_STATUS",
                "PAY_PERIOD",
                 "ORDER_STATUS_DT_TEXT",
                 "ACCESS_NUMBER_MASKING",
                 "ACCOUNT_NAME",
                 "PROMOTION_NAME",
                 "SUBCONTRACT_NAME",
                 "LENGTH_DISTANCE",
                 "OUTDOOR_COST",
                 "INDOOR_COST",
                 "MAPPING_COST",
                 "OVER_LENGTH",
                 "OVER_COST",
                 "TOTAL_COST",
                 "ENTRY_FEE",
                 "ORDER_NO",
                 "PRODUCT_NAME",
                 "SUBCONTRACT_CODE",
                 "INVOICE_DATE_TEXT",
                 "INVOICE_NO",
                 "REMARK",
                 "PAID_DATE_TEXT",
                 "OM_ORDER_STATUS",
                 "ORDER_SFF",
                 "FOA_SUBMIT_DATE",
                 "CS_APPROVE_DATE_TEXT",
                 "EVENT_CODE",
                 "INSTALLATION_ADDRESS",
                 "SUBCONTRACT_TYPE",
                 "SUBCONTRACT_SUB_TYPE",
                 "RECAL_OVER_LENGTH",
                 "RECAL_OVER_COST",
                 "RECAL_MAPPING_COST",
                 "RECAL_RATE",
                "TOTAL_PAID",
                // "INVOICE_AMOUNT_BFVAT",   //BEVAT
                "INVOICE_AMOUNT_VAT", //Vat7%
                "TOTAL_INVOICE_AMOUNT_VAT", //IncludeVat
                "RULE_ID",
                  "INV_GRP",
               "ADDR_ID",
               "PHASE_PO",

               "SUBCONTRACT_LOCATION"


                );

            GetAcctOrderExcelCaption(table);

            return table;
        }

        private DataTable ScmOrderExcelColumnWithtab(DataTable table, string tab, List<string> filteredColumns)
        {
            if (filteredColumns != null && filteredColumns.Any())
            {
                var columnsToRemove = table.Columns.Cast<DataColumn>()
                                                   .Where(column => !filteredColumns.Contains(column.ColumnName))
                                                   .ToList();

                foreach (var column in columnsToRemove)
                {
                    table.Columns.Remove(column);
                }

                SetColumnsOrder(table, filteredColumns.ToArray());
            }

            if (tab == "INSTALL")
            {
                GetScmOrderExcelCaption(table);
            }
            else if (tab == "MA")
            {
                GetScmOrderExcelCaptionMA(table);
            }

            return table;
        }
        private DataTable FapoOrderExcelColumnWithtab(DataTable table, string tab, List<string> filteredColumns)
        {
            if (filteredColumns != null && filteredColumns.Any())
            {
                var columnsToRemove = table.Columns.Cast<DataColumn>()
                                                   .Where(column => !filteredColumns.Contains(column.ColumnName))
                                                   .ToList();

                foreach (var column in columnsToRemove)
                {
                    table.Columns.Remove(column);
                }

                SetColumnsOrder(table, filteredColumns.ToArray());
            }

            if (tab == "INSTALL")
            {
                GetFapoOrderExcelCaption(table);
            }
            else if (tab == "MA")
            {
                GetFapoOrderExcelCaptionMA(table);
            }

            return table;
        }
        private DataTable AcctOrderExcelColumnWithtab(DataTable table, string tab, List<string> filteredColumns)
        {
            if (filteredColumns != null && filteredColumns.Any())
            {
                var columnsToRemove = table.Columns.Cast<DataColumn>()
                                                   .Where(column => !filteredColumns.Contains(column.ColumnName))
                                                   .ToList();

                foreach (var column in columnsToRemove)
                {
                    table.Columns.Remove(column);
                }

                SetColumnsOrder(table, filteredColumns.ToArray());
            }

            if (tab == "INSTALL")
            {
                GetAcctOrderExcelCaption(table);
            }
            else if (tab == "MA")
            {
                GetAcctOrderExcelCaptionMA(table);
            }

            return table;
        }

        private void GetScmOrderExcelCaptionMA(DataTable table)
        {
            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_SCM_REPORT_INSTALLATION"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();

            void SetColumnCaption(string columnName, string lovName)
            {
                try
                {
                    var caption = configscreen.FirstOrDefault(f => f.Name == lovName)?.LovValue1 ?? columnName;
                    table.Columns[columnName].Caption = caption.ToUpper();
                }
                catch (Exception ex)
                {
                    _Logger.Error($"Failed to set caption for column '{columnName}': {ex.Message}");
                }
            }
            SetColumnCaption("ORDER_STATUS", "L_Status");
            SetColumnCaption("ACCESS_NO", "L_ACC_NBR");
            SetColumnCaption("ORDER_NO_SFF", "L_ORD_NO");
            SetColumnCaption("ORDER_TYPE", "L_ORD_TYPE");
            SetColumnCaption("SOA_SUBMIT_DATE_TEXT", "L_SOA_SUBMIT_DATE");
            SetColumnCaption("SUBCONTRACT_NAME", "L_SUBCONTRACT_NAME");
            SetColumnCaption("SUBCONTRACT_TYPE", "L_SUB_CONTRACT_TYPE");
            SetColumnCaption("SUBCONTRACT_SUB_TYPE", "L_SUB_CONTRACT_SUB_TYPE");
            SetColumnCaption("LOOKUP_ID", "L_LOOKUP_ID");
            SetColumnCaption("LOOKUP_NAME", "L_LOOKUP_NAME");
            SetColumnCaption("LOOKUP_COST", "L_LOOKUP_COST");
            SetColumnCaption("ONTOP_LOOKUP_ID", "L_ONTOP_LOOKUP_ID");
            SetColumnCaption("ONTOP_LOOKUP_NAME", "L_ONTOP_LOOKUP_NAME");
            SetColumnCaption("ONTOP_COST", "L_ONTOP_COST");
            SetColumnCaption("BASE_COST", "L_BASE_COST");
            SetColumnCaption("RECAL_COST", "L_RECAL_COST");
            SetColumnCaption("SYMPTOM_GROUP", "L_SYMPTOM_GROUP");
            SetColumnCaption("SYMPTOM_NAME", "L_SYMPTOM_NAME");
            SetColumnCaption("MODIFY_DATE_TEXT", "L_MODIFY_DATE");
            SetColumnCaption("MODIFY_BY", "L_MODIFY_BY");
        }
        private void GetFapoOrderExcelCaptionMA(DataTable table)
        {
            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_FAPO_REPORT_INSTALLATION"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();
            void SetColumnCaption(string columnName, string lovName)
            {
                try
                {
                    var caption = configscreen.FirstOrDefault(f => f.Name == lovName)?.LovValue1 ?? columnName;
                    table.Columns[columnName].Caption = caption.ToUpper();
                }
                catch (Exception ex)
                {
                    _Logger.Error($"Failed to set caption for column '{columnName}': {ex.Message}");
                }
            }
            SetColumnCaption("ORDER_STATUS", "L_STATUS");
            SetColumnCaption("ACCESS_NO", "L_INTERNET_NO");
            SetColumnCaption("ORDER_NO_SFF", "L_ORDER_NO");
            SetColumnCaption("ORDER_TYPE", "L_ORDER_TYPE");
            SetColumnCaption("SOA_SUBMIT_DATE_TEXT", "L_SOA_SUBMIT_DATE");
            SetColumnCaption("SUBCONTRACT_NAME", "L_SUBCONTRACT_NAME");
            SetColumnCaption("SUBCONTRACT_TYPE", "L_SUB_CONTRACT_TYPE");
            SetColumnCaption("SUBCONTRACT_SUB_TYPE", "L_SUB_CONTRACT_SUB_TYPE");
            SetColumnCaption("LOOKUP_ID", "L_LOOKUP_ID");
            SetColumnCaption("LOOKUP_NAME", "L_LOOKUP_NAME");
            SetColumnCaption("LOOKUP_COST", "L_LOOKUP_COST");
            SetColumnCaption("ONTOP_LOOKUP_ID", "L_ONTOP_LOOKUP_ID");
            SetColumnCaption("ONTOP_LOOKUP_NAME", "L_ONTOP_LOOKUP_NAME");
            SetColumnCaption("ONTOP_COST", "L_ONTOP_COST");
            SetColumnCaption("BASE_COST", "L_BASE_COST");
            SetColumnCaption("RECAL_COST", "L_RECAL_COST");
            SetColumnCaption("SYMPTOM_GROUP", "L_SYMPTOM_GROUP");
            SetColumnCaption("SYMPTOM_NAME", "L_SYMPTOM_NAME");
            SetColumnCaption("MODIFY_DATE_TEXT", "L_MODIFY_DATE");
            SetColumnCaption("MODIFY_BY", "L_MODIFY_BY");

            SetColumnCaption("TEAM_ID", "L_TEAM_ID");
            SetColumnCaption("MAIN_PROMO_CODE", "L_MAIN_PROMO_CODE");
            SetColumnCaption("REGION", "L_REGION");
            SetColumnCaption("PRODUCT_NAME", "L_PRODUCT_NAME");
            SetColumnCaption("SUBCONTRACT_CODE", "L_VENDOR_CODE");
            //SetColumnCaption("IR_DOC", "L_IR_DOC");
            SetColumnCaption("PRODUCT_OWNER", "L_PRODUCT_OWNER");


        }
        private void GetAcctOrderExcelCaptionMA(DataTable table)
        {
            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_ACCT_REPORT_INSTALLATION"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();

            void SetColumnCaption(string columnName, string lovName)
            {
                try
                {
                    var caption = configscreen.FirstOrDefault(f => f.Name == lovName)?.LovValue1 ?? columnName;
                    table.Columns[columnName].Caption = caption.ToUpper();
                }
                catch (Exception ex)
                {
                    _Logger.Error($"Failed to set caption for column '{columnName}': {ex.Message}");
                }
            }
            SetColumnCaption("ORDER_STATUS", "L_ORDER_STATUS");
            SetColumnCaption("ACCESS_NO", "L_INTERNET_NO");
            SetColumnCaption("ORDER_NO_SFF", "L_ORDER_NO");
            SetColumnCaption("ORDER_TYPE", "L_ORDER_TYPE");
            SetColumnCaption("SOA_SUBMIT_DATE_TEXT", "L_SOA_SUBMIT_DATE");
            SetColumnCaption("SUBCONTRACT_NAME", "L_SUBCONTRACT_NAME");
            SetColumnCaption("SUBCONTRACT_TYPE", "L_SUBCONTRACT_TYPE");
            SetColumnCaption("SUBCONTRACT_SUB_TYPE", "L_SUBCONTRACT_SUB_TYPE");
            SetColumnCaption("LOOKUP_ID", "L_LOOKUP_ID");
            SetColumnCaption("LOOKUP_NAME", "L_LOOKUP_NAME");
            SetColumnCaption("LOOKUP_COST", "L_LOOKUP_COST");
            SetColumnCaption("ONTOP_LOOKUP_ID", "L_ONTOP_LOOKUP_ID");
            SetColumnCaption("ONTOP_LOOKUP_NAME", "L_ONTOP_LOOKUP_NAME");
            SetColumnCaption("ONTOP_COST", "L_ONTOP_COST");
            SetColumnCaption("BASE_COST", "L_BASE_COST");
            SetColumnCaption("RECAL_COST", "L_RECAL_COST");
            SetColumnCaption("SYMPTOM_GROUP", "L_SYMPTOM_GROUP");
            SetColumnCaption("SYMPTOM_NAME", "L_SYMPTOM_NAME");
            SetColumnCaption("MODIFY_DATE_TEXT", "L_MODIFY_DATE");
            SetColumnCaption("MODIFY_BY", "L_MODIFY_BY");

        }


        private void GetScmOrderExcelCaption(DataTable table)
        {
            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_SCM_REPORT_INSTALLATION"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();
            void SetColumnCaption(string columnName, string lovName)
            {
                try
                {
                    var caption = configscreen.FirstOrDefault(f => f.Name == lovName)?.LovValue1 ?? columnName;
                    table.Columns[columnName].Caption = caption.ToUpper();
                }
                catch (Exception ex)
                {
                    _Logger.Error($"Failed to set caption for column '{columnName}': {ex.Message}");
                }
            }

            SetColumnCaption("WORK_STATUS", "L_ORDER_STATUS");
            SetColumnCaption("ACCESS_NUMBER_MASKING", "L_ACC_NBR");
            SetColumnCaption("ORDER_NO_SFF", "L_ORD_NO");
            SetColumnCaption("ORDER_TYPE", "L_ORD_TYPE");
            SetColumnCaption("COMPLETE_DATE_TEXT", "L_COMPLETED_DATE");
            SetColumnCaption("SOA_SUBMIT_DATE_TEXT", "L_SOA_SUBMIT_DATE");
            SetColumnCaption("SUBCONTRACT_NAME", "L_SUBCONTRACT_NAME");
            SetColumnCaption("SUBCONTRACT_TYPE", "L_SUB_CONTRACT_TYPE");
            SetColumnCaption("SUBCONTRACT_SUB_TYPE", "L_SUB_CONTRACT_SUB_TYPE");
            SetColumnCaption("LOOKUP_ID", "L_LOOKUP_ID");
            SetColumnCaption("LOOKUP_NAME", "L_LOOKUP_NAME");
            SetColumnCaption("LOOKUP_COST", "L_LOOKUP_COST");
            SetColumnCaption("ONTOP_LOOKUP_ID", "L_ONTOP_LOOKUP_ID");
            SetColumnCaption("ONTOP_LOOKUP_NAME", "L_ONTOP_LOOKUP_NAME");
            SetColumnCaption("ONTOP_COST", "L_ONTOP_COST");
            SetColumnCaption("TOTAL_DISTANCE", "L_TOTAL_DISTANCE");
            SetColumnCaption("BASE_COST", "L_BASE_COST");
            SetColumnCaption("OVER_LENGTH", "L_Over_Length");
            SetColumnCaption("OVER_COST", "L_Over_Cost");
            SetColumnCaption("TOTAL_SOA", "L_TOTAL_SOA");
            SetColumnCaption("RECAL_LOOKUP_ID", "L_RECAL_LOOKUP_ID");
            SetColumnCaption("RECAL_ONTOP_LOOKUP_ID", "L_RECAL_ONTOP_LOOKUP_ID");
            SetColumnCaption("RECAL_DISTANCE", "L_RECAL_DIS");
            SetColumnCaption("RECAL_COST", "L_RECAL_COST");
            SetColumnCaption("RECAL_OVER_LENGTH", "L_RECAL_OVER_LENGTH");
            SetColumnCaption("RECAL_OVER_COST", "L_RECAL_OVER_COST");
            SetColumnCaption("TOTAL_RECAL", "L_TOTAL_RECAL");
            SetColumnCaption("REUSED_FLAG", "L_Reuse_Flag");
            SetColumnCaption("REQUEST_SUB_FLAG", "L_REQUEST_SUB_FLAG");
            SetColumnCaption("TOTAL_FEE", "L_TOTAL_FEE");
            SetColumnCaption("LAST_UPDATE_DATE_TEXT", "L_LAST_UPDATE_DATE");
            SetColumnCaption("LAST_UPDATE_BY", "L_LAST_UPDATE_BY");
        }
        private void GetFapoOrderExcelCaption(DataTable table)
        {

            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_FAPO_REPORT_INSTALLATION"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();
            void SetColumnCaption(string columnName, string lovName)
            {
                try
                {
                    var caption = configscreen.FirstOrDefault(f => f.Name == lovName)?.LovValue1 ?? columnName;
                    table.Columns[columnName].Caption = caption.ToUpper();
                }
                catch (Exception ex)
                {
                    _Logger.Error($"Failed to set caption for column '{columnName}': {ex.Message}");
                }
            }

            SetColumnCaption("WORK_STATUS", "L_STATUS");
            SetColumnCaption("ACCESS_NUMBER_MASKING", "L_INTERNET_NO");
            SetColumnCaption("ORDER_NO_SFF", "L_ORDER_NO");
            SetColumnCaption("ORDER_TYPE", "L_ORDER_TYPE");
            SetColumnCaption("COMPLETE_DATE_TEXT", "L_COMPLETE_DATE");
            SetColumnCaption("SOA_SUBMIT_DATE_TEXT", "L_SOA_SUBMIT_DATE");
            SetColumnCaption("SUBCONTRACT_NAME", "L_SUBCONTRACT_NAME");
            SetColumnCaption("SUBCONTRACT_TYPE", "L_SUB_CONTRACT_TYPE");
            SetColumnCaption("SUBCONTRACT_SUB_TYPE", "L_SUB_CONTRACT_SUB_TYPE");
            SetColumnCaption("LOOKUP_ID", "L_LOOKUP_ID");
            SetColumnCaption("LOOKUP_NAME", "L_LOOKUP_NAME");
            SetColumnCaption("LOOKUP_COST", "L_LOOKUP_COST");
            SetColumnCaption("ONTOP_LOOKUP_ID", "L_ONTOP_LOOKUP_ID");
            SetColumnCaption("ONTOP_LOOKUP_NAME", "L_ONTOP_LOOKUP_NAME");
            SetColumnCaption("ONTOP_COST", "L_ONTOP_COST");
            SetColumnCaption("TOTAL_DISTANCE", "L_DISTANCE_TOTAL");
            SetColumnCaption("BASE_COST", "L_BASE_COST");
            SetColumnCaption("OVER_LENGTH", "L_Over_Length");
            SetColumnCaption("OVER_COST", "L_Over_Cost");
            SetColumnCaption("TOTAL_SOA", "L_TOTAL_SOA");
            SetColumnCaption("RECAL_LOOKUP_ID", "L_RECAL_LOOKUP_ID");
            SetColumnCaption("RECAL_ONTOP_LOOKUP_ID", "L_RECAL_ONTOP_LOOKUP_ID");
            SetColumnCaption("RECAL_DISTANCE", "L_RECAL_DIS");
            SetColumnCaption("RECAL_COST", "L_RECAL_COST");
            SetColumnCaption("RECAL_OVER_LENGTH", "L_RECAL_OVER_LENGTH");
            SetColumnCaption("RECAL_OVER_COST", "L_RECAL_OVER_COST");
            SetColumnCaption("TOTAL_RECAL", "L_TOTAL_RECAL");
            SetColumnCaption("REUSED_FLAG", "L_Reuse_Flag");
            SetColumnCaption("REQUEST_SUB_FLAG", "L_REQUEST_SUB_FLAG");
            SetColumnCaption("TOTAL_FEE", "L_TOTAL_FEE");
            SetColumnCaption("LAST_UPDATE_DATE_TEXT", "L_LAST_UPDATE_DATE");
            SetColumnCaption("LAST_UPDATE_BY", "L_LAST_UPDATE_BY");

            SetColumnCaption("TEAM_ID", "L_TEAM_ID");
            SetColumnCaption("MAIN_PROMO_CODE", "L_MAIN_PROMO_CODE");
            SetColumnCaption("REGION", "L_REGION");
            SetColumnCaption("PRODUCT_NAME", "L_PRODUCT_NAME");
            SetColumnCaption("SUBCONTRACT_CODE", "L_VENDOR_CODE");
            SetColumnCaption("IR_DOC", "L_IR_DOC");
            SetColumnCaption("INVOICE_NO", "L_INVOICE_NO");
            SetColumnCaption("PRODUCT_OWNER", "L_PRODUCT_OWNER");
            SetColumnCaption("ORDER_STATUS", "L_ORDER_STATUS");
            SetColumnCaption("PERIOD_DATE_TEXT", "L_PERIOD_DATE");
            SetColumnCaption("TRANSFER_DATE_TEXT", "L_TRANSFER_DATE");

        }
        private void GetAcctOrderExcelCaption(DataTable table)
        {

            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_ACCT_REPORT_INSTALLATION"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();

            void SetColumnCaption(string columnName, string lovName)
            {
                try
                {
                    var caption = configscreen.FirstOrDefault(f => f.Name == lovName)?.LovValue1 ?? columnName;
                    table.Columns[columnName].Caption = caption.ToUpper();
                }
                catch (Exception ex)
                {
                    _Logger.Error($"Failed to set caption for column '{columnName}': {ex.Message}");
                }
            }
            SetColumnCaption("WORK_STATUS", "L_ORDER_STATUS");
            SetColumnCaption("ACCESS_NUMBER_MASKING", "L_INTERNET_NO");
            SetColumnCaption("ORDER_NO_SFF", "L_ORDER_NO");
            SetColumnCaption("ORDER_TYPE", "L_ORDER_TYPE");
            SetColumnCaption("COMPLETE_DATE_TEXT", "L_COMPLETE_DATE");
            SetColumnCaption("SOA_SUBMIT_DATE_TEXT", "L_SOA_SUBMIT_DATE");
            SetColumnCaption("SUBCONTRACT_NAME", "L_SUBCONTRACT_NAME");
            SetColumnCaption("SUBCONTRACT_TYPE", "L_SUBCONTRACT_TYPE");
            SetColumnCaption("SUBCONTRACT_SUB_TYPE", "L_SUBCONTRACT_SUB_TYPE");
            SetColumnCaption("LOOKUP_ID", "L_LOOKUP_ID");
            SetColumnCaption("LOOKUP_NAME", "L_LOOKUP_NAME");
            SetColumnCaption("LOOKUP_COST", "L_LOOKUP_COST");
            SetColumnCaption("ONTOP_LOOKUP_ID", "L_ONTOP_LOOKUP_ID");
            SetColumnCaption("ONTOP_LOOKUP_NAME", "L_ONTOP_LOOKUP_NAME");
            SetColumnCaption("ONTOP_COST", "L_ONTOP_COST");
            SetColumnCaption("TOTAL_DISTANCE", "L_TOTAL_DISTANCE");
            SetColumnCaption("BASE_COST", "L_BASE_COST");
            SetColumnCaption("OVER_LENGTH", "L_Over_Length");
            SetColumnCaption("OVER_COST", "L_Over_Cost");
            SetColumnCaption("TOTAL_SOA", "L_TOTAL_SOA");
            SetColumnCaption("RECAL_LOOKUP_ID", "L_RECAL_LOOKUP_ID");
            SetColumnCaption("RECAL_ONTOP_LOOKUP_ID", "L_RECAL_ONTOP_LOOKUP_ID");
            SetColumnCaption("RECAL_DISTANCE", "L_RECAL_DISTANCE");
            SetColumnCaption("RECAL_COST", "L_RECAL_COST");
            SetColumnCaption("RECAL_OVER_LENGTH", "L_RECAL_OVER_LENGTH");
            SetColumnCaption("RECAL_OVER_COST", "L_RECAL_OVER_COST");
            SetColumnCaption("TOTAL_RECAL", "L_TOTAL_RECAL");
            SetColumnCaption("REUSED_FLAG", "L_Reuse_Flag");
            SetColumnCaption("REQUEST_SUB_FLAG", "L_REQUEST_SUB_FLAG");
            SetColumnCaption("TOTAL_FEE", "L_TOTAL_FEE");
            SetColumnCaption("LAST_UPDATE_DATE_TEXT", "L_LAST_UPDATE_DATE");
            SetColumnCaption("LAST_UPDATE_BY", "L_LAST_UPDATE_BY");


        }




        public void SetColumnsOrder(DataTable table, params String[] columnNames)
        {
            int columnIndex = 0;
            foreach (var columnName in columnNames)
            {
                table.Columns[columnName].SetOrdinal(columnIndex);
                columnIndex++;
            }
        }

        private byte[] GenerateExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName)
        {
            //  _Logger.Info("GenerateSaleTrackingExcel start");
            string tempFilePath = Path.Combine(directoryPath, fileName + ".xlsx");

            //Delete existing file with same file name.
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }

            //var newFile = new FileInfo(finalFileNameWithPath);
            var newFile = new FileInfo(tempFilePath);

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = dataToExcel.Columns.Count;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);
                iRow = 1;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                ExcelRange rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Font.Bold = true;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(tempFilePath);
                return data;
            }
        }




        private byte[] GenerateCSV(DataTable dataToCSV, string csvFileName)
        {
            //  _Logger.Info("GenerateSaleTrackingCSV start");
            //string tempFilePath = Path.Combine(Path.GetTempPath(), csvFileName + ".csv");

            //Delete existing file with same file name.
            if (System.IO.File.Exists(csvFileName))
            {
                System.IO.File.Delete(csvFileName);
            }

            using (var writer = new StreamWriter(csvFileName, false, Encoding.UTF8))
            {
                // Write header
                var header = string.Join(",", dataToCSV.Columns.Cast<DataColumn>().Select(c => "\"" + c.ColumnName + "\""));
                writer.WriteLine(header);

                // Write rows
                foreach (DataRow row in dataToCSV.Rows)
                {
                    var fields = row.ItemArray.Select(field => "\"" + field.ToString() + "\"");
                    writer.WriteLine(string.Join(",", fields));
                }
            }

            byte[] data = System.IO.File.ReadAllBytes(csvFileName);
            //string csvText = Encoding.UTF8.GetString(data);
            return data;
        }


        #endregion

        public ActionResult FLSUPDATEOrderList([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                if (CurrentUser == null)
                {
                    return RedirectToAction("logout", "Account");
                }


                var start = request.Page - 1;
                // Paging Length 10,20
                var length = request.PageSize;

                int pageSize = length != null ? Convert.ToInt32(length) : 20;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                var UpdateModels = new JavaScriptSerializer().Deserialize<FLSUpdateModel>(dataS);
                string[] str;
                string[] prdname; string INV_DATE = string.Empty;
                string checkDispute = UpdateModels.UPDATESTATUS.ToSafeString();
                ReportInstallationCostbyOrderModel searchModel = new ReportInstallationCostbyOrderModel();
                var result = new ReportInstallationCostbyOrderListReturn();
                prdname = new string[1] { "ALL" };
                INV_DATE = UpdateModels.INVOICE_DATE.ToSafeString();
                searchModel.ORDER_NO = "";
                searchModel.ACCESS_NO = "";
                searchModel.PRODUCT_NAME = prdname;
                searchModel.SUBCONT_CODE = "ALL";
                searchModel.ORG_ID = "ALL";
                searchModel.IR_DOC = "ALL";
                searchModel.ORD_TYPE = "ALL";
                searchModel.REGION = "ALL";
                searchModel.SUBCONT_TYPE = "ALL";
                searchModel.SUBCONTSUB_TYPE = "ALL";
                searchModel.PERIOD_FM = "";
                searchModel.PERIOD_TO = "";
                searchModel.APPROVE_FM = "";
                searchModel.APPROVE_TO = "";
                searchModel.TRANS_FM = "";
                searchModel.TRANS_TO = "";
                searchModel.UPDATE_BY = "";
                str = new string[1] { UpdateModels.WF_STATUS };
                searchModel.FOA_FM = UpdateModels.DateFrom;
                searchModel.FOA_TO = UpdateModels.DateTo;
                searchModel.INVOICE_DT = UpdateModels.INVOICE_DATE;
                searchModel.INVOICE_NO = UpdateModels.INVOICE_NO;
                searchModel.WORK_STATUS = str;
                searchModel.STATUS = UpdateModels.UPDATESTATUS;
                searchModel.ORD_STATUS = "Approve";
                searchModel.PAGE_INDEX = UpdateModels.PAGE_INDEX;
                searchModel.PAGE_SIZE = UpdateModels.PAGE_SIZE;
                searchModel.report_type = UpdateModels.report_type;



                if (Session["TempSearchFLSUPDATE"] == null)
                {
                    Session["TempSearchFLSUPDATE"] = searchModel;
                    TempData["TempSearchFLSUPDATE"] = searchModel;
                    result = GetReportInstallationOrderListByPage(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                    TempData["TempSearchFLSUPDATEOrderList"] = result;
                }
                else
                {
                    var session_criteria = (ReportInstallationCostbyOrderModel)Session["TempSearchFLSUPDATE"];
                    if (searchModel.SUBCONT_CODE == session_criteria.SUBCONT_CODE &&
                        searchModel.ORG_ID == session_criteria.ORG_ID &&
                        searchModel.IR_DOC == session_criteria.IR_DOC &&
                        searchModel.INVOICE_NO == session_criteria.INVOICE_NO &&
                        searchModel.REGION == session_criteria.REGION &&
                        searchModel.SUBCONT_TYPE == session_criteria.SUBCONT_TYPE &&
                        searchModel.SUBCONTSUB_TYPE == session_criteria.SUBCONTSUB_TYPE &&
                        searchModel.ORD_STATUS == session_criteria.ORD_STATUS &&
                        searchModel.ORD_TYPE == session_criteria.ORD_TYPE &&
                        searchModel.FOA_FM == session_criteria.FOA_FM &&
                        searchModel.FOA_TO == session_criteria.FOA_TO &&
                        searchModel.APPROVE_FM == session_criteria.APPROVE_FM &&
                        searchModel.APPROVE_TO == session_criteria.APPROVE_TO &&
                        searchModel.PERIOD_FM == session_criteria.PERIOD_FM &&
                        searchModel.PERIOD_TO == session_criteria.PERIOD_TO &&
                        searchModel.TRANS_FM == session_criteria.TRANS_FM &&
                        searchModel.TRANS_TO == session_criteria.TRANS_TO &&
                        searchModel.INVOICE_DT == session_criteria.INVOICE_DT &&
                        searchModel.STATUS == session_criteria.STATUS &&
                    searchModel.report_type == session_criteria.report_type)

                    {
                        Session["TempSearchFLSUPDATE"] = searchModel;
                        result = (ReportInstallationCostbyOrderListReturn)Session["TempFLSUPDATEList"];
                    }
                    else
                    {
                        Session["TempSearchFLSUPDATE"] = searchModel;
                        TempData["TempSearchFLSUPDATE"] = searchModel;
                        result = GetReportInstallationOrderListByPageNew(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                    }
                }




                if (checkDispute.ToUpper() != "DISPUTE")
                {
                    result.cur = result.cur.Where(x => !string.IsNullOrEmpty(x.ACCESS_NUMBER)).ToList();
                }
                if (INV_DATE != "")
                {
                    result.cur = result.cur.Where(x => x.INVOICE_DATE_TEXT.Contains(INV_DATE)).ToList();
                }

                Session["TempFLSUPDATEList"] = result;
                string ColummName = string.Empty; string sortType = string.Empty;
                foreach (var SortD in request.Sorts)
                {
                    ColummName = SortD.Member.ToSafeString();
                    sortType = SortD.SortDirection.ToSafeString();
                }

                var SortData = (object)null;

                string userGroup = GetUserGroup();
                if (ColummName != "")
                {
                    if (sortType == "Ascending")
                    {
                        SortData = result.cur.OrderBy(o =>
                        {
                            var property = o.GetType().GetProperty(ColummName);
                            return property != null ? property.GetValue(o) : null;
                        }).ToList();
                    }
                    else
                    {
                        SortData = result.cur.OrderByDescending(o =>
                        {
                            var property = o.GetType().GetProperty(ColummName);
                            return property != null ? property.GetValue(o) : null;
                        }).ToList();
                    }
                }
                else
                {
                    SortData = result.cur;
                }


                var SortDataNew = SortData as List<ReportInstallationCostbyOrderListModel_Binding>;
                TempData["TempSearch"] = result.cur;

                if (result.cur.Count > 0)
                {

                    SortData = SortDataNew.Skip(skip * pageSize).Take(pageSize).ToList();
                    return Json(new
                    {
                        Data = SortData,
                        Total = result.cur.Count
                    });

                }
                return null;

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

        }

        public ActionResult ACCUPDATEOrderList([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {

                ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");
                var result = GetReportInstallationOrderListByPage(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                JObject jsonObject = JObject.Parse(dataS);
                decimal TOTAL_COUNT = Convert.ToDecimal(jsonObject["TOTAL_COUNT"]);
                decimal _total = result.cur[0].CNT;
                if (result != null)
                {

                    return Json(new
                    {
                        Data = result.cur,
                        Total = TOTAL_COUNT
                    });
                }
                return null;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

        }


        public ActionResult getSumValue([DataSourceRequest] DataSourceRequest request)
        {
            decimal? sumtotal = 0; decimal? sumvat = 0; decimal? sumincvat = 0;
            var DataModel = TempData.Peek("TempSearch");
            if (DataModel != null)
            {
                List<ReportInstallationCostbyOrderListModel_Binding> SearchResults = (List<ReportInstallationCostbyOrderListModel_Binding>)DataModel;
                if (SearchResults.Count > 0)
                {

                    var data = SearchResults.FirstOrDefault();

                    if (data.TOTAL_PAID_AMOUNT != null)
                    {
                        sumtotal = data.TOTAL_PAID_AMOUNT;
                    }
                    if (data.TOTAL_PAID_AMOUNT != null)
                    {
                        sumvat = data.TOTAL_VAT;
                    }
                    if (data.TOTAL_PAID_AMOUNT != null)
                    {
                        sumincvat = data.TOTAL_AMOUNT;
                    }

                }
            }
            else
            {
                sumtotal = 0;
                sumvat = 0;
                sumincvat = 0;
            }

            return Json(new
            {
                _sumtotal = string.Format("{0:#,0.00}", sumtotal), //.ToSafeString(),
                _sumvat = string.Format("{0:#,0.00}", sumvat),//sumvat.ToSafeString(),
                _sumincvat = string.Format("{0:#,0.00}", sumincvat),//.ToSafeString(),

            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult getDisputeRecord([DataSourceRequest] DataSourceRequest request, string AccNo)
        {
            string dp = string.Empty;
            var query = new GetReportInstallationCostbyOrderHistoryDetailQuery()
            {
                p_ACCESS_NO = AccNo,
                p_ORDER_NO = "",

            };
            var result = _queryProcessor.Execute(query);
            foreach (var dd in result)
            {
                if (dd.WORK_STATUS == "Dispute")
                {
                    dp = "FlagDispute";
                    break;
                }
                else
                {
                    dp = "";
                }
            }
            return Json(new
            {
                _flagDispite = dp


            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckSession()
        {
            if (CurrentUser == null)
            {
                return Json(new { sessionActive = false });
            }

            return Json(new { sessionActive = true });
        }

        public class DynamicGridModel
        {
            public IEnumerable<dynamic> Data { get; set; }
            public int Total { get; set; }
        }
        public ActionResult GetPartialView(int index, [DataSourceRequest] DataSourceRequest request)
        {
            List<dynamic> columns = new List<dynamic>();
            List<Dictionary<string, object>> searchModelList = new List<Dictionary<string, object>>();

            SelectedLookupReportInstallationCostbyOrderReturn queryResult = (SelectedLookupReportInstallationCostbyOrderReturn)TempData.Peek("QueryResult");
            var selectedList = queryResult.result_lookup_id_cur[index - 1];
            string wrappedXml = $"<ROOT>{selectedList.lookup_list.ToString()}</ROOT>";
            var xdoc = XDocument.Parse(wrappedXml);
            foreach (var lookupList in xdoc.Descendants("LOOKUP_LIST"))
            {
                foreach (var lookup in lookupList.Elements("LOOKUP"))
                {
                    var dataEntry = new Dictionary<string, object>
            {
                { "Id", searchModelList.Count + 1 }
            };

                    foreach (var element in lookup.Elements())
                    {
                        var fieldName = element.Name.LocalName;
                        var title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldName.Replace("_", " "));

                        if (!columns.Any(c => c.Field == fieldName))
                        {
                            columns.Add(new { Field = fieldName, Title = title });
                        }

                        dataEntry[fieldName] = element.Value;
                    }

                    searchModelList.Add(dataEntry);
                }
            }

            ViewBag.DynamicIdField = "Id";
            ViewBag.DynamicColumns = columns;
            ViewBag.GridName = "DynamicGrid" + index;
            ViewBag.GridIndex = index;

            var dataResult = searchModelList.ToDataSourceResult(request);

            var model = new DynamicGridModel
            {
                Data = dataResult.Data.Cast<dynamic>(),
                Total = dataResult.Total
            };

            return PartialView("_PartialReCalDistanceFAPOTable", model);
        }




        public JsonResult GetSelectedLookup(string value = "", string p_LOOKUP_NAME = "")
        {

            var formattedDate = TempData.Peek("FormattedDate") as string;
            var query = new GetSelectedLookupReportInstallationCostbyOrderQuery
            {
                p_LOOKUP_NAME = p_LOOKUP_NAME,
                p_FOA_SUBMIT_DATE = formattedDate
            };

            var result = _queryProcessor.Execute(query);
            if (result == null)
            {
                return null;
            }
            var orderedLookupItems = result.result_lookup_id_cur
            .OrderBy(item => item.ontop_flag == "N" ? 0 : 1)
            .ToList();
            result.result_lookup_id_cur = orderedLookupItems;
            TempData["QueryResult"] = result;
            TempData.Keep("QueryResult");

            var lookupNames = new List<string>();
            for (int i = 0; i < result.result_lookup_id_cur.Count; i++)
            {
                // Assuming lookup_name is a property of each item in result_lookup_id_cur
                var lookupName = result.result_lookup_id_cur[i].lookup_name;
                lookupNames.Add(lookupName);
            }

            var CountIndex = result.result_lookup_id_cur.Count;
            ViewBag.CountIndex = result.result_lookup_id_cur.Count;

            return Json(new { result = result, countIndex = CountIndex, lookupNames = lookupNames }, JsonRequestBehavior.AllowGet);

        }
        private List<dynamic> CreateDynamicColumns(string xmlData)
        {
            var lookups = XDocument.Parse(xmlData).Descendants("LOOKUP")
                .Select(lookup => lookup.Elements().ToDictionary(
                    element => element.Name.LocalName,
                    element => (object)element.Value
                ))
                .ToList();

            List<dynamic> columns = new List<dynamic>();

            if (lookups.Any())
            {
                var firstLookup = lookups.First();

                foreach (var key in firstLookup.Keys)
                {
                    columns.Add(new
                    {
                        Field = key,
                        Title = key.Replace("_", " ")
                    });
                }
            }
            return columns;
        }


        private List<Dictionary<string, string>> GetLookupDataFromXml(string xmlData)
        {
            var lookupList = new List<Dictionary<string, string>>();

            XDocument doc = XDocument.Parse(xmlData);
            foreach (var lookup in doc.Descendants("LOOKUP"))
            {
                var dynamicFields = lookup.Elements().ToDictionary(
                    element => element.Name.LocalName,
                    element => element.Value
                );

                lookupList.Add(dynamicFields);
            }

            return lookupList;
        }

        class DynamicLookupModel
        {
            public Dictionary<string, string> Fields { get; set; }
        }
        public ActionResult EditRecalReportInstallationCostbyOrderList([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (string.IsNullOrEmpty(dataS))
                return null;

            var searchModel = JsonConvert.DeserializeObject<ReportInstallationCostbyOrderListModel_Binding>(dataS);
            var searchModelList = new List<ExistCurReportInstallationCostbyOrderListModel>();
            var dropdownData = new List<SelectListItem>();

            DateTime parsedDate = DateTime.ParseExact(searchModel.SOA_SUBMIT_DATE_TEXT, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string formattedDate = parsedDate.ToString("dd/MM/yyyy") + " 000000";
            TempData["FormattedDate"] = formattedDate;
            var query = new GetExistLookupReportInstallationCostbyOrderQuery
            {
                p_ACCESS_NO = searchModel.ACCESS_NO,
                p_ORDER_NO = searchModel.ORDER_NO_SFF,
                p_FOA_SUBMIT_DATE = formattedDate
            };
            var result = _queryProcessor.Execute(query);
            if (result == null)
            {
                return null;
            }
            var orderedExistsLookupCur = result.RETURN_EXISTS_LOOKUP_CUR
            .OrderBy(item => item.TYPE == "Main" ? 0 : 1)
            .ToList();
            result.RETURN_EXISTS_LOOKUP_CUR = orderedExistsLookupCur;


            TempData["MainLookupData"] = result;
            TempData["DeserializeSearchModel"] = searchModel;
            TempData.Keep("MainLookupData");
            TempData.Keep("FormattedDate");

            foreach (var tempData in result.RETURN_EXISTS_LOOKUP_CUR)
            {
                searchModelList.Add(new ExistCurReportInstallationCostbyOrderListModel
                {
                    LOOKUP_NAME = tempData.LOOKUP_NAME,
                    TYPE = tempData.TYPE,
                    LOOKUP_ID = tempData.LOOKUP_ID,
                    PARAMETER_VALUE = tempData.PARAMETER_VALUE
                });
            }


            foreach (var tempData in result.RETURN_MAIN_LOOKUP_CUR)
            {
                dropdownData.Add(new SelectListItem
                {
                    Text = tempData.LOOKUP_NAME,
                    Value = tempData.ONTOP_LOOKUP.ToString()
                });
            }



            return Json(searchModelList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectLookUp(string lookupName = "")
        {

            ExistReportInstallationCostbyOrderReturn MainLookupData = (ExistReportInstallationCostbyOrderReturn)TempData.Peek("MainLookupData");

            var data = new List<SelectListItem>
            {
                new SelectListItem { Text = "SELECT", Value = "0" }
            };

            if (MainLookupData != null)
            {
                foreach (var lookup in MainLookupData.RETURN_MAIN_LOOKUP_CUR)
                {
                    data.Add(new SelectListItem
                    {
                        Text = lookup.LOOKUP_NAME,
                        Value = lookup.LOOKUP_ID
                    });
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SearchReportInstallationCostbyOrderList([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("logout", "Account");
            }
            // Skiping number of Rows count
            var start = request.Page - 1;
            // Paging Length 10,20
            var length = request.PageSize;

            int pageSize = length != null ? Convert.ToInt32(length) : 20;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            if (string.IsNullOrEmpty(dataS))
                return null;
            try
            {
                //var rollback = getLov("FIXED_ASSET_REPORT_INSTALLATION", "ROLLBACK");

                var searchModel = new JavaScriptSerializer().Deserialize<ReportInstallationCostbyOrderModel>(dataS);
                var result = new ReportInstallationCostbyOrderListReturn();

                if (Session["TempSearchCriteria"] == null)
                {
                    Session["TempSearchCriteria"] = searchModel;
                    TempData["TempSearchCriteria"] = searchModel;
                    result = GetReportInstallationOrderListByPageNew(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                    TempData["TempDataOrderList"] = result;
                }
                else
                {
                    var session_criteria = (ReportInstallationCostbyOrderModel)Session["TempSearchCriteria"];

                    if (searchModel.ORDER_STATUS == null)
                        searchModel.ORDER_STATUS = new string[1];
                    if (session_criteria.ORDER_STATUS == null)
                        session_criteria.ORDER_STATUS = new string[1];
                    if (searchModel.PRODUCT_NAME == null)
                        searchModel.PRODUCT_NAME = new string[1];
                    if (session_criteria.PRODUCT_NAME == null)
                        session_criteria.PRODUCT_NAME = new string[1];

                    var order_check = searchModel.ORDER_STATUS.Except(session_criteria.ORDER_STATUS).ToList();
                    var product_check = searchModel.PRODUCT_NAME.Except(session_criteria.PRODUCT_NAME).ToList();
                    //check criteria search wasn't change?
                    if (searchModel.INV_GRP == session_criteria.INV_GRP &&
                        searchModel.ORDER_NO == session_criteria.ORDER_NO &&
                        searchModel.ACCESS_NO == session_criteria.ACCESS_NO &&
                        order_check.Count == 0 &&
                        product_check.Count == 0 &&
                        //searchModel.ORDER_STATUS == session_criteria.ORDER_STATUS &&
                        //searchModel.PRODUCT_NAME == session_criteria.PRODUCT_NAME &&
                        searchModel.SUBCONT_CODE == session_criteria.SUBCONT_CODE &&
                        searchModel.ORG_ID == session_criteria.ORG_ID &&
                        searchModel.IR_DOC == session_criteria.IR_DOC &&
                        searchModel.INVOICE_NO == session_criteria.INVOICE_NO &&
                        searchModel.REGION == session_criteria.REGION &&
                        searchModel.SUBCONT_TYPE == session_criteria.SUBCONT_TYPE &&
                        searchModel.SUBCONTSUB_TYPE == session_criteria.SUBCONTSUB_TYPE &&
                        searchModel.ORD_STATUS == session_criteria.ORD_STATUS &&
                        searchModel.ORD_TYPE == session_criteria.ORD_TYPE &&
                        searchModel.FOA_FM == session_criteria.FOA_FM &&
                        searchModel.FOA_TO == session_criteria.FOA_TO &&
                        searchModel.APPROVE_FM == session_criteria.APPROVE_FM &&
                        searchModel.APPROVE_TO == session_criteria.APPROVE_TO &&
                        searchModel.PERIOD_FM == session_criteria.PERIOD_FM &&
                        searchModel.PERIOD_TO == session_criteria.PERIOD_TO &&
                        searchModel.TRANS_FM == session_criteria.TRANS_FM &&
                        searchModel.TRANS_TO == session_criteria.TRANS_TO &&
                        searchModel.UPDATE_BY == session_criteria.UPDATE_BY &&
                        searchModel.INTERFACE == session_criteria.INTERFACE &&
                        searchModel.USER == session_criteria.USER &&
                        searchModel.STATUS == session_criteria.STATUS &&
                        searchModel.INVOICE_DT == session_criteria.INVOICE_DT &&
                        searchModel.VALIDATE_DIS == session_criteria.VALIDATE_DIS &&
                        searchModel.REASON == session_criteria.REASON &&
                        searchModel.REMARK == session_criteria.REMARK &&
                        searchModel.REMARK_FOR_SUB == session_criteria.REMARK_FOR_SUB &&
                        searchModel.TRANSFER_DT == session_criteria.TRANSFER_DT &&
                        searchModel.WORK_STATUS == session_criteria.WORK_STATUS &&
                        //searchModel.PAGE_INDEX == session_criteria.PAGE_INDEX &&
                        //searchModel.PAGE_SIZE == session_criteria.PAGE_SIZE &&
                        searchModel.EXISTING_RULE == session_criteria.EXISTING_RULE &&
                        searchModel.report_type == session_criteria.report_type &&
                        searchModel.PRODUCT_OWNER == session_criteria.PRODUCT_OWNER &&
                        searchModel.NEW_RULE == session_criteria.NEW_RULE)

                    {
                        Session["TempSearchCriteria"] = searchModel;
                        result = (ReportInstallationCostbyOrderListReturn)Session["TempDataList"];
                    }
                    else
                    {
                        Session["TempSearchCriteria"] = searchModel;
                        TempData["TempSearchCriteria"] = searchModel;
                        result = GetReportInstallationOrderListByPageNew(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                    }
                }
                Session["TempDataList"] = result;

                string ColummName = string.Empty; string sortType = string.Empty;
                foreach (var SortD in request.Sorts)
                {
                    ColummName = SortD.Member.ToSafeString();
                    sortType = SortD.SortDirection.ToSafeString();
                }

                var SortData = (object)null;

                string userGroup = GetUserGroup();
                if (ColummName != "")
                {
                    if (sortType == "Ascending")
                    {
                        SortData = result.cur.OrderBy(o =>
                        {
                            var property = o.GetType().GetProperty(ColummName);
                            return property != null ? property.GetValue(o) : null;
                        }).ToList();
                    }
                    else
                    {
                        SortData = result.cur.OrderByDescending(o =>
                        {
                            var property = o.GetType().GetProperty(ColummName);
                            return property != null ? property.GetValue(o) : null;
                        }).ToList();
                    }
                }
                else
                {
                    SortData = result.cur;
                }


                var SortDataNew = SortData as List<ReportInstallationCostbyOrderListModel_Binding>;
                TempData["TempSearch"] = result.cur;

                if (result.cur.Count > 0)
                {

                    SortData = SortDataNew.Skip(skip * pageSize).Take(pageSize).ToList();
                    return Json(new
                    {
                        Data = SortData,
                        Total = result.cur.Count
                    });

                }

                return null;

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        public async Task<ReportInstallationCostbyOrderListReturn> getAllRecord()
        {
            string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            _Logger.Info("StartGetAllRecord" + DateNow.ToSafeString());
            ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");

            string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
            string orderStatus = searchModel.ORDER_STATUS == null ? "" : string.Join(",", searchModel.ORDER_STATUS);
            string workStatus = "ALL";
            if (searchModel.WORK_STATUS != null)
            {
                workStatus = searchModel.WORK_STATUS == null ? "" : string.Join(",", searchModel.WORK_STATUS);
            }

            var query = new SearchReportInstallationCostbyOrderListNewQuery
            {
                p_ORDER_NO = searchModel.ORDER_NO == "" ? "ALL" : searchModel.ORDER_NO,
                p_ACCESS_NO = searchModel.ACCESS_NO == "" ? "ALL" : searchModel.ACCESS_NO,
                p_PRODUCT_NAME = productName == "" ? "ALL" : productName,
                p_SUBCONT_CODE = searchModel.SUBCONT_CODE == "" ? "ALL" : searchModel.SUBCONT_CODE,
                p_ORG_ID = searchModel.ORG_ID == "" ? "ALL" : searchModel.ORG_ID,
                //p_SUBCONT_NAME = searchModel.SUBCONT_NAME == "" ? "ALL" : searchModel.SUBCONT_NAME,
                p_IR_DOC = searchModel.IR_DOC == "" ? "ALL" : searchModel.IR_DOC,
                p_INVOICE_NO = searchModel.INVOICE_NO == "" ? "ALL" : searchModel.INVOICE_NO,
                p_REGION = searchModel.REGION == "" ? "ALL" : searchModel.REGION,
                p_WORK_STATUS = workStatus == "" ? "ALL" : workStatus,
                //p_WORK_STATUS = orderStatus == "" ? "ALL" : orderStatus,
                p_ORDER_STATUS = searchModel.ORD_STATUS == "" ? "ALL" : searchModel.ORD_STATUS,
                p_ORD_STATUS = "ALL",
                p_ORDER_TYPE = searchModel.ORD_TYPE == "" ? "ALL" : searchModel.ORD_TYPE,
                p_SUBCONT_TYPE = searchModel.SUBCONT_TYPE == "" ? "ALL" : searchModel.SUBCONT_TYPE,
                p_SUBCONT_SUB_TYPE = searchModel.SUBCONTSUB_TYPE == "" ? "ALL" : searchModel.SUBCONTSUB_TYPE,
                p_FOA_FM = searchModel.FOA_FM == "" ? null : searchModel.FOA_FM.Replace("/", ""),
                p_FOA_TO = searchModel.FOA_TO == "" ? null : searchModel.FOA_TO.Replace("/", ""),

                p_APPROVE_FM = searchModel.APPROVE_FM == "" ? null : searchModel.APPROVE_FM.Replace("/", ""),
                p_APPROVE_TO = searchModel.APPROVE_TO == "" ? null : searchModel.APPROVE_TO.Replace("/", ""),
                p_PERIOD_FM = searchModel.PERIOD_FM == "" ? null : searchModel.PERIOD_FM.Replace("/", ""),
                p_PERIOD_TO = searchModel.PERIOD_TO == "" ? null : searchModel.PERIOD_TO.Replace("/", ""),
                p_TRANS_FM = searchModel.TRANS_FM == "" ? null : searchModel.TRANS_FM.Replace("/", ""),
                p_TRANS_TO = searchModel.TRANS_TO == "" ? null : searchModel.TRANS_TO.Replace("/", ""),
                p_UPDATE_BY = searchModel.UPDATE_BY,
                p_PRODUCT_OWNER = searchModel.PRODUCT_OWNER,
                p_REPORT_TYPE = searchModel.report_type,
                P_PAGE_INDEX = 1,
                P_PAGE_SIZE = decimal.MaxValue
            };
            var result = _queryProcessor.Execute(query);
            var callvat = SelectFbbCfgLov("VAT_RATE_REPORT_INSTALLATION", "REPORT_INSTALLATION").FirstOrDefault();
            foreach (var item in result.cur)
            {

                item.INVOICE_AMOUNT_BFVAT = "";
                item.INVOICE_AMOUNT_VAT = getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL);
                item.TOTAL_INVOICE_AMOUNT_VAT = string.Format("{0:#,0.00}", (item.TOTAL_PAID + getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL)));
                item.CS_APPROVE_DATE_TEXT = item.CS_APPROVE_DATE.ToDateDisplayText();
                item.INVOICE_DATE_TEXT = item.INVOICE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE == DateTime.MinValue ? "-" : item.LAST_UPDATE_DATE.ToDateDisplayText();
                item.ORDER_STATUS_DT_TEXT = item.ORDER_STATUS_DT.ToDateDisplayText();
                item.APPOINTMENNT_DT_TEXT = item.APPOINTMENNT_DT.ToDateDisplayText();
                item.EFFECTIVE_END_DT_TEXT = item.EFFECTIVE_END_DT.ToDateDisplayText();
                item.SFF_ACTIVE_DATE_TEXT = item.SFF_ACTIVE_DATE.ToDateDisplayText();
                item.SFF_SUBMITTED_DATE_TEXT = item.SFF_SUBMITTED_DATE.ToDateDisplayText();
                item.FOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE.ToDateDisplayText();
                item.PAID_DATE_TEXT = item.PAID_DATE.ToDateDisplayText();

                item.MODIFY_DATE_TEXT = item.MODIFY_DATE == DateTime.MinValue ? "-" : item.MODIFY_DATE.ToDateDisplayText();
                item.COMPLETE_DATE_TEXT = item.COMPLETE_DATE == DateTime.MinValue ? "-" : item.COMPLETE_DATE.ToDateDisplayText();
                item.TRANSFER_DATE_TEXT = item.TRANSFER_DATE == DateTime.MinValue ? "-" : item.TRANSFER_DATE.ToDateDisplayText();
                item.PERIOD_DATE_TEXT = item.PERIOD_DATE == DateTime.MinValue ? "-" : item.PERIOD_DATE.ToDateDisplayText();
                //item.SOA_SUBMIT_DATE_TEXT = item.SOA_SUBMIT_DATE == DateTime.MinValue ? "-" : item.SOA_SUBMIT_DATE.ToDateDisplayText();
                item.SOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE == DateTime.MinValue ? "-" : item.FOA_SUBMIT_DATE.ToDateDisplayText();
            }
            return result;

        }
        public decimal? getVatRATE(decimal? totalvalue, string callvat)
        {
            decimal vat_value = 7;
            decimal? bfvat = 0;
            if (callvat != null)
            {
                vat_value = Convert.ToInt16(callvat.ToSafeString());
            }
            bfvat = ((vat_value * totalvalue) / 100);
            return bfvat;
        }

        private ReportInstallationCostbyOrderListReturn GetReportInstallationOrderListByPage(ReportInstallationCostbyOrderModel searchModel, decimal PAGE_INDEX, decimal PAGE_SIZE)
        {
            string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
            string orderStatus = searchModel.WORK_STATUS == null ? "" : string.Join(",", searchModel.WORK_STATUS);

            var query = new SearchReportInstallationCostbyOrderListNewQuery
            {
                p_ORDER_NO = searchModel.ORDER_NO == "" ? "ALL" : searchModel.ORDER_NO,
                p_ACCESS_NO = searchModel.ACCESS_NO == "" ? "ALL" : searchModel.ACCESS_NO,
                p_PRODUCT_NAME = productName == "" ? "ALL" : productName,
                p_SUBCONT_CODE = searchModel.SUBCONT_CODE == "" ? "ALL" : searchModel.SUBCONT_CODE,
                p_ORG_ID = searchModel.ORG_ID == "" ? "ALL" : searchModel.ORG_ID,
                //p_SUBCONT_NAME = searchModel.SUBCONT_NAME == "" ? "ALL" : searchModel.SUBCONT_NAME,
                p_IR_DOC = searchModel.IR_DOC == "" ? "ALL" : searchModel.IR_DOC,
                p_INVOICE_NO = searchModel.INVOICE_NO == "" ? "ALL" : searchModel.INVOICE_NO,
                p_REGION = searchModel.REGION == "" ? "ALL" : searchModel.REGION,
                p_WORK_STATUS = orderStatus == "" ? "ALL" : orderStatus,
                p_ORDER_STATUS = searchModel.ORD_STATUS == "" ? "ALL" : searchModel.ORD_STATUS,
                p_ORD_STATUS = "ALL",
                p_ORDER_TYPE = searchModel.ORD_TYPE == "" ? "ALL" : searchModel.ORD_TYPE,
                p_SUBCONT_TYPE = searchModel.SUBCONT_TYPE == "" ? "ALL" : searchModel.SUBCONT_TYPE,
                p_SUBCONT_SUB_TYPE = searchModel.SUBCONTSUB_TYPE == "" ? "ALL" : searchModel.SUBCONTSUB_TYPE,
                p_FOA_FM = searchModel.FOA_FM == "" ? null : searchModel.FOA_FM.Replace("/", ""),
                p_FOA_TO = searchModel.FOA_TO == "" ? null : searchModel.FOA_TO.Replace("/", ""),

                p_APPROVE_FM = searchModel.APPROVE_FM == "" ? null : searchModel.APPROVE_FM.Replace("/", ""),
                p_APPROVE_TO = searchModel.APPROVE_TO == "" ? null : searchModel.APPROVE_TO.Replace("/", ""),
                p_PERIOD_FM = searchModel.PERIOD_FM == "" ? null : searchModel.PERIOD_FM.Replace("/", ""),
                p_PERIOD_TO = searchModel.PERIOD_TO == "" ? null : searchModel.PERIOD_TO.Replace("/", ""),
                p_TRANS_FM = searchModel.TRANS_FM == "" ? null : searchModel.TRANS_FM.Replace("/", ""),
                p_TRANS_TO = searchModel.TRANS_TO == "" ? null : searchModel.TRANS_TO.Replace("/", ""),
                p_PRODUCT_OWNER = searchModel.PRODUCT_OWNER == "" ? "ALL" : searchModel.PRODUCT_OWNER,
                p_REPORT_TYPE = searchModel.report_type
            };
            var result = _queryProcessor.Execute(query);
            var callvat = SelectFbbCfgLov("VAT_RATE_REPORT_INSTALLATION", "REPORT_INSTALLATION").FirstOrDefault();

            foreach (var item in result.cur)
            {

                item.INVOICE_AMOUNT_BFVAT = "";
                item.INVOICE_AMOUNT_VAT = getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL);
                item.TOTAL_INVOICE_AMOUNT_VAT = string.Format("{0:#,0.00}", (item.TOTAL_PAID + getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL)));
                item.CS_APPROVE_DATE_TEXT = item.CS_APPROVE_DATE.ToDateDisplayText();
                //item.INVOICE_DATE_TEXT = item.INVOICE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE.ToDateDisplayText();
                item.ORDER_STATUS_DT_TEXT = item.ORDER_STATUS_DT.ToDateDisplayText();
                item.APPOINTMENNT_DT_TEXT = item.APPOINTMENNT_DT.ToDateDisplayText();
                item.EFFECTIVE_END_DT_TEXT = item.EFFECTIVE_END_DT.ToDateDisplayText();
                item.SFF_ACTIVE_DATE_TEXT = item.SFF_ACTIVE_DATE.ToDateDisplayText();
                item.SFF_SUBMITTED_DATE_TEXT = item.SFF_SUBMITTED_DATE.ToDateDisplayText();
                item.FOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE.ToDateDisplayText();
                item.PAID_DATE_TEXT = item.PAID_DATE.ToDateDisplayText();

                item.MODIFY_DATE_TEXT = item.MODIFY_DATE == DateTime.MinValue ? "-" : item.MODIFY_DATE.ToDateDisplayText();
                item.INVOICE_DATE_TEXT = item.INVOICE_DATE == DateTime.MinValue ? "-" : item.INVOICE_DATE.ToDateDisplayText();
                item.COMPLETE_DATE_TEXT = item.COMPLETE_DATE == DateTime.MinValue ? "-" : item.COMPLETE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE == DateTime.MinValue ? "-" : item.LAST_UPDATE_DATE.ToDateDisplayText();
                //item.SOA_SUBMIT_DATE_TEXT = item.SOA_SUBMIT_DATE == DateTime.MinValue ? "-" : item.SOA_SUBMIT_DATE.ToDateDisplayText();
                item.SOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE == DateTime.MinValue ? "-" : item.FOA_SUBMIT_DATE.ToDateDisplayText();
            }

            return result;
        }

        private ReportInstallationCostbyOrderListReturn GetReportInstallationOrderListByPageNew(ReportInstallationCostbyOrderModel searchModel, decimal PAGE_INDEX, decimal PAGE_SIZE)
        {
            string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
            //string workStatus = searchModel.WORK_STATUS == null ? "" : string.Join(",", searchModel.WORK_STATUS);
            string workStatus = "ALL";
            if (searchModel.STATUS != null)
            {
                workStatus = searchModel.STATUS == null ? "" : string.Join(",", searchModel.STATUS);
            }
            else if (searchModel.WORK_STATUS != null)
            {
                workStatus = searchModel.WORK_STATUS == null ? "" : string.Join(",", searchModel.WORK_STATUS);
            }

            var RevampStartDateTime = Get_FBB_CFG_LOV("FBB_CONSTANT", "ROLLBACK_REVAMPPAYG").FirstOrDefault();

            DateTime revampStartDateTime = DateTime.ParseExact(RevampStartDateTime.LovValue1.ToSafeString(), RevampStartDateTime.LovValue3.ToSafeString(), CultureInfo.InvariantCulture);

            var query = new SearchReportInstallationCostbyOrderListNewQuery
            {
                p_ORDER_NO = searchModel.ORDER_NO == "" ? "ALL" : searchModel.ORDER_NO,
                p_ACCESS_NO = searchModel.ACCESS_NO == "" ? "ALL" : searchModel.ACCESS_NO,
                p_PRODUCT_NAME = productName == "" ? "ALL" : productName,
                p_SUBCONT_CODE = searchModel.SUBCONT_CODE == "" ? "ALL" : searchModel.SUBCONT_CODE,
                p_ORG_ID = searchModel.ORG_ID == "" ? "ALL" : searchModel.ORG_ID,
                //p_SUBCONT_NAME = searchModel.SUBCONT_NAME == "" ? "ALL" : searchModel.SUBCONT_NAME,
                p_IR_DOC = searchModel.IR_DOC == "" ? "ALL" : searchModel.IR_DOC,
                p_INVOICE_NO = searchModel.INVOICE_NO == "" ? "ALL" : searchModel.INVOICE_NO,
                p_REGION = searchModel.REGION == "" ? "ALL" : searchModel.REGION,
                p_WORK_STATUS = workStatus == "" ? "ALL" : workStatus,
                //p_WORK_STATUS = searchModel.STATUS == "" ? "ALL" : searchModel.STATUS,

                p_ORDER_STATUS = searchModel.ORD_STATUS == "" ? "ALL" : searchModel.ORD_STATUS,
                //p_ORD_STATUS = "ALL",
                p_ORDER_TYPE = searchModel.ORD_TYPE == "" ? "ALL" : searchModel.ORD_TYPE,
                p_SUBCONT_TYPE = searchModel.SUBCONT_TYPE == "" ? "ALL" : searchModel.SUBCONT_TYPE.ToUpper(),
                p_SUBCONT_SUB_TYPE = searchModel.SUBCONTSUB_TYPE == "" ? "ALL" : searchModel.SUBCONTSUB_TYPE,
                p_FOA_FM = searchModel.FOA_FM == "" ? null : searchModel.FOA_FM.Replace("/", ""),
                p_FOA_TO = searchModel.FOA_TO == "" ? null : searchModel.FOA_TO.Replace("/", ""),

                p_APPROVE_FM = searchModel.APPROVE_FM == "" ? null : searchModel.APPROVE_FM.Replace("/", ""),
                p_APPROVE_TO = searchModel.APPROVE_TO == "" ? null : searchModel.APPROVE_TO.Replace("/", ""),
                p_PERIOD_FM = searchModel.PERIOD_FM == "" ? null : searchModel.PERIOD_FM.Replace("/", ""),
                p_PERIOD_TO = searchModel.PERIOD_TO == "" ? null : searchModel.PERIOD_TO.Replace("/", ""),
                p_TRANS_FM = searchModel.TRANS_FM == "" ? null : searchModel.TRANS_FM.Replace("/", ""),
                p_TRANS_TO = searchModel.TRANS_TO == "" ? null : searchModel.TRANS_TO.Replace("/", ""),
                p_PRODUCT_OWNER = searchModel.PRODUCT_OWNER == "" ? "ALL" : searchModel.PRODUCT_OWNER,
                p_REPORT_TYPE = searchModel.report_type
                //p_UPDATE_BY = searchModel.UPDATE_BY,
                //P_PAGE_INDEX = PAGE_INDEX,
                //P_PAGE_SIZE = PAGE_SIZE
            };
            var result = _queryProcessor.Execute(query);

            //int row_no = 1;
            foreach (var item in result.cur)
            {
                //item.RowNumber = row_no++;
                //item.CNT = result.cur.Count();
                //item.INVOICE_AMOUNT_BFVAT = "";
                //item.INVOICE_AMOUNT_VAT = getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL);
                //item.TOTAL_INVOICE_AMOUNT_VAT = string.Format("{0:#,0.00}", (item.TOTAL_PAID + getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL)));
                item.CS_APPROVE_DATE_TEXT = item.CS_APPROVE_DATE.ToDateDisplayText();
                //item.INVOICE_DATE_TEXT = item.INVOICE_DATE.ToDateDisplayText();
                item.ORDER_STATUS_DT_TEXT = item.ORDER_STATUS_DT.ToDateDisplayText();
                item.APPOINTMENNT_DT_TEXT = item.APPOINTMENNT_DT.ToDateDisplayText();
                item.EFFECTIVE_END_DT_TEXT = item.EFFECTIVE_END_DT.ToDateDisplayText();
                item.SFF_ACTIVE_DATE_TEXT = item.SFF_ACTIVE_DATE.ToDateDisplayText();
                item.SFF_SUBMITTED_DATE_TEXT = item.SFF_SUBMITTED_DATE.ToDateDisplayText();
                item.FOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE.ToDateDisplayText();
                item.PAID_DATE_TEXT = item.PAID_DATE.ToDateDisplayText();


                item.INVOICE_DATE_TEXT = item.INVOICE_DATE == DateTime.MinValue ? "-" : item.INVOICE_DATE.ToDateDisplayText();
                item.MODIFY_DATE_TEXT = item.MODIFY_DATE == DateTime.MinValue ? "-" : item.MODIFY_DATE.ToDateDisplayText();
                item.COMPLETE_DATE_TEXT = item.COMPLETE_DATE == DateTime.MinValue ? "-" : item.COMPLETE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE == DateTime.MinValue ? "-" : item.LAST_UPDATE_DATE.ToDateDisplayText();
                //item.SOA_SUBMIT_DATE_TEXT = item.SOA_SUBMIT_DATE == DateTime.MinValue ? "-" : item.SOA_SUBMIT_DATE.ToDateDisplayText();

                item.SOA_SUBMIT_DATE = item.FOA_SUBMIT_DATE == DateTime.MinValue ? DateTime.MinValue : item.FOA_SUBMIT_DATE;
                item.SOA_SUBMIT_DATE_TEXT = item.SOA_SUBMIT_DATE == DateTime.MinValue ? "-" : item.SOA_SUBMIT_DATE.ToDateDisplayText();
                item.TRANSFER_DATE_TEXT = item.TRANSFER_DATE == DateTime.MinValue ? "-" : item.TRANSFER_DATE.ToDateDisplayText();
                item.PERIOD_DATE_TEXT = item.PERIOD_DATE == DateTime.MinValue ? "-" : item.PERIOD_DATE.ToDateDisplayText();
                item.action_flag = item.SOA_SUBMIT_DATE < revampStartDateTime ? "hide" : "show";
            }

            return result;
        }

        private List<string> GetOrderStatusByUserGroup()
        {
            List<string> orderStatusList = null;
            string userGroup = GetUserGroup();

            if (userGroup == "SCM")
            {
                orderStatusList = new List<string>(new[] { "Waiting Sub Verify", "Waiting Paid", "Dispute", "Confirm Paid" });
            }
            else if (userGroup == "FAPO")
            {
                orderStatusList = new List<string>(new[] { "Re Check", "Dispute" });
            }
            else if (userGroup == "ACCT")
            {
                orderStatusList = new List<string>(new[] { "Confirm Paid", "Paid", "Hold" });
            }

            return orderStatusList;
        }

        public JsonResult UpdateByOrder([DataSourceRequest] DataSourceRequest request, List<ReportInstallation_FBB_access_list> AccNOList,
            string AccNO, string USER, string IntFace, string Status, string InvNo, string InvDate, string IrDoc,
            string remark, string remarksub, string ValDis, string Reason, string TranDT, string chkirdoc, string prddate)
        {
            try
            {
                ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");
                string msg = "";
                IntFace = GetUserGroup();
                if (AccNOList != null)
                {
                    string checkMax = AccNOList.FirstOrDefault().ACCESS_NUMBER.ToSafeString();

                    string ACCNO = string.Empty;
                    //foreach (var d in AccNOList)
                    //{
                    //    //  ACCNO = d.ACCESS_NUMBER;
                    //    if (d.ACCESS_NUMBER == "MAX-ACCNO")
                    //    {

                    //    }
                    //}
                    if (checkMax == "MAX-ACCNO")
                    {
                        var result = GetReportInstallationOrderListByPage(searchModel, 1, Decimal.MaxValue);
                        var results = result.cur.Where(x => x.APPROVE_FLAG.Equals("Approved") && x.ORDER_STATUS.Equals("Confirm Paid")).ToList();

                        var RecList = new List<ReportInstallation_FBB_access_list>();
                        foreach (var dd in results)
                        {
                            var model = new ReportInstallation_FBB_access_list
                            {
                                ACCESS_NUMBER = dd.ACCESS_NUMBER,

                            };

                            RecList.Add(model);
                        }

                        AccNOList = RecList;
                    }



                }
                else
                {
                    var RecList = new List<ReportInstallation_FBB_access_list>();
                    var model = new ReportInstallation_FBB_access_list
                    {
                        ACCESS_NUMBER = AccNO,

                    };

                    RecList.Add(model);
                    AccNOList = RecList;


                }


                USER = CurrentUser.UserName;

                var command = new UpdateReportInstallationCostbyOrderCommand()
                {
                    p_ACCESS_list = AccNOList,
                    // p_ACCESS_NO = AccNO,
                    p_INTERFACE = IntFace.ToSafeString(),
                    p_USER = USER.ToSafeString(),
                    p_STATUS = Status.ToSafeString(),
                    p_INVOICE_NO = InvNo.ToSafeString(),
                    p_INVOICE_DT = InvDate.ToSafeString(),
                    p_IR_DOC = IrDoc.ToSafeString(),
                    p_VALIDATE_DIS = ValDis.ToSafeString(),
                    p_REASON = Reason.ToSafeString(),
                    p_REMARK = remark.ToSafeString(),
                    p_REMARK_FOR_SUB = remarksub.ToSafeString(),
                    p_TRANSFER_DT = TranDT.ToSafeString(),
                    p_CHECK_IRDOC = chkirdoc.ToSafeString(),
                    p_PERIOD_DATE = prddate.ToSafeString()

                };
                _UpdateReportInstallationCostbyOrderCommand.Handle(command);
                if (command.ret_msg == "Success.")
                {
                    msg = "Success";
                    //result[0] = "0";
                    //result[1] = "";
                }
                else
                {
                    msg = command.ret_msg;
                    //result[0] = "-1";
                    //result[1] = command.ReturnMessage;
                }
                return Json(msg);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }


        }


        public JsonResult UpdateAllByOrder([DataSourceRequest] DataSourceRequest request,
        string AccNO, string USER, string IntFace, string Status, string InvNo, string InvDate, string IrDoc,
        string remark, string remarksub, string ValDis, string Reason, string TranDT, string chkirdoc)
        {
            string msg = "";


            List<string> dd = new List<string>();
            ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");
            var AllRecord = GetReportInstallationOrderListByPage(searchModel, 1, Decimal.MaxValue);

            var result = AllRecord.cur;

            var AccNOList = result.Where(a => GetOrderStatusByUserGroup().Contains(a.ORDER_STATUS)).Select(a => new ReportInstallation_FBB_access_list { ACCESS_NUMBER = a.ACCESS_NUMBER }).ToList();


            try
            {
                USER = CurrentUser.UserName;
                IntFace = GetUserGroup();
                var command = new UpdateReportInstallationCostbyOrderCommand()
                {
                    p_ACCESS_list = AccNOList,
                    // p_ACCESS_NO = AccNO,
                    p_INTERFACE = IntFace,
                    p_USER = USER,
                    p_STATUS = Status,
                    p_INVOICE_NO = InvNo,
                    p_INVOICE_DT = InvDate,
                    p_IR_DOC = IrDoc,
                    p_VALIDATE_DIS = ValDis,
                    p_REASON = Reason,
                    p_REMARK = remark,
                    p_REMARK_FOR_SUB = remarksub,
                    p_TRANSFER_DT = TranDT,
                    p_CHECK_IRDOC = chkirdoc

                };
                _UpdateReportInstallationCostbyOrderCommand.Handle(command);
                if (command.ret_msg == "Success.")
                {
                    msg = "Success";
                }
                else
                {
                    msg = command.ret_msg;
                }

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

            return Json(msg);
        }




        public JsonResult getOrderListHistory([DataSourceRequest] DataSourceRequest request, string AccNO, string OrdNO)
        {

            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
                return null;
            try
            {
                var query = new GetReportInstallationCostbyOrderHistoryDetailQuery()
                {
                    p_ACCESS_NO = AccNO,
                    p_ORDER_NO = OrdNO,

                };
                var result = _queryProcessor.Execute(query);

                foreach (var item in result)
                {
                    item.UPDATE_DATE_TEXT = item.UPDATE_DATE.ToDateTimeDisplayText();
                }

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        public JsonResult getOrderListPackage([DataSourceRequest] DataSourceRequest request, string AccNO, string OrdNO)
        {

            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
                return null;
            try
            {
                var query = new GetReportInstallationCostbyOrderPackageDetailQuery()
                {
                    p_ACCESS_NO = AccNO,
                    p_ORDER_NO = OrdNO,

                };
                var result = _queryProcessor.Execute(query);
                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        public JsonResult getOrderListFee([DataSourceRequest] DataSourceRequest request, string AccNO, string OrdNO)
        {

            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
                return null;
            try
            {
                var query = new GetReportInstallationCostbyOrderFeeDetailQuery()
                {
                    p_ACCESS_NO = AccNO,
                    p_ORDER_NO = OrdNO,

                };
                var result = _queryProcessor.Execute(query);
                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        public ActionResult SCMOrderDetail([DataSourceRequest] DataSourceRequest request, string AccNO, string OrdNO)
        {
            var result = getOrderDetail(AccNO, OrdNO);
            string _ACC_NBR = string.Empty;
            string _USER_NAME = string.Empty;
            string _SBC_CPY = string.Empty;
            string _PRODUCT_NAME = string.Empty;
            string _ON_TOP1 = string.Empty;
            string _ON_TOP2 = string.Empty;
            string _VOIP_NUMBER = string.Empty;
            string _SERVICE_PACK_NAME = string.Empty;
            string _ORD_NO = string.Empty;
            string _ORDER_TYPE = string.Empty;
            string _ORDER_SFF = string.Empty;
            string _APPOINTMENT_DATE = string.Empty;
            string _SFF_ACTIVE_DATE = string.Empty;
            string _APPROVE_JOB_FBSS_DATE = string.Empty;
            string _COMPLETED_DATE = string.Empty;
            string _WORK_STATUS = string.Empty;
            string _REJECT_REASON = string.Empty;
            string _MATERIAL_CODE_CPESN = string.Empty;
            string _CPE_SN = string.Empty;
            string _CPE_MODE = string.Empty;
            string _MATERIAL_CODE_STBSN = string.Empty;
            string _STB_SN = string.Empty;
            string _MATERIAL_CODE_ATASN = string.Empty;
            string _ATA_SN = string.Empty;
            string _MATERIAL_CODE_WIFIROUTESN = string.Empty;
            string _WIFI_ROUTER_SN = string.Empty;
            string _STO_LOCATION = string.Empty;
            string _VENDOR_CODE = string.Empty;
            string _FOA_REJECT_REASON = string.Empty;
            string _RE_APPOINTMENT_REASON = string.Empty;
            string _PHASE_PO = string.Empty;
            string _SFF_SUBMITTED_DATE = string.Empty;
            string _EVENT_CODE = string.Empty;
            string _REGION = string.Empty;
            string _ENTRY_FEE = string.Empty;
            string _TOTAL_FEE = string.Empty;

            if (result != null)
            {
                if (result.Count == 0)
                {
                    _ACC_NBR = "";
                    _USER_NAME = "";
                    _SBC_CPY = "";
                    _PRODUCT_NAME = "";
                    _ON_TOP1 = "";
                    _ON_TOP2 = "";
                    _VOIP_NUMBER = "";
                    _SERVICE_PACK_NAME = "";
                    _ORD_NO = "";
                    _ORDER_TYPE = "";
                    _ORDER_SFF = "";
                    _APPOINTMENT_DATE = "";
                    _SFF_ACTIVE_DATE = "";
                    _APPROVE_JOB_FBSS_DATE = "";
                    _COMPLETED_DATE = "";
                    _WORK_STATUS = "";
                    _REJECT_REASON = "";
                    _MATERIAL_CODE_CPESN = "";
                    _CPE_SN = "";
                    _CPE_MODE = "";
                    _MATERIAL_CODE_STBSN = "";
                    _STB_SN = "";
                    _MATERIAL_CODE_ATASN = "";
                    _ATA_SN = "";
                    _MATERIAL_CODE_WIFIROUTESN = "";
                    _WIFI_ROUTER_SN = "";
                    _STO_LOCATION = "";
                    _VENDOR_CODE = "";
                    _FOA_REJECT_REASON = "";
                    _RE_APPOINTMENT_REASON = "";
                    _PHASE_PO = "";
                    _SFF_SUBMITTED_DATE = "";
                    _EVENT_CODE = "";
                    _REGION = "";
                    _ENTRY_FEE = "";
                    _TOTAL_FEE = "";
                }
                else
                {
                    _ACC_NBR = result[0].ACC_NBR.ToSafeString();
                    _USER_NAME = result[0].USER_NAME.ToSafeString();
                    _SBC_CPY = result[0].SBC_CPY.ToSafeString();
                    _PRODUCT_NAME = result[0].PRODUCT_NAME.ToSafeString();
                    _ON_TOP1 = result[0].ON_TOP1.ToSafeString();
                    _ON_TOP2 = result[0].ON_TOP2.ToSafeString();
                    _VOIP_NUMBER = result[0].VOIP_NUMBER.ToSafeString();
                    _SERVICE_PACK_NAME = result[0].SERVICE_PACK_NAME.ToSafeString();
                    _ORD_NO = result[0].ORD_NO.ToSafeString();
                    _ORDER_TYPE = result[0].ORD_TYPE.ToSafeString();
                    _ORDER_SFF = result[0].ORDER_SFF.ToSafeString();
                    _APPOINTMENT_DATE = result[0].APPOINTMENT_DATE.ToDateDisplayText();
                    _SFF_ACTIVE_DATE = result[0].SFF_ACTIVE_DATE.ToDateDisplayText();
                    _APPROVE_JOB_FBSS_DATE = result[0].APPROVE_JOB_FBSS_DATE.ToDateDisplayText();
                    _COMPLETED_DATE = result[0].COMPLETED_DATE.ToDateDisplayText();
                    _WORK_STATUS = result[0].ORDER_STATUS.ToSafeString();
                    _REJECT_REASON = result[0].REJECT_REASON.ToSafeString();
                    _MATERIAL_CODE_CPESN = result[0].MATERIAL_CODE_CPESN.ToSafeString();
                    _CPE_SN = result[0].CPE_SN.ToSafeString();
                    _CPE_MODE = result[0].CPE_MODE.ToSafeString();
                    _MATERIAL_CODE_STBSN = result[0].MATERIAL_CODE_STBSN.ToSafeString();
                    _STB_SN = result[0].STB_SN.ToSafeString();
                    _MATERIAL_CODE_ATASN = result[0].MATERIAL_CODE_ATASN.ToSafeString();
                    _ATA_SN = result[0].ATA_SN.ToSafeString();
                    _MATERIAL_CODE_WIFIROUTESN = result[0].MATERIAL_CODE_WIFIROUTESN.ToSafeString();
                    _WIFI_ROUTER_SN = result[0].WIFI_ROUTER_SN.ToSafeString();
                    _STO_LOCATION = result[0].STO_LOCATION.ToSafeString();
                    _VENDOR_CODE = result[0].VENDOR_CODE.ToSafeString();
                    _FOA_REJECT_REASON = result[0].FOA_REJECT_REASON.ToSafeString();
                    _RE_APPOINTMENT_REASON = result[0].RE_APPOINTMENT_REASON.ToSafeString();
                    _PHASE_PO = result[0].PHASE_PO.ToSafeString();
                    _SFF_SUBMITTED_DATE = result[0].SFF_SUBMITTED_DATE.ToDateDisplayText();
                    _EVENT_CODE = result[0].EVENT_CODE.ToSafeString();
                    _REGION = result[0].REGION.ToSafeString();
                    _ENTRY_FEE = result[0].FEE_CODE.ToSafeString();
                    _TOTAL_FEE = result[0].TOTAL_FEE.ToSafeString();

                }
            }
            else
            {
                _ACC_NBR = "-";
                _USER_NAME = "-";
                _SBC_CPY = "-";
                _PRODUCT_NAME = "-";
                _ON_TOP1 = "-";
                _ON_TOP2 = "-";
                _VOIP_NUMBER = "-";
                _SERVICE_PACK_NAME = "-";
                _ORD_NO = "-";
                _ORDER_TYPE = "-";
                _ORDER_SFF = "-";
                _APPOINTMENT_DATE = "-";
                _SFF_ACTIVE_DATE = "-";
                _APPROVE_JOB_FBSS_DATE = "-";
                _COMPLETED_DATE = "-";
                _WORK_STATUS = "-";
                _REJECT_REASON = "-";
                _MATERIAL_CODE_CPESN = "-";
                _CPE_SN = "-";
                _CPE_MODE = "-";
                _MATERIAL_CODE_STBSN = "-";
                _STB_SN = "-";
                _MATERIAL_CODE_ATASN = "-";
                _ATA_SN = "-";
                _MATERIAL_CODE_WIFIROUTESN = "-";
                _WIFI_ROUTER_SN = "-";
                _STO_LOCATION = "-";
                _VENDOR_CODE = "-";
                _FOA_REJECT_REASON = "-";
                _RE_APPOINTMENT_REASON = "-";
                _PHASE_PO = "-";
                _SFF_SUBMITTED_DATE = "-";
                _EVENT_CODE = "-";
                _REGION = "-";
                _ENTRY_FEE = "-";
                _TOTAL_FEE = "-";

            }

            return Json(new
            {
                _ACC_NBR = _ACC_NBR,
                _USER_NAME = _USER_NAME,
                _SBC_CPY = _SBC_CPY,
                _PRODUCT_NAME = _PRODUCT_NAME,
                _ON_TOP1 = _ON_TOP1,
                _ON_TOP2 = _ON_TOP2,
                _VOIP_NUMBER = _VOIP_NUMBER,
                _SERVICE_PACK_NAME = _SERVICE_PACK_NAME,
                _ORD_NO = _ORD_NO,
                _ORDER_TYPE = _ORDER_TYPE,
                _ORDER_SFF = _ORDER_SFF,
                _APPOINTMENT_DATE = _APPOINTMENT_DATE,
                _SFF_ACTIVE_DATE = _SFF_ACTIVE_DATE,
                _APPROVE_JOB_FBSS_DATE = _APPROVE_JOB_FBSS_DATE,
                _COMPLETED_DATE = _COMPLETED_DATE,
                _WORK_STATUS = _WORK_STATUS,
                _REJECT_REASON = _REJECT_REASON,
                _MATERIAL_CODE_CPESN = _MATERIAL_CODE_CPESN,
                _CPE_SN = _CPE_SN,
                _CPE_MODE = _CPE_MODE,
                _MATERIAL_CODE_STBSN = _MATERIAL_CODE_STBSN,
                _STB_SN = _STB_SN,
                _MATERIAL_CODE_ATASN = _MATERIAL_CODE_ATASN,
                _ATA_SN = _ATA_SN,
                _MATERIAL_CODE_WIFIROUTESN = _MATERIAL_CODE_WIFIROUTESN,
                _WIFI_ROUTER_SN = _WIFI_ROUTER_SN,
                _STO_LOCATION = _STO_LOCATION,
                _VENDOR_CODE = _VENDOR_CODE,
                _FOA_REJECT_REASON = _FOA_REJECT_REASON,
                _RE_APPOINTMENT_REASON = _RE_APPOINTMENT_REASON,
                _PHASE_PO = _PHASE_PO,
                _SFF_SUBMITTED_DATE = _SFF_SUBMITTED_DATE,
                _EVENT_CODE = _EVENT_CODE,
                _REGION = _REGION,
                _ENTRY_FEE = _ENTRY_FEE,
                _TOTAL_FEE = _TOTAL_FEE,
            }, JsonRequestBehavior.AllowGet);

        }

        public List<reportInstallOrderDetailModel> getOrderDetail(string AccNO, string OrdNO)
        {
            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
                return null;
            try
            {
                var query = new GetReportInstallationOrderDetailQuery()
                {
                    p_ORDER_NO = OrdNO == "" ? "ALL" : OrdNO,
                    p_ACCESS_NO = AccNO == "" ? "ALL" : AccNO
                };
                var result = _queryProcessor.Execute(query);
                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        public List<reportInstallPostSapDetail> getPostSapDetail(string AccNO, string OrdNO)
        {
            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
                return null;
            try
            {
                var query = new GetReportInstallationCostbyOrderPostSapDetailQuery()
                {
                    p_ORDER_NO = OrdNO,
                    p_ACCESS_NO = AccNO

                };
                var result = _queryProcessor.Execute(query);
                return result;

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        public JsonResult getDistanceDetail([DataSourceRequest] DataSourceRequest request, string AccNO, string OrdNO)
        {
            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
            {
                return null;
            }
            try
            {
                var query = new GetReportInstallationDistanceDetailQuery()
                {
                    p_ORDER_NO = OrdNO,
                    p_ACCESS_NO = AccNO

                };
                var result = _queryProcessor.Execute(query);

                foreach (var item in result)
                {
                    item.ACTION_DATE_TEXT = item.ACTION_DATE.ToDateDisplayText();
                }

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateSCMOrderList([DataSourceRequest] DataSourceRequest request,
            ReportInstallationCostbyOrderListModel_Binding model)
        {

            try
            {

                //string result = "";
                var command = new ReportInstallationUpdateNoteCommand()
                {
                    p_ACCESS_NO = model.ACCESS_NUMBER,
                    p_ORDER_NO = model.ORDER_NO,
                    p_USER = CurrentUser.UserName,
                    p_REMARK_FOR_SUB = model.NOTE,
                };

                _ReportInstallationUpdateNoteCommand.Handle(command);


            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }



        public ActionResult SCMPostSapDetail([DataSourceRequest] DataSourceRequest request, string AccNO, string OrdNO)
        {
            var result = getPostSapDetail(AccNO, OrdNO);



            string _ordNo = string.Empty;
            string _ordDate = string.Empty;
            string _ordType = string.Empty;
            string _instCost = string.Empty;
            string _ReuseFlag = string.Empty;
            string _PrdName = string.Empty;
            string _Phase = string.Empty;
            string _SubContractName = string.Empty;
            string _ordStatus = string.Empty;
            string _addID = string.Empty;
            string _addName = string.Empty;
            string _subconttype = string.Empty;
            string _subcontsubtype = string.Empty;
            string _ruleID = string.Empty;

            if (result != null)
            {
                if (result.Count == 0)
                {
                    _ordNo = "";
                    _ordDate = "";
                    _ordType = "";
                    _instCost = "";
                    _ReuseFlag = "";
                    _PrdName = "";
                    _Phase = "";
                    _SubContractName = "";
                    _ordStatus = "";
                    _addID = "";
                    _addName = "";
                    _subconttype = "";
                    _subcontsubtype = "";
                    _ruleID = "";
                }
                else
                {

                    _ordNo = result[0].ORDER_NO.ToSafeString();
                    _ordDate = result[0].ORDER_DATE.ToDateDisplayText();
                    _ordType = result[0].ORDER_TYPE.ToSafeString().Trim();
                    _instCost = result[0].INSTALL_COST.ToSafeString().Trim();
                    _ReuseFlag = result[0].REUSE_FLAG.ToSafeString().Trim();
                    _PrdName = result[0].PRODUCT_NAME.ToSafeString().Trim();
                    _Phase = result[0].PHASE.ToSafeString().Trim();
                    _SubContractName = result[0].VENDOR_NAME.ToSafeString().Trim();
                    _ordStatus = result[0].ORDER_STATUS.ToSafeString().Trim();

                    _addID = result[0].ADDRESS_ID.ToSafeString().Trim();
                    _addName = result[0].BUILDING_NAME.ToSafeString().Trim();
                    _subconttype = result[0].SUBCONTRACT_TYPE.ToSafeString().Trim();
                    _subcontsubtype = result[0].SUBCONTRACT_SUB_TYPE.ToSafeString().Trim();
                    _ruleID = result[0].RULE_ID.ToSafeString().Trim();
                }
            }
            else
            {
                _ordNo = "-";
                _ordDate = "-";
                _ordType = "-";
                _instCost = "-";
                _ReuseFlag = "-";
                _PrdName = "-";
                _Phase = "-";
                _SubContractName = "-";
                _ordStatus = "-";
                _addID = "-";
                _addName = "-";
                _subconttype = "-";
                _subcontsubtype = "-";
                _ruleID = "-";

            }

            return Json(new
            {
                _ordNo = _ordNo,
                _ordDate = _ordDate,
                _instCost = _instCost,
                _ordType = _ordType,
                _ReuseFlag = _ReuseFlag,
                _PrdName = _PrdName,
                _Phase = _Phase,
                _SubContractName = _SubContractName,
                _ordStatus = _ordStatus,
                _addID = _addID,
                _addName = _addName,
                _subconttype = _subconttype,
                _subcontsubtype = _subcontsubtype,
                _ruleID = _ruleID,
            }, JsonRequestBehavior.AllowGet);

        }

        #region Dropdown list

        public JsonResult SelectProductname()
        {
            var data = SelectFbbCfgLov("PRODUCT_NAME_REPORT_INSTALLATION", "REPORT_INSTALLATION");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectRegion(string text)
        {
            var data = SelectFbbCfgLov("REGION_REPORT_INSTALLATION", "REPORT_INSTALLATION");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectIrDoc(string text)
        {
            var data = SelectFbbCfgLov("IR_DOC_REPORT_INSTALLATION", "REPORT_INSTALLATION");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectIrDocFLSUPDATE(string text)
        {
            var data = SelectFbbCfgLov("IR_DOC_REPORT_INSTALLATION", "REPORT_INSTALLATION");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectWorkFlowStatus()
        {
            var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectSubContractorName(string text)
        {
            var query = new ReportInstallationSelectSubContractorNameQuery
            {
                p_name = "",
                p_code = "",
                p_code_distinct = false
            };
            var data = _queryProcessor.Execute(query);
            data = data.Where(p => p.SUB_CONTRACTOR_NAME.Contains(text)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectVendorCode(string text)
        {
            var query = new ReportInstallationSelectSubContractorNameQuery
            {
                p_code = "",
                p_name = "",
                p_code_distinct = true
            };
            var data = _queryProcessor.Execute(query);
            data = data.Where(p => p.SUB_CONTRACTOR_CODE.Contains(text)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeddlSubContract(string subContractorName, string subContractorCode)
        {
            var query = new ReportInstallationSelectSubContractorNameQuery
            {
                p_name = subContractorName,
                p_code = subContractorCode,
                p_code_distinct = true
            };
            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeddlVendorcode(string subContractorName, string subContractorCode)
        {
            var query = new ReportInstallationSelectSubContractorNameQuery
            {
                p_name = subContractorName,
                p_code = subContractorCode,
                p_code_distinct = false
            };
            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectSubContractsubtype()
        {
            var data = SelectFbbCfgLov("SUBCONTRACT_SUB_TYPE_REPORT_INSTALLATION", "REPORT_INSTALLATION")
               .Where(d => d.LOV_VAL1 != null).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectSubContracttype()
        {


            List<LovValueModel> originalData = getLov("DROPDOWNLIST", "Subcontract Type")
                .Where(d => d.LovValue1 != null)
                .ToList();

            List<LovModel> data = originalData
                .Select(d => new LovModel
                {
                    DISPLAY_VAL = null,
                    LOV_VAL1 = d.LovValue1
                })
                .ToList();

            data.Insert(0, new LovModel
            {
                DISPLAY_VAL = "SELECT ALL",
                LOV_VAL1 = "ALL"
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectProductOwner()
        {
            var data = getLov("DROPDOWNLIST", "Product Owner").Where(d => d.LovValue1 != null).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOrderType()
        {
            var data = SelectFbbCfgLov("ORD_TYPE_REPORT_INSTALLATION", "REPORT_INSTALLATION")
               .Where(d => d.LOV_VAL1 != null).ToList();

            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectOrderStatus()
        {
            var data = SelectFbbCfgLov("ORD_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
               .Where(d => d.LOV_VAL1 != null).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatusUpdateByFile()
        {
            string _usergroup = string.Empty;
            _usergroup = GetUserGroup();
            if (_usergroup == "SCM")
            {
                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                     .Where(d => d.LOV_VAL2 != null && d.LOV_VAL2.Contains("SCM") && d.LOV_VAL1.Equals("Confirm Paid")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                    .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Paid") || d.LOV_VAL1.Equals("Hold")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult SelectACCTUpdateAllStatus()
        {

            var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                .Where(d => d.LOV_VAL1.Equals("Paid") || d.LOV_VAL1.Equals("Hold")).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectScmWorkFlowStatus(string WFS)
        {

            if (WFS == "Waiting Sub Verify")
            {
                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                    .Where(d => d.LOV_VAL1.Equals("Confirm Paid") || d.LOV_VAL1.Equals("Dispute")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Waiting Paid")
            {

                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                 .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Confirm Paid") || d.LOV_VAL1.Equals("Waiting Paid")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Dispute")
            {

                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                 .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Dispute")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Confirm Paid")
            {

                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                 .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Confirm Paid")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Hold")
            {
                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
              .Where(d => d.LOV_VAL1.Equals("Confirm Paid") || d.LOV_VAL1.Equals("Cancelled")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                     .Where(d => d.LOV_VAL2 != null && d.LOV_VAL2.Contains("SCM")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectReason(string text)
        {
            var data = SelectFbbCfgLov("REASON_REPORT_INSTALLATION", "REPORT_INSTALLATION");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SelectAccWorkFlowStatus(string order_status = "")
        {
            var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
                .Where(d => (d.LOV_VAL2 != null && d.LOV_VAL2.Contains("ACC")) || (!string.IsNullOrEmpty(d.LOV_VAL4) && d.LOV_VAL4.Contains("ACC"))).ToList();

            if (order_status == "Confirm Paid")
            {
                data = data.Where(x => x.LOV_VAL3 == "Confirm Paid").ToList();
            }
            else if (order_status == "Hold")
            {
                data = data.Where(x => x.LOV_VAL3 == "Hold").ToList();
            }
            else
            {
                data = data.ToList();
            }


            data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<LovModel> SelectFbbCfgLov(string lov_type, string lov_val5)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = lov_type,
                LOV_VAL5 = lov_val5
            };
            return _queryProcessor.Execute(query);
        }

        public List<LovValueModel> Get_FBB_CFG_LOV(string LOV_TYPE, string LOV_NAME)
        {
            var query = new GetLovWithParamsQuery()
            {
                LovType = LOV_TYPE,
                LovName = LOV_NAME
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);


            return _FbbCfgLov;
        }

        #endregion

        #region Update status by file
        public decimal ConvertValidateDistance(string datas)
        {
            decimal _valdis = 0;
            try
            {
                _valdis = Convert.ToDecimal(datas);
            }
            catch
            {
                _valdis = 0;
            }
            return _valdis;
        }
        public JsonResult ReportInstallationCostbyOrderUpdateByFile([DataSourceRequest] DataSourceRequest request, string status,
            string fileName)
        {
            if (string.IsNullOrEmpty(reportInstallFileModel.csv))
            {
                return Json("Please upload file", JsonRequestBehavior.AllowGet);
            }

            try
            {
                var lines = reportInstallFileModel.csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    throw new Exception("Blank .csv file");
                }

                var fileList = new List<ReportInstallation_FBB_update_file_list>();

                var ReportInstallationRecalList = new List<ReportInstallationRecal>();

                string accNbr = string.Empty;
                string invoiceNo = string.Empty;
                string invoiceDate = string.Empty;
                string irDoc = string.Empty;
                var irDocValue = new List<string> { "Y", "N" };

                string remark = string.Empty;
                string remarkForSub = string.Empty;
                decimal validateDis = 0;
                string reason = string.Empty;
                string tranferDate = string.Empty;

                #region csv to model
                switch (GetUserGroup())
                {
                    case "SCM":
                        foreach (var item in lines)
                        {
                            var values = item.Split(',', '|');
                            string chkData = values[0].ToSafeString();
                            if (chkData.Contains("End of File"))
                            {
                                break;
                            }
                            if (!string.IsNullOrEmpty(chkData) && (chkData.Length == 10 || chkData.Trim().ToUpper() == "ACCESS NUMBER") && values != null)
                            {
                                for (int i = 0; i <= values.Count() - 1; i++)
                                {
                                    if (i == 0)
                                    {
                                        accNbr = values[i];
                                    }
                                    else if (i == 1)
                                    {
                                        invoiceNo = values[i];
                                    }
                                    else if (i == 2)
                                    {
                                        invoiceDate = values[i];
                                    }
                                    else if (i == 3)
                                    {
                                        if (irDocValue.Any(s => values[i].Contains(s)))
                                        {
                                            irDoc = values[i];
                                        }
                                        else
                                        {
                                            throw new Exception("Invalid IR DOC value (Y/N).");
                                        }
                                    }
                                    else if (i == 4)
                                    {
                                        remark = values[i];
                                    }
                                    else if (i == 5)
                                    {
                                        DateTime dateNow = DateTime.Now;
                                        if (values[i].Contains("/"))
                                        {
                                            string date = values[i] + " 23:59:59";
                                            DateTime CheckDate = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                                            if (CheckDate >= dateNow)
                                            {
                                                tranferDate = values[i];
                                            }
                                            else
                                            {
                                                throw new Exception("Paid Date is Invaid.");
                                            }

                                        }
                                        else
                                        {
                                            tranferDate = values[i];
                                        }




                                    }


                                }

                                var model = new ReportInstallation_FBB_update_file_list
                                {
                                    ACC_NBR = accNbr,
                                    INVOICE_NO = invoiceNo,
                                    INVOICE_DATE = invoiceDate,
                                    IR_DOC = irDoc,
                                    REMARK = remark,
                                    TRANSFER_DATE = tranferDate,
                                    REMARK_FOR_SUB = remarkForSub
                                };

                                fileList.Add(model);
                            }

                        }
                        break;
                    case "FAPO":
                        foreach (var item in lines)
                        {
                            var values = item.Split(',', '|');
                            string chkData = values[0].ToSafeString();
                            if (chkData.Contains("End of File"))
                            {
                                break;
                            }
                            if (!string.IsNullOrEmpty(chkData) && (chkData.Length == 10 || chkData.Trim().ToUpper() == "ACCESS NUMBER") && values != null)
                            {
                                for (int i = 0; i <= values.Count() - 1; i++)
                                {
                                    if (i == 0)
                                    {
                                        accNbr = values[i];
                                    }
                                    else if (i == 1)
                                    {
                                        validateDis = ConvertValidateDistance(values[i]);
                                    }
                                    else if (i == 2)
                                    {
                                        reason = values[i];
                                    }
                                    else if (i == 3)
                                    {
                                        remark = values[i];
                                    }

                                }

                                //old list
                                //var model = new FBB_update_file_list
                                //{
                                //    ACC_NBR = accNbr,
                                //    VALIDATE_DIS = validateDis,
                                //    REASON = reason,
                                //    REMARK = remark
                                //};

                                //fileList.Add(model);

                                //new list 12.2021
                                var model = new ReportInstallationRecal
                                {
                                    ACCESS_NUMBER = accNbr.ToSafeString().Replace(" ", ""),
                                    //NEW_RULE_ID = "",
                                    ORDER_NO = "",
                                    DISTANCE = validateDis.ToSafeString(),
                                    FLAG_RECAL = "DISPUTE",
                                    REASON = reason,
                                    REMARK = remark
                                };
                                //AccessListModel.ACCESS_NUMBER = $("#accNo").val();
                                //AccessListModel.NEW_RULE_ID = "";
                                //AccessListModel.REMARK = remark;
                                //AccessListModel.ORDER_NO = res[0];
                                //AccessListModel.FLAG_RECAL = "DISPUTE";
                                //AccessListModel.DISTANCE = $("#VALIDATE_DIS2").val();
                                //AccessListModel.REASON = Reason;

                                ReportInstallationRecalList.Add(model);
                            }
                        }
                        break;
                    case "ACCT":
                        foreach (var item in lines)
                        {
                            var values = item.Split(',', '|');
                            string chkData = values[0].ToSafeString();

                            if (chkData.Contains("End of File"))
                            {
                                break;
                            }

                            if (!string.IsNullOrEmpty(chkData) && (chkData.Length == 10 || chkData.Trim().ToUpper() == "ACCESS NUMBER") && values != null)
                            {

                                for (int i = 0; i <= values.Count() - 1; i++)
                                {
                                    if (i == 0)
                                    {
                                        accNbr = values[i];
                                    }
                                    else if (i == 1)
                                    {
                                        invoiceNo = values[i];
                                    }
                                    else if (i == 2)
                                    {
                                        invoiceDate = values[i];
                                    }
                                    else if (i == 3)
                                    {
                                        tranferDate = values[i];
                                    }
                                    else if (i == 4)
                                    {
                                        remark = values[i];
                                    }


                                }

                                var model = new ReportInstallation_FBB_update_file_list
                                {
                                    ACC_NBR = accNbr,
                                    INVOICE_NO = invoiceNo,
                                    INVOICE_DATE = invoiceDate,
                                    TRANSFER_DATE = tranferDate,
                                    REMARK = remark
                                };

                                fileList.Add(model);
                            }

                        }
                        break;
                    default:
                        break;
                }
                #endregion

                string code = string.Empty;
                string msg = string.Empty;
                if (GetUserGroup() == "FAPO")
                {
                    ReportInstallationRecalList.RemoveAt(0);
                    if (ReportInstallationRecalList.Count > 0)
                    {
                        var command = new ReportInstallationRecalByOrderCommand()
                        {
                            p_recal_access_list = ReportInstallationRecalList,
                            //p_NEW_RULE_ID = new_ruid,
                            p_USER = CurrentUser.UserName,
                            //p_REMARK = remark

                        };
                        _ReportInstallationRecalByOrderCommand.Handle(command);

                        #region Call API Subpayment
                        ReportInstallationModelApiSubpayment modelSub = new ReportInstallationModelApiSubpayment();
                        modelSub.Order_list = new List<ReportInstallationOrderList>();
                        List<ReportInstallationOrderList> ListApi = new List<ReportInstallationOrderList>();
                        foreach (var item in command.return_subpayment_cur)
                        {
                            var model = new ReportInstallationOrderList()
                            {
                                Internet_no = item.access_number,
                                Order_no = item.order_no,
                                Distance_to_paid = item.distance_to_paid.ToSafeString(),
                                Total_Paid = item.total_paid.ToSafeString(),
                                Product = item.product,
                                Order_type = item.order_type,
                                Vendor_code = item.vendor_code,
                                LMD_status = item.lmd_status
                            };
                            ListApi.Add(model);
                        }
                        modelSub.Order_list.AddRange(ListApi);

                        if (modelSub.Order_list.Count != 0)
                        {
                            var result = Call_Subpayment_APIAsync(modelSub, CurrentUser.UserName);
                        }
                        Session["TempSearchCriteria"] = null;

                        #endregion

                        if (command.ret_code == "0")
                        {
                            msg = "Update file success.";
                        }
                        else
                        {
                            msg = "Update file not success.";
                        }
                    }
                    else
                    {
                        msg = "Record Data Not Found";
                    }

                }
                else
                {
                    fileList.RemoveAt(0);
                    if (fileList.Count > 0)
                    {
                        var command = new ReportInstallationCostbyOrderUpdateByFileCommand()
                        {
                            p_INTERFACE = GetUserGroup(),
                            p_USER = CurrentUser.UserName,
                            p_STATUS = status,
                            p_filename = fileName,
                            p_file_list = fileList
                        };
                        _ReportInstallationCostbyOrderUpdateByFileCommand.Handle(command);
                        code = command.ret_code;
                        msg = command.ret_msg;
                        if (code == "0")
                        {
                            msg = "Update file success.";
                        }
                    }
                    else
                    {
                        msg = "Record Data Not Found";
                    }
                }
                Session["TempSearchCriteria"] = null;

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                if (e.GetErrorMessage().Contains("Index was outside the bounds of the array."))
                {
                    return Json("Cannot import excel.", JsonRequestBehavior.AllowGet);
                }
                return Json(e.GetErrorMessage(), JsonRequestBehavior.AllowGet);
            }
        }

        private ActionResult UploadReportInstallationFile(HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);
            string userGroup = GetUserGroup();

            if (fileName != null && Path.GetExtension(fileName).ToLower() == ".csv")
            {
                var impersonateVar = LovData.SingleOrDefault(l => l.Type == "CONFIG_PATH" && l.Text == userGroup);
                if (impersonateVar != null)
                {
                    string user = impersonateVar.LovValue1;
                    string pass = impersonateVar.LovValue2;
                    string ip = impersonateVar.LovValue3;
                    string directoryPath = impersonateVar.LovValue4;
                    fileName = userGroup + "_REPORTINSTALLATION_UPLOAD_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                    using (var impersonator = new Impersonator(user, ip, pass, false))
                    {
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        var physicalPath = Path.Combine(directoryPath, fileName);
                        file.SaveAs(physicalPath);

                        var modelResponse = new { status = true, message = "File's exceeded", fileName };
                        return Json(modelResponse, "text/plain");
                    }
                }
            }
            else
            {
                var modelResponse = new { status = false, message = fileName + "is already exist.", fileName };
                return Json(modelResponse, "text/plain");
            }

            return Content("");
        }

        public ActionResult ScmReportInstallationfile_Save(IEnumerable<HttpPostedFileBase> scmReportInstallationfile)
        {
            if (scmReportInstallationfile != null)
            {
                try
                {
                    foreach (var file in scmReportInstallationfile)
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".csv")
                        {
                            // Read bytes from http input stream
                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            reportInstallFileModel.csv = System.Text.Encoding.Default.GetString(binData);
                        }
                        else
                        {
                            var modelResponse = new { status = false, message = "Please upload .csv file extension", fileName = file.FileName };
                            return Json(modelResponse, "text/plain");
                        }
                    }
                }
                catch (Exception e)
                {
                    var modelResponse = new { status = false, message = e.GetErrorMessage(), fileName = "" };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }

        public ActionResult ScmReportInstallationfile_Remove(string[] scmReportInstallationfile)
        {
            if (scmReportInstallationfile != null)
            {
                try
                {
                    reportInstallFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        public ActionResult FapoReportInstallation_Save(IEnumerable<HttpPostedFileBase> fapoReportInstallationfile)
        {
            if (fapoReportInstallationfile != null)
            {
                try
                {
                    foreach (var file in fapoReportInstallationfile)
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".csv")
                        {
                            // Read bytes from http input stream
                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            reportInstallFileModel.csv = Encoding.Default.GetString(binData);
                        }
                        else
                        {
                            var modelResponse = new { status = false, message = "Please upload .csv file extension", fileName = file.FileName };
                            return Json(modelResponse, "text/plain");
                        }
                    }
                }
                catch (Exception e)
                {
                    var modelResponse = new { status = false, message = e.GetErrorMessage(), fileName = "" };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }

        public ActionResult FapoReportInstallationfile_Remove(string[] fapoReportInstallationfile)
        {
            if (fapoReportInstallationfile != null)
            {
                try
                {
                    reportInstallFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        public ActionResult AccReportInstallationfile_Save(IEnumerable<HttpPostedFileBase> accReportInstallationfile)
        {
            if (accReportInstallationfile != null)
            {
                try
                {
                    foreach (var file in accReportInstallationfile)
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".csv")
                        {
                            // Read bytes from http input stream
                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            reportInstallFileModel.csv = System.Text.Encoding.Default.GetString(binData);
                        }
                        else
                        {
                            var modelResponse = new { status = false, message = "Please upload .csv file extension", fileName = file.FileName };
                            return Json(modelResponse, "text/plain");
                        }
                    }
                }
                catch (Exception e)
                {
                    var modelResponse = new { status = false, message = e.GetErrorMessage(), fileName = "" };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }

        public ActionResult AccReportInstallationfile_Remove(string[] accReportInstallationfile)
        {
            if (accReportInstallationfile != null)
            {
                try
                {
                    reportInstallFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        private List<LovValueModel> getLov(string lov_type, string lov_name)
        {
            var headerQuery = new GetLovQuery
            {
                LovType = lov_type,
                LovName = lov_name,
            };
            var data = _queryProcessor.Execute(headerQuery);
            return data;
        }

        [HttpGet]
        public ActionResult GetPartialFromRadioBtn(string path)
        {
            if (path == "price")
            {
                return PartialView(Url.Content("~/Views/ReportInstallationCostbyOrder/_PartialReCalDistanceFAPOOrderPopup.cshtml"));
            }
            else
            {
                return PartialView(Url.Content("~/Views/ReportInstallationCostbyOrder/_PartialEditFAPOOrderPopupWithRadioBtn.cshtml"));
            }
        }

        #endregion
        #region ExportSample
        public ActionResult ExportSample([DataSourceRequest] DataSourceRequest request, string Status = "")
        {
            //DataTable table = new DataTable();
            //string tempPath = System.IO.Path.GetTempPath();
            //string _Status = Status;
            string usertype = GetUserGroup();
            if (usertype == "SCM")
            {
                return RedirectToAction("ExportTemplateConfirmPaid");
            }
            else if (usertype == "FAPO")
            {
                return RedirectToAction("ExportTemplateValidateDistance");
            }
            else if (usertype == "ACCT")
            {
                return RedirectToAction("ExportTemplatePaid");
            }
            return null;

            //        table = SetEntity("ExcelTemplate" + Status, Status, usertype);
            //var data_ = GenerateALLExcel(table, "Template" + Status, tempPath);
            //return null;//File(data_, "application/excel", "Template" + Status + ".csv");

        }

        private byte[] GenerateALLExcel(DataTable dataToExcel, string fileName, string directoryPath)
        {
            _Logger.Info("ExcelTemplate start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".csv"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".csv"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.csv", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow = 2;
            int iHeaderRow = 0;
            string strRow = iRow.ToSafeString();
            string strHeader = iHeaderRow.ToSafeString();
            string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            string strColumn2 = string.Empty;

            int i = 0;

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment;filename=test.csv");


            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(fileName);

                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;

                iRow = 1;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                worksheet.Cells["A1"].LoadFromDataTable(dataToExcel, false);
                var ms = new System.IO.MemoryStream();
                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"ExcelTemplate";
                package.Workbook.Properties.Author = "ExcelTemplate";
                package.Workbook.Properties.Subject = @"ExcelTemplate" + fileName;


                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);

                byte[] datas = null;// System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return datas;
            }



            //byte[] datas =  System.IO.File.ReadAllBytes(finalFileNameWithPath);
            // return null;

        }



        private DataTable SetEntity(string fileName, string status, string usergroup)
        {
            string _TableName = fileName;

            // int col = 0;

            DataTable table = new DataTable();
            table.Columns.Add("ACC_NBR", System.Type.GetType("System.String"));
            table.Columns.Add("INVOICE_NO", System.Type.GetType("System.String"));
            table.Columns.Add("INVOICE_DATE", System.Type.GetType("System.String"));
            table.Columns.Add("IR_DOC", System.Type.GetType("System.String"));
            table.Columns.Add("SCMREMARK", System.Type.GetType("System.String"));
            table.Columns.Add("REMARK_FOR_SUB", System.Type.GetType("System.String"));
            table.Columns.Add("VALIDATE_DIS", System.Type.GetType("System.String"));
            table.Columns.Add("REASON", System.Type.GetType("System.String"));
            table.Columns.Add("TRANFERDATE", System.Type.GetType("System.String"));
            table.Columns.Add("REMARK", System.Type.GetType("System.String"));



            object[] values = new object[table.Columns.Count];
            values = new object[table.Columns.Count];
            values[0] = "88xxxxxxxx";
            values[1] = "IVR0001";
            values[2] = "01/01/2019";
            values[3] = "Y";
            values[4] = "CS Approve แล้ว";
            values[5] = "ลูกค้าขอเลื่อนเวลาการเข้าติดตั้ง ขอคืนงานเพื่อให้ทำการนัดใหม่";
            values[6] = "60";
            values[7] = "จ่ายจริงตามระยะ";
            values[8] = "01/01/2019";
            values[9] = "REMARK";
            table.Rows.Add(values);
            if (usergroup == "SCM")
            {


                if (status == "Dispute")
                {

                    table.Columns.Remove("REASON");
                    table.Columns.Remove("TRANFERDATE");
                    table.Columns.Remove("REMARK");


                }
                else
                {

                    table.Columns.Remove("VALIDATE_DIS");
                    table.Columns.Remove("REASON");
                    table.Columns.Remove("TRANFERDATE");
                    table.Columns.Remove("REMARK");

                }
            }
            if (usergroup == "FAPO")
            {
                table.Columns.Remove("INVOICE_NO");
                table.Columns.Remove("INVOICE_DATE");
                table.Columns.Remove("IR_DOC");
                table.Columns.Remove("SCMREMARK");
                table.Columns.Remove("REMARK_FOR_SUB");
                table.Columns.Remove("TRANFERDATE");
                //  accNbr = values[0];
                //  validateDis = Convert.ToDecimal(values[1]);
                //   reason = values[2];
                //  remark = values[3];

            }
            if (usergroup == "ACCT")
            {
                table.Columns.Remove("VALIDATE_DIS");
                table.Columns.Remove("INVOICE_NO");
                table.Columns.Remove("INVOICE_DATE");
                table.Columns.Remove("IR_DOC");
                table.Columns.Remove("SCMREMARK");
                table.Columns.Remove("REMARK_FOR_SUB");
                table.Columns.Remove("REASON");

                //  accNbr = values[0];
                //  tranferDate = values[1];
                //  remark = values[2];
            }
            return table;
        }
        #endregion

        public ActionResult ClearListEquip()
        {
            resultInstall = null;
            newResultList = null;
            return Json("Success");
        }
    }
}
