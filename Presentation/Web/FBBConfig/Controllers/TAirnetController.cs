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
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class TAirnetController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FBB_APCOVERAGE> _COMMAND_FBB_APCOVERAGE;
        private readonly ICommandHandler<FBB_AP_INFO> _COMMAND_FBB_AP_INFO;
        private readonly ICommandHandler<ImaportExcelCommand> _ImaportExcelCommand;
        //public List<int>  dupinfile = new List<int>();

        public TAirnetController(ILogger logger,
             IQueryProcessor queryProcessor,
            ICommandHandler<FBB_APCOVERAGE> COMMAND_FBB_APCOVERAGE,
            ICommandHandler<FBB_AP_INFO> COMMAND_FBB_AP_INFO,
            ICommandHandler<ImaportExcelCommand> ImaportExcelCommand)
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _COMMAND_FBB_APCOVERAGE = COMMAND_FBB_APCOVERAGE;
            _COMMAND_FBB_AP_INFO = COMMAND_FBB_AP_INFO;
            _ImaportExcelCommand = ImaportExcelCommand;
        }

        [AuthorizeUserAttribute]
        public ActionResult Index(string modelpage1 = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;


            var addedit = Session["tempupload"] as List<AWCexportlist>;
            if (addedit != null)
                Session.Remove("tempupload");
            //var size = Session["filesizeIE"];
            //if (size != null)
            //{
            //    Session.Remove("filesizeIE");
            //}
            //else
            //{
            //    Session["filesizeIE"] = "0";
            //}           
            var a = getnew(modelpage1);
            return View(a);
        }
        public ActionResult clearSession()
        {
            var addedit = Session["tempupload"] as List<AWCexportlist>;
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

            var tempupload = Session["tempupload"] as List<AWCexportlist>;


            ttr = tempupload.Count.ToString();



            return Json(new { ttr = ttr }, JsonRequestBehavior.AllowGet);
        }
        public AWCinformation getnew(string modelpage1 = "")
        {
            var oldmodelpage1 = new JavaScriptSerializer().Deserialize<AWCModel>(modelpage1);
            var result = new AWCinformation();
            result.oldmodelpage1 = oldmodelpage1;
            return result;
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            //ExcelWorksheet wc6 = p.Workbook.Worksheets[2];
            var tempupload = Session["tempupload"] as List<AWCexportlist>;
            if (tempupload == null)
                tempupload = new List<AWCexportlist>();

            if (files != null)
            {

                var result = new List<AWCexportlist>();
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
                            var checkdup = GetcheckDupbase(fileName);
                            if (!checkdup.dup)
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
                                    DataTable table = getDataSet(destinationPath);
                                    table = RemoveAllNullRowsFromDataTable(table);
                                    table = RemoveDuplicateRows(table, "AP Name");
                                    //var i = 0;
                                    bool checkrequire = false;
                                    foreach (DataRow dRow in table.Rows)
                                    {
                                        //i++;
                                        if (dRow["Sitename"].ToString().Equals("")
                                            || dRow["Base L2"].ToString().Equals("")
                                            || dRow["District"].ToString().Equals("")
                                            || dRow["Sub district"].ToString().Equals("")
                                            || dRow["Province"].ToString().Equals("")
                                            || dRow["Region"].ToString().Equals("")
                                            || dRow["Lat"].ToString().Equals("")
                                            || dRow["Long"].ToString().Equals("")
                                            || dRow["AP Name"].ToString().Equals("")
                                            || dRow["Sector"].ToString().Equals(""))
                                        {
                                            checkrequire = true;
                                            break;
                                        }
                                        var temp = new AWCexportlist();
                                        temp.Site_Name = dRow["Sitename"].ToString();
                                        temp.Base_L2 = dRow["Base L2"].ToString();
                                        temp.Aumphur = dRow["District"].ToString();
                                        temp.Tumbon = dRow["Sub district"].ToString();
                                        temp.Province = dRow["Province"].ToString();
                                        temp.Zone = dRow["Region"].ToString();
                                        temp.Lat = dRow["Lat"].ToString();
                                        temp.Lon = dRow["Long"].ToString();
                                        temp.AP_Name = dRow["AP Name"].ToString();
                                        temp.Sector = dRow["Sector"].ToString();

                                        temp.Tower_Type = dRow["Tower Type"].ToString();
                                        temp.Tower_Height = dRow["Tower Height"].ToString();
                                        temp.VLAN = dRow["VLAN"].ToString();
                                        temp.Subnet_Mask_26 = dRow["Subnet Mask 26"].ToString();
                                        temp.Gateway = dRow["Gateway"].ToString();
                                        temp.Comment = dRow["Comment"].ToString();

                                        temp.IP_Address = dRow["IP Address"].ToString();
                                        temp.Status = dRow["Status"].ToString();
                                        temp.Implement_Phase = dRow["Implement Phase"].ToString();

                                        if (fileName.EndsWith("xls"))
                                        {
                                            if (dRow["Implement date"].GetType() == typeof(string))
                                            {
                                                var tempp = dRow["Implement date"].ToString().Split('/');
                                                if (tempp[0].Length == 1)
                                                {
                                                    tempp[0] = "0" + tempp[0];
                                                }
                                                if (tempp[1].Length == 1)
                                                {
                                                    tempp[1] = "0" + tempp[1];
                                                }
                                                temp.Implement_date = tempp[0] + "/" + tempp[1] + "/" + tempp[2];
                                                //temp.Implement_date = String.Format("{0:dd/MM/yyyy}", dRow["Implement date"]);
                                            }
                                            else
                                            {
                                                temp.Implement_date = DateTime.FromOADate(Convert.ToDouble(dRow["Implement date"].ToString())).ToString("dd/MM/yyyy");
                                            }
                                            if (dRow["On service date"].GetType() == typeof(string))
                                            {
                                                var tempp = dRow["On service date"].ToString().Split('/');
                                                tempp = dRow["On service date"].ToString().Split('/');
                                                if (tempp[0].Length == 1)
                                                {
                                                    tempp[0] = "0" + tempp[0];
                                                }
                                                if (tempp[1].Length == 1)
                                                {
                                                    tempp[1] = "0" + tempp[1];
                                                }
                                                temp.Onservice_date = tempp[0] + "/" + tempp[1] + "/" + tempp[2];
                                                //temp.Onservice_date = String.Format("{0:dd/MM/yyyy}", dRow["On service date"]);
                                            }
                                            else
                                            {
                                                temp.Onservice_date = DateTime.FromOADate(Convert.ToDouble(dRow["On service date"].ToString())).ToString("dd/MM/yyyy");
                                            }
                                        }
                                        else if (fileName.EndsWith("xlsx"))
                                        {
                                            temp.Implement_date = String.Format("{0:dd/MM/yyyy}", dRow["Implement date"]);
                                            temp.Onservice_date = String.Format("{0:dd/MM/yyyy}", dRow["On service date"]);
                                        }


                                        //temp.Implement_date = dRow["Implement date"].ToString();
                                        //temp.Onservice_date = dRow["On service date"].ToString();
                                        temp.PO_Number = dRow["PO Number"].ToString();
                                        temp.AP_Company = dRow["AP Company"].ToString();
                                        temp.AP_Lot = dRow["AP Lot"].ToString();
                                        //temp.AP_ID = i;
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
        public DataTable getDataSet(string path)
        {
            // Get the Excel file and convert to dataset 
            DataTable res = null;
            DataSet dataSet;
            IExcelDataReader iExcelDataReader = null;
            FileStream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read);
            if (path.EndsWith("xls"))
                iExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
            if (path.EndsWith("xlsx"))
                iExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

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

        public ActionResult Remove(string[] fileNames)
        {

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    if (System.IO.File.Exists(physicalPath))
                    {

                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Gettogrid([DataSourceRequest] DataSourceRequest request)
        {
            var tempupload = Session["tempupload"] as List<AWCexportlist>;
            if (tempupload == null)
                tempupload = new List<AWCexportlist>();

            return Json(tempupload.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Getsize(string  filename)
        //{
        //    string size = string.Empty;
        //    var destinationPath = Path.Combine(Server.MapPath("~/App_Data"), filename);           
        //    FileInfo fInfo = new FileInfo(destinationPath);
        //    size = fInfo.Length.ToString();

        //    return Json(new { size = size }, JsonRequestBehavior.AllowGet);
        //}        

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

        public Dupfile GetcheckDupbase(string filename)
        {
            //HashSet<int> sentIDs = new HashSet<int>(SentList.Select(s => s.MsgID));
            //var results = MsgList.Where(m => !sentIDs.Contains(m.MsgID));

            //List<int> list1 = new List<int>(){1,2,3,4,5,6};
            //List<int> list2 = new List<int>(){3,5,6,7,8};


            //list1 = list1.Except(list2).ToList(); // gives me an error.

            //var query = new GetAWCAllC()
            //{
            //};
            //var frombase = _queryProcessor.Execute(query);



            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //var notdup =
            //    from msg in fromexcel
            //    where !fromexcel.Any(x => x.Base_L2 == msg.Base_L2 || x.AP_Name == msg.AP_Name)
            //    select msg;

            //var ss = notdup.ToList();
            //var firstNotSecond = fromexcel.Except(frombase).ToList();
            //stopwatch.Stop();
            //var sds = stopwatch.Elapsed;

            #region test
            //int count = 0;
            //Stopwatch stopwatch2 = new Stopwatch();
            //stopwatch2.Start();
            ////var dup =
            ////   from msg in fromexcel
            ////   where fromexcel.Any(x => x.Base_L2 == msg.Base_L2 || x.AP_Name == msg.AP_Name)
            ////   select msg;
            //for (var i = 0; i < fromexcel.Count; i++)
            //{
            //    for (var j = 0; j < frombase.Count(); j++)
            //    {
            //        if (frombase[j].Base_L2 == fromexcel[i].Base_L2 || frombase[j].AP_Name == fromexcel[i].AP_Name)
            //        {
            //            count++;
            //            break;
            //        }
            //    }
            //}
            //var ss = count;
            ////var ss2 = dup.ToList();
            //stopwatch2.Stop();
            //var sds2 = stopwatch2.Elapsed;

            //return Json(true, JsonRequestBehavior.AllowGet);
            #endregion

            var query = new GetDupfileQuery()
            {
                file_name = filename,
                user = base.CurrentUser.UserName.ToString()

            };
            return _queryProcessor.Execute(query);
        }

        public ActionResult Sendpackage()
        {
            var tempupload = Session["tempupload"] as List<AWCexportlist>;
            var filename = Session["filename"] as string;
            try
            {
                var command = new ImaportExcelCommand
                {
                    Imex = tempupload,
                    filename = filename,
                    user = base.CurrentUser.UserName.ToString()

                };
                _ImaportExcelCommand.Handle(command);

                if (command.Return_Code == -1)
                {
                    return Json(command.Return_Desc, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json("Cannot upload file. Please contact support.", JsonRequestBehavior.AllowGet);

            }
            //return Json(false, JsonRequestBehavior.AllowGet);
        }








    }
}



