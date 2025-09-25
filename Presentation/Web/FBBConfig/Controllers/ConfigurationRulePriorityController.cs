using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.Extensions;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBEntity.PanelModels;
using WBBBusinessLayer;
using WBBContract.Commands.FBBWebConfigCommands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace FBBConfig.Controllers
{
    public class ConfigurationRulePriorityController : FBBConfigController
    {

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveConfigRulePriorityCommand> _SaveConfigRulePriorityCommand;
        private readonly ICommandHandler<SaveEditConfigRulePriorityCommand> _SaveEditConfigRulePriorityCommand;
        private readonly ICommandHandler<DeleteConfigurationRulePriorityCommand> _DeleteConfigurationRulePriorityCommand;
        static List<LovModel> ListFbbCfgLovSearchRulePriority = null;
        static List<LovModel> ListFbbCfgLovUpsertRulePriority = null;
        static List<LovValueModel> ListFbbLovDataScreen = null;
        static List<LovValueModel> ListFbbPriorityLov = null;
        static List<LovValueModel> ListFbbConditionParameterLovv = null;
        static List<LovValueModel> FULLListFbbConditionParameterLovv = null;
        static List<LovValueModel> ListFbbValueLovv = null;
        static List<GetListPLookupNameModel> listGetListRulePriorityNameModel = null;
        static List<GetListLookupParamModel> listGetListLookupParamModel = null;
        static List<OperatorList> ListCurrentOperator = null;
        static List<DataConfigEDITRulePriorityTable> ListCurrentConditionEditDataTable = null;
        static List<Lookup_param_list> ListCurrentLookup_param_list = null;
        static SaveConfigRulePriorityCommand TempEditRulePriority = null;
        static List<string> ListFieldnameDupState = null;
        static List<string> TempListFieldnameDupState = null;
        static List<string> currentProvince = null;
        static List<string> currentDistrict = null;
        static List<string> currentSubDistrict = null;
        static string tempProvince = "";
        static string tempDistrict = "";
        static string tempSymptomGroup = "";
        static string sessionUsername = "";

        public ConfigurationRulePriorityController(IQueryProcessor queryProcessor, ILogger logger,
             ICommandHandler<SaveConfigRulePriorityCommand> SaveConfigRulePriorityCommand,
             ICommandHandler<SaveEditConfigRulePriorityCommand> SaveEditConfigRulePriorityCommand,
             ICommandHandler<DeleteConfigurationRulePriorityCommand> DeleteConfigurationRulePriorityCommand)
        {

            _queryProcessor = queryProcessor;
            _SaveConfigRulePriorityCommand = SaveConfigRulePriorityCommand;
            _SaveEditConfigRulePriorityCommand = SaveEditConfigRulePriorityCommand;
            _DeleteConfigurationRulePriorityCommand = DeleteConfigurationRulePriorityCommand;
            _Logger = logger;
        }

        private void SetViewBagLov()
        {
            //var LovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == "FBBOR013").ToList();
            //ViewBag.configscreen = LovDataScreen;
        }

        public ActionResult Index()
        {
            //Account-User
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            ViewBag.User = CurrentUser;
            ViewBag.UserGroup = GetUserGroup();
            var User = CurrentUser;
            ViewBag.username = User.UserName;
            sessionUsername = User.UserName;

            ListFbbCfgLovSearchRulePriority = new List<LovModel>();
            ListFbbCfgLovUpsertRulePriority = new List<LovModel>();

            ListFbbLovDataScreen = new List<LovValueModel>();
            ListFbbPriorityLov = new List<LovValueModel>();
            List<LovValueModel> ListFbbConditionParameterLovv = new List<LovValueModel>();
            List<LovValueModel> FULLListFbbConditionParameterLovv = new List<LovValueModel>();

            listGetListRulePriorityNameModel = new List<GetListPLookupNameModel>();
            listGetListLookupParamModel = new List<GetListLookupParamModel>();

            ListFbbValueLovv = new List<LovValueModel>();
            ListCurrentConditionEditDataTable = new List<DataConfigEDITRulePriorityTable>();
            ListCurrentOperator = new List<OperatorList>();

            ListFieldnameDupState = new List<string>();
            TempListFieldnameDupState = new List<string>();

            currentProvince = new List<string>();
            currentDistrict = new List<string>();
            currentSubDistrict = new List<string>();

            TempEditRulePriority = new SaveConfigRulePriorityCommand();
            ListCurrentLookup_param_list = new List<Lookup_param_list>();

            getListData();
            return View();
        }

        private string GetUserGroup()
        {
            string ReSult = "";
            var query = new GetUserGroupQuery()
            {
                p_USER_NAME = CurrentUser.UserName
            };

            var result = _queryProcessor.Execute(query);

            if (result != null)
            {
                ReSult = result.GROUP_NAME.ToSafeString();
            }

            return ReSult;
        }

        private void getListData()
        {
            //LOV_VAL5 = SEARCH_RULE_PRIORITY
            if (ListFbbCfgLovSearchRulePriority == null || ListFbbCfgLovSearchRulePriority.Count == 0)
                ListFbbCfgLovSearchRulePriority = SelectFbbCfgLov_searchLookup("SEARCH_RULE_PRIORITY");
            ViewBag.configscreenSearchPriority = ListFbbCfgLovSearchRulePriority;

            //LOV_VAL5 = RULE_PRIORITY_UPSERT
            if (ListFbbCfgLovUpsertRulePriority == null || ListFbbCfgLovUpsertRulePriority.Count == 0)
                ListFbbCfgLovUpsertRulePriority = SelectFbbCfgLov_searchLookup("RULE_PRIORITY_UPSERT");
            ViewBag.configscreenUpsertRulePriority = ListFbbCfgLovUpsertRulePriority;

            //lovname=payg_condition_parameter
            var ListConditionParameterLov = GetLovListByLovName("SCREEN", "payg_condition_parameter");
            var ListConditionParameterLovFA1 = ListConditionParameterLov.OrderBy(t => t.OrderBy).ToList();
            var ListConditionParameterLovFA2 = ListConditionParameterLov.OrderBy(t => t.LovValue2).ToList();
            ListFbbConditionParameterLovv = ListConditionParameterLovFA2;
            FULLListFbbConditionParameterLovv = ListConditionParameterLovFA2;

            if (listGetListRulePriorityNameModel == null || listGetListRulePriorityNameModel.Count == 0)
                listGetListRulePriorityNameModel = _queryProcessor.Execute(new GetListPLookupNameQuery());

            //if (listGetListLookupParamModel == null || listGetListLookupParamModel.Count == 0)
            //    listGetListLookupParamModel = _queryProcessor.Execute(new GetListLookupParameterQuery());

            var ListPriorityLov = GetLovListByLovName("FBB_CONSTANT", "RULE_PRIORITY");
            ListFbbPriorityLov = ListPriorityLov;

            var LovDataScreen = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
            ListFbbLovDataScreen = LovDataScreen;

        }

        private List<LovModel> SelectFbbCfgLov_searchLookup(string lov_val5)
        {
            try
            {
                var query = new SelectLovVal5Query
                {
                    LOV_VAL5 = lov_val5
                };
                return _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LovModel>();
            }

        }

        private List<LovModel> SelectFbbCfgLovCost(string lov_val5)
        {
            var query = new SelectLovVal5Query
            {
                LOV_VAL5 = lov_val5
            };
            return _queryProcessor.Execute(query);
        }

        private List<LovValueModel> GetLovListByLovName(string type, string name)
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        //lovname=payg_condition_parameter
        public JsonResult SelectListPaygConditionParameter()
        {
            try
            {
                var data = ListFbbConditionParameterLovv.ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        //lovname=payg_condition_parameter=Operator
        public JsonResult SelectListOperatorParameter()
        {
            try
            {
                List<OperatorList> Operatorlist = new List<OperatorList>();

                List<string> stringList = ListFbbConditionParameterLovv[0].LovValue4.Split(',').ToList();

                foreach (string str in stringList)
                {
                    Operatorlist.Add(new OperatorList { Value1 = "" + str + "", Value2 = "" + str + "" });
                }

                var data = Operatorlist.ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectListLookupName()
        {
            try
            {
                var data = listGetListRulePriorityNameModel.ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult StoringValueLookupname([DataSourceRequest] DataSourceRequest request, string lookupData = "")
        {
            try
            {
                var SortData = new ConfigurationLookupParamView
                {
                    dataConfigLookupParam = null
                };

                var result = SelectFbbCfgLov_searchLookupParam(lookupData);
                if (result != null)
                {

                    ////lovname=payg_condition_parameter
                    //var ListValueLov = GetLovListByLovName("DROPDOWNLIST", ""+ lookupData + "");
                    //if(ListValueLov != null)
                    //{
                    //    ListFbbValueLovv = ListValueLov;
                    //}

                    SortData.dataConfigLookupParam = result.dataConfigLookupParam;

                    return Json(new
                    {
                        Data = SortData.dataConfigLookupParam,
                        Total = result.dataConfigLookupParam.Count
                    });
                }
                else
                {
                    return Json(new
                    {
                        code = "-1",
                        msg = "filter failed no data"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StoringValueDuplicateFieldname([DataSourceRequest] DataSourceRequest request, string FieldnameData = "", string OldFieldnameDate = "")
        {
            try
            {
                string CurrentCode = "-1";
                string currentProvinceFilterState = "-1";
                string currentDistrictFilterState = "-1";

                //DuplicateCheckandStroingLogic
                //if (ListFieldnameDupState.Count() > 0)
                //{
                //    for (int iDupTemp = 0; iDupTemp < ListFieldnameDupState.Count(); iDupTemp++)
                //    {
                //        if (ListFieldnameDupState[iDupTemp] == OldFieldnameDate)
                //        {
                //            ListFieldnameDupState.RemoveAt(iDupTemp);
                //        }
                //    }

                //    ListFieldnameDupState.Add(FieldnameData);

                //}
                //else if (ListFieldnameDupState.Count() == 0)
                //{
                //    ListFieldnameDupState.Add(FieldnameData);
                //}

                if (FieldnameData != "")
                {
                    //CurrentCode = "1";

                    if (FieldnameData == "District" || FieldnameData == "Sub District")
                    {
                        if (FieldnameData == "District")
                        {
                            if (ListFieldnameDupState.Count() > 0)
                            {
                                for (int i2 = 0; i2 < ListFieldnameDupState.Count(); i2++)
                                {
                                    if (ListFieldnameDupState[i2] == "Province")
                                    {
                                        currentProvinceFilterState = "1";
                                        currentDistrictFilterState = "1";
                                    }
                                }
                            }
                        }
                        else if (FieldnameData == "Sub District")
                        {
                            if (ListFieldnameDupState.Count() > 0)
                            {
                                for (int i2 = 0; i2 < ListFieldnameDupState.Count(); i2++)
                                {
                                    if (ListFieldnameDupState[i2] == "Province")
                                    {
                                        currentProvinceFilterState = "1";
                                    }

                                    if (ListFieldnameDupState[i2] == "District")
                                    {
                                        currentDistrictFilterState = "1";
                                    }
                                }
                            }
                        }

                        //last_check_up
                        if (currentProvinceFilterState != "1" || currentDistrictFilterState != "1")
                        {
                            CurrentCode = "99";
                            return Json(new
                            {
                                code = CurrentCode,
                                msg = "False step for filter"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (FieldnameData == "Symptom Name")
                    {
                        if (ListFieldnameDupState.Count() > 0)
                        {
                            for (int i2 = 0; i2 < ListFieldnameDupState.Count(); i2++)
                            {
                                if (ListFieldnameDupState[i2] == "Symptom Group")
                                {
                                    currentProvinceFilterState = "1";
                                    currentDistrictFilterState = "1";
                                }
                            }
                        }

                        //last_check_up
                        if (currentProvinceFilterState != "1" || currentDistrictFilterState != "1")
                        {
                            CurrentCode = "98";
                            return Json(new
                            {
                                code = CurrentCode,
                                msg = "False step for filter"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }



                    //List<string> initialList = new List<string>();
                    //// Put whatever you want in the initial list

                    //List<string> listToAdd = new List<string>();
                    //// Put whatever you want in the second list

                    //ListFieldnameDupState.Add(FieldnameData);

                    return Json(new
                    {
                        code = CurrentCode,
                        msg = "done stroe data"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        code = "-1",
                        msg = "filter failed no data"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReturnMistepcaseDuplicateFieldname([DataSourceRequest] DataSourceRequest request, string FieldnameData = "")
        {
            try
            {
                for (int i2 = 0; i2 < ListFieldnameDupState.Count(); i2++)
                {
                    if (ListFieldnameDupState[i2] == FieldnameData)
                    {
                        ListFieldnameDupState.RemoveAt(i2);
                    }
                }

                return Json(new
                {
                    code = "-1",
                    msg = "filter failed no data"
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StoringProvinceandDistrict([DataSourceRequest] DataSourceRequest request, string flagProvince = "", string stringProvinceAndDistrictData = "")
        {
            try
            {
                if (flagProvince == "Province")
                {
                    tempProvince = stringProvinceAndDistrictData;

                    return Json(new
                    {
                        code = "1",
                        msg = "Province filter storing"
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (flagProvince == "District")
                {
                    tempDistrict = stringProvinceAndDistrictData;

                    return Json(new
                    {
                        code = "1",
                        msg = "District filter storing"
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (flagProvince == "Symptom Group")
                {
                    tempSymptomGroup = stringProvinceAndDistrictData;

                    return Json(new
                    {
                        code = "1",
                        msg = "District filter storing"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        code = "1",
                        msg = "No filter storing"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StoringValueLookupOperator([DataSourceRequest] DataSourceRequest request, string lookupData = "")
        {
            try
            {
                string CurrentCode = "-1";
             

                if (lookupData != "")
                {
                    List<OperatorList> Operatorlist = new List<OperatorList>();

      
                        for (int i = 0; i < FULLListFbbConditionParameterLovv.Count(); i++)
                        {
                            if (FULLListFbbConditionParameterLovv[i].Text == lookupData)
                            {
                                List<string> stringList = FULLListFbbConditionParameterLovv[i].LovValue4.Split(',').ToList();
                                foreach (string str in stringList)
                                {
                                    Operatorlist.Add(new OperatorList { Value1 = "" + str + "", Value2 = "" + str + "" });
                                }
                            }
                        }

                    
         

                    ListCurrentOperator = Operatorlist;

                    //if(lookupData == "")
                    //string param = dataS.Replace("\"", "");
                    //if(lookupData == "")
                    string param = Storevaluemakeup(lookupData);
                    //string param = "v_province";

                    //if (param == "v_province" || param == "v_district" || param == "v_subdistrict" || param == "p_Reject_reason")
                    if (param != "nodatamatch" && param != "" && param != null)
                    {
                        //List<DataDropDownLookup> dataDropDownLookupList = new List<DataDropDownLookup>();
                        List<LovValueModel> dataDropDownLookupList = new List<LovValueModel>();
                        if (param == "v_province")
                        {
                            var config_name = param.ToSafeString() == string.Empty || param.ToSafeString() == null ? string.Empty : "PROVINCE";
                            var queryProvince = new GetConfigDropDownQuery
                            {
                                config_name = config_name,
                                symptom_group = string.Empty,
                                province_th = string.Empty,
                                district_th = string.Empty
                            };
                            var resultProvince = _queryProcessor.Execute(queryProvince);
                            var ProvinceList = resultProvince.Select(p => p.province_th).ToList();
                            foreach (var value in ProvinceList)
                            {
                                dataDropDownLookupList.Add(new LovValueModel { LovValue1 = value, OrderBy = 0, Name = "Province" });
                            }

                            dataDropDownLookupList.Add(new LovValueModel { LovValue1 = "ALL", OrderBy = 0, Name = "Province" });

                            ListFbbValueLovv = dataDropDownLookupList;
                            CurrentCode = "2";
                        }
                        else if (param == "v_district")
                        {

                            var config_name = param.ToSafeString() == string.Empty || param.ToSafeString() == null ? string.Empty : "DISTRICT";
                            var queryProvince = new GetConfigDropDownQuery
                            {
                                config_name = config_name,
                                symptom_group = string.Empty,
                                province_th = tempProvince,
                                district_th = string.Empty
                            };
                            var resultProvince = _queryProcessor.Execute(queryProvince);
                            var ProvinceList = resultProvince.Select(p => p.district_th).ToList();
                            foreach (var value in ProvinceList)
                            {
                                dataDropDownLookupList.Add(new LovValueModel { LovValue1 = value, OrderBy = 0, Name = "District" });
                            }

                            dataDropDownLookupList.Add(new LovValueModel { LovValue1 = "ALL", OrderBy = 0, Name = "District" });

                            ListFbbValueLovv = dataDropDownLookupList;
                            CurrentCode = "2";
                        }
                        else if (param == "v_subdistrict")
                        {
                            var config_name = param.ToSafeString() == string.Empty || param.ToSafeString() == null ? string.Empty : "SUBDISTRICT";
                            var queryProvince = new GetConfigDropDownQuery
                            {
                                config_name = config_name,
                                symptom_group = string.Empty,
                                province_th = tempProvince,
                                district_th = tempDistrict
                            };
                            var resultProvince = _queryProcessor.Execute(queryProvince);
                            var ProvinceList = resultProvince.Select(p => p.sub_district_th).ToList();
                            foreach (var value in ProvinceList)
                            {
                                dataDropDownLookupList.Add(new LovValueModel { LovValue1 = value, OrderBy = 0, Name = "Sub District" });
                            }

                            dataDropDownLookupList.Add(new LovValueModel { LovValue1 = "ALL", OrderBy = 0, Name = "Sub District" });

                            ListFbbValueLovv = dataDropDownLookupList;
                            CurrentCode = "2";
                        }
                        else if (param == "v_symptom_group")
                        {

                            var config_name = param.ToSafeString() == string.Empty || param.ToSafeString() == null ? string.Empty : "SYMPTOM_GROUP";
                            var queryProvince = new GetConfigDropDownQuery
                            {
                                config_name = config_name,
                                symptom_group = string.Empty,
                                province_th = string.Empty,
                                district_th = string.Empty
                            };
                            var resultProvince = _queryProcessor.Execute(queryProvince);
                            var ProvinceList = resultProvince.Select(p => p.symptom_group).ToList();
                            foreach (var value in ProvinceList)
                            {
                                dataDropDownLookupList.Add(new LovValueModel { LovValue1 = value });
                            }
                            ListFbbValueLovv = dataDropDownLookupList;
                            CurrentCode = "2";
                        }
                        else if (param == "p_Reject_reason")
                        {

                            var config_name = param.ToSafeString() == string.Empty || param.ToSafeString() == null ? string.Empty : "SYMPTOM_NAME";
                            var queryProvince = new GetConfigDropDownQuery
                            {
                                config_name = config_name,
                                symptom_group = tempSymptomGroup,
                                province_th = string.Empty,
                                district_th = string.Empty
                            };
                            var resultProvince = _queryProcessor.Execute(queryProvince);
                            var ProvinceList = resultProvince.Select(p => p.symptom_name).ToList();
                            foreach (var value in ProvinceList)
                            {
                                dataDropDownLookupList.Add(new LovValueModel { LovValue1 = value });
                            }
                            ListFbbValueLovv = dataDropDownLookupList;
                            CurrentCode = "2";
                        }
                        else if (param == "v_fttr_flag")
                        {
                            var ListValueLov = GetLovListByLovName("DROPDOWNLIST", "FTTR Flag");
                            if (ListValueLov != null && ListValueLov.Count() > 0)
                            {
                                ListFbbValueLovv = ListValueLov;
                                CurrentCode = "2";
                            }
                            if (ListValueLov.Count() == 0)
                            {
                                CurrentCode = "1";
                            }
                        }
                        //return Json(dataDropDownLookupList, JsonRequestBehavior.AllowGet);
                    }
                    else if (param == "nodatamatch" && param != "" && param != null)
                    {
                
                      
                            //lovname=payg_condition_parameter
                            var ListValueLov = GetLovListByLovName("DROPDOWNLIST", "" + lookupData + "");
                            if (ListValueLov != null && ListValueLov.Count() > 0)
                            {
                                ListFbbValueLovv = ListValueLov;
                                CurrentCode = "2";
                            }
                            if (ListValueLov.Count() == 0)
                            {

                                CurrentCode = "1";
                            }

                        }
                    //var data = Operatorlist.ToList();
                    //return Json(data, JsonRequestBehavior.AllowGet);
                    return Json(new
                    {
                        code = CurrentCode,
                        msg = "filter data done"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        code = "-1",
                        msg = "filter failed no data"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public class UsingLovValueModelforTemp
        {
            public string Text { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
            public decimal Id { get; set; }
            public decimal ParId { get; set; }
            public string LovValue1 { get; set; }
            public string LovValue2 { get; set; }
            public string LovValue3 { get; set; }
            public string LovValue4 { get; set; }
            public string LovValue5 { get; set; }
            public byte[] Image_blob { get; set; }

            public decimal? OrderBy { get; set; }
            public string DefaultValue { get; set; }
            public string ActiveFlag { get; set; }
        }

        public JsonResult GetvalueDropDown(string dataS = "" ,string option = "")
        {
            try
            {

                if (dataS == "Field_Name")
                {
                    List<UsingLovValueModelforTemp> TempLovFbbCondition = new List<UsingLovValueModelforTemp>();
                    //TempLovFbbCondition = ListFbbConditionParameterLovv;

                    for (int Tempi = 0; Tempi < ListFbbConditionParameterLovv.Count(); Tempi++)
                    {
                        TempLovFbbCondition.Add(new UsingLovValueModelforTemp { Text = ListFbbConditionParameterLovv[Tempi].Text, Type = ListFbbConditionParameterLovv[Tempi].Type, Name = ListFbbConditionParameterLovv[Tempi].Name, Id = ListFbbConditionParameterLovv[Tempi].Id, ParId = ListFbbConditionParameterLovv[Tempi].ParId, LovValue1 = ListFbbConditionParameterLovv[Tempi].LovValue1, LovValue2 = ListFbbConditionParameterLovv[Tempi].LovValue2, LovValue3 = ListFbbConditionParameterLovv[Tempi].LovValue3, LovValue4 = ListFbbConditionParameterLovv[Tempi].LovValue4, LovValue5 = ListFbbConditionParameterLovv[Tempi].LovValue5, OrderBy = ListFbbConditionParameterLovv[Tempi].OrderBy, DefaultValue = ListFbbConditionParameterLovv[Tempi].DefaultValue, ActiveFlag = ListFbbConditionParameterLovv[Tempi].ActiveFlag });
                    }

                    //ListFieldnameDupState

                    for (int i2 = 0; i2 < ListFieldnameDupState.Count(); i2++)
                    {

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == ListFieldnameDupState[i2])
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }
                            
                        }

                    }

                    //delete-same-row-data-dup
                    if (option == "Province")
                    {
                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Sub District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Symptom Name")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                    }
                    else if (option == "District")
                    {
                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Sub District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Province")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }
                    }
                    else if (option == "Sub District")
                    {
                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Province")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }
                    }
                    else if (option == "Symptom Name")
                    {
                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Symptom Group")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Sub District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }
                    }
                    else if (option == "Symptom Group")
                    {
                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Symptom Name")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "Sub District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }

                        for (int i1 = 0; i1 < TempLovFbbCondition.Count(); i1++)
                        {
                            if (TempLovFbbCondition[i1].Text == "District")
                            {
                                TempLovFbbCondition.RemoveAt(i1);
                            }

                        }
                    }

                    var data = TempLovFbbCondition.ToList();

                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (dataS == "Operator")
                {
                    var data = ListCurrentOperator.ToList();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                //if(dataS == "Value")
                //{

                //    var data = ListFbbConditionParameterLovv.ToList();

                //    return Json(data, JsonRequestBehavior.AllowGet);
                //}
                else
                {
                    //var result = lov.OrderBy(p => p.OrderBy);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        private ConfigurationLookupParamView SelectFbbCfgLov_searchLookupParam(string p_lookup_name)
        {
            try
            {
                var query = new GetListLookupParameterQuery
                {
                    p_lookup_name = p_lookup_name
                };
                //return _queryProcessor.Execute(query);

                var result = _queryProcessor.Execute(query);
                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new ConfigurationLookupParamView();
            }

        }

        public JsonResult SelectListLookupParam()
        {
            try
            {
                var data = listGetListLookupParamModel.ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectListPriority()
        {
            try
            {
                var data = ListFbbPriorityLov.ToList();
                var minPriority = ListFbbPriorityLov[0].LovValue1.ToSafeInteger();
                var maxPriority = ListFbbPriorityLov[0].LovValue2.ToSafeInteger();
                //List<string> Finallist = new List<string>();
                List<FinallooopNumberlist> Finallist = new List<FinallooopNumberlist>();
                int currentNumber = minPriority;
                while (currentNumber <= maxPriority)
                {
                    //Finallist.Add(string.Format("" + currentNumber + ""));
                    Finallist.Add(new FinallooopNumberlist { Value1 = "" + currentNumber + "", Value2 = "" + currentNumber + "" });
                    currentNumber = currentNumber + 1;
                }
                //Finallist.Add(string.Format("" + currentNumber + ""));

                return Json(Finallist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectListLookupFieldname()
        {
            try
            {
                var data = ListFbbLovDataScreen.ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectListValue()
        {
            try
            {
                var data = ListFbbValueLovv.ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectListEditValue([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var data = "";
                List<GridEditmakeupModel> GridEditmakeupModel = new List<GridEditmakeupModel>();
                List<GridEditmakeupModelTextbox> GridEditmakeupModelTextbox = new List<GridEditmakeupModelTextbox>();
                List<Condition_list> Condition_list = new List<Condition_list>();
                ListFieldnameDupState.Clear();

                if (ListCurrentConditionEditDataTable.Count() > 0)
                {
                    //ListFieldnameDupState.Clear();
                    //ListCurrentConditionEditDataTable.ToList();

                    string[] tokenFieldname = ListCurrentConditionEditDataTable[0].CONDITION_PARAMETER.Split(',');
                    string[] tokenValue = ListCurrentConditionEditDataTable[0].VALUE.Split(',');
                   // string rawValueAll = ListCurrentConditionEditDataTable[0].VALUE ?? "";
                  //  string[] tokenValue = Enumerable.Repeat(rawValueAll, tokenFieldname.Length).ToArray();
                    string[] tokenOperator = ListCurrentConditionEditDataTable[0].OPERATOR.Split(',');
                    string[] tokenID = ListCurrentConditionEditDataTable[0].CONDITION_ID.Split(',');

                    //List<LovValueModel> ValueFieldTemp = new List<LovValueModel> { };
                    //int IntRunningCinditionList = 0;

                    for (int i1 = 0; i1 < tokenFieldname.Count(); i1++)
                    {
                        //Name
                        string TempParameterName = "";
                        string TempType = "";
                        for (int TempListConRun = 0; TempListConRun < ListFbbConditionParameterLovv.Count(); TempListConRun++)
                        {
                            if (ListFbbConditionParameterLovv[TempListConRun].LovValue1 == tokenFieldname[i1])
                            {
                                TempParameterName = ListFbbConditionParameterLovv[TempListConRun].Text;
                                TempType = ListFbbConditionParameterLovv[TempListConRun].DefaultValue;
                            }
                        }

                        ListFieldnameDupState.Add(TempParameterName);
                        string[] tokenValueSub = null;
                        
                        if (TempType == "DROPDOWNLIST")
                        {
                            GridEditmakeupModel.Add(new GridEditmakeupModel { id = i1 + 1, Field_Name = tokenFieldname[i1], Operator = tokenOperator[i1] });


                            tokenValueSub = tokenValue[i1].Split('|');


                            for (int i1ValueSub = 0; i1ValueSub < tokenValueSub.Count(); i1ValueSub++)
                            {
                                //GridEditmakeupModel.Add(new GridEditmakeupModel { EditGridID = i1 + 1, Field_Name = tokenFieldname[i1], Operator = tokenOperator[i1] });
                                if (tokenValueSub[i1ValueSub] != "")
                                {
                                    Condition_list.Add(new Condition_list { Condition_id = tokenID[i1], Conditaion_operator = tokenOperator[i1], Condition_parameter = tokenFieldname[i1], Condition_flag = "modify", Conditaion_value = tokenValueSub[i1ValueSub] });
                                }

                            }
                        }
                        else if (TempType == "TEXTBOX")
                        {
                            GridEditmakeupModelTextbox.Add(new GridEditmakeupModelTextbox { id = i1 + 1, Field_Name = tokenFieldname[i1], Operator = tokenOperator[i1] });
                            Condition_list.Add(new Condition_list { Condition_id = tokenID[i1], Conditaion_operator = tokenOperator[i1], Condition_parameter = tokenFieldname[i1], Condition_flag = "modify", Conditaion_value = tokenValue[i1] });


                        }
                    }

                    tempProvince = "";
                    tempDistrict = "";
                    tempSymptomGroup = "";

                    for (int CheckallTake = 0; CheckallTake < GridEditmakeupModelTextbox.Count(); CheckallTake++)
                    {
                        for (int TakeCheck = 0; TakeCheck < Condition_list.Count(); TakeCheck++)
                        {
                            if (Condition_list[TakeCheck].Condition_parameter == GridEditmakeupModelTextbox[CheckallTake].Field_Name)
                            {
                                //Name
                                GridEditmakeupModelTextbox[CheckallTake].ValueField = Condition_list[TakeCheck].Conditaion_value;

                                string TempParameterText = "";
                                for (int TempListConRun = 0; TempListConRun < ListFbbConditionParameterLovv.Count(); TempListConRun++)
                                {
                                    if (ListFbbConditionParameterLovv[TempListConRun].LovValue1 == Condition_list[TakeCheck].Condition_parameter)
                                    {
                                        TempParameterText = ListFbbConditionParameterLovv[TempListConRun].Text;
                                    }
                                }

                                GridEditmakeupModelTextbox[CheckallTake].Field_Name = TempParameterText;

                            }
                        }
                    }
                        //MakeCondition_list_for_Grid
                    for (int CheckallTake = 0; CheckallTake < GridEditmakeupModel.Count(); CheckallTake++)
                    {
                        List<LovValueModel> ValueFieldTemp = new List<LovValueModel> { };
                        for (int TakeCheck = 0; TakeCheck < Condition_list.Count(); TakeCheck++)
                        {

                            if (Condition_list[TakeCheck].Condition_parameter == GridEditmakeupModel[CheckallTake].Field_Name)
                            {
                                //Name
                                string TempParameterText = "";
                                for (int TempListConRun = 0; TempListConRun < ListFbbConditionParameterLovv.Count(); TempListConRun++)
                                {
                                    if (ListFbbConditionParameterLovv[TempListConRun].LovValue1 == Condition_list[TakeCheck].Condition_parameter)
                                    {
                                        TempParameterText = ListFbbConditionParameterLovv[TempListConRun].Text;
                                    }
                                }

                                ValueFieldTemp.Add(new LovValueModel { LovValue1 = Condition_list[TakeCheck].Conditaion_value, Name = TempParameterText });

                            }

                            if (Condition_list[TakeCheck].Condition_parameter == "v_province")
                            {
                                if(tempProvince != "")
                                {
                                    tempProvince = tempProvince + "," + Condition_list[TakeCheck].Conditaion_value;
                                }
                                else
                                {
                                    tempProvince = Condition_list[TakeCheck].Conditaion_value;
                                }
                            }
                            else if (Condition_list[TakeCheck].Condition_parameter == "v_district")
                            {
                                if (tempDistrict != "")
                                {
                                    tempDistrict = tempDistrict + "," + Condition_list[TakeCheck].Conditaion_value;
                                }
                                else
                                {
                                    tempDistrict = Condition_list[TakeCheck].Conditaion_value;
                                }
                                    
                            }
                            else if (Condition_list[TakeCheck].Condition_parameter == "v_symptom_group")
                            {
                                if (tempSymptomGroup != "")
                                {
                                    tempSymptomGroup = tempSymptomGroup + "," + Condition_list[TakeCheck].Conditaion_value;
                                }
                                else
                                {
                                    tempSymptomGroup = Condition_list[TakeCheck].Conditaion_value;
                                }
                                
                            }

                        }

                        GridEditmakeupModel[CheckallTake].ValueField = ValueFieldTemp;

                        //Name
                        string TempParameterName = "";
                        for (int TempListConRun = 0; TempListConRun < ListFbbConditionParameterLovv.Count(); TempListConRun++)
                        {
                            if (ListFbbConditionParameterLovv[TempListConRun].LovValue1 == GridEditmakeupModel[CheckallTake].Field_Name)
                            {
                                TempParameterName = ListFbbConditionParameterLovv[TempListConRun].Text;
                            }
                        }

                        GridEditmakeupModel[CheckallTake].Field_Name = TempParameterName;
                    }

           

                    var query = new GetLovQuery
                    {
                        LovType = "DROPDOWNLIST",
                        LovName = "Main Package Group"
                    };
                    var lovAll = _queryProcessor.Execute(query) ?? Enumerable.Empty<LovValueModel>();

                    // ใช้เฉพาะที่ ActiveFlag == "Y"
                    var lovActive = lovAll
                        .Where(p => string.Equals(p.ActiveFlag, "Y", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    // 2) สร้างแผนที่ LovValue1 -> Text (กัน null/ช่องว่าง/เคส)
                    var lovMap = lovActive
                        .GroupBy(x => (x.LovValue1 ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            g => g.Key,
                            g => (g.First().Text ?? "").Trim(),
                            StringComparer.OrdinalIgnoreCase
                        );

                    // 3) ฟังก์ชันช่วย: แปลงรหัส -> ข้อความ
                    string ToText(string lovValue1)
                    {
                        var key = (lovValue1 ?? "").Trim();
                        return lovMap.TryGetValue(key, out var text) ? text : lovValue1; // ถ้าไม่เจอ คืนรหัสเดิมไว้ก่อน
                    }

                    // 4) วนปรับค่าใน GridEditmakeupModel
                    for (int i = 0; i < GridEditmakeupModel.Count; i++)
                    {
                        var row = GridEditmakeupModel[i];

                        // --- 4.1 แปลง Field_Name ---
                        // กรณี Field_Name ตอนนี้เป็น 'รหัส' ให้เปลี่ยนเป็น 'Text'
                        row.Field_Name = ToText(row.Field_Name);

                        // --- 4.2 เติม Text ให้ ValueField ---
                        // เคส A: ถ้า ValueField เป็น List<LovValueModel>
                        if (row.ValueField is IEnumerable<LovValueModel> vfList)
                        {
                            foreach (var vf in vfList)
                            {
                                // ใส่ Text ตาม LovValue1
                                vf.Text = ToText(vf.LovValue1);
                            }
                        }
                        // เคส B: ถ้า ValueField เป็น List<string> (เก็บแค่รหัส)
                        else if (row.ValueField is IEnumerable<string> vfCodeList)
                        {
                            var texts = vfCodeList.Select(code => ToText(code)).ToList();

                            // ถ้ามีฟิลด์ไว้โชว์ string รวม (เช่น __ValueFieldDisplay) ก็ใส่ได้:
                            // row.__ValueFieldDisplay = string.Join(", ", texts);

                            // หรือถ้าต้องการแทนค่า ValueField ด้วยข้อความเลย (ขึ้นกับชนิดข้อมูลจริงของโมเดลคุณ)
                            // row.ValueField = texts;  // <- ทำได้เฉพาะกรณี type ของ ValueField เป็น List<string>
                        }
                        // ถ้าเป็นชนิดอื่น ๆ ให้ปรับตามโครงสร้างจริงที่คุณใช้
                    }

                    //Condition_list.Add(new Condition_list { Condition_id })
                    TempEditRulePriority.Condition_list = Condition_list;

                    string dataJson1 = JsonConvert.SerializeObject(GridEditmakeupModelTextbox);
                    string dataJson2 = JsonConvert.SerializeObject(GridEditmakeupModel);



                    //var l1 = JsonConvert.DeserializeObject<IList<EntityEditGridListSum>>(dataJson1);

                    //var l2 = JsonConvert.DeserializeObject<IList<EntityEditGridListSum>>(dataJson2);

                    //// LINQ
                    //var res = l1.Concat(l2).GroupBy(x => x.EditGridID).Select(x => x.Last()).ToList();

                    //// Foraech
                    //var res2 = new List<EntityEditGridListSum>(l1);
                    //foreach (var l2Entity in l2)
                    //{
                    //    var resEntity = res2.FirstOrDefault(x => x.EditGridID == l2Entity.EditGridID);
                    //    if (resEntity == null)
                    //    {
                    //        res2.Add(l2Entity);
                    //    }
                    //    else
                    //    {
                    //        res2[res2.IndexOf(resEntity)] = l2Entity;
                    //    }
                    //}


                    var dataList1 = dataJson1;
                    var dataList2 = dataJson2;
                    //return Json(dataListA1 = dataList1, datalistA2 = datalist2, JsonRequestBehavior.AllowGet);

                    return Json(new
                    {
                        code = "0",
                        msg = "Success",
                        _dataListA1 = dataList1,
                        _dataListA2 = dataList2
                    }, JsonRequestBehavior.AllowGet);
                }

                data = "";
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        [DataContract]
        public class EntityEditGridListSum
        {
            [DataMember(Name = "EditGridID")]
            public int? EditGridID { get; set; }

            [DataMember(Name = "Field_Name")]
            public string Field_Name { get; set; }

            [DataMember(Name = "Operator")]
            public string Operator { get; set; }

            [DataMember(Name = "ValueField")]
            public string ValueField { get; set; }
        }


        public class ListGridEditmakeupModel
        {
            public List<GridEditmakeupModel> GridEditmakeupModel { get; set; }
        }

        public class GridEditmakeupModel
        {
            public int id { get; set; }
            public string Field_Name { get; set; }
            public string Operator { get; set; }
            public List<LovValueModel> ValueField { get; set; }
            //public string Textbox { get; set; }
            //public string Condition_ID { get; set; }
        }

        public class ValueField
        {
            public string LovValue1 { get; set; }
        }

        public class FinallooopNumberlist
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }

        public class OperatorList
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }

        public class ListGridEditmakeupModelTextbox
        {
            public List<GridEditmakeupModelTextbox> GridEditmakeupModel { get; set; }
        }

        public class GridEditmakeupModelTextbox
        {
            public int id { get; set; }
            public string Field_Name { get; set; }
            public string Operator { get; set; }
            public string ValueField { get; set; }
            //public string Textbox { get; set; }
            //public string Condition_ID { get; set; }
        }

        public ActionResult getConfigRulePriorityDataTable([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            var start = request.Page - 1;
            // Paging Length 10,20
            var length = request.PageSize;

            int pageSize = length != null ? Convert.ToInt32(length) : 20;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var searchEventModel = new JavaScriptSerializer().Deserialize<DataConfigRulePriorityTable>(dataS);
            string ColummName = string.Empty;
            string sortType = string.Empty;
            foreach (var SortD in request.Sorts)
            {
                ColummName = SortD.Member.ToSafeString();
                sortType = SortD.SortDirection.ToSafeString();
            }
            var SortData = new ConfigurationRulePriorityView
            {
                dataConfigRulePriority = null
            };

            var result = GetConfigRulePriorityTable(searchEventModel);
            if (result != null)
            {
                if (result.dataConfigRulePriority.Count() == 0)
                {
                    //SortData.dataConfigRulePriority = result.dataConfigRulePriority;
                    //SortData.dataConfigRulePriority.Add(new DataConfigRulePriorityTable { RULE_NAME = "No Data found." });

                    return Json(new
                    {
                        Data = SortData.dataConfigRulePriority,
                        Total = 0
                    });
                }

                SortData.dataConfigRulePriority = result.dataConfigRulePriority;

                for (int i = 0; i < SortData.dataConfigRulePriority.Count; i++)
                {
                    string TextForReplace = SortData.dataConfigRulePriority[i].LOOKUP_PARAMETER_DISPLAY;
                    string TextAlreadyReplace = TextForReplace.Replace(",", " </br> ");
                    SortData.dataConfigRulePriority[i].LOOKUP_PARAMETER_DISPLAY = TextAlreadyReplace;
                    //SortData.dataConfigRulePriority[i] = SortData.dataConfigRulePriority[i].LOOKUP_PARAMETER_DISPLAY.Replace(",", " </br> ");
                }

                if (ColummName != "")
                {
                    if (sortType == "Ascending")
                    {
                        if (ColummName == "RULE_NAME") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderBy(o => o.RULE_NAME).ToList(); }
                        else if (ColummName == "PRIORITY") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderBy(o => o.PRIORITY.ToSafeInteger()).ToList(); }
                        else if (ColummName == "LOOKUP_NAME") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderBy(o => o.LOOKUP_NAME).ToList(); }
                        else if (ColummName == "LOOKUP_PARAMETER_DISPLAY") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderBy(o => o.LOOKUP_PARAMETER_DISPLAY).ToList(); }
                        else if (ColummName == "EFFECTIVE_DATE_START") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderBy(o => o.EFFECTIVE_DATE_START_DT).ToList(); }
                        else if (ColummName == "EFFECTIVE_DATE_END") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderBy(o => o.EFFECTIVE_DATE_END_DT).ToList(); }
                        else { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderBy(o => o.RULE_NAME).ToList(); }
                    }
                    else
                    {
                        if (ColummName == "RULE_NAME") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderByDescending(o => o.RULE_NAME).ToList(); }
                        else if (ColummName == "PRIORITY") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderByDescending(o => o.PRIORITY.ToSafeInteger()).ToList(); }
                        else if (ColummName == "LOOKUP_NAME") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderByDescending(o => o.LOOKUP_NAME).ToList(); }
                        else if (ColummName == "LOOKUP_PARAMETER_DISPLAY") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderByDescending(o => o.LOOKUP_PARAMETER_DISPLAY).ToList(); }
                        else if (ColummName == "EFFECTIVE_DATE_START") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderByDescending(o => o.EFFECTIVE_DATE_START_DT).ToList(); }
                        else if (ColummName == "EFFECTIVE_DATE_END") { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderByDescending(o => o.EFFECTIVE_DATE_END_DT).ToList(); }
                        else { SortData.dataConfigRulePriority = result.dataConfigRulePriority.OrderByDescending(o => o.RULE_NAME).ToList(); }
                    }
                }
                else
                {
                    SortData.dataConfigRulePriority = result.dataConfigRulePriority;
                }

                SortData.dataConfigRulePriority = SortData.dataConfigRulePriority.Skip(skip * pageSize).Take(pageSize).ToList();
                return Json(new
                {
                    Data = SortData.dataConfigRulePriority,
                    Total = result.dataConfigRulePriority.Count
                });
            }
            else
            {
                return null;
            }

            //MockFakeData
            //var result1 = new List<DataConfigRulePriorityTable>
            //{
            //    new DataConfigRulePriorityTable { RULE_NAME = "Test Rule Name 1" ,LOOKUP_NAME ="A000",PRIORITY = "98",LOOKUP_PARAMETER = "Sub Contact Type 01",EFFECTIVE_DATE_START = "1-May-2024" , EFFECTIVE_DATE_END ="30-May-2025" },
            //    new DataConfigRulePriorityTable { RULE_NAME = "Test Rule Name 2" ,LOOKUP_NAME ="A111",PRIORITY = "98",LOOKUP_PARAMETER = "Sub Contact Type 02",EFFECTIVE_DATE_START = "1-May-2024" , EFFECTIVE_DATE_END ="30-May-2025" }
            //};

            //SortData.dataConfigRulePriority = result1.OrderByDescending(o => o.LOOKUP_NAME).ToList();
            //return Json(new
            //{
            //    Data = SortData.dataConfigRulePriority,
            //    Total = result1.Count
            //});
        }

        private ConfigurationRulePriorityView GetConfigRulePriorityTable(DataConfigRulePriorityTable searchmodel)
        {
            try
            {

                if (searchmodel.PRIORITY == "ALL")
                {
                    searchmodel.PRIORITY = "0";
                }

                var query = new GetListRulePriorityQuery()
                {
                    RULE_ID = searchmodel.RULE_ID,
                    RULE_NAME = searchmodel.RULE_NAME,
                    PRIORITY = searchmodel.PRIORITY.ToSafeDecimal(),
                    LOOKUP_NAME = searchmodel.LOOKUP_NAME,
                    EFFECTIVE_DATE_START = searchmodel.EFFECTIVE_DATE_START,
                    EFFECTIVE_DATE_END = searchmodel.EFFECTIVE_DATE_END,

                };

                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    foreach (var item in result.dataConfigRulePriority)
                    {
                        if (item.EFFECTIVE_DATE_START != "")
                        {
                            item.EFFECTIVE_DATE_START_DT = DateTime.ParseExact(item.EFFECTIVE_DATE_START, Constants.DisplayFormats.DateFormat, Constants.DisplayFormats.DefaultCultureInfo);
                            //item.EFFECTIVE_DATE_START_DT = DateTime.ParseExact(item.EFFECTIVE_DATE_START, Constants.DisplayFormats.DateTimeFormat, Constants.DisplayFormats.DefaultCultureInfo);
                            if (item.EFFECTIVE_DATE_START_DT.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_START = item.EFFECTIVE_DATE_START_DT.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_START = "";
                            }
                        }

                        if(item.EFFECTIVE_DATE_END != "")
                        {
                            item.EFFECTIVE_DATE_END_DT = DateTime.ParseExact(item.EFFECTIVE_DATE_END, Constants.DisplayFormats.DateFormat, Constants.DisplayFormats.DefaultCultureInfo);
                            //item.EFFECTIVE_DATE_END_DT = DateTime.ParseExact(item.EFFECTIVE_DATE_END, Constants.DisplayFormats.DateFormat, Constants.DisplayFormats.DefaultCultureInfo);
                            if (item.EFFECTIVE_DATE_END_DT.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_END = item.EFFECTIVE_DATE_END_DT.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_END = "";
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

        }

        //GetEditDatabyRuleId
        public ActionResult GetEditRuleData([DataSourceRequest] DataSourceRequest request, string RULE_ID_DATA = "")
        {
            try
            {
                //RULE_ID_DATA
                if (RULE_ID_DATA != "")
                {
                    var query = new GetListEditRulePriorityQuery()
                    {
                        RULE_ID = RULE_ID_DATA,
                    };
                    var result = _queryProcessor.Execute(query);

                    ////RULE_ID = result.ret_rule_id,
                    //ViewBag.RULE_ID = result.ret_rule_id;
                    ////RULE_NAME = result.ret_rule_name,
                    //ViewBag.RULE_NAME = result.ret_rule_name;
                    ////RULE_PRIORITY = result.ret_priority,
                    //ViewBag.RULE_PRIORITY = result.ret_priority;
                    ////RULE_lmr_flag = result.ret_lmr_flag,
                    //ViewBag.RULE_lmr_flag = result.ret_lmr_flag;
                    ////RULE_lookup_name = result.ret_lookup_name,
                    //ViewBag.RULE_lookup_name = result.ret_lookup_name;
                    ////RULE_lookup_parameter = result.ret_lookup_parameter,
                    //ViewBag.RULE_lookup_parameter = result.ret_lookup_parameter;
                    ////RULE_param_id = result.ret_rule_param_id,
                    //ViewBag.RULE_param_id = result.ret_rule_param_id;
                    ////RULE_effective_date_start = result.ret_effective_date_start,
                    //ViewBag.RULE_effective_date_start = result.ret_effective_date_start;
                    ////RULE_effective_date_end = result.ret_effective_date_end,
                    //ViewBag.RULE_effective_date_end = result.ret_effective_date_end;
                    ////RULE_code = result.ret_code,
                    //ViewBag.RULE_code = result.ret_code;
                    ////RULE_msg = result.ret_msg
                    //ViewBag.RULE_msg = result.ret_msg;

                    ListCurrentConditionEditDataTable = result.dataConfigEditRulePriority;

                    string[] ParamID = result.ret_rule_param_id.Split(',');
                    string[] ParamName = result.ret_lookup_parameter.Split(',');
                    for (int iparam = 0; iparam < ParamName.Count(); iparam++) {
                        ListCurrentLookup_param_list.Add(new Lookup_param_list { Param_flag = "modify", Param_name = ParamName[iparam], Param_rule_id = ParamID[iparam] });
                    }

                    //return result;
                    if (result != null)
                    {
                        //TempEditRulePriority
                        TempEditRulePriority = new SaveConfigRulePriorityCommand();

                        TempEditRulePriority.Rule_id = RULE_ID_DATA;
                        TempEditRulePriority.Rule_name = result.ret_rule_name;
                        TempEditRulePriority.Lookup_name = result.ret_lookup_name;
                        //TempEditRulePriority.Lookup_param_list = result.ret_lookup_parameter;
                        TempEditRulePriority.effective_start = result.ret_effective_date_start;
                        if (result.ret_effective_date_end == "null") {
                            TempEditRulePriority.effective_end = "";
                        }
                        else
                        {
                            TempEditRulePriority.effective_end = result.ret_effective_date_end;
                        }

                        TempEditRulePriority.Lmr_flag = result.ret_lmr_flag;
                        //TempEditRulePriority.Condition_list = ListCurrentConditionEditDataTable;

                        return Json(new
                        {
                            code = "0",
                            msg = "Success",
                            _RULE_NAME = result.ret_rule_name,
                            _RULE_PRIORITY = result.ret_priority,
                            _RULE_lookup_name = result.ret_lookup_name,
                            _RULE_lookup_parameter = result.ret_lookup_parameter,
                            _RULE_DATE_FROM = result.ret_effective_date_start,
                            _RULE_DATE_END = TempEditRulePriority.effective_end,
                            _RULE_LMR = TempEditRulePriority.Lmr_flag

                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { code = "1", msg = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { code = "2", msg = "Exception" }, JsonRequestBehavior.AllowGet);
                }


                //var result = _queryProcessor.Execute(query);
                //return result;

                //PR2406180001
                //return Json(new { code = "0", msg = "Success" }, JsonRequestBehavior.AllowGet);
                //return Json(new { code = "1", msg = "Fail" }, JsonRequestBehavior.AllowGet);
                //return Json(new { code = "2", msg = "Exception" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                string _msg = string.Empty;
                _msg = "Please Contact System Admin";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult AddNewRulepriorityDataTable([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                return Json(new { code = "-1", msg = "Maximun Length is  Invaild" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                string _msg = string.Empty;
                _msg = "Please Contact System Admin";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);

            }


        }

        public ActionResult AddNewRulePriorityConfiguration([DataSourceRequest] DataSourceRequest request, string Rule_id = "", string Rule_name = "", string Priority = "", string Lookup_name = "", string Lookup_param_list = "", string effective_start = "", string effective_end = "", string Lmr_flag = "", string Condition_list = "", string Create_by = "", string Modified_by = "")
        {
            try
            {
                
                SaveConfigRulePriorityCommand SaveConfigRulePriorityCommand = new SaveConfigRulePriorityCommand();
                List<Lookup_param_list> SumLookup_param_list = new List<Lookup_param_list>();
                List<Condition_list> SumCondition_list = new List<Condition_list>();

                //OutGrid
                SaveConfigRulePriorityCommand.Rule_name = Rule_name;
                SaveConfigRulePriorityCommand.Priority = Priority;
                SaveConfigRulePriorityCommand.Lookup_name = Lookup_name;
                SaveConfigRulePriorityCommand.effective_start = effective_start;
                SaveConfigRulePriorityCommand.effective_end = effective_end;
                SaveConfigRulePriorityCommand.Create_by = sessionUsername;

                //LMR_FLAG_CHECK
                if (Lmr_flag == "")
                {
                    SaveConfigRulePriorityCommand.Lmr_flag = "N";
                }
                else if (Lmr_flag != "")
                {
                    SaveConfigRulePriorityCommand.Lmr_flag = "Y";
                }

                //Lookup_param_list
                var SortData = new ConfigurationLookupParamView
                {
                    dataConfigLookupParam = null
                };

                var result = SelectFbbCfgLov_searchLookupParam(Lookup_name);
                if (result != null)
                {
                    SortData.dataConfigLookupParam = result.dataConfigLookupParam;
                }

                string[] tokensLookup_param_list = Lookup_param_list.Split(',');
                for (int i5 = 0; i5 < tokensLookup_param_list.Count(); i5++)
                {
                    if (SortData.dataConfigLookupParam != null)
                    {
                        for (int i6 = 0; i6 < SortData.dataConfigLookupParam.Count(); i6++)
                        {
                            if (SortData.dataConfigLookupParam[i6].PARAMETER_NAME == tokensLookup_param_list[i5])
                            {
                                Lookup_param_list TempLookup_param_list = new Lookup_param_list();
                                TempLookup_param_list.Param_rule_id = "0";
                                TempLookup_param_list.Param_name = SortData.dataConfigLookupParam[i6].PARAMETER_NAME;
                                TempLookup_param_list.Param_flag = "new";
                                SumLookup_param_list.Add(TempLookup_param_list);
                            }
                        }

                    }
                }
                //END_Lookup_param_list
                
                //Condition_List
                JArray jsonArray = JArray.Parse(Condition_list);
                for (int iJ1 = 0; iJ1 < jsonArray.Count(); iJ1++)
                {
                    dynamic data = JObject.Parse(jsonArray[iJ1].ToString());
                    var P1 = data.Field_Name.Value;
                    var P2 = data.Operator.Value;
                    var P3 = "";
                    int textFlag = 0;

                    try
                    {
                        P3 = data.ValueField.Value;
                        textFlag = 1;
                    }
                    catch
                    {
                        textFlag = 2;
                    }
                      
                    if(textFlag == 2)
                    {
                        var SumP3 = "";

                        int length = data.ValueField.Count;
                        //int iJ2 = 0;
                        for (int iJ2 = 0; iJ2 < length; iJ2++)
                        {
                            var SData = data.ValueField[iJ2];
                            var P3Temp = SData.LovValue1.Value;
                            if(iJ2 == 0)
                            {
                                SumP3 = P3Temp;
                            }
                            else
                            {
                                SumP3 = SumP3 + "|" + P3Temp;
                            }
                            
                        }
                        //equal
                        P3 = SumP3;
                    }

                    string TempParameterName = "";
                    for (int TempListConRun = 0; TempListConRun < ListFbbConditionParameterLovv.Count(); TempListConRun++)
                    {
                        if (ListFbbConditionParameterLovv[TempListConRun].Text == P1)
                        {
                            TempParameterName = ListFbbConditionParameterLovv[TempListConRun].LovValue1;
                        }
                    }

                    if (TempParameterName != "")
                    {
                        SumCondition_list.Add(new Condition_list {

                            Condition_parameter = TempParameterName,
                            Condition_id = "0",
                            Condition_flag = "new",
                            Conditaion_operator = P2,
                            Conditaion_value = P3
                        });
                    }
                        
                }
                
                //Final_equal_section
                SaveConfigRulePriorityCommand.Lookup_param_list = SumLookup_param_list;
                SaveConfigRulePriorityCommand.Condition_list = SumCondition_list;
                var json = new JavaScriptSerializer().Serialize(SaveConfigRulePriorityCommand);

                _SaveConfigRulePriorityCommand.Handle(SaveConfigRulePriorityCommand);

                if (SaveConfigRulePriorityCommand.return_code == "0")
                {
                    return Json(new
                    {
                        code = SaveConfigRulePriorityCommand.return_code,
                        msg = SaveConfigRulePriorityCommand.return_msg
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        code = SaveConfigRulePriorityCommand.return_code,
                        msg = SaveConfigRulePriorityCommand.return_msg
                    }, JsonRequestBehavior.AllowGet);
                }
                

            }
            catch (Exception ex)
            {
                string _msg = string.Empty;
                _msg = "Please Contact System Admin. ( FATAL ERROR )";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditRulePriorityConfiguration([DataSourceRequest] DataSourceRequest request, string Rule_id = "", string Rule_name = "", string Priority = "", string Lookup_name = "", string Lookup_param_list = "", string effective_start = "", string effective_end = "", string Lmr_flag = "", string Condition_list = "", string Create_by = "", string Modified_by = "")
        {
            try
            {
                
                SaveEditConfigRulePriorityCommand SaveEditConfigRulePriorityCommand = new SaveEditConfigRulePriorityCommand();
                List<Lookup_param_list_Edit> SumLookup_param_list_Edit = new List<Lookup_param_list_Edit>();
                List<Condition_list_Edit> SumCondition_list_Edit = new List<Condition_list_Edit>();

                //OutGrid
                SaveEditConfigRulePriorityCommand.Rule_id = TempEditRulePriority.Rule_id;
                SaveEditConfigRulePriorityCommand.Rule_name = Rule_name;
                SaveEditConfigRulePriorityCommand.Priority = Priority;
                SaveEditConfigRulePriorityCommand.Lookup_name = Lookup_name;
                SaveEditConfigRulePriorityCommand.effective_start = effective_start;
                SaveEditConfigRulePriorityCommand.effective_end = effective_end;
                SaveEditConfigRulePriorityCommand.Create_by = sessionUsername;

                //LMR_FLAG_CHECK
                if (Lmr_flag == "")
                {
                    SaveEditConfigRulePriorityCommand.Lmr_flag = "N";
                }
                else if (Lmr_flag != "")
                {
                    SaveEditConfigRulePriorityCommand.Lmr_flag = "Y";
                }

                //Lookup_param_list
                var SortData = new ConfigurationLookupParamView
                {
                    dataConfigLookupParam = null
                };

                var result = SelectFbbCfgLov_searchLookupParam(Lookup_name);
                if (result != null)
                {
                    SortData.dataConfigLookupParam = result.dataConfigLookupParam;
                }
                
                string[] tokensLookup_param_list = Lookup_param_list.Split(',');
                for (int i5 = 0; i5 < tokensLookup_param_list.Count(); i5++)
                {
                    if (SortData.dataConfigLookupParam != null)
                    {
                        for (int i6 = 0; i6 < SortData.dataConfigLookupParam.Count(); i6++)
                        {
                            if (SortData.dataConfigLookupParam[i6].PARAMETER_NAME == tokensLookup_param_list[i5])
                            {
                                Lookup_param_list_Edit TempLookup_param_list_Edit = new Lookup_param_list_Edit();
                                TempLookup_param_list_Edit.Param_rule_id = "0";
                                TempLookup_param_list_Edit.Param_name = SortData.dataConfigLookupParam[i6].PARAMETER_NAME;
                                TempLookup_param_list_Edit.Param_flag = "new";

                                for (int iParam = 0; iParam < ListCurrentLookup_param_list.Count(); iParam++)
                                {
                                    if (TempLookup_param_list_Edit.Param_name == ListCurrentLookup_param_list[iParam].Param_name)
                                    {
                                        TempLookup_param_list_Edit.Param_rule_id = ListCurrentLookup_param_list[iParam].Param_rule_id;
                                        TempLookup_param_list_Edit.Param_flag = "modify";
                                    }
                                }

                                //Add_this_row _if_modify_Param
                                SumLookup_param_list_Edit.Add(TempLookup_param_list_Edit);
                            }

                        }
                        
                    }
                }

                //Add-Delete-Param-List
                for (int iParam = 0; iParam < ListCurrentLookup_param_list.Count(); iParam++)
                {
                    Lookup_param_list_Edit TempLookup_param_list_delete = new Lookup_param_list_Edit();
                    int deleteFlag = 0;

                    for (int iSubParamDelete = 0; iSubParamDelete < SumLookup_param_list_Edit.Count(); iSubParamDelete++)
                    {
                        //check_if_list_has_delete_already_param
                        if (SumLookup_param_list_Edit[iSubParamDelete].Param_name == ListCurrentLookup_param_list[iParam].Param_name)
                        {
                            deleteFlag = 1;
                        }

                    }

                    if (deleteFlag == 0)
                    {
                        TempLookup_param_list_delete.Param_name = ListCurrentLookup_param_list[iParam].Param_name;
                        TempLookup_param_list_delete.Param_flag = "delete";
                        TempLookup_param_list_delete.Param_rule_id = ListCurrentLookup_param_list[iParam].Param_rule_id;
                        SumLookup_param_list_Edit.Add(TempLookup_param_list_delete);
                    }

                }
                //END_Lookup_param_list

                //Condition_List
                JArray jsonArray = JArray.Parse(Condition_list);
                for (int iJ1 = 0; iJ1 < jsonArray.Count(); iJ1++)
                {
                    dynamic data = JObject.Parse(jsonArray[iJ1].ToString());
                    var P1 = data.Field_Name.Value;
                    var P2 = data.Operator.Value;
                    var P3 = "";
                    int textFlag = 0;

                    //Temp-name-for-compare
                    string TempParameterName = "";
                    for (int TempListConRun = 0; TempListConRun < ListFbbConditionParameterLovv.Count(); TempListConRun++)
                    {
                        if (ListFbbConditionParameterLovv[TempListConRun].Text == P1)
                        {
                            TempParameterName = ListFbbConditionParameterLovv[TempListConRun].LovValue1;
                        }
                    }

                    string flagTempID = "0";
                    string FlagInsertandModify = "INSERT";
                    //Find-correct-condition-list-from-Edit-Temp
                    for (int iTemp1 = 0; iTemp1 < TempEditRulePriority.Condition_list.Count(); iTemp1++)
                    {
                        if (TempEditRulePriority.Condition_list[iTemp1].Condition_parameter == TempParameterName)
                        {
                            FlagInsertandModify = "MODIFY";
                            flagTempID = TempEditRulePriority.Condition_list[iTemp1].Condition_id;
                        }
                    }

                    try
                    {
                        P3 = data.ValueField.Value;
                        textFlag = 1;
                    }
                    catch
                    {
                        textFlag = 2;
                    }

                    if (textFlag == 2)
                    {
                        var SumP3 = "";

                        int length = data.ValueField.Count;
                        //int iJ2 = 0;
                        for (int iJ2 = 0; iJ2 < length; iJ2++)
                        {
                            var SData = data.ValueField[iJ2];
                            var P3Temp = SData.LovValue1.Value;
                            if (iJ2 == 0)
                            {
                                SumP3 = P3Temp;
                            }
                            else
                            {
                                SumP3 = SumP3 + "|" + P3Temp;
                            }

                        }
                        //equal
                        P3 = SumP3;
                    }

                    if(FlagInsertandModify == "INSERT")
                    {
                        //add-new-case
                        SumCondition_list_Edit.Add(new Condition_list_Edit
                        {
                            Condition_parameter = TempParameterName,
                            Condition_id = "0",
                            Condition_flag = "new",
                            Conditaion_operator = P2,
                            Conditaion_value = P3
                        });
                    }
                    else if(FlagInsertandModify == "MODIFY")
                    {
                        //modify-case
                        SumCondition_list_Edit.Add(new Condition_list_Edit
                        {
                            Condition_parameter = TempParameterName,
                            Condition_id = flagTempID,
                            Condition_flag = "modify",
                            Conditaion_operator = P2,
                            Conditaion_value = P3
                        });
                    }

                }

                //Delete-row-add-from-temp-compare
                for (int iTemp1 = 0; iTemp1 < TempEditRulePriority.Condition_list.Count(); iTemp1++)
                {
                    int deleteFlag = 1;
                    for (int itemp2 = 0; itemp2 < SumCondition_list_Edit.Count(); itemp2++)
                    {
                        if (TempEditRulePriority.Condition_list[iTemp1].Condition_parameter == SumCondition_list_Edit[itemp2].Condition_parameter)
                        {
                            deleteFlag = 0;
                        }
                    }
                    
                    if(deleteFlag == 1)
                    {
                        var P3 = "";
                        for (int itempP3 = 0; itempP3 < TempEditRulePriority.Condition_list.Count(); itempP3++)
                        {
                            if (TempEditRulePriority.Condition_list[itempP3].Condition_parameter == TempEditRulePriority.Condition_list[iTemp1].Condition_parameter)
                            {
                                P3 = P3 + "|" + TempEditRulePriority.Condition_list[itempP3].Conditaion_value;
                            }
                        }
                        //modify-case
                        SumCondition_list_Edit.Add(new Condition_list_Edit
                        {
                            Condition_parameter = TempEditRulePriority.Condition_list[iTemp1].Condition_parameter,
                            Condition_id = TempEditRulePriority.Condition_list[iTemp1].Condition_id,
                            Condition_flag = "delete",
                            Conditaion_operator = TempEditRulePriority.Condition_list[iTemp1].Conditaion_operator,
                            Conditaion_value = P3
                        });
                    }
                    
                }
                

                //Final_equal_section
                SaveEditConfigRulePriorityCommand.Lookup_param_list_Edit = SumLookup_param_list_Edit;
                SaveEditConfigRulePriorityCommand.Condition_list_Edit = SumCondition_list_Edit;

                var json = new JavaScriptSerializer().Serialize(SaveEditConfigRulePriorityCommand);

                _SaveEditConfigRulePriorityCommand.Handle(SaveEditConfigRulePriorityCommand);
                

                if (SaveEditConfigRulePriorityCommand.return_code == "0")
                {
                    return Json(new
                    {
                        code = SaveEditConfigRulePriorityCommand.return_code,
                        msg = SaveEditConfigRulePriorityCommand.return_msg
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        code = SaveEditConfigRulePriorityCommand.return_code,
                        msg = SaveEditConfigRulePriorityCommand.return_msg
                    }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                string _msg = string.Empty;
                _msg = "Please Contact System Admin. ( FATAL ERROR )";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public string Storevaluemakeup(string lookupdata)
        {
            var param = "";
            if (lookupdata == "Province")
            {
                param = "v_province";
            }
            else if (lookupdata == "District")
            {
                param = "v_district";
            }
            else if (lookupdata == "Sub District")
            {
                param = "v_subdistrict";
            }
            else if (lookupdata == "Symptom Group")
            {
                param = "v_symptom_group";
            }
            else if (lookupdata == "Symptom Group")
            {
                param = "v_symptom_group";
            }
            else if (lookupdata == "Symptom Group")
            {
                param = "v_symptom_group";
            }
            else if (lookupdata == "Symptom Name")
            {
                param = "p_Reject_reason";
            }
            else if (lookupdata == "Address ID Flag" || lookupdata == "Address id Flag")
            {
                param = "v_fttr_flag";
            }
            else
            {
                param = "nodatamatch";
            }
            return param;
        }

        public ActionResult ClearDuplicateValue([DataSourceRequest] DataSourceRequest request, string param = "")
        {
            try
            {
                ListFieldnameDupState.Clear();
                
                    return Json(new
                    {
                        code = "1",
                        msg = "correct"
                    }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                string _msg = string.Empty;
                _msg = "Please Contact System Admin";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddDupfromGridModel([DataSourceRequest] DataSourceRequest request, string gridFieldnameModel = "")
        {
            try {

                string[] tokenFieldname = gridFieldnameModel.Split(',');
                //string CurrentCode = "-1";
                //string currentProvinceFilterState = "-1";
                //string currentDistrictFilterState = "-1";
                //Operator

                for(int i2 = 0; i2 < tokenFieldname.Count(); i2++)
                {
                    int pFrom = tokenFieldname[i2].IndexOf("\"") + 1;
                    int pTo = tokenFieldname[i2].LastIndexOf("\"");

                    //if (pFrom >= 0 && pTo >= 0)
                    //{
                    tokenFieldname[i2] = tokenFieldname[i2].Substring(pFrom, pTo - pFrom);
                        //ConditionTokenOperator = result;
                    //}
                }

                
                //ListFieldnameDupState.Clear();
                ////DuplicateCheckandStroingLogic
                if (tokenFieldname.Count() > 0)
                {
                    ListFieldnameDupState.Clear();

                    for (int iDupTemp = 0; iDupTemp < tokenFieldname.Count(); iDupTemp++)
                    {
                        //    if (ListFieldnameDupState[iDupTemp] == OldFieldnameDate)
                        //    {
                        //        ListFieldnameDupState.RemoveAt(iDupTemp);
                        //    }
                        ListFieldnameDupState.Add(tokenFieldname[iDupTemp]);
                    }

                    //ListFieldnameDupState.Add(FieldnameData);

                }
                //else if (ListFieldnameDupState.Count() == 0)
                //{
                //    ListFieldnameDupState.Add(FieldnameData);
                //}

                return Json(new
                {
                    code = "0",
                    msg = "filter failed no data"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckCaseStepGridModel([DataSourceRequest] DataSourceRequest request, string gridFieldnameModel = "")
        {
            try
            {
                int flagReturnflag = 0;

                if (gridFieldnameModel != "" && gridFieldnameModel != null)
                {
                    string flagCheckStep = "";

                    if (gridFieldnameModel == "Province")
                    {
                        flagCheckStep = "Province";
                    }
                    else if (gridFieldnameModel == "District")
                    {
                        flagCheckStep = "District";
                    }
                    else if (gridFieldnameModel == "Sub District")
                    {
                        flagCheckStep = "Sub District";
                    }
                    else if (gridFieldnameModel == "Symptom Name")
                    {
                        flagCheckStep = "Symptom Name";
                    }
                    else if (gridFieldnameModel == "Symptom Group")
                    {
                        flagCheckStep = "Symptom Group";
                    }

                    //int flagReturnflag = 0;

                    for (int i2 = 0; i2 < ListFieldnameDupState.Count(); i2++)
                    {
                        if(flagCheckStep == "Province")
                        {
                            if (ListFieldnameDupState[i2] == "District")
                            {
                                flagReturnflag = 99;
                            }

                            if (ListFieldnameDupState[i2] == "Sub District")
                            {
                                flagReturnflag = 99;
                            }

                        }
                        else if (flagCheckStep == "District")
                        {
                            if (ListFieldnameDupState[i2] == "Sub District")
                            {
                                flagReturnflag = 99;
                            }
                            
                        }
                        else if (flagCheckStep == "Symptom Group")
                        {
                            if (ListFieldnameDupState[i2] == "Symptom Name")
                            {
                                flagReturnflag = 98;
                            }
                        }

                    }

                }
                else
                {
                    flagReturnflag = 100;
                }
                

                return Json(new
                {
                    code = flagReturnflag,
                    msg = "Return code"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteConfigurationRulePriority([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(dataS))
                {
                    //var DeleteDataModel = new JavaScriptSerializer().Deserialize<DeleteConfigLookup>(dataS);
                    var command = new DeleteConfigurationRulePriorityCommand
                    {
                        RULE_ID = dataS

                    };
                    _DeleteConfigurationRulePriorityCommand.Handle(command);
                    if (command.return_code != null)
                    {
                        //string MSG = command.return_code.ToSafeString();
                        return Json(new
                        {
                            code = command.return_code.ToSafeString(),
                            msg = command.return_msg.ToSafeString()
                            //msg = MSG
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            code = "2",
                            msg = "เกิดข้อผิดพลาด ไม่สามารถทำรายการได้"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        code = "2",
                        msg = "เกิดข้อผิดพลาด ข้อมูลที่ต้องการลบไม่ถูกต้อง"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string _msg = string.Empty;
                _msg = "เกิดข้อผิดพลาด ไม่สามารถทำรายการได้ กรุณาติดต่อ Admin";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
