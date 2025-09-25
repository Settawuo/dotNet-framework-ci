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
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class UpdateAssetController : FBBConfigController
    {
        //
        // GET: /UpdateAsset/
        //  private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateAssetCommand> _UpdateAssetCommand;
        public UpdateAssetController(
               ILogger logger,
              ICommandHandler<UpdateAssetCommand> UpdateAssetCommand
         //  IQueryProcessor queryProcessor
         )
        {
            _Logger = logger;
            _UpdateAssetCommand = UpdateAssetCommand;
            //_queryProcessor = queryProcessor;
        }
        public ActionResult Index()
        {
            TempData.Remove("TempDataTableUpdAsset");

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");


            SetViewBagLov("FBBPAYG_SCREEN", "UPDATE_ASSET");
            ViewBag.User = base.CurrentUser;
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
        public ActionResult ImportFileUpd_Save(HttpPostedFileBase UpdateAssetfile)
        {
            TempData.Remove("TempDataTableUpdAsset");
            bool ckdata = false;

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");


            if (UpdateAssetfile != null)
            {

                try
                {
                    if (Path.GetExtension(UpdateAssetfile.FileName).ToLower() == ".xls"
                        || Path.GetExtension(UpdateAssetfile.FileName).ToLower() == ".xlsx")
                    {

                        TempData["TempDataTableUpdAsset"] = GetDataTableFromExcel(UpdateAssetfile);
                        DataTable TempDataTable = (DataTable)TempData.Peek("TempDataTableUpdAsset");

                        if (TempDataTable.Rows.Count > 0)
                        {
                            ckdata = true;
                        }

                        var modelResponse = new
                        {
                            status = true,
                            message = "",
                            ckdata = ckdata,
                            fileName = UpdateAssetfile.FileName

                        };

                        return Json(modelResponse, "text/plain");
                    }
                    else
                    {
                        var modelResponse = new { status = false, message = "Please upload .xls,.xlsx file extension", fileName = UpdateAssetfile.FileName };

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
        public List<UpdateAssetModel> ConvertDataTableToList()
        {

            DataTable TempDataTable = (DataTable)TempData.Peek("TempDataTableUpdAsset");
            if (TempDataTable == null)
                return null;

            var Datafile = new List<UpdateAssetModel>();


            try
            {
                if (TempDataTable != null)
                {
                    foreach (DataRow row in TempDataTable.Rows)
                    {
                        try
                        {



                            var model = new UpdateAssetModel
                            {
                                Access_No = row.ItemArray[0].ToSafeString(),
                                Serial_Number = row.ItemArray[1].ToSafeString(),
                                Asset_Code = row.ItemArray[2].ToSafeString(),
                                Mat_Doc = row.ItemArray[3].ToSafeString(),
                                Doc_Year = row.ItemArray[4].ToSafeString()

                            };

                            Datafile.Add(model);
                        }
                        catch (Exception ex)
                        {
                            _Logger.Info("Error ConvertDataTableToList TempDataTableUpdAsset:" + ex.GetErrorMessage());
                        }
                    }
                }
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList TempDataTableUpdAsset:" + ex.GetErrorMessage());
                return null;
            }

        }

        private bool CheckNumberic(string Str = "")
        {

            //int Num;
            //bool isNum = false;

            //isNum = int.TryParse(Str, out Num);

            //return isNum;


            try
            {
                //Str = Str.Trim();
                //int foo = int.Parse(Str);
                //return (true);

                foreach (char c in Str)
                {
                    if (c < '0' || c > '9')
                        return false;
                }

                return true;

            }
            catch (FormatException)
            {
                // Not a numeric value
                return (false);
            }



        }
        public List<FBB_Update_asset_list> ConvertDataTableToListCommnad()
        {

            DataTable TempDataTable = (DataTable)TempData.Peek("TempDataTableUpdAsset");
            if (TempDataTable == null)
                return null;

            var Datafile = new List<FBB_Update_asset_list>();
            //int out_success = 0, out_error = 0;

            try
            {
                if (TempDataTable != null)
                {
                    foreach (DataRow row in TempDataTable.Rows)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(row.ItemArray[2].ToSafeString())
                                 && !string.IsNullOrEmpty(row.ItemArray[3].ToSafeString())
                                 && !string.IsNullOrEmpty(row.ItemArray[4].ToSafeString())
                                 && row.ItemArray[2].ToSafeString().Length == 12
                                 && row.ItemArray[3].ToSafeString().Length == 10
                                 && row.ItemArray[4].ToSafeString().Length == 4
                                 && CheckNumberic(row.ItemArray[2].ToSafeString()) == true
                                 && CheckNumberic(row.ItemArray[3].ToSafeString()) == true
                                 && CheckNumberic(row.ItemArray[4].ToSafeString()) == true

                         )
                            {
                                // out_success += 1;

                                var model = new FBB_Update_asset_list
                                {
                                    p_Access_No = row.ItemArray[0].ToSafeString(),
                                    p_Serial_Number = row.ItemArray[1].ToSafeString(),
                                    p_Asset_Code = row.ItemArray[2].ToSafeString(),
                                    p_Mat_Doc = row.ItemArray[3].ToSafeString(),
                                    p_Doc_Year = row.ItemArray[4].ToSafeString(),

                                };

                                Datafile.Add(model);

                            }
                            //else
                            //{
                            //    out_error += 1;
                            //}


                        }
                        catch (Exception ex)
                        {
                            _Logger.Info("Error ConvertDataTableToListCommnad  Loop For TempDataTableUpdAsset:" + ex.GetErrorMessage());
                        }
                    }
                }
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToListCommnad TempDataTableUpdAsset:" + ex.GetErrorMessage());
                return null;
            }

        }

        public JsonResult GetUpdateAssetDataFromExcel([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {

            var Datafile = new List<UpdateAssetModel>();
            try
            {


                Datafile = ConvertDataTableToList();

                return Json(Datafile.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _Logger.Info("Error GetUpdateAssetDataFromExcel  " + ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet); ;
            }
        }

        public validateDataModel validateData()
        {

            DataTable TempDataTable = (DataTable)TempData.Peek("TempDataTableUpdAsset");
            if (TempDataTable == null)
                return null;

            validateDataModel Datafile = new validateDataModel();

            var _UpdSuccess = new List<UpdateAssetModel>();
            var _UpdError = new List<UpdateAssetModel>();

            int out_success = 0, out_error = 0, total = 0;

            try
            {
                if (TempDataTable != null)
                {
                    foreach (DataRow row in TempDataTable.Rows)
                    {
                        try
                        {

                            if (!string.IsNullOrEmpty(row.ItemArray[2].ToSafeString())
                                 && !string.IsNullOrEmpty(row.ItemArray[3].ToSafeString())
                                 && !string.IsNullOrEmpty(row.ItemArray[4].ToSafeString())
                                 && row.ItemArray[2].ToSafeString().Length == 12
                                 && row.ItemArray[3].ToSafeString().Length == 10
                                 && row.ItemArray[4].ToSafeString().Length == 4
                                 && CheckNumberic(row.ItemArray[2].ToSafeString()) == true
                                 && CheckNumberic(row.ItemArray[3].ToSafeString()) == true
                                 && CheckNumberic(row.ItemArray[4].ToSafeString()) == true
                                 )
                            {
                                _UpdSuccess.Add(new UpdateAssetModel()
                                {
                                    Access_No = row.ItemArray[0].ToSafeString(),
                                    Serial_Number = row.ItemArray[1].ToSafeString(),
                                    Asset_Code = row.ItemArray[2].ToSafeString(),
                                    Mat_Doc = row.ItemArray[3].ToSafeString(),
                                    Doc_Year = row.ItemArray[4].ToSafeString()
                                });



                                out_success += 1;

                            }
                            else
                            {
                                _UpdError.Add(new UpdateAssetModel()
                                {
                                    Access_No = row.ItemArray[0].ToSafeString(),
                                    Serial_Number = row.ItemArray[1].ToSafeString(),
                                    Asset_Code = row.ItemArray[2].ToSafeString(),
                                    Mat_Doc = row.ItemArray[3].ToSafeString(),
                                    Doc_Year = row.ItemArray[4].ToSafeString()
                                });


                                out_error += 1;

                            }


                        }
                        catch (Exception ex)
                        {
                            _Logger.Info("Error ConvertDataTableToList TempDataTableUpdAsset:" + ex.GetErrorMessage());
                        }

                        Datafile.total = total += 1;
                    } // end for

                    Datafile.ListUpdSuccess = _UpdSuccess;
                    Datafile.UpdSuccess = out_success;
                    Datafile.ListUpdError = _UpdError;
                    Datafile.UpdUpdError = out_error;
                }
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList TempDataTableUpdAsset:" + ex.GetErrorMessage());
                return null;
            }

        }
        public JsonResult ConfirmUpdateAssetFromFile(string dataS = "")
        {


            validateDataModel validateDataOjb = new validateDataModel();

            List<FBB_Update_asset_list> Update_asset_data_list = new List<FBB_Update_asset_list>();
            List<FBB_Update_asset_list> Update_asset_data_list_success = new List<FBB_Update_asset_list>();
            List<FBB_Update_asset_list> Update_asset_data_list_error = new List<FBB_Update_asset_list>();
            try
            {
                validateDataOjb = validateData();
                Update_asset_data_list = ConvertDataTableToListCommnad();

                if (Update_asset_data_list != null)
                {
                    if (validateDataOjb.ListUpdSuccess.Count > 0 || validateDataOjb.ListUpdError.Count > 0)
                    {

                        #region ConvertClassModel
                        // ----------------------------ListUpdSuccess
                        if (validateDataOjb.ListUpdSuccess.Count > 0)
                        {

                            foreach (var ResuleSuccess in validateDataOjb.ListUpdSuccess)
                            {
                                Update_asset_data_list_success.Add(new FBB_Update_asset_list
                                {

                                    p_Access_No = ResuleSuccess.Access_No,
                                    p_Asset_Code = ResuleSuccess.Asset_Code,
                                    p_Doc_Year = ResuleSuccess.Doc_Year,
                                    p_Mat_Doc = ResuleSuccess.Mat_Doc,
                                    p_Serial_Number = ResuleSuccess.Serial_Number,

                                });
                            }
                        }
                        // ----------------------------ListUpdError 
                        if (validateDataOjb.ListUpdError.Count > 0)
                        {
                            foreach (var ResuleSuccess in validateDataOjb.ListUpdError)
                            {
                                Update_asset_data_list_error.Add(new FBB_Update_asset_list
                                {

                                    p_Access_No = ResuleSuccess.Access_No,
                                    p_Asset_Code = ResuleSuccess.Asset_Code,
                                    p_Doc_Year = ResuleSuccess.Doc_Year,
                                    p_Mat_Doc = ResuleSuccess.Mat_Doc,
                                    p_Serial_Number = ResuleSuccess.Serial_Number,

                                });
                            }
                        }
                        #endregion
                        //--------------------------
                        #region CalltCommand
                        string Code = "-1", ret_msg = "ERROR";

                        var ExcuteComand = new UpdateAssetCommand
                        {
                            p_Update_asset_list = Update_asset_data_list,
                            p_Update_asset_list_success = Update_asset_data_list_success,
                            p_Update_asset_list_Error = Update_asset_data_list_error
                        };
                        _UpdateAssetCommand.Handle(ExcuteComand);
                        Code = ExcuteComand.ret_code.ToSafeString();
                        ret_msg = ExcuteComand.ret_msg.ToSafeString();
                        //--------------------------------------
                        if (Code == "0" && ret_msg == "Success")
                        {


                            return Json(new
                            {
                                Code = 0,
                                DataTotal = validateDataOjb.total,
                                SuccessCount = validateDataOjb.UpdSuccess,
                                ErrorCount = validateDataOjb.UpdUpdError,
                                message = "Update Asset Success."
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                Code = -1,
                                DataTotal = validateDataOjb.total,
                                SuccessCount = validateDataOjb.UpdSuccess,
                                ErrorCount = validateDataOjb.UpdUpdError,
                                message = "Update Asset Not Success."
                            }, JsonRequestBehavior.AllowGet);
                        }

                        #endregion
                    }
                    else
                    {
                        return Json(new
                        {
                            Code = -1,
                            DataTotal = validateDataOjb.total,
                            SuccessCount = validateDataOjb.UpdSuccess,
                            ErrorCount = validateDataOjb.UpdUpdError,
                            message = "Data Not Found."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    //--------------------------------------
                }
                else
                {
                    return Json(new
                    {
                        Code = -1,
                        DataTotal = validateDataOjb.total,
                        SuccessCount = validateDataOjb.UpdSuccess,
                        ErrorCount = validateDataOjb.UpdUpdError,
                        message = "Data Not Found."
                    }, JsonRequestBehavior.AllowGet);
                }

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
                    message = "Update Asset Not Success."
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ImportFileUpd_Remove()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            TempData.Remove("TempDataTableUpdAsset");
            return Json(new
            {
                Code = 0,
                message = "ImportFileUpd Remove Success."
            }, "text/plain"); ;
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
                    tbl.Columns.Add("Access No");
                    tbl.Columns.Add("Serial Number");
                    tbl.Columns.Add("Asset Code");
                    tbl.Columns.Add("Mat Doc");
                    tbl.Columns.Add("Doc Year");
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
                        if (string.IsNullOrEmpty(row[0].ToSafeString())
                            && string.IsNullOrEmpty(row[1].ToSafeString())
                            && string.IsNullOrEmpty(row[2].ToSafeString())
                            && string.IsNullOrEmpty(row[3].ToSafeString())
                            && string.IsNullOrEmpty(row[4].ToSafeString())
                         )
                        {

                        }
                        else
                        {
                            tbl.Rows.Add(row);
                        }

                    }
                    //   UpdateAssetFileModel.FileData = pck.GetAsByteArray();
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
        public ActionResult ExportTemplateUpdateAsset()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "TemplateUpdateAsset.xlsx";
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
