using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    [CustomActionFilter]
    [IENoCache]
    public class ConfigurationEventsController : FBBConfigController
    {
        //
        // GET: /ConfigurationEvents/

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveConfigEventCommand> _SaveConfigEventCommand;


        public ConfigurationEventsController(ILogger logger,
             IQueryProcessor queryProcessor,
            ICommandHandler<SaveConfigEventCommand> saveConfigEventCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _SaveConfigEventCommand = saveConfigEventCommand;
        }

        private void SetViewBagLov()
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == "FBBOR013").ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        public ActionResult Index(string SaveStatus = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            Session["SubcontractListDataTmp"] = null;
            Session["SubcontractSearchTmp"] = null;
            ViewBag.SaveStatus = SaveStatus;
            return View();
        }

        public ActionResult ConfigurationEventsEdit(string EventCode, string HasSearch)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            DateTime Today = DateTime.Now;

            ConfigurationEventSubContactSearchData Model = new ConfigurationEventSubContactSearchData();
            string ValidateStaff = "";
            if (Session["ValidateAddSubContact"] != null)
            {
                ConfigurationEventSubContactData ValidateAddSubContact = (ConfigurationEventSubContactData)Session["ValidateAddSubContact"];
                ValidateStaff = ValidateAddSubContact.StaffValidateFail;
                Session["ValidateAddSubContact"] = null;
                Model.hastmp = "Y";
            }
            Model.StaffValidateFail = ValidateStaff;
            List<ConfigurationEventSubContactData> configurationEventSubContactDataList = new List<ConfigurationEventSubContactData>();
            if (EventCode == "N")
            {
                if (HasSearch == null)
                {
                    Model.service_option = "N";
                    Model.start_date = Today.AddDays(1).ToString("dd/MM/yyyy");
                    Model.end_date = Today.AddDays(8).ToString("dd/MM/yyyy");
                    Model.ConfigurationEventSubContactDataList = configurationEventSubContactDataList;
                }
                else
                {
                    ConfigurationEventSearchData searchModel = (ConfigurationEventSearchData)Session["SubcontractSearchTmp"];
                    Model.service_option = "N";
                    Model.event_code = searchModel.event_code;
                    Model.technology = searchModel.technology;
                    Model.provice = searchModel.provice;
                    Model.district = searchModel.amphur;
                    Model.sub_district = searchModel.tumbon;
                    Model.post_code = searchModel.zipcode;
                    Model.start_date = searchModel.effective_date;
                    Model.end_date = searchModel.expire_date;
                    Model.plug_and_play_flag_bool = searchModel.plug_and_play_flag_bool;
                    Model.plug_and_play_flag = searchModel.plug_and_play_flag;
                    if (Session["SubcontractListDataTmp"] != null)
                    {
                        Model.ConfigurationEventSubContactDataList = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];
                        Model.hastmp = "Y";
                        Model.ShownButtom = "";
                    }
                    else
                    {
                        Model.ConfigurationEventSubContactDataList = configurationEventSubContactDataList;
                    }
                }
            }
            else
            {

                List<ConfigurationEventData> results = GetDataConfigurationEventByEventCode(EventCode);

                if (results != null && results.Count > 0)
                {
                    ConfigurationEventData result = results.FirstOrDefault();

                    // Set Tmp Search
                    if (Session["SubcontractSearchTmp"] == null)
                    {
                        ConfigurationEventSearchData searchModel = new ConfigurationEventSearchData();
                        searchModel.service_option = "E";
                        searchModel.event_code = result.event_code;
                        searchModel.technology = result.technology;
                        searchModel.provice = result.provice;
                        searchModel.amphur = result.amphur;
                        searchModel.tumbon = result.tumbon;
                        searchModel.zipcode = result.zipcode;
                        searchModel.effective_date = result.effective_date;
                        searchModel.expire_date = result.expire_date;
                        Session["SubcontractSearchTmp"] = searchModel;

                        // Set MasterData

                        var SubContactMasterData = GetDataSearchSubContactMaster(searchModel);
                        SetSubContactMasterDataToSesion(SubContactMasterData);

                        // End Set MasterData
                    }

                    Model.service_option = "E";
                    Model.event_code = result.event_code;
                    Model.technology = result.technology;
                    Model.provice = result.provice;
                    Model.district = result.amphur;
                    Model.sub_district = result.tumbon;
                    Model.post_code = result.zipcode;
                    Model.start_date = result.effective_date;
                    Model.end_date = result.expire_date;
                    Model.hastmp = "Y";
                    Model.plug_and_play_flag = result.plug_and_play_flag;
                    if (result.plug_and_play_flag == "Y")
                    {
                        Model.plug_and_play_flag_bool = true;
                    }
                    else
                    {
                        Model.plug_and_play_flag_bool = false;
                    }

                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
                    DateTime Start_date = DateTime.ParseExact(result.effective_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    Model.ShownButtom = "";
                    if (Today > Start_date)
                    {
                        Model.ShownButtom = "N";
                    }

                    if (Session["SubcontractListDataTmp"] == null)
                    {
                        var GroupResults = results.Select(p => new { p.sub_location_id, p.sub_contract_name, p.sub_team_id, p.sub_team_name, p.event_start_date, p.event_end_date }).Distinct().ToList();

                        foreach (var item in GroupResults)
                        {
                            ConfigurationEventSubContactData configurationEventSubContactData = new ConfigurationEventSubContactData();
                            configurationEventSubContactData.service_option = "E";
                            configurationEventSubContactData.sub_contact_id = item.sub_location_id;
                            configurationEventSubContactData.sub_contact = item.sub_contract_name;
                            configurationEventSubContactData.sub_team_id = item.sub_team_id;
                            configurationEventSubContactData.sub_team = item.sub_team_name;
                            configurationEventSubContactData.start_date_event = item.event_start_date;
                            configurationEventSubContactData.end_date_event = item.event_end_date;

                            var SubNameList = results.Where(p => p.sub_location_id == item.sub_location_id && p.sub_contract_name == item.sub_contract_name && p.sub_team_id == item.sub_team_id && p.sub_team_name == item.sub_team_name && p.event_start_date == item.event_start_date && p.event_end_date == item.event_end_date).ToList();
                            configurationEventSubContactData.SubNameList = new List<SubNameData>();
                            foreach (var SubName in SubNameList)
                            {
                                SubNameData subNameData = new SubNameData();
                                subNameData.sub_name_id = SubName.install_staff_id;
                                subNameData.sub_name = SubName.install_staff_name;
                                subNameData.sub_name_select = true;
                                subNameData.sub_name_select_old = true;
                                configurationEventSubContactData.SubNameList.Add(subNameData);
                            }
                            configurationEventSubContactDataList.Add(configurationEventSubContactData);
                        }
                        Model.ConfigurationEventSubContactDataList = configurationEventSubContactDataList;
                        Session["SubcontractListDataTmp"] = configurationEventSubContactDataList;
                    }
                    else
                    {
                        Model.ConfigurationEventSubContactDataList = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];
                    }
                }
                else
                {

                }

            }

            return View(Model);
        }

        public ActionResult ConfigurationEventsSubContactEdit(string IndexValue = "", string EventCode = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            List<SubcontractForDDL> MasterSubcontractData = (List<SubcontractForDDL>)Session["SubcontractTmp"];
            List<SubcontractTeamForDDL> MasterTeamData = (List<SubcontractTeamForDDL>)Session["SubcontractTeamTmp"];
            List<SubcontractStaffForDDL> MasterStaffData = (List<SubcontractStaffForDDL>)Session["SubcontractStaffTmp"];
            ConfigurationEventSubContactData Model = new ConfigurationEventSubContactData();
            Model.event_code = EventCode;

            if (IndexValue == "")
            {
                SubcontractForDDL MasterSubcontract = MasterSubcontractData.FirstOrDefault();
                SubcontractTeamForDDL MasterTeam = MasterTeamData.Where(p => p.Subcontract_Code == MasterSubcontract.Subcontract_Code).FirstOrDefault();

                ConfigurationEventSearchData searchModel = (ConfigurationEventSearchData)Session["SubcontractSearchTmp"];

                Model.service_option = "N";
                Model.indexData = IndexValue;
                Model.start_date_subcontact = searchModel.effective_date;
                Model.end_date_subcontact = searchModel.expire_date;
                Model.start_date_event = searchModel.effective_date;
                Model.end_date_event = searchModel.expire_date;
                Model.plug_and_play_flag = searchModel.plug_and_play_flag;
                Model.plug_and_play_flag_bool = searchModel.plug_and_play_flag_bool;
                Model.sub_contact = MasterSubcontract.Subcontract_Name;
                Model.sub_contact_id = MasterSubcontract.Subcontract_Code;
                Model.sub_team = MasterTeam.Subcontract_Team_Name;
                Model.sub_team_id = MasterTeam.Subcontract_Team_Id;
                Model.SubNameList = new List<SubNameData>();
                foreach (var item in MasterStaffData.Where(p => p.Subcontract_Name == Model.sub_contact && p.Subcontract_Code == Model.sub_contact_id && p.Subcontract_Team_Id == Model.sub_team_id).ToList())
                {
                    SubNameData subNameData = new SubNameData();
                    subNameData.sub_name = item.staff_Name;
                    subNameData.sub_name_id = item.staff_Code;
                    Model.SubNameList.Add(subNameData);
                }


            }
            else
            {
                ConfigurationEventSearchData searchModel = (ConfigurationEventSearchData)Session["SubcontractSearchTmp"];
                List<ConfigurationEventSubContactData> SubcontractListData = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];
                ConfigurationEventSubContactData SubcontractData = SubcontractListData[int.Parse(IndexValue)];

                string SubContactID = "";
                if (SubcontractData.sub_contact_id != null)
                {
                    SubContactID = SubcontractData.sub_contact_id;
                }

                string SubTeamID = "";
                if (SubcontractData.sub_team_id != null)
                {
                    SubTeamID = SubcontractData.sub_team_id;
                }


                Model.service_option = "E";
                Model.indexData = IndexValue;
                Model.start_date_subcontact = searchModel.effective_date;
                Model.end_date_subcontact = searchModel.expire_date;
                Model.start_date_event = SubcontractData.start_date_event;
                Model.end_date_event = SubcontractData.end_date_event;
                Model.sub_contact = SubcontractData.sub_contact;
                Model.sub_contact_id = SubContactID;
                Model.sub_team = SubcontractData.sub_team;
                Model.sub_team_id = SubTeamID;
                Model.SubNameList = new List<SubNameData>();

                var SelectTeam = MasterTeamData.Where(p => p.Subcontract_Code == SubContactID
                    && p.Subcontract_Name == SubcontractData.sub_contact && p.Subcontract_Team_Id == SubTeamID
                    && p.Subcontract_Team_Name == SubcontractData.sub_team).ToList();
                if (SelectTeam.Count == 0)
                {
                    var StaffinDB = SelectStaffInDB(Model.event_code, SubContactID, Model.sub_contact, SubTeamID, Model.sub_team);
                    foreach (var item in StaffinDB)
                    {
                        SubNameData subNameData = new SubNameData();
                        subNameData.sub_name = item.sub_name;
                        subNameData.sub_name_id = item.sub_name_id;

                        var SubNameOld = SubcontractData.SubNameList.Where(p => p.sub_name == item.sub_name && p.sub_name_id == item.sub_name_id).ToList();
                        if (SubNameOld.Count > 0)
                        {
                            subNameData.sub_name_select = true;
                            subNameData.sub_name_select_old = true;
                        }
                        else
                        {
                            subNameData.sub_name_select = false;
                            subNameData.sub_name_select_old = false;
                        }

                        Model.SubNameList.Add(subNameData);
                    }
                }
                else
                {
                    var MasterStaffDataFilter = MasterStaffData.Where(p => p.Subcontract_Code == SubContactID
                     && p.Subcontract_Name == SubcontractData.sub_contact && p.Subcontract_Team_Id == SubTeamID).ToList();
                    foreach (var item in MasterStaffDataFilter)
                    {
                        SubNameData subNameData = new SubNameData();
                        subNameData.sub_name = item.staff_Name;
                        subNameData.sub_name_id = item.staff_Code;

                        var SubNameOld = SubcontractData.SubNameList.Where(p => p.sub_name == item.staff_Name && p.sub_name_id == item.staff_Code).ToList();
                        if (SubNameOld.Count > 0)
                        {
                            subNameData.sub_name_select = true;
                            subNameData.sub_name_select_old = true;
                        }
                        else
                        {
                            subNameData.sub_name_select = false;
                            subNameData.sub_name_select_old = false;
                        }
                        Model.SubNameList.Add(subNameData);
                    }
                }

            }
            if (Model.service_option == "E")
            {

                return View(Model);
            }
            else
            {
                return View("ConfigurationEventsSubContactNew", Model);
            }
        }

        public ActionResult SearchDataSourceRequest([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<ConfigurationEventsSearchModel>(dataS);
                var result = GetDataSearchModel(searchEventModel);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }

        public ActionResult AddSubContact(string SubContactIndex, string StartDate, string EndDate, string ShownButtom = "")
        {
            ViewBag.SubContactIndex = SubContactIndex;
            SetViewBagLov();

            var SubcontractListData = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];

            var SubcontractData = SubcontractListData[int.Parse(SubContactIndex)];

            ConfigurationEventSubContactData Model = new ConfigurationEventSubContactData();
            Model.ShownButtom = ShownButtom;
            Model.SubNameList = new List<SubNameData>();
            foreach (var item in SubcontractData.SubNameList)
            {
                SubNameData subName = new SubNameData();
                subName.sub_name = item.sub_name;
                subName.sub_name_id = item.sub_name_id;
                subName.sub_name_select = true;
                subName.sub_name_select_old = true;
                Model.SubNameList.Add(subName);
            }
            Model.service_option = SubcontractData.service_option;
            Model.sub_contact_id = SubcontractData.sub_contact_id;
            Model.sub_contact = SubcontractData.sub_contact;
            Model.sub_team_id = SubcontractData.sub_team_id;
            Model.sub_team = SubcontractData.sub_team;
            Model.start_date_event = SubcontractData.start_date_event;
            Model.end_date_event = SubcontractData.end_date_event;
            Model.start_date_subcontact = StartDate;
            Model.end_date_subcontact = EndDate;
            Model.is_delete = SubcontractData.is_delete;

            List<ConfigurationEventSubContactData> ModelList = new List<ConfigurationEventSubContactData>();
            ModelList.Add(Model);

            return PartialView("_SubContact", ModelList);
        }

        public ActionResult SaveConfigurationEvent(ConfigurationEventSubContactSearchData Model)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var User = base.CurrentUser;
            Model.user = User.UserName;

            string SaveStatus = "";
            SaveStatus = SaveConfigEventHandle(Model);

            return RedirectToAction("Index", "ConfigurationEvents", new { SaveStatus = SaveStatus });
        }

        private string SaveConfigEventHandle(ConfigurationEventSubContactSearchData Model)
        {
            SaveConfigEventCommand command = new SaveConfigEventCommand();
            command.user = Model.user;
            command.technology = Model.technology;
            command.provice = Model.provice;
            command.amphur = Model.district;
            command.tumbon = Model.sub_district;
            command.zipcode = Model.post_code;
            command.effective_date = Model.start_date;
            command.expire_date = Model.end_date;
            if (Model.plug_and_play_flag_bool)
            {
                command.plug_and_play_flag = "Y";
            }
            else
            {
                command.plug_and_play_flag = "N";
            }

            bool CheckSubContactServiceOption = true;
            if (Model.service_option == "N")
            {
                command.event_code = "";
                command.service_option = "N";

                List<FbbEventSub> FbbEventSubList = new List<FbbEventSub>();
                foreach (var item in Model.ConfigurationEventSubContactDataList)
                {
                    if (item.is_delete)
                    {
                        CheckSubContactServiceOption = CheckSubContactServiceOption & true;
                    }
                    else
                    {
                        foreach (var SubName in item.SubNameList)
                        {
                            FbbEventSub fbbEventSub = new FbbEventSub();
                            fbbEventSub.SERVICE_OPTION = "N";
                            fbbEventSub.EVENT_CODE = "";
                            fbbEventSub.EVENT_START_DATE = item.start_date_event;
                            fbbEventSub.EVENT_END_DATE = item.end_date_event;
                            fbbEventSub.SUB_LOCATION_ID = item.sub_contact_id;
                            fbbEventSub.SUB_CONTRACT_NAME = item.sub_contact;
                            fbbEventSub.SUB_TEAM_ID = item.sub_team_id;
                            fbbEventSub.SUB_TEAM_NAME = item.sub_team;
                            fbbEventSub.INSTALL_STAFF_ID = SubName.sub_name_id;
                            fbbEventSub.INSTALL_STAFF_NAME = SubName.sub_name;
                            fbbEventSub.SUB_ROW_ID = "";

                            FbbEventSubList.Add(fbbEventSub);
                        }
                        CheckSubContactServiceOption = CheckSubContactServiceOption & false;
                    }
                }
                command.fbbEventSubArray = FbbEventSubList;
            }
            else if (Model.service_option == "E")
            {
                List<ConfigurationEventData> results = GetDataConfigurationEventByEventCode(Model.event_code);
                if (results != null && results.Count > 0)
                {

                    command.event_code = Model.event_code;

                    List<FbbEventSub> FbbEventSubList = new List<FbbEventSub>();
                    foreach (var item in Model.ConfigurationEventSubContactDataList)
                    {
                        if (item.is_delete)
                        {
                            if (item.service_option == "E")
                            {
                                foreach (var SubName in item.SubNameList)
                                {
                                    FbbEventSub fbbEventSub = new FbbEventSub();

                                    var result = results.Where(p => p.event_code == Model.event_code && p.event_start_date == item.start_date_event
                                        && p.event_end_date == item.end_date_event && p.sub_location_id == item.sub_contact_id
                                        && p.sub_contract_name == item.sub_contact && p.sub_team_id == item.sub_team_id && p.sub_team_name == item.sub_team
                                        && p.install_staff_id == SubName.sub_name_id && p.install_staff_name == SubName.sub_name).ToList();
                                    if (result.Count > 0)
                                    {
                                        var ItemData = result.FirstOrDefault();
                                        fbbEventSub.SERVICE_OPTION = "D";
                                        fbbEventSub.SUB_ROW_ID = ItemData.sub_row_id;
                                        fbbEventSub.EVENT_CODE = Model.event_code;
                                        fbbEventSub.EVENT_START_DATE = item.start_date_event;
                                        fbbEventSub.EVENT_END_DATE = item.end_date_event;
                                        fbbEventSub.SUB_LOCATION_ID = item.sub_contact_id;
                                        fbbEventSub.SUB_CONTRACT_NAME = item.sub_contact;
                                        fbbEventSub.SUB_TEAM_ID = item.sub_team_id;
                                        fbbEventSub.SUB_TEAM_NAME = item.sub_team;
                                        fbbEventSub.INSTALL_STAFF_ID = SubName.sub_name_id;
                                        fbbEventSub.INSTALL_STAFF_NAME = SubName.sub_name;

                                        FbbEventSubList.Add(fbbEventSub);
                                    }
                                }
                            }

                            CheckSubContactServiceOption = CheckSubContactServiceOption & true;
                        }
                        else
                        {
                            if (item.service_option == "N")
                            {
                                foreach (var SubName in item.SubNameList)
                                {
                                    FbbEventSub fbbEventSub = new FbbEventSub();
                                    fbbEventSub.SERVICE_OPTION = "N";
                                    fbbEventSub.EVENT_CODE = Model.event_code;
                                    fbbEventSub.EVENT_START_DATE = item.start_date_event;
                                    fbbEventSub.EVENT_END_DATE = item.end_date_event;
                                    fbbEventSub.SUB_LOCATION_ID = item.sub_contact_id;
                                    fbbEventSub.SUB_CONTRACT_NAME = item.sub_contact;
                                    fbbEventSub.SUB_TEAM_ID = item.sub_team_id;
                                    fbbEventSub.SUB_TEAM_NAME = item.sub_team;
                                    fbbEventSub.INSTALL_STAFF_ID = SubName.sub_name_id;
                                    fbbEventSub.INSTALL_STAFF_NAME = SubName.sub_name;
                                    fbbEventSub.SUB_ROW_ID = "";

                                    FbbEventSubList.Add(fbbEventSub);
                                }
                            }
                            else
                            {
                                var resultTmp = results.Where(p => p.event_code == Model.event_code && p.event_start_date == item.start_date_event
                                            && p.event_end_date == item.end_date_event && p.sub_location_id == item.sub_contact_id
                                            && p.sub_contract_name == item.sub_contact && p.sub_team_id == item.sub_team_id
                                            && p.sub_team_name == item.sub_team).ToList();

                                foreach (var SubName in item.SubNameList)
                                {
                                    FbbEventSub fbbEventSub = new FbbEventSub();
                                    var result = results.Where(p => p.event_code == Model.event_code && p.event_start_date == item.start_date_event
                                            && p.event_end_date == item.end_date_event && p.sub_location_id == item.sub_contact_id
                                            && p.sub_contract_name == item.sub_contact && p.sub_team_id == item.sub_team_id && p.sub_team_name == item.sub_team
                                            && p.install_staff_id == SubName.sub_name_id && p.install_staff_name == SubName.sub_name).ToList();

                                    if (result.Count > 0)
                                    {
                                        var ItemData = result.FirstOrDefault();
                                        fbbEventSub.SERVICE_OPTION = "E";
                                        fbbEventSub.SUB_ROW_ID = ItemData.sub_row_id;
                                        fbbEventSub.EVENT_CODE = Model.event_code;
                                        fbbEventSub.EVENT_START_DATE = item.start_date_event;
                                        fbbEventSub.EVENT_END_DATE = item.end_date_event;
                                        fbbEventSub.SUB_LOCATION_ID = item.sub_contact_id;
                                        fbbEventSub.SUB_CONTRACT_NAME = item.sub_contact;
                                        fbbEventSub.SUB_TEAM_ID = item.sub_team_id;
                                        fbbEventSub.SUB_TEAM_NAME = item.sub_team;
                                        fbbEventSub.INSTALL_STAFF_ID = SubName.sub_name_id;
                                        fbbEventSub.INSTALL_STAFF_NAME = SubName.sub_name;

                                        FbbEventSubList.Add(fbbEventSub);
                                        resultTmp.Remove(ItemData);
                                    }
                                    else
                                    {
                                        var ItemData = result.FirstOrDefault();
                                        fbbEventSub.SERVICE_OPTION = "N";
                                        fbbEventSub.SUB_ROW_ID = "";
                                        fbbEventSub.EVENT_CODE = Model.event_code;
                                        fbbEventSub.EVENT_START_DATE = item.start_date_event;
                                        fbbEventSub.EVENT_END_DATE = item.end_date_event;
                                        fbbEventSub.SUB_LOCATION_ID = item.sub_contact_id;
                                        fbbEventSub.SUB_CONTRACT_NAME = item.sub_contact;
                                        fbbEventSub.SUB_TEAM_ID = item.sub_team_id;
                                        fbbEventSub.SUB_TEAM_NAME = item.sub_team;
                                        fbbEventSub.INSTALL_STAFF_ID = SubName.sub_name_id;
                                        fbbEventSub.INSTALL_STAFF_NAME = SubName.sub_name;

                                        FbbEventSubList.Add(fbbEventSub);
                                    }
                                }
                                if (resultTmp != null && resultTmp.Count > 0)
                                {
                                    foreach (var ItemData in resultTmp)
                                    {
                                        FbbEventSub fbbEventSub = new FbbEventSub();
                                        fbbEventSub.SERVICE_OPTION = "D";
                                        fbbEventSub.SUB_ROW_ID = ItemData.sub_row_id;
                                        fbbEventSub.EVENT_CODE = Model.event_code;
                                        fbbEventSub.EVENT_START_DATE = item.start_date_event;
                                        fbbEventSub.EVENT_END_DATE = item.end_date_event;
                                        fbbEventSub.SUB_LOCATION_ID = item.sub_contact_id;
                                        fbbEventSub.SUB_CONTRACT_NAME = item.sub_contact;
                                        fbbEventSub.SUB_TEAM_ID = item.sub_team_id;
                                        fbbEventSub.SUB_TEAM_NAME = item.sub_team;
                                        fbbEventSub.INSTALL_STAFF_ID = ItemData.install_staff_id;
                                        fbbEventSub.INSTALL_STAFF_NAME = ItemData.install_staff_name;

                                        FbbEventSubList.Add(fbbEventSub);
                                    }
                                }


                            }
                            CheckSubContactServiceOption = CheckSubContactServiceOption & false;
                        }
                    }
                    command.fbbEventSubArray = FbbEventSubList;

                    if (CheckSubContactServiceOption)
                    {
                        command.service_option = "D";
                    }
                    else
                    {
                        command.service_option = "E";
                    }
                }

            }
            try
            {
                if (command.service_option == "N")
                {
                    if (!CheckSubContactServiceOption)
                    {
                        _SaveConfigEventCommand.Handle(command);
                    }
                }
                else
                {
                    _SaveConfigEventCommand.Handle(command);
                }
            }
            catch (Exception ex)
            {
                return "ERROR Save Event config";
            }

            string EventCode = command.event_code;

            try
            {
                // Service ReserveCapability
                string Status = "";
                ReserveCapabilityQuery query = new ReserveCapabilityQuery();
                query.EventCode = EventCode;
                var DataForSent = command.fbbEventSubArray.Where(p => p.SERVICE_OPTION != "E").ToList();
                var GroupDataForSent = DataForSent.Select(p => new { p.SUB_LOCATION_ID, p.SUB_TEAM_ID, p.EVENT_START_DATE, p.EVENT_END_DATE }).Distinct().ToList();
                foreach (var item in GroupDataForSent)
                {
                    string Event_Date_From = item.EVENT_START_DATE.Replace("/", "-");
                    string Event_Date_To = item.EVENT_END_DATE.Replace("/", "-");

                    var DataForSentFilter = DataForSent.Where(p => p.SUB_LOCATION_ID == item.SUB_LOCATION_ID && p.SUB_TEAM_ID == item.SUB_TEAM_ID
                        && p.EVENT_START_DATE == item.EVENT_START_DATE && p.EVENT_END_DATE == item.EVENT_END_DATE).ToList();
                    var DataForSentFilterN = DataForSent.Where(p => p.SUB_LOCATION_ID == item.SUB_LOCATION_ID && p.SUB_TEAM_ID == item.SUB_TEAM_ID
                        && p.EVENT_START_DATE == item.EVENT_START_DATE && p.EVENT_END_DATE == item.EVENT_END_DATE && p.SERVICE_OPTION == "N").ToList();
                    var DataForSentFilterD = DataForSent.Where(p => p.SUB_LOCATION_ID == item.SUB_LOCATION_ID && p.SUB_TEAM_ID == item.SUB_TEAM_ID
                        && p.EVENT_START_DATE == item.EVENT_START_DATE && p.EVENT_END_DATE == item.EVENT_END_DATE && p.SERVICE_OPTION == "D").ToList();

                    if (DataForSentFilterN.Count > DataForSentFilterD.Count)
                    {
                        query.Service_Option = "N";
                        ReserveSubcontract reserveSubcontract = new ReserveSubcontract();
                        reserveSubcontract.Subcontract_Location_Code = item.SUB_LOCATION_ID;
                        reserveSubcontract.Subcontract_Team_Id = item.SUB_TEAM_ID;
                        reserveSubcontract.Event_Date_From = Event_Date_From;
                        reserveSubcontract.Event_Date_To = Event_Date_To;
                        reserveSubcontract.Capacity_Amount = (DataForSentFilterN.Count - DataForSentFilterD.Count).ToString();
                        reserveSubcontract.Post_Code = Model.post_code;
                        reserveSubcontract.Sub_District = Model.sub_district;

                        query.ReserveSubcontract = reserveSubcontract;

                        System.Threading.Thread.Sleep(2000);
                        Status = ReserveCapability(query);
                        if (Status == "-1")
                        {
                            break;
                        }
                    }
                    else if (DataForSentFilterN.Count < DataForSentFilterD.Count)
                    {
                        query.Service_Option = "D";
                        ReserveSubcontract reserveSubcontract = new ReserveSubcontract();
                        reserveSubcontract.Subcontract_Location_Code = item.SUB_LOCATION_ID;
                        reserveSubcontract.Subcontract_Team_Id = item.SUB_TEAM_ID;
                        reserveSubcontract.Event_Date_From = Event_Date_From;
                        reserveSubcontract.Event_Date_To = Event_Date_To;
                        reserveSubcontract.Capacity_Amount = (DataForSentFilterD.Count - DataForSentFilterN.Count).ToString();
                        reserveSubcontract.Post_Code = Model.post_code;
                        reserveSubcontract.Sub_District = Model.sub_district;

                        query.ReserveSubcontract = reserveSubcontract;

                        System.Threading.Thread.Sleep(2000);
                        Status = ReserveCapability(query);
                        if (Status == "-1")
                        {
                            break;
                        }
                    }
                }
                if (Status == "-1")
                {
                    return "Please contact your system administrator. Event code: " + EventCode;
                }
                else
                {
                    return "Saved successfully.Event code: " + EventCode;
                }
            }
            catch (Exception ex)
            {
                return "Please contact your system administrator. Event code: " + EventCode;
            }
        }

        public string SearchDataSubContactMaster(string technology, string provice, string amphur, string tumbon, string zipcode, string effective_date, string expire_date, string plug_and_play_flag)
        {
            ConfigurationEventSearchData searchModel = new ConfigurationEventSearchData();
            searchModel.service_option = "N";
            searchModel.technology = technology;
            searchModel.provice = provice;
            searchModel.amphur = amphur;
            searchModel.tumbon = tumbon;
            searchModel.zipcode = zipcode;
            searchModel.effective_date = effective_date;
            searchModel.expire_date = expire_date;
            if (plug_and_play_flag == "true")
            {
                searchModel.plug_and_play_flag = "Y";
                searchModel.plug_and_play_flag_bool = true;
            }
            else
            {
                searchModel.plug_and_play_flag = "N";
                searchModel.plug_and_play_flag_bool = false;
            }

            Session["SubcontractSearchTmp"] = searchModel;
            var SubContactMasterData = GetDataSearchSubContactMaster(searchModel);
            SetSubContactMasterDataToSesion(SubContactMasterData);
            if (SubContactMasterData != null && SubContactMasterData.Count > 0)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        private List<ConfigurationEventSearchData> GetDataSearchModel(ConfigurationEventsSearchModel searchawcModel)
        {

            var query = new GetAWConfigurationEventSearchQuery()
            {
                EventCode = searchawcModel.EventCode ?? ""
            };
            List<ConfigurationEventSearchData> result;
            result = _queryProcessor.Execute(query);
            return result;
        }

        private List<ConfigurationEventData> GetDataConfigurationEventByEventCode(string EventCode)
        {

            var query = new GetAWConfigurationEventQuery()
            {
                EventCode = EventCode ?? ""
            };
            List<ConfigurationEventData> result;
            result = _queryProcessor.Execute(query);
            return result;
        }

        private List<CapabilityData> GetDataSearchSubContactMaster(ConfigurationEventSearchData searchModel)
        {

            var query = new GetCapabilityQuery()
            {
                Lang_Flag = "TH",
                Technology = searchModel.technology ?? "",
                Sub_District = searchModel.tumbon ?? "",
                Post_Code = searchModel.zipcode ?? "",
                Event_Start_Date = searchModel.effective_date ?? "",
                Event_End_Date = searchModel.expire_date ?? ""
            };
            List<CapabilityData> result;
            result = _queryProcessor.Execute(query);

            return result;
        }

        private void SetSubContactMasterDataToSesion(List<CapabilityData> CapabilityDataList)
        {
            List<SubcontractForDDL> subcontractForDDLList = new List<SubcontractForDDL>();
            List<SubcontractTeamForDDL> subcontractTeamForDDLList = new List<SubcontractTeamForDDL>();
            List<SubcontractStaffForDDL> subcontractStaffForDDLList = new List<SubcontractStaffForDDL>();
            if (CapabilityDataList != null)
            {
                foreach (var CapabilityData in CapabilityDataList)
                {
                    SubcontractForDDL subcontractForDDL = new SubcontractForDDL();
                    string Subcontract_Code = CapabilityData.subcontract_Location_CodeField;
                    string Subcontract_Name = CapabilityData.subcontract_Company_NameField;

                    subcontractForDDL.Subcontract_Code = Subcontract_Code;
                    subcontractForDDL.Subcontract_Name = Subcontract_Name;
                    subcontractForDDLList.Add(subcontractForDDL);
                    foreach (var CapabilityTeam in CapabilityData.CapabilityTeamList)
                    {
                        SubcontractTeamForDDL subcontractTeamForDDL = new SubcontractTeamForDDL();
                        subcontractTeamForDDL.Subcontract_Code = Subcontract_Code;
                        subcontractTeamForDDL.Subcontract_Name = Subcontract_Name;
                        string Subcontract_Team_Id = CapabilityTeam.subcontract_Team_IdField;
                        subcontractTeamForDDL.Subcontract_Team_Id = Subcontract_Team_Id;
                        subcontractTeamForDDL.Subcontract_Team_Name = CapabilityTeam.subcontract_Team_NameField;
                        subcontractTeamForDDLList.Add(subcontractTeamForDDL);

                        foreach (var staffField in CapabilityTeam.staffField)
                        {
                            SubcontractStaffForDDL subcontractStaffForDDL = new SubcontractStaffForDDL();
                            subcontractStaffForDDL.Subcontract_Code = Subcontract_Code;
                            subcontractStaffForDDL.Subcontract_Name = Subcontract_Name;
                            subcontractStaffForDDL.Subcontract_Team_Id = Subcontract_Team_Id;
                            subcontractStaffForDDL.staff_Code = staffField.staff_CodeField;
                            subcontractStaffForDDL.staff_Name = staffField.staff_NameField;
                            subcontractStaffForDDLList.Add(subcontractStaffForDDL);
                        }
                    }
                }
            }

            Session["SubcontractTmp"] = subcontractForDDLList;
            Session["SubcontractTeamTmp"] = subcontractTeamForDDLList;
            Session["SubcontractStaffTmp"] = subcontractStaffForDDLList;
        }

        public JsonResult SubContactDDLData()
        {

            var datas = (List<SubcontractForDDL>)Session["SubcontractTmp"];

            List<LovModel> LovList = new List<LovModel>();
            foreach (var data in datas)
            {
                LovModel Lov = new LovModel();
                Lov.DISPLAY_VAL = data.Subcontract_Name;
                Lov.LOV_NAME = data.Subcontract_Code;
                //Lov.LOV_NAME = data.Subcontract_Name;
                LovList.Add(Lov);
            }

            return Json(LovList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubContactTeamDDLData(string Subcontract_Name)
        {

            var datas = (List<SubcontractTeamForDDL>)Session["SubcontractTeamTmp"];

            if (Subcontract_Name != null)
            {
                datas = datas.Where(p => p.Subcontract_Code == Subcontract_Name).ToList();
            }

            List<LovModel> LovList = new List<LovModel>();
            foreach (var data in datas)
            {
                LovModel Lov = new LovModel();
                Lov.DISPLAY_VAL = data.Subcontract_Team_Name;
                Lov.LOV_NAME = data.Subcontract_Team_Id;
                //Lov.LOV_NAME = data.Subcontract_Team_Name;
                LovList.Add(Lov);
            }

            return Json(LovList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetNewSubName(string Subcontract_Name, string Subcontract_Team)
        {
            var datas = (List<SubcontractStaffForDDL>)Session["SubcontractStaffTmp"];
            SetViewBagLov();
            if (Subcontract_Name != null && Subcontract_Team != null)
            {
                datas = datas.Where(p => p.Subcontract_Code == Subcontract_Name && p.Subcontract_Team_Id == Subcontract_Team).ToList();
            }

            List<SubNameData> subNameDataList = new List<SubNameData>();
            foreach (var item in datas)
            {
                SubNameData subNameData = new SubNameData();
                subNameData.sub_name_id = item.staff_Code;
                subNameData.sub_name = item.staff_Name;
                subNameDataList.Add(subNameData);
            }

            return PartialView("_SubNameEdit", subNameDataList);
        }

        public ActionResult AddSubContactToTmp(ConfigurationEventSubContactData Model)
        {
            /// Validate
            bool CheckInTmp = true;
            bool CheckInDB = true;
            string StaffValidateFail = "";
            if (Session["SubcontractListDataTmp"] != null)
            {
                List<ConfigurationEventSubContactData> SubcontractListData;
                List<ConfigurationEventSubContactData> SubcontractListDataForValidate = new List<ConfigurationEventSubContactData>();
                SubcontractListData = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];
                foreach (var item in SubcontractListData)
                {
                    SubcontractListDataForValidate.Add(item);
                }
                int IndexData = 0;
                if (int.TryParse(Model.indexData, out IndexData))
                {
                    SubcontractListDataForValidate.RemoveAt(IndexData);
                }
                if (SubcontractListDataForValidate.Count > 0)
                {
                    List<ConfigurationEventData> SubNameList = new List<ConfigurationEventData>();
                    var SubcontractListDataNoDeleteS = SubcontractListDataForValidate.Where(p => p.is_delete == false).ToList();
                    foreach (var SubcontractListDataNoDelete in SubcontractListDataNoDeleteS)
                    {
                        var SubNameListInSubcontract = SubcontractListDataNoDelete.SubNameList;
                        foreach (var SubNameInSubcontract in SubNameListInSubcontract)
                        {
                            ConfigurationEventData configurationEventData = new ConfigurationEventData();
                            configurationEventData.event_start_date = SubcontractListDataNoDelete.start_date_event;
                            configurationEventData.event_end_date = SubcontractListDataNoDelete.end_date_event;
                            configurationEventData.install_staff_id = SubNameInSubcontract.sub_name_id;
                            configurationEventData.install_staff_name = SubNameInSubcontract.sub_name;
                            SubNameList.Add(configurationEventData);
                        }
                    }
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
                    DateTime StartDate = DateTime.ParseExact(Model.start_date_event, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime EndDate = DateTime.ParseExact(Model.end_date_event, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    foreach (var item in Model.SubNameList)
                    {
                        if (item.sub_name_select)
                        {
                            bool CheckHaveDate = false;
                            var SubNameListSameAs = SubNameList.Where(p => p.install_staff_id == item.sub_name_id && p.install_staff_name == item.sub_name).ToList();
                            foreach (var SubNameSameAs in SubNameListSameAs)
                            {
                                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
                                DateTime StartDateSubName = DateTime.ParseExact(SubNameSameAs.event_start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                DateTime EndDateSubName = DateTime.ParseExact(SubNameSameAs.event_end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                TimeSpan timeSpanDateDiff = EndDateSubName - StartDateSubName;
                                int dateDiff = timeSpanDateDiff.Days;
                                for (var i = 0; i <= dateDiff; i++)
                                {
                                    DateTime ThisDate = StartDateSubName.AddDays(i);
                                    if (ThisDate >= StartDate && ThisDate <= EndDate)
                                    {
                                        CheckHaveDate = true;
                                        break;
                                    }
                                }
                                if (CheckHaveDate)
                                {
                                    if (StaffValidateFail == "")
                                    {
                                        StaffValidateFail = item.sub_name;
                                    }
                                    else
                                    {
                                        StaffValidateFail = StaffValidateFail + ", " + item.sub_name;
                                    }
                                    break;
                                }

                            }

                        }
                    }
                    if (StaffValidateFail != "")
                    {
                        CheckInTmp = false;
                    }
                }
            }

            if (CheckInTmp)
            {
                foreach (var item in Model.SubNameList)
                {
                    if (Model.service_option == "N")
                    {
                        if (item.sub_name_select)
                        {
                            string strCheckStaff = ValidateEngineer(item.sub_name_id, item.sub_name, Model.start_date_event, Model.end_date_event);
                            if (strCheckStaff == "false")
                            {
                                if (StaffValidateFail == "")
                                {
                                    StaffValidateFail = item.sub_name;
                                }
                                else
                                {
                                    StaffValidateFail = StaffValidateFail + "," + item.sub_name;
                                }
                                CheckInDB = CheckInDB & false;
                            }
                        }
                    }
                    else if (Model.service_option == "E")
                    {
                        if (item.sub_name_select && (!item.sub_name_select_old))
                        {
                            string strCheckStaff = ValidateEngineer(item.sub_name_id, item.sub_name, Model.start_date_event, Model.end_date_event);
                            if (strCheckStaff == "false")
                            {
                                if (StaffValidateFail == "")
                                {
                                    StaffValidateFail = item.sub_name;
                                }
                                else
                                {
                                    StaffValidateFail = StaffValidateFail + "," + item.sub_name;
                                }
                                CheckInDB = CheckInDB & false;
                            }
                        }
                    }
                }
            }

            Model.CheckValidateStaff = CheckInTmp & CheckInDB;
            Model.StaffValidateFail = StaffValidateFail;
            Session["ValidateAddSubContact"] = Model;
            /// End Validate
            if (Model.CheckValidateStaff)
            {

                List<SubcontractForDDL> subcontractForDDLList = (List<SubcontractForDDL>)Session["SubcontractTmp"];
                List<SubcontractTeamForDDL> subcontractTeamForDDLList = (List<SubcontractTeamForDDL>)Session["SubcontractTeamTmp"];
                //string subcontractName = subcontractForDDLList.Where(p => p.Subcontract_Code == Model.sub_contact_id).Select(p => p.Subcontract_Name).FirstOrDefault().ToString();
                //string subcontractTeamName = subcontractTeamForDDLList.Where(p => p.Subcontract_Team_Id == Model.sub_team_id).Select(p => p.Subcontract_Team_Name).FirstOrDefault().ToString();

                if (Model.service_option == "E")
                {
                    List<ConfigurationEventSubContactData> SubcontractListData = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];
                    ConfigurationEventSubContactData configurationEventSubContactData = SubcontractListData[int.Parse(Model.indexData)];
                    configurationEventSubContactData.service_option = Model.service_option;
                    configurationEventSubContactData.sub_contact_id = Model.sub_contact_id;
                    configurationEventSubContactData.sub_contact = Model.sub_contact;
                    configurationEventSubContactData.sub_team_id = Model.sub_team_id;
                    configurationEventSubContactData.sub_team = Model.sub_team;
                    configurationEventSubContactData.start_date_event = Model.start_date_event;
                    configurationEventSubContactData.end_date_event = Model.end_date_event;

                    var SubNameList = Model.SubNameList.Where(p => p.sub_name_select == true).ToList();
                    configurationEventSubContactData.SubNameList = new List<SubNameData>();
                    foreach (var SubName in SubNameList)
                    {
                        SubNameData subNameData = new SubNameData();
                        subNameData.sub_name_id = SubName.sub_name_id;
                        subNameData.sub_name = SubName.sub_name;
                        configurationEventSubContactData.SubNameList.Add(subNameData);
                    }
                    SubcontractListData.RemoveAt(int.Parse(Model.indexData));
                    SubcontractListData.Insert(int.Parse(Model.indexData), configurationEventSubContactData);

                    Session["SubcontractListDataTmp"] = SubcontractListData;
                }
                else
                {
                    List<ConfigurationEventSubContactData> SubcontractListData = new List<ConfigurationEventSubContactData>();
                    if (Session["SubcontractListDataTmp"] != null)
                    {
                        SubcontractListData = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];
                    }

                    ConfigurationEventSubContactData configurationEventSubContactData = new ConfigurationEventSubContactData();
                    configurationEventSubContactData.service_option = Model.service_option;
                    configurationEventSubContactData.sub_contact_id = Model.sub_contact_id;
                    configurationEventSubContactData.sub_contact = Model.sub_contact;
                    configurationEventSubContactData.sub_team_id = Model.sub_team_id;
                    configurationEventSubContactData.sub_team = Model.sub_team;
                    configurationEventSubContactData.start_date_event = Model.start_date_event;
                    configurationEventSubContactData.end_date_event = Model.end_date_event;

                    var SubNameList = Model.SubNameList.Where(p => p.sub_name_select == true).ToList();
                    configurationEventSubContactData.SubNameList = new List<SubNameData>();
                    foreach (var SubName in SubNameList)
                    {
                        SubNameData subNameData = new SubNameData();
                        subNameData.sub_name_id = SubName.sub_name_id;
                        subNameData.sub_name = SubName.sub_name;
                        configurationEventSubContactData.SubNameList.Add(subNameData);
                    }

                    SubcontractListData.Add(configurationEventSubContactData);

                    Session["SubcontractListDataTmp"] = SubcontractListData;
                }

            }
            if (Model.event_code != null && Model.event_code != "")
            {
                return RedirectToAction("ConfigurationEventsEdit", new { EventCode = Model.event_code, HasSearch = "Y" });
            }
            else
            {
                return RedirectToAction("ConfigurationEventsEdit", new { EventCode = "N", HasSearch = "Y" });
            }
        }

        public string DelSubContactToTmp(string IndexData)
        {
            List<ConfigurationEventSubContactData> SubcontractListData = (List<ConfigurationEventSubContactData>)Session["SubcontractListDataTmp"];
            ConfigurationEventSubContactData configurationEventSubContactData = SubcontractListData[int.Parse(IndexData)];
            configurationEventSubContactData.is_delete = true;
            SubcontractListData.RemoveAt(int.Parse(IndexData));
            SubcontractListData.Insert(int.Parse(IndexData), configurationEventSubContactData);
            Session["SubcontractListDataTmp"] = SubcontractListData;
            int SubContactAll = SubcontractListData.Count;
            int SubContactDelete = SubcontractListData.Where(p => p.is_delete == true).Count();
            if (SubContactAll == SubContactDelete)
            {
                return "เมื่อกดปุ่ม Save จะทำการลบ Event นี้";
            }
            else
            {
                return "";
            }
        }

        private string ValidateEngineer(string Staff_Id, string Staff_Name, string Start_Date, string End_Date)
        {
            ValidateEngineerQuery query = new ValidateEngineerQuery()
            {
                Install_Staff_Id = Staff_Id ?? "",
                Install_Staff_Name = Staff_Name ?? "",
                Event_Start_Date = Start_Date ?? "",
                Event_End_Date = End_Date ?? ""
            };
            try
            {
                var resultDB = _queryProcessor.Execute(query);
                string result = resultDB[0].o_return_code;
                if (result == "0")
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string ReserveCapability(ReserveCapabilityQuery reserveCapabilityQuery)
        {

            ReserveSubcontract reserveSubcontract = reserveCapabilityQuery.ReserveSubcontract;
            var query = new ReserveCapabilityQuery()
            {
                EventCode = reserveCapabilityQuery.EventCode,
                Service_Option = reserveCapabilityQuery.Service_Option,
                ReserveSubcontract = reserveSubcontract
            };
            return _queryProcessor.Execute(query);
        }

        private List<SubNameData> SelectStaffInDB(string EventCode, string ContractCode, string ContractName, string TeamID, string TeamName)
        {
            List<SubNameData> SubNameDataList = new List<SubNameData>();
            List<ConfigurationEventData> results = GetDataConfigurationEventByEventCode(EventCode);
            if (results != null && results.Count > 0)
            {
                List<ConfigurationEventData> resultFilter = results.Where(p => p.sub_location_id == ContractCode && p.sub_contract_name == ContractName
                    && p.sub_team_id == TeamID && p.sub_team_name == TeamName).ToList();
                foreach (var SubName in resultFilter)
                {
                    SubNameData subNameData = new SubNameData();
                    subNameData.sub_name_id = SubName.install_staff_id;
                    subNameData.sub_name = SubName.install_staff_name;
                    subNameData.sub_name_select = false;
                    subNameData.sub_name_select_old = false;
                    SubNameDataList.Add(subNameData);
                }

            }

            return SubNameDataList;
        }

    }
}
