using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class ConfigurationSAPOnlineController : FBBConfigController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CostInstallationCommand> _COMMAND_CostInstallationCommand;

        private string rptName = "Report Name : {0}";
        private string rptName_1 = "Report Name : {0}";
        private string rptName_2 = "Report Name : {0}";
        private string rptDate = "Run Report Date/Time : {0}";

        #endregion

        #region Constructor

        public ConfigurationSAPOnlineController(ILogger logger,
            ICommandHandler<CostInstallationCommand> COMMAND_CostInstallationCommand,
            IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _COMMAND_CostInstallationCommand = COMMAND_CostInstallationCommand;
            _queryProcessor = queryProcessor;
        }

        #endregion

        #region ActionResult

        // Update 17.10 By Jirawadee.p 2017-09-26
        public ActionResult Index()
        {
            this.ConfigurationSAPOnline();
            return View();
        }

        // Create By auth
        public ActionResult ConfigurationSAPOnline()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBB_COST_INSTALL_SCREEN");

            return null;
        }

        public ActionResult CostInstallationAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (request.Sorts.Count == 0) // order by UPDATED_DATE desc
            {
                Kendo.Mvc.SortDescriptor _sort = new Kendo.Mvc.SortDescriptor();
                _sort.Member = "UPDATED_DATE";
                _sort.SortDirection = ListSortDirection.Descending;
                request.Sorts.Add(_sort);
            }

            if (!string.IsNullOrEmpty(dataS))
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<CostInstallation>(dataS);
                var result = this.GetCostInstallation(searchEventModel);

                return Json(result.ToDataSourceResult(request));
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region JsonResult

        // Set Lov DDL OrderType
        public JsonResult SetDDLOrderType(string typeMode)
        {
            var LovData = new List<FbssConfigTBL>();
            LovData = Get_FBSS_CONFIG_TBL_LOV(typeMode, "FBB_COST_INSTALL_ORDTYPE", "");
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string typeMode, string _CON_TYPE, string _VAL4)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                VAL4 = _VAL4
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            if (_VAL4.Length == 0) // except Pattern and Vender/Building
            {
                _FbssConfig.ForEach(u => { u.VAL1 = u.VAL1.ToSafeString().ToUpper(); u.DISPLAY_VAL = u.DISPLAY_VAL.ToSafeString().ToUpper(); }); // Upper data.
                if (typeMode == "Search")
                {
                    _FbssConfig.Insert(0, new FbssConfigTBL { DISPLAY_VAL = "ALL", VAL1 = "" });
                }
                else
                {
                    _FbssConfig.Insert(0, new FbssConfigTBL { DISPLAY_VAL = "-- Please Select --", VAL1 = "PLEASE_SELECT" });
                }
            }
            else
            {
                _FbssConfig.ForEach(u => { u.VAL1 = u.VAL1.ToSafeString().ToUpper(); u.DISPLAY_VAL = u.DISPLAY_VAL.ToSafeString().ToUpper(); }); // Upper data.
            }
            return _FbssConfig;
        }

        // Set Lov DDL Service
        public JsonResult SetDDLService(string typeMode, string orderType)
        {
            var LovData = new List<FbssConfigTBL>();
            LovData = Get_FBSS_CONFIG_TBL_LOV(typeMode, "FBB_COST_INSTALL_SERVICE", "");
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set Lov DDL Customer
        public JsonResult SetDDLCustomer(string typeMode, string orderType, string service)
        {
            var LovData = new List<FbssConfigTBL>();
            LovData = Get_FBSS_CONFIG_TBL_LOV(typeMode, "FBB_COST_INSTALL_CUSTOMER", "");
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set Lov DDL Name
        public JsonResult SetDDLName(string typeMode, string orderType, string service, string customer)
        {
            var _customer = (customer == null) ? "" : customer.Trim().ToUpper();
            _customer = _customer.Replace(" ", "");
            var LovDataList = new List<FbssConfigTBL>();
            // Bind Dropdownlist
            if (_customer == "PROJECT" || _customer == "SALETARGET")
            {
                var LovBuildingList = new List<FbssConfigTBL>();
                try
                {
                    var query = new GetBuildingCostInstallationQuery() { SERVICE_TYPE = "XDSL", TYPE = "Building" };
                    var LovBuildingData = _queryProcessor.Execute(query);

                    LovBuildingList = LovBuildingData.DistinctBy(b => b.LOV_NAME).OrderBy(b => b.LOV_VAL1)
                                        .Select(b =>
                                        {
                                            return new FbssConfigTBL { DISPLAY_VAL = b.LOV_NAME, VAL1 = b.LOV_VAL1 };
                                        }).ToList();
                    LovBuildingList.ForEach(u => { u.VAL1 = u.VAL1.ToSafeString().ToUpper(); u.DISPLAY_VAL = u.DISPLAY_VAL.ToSafeString().ToUpper(); }); // Upper data.
                }
                catch (Exception ex) { _Logger.Info(ex.GetErrorMessage()); }
                LovDataList.AddRange(LovBuildingList);
            }
            else if (_customer == "DEFAULT")
            {
                LovDataList.AddRange(Get_FBSS_CONFIG_TBL_LOV(typeMode, "FBB_COST_INSTALL_CUSTOMER_NAME", "Default"));
            }
            else if (_customer == "PHASE2")
            {
                LovDataList.AddRange(Get_FBSS_CONFIG_TBL_LOV(typeMode, "FBB_COST_INSTALL_CUSTOMER_NAME", "Phase2"));
            }
            else if (_customer == "PHASE3")
            {
                LovDataList.AddRange(Get_FBSS_CONFIG_TBL_LOV(typeMode, "FBB_COST_INSTALL_CUSTOMER_NAME", "Phase3"));
            }

            if (typeMode == "Search") LovDataList.Insert(0, new FbssConfigTBL { DISPLAY_VAL = "ALL", VAL1 = "" });
            else LovDataList.Insert(0, new FbssConfigTBL { DISPLAY_VAL = "-- Please Select --", VAL1 = "PLEASE_SELECT" });

            return Json(LovDataList, JsonRequestBehavior.AllowGet);
        }

        public List<LovModel> Get_FBB_COST_INSTALL_LOV(string typeMode, string Input_1, string Input_2)
        {
            Input_2 = (Input_2 == null) ? Input_2 = "" : Input_2.Trim().ToUpper();
            var LovData = new List<LovModel>();
            LovData = base.LovData.Where(p => p.Type == Input_1 && p.LovValue4.ToUpper() == Input_2).OrderBy(p => p.Name)
                                        .Select(p =>
                                        {
                                            return new LovModel { LOV_NAME = p.Name, LOV_VAL1 = p.LovValue1 };
                                        }).ToList();
            return LovData;
        }

        public JsonResult Update_config_tbl(ActionType type, string dataS)
        {
            if (!string.IsNullOrEmpty(dataS))
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<FbssConfigTBL>(dataS);
                var result = this.Update_config_tbl_Step2(type, searchEventModel);

                return Json(result);
            }
            else
            {
                return null;
            }
        }

        public ReturnCreate Update_config_tbl_Step2(ActionType action, FbssConfigTBL model)
        {
            try
            {
                var query = new CreateFbssConfigTBLQuery()
                {
                    ACTION = action,
                    CON_ID = model.CON_ID,
                    CON_TYPE = "FBB_COST_INSTALL_CUSTOMER_NAME",
                    DISPLAY_VAL = model.DISPLAY_VAL.Trim().ToUpper(), // upper step save and insert.
                    VAL2 = "Y",
                    VAL4 = "DEFAULT",
                    CREATED_BY = base.CurrentUser.UserName,
                    CREATED_DATE = DateTime.Now,
                    UPDATED_BY = base.CurrentUser.UserName,
                    UPDATED_DATE = DateTime.Now
                };
                ReturnCreate _ReturnCreate = _queryProcessor.Execute(query);

                return _ReturnCreate;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new ReturnCreate() { RETURN_CODE = -1, RETURN_MSG = ex.GetErrorMessage() };
            }
        }

        // Update 17.10 By Jirawadee.p 2017-09-26
        public JsonResult CreateNewInstallCost(ActionType type, string dataS)
        {
            if (!string.IsNullOrEmpty(dataS))
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<CostInstallation>(dataS);
                bool Is_DealerLastmile = false;
                bool Is_SaleTarget = false;
                bool Is_DefaultAverage = false;

                //-------------------- Is_DefaultAverage
                if (searchEventModel.CUSTOMER.Replace(" ", "").ToUpper() == "DEFAULT")
                {
                    Is_DefaultAverage = true;
                }

                if (searchEventModel.ORDER_TYPE.ToUpper() == "NEW")
                {
                    if (searchEventModel.SERVICE.ToUpper() == "FTTH")
                    {
                        if (searchEventModel.CUSTOMER.ToUpper() == "PHASE3" || searchEventModel.CUSTOMER.ToUpper() == "PHASE3/4")
                        {
                            if (searchEventModel.CUSTOMER_NAME.ToUpper() == "DEALER-LASTMILE" || searchEventModel.CUSTOMER_NAME.ToUpper() == "LASTMILE DISTANCE" || searchEventModel.CUSTOMER_NAME.ToUpper() == "LASTMILE")
                            {
                                Is_DealerLastmile = true;
                            }
                        }
                        else if (searchEventModel.CUSTOMER.ToUpper() == "PHASE4" || searchEventModel.CUSTOMER.ToUpper() == "PHASE3/4")
                        {
                            if (searchEventModel.CUSTOMER_NAME.ToUpper() == "DEALER-LASTMILE" || searchEventModel.CUSTOMER_NAME.ToUpper() == "LASTMILE DISTANCE" || searchEventModel.CUSTOMER_NAME.ToUpper() == "LASTMILE")
                            {
                                Is_DealerLastmile = true;
                            }
                        }
                    }
                }

                //--------------------
                if (
                    searchEventModel.ORDER_TYPE.ToSafeString().ToUpper() == "NEW"
                    && searchEventModel.SERVICE.ToSafeString().ToUpper() == "DORM"
                    && searchEventModel.CUSTOMER.ToSafeString().Replace(" ", "").ToUpper() == "SALETARGET"
                    )
                {
                    Is_SaleTarget = true;
                }
                //--------------------
                if (Is_DealerLastmile == true)
                {

                    searchEventModel.PLAYBOX_RATE = null;
                    searchEventModel.VOIP_RATE = null;
                    searchEventModel.TOTAL_PRICE = ((searchEventModel.IN_DOOR_PRICE) == null ? 0 : searchEventModel.IN_DOOR_PRICE) + ((searchEventModel.OUT_DOOR_PRICE) == null ? 0 : searchEventModel.OUT_DOOR_PRICE);
                    if (searchEventModel.INTERNET_RATE == 0) searchEventModel.INTERNET_RATE = null;
                    //string V_DATETIME = "V" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    //searchEventModel.CUSTOMER_NAME = V_DATETIME;
                }
                else if (Is_SaleTarget == true)
                {
                    searchEventModel.OUT_DOOR_PRICE = null;
                    searchEventModel.IN_DOOR_PRICE = null;
                }
                else if (Is_DefaultAverage == true)
                {
                    searchEventModel.CUSTOMER = "Default";
                    //searchEventModel.CUSTOMER_NAME = "Default";
                    searchEventModel.PLAYBOX_RATE = null;
                    searchEventModel.VOIP_RATE = null;
                    searchEventModel.LENGTH_FR = null;
                    searchEventModel.LENGTH_TO = null;
                    searchEventModel.OUT_DOOR_PRICE = null;
                    searchEventModel.IN_DOOR_PRICE = null;
                }
                else
                {
                    searchEventModel.LENGTH_FR = null;
                    searchEventModel.LENGTH_TO = null;
                    searchEventModel.OUT_DOOR_PRICE = null;
                    searchEventModel.IN_DOOR_PRICE = null;
                }
                //--------------------
                if (searchEventModel.REMARK != null)
                {
                    if (searchEventModel.REMARK.Trim() != "")
                    {
                        searchEventModel.CUSTOMER = searchEventModel.CUSTOMER + "_S"; // plug and play;
                    }
                }

                //--------------------
                if (searchEventModel.CUSTOMER.Replace(" ", "").ToUpper() == "SALETARGET"
                    || searchEventModel.CUSTOMER.Replace(" ", "").ToUpper() == "PROJECT")
                {
                    searchEventModel.ADDRESS_ID = searchEventModel.CUSTOMER_NAME;
                }
                else
                {
                    searchEventModel.ADDRESS_ID = null;
                }

                var result = this.UpdateCostInstallation(searchEventModel, type);

                return Json(result);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region ReturnCreate

        public ReturnCreate UpdateCostInstallation(CostInstallation model, ActionType action)
        {
            try
            {
                var query = new CreateCostInstallationQuery()
                {
                    ACTION = action,
                    ID = model.ID,
                    SERVICE = model.SERVICE.ToString().ToUpper(), // upper
                    INS_OPTION = model.CUSTOMER.ToString().ToUpper(), // upper
                    VENDOR = model.CUSTOMER_NAME.ToString().ToUpper(), // upper
                    ORDER_TYPE = model.ORDER_TYPE.ToString().ToUpper(), // upper
                    RATE = model.INTERNET_RATE,
                    PLAYBOX = model.PLAYBOX_RATE,
                    VOIP = model.VOIP_RATE,
                    EFFECTIVE_DATE = model.EFFECTIVE_DATE,
                    EXPIRE_DATE = model.EXPIRE_DATE,
                    LENGTH_FR = model.LENGTH_FR,
                    LENGTH_TO = model.LENGTH_TO,
                    OUT_DOOR_PRICE = model.OUT_DOOR_PRICE,
                    IN_DOOR_PRICE = model.IN_DOOR_PRICE,
                    REMARK = model.REMARK,
                    ADDRESS_ID = model.ADDRESS_ID,
                    TOTAL_PRICE = model.TOTAL_PRICE,
                    IS_DELETE_FIXASSCONFIG = model.IS_DELETE_FIXASSCONFIG
                };
                string _action = action.ToSafeString();
                if (_action.ToUpper() == "INSERT")
                {
                    query.CREATE_DATE = DateTime.Now;
                    query.CREATE_BY = base.CurrentUser.UserName;
                    query.UPDATED_DATE = DateTime.Now;
                    query.UPDATED_BY = base.CurrentUser.UserName;
                }
                else // update
                {
                    query.CREATE_DATE = model.CREATE_DATE;
                    query.CREATE_BY = model.CREATE_BY;
                    query.UPDATED_DATE = DateTime.Now;
                    query.UPDATED_BY = base.CurrentUser.UserName;
                }
                ReturnCreate cost = _queryProcessor.Execute(query);

                return cost;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new ReturnCreate() { RETURN_CODE = -1, RETURN_MSG = ex.GetErrorMessage() };
            }
        }

        #endregion

        #region Method

        // Create By auth
        private void SetViewBagLov(string screenType)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        // Update 17.10 By Jirawadee.p 2017-09-26
        public List<CostInstallation> GetCostInstallation(CostInstallation model)
        {
            if (model.CUSTOMER.Equals("B"))
            {
                model.CUSTOMER = "BUILDING";
                //model.CUSTOMER = "Project";
            }
            else if (model.CUSTOMER.Equals("V"))
            {
                model.CUSTOMER = "VENDOR";
                //model.CUSTOMER = "Sub";
            }

            if (string.IsNullOrEmpty(model.CUSTOMER_NAME))
            {
                model.CUSTOMER_NAME = "";
            }
            else
            {
                // Set Name : 2191=NSN , 2270=SRC , 0000=Dealer
                var queryVendor = new GetVendorCostInstallationQuery() { VENDER_MODE = "" };
                var LovVendorData = _queryProcessor.Execute(queryVendor);
                try
                {
                    var resultvender = LovVendorData.Where(v => v.LOV_VAL1 == model.CUSTOMER_NAME)
                                              .Select(v => { return v.LOV_NAME; }).First().ToSafeString();
                    model.CUSTOMER_NAME = string.IsNullOrEmpty(model.CUSTOMER_NAME) ? model.CUSTOMER_NAME : resultvender;
                }
                catch
                {
                    // นอกเหนือจาก Vendor
                }
            }

            try
            {
                var query = new GetCostInstallationQuery()
                {
                    SERVICE = model.SERVICE,
                    INS_OPTION = model.CUSTOMER,
                    VENDOR = model.CUSTOMER_NAME,
                    ORDER_TYPE = model.ORDER_TYPE

                };
                //if (model.CUSTOMER_NAME != "") if (model.CUSTOMER_NAME.ToUpper() == "LASTMILE") query.VENDOR = "";
                var cost = _queryProcessor.Execute(query);
                List<CostInstallation> cost_filter = new List<CostInstallation>();
                cost_filter = cost.ToList();
                //if (model.CUSTOMER_NAME.ToUpper() == "LASTMILE") // search filter
                //{
                //    cost_filter = cost.Where(f => "V" == f.CUSTOMER_NAME.ToUpper().Substring(0,1)).ToList();
                //}
                //else
                //{
                //    cost_filter = cost.ToList();
                //}


                List<CostInstallation> tmp = new List<CostInstallation>();
                var _ADDRESS_ID = new List<string>();
                //var _cost_ADDRESS_ID = cost.DistinctBy(b => b.ADDRESS_ID);

                var _cost_ADDRESS_ID = cost.Where(v => v.ADDRESS_ID != null).DistinctBy(b => b.ADDRESS_ID).DistinctBy(b => b.ADDRESS_ID).OrderBy(b => b.ADDRESS_ID)
                                        .Select(b =>
                                        {
                                            return new CostInstallation { ADDRESS_ID = b.ADDRESS_ID };
                                        }).ToList();

                foreach (var cost_ADDRESS_ID in _cost_ADDRESS_ID)
                {
                    _ADDRESS_ID.Add(cost_ADDRESS_ID.ADDRESS_ID);
                }

                List<LovModel> LovBuildingData = new List<LovModel>();
                if (_ADDRESS_ID.Count > 0)
                {
                    var queryBuilding = new GetBuildingCostInstallationQuery() { SERVICE_TYPE = "XDSL", TYPE = "Building", ADDRESS_ID = _ADDRESS_ID };
                    LovBuildingData = _queryProcessor.Execute(queryBuilding);
                }


                foreach (var costresult in cost_filter)
                {
                    string _CUSTOMER = costresult.CUSTOMER;
                    _CUSTOMER = (_CUSTOMER == null) ? "" : _CUSTOMER.Trim().Replace("_S", "").ToUpper();
                    _CUSTOMER = _CUSTOMER.Replace(" ", "");
                    if (_CUSTOMER == "PROJECT" || _CUSTOMER == "SALETARGET")
                    {
                        var LovBuildingList = new List<LovModel>();
                        try
                        {
                            var resultvender = LovBuildingData.Where(v => v.LOV_VAL1 == costresult.CUSTOMER_NAME)
                                                  .Select(v => { return v.LOV_NAME; }).First().ToSafeString();
                            costresult.TMP_CODE_CUSTOMER_NAME = resultvender;
                        }
                        catch (Exception ex) { _Logger.Info(ex.GetErrorMessage()); }
                    }
                    else
                    {
                        costresult.TMP_CODE_CUSTOMER_NAME = costresult.CUSTOMER_NAME;
                    }
                    costresult.CUSTOMER = costresult.CUSTOMER.Replace("_S", "");
                    //if (costresult.CUSTOMER_NAME.ToUpper().Substring(0,1) == "V")
                    //{
                    //    costresult.CUSTOMER_NAME = "Lastmile";
                    //    costresult.TMP_CODE_CUSTOMER_NAME = "Lastmile";
                    //}

                    // 
                    if (string.IsNullOrEmpty(costresult.LENGTH_FR.ToSafeString()) == false)
                    {
                        string aa = String.Format("{0:0.00}", costresult.LENGTH_FR.ToSafeString());
                        string _TMP_LENGTH_FR_TO = String.Format("{0:0.00}", costresult.LENGTH_FR) + " - " + String.Format("{0:0.00}", costresult.LENGTH_TO);
                        costresult.TMP_LENGTH_FR_TO = _TMP_LENGTH_FR_TO;
                    }
                    else { costresult.TMP_LENGTH_FR_TO = ""; }
                }
                cost_filter.ForEach(u =>
                {
                    u.ORDER_TYPE = u.ORDER_TYPE.ToSafeString().ToUpper();  // Upper data.
                    u.SERVICE = u.SERVICE.ToSafeString().ToUpper(); // Upper data.
                    u.CUSTOMER = u.CUSTOMER.ToSafeString().ToUpper(); // Upper data.
                    u.CUSTOMER_NAME = u.CUSTOMER_NAME.ToSafeString().ToUpper(); // Upper data.
                    u.TMP_CODE_CUSTOMER_NAME = u.TMP_CODE_CUSTOMER_NAME.ToSafeString().ToUpper(); // Upper data.
                });

                return cost_filter;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<CostInstallation>();
            }
        }

        #endregion

        #region Export Excel

        // [ExportExcel]
        // dataS_tab1
        public ActionResult Export_Subcontractor_CostInstallation(string dataS_tab1, string dataS_tab2)
        {
            //rptName = string.Format(rptName, "CostInstallation and Subcontractor Report");
            rptName_1 = string.Format(rptName_1, "Sub Contractor Report");
            rptName_2 = string.Format(rptName_2, "Cost Installation Report");

            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetCostInstallationExcelName("CostInstallation and Subcontractor Report");

            //var bytes = GenerateGetCostInstallationEntitytoExcel<CostInstallationExportList>(result1, filename, "");
            var bytes = GenerateGetCostInstallationEntitytoExcel_2(dataS_tab1, dataS_tab2, filename, "");

            return File(bytes, "application/excel", filename + ".xls");
        }
        public ActionResult ExportCostInstallationData(string dataS)
        {
            var costInstallationModel = new JavaScriptSerializer().Deserialize<CostInstallation>(dataS);

            var query = GetCostInstallation(costInstallationModel);

            var result = ConvertCostInstallationModel(query);

            rptName = string.Format(rptName, "Configuration : Cost Installation - SAP Online Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetCostInstallationExcelName("Configuration - Cost Installation - SAP Online Report");


            var bytes = GenerateGetCostInstallationEntitytoExcel<CostInstallationExportList>(result, filename, "");

            return File(bytes, "application/excel", filename + ".xls");
        }

        // [ExportExcel] Convert CostInstallation To CostInstallationExportList
        private List<CostInstallationExportList> ConvertCostInstallationModel(List<CostInstallation> list)
        {
            var dataexportlist = list.OrderByDescending(o => o.UPDATED_DATE).Select(x => new CostInstallationExportList()
            {
                ORDER_TYPE = x.ORDER_TYPE.ToSafeString(),
                SERVICE = x.SERVICE.ToSafeString(),
                CUSTOMER = x.CUSTOMER.ToSafeString(),
                TYPE = x.TMP_CODE_CUSTOMER_NAME.ToSafeString(),
                TMP_LENGTH_FR_TO = x.TMP_LENGTH_FR_TO.ToSafeString(),
                INTERNET_RATE = x.INTERNET_RATE.ToSafeString(),
                PLAYBOX_RATE = x.PLAYBOX_RATE.ToSafeString(),
                VOIP_RATE = x.VOIP_RATE.ToSafeString(),
                EFFECTIVE_DATE = x.EFFECTIVE_DATE.ToString("dd/MM/yyyy"),
                EXPIRE_DATE = x.EXPIRE_DATE == null ? "" : x.EXPIRE_DATE.Value.ToString("dd/MM/yyyy"),
                REMARK = x.REMARK.ToSafeString()
            });

            return dataexportlist.ToList();
        }

        // [ExportExcel] Get Excel File Name
        private string GetCostInstallationExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateGetCostInstallationEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateGetCostInstallationEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.Type == "FBB_COST_INSTALL_SCREEN" && p.Name.StartsWith("E_")).OrderBy(p => p.OrderBy).ToList();

            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue1.ToSafeString(), System.Type.GetType("System.String"));
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, System.Type.GetType("System.String"));
                }
            }

            table = ConvertToDataTable(data);
            //table.Columns["ORDER_TYPE"].SetOrdinal(0);
            //table.Columns["TMP_LENGTH_FR_TO"].SetOrdinal(3);

            for (int i = 0; i < table.Columns.Count; i++)
            {
                table.Columns[i].ColumnName = lovDataScreen[i].LovValue1.ToSafeString();
            }

            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateCostInstallationExcel(table, "WorkSheet", tempPath, fileName, "");
            return data_;
        }

        public byte[] GenerateGetCostInstallationEntitytoExcel_2(string dataS_tab1, string dataS_tab2, string fileName, string LovValue5)
        {
            // SupContractor
            var SupContractorReportModel = new JavaScriptSerializer().Deserialize<SupContractorReportList>(dataS_tab1);
            var query1 = new SupContractorReportQuery
            {
                ORG_ID = SupContractorReportModel.ORG_ID.ToString(),
                SUB_CONTRACTOR_NAME_TH = SupContractorReportModel.SUB_CONTRACTOR_NAME_TH,
                STORAGE_LOCATION = SupContractorReportModel.STORAGE_LOCATION,
                PHASE = SupContractorReportModel.PHASE,
                REQUEST_BY = base.CurrentUser.UserName.ToSafeString()
            };
            var result1 = _queryProcessor.Execute(query1);

            // Cost
            var costInstallationModel = new JavaScriptSerializer().Deserialize<CostInstallation>(dataS_tab2);
            var query2 = GetCostInstallation(costInstallationModel);
            var result2 = ConvertCostInstallationModel(query2);

            _Logger.Info("GenerateGetCostInstallationEntitytoExcel start");
            DataTable dt_1 = ToDataTable(result1, fileName, LovValue5, "FBB_MASTERCONFIG_PHASE");
            DataTable dt_2 = ToDataTable(result2, fileName, LovValue5, "FBB_COST_INSTALL_SCREEN");

            string tempPath = System.IO.Path.GetTempPath();
            var data_ = GenerateCostInstallationExcel_Sub(dt_1, dt_2, "Sub Contractor", "Cost Installation", tempPath, fileName, "");
            return data_;
        }

        public DataTable ToDataTable<T>(List<T> data, string fileName, string LovValue5, string lovType)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.Type == lovType && p.Name.StartsWith("E_")).OrderBy(p => p.OrderBy).ToList();

            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue1.ToSafeString(), System.Type.GetType("System.String"));
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, System.Type.GetType("System.String"));
                }
            }

            table = ConvertToDataTable(data);
            //if (lovType == "FBB_COST_INSTALL_SCREEN")
            //{
            //    table.Columns["ORDER_TYPE"].SetOrdinal(0);
            //    table.Columns["TMP_LENGTH_FR_TO"].SetOrdinal(3);
            //}

            for (int i = 0; i < table.Columns.Count; i++)
            {
                table.Columns[i].ColumnName = lovDataScreen[i].LovValue1.ToSafeString();
            }
            return table;
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }



        // [ExportExcel] Generate Excel
        private byte[] GenerateCostInstallationExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateCostInstallationExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow = 2;
            int iHeaderRow = 0;
            string strRow = iRow.ToSafeString();
            string strHeader = iHeaderRow.ToSafeString();
            string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            string strColumn2 = string.Empty;
            int iCol = 11;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells["A3,I3"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 5;
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
        // hot work style.
        private byte[] GenerateCostInstallationExcel_Sub(DataTable dataToExcel_1, DataTable dataToExcel_2, string excelSheetName_1, string excelSheetName_2, string directoryPath, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateCostInstallationExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow = 2;
            int iHeaderRow = 0;
            string strRow = iRow.ToSafeString();
            string strHeader = iHeaderRow.ToSafeString();
            string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            string strColumn2 = string.Empty;
            int iCol = 11;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet_1 = package.Workbook.Worksheets.Add(excelSheetName_1);
                ExcelWorksheet worksheet_2 = package.Workbook.Worksheets.Add(excelSheetName_2);

                worksheet_1.Cells["A2:G2"].Merge = true;
                worksheet_1.Cells["A2,G2"].LoadFromText(rptName_1);
                worksheet_1.Cells["A3:I3"].Merge = true;
                worksheet_1.Cells["A3,I3"].LoadFromText(rptDate);
                rangeReportDetail = worksheet_1.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                worksheet_2.Cells["A2:G2"].Merge = true;
                worksheet_2.Cells["A2,G2"].LoadFromText(rptName_2);
                worksheet_2.Cells["A3:I3"].Merge = true;
                worksheet_2.Cells["A3,I3"].LoadFromText(rptDate);
                rangeReportDetail = worksheet_2.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 5;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                rangeHeader = worksheet_1.SelectedRange[iRow, 1, iRow, 8];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rangeHeader = worksheet_2.SelectedRange[iRow, 1, iRow, 11];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet_1.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                worksheet_2.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet_1.Cells[strColumn1].LoadFromDataTable(dataToExcel_1, true, TableStyles.None);
                worksheet_2.Cells[strColumn1].LoadFromDataTable(dataToExcel_2, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName_1;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }
        }

        #endregion
    }
}
