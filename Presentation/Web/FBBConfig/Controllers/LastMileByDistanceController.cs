using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public partial class LastMileByDistanceController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendMailLastMileNotificationCommand> _sendMail;
        private readonly ICommandHandler<SendMailLastMileCommand> _SendMailLastMileExp;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLogAPICommand;

        private readonly ICommandHandler<UpdateLastMileByDistanceByOrderCommand>
            _UpdateLastMileByDistanceByOrderCommand;

        private readonly ICommandHandler<LastMileUpdateNoteCommand> _LastMileUpdateNoteCommand;
        private readonly ICommandHandler<LastMileByDistanceUpdateByFileCommand> _LastMileByDistanceUpdateByFileCommand;

        //R19.03
        private readonly ICommandHandler<LastMileByDistanceRecalByOrderCommand> _LastMileByDistanceRecalByOrderCommand;
        private readonly ICommandHandler<LastMileByDistanceRecalByFileCommand> _LastMileByDistanceRecalByFileCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        public LastMileByDistanceController(ILogger logger, IQueryProcessor queryProcessor
            , ICommandHandler<SendMailLastMileNotificationCommand> sendMail
            , ICommandHandler<SendMailLastMileCommand> SendMailLastMileExp
            , ICommandHandler<UpdateLastMileByDistanceByOrderCommand> UpdateLastMileByDistanceByOrderCommand
            , ICommandHandler<LastMileUpdateNoteCommand> LastMileUpdateNoteCommand
            , ICommandHandler<LastMileByDistanceUpdateByFileCommand> LastMileByDistanceUpdateByFileCommand
            , ICommandHandler<LastMileByDistanceRecalByOrderCommand> LastMileByDistanceRecalByOrderCommand
            , ICommandHandler<LastMileByDistanceRecalByFileCommand> LastMileByDistanceRecalByFileCommand
            , ICommandHandler<InterfaceLogCommand> intfLogCommand
            , ICommandHandler<InterfaceLogPayGCommand> intfLogAPICommand

            )
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _sendMail = sendMail;
            _SendMailLastMileExp = SendMailLastMileExp;
            _UpdateLastMileByDistanceByOrderCommand = UpdateLastMileByDistanceByOrderCommand;
            _LastMileUpdateNoteCommand = LastMileUpdateNoteCommand;
            _LastMileByDistanceUpdateByFileCommand = LastMileByDistanceUpdateByFileCommand;
            _LastMileByDistanceRecalByOrderCommand = LastMileByDistanceRecalByOrderCommand;
            _LastMileByDistanceRecalByFileCommand = LastMileByDistanceRecalByFileCommand;
            _intfLogCommand = intfLogCommand;
            _intfLogAPICommand = intfLogAPICommand;
        }


        // GET: /LastMileByDistance/
        public ActionResult Index()
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            //  CurrentUser.E
            ViewBag.User = CurrentUser;
            ViewBag.UserGroup = GetUserGroup();

            SetViewBagLovV2("L_SEARCH", "L_SEARCH");
            SetViewBagLovV2("L_ORD_LIST_SCM", "L_ORD_LIST_SCM");
            SetViewBagLovV2("L_ORD_LIST_FAPO", "L_ORD_LIST_FAPO");
            SetViewBagLovV2("L_ORD_LIST_ACCT", "L_ORD_LIST_ACCT");

            SetViewBagLovV2("L_ORD_DETAIL", "L_ORD_DETAIL");
            SetViewBagLovV2("L_DIST_DETAIL", "L_DIST_DETAIL");
            SetViewBagLovV2("L_ORD_HIS", "L_ORD_HIS");
            SetViewBagLovV2("L_POST_SAP_DETAIL", "L_POST_SAP_DETAIL");

            var model = new LastMileByDistanceModel();
            Session["TempSearchCriteria"] = null;
            Session["TempTotalPrice"] = null;
            return View(model);
        }

        private string GetUserGroup()
        {
            string ReSult = "";
            var query = new GetUserGroupQuery()
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

        // Set DDL ProductOwner
        public JsonResult SetDDLProductOwner()
        {
            var data = Get_CONFIG_LOV("DROPDOWNLIST", "Product Owner").Where(d => d.LovValue1 != null).ToList();

            //var LovQueryData = _queryProcessor.Execute(query).ToList();
            //var LovData = LovQueryData.Select(p => { return new { LOV_NAME = p.LovValue1, LOV_VAL1 = p.LovValue1 }; }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public List<LovValueModel> Get_CONFIG_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetLovQuery()
            {
                LovType = _CON_TYPE,
                LovName = _CON_NAME
            };
            var _ConfigLov = _queryProcessor.Execute(query).OrderBy(p => p.OrderBy).ToList();

            return _ConfigLov;
        }

        private void SetViewBagLovV2(string screenType, string type)
        {
            var query = new GetLovV2Query()
            {
                LovType = screenType
            };

            var LovDataScreen = _queryProcessor.Execute(query).ToList();

            if (type == "L_SEARCH")
            {
                ViewBag.SearchListScreen = LovDataScreen;
            }

            else if (type == "L_ORD_LIST_SCM")
            {
                ViewBag.SCMOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_FAPO")
            {
                ViewBag.FAPOOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_ACCT")
            {
                ViewBag.ACCOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_DETAIL")
            {
                ViewBag.OrderDetailListScreen = LovDataScreen;
            }
            else if (type == "L_DIST_DETAIL")
            {
                ViewBag.DisDetailListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_HIS")
            {
                ViewBag.OrderHisListScreen = LovDataScreen;
            }
            else if (type == "L_POST_SAP_DETAIL")
            {
                ViewBag.POSTSAPListScreen = LovDataScreen;
            }

            
        }

        private void SetViewBagLov(string screenType, string type)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType).ToList();
            if (type == "L_SEARCH")
            {
                ViewBag.SearchListScreen = LovDataScreen;
            }

            else if (type == "L_ORD_LIST_SCM")
            {
                ViewBag.SCMOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_FAPO")
            {
                ViewBag.FAPOOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_LIST_ACCT")
            {
                ViewBag.ACCOrderListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_DETAIL")
            {
                ViewBag.OrderDetailListScreen = LovDataScreen;
            }
            else if (type == "L_DIST_DETAIL")
            {
                ViewBag.DisDetailListScreen = LovDataScreen;
            }
            else if (type == "L_ORD_HIS")
            {
                ViewBag.OrderHisListScreen = LovDataScreen;
            }
            else if (type == "L_POST_SAP_DETAIL")
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
        private string rptName = "LastMileByDistance";

        public ActionResult ExportExcel(string dataS = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            LastMileByDistanceModel searchModel = (LastMileByDistanceModel)TempData.Peek("TempSearchCriteria");
            var result = GetLastMileOrderListByPage(searchModel, 1, Decimal.MaxValue);
            decimal? sumtotal = 0; decimal? sumvat = 0; decimal? sumincvat = 0;
            var items = result.cur.FirstOrDefault();
            sumtotal = items.TOTAL_PAID_AMOUNT;
            sumvat = items.TOTAL_VAT;
            sumincvat = items.TOTAL_AMOUNT;

            string filename = "LASTMILEByDistance";
            var bytes = GenerateEntitytoExcel(result.cur, filename, sumtotal, sumvat, sumincvat);

            return File(bytes, "application/octet-stream", filename + ".xlsx");
        }

        public string EmailTemplate(string fileName, LastMileByDistanceModel searchModel)
        {

            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                StringBuilder tempBody = new StringBuilder();

                string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
                string orderStatus = searchModel.ORDER_STATUS == null ? "" : string.Join(",", searchModel.ORDER_STATUS);

                #region tempBody

                tempBody.Append("<p style='font-weight:bolder;'>เรียน..." + CurrentUser.UserFullNameInThai + "</p>");
                tempBody.Append("<br/>");


                tempBody.Append("<span> ExportDate:" + DateNow.ToSafeString());
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<span>File Export Last Mile By Distance available for Download :" + fileName);
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
                if (orderStatus != "")
                {
                    tempBody.Append("<span>Work Flow Status:" + orderStatus);
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
                string urlName = url + "LastMileByDistance/DownloadFileExport?fileName=" + fileName;
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
            try
            {
                string tempPath = Path.GetTempPath();
                var filepath = tempPath + fileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                return File(fileBytes, "application/octet-stream", fileName + ".xlsx");
            }
            catch (Exception Ex)
            {
                _Logger.Info("DownLoad: " + Ex.GetErrorMessage());
                return null;
            }
        }


        public async Task ExportByEmail(string Email = "")
        {
            LastMileByDistanceModel searchModel = (LastMileByDistanceModel)TempData.Peek("TempSearchCriteria");
            InterfaceLogCommand log = null;
            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                _Logger.Info("ExportExcelByAsync" + DateNow.ToSafeString());
                Task<string> Logresult = null;
                log = StartInterface(searchModel, "EmailExport", "", null, "EmailExport", "Start EmailExport");

                LastMileByDistanceOrderListReturn result = await getAllRecord();
                decimal? sumtotal = 0; decimal? sumvat = 0; decimal? sumincvat = 0;

                var items = result.cur.FirstOrDefault();
                sumtotal = items.TOTAL_PAID_AMOUNT;
                sumvat = items.TOTAL_VAT;
                sumincvat = items.TOTAL_AMOUNT;
                string filename = "LastMileByDistance_" + CurrentUser.UserName.ToSafeString() + DateNow;

                GenerateEntitytoExcelAsync(result.cur, filename, sumtotal, sumvat, sumincvat, searchModel, Email, log);
                EndInterface(searchModel, log, null, "Success", "Send EmailComplete", "TotalRecord" + result.cur.Count.ToSafeString(), CurrentUser.UserName.ToSafeString());
                _Logger.Info("SuccessExportExcelByAsync" + DateNow.ToSafeString());
            }
            catch (Exception Ex)
            {
                EndInterface(searchModel, log, null, "Error", Ex.Message.ToSafeString(), null, "FBBConfig");
                _Logger.Info("Fail:" + Ex.Message.ToSafeString());
            }
        }

        private async Task<string> sendEmailExport(string fileName, LastMileByDistanceModel searchModel, string Email, InterfaceLogCommand log)
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
                var command = new SendMailLastMileNotificationCommand
                {
                    ProcessName = "SEND_EMAIL_FIXED_LASTMILE",
                    Subject = "ExportFile LMD Success:" + DateNow,
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
        private async Task<string> GenerateEntitytoExcelAsync<T>(List<T> data, string fileName, decimal? sumtotal, decimal? sumvat, decimal? sumincvat, LastMileByDistanceModel searchModel, string Email, InterfaceLogCommand log)
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

                var resData = SelectFbbCfgLov("EMAILEXPORT", "FIXED_LASTMILE")
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
                    table.Rows.Add(dr);
                    table = AcctOrderExcelColumn(table);
                }

                string tempPath = Path.GetTempPath();


                string tempFilePath = Path.Combine(tempPath, fileName + ".xlsx");

                //Delete existing file with same file name.
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }

                var newFile = new FileInfo(tempFilePath);
                ScmOrderListModelBySendMail scmOrderListModelBySendMail = new ScmOrderListModelBySendMail();

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
                using (var package = new ExcelPackage(newFile))
                {

                    for (int i = 0; i <= splitdt.Count - 1; i++)
                    {
                        int page = 0;
                        page = i + 1;
                        //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet" + page);
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
                        worksheet.Cells[strColumn1].LoadFromDataTable(splitdt[i], true, TableStyles.None);
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        //Step 4 : (Optional) Set the file properties like title, author and subject
                        package.Workbook.Properties.Title = @"FBB Config";
                        package.Workbook.Properties.Author = "FBB";
                        package.Workbook.Properties.Subject = @"" + "Sheet";

                    }

                    //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                    //  package.Save();
                    scmOrderListModelBySendMail.FileData = package.GetAsByteArray();

                    // byte[] data = System.IO.File.ReadAllBytes(tempFilePath);
                    //return data;
                }
                //scmOrderListModelBySendMail.msExcel = new MemoryStream(scmOrderListModelBySendMail.FileData);
                // _Logger.Info("EndSplitSheet" + DateNow.ToSafeString());
                //_Logger.Info("StartFileStream" + DateNow.ToSafeString());
                using (FileStream fs = new FileStream(tempPath.Trim() + fileName.Trim(), FileMode.OpenOrCreate))
                {
                    new MemoryStream(scmOrderListModelBySendMail.FileData).CopyTo(fs);
                    fs.Flush();
                }
                _Logger.Info("EndFileStream" + DateNow.ToSafeString());
                _Logger.Info("GenerateSuccess");
                _Logger.Info("StartSendEmail" + DateNow.ToSafeString());
                sendEmailExport(fileName, searchModel, Email, log);
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

            var data_ = GenerateExcel(table, "WorkSheet", tempPath, fileName);
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
                "SUBCONTRACT_LOCATION",
                "PRODUCT_OWNER"
                );


            GetScmOrderExcelCaption(table);

            return table;
        }

        private void GetScmOrderExcelCaption(DataTable table)
        {
            //var configscreen = LovData.Where(p => p.Type == "L_ORD_LIST_SCM").ToList();
            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_SCM"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();
            table.Columns["ACCESS_NUMBER_MASKING"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ACC_NBR").LovValue1 ?? "";
            table.Columns["ACCOUNT_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_USER_NAME").LovValue1 ?? "";
            table.Columns["SBC_CPY"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SBC_CPY").LovValue1 ?? "";
            table.Columns["PRODUCT_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PRODUCT_NAME").LovValue1 ?? "";
            table.Columns["ON_TOP1"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ON_TOP1").LovValue1 ?? "";
            table.Columns["ON_TOP2"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ON_TOP2").LovValue1 ?? "";
            table.Columns["VOIP_NUMBER"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_VOIP_NUMBER").LovValue1 ?? "";
            table.Columns["PROMOTION_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SERVICE_PACK_NAME").LovValue1 ?? "";
            table.Columns["ORDER_NO"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORD_NO").LovValue1 ?? "";
            table.Columns["ORD_TYPE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORD_TYPE").LovValue1 ?? "";
            table.Columns["ORDER_SFF"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORDER_SFF").LovValue1 ?? "";
            table.Columns["APPOINTMENNT_DT_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_APPOINTMENT_DATE").LovValue1 ?? "";
            table.Columns["SFF_ACTIVE_DATE_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SFF_ACTIVE_DATE").LovValue1 ?? "";
            table.Columns["CS_APPROVE_DATE_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_APPROVE_JOB_FBSS_DATE").LovValue1 ?? "";
            table.Columns["ORDER_STATUS_DT_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_COMPLETED_DATE").LovValue1 ?? "";
            table.Columns["OM_ORDER_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_OM_ORDER_STATUS").LovValue1 ?? "";
            table.Columns["REJECT_REASON"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_REJECT_REASON").LovValue1 ?? "";
            table.Columns["MATERIAL_CODE_CPESN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_MATERIAL_CODE_CPESN").LovValue1 ?? "";
            table.Columns["CPE_SN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_CPE_SN").LovValue1 ?? "";
            table.Columns["CPE_MODE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_CPE_MODE").LovValue1 ?? "";
            table.Columns["MATERIAL_CODE_STBSN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_MATERIAL_CODE_STBSN").LovValue1 ?? "";
            table.Columns["STB_SN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_STB_SN").LovValue1 ?? "";
            table.Columns["MATERIAL_CODE_ATASN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_MATERIAL_CODE_ATASN").LovValue1 ?? "";
            table.Columns["ATA_SN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ATA_SN").LovValue1 ?? "";
            table.Columns["MATERIAL_CODE_WIFIROUTESN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_MATERIAL_CODE_WIFIROUTESN").LovValue1 ?? "";
            table.Columns["WIFI_ROUTER_SN"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_WIFI_ROUTER_SN").LovValue1 ?? "";
            table.Columns["STO_LOCATION"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_STO_LOCATION").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_CODE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_VENDOR_CODE").LovValue1 ?? "";
            table.Columns["FOA_REJECT_REASON"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_FOA_REJECT_REASON").LovValue1 ?? "";
            table.Columns["RE_APPOINTMENT_REASON"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RE_APPOINTMENT_REASON").LovValue1 ?? "";
            table.Columns["PHASE_PO"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PHASE_PO").LovValue1 ?? "";
            table.Columns["SFF_SUBMITTED_DATE_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SFF_SUBMITTED_DATE").LovValue1 ?? "";
            table.Columns["EVENT_CODE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_EVENT_CODE").LovValue1 ?? "";
            table.Columns["REGION"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_REGION").LovValue1 ?? "";
            table.Columns["ENTRY_FEE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_TOTAL_FEE").LovValue1 ?? "";
            table.Columns["FEE_CODE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_FEE_CODE").LovValue1 ?? "";
            table.Columns["ADDR_ID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ADDR_ID").LovValue1 ?? "";
            table.Columns["INSTALLATION_ADDRESS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INSTALLATION_ADDRESS").LovValue1 ?? "";

            table.Columns["SUBCONTRACT_TYPE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUB_CONTRACT_TYPE").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_SUB_TYPE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUB_CONTRACT_SUB_TYPE").LovValue1 ?? "";


            table.Columns["REQUEST_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Request_Distance").LovValue1 ?? "";
            table.Columns["APPROVE_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Approve_Distance").LovValue1 ?? "";
            table.Columns["APPROVE_STAFF"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Approve_STAFF").LovValue1 ?? "";
            table.Columns["APPROVE_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Approve_STATUS").LovValue1 ?? "";
            table.Columns["REUSED_FLAG"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Reuse_Flag").LovValue1 ?? "";
            table.Columns["REQUEST_SUB_FLAG"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_REQUEST_SUB_FLAG").LovValue1 ?? "";


            //  table.Columns["DISTANCE_PAID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Distance_To_Paid").LovValue1 ?? "";
            table.Columns["OUTDOOR_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Out_door").LovValue1 ?? "";
            table.Columns["INDOOR_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_In_door").LovValue1 ?? "";

            table.Columns["MAPPING_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Maping_Cost").LovValue1 ?? "";
            table.Columns["OVER_LENGTH"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Over_Length").LovValue1 ?? "";
            table.Columns["OVER_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Over_Cost").LovValue1 ?? "";
            table.Columns["TOTAL_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_TOTAL_COST").LovValue1 ?? "";

            table.Columns["RECAL_DIS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_DIS").LovValue1 ?? "";
            table.Columns["RECAL_RATE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_RATE").LovValue1 ?? "";
            table.Columns["RECAL_OVER_LENGTH"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_OVER_LENGTH").LovValue1 ?? "";
            table.Columns["RECAL_OVER_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_OVER_COST").LovValue1 ?? "";
            table.Columns["RECAL_MAPPING_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_MAPPING_COST").LovValue1 ?? "";

            // table.Columns["DIFF_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DIFF_DISTANCE").LovValue1 ?? "";



            table.Columns["MAX_LENGTH"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_MAX_LENGTH").LovValue1 ?? "";

            table.Columns["TOTAL_PAID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Total Paid").LovValue1 ?? "";


            table.Columns["ORDER_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Status ").LovValue1 ?? "";
            table.Columns["RULE_ID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Rule_ID").LovValue1 ?? "";

            table.Columns["REMARK"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Remark").LovValue1 ?? "";
            table.Columns["IR_DOC"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_IR_DOC").LovValue1 ?? "";
            table.Columns["INVOICE_NO"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INVOICE_NO").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_LOCATION"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUBCONTRACT_LOCATION").LovValue1 ?? "";
            table.Columns["PRODUCT_OWNER"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PRODUCT_OWNER").LovValue1 ?? "";

            //  table.Columns["NOTE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_NOTE").LovValue1 ?? "";
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
            table.Columns.Remove("FOA_SUBMIT_DATE");
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

            SetColumnsOrder(table,
                "ORDER_STATUS",
                "ACCESS_NUMBER_MASKING",
                "SUBCONTRACT_NAME",
                "DISTANCE_LASTMILE_APP",
                "DISTANCE_ESRI",
                "DISTANCE_GMAP",
                "DISTANCE_STRAIT",
                "DISPUTE_DISTANCE",
                "DISTANCE_VALIDATE",
                "DISTANCE_TOTAL",
                  // "DISTANCE_PAID",
                  //  "Distance_MORE_325",
                  "SUBCONTRACT_TYPE",
                 "SUBCONTRACT_SUB_TYPE",
            "REQUEST_DISTANCE",
            "APPROVE_DISTANCE",
            "APPROVE_STAFF",
            "APPROVE_STATUS",
            "REUSED_FLAG",
              "REQUEST_SUB_FLAG",

                "ENTRY_FEE",
                "INDOOR_COST",
                "OUTDOOR_COST",
                "MAPPING_COST",
            "OVER_LENGTH",
            "OVER_COST",
            "TOTAL_COST",
             "RECAL_DIS",
            "RECAL_RATE",
            "RECAL_MAPPING_COST",
            "RECAL_OVER_LENGTH",
            "RECAL_OVER_COST",

                "TOTAL_PAID",
                "RULE_ID",
                "LAST_UPDATE_DATE_TEXT",
                "LAST_UPDATE_BY",
               "OM_ORDER_STATUS",
                "REMARK",
                "USER_ID",
                "ORDER_NO",
                "SUBCONTRACT_LOCATION",
                "PRODUCT_OWNER"
                );

            GetFapoOrderExcelCaption(table);

            return table;
        }

        private void GetFapoOrderExcelCaption(DataTable table)
        {
            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_FAPO"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();
            table.Columns["ORDER_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_STATUS").LovValue1 ?? "";
            table.Columns["ACCESS_NUMBER_MASKING"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INTERNET_NO").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUB_NAME").LovValue1 ?? "";
            table.Columns["DISTANCE_LASTMILE_APP"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_APP").LovValue1 ?? "";
            table.Columns["DISTANCE_ESRI"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_ESRI").LovValue1 ?? "";
            table.Columns["DISTANCE_GMAP"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_MAP").LovValue1 ?? "";
            table.Columns["DISTANCE_STRAIT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_REAL").LovValue1 ?? "";
            table.Columns["DISPUTE_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISPUTE_DISTANCE").LovValue1 ?? "";

            table.Columns["DISTANCE_VALIDATE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_VALIDATE").LovValue1 ?? "";
            table.Columns["DISTANCE_TOTAL"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_TOTAL").LovValue1 ?? "";
            //    table.Columns["DISTANCE_PAID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_PAID").LovValue1 ?? "";
            //   table.Columns["Distance_MORE_325"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DISTANCE_MORE_325").LovValue1 ?? "";
            //   table.Columns["LASTMILE_PRICE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_LASTMILE_PRICE").LovValue1 ?? "";

            table.Columns["SUBCONTRACT_TYPE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUB_CONTRACT_TYPE").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_SUB_TYPE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUB_CONTRACT_SUB_TYPE").LovValue1 ?? "";

            table.Columns["REQUEST_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Request_Distance").LovValue1 ?? "";
            table.Columns["APPROVE_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Approve_Distance").LovValue1 ?? "";
            table.Columns["APPROVE_STAFF"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Approve_STAFF").LovValue1 ?? "";
            table.Columns["APPROVE_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Approve_STATUS").LovValue1 ?? "";
            table.Columns["REUSED_FLAG"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Reuse_Flag").LovValue1 ?? "";
            table.Columns["REQUEST_SUB_FLAG"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_REQUEST_SUB_FLAG").LovValue1 ?? "";

            table.Columns["ENTRY_FEE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ENTRY_FEE").LovValue1 ?? "";
            table.Columns["INDOOR_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INDOOR_COST").LovValue1 ?? "";

            table.Columns["OUTDOOR_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_OUTDOOR_COST").LovValue1 ?? "";

            table.Columns["MAPPING_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Maping_Cost").LovValue1 ?? "";
            table.Columns["OVER_LENGTH"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Over_Length").LovValue1 ?? "";
            table.Columns["OVER_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Over_Cost").LovValue1 ?? "";
            table.Columns["TOTAL_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_TOTAL_COST").LovValue1 ?? "";


            table.Columns["RECAL_DIS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_DIS").LovValue1 ?? "";
            table.Columns["RECAL_RATE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_RATE").LovValue1 ?? "";
            table.Columns["RECAL_OVER_LENGTH"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_OVER_LENGTH").LovValue1 ?? "";
            table.Columns["RECAL_OVER_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_OVER_COST").LovValue1 ?? "";
            table.Columns["RECAL_MAPPING_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_MAPPING_COST").LovValue1 ?? "";

            // table.Columns["DIFF_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_DIFF_DISTANCE").LovValue1 ?? "";



            table.Columns["TOTAL_PAID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_TOTAL_PAID").LovValue1 ?? "";
            table.Columns["RULE_ID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Rule_ID").LovValue1 ?? "";

            table.Columns["LAST_UPDATE_DATE_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_LAST_UPDATE_DATE").LovValue1 ?? "";
            table.Columns["LAST_UPDATE_BY"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_LAST_UPDATE_BY").LovValue1 ?? "";
            //   table.Columns["APPROVE_FLAG"].Caption = "APPROVE FLAG";
            table.Columns["OM_ORDER_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_OM_ORDER_STATUS").LovValue1 ?? "";
            table.Columns["REMARK"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_REMARK").LovValue1 ?? "";
            table.Columns["USER_ID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_USER_ID").LovValue1 ?? "";
            table.Columns["ORDER_NO"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORDER_NO").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_LOCATION"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUBCONTRACT_LOCATION").LovValue1 ?? "";
            table.Columns["PRODUCT_OWNER"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PRODUCT_OWNER").LovValue1 ?? "";
        }

        private DataTable AcctOrderExcelColumn(DataTable table)
        {
            // table.Columns.Remove("RowNumber");
            // table.Columns.Remove("RowNumber");
            table.Columns.Remove("USER_ID");
            table.Columns.Remove("REUSED_FLAG");
            table.Columns.Remove("REQUEST_SUB_FLAG");
            table.Columns.Remove("INVOICE_DATE");
            table.Columns.Remove("IR_DOC");
            table.Columns.Remove("NOTE");
            table.Columns.Remove("DISTANCE_LASTMILE_APP");
            table.Columns.Remove("DISTANCE_ESRI");
            table.Columns.Remove("DISTANCE_GMAP");
            table.Columns.Remove("DISTANCE_STRAIT");
            table.Columns.Remove("DISTANCE_VALIDATE");
            table.Columns.Remove("DISTANCE_TOTAL");
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

               "SUBCONTRACT_LOCATION",
               "PRODUCT_OWNER"


                );

            GetAcctOrderExcelCaption(table);

            return table;
        }

        private void GetAcctOrderExcelCaption(DataTable table)
        {
            //var configscreen = LovData.Where(p => p.Type == "L_ORD_LIST_ACCT").ToList();
            var query = new GetLovV2Query()
            {
                LovType = "L_ORD_LIST_ACCT"
            };
            var configscreen = _queryProcessor.Execute(query).ToList();
            table.Columns["RowNumber"].Caption = "No";

            table.Columns["ORDER_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORDER_STATUS").LovValue1 ?? "";
            table.Columns["PAY_PERIOD"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PERIOD").LovValue1 ?? "";
            table.Columns["ORDER_STATUS_DT_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORDER_STATUS_DATE").LovValue1 ?? "";
            table.Columns["ACCESS_NUMBER_MASKING"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INTERNET_NO").LovValue1 ?? "";
            table.Columns["ACCOUNT_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ACCOUNT_NAME").LovValue1 ?? "";
            table.Columns["PROMOTION_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PROMOTION").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUB_NAME").LovValue1 ?? "";
            table.Columns["LENGTH_DISTANCE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_LENGTH_DISTANCE").LovValue1 ?? "";
            table.Columns["INDOOR_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INDOOR").LovValue1 ?? "";
            table.Columns["OUTDOOR_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_OUTDOOR").LovValue1 ?? "";
            table.Columns["MAPPING_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Maping_Cost").LovValue1 ?? "";
            table.Columns["OVER_LENGTH"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Over_Length").LovValue1 ?? "";
            table.Columns["OVER_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Over_Cost").LovValue1 ?? "";
            table.Columns["TOTAL_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_TOTAL_COST").LovValue1 ?? "";
            table.Columns["ENTRY_FEE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ENTRY_FEE").LovValue1 ?? "";
            table.Columns["ORDER_NO"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORDER_NO").LovValue1 ?? "";
            table.Columns["PRODUCT_NAME"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PRODUCT_NAME").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_CODE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_VENDOR_CODE").LovValue1 ?? "";
            table.Columns["INVOICE_DATE_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INVOICE_DATE").LovValue1 ?? "";
            table.Columns["INVOICE_NO"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INVOICE_NO").LovValue1 ?? "";
            table.Columns["REMARK"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_REMARK").LovValue1 ?? "";
            table.Columns["PAID_DATE_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PAID_DATE").LovValue1 ?? "";
            table.Columns["OM_ORDER_STATUS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_OM_ORDER_STATUS").LovValue1 ?? "";
            table.Columns["ORDER_SFF"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ORDER_SFF").LovValue1 ?? "";
            table.Columns["FOA_SUBMIT_DATE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_FOA_SUBMIT_DATE").LovValue1 ?? "";
            table.Columns["CS_APPROVE_DATE_TEXT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_CS_APPROVE_DATE_TEXT").LovValue1 ?? "";
            table.Columns["EVENT_CODE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_EVENT_CODE").LovValue1 ?? "";
            table.Columns["INSTALLATION_ADDRESS"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INSTALLATION_ADDRESS").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_TYPE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUBCONTRACT_TYPE").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_SUB_TYPE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUBCONTRACT_SUB_TYPE").LovValue1 ?? "";
            table.Columns["RECAL_OVER_LENGTH"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_OVER_LENGTH").LovValue1 ?? "";
            table.Columns["RECAL_OVER_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_OVER_COST").LovValue1 ?? "";
            table.Columns["RECAL_MAPPING_COST"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_MAPPING_COST").LovValue1 ?? "";
            table.Columns["RECAL_RATE"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_RECAL_RATE").LovValue1 ?? "";

            table.Columns["TOTAL_PAID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_TOTAL_PAID").LovValue1 ?? "";
            // table.Columns["INVOICE_AMOUNT_BFVAT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INV_AMT_BFVAT").LovValue1 ?? "";
            table.Columns["INVOICE_AMOUNT_VAT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INV_AMT_VAT").LovValue1 ?? "";
            table.Columns["TOTAL_INVOICE_AMOUNT_VAT"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_TOTAL_INV_AMT_VAT").LovValue1 ?? "";
            table.Columns["RULE_ID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_Rule_ID").LovValue1 ?? "";
            table.Columns["INV_GRP"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_INV_GRP").LovValue1 ?? "";
            table.Columns["ADDR_ID"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_ADDR_ID").LovValue1 ?? "";
            table.Columns["PHASE_PO"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PHASE_PO").LovValue1 ?? "";
            table.Columns["SUBCONTRACT_LOCATION"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_SUBCONTRACT_LOCATION").LovValue1 ?? "";
            table.Columns["PRODUCT_OWNER"].Caption = configscreen.FirstOrDefault(f => f.Name == "L_PRODUCT_OWNER").LovValue1 ?? "";
            // table.Columns["APPROVE_FLAG"].Caption = "APPROVE FLAG";

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

        #endregion

        public ActionResult FLSUPDATEOrderList([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                var UpdateModels = new JavaScriptSerializer().Deserialize<FLSUpdateModel>(dataS);
                string[] str; string[] prdname; string INV_DATE = string.Empty;
                string checkDispute = UpdateModels.UPDATESTATUS.ToSafeString();
                //  LastMileByDistanceModel searchModel =  (LastMileByDistanceModel)TempData.Peek("TempSearchCriteria");
                LastMileByDistanceModel searchModel = new LastMileByDistanceModel();
                prdname = new string[1] { "ALL" };
                INV_DATE = UpdateModels.INVOICE_DATE.ToSafeString();

                searchModel.ORDER_NO = "";
                searchModel.ACCESS_NO = "";
                searchModel.PRODUCT_NAME = prdname;
                searchModel.SUBCONT_CODE = "ALL";
                searchModel.ORG_ID = "ALL";
                //searchResult.SUBCONT_NAME ="";
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
                searchModel.ORDER_STATUS = str;
                searchModel.ORD_STATUS = "Approve";
                searchModel.PAGE_INDEX = UpdateModels.PAGE_INDEX;
                searchModel.PAGE_SIZE = UpdateModels.PAGE_SIZE;


                var result = GetLastMileOrderListByPage(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                if (checkDispute.ToUpper() != "DISPUTE")
                {
                    result.cur = result.cur.Where(x => !string.IsNullOrEmpty(x.ACCESS_NUMBER)).ToList();
                }
                if (INV_DATE != "")
                {
                    result.cur = result.cur.Where(x => x.INVOICE_DATE_TEXT.Contains(INV_DATE)).ToList();
                }

                decimal _total = result.cur[0].CNT;
                if (result != null)
                {

                    return Json(new
                    {
                        Data = result.cur,
                        Total = _total,
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
                //var UpdateModels = new JavaScriptSerializer().Deserialize<FLSUpdateModel>(dataS);
                //string[] str; string[] prdname; string INV_DATE = string.Empty;
                //string checkDispute = UpdateModels.UPDATESTATUS.ToSafeString();
                LastMileByDistanceModel searchModel = (LastMileByDistanceModel)TempData.Peek("TempSearchCriteria");
                //LastMileByDistanceModel searchModel = new LastMileByDistanceModel();
                //prdname = new string[1] { "ALL" };
                //INV_DATE = UpdateModels.INVOICE_DATE.ToSafeString();

                //searchModel.ORDER_NO = "";
                //searchModel.ACCESS_NO = "";
                //searchModel.PRODUCT_NAME = prdname;
                //searchModel.SUBCONT_CODE = "ALL";
                //searchModel.ORG_ID = "ALL";
                ////searchResult.SUBCONT_NAME ="";
                //searchModel.IR_DOC = "ALL";



                //searchModel.ORD_TYPE = "ALL";
                //searchModel.REGION = "ALL";

                //searchModel.SUBCONT_TYPE = "ALL";
                //searchModel.SUBCONTSUB_TYPE = "ALL";



                //searchModel.PERIOD_FM = "";
                //searchModel.PERIOD_TO = "";
                //searchModel.APPROVE_FM = "";
                //searchModel.APPROVE_TO = "";
                //searchModel.TRANS_FM = "";
                //searchModel.TRANS_TO = "";
                //searchModel.UPDATE_BY = "";
                //str = new string[1] { UpdateModels.WF_STATUS };
                //searchModel.FOA_FM = UpdateModels.DateFrom;
                //searchModel.FOA_TO = UpdateModels.DateTo;
                //searchModel.INVOICE_DT = "";
                //searchModel.INVOICE_NO = UpdateModels.INVOICE_NO;
                //searchModel.ORDER_STATUS = str;
                //searchModel.ORD_STATUS = "Approve";
                //searchModel.PAGE_INDEX = UpdateModels.PAGE_INDEX;
                //searchModel.PAGE_SIZE = UpdateModels.PAGE_SIZE;


                var result = GetLastMileOrderListByPage(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                //if (checkDispute != "Dispute")

                //{
                //    result.cur.Where(x => x.ACCESS_NUMBER != "").ToList();
                //}
                //if (INV_DATE != "")
                //{
                //    result.cur = result.cur.Where(x => x.INVOICE_DATE_TEXT.Contains(INV_DATE)).ToList();
                //}


                decimal _total = result.cur[0].CNT;
                if (result != null)
                {

                    return Json(new
                    {
                        Data = result.cur,
                        Total = _total,
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
                List<LastMileByDistanceOrderListModel> SearchResults = (List<LastMileByDistanceOrderListModel>)DataModel;
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
            var query = new GetLastMileByDistanceOrderHistoryDetailQuery()
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
        public ActionResult SearchLastMileByDistanceOrderList([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("login", "Account");
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
                var rollback = getLov("FIXED_ASSET_LASTMILE", "ROLLBACK");
                if (rollback.Count != 0)
                {
                    var searchModel = new JavaScriptSerializer().Deserialize<LastMileByDistanceModel>(dataS);
                    TempData["TempSearchCriteria"] = searchModel;
                    string ColummName = string.Empty; string sortType = string.Empty;
                    foreach (var SortD in request.Sorts)
                    {
                        ColummName = SortD.Member.ToSafeString();
                        sortType = SortD.SortDirection.ToSafeString();
                    }


                    var SortData = (object)null;

                    var result = GetLastMileOrderListByPage(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);

                    string userGroup = GetUserGroup();
                    if (userGroup == "SCM")
                    {
                        if (ColummName != "")
                        {
                            if (sortType == "Ascending")
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "SBC_CPY") SortData = result.cur.OrderBy(o => o.SBC_CPY).ToList();
                                else if (ColummName == "ORDER_STATUS_DT_TEXT") { SortData = result.cur.OrderBy(o => o.ORDER_STATUS_DT).ToList(); }
                                else if (ColummName == "CS_APPROVE_DATE_TEXT") { SortData = result.cur.OrderBy(o => o.CS_APPROVE_DATE).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                            else
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderByDescending(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "SBC_CPY") SortData = result.cur.OrderByDescending(o => o.SBC_CPY).ToList();
                                else if (ColummName == "ORDER_STATUS_DT_TEXT") { SortData = result.cur.OrderByDescending(o => o.ORDER_STATUS_DT).ToList(); }
                                else if (ColummName == "CS_APPROVE_DATE_TEXT") { SortData = result.cur.OrderByDescending(o => o.CS_APPROVE_DATE).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                        }
                        else
                        {
                            SortData = result.cur;
                        }
                    }
                    else if (userGroup == "FAPO")
                    {
                        if (ColummName != "")
                        {

                            if (sortType == "Ascending")
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "RULE_ID") SortData = result.cur.OrderBy(o => o.RULE_ID).ToList();
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderBy(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                            else
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderByDescending(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "RULE_ID") SortData = result.cur.OrderByDescending(o => o.RULE_ID).ToList();
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderByDescending(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                        }
                        else
                        {
                            SortData = result.cur;
                        }
                    }
                    else
                    {

                        if (ColummName != "")
                        {

                            if (sortType == "Ascending")
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "PAID_DATE_TEXT") SortData = result.cur.OrderBy(o => o.PAID_DATE).ToList();
                                else if (ColummName == "INVOICE_NO") { SortData = result.cur.OrderBy(o => o.INVOICE_NO).ToList(); }
                                else if (ColummName == "INVOICE_DATE_TEXT") { SortData = result.cur.OrderBy(o => o.INVOICE_DATE).ToList(); }
                                else if (ColummName == "ORDER_NO") { SortData = result.cur.OrderBy(o => o.ORDER_NO).ToList(); }
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderBy(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                            else
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderByDescending(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "PAID_DATE_TEXT") SortData = result.cur.OrderByDescending(o => o.PAID_DATE).ToList();
                                else if (ColummName == "INVOICE_NO") { SortData = result.cur.OrderByDescending(o => o.INVOICE_NO).ToList(); }
                                else if (ColummName == "INVOICE_DATE_TEXT") { SortData = result.cur.OrderBy(o => o.INVOICE_DATE).ToList(); }
                                else if (ColummName == "ORDER_NO") { SortData = result.cur.OrderBy(o => o.ORDER_NO).ToList(); }
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderBy(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                        }
                        else
                        {
                            SortData = result.cur;
                        }

                    }
                    //getSumValue();
                    TempData["TempSearch"] = result.cur;

                    if (result.cur.Count > 0)
                    {

                        return Json(new
                        {
                            Data = SortData,
                            Total = result.cur[0].CNT
                        });
                    }

                    return null;
                }
                else
                {
                    var searchModel = new JavaScriptSerializer().Deserialize<LastMileByDistanceModel>(dataS);
                    var result = new LastMileByDistanceOrderListReturn();

                    if (Session["TempSearchCriteria"] == null)
                    {
                        Session["TempSearchCriteria"] = searchModel;
                        TempData["TempSearchCriteria"] = searchModel;
                        result = GetLastMileOrderListByPageNew(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
                    }
                    else
                    {
                        var session_criteria = (LastMileByDistanceModel)Session["TempSearchCriteria"];

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
                            //searchModel.PAGE_INDEX == session_criteria.PAGE_INDEX &&
                            //searchModel.PAGE_SIZE == session_criteria.PAGE_SIZE &&
                            searchModel.EXISTING_RULE == session_criteria.EXISTING_RULE &&
                            searchModel.NEW_RULE == session_criteria.NEW_RULE &&
                            searchModel.PRODUCT_OWNER == session_criteria.PRODUCT_OWNER)

                        {
                            result = (LastMileByDistanceOrderListReturn)Session["TempDataList"];
                        }
                        else
                        {
                            Session["TempSearchCriteria"] = searchModel;
                            result = GetLastMileOrderListByPageNew(searchModel, searchModel.PAGE_INDEX, searchModel.PAGE_SIZE);
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
                    if (userGroup == "SCM")
                    {
                        if (ColummName != "")
                        {
                            if (sortType == "Ascending")
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "SBC_CPY") SortData = result.cur.OrderBy(o => o.SBC_CPY).ToList();
                                else if (ColummName == "ORDER_STATUS_DT_TEXT") { SortData = result.cur.OrderBy(o => o.ORDER_STATUS_DT).ToList(); }
                                else if (ColummName == "CS_APPROVE_DATE_TEXT") { SortData = result.cur.OrderBy(o => o.CS_APPROVE_DATE).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                            else
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderByDescending(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "SBC_CPY") SortData = result.cur.OrderByDescending(o => o.SBC_CPY).ToList();
                                else if (ColummName == "ORDER_STATUS_DT_TEXT") { SortData = result.cur.OrderByDescending(o => o.ORDER_STATUS_DT).ToList(); }
                                else if (ColummName == "CS_APPROVE_DATE_TEXT") { SortData = result.cur.OrderByDescending(o => o.CS_APPROVE_DATE).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                        }
                        else
                        {
                            SortData = result.cur;
                        }
                    }
                    else if (userGroup == "FAPO")
                    {
                        if (ColummName != "")
                        {

                            if (sortType == "Ascending")
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "RULE_ID") SortData = result.cur.OrderBy(o => o.RULE_ID).ToList();
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderBy(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                            else
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderByDescending(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "RULE_ID") SortData = result.cur.OrderByDescending(o => o.RULE_ID).ToList();
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderByDescending(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                        }
                        else
                        {
                            SortData = result.cur;
                        }
                    }
                    else
                    {

                        if (ColummName != "")
                        {

                            if (sortType == "Ascending")
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "PAID_DATE_TEXT") SortData = result.cur.OrderBy(o => o.PAID_DATE).ToList();
                                else if (ColummName == "INVOICE_NO") { SortData = result.cur.OrderBy(o => o.INVOICE_NO).ToList(); }
                                else if (ColummName == "INVOICE_DATE_TEXT") { SortData = result.cur.OrderBy(o => o.INVOICE_DATE).ToList(); }
                                else if (ColummName == "ORDER_NO") { SortData = result.cur.OrderBy(o => o.ORDER_NO).ToList(); }
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderBy(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                            else
                            {
                                if (ColummName == "ACCESS_NUMBER") { SortData = result.cur.OrderByDescending(o => o.ACCESS_NUMBER).ToList(); }
                                else if (ColummName == "PAID_DATE_TEXT") SortData = result.cur.OrderByDescending(o => o.PAID_DATE).ToList();
                                else if (ColummName == "INVOICE_NO") { SortData = result.cur.OrderByDescending(o => o.INVOICE_NO).ToList(); }
                                else if (ColummName == "INVOICE_DATE_TEXT") { SortData = result.cur.OrderBy(o => o.INVOICE_DATE).ToList(); }
                                else if (ColummName == "ORDER_NO") { SortData = result.cur.OrderBy(o => o.ORDER_NO).ToList(); }
                                else if (ColummName == "SUBCONTRACT_NAME") { SortData = result.cur.OrderBy(o => o.SUBCONTRACT_NAME).ToList(); }
                                else { SortData = result.cur.OrderBy(o => o.ACCESS_NUMBER).ToList(); }
                            }
                        }
                        else
                        {
                            SortData = result.cur;
                        }

                    }

                    var SortDataNew = SortData as List<LastMileByDistanceOrderListModel>;
                    TempData["TempSearch"] = result.cur;

                    if (result.cur.Count > 0)
                    {
                        if (rollback.Count != 0)
                        {
                            return Json(new
                            {
                                Data = SortData,
                                Total = result.cur[0].CNT
                            });
                        }
                        else
                        {
                            SortData = SortDataNew.Skip(skip * pageSize).Take(pageSize).ToList();
                            return Json(new
                            {
                                Data = SortData,
                                Total = result.cur.Count
                            });
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }
        public async Task<LastMileByDistanceOrderListReturn> getAllRecord()
        {
            string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            _Logger.Info("StartGetAllRecord" + DateNow.ToSafeString());
            LastMileByDistanceModel searchModel = (LastMileByDistanceModel)TempData.Peek("TempSearchCriteria");

            string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
            string orderStatus = searchModel.ORDER_STATUS == null ? "" : string.Join(",", searchModel.ORDER_STATUS);

            var query = new SearchLastMileByDistanceOrderListNewQuery
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
                p_PRODUCT_OWNER = searchModel.PRODUCT_OWNER == "" ? null : searchModel.PRODUCT_OWNER.Replace("/", "")
                //p_UPDATE_BY = searchModel.UPDATE_BY,
                //P_PAGE_INDEX = 1,
                //P_PAGE_SIZE = decimal.MaxValue
            };
            var result = _queryProcessor.Execute(query);
            var callvat = SelectFbbCfgLov("VAT_RATE", "FIXED_LASTMILE").FirstOrDefault();
            foreach (var item in result.cur)
            {

                item.INVOICE_AMOUNT_BFVAT = "";
                item.INVOICE_AMOUNT_VAT = getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL);
                item.TOTAL_INVOICE_AMOUNT_VAT = string.Format("{0:#,0.00}", (item.TOTAL_PAID + getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL)));
                item.CS_APPROVE_DATE_TEXT = item.CS_APPROVE_DATE.ToDateDisplayText();
                item.INVOICE_DATE_TEXT = item.INVOICE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE.ToDateDisplayText();
                item.ORDER_STATUS_DT_TEXT = item.ORDER_STATUS_DT.ToDateDisplayText();
                item.APPOINTMENNT_DT_TEXT = item.APPOINTMENNT_DT.ToDateDisplayText();
                item.EFFECTIVE_END_DT_TEXT = item.EFFECTIVE_END_DT.ToDateDisplayText();
                item.SFF_ACTIVE_DATE_TEXT = item.SFF_ACTIVE_DATE.ToDateDisplayText();
                item.SFF_SUBMITTED_DATE_TEXT = item.SFF_SUBMITTED_DATE.ToDateDisplayText();
                item.FOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE.ToDateDisplayText();
                item.PAID_DATE_TEXT = item.PAID_DATE.ToDateDisplayText();
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

        private LastMileByDistanceOrderListReturn GetLastMileOrderListByPage(LastMileByDistanceModel searchModel, decimal PAGE_INDEX, decimal PAGE_SIZE)
        {
            string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
            string orderStatus = searchModel.ORDER_STATUS == null ? "" : string.Join(",", searchModel.ORDER_STATUS);

            var query = new SearchLastMileByDistanceOrderListQuery
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
                p_UPDATE_BY = searchModel.UPDATE_BY,
                P_PAGE_INDEX = PAGE_INDEX,
                P_PAGE_SIZE = PAGE_SIZE
            };
            var result = _queryProcessor.Execute(query);
            var callvat = SelectFbbCfgLov("VAT_RATE", "FIXED_LASTMILE").FirstOrDefault();

            foreach (var item in result.cur)
            {

                item.INVOICE_AMOUNT_BFVAT = "";
                item.INVOICE_AMOUNT_VAT = getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL);
                item.TOTAL_INVOICE_AMOUNT_VAT = string.Format("{0:#,0.00}", (item.TOTAL_PAID + getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL)));
                item.CS_APPROVE_DATE_TEXT = item.CS_APPROVE_DATE.ToDateDisplayText();
                item.INVOICE_DATE_TEXT = item.INVOICE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE.ToDateDisplayText();
                item.ORDER_STATUS_DT_TEXT = item.ORDER_STATUS_DT.ToDateDisplayText();
                item.APPOINTMENNT_DT_TEXT = item.APPOINTMENNT_DT.ToDateDisplayText();
                item.EFFECTIVE_END_DT_TEXT = item.EFFECTIVE_END_DT.ToDateDisplayText();
                item.SFF_ACTIVE_DATE_TEXT = item.SFF_ACTIVE_DATE.ToDateDisplayText();
                item.SFF_SUBMITTED_DATE_TEXT = item.SFF_SUBMITTED_DATE.ToDateDisplayText();
                item.FOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE.ToDateDisplayText();
                item.PAID_DATE_TEXT = item.PAID_DATE.ToDateDisplayText();
            }

            return result;
        }

        private LastMileByDistanceOrderListReturn GetLastMileOrderListByPageNew(LastMileByDistanceModel searchModel, decimal PAGE_INDEX, decimal PAGE_SIZE)
        {
            string productName = searchModel.PRODUCT_NAME == null ? "" : string.Join(",", searchModel.PRODUCT_NAME);
            string orderStatus = searchModel.ORDER_STATUS == null ? "" : string.Join(",", searchModel.ORDER_STATUS);

            var query = new SearchLastMileByDistanceOrderListNewQuery
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
                //p_ORD_STATUS = "ALL",
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
                p_PRODUCT_OWNER = searchModel.PRODUCT_OWNER == "" ? null : searchModel.PRODUCT_OWNER.Replace("/", ""),
                //p_UPDATE_BY = searchModel.UPDATE_BY,
                //P_PAGE_INDEX = PAGE_INDEX,
                //P_PAGE_SIZE = PAGE_SIZE
            };
            var result = _queryProcessor.Execute(query);
            //var callvat = SelectFbbCfgLov("VAT_RATE", "FIXED_LASTMILE").FirstOrDefault();

            //int row_no = 1;
            foreach (var item in result.cur)
            {
                //item.RowNumber = row_no++;
                //item.CNT = result.cur.Count();
                //item.INVOICE_AMOUNT_BFVAT = "";
                //item.INVOICE_AMOUNT_VAT = getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL);
                //item.TOTAL_INVOICE_AMOUNT_VAT = string.Format("{0:#,0.00}", (item.TOTAL_PAID + getVatRATE(item.TOTAL_PAID, callvat.DISPLAY_VAL)));
                item.CS_APPROVE_DATE_TEXT = item.CS_APPROVE_DATE.ToDateDisplayText();
                item.INVOICE_DATE_TEXT = item.INVOICE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE.ToDateDisplayText();
                item.ORDER_STATUS_DT_TEXT = item.ORDER_STATUS_DT.ToDateDisplayText();
                item.APPOINTMENNT_DT_TEXT = item.APPOINTMENNT_DT.ToDateDisplayText();
                item.EFFECTIVE_END_DT_TEXT = item.EFFECTIVE_END_DT.ToDateDisplayText();
                item.SFF_ACTIVE_DATE_TEXT = item.SFF_ACTIVE_DATE.ToDateDisplayText();
                item.SFF_SUBMITTED_DATE_TEXT = item.SFF_SUBMITTED_DATE.ToDateDisplayText();
                item.FOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE.ToDateDisplayText();
                item.PAID_DATE_TEXT = item.PAID_DATE.ToDateDisplayText();
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

        public JsonResult UpdateByOrder([DataSourceRequest] DataSourceRequest request, List<FBB_access_list> AccNOList,
            string AccNO, string USER, string IntFace, string Status, string InvNo, string InvDate, string IrDoc,
            string remark, string remarksub, string ValDis, string Reason, string TranDT, string chkirdoc, string prddate)
        {
            try
            {
                LastMileByDistanceModel searchModel = (LastMileByDistanceModel)TempData.Peek("TempSearchCriteria");
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
                        var result = GetLastMileOrderListByPage(searchModel, 1, Decimal.MaxValue);
                        var results = result.cur.Where(x => x.APPROVE_FLAG.Equals("Approved") && x.ORDER_STATUS.Equals("Confirm Paid")).ToList();

                        var RecList = new List<FBB_access_list>();
                        foreach (var dd in results)
                        {
                            var model = new FBB_access_list
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
                    var RecList = new List<FBB_access_list>();
                    var model = new FBB_access_list
                    {
                        ACCESS_NUMBER = AccNO,

                    };

                    RecList.Add(model);
                    AccNOList = RecList;


                }


                USER = CurrentUser.UserName;

                var command = new UpdateLastMileByDistanceByOrderCommand()
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
                _UpdateLastMileByDistanceByOrderCommand.Handle(command);
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
            LastMileByDistanceModel searchModel = (LastMileByDistanceModel)TempData.Peek("TempSearchCriteria");
            var AllRecord = GetLastMileOrderListByPage(searchModel, 1, Decimal.MaxValue);

            var result = AllRecord.cur;

            var AccNOList = result.Where(a => GetOrderStatusByUserGroup().Contains(a.ORDER_STATUS)).Select(a => new FBB_access_list { ACCESS_NUMBER = a.ACCESS_NUMBER }).ToList();


            try
            {
                USER = CurrentUser.UserName;
                IntFace = GetUserGroup();
                var command = new UpdateLastMileByDistanceByOrderCommand()
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
                _UpdateLastMileByDistanceByOrderCommand.Handle(command);
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
                var query = new GetLastMileByDistanceOrderHistoryDetailQuery()
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
            string _ORDER_STATUS = string.Empty;
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
                    _ORDER_STATUS = "";
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
                    _ORDER_STATUS = result[0].ORDER_STATUS.ToSafeString();
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
                _ORDER_STATUS = "-";
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
                _ORDER_STATUS = _ORDER_STATUS,
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

        public List<OrderDetailModel> getOrderDetail(string AccNO, string OrdNO)
        {
            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
                return null;
            try
            {
                var query = new GetLastMileByDistanceOrderDetailQuery()
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

        public List<PostSapDetail> getPostSapDetail(string AccNO, string OrdNO)
        {
            if (string.IsNullOrEmpty(AccNO) && string.IsNullOrEmpty(OrdNO))
                return null;
            try
            {
                var query = new GetLastMileByDistancePostSapDetailQuery()
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
                var query = new GetDistanceDetailQuery()
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
            LastMileByDistanceOrderListModel model)
        {

            try
            {

                //string result = "";
                var command = new LastMileUpdateNoteCommand()
                {
                    p_ACCESS_NO = model.ACCESS_NUMBER,
                    p_ORDER_NO = model.ORDER_NO,
                    p_USER = CurrentUser.UserName,
                    p_REMARK_FOR_SUB = model.NOTE,
                };

                _LastMileUpdateNoteCommand.Handle(command);

                //if (command.ret_msg == "Update success.")
                //{
                //    result = "Success";
                //    //result[0] = "0";
                //    //result[1] = "";
                //}
                //else
                //{
                //    result = command.ret_msg;
                //    //result[0] = "-1";
                //    //result[1] = command.ReturnMessage;
                //}

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
            var data = SelectFbbCfgLov("PRODUCT_NAME", "FIXED_LASTMILE");
            //data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectRegion(string text)
        {
            var data = SelectFbbCfgLov("REGION", "FIXED_LASTMILE");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectIrDoc(string text)
        {
            var data = SelectFbbCfgLov("IR_DOC", "FIXED_LASTMILE");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectIrDocFLSUPDATE(string text)
        {
            var data = SelectFbbCfgLov("IR_DOC", "FIXED_LASTMILE");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectWorkFlowStatus()
        {
            var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE");
            //data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectSubContractorName(string text)
        {
            var query = new SelectSubContractorNameQuery
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
            var query = new SelectSubContractorNameQuery
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
            var query = new SelectSubContractorNameQuery
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
            var query = new SelectSubContractorNameQuery
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
            var data = SelectFbbCfgLov("SUBCONTRACT_SUB_TYPE", "FIXED_LASTMILE")
               .Where(d => d.LOV_VAL1 != null).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectSubContracttype()
        {
            var data = SelectFbbCfgLov("SUBCONTRACT_TYPE", "FIXED_LASTMILE")
               .Where(d => d.LOV_VAL1 != null).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOrderType()
        {
            var data = SelectFbbCfgLov("ORD_TYPE", "FIXED_LASTMILE")
               .Where(d => d.LOV_VAL1 != null).ToList();

            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectOrderStatus()
        {
            var data = SelectFbbCfgLov("ORD_STATUS", "FIXED_LASTMILE")
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
                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                     .Where(d => d.LOV_VAL2 != null && d.LOV_VAL2.Contains("SCM") && d.LOV_VAL1.Equals("Confirm Paid")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                    .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Paid") || d.LOV_VAL1.Equals("Hold")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult SelectACCTUpdateAllStatus()
        {

            var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                .Where(d => d.LOV_VAL1.Equals("Paid") || d.LOV_VAL1.Equals("Hold")).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectScmWorkFlowStatus(string WFS)
        {

            if (WFS == "Waiting Sub Verify")
            {
                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                    .Where(d => d.LOV_VAL1.Equals("Confirm Paid") || d.LOV_VAL1.Equals("Dispute")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Waiting Paid")
            {

                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                 .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Confirm Paid") || d.LOV_VAL1.Equals("Waiting Paid")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Dispute")
            {

                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                 .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Dispute")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Confirm Paid")
            {

                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                 .Where(d => d.LOV_VAL1 != null && d.LOV_VAL1.Equals("Confirm Paid")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (WFS == "Hold")
            {
                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
              .Where(d => d.LOV_VAL1.Equals("Confirm Paid") || d.LOV_VAL1.Equals("Cancelled")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
                     .Where(d => d.LOV_VAL2 != null && d.LOV_VAL2.Contains("SCM")).ToList();
                data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectReason(string text)
        {
            var data = SelectFbbCfgLov("REASON", "FIXED_LASTMILE");
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => p.DISPLAY_VAL.Contains(text)).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectAccWorkFlowStatus(string order_status = "")
        {
            var data = SelectFbbCfgLov("WF_STATUS", "FIXED_LASTMILE")
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
        public JsonResult LastMileByDistanceUpdateByFile([DataSourceRequest] DataSourceRequest request, string status,
            string fileName)
        {
            if (string.IsNullOrEmpty(LastMileFileModel.csv))
            {
                return Json("Please upload file", JsonRequestBehavior.AllowGet);
            }

            try
            {
                var lines = LastMileFileModel.csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    throw new Exception("Blank .csv file");
                }

                var fileList = new List<FBB_update_file_list>();

                var LastMileRecalList = new List<LastMileRecal>();

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

                                var model = new FBB_update_file_list
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
                                var model = new LastMileRecal
                                {
                                    ACCESS_NUMBER = accNbr.ToSafeString().Replace(" ", ""),
                                    NEW_RULE_ID = "",
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

                                LastMileRecalList.Add(model);
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

                                var model = new FBB_update_file_list
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
                    LastMileRecalList.RemoveAt(0);
                    if (LastMileRecalList.Count > 0)
                    {
                        var command = new LastMileByDistanceRecalByOrderCommand()
                        {
                            p_recal_access_list = LastMileRecalList,
                            //p_NEW_RULE_ID = new_ruid,
                            p_USER = CurrentUser.UserName,
                            //p_REMARK = remark

                        };
                        _LastMileByDistanceRecalByOrderCommand.Handle(command);

                        #region Call API Subpayment
                        ModelApiSubpayment modelSub = new ModelApiSubpayment();
                        modelSub.Order_list = new List<OrderList>();
                        List<OrderList> ListApi = new List<OrderList>();
                        foreach (var item in command.return_subpayment_cur)
                        {
                            var model = new OrderList()
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
                        var command = new LastMileByDistanceUpdateByFileCommand()
                        {
                            p_INTERFACE = GetUserGroup(),
                            p_USER = CurrentUser.UserName,
                            p_STATUS = status,
                            p_filename = fileName,
                            p_file_list = fileList
                        };
                        _LastMileByDistanceUpdateByFileCommand.Handle(command);
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

        private ActionResult UploadLastMileFile(HttpPostedFileBase file)
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
                    fileName = userGroup + "_LASTMILE_UPLOAD_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

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

        public ActionResult ScmLastMilefile_Save(IEnumerable<HttpPostedFileBase> scmLastMilefile)
        {
            if (scmLastMilefile != null)
            {
                try
                {
                    foreach (var file in scmLastMilefile)
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".csv")
                        {
                            // Read bytes from http input stream
                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            //LastMileFileModel.csv = System.Text.Encoding.UTF8.GetString(binData);
                            LastMileFileModel.csv = System.Text.Encoding.Default.GetString(binData);
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

        public ActionResult ScmLastMilefile_Remove(string[] scmLastMilefile)
        {
            if (scmLastMilefile != null)
            {
                try
                {
                    LastMileFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        public ActionResult FapoLastMilefile_Save(IEnumerable<HttpPostedFileBase> fapoLastMilefile)
        {
            if (fapoLastMilefile != null)
            {
                try
                {
                    foreach (var file in fapoLastMilefile)
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".csv")
                        {
                            // Read bytes from http input stream
                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            //LastMileFileModel.csv = System.Text.Encoding.UTF8.GetString(binData);
                            LastMileFileModel.csv = System.Text.Encoding.Default.GetString(binData);
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

        public ActionResult FapoLastMilefile_Remove(string[] fapoLastMilefile)
        {
            if (fapoLastMilefile != null)
            {
                try
                {
                    LastMileFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        public ActionResult AccLastMilefile_Save(IEnumerable<HttpPostedFileBase> accLastMilefile)
        {
            if (accLastMilefile != null)
            {
                try
                {
                    foreach (var file in accLastMilefile)
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".csv")
                        {
                            // Read bytes from http input stream
                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            //LastMileFileModel.csv = System.Text.Encoding.UTF8.GetString(binData);
                            LastMileFileModel.csv = System.Text.Encoding.Default.GetString(binData);
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

        public ActionResult AccLastMilefile_Remove(string[] accLastMilefile)
        {
            if (accLastMilefile != null)
            {
                try
                {
                    LastMileFileModel.csv = "";
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
                return PartialView(Url.Content("~/Views/LastMileByDistance/_PartialReCalDistanceFAPOOrderPopup.cshtml"));
            }
            else
            {
                return PartialView(Url.Content("~/Views/LastMileByDistance/_PartialEditFAPOOrderPopupWithRadioBtn.cshtml"));
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
    }
}
