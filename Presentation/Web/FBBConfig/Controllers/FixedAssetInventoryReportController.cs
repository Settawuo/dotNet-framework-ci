using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class FixedAssetInventoryReportController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private string rptName = "Report Name : {0}";
        private string rptCriteria = "OrderType: {0}     ProductName : {1}";
        private string rptCriteriaDate = "Date From : {0}  To : {1}";
        private string rptDate = "Run Report Date/Time : {0}";
        //
        // GET: /FixedAssetInventoryReport/

        public FixedAssetInventoryReportController(ILogger logger,

            IQueryProcessor queryProcessor)
        {
            base._Logger = logger;

            _queryProcessor = queryProcessor;
        }
        private void SetViewBagLovV2(string screenType)
        {
            var query = new GetLovQuery()
            {
                LovType = screenType
            };

            var LovDataScreen = _queryProcessor.Execute(query).ToList();
            ViewBag.configscreen = LovDataScreen;
        }
        private void SetViewBagLov(string screenType)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType).ToList();
            ViewBag.configscreen = LovDataScreen;
        }
        public ActionResult FixedAssetInventoryReport()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;

            SetViewBagLovV2("FIXED_RPT");
            var rptModel = new FixedAssetInventoryModel();

            return View(rptModel);

        }
        public JsonResult SelectAllOrderType()
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = "ORDER_TYPE"
            };

            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "ALL", LOV_NAME = "ALL" });


            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SelectAllServiceName()
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = "SEVICE_NAME"
            };

            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "ALL", LOV_NAME = "ALL" });


            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SelectAllProductName()
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = "EVA5"
            };

            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "ALL", LOV_NAME = "ALL" });


            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult SearchFixAssetInventory([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            //   _Logger.Info("ReadSearchTrackingCompetitorRpt02 start");
            if (dataS != null && dataS != "")
            {
                try
                {
                    // _Logger.Info("ReadSearchTrackingCompetitorRpt02 try");
                    var searchModel = new JavaScriptSerializer().Deserialize<FixedAssetInventoryScreenModel>(dataS);
                    string df = string.Empty; string dt = string.Empty;
                    if (searchModel.DateFrom != null)
                    {
                        df = searchModel.DateFrom.Replace("/", "");
                    }
                    if (searchModel.DateTo != null)
                    {
                        dt = searchModel.DateTo.Replace("/", "");
                    }


                    var query = new GetFixedAssetResultQuery()
                    {
                        p_order_type = searchModel.OrderType,
                        p_product_name = searchModel.ProductName,
                        //  p_service_name = ServName,
                        p_ord_dt_from = df,
                        p_ord_dt_to = dt


                    };
                    var result = _queryProcessor.Execute(query);

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //  _Logger.Info("Error when call ReadSearchTrackingCompetitorRpt02");
                    _Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        private string GetSaleTrackingExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        public ActionResult ExportFixAssetInventory(string dataS)
        {

            var searchModel = new JavaScriptSerializer().Deserialize<FixedAssetInventoryScreenModel>(dataS);

            List<FixedAssetInventoryModel> listall = new List<FixedAssetInventoryModel>();
            string df = string.Empty; string dt = string.Empty;
            if (searchModel.DateFrom != null)
            {
                df = searchModel.DateFrom.Replace("/", "");
            }
            if (searchModel.DateTo != null)
            {
                dt = searchModel.DateTo.Replace("/", "");
            }


            var query = new GetFixedAssetResultQuery()
            {
                p_order_type = searchModel.OrderType,
                p_product_name = searchModel.ProductName,
                //  p_service_name = ServName,
                p_ord_dt_from = df,
                p_ord_dt_to = dt


            };
            var dataout = _queryProcessor.Execute(query);
            listall = dataout;
            rptCriteria = string.Format(rptCriteria, searchModel.OrderType, searchModel.ProductName);
            rptCriteriaDate = string.Format(rptCriteriaDate, searchModel.DateFrom, searchModel.DateTo);
            rptName = string.Format(rptName, "Fix Asset Inventory Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetSaleTrackingExcelName("FBSS_ASSET_REPORT");
            var bytes = GenerateFixzAssetEntitytoExcel<FixedAssetInventoryModel>(listall, filename, "PKG_FIXED_ASSET_INVENTORY_RPT");

            return File(bytes, "application/excel", filename + ".xls");
            //   return File("", "application/excel", "" + ".xls");
        }

        private List<FixedAssetInventoryModel> ConvertFixAssetModel(List<FixedAssetInventoryModel> list)
        {
            var dataexportlist = list.Select(x => new FixedAssetInventoryModel()
            {
                ACC_NBR = x.ACC_NBR.ToSafeString(),

                ORD_NO = x.ORD_NO.ToSafeString(),
                ON_TOP1 = x.ON_TOP1.ToSafeString(),
                ON_TOP2 = x.ON_TOP2.ToSafeString(),
                ORDER_TYPE = x.ORDER_TYPE.ToSafeString(),
                ORDER_DATE = x.ORDER_DATE.ToSafeString(),
                PRODUCT_NAME = x.PRODUCT_NAME.ToSafeString(),
                MATERIAL_CODE_CPESN = x.MATERIAL_CODE_CPESN.ToSafeString(),
                CPE_SN = x.CPE_SN.ToSafeString(),
                SBC_CPY = x.SBC_CPY.ToSafeString(),



                MATERIAL_CODE_STBSN1 = x.MATERIAL_CODE_STBSN1.ToSafeString(),
                STB_SN1 = x.STB_SN1.ToSafeString(),

                MATERIAL_CODE_STBSN2 = x.MATERIAL_CODE_STBSN2.ToSafeString(),
                STB_SN2 = x.STB_SN2.ToSafeString(),

                MATERIAL_CODE_STBSN3 = x.MATERIAL_CODE_STBSN3.ToSafeString(),
                STB_SN3 = x.STB_SN3.ToSafeString(),

                MATERIAL_CODE_ATASN = x.MATERIAL_CODE_ATASN.ToSafeString(),
                ATA_SN = x.ATA_SN.ToSafeString(),
                MATERIAL_CODE_WIFIROUTESN = x.MATERIAL_CODE_WIFIROUTESN.ToSafeString(),
                WIFI_ROUTER_SN = x.WIFI_ROUTER_SN.ToSafeString(),
                ASSET_GI = x.ASSET_GI.ToSafeString(),
                GI_ASSET_DATE = x.GI_ASSET_DATE.ToSafeString(),
                ASSET_PB1 = x.ASSET_PB1.ToSafeString(),
                PB1_ASSET_DATE = x.PB1_ASSET_DATE.ToSafeString(),
                ASSET_PB2 = x.ASSET_PB2.ToSafeString(),
                PB2_ASSET_DATE = x.PB2_ASSET_DATE.ToSafeString(),
                ASSET_PB3 = x.ASSET_PB3.ToSafeString(),
                PB3_ASSET_DATE = x.PB3_ASSET_DATE.ToSafeString(),
                INSTALL_EXPENSE = x.INSTALL_EXPENSE.ToSafeString(),
                ASSET_INS = x.ASSET_INS.ToSafeString(),
                FLAG_TYPE = x.FLAG_TYPE.ToSafeString()

            });

            return dataexportlist.ToList();
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateFixzAssetEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            //  _Logger.Info("GenerateSaleTrackingEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.Type == "FIXED_RPT" && p.LovValue5 == LovValue5).ToList();

            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].Name.ToSafeString(), System.Type.GetType("System.String"));
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
            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateSaleTrackingExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // [ExportExcel] Generate Excel
        private byte[] GenerateSaleTrackingExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            //  _Logger.Info("GenerateSaleTrackingExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 31;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                worksheet.Cells["A4:I4"].Merge = true;
                worksheet.Cells["A4,I4"].LoadFromText(rptCriteriaDate);
                worksheet.Cells["A5:I5"].Merge = true;
                worksheet.Cells["A5,I5"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 7;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                //  worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, false);
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true);



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


    }
}
