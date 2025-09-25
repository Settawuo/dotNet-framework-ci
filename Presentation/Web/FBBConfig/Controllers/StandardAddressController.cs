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
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace FBBConfig.Controllers
{
    public class StandardAddressController : FBBConfigController
    {
        //
        // GET: /StandardAddress/
        private readonly IQueryProcessor _queryProcessor;

        public StandardAddressController(ILogger logger,
              IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        public ActionResult Index()
        {
            return View();
        }

        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        public void SetColumnForExcel(string lovVal5, ref DataTable table)
        {
            string[] colHeader = null;
            string[] typeCol = null;

            int index = 0;
            var colHeaderList = new List<string>();
            List<LovValueModel> tmpVal;
            var colVal = string.Empty;
            var dupVal = string.Empty;

            var lovDataScreen = base.LovData.Where(p => p.Type == "FBBPAYG_SCREEN" && p.LovValue5 == lovVal5 && p.Name.Contains("R_HEADER_"))
                                .OrderBy(p => p.OrderBy).ToList();

            switch (lovVal5)
            {
                case "FULLCONNECTION":

                    colHeader = new string[] { "R_HEADER_region", "R_HEADER_olt_name", "R_HEADER_odf_port_out", "R_HEADER_odf_no", "R_HEADER_odf_port_out", "R_HEADER_site_no",
                                               "R_HEADER_sp1_no", "R_HEADER_sp1_port_out", "R_HEADER_sp1_laltitude", "R_HEADER_sp1_longitude", "R_HEADER_sp2_no",
                                               "R_HEADER_available_port", "R_HEADER_used_port", "R_HEADER_sp2_alias", "R_HEADER_sp2_laltitude", "R_HEADER_sp2_longitude",
                                               "R_HEADER_addr_id", "R_HEADER_addr_name_en", "R_HEADER_addr_name_th"};

                    typeCol = new string[] { "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String" };

                    break;
            }

            if (colHeader != null && colHeader.Length != 0)
            {
                for (index = 0; index < colHeader.Length; index++)
                {
                    tmpVal = lovDataScreen.Where(p => p.Name == colHeader[index]).ToList();
                    if (tmpVal.Any())
                    {
                        colHeaderList.Add(tmpVal.FirstOrDefault().LovValue1.ToSafeString());
                    }
                    else
                    {
                        colHeaderList.Add(colHeader[index].ToSafeString());
                    }
                }
            }

            DataColumn col = new DataColumn();
            for (index = 0; index < colHeaderList.Count; index++)
            {
                colVal = colHeaderList[index].ToSafeString();
                if (typeCol != null && typeCol.Length != 0)
                {
                    if (!table.Columns.Contains(colVal))
                    {
                        table.Columns.Add(colVal, System.Type.GetType(typeCol[index].ToSafeString()));
                    }
                    else
                    {
                        dupVal += " ";
                        table.Columns.Add(colVal + dupVal, System.Type.GetType(typeCol[index].ToSafeString()));
                    }
                }
            }
        }

        public byte[] GenerateSTDEntitytoExcel<T>(List<T> data, string fileName, string lovVal5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var groupHeaderList = new List<string>();
            var colHeaderList = new List<string>();
            //int index = 0;
            string tmp = string.Empty;

            var criterList = new List<LovValueModel>();

            SetColumnForExcel(lovVal5, ref table);

            //Replace Value.
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

            var data_ = GenerateExcelForStandAddress(table, "WorkSheet", tempPath, fileName, lovVal5, props.Count);
            return data_;
        }

        private byte[] GenerateExcelForStandAddress(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string lovVal5, int columnCount)
        {
            string finalFileNameWithPath = PrepareFileExcel(directoryPath, fileName);
            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range1 = null;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                //worksheet 
                if (string.Compare(lovVal5, "FULLCONNECTION", true) == 0)
                {
                    #region FULLCONNECTION
                    //int x = (table1.Rows.Count / 10000);

                    range1 = worksheet.SelectedRange[1, 1, 1, columnCount];
                    range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range1.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                    range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.View.FreezePanes(2, 1);

                    worksheet.Cells["A1"].LoadFromDataTable(dataToExcel, true, TableStyles.None);
                    #endregion
                }
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


        private string PrepareFileExcel(string directoryPath, string fileName)
        {
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            //fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
            finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", directoryPath, fileName);

            //Delete existing file with same file name.
            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            return finalFileNameWithPath;
        }
        #region 
        /*******************************************************************************************************************
         * Begin Report FullConnection                                                                              *
         *******************************************************************************************************************/

        public ActionResult StandardFullConnection()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBPAYG_SCREEN", "FULLCONNECTION");

            return View();
        }

        public ActionResult ReadReport01Search([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt7Model = new JavaScriptSerializer().Deserialize<StdAddressFullConModel>(dataS);
                var result = GetDataRpt1SearchModel(searchrpt7Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }

        public List<StdAddressFullConList> GetDataRpt1SearchModel(StdAddressFullConModel searchrptModel)
        {
            try
            {
                var query = new STDFullConQuery()
                {
                    //OltNo = (searchrptModel.OltNo.Length > 0) ? searchrptModel.OltNo : "ALL",
                    //Region = searchrptModel.Region
                };

                return GetRpt1SearchReqCurStageQueryData(query);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<StdAddressFullConList>();
            }
        }

        public List<StdAddressFullConList> GetRpt1SearchReqCurStageQueryData(STDFullConQuery query)
        {
            StdAddressFullConListResult result = _queryProcessor.Execute(query);
            return result.Data;
            //return new List<StdAddressFullConList>();
        }

        public ActionResult ExportReadSearchStandFullConReportReport(string dataS, string criteria)
        {
            List<StdAddressFullConList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<StdAddressFullConModel>(dataS);
            /*var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            rptCriteria = criteriaModel.CRITERIA;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;*/

            listall = GetDataRpt1SearchModel(searchModel);


            string filename = GetExcelName("StandardFullConnectionReport");

            var bytes = GenerateSTDEntitytoExcel<StdAddressFullConList>(listall, filename, "FULLCONNECTION");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        /*******************************************************************************************************************
         * End Report FullConnection                                                                                *
         *******************************************************************************************************************/
        #endregion

    }
}
