using Excel;
using FBBConfig.Extensions;
using FBBConfig.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{

    public class ResendOrderBulkCorpController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        //private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public ResendOrderBulkCorpController(ILogger logger,
                IQueryProcessor queryProcessor
            //,
            //  ICommandHandler<InterfaceLogCommand> intfLogCommand
            )
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            // _intfLogCommand = intfLogCommand;

        }

        public ActionResult ResendOrder()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBBULK001");

            return View();
        }

        //set lov screen
        private void SetViewBagLov(string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        public JsonResult GetDataResendOrder(string Bulk_No)
        {
            _Logger.Info("Start Search ResendOrder");
            ResendOrderBulkCorpModel data;
            var QueryGetdata = new GetDataResendOrderBulkCorpQuery
            {
                p_bulk_number = Bulk_No
            };

            data = _queryProcessor.Execute(QueryGetdata);


            if (null != data)
            {
                _Logger.Info("Search Bulk Corp Existing Account Data: " + "\r\n" +
                        "Bulk Number : " + QueryGetdata.p_bulk_number + "\r\n" +
                        "Message : " + data.p_message[0].p_message.ToSafeString() + "\r\n" +
                        "Technology : " + data.p_resend_tech[0].TYPE_TECHNOLOGY.ToSafeString() + "\r\n" +
                        "Technology : " + data.p_resend_tech[0].ADDRESS_ID.ToSafeString() + "\r\n" +
                        "Event Code : " + data.p_resend_tech[0].EVENT_CODE.ToSafeString() + "\r\n" +
                        "First Name : " + data.p_resend_tech[0].FIRST_NAME.ToSafeString() + "\r\n" +
                        "Last Name : " + data.p_resend_tech[0].LAST_NAME.ToSafeString() + "\r\n" +
                        "Main Mobile : " + data.p_resend_tech[0].MAIN_MOBILE.ToSafeString() + "\r\n" +
                        "Main Phone : " + data.p_resend_tech[0].MAIN_PHONE.ToSafeString() + "\r\n" +
                        "Email : " + data.p_resend_tech[0].EMAIL.ToSafeString() + "\r\n" +
                        "Package Main Code : " + data.p_package_main[0].PACKAGE_MAIN_CODE.ToSafeString() + "\r\n" +
                        "Package Main Name : " + data.p_package_main[0].PACKAGE_MAIN_NAME.ToSafeString() + "\r\n" +
                        "Package Main Description : " + data.p_package_main[0].PACKAGE_MAIN_DESCRIP.ToSafeString() + "\r\n" +
                        "Package Ontop Code : " + data.p_package_discount[0].PACKAGE_DISCOUNT_CODE.ToSafeString() + "\r\n" +
                        "Package Ontop Name : " + data.p_package_discount[0].PACKAGE_DISCOUNT_NAME.ToSafeString() + "\r\n" +
                        "Package Ontop Description : " + data.p_package_discount[0].PACKAGE_DISCOUNT_DESCRIP.ToSafeString());

            }

            _Logger.Info("End Search ResendOrder");

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateResendOrder(string BulkNo)
        {
            ResendOrderBulkCorpModel data = new ResendOrderBulkCorpModel();

            var LoginUser = base.CurrentUser;
            if (null == base.CurrentUser)
                return Json("Sesson Time Out", JsonRequestBehavior.AllowGet);

            var returnexcel = new List<Resendreturndata>();
            Resendreturndata retexcel = new Resendreturndata();

            var filename = Session["filename"] as String;
            if (string.IsNullOrEmpty(filename))
            {
                retexcel.p_bulk_number_return = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR ListExcelData is null";
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }

            var filesize = Session["totalsize"] as String;
            if (string.IsNullOrEmpty(filesize))
            {
                retexcel.p_bulk_number_return = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR total size of ListExcelData is null";
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }

            var filetotalrow = Session["totalrow"] as String;
            if (string.IsNullOrEmpty(filetotalrow))
            {
                retexcel.p_bulk_number_return = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR total row of ListExcelData is null";
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }

            #region Read Excel from NAS
            List<BulkExcelDataResend> ListExcelData = new List<BulkExcelDataResend>();
            byte[] excelFiles;
            IExcelDataReader iExcelDataReader = null;
            DataTable res = null;
            DataSet dataSet = new DataSet();
            FileStream stream = null;
            try
            {
                var ExcelName = filename;
                if (ExcelName.EndsWith("xls") || ExcelName.EndsWith("xlsx"))
                {
                    //////
                    try
                    {
                        var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate").SingleOrDefault();
                        var UploadImageFile = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "UploadImageFile").SingleOrDefault();

                        var imagePath = UploadImageFile.LovValue1;
                        var imagepathimer = @ImpersonateVar.LovValue4;
                        string user = ImpersonateVar.LovValue1;
                        string pass = ImpersonateVar.LovValue2;
                        string ip = ImpersonateVar.LovValue3;


                        string yearweek = (DateTime.Now.Year.ToString());
                        string monthyear = (DateTime.Now.Month.ToString("00"));

                        var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));

                        imagepathimer = imagepathimerTemp;
                        _Logger.Info("Start Impersonate: ");

                        using (var impersonator = new Impersonator(user, ip, pass, false))
                        {
                            excelFiles = System.IO.File.ReadAllBytes(filename);
                            iExcelDataReader = null;
                            stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read);
                        }
                    }
                    catch (Exception ex)
                    {
                        _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                        _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                    }
                    //////

                    var checkexcel = true;
                    if (checkexcel)
                    {
                        if (filename.EndsWith("xls"))
                        {
                            iExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }

                        if (filename.EndsWith("xlsx"))
                        {
                            iExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }

                        iExcelDataReader.IsFirstRowAsColumnNames = true;

                        dataSet = iExcelDataReader.AsDataSet();

                        iExcelDataReader.Close();

                        if (dataSet != null && dataSet.Tables.Count > 0)
                        {
                            res = dataSet.Tables[0];
                        }

                        DataTable table = res;
                        var inttotalrow = table.Rows.Count;
                        if (filetotalrow == inttotalrow.ToSafeString())
                        {

                            foreach (DataRow dRow in table.Rows)
                            {
                                //i++;
                                if (null == dRow["No"].ToString() ||
                                    null == dRow["OrderNumber"].ToString() ||
                                    null == dRow["installAddress1"].ToString() ||
                                    null == dRow["installAddress2"].ToString() ||
                                    null == dRow["installAddress3"].ToString() ||
                                    null == dRow["installAddress4"].ToString() ||
                                    null == dRow["installAddress5"].ToString() ||
                                    null == dRow["latitude"].ToString() ||
                                    null == dRow["longitude"].ToString() ||
                                    null == dRow["install_date"].ToString()
                                    )
                                {
                                    break;
                                }

                                if (dRow["No"].ToString().Equals("")
                                    || dRow["OrderNumber"].ToString().Equals("")
                                    || dRow["installAddress1"].ToString().Equals("")
                                    || dRow["installAddress2"].ToString().Equals("")
                                    || dRow["installAddress3"].ToString().Equals("")
                                    || dRow["installAddress4"].ToString().Equals("")
                                    || dRow["installAddress5"].ToString().Equals("")
                                    || dRow["latitude"].ToString().Equals("")
                                    || dRow["longitude"].ToString().Equals("")
                                    || dRow["install_date"].ToString().Equals("")
                                    )
                                {
                                    break;
                                }

                                var temp = new BulkExcelDataResend();
                                temp.No = dRow["No"].ToString();
                                temp.OrderNumber = dRow["OrderNumber"].ToString();
                                temp.installAddress1 = dRow["installAddress1"].ToString();
                                temp.installAddress2 = dRow["installAddress2"].ToString();
                                temp.installAddress3 = dRow["installAddress3"].ToString();
                                temp.installAddress4 = dRow["installAddress4"].ToString();
                                temp.installAddress5 = dRow["installAddress5"].ToString();
                                temp.latitude = dRow["latitude"].ToString();
                                temp.longitude = dRow["longitude"].ToString();
                                temp.install_date = dRow["install_date"].ToString();

                                ListExcelData.Add(temp);
                            }
                        }
                        stream.Dispose();
                    }
                }

            }
            catch (Exception e)
            {
                if (e.Message.Contains("does not belong to table"))
                {
                    _Logger.Info("Error excel file was missing field format");
                    var modelResponse = new { status = false, message = "This file was missing field format", filename = "" };
                    return Json(modelResponse, "text/plain");
                }
                else
                {
                    _Logger.Info("Error read excel file " + e.GetErrorMessage());
                }

            }
            #endregion

            if (null != ListExcelData)
            {
                var result = new List<BulkInsertExcelResend>();

                // We only care about the file name.
                var fileName = Session["filename"];
                var file_size = Session["totalsize"];

                var tableexcel = ListExcelData;
                var inttotalrow = Session["totalrow"];

                foreach (var DataRow in tableexcel)
                {
                    var temp = new BulkInsertExcelResend();

                    temp.bulk_number = BulkNo.ToSafeString();
                    temp.p_user = LoginUser.UserName.ToSafeString();
                    temp.No = DataRow.No.ToSafeString();
                    temp.OrderNumber = DataRow.OrderNumber.ToSafeString();
                    temp.installAddress1 = DataRow.installAddress1.ToSafeString();
                    temp.installAddress2 = DataRow.installAddress2.ToSafeString();
                    temp.installAddress3 = DataRow.installAddress3.ToSafeString();
                    temp.installAddress4 = DataRow.installAddress4.ToSafeString();
                    temp.installAddress5 = DataRow.installAddress5.ToSafeString();
                    temp.latitude = DataRow.latitude.ToSafeString();
                    temp.longitude = DataRow.longitude.ToSafeString();
                    temp.install_date = DataRow.install_date.ToSafeString();
                    temp.p_file_name = fileName.ToSafeString();

                    result.Add(temp);

                    try
                    {
                        var query = new UpdateResendOrderBulkCorpQuery()
                        {
                            bulk_number = temp.bulk_number,
                            p_user = temp.p_user,
                            p_no = temp.No,
                            p_order_number = temp.OrderNumber,
                            p_installaddress1 = temp.installAddress1,
                            p_installaddress2 = temp.installAddress2,
                            p_installaddress3 = temp.installAddress3,
                            p_installaddress4 = temp.installAddress4,
                            p_installaddress5 = temp.installAddress5,
                            p_latitude = temp.latitude,
                            p_longitude = temp.longitude,
                            p_install_date = temp.install_date,
                            p_file_name = temp.p_file_name

                        };

                        var returnexceltemp = _queryProcessor.Execute(query);

                        retexcel.p_bulk_number_return = returnexceltemp.p_bulk_number_return;
                        retexcel.output_return_code = returnexceltemp.output_return_code;
                        retexcel.output_return_message = returnexceltemp.output_return_message;

                        returnexcel.Add(retexcel);

                    }

                    catch (Exception ex)
                    {
                        _Logger.Info("Error when call RegisterBulkCorpInsertExcelQuery in SaveRegisterPkg " + ex.GetErrorMessage());
                        retexcel.p_bulk_number_return = "";
                        retexcel.output_return_code = "-1";
                        retexcel.output_return_message = "ERROR " + ex.GetErrorMessage();

                        returnexcel.Add(retexcel);
                        return Json(returnexcel, JsonRequestBehavior.AllowGet);
                    }

                }//End foreach


                try
                {
                    var queryp = new SaveUploadFileBulkCorpQuery()
                    {
                        p_bulk_number = BulkNo.ToSafeString(),
                        p_file_name = fileName.ToSafeString()
                    };
                    var datap = _queryProcessor.Execute(queryp);

                }
                catch (Exception ex)
                {
                    _Logger.Info("Error When Call SaveUploadFileBulkCorpQuery for Excel File " + ex.GetErrorMessage());

                    retexcel.p_bulk_number_return = "";
                    retexcel.output_return_code = "-1";
                    retexcel.output_return_message = "ERROR ListExcelData is null";
                    return Json(retexcel, JsonRequestBehavior.AllowGet);
                }

                return Json(returnexcel, JsonRequestBehavior.AllowGet);

            }
            else
            {

                _Logger.Info("Error when get value from Session in SaveRegisterPkg ");
                retexcel.p_bulk_number_return = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR ListExcelData is null";

                //var modelResponse = new { status = false, message = "This file has missing field format", filename = "" };
                //return Json(modelResponse, "text/plain");
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }


        }


        #region Upload Excel file for Resend Bulk Corp

        public ActionResult SaveExcel(IEnumerable<HttpPostedFileBase> files, string cateType, string cardNo,
            string cardType, string register_dv, string MobileNumber, string Bulk_No)
        {

            if (null != Session["totalrow"])
            {
                Session["totalrow"] = "";
            }

            if (null != Session["totalsize"])
            {
                Session["totalsize"] = "";
            }

            var tempupload = Session["Bulktempupload"] as List<BulkExcelDataResend>;
            if (tempupload == null)
                tempupload = new List<BulkExcelDataResend>();

            if (files != null)
            {
                var result = new List<BulkExcelDataResend>();
                try
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path. We only care about the file name.
                        var fileName = Path.GetFileName(file.FileName);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        if (fileName.EndsWith("xls") || fileName.EndsWith("xlsx"))
                        {
                            var checkdup = true;
                            if (checkdup)
                            {
                                var destinationPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                                file.SaveAs(destinationPath);
                                FileInfo fInfo = new FileInfo(destinationPath);
                                long size = fInfo.Length;
                                Session["totalsize"] = size.ToSafeString();

                                if (size > 10000000)
                                {
                                    var modelResponse = new { status = false, message = "File's exceeded", filename = fileName };
                                    return Json(modelResponse, "text/plain");
                                }
                                else
                                {

                                    DataTable table = getDataSet(destinationPath);

                                    table = RemoveAllNullRowsFromDataTable(table);
                                    var inttotalrow = table.Rows.Count;
                                    Session["totalrow"] = inttotalrow.ToSafeString();

                                    var pathimper = UploadExcelBulk("9999999", "9999999999999", "", register_dv, "9999999999", Bulk_No, table);

                                    foreach (DataRow dRow in table.Rows)
                                    {
                                        //i++;
                                        if (null == dRow["No"].ToString())
                                        {
                                            break;
                                        }

                                        if (dRow["No"].ToString().Equals("")
                                            || dRow["OrderNumber"].ToString().Equals("")
                                            || dRow["installAddress1"].ToString().Equals("")
                                            || dRow["installAddress2"].ToString().Equals("")
                                            || dRow["installAddress3"].ToString().Equals("")
                                            || dRow["installAddress4"].ToString().Equals("")
                                            || dRow["installAddress5"].ToString().Equals("")
                                            || dRow["latitude"].ToString().Equals("")
                                            || dRow["longitude"].ToString().Equals("")
                                            || dRow["install_date"].ToString().Equals(""))
                                        {
                                            break;
                                        }

                                    }

                                    if (table.Rows.Count == 0)
                                    {
                                        var modelResponse = new { status = false, message = fileName + " does not complete, please check require field in file.", filename = fileName };
                                        return Json(modelResponse, "text/plain");
                                    }

                                    stopwatch.Stop();
                                    var sds = stopwatch.Elapsed;
                                    Session["filename"] = pathimper.ExcelBulk.FileExcelBulk;

                                }
                            }
                            else
                            {
                                var modelResponse = new { status = false, message = fileName + "is already exist.", filename = fileName };
                                return Json(modelResponse, "text/plain");
                            }
                        }
                        else
                        {
                            var modelResponse = new { status = false, message = "File Format type Error", filename = fileName };
                            return Json(modelResponse, "text/plain");
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("does not belong to table"))
                    {
                        var modelResponse = new { status = false, message = "This file has missing field format", filename = "" };
                        return Json(modelResponse, "text/plain");
                    }
                    else
                    {
                        var modelResponse = new { status = false, message = e.Message, filename = "" };
                        return Json(modelResponse, "text/plain");
                    }
                }

            }

            return Content("");
        }


        public DataTable getDataSet(string path)
        {
            // Get the Excel file and convert to dataset 
            DataTable res = null;
            DataSet dataSet;
            IExcelDataReader iExcelDataReader = null;
            FileStream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read);

            if (path.EndsWith("xls"))
            {
                iExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }

            if (path.EndsWith("xlsx"))
            {
                iExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            stream.Dispose();
            if (iExcelDataReader != null)
            {
                iExcelDataReader.IsFirstRowAsColumnNames = true;

                dataSet = iExcelDataReader.AsDataSet();

                iExcelDataReader.Close();

                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    res = dataSet.Tables[0];
                }
            }
            return res;
        }

        public static DataTable RemoveAllNullRowsFromDataTable(DataTable dt)
        {
            int columnCount = dt.Columns.Count;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                bool allNull = true;
                for (int j = 0; j < columnCount; j++)
                {
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        allNull = false;
                    }
                }
                if (allNull)
                {
                    dt.Rows[i].Delete();
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var i = 0;
            foreach (DataRow drow in dTable.Rows)
            {
                i++;
                if (hTable.Contains(drow[colName]))
                {
                    duplicateList.Add(drow);
                    //dupinfile.Add(i);
                }
                else
                {
                    hTable.Add(drow[colName], string.Empty);
                }
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            stopwatch.Stop();
            var sds = stopwatch.Elapsed;
            return dTable;
        }

        public ActionResult Gettogrid([DataSourceRequest] DataSourceRequest request)
        {
            var tempupload = Session["Bulktempupload"] as List<BulkExcelDataResend>;
            if (tempupload == null)
                tempupload = new List<BulkExcelDataResend>();

            return Json(tempupload.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public RegisterBulkCorpInstallModel UploadExcelBulk(string cateType, string cardNo, string cardType, string register_dv, string MobileNumber,
            string Bulk_No, DataTable dt)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    #region Get IP Address Interface Log (Update 17.2)

                    // Get IP Address
                    string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    #endregion

                    List<string> Arr_files = new List<string>();
                    RegisterBulkCorpInstallModel model = new RegisterBulkCorpInstallModel();
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase[] filesPosted = new HttpPostedFileBase[files.Count];

                    for (int i = 0; i < files.Count; i++)
                    {
                        filesPosted[i] = files[i];
                    }

                    model.Register_device = register_dv;
                    model.account_category = cateType;
                    model.id_card_no = cardNo;
                    model.id_card_type = cardType;

                    model.ca_main_mobile = MobileNumber;
                    model.ClientIP = ipAddress;
                    model.output_bulk_no = Bulk_No;

                    filesPostedRegisterExcelBulkTempStep = filesPosted;
                    model = SaveFileExcel(filesPosted, model, dt);
                    if (null != model.ExcelBulk)
                        //return Json(model.ExcelBulk);
                        return model;
                    else
                        throw new Exception("Null ListExcelFile");
                }
                catch (Exception ex)
                {

                    _Logger.Info("Error Upload Excel RegisterBulkCorp:" + ex.GetErrorMessage());
                    _Logger.Info("Error Upload Excel With Stack Trace : " + ex.RenderExceptionMessage());
                    return new RegisterBulkCorpInstallModel();
                    //return Json(false);
                }
            }
            else
            {
                return new RegisterBulkCorpInstallModel();
                //return Json(false);
            }
        }

        private RegisterBulkCorpInstallModel SaveFileExcel(HttpPostedFileBase[] files, RegisterBulkCorpInstallModel model, DataTable dt)
        {
            string cardNo = model.id_card_no;

            //InterfaceLogCommand log = null;
            //InterfaceLogCommand log2 = null;

            string transactionId = (model.ca_main_mobile + model.ClientIP).ToSafeString();

            //log = StartInterface("IdcardNo:" + cardNo + "\r\n", "SaveFileExcel", transactionId, cardNo, "WEB");

            try
            {
                var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate").SingleOrDefault();
                var UploadImageFile = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "UploadImageFile").SingleOrDefault();

                var imagePath = UploadImageFile.LovValue1;
                var imagepathimer = @ImpersonateVar.LovValue4;
                string user = ImpersonateVar.LovValue1;
                string pass = ImpersonateVar.LovValue2;
                string ip = ImpersonateVar.LovValue3;


                string yearweek = (DateTime.Now.Year.ToString());
                string monthyear = (DateTime.Now.Month.ToString("00"));

                var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));

                //log2 = StartInterface("IdcardNo:" + cardNo + "Path : " + imagepathimerTemp + "\r\n", "Directory Check", transactionId, cardNo, "WEB");
                //if (Directory.Exists(imagepathimerTemp))
                //EndInterface("", log2, transactionId, "Success", "");
                //else
                //{
                //    EndInterface("", log2, cardNo, "ERROR", "Directory Not Found : " + imagepathimerTemp + "\r\n" + "DirectoryExists: " + Directory.Exists(imagepathimerTemp) + "\r\n" + "imagepathimer: " + imagepathimer);
                //    imagepathimerTemp = imagepathimer;
                //}

                imagepathimer = imagepathimerTemp;
                _Logger.Info("Start Impersonate:");

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    if (string.IsNullOrEmpty(model.Register_device))
                    {
                        model.ExcelBulk
                                = ConvertHttpPostedFileBaseToUploadExcel(files, model, imagepathimer, dt);

                        if (null == model.ExcelBulk)
                        {
                            var base64photoDict = Session["ExcelBulk"] as Dictionary<string, string>;
                            model.ExcelBulk
                                = ConvertBase64PhotoToUploadExcel(base64photoDict, model, imagepathimer);
                        }
                    }
                    else if (model.Register_device == "MOBILE APP")
                    {
                        var base64photoDict = Session["ExcelBulk"] as Dictionary<string, string>;
                        model.ExcelBulk
                            = ConvertBase64PhotoToUploadExcel(base64photoDict, model, imagepathimer);
                    }
                    else
                    {
                        model.ExcelBulk
                            = ConvertHttpPostedFileBaseToUploadExcel(files, model, imagepathimer, dt);
                    }

                    _Logger.Info("End Impersonate:");
                    Session["ExcelBulk"] = null;

                    //EndInterface("", log, transactionId, "Success", "");

                    return model;

                }
            }
            catch (Exception ex)
            {
                //EndInterface("", log, transactionId, "ERROR", ex.GetErrorMessage());

                _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return model;
            }
        }

        private HttpPostedFileBase[] filesPostedRegisterExcelBulkTempStep;

        private UploadExcelBulk ConvertBase64PhotoToUploadExcel(
            Dictionary<string, string> excelDict,
            RegisterBulkCorpInstallModel model,
            string imagepathimer)
        {
            if (null == excelDict)
            {
                _Logger.Info("excel not contain valid files");
                return new UploadExcelBulk();
            }

            foreach (var item in excelDict)
            {
                var uploadExcelBulk = new UploadExcelBulk();
                uploadExcelBulk.FileExcelBulk = item.Key + Guid.NewGuid().ToString() + ".xlsx";
                model.ExcelBulk = uploadExcelBulk;
            }

            var path = "";
            var resultFormat = GetFormatExcelFile(model);
            var tempfile = new UploadExcelBulk();

            var fileIndex = 0;
            foreach (var item in excelDict)
            {
                path = Path.Combine(imagepathimer, resultFormat[fileIndex].file_name_bulk);
                var uploadImageWithGeneratedName = new UploadExcelBulk();
                uploadImageWithGeneratedName.FileExcelBulk = path;
                tempfile = uploadImageWithGeneratedName;

                var imgBytes = Convert.FromBase64String(excelDict[item.Key]);
                System.IO.File.WriteAllBytes(path, imgBytes);
                fileIndex++;
            }

            return tempfile;
        }

        private UploadExcelBulk ConvertHttpPostedFileBaseToUploadExcel(
            HttpPostedFileBase[] files,
            RegisterBulkCorpInstallModel model,
            string imagepathimer, DataTable dt)
        {
            string cardNo = model.id_card_no;
            model.ExcelBulk = new UploadExcelBulk();

            //InterfaceLogCommand log = null;

            //log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertHttpPostedFileBaseToUploadExcel", model.ca_main_mobile + model.ClientIP, cardNo, "WEB");
            try
            {

                if (files.Count() <= 0)
                {
                    _Logger.Info("upload excel file not contain valid excel files");

                    return new UploadExcelBulk();
                }

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            var varfileName = Path.GetFileName(files[i].FileName);
                            var p = new UploadExcelBulk();
                            p.FileExcelBulk = varfileName;
                            model.ExcelBulk = p;
                        }
                    }
                }

                var resultFormat = GetFormatExcelFile(model);
                var tempfile = new UploadExcelBulk();
                var j = 0;

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        var path = Path.Combine(imagepathimer, resultFormat[j].file_name_bulk);
                        var type = resultFormat[j].file_name_bulk.Substring(resultFormat[j].file_name_bulk.IndexOf(".") + 1).ToLower();
                        var p2 = new UploadExcelBulk();


                        p2.FileExcelBulk = path;
                        tempfile = p2;
                        /////////////////////////////////////////////////////////////////////////////////////////

                        //files[i].SaveAs(path);
                        FileInfo excelInfo = new FileInfo(path);
                        using (ExcelPackage pck = new ExcelPackage(excelInfo))
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ResendBulk");
                            ws.Cells["A1"].LoadFromDataTable(dt, true);
                            pck.Save();
                        }

                        j++;
                    }
                }

                //EndInterface("", log, model.ca_main_mobile + model.ClientIP, "Success", "");
                return tempfile;
            }
            catch (Exception ex)
            {
                //EndInterface("", log, model.ca_main_mobile + model.ClientIP, "ERROR", ex.GetErrorMessage());

                _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new UploadExcelBulk();
            }


        }

        private List<FileFormatModelBulk> GetFormatExcelFile(RegisterBulkCorpInstallModel model)
        {
            var lang = (SiteSession.CurrentUICulture.IsThaiCulture() ? "THAI" : "ENG");

            string lovdatatext = "99";
            /*base.LovData.Where(l => l.Type == "ID_CARD_TYPE" && l.Name == model.id_card_type)
            .Select(l => l.Text).FirstOrDefault();*/

            string resultstr = string.Empty;
            List<FileFormatModelBulk> result = new List<FileFormatModelBulk>();

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmssffffff");
            string dateNow = currDateTime.ToString("yyyyMMdd");
            string idcardstr = "9999";//model.id_card_no.Substring(model.id_card_no.Length - 4);
            string fileformatt = ".xlsx";


            var iistr = "01";
            resultstr = string.Format("FBBExcel_{0}_{1}_{2}{3}{4}{5}", lovdatatext, idcardstr, dateNow, timeNow, iistr, fileformatt);
            FileFormatModelBulk resulttemp = new FileFormatModelBulk();
            resulttemp.file_name_bulk = resultstr;
            result.Add(resulttemp);


            return result;
        }

        #endregion

    }
}
