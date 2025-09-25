using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBContract.Queries.PatchEquipment;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using PatchCPEModel = WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel;

namespace FBBConfig.Controllers
{
    public class PatchCpeController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FBBPaygPatchDataInsertCommand> _insertCommand;
        private readonly ICommandHandler<FBBPaygPatchDataUpdateCommand> _updateCommand;
        private readonly ICommandHandler<FBBPaygPatchDataListUpdateCommand> _updateListCommand;
        private readonly ICommandHandler<FBBPaygPatchDataInsertSendMailCommand> _SendMailCommand;

        public PatchCpeController(
            IQueryProcessor queryProcessor,
            ICommandHandler<FBBPaygPatchDataInsertCommand> insertCommand,
            ICommandHandler<FBBPaygPatchDataUpdateCommand> updateCommand,
            ICommandHandler<FBBPaygPatchDataListUpdateCommand> updateListCommand,
            ICommandHandler<FBBPaygPatchDataInsertSendMailCommand> SendMailCommand)
        {
            _queryProcessor = queryProcessor;
            _insertCommand = insertCommand;
            _updateCommand = updateCommand;
            _updateListCommand = updateListCommand;
            _SendMailCommand = SendMailCommand;
        }

        public ActionResult Index()
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            //  CurrentUser.E
            ViewBag.User = CurrentUser;
            // Clear SESSTION
            Session["DataCheckbox"] = null;
            Session["DataList"] = null;
            Session["SNList"] = null;
            try
            {
                ViewBag.Page_Search_Order = Get_FBB_CFG_LOV("SCREEN", "", "Page_Search_Order");
                ViewBag.Page_Patch_SN = Get_FBB_CFG_LOV("SCREEN", "", "Page_Patch_SN");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Account");
            }

            return View();
        }

        public void SetViewBagLov(string LOV_TYPE, string LOV_NAME, string LOV_VAL5)
        {
            var query = new GetLovQuery()
            {
                LovType = LOV_TYPE,
                LovName = LOV_NAME
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);
        }

        public List<LovValueModel> Get_FBB_CFG_LOV(string LOV_TYPE, string LOV_NAME, string LOV_VAL5)
        {
            var query = new GetLovWithParamsQuery()
            {
                LovType = LOV_TYPE,
                LovName = LOV_NAME,
                LovValue5 = LOV_VAL5
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);


            return _FbbCfgLov;
        }

        public JsonResult SetDDLStatus()
        {
            var EventStatus = Get_FBB_CFG_LOV("SCREEN", "EVENT_STATUS", "Page_Patch_SN");
            var model = new List<LovModel>();
            if (EventStatus.Count != 0)
            {
                foreach (var item in EventStatus)
                {
                    model.Add(new LovModel() { LOV_NAME = item.LovValue1, LOV_VAL1 = item.LovValue2 });
                }
            }
            else
            {
                model.Add(new LovModel() { LOV_NAME = "SELECT ALL", LOV_VAL1 = "ALL" });
                model.Add(new LovModel() { LOV_NAME = "COMPLETE", LOV_VAL1 = "COMPLETE" });
                model.Add(new LovModel() { LOV_NAME = "PENDING", LOV_VAL1 = "Pending" });
                model.Add(new LovModel() { LOV_NAME = "PENDING_NEWREGIST", LOV_VAL1 = "PENDING_NEWREGIST" });
                model.Add(new LovModel() { LOV_NAME = "SUCCESS", LOV_VAL1 = "SUCCESS" });
                model.Add(new LovModel() { LOV_NAME = "CANCEL", LOV_VAL1 = "CANCEL" });
            }
            var LovData = model.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        #region Search Data
        public ActionResult SeachOrderBySN(List<PatchEquipmentQuery> DataModel)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            try
            {
                var process_hvr = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
                if (process_hvr.DISPLAY_VAL == "Y")
                {
                    var query = new PatchSearchOrderHVRQuery
                    {
                        SerialNo = DataModel.Select(x => x.SerialNo).ToList()
                    };
                    var result = _queryProcessor.Execute(query);
                    if (result != null)
                    {
                        Session["SNList"] = result;
                        int row = 1;
                        foreach (var item in result)
                        {
                            item.NO = row;
                            row++;
                        }
                        return Json(result);
                    }
                    else
                    {
                        return Json("");
                    }
                }
                else
                {
                    var query = new PatchSearchOrderQuery
                    {
                        SerialNo = DataModel.Select(x => x.SerialNo).ToList()
                    };
                    var result = _queryProcessor.Execute(query);
                    if (result != null)
                    {
                        Session["SNList"] = result;
                        int row = 1;
                        foreach (var item in result)
                        {
                            item.NO = row;
                            row++;
                        }
                        return Json(result);
                    }

                    else
                    {
                        return Json("");
                    }

                }

            }
            catch (Exception ex)
            {
                _Logger.Info("ERROR : SeachOrderBySN " + ex.ToSafeString());
                return Json(new { ret_code = 1, ret_msg = "Error. Please contact System Admin" });
            }

        }
        #endregion

        #region Export Excel
        public ActionResult Export_Template(string type = "")
        {
            if (type == "patchdata")
            {
                string filename = GetExcelName("Template_PatchData");

                var bytes = Generate_Template<SubmitFOAEquipment>(filename, type);

                return File(bytes, "application/excel", filename + ".xls");
            }
            else
            {
                string filename = GetExcelName("Template_SearchOrder");
                //filename = "Template_SearchOrder";

                var bytes = Generate_Template<SubmitFOAEquipment>(filename, type);

                return File(bytes, "application/excel", filename + ".xls");
            }
        }

        public byte[] Generate_Template<T>(string fileName, string type)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            if (type == "patchdata")
            {
                table.Columns.Add("NO", System.Type.GetType("System.String"));
                table.Columns.Add("INTERNET NO", System.Type.GetType("System.String"));
                table.Columns.Add("SERIAL NUMBER", System.Type.GetType("System.String"));
                table.Columns.Add("FOA CODE", System.Type.GetType("System.String"));
                table.Columns.Add("CREATED DATE", System.Type.GetType("System.String"));
                table.Columns.Add("POSTING DATE", System.Type.GetType("System.String"));
                table.Columns.Add("MOVEMENT TYPE", System.Type.GetType("System.String"));
            }
            else
            {
                table.Columns.Add("SERIAL NUMBER", System.Type.GetType("System.String"));
            }

            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateExcel(table, "WorkSheet", tempPath, fileName, type);
            return data_;
        }

        private byte[] GenerateExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string type)
        {
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
            int iCol = 0;
            if (type == "patchdata")
            {
                iCol = 7;
            }
            else
            {
                iCol = 1;
            }
            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 7, 9]; //[2, 1, 4, 4];
                //rangeReportDetail = worksheet.SelectedRange[]
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 1;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

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

        private byte[] GenerateSearchOrderExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName)
        {
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
            int iCol = 1;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 7, 9]; //[2, 1, 4, 4];
                //rangeReportDetail = worksheet.SelectedRange[]
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 1;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

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

        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }


        #endregion

        #region Import Excel Search By File
        public ActionResult SearchByFile_Save(HttpPostedFileBase Searchbyfile)    //HttpPostedFileBase
        {
            if (Searchbyfile != null)
            {
                try
                {
                    if (Path.GetExtension(Searchbyfile.FileName).ToLower() == ".xls" || Path.GetExtension(Searchbyfile.FileName).ToLower() == ".xlsx")
                    {
                        GetSNTableFromExcel(Searchbyfile);
                    }
                    else
                    {
                        var modelResponse = new { status = false, message = "Please upload .xls .xlxs file extension", fileName = Searchbyfile.FileName };
                        return Json(modelResponse, "text/plain");
                    }
                }
                catch (Exception e)
                {
                    var modelResponse = new { status = false, message = e.GetErrorMessage() };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }

        public ActionResult SearchByFile_Remove(string[] Searchbyfile)
        {
            if (Searchbyfile != null)
            {
                try
                {
                    SubmitFOAByFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        private void GetSNTableFromExcel(HttpPostedFileBase File)
        {
            DataTable tbl = new DataTable();
            TempDataTableResendOrder.dt = tbl;
            try
            {
                //   FileInfo File = new FileInfo(path);
                using (var pck = new ExcelPackage(File.InputStream))
                {
                    var workbook = pck.Workbook;
                    var ws = pck.Workbook.Worksheets.First();

                    bool hasHeader = true;
                    TempDataTableResendOrder.dt.Columns.Add("SERIAL NUMBER");

                    var startRow = hasHeader ? 2 : 1;
                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, 1];
                        var row = TempDataTableResendOrder.dt.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                        TempDataTableResendOrder.dt.Rows.Add(row);
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
        private List<WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel> ConvertSNTableToList()
        {
            List<WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel> Datafile = new List<WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel>();
            // string p_UserName = base.CurrentUser.UserName.ToString();
            try
            {
                int i = 1;
                foreach (DataRow row in TempDataTableResendOrder.dt.Rows)
                {
                    if (row.ItemArray[0].ToSafeString() != "")
                    {
                        try
                        {
                            var model = new WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel
                            {
                                SERIAL_NUMBER = row.ItemArray[0].ToSafeString().Trim().Replace(",", "").Replace("/", "").Replace("\\", "")
                            };
                            Datafile.Add(model);
                        }
                        catch (Exception ex)
                        {
                            _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                        }
                    }
                }
                // Clear data 
                DataTable tbl = new DataTable();
                TempDataTableResendOrder.dt = tbl;
                // Clear
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                return null;
            }

        }
        public ActionResult Binding_SN_From_Excel()
        {
            //ส่งไปหา DataList status ใน pk หลังจากนั้น count status และ assign list ใหม่
            List<PatchCPEModel> Data = ConvertSNTableToList();
            try
            {
                var process_hvr = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
                
                if(process_hvr.DISPLAY_VAL == "Y")
                {
                    var query = new PatchSearchOrderHVRQuery
                    {
                        SerialNo = Data.Select(x => x.SERIAL_NUMBER).ToList()
                    };
                    var result = _queryProcessor.Execute(query);
                    if (result != null)
                    {
                        Session["SNList"] = result;
                        int row = 1;
                        foreach (var item in result)
                        {
                            item.NO = row;
                            row++;
                        }
                        return Json(result);
                    }
                    else
                    {
                        return Json("");
                    }
                }
                else
                {
                    var query = new PatchSearchOrderQuery
                    {
                        SerialNo = Data.Select(x => x.SERIAL_NUMBER).ToList()
                    };
                    var result = _queryProcessor.Execute(query);
                    if (result != null)
                    {
                        Session["SNList"] = result;
                        int row = 1;
                        foreach (var item in result)
                        {
                            item.NO = row;
                            row++;
                        }
                        return Json(result);
                    }
                    else
                    {
                        return Json("");
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.Info("ERROR : Binding_SN_From_Excel " + ex.ToSafeString());
                return Json(new { ret_code = 1, ret_msg = "Error. Please contact System Admin" });
            }
        }
        #endregion

        #region Import Excel Patch By File
        public ActionResult PatchByFile_Save(HttpPostedFileBase Patchbyfile)    //HttpPostedFileBase
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            if (Patchbyfile != null)
            {
                var find_filename = Patchbyfile.FileName.ToSafeString().Split('\\');
                try
                {
                    if (Path.GetExtension(Patchbyfile.FileName).ToLower() == ".xls" || Path.GetExtension(Patchbyfile.FileName).ToLower() == ".xlsx")
                    {
                        //Check Duplicate File Name.
                        var patchEquipment = new PatchEquipmentQuery()
                        {
                            FileName = find_filename.Last(),
                            PatchStatus = "ALL",
                            SerialNo = "ALL",
                            InternetNo = "ALL",
                            Flag = "INSERT"
                        };
                        List<RetPatchEquipment> retPatchEquipment = _queryProcessor.Execute(patchEquipment);
                        if (retPatchEquipment.Count() == 0)
                        {
                            GetDataTableFromExcel(Patchbyfile);
                            Session["FileName"] = find_filename.Last();
                        }
                        else
                        {
                            var modelResponse = new { status = false, message = "มีไฟล์ " + find_filename.Last() + " แล้ว กรุณาเปลี่ยนชื่อไฟล์" };
                            return Json(modelResponse, "text/plain");
                        }
                    }
                    else
                    {
                        var modelResponse = new { status = false, message = "Please upload .xls .xlxs file extension", fileName = find_filename.Last() };
                        return Json(modelResponse, "text/plain");
                    }
                }
                catch (Exception e)
                {
                    var modelResponse = new { status = false, message = e.GetErrorMessage() };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }

        public ActionResult PatchByFile_Remove(string[] Patchbyfile)
        {
            if (Patchbyfile != null)
            {
                try
                {
                    SubmitFOAByFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        private void GetDataTableFromExcel(HttpPostedFileBase File)
        {
            DataTable tbl = new DataTable();
            TempDataTableResendOrder.dt = tbl;
            try
            {
                //   FileInfo File = new FileInfo(path);
                using (var pck = new ExcelPackage(File.InputStream))
                {
                    var workbook = pck.Workbook;
                    var ws = pck.Workbook.Worksheets.First();

                    bool hasHeader = true;
                    TempDataTableResendOrder.dt.Columns.Add("NO");
                    TempDataTableResendOrder.dt.Columns.Add("INTERNET NO");
                    TempDataTableResendOrder.dt.Columns.Add("SERIAL NUMBER");
                    TempDataTableResendOrder.dt.Columns.Add("SERIAL STATUS");
                    TempDataTableResendOrder.dt.Columns.Add("FOA CODE");
                    TempDataTableResendOrder.dt.Columns.Add("CREATED DATE");
                    TempDataTableResendOrder.dt.Columns.Add("POSTING DATE");
                    TempDataTableResendOrder.dt.Columns.Add("MOVEMENT TYPE");
                    TempDataTableResendOrder.dt.Columns.Add("REMARK");

                    var startRow = hasHeader ? 2 : 1;
                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, 9];
                        var row = TempDataTableResendOrder.dt.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }

                        if (row.ItemArray[0].ToSafeString().Trim() != "" ||
                            row.ItemArray[1].ToSafeString().Trim() != "" ||
                            row.ItemArray[2].ToSafeString().Trim() != "" ||
                            row.ItemArray[3].ToSafeString().Trim() != "" ||
                            row.ItemArray[4].ToSafeString().Trim() != "" ||
                            row.ItemArray[5].ToSafeString().Trim() != "")
                        {
                            TempDataTableResendOrder.dt.Rows.Add(row);
                        }
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

        private List<WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel> ConvertDataTableToList()
        {
            List<WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel> Datafile = new List<WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel>();
            // string p_UserName = base.CurrentUser.UserName.ToString();
            try
            {
                int i = 1;
                foreach (DataRow row in TempDataTableResendOrder.dt.Rows)
                {
                    try
                    {
                        var model = new WBBEntity.PanelModels.FBBWebConfigModels.PatchCPEModel
                        {
                            NO = i.ToString(),
                            INTERNET_NO = row.ItemArray[1].ToSafeString(),
                            SERIAL_NUMBER = row.ItemArray[2].ToSafeString(),
                            //SERIAL_STATUS = row.ItemArray[3].ToSafeString(),
                            FOA_CODE = row.ItemArray[3].ToSafeString(),
                            CREATE_DATE = row.ItemArray[4].ToSafeString(),
                            POST_DATE = row.ItemArray[5].ToSafeString(),
                            MOVEMENT_TYPE = row.ItemArray[6].ToSafeString()
                        };
                        Datafile.Add(model);
                    }
                    catch (Exception ex)
                    {
                        _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                    }

                    i++;
                }
                // Clear data 
                DataTable tbl = new DataTable();
                TempDataTableResendOrder.dt = tbl;
                // Clear
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                return null;
            }

        }

        public ActionResult Binding_Data_From_Excel([DataSourceRequest] DataSourceRequest request)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            //List<RetPatchEquipment> Lists = new List<RetPatchEquipment>();
            List<PatchCPEModel> ResultDataList = new List<PatchCPEModel>();
            //ส่งไปหา DataList status ใน pk หลังจากนั้น count status และ assign list ใหม่
            List<PatchCPEModel> Data = ConvertDataTableToList();
            var LOOP_QUERY_SPLX = Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "LOOP_QUERY_SPLX").FirstOrDefault();
            int PageSize = LOOP_QUERY_SPLX == null ? 1 : LOOP_QUERY_SPLX.VAL1.ToSafeInteger();
            var Page = (Data.Count() / PageSize);
            int rows = 1;


            var process_hvr = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
            for (int i = 0; i <= Page; i++)
            {
                var task = Task.Run(async delegate
                {
                    try
                    {
                        List<RetCheckSerialStatus> data = new List<RetCheckSerialStatus>();
                        var ListTake = Data.Skip(i * PageSize).Take(PageSize).ToArray();

                        if (process_hvr.DISPLAY_VAL == "Y")
                        {
                            var chkserialstatus = new CheckSerialStatusHVRQuery();
                            chkserialstatus.checkSerials = (from item in ListTake
                                                            select new CheckSerialStatus()
                                                            {
                                                                SERIAL_NUMBER = item.SERIAL_NUMBER,
                                                                INTERNET_NO = item.INTERNET_NO,
                                                                FOA_CODE = item.FOA_CODE,
                                                                LOCATION_CODE = item.LOCATION_CODE,
                                                                SERIAL_STATUS = item.SERIAL_STATUS,
                                                                MOVEMENT_TYPE = item.MOVEMENT_TYPE,
                                                                POST_DATE = item.POST_DATE,
                                                                CREATE_DATE = item.CREATE_DATE
                                                            }).ToList();
                            data = _queryProcessor.Execute(chkserialstatus);
                        }
                        else
                        {
                             var chkserialstatus = new CheckSerialStatusQuery();
                            chkserialstatus.checkSerials = (from item in ListTake
                                                            select new CheckSerialStatus()
                                                            {
                                                                SERIAL_NUMBER = item.SERIAL_NUMBER,
                                                                INTERNET_NO = item.INTERNET_NO,
                                                                FOA_CODE = item.FOA_CODE,
                                                                LOCATION_CODE = item.LOCATION_CODE,
                                                                SERIAL_STATUS = item.SERIAL_STATUS,
                                                                MOVEMENT_TYPE = item.MOVEMENT_TYPE,
                                                                POST_DATE = item.POST_DATE,
                                                                CREATE_DATE = item.CREATE_DATE
                                                            }).ToList();
                             data = _queryProcessor.Execute(chkserialstatus);

                        }

                        foreach (var item in (from a in data
                                              select new PatchCPEModel()
                                              {
                                                  INTERNET_NO = a.INTERNET_NO,
                                                  SERIAL_NUMBER = a.SN,
                                                  CREATE_DATE = a.CREATED_DATE,
                                                  FOA_CODE = a.FOA_CODE,
                                                  MOVEMENT_TYPE = a.MOVEMENT_TYPE,
                                                  SERIAL_STATUS = a.STATUS,
                                                  POST_DATE = a.POSTING_DATE,
                                                  REMARK = a.REMARK
                                              }).ToList())
                        {
                            item.NO = rows.ToString();
                            rows++;
                            ResultDataList.Add(item);
                        }

                    }
                    catch (Exception ex)
                    {
                        _Logger.Info("ERROR : Binding_Data_From_Excel "+ex.ToSafeString());
                    }
                    await Task.Delay(TimeSpan.FromSeconds(0));
                });
                task.Wait(60000);

            }

            Session["SN_Status"] = ResultDataList.Count(x => x.SERIAL_STATUS != "Available" && x.SERIAL_STATUS != "In Service");
            Session["DataList"] = ResultDataList;
            return Json(new { data = ResultDataList, SN_Status = ResultDataList.Count(x => x.SERIAL_STATUS != "Available" && x.SERIAL_STATUS != "In Service"), Filename = Session["FileName"].ToSafeString() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Binding_Detail_Data()
        {
            return Json(new { SN_Status = Session["SN_Status"], Filename = Session["FileName"].ToSafeString() });
        }

        #endregion

        #region Export Patch To Excel
        public ActionResult Export_SN_Report()
        {
            string filename = GetExcelName("FBBPAYG_Query_Serial");
            var process_hvr = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
            if (process_hvr.DISPLAY_VAL == "Y")
            {
                List<PatchSearchOrdersHVR> Models = (List<PatchSearchOrdersHVR>)Session["SNList"];
                var bytes = Generate_SN_Report<PatchSearchOrdersHVR>(Models, filename, "FBBPAYG_PATCHCPE");
                return File(bytes, "application/excel", filename + ".xls");
            }
            else
            {
                List<RetPatchSearchOrders> Models = (List<RetPatchSearchOrders>)Session["SNList"];
                var bytes = Generate_SN_Report<RetPatchSearchOrders>(Models, filename, "FBBPAYG_PATCHCPE");
                return File(bytes, "application/excel", filename + ".xls");
            }
        }

        public byte[] Generate_SN_Report<T>(List<T> data, string fileName, string LovValue5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            table.Columns.Add("NO", System.Type.GetType("System.String"));
            table.Columns.Add("SERIAL NUMBER", System.Type.GetType("System.String"));
            table.Columns.Add("INTERNET NO", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER NO", System.Type.GetType("System.String"));
            table.Columns.Add("TECHNOLOGY", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER TYPE", System.Type.GetType("System.String"));
            table.Columns.Add("SYMPTOM GROUP", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER DATE", System.Type.GetType("System.String"));
            table.Columns.Add("SERIAL STATUS", System.Type.GetType("System.String"));
            table.Columns.Add("LAST UPDATE", System.Type.GetType("System.String"));
            object[] values = new object[table.Columns.Count];
            if (data != null)
            {
                foreach (T item in data)
                {
                    values[0] = props["NO"].GetValue(item);
                    values[1] = props["SN"].GetValue(item);
                    values[2] = props["INTERNET_NO"].GetValue(item);
                    values[3] = props["ORDER_NO"].GetValue(item);
                    values[4] = props["TECHNOLOGY"].GetValue(item);
                    values[5] = props["ORDER_TYPE"].GetValue(item);
                    values[6] = props["SYMPTOM_GROUP"].GetValue(item);
                    values[7] = props["ORDER_DATE"].GetValue(item);
                    values[8] = props["STATUS"].GetValue(item);
                    values[9] = props["LAST_UPDATE_DATE"].GetValue(item);
                    table.Rows.Add(values);
                }
            }

            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateSNExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        private byte[] GenerateSNExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
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
            int iCol = 10;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                iRow = 1;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

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
        #endregion

        #region Export Patch To Excel
        public ActionResult Export_Patch_Report()
        {
            string filename = GetExcelName("FBBPAYG_PATCHCPE");
            List<PatchCPEModel> Models = (List<PatchCPEModel>)Session["DataList"];
            var bytes = Generate_Patch_Report<PatchCPEModel>(Models, filename, "FBBPAYG_PATCHCPE");
            return File(bytes, "application/excel", filename + ".xls");
        }

        public byte[] Generate_Patch_Report<T>(List<T> data, string fileName, string LovValue5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            //ACCESS NO	ORDER NUMBER	SUBCONTRACTOR CODE	SUBCONTRACTOR NAME	PRODUCT NAME	ORDER TYPE	SUBMIT DATE	
            //SERIAL NUMBER	MATERIAL CODE	COMPANY CODE	PLANT	STORAGE LOCATION	SN PATTERN MOVEMENT 
            //ERROR CODE ERROR MESSAGE
            table.Columns.Add("NO", System.Type.GetType("System.String"));
            table.Columns.Add("INTERNET NO", System.Type.GetType("System.String"));
            table.Columns.Add("SERIAL NUMBER", System.Type.GetType("System.String"));
            table.Columns.Add("SERIAL STATUS", System.Type.GetType("System.String"));
            table.Columns.Add("FOA CODE", System.Type.GetType("System.String"));
            table.Columns.Add("CREATED DATE", System.Type.GetType("System.String"));
            table.Columns.Add("POSTING DATE", System.Type.GetType("System.String"));
            table.Columns.Add("MOVEMENT TYPE", System.Type.GetType("System.String"));
            table.Columns.Add("REMARK", System.Type.GetType("System.String"));
            object[] values = new object[table.Columns.Count];
            foreach (T item in data)
            {
                values[0] = props["NO"].GetValue(item);
                values[1] = props["INTERNET_NO"].GetValue(item);
                values[2] = props["SERIAL_NUMBER"].GetValue(item);
                values[3] = props["SERIAL_STATUS"].GetValue(item);
                values[4] = props["FOA_CODE"].GetValue(item);
                values[5] = props["CREATE_DATE"].GetValue(item);
                values[6] = props["POST_DATE"].GetValue(item);
                values[7] = props["MOVEMENT_TYPE"].GetValue(item);
                values[8] = props["REMARK"].GetValue(item);
                table.Rows.Add(values);
            }
            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateequipExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        private byte[] GenerateequipExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
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
            int iCol = 9;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                //worksheet.Cells["A2:G2"].Merge = true;
                //worksheet.Cells["A2,G2"].LoadFromText("Report Name  : EQUIPMENT");

                //worksheet.Cells["A3:D3"].Merge = true;
                //worksheet.Cells["A3,D3"].LoadFromText("Access No : " + searchModel.internetNo);
                //worksheet.Cells["E3:H3"].Merge = true;
                //worksheet.Cells["E3,H3"].LoadFromText("Order Number : " + searchModel.orderNo);
                //worksheet.Cells["I3:L3"].Merge = true;
                //worksheet.Cells["I3,L3"].LoadFromText("Status : " + searchModel.status);

                //worksheet.Cells["A4:D4"].Merge = true;
                //worksheet.Cells["A4,D4"].LoadFromText("Product Name : " + searchModel.productName);
                //worksheet.Cells["E4:H4"].Merge = true;
                //worksheet.Cells["E4,H4"].LoadFromText("Order Type : " + searchModel.orderType);
                //worksheet.Cells["I4:L4"].Merge = true;
                //worksheet.Cells["I4,L4"].LoadFromText("Company Code : " + searchModel.companyCode);

                //worksheet.Cells["A5:D5"].Merge = true;
                //worksheet.Cells["A5,D5"].LoadFromText("Service Name : " + searchModel.serviceName);
                //worksheet.Cells["E5:H5"].Merge = true;
                //worksheet.Cells["E5,H5"].LoadFromText("Subcontractor Code : " + searchModel.subcontractorCode);
                //worksheet.Cells["I5:L5"].Merge = true;
                //worksheet.Cells["I5,L5"].LoadFromText("Plant : " + searchModel.plant);

                //worksheet.Cells["A6:D6"].Merge = true;
                //worksheet.Cells["A6,D6"].LoadFromText("Material Code : " + searchModel.materialCode);
                //worksheet.Cells["E6:H6"].Merge = true;
                //worksheet.Cells["E6,H6"].LoadFromText("Storage Location : " + searchModel.storLocation);

                //worksheet.Cells["A7:D7"].Merge = true;
                //worksheet.Cells["A7,D7"].LoadFromText("Order Create From : " + searchModel.dateFrom);
                //worksheet.Cells["E7:H7"].Merge = true;
                //worksheet.Cells["E7,H7"].LoadFromText("Order Create To : " + searchModel.dateTo);
                //rangeReportDetail = worksheet.SelectedRange[2, 1, 7, 9]; //[2, 1, 4, 4];
                ////rangeReportDetail = worksheet.SelectedRange[]
                //rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                //rangeReportDetail.Style.Font.Bold = true;
                //rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 1;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

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
        #endregion

        #region Patch Data
        public ActionResult Binding_Data_Search(PatchEquipmentQuery DataCriteria)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            var patchEquipment = new PatchEquipmentQuery()
            {
                FileName = DataCriteria.FileName,
                InternetNo = DataCriteria.InternetNo,
                SerialNo = DataCriteria.SerialNo,
                PatchStatus = DataCriteria.PatchStatus,
                CreateDateFrom = DataCriteria.CreateDateFrom.ToSafeString().Replace("/", ""),
                CreateDateTo = DataCriteria.CreateDateTo.ToSafeString().Replace("/", ""),
                Flag = DataCriteria.Flag
            };
            List<RetPatchEquipment> Data = _queryProcessor.Execute(patchEquipment);

            int no = 1;
            List<RetPatchEquipment> list = new List<RetPatchEquipment>();

            for (int i = 0; i < Data.Count; i++)
            {
                RetPatchEquipment item = Data[i];
                item.NO = no;
                no++;
                list.Add(item);
            }
            Session["FileName"] = null;
            Session["DataCheckbox"] = null;
            Session["DataList"] = Data;
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertDataByFile(string _email = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            try
            {
                var ret_code = "";
                var ret_msg = "";
                var test = Session["DataList"];
                var test2 = ConvertListAny<PatchCPEModel>(Session["DataList"]);
                var Data = (List<PatchCPEModel>)Session["DataList"];
                var LOOP_INSERT_PAYG = Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "LOOP_INSERT_PAYG").FirstOrDefault();
                int PageSize = LOOP_INSERT_PAYG == null ? 1 : LOOP_INSERT_PAYG.VAL1.ToSafeInteger();
                var Page = (Data.Count() / PageSize);
                int rows = 1;
                for (int i = 0; i <= Page; i++)
                {
                    List<WBBContract.Commands.FBBWebConfigCommands.PatchCPEModel> List = new List<WBBContract.Commands.FBBWebConfigCommands.PatchCPEModel>();
                    var task = Task.Run(async delegate
                    {
                        try
                        {
                            var ListTake = Data.Skip(i * PageSize).Take(PageSize).ToArray();
                            foreach (var data in ListTake)
                            {
                                var attr = new WBBContract.Commands.FBBWebConfigCommands.PatchCPEModel()
                                {
                                    FILE_NAME = Session["FileName"].ToSafeString(),
                                    INTERNET_NO = data.INTERNET_NO,
                                    SERIAL_NO = data.SERIAL_NUMBER,
                                    SERIAL_STATUS = data.SERIAL_STATUS,
                                    FOA_CODE = data.FOA_CODE,
                                    SUBMIT_DATE = data.CREATE_DATE,
                                    POST_DATE = data.POST_DATE,
                                    MOVEMENT_TYPE = data.MOVEMENT_TYPE,
                                    CREATE_BY = base.CurrentUser.UserName,
                                    REMARK = data.REMARK,

                                };
                                List.Add(attr);
                            }
                            var command = new FBBPaygPatchDataInsertCommand
                            {
                                p_Product_List = List
                            };
                            _insertCommand.Handle(command);
                            ret_code = command.returnCode.ToSafeString();
                            ret_msg = command.returnMsg.ToSafeString();
                        }
                        catch (Exception ex)
                        {
                            _Logger.Info(ex.ToSafeString());
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    });
                    task.Wait(60000);
                }

                //SendMail
                var commandSendmail = new FBBPaygPatchDataInsertSendMailCommand
                {
                    FILE_NAME = Session["FileName"].ToSafeString(),
                    EMAIL = _email,
                    USER_NAME = base.CurrentUser.UserName,
                    ACTIVE_FLAG = "Y"
                };
                _SendMailCommand.Handle(commandSendmail);

                var patchEquipment = new PatchEquipmentQuery()
                {
                    FileName = Session["FileName"].ToSafeString(),
                    InternetNo = "ALL",
                    SerialNo = "ALL",
                    PatchStatus = "ALL",
                    //CreateDateFrom = DataCriteria.CreateDateFrom.ToSafeString().Replace("/", ""),
                    //CreateDateTo = DataCriteria.CreateDateTo.ToSafeString().Replace("/", ""),
                    Flag = "INSERT"
                };
                List<RetPatchEquipment> sum_result = _queryProcessor.Execute(patchEquipment);
                var no = 1;
                for (int i = 0; i < Data.Count; i++)
                {
                    sum_result[i].NO = no;
                    no++;
                }

                Session["DataCheckbox"] = null;
                Session["DataList"] = sum_result;
                string alert_msg = "Pending : " + sum_result.Count(x => x.PATCH_STATUS.ToUpper() == "PENDING") + " Rows, PendingNewRegis : " + sum_result.Count(x => x.PATCH_STATUS.ToUpper() == "PENDING_NEWREGIST") + " Rows, Cancel : " + sum_result.Count(x => x.PATCH_STATUS.ToUpper() == "CANCEL") + " Rows. Total Data : " + Data.Count() + " Rows.";
                return Json(new { Data = sum_result, ret_code = ret_code, ret_msg = alert_msg });
            }
            catch (Exception ex)
            {
                return Json(new { ret_code = 1, ret_msg = "Error. Please contact System Admin" });
            }

        }

        public static object ConvertListAny<T>(object data)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            return data;
        }

        public ActionResult UpdateSNError(PatchEquipmentQuery DataModel)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            try
            {
                var ret_code = "";
                var ret_msg = "";
                var command = new FBBPaygPatchDataUpdateCommand
                {
                    serialno = DataModel.SerialNo,
                    snstatus = DataModel.SerialStatus,
                    patchstatus = DataModel.PatchStatus,
                    FileName = DataModel.FileName,
                    CREATE_BY = base.CurrentUser.UserName
                };
                _updateCommand.Handle(command);
                ret_code = command.returnCode.ToSafeString();
                ret_msg = command.returnMsg.ToSafeString();
                //Success Update [File name : fbb_scan_serial_20210709.xlsx , SN : HG2021062102 , STATUS : COMPLETE , REMARK : ]
                var msg = ret_msg.Replace(" [", "<br>").Replace(",", "<br>").Replace("]", "");
                return Json(new { ret_code = ret_code, ret_msg = msg });
            }
            catch (Exception ex)
            {
                return Json(new { ret_code = 1, ret_msg = "Error. Please contact System Admin" });
            }

        }
        public ActionResult UpdateSNErrorList(string DataModel)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            try
            {
                var deserializedDataModel = JsonConvert.DeserializeObject<List<RetPatchEquipment>>(DataModel);
                var ret_code = "";
                var ret_msg = "";
                int SuccessTotal = 0;
                int ErrorTotal = 0;
                if (deserializedDataModel != null)
                {
                    var PageSize = 1000;
                    var Page = (deserializedDataModel.Count() / PageSize);
                    for (int i = 0; i <= Page; i++)
                    {
                        List<WBBContract.Commands.FBBWebConfigCommands.PatchCPEModel> List = new List<WBBContract.Commands.FBBWebConfigCommands.PatchCPEModel>();
                        var task = Task.Run(async delegate
                        {
                            try
                            {
                                var ListTake = deserializedDataModel.Skip(i * PageSize).Take(PageSize).ToArray();
                                var chkserialstatus = new List<FBBPaygPatchDataListUpdateCommand>();
                                chkserialstatus = (from item in ListTake
                                                   select new FBBPaygPatchDataListUpdateCommand()
                                                   {
                                                       SERIAL_NO = item.SERIAL_NO,
                                                       FILE_NAME = item.FILE_NAME,
                                                       PATCH_STATUS = item.PATCH_STATUS,
                                                       CREATE_BY = base.CurrentUser.UserName
                                                   }).ToList();
                                var command = new FBBPaygPatchDataListUpdateCommand
                                {
                                    p_Product_List = chkserialstatus
                                };
                                _updateListCommand.Handle(command);

                                if (command.ret_code == 0)
                                {
                                    SuccessTotal += (command.ret_msg.ToSafeInteger());
                                    ErrorTotal += chkserialstatus.Count() - command.ret_msg.ToSafeInteger();
                                }
                            }
                            catch (Exception ex)
                            {
                                _Logger.Info(ex.ToSafeString());
                            }
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        });
                        task.Wait(60000);
                    }


                }

                ret_msg = $"Success : {SuccessTotal} Records. <br>Fail : {ErrorTotal} Records. <br>Total : {deserializedDataModel.Count()} Records.";

                return Json(new { ret_code = 0, ret_msg = ret_msg });
            }
            catch (Exception ex)
            {
                return Json(new { ret_code = 1, ret_msg = "Error. Please contact System Admin" });
            }

        }
        #endregion

        public ActionResult Bind_Data_Session(PatchEquipmentQuery DataCriteria)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            var patchEquipment = new PatchEquipmentQuery()
            {
                FileName = Session["FileName"].ToSafeString() == "" ? DataCriteria.FileName : Session["FileName"].ToSafeString(),
                InternetNo = DataCriteria.InternetNo,
                SerialNo = DataCriteria.SerialNo,
                PatchStatus = DataCriteria.PatchStatus,
                CreateDateFrom = DataCriteria.CreateDateFrom.ToSafeString().Replace("/", ""),
                CreateDateTo = DataCriteria.CreateDateTo.ToSafeString().Replace("/", ""),
                Flag = DataCriteria.Flag
            };
            List<RetPatchEquipment> Data = _queryProcessor.Execute(patchEquipment);

            Session["DataList"] = Data;
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Data_SNList(string check = "", string SN = "", string checkAll_was_check = "", string filename = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            List<RetPatchEquipment> DataCheckbox = new List<RetPatchEquipment>();
            List<RetPatchEquipment> list = (List<RetPatchEquipment>)Session["DataList"]; //list หลักได้จากการ search
            var DataCancel = list.Where(x => x.PATCH_STATUS == "CANCEL"); //list หลักได้จากการ search เอาแต่ status == cancel
            var lenght = DataCancel.Count();
            var str = SN.Split('|');
            string file_name = "";
            if (str.Length != 0)
            {
                SN = str.First();
                file_name = str.Last();

            }

            if (check == "true" && SN == "ALL")
            {
                //checkall == true
                Session["DataCheckbox"] = null;
                return Json(new { Data = DataCancel, isFull = true }, JsonRequestBehavior.AllowGet);
            }
            else if (check == "false" && SN == "ALL")
            {
                Session["DataCheckbox"] = null;
                return Json(new { Data = new List<RetPatchEquipment>(), isFull = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (checkAll_was_check == "true")
                {
                    if (check == "true")
                    {
                        if (Session["DataCheckbox"] == null)
                        {
                            DataCheckbox = (List<RetPatchEquipment>)Session["DataCheckbox"];

                            if (DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name) != null)
                            {
                                DataCheckbox.Add(DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name));
                            }
                        }
                        else
                        {
                            DataCheckbox = (List<RetPatchEquipment>)Session["DataCheckbox"];

                            if (DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name) != null)
                            {
                                DataCheckbox.Add(DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name));
                            }
                        }
                    }
                    else
                    {
                        if (Session["DataCheckbox"] == null)
                        {
                            DataCheckbox = (List<RetPatchEquipment>)Session["DataList"];
                            var remove_val = DataCheckbox.Where(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name).ToList();
                            DataCheckbox.Remove(remove_val.First());
                        }
                        else
                        {
                            DataCheckbox = (List<RetPatchEquipment>)Session["DataCheckbox"];
                            var remove_val = DataCheckbox.Where(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name).ToList();
                            DataCheckbox.Remove(remove_val.First());
                        }
                    }

                    Session["DataCheckbox"] = DataCheckbox;
                    return Json(new { Data = DataCheckbox, isFull = (DataCheckbox.Count() - lenght == 0 ? true : false) }, JsonRequestBehavior.AllowGet);
                }
                else if (checkAll_was_check == "false")
                {
                    //ให้เพิ่มหรือลบทำเป็น list ใหม่
                    if (check == "true")
                    {
                        //เพิ่มเข้าไปใน list
                        if (Session["DataCheckbox"] == null)
                        {
                            DataCheckbox = new List<RetPatchEquipment>();
                            if (DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name) != null)
                            {
                                DataCheckbox.Add(DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name));
                            }
                        }
                        else
                        {
                            DataCheckbox = (List<RetPatchEquipment>)Session["DataCheckbox"];

                            if (DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name) != null)
                            {
                                DataCheckbox.Add(DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name));
                            }
                        }
                    }
                    else
                    {
                        //ลบออกจาก list 
                        if (Session["DataCheckbox"] == null)
                        {
                            if (DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name) != null)
                            {
                                DataCheckbox.Add(DataCancel.FirstOrDefault(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name));
                            }
                        }
                        else
                        {
                            DataCheckbox = (List<RetPatchEquipment>)Session["DataCheckbox"];
                            var remove_val = DataCheckbox.Where(x => x.SERIAL_NO == SN && x.FILE_NAME == file_name).ToList();
                            DataCheckbox.Remove(remove_val.First());
                        }
                    }
                    Session["DataCheckbox"] = DataCheckbox;
                    return Json(new { Data = DataCheckbox, isFull = (DataCheckbox.Count() - lenght == 0 ? true : false) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //checkall == false
                    return Json(new { Data = new List<RetPatchEquipment>(), isFull = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public IEnumerable<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }

    }
}
