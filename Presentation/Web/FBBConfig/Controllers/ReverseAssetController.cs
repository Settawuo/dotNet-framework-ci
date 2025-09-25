using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class ReverseAssetController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;

        public ReverseAssetController(ILogger logger, IQueryProcessor queryProcessor

            )
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
        }


        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            //  SiteSession.CurrentUICulture = 1;

            TempData.Remove("TempDataTable");

            SetViewBagLov("FBBPAYG_SCREEN", "REVERSEASSET");
            return View();
        }

        private void SetViewBagLov(string screenType, string LovValue5)
        {


            try
            {
                var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
                ViewBag.SearchListScreen = LovDataScreen;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error SetViewBagLov " + ex.GetErrorMessage());
            }



        }

        public ActionResult ImportFileInternet_Save(HttpPostedFileBase ReverseAssetfile)
        {
            TempData.Remove("TempDataTable");

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (ReverseAssetfile != null)
            {

                try
                {
                    if (Path.GetExtension(ReverseAssetfile.FileName).ToLower() == ".xls"
                        || Path.GetExtension(ReverseAssetfile.FileName).ToLower() == ".xlsx")
                    {
                        TempData["TempDataTable"] = GetDataTableFromExcel(ReverseAssetfile);
                        var modelResponse = new { status = true, message = "", fileName = ReverseAssetfile.FileName };

                        return Json(modelResponse, "text/plain");
                    }
                    else
                    {
                        var modelResponse = new { status = false, message = "Please upload .xls,.xlsx file extension", fileName = ReverseAssetfile.FileName };

                        return Json(modelResponse, "text/plain");
                    }


                }
                catch (Exception e)
                {
                    _Logger.Info("Error Import Excel File" + e.GetErrorMessage());
                    var modelResponse = new { status = false, message = e.GetErrorMessage(), fileName = "" };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }

        public ActionResult ImportFileInternet_Remove()
        {
            TempData.Remove("TempDataTable");
            return Json(null, "text/plain"); ;
        }

        public List<ReverseAssetAcessNumber> ConvertDataTableToList()
        {
            var historyLog = new FBB_HISTORY_LOG();

            DataTable TempDataTable = (DataTable)TempData.Peek("TempDataTable");
            if (TempDataTable == null)
                return null;

            var Datafile = new List<ReverseAssetAcessNumber>();

            if (TempDataTable == null)
                return null;
            try
            {
                if (TempDataTable != null)
                {
                    foreach (DataRow row in TempDataTable.Rows)
                    {
                        try
                        {



                            var model = new ReverseAssetAcessNumber
                            {
                                ACCESS_NUMBER = row.ItemArray[0].ToSafeString(),
                                ASSET_CODE = row.ItemArray[1].ToSafeString(),
                                ACTION = row.ItemArray[2].ToSafeString(),

                            };

                            Datafile.Add(model);
                        }
                        catch (Exception ex)
                        {
                            _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                        }
                    }
                }
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                return null;
            }

        }



        public ActionResult getReverseAsset([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");


            TempData.Remove("TempDataTable");

            if (dataS != null && dataS != "")
            {

                try
                {

                    var searchReverseAssetModel = new JavaScriptSerializer().Deserialize<ReverseAssetSearchModel>(dataS);



                    if (!string.IsNullOrEmpty(searchReverseAssetModel.ACCESS_NUMBER)
                         || !string.IsNullOrEmpty(searchReverseAssetModel.ASSET_CODE)
                        )
                    {

                        var query = new ReverseAssetQuery()
                        {
                            p_ACCESS_NO = searchReverseAssetModel.ACCESS_NUMBER.ToSafeString(),
                            p_ASSET_CODE = searchReverseAssetModel.ASSET_CODE.ToSafeString()


                        };
                        var result = _queryProcessor.Execute(query);

                        return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    _Logger.Info("Error   Call getReverseAsset" + ex.GetErrorMessage());

                    return null;
                }
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ConfirmReverseAssetByFile(string dataS = "")
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            try
            {
                var AessetNumberToList = ConvertDataTableToList();

                dictionary = ReverseAssetCallSapServices(AessetNumberToList);

                return Json(new
                {
                    Code = 0,
                    DataTotal = dictionary["DataTotal"],
                    SuccessCount = dictionary["ResSuccess"],
                    ErrorCount = dictionary["ResError"],
                    message = "Reverse Asset Success."
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _Logger.Info("Error   Call ConfirmReverseAssetByFile" + ex.GetErrorMessage());

                return Json(new
                {
                    Code = -1,
                    DataTotal = 0,
                    SuccessCount = 0,
                    ErrorCount = 0,
                    message = "Reverse Asset Not Success."
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ReverseAssetConfirmByCheck(List<ReverseAssetAcessNumber> dataSModel)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            try
            {
                dictionary = ReverseAssetCallSapServices(dataSModel);

                return Json(new
                {
                    Code = 0,
                    DataTotal = dictionary["DataTotal"],
                    SuccessCount = dictionary["ResSuccess"],
                    ErrorCount = dictionary["ResError"],
                    message = "Reverse Asset Success."
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                _Logger.Info("Error ReverseAssetConfirmByCheck Call Sap :" + e.GetErrorMessage());
                return Json(new
                {
                    Code = -1,
                    DataTotal = 0,
                    SuccessCount = 0,
                    ErrorCount = 0,
                    message = "Reverse Asset Not Success."
                }, JsonRequestBehavior.AllowGet);

            }

        }

        public Dictionary<string, int> ReverseAssetCallSapServices(List<ReverseAssetAcessNumber> dataSModel)
        {
            string p_USER_CODE = base.CurrentUser.UserId.ToString();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            ReturnReverseSapModel result = new ReturnReverseSapModel();

            try
            {
                var query = new ReverseAssetSapQuery()
                {
                    p_ACCESS_list = dataSModel,
                    p_USER_CODE = p_USER_CODE,

                };

                result = _queryProcessor.Execute(query);


                dictionary.Add("DataTotal", result.p_ws_revalue_cur.Count());
                dictionary.Add("ResSuccess", result.ResSuccess);
                dictionary.Add("ResError", result.ResError);

                return dictionary;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error   Call Sap Services" + ex.GetErrorMessage());
                dictionary.Add("ResSuccess", 0);
                dictionary.Add("ResError", 0);
                dictionary.Add("DataTotal", 0);
                return dictionary;
            }


        }
        public DataTable GetDataTableFromExcel(HttpPostedFileBase File)
        {
            DataTable tbl = new DataTable();
            try
            {
                //   FileInfo File = new FileInfo(path);
                using (var pck = new ExcelPackage(File.InputStream))
                {
                    var workbook = pck.Workbook;
                    var ws = pck.Workbook.Worksheets.First();

                    bool hasHeader = true; // adjust it accordingly( i've mentioned that this is a simple approach)
                    tbl.Columns.Add("ACCESS_NUMBER");
                    tbl.Columns.Add("ASSET_CODE");
                    tbl.Columns.Add("ACTION");
                    //foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    //{
                    //    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    //}
                    var startRow = hasHeader ? 2 : 1;
                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        var row = tbl.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                        tbl.Rows.Add(row);
                    }
                    //   ReverseAssetFileModel.FileData = pck.GetAsByteArray();
                    return tbl;

                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return tbl;
            }
        }


        //R19.05
        public ActionResult ExportTemplateReverseAsset()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "TemplateReverseAsset.xlsx";
                byte[] bytes = dlf.ExportExcelTemplate(filename);
                return File(bytes, MediaTypeNames.Application.Octet, filename);
            }
            catch (Exception ex)
            {
                return null;
            }


        }
    }
}
