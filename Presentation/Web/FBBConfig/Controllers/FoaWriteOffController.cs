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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class FoaWriteOffController : FBBConfigController
    {
        //
        // GET: /FoaWriteOff/
        private readonly IQueryProcessor _queryProcessor;
        public FoaWriteOffController(ILogger logger, IQueryProcessor queryProcessor

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

            SetViewBagLov("FBBPAYG_SCREEN", "FOA_WRITE_OFF");

            return View();
        }
        public JsonResult getFoaWriteOff([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {


            try
            {

                var searchFoaWriteOffModel = new JavaScriptSerializer().Deserialize<SearchFoaWriteOffModel>(dataS);

                if (searchFoaWriteOffModel.ACCESS_NO == "" || string.IsNullOrEmpty(searchFoaWriteOffModel.ACCESS_NO))
                {
                    searchFoaWriteOffModel.ACCESS_NO = "ALL";
                }
                if (searchFoaWriteOffModel.SERIAL_NO == "" || string.IsNullOrEmpty(searchFoaWriteOffModel.SERIAL_NO))
                {
                    searchFoaWriteOffModel.SERIAL_NO = "ALL";
                }
                var query = new GetFoawriteoffQuery()
                {
                    p_access_number = searchFoaWriteOffModel.ACCESS_NO,
                    p_serialNumber = searchFoaWriteOffModel.SERIAL_NO
                };
                var result = _queryProcessor.Execute(query);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

                // return null;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error   Call getWriteOff" + ex.GetErrorMessage());

                return null;
            }


        }
        public JsonResult FoaWriteOffConfirmByCheck(List<FoaWriteOffModel> dataSModel)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            List<WriteOffQueryList> NewDataList = new List<WriteOffQueryList>();
            // FoaWriteOffModel WriteOffModels = new FoaWriteOffModel();

            string p_UserName = base.CurrentUser.UserName.ToString();
            try
            {

                foreach (var reusltModel in dataSModel)
                {
                    WriteOffQueryList WriteOffModels = new WriteOffQueryList();

                    WriteOffModels.Access_No = reusltModel.ACCESS_NUMBER;
                    //WriteOffModels.P_ORDER_TYPE = "WRITEOFF";
                    WriteOffModels.SerialNumber = reusltModel.SN;
                    WriteOffModels.MaterialCode = reusltModel.MATERIAL_CODE;
                    WriteOffModels.CompanyCode = reusltModel.COMPANY_CODE;
                    WriteOffModels.Plant = reusltModel.PLANT;
                    WriteOffModels.StorageLocation = reusltModel.STORAGE_LOCATION;
                    // WriteOffModels.P_MOVEMENT_TYPE = "511";
                    // WriteOffModels.P_SNPATTERN = reusltModel.SNPATTERN;
                    // WriteOffModels.P_SN_STATUS = reusltModel.SN_STATUS;
                    WriteOffModels.Create_by = p_UserName;
                    NewDataList.Add(WriteOffModels);
                }

                var query = new GetDataCallSapWriteOffQuery()
                {
                    WriteOffQueryListModels = NewDataList

                };

                var result = _queryProcessor.Execute(query);
                string Returnmessage = string.Empty;

                if (result.ret_code != "0" && !string.IsNullOrEmpty(result.ret_code))
                {
                    Returnmessage = "Write Off Foa Not Success.";
                }
                else
                {
                    Returnmessage = "Write Off Foa  Success.";
                    // Returnmessage = result.ret_msg;
                }
                return Json(new
                {
                    Code = result.ret_code,
                    DataTotal = result.p_ws_writeoff_cur.Count(),
                    SuccessCount = result.ResSuccess,
                    ErrorCount = result.ResError,
                    message = Returnmessage
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                _Logger.Info("Error FoaWrite OffConfirmByCheck Call Sap :" + e.GetErrorMessage());
                return Json(new
                {
                    Code = -1,
                    DataTotal = 0,
                    SuccessCount = 0,
                    ErrorCount = 0,
                    message = " Write Off Foa Not Success."
                }, JsonRequestBehavior.AllowGet);

            }

        }
        private void SetViewBagLov(string screenType, string LovValue5)
        {


            try
            {
                var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
                ViewBag.ListScreen = LovDataScreen;
            }
            catch (Exception ex)
            {

            }



        }
        private void GetDataTableFromExcel(HttpPostedFileBase File)
        {
            DataTable tbl = new DataTable();
            TempDataTableWriteOff.dt = tbl;
            try
            {
                //   FileInfo File = new FileInfo(path);
                using (var pck = new ExcelPackage(File.InputStream))
                {
                    var workbook = pck.Workbook;
                    var ws = pck.Workbook.Worksheets.First();

                    bool hasHeader = true;
                    TempDataTableWriteOff.dt.Columns.Add("ACCESS_NUMBER");
                    TempDataTableWriteOff.dt.Columns.Add("SERIAL_NO");
                    TempDataTableWriteOff.dt.Columns.Add("COMPANY_CODE");
                    TempDataTableWriteOff.dt.Columns.Add("MATERIAL_NO");
                    TempDataTableWriteOff.dt.Columns.Add("PLANT");
                    TempDataTableWriteOff.dt.Columns.Add("STORAGE_LOCATION");
                    //TempDataTableWriteOff.dt.Columns.Add("SN_PATTERN");
                    //TempDataTableWriteOff.dt.Columns.Add("SN_STATUS");

                    var startRow = hasHeader ? 2 : 1;
                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        var row = TempDataTableWriteOff.dt.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                        TempDataTableWriteOff.dt.Rows.Add(row);
                    }

                    // return TempDataTableWriteOff.dt;

                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                // return TempDataTableWriteOff.dt;
            }
        }
        private List<WriteOffQueryList> ConvertDataTableToList()
        {
            List<WriteOffQueryList> Datafile = new List<WriteOffQueryList>();
            string p_UserName = base.CurrentUser.UserName.ToString();
            try
            {

                foreach (DataRow row in TempDataTableWriteOff.dt.Rows)
                {
                    try
                    {

                        var model = new WriteOffQueryList
                        {
                            Access_No = row.ItemArray[0].ToSafeString(),
                            // P_ORDER_TYPE="WRITEOFF",
                            SerialNumber = row.ItemArray[1].ToSafeString(),
                            CompanyCode = row.ItemArray[2].ToSafeString(),
                            MaterialCode = row.ItemArray[3].ToSafeString(),
                            Plant = row.ItemArray[4].ToSafeString(),
                            StorageLocation = row.ItemArray[5].ToSafeString(),
                            // P_SNPATTERN = row.ItemArray[6].ToSafeString(),
                            //P_SN_STATUS = row.ItemArray[7].ToSafeString(),
                            Create_by = p_UserName.ToSafeString()
                        };

                        Datafile.Add(model);
                    }
                    catch (Exception ex)
                    {
                        _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                    }

                }
                // Clear data 
                DataTable tbl = new DataTable();
                TempDataTableWriteOff.dt = tbl;
                // Clear
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                return null;
            }

        }
        public ActionResult ImportFileInternet_Save(HttpPostedFileBase WriteOffByfile)
        {

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (WriteOffByfile != null)
            {

                try
                {
                    if (Path.GetExtension(WriteOffByfile.FileName).ToLower() == ".xls"
                        || Path.GetExtension(WriteOffByfile.FileName).ToLower() == ".xlsx")
                    {
                        //=------------Read Data to DataTable
                        GetDataTableFromExcel(WriteOffByfile);
                        //=------------Read Data to DataTable

                        var modelResponse = new { status = true, message = "", fileName = WriteOffByfile.FileName };

                        return Json(modelResponse, "text/plain");
                    }
                    else
                    {
                        var modelResponse = new { status = false, message = "Please upload .xls,.xlsx file extension", fileName = WriteOffByfile.FileName };

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
            DataTable tbl = new DataTable();
            TempDataTableWriteOff.dt = tbl;
            return Json(null, "text/plain"); ;
        }
        public JsonResult ConfirmFoaWriteOffByFile()
        {
            try
            {


                List<WriteOffQueryList> Models = ConvertDataTableToList();
                List<WriteOffQueryList> modelSendQuerySap = new List<WriteOffQueryList>();
                int ResError = 0;
                bool BoolAccessNumber = true;
                bool BoolSerialno = true;
                bool BoolComCode = true;
                bool BoolMaterialNo = true;
                bool BoolPlant = true;
                bool BoolStorage = true;

                foreach (var re in Models)
                {
                    WriteOffQueryList QueryList = new WriteOffQueryList();
                    //--------------------chk AccessNumber
                    BoolAccessNumber = ChkNumber(re.Access_No);
                    if (BoolAccessNumber)
                    {
                        BoolAccessNumber = ChkCheckLengFix(re.Access_No, 10);
                    }
                    //--------------------chk Serialno
                    BoolSerialno = ChkNumOrChar(re.SerialNumber);
                    if (BoolSerialno)
                    {
                        BoolSerialno = ChkCheckLengMax(re.SerialNumber, 50);
                    }
                    //--------------------chk Serialno
                    BoolComCode = ChkNumber(re.CompanyCode);
                    if (BoolComCode)
                    {
                        BoolComCode = ChkCheckLengMax(re.CompanyCode, 4);
                    }
                    //--------------------chk Serialno
                    BoolMaterialNo = ChkNumOrChar(re.MaterialCode);
                    if (BoolMaterialNo)
                    {
                        BoolMaterialNo = ChkCheckLengMax(re.MaterialCode, 50);
                    }

                    //--------------------chk Serialno
                    BoolPlant = ChkNumber(re.Plant);
                    if (BoolPlant)
                    {
                        BoolPlant = ChkCheckLengMax(re.Plant, 4);
                    }
                    //--------------------chk Serialno
                    BoolStorage = ChkNumber(re.StorageLocation);
                    if (BoolStorage)
                    {
                        BoolStorage = ChkCheckLengMax(re.StorageLocation, 50);
                    }
                    //---------------------------end------------------
                    if (BoolAccessNumber == true
                        && BoolSerialno == true
                        && BoolComCode == true
                        && BoolMaterialNo == true
                        && BoolPlant == true
                        && BoolStorage == true)
                    {
                        QueryList.Access_No = re.Access_No;
                        QueryList.SerialNumber = re.SerialNumber;
                        QueryList.CompanyCode = re.CompanyCode;
                        QueryList.MaterialCode = re.MaterialCode;
                        QueryList.Plant = re.Plant;
                        QueryList.StorageLocation = re.StorageLocation;

                        modelSendQuerySap.Add(QueryList);

                    }
                    else
                    {

                        ResError += 1;
                    }
                }


                var query = new GetDataCallSapWriteOffQuery()
                {
                    WriteOffQueryListModels = modelSendQuerySap

                };

                var result = _queryProcessor.Execute(query);
                string Returnmessage = string.Empty;


                // Clear  DataTable Temp
                DataTable tbl = new DataTable();
                TempDataTableWriteOff.dt = tbl;
                // End Clear  DataTable Temp 
                if (result.ret_code != "0" && !string.IsNullOrEmpty(result.ret_code))
                {
                    Returnmessage = "Write Off Foa not Success.";
                }
                else
                {

                    Returnmessage = "Write Off Foa  Success.";

                }
                return Json(new
                {
                    Code = result.ret_code,
                    DataTotal = Models.Count(),
                    SuccessCount = result.ResSuccess,
                    ErrorCount = result.ResError + ResError,
                    message = Returnmessage
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {

            }
            return Json(new
            {
                Code = -1,
                DataTotal = 0,
                SuccessCount = 0,
                ErrorCount = 0,
                message = " Write Off Foa Not Success."
            }, JsonRequestBehavior.AllowGet);
        }
        #region SetDropDowns
        public JsonResult SetDDLCompanyCode()
        {
            var queryCompanyCode = new GetListFbssFixedAssetConfigQuery() { DDLName = "CompanyCode" };
            var LovCompanyCodeData = _queryProcessor.Execute(queryCompanyCode);
            var LovData = LovCompanyCodeData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();

            LovData.Insert(0, new { LOV_NAME = "-- Select --", LOV_VAL1 = "ALL" });

            return Json(LovData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetDDLPlant(string CompanyCode)
        {
            List<LovModel> LovPlantData = new List<LovModel>();

            if (!string.IsNullOrEmpty(CompanyCode))
            {
                var queryPlant = new GetListFbssFixedAssetConfigQuery() { DDLName = "Plant", Param1 = CompanyCode };
                LovPlantData = _queryProcessor.Execute(queryPlant);
                var LovData = LovPlantData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
                if (string.IsNullOrEmpty(CompanyCode)) LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
                return Json(LovData, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var LovData = LovPlantData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
                if (string.IsNullOrEmpty(CompanyCode)) LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
                return Json(LovData, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion
        public bool ChkNumber(string strInput)
        {
            if (!string.IsNullOrEmpty(strInput.Trim().ToSafeString()))
            {
                var result = Regex.IsMatch(strInput, "^[0-9]*$");
                return result;
            }
            else
            {
                return false;
            }


        }
        public bool ChkNumOrChar(string strInput)
        {
            if (!string.IsNullOrEmpty(strInput.Trim().ToSafeString()))
            {
                var result = Regex.IsMatch(strInput, "^[A-Za-z0-9]*$");
                return result;
            }
            else
            {
                return false;
            }

        }
        public bool ChkCheckLengFix(string v, int len_num)
        {
            if (v.Length > len_num || v.Length < len_num)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool ChkCheckLengMax(string v, int len_num)
        {
            if (v.Length > len_num)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //R19.05
        public ActionResult ExportTemplateWriteOffFoa()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "WriteOffFoa.xlsx";
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
