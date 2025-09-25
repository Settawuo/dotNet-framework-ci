using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WBBContract;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBSS;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.Account;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class CheckInController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;

        public CheckInController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public ActionResult Index()
        {
            if (null != this.CurrentUser)
                return RedirectToAction("Main", "CheckIn");

            ViewBag.User = this.CurrentUser;

            return View();
        }

        private string rptName = "Report Name : {0}";
        private string rptDate = "Run Report Date/Time : {0}";
        private List<string> rptCriterias = new List<string>();

        [HttpPost]
        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(LoginPanelModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userName = model.UserName.Replace("\"", "").Replace("'", "");
                var passWord = model.Password.Replace("'", "").Replace("'", "");
                List<LovValueModel> groupId;

                var grouIdSaleQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "USER_FBB_PORTAL_GROUP"
                };
                groupId = _queryProcessor.Execute(grouIdSaleQuery);
                var authenticatedUser = GetUser(userName, groupId[0].LovValue1);

                Session["isPortal"] = "true";

                var msgUserQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_FAILD"
                };
                var msgUser = _queryProcessor.Execute(msgUserQuery);

                var msgPassQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_PASS_FAILD"
                };
                var msgPass = _queryProcessor.Execute(msgPassQuery);

                var cfgLDAPQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "URLAuthenLDAP"
                };
                var cfgLDAP = _queryProcessor.Execute(cfgLDAPQuery);
                bool useLDAP = false;
                if (cfgLDAP != null && cfgLDAP.Count > 0)
                {
                    useLDAP = true;
                }

                if (useLDAP)
                {

                    if (null != authenticatedUser.UserName
                        && (authenticatedUser.Groups != null)
                        && (authenticatedUser.Groups[0] == Convert.ToDecimal(groupId[0].LovValue1)))
                    {
                        var authenResultMessage = "";
                        if (AuthenLDAP(userName, passWord, out authenResultMessage))
                        {
                            //var authenticatedUser = GetUser(userName);
                            authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                            Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                            this.CurrentUser = authenticatedUser;

                            return RedirectToAction("Main", "CheckIn");
                        }
                        else
                        {
                            ModelState.AddModelError("", msgPass[0].LovValue1);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                    }
                }
                else
                {
                    // bypass authen
                    //Session["userName"] = userName;
                    if (null != authenticatedUser.UserName && (authenticatedUser.Groups != null))
                    {
                        authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                        Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                        this.CurrentUser = authenticatedUser;
                        return RedirectToAction("Main", "CheckIn");
                    }
                    else
                    {
                        ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                    }
                }
            }
            return View("Index");
        }

        public ActionResult Main()
        {

            if (null == this.CurrentUser)
                return RedirectToAction("Index", "CheckIn");

            //Maintenance, New

            string LovData = base.LovData.Where(l => l.Name == "ORDER_TYPE" && l.LovValue5 == "CHECKIN").OrderBy(l => l.OrderBy).Select(t => t.LovValue1).FirstOrDefault().ToSafeString();

            var checkInQuery = new CheckInOrderQuery
            {
                queryString = LovData
            };
            var CheckInOrderData = _queryProcessor.Execute(checkInQuery);
            foreach (System.Data.DataColumn column in CheckInOrderData.Data.Columns)
            {
                column.ColumnName = column.ColumnName.Replace(' ', '_');
                column.ColumnName = column.ColumnName.Replace(".", "");

            }

            return View("Main", CheckInOrderData);
        }

        public ActionResult CheckInOrderData([DataSourceRequest] DataSourceRequest request, string dataQuery = "")
        {
            //Logger.Info("ReadSearchSalePortalLeaveMessage start");
            try
            {
                if (dataQuery == "")
                {
                    dataQuery = base.LovData.Where(l => l.Name == "ORDER_TYPE" && l.LovValue5 == "CHECKIN").OrderBy(l => l.OrderBy).Select(t => t.LovValue1).FirstOrDefault().ToSafeString();
                }

                //Logger.Info("ReadSearchSalePortalLeaveMessage try");
                //Maintenance, New
                var checkInQuery = new CheckInOrderQuery
                {
                    queryString = dataQuery
                };
                var CheckInOrderData = _queryProcessor.Execute(checkInQuery);

                foreach (System.Data.DataColumn column in CheckInOrderData.Data.Columns)
                {
                    column.ColumnName = column.ColumnName.Replace(' ', '_');
                    column.ColumnName = column.ColumnName.Replace(".", "");
                }

                return Json(CheckInOrderData.Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Logger.Info("Error when call ReadSearchSalePortalLeaveMessage");
                //Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        public UserModel GetUser(string userName, string groupId)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = userName,
                GroupId = groupId
            };

            var authenticatedUser = _queryProcessor.Execute(userQuery);
            return authenticatedUser;
        }

        public bool AuthenLDAP(string userName, string password, out string authenMessage)
        {
            var authLDAPQuery = new GetAuthenLDAPQuery
            {
                UserName = userName,
                Password = password,
                ProjectCode = Configurations.ProjectCodeLdapFBB,
            };

            var authenLDAPResult = _queryProcessor.Execute(authLDAPQuery);
            authenMessage = "";
            return authenLDAPResult;
        }

        private HttpCookie CreateAuthenticatedCookie(string userName)
        {
            var authTicket = new FormsAuthenticationTicket(2, userName, DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "");

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(authTicket))
            { HttpOnly = true };

            return authCookie;
        }

        public UserModel CurrentUser
        {
            get { return (UserModel)Session[WebConstants.FBBConfigSessionKeys.User]; }
            set { Session[WebConstants.FBBConfigSessionKeys.User] = value; }
        }

        public ActionResult ExportCheckInEngineerData()
        {

            var checkInQuery = new CheckInOrderQuery
            {
                queryString = "All"
            };
            var CheckInOrderData = _queryProcessor.Execute(checkInQuery);

            var table = CheckInOrderData.Data;

            rptName = string.Format(rptName, "Check In Engineer Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetCheckInEngineerExcelName("CheckInEngineerReport");
            string tempPath = System.IO.Path.GetTempPath();
            var bytes = GenerateCheckInEngineerRptExcel(table, "WorkSheet", tempPath, filename);

            return File(bytes, "application/excel", filename + ".xls");

        }

        public ActionResult GetActivity()
        {
            var LovData = base.LovData.Where(l => l.Name == "ORDER_TYPE" && l.LovValue5 == "CHECKIN").OrderBy(l => l.OrderBy).ToList();
            var data = new List<DropdownModel>();
            foreach (var item in LovData)
            {
                data.Add(new DropdownModel { Value = item.LovValue1, Text = item.LovValue1 });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private string GetCheckInEngineerExcelName(string fileName)
        {
            string result = string.Empty;

            result = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd_MM_yyyy"));

            return result;
        }

        private byte[] GenerateCheckInEngineerRptExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName)
        {
            //Logger.Info("GenerateSalePortalLeaveMessageRptExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            string strColumn2 = string.Empty;
            int iCol = 8;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells["A3,I3"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 7;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                range = worksheet.SelectedRange[1, 8, dataToExcel.Rows.Count + iHeaderRow, 8];
                range.Style.Numberformat.Format = "dd/MM/yyyy";

                //switch (LovValue5)
                //{

                //    case "SALE_PORTAL_RPT_1":
                //        range = worksheet.SelectedRange[1, 8, dataToExcel.Rows.Count + iHeaderRow, 8];
                //        range.Style.Numberformat.Format = "dd/MM/yyyy";
                //        break;
                //    default:
                //        iCol = 14;
                //        break;
                //}

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"Check In Engineer";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }

    }
}
