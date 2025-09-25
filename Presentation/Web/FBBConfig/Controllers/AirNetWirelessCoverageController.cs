using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class AirNetWirelessCoverageController : FBBConfigController
    {
        //
        // GET: /AirNetWirelessCoverage/
        private readonly IQueryProcessor _queryProcessor;
        private readonly IEntityRepository<FBB_AP_INFO> _apifo;
        private readonly ICommandHandler<DeleteAWCconfigCommand> _DeleteAWCconfigCommand;
        private readonly ICommandHandler<DeleteAWCInfoCommand> _DeleteAWCInfoCommand;
        private readonly ICommandHandler<CreateAWCconfigCommand> _CreateAWCconfigCommand;
        private readonly ICommandHandler<CreateAWCInfoCommand> _CreateAWCInfoCommand;
        private readonly ICommandHandler<UpdateAWCconfigCommand> _UpdateAWCconfigCommand;
        private readonly ICommandHandler<UpdateAWCInfoCommand> _UpdateAWCInfoCommand;
        private readonly ICommandHandler<DeletetempAPCommand> _DeletetempAPCommand;



        public AirNetWirelessCoverageController(ILogger logger,
             IQueryProcessor queryProcessor, ICommandHandler<DeleteAWCconfigCommand> DeleteAWCconfigCommand,
           ICommandHandler<DeleteAWCInfoCommand> DeleteAWCInfoCommand, ICommandHandler<CreateAWCconfigCommand> CreateAWCconfigCommand
           , ICommandHandler<CreateAWCInfoCommand> CreateAWCInfoCommand, ICommandHandler<UpdateAWCconfigCommand> UpdateAWCconfigCommand
           , ICommandHandler<UpdateAWCInfoCommand> UpdateAWCInfoCommand, ICommandHandler<DeletetempAPCommand> DeletetempAPCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _DeleteAWCInfoCommand = DeleteAWCInfoCommand;
            _DeleteAWCconfigCommand = DeleteAWCconfigCommand;
            _CreateAWCconfigCommand = CreateAWCconfigCommand;
            _CreateAWCInfoCommand = CreateAWCInfoCommand;
            _UpdateAWCconfigCommand = UpdateAWCconfigCommand;
            _UpdateAWCInfoCommand = UpdateAWCInfoCommand;
            _DeletetempAPCommand = DeletetempAPCommand;
        }


        public ActionResult Index(string saveFlag = "")
        {

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");


            var addedit = Session["addeditap"] as List<AWCconfig>;
            if (addedit != null)
                Session.Remove("addeditap");
            var addnew = Session["addnewap"] as List<AWCconfig>;
            if (addnew != null)
                Session.Remove("addnewap");
            ViewBag.User = base.CurrentUser;
            return View();
        }

        //public ActionResult AirNetWirelessCoverage()
        //{

        //    if (null == base.CurrentUser)
        //        return RedirectToAction("Logout", "Account");

        //    ViewBag.User = base.CurrentUser;
        //    return View();
        //}        

        public ActionResult aa(string dataS = "")
        {
            var searchoawcModel = new JavaScriptSerializer().Deserialize<AWCModel>(dataS);
            var result = GetDataSearchModel(searchoawcModel);

            string ttc = string.Empty;
            string ttp = string.Empty;

            if (result.Count == 0)
            {
                ttc = "0";
                ttp = "0";
            }
            else
            {
                ttc = result[0].TotalCoverage.ToString();
                ttp = result[0].TotalAP.ToString();
            }

            return Json(new { ttc = ttc, ttp = ttp }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult ReadSearch([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchoawcModel = new JavaScriptSerializer().Deserialize<AWCModel>(dataS);
                var result = GetDataSearchModel(searchoawcModel);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var searchoawcModel = new JavaScriptSerializer().Deserialize<AWCModel>(dataS);
                //var result = GetDataSearchModel(searchoawcModel);
                //return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                return null;
            }
        }

        public List<AWCSearchlist> GetDataSearchModel(AWCModel searchawcModel)
        {

            var query = new GetAWCQuery()
            {
                region = searchawcModel.region ?? "",
                province = searchawcModel.province ?? "",
                aumphur = searchawcModel.aumphur ?? "",
                tumbon = searchawcModel.tumbon ?? "",
                APname = searchawcModel.APname ?? ""

            };
            return GetSearchReqCurStageQueryData(query);
        }
        public List<AWCSearchlist> GetSearchReqCurStageQueryData(GetAWCQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportData(string dataS)
        {
            List<AWCexportResultlist> listall;
            var searchoawcModel = new JavaScriptSerializer().Deserialize<AWCModel>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            listall = GetexportAWC(searchoawcModel);


            string filename = "AirNetWirelessReport";

            var bytes = GenerateEntitytoExcel<AWCexportResultlist>(listall, filename);

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public List<AWCexportResultlist> GetexportAWC(AWCModel searchoawcModel)
        {

            try
            {
                var query = new GetAWCAllQuery
                {
                    region = searchoawcModel.region ?? "",
                    province = searchoawcModel.province ?? "",
                    aumphur = searchoawcModel.aumphur ?? "",
                    tumbon = searchoawcModel.tumbon ?? "",
                    APname = searchoawcModel.APname ?? ""

                };

                return _queryProcessor.Execute(query);


            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<AWCexportResultlist>();
            }

        }

        public ActionResult DataConfig([DataSourceRequest] DataSourceRequest request, string dataS = "", string fag = "")
        {
            if (dataS != null && dataS != "")
            {
                List<AWCconfig> listall;

                listall = GetDataConfig(dataS);

                return Json(listall.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {

                var listall = new List<AWCconfig>();
                return Json(listall.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult GetSectorEdit(string dataS = "")
        {

            var result = GetDataConfig(dataS).Select(l => new DropdownModel { Text = l.Sector.ToString(), Value = l.Sector.ToString() }).ToList();
            result.Insert(0, new DropdownModel { Text = "กรุุณาเลือก", Value = "" });
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        public List<AWCconfig> GetDataConfig(string searchoawcModel = "")
        {

            try
            {
                var query = new GetAWCconfigQuey
                {
                };

                var AWCall = _queryProcessor.Execute(query)
                   .Where(z => z.Site_id.ToString() == searchoawcModel && z.ACTIVE_FLAGINFO == "Y")
                   .ToList();

                return AWCall;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<AWCconfig>();
            }

        }

        public ActionResult DataInformation(string dataS = "")
        {
            var result = GetDataInformation(dataS);

            string Base = string.Empty;
            string Sitename = string.Empty;
            string region = string.Empty;
            string province = string.Empty;
            string aumphur = string.Empty;
            string tumbon = string.Empty;
            string lat = string.Empty;
            string lon = string.Empty;

            if (result.Count == 0)
            {
                Base = "";
                Sitename = "";
                region = "";
                province = "";
                aumphur = "";
                tumbon = "";
                lat = "";
                lon = "";

            }
            else
            {
                Base = result[0].Base_L2.ToString().Trim();
                Sitename = result[0].Site_Name.ToString().Trim();
                region = result[0].Zone.ToString().Trim();
                province = result[0].Province.ToString().Trim();
                aumphur = result[0].Aumphur.ToString().Trim();
                tumbon = result[0].Tumbon.ToString().Trim();
                lat = result[0].Lat.ToString().Trim();
                lon = result[0].Lon.ToString().Trim();
            }

            return Json(new
            {
                Base = Base,
                Sitename = Sitename,
                region = region,
                province = province,
                aumphur = aumphur,
                tumbon = tumbon,
                lat = lat,
                lon = lon,
            }, JsonRequestBehavior.AllowGet);

        }
        public List<AWCinformation> GetDataInformation(string dataS = "")
        {

            try
            {
                var query = new GetAWCinformationQuery
                {
                };

                var AWCall = _queryProcessor.Execute(query)
                   .Where(z => z.APP_ID.ToString() == dataS && z.ACTIVE_FLAGAPPC == "Y")
                   .ToList();

                return AWCall;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<AWCinformation>();
            }

        }

        public ActionResult DeleteAPConfig(string ID)
        {
            try
            {
                var a = DateTime.Now;

                var command = new DeleteAWCconfigCommand
                {
                    UPDATED_BY = base.CurrentUser.UserName.ToString(),
                    UPDATED_DATE = a,
                    AP_ID = Convert.ToDecimal(ID)
                };
                _DeleteAWCconfigCommand.Handle(command);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteInfo(string ID)
        {
            try
            {
                var a = DateTime.Now;
                var addedit = Session["addeditap"] as List<AWCconfig>;

                var command = new DeleteAWCInfoCommand
                {
                    UPDATED_BY = base.CurrentUser.UserName.ToString(),
                    UPDATED_DATE = a,
                    APP_ID = Convert.ToDecimal(ID),
                    model = addedit
                };
                _DeleteAWCInfoCommand.Handle(command);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddAPconfig(string data)
        {
            try
            {
                AWCconfig model = new JavaScriptSerializer().Deserialize<AWCconfig>(data);
                model.user = base.CurrentUser.UserName;

                var command = new CreateAWCconfigCommand
                {
                    AWCconfigModel = model
                };
                _CreateAWCconfigCommand.Handle(command);

                if (command.FlagDup == true)
                    return Json("dup", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddCoverageInfo(string data)
        {
            try
            {
                AWCinformation model = new JavaScriptSerializer().Deserialize<AWCinformation>(data);
                model.user = base.CurrentUser.UserName;

                var command = new CreateAWCInfoCommand
                {
                    awcmodel = model
                };
                _CreateAWCInfoCommand.Handle(command);

                if (command.FlagDup == true)
                    return Json("dup", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletetempAP()
        {
            try
            {
                var command = new DeletetempAPCommand
                {
                };
                _DeletetempAPCommand.Handle(command);


                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveAPConfig(string ID, string apname, string sector)
        {
            try
            {
                var command = new UpdateAWCconfigCommand
                {
                    user = base.CurrentUser.UserName,
                    AP_ID = Convert.ToDecimal(ID),
                    AP_NAME = apname,
                    SECTOR = sector
                };
                _UpdateAWCconfigCommand.Handle(command);

                if (command.FlagDup == true)
                    return Json("dup", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveInfo(string data)
        {
            try
            {
                AWCinformation model = new JavaScriptSerializer().Deserialize<AWCinformation>(data);
                model.user = base.CurrentUser.UserName;

                var command = new UpdateAWCInfoCommand
                {
                    awcmodel = model
                };
                _UpdateAWCInfoCommand.Handle(command);

                if (command.FlagDup == true)
                    return Json("dup", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddAPconfig2(string data, string fag, string id = "")
        {
            try
            {
                Boolean inlistdb = false;
                Boolean inlistdb2 = false;

                AWCconfig model = new JavaScriptSerializer().Deserialize<AWCconfig>(data);
                //if (model.onservice_date != null)
                //{
                //    DateTime temponservice = model.onservice_date.Value;
                //    model.onservice_date = temponservice.AddDays(1);
                //}
                //if (model.implement_date != null)
                //{
                //    DateTime temponimplement = model.implement_date.Value;
                //    model.implement_date = temponimplement.AddDays(1);
                //}
                //model.user = base.CurrentUser.UserName;
                var sss = DateTime.Now;
                model.updatedate = sss;
                model.ACTIVE_FLAGINFO = "Y";
                model.user = base.CurrentUser.UserName;

                if (fag == "edit")
                {
                    var listall = new List<AWCconfig>();
                    listall = GetDataConfig(id);
                    var addedit = Session["addeditap"] as List<AWCconfig>;
                    if (addedit == null)
                        addedit = new List<AWCconfig>();
                    //foreach (var e in addedit)
                    //{
                    var inlist = listall.Select(x => x.AP_Name);
                    if (inlist.Contains(model.AP_Name))
                    {
                        inlistdb = true;
                        if (addedit.Count == 0)
                        {
                            return Json("dup", JsonRequestBehavior.AllowGet);
                        }
                    }
                    //}
                    foreach (var e in addedit)
                    {
                        var inlist2 = listall.Select(x => x.AP_Name);
                        if (inlist2.Contains(e.AP_Name))
                        {
                            inlistdb2 = true;
                            break;
                        }
                    }
                    if (inlistdb == false)
                    {

                        var command = new CreateAWCconfigCommand
                        {
                            AWCconfigModel = model
                        };
                        _CreateAWCconfigCommand.Handle(command);

                        if (command.FlagDup == true)
                        {
                            return Json("dup", JsonRequestBehavior.AllowGet);
                        }
                    }

                    var sum = from s in addedit where s.AP_Name == model.AP_Name && s.ACTIVE_FLAGINFO == "Y" select s;
                    if (sum.Any())
                    {
                        return Json("dup", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        model.AP_ID = 0;
                        addedit.Add(model);
                        if (inlistdb2 == false)
                        {
                            addedit.AddRange(listall);
                        }
                        var aa = (from y in addedit where y.ACTIVE_FLAGINFO == "Y" || y.ACTIVE_FLAGINFO == "N" select y).ToList();
                        var tempd = new List<AWCconfig>();
                        tempd = aa.OrderByDescending(a => a.updatedate).ToList();
                        var temp3 = from r in tempd where r.ACTIVE_FLAGINFO == "Y" select r;
                        Session["addeditap"] = tempd;
                        return Json((List<AWCconfig>)temp3.ToList(), JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {

                    var addnew = Session["addnewap"] as List<AWCconfig>;
                    if (addnew == null)
                        addnew = new List<AWCconfig>();
                    var command = new CreateAWCconfigCommand
                    {
                        AWCconfigModel = model
                    };
                    _CreateAWCconfigCommand.Handle(command);

                    if (command.FlagDup == true)
                    {
                        return Json("dup", JsonRequestBehavior.AllowGet);
                    }
                    var sum = from s in addnew where s.AP_Name == model.AP_Name select s;
                    if (sum.Any())
                    {
                        return Json("dup", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        model.AP_ID = 0;
                        addnew.Add(model);
                        var tempd = new List<AWCconfig>();
                        tempd = addnew.OrderByDescending(a => a.updatedate).ToList();
                        Session["addnewap"] = tempd;
                        return Json((List<AWCconfig>)Session["addnewap"], JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteAPConfig2(string data, string ID, string fag, string siteid, string apname)
        {
            //try
            //{
            //    var a = DateTime.Now;

            //    var command = new DeleteAWCconfigCommand
            //    {
            //        UPDATED_BY = base.CurrentUser.UserName.ToString(),
            //        UPDATED_DATE = a,
            //        AP_ID = Convert.ToDecimal(ID)
            //    };
            //    _DeleteAWCconfigCommand.Handle(command);

            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            //}
            try
            {
                AWCconfig model = new JavaScriptSerializer().Deserialize<AWCconfig>(data);
                model.ACTIVE_FLAGINFO = "N";
                Boolean inlistdb = false;

                if (fag == "edit")
                {
                    var addedit = Session["addeditap"] as List<AWCconfig>;
                    if (addedit == null)
                        addedit = new List<AWCconfig>();

                    var listall = new List<AWCconfig>();
                    listall = GetDataConfig(siteid);
                    foreach (var e in addedit)
                    {
                        var inlist = listall.Select(x => x.AP_Name);
                        if (inlist.Contains(e.AP_Name))
                        {
                            inlistdb = true;
                            break;
                        }
                    }

                    //if (checkdb.Any())
                    //{
                    //    //var command = new DeleteAWCconfigCommand
                    //    //{
                    //    //    UPDATED_BY = base.CurrentUser.UserName.ToString(),
                    //    //    UPDATED_DATE = DateTime.Now,
                    //    //    AP_ID = Convert.ToDecimal(ID)
                    //    //};
                    //    //_DeleteAWCconfigCommand.Handle(command);

                    //    //listall = GetDataConfig(siteid);
                    //    addedit.AddRange(listall);
                    //    var aa = addedit.DistinctBy(a => a.AP_Name);
                    //    var tempd = new List<AWCconfig>();
                    //    tempd = aa.OrderByDescending(a => a.updatedate).ToList();
                    //    tempd.RemoveAll(x => x.AP_Name == apname && x.AP_ID == Convert.ToDecimal(ID));
                    //    Session["addeditap"] = tempd;

                    //    return Json((List<AWCconfig>)Session["addeditap"], JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    AWCconfig model2 = new AWCconfig();

                    addedit.Add(model2);
                    if (inlistdb == false)
                    {
                        addedit.AddRange(listall);
                    }
                    //var aa = addedit.DistinctBy(a => a.AP_Name);
                    var tempd = new List<AWCconfig>();
                    tempd = addedit.OrderByDescending(a => a.updatedate).ToList();
                    var temp2 = tempd.Select(x => x.AP_Name == apname && x.AP_ID == Convert.ToDecimal(ID));
                    var i = 0;
                    foreach (var ss in temp2)
                    {
                        if (ss == true)
                        {
                            tempd[i].ACTIVE_FLAGINFO = "N";

                            break;
                        }
                        i++;
                    }
                    List<AWCconfig> temp3 = (from r in tempd where r.ACTIVE_FLAGINFO == "Y" select r).ToList();
                    Session["addeditap"] = tempd;
                    JsonResult o = new JsonResult();

                    //o = Json(temp3, JsonRequestBehavior.AllowGet);
                    o = Json(temp3, JsonRequestBehavior.AllowGet);

                    return o;
                    //}           

                }
                else
                {

                    var addnew = Session["addnewap"] as List<AWCconfig>;
                    addnew.RemoveAll(x => x.AP_Name == apname);
                    Session["addnewap"] = addnew;
                    return Json((List<AWCconfig>)Session["addnewap"], JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddCoverageInfo2(string data)
        {
            try
            {
                AWCinformation model = new JavaScriptSerializer().Deserialize<AWCinformation>(data);
                model.user = base.CurrentUser.UserName;

                var addnew = Session["addnewap"] as List<AWCconfig>;

                var command = new CreateAWCInfoCommand
                {
                    awcmodel = model,
                    awcmodelconfig = addnew

                };
                _CreateAWCInfoCommand.Handle(command);

                if (command.FlagDup == true)
                {
                    return Json(new { dup = "dup", dupname = command.dupname }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveAPConfig2(string ID, string apname, string sector, string fag, string siteid, string oldname, string oldsector, string MODEL,
            //string ipaddress, string impdate, string status, string imphase, string onservice, string ponumber, string apcom, string aplot,
            string oldipaddress, string oldimpdate, string oldstatus, string oldimphase, string oldonservice, string oldponumber, string oldapcom, string oldaplot)
        {   ////////////กรณี อัพเดทค่อ fag เป็น u และค่า ap อยู่ใน user ใช้อ้างเวลาอัพ
            try
            {
                Boolean inlistdb = false;
                Boolean inlistdb2 = false;

                //model2.AP_ID = Convert.ToDecimal(ID);
                //model2.AP_Name = apname;
                //model2.Sector = sector;
                //model2.ACTIVE_FLAGINFO = "Y";
                //model2.updatedate = DateTime.Now;
                //model2.user = base.CurrentUser.UserName;
                //model2.Site_id = Convert.ToDecimal(siteid);
                if (fag == "edit")
                {
                    AWCconfig model = new JavaScriptSerializer().Deserialize<AWCconfig>(MODEL);
                    var sss = DateTime.Now;
                    model.updatedate = sss;
                    model.user = base.CurrentUser.UserName;

                    var date1 = String.Format("{0:dd/MM/yyyy}", model.implement_date);
                    var date2 = String.Format("{0:dd/MM/yyyy}", model.onservice_date);

                    var addedit = Session["addeditap"] as List<AWCconfig>;
                    if (addedit == null)
                        addedit = new List<AWCconfig>();

                    var listall = new List<AWCconfig>();
                    listall = GetDataConfig(siteid);

                    var inlist = listall.Select(x => x.AP_Name);
                    if (inlist.Contains(apname))
                    {
                        inlistdb = true;
                    }
                    //}
                    foreach (var e in addedit)
                    {
                        var inlist2 = listall.Select(x => x.AP_Name);
                        if (inlist2.Contains(e.AP_Name))
                        {
                            inlistdb2 = true;
                            break;
                        }
                    }

                    if (inlistdb == false)
                    {
                        //var model = new AWCconfig();
                        //model.AP_Name = apname;
                        var command = new CreateAWCconfigCommand
                        {
                            AWCconfigModel = model
                        };
                        _CreateAWCconfigCommand.Handle(command);

                        if (command.FlagDup == true)
                        {
                            return Json("dup", JsonRequestBehavior.AllowGet);
                        }
                    }
                    //var checkdb = from l in listall where l.AP_ID == Convert.ToDecimal(ID) && l.AP_Name == oldname select l;
                    //if (checkdb.Any())
                    //{
                    //    //var command = new UpdateAWCconfigCommand
                    //    //{
                    //    //    user = base.CurrentUser.UserName,
                    //    //    AP_ID = Convert.ToDecimal(ID),
                    //    //    AP_NAME = apname,
                    //    //    SECTOR = sector
                    //    //};
                    //    //_UpdateAWCconfigCommand.Handle(command);

                    //    listall = GetDataConfig(siteid);
                    //    addedit.AddRange(listall);
                    //    var aa = addedit.DistinctBy(a => a.AP_Name);
                    //    var tempd = new List<AWCconfig>();
                    //    tempd = aa.OrderByDescending(a => a.updatedate).ToList();                        
                    //    Session["addeditap"] = tempd;

                    //    return Json((List<AWCconfig>)Session["addeditap"], JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{

                    //else
                    //{     

                    if (inlistdb2 == false)
                    {
                        addedit.AddRange(listall);
                    }
                    if (apname != oldname || (sector != oldsector || model.ip_address != oldipaddress || date1 != oldimpdate || model.status != oldstatus
                                || model.implement_phase != oldimphase || date2 != oldonservice || model.po_number != oldponumber || model.ap_company != oldapcom || model.ap_lot != oldaplot))
                    {
                        if (apname == oldname && (sector != oldsector || model.ip_address != oldipaddress || date1 != oldimpdate || model.status != oldstatus
                                    || model.implement_phase != oldimphase || date2 != oldonservice || model.po_number != oldponumber || model.ap_company != oldapcom || model.ap_lot != oldaplot))
                        {
                            var tempd = new List<AWCconfig>();
                            //var aa = addedit.DistinctBy(a => a.AP_Name);                                 
                            var temp2 = addedit.Select(x => x.AP_ID == Convert.ToDecimal(ID) && x.AP_Name == oldname);
                            var i = 0;
                            foreach (var ss in temp2)
                            {
                                if (ss == true)
                                {
                                    addedit[i].AP_ID = Convert.ToDecimal(ID);
                                    addedit[i].ACTIVE_FLAGINFO = "Y";
                                    addedit[i].AP_Name = apname;
                                    addedit[i].updatedate = DateTime.Now;
                                    addedit[i].user = base.CurrentUser.UserName;
                                    addedit[i].Sector = sector;

                                    addedit[i].ip_address = model.ip_address;
                                    addedit[i].implement_date = model.implement_date;
                                    addedit[i].implement_phase = model.implement_phase;
                                    addedit[i].status = model.status;
                                    addedit[i].po_number = model.po_number;
                                    addedit[i].onservice_date = model.onservice_date;
                                    addedit[i].ap_company = model.ap_company;
                                    addedit[i].ap_lot = model.ap_lot;

                                    break;
                                }
                                i++;
                            }
                            //addedit.Add(model2);
                            var aa = (from y in addedit where y.ACTIVE_FLAGINFO == "Y" || y.ACTIVE_FLAGINFO == "N" select y).ToList();
                            tempd = aa.OrderByDescending(a => a.updatedate).ToList();


                            var temp3 = from r in tempd where r.ACTIVE_FLAGINFO == "Y" select r;
                            Session["addeditap"] = tempd;
                            return Json((List<AWCconfig>)temp3.ToList(), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var sum = from s in addedit where s.AP_Name == apname && s.ACTIVE_FLAGINFO == "Y" select s;
                            if (sum.Any())
                            {
                                return Json("dup", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {

                                var tempd = new List<AWCconfig>();
                                //var aa = addedit.DistinctBy(a => a.AP_Name);                                 
                                var temp2 = addedit.Select(x => x.AP_ID == Convert.ToDecimal(ID) && x.AP_Name == oldname);
                                var i = 0;
                                foreach (var ss in temp2)
                                {
                                    if (ss == true)
                                    {
                                        addedit[i].AP_ID = Convert.ToDecimal(ID);
                                        addedit[i].ACTIVE_FLAGINFO = "Y";
                                        addedit[i].AP_Name = apname;
                                        addedit[i].updatedate = DateTime.Now;
                                        addedit[i].user = base.CurrentUser.UserName;
                                        addedit[i].Sector = sector;

                                        addedit[i].ip_address = model.ip_address;
                                        addedit[i].implement_date = model.implement_date;
                                        addedit[i].implement_phase = model.implement_phase;
                                        addedit[i].status = model.status;
                                        addedit[i].po_number = model.po_number;
                                        addedit[i].onservice_date = model.onservice_date;
                                        addedit[i].ap_company = model.ap_company;
                                        addedit[i].ap_lot = model.ap_lot;
                                        break;
                                    }
                                    i++;
                                }
                                //addedit.Add(model2);
                                var aa = (from y in addedit where y.ACTIVE_FLAGINFO == "Y" || y.ACTIVE_FLAGINFO == "N" select y).ToList();
                                tempd = aa.OrderByDescending(a => a.updatedate).ToList();


                                var temp3 = from r in tempd where r.ACTIVE_FLAGINFO == "Y" select r;
                                Session["addeditap"] = tempd;
                                return Json((List<AWCconfig>)temp3.ToList(), JsonRequestBehavior.AllowGet);
                            }
                        }
                        //var sum = from s in addedit where s.AP_Name == apname && s.ACTIVE_FLAGINFO == "Y" select s;
                        //if (sum.Any())
                        //{
                        //    return Json("dup", JsonRequestBehavior.AllowGet);
                        //}                      
                        //else
                        //{

                        //    var tempd = new List<AWCconfig>();
                        //    //var aa = addedit.DistinctBy(a => a.AP_Name);                                 
                        //    var temp2 = addedit.Select(x => x.AP_ID == Convert.ToDecimal(ID) && x.AP_Name == oldname);
                        //    var i = 0;
                        //    foreach (var ss in temp2)
                        //    {
                        //        if (ss == true)
                        //        {
                        //            addedit[i].AP_ID = Convert.ToDecimal(ID);
                        //            addedit[i].ACTIVE_FLAGINFO = "Y";
                        //            addedit[i].AP_Name = apname;
                        //            addedit[i].updatedate = DateTime.Now;
                        //            addedit[i].user = base.CurrentUser.UserName;
                        //            addedit[i].Sector = sector;
                        //            break;
                        //        }
                        //        i++;
                        //    }
                        //    //addedit.Add(model2);
                        //    var aa = (from y in addedit where y.ACTIVE_FLAGINFO == "Y" || y.ACTIVE_FLAGINFO == "N" select y).ToList();
                        //    tempd = aa.OrderByDescending(a => a.updatedate).ToList();


                        //    var temp3 = from r in tempd where r.ACTIVE_FLAGINFO == "Y" select r;
                        //    Session["addeditap"] = tempd;
                        //    return Json((List<AWCconfig>)temp3.ToList(), JsonRequestBehavior.AllowGet);
                        //}
                    }
                    else
                    {
                        var tempd = new List<AWCconfig>();
                        var aa = (from y in addedit where y.ACTIVE_FLAGINFO == "Y" || y.ACTIVE_FLAGINFO == "N" select y).ToList();
                        tempd = aa.OrderByDescending(a => a.updatedate).ToList();
                        var temp3 = from r in tempd where r.ACTIVE_FLAGINFO == "Y" select r;
                        Session["addeditap"] = tempd;
                        return Json(((List<AWCconfig>)temp3.ToList()), JsonRequestBehavior.AllowGet);
                    }
                    //}
                    //}           
                    //var command = new UpdateAWCconfigCommand
                    //{
                    //    user = base.CurrentUser.UserName,
                    //    AP_ID = Convert.ToDecimal(ID),
                    //    AP_NAME = apname,
                    //    SECTOR = sector
                    //};

                    //_UpdateAWCconfigCommand.Handle(command);

                    //if (command.FlagDup == true)
                    //{
                    //    return Json("dup", JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(true, JsonRequestBehavior.AllowGet);
                    //}
                }
                else
                {

                    //AWCconfig model = new AWCconfig();
                    //model.user = base.CurrentUser.UserName;
                    AWCconfig model = new JavaScriptSerializer().Deserialize<AWCconfig>(MODEL);
                    var sss = DateTime.Now;
                    model.updatedate = sss;
                    model.user = base.CurrentUser.UserName;
                    //model.AP_Name = apname;
                    //model.Sector = sector;
                    //model.ip_address = ipaddress;
                    //model.implement_phase = imphase;
                    //model.implement_date = DateTime.Parse(impdate);
                    //model.onservice_date = DateTime.Parse(onservice);
                    //model.po_number = ponumber;
                    //model.ap_company = apcom;
                    //model.ap_lot = aplot;
                    //model.status = status;
                    var date1 = String.Format("{0:dd/MM/yyyy}", model.implement_date);
                    var date2 = String.Format("{0:dd/MM/yyyy}", model.onservice_date);

                    if (apname != oldname || (sector != oldsector || model.ip_address != oldipaddress || date1 != oldimpdate || model.status != oldstatus
                                || model.implement_phase != oldimphase || date2 != oldonservice || model.po_number != oldponumber || model.ap_company != oldapcom || model.ap_lot != oldaplot))
                    {

                        var command = new CreateAWCconfigCommand
                        {
                            AWCconfigModel = model
                        };
                        _CreateAWCconfigCommand.Handle(command);

                        if (command.FlagDup == true)
                        {
                            return Json("dup", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (apname == oldname && (sector != oldsector || model.ip_address != oldipaddress || date1 != oldimpdate || model.status != oldstatus
                                || model.implement_phase != oldimphase || date2 != oldonservice || model.po_number != oldponumber || model.ap_company != oldapcom || model.ap_lot != oldaplot))
                            {
                                var addnew = Session["addnewap"] as List<AWCconfig>;
                                addnew.RemoveAll(x => x.AP_Name == oldname);
                                addnew.Add(model);
                                var tempd = new List<AWCconfig>();
                                tempd = addnew.OrderByDescending(a => a.updatedate).ToList();
                                Session["addnewap"] = tempd;
                                return Json((List<AWCconfig>)Session["addnewap"], JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var addnew = Session["addnewap"] as List<AWCconfig>;
                                var sum = from s in addnew where s.AP_Name == apname select s;
                                if (sum.Any())
                                {
                                    return Json("dup", JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    addnew.RemoveAll(x => x.AP_Name == oldname);
                                    addnew.Add(model);
                                    var tempd = new List<AWCconfig>();
                                    tempd = addnew.OrderByDescending(a => a.updatedate).ToList();
                                    Session["addnewap"] = tempd;
                                    return Json((List<AWCconfig>)Session["addnewap"], JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                    else
                    {
                        // var addnew = Session["addnewap"] as List<AWCconfig>;
                        //// addnew.RemoveAll(x => x.AP_Name == oldname);
                        //// addnew.Add(model);
                        //var tempd = new List<AWCconfig>();
                        ////tempd = addnew.OrderByDescending(a => a.updatedate).ToList();
                        //Session["addnewap"] = tempd;
                        return Json((List<AWCconfig>)Session["addnewap"], JsonRequestBehavior.AllowGet);

                    }
                }

            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletetempAP2()
        {
            try
            {
                var addedit = Session["addeditap"] as List<AWCconfig>;
                if (addedit != null)
                    Session.Remove("addeditap");
                var addnew = Session["addnewap"] as List<AWCconfig>;
                if (addnew != null)
                    Session.Remove("addnewap");

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveInfo2(string data, string siteid = "", string oldbasename = "", string oldsitename = "")
        {
            try
            {/////ส่งผ่านค่า active fagมา สำหรับค่า base กับ sitename ที่เป็นชื่อเก่า
                AWCinformation model = new JavaScriptSerializer().Deserialize<AWCinformation>(data);
                model.user = base.CurrentUser.UserName;
                var addnew = Session["addeditap"] as List<AWCconfig>;

                var command = new UpdateAWCInfoCommand
                {
                    awcmodel = model,
                    awcmodelconfig = addnew,
                    oldbasename = oldbasename,
                    oldsitename = oldsitename
                };
                _UpdateAWCInfoCommand.Handle(command);

                if (command.FlagDup == true)
                    return Json(new { dup = "dup", dupname = command.dupname }, JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

    }
}


