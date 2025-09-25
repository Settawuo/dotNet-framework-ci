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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.IO;
using System.Web.Http;
using System.Globalization;
using System.Data;
using System.ComponentModel;
using FBBConfig.Models;
using Newtonsoft.Json.Linq;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using System.Reflection;

namespace FBBConfig.Controllers
{
    public partial class ConfigurationLookupController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<DeleteConfigurationLookupCommand> _deleteconfigurationLookupCommand;
        private readonly ICommandHandler<AddConfigurationLookupCommand> _addconfigurationLookupCommand;
        private readonly ICommandHandler<UpdateConfigurationLookupCommand> _updateconfigurationLookupCommand;
        static List<LovModel> ListFbbCfgLov = null;
        static List<LovModel> ListFbbCfgLov_Upsert = null;
        static List<GetListLookupNameModel> listGetListLookupNameModel = null;
        static List<SymptomNameDropDown> dataDropDownSymptomName = null;
        static List<DistrictDropDown> dataDropDownDistrict = null;
        static List<SubDistrictDropDown> dataDropDownSubDistrict = null;
        static List<result_lookup_id_cur> Result_Lookup_Id_Curs = null;
        static List<string> param_name_lookup = null;

        public ConfigurationLookupController(IQueryProcessor queryProcessor, ILogger logger,
            ICommandHandler<DeleteConfigurationLookupCommand> deleteconfigurationLookupCommand,
            ICommandHandler<AddConfigurationLookupCommand> addconfigurationLookupCommand,
            ICommandHandler<UpdateConfigurationLookupCommand> updateconfigurationLookupCommand)
        {
            _queryProcessor = queryProcessor;
            _deleteconfigurationLookupCommand = deleteconfigurationLookupCommand;
            _addconfigurationLookupCommand = addconfigurationLookupCommand;
            _updateconfigurationLookupCommand = updateconfigurationLookupCommand;
            _Logger = logger;
        }
        public ActionResult Index()
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            ViewBag.User = CurrentUser;
            ViewBag.UserGroup = GetUserGroup();
            var User = CurrentUser;
            ViewBag.username = User.UserName;

            ListFbbCfgLov = new List<LovModel>();
            listGetListLookupNameModel = new List<GetListLookupNameModel>();
            Result_Lookup_Id_Curs = new List<result_lookup_id_cur>();
            param_name_lookup = new List<string>();
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
            if (ListFbbCfgLov == null || ListFbbCfgLov.Count == 0)
                ListFbbCfgLov = SelectFbbCfgLov_searchLookup("SEARCH_LOOKUP");
            if (ListFbbCfgLov_Upsert == null || ListFbbCfgLov_Upsert.Count == 0)
                ListFbbCfgLov_Upsert = SelectFbbCfgLov_searchLookup("LOOKUP_NAME_UPSERT");


            var LovDataScreen = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
            ViewBag.configscreenAddLookup = LovDataScreen;
            ViewBag.configscreenLookup = ListFbbCfgLov;
            ViewBag.configscreenLookup_Upsert = ListFbbCfgLov_Upsert;
        }
        public ActionResult getDataConfigLookupTable([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            var start = request.Page - 1;
            // Paging Length 10,20
            var length = request.PageSize;

            int pageSize = length != null ? Convert.ToInt32(length) : 20;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            try
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<DataConfigLookupTable>(dataS);
                string ColummName = string.Empty;
                string sortType = string.Empty;

                foreach (var SortD in request.Sorts)
                {
                    ColummName = SortD.Member.ToSafeString();
                    sortType = SortD.SortDirection.ToSafeString();
                }

                var SortData = new ConfigurationLookupView
                {
                    dataConfigLookup = null
                };

                var result = GetDataConfigLookupTable(searchEventModel);

                if (result.dataConfigLookup != null)
                {
                    SortData.dataConfigLookup = result.dataConfigLookup;

                    for (int i = 0; i < SortData.dataConfigLookup.Count; i++)
                    {
                        string TextForReplace_ontop = SortData.dataConfigLookup[i].ONTOP_LOOKUP;
                        string TextAlreadyReplace_ontop = TextForReplace_ontop.Replace(",", " </br> ");
                        SortData.dataConfigLookup[i].ONTOP_LOOKUP = TextAlreadyReplace_ontop;
                        if (SortData.dataConfigLookup[i].ONTOP_FLAG == "Y")
                        {
                            SortData.dataConfigLookup[i].ONTOP_FLAG = "Ontop";
                        }
                        else
                        {
                            SortData.dataConfigLookup[i].ONTOP_FLAG = "Main";
                        }

                        string TextForReplace_rule = SortData.dataConfigLookup[i].RULE_NAME;
                        string TextAlreadyReplace_rule = TextForReplace_rule.Replace(",", " </br> ");
                        SortData.dataConfigLookup[i].RULE_NAME = TextAlreadyReplace_rule;
                    }

                    if (ColummName != "")
                    {
                        if (sortType == "Ascending")
                        {
                            if (ColummName == "LOOKUP_NAME") { SortData.dataConfigLookup = result.dataConfigLookup.OrderBy(o => o.LOOKUP_NAME).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.dataConfigLookup = result.dataConfigLookup.OrderBy(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ONTOP_FLAG") { SortData.dataConfigLookup = result.dataConfigLookup.OrderBy(o => o.ONTOP_FLAG).ToList(); }
                            else if (ColummName == "ONTOP_LOOKUP") { SortData.dataConfigLookup = result.dataConfigLookup.OrderBy(o => o.ONTOP_LOOKUP).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_START") { SortData.dataConfigLookup = result.dataConfigLookup.OrderBy(o => o.EFFECTIVE_DATE_START_DT).ToList(); }
                            else if (ColummName == "FLAG_DELETE") { SortData.dataConfigLookup = result.dataConfigLookup.OrderBy(o => o.FLAG_DELETE).ToList(); }
                            else { SortData.dataConfigLookup = result.dataConfigLookup.OrderBy(o => o.LOOKUP_NAME).ToList(); }
                        }
                        else
                        {
                            if (ColummName == "LOOKUP_NAME") { SortData.dataConfigLookup = result.dataConfigLookup.OrderByDescending(o => o.LOOKUP_NAME).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.dataConfigLookup = result.dataConfigLookup.OrderByDescending(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ONTOP_FLAG") { SortData.dataConfigLookup = result.dataConfigLookup.OrderByDescending(o => o.ONTOP_FLAG).ToList(); }
                            else if (ColummName == "ONTOP_LOOKUP") { SortData.dataConfigLookup = result.dataConfigLookup.OrderByDescending(o => o.ONTOP_LOOKUP).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_START") { SortData.dataConfigLookup = result.dataConfigLookup.OrderByDescending(o => o.EFFECTIVE_DATE_START_DT).ToList(); }
                            else if (ColummName == "FLAG_DELETE") { SortData.dataConfigLookup = result.dataConfigLookup.OrderByDescending(o => o.FLAG_DELETE).ToList(); }
                            else { SortData.dataConfigLookup = result.dataConfigLookup.OrderByDescending(o => o.LOOKUP_NAME).ToList(); }
                        }
                    }
                    else
                    {
                        SortData.dataConfigLookup = result.dataConfigLookup;
                    }


                    SortData.dataConfigLookup = SortData.dataConfigLookup.Skip(skip * pageSize).Take(pageSize).ToList();
                    return Json(new
                    {
                        Data = SortData.dataConfigLookup,
                        Total = result.dataConfigLookup.Count
                    });
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }


        }
        private ConfigurationLookupView GetDataConfigLookupTable(DataConfigLookupTable searchmodel)
        {
            try
            {
                var query = new GetDataConfigLookupTableQuery()
                {
                    LOOKUP_NAME = searchmodel.LOOKUP_NAME
                };
                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    foreach (var item in result.dataConfigLookup)
                    {
                        item.EFFECTIVE_DATE_START_DT = DateTime.ParseExact(item.EFFECTIVE_DATE_START, Constants.DisplayFormats.DateFormat, Constants.DisplayFormats.DefaultCultureInfo);
                        if (item.EFFECTIVE_DATE_START_DT.ToDateDisplayText() != "01/01/0001")
                        {
                            item.EFFECTIVE_DATE_START = item.EFFECTIVE_DATE_START_DT.ToDateDisplayText();
                        }
                        else
                        {
                            item.EFFECTIVE_DATE_START = "";
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
        public ActionResult DeleteConfigurationLookupName([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(dataS))
                {
                    var AddDataModel = new JavaScriptSerializer().Deserialize<DeleteConfigLookup>(dataS);
                    var command = new DeleteConfigurationLookupCommand
                    {
                        LOOKUP_NAME = AddDataModel.LOOKUP_NAME,
                        USER = AddDataModel.USER

                    };
                    _deleteconfigurationLookupCommand.Handle(command);
                    if (command.return_code != null)
                    {
                        return Json(new
                        {
                            code = command.return_code.ToSafeString(),
                            msg = command.return_msg.ToSafeString()
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
        public JsonResult SelectListLookupName()
        {
            try
            {
                listGetListLookupNameModel = new List<GetListLookupNameModel>();
                listGetListLookupNameModel = _queryProcessor.Execute(new GetListLookupNameQuery());
                var data = listGetListLookupNameModel.ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult AddNewConfigurationLookupDetails([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                AddConfigurationLookupCommand addConfigurationLookupCommand = new AddConfigurationLookupCommand();
                var addLookupModel = new JavaScriptSerializer().Deserialize<LookupDataInsert>(dataS);
                List<LookupDataList> lookupDataList = new List<LookupDataList>();
                LookupDataList lookupData = new LookupDataList();
                List<lookupDataHeader> lookupDataHeadersList = new List<lookupDataHeader>();
                lookupDataHeader lookupDataHeader = new lookupDataHeader();
                for (int i = 0; i < addLookupModel.lookup_header_list.Count; i++)
                {
                    lookupData = new LookupDataList();
                    lookupDataHeadersList = new List<lookupDataHeader>();
                    lookupData.lookup_id = string.Empty;
                    lookupData.lookup_status = "new";
                    var detail = addLookupModel.lookup_header_list[i];
                    foreach (var value_detail in detail)
                    {
                        lookupDataHeader = new lookupDataHeader();
                        lookupDataHeader.parameter_name = value_detail.parameter_name;
                        lookupDataHeader.parameter_value = value_detail.parameter_value;
                        if (lookupDataHeader.parameter_name == "effective_date_start")
                        {
                            lookupDataHeader.parameter_value = value_detail.parameter_value;
                            lookupDataHeader.lookup_flag = value_detail.lookup_flag;
                        }
                        else if (lookupDataHeader.parameter_name == "effective_date_to")
                        {
                            if (!string.IsNullOrEmpty(value_detail.parameter_value))
                            {
                                lookupDataHeader.parameter_value = value_detail.parameter_value;
                                lookupDataHeader.lookup_flag = value_detail.lookup_flag;
                            }
                            else
                            {
                                lookupDataHeader.parameter_value = string.Empty;
                                lookupDataHeader.lookup_flag = value_detail.lookup_flag;
                            }

                        }
                        else
                        {
                            lookupDataHeader.parameter_value = value_detail.parameter_value;
                            lookupDataHeader.lookup_flag = value_detail.lookup_flag;
                        }

                        lookupDataHeadersList.Add(lookupDataHeader);

                    }
                    lookupData.lookup_header_list = lookupDataHeadersList;
                    lookupDataList.Add(lookupData);
                }

                addConfigurationLookupCommand.lookup_name = addLookupModel.lookup_name;
                addConfigurationLookupCommand.lookup_ontopflag = addLookupModel.lookup_ontopflag;
                addConfigurationLookupCommand.lookup_ontop = addLookupModel.lookup_ontop;
                addConfigurationLookupCommand.lookup_list = lookupDataList;
                addConfigurationLookupCommand.create_by = addLookupModel.user;
                addConfigurationLookupCommand.modify_by = addLookupModel.user;
                var jsonAdd = new JavaScriptSerializer().Serialize(addConfigurationLookupCommand);
                _addconfigurationLookupCommand.Handle(addConfigurationLookupCommand);

                if (addConfigurationLookupCommand.return_code == "0")
                {
                    return Json(new
                    {
                        code = "0",
                        msg = "ดำเนินการเพิ่มข้อมูล Look up เรียบร้อย"
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (addConfigurationLookupCommand.return_code == "1")
                {
                    return Json(new
                    {
                        code = "1",
                        msg = "ไม่สามารถทำรายการได้ เนื่องจากมีชื่อ Look up Name อยู่ก่อนแล้ว"
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
            catch (Exception ex)
            {
                string _msg = string.Empty;
                _msg = "เกิดข้อผิดพลาด ไม่สามารถทำรายการได้ กรุณาติดต่อ Admin";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetvalueDropDownLov(string FieldName)
        {
            try
            {
                List<DataDropDownLookup> dataDropDownLookupList = new List<DataDropDownLookup>();
                if (!string.IsNullOrEmpty(FieldName))
                {
                    if (FieldName == "v_symptom_group")
                    {
                        var config_name = FieldName.ToSafeString() == string.Empty || FieldName.ToSafeString() == null ? string.Empty : "SYMPTOM_GROUP";
                        var querySymtom_name = new GetConfigDropDownQuery
                        {
                            config_name = config_name,
                            symptom_group = string.Empty,
                            province_th = string.Empty,
                            district_th = string.Empty
                        };
                        var resultSymtom_name = _queryProcessor.Execute(querySymtom_name);
                        var Symtom_nameList = resultSymtom_name.Select(p => p.symptom_group).ToList();
                        foreach (var value in Symtom_nameList)
                        {
                            dataDropDownLookupList.Add(new DataDropDownLookup { LovValue1 = value });
                        }
                        return Json(dataDropDownLookupList, JsonRequestBehavior.AllowGet);

                    }
                    else if (FieldName == "p_Reject_reason")
                    {
                        var data = dataDropDownSymptomName.ToList();
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "v_province")
                    {
                        var config_name = FieldName.ToSafeString() == string.Empty || FieldName.ToSafeString() == null ? string.Empty : "PROVINCE";
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
                            dataDropDownLookupList.Add(new DataDropDownLookup { LovValue1 = value });
                        }
                        return Json(dataDropDownLookupList, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "v_district")
                    {
                        var data = dataDropDownDistrict.ToList();
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "v_subdistrict")
                    {
                        var data = dataDropDownSubDistrict.ToList();
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (FieldName == "v_fttr_flag")
                        {
                            var LovDataScreen = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
                            var fieldname = "FTTR Flag";
                            var query = new GetLovQuery
                            {
                                LovType = "DROPDOWNLIST",
                                LovName = fieldname
                            };

                            var lov = _queryProcessor.Execute(query);
                            var result = lov.OrderBy(p => p.OrderBy);
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else if (FieldName == "p_main_promo_code")
                        {
                            var main_package = new GetConfigDropDownQuery
                            {
                                config_name = "PACKAGE",
                                symptom_group = string.Empty,
                                province_th = string.Empty,
                                district_th = string.Empty
                            };
                            var resultMain_package = _queryProcessor.Execute(main_package);
                            var Main_packageList = resultMain_package.Select(p => p.package_name).ToList();
                            foreach (var value in Main_packageList)
                            {
                                dataDropDownLookupList.Add(new DataDropDownLookup { LovValue1 = value });
                            }
                            return Json(dataDropDownLookupList, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var LovDataScreen = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
                            var lov_name = LovDataScreen.Where(p => p.LovValue1 == FieldName).Select(o => o.Text).FirstOrDefault().ToSafeString();
                            var query = new GetLovQuery
                            {
                                LovType = "DROPDOWNLIST",
                                LovName = lov_name
                            };

                            var lov = _queryProcessor.Execute(query);
                            var result = lov.Where(p => p.ActiveFlag == "Y").OrderBy(p => p.OrderBy);
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getDefaultFirstRow()
        {
            try
            {
                var parameter_lov = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
                var strParameter = parameter_lov.Select(p => p.LovValue1).ToList();
                var addedDateTime = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                string stringJson = "";
                foreach (var val in strParameter)
                {
                    if (stringJson != "")
                    {
                        stringJson += ",";
                    }
                    if (val == "base_price")
                    {
                        stringJson += "\"" + val + "\":null";
                    }
                    else if (val == "effective_date_start")
                    {
                        stringJson += "\"" + val + "\":\"" + addedDateTime + "\"";
                    }
                    else if (val == "effective_date_to")
                    {
                        stringJson += "\"" + val + "\":null";
                    }
                    else
                    {
                        stringJson += "\"" + val + "\":\"Default\"";
                    }
                }
                string dataJson = "{" + "\"id\":1," + stringJson + "}";
                return Json(dataJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getDefaultFirstRow_Edit([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                var parameter_lov = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
                var strParameter = parameter_lov.Select(p => p.LovValue1).ToList();
                var addedDateTime = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                var rownum = Convert.ToInt32(dataS);
                var count_row = rownum + 1;
                string stringJson = "";
                foreach (var val in strParameter)
                {
                    if (stringJson != "")
                    {
                        stringJson += ",";
                    }
                    if (val == "base_price")
                    {
                        stringJson += "\"" + val + "\":null";
                    }
                    else if (val == "effective_date_start")
                    {
                        stringJson += "\"" + val + "\":\"" + addedDateTime + "\"";
                    }
                    else if (val == "effective_date_to")
                    {
                        stringJson += "\"" + val + "\":null";
                    }
                    else
                    {
                        stringJson += "\"" + val + "\":\"Default\"";
                    }
                }
                string dataJson = "{" + "\"id\":" + count_row + "," + stringJson + "}";
                return Json(dataJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSymptomName(string FieldData)
        {
            try
            {
                dataDropDownSymptomName = new List<SymptomNameDropDown>();
                var querySymtom_name = new GetConfigDropDownQuery
                {
                    config_name = "SYMPTOM_NAME",
                    symptom_group = FieldData,
                    province_th = string.Empty,
                    district_th = string.Empty
                };
                var resultSymtom_name = _queryProcessor.Execute(querySymtom_name);
                var Symtom_nameList = resultSymtom_name.Select(p => p.symptom_name).ToList();
                foreach (var value in Symtom_nameList)
                {
                    dataDropDownSymptomName.Add(new SymptomNameDropDown { LovValue1 = value });
                }
                return Json(dataDropDownSymptomName, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDistrict(string Province)
        {
            try
            {
                dataDropDownDistrict = new List<DistrictDropDown>();
                var queryDistrict = new GetConfigDropDownQuery
                {
                    config_name = "DISTRICT",
                    symptom_group = string.Empty,
                    province_th = Province,
                    district_th = string.Empty
                };
                var resultDistrict = _queryProcessor.Execute(queryDistrict);
                var DistrictList = resultDistrict.Select(p => p.district_th).ToList();
                foreach (var value in DistrictList)
                {
                    dataDropDownDistrict.Add(new DistrictDropDown { LovValue1 = value });
                }
                return Json(dataDropDownDistrict, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSubDistrict(string Province, string District)
        {
            try
            {
                dataDropDownSubDistrict = new List<SubDistrictDropDown>();
                var querySubDistrict = new GetConfigDropDownQuery
                {
                    config_name = "SUBDISTRICT",
                    symptom_group = string.Empty,
                    province_th = Province,
                    district_th = District
                };
                var resultSubDistrict = _queryProcessor.Execute(querySubDistrict);
                var SubDistrictList = resultSubDistrict.Select(p => p.sub_district_th).ToList();
                foreach (var value in SubDistrictList)
                {
                    dataDropDownSubDistrict.Add(new SubDistrictDropDown { LovValue1 = value });
                }
                return Json(dataDropDownSubDistrict, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SelectOntop()
        {
            try
            {
                var result_ontop_name = _queryProcessor.Execute(new GetDataOntopLookupQuery());

                return Json(result_ontop_name, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetEditLookupData(string lookupName)
        {
            try
            {
                int pageIndex = 1;
                int pageSize = 99999;
                var result_getdata = _queryProcessor.Execute(new GetDataLookupIDQuery()
                {
                    LOOKUP_NAME = lookupName.ToSafeString(),
                    PAGE_INDEX = pageIndex,
                    PAGE_SIZE = pageSize
                });
                Result_Lookup_Id_Curs = result_getdata[0].result_lookup_id_cur_data;
                if (result_getdata != null)
                {
                    //ใช้เช็คในขั้นตอน update lookup ว่ามี parameter_name ที่ตรงกันหรือเปล่า
                    param_name_lookup = result_getdata[0].return_param_name.Split(',')
                                         .Select(p => p.Trim())
                                         .ToList();
                    return Json(new
                    {
                        code = result_getdata[0].return_code,
                        msg = result_getdata[0].return_msg,
                        param_name = result_getdata[0].return_param_name,
                        ontop_flag = result_getdata[0].return_ontop_flag,
                        ontop_lookup = result_getdata[0].return_ontop_lookup,
                        lookup_cur = result_getdata[0].result_lookup_id_cur_data
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SelectListEditValue()
        {
            try
            {
                List<GridEditmakeupModel> GridEditmakeupModel = new List<GridEditmakeupModel>();
                if (Result_Lookup_Id_Curs.Count() > 0)
                {
                    for (int i = 0; i < Result_Lookup_Id_Curs.Count; i++)
                    {
                        GridEditmakeupModel.Add(new GridEditmakeupModel
                        {
                            id = i + 1,
                            LOOKUP_ID = Result_Lookup_Id_Curs[i].LOOKUP_ID.ToSafeString(),
                            base_price = Result_Lookup_Id_Curs[i].base_price.ToSafeString(),
                            effective_date_start = string.IsNullOrEmpty(Result_Lookup_Id_Curs[i].effective_date_start) ? null : Result_Lookup_Id_Curs[i].effective_date_start.ToSafeString(),
                            effective_date_to = string.IsNullOrEmpty(Result_Lookup_Id_Curs[i].effective_date_to) ? null : Result_Lookup_Id_Curs[i].effective_date_to.ToSafeString(),
                            p_ORDER_TYPE = Result_Lookup_Id_Curs[i].p_ORDER_TYPE.ToSafeString(),
                            p_PRODUCT_NAME = Result_Lookup_Id_Curs[i].p_PRODUCT_NAME.ToSafeString(),
                            p_Reject_reason = Result_Lookup_Id_Curs[i].p_Reject_reason.ToSafeString(),
                            p_product_owner = Result_Lookup_Id_Curs[i].p_product_owner.ToSafeString(),
                            v_same_day = Result_Lookup_Id_Curs[i].v_same_day.ToSafeString(),
                            p_event_flow_flag = Result_Lookup_Id_Curs[i].p_event_flow_flag.ToSafeString(),
                            p_request_sub_flag = Result_Lookup_Id_Curs[i].p_request_sub_flag.ToSafeString(),
                            v_province = Result_Lookup_Id_Curs[i].v_province.ToSafeString(),
                            v_district = Result_Lookup_Id_Curs[i].v_district.ToSafeString(),
                            v_subdistrict = Result_Lookup_Id_Curs[i].v_subdistrict.ToSafeString(),
                            p_addess_id = Result_Lookup_Id_Curs[i].p_addess_id.ToSafeString(),
                            v_fttr_flag = Result_Lookup_Id_Curs[i].v_fttr_flag.ToSafeString(),
                            p_subcontract_type = Result_Lookup_Id_Curs[i].p_subcontract_type.ToSafeString(),
                            p_subcontract_sub_type = Result_Lookup_Id_Curs[i].p_subcontract_sub_type.ToSafeString(),
                            v_region = Result_Lookup_Id_Curs[i].v_region.ToSafeString(),
                            p_org_id = Result_Lookup_Id_Curs[i].p_org_id.ToSafeString(),
                            p_SUBCONTRACT_CODE = Result_Lookup_Id_Curs[i].p_SUBCONTRACT_CODE.ToSafeString(),
                            p_SUBCONTRACT_NAME = Result_Lookup_Id_Curs[i].p_SUBCONTRACT_NAME.ToSafeString(),
                            v_reused_flag = Result_Lookup_Id_Curs[i].v_reused_flag.ToSafeString(),
                            distance_from = Result_Lookup_Id_Curs[i].distance_from.ToSafeString(),
                            distance_to = Result_Lookup_Id_Curs[i].distance_to.ToSafeString(),
                            v_subcontract_location = Result_Lookup_Id_Curs[i].v_subcontract_location.ToSafeString(),
                            indoor_cost = Result_Lookup_Id_Curs[i].indoor_cost.ToSafeString(),
                            outdoor_cost = Result_Lookup_Id_Curs[i].outdoor_cost.ToSafeString(),
                            v_over_cost_pm = Result_Lookup_Id_Curs[i].v_over_cost_pm.ToSafeString(),
                            v_max_distance = Result_Lookup_Id_Curs[i].v_max_distance.ToSafeString(),
                            v_symptom_group = Result_Lookup_Id_Curs[i].v_symptom_group.ToSafeString(),
                            v_same_subs = Result_Lookup_Id_Curs[i].v_same_subs.ToSafeString(),
                            v_same_team = Result_Lookup_Id_Curs[i].v_same_team.ToSafeString(),
                            p_main_promo_code = Result_Lookup_Id_Curs[i].p_main_promo_code.ToSafeString(),
                        });
                    }


                    // ค้นหา index ของโมเดลที่มีคำว่า "default"
                    var findDefault = GridEditmakeupModel;
                    List<int> numDefault = new List<int>();
                    for (int i = 0; i < findDefault.Count; i++)
                    {
                        var properties = findDefault[i].GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            var value = prop.GetValue(findDefault[i])?.ToString();
                            if (value != null && (value.Contains("Default") || value.Contains("default")))
                            {
                                numDefault.Add(i);
                                break; // หยุดการตรวจสอบเมื่อพบคำว่า "default"
                            }
                        }
                    }

                    for (int i = 0; i < numDefault.Count(); i++)
                    {
                        var num = numDefault[i];
                        for (int j = num; j < GridEditmakeupModel.Count(); j++)
                        {
                            if (j == num)
                            {
                                if (GridEditmakeupModel[i].p_ORDER_TYPE == "" || GridEditmakeupModel[i].p_ORDER_TYPE == null)
                                {
                                    GridEditmakeupModel[i].p_ORDER_TYPE = "Default";
                                }
                                if (GridEditmakeupModel[i].p_PRODUCT_NAME == "" || GridEditmakeupModel[i].p_PRODUCT_NAME == null)
                                {
                                    GridEditmakeupModel[i].p_PRODUCT_NAME = "Default";
                                }
                                if (GridEditmakeupModel[i].p_Reject_reason == "" || GridEditmakeupModel[i].p_Reject_reason == null)
                                {
                                    GridEditmakeupModel[i].p_Reject_reason = "Default";
                                }
                                if (GridEditmakeupModel[i].p_product_owner == "" || GridEditmakeupModel[i].p_product_owner == null)
                                {
                                    GridEditmakeupModel[i].p_product_owner = "Default";
                                }
                                if (GridEditmakeupModel[i].v_same_day == "" || GridEditmakeupModel[i].v_same_day == null)
                                {
                                    GridEditmakeupModel[i].v_same_day = "Default";
                                }
                                if (GridEditmakeupModel[i].p_event_flow_flag == "" || GridEditmakeupModel[i].p_event_flow_flag == null)
                                {
                                    GridEditmakeupModel[i].p_event_flow_flag = "Default";
                                }
                                if (GridEditmakeupModel[i].p_request_sub_flag == "" || GridEditmakeupModel[i].p_request_sub_flag == null)
                                {
                                    GridEditmakeupModel[i].p_request_sub_flag = "Default";
                                }
                                if (GridEditmakeupModel[i].v_province == "" || GridEditmakeupModel[i].v_province == null)
                                {
                                    GridEditmakeupModel[i].v_province = "Default";
                                }
                                if (GridEditmakeupModel[i].v_district == "" || GridEditmakeupModel[i].v_district == null)
                                {
                                    GridEditmakeupModel[i].v_district = "Default";
                                }
                                if (GridEditmakeupModel[i].v_subdistrict == "" || GridEditmakeupModel[i].v_subdistrict == null)
                                {
                                    GridEditmakeupModel[i].v_subdistrict = "Default";
                                }
                                if (GridEditmakeupModel[i].p_addess_id == "" || GridEditmakeupModel[i].p_addess_id == null)
                                {
                                    GridEditmakeupModel[i].p_addess_id = "Default";
                                }
                                if (GridEditmakeupModel[i].v_fttr_flag == "" || GridEditmakeupModel[i].v_fttr_flag == null)
                                {
                                    GridEditmakeupModel[i].v_fttr_flag = "Default";
                                }
                                if (GridEditmakeupModel[i].p_subcontract_type == "" || GridEditmakeupModel[i].p_subcontract_type == null)
                                {
                                    GridEditmakeupModel[i].p_subcontract_type = "Default";
                                }
                                if (GridEditmakeupModel[i].p_subcontract_sub_type == "" || GridEditmakeupModel[i].p_subcontract_sub_type == null)
                                {
                                    GridEditmakeupModel[i].p_subcontract_sub_type = "Default";
                                }
                                if (GridEditmakeupModel[i].v_region == "" || GridEditmakeupModel[i].v_region == null)
                                {
                                    GridEditmakeupModel[i].v_region = "Default";
                                }
                                if (GridEditmakeupModel[i].p_org_id == "" || GridEditmakeupModel[i].p_org_id == null)
                                {
                                    GridEditmakeupModel[i].p_org_id = "Default";
                                }
                                if (GridEditmakeupModel[i].p_SUBCONTRACT_CODE == "" || GridEditmakeupModel[i].p_SUBCONTRACT_CODE == null)
                                {
                                    GridEditmakeupModel[i].p_SUBCONTRACT_CODE = "Default";
                                }
                                if (GridEditmakeupModel[i].p_SUBCONTRACT_NAME == "" || GridEditmakeupModel[i].p_SUBCONTRACT_NAME == null)
                                {
                                    GridEditmakeupModel[i].p_SUBCONTRACT_NAME = "Default";
                                }
                                if (GridEditmakeupModel[i].v_reused_flag == "" || GridEditmakeupModel[i].v_reused_flag == null)
                                {
                                    GridEditmakeupModel[i].v_reused_flag = "Default";
                                }
                                if (GridEditmakeupModel[i].distance_from == "" || GridEditmakeupModel[i].distance_from == null)
                                {
                                    GridEditmakeupModel[i].distance_from = "Default";
                                }
                                if (GridEditmakeupModel[i].distance_to == "" || GridEditmakeupModel[i].distance_to == null)
                                {
                                    GridEditmakeupModel[i].distance_to = "Default";
                                }
                                if (GridEditmakeupModel[i].v_subcontract_location == "" || GridEditmakeupModel[i].v_subcontract_location == null)
                                {
                                    GridEditmakeupModel[i].v_subcontract_location = "Default";
                                }
                                if (GridEditmakeupModel[i].indoor_cost == "" || GridEditmakeupModel[i].indoor_cost == null)
                                {
                                    GridEditmakeupModel[i].indoor_cost = "Default";
                                }
                                if (GridEditmakeupModel[i].outdoor_cost == "" || GridEditmakeupModel[i].outdoor_cost == null)
                                {
                                    GridEditmakeupModel[i].outdoor_cost = "Default";
                                }
                                if (GridEditmakeupModel[i].v_over_cost_pm == "" || GridEditmakeupModel[i].v_over_cost_pm == null)
                                {
                                    GridEditmakeupModel[i].v_over_cost_pm = "Default";
                                }
                                if (GridEditmakeupModel[i].v_max_distance == "" || GridEditmakeupModel[i].v_max_distance == null)
                                {
                                    GridEditmakeupModel[i].v_max_distance = "Default";
                                }
                                if (GridEditmakeupModel[i].v_symptom_group == "" || GridEditmakeupModel[i].v_symptom_group == null)
                                {
                                    GridEditmakeupModel[i].v_symptom_group = "Default";
                                }
                                if (GridEditmakeupModel[i].v_same_subs == "" || GridEditmakeupModel[i].v_same_subs == null)
                                {
                                    GridEditmakeupModel[i].v_same_subs = "Default";
                                }
                                if (GridEditmakeupModel[i].v_same_team == "" || GridEditmakeupModel[i].v_same_team == null)
                                {
                                    GridEditmakeupModel[i].v_same_team = "Default";
                                }
                                if (GridEditmakeupModel[i].p_main_promo_code == "" || GridEditmakeupModel[i].p_main_promo_code == null)
                                {
                                    GridEditmakeupModel[i].p_main_promo_code = "Default";
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    string dataJson = JsonConvert.SerializeObject(GridEditmakeupModel);
                    return Json(dataJson, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public class GridEditmakeupModel
        {
            public int id { get; set; }
            public string LOOKUP_ID { get; set; }
            public string LOOKUP_NAME { get; set; }
            public string p_ORDER_TYPE { get; set; }
            public string p_PRODUCT_NAME { get; set; }
            public string p_Reject_reason { get; set; }
            public string p_product_owner { get; set; }
            public string v_same_day { get; set; }
            public string p_event_flow_flag { get; set; }
            public string p_request_sub_flag { get; set; }
            public string v_province { get; set; }
            public string v_district { get; set; }
            public string v_subdistrict { get; set; }
            public string p_addess_id { get; set; }
            public string v_fttr_flag { get; set; }
            public string p_subcontract_type { get; set; }
            public string p_subcontract_sub_type { get; set; }
            public string v_region { get; set; }
            public string p_org_id { get; set; }
            public string p_SUBCONTRACT_CODE { get; set; }
            public string p_SUBCONTRACT_NAME { get; set; }
            public string v_reused_flag { get; set; }
            public string distance_from { get; set; }
            public string distance_to { get; set; }
            public string v_subcontract_location { get; set; }
            public string indoor_cost { get; set; }
            public string outdoor_cost { get; set; }
            public string v_over_cost_pm { get; set; }
            public string v_max_distance { get; set; }
            public string base_price { get; set; }
            public string effective_date_start { get; set; }
            public string effective_date_to { get; set; }
            public string v_symptom_group { get; set; }
            public string v_same_subs { get; set; }
            public string v_same_team { get; set; }
            public string p_main_promo_code { get; set; }
        }

        public ActionResult UpdateConfigurationLookupDetails([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            try
            {
                UpdateConfigurationLookupCommand updateConfigurationLookupCommand = new UpdateConfigurationLookupCommand();
                var UpdateLookupModel = new JavaScriptSerializer().Deserialize<LookupDataUpdate>(dataS);

                List<LookupDataListUpdate> lookupDataList = new List<LookupDataListUpdate>();
                LookupDataListUpdate lookupData = new LookupDataListUpdate();
                List<lookupDataHeaderUpdate> lookupDataHeadersList = new List<lookupDataHeaderUpdate>();
                lookupDataHeaderUpdate lookupDataHeader = new lookupDataHeaderUpdate();

                // กรณีจำนวน row เท่ากัน(update row เดิม)
                if (Result_Lookup_Id_Curs.Count() == UpdateLookupModel.lookup_header_list.Count())
                {
                    //กรณีเช่น ข้อมูลที่ 2 row ลบ 1 row เพิ่ม 1 row ก็จะเป็น 2 เหมือนเดิมแต่ค่าที่ส่งไปจะเป็น modify 1,new 1,delete 1
                    //เก็บ list ของ lookup_name จากเบส
                    var lookup_id_from_base = Result_Lookup_Id_Curs.Select(p => p.LOOKUP_ID).ToList();

                    //เก็บ list ของ lookup_name จากหน้าจอ
                    List<string> lookup_id_from_display = new List<string>();
                    for (int i = 0; i < UpdateLookupModel.lookup_header_list.Count; i++)
                    {
                        var detail = UpdateLookupModel.lookup_header_list[i];
                        for (int j = 0; j < detail.Count(); j++)
                        {
                            if (detail[j].parameter_name == "LOOKUP_ID")
                            {
                                lookup_id_from_display.Add(detail[j].parameter_value);
                            }
                        }
                    }
                    //เช็คว่า lookup_id ไหน ไม่อยู่ในที่ return lookup_id จากเบส
                    var missing_lookup_id = lookup_id_from_base.Except(lookup_id_from_display).ToList();
                    if (missing_lookup_id.Count() > 0)
                    {
                        foreach (var value in missing_lookup_id)
                        {
                            lookupData = new LookupDataListUpdate();
                            lookupData.lookup_id = value;
                            lookupData.lookup_status = "delete";

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }
                    }



                    for (int i = 0; i < UpdateLookupModel.lookup_header_list.Count; i++)
                    {
                        lookupData = new LookupDataListUpdate();
                        lookupDataHeadersList = new List<lookupDataHeaderUpdate>();
                        var detail = UpdateLookupModel.lookup_header_list[i];
                        lookupData.lookup_id = getLookupIDUpdate(UpdateLookupModel.lookup_header_list[i]);
                        if (lookupData.lookup_id == "")
                        {
                            lookupData.lookup_status = "new";

                            var list_param_model = detail.Where(o => o.parameter_name != "LOOKUP_ID").Select(p => p.parameter_name).ToList();

                            for (int j = 0; j < detail.Count(); j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    lookupDataHeader = new lookupDataHeaderUpdate();
                                    lookupDataHeader.parameter_name = detail[j].parameter_name;

                                    if (lookupDataHeader.parameter_name == "effective_date_start")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "new";
                                    }
                                    else if (lookupDataHeader.parameter_name == "effective_date_to")
                                    {
                                        if (!string.IsNullOrEmpty(detail[j].parameter_value))
                                        {

                                            lookupDataHeader.parameter_value = detail[j].parameter_value;
                                            lookupDataHeader.lookup_flag = "new";
                                        }
                                        else
                                        {
                                            lookupDataHeader.parameter_value = string.Empty;
                                            lookupDataHeader.lookup_flag = "new";
                                        }

                                    }
                                    else if (lookupDataHeader.parameter_name == "base_price")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "new";
                                    }
                                    else
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "new";
                                    }

                                    lookupDataHeadersList.Add(lookupDataHeader);

                                }
                            }

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }
                        else
                        {
                            lookupData.lookup_status = "modify";

                            var list_param_model = detail.Where(o => o.parameter_name != "LOOKUP_ID").Select(p => p.parameter_name).ToList();

                            for (int j = 0; j < detail.Count(); j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    lookupDataHeader = new lookupDataHeaderUpdate();
                                    lookupDataHeader.parameter_name = detail[j].parameter_name;

                                    if (lookupDataHeader.parameter_name == "effective_date_start")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "modify";
                                    }
                                    else if (lookupDataHeader.parameter_name == "effective_date_to")
                                    {
                                        if (!string.IsNullOrEmpty(detail[j].parameter_value))
                                        {
                                            lookupDataHeader.parameter_value = detail[j].parameter_value;
                                            lookupDataHeader.lookup_flag = "modify";
                                        }
                                        else
                                        {
                                            lookupDataHeader.parameter_value = string.Empty;
                                            lookupDataHeader.lookup_flag = "modify";
                                        }

                                    }
                                    else if (lookupDataHeader.parameter_name == "base_price")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "modify";
                                    }
                                    else
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = detail[j].lookup_flag;
                                    }

                                    lookupDataHeadersList.Add(lookupDataHeader);

                                }
                            }

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }

                    }
                }
                // กรณีเพิ่ม row จากหน้าจอ
                else if (Result_Lookup_Id_Curs.Count() < UpdateLookupModel.lookup_header_list.Count())
                {
                    var lookup_id_from_base = Result_Lookup_Id_Curs.Select(p => p.LOOKUP_ID).ToList();

                    //เก็บ list ของ lookup_name จากหน้าจอ
                    List<string> lookup_id_from_display = new List<string>();
                    for (int i = 0; i < UpdateLookupModel.lookup_header_list.Count; i++)
                    {
                        var detail = UpdateLookupModel.lookup_header_list[i];
                        for (int j = 0; j < detail.Count(); j++)
                        {
                            if (detail[j].parameter_name == "LOOKUP_ID")
                            {
                                lookup_id_from_display.Add(detail[j].parameter_value);
                            }
                        }
                    }
                    //เช็คว่า lookup_id ไหน ไม่อยู่ในที่ return lookup_id จากเบส
                    var missing_lookup_id = lookup_id_from_base.Except(lookup_id_from_display).ToList();
                    if (missing_lookup_id.Count() > 0)
                    {
                        foreach (var value in missing_lookup_id)
                        {
                            lookupData = new LookupDataListUpdate();
                            lookupData.lookup_id = value;
                            lookupData.lookup_status = "delete";

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }
                    }


                    for (int i = 0; i < UpdateLookupModel.lookup_header_list.Count; i++)
                    {
                        lookupData = new LookupDataListUpdate();
                        lookupDataHeadersList = new List<lookupDataHeaderUpdate>();
                        var detail = UpdateLookupModel.lookup_header_list[i];
                        lookupData.lookup_id = getLookupIDUpdate(UpdateLookupModel.lookup_header_list[i]);
                        if (lookupData.lookup_id == "")
                        {
                            lookupData.lookup_status = "new";

                            var list_param_model = detail.Where(o => o.parameter_name != "LOOKUP_ID").Select(p => p.parameter_name).ToList();

                            for (int j = 0; j < detail.Count(); j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    lookupDataHeader = new lookupDataHeaderUpdate();
                                    lookupDataHeader.parameter_name = detail[j].parameter_name;

                                    if (lookupDataHeader.parameter_name == "effective_date_start")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "new";
                                    }
                                    else if (lookupDataHeader.parameter_name == "effective_date_to")
                                    {
                                        if (!string.IsNullOrEmpty(detail[j].parameter_value))
                                        {
                                            lookupDataHeader.parameter_value = detail[j].parameter_value;
                                            lookupDataHeader.lookup_flag = "new";
                                        }
                                        else
                                        {
                                            lookupDataHeader.parameter_value = string.Empty;
                                            lookupDataHeader.lookup_flag = "new";
                                        }

                                    }
                                    else if (lookupDataHeader.parameter_name == "base_price")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "new";
                                    }
                                    else
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = detail[j].lookup_flag == "delete" ? "delete" : "new";  // "new"; 
                                    }

                                    lookupDataHeadersList.Add(lookupDataHeader);

                                }
                            }

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }
                        else
                        {
                            lookupData.lookup_status = "modify"; //modified

                            var list_param_model = detail.Where(o => o.parameter_name != "LOOKUP_ID").Select(p => p.parameter_name).ToList();

                            for (int j = 0; j < detail.Count(); j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    lookupDataHeader = new lookupDataHeaderUpdate();
                                    lookupDataHeader.parameter_name = detail[j].parameter_name;

                                    if (lookupDataHeader.parameter_name == "effective_date_start")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "modify";
                                    }
                                    else if (lookupDataHeader.parameter_name == "effective_date_to")
                                    {
                                        if (!string.IsNullOrEmpty(detail[j].parameter_value))
                                        {
                                            lookupDataHeader.parameter_value = detail[j].parameter_value;
                                            lookupDataHeader.lookup_flag = "modify";
                                        }
                                        else
                                        {
                                            lookupDataHeader.parameter_value = string.Empty;
                                            lookupDataHeader.lookup_flag = "modify";
                                        }

                                    }
                                    else if (lookupDataHeader.parameter_name == "base_price")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "modify";
                                    }
                                    else
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = detail[j].lookup_flag;
                                    }

                                    lookupDataHeadersList.Add(lookupDataHeader);

                                }
                            }

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }

                    }
                }
                // กรณีลบ row จากหน้าจอ
                else if (Result_Lookup_Id_Curs.Count() > UpdateLookupModel.lookup_header_list.Count())
                {
                    var lookup_id_from_base = Result_Lookup_Id_Curs.Select(p => p.LOOKUP_ID).ToList();

                    //เก็บ list ของ lookup_name จากหน้าจอ
                    List<string> lookup_id_from_display = new List<string>();
                    for (int i = 0; i < UpdateLookupModel.lookup_header_list.Count; i++)
                    {
                        var detail = UpdateLookupModel.lookup_header_list[i];
                        for (int j = 0; j < detail.Count(); j++)
                        {
                            if (detail[j].parameter_name == "LOOKUP_ID")
                            {
                                lookup_id_from_display.Add(detail[j].parameter_value);
                            }
                        }
                    }
                    //เช็คว่า lookup_id ไหน ไม่อยู่ในที่ return lookup_id จากเบส
                    var missing_lookup_id = lookup_id_from_base.Except(lookup_id_from_display).ToList();
                    if (missing_lookup_id.Count() > 0)
                    {
                        foreach (var value in missing_lookup_id)
                        {
                            lookupData = new LookupDataListUpdate();
                            lookupData.lookup_id = value;
                            lookupData.lookup_status = "delete";

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }
                    }


                    for (int i = 0; i < UpdateLookupModel.lookup_header_list.Count; i++)
                    {
                        lookupData = new LookupDataListUpdate();
                        lookupDataHeadersList = new List<lookupDataHeaderUpdate>();
                        var detail = UpdateLookupModel.lookup_header_list[i];
                        lookupData.lookup_id = getLookupIDUpdate(UpdateLookupModel.lookup_header_list[i]);
                        if (lookupData.lookup_id == "")
                        {
                            lookupData.lookup_status = "new";

                            var list_param_model = detail.Where(o => o.parameter_name != "LOOKUP_ID").Select(p => p.parameter_name).ToList();

                            for (int j = 0; j < detail.Count(); j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    lookupDataHeader = new lookupDataHeaderUpdate();
                                    lookupDataHeader.parameter_name = detail[j].parameter_name;

                                    if (lookupDataHeader.parameter_name == "effective_date_start")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "new";
                                    }
                                    else if (lookupDataHeader.parameter_name == "effective_date_to")
                                    {
                                        if (!string.IsNullOrEmpty(detail[j].parameter_value))
                                        {
                                            lookupDataHeader.parameter_value = detail[j].parameter_value;
                                            lookupDataHeader.lookup_flag = "new";
                                        }
                                        else
                                        {
                                            lookupDataHeader.parameter_value = string.Empty;
                                            lookupDataHeader.lookup_flag = "new";
                                        }

                                    }
                                    else if (lookupDataHeader.parameter_name == "base_price")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "new";
                                    }
                                    else
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = detail[j].lookup_flag == "delete" ? "delete" : "new";
                                    }

                                    lookupDataHeadersList.Add(lookupDataHeader);

                                }
                            }

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }
                        else
                        {
                            lookupData.lookup_status = "modify";

                            var list_param_model = detail.Where(o => o.parameter_name != "LOOKUP_ID").Select(p => p.parameter_name).ToList();

                            for (int j = 0; j < detail.Count(); j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    lookupDataHeader = new lookupDataHeaderUpdate();
                                    lookupDataHeader.parameter_name = detail[j].parameter_name;

                                    if (lookupDataHeader.parameter_name == "effective_date_start")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "modify";
                                    }
                                    else if (lookupDataHeader.parameter_name == "effective_date_to")
                                    {

                                        if (!string.IsNullOrEmpty(detail[j].parameter_value))
                                        {

                                            lookupDataHeader.parameter_value = detail[j].parameter_value;
                                            lookupDataHeader.lookup_flag = "modify";
                                        }
                                        else
                                        {
                                            lookupDataHeader.parameter_value = string.Empty;
                                            lookupDataHeader.lookup_flag = "modify";
                                        }

                                    }
                                    else if (lookupDataHeader.parameter_name == "base_price")
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = "modify";
                                    }
                                    else
                                    {
                                        lookupDataHeader.parameter_value = detail[j].parameter_value;
                                        lookupDataHeader.lookup_flag = detail[j].lookup_flag;
                                    }

                                    lookupDataHeadersList.Add(lookupDataHeader);

                                }
                            }

                            lookupData.lookup_header_list = lookupDataHeadersList;
                            lookupDataList.Add(lookupData);
                        }


                    }

                }

                updateConfigurationLookupCommand.lookup_name = UpdateLookupModel.lookup_name;
                updateConfigurationLookupCommand.lookup_ontopflag = UpdateLookupModel.lookup_ontopflag;
                updateConfigurationLookupCommand.lookup_ontop = UpdateLookupModel.lookup_ontop;
                updateConfigurationLookupCommand.lookup_list = lookupDataList;
                updateConfigurationLookupCommand.create_by = UpdateLookupModel.user;
                updateConfigurationLookupCommand.modify_by = UpdateLookupModel.user;




                var jsonUpdate = new JavaScriptSerializer().Serialize(updateConfigurationLookupCommand);
                _updateconfigurationLookupCommand.Handle(updateConfigurationLookupCommand);

                if (updateConfigurationLookupCommand.return_code == "0")
                {
                    return Json(new
                    {
                        code = updateConfigurationLookupCommand.return_code,
                        msg = updateConfigurationLookupCommand.return_msg
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        code = updateConfigurationLookupCommand.return_code,
                        msg = updateConfigurationLookupCommand.return_msg
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
        private string getLookupIDUpdate(List<LookupHeaderListUpdate> data)
        {
            string result = string.Empty;
            try
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    if (data[i].parameter_name == "LOOKUP_ID")
                    {
                        result = string.IsNullOrEmpty(data[i].parameter_value) ? "" : data[i].parameter_value.ToSafeString();
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string checkLookupFlag(string param_name, List<string> param_name_cur, List<string> list_param_model)
        {
            string result = string.Empty;
            try
            {
                var filter_param_name_cur = param_name_cur.Where(p => p != "base_price" && p != "effective_date_start" && p != "effective_date_to").ToList();
                int param_name_equal = 0;

                if (param_name_cur.Count() == list_param_model.Count())
                {
                    foreach (var val in filter_param_name_cur)
                    {
                        if (param_name == val)
                        {
                            param_name_equal += 1;
                        }
                    }
                    if (param_name_equal > 0)
                    {
                        result = "modify";
                    }

                }
                else if (param_name_cur.Count() < list_param_model.Count())
                {
                    foreach (var val in filter_param_name_cur)
                    {
                        if (param_name == val)
                        {
                            param_name_equal += 1;
                        }
                        else
                        {
                            param_name_equal = -1;
                        }
                    }
                    if (param_name_equal > 0)
                    {
                        result = "modify";
                    }
                    else if (param_name_equal == -1)
                    {
                        result = "new";
                    }

                }
                else if (param_name_cur.Count() > list_param_model.Count())
                {
                    foreach (var val in filter_param_name_cur)
                    {
                        if (param_name == val)
                        {
                            param_name_equal += 1;
                        }
                        else
                        {
                            param_name_equal = -1;
                        }
                    }
                    if (param_name_equal > 0)
                    {
                        result = "modify";
                    }
                    else if (param_name_equal == -1)
                    {
                        result = "delete";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public JsonResult getListParamNameFromBase()
        {
            try
            {
                var data = param_name_lookup.ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportDataToExcel(string lookupName)
        {
            string filename = string.Empty;
            try
            {

                var result_getdata = _queryProcessor.Execute(new GetDataLookupIDQuery()
                {
                    LOOKUP_NAME = lookupName.ToSafeString(),
                    PAGE_INDEX = 1,
                    PAGE_SIZE = 99999
                });

                var Lovcolumn = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
                var param_name = result_getdata[0].return_param_name.ToSafeString();
                var split_param_name = param_name.Split(',');
                List<string> columnList = new List<string>();
                foreach (var value in split_param_name)
                {
                    var column = Lovcolumn.Where(p => p.LovValue1 == value).Select(p => p.Text).FirstOrDefault().ToSafeString();
                    columnList.Add(column);
                }

                filename = "Configuration_Lookup_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xlsx";
                string tempPath = System.IO.Path.GetTempPath();

                if (System.IO.File.Exists($"{tempPath}\\{filename}"))
                { System.IO.File.Delete($"{tempPath}\\{filename}"); }

                string finalFileNameWithPath = string.Empty;

                finalFileNameWithPath = string.Format("{0}\\{1}", tempPath, filename);

                if (System.IO.File.Exists(finalFileNameWithPath))
                { System.IO.File.Delete(finalFileNameWithPath); }
                var newFile = new FileInfo(finalFileNameWithPath);

                LookupIdModel lookupIdModel = new LookupIdModel();
                List<LookupIdModel> lookupIdModelList = new List<LookupIdModel>();
                foreach (var item in result_getdata[0].result_lookup_id_cur_data)
                {
                    lookupIdModel = new LookupIdModel();
                    lookupIdModel.LOOKUP_ID = item.LOOKUP_ID.ToSafeString();
                    lookupIdModel.LOOKUP_NAME = item.LOOKUP_NAME.ToSafeString();
                    lookupIdModel.p_ORDER_TYPE = item.p_ORDER_TYPE.ToSafeString();
                    lookupIdModel.p_PRODUCT_NAME = item.p_PRODUCT_NAME.ToSafeString();
                    lookupIdModel.p_Reject_reason = item.p_Reject_reason.ToSafeString();
                    lookupIdModel.p_product_owner = item.p_product_owner.ToSafeString();
                    lookupIdModel.v_same_day = item.v_same_day.ToSafeString();
                    lookupIdModel.p_event_flow_flag = item.p_event_flow_flag.ToSafeString();
                    lookupIdModel.p_request_sub_flag = item.p_request_sub_flag.ToSafeString();
                    lookupIdModel.v_province = item.v_province.ToSafeString();
                    lookupIdModel.v_district = item.v_district.ToSafeString();
                    lookupIdModel.v_subdistrict = item.v_subdistrict.ToSafeString();
                    lookupIdModel.p_addess_id = item.p_addess_id.ToSafeString();
                    lookupIdModel.v_fttr_flag = item.v_fttr_flag.ToSafeString();
                    lookupIdModel.p_subcontract_type = item.p_subcontract_type.ToSafeString();
                    lookupIdModel.p_subcontract_sub_type = item.p_subcontract_sub_type.ToSafeString();
                    lookupIdModel.v_region = item.v_region.ToSafeString();
                    lookupIdModel.p_org_id = item.p_org_id.ToSafeString();
                    lookupIdModel.p_SUBCONTRACT_CODE = item.p_SUBCONTRACT_CODE.ToSafeString();
                    lookupIdModel.p_SUBCONTRACT_NAME = item.p_SUBCONTRACT_NAME.ToSafeString();
                    lookupIdModel.v_reused_flag = item.v_reused_flag.ToSafeString();
                    lookupIdModel.distance_from = item.distance_from.ToSafeString();
                    lookupIdModel.distance_to = item.distance_to.ToSafeString();
                    lookupIdModel.v_subcontract_location = item.v_subcontract_location.ToSafeString();
                    lookupIdModel.indoor_cost = item.indoor_cost.ToSafeString();
                    lookupIdModel.outdoor_cost = item.outdoor_cost.ToSafeString();
                    lookupIdModel.v_over_cost_pm = item.v_over_cost_pm.ToSafeString();
                    lookupIdModel.v_max_distance = item.v_max_distance.ToSafeString();
                    lookupIdModel.base_price = item.base_price.ToSafeString();
                    lookupIdModel.effective_date_start = item.effective_date_start.ToSafeString();
                    lookupIdModel.effective_date_to = item.effective_date_to.ToSafeString();
                    lookupIdModel.v_symptom_group = item.v_symptom_group.ToSafeString();
                    lookupIdModel.v_same_subs = item.v_same_subs.ToSafeString();
                    lookupIdModel.v_same_team = item.v_same_team.ToSafeString();
                    lookupIdModel.p_main_promo_code = item.p_main_promo_code.ToSafeString();
                    lookupIdModel.L_Ontop_Lookup = result_getdata[0].return_ontop_lookup == "null" || result_getdata[0].return_ontop_lookup == "" || result_getdata[0].return_ontop_lookup == null ? "-" : result_getdata[0].return_ontop_lookup.ToSafeString();
                    lookupIdModelList.Add(lookupIdModel);
                }

                var bytes = GenerateEntitytoExcelLookup(lookupIdModelList, filename);
                return File(bytes, "application/excel", filename);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public byte[] GenerateEntitytoExcelLookup<T>(List<T> data, string fileName)
        {
            try
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
                DataTable table = new DataTable();
                var Lovcolumn = GetLovListByLovName("SCREEN", "payg_lookup_parameter");
                for (int i = 0; i < props.Count; i++)
                {
                    string col;
                    PropertyDescriptor prop = props[i];
                    if (prop.Name == "LOOKUP_ID")
                    {
                        col = "LOOKUP_ID";
                    }
                    else if (prop.Name == "LOOKUP_NAME")
                    {
                        col = "LOOKUP_NAME";
                    }
                    else if (prop.Name == "L_Ontop_Lookup")
                    {
                        var get_col_ontop = SelectFbbCfgLov_searchLookup("LOOKUP_NAME_UPSERT");
                        col = get_col_ontop.Where(p => p.LOV_NAME == prop.Name).Select(o => o.LOV_VAL1).FirstOrDefault().ToSafeString();
                    }
                    else
                    {
                        col = Lovcolumn.Where(p => p.LovValue1 == prop.Name).Select(p => p.Text).FirstOrDefault().ToSafeString();
                    }

                    table.Columns.Add(col);
                }
                object[] values = new object[props.Count];
                foreach (T item in data)
                {

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);

                    }

                    table.Rows.Add(values);

                }
                // ลบคอลัมน์ที่ว่าง
                var columnsToRemove = table.Columns.Cast<DataColumn>()
                .Where(col => table.AsEnumerable().All(row => row.IsNull(col) || string.IsNullOrWhiteSpace(row[col].ToString())))
                .ToList();

                // ลบคอลัมน์ที่ว่าง
                foreach (var column in columnsToRemove)
                {
                    table.Columns.Remove(column);
                }
                string tempPath = Path.GetTempPath();
                var data_ = GenerateExcel(table, "ConfigurationLookup", tempPath, fileName);
                return data_;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private byte[] GenerateExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName)
        {
            //  _Logger.Info("GenerateSaleTrackingExcel start");
            string tempFilePath = Path.Combine(directoryPath, fileName + ".xlsx");

            //Delete existing file with same file name.
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }

            var newFile = new FileInfo(tempFilePath);

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = dataToExcel.Columns.Count;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);
                iRow = 1;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                ExcelRange rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Font.Bold = true;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();


                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(tempFilePath);
                return data;
            }
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
    }
}
