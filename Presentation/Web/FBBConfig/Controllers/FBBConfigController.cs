using FBBConfig.Extensions;
using FBBConfig.Models;
using FBBConfig.Solid.CompositionRoot;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.Account;


namespace FBBConfig.Controllers
{
    public class FBBConfigController : Controller
    {
        public ILogger _Logger { get; set; }
        //
        // GET: /FBBConfig/        

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
            JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new ServiceStackJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        public UserModel CurrentUser
        {
            get { return (UserModel)Session[WebConstants.FBBConfigSessionKeys.User]; }
            set { Session[WebConstants.FBBConfigSessionKeys.User] = value; }
        }

        public List<LovValueModel> LovData
        {
            get
            {
                var masterController = Bootstrapper.GetInstance<MasterDataController>();
                if (null == Session[WebConstants.FBBConfigSessionKeys.AllLov])
                {
                    Session[WebConstants.FBBConfigSessionKeys.AllLov] = masterController.GetLovList("", "");
                }

                return (List<LovValueModel>)Session[WebConstants.FBBConfigSessionKeys.AllLov];
            }
        }

        public List<ZipCodeModel> ZipCodeData(int currentCulture)
        {
            if (currentCulture != SiteSession.LatestUICulture || (null == Session[WebConstants.FBBConfigSessionKeys.ZipCodeData]))
            {
                SiteSession.LatestUICulture = currentCulture;
                var masterController = Bootstrapper.GetInstance<MasterDataController>();
                var data = masterController.GetZipCodeList(currentCulture); // 1 = th , 2 = en
                Session[WebConstants.FBBConfigSessionKeys.ZipCodeData] = data;
            }

            return (List<ZipCodeModel>)Session[WebConstants.FBBConfigSessionKeys.ZipCodeData];
        }

        public List<ZipCodeModel> ZipCodeDataAir(int currentCulture, string regioncode)
        {
            //if (currentCulture != SiteSession.LatestUICulture || (null == Session[WebConstants.FBBConfigSessionKeys.ZipCodeData]))
            //{
            //SiteSession.LatestUICulture = currentCulture;
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            var data = masterController.GetZipCodeAirList(currentCulture, regioncode); // 1 = th , 2 = en
            Session[WebConstants.FBBConfigSessionKeys.ZipCodeData] = data;
            //}

            return (List<ZipCodeModel>)Session[WebConstants.FBBConfigSessionKeys.ZipCodeData];
        }


        // action
        //public ActionResult ChangeCurrentCulture(int culture)
        //{
        //    SiteSession.CurrentUICulture = culture;
        //    Session[WebConstants.FBBConfigSessionKeys.CurrentUICulture] = culture;
        //    return Redirect(Request.UrlReferrer.ToString());
        //}

        //public int GetCurrentCulture()
        //{
        //    return Convert.ToInt32(Session[WebConstants.FBBConfigSessionKeys.CurrentUICulture].ToString());
        //}

        public byte[] GenerateEntitytoExcel<T>(List<T> data, string fileName)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                System.ComponentModel.PropertyDescriptor prop = props[i];

                table.Columns.Add(prop.Name, System.Type.GetType("System.String"));
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
            string tempPath = System.IO.Path.GetTempPath();
            var data_ = GenerateExcel(table, "WorkSheet", tempPath, fileName);
            return data_;
        }

        private byte[] GenerateExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName)
        {
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
            finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells["A1"].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }

        public byte[] GenerateEntitytoExcel<T>(List<T> data, string fileName, string[] headerColumn, string lovType, string lov5,
            string rptName, string rptCriteria, string rptDate, int[] headerHideColumn, bool[] getLov2)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = LovData.Where(p => p.Type == lovType && p.LovValue5 == lov5 && headerColumn.Contains(p.Name)).ToList();
            List<LovValueModel> tmp;
            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    tmp = lovDataScreen.Where(p => p.Name == headerColumn[i]).ToList();
                    if (tmp.Any())
                    {
                        if (getLov2 == null)
                        {
                            table.Columns.Add(tmp.FirstOrDefault().LovValue2, System.Type.GetType("System.String"));
                        }
                        else
                        {
                            table.Columns.Add(getLov2[i] ? tmp.FirstOrDefault().LovValue2 : tmp.FirstOrDefault().LovValue1, System.Type.GetType("System.String"));
                        }
                    }
                    else
                    {
                        System.ComponentModel.PropertyDescriptor prop = props[i];
                        table.Columns.Add(prop.Name, System.Type.GetType("System.String"));
                    }
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, System.Type.GetType("System.String"));
                }
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

            if (headerHideColumn != null && headerHideColumn.Length > 0)
            {
                for (int i = 0; i < headerHideColumn.Length; i++)
                {
                    table.Columns.RemoveAt(headerHideColumn[i]);
                }
            }

            string tempPath = System.IO.Path.GetTempPath();
            var data_ = GenerateExcel(table, "WorkSheet", tempPath, fileName, rptName, rptCriteria, rptDate);
            return data_;
        }

        private byte[] GenerateExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName,
            string rptName, string rptCriteria, string rptDate)
        {
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
            finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", directoryPath, fileName);

            //Delete existing file with same file name.
            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

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

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:M3"].Merge = true;
                worksheet.Cells["A3,M3"].LoadFromText(rptCriteria);
                worksheet.Cells["A4:I4"].Merge = true;
                worksheet.Cells["A4,I4"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 7;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToString();
                //iCol = 14;

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }

        public string GetExcelNameWithDateTime(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }
        public int GetCurrentCulture()
        {
            return Convert.ToInt32(Session[WebConstants.FBBConfigSessionKeys.CurrentUICulture].ToString());
        }
        [HttpPost]
        public JsonResult getRedirectURL(string location)
        {
            //// var RequestUrl = Request.Url.OriginalString.Replace(Request.Path,$"/{Controllers}/{Actions}");
            var RequestUrl = location;
            return Json(RequestUrl, JsonRequestBehavior.AllowGet);
        }

    }
}
