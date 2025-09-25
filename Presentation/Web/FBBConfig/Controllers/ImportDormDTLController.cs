using Excel;
using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class ImportDormDTLController : FBBConfigController
    {
        //
        // GET: /ImportDormDTL/

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<ImportExcelDormCommand> _ImportExcelCommand;
        private readonly ICommandHandler<UpdatePrepaidNonMobileStatusCommand> _updatePrepaidNonMobileStatusCommand;

        private string rptName = string.Empty;
        private string rptCriteria = string.Empty;
        private string rptDate = string.Empty;
        private List<string> rptCriterias = new List<string>();

        private List<DomitoryModel> data;//= GetDormitaryALL();
        public ImportDormDTLController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<ImportExcelDormCommand> ImportExcelCommand,
            ICommandHandler<UpdatePrepaidNonMobileStatusCommand> UpdatePrepaidNonMobileStatusCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _ImportExcelCommand = ImportExcelCommand;
            _updatePrepaidNonMobileStatusCommand = UpdatePrepaidNonMobileStatusCommand;
        }

        public ActionResult AddNewFibrenet()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();
            return View();
        }

        private void SetViewBagLov()
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == "FBBDORM_ADMIN_SCREEN" && p.LovValue5 == "ADMIN_FBBDORM001").ToList();
            ViewBag.configscreen = LovDataScreen;
            ViewBag.DormConstant = GetFbbConstantModel(WebConstants.LovConfigName.DormConstants);

        }
        private List<FbbConstantModel> GetFbbConstantModel(string DormConstants)
        {
            var data = base.LovData
               .Where(l => l.Type.Equals(DormConstants))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }

        #region Configuration fibrenet - Upload Mobile by Excel
        /*******************************************************************************************************************
         * Begin ImportDTLDorm - Configuration Fibrenet - Upload Mobile by Excel ()                                              *
         *******************************************************************************************************************/

        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();
            return View();
        }

        public ActionResult ViewPrepaidNonMobile()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();
            return View();
        }

        public ActionResult clearSession()
        {
            var addedit = Session["tempupload"] as List<IPDexportlist>;
            if (addedit != null)
                Session.Remove("tempupload");

            var filename = Session["filename"];
            if (filename != null)
                Session.Remove("filename");
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult total()
        {
            string ttr = string.Empty;
            var tempupload = Session["tempupload"] as List<IPDexportlist>;
            ttr = tempupload.Count.ToString();

            return Json(new { ttr = ttr }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            //ExcelWorksheet wc6 = p.Workbook.Worksheets[2];
            var tempupload = Session["tempupload"] as List<IPDexportlist>;
            if (tempupload == null)
                tempupload = new List<IPDexportlist>();

            if (files != null)
            {

                var result = new List<IPDexportlist>();
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
                            var Dup = GetcheckDupbase(fileName);
                            if (checkdup == Dup)
                            {

                                var destinationPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                                file.SaveAs(destinationPath);
                                FileInfo fInfo = new FileInfo(destinationPath);
                                long size = fInfo.Length;
                                if (size > 10000000)
                                {
                                    var modelResponse2 = new { status = false, message = "File's exceeded", filename = fileName };
                                    return Json(modelResponse2, "text/plain");
                                }
                                else
                                {
                                    //**********  **********//
                                    DataTable table = getDataSet(destinationPath);
                                    table = RemoveAllNullRowsFromDataTable(table);
                                    //   table = RemoveDuplicateRows(table, "Fibre_ID");
                                    //var i = 0;
                                    bool checkrequire = false;
                                    foreach (DataRow dRow in table.Rows)
                                    {
                                        //i++;
                                        if (dRow["No"].ToString().Equals("")
                                            || dRow["Condo_Name"].ToString().Equals("")
                                            || dRow["Building"].ToString().Equals("")
                                            || dRow["Floor_no"].ToString().Equals("")
                                            || dRow["Room_no"].ToString().Equals("")
                                            || dRow["Fibre_ID"].ToString().Equals("")
                                            || dRow["PIN_ID"].ToString().Equals(""))
                                        {
                                            checkrequire = true;
                                            break;
                                        }
                                        var temp = new IPDexportlist();
                                        temp.No = dRow["No"].ToString();
                                        temp.Dormitory_Name = dRow["Condo_Name"].ToString();
                                        temp.Building_Name = dRow["Building"].ToString();
                                        temp.Floor = dRow["Floor_no"].ToString();
                                        temp.Room = dRow["Room_no"].ToString();
                                        temp.Fibrenet_id = dRow["Fibre_ID"].ToString();
                                        temp.Pin = dRow["PIN_ID"].ToString();
                                        result.Add(temp);
                                    }

                                    if (checkrequire == true || table.Rows.Count == 0)
                                    {
                                        var modelResponse2 = new { status = false, message = fileName + " does not compleate, please check require field in file.", filename = fileName };
                                        return Json(modelResponse2, "text/plain");
                                    }
                                    stopwatch.Stop();
                                    var sds = stopwatch.Elapsed;
                                    Session["tempupload"] = result;
                                    Session["filename"] = fileName;
                                    var modelResponse = new { status = true, message = "", filename = fileName };
                                    return Json(modelResponse, "text/plain");
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
                    var modelResponse2 = new { status = false, message = e.Message, filename = "" };
                    return Json(modelResponse2, "text/plain");
                }

            }

            return Content("");
        }

        /****** Check Duplicate and Insert file name in DB ******/
        public bool GetcheckDupbase(string filename)
        {
            var query = new GetDupFileDorm()
            {
                file_name = filename,
                user = base.CurrentUser.UserName.ToString()
            };
            var result = _queryProcessor.Execute(query);

            if (result.result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public DataTable getDataSet(string path)
        {
            // Get the Excel file and convert to dataset 
            DataTable res = null;
            DataSet dataSet = new DataSet();
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
            }

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                res = dataSet.Tables[0];
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
            var tempupload = Session["tempupload"] as List<IPDexportlist>;
            if (tempupload == null)
                tempupload = new List<IPDexportlist>();

            return Json(tempupload.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /****** Insert each Record in Excel to Stored Procedure PKG_FBBDORM_ADMIN001 ******/
        public ActionResult Sendpackage(string Province, string DormitoryName)
        {
            var tempupload = Session["tempupload"] as List<IPDexportlist>;
            var filename = Session["filename"] as string;
            try
            {
                bool statusInsert = true;
                foreach (var file in tempupload)
                {
                    _Logger.Info("Fibrenet_id : " + file.Fibrenet_id);

                    var command = new ImportExcelDormCommand
                    {
                        Dormitory_NameTH = file.Dormitory_Name,
                        Building_Name = file.Building_Name,
                        Floor = file.Floor,
                        Room = file.Room,
                        Fibrenet_id = file.Fibrenet_id,
                        Pin = file.Pin,
                        Dormitory_Name = DormitoryName,
                        filename = filename,
                        user = base.CurrentUser.UserName
                    };
                    _ImportExcelCommand.Handle(command);

                    if (command.Return_Code == "-1")
                    {
                        statusInsert = statusInsert & false;
                        _Logger.Info("Return_Code : " + command.Return_Code + " statusInsert : " + statusInsert);
                    }
                    else
                    {
                        statusInsert = statusInsert & true;
                        _Logger.Info("Return_Code : " + command.Return_Code + " statusInsert : " + statusInsert);
                    }

                }

                if (statusInsert)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json("Cannot upload file. Please contact support.", JsonRequestBehavior.AllowGet);
            }

        }

        /*******************************************************************************************************************
         * End ImportDTLDorm - Configuration Fibrenet - Upload Mobile by Excel ()                                              *
         *******************************************************************************************************************/
        #endregion


        public JsonResult SelectDormStatusForSearch()
        {
            var query = new SelectDormStatusQuery
            {
                FlagSearch = true
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new LovModel { DISPLAY_VAL = "เลือกทั้งหมด", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchDataSourceRequest([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (dataS != null && dataS != "")
            {
                var SearchPara = new JavaScriptSerializer().Deserialize<ConfigurationFibrenetID>(dataS);
                var result = GetDataSearchModel(SearchPara);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        private List<ConfigurationPrepaidNonMobileData> GetDataSearchModel(ConfigurationFibrenetID SearchPara)
        {
            var User = base.CurrentUser;

            var query = new SearchConfigurationPrepaidNonMobileDataQuery()
            {
                User = User.UserName,
                DormitoryName = SearchPara.DormitoryName ?? "",
                BuildingNo = SearchPara.Building ?? "",
                DormitoryProvince = SearchPara.Province ?? "",
                Stutus = SearchPara.Status ?? "",

                FibrenetID = SearchPara.FibrenetID ?? "",
                Region = SearchPara.Region ?? "",
                FloorNo = SearchPara.FloorNo ?? "",
                RoomNo = SearchPara.RoomNo ?? "",

            };
            List<ConfigurationPrepaidNonMobileData> result = _queryProcessor.Execute(query);
            return result;
        }

        public List<DomitoryModel> GetDormitaryALL()
        {
            var result = new GetDormitoryQuery
            {
                language = "TH"
            };

            var q_result = _queryProcessor.Execute(result);

            return q_result;
        }

        public JsonResult SelectDormitaryFloor(string Dormitory_Name = "", string Building_No = "")
        {
            var newListFloor = new List<LovModel>();

            if (Dormitory_Name != "" && Building_No != "")
            {
                data = GetDormitaryALL();

                //var tempDate = data;
                data = data.Where(x => (x.Pre_dormitory_name_th).Equals(Dormitory_Name)
                                && (x.Pre_dormitory_no_th).Equals(Building_No)).ToList();

                List<DropdownModel> newList = data.GroupBy(x => new { x.Pre_floor_no })
                 .Select(y => new DropdownModel()
                 {
                     Value = y.Key.Pre_floor_no,
                     Text = y.Key.Pre_floor_no
                 }
                  ).ToList();
                List<string> newStrList = new List<string>();
                foreach (var item in newList)
                {
                    string ddlstr = item.Text;
                    newStrList.Add(ddlstr);
                }

                newStrList.Sort((a, b) => new StringNum(a).CompareTo(new StringNum(b)));

                foreach (var item in newStrList)
                {
                    LovModel newlist = new LovModel();
                    newlist.DISPLAY_VAL = item;
                    newlist.LOV_NAME = item;
                    newListFloor.Insert(newListFloor.Count(), newlist);
                }
            }

            newListFloor.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });

            return Json(newListFloor, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectDormitaryRoom(string Dormitory_Name = "", string Building_No = "", string Floor = "")
        {
            var newListFloor = new List<LovModel>();

            if (Dormitory_Name != "" && Building_No != "" && Floor != "")
            {
                data = GetDormitaryALL();

                //var tempDate = data;
                data = data.Where(x => (x.Pre_dormitory_name_th).Equals(Dormitory_Name)
                                 && (x.Pre_dormitory_no_th).Equals(Building_No) && (x.Pre_floor_no).Equals(Floor)).ToList();

                List<DropdownModel> newList = data.GroupBy(x => new { x.Pre_room_no })
                  .Select(y => new DropdownModel()
                  {
                      Value = y.Key.Pre_room_no,
                      Text = y.Key.Pre_room_no
                  }
                   ).ToList();
                List<string> newStrList = new List<string>();
                foreach (var item in newList)
                {
                    string ddlstr = item.Text;
                    newStrList.Add(ddlstr);
                }

                newStrList.Sort((a, b) => new StringNum(a).CompareTo(new StringNum(b)));

                foreach (var item in newStrList)
                {
                    LovModel newlist = new LovModel();
                    newlist.DISPLAY_VAL = item;
                    newlist.LOV_NAME = item;
                    newListFloor.Insert(newListFloor.Count(), newlist);
                }

            }
            newListFloor.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });

            return Json(newListFloor, JsonRequestBehavior.AllowGet);
        }

        public class StringNum : IComparable<StringNum>
        {

            private List<string> _strings;
            private List<int> _numbers;

            public StringNum(string value)
            {
                _strings = new List<string>();
                _numbers = new List<int>();
                int pos = 0;
                bool number = false;
                while (pos < value.Length)
                {
                    int len = 0;
                    while (pos + len < value.Length && Char.IsDigit(value[pos + len]) == number)
                    {
                        len++;
                    }
                    if (number)
                    {
                        _numbers.Add(int.Parse(value.Substring(pos, len)));
                    }
                    else
                    {
                        _strings.Add(value.Substring(pos, len));
                    }
                    pos += len;
                    number = !number;
                }
            }

            public int CompareTo(StringNum other)
            {
                int index = 0;
                while (index < _strings.Count && index < other._strings.Count)
                {
                    int result = _strings[index].CompareTo(other._strings[index]);
                    if (result != 0) return result;
                    if (index < _numbers.Count && index < other._numbers.Count)
                    {
                        result = _numbers[index].CompareTo(other._numbers[index]);
                        if (result != 0) return result;
                    }
                    else
                    {
                        return index == _numbers.Count && index == other._numbers.Count ? 0 : index == _numbers.Count ? -1 : 1;
                    }
                    index++;
                }
                return index == _strings.Count && index == other._strings.Count ? 0 : index == _strings.Count ? -1 : 1;
            }

        }

        public ActionResult UpdateStatusData(string dataS)
        {
            try
            {
                UpdatePrepaidNonMobileStatusDataModel model = new JavaScriptSerializer().Deserialize<UpdatePrepaidNonMobileStatusDataModel>(dataS);
                model.User = base.CurrentUser.UserName;

                var command = new UpdatePrepaidNonMobileStatusCommand
                {
                    FibrenetID = model.FibrenetID,
                    Status = model.Status,
                    User = this.CurrentUser.UserName
                };
                _updatePrepaidNonMobileStatusCommand.Handle(command);


                return Json(new { result = command.Return_Code == 0, message = command.Return_Message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Message: " + ex.Message + "InnerException: " + ex.InnerException }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportExcelData([DataSourceRequest] DataSourceRequest request, List<ConfigurationPrepaidNonMobileData> griditems)
        {
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(string dataS, string criteria)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<ConfigurationFibrenetID>(dataS);
            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            rptCriteria = criteriaModel.CRITERIA;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;

            List<ConfigurationPrepaidNonMobileData> listall = GetDataSearchModel(searchModel);


            string filename = GetExcelNameWithDateTime("ConfigurationFibrenetID");
            string[] headerCol = { "G_REGION", "G_DORMITORY_PROVINCE", "L_DORMITORY_NAME", "G_BUILDING_NAME", "L_FLOOR", "L_ROOM", "L_FIBRE_NET_ID", "L_PIN", "L_STATUS" };

            var bytes = GenerateEntitytoExcel<ConfigurationPrepaidNonMobileData>(listall, filename, headerCol, "FBBDORM_ADMIN_SCREEN", "ADMIN_FBBDORM001", rptName, rptCriteria, rptDate, null, null);


            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public JsonResult SelectDormStatusForUpdate()
        {
            var query = new SelectDormStatusQuery
            {
                FlagSearch = false
            };
            var data = _queryProcessor.Execute(query);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }

}
