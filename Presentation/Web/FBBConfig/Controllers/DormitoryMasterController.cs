using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace FBBConfig.Controllers
{
    [IENoCache(Order = 1)]
    public class DormitoryMasterController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveDormMasterCommand> _SaveDormMasterCommand;
        private readonly ICommandHandler<EditBuildingCommand> _SaveEditBuildingCommand;
        private readonly ICommandHandler<EditAddBuildingCommand> _SaveEditAddBuildingCommand;
        private readonly ICommandHandler<EditDormMasterCommand> _SaveEditDormCommand;



        public DormitoryMasterController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<SaveDormMasterCommand> saveDormMasterCommand
            , ICommandHandler<EditBuildingCommand> saveEditBuildingCommand, ICommandHandler<EditAddBuildingCommand> saveEditAddBuildingCommand,
            ICommandHandler<EditDormMasterCommand> saveEditDormCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _SaveDormMasterCommand = saveDormMasterCommand;
            _SaveEditBuildingCommand = saveEditBuildingCommand;
            _SaveEditAddBuildingCommand = saveEditAddBuildingCommand;
            _SaveEditDormCommand = saveEditDormCommand;
        }
        //
        // GET: /DormMaster/
        [AuthorizeUserAttribute]
        public ActionResult Index(string SaveStatus = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();
            Session["DormitoryBuildingList"] = null;
            ViewBag.SaveStatus = SaveStatus;
            return View();
        }
        public ActionResult ConfigurationDormitoryDetail(string DormitoryID = "", string SaveStatus = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();
            DormitoryMasterModel Model = new DormitoryMasterModel();
            if (DormitoryID == "N")
            {
                Model.Mode = "N";
                return View("ConfigurationDormitoryDetailNew", Model);
            }
            else
            {
                Model = GetConfigurationDormitoryByID(DormitoryID);
                Model.dormitory_id = DormitoryID;
                Model.Mode = "E";
                ViewBag.SaveStatus = SaveStatus;
                return View(Model);
            }
        }

        public ActionResult ViewToEditConfigurationDormitory(string DormitoryID = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            DormitoryMasterModel Model = GetConfigurationDormitoryByID(DormitoryID);
            Model.dormitory_id = DormitoryID;
            Model.Mode = "E";

            return View("ConfigurationDormitoryDetailEdit", Model);
        }

        public ActionResult ViewToEditConfigurationDormitoryBuilding(string DormitoryID = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            DormitoryMasterModel Model = GetConfigurationDormitoryByID(DormitoryID);
            Model.dormitory_id = DormitoryID;
            Model.Mode = "E";
            return View("ConfigurationDormitoryBuildingEdit", Model);
        }

        public ActionResult SearchDataSourceRequest([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (dataS != null && dataS != "")
            {
                var SearchPara = new JavaScriptSerializer().Deserialize<DormitorySearchPara>(dataS);
                var result = GetDataSearchModel(SearchPara);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }

        public ActionResult BuildingData([DataSourceRequest] DataSourceRequest request)
        {
            var result = GetBuildingData();

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public string AddBuildingData(string DormitoryID, string BuildingNameTH, string BuildingNameEN, string NumberOfRoom)
        {
            int CountBuilding = 0;
            List<DormitoryBuilding> DormitoryBuildingList = new List<DormitoryBuilding>();
            var query = new CheckdupBuildingQuery()
            {
                p_dormitory_id = DormitoryID,
                p_building_th = BuildingNameTH,
                p_building_en = BuildingNameEN,
            };
            var result = _queryProcessor.Execute(query);
            if (result.result == 0)
            {

                if (null != Session["DormitoryBuildingList"])
                {
                    List<DormitoryBuilding> DormitoryBuildingListTemp = (List<DormitoryBuilding>)Session["DormitoryBuildingList"];
                    CountBuilding = DormitoryBuildingListTemp.Count();
                    DormitoryBuildingList = DormitoryBuildingListTemp;
                }
                DormitoryBuilding dormitoryBuilding = new DormitoryBuilding();
                dormitoryBuilding.dormitory_no_th = BuildingNameTH;
                dormitoryBuilding.dormitory_no_th_old = BuildingNameTH;
                dormitoryBuilding.dormitory_no_en = BuildingNameEN;
                dormitoryBuilding.number_of_room = NumberOfRoom;
                dormitoryBuilding.state = "Out of Service";
                dormitoryBuilding.mode = "N";
                dormitoryBuilding.indexBuilding = (CountBuilding).ToString();
                DormitoryBuildingList.Add(dormitoryBuilding);
                Session["DormitoryBuildingList"] = DormitoryBuildingList;
            }
            else
            {
                return "ChkFail";
            }
            return "Success";
        }

        public string EditBuildingData(string DormitoryID, string BuildingNameTH, string BuildingNameOld, string BuildingNameEN, string NumberOfRoom, string Index)
        {
            List<DormitoryBuilding> DormitoryBuildingList;
            var query = new CheckdupBuildingQuery()
            {
                p_dormitory_id = DormitoryID,
                p_building_th = BuildingNameTH,
                p_building_en = BuildingNameEN,
                p_room_amount = NumberOfRoom,
            };
            var result = _queryProcessor.Execute(query);
            if (result.result == 0)
            {
                if (null != Session["DormitoryBuildingList"])
                {
                    DormitoryBuildingList = (List<DormitoryBuilding>)Session["DormitoryBuildingList"];
                    DormitoryBuilding dormitoryBuilding = DormitoryBuildingList.Where(p => p.indexBuilding == Index).FirstOrDefault();
                    dormitoryBuilding.dormitory_no_th = BuildingNameTH;
                    dormitoryBuilding.dormitory_no_th_old = BuildingNameOld;
                    dormitoryBuilding.dormitory_no_en = BuildingNameEN;
                    dormitoryBuilding.number_of_room = NumberOfRoom;
                    if (dormitoryBuilding.mode == "O")
                    {
                        dormitoryBuilding.mode = "E";
                    }
                    dormitoryBuilding.indexBuilding = Index;

                    DormitoryBuildingList.RemoveAt(int.Parse(Index));
                    DormitoryBuildingList.Insert(int.Parse(Index), dormitoryBuilding);
                    Session["DormitoryBuildingList"] = DormitoryBuildingList;
                }
            }
            else
            {
                return "ChkFail";
            }
            return "Success";
        }

        private List<ConfigurationDormitoryData> GetDataSearchModel(DormitorySearchPara SearchPara)
        {

            var query = new GetAWConfigurationDormitoryQuery()
            {
                Region = SearchPara.Region ?? "",
                DormitoryProvince = SearchPara.Province ?? "",
                DormitoryName = SearchPara.DormitoryName ?? ""
            };
            List<ConfigurationDormitoryData> result = _queryProcessor.Execute(query);
            return result;
        }

        private List<DormitoryBuilding> GetBuildingData()
        {
            List<DormitoryBuilding> DormitoryBuildingList = new List<DormitoryBuilding>();
            if (null != Session["DormitoryBuildingList"])
            {
                DormitoryBuildingList = (List<DormitoryBuilding>)Session["DormitoryBuildingList"];
            }
            return DormitoryBuildingList;
        }

        private DormitoryMasterModel GetConfigurationDormitoryByID(string DormitoryID)
        {

            var query = new GetAWConfigurationDormitoryByIDQuery()
            {
                p_dormitory_id = DormitoryID ?? ""
            };
            DormitoryMasterModel Model = new DormitoryMasterModel();
            //DateTime myDate = DateTime.Parse(dateString);
            ConfigurationDormitoryModel result = _queryProcessor.Execute(query);

            if (result != null)
            {
                Model.dormitory_name_en = result.p_dormitory_name_en;
                Model.dormitory_name_th = result.p_dormitory_name_th;
                Model.HOME_NO_EN = result.p_home_no_en;
                Model.HOME_NO_TH = result.p_home_no_th;
                Model.MOO_EN = result.p_moo_en;
                Model.MOO_TH = result.p_moo_th;
                Model.SOI_EN = result.p_soi_en;
                Model.SOI_TH = result.p_soi_th;
                Model.STREET_NAME_EN = result.p_Street_en;
                Model.STREET_NAME_TH = result.p_Street_th;
                Model.TUMBON_EN = result.p_tumbol_en;
                Model.TUMBON_TH = result.p_tumbol_th;
                Model.AMPHUR_EN = result.p_amphur_en;
                Model.AMPHUR_TH = result.p_amphur_th;
                Model.Province_EN = result.p_province_en;
                Model.Province_TH = result.p_province_th;
                Model.Postcode_EN = result.p_zipcode_en;
                Model.Postcode_TH = result.p_zipcode_th;
                Model.Contact_Name = result.p_dorm_contract_name;
                Model.contract_email = result.p_dorm_contract_email;
                Model.contract_phone = result.p_dorm_contract_phone;
                Model.target_launch_dt = DateTime.ParseExact(result.p_target_launch_dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Model.launch_dt = DateTime.ParseExact(result.p_launch_dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Model.target_volumn = result.p_target_volumn;
                Model.volumn = result.p_volumn;
                Model.target_volumn = result.p_target_volumn;

                if (result.ConfigurationDormitoryBuildingDataList != null && result.ConfigurationDormitoryBuildingDataList.Count > 0)
                {
                    int CountBuilding = 0;
                    List<DormitoryBuilding> DataList = new List<DormitoryBuilding>();
                    foreach (var item in result.ConfigurationDormitoryBuildingDataList)
                    {
                        DormitoryBuilding dormitoryBuilding = new DormitoryBuilding();
                        dormitoryBuilding.dormitory_id = DormitoryID;
                        dormitoryBuilding.dormitory_no_en = item.dormitory_no_en;
                        dormitoryBuilding.dormitory_no_th_old = item.dormitory_no_th;
                        dormitoryBuilding.dormitory_no_th = item.dormitory_no_th;
                        dormitoryBuilding.number_of_room = item.number_of_room;
                        dormitoryBuilding.state = item.state;
                        dormitoryBuilding.mode = "O";
                        dormitoryBuilding.indexBuilding = CountBuilding.ToString();
                        DataList.Add(dormitoryBuilding);
                        CountBuilding++;
                    }
                    Session["DormitoryBuildingList"] = DataList;
                }
            }

            return Model;
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
        public JsonResult SelectProvinceForSearch(string regionCode = "", string langFlag = "N")
        {
            var query = new SelectProvinceDormQuery
            {
                Lang_Flag = "N",
                REGION_CODE = regionCode
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "เลือกทั้งหมด", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectRegion(string langFlag = "N")
        {
            var query = new SelectRegionQuery
            {
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "เลือกทั้งหมด", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectProvince(string langFlag = "N")
        {
            var query = new SelectProvinceDormQuery
            {
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectProvinceConfigEvent(string langFlag = "N")
        {
            var query = new SelectProvinceConfigEventQuery
            {
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectProvinceEN(string langFlag = "Y")
        {
            var query = new SelectProvinceDormENQuery
            {
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectAmphur(string regionCode = "", string province = "", string langFlag = "N")
        {
            var query = new SelectAmphurDormQuery
            {
                PROVINCE = province,
                Lang_Flag = langFlag

            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectAmphurEN(string province = "", string langFlag = "Y")
        {
            var query = new SelectAmphurDormENQuery
            {
                PROVINCE = province,
                Lang_Flag = langFlag

            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectTumbon(string province = "", string aumphur = "", string langFlag = "N")
        {
            var query = new SelectTumbonDormQuery
            {
                PROVINCE = province,
                AUMPHUR = aumphur,
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectTumbonEN(string province = "", string aumphur = "", string langFlag = "Y")
        {
            var query = new SelectTumbonDormENQuery
            {
                PROVINCE = province,
                AUMPHUR = aumphur,
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectPostcode(string province = "", string aumphur = "", string tumbon = "", string langFlag = "N")
        {
            var query = new SelectPostalCodeDormQuery
            {
                PROVINCE = province,
                AUMPHUR = aumphur,
                TUMBON = tumbon,
                Lang_Flag = langFlag
            };

            var data = _queryProcessor.Execute(query);
            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectPostcodeEN(string province = "", string aumphur = "", string tumbon = "", string langFlag = "Y")
        {
            var query = new SelectPostalCodeENDormQuery
            {
                PROVINCE = province,
                AUMPHUR = aumphur,
                TUMBON = tumbon,
                Lang_Flag = langFlag
            };

            var data = _queryProcessor.Execute(query);
            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectAllDormitory(string regionCode = "", string province = "")
        {
            var query = new SelectAllDormitoryNameQuery
            {
                Region = regionCode,
                Province = province
            };

            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "เลือกทั้งหมด", LOV_NAME = "" });


            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectAllBuilding(string province = "", string Dormitory_Name = "")
        {
            var query = new SelectAllDormitoryBuildingQuery
            {
                Province = province,
                DormitoryName = Dormitory_Name,


            };

            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "เลือกทั้งหมด", LOV_NAME = "" });


            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDormMaster(string dormitory_name_th, string HOME_NO_TH, string MOO_TH, string SOI_TH, string STREET_NAME_TH, string TUMBON_TH,
        string AMPHUR_TH, string Province_TH, string Postcode_TH, string dormitory_name_en, string HOME_NO_EN, string MOO_EN, string SOI_EN, string STREET_NAME_EN,
        string TUMBON_EN, string AMPHUR_EN, string Province_EN, string Postcode_EN, string Contact_Name, string Email, string Phone, string Target_launch_dt, string Launch_dt,
        string Target_volumn, string Volumn)
        {
            if (null == base.CurrentUser)
                return Json("Sesson Time Out", JsonRequestBehavior.AllowGet);
            var LoginUser = base.CurrentUser;
            string MsgSaveComplete = "";
            string MsgSaveFail = "";
            try
            {
                List<DormMasterData> DormMasterDataList = new List<DormMasterData>();
                List<DormitoryBuilding> DormitoryBuildingList = new List<DormitoryBuilding>();
                if (null != Session["DormitoryBuildingList"])
                {
                    DormitoryBuildingList = (List<DormitoryBuilding>)Session["DormitoryBuildingList"];
                }
                if (DormitoryBuildingList != null && DormitoryBuildingList.Count > 0)
                {
                    foreach (var item in DormitoryBuildingList)
                    {
                        DormMasterData dormMasterData = new DormMasterData();
                        dormMasterData.Save_dormitory_name_th = dormitory_name_th;
                        dormMasterData.Save_HOME_NO_TH = HOME_NO_TH;
                        dormMasterData.Save_MOO_TH = MOO_TH;
                        dormMasterData.Save_SOI_TH = SOI_TH;
                        dormMasterData.Save_STREET_NAME_TH = STREET_NAME_TH;
                        dormMasterData.Save_TUMBON_TH = TUMBON_TH;
                        dormMasterData.Save_AMPHUR_TH = AMPHUR_TH;
                        dormMasterData.Save_Province_TH = Province_TH;
                        dormMasterData.Save_Postcode_TH = Postcode_TH;
                        dormMasterData.Save_dormitory_name_en = dormitory_name_en;
                        dormMasterData.Save_HOME_NO_EN = HOME_NO_EN;
                        dormMasterData.Save_MOO_EN = MOO_EN;
                        dormMasterData.Save_SOI_EN = SOI_EN;
                        dormMasterData.Save_STREET_NAME_EN = STREET_NAME_EN;
                        dormMasterData.Save_TUMBON_EN = TUMBON_EN;
                        dormMasterData.Save_AMPHUR_EN = AMPHUR_EN;
                        dormMasterData.Save_Province_EN = Province_EN;
                        dormMasterData.Save_Postcode_EN = Postcode_EN;
                        dormMasterData.User = LoginUser.UserName;
                        dormMasterData.Save_building_th = item.dormitory_no_th;
                        dormMasterData.Save_building_en = item.dormitory_no_en;
                        dormMasterData.Save_room_amount = item.number_of_room;
                        dormMasterData.Save_target_launch_dt = Target_launch_dt;
                        dormMasterData.Save_launch_dt = Launch_dt;
                        dormMasterData.Save_target_volumn = Target_volumn;
                        dormMasterData.Save_volumn = Volumn;
                        dormMasterData.Save_dorm_contract_name = Contact_Name;
                        dormMasterData.Save_dorm_contract_email = Email;
                        dormMasterData.Save_dorm_contract_phone = Phone;

                        DormMasterDataList.Add(dormMasterData);
                    }

                    var command = new SaveDormMasterCommand()
                    {
                        DormMasterDataList = DormMasterDataList
                    };

                    _SaveDormMasterCommand.Handle(command);

                    if (command.Save_complete != "")
                    {
                        MsgSaveComplete = "Save complete (" + command.Save_complete + " )";
                    }
                    if (command.Save_fail != "")
                    {
                        MsgSaveFail = "Save fail (" + command.Save_fail + " )";
                    }
                    return Json(MsgSaveComplete + " : " + MsgSaveFail, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json("No Data If Sesson Time Out", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(MsgSaveComplete + " : " + MsgSaveFail, JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult SaveEditDormMaster(string DormID, string dormitory_name_th, string HOME_NO_TH, string MOO_TH, string SOI_TH, string STREET_NAME_TH, string TUMBON_TH,
       string AMPHUR_TH, string Province_TH, string Postcode_TH, string dormitory_name_en, string HOME_NO_EN, string MOO_EN, string SOI_EN, string STREET_NAME_EN,
       string TUMBON_EN, string AMPHUR_EN, string Province_EN, string Postcode_EN, string Contact_Name, string Email, string Phone, string Target_launch_dt, string Launch_dt,
        string Target_volumn, string Volumn)
        {
            if (null == base.CurrentUser)
                return Json("Sesson Time Out", JsonRequestBehavior.AllowGet);
            var LoginUser = base.CurrentUser;
            string MsgSaveComplete = "";
            string MsgSaveFail = "";
            try
            {
                List<DormMasterData> DormMasterDataList = new List<DormMasterData>();
                List<DormitoryBuilding> DormitoryBuildingList = new List<DormitoryBuilding>();
                if (null != Session["DormitoryBuildingList"])
                {
                    DormitoryBuildingList = (List<DormitoryBuilding>)Session["DormitoryBuildingList"];
                }
                if (DormitoryBuildingList != null && DormitoryBuildingList.Count > 0)
                {
                    foreach (var item in DormitoryBuildingList)
                    {
                        var command3 = new EditDormMasterCommand()
                        {
                            Edit_dormitory_ID = DormID,
                            Edit_dormitory_name_th = dormitory_name_th,
                            Edit_HOME_NO_TH = HOME_NO_TH,
                            Edit_MOO_TH = MOO_TH,
                            Edit_SOI_TH = SOI_TH,
                            Edit_STREET_NAME_TH = STREET_NAME_TH,
                            Edit_TUMBON_TH = TUMBON_TH,
                            Edit_AMPHUR_TH = AMPHUR_TH,
                            Edit_Province_TH = Province_TH,
                            Edit_Postcode_TH = Postcode_TH,
                            Edit_dormitory_name_en = dormitory_name_en,
                            Edit_HOME_NO_EN = HOME_NO_EN,
                            Edit_MOO_EN = MOO_EN,
                            Edit_SOI_EN = SOI_EN,
                            Edit_STREET_NAME_EN = STREET_NAME_EN,
                            Edit_TUMBON_EN = TUMBON_EN,
                            Edit_AMPHUR_EN = AMPHUR_EN,
                            Edit_Province_EN = Province_EN,
                            Edit_Postcode_EN = Postcode_EN,
                            Edit_dorm_contract_name = Contact_Name,
                            Edit_dorm_contract_email = Email,
                            Edit_dorm_contract_phone = Phone,
                            Edit_target_launch_dt = Target_launch_dt,
                            Edit_launch_dt = Launch_dt,
                            Edit_target_volumn = Target_volumn,
                            Edit_volumn = Volumn,
                            User = LoginUser.UserName,
                            Save_building_th = item.dormitory_no_th
                        };
                        _SaveEditDormCommand.Handle(command3);
                        if (command3.Save_complete != "")
                        {
                            MsgSaveComplete = "Save complete (" + command3.Save_complete + " )";
                        }
                        if (command3.Save_fail != "")
                        {
                            MsgSaveFail = "Save fail (" + command3.Save_fail + " )";
                        }

                    }
                    return Json(DormID, JsonRequestBehavior.AllowGet);
                }

                else
                {

                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult SaveDormBuildingMaster(string DormitoryID)
        {
            var LoginUser = base.CurrentUser;
            string MsgSaveComplete = "";
            string MsgSaveFail = "";
            List<DormitoryBuilding> DormitoryBuildingList = new List<DormitoryBuilding>();
            if (null != Session["DormitoryBuildingList"])
            {
                DormitoryBuildingList = (List<DormitoryBuilding>)Session["DormitoryBuildingList"];
            }
            if (DormitoryBuildingList != null && DormitoryBuildingList.Count > 0)
            {
                foreach (var item in DormitoryBuildingList)
                {


                    if (item.mode == "E")
                    {
                        try
                        {

                            var command = new EditBuildingCommand()
                            {
                                Edit_dormitory_ID = DormitoryID,
                                Edit_dormitory_no_th = item.dormitory_no_th_old,
                                Edit_building_th = item.dormitory_no_th,
                                Edit_building_en = item.dormitory_no_en,
                                Edit_room_amount = item.number_of_room,
                                User = LoginUser.UserName

                            };

                            _SaveEditBuildingCommand.Handle(command);

                            if (command.Save_complete != "")
                            {
                                MsgSaveComplete = "Save complete (" + command.Save_complete + " )";
                            }
                            if (command.Save_fail != "")
                            {
                                MsgSaveFail = "Save fail (" + command.Save_fail + " )";
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(MsgSaveComplete + " : " + MsgSaveFail, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else if (item.mode == "N")
                    {
                        try
                        {
                            var command2 = new EditAddBuildingCommand()
                            {
                                EditAdd_dormitory_ID = DormitoryID,
                                EditAdd_building_th = item.dormitory_no_th,
                                EditAdd_building_en = item.dormitory_no_en,
                                EditAdd_room_amount = item.number_of_room,
                                User = LoginUser.UserName
                            };
                            _SaveEditAddBuildingCommand.Handle(command2);

                            if (command2.Save_complete != "")
                            {
                                MsgSaveComplete = "Save complete (" + command2.Save_complete + " )";
                            }
                            if (command2.Save_fail != "")
                            {
                                MsgSaveFail = "Save fail (" + command2.Save_fail + " )";
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(MsgSaveComplete + " : " + MsgSaveFail, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

            }
            return Json(MsgSaveComplete + " : " + MsgSaveFail, JsonRequestBehavior.AllowGet);
        }

    }
}




