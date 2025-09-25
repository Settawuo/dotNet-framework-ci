using FBBConfig.Extensions;
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
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.Account;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public partial class ConfigurationCostInstallationController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<ConfigurationInstallCostCommand> _configurationInstallCostCommand;
        private readonly ICommandHandler<ConfigutationTypeCommand> _configurationTypeCommand;
        //
        private readonly ICommandHandler<SendMailLastMileNotificationCommand> _sendMail;
        // GET: /ConfigurationCostInstallation/
        public ConfigurationCostInstallationController(
              ILogger logger
            , ICommandHandler<SendMailLastMileNotificationCommand> sendMail
            , IQueryProcessor queryProcessor
            , ICommandHandler<ConfigutationTypeCommand> configurationTypeCommand
            , ICommandHandler<ConfigurationInstallCostCommand> configurationInstallCostCommand)

        {
            _sendMail = sendMail;
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _configurationTypeCommand = configurationTypeCommand;
            _configurationInstallCostCommand = configurationInstallCostCommand;
        }
        public UserModel CurrentUser
        {
            get { return (UserModel)Session[WebConstants.FBBConfigSessionKeys.User]; }
            set { Session[WebConstants.FBBConfigSessionKeys.User] = value; }
        }

        static List<LovModel> ListFbbCfgLov = null;
        static List<ListSubcontractModel> listSubcontractModels = null;
        static List<ListSubCompanyNameModel> listSubCompanyNameModels = null;

        public ActionResult Index()
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            ViewBag.User = CurrentUser;

            ListFbbCfgLov = new List<LovModel>();
            listSubcontractModels = new List<ListSubcontractModel>();
            listSubCompanyNameModels = new List<ListSubCompanyNameModel>();

            getListData(); //Load Data


            return View();
        }

        private void getListData()
        {
            //-------------------Start : Add Listdata -----------------------
            //-------------------Date : 17/12/2020 ----------------------
            if (ListFbbCfgLov == null || ListFbbCfgLov.Count == 0)
                ListFbbCfgLov = SelectFbbCfgLovCost("CONFIG_COST");
            if (listSubcontractModels == null || listSubcontractModels.Count == 0)
                listSubcontractModels = _queryProcessor.Execute(new GetListSubContractQuery());
            if (listSubCompanyNameModels == null || listSubCompanyNameModels.Count == 0)
                listSubCompanyNameModels = _queryProcessor.Execute(new GetListSubCompanyNameQuery());

            //--------------------End Add Listdata ----------------------------
        }
        private List<LovModel> SelectFbbCfgLovINS(string lov_type, string lov_val5)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = lov_type,
                LOV_VAL5 = lov_val5
            };
            return _queryProcessor.Execute(query);
        }
        private List<LovModel> SelectFbbCfgLov(string lov_type)
        {
            return ListFbbCfgLov.Where(l => l.LOV_TYPE.Equals(lov_type))
                .Select(s => new LovModel
                {
                    LOV_NAME = s.LOV_NAME,
                    DISPLAY_VAL = s.DISPLAY_VAL,
                    LOV_VAL1 = s.LOV_VAL1,
                    LOV_VAL2 = s.LOV_VAL2,
                    LOV_VAL3 = s.LOV_VAL3,
                    LOV_VAL4 = s.LOV_VAL4
                }).ToList();

        }
        private List<LovModel> SelectFbbCfgLovCost(string lov_val5)
        {
            var query = new SelectLovVal5Query
            {
                LOV_VAL5 = lov_val5
            };
            return _queryProcessor.Execute(query);
        }
        public JsonResult GetSUBCONTRACTTYPE()
        {
            var data = SelectFbbCfgLov("SUBCONTRACT_TYPE")
             .Where(d => d.LOV_VAL1 != null).OrderBy(d => d.LOV_VAL1).ToList();

            data.ForEach(z =>
            {
                z.LOV_VAL1 = z.LOV_VAL1.ToUpper();
                z.DISPLAY_VAL = z.DISPLAY_VAL.ToUpper();
            });
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetSUBCONTRACTSUBTYPE()
        {
            var data = SelectFbbCfgLov("SUBCONTRACT_SUB_TYPE")
               .Where(d => d.LOV_VAL1 != null).OrderBy(d => d.LOV_VAL1).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);


        }
        public JsonResult SelectListTable()
        {
            var data = SelectFbbCfgLov("CONFIG_COST_INS")
               .Where(d => d.LOV_VAL1 != null).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectListTechnology()
        {
            var data = SelectFbbCfgLov("TECH_TYPE")
      .Where(d => d.LOV_VAL1 != null).OrderBy(d => d.LOV_VAL1).ToList();

            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectListOrderType()
        {
            var data = SelectFbbCfgLov("ORD_TYPE")
               .Where(d => d.LOV_VAL1 != null).OrderBy(d => d.LOV_VAL1).ToList();

            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSUBCONTRACTLOCATION()
        {
            var data = listSubcontractModels.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCOMPANYNAME()
        {
            var data = listSubCompanyNameModels.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataPopup(string dataS = "")
        {
            var AddDataModel = new JavaScriptSerializer().Deserialize<CostInstallationData>(dataS);
            var query = new GetDataPopupQuery
            {
                p_search_column = AddDataModel.SUBCONTRACT_LOCATION,
                p_value = AddDataModel.COMPANY_NAME
            };
            var data = _queryProcessor.Execute(query);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectListSAMEDAY()
        {
            var data = SelectFbbCfgLov("SAME_DAY")
               .Where(d => d.LOV_VAL1 != null).ToList();

            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectListEVENTCODE()
        {
            var data = SelectFbbCfgLov("EVT_CODE").ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectListROOMFLAG()
        {
            var data = SelectFbbCfgLov("ROOM_FLAG")
               .Where(d => d.LOV_VAL1 != null).ToList();

            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectListREUSEFLAG()
        {
            var data = SelectFbbCfgLov("REUSE_FLAG")
               .Where(d => d.LOV_VAL1 != null).ToList();

            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT ALL", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        int MaxLength = 0;
        public int GetMaxLengthLine()
        {
            var data = SelectFbbCfgLovINS("CONFIG_INS", "FIXED_LASTMILE")
              .FirstOrDefault(d => d.LOV_NAME == "Z4");
            MaxLength = Convert.ToInt32(data.DISPLAY_VAL);

            return MaxLength;
        }
        public JsonResult SelectVendorCode(string text)
        {
            if (text != null || text != "")
            {
                var query = new SelectSubContractorNameQuery
                {
                    p_code = "",
                    p_name = "",
                    p_code_distinct = true
                };
                var data = _queryProcessor.Execute(query);
                data = data.Where(p => p.SUB_CONTRACTOR_CODE.Contains(text)).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetRuleId(string table)
        {
            table = table == "" ? "T1" : table;
            List<RuleLastMileByDistanceModel> Data = _queryProcessor.Execute(new GetRuleLastMileByDistanceQuery { p_table_name = table });
            //List<RuleLastMileByDistanceModel> Data = _queryProcessor.Execute(new GetRuleLastMileByDistanceQuery());
            //if (table == "T1" || table == "" || table == null)
            //{
            //    Data = Data.Where(p => p.ruleid.Contains("R1")).OrderBy(p => p.ruleid).ToList();
            //}
            //if (table == "T2")
            //{
            //    Data = Data.Where(p => p.ruleid.Contains("R2")).OrderBy(p => p.ruleid).ToList();
            //}
            //if (table == "T3")
            //{
            //    Data = Data.Where(p => p.ruleid.Contains("R3")).OrderBy(p => p.ruleid).ToList();
            //}
            //if (table == "T4")
            //{
            //    Data = Data.Where(p => p.ruleid.Contains("R4")).OrderBy(p => p.ruleid).ToList();
            //}
            //if (table == "T5")
            //{
            //    Data = Data.Where(p => p.ruleid.Contains("D")).OrderBy(p => p.ruleid).ToList();
            //}
            //if (table == "T6")
            //{
            //    Data = Data.Where(p => p.ruleid.Contains("E")).OrderBy(p => p.ruleid).ToList();
            //}
            Data.Insert(0, new RuleLastMileByDistanceModel { rule_name = "SELECT", ruleid = "ALL" });
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddNewConfigData(string DataName, string DataType)
        {
            string MSG = string.Empty; string code = string.Empty;

            var addresult = new ConfigutationTypeCommand
            {
                p_name = DataName,
                p_type = DataType,
                p_username = CurrentUser.UserName
            };
            _configurationTypeCommand.Handle(addresult);
            if (addresult.ret_code != null)
            {
                code = addresult.ret_code.ToSafeString();
                MSG = addresult.ret_msg.ToSafeString();

            }
            return Json(new
            {
                code = code,
                msg = MSG
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddNewCostInstallationDataTable([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            string SUBCONTSUBTYPE = string.Empty;
            int _dis = 0;
            float lengt = 0;
            ////string msg = "";
            ////string code = string.Empty;
            float chkLength = 0;
            try
            {
                if (!string.IsNullOrEmpty(dataS))
                {
                    var AddDataModel = new JavaScriptSerializer().Deserialize<CostInstallationData>(dataS);

                    SUBCONTSUBTYPE = AddDataModel.SUBCONTRACT_SUB_TYPE == null ? "" : string.Join(",", AddDataModel.SUBCONTRACT_SUB_TYPE);
                    if (AddDataModel.TABLE == "T4")
                    {
                        if (AddDataModel.DISTANCE_TO == null || AddDataModel.DISTANCE_TO == "")
                        {
                            lengt = 0;
                        }
                        else
                        {
                            lengt = float.Parse(AddDataModel.DISTANCE_TO);
                        }

                    }
                    else
                    {
                        lengt = 0;
                    }

                    _dis = GetMaxLengthLine();
                    chkLength = float.Parse(_dis.ToSafeString());
                    if (lengt > chkLength)
                    {

                        return Json(new { code = "-1", msg = "Maximun Length is  Invaild" }, JsonRequestBehavior.AllowGet);
                    }

                    else
                    {
                        var addresult = new ConfigurationInstallCostCommand
                        {
                            P_COMMAND = AddDataModel.COMMAND,
                            P_TABLE = AddDataModel.TABLE,
                            P_RULE_ID = AddDataModel.RULD_ID,
                            P_RULE_NAME = AddDataModel.RULE_NAME,
                            P_ORDER_TYPE = AddDataModel.ORDER_TYPE,
                            P_SUBCONTRACT_TYPE = AddDataModel.SUBCONTRACT_TYPE,
                            P_SUBCONTRACT_SUB_TYPE = SUBCONTSUBTYPE,
                            P_VENDOR_CODE = AddDataModel.VENDOR_CODE,
                            P_TECHNOLOGY = AddDataModel.TECHNOLOGY,
                            P_TOTAL_PRICE = AddDataModel.TOTAL_PRICE,
                            P_EVENT_CODE = AddDataModel.EVENT_CODE,
                            P_ROOM_FLAG = AddDataModel.ROOM_FLAG,
                            P_REUSE_FLAG = AddDataModel.REUSE_FLAG,
                            P_DISTANCE_FROM = AddDataModel.DISTANCE_FROM,
                            P_DISTANCE_TO = AddDataModel.DISTANCE_TO,
                            P_INDOOR_PRICE = AddDataModel.INDOOR_PRICE,
                            P_OUTDOOR_PRICE = AddDataModel.OUTDOOR_PRICE,
                            P_INTERNET_PRICE = AddDataModel.INTERNET_PRICE,
                            P_VOIP_PRICE = AddDataModel.VOIP_PRICE,
                            P_PLAYBOX_PRICE = AddDataModel.PLAYBOX_PRICE,
                            P_MECH_PRICE = AddDataModel.MECH_PRICE,
                            P_ADDRESS_ID = AddDataModel.ADDRESS_ID,
                            P_EVENT_TYPE = AddDataModel.EVENT_TYPE,
                            P_EFFECTIVE_DATE = AddDataModel.EFFECTIVE_DATE,
                            P_EXPIRE_DATE = AddDataModel.EXPIRE_DATE,
                            P_SAME_DAY = AddDataModel.SAME_DAY,
                            P_USERNAME = CurrentUser.UserName,
                            P_COMPANY_NAME = AddDataModel.COMPANY_NAME,
                            P_SUB_LOCATION = AddDataModel.SUBCONTRACT_LOCATION,

                        };
                        _configurationInstallCostCommand.Handle(addresult);
                        string emailmsg;


                        if (addresult.ret_code != null)
                        {
                            string MSG = addresult.ret_msg.ToSafeString();
                            if (AddDataModel.COMMAND == "A")
                            {
                                AddDataModel.RULD_ID = MSG.Substring(18);
                            }
                            if (addresult.ret_code.ToSafeString() == "0")
                            {
                                emailmsg = SendEmail(AddDataModel);

                                MSG = MSG + emailmsg;

                            }

                            return Json(new
                            {
                                code = addresult.ret_code.ToSafeString(),
                                msg = MSG
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {


                            return Json(new
                            {
                                code = "-1",
                                msg = "Cannot Update Please Contact System Admin"
                            }, JsonRequestBehavior.AllowGet);
                        }


                    }
                }

                else
                {
                    return Json(new
                    {
                        code = "-1",
                        msg = "Data isnot correct, Please Check Data or Contact System Admin"
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch
            {
                string _msg = string.Empty;
                _msg = "Please Contact System Admin";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);

            }


        }
        public string EmailTemplate(CostInstallationData Datas)
        {

            try
            {
                var data = SelectFbbCfgLov("CONFIG_INS_EMAIL").ToList();
                var dsubjdtl = data.FirstOrDefault(d => d.LOV_NAME == "SUBJECTDTL");

                string FullNAMETH = CurrentUser.UserFullNameInThai.ToSafeString();
                string SubjectDTL = dsubjdtl.LOV_VAL1.ToSafeString();
                string SubjectDTL2 = dsubjdtl.LOV_VAL2.ToSafeString();
                string SubjectDTL3 = dsubjdtl.LOV_VAL3.ToSafeString();
                string SubjectDTL4 = dsubjdtl.LOV_VAL4.ToSafeString();
                StringBuilder tempBody = new StringBuilder();
                ////CultureInfo ThaiCulture = new CultureInfo("th-TH");
                ////CultureInfo UsaCulture = new CultureInfo("en-US");
                string subsubtype = Datas.SUBCONTRACT_SUB_TYPE == null ? "" : string.Join(",", Datas.SUBCONTRACT_SUB_TYPE);




                string TabelName = string.Empty;

                if (Datas.TABLE == "T1") { TabelName = "Table1"; }
                if (Datas.TABLE == "T2") { TabelName = "Table2"; }
                if (Datas.TABLE == "T3") { TabelName = "Table3"; }
                if (Datas.TABLE == "T4") { TabelName = "Table4"; }
                if (Datas.TABLE == "T5") { TabelName = "Table5"; }
                if (Datas.TABLE == "T6") { TabelName = "Table6"; }
                #region tempBody

                tempBody.Append("<p style='font-weight:bold;'></span>&nbsp;" + SubjectDTL);
                tempBody.Append("<br/>");
                //// tempBody.Append("</span>");
                tempBody.Append("<br/>");

                tempBody.Append(SubjectDTL2);
                tempBody.Append("<br/><br/><br/>");


                if (Datas.TABLE == "T1")
                {
                    ////    tempBody.Append("<span><table border='1px thin #ddd' width='100%' cellpadding='0' cellspacing='0'>");


                    tempBody.Append("<table>");
                    tempBody.Append("<tr><td colspan ='2'>Table:&nbsp;" + TabelName + "</td></tr>");
                    tempBody.Append("<tr><td>RULE ID:&nbsp;</td><td>" + Datas.RULD_ID + "</td></tr>");
                    tempBody.Append("<tr><td>RULE NAME:&nbsp;</td><td>" + Datas.RULE_NAME.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ORDER TYPE:&nbsp;</td><td>" + Datas.ORDER_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SUBCONTRACT TYPE:&nbsp;</td><td>" + Datas.SUBCONTRACT_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SUBCONTRACT SUB TYPE:&nbsp;</td><td>" + subsubtype + "</td></tr>");
                    tempBody.Append("<tr><td>VENDOR CODE:&nbsp;</td><td>" + Datas.VENDOR_CODE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TECHNOLOGY:&nbsp;</td><td>" + Datas.TECHNOLOGY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TOTAL PRICE:&nbsp;</td><td>" + Datas.TOTAL_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EFFECTIVE DATE:&nbsp;</td><td>" + Datas.EFFECTIVE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EXPIRE DATE:&nbsp;</td><td>" + Datas.EXPIRE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("</table>");
                }
                else if (Datas.TABLE == "T2")
                {

                    tempBody.Append("<table>");
                    tempBody.Append("<tr><td colspan ='2'>Table:&nbsp;" + TabelName + "</td></tr>");
                    tempBody.Append("<tr><td>RULE ID:&nbsp;</td><td>" + Datas.RULD_ID + "</td></tr>");
                    tempBody.Append("<tr><td>RULE NAME:&nbsp;</td><td>" + Datas.RULE_NAME.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ORDER TYPE:&nbsp;</td><td>" + Datas.ORDER_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SUBCONTRACT TYPE:&nbsp;</td><td>" + Datas.SUBCONTRACT_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TECHNOLOGY:&nbsp;</td><td>" + Datas.TECHNOLOGY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>INTERNET PRICE:&nbsp;</td><td>" + Datas.INTERNET_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>VOIP PRICE:&nbsp;</td><td>" + Datas.VOIP_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>PLAYBOX PRICE:&nbsp;</td><td>" + Datas.PLAYBOX_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SAME DAY:&nbsp;</td><td>" + Datas.SAME_DAY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ADDRESS ID:&nbsp;</td><td>" + Datas.ADDRESS_ID.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>MESH PRICE:&nbsp;</td><td>" + Datas.MECH_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EVENT TYPE:&nbsp;</td><td>" + Datas.EVENT_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EFFECTIVE DATE:&nbsp;</td><td>" + Datas.EFFECTIVE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EXPIRE DATE:&nbsp;</td><td>" + Datas.EXPIRE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("</table>");
                }
                else if (Datas.TABLE == "T3")
                {

                    tempBody.Append("<table>");
                    tempBody.Append("<tr><td colspan ='2'>Table:&nbsp;" + TabelName + "</td></tr>");
                    tempBody.Append("<tr><td>RULE ID:&nbsp;</td><td>" + Datas.RULD_ID + "</td></tr>");
                    tempBody.Append("<tr><td>RULE NAME:&nbsp;</td><td>" + Datas.RULE_NAME.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ORDER TYPE:&nbsp;</td><td>" + Datas.ORDER_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SUBCONTRACT TYPE:&nbsp;</td><td>" + Datas.SUBCONTRACT_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TECHNOLOGY:&nbsp;</td><td>" + Datas.TECHNOLOGY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EVENT CODE:&nbsp;</td><td>" + Datas.EVENT_CODE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ROOM FLAG:&nbsp;</td><td>" + Datas.ROOM_FLAG.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SAME DAY:&nbsp;</td><td>" + Datas.SAME_DAY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>INTERNET PRICE:&nbsp;</td><td>" + Datas.INTERNET_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>VOIP PRICE:&nbsp;</td><td>" + Datas.VOIP_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>PLAYBOX PRICE:&nbsp;</td><td>" + Datas.PLAYBOX_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>MESH PRICE:&nbsp;</td><td>" + Datas.MECH_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EFFECTIVE DATE:&nbsp;</td><td>" + Datas.EFFECTIVE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EXPIRE DATE:&nbsp;</td><td>" + Datas.EXPIRE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("</table>");

                }
                else if (Datas.TABLE == "T4")
                {

                    tempBody.Append("<table>");
                    tempBody.Append("<tr><td colspan ='2'>Table:&nbsp;" + TabelName + "</td></tr>");
                    tempBody.Append("<tr><td>RULE ID:&nbsp;</td><td>" + Datas.RULD_ID + "</td></tr>");
                    tempBody.Append("<tr><td>RULE NAME:&nbsp;</td><td>" + Datas.RULE_NAME.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ORDER TYPE:&nbsp;</td><td>" + Datas.ORDER_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SUBCONTRACT TYPE:&nbsp;</td><td>" + Datas.SUBCONTRACT_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TECHNOLOGY:&nbsp;</td><td>" + Datas.TECHNOLOGY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>REUSE FLAG:&nbsp;</td><td>" + Datas.REUSE_FLAG.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>DISTANCE FROM:&nbsp;</td><td>" + Datas.DISTANCE_FROM.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>DISTANCE TO:&nbsp;</td><td>" + Datas.DISTANCE_TO.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>INDOOR PRICE:&nbsp;</td><td>" + Datas.INDOOR_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>OUTDOOR PRICE:&nbsp;</td><td>" + Datas.OUTDOOR_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TOTAL PRICE:&nbsp;</td><td>" + Datas.TOTAL_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EFFECTIVE DATE:&nbsp;</td><td>" + Datas.EFFECTIVE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EXPIRE DATE:&nbsp;</td><td>" + Datas.EXPIRE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("</table>");
                }
                else if (Datas.TABLE == "T5")
                {

                    tempBody.Append("<table>");
                    tempBody.Append("<tr><td colspan ='2'>Table:&nbsp;" + TabelName + "</td></tr>");
                    tempBody.Append("<tr><td>RULE ID:&nbsp;</td><td>" + Datas.RULD_ID + "</td></tr>");
                    tempBody.Append("<tr><td>RULE NAME:&nbsp;</td><td>" + Datas.RULE_NAME.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ORDER TYPE:&nbsp;</td><td>" + Datas.ORDER_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SUBCONTRACT_TYPE:&nbsp;</td><td>" + Datas.SUBCONTRACT_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>SUBCONTRACT SUB TYPE:&nbsp;</td><td>" + subsubtype.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TECHNOLOGY:&nbsp;</td><td>" + Datas.TECHNOLOGY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>REUSE FLAG:&nbsp;</td><td>" + Datas.REUSE_FLAG.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TOTAL PRICE:&nbsp;</td><td>" + Datas.TOTAL_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EFFECTIVE_DATE:&nbsp;</td><td>" + Datas.EFFECTIVE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EXPIRE DATE:&nbsp;</td><td>" + Datas.EXPIRE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("</table>");
                }
                else
                {

                    tempBody.Append("<table>");
                    tempBody.Append("<tr><td colspan ='2'>Table:&nbsp;" + TabelName + "</td></tr>");
                    tempBody.Append("<tr><td>RULE ID:&nbsp;</td><td>" + Datas.RULD_ID + "</td></tr>");
                    tempBody.Append("<tr><td>RULE NAME:&nbsp;</td><td>" + Datas.RULE_NAME.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>ORDER TYPE:&nbsp;</td><td>" + Datas.ORDER_TYPE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TECHNOLOGY:&nbsp;</td><td>" + Datas.TECHNOLOGY.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>TOTAL PRICE:&nbsp;</td><td>" + Datas.TOTAL_PRICE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EFFECTIVE DATE:&nbsp;</td><td>" + Datas.EFFECTIVE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("<tr><td>EXPIRE DATE:&nbsp;</td><td>" + Datas.EXPIRE_DATE.ToSafeString() + "</td></tr>");
                    tempBody.Append("</table>");
                }


                tempBody.Append("<br/>");
                tempBody.Append("<span>");
                tempBody.Append(SubjectDTL3);
                tempBody.Append(SubjectDTL4);

                tempBody.Append("<br/></span></span></span></span>Best & Regard");
                tempBody.Append("<br/></span></span></span></span></span>" + FullNAMETH);

                tempBody.Append("<br/>");
                tempBody.Append("<br/>");


                #endregion
                string body = "";
                body = tempBody.ToSafeString();
                return body;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error last mile send mail Menthod EmailTemplate: " + ex.GetErrorMessage());
                return ex.GetErrorMessage();
            }
        }
        private string SendEmail(CostInstallationData Datas)
        {
            var data = SelectFbbCfgLov("CONFIG_INS_EMAIL").ToList();
            var dsendto = data.FirstOrDefault(d => d.LOV_NAME == "SEND_TO");
            var dsubj = data.FirstOrDefault(d => d.LOV_NAME == "SUBJECT");

            string SENDTO = dsendto.LOV_VAL1.ToSafeString();
            string SENDCC = dsendto.LOV_VAL2.ToSafeString();
            string SUBJECT = dsubj.LOV_VAL1.ToSafeString();
            string emailMsg = "";
            string body = "";
            try
            {

                body = EmailTemplate(Datas);



                var command = new SendMailLastMileNotificationCommand
                {
                    ProcessName = "SEND_EMAIL_CONFIG_INS",
                    Subject = SUBJECT,
                    Body = body,
                    SendCC = SENDCC,
                    SendTo = SENDTO
                };
                _sendMail.Handle(command);
                emailMsg = command.ReturnMessage.ToSafeString();

            }
            catch (Exception e)
            {

                emailMsg = e.Message;
            }
            return emailMsg;

        }
        public ActionResult getCostInstallationDataTable([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            // if (!string.IsNullOrEmpty(dataS))
            //// {
            var searchEventModel = new JavaScriptSerializer().Deserialize<CostInstallationtable>(dataS);
            string ColummName = string.Empty; string sortType = string.Empty;
            foreach (var SortD in request.Sorts)
            {
                ColummName = SortD.Member.ToSafeString();
                sortType = SortD.SortDirection.ToSafeString();
            }

            var SortData = new ConfigurationCostInstallationView
            {
                CostInstallationtable1 = null,
                CostInstallationtable2 = null,
                CostInstallationtable3 = null,
                CostInstallationtable4 = null,
                CostInstallationtable5 = null,
                CostInstallationtable6 = null
            };

            if (searchEventModel.SUBCONTRACT_LOCATION == "")
            {
                searchEventModel.SUBCONTRACT_LOCATION = "ALL";
            }
            if (searchEventModel.COMPANY_NAME == "")
            {
                searchEventModel.COMPANY_NAME = "ALL";
            }

            var result = GetCostInstallationTable(searchEventModel);

            if (result != null)
            {

                if (searchEventModel.TB_NAME == "T1")
                {
                    if (ColummName != "")
                    {
                        if (sortType == "Ascending")
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_SUB_TYPE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.SUBCONTRACT_SUB_TYPE).ToList(); }
                            else if (ColummName == "VENDOR_CODE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.VENDOR_CODE).ToList(); }
                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderBy(o => o.RULEID).ToList(); }

                        }
                        else
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_SUB_TYPE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.SUBCONTRACT_SUB_TYPE).ToList(); }
                            else if (ColummName == "VENDOR_CODE") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.VENDOR_CODE).ToList(); }
                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.RULEID).ToList(); }
                        }
                    }
                    else
                    {
                        SortData.CostInstallationtable1 = result.CostInstallationtable1.OrderByDescending(o => o.RULEID).ToList();

                    }
                    return Json(new
                    {
                        Data = SortData.CostInstallationtable1,
                        Total = result.CostInstallationtable1.Count == 0 ? 0 : result.CostInstallationtable1[0].CNT
                    });
                }
                if (searchEventModel.TB_NAME == "T2")
                {
                    if (ColummName != "")
                    {
                        if (sortType == "Ascending")
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "ADDRESS_ID") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.ADDRESS_ID).ToList(); }

                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.EXPIRE_DATE).ToList(); }
                            else if (ColummName == "SAME_DAY") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.SAME_DAY).ToList(); }

                            else { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderBy(o => o.RULEID).ToList(); }

                        }
                        else
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "ADDRESS_ID") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.ADDRESS_ID).ToList(); }

                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.EXPIRE_DATE).ToList(); }
                            else if (ColummName == "SAME_DAY") { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.SAME_DAY).ToList(); }

                            else { SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.RULEID).ToList(); }
                        }
                    }
                    else
                    {
                        SortData.CostInstallationtable2 = result.CostInstallationtable2.OrderByDescending(o => o.RULEID).ToList();

                    }

                    return Json(new
                    {
                        Data = SortData.CostInstallationtable2,
                        Total = result.CostInstallationtable2.Count == 0 ? 0 : result.CostInstallationtable2[0].CNT
                    });
                }
                if (searchEventModel.TB_NAME == "T3")
                {

                    if (ColummName != "")
                    {
                        if (sortType == "Ascending")
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "EVENT_CODE") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.EVENT_CODE).ToList(); }
                            else if (ColummName == "ROOM_FLAG") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.ROOM_FLAG).ToList(); }
                            else if (ColummName == "SAME_DAY") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.SAME_DAY).ToList(); }

                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderBy(o => o.RULEID).ToList(); }

                        }
                        else
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "EVENT_CODE") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.EVENT_CODE).ToList(); }
                            else if (ColummName == "ROOM_FLAG") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.ROOM_FLAG).ToList(); }
                            else if (ColummName == "SAME_DAY") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.SAME_DAY).ToList(); }

                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.RULEID).ToList(); }
                        }
                    }
                    else
                    {
                        SortData.CostInstallationtable3 = result.CostInstallationtable3.OrderByDescending(o => o.RULEID).ToList();

                    }


                    return Json(new
                    {
                        Data = SortData.CostInstallationtable3,
                        Total = result.CostInstallationtable3.Count == 0 ? 0 : result.CostInstallationtable3[0].CNT
                    });
                }
                if (searchEventModel.TB_NAME == "T4")
                {
                    if (ColummName != "")
                    {
                        if (sortType == "Ascending")
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_LOCATION") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.SUBCONTRACT_LOCATION).ToList(); }
                            else if (ColummName == "REUSE_FLAG") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.REUSE_FLAG).ToList(); }
                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderBy(o => o.RULEID).ToList(); }

                        }
                        else
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_LOCATION") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.SUBCONTRACT_LOCATION).ToList(); }
                            else if (ColummName == "REUSE_FLAG") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.REUSE_FLAG).ToList(); }

                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.RULEID).ToList(); }
                        }
                    }
                    else
                    {
                        SortData.CostInstallationtable4 = result.CostInstallationtable4.OrderByDescending(o => o.RULEID).ToList();

                    }

                    return Json(new
                    {
                        Data = SortData.CostInstallationtable4,
                        Total = result.CostInstallationtable4.Count == 0 ? 0 : result.CostInstallationtable4[0].CNT
                    });



                }
                if (searchEventModel.TB_NAME == "T5")
                {

                    if (ColummName != "")
                    {
                        if (sortType == "Ascending")
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "REUSE_FLAG") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.REUSE_FLAG).ToList(); }

                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderBy(o => o.RULEID).ToList(); }

                        }
                        else
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "SUBCONTRACT_TYPE") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.SUBCONTRACT_TYPE).ToList(); }
                            else if (ColummName == "REUSE_FLAG") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.REUSE_FLAG).ToList(); }

                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.RULEID).ToList(); }
                        }
                    }
                    else
                    {
                        SortData.CostInstallationtable5 = result.CostInstallationtable5.OrderByDescending(o => o.RULEID).ToList();

                    }

                    return Json(new
                    {
                        Data = SortData.CostInstallationtable5,
                        Total = result.CostInstallationtable5.Count == 0 ? 0 : result.CostInstallationtable5[0].CNT
                    });
                }
                if (searchEventModel.TB_NAME == "T6")
                {
                    if (ColummName != "")
                    {
                        if (sortType == "Ascending")
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderBy(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderBy(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderBy(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderBy(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderBy(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderBy(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderBy(o => o.RULEID).ToList(); }

                        }
                        else
                        {
                            if (ColummName == "RULEID") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.RULEID).ToList(); }
                            else if (ColummName == "RULE_NAME") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.RULE_NAME).ToList(); }
                            else if (ColummName == "ORDER_TYPE") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.ORDER_TYPE).ToList(); }
                            else if (ColummName == "TECHNOLOGY") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.TECHNOLOGY).ToList(); }
                            else if (ColummName == "EFFECTIVE_DATE_TEXT") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.EFFECTIVE_DATE).ToList(); }
                            else if (ColummName == "EXPIRE_DATE_TEXT") { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.EXPIRE_DATE).ToList(); }
                            else { SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.RULEID).ToList(); }
                        }
                    }
                    else
                    {
                        SortData.CostInstallationtable6 = result.CostInstallationtable6.OrderByDescending(o => o.RULEID).ToList();

                    }



                    return Json(new
                    {
                        Data = SortData.CostInstallationtable6,
                        Total = result.CostInstallationtable6.Count == 0 ? 0 : result.CostInstallationtable6[0].CNT
                    });
                }

            }

            return null;
        }
        private ConfigurationCostInstallationView GetCostInstallationTable(CostInstallationtable searchmodel)
        {
            ///  searchModel.DATE_FROM == "" ? null : searchModel.DATE_FROM.Replace("/", ""),
            try
            {
                var query = new GetCostInstallationTableQuery()
                {

                    TB_NAME = searchmodel.TB_NAME,
                    RULE_ID = searchmodel.RULE_ID,
                    SUBCONTTYPE = searchmodel.SUBCONTTYPE,
                    SUBCONTRACT_LOCATION = searchmodel.SUBCONTRACT_LOCATION == "" ? "ALL" : searchmodel.SUBCONTRACT_LOCATION,
                    COMPANY_NAME = searchmodel.COMPANY_NAME == "" ? "ALL" : searchmodel.COMPANY_NAME,
                    ORD_TYPE = searchmodel.ORD_TYPE,
                    TECH_TYPE = searchmodel.TECH_TYPE,
                    DATE_FROM = searchmodel.DATE_FROM == "" ? null : searchmodel.DATE_FROM.Replace("/", ""),
                    DATE_TO = searchmodel.DATE_TO == "" ? null : searchmodel.DATE_TO.Replace("/", ""),
                    EXPDATE_FROM = searchmodel.EXPDATE_FROM == "" ? null : searchmodel.EXPDATE_FROM.Replace("/", ""),
                    EXPDATE_TO = searchmodel.EXPDATE_TO == "" ? null : searchmodel.EXPDATE_TO.Replace("/", ""),
                    PAGE_INDEX = searchmodel.PAGE_INDEX,
                    PAGE_SIZE = searchmodel.PAGE_SIZE,

                };
                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    if (searchmodel.TB_NAME == "T1")
                    {

                        foreach (var item in result.CostInstallationtable1)
                        {
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.EXPIRE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EXPIRE_DATE_TEXT = item.EXPIRE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EXPIRE_DATE_TEXT = "";
                            }
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.CREATE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.CREATE_DATE_TEXT = item.CREATE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.CREATE_DATE_TEXT = "";
                            }

                            if (item.UPDATED_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.UPDATED_DATE_TEXT = item.UPDATED_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.UPDATED_DATE_TEXT = "";
                            }


                        }
                    }
                    if (searchmodel.TB_NAME == "T2")
                    {
                        foreach (var item in result.CostInstallationtable2)
                        {
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.EXPIRE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EXPIRE_DATE_TEXT = item.EXPIRE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EXPIRE_DATE_TEXT = "";
                            }
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.CREATE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.CREATE_DATE_TEXT = item.CREATE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.CREATE_DATE_TEXT = "";
                            }

                            if (item.UPDATED_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.UPDATED_DATE_TEXT = item.UPDATED_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.UPDATED_DATE_TEXT = "";
                            }

                        }
                    }
                    if (searchmodel.TB_NAME == "T3")
                    {
                        foreach (var item in result.CostInstallationtable3)
                        {
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.EXPIRE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EXPIRE_DATE_TEXT = item.EXPIRE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EXPIRE_DATE_TEXT = "";
                            }
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.CREATE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.CREATE_DATE_TEXT = item.CREATE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.CREATE_DATE_TEXT = "";
                            }

                            if (item.UPDATED_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.UPDATED_DATE_TEXT = item.UPDATED_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.UPDATED_DATE_TEXT = "";
                            }

                        }
                    }
                    if (searchmodel.TB_NAME == "T4")
                    {
                        foreach (var item in result.CostInstallationtable4)
                        {
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.EXPIRE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EXPIRE_DATE_TEXT = item.EXPIRE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EXPIRE_DATE_TEXT = "";
                            }
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.CREATE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.CREATE_DATE_TEXT = item.CREATE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.CREATE_DATE_TEXT = "";
                            }

                            if (item.UPDATED_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.UPDATED_DATE_TEXT = item.UPDATED_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.UPDATED_DATE_TEXT = "";
                            }

                        }
                    }
                    if (searchmodel.TB_NAME == "T5")
                    {
                        foreach (var item in result.CostInstallationtable5)
                        {
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.EXPIRE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EXPIRE_DATE_TEXT = item.EXPIRE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EXPIRE_DATE_TEXT = "";
                            }
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.CREATE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.CREATE_DATE_TEXT = item.CREATE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.CREATE_DATE_TEXT = "";
                            }

                            if (item.UPDATED_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.UPDATED_DATE_TEXT = item.UPDATED_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.UPDATED_DATE_TEXT = "";
                            }

                        }
                    }
                    if (searchmodel.TB_NAME == "T6")
                    {
                        foreach (var item in result.CostInstallationtable6)
                        {
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.EXPIRE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EXPIRE_DATE_TEXT = item.EXPIRE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EXPIRE_DATE_TEXT = "";
                            }
                            if (item.EFFECTIVE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.EFFECTIVE_DATE_TEXT = item.EFFECTIVE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.EFFECTIVE_DATE_TEXT = "";
                            }
                            if (item.CREATE_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.CREATE_DATE_TEXT = item.CREATE_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.CREATE_DATE_TEXT = "";
                            }

                            if (item.UPDATED_DATE.ToDateDisplayText() != "01/01/0001")
                            {
                                item.UPDATED_DATE_TEXT = item.UPDATED_DATE.ToDateDisplayText();
                            }
                            else
                            {
                                item.UPDATED_DATE_TEXT = "";
                            }

                        }
                    }

                }

                return result;
            }
            catch
            {
                return null;
            }

        }
        public ActionResult getEditCostInstallationDataTable([DataSourceRequest] DataSourceRequest request, string TABLE, string RULEID)
        {


            string _RULDID = string.Empty;
            string _RULE_NAME = string.Empty;
            string _ORDER_TYPE = string.Empty;
            string _SUBCONTRACT_TYPE = string.Empty;
            string _SUBCONTRACT_SUB_TYPE = string.Empty;
            string _VENDOR_CODE = string.Empty;
            string _TECHNOLOGY = string.Empty;
            string _REUSE_FLAG = string.Empty;
            string _DISTANCE_FROM = string.Empty;
            string _DISTANCE_TO = string.Empty;
            string _INDOOR_PRICE = string.Empty;
            string _OUTDOOR_PRICE = string.Empty;
            string _ROOM_FLAG = string.Empty;
            string _INTERNET_PRICE = string.Empty;
            string _VOIP_PRICE = string.Empty;
            string _MECH_PRICE = string.Empty;
            string _PLAYBOX_PRICE = string.Empty;
            string _ADDRESS_ID = string.Empty;
            string _EVENT_TYPE = string.Empty;
            string _EVENT_CODE = string.Empty;
            string _TOTAL_PRICE = string.Empty;
            string _EFFECTIVE_DATE = string.Empty;
            string _EXPIRE_DATE = string.Empty;
            string _CREATE_DATE = string.Empty;
            string _CREATE_BY = string.Empty;
            string _UPDATE_DATE = string.Empty;
            string _UPDATE_BY = string.Empty;
            string _SAME_DAY = string.Empty;
            string _SUBCONTRACT_LOCATION = string.Empty;
            string _COMPANY_NAME = string.Empty;

            if (!string.IsNullOrEmpty(TABLE) && !string.IsNullOrEmpty(RULEID))
            {


                var query = new GetCostInstallationTableQuery()
                {
                    TB_NAME = TABLE,
                    RULE_ID = RULEID,
                    SUBCONTTYPE = "ALL",
                    SUBCONTRACT_LOCATION = "ALL",
                    COMPANY_NAME = "ALL",
                    ORD_TYPE = "ALL",
                    TECH_TYPE = "ALL",
                    DATE_FROM = "",
                    DATE_TO = "",
                    EXPDATE_FROM = "",
                    EXPDATE_TO = "",
                    PAGE_INDEX = "1",
                    PAGE_SIZE = "9999",

                };
                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    if (TABLE == "T1")
                    {

                        var selectResult = result.CostInstallationtable1;
                        _RULDID = selectResult[0].RULEID.ToSafeString();
                        _RULE_NAME = selectResult[0].RULE_NAME.ToSafeString();
                        _ORDER_TYPE = selectResult[0].ORDER_TYPE.ToSafeString();
                        _SUBCONTRACT_TYPE = selectResult[0].SUBCONTRACT_TYPE.ToSafeString();
                        _SUBCONTRACT_SUB_TYPE = selectResult[0].SUBCONTRACT_SUB_TYPE.ToSafeString();
                        _VENDOR_CODE = selectResult[0].VENDOR_CODE.ToSafeString();
                        _TECHNOLOGY = selectResult[0].TECHNOLOGY.ToSafeString();
                        _TOTAL_PRICE = selectResult[0].TOTAL_PRICE.ToSafeString();
                        _EFFECTIVE_DATE = selectResult[0].EFFECTIVE_DATE.ToShortDateString();
                        _EXPIRE_DATE = selectResult[0].EXPIRE_DATE.ToShortDateString();
                        _CREATE_DATE = selectResult[0].CREATE_DATE.ToDateDisplayText();
                        _CREATE_BY = selectResult[0].CREATE_BY.ToSafeString();
                        _UPDATE_DATE = selectResult[0].UPDATED_DATE.ToDateDisplayText();
                        _UPDATE_BY = selectResult[0].UPDATED_BY.ToSafeString();


                        return Json(new
                        {
                            _RULDID = _RULDID,
                            _RULE_NAME = _RULE_NAME,
                            _ORDER_TYPE = _ORDER_TYPE,
                            _SUBCONTRACT_TYPE = _SUBCONTRACT_TYPE,
                            _SUBCONTRACT_SUB_TYPE = _SUBCONTRACT_SUB_TYPE,
                            _VENDOR_CODE = _VENDOR_CODE,
                            _TECHNOLOGY = _TECHNOLOGY,
                            _TOTAL_PRICE = _TOTAL_PRICE,
                            _EFFECTIVE_DATE = _EFFECTIVE_DATE,
                            _EXPIRE_DATE = _EXPIRE_DATE,
                            _CREATE_DATE = _CREATE_DATE,
                            _CREATE_BY = _CREATE_BY,
                            _UPDATE_DATE = _UPDATE_DATE,
                            _UPDATE_BY = _UPDATE_BY,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (TABLE == "T2")
                    {
                        var selectResult = result.CostInstallationtable2;
                        _RULDID = selectResult[0].RULEID.ToSafeString();
                        _RULE_NAME = selectResult[0].RULE_NAME.ToSafeString();
                        _ORDER_TYPE = selectResult[0].ORDER_TYPE.ToSafeString();
                        _SUBCONTRACT_TYPE = selectResult[0].SUBCONTRACT_TYPE.ToSafeString();
                        _TECHNOLOGY = selectResult[0].TECHNOLOGY.ToSafeString();

                        _INTERNET_PRICE = selectResult[0].INTERNET_PRICE.ToSafeString();
                        _VOIP_PRICE = selectResult[0].VOIP_PRICE.ToSafeString();
                        _PLAYBOX_PRICE = selectResult[0].PLAYBOX_PRICE.ToSafeString();
                        _MECH_PRICE = selectResult[0].MESH_PRICE.ToSafeString();
                        _ADDRESS_ID = selectResult[0].ADDRESS_ID.ToSafeString();
                        _EVENT_TYPE = selectResult[0].EVENT_TYPE.ToSafeString();

                        _EFFECTIVE_DATE = selectResult[0].EFFECTIVE_DATE.ToShortDateString();
                        _EXPIRE_DATE = selectResult[0].EXPIRE_DATE.ToShortDateString();
                        _CREATE_DATE = selectResult[0].CREATE_DATE.ToDateDisplayText();
                        _CREATE_BY = selectResult[0].CREATE_BY.ToSafeString();
                        _UPDATE_DATE = selectResult[0].UPDATED_DATE.ToDateDisplayText();
                        _UPDATE_BY = selectResult[0].UPDATED_BY.ToSafeString();
                        _SAME_DAY = selectResult[0].SAME_DAY.ToSafeString();
                        return Json(new
                        {
                            _RULDID = _RULDID,
                            _RULE_NAME = _RULE_NAME,
                            _ORDER_TYPE = _ORDER_TYPE,
                            _SUBCONTRACT_TYPE = _SUBCONTRACT_TYPE,
                            _TECHNOLOGY = _TECHNOLOGY,

                            _INTERNET_PRICE = _INTERNET_PRICE,
                            _VOIP_PRICE = _VOIP_PRICE,
                            _PLAYBOX_PRICE = _PLAYBOX_PRICE,
                            _MECH_PRICE = _MECH_PRICE,
                            _ADDRESS_ID = _ADDRESS_ID,
                            _EVENT_TYPE = _EVENT_TYPE,


                            _EFFECTIVE_DATE = _EFFECTIVE_DATE,
                            _EXPIRE_DATE = _EXPIRE_DATE,
                            _CREATE_DATE = _CREATE_DATE,
                            _CREATE_BY = _CREATE_BY,
                            _UPDATE_DATE = _UPDATE_DATE,
                            _UPDATE_BY = _UPDATE_BY,
                            _SAME_DAY = _SAME_DAY,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (TABLE == "T3")
                    {
                        var selectResult = result.CostInstallationtable3;
                        _RULDID = selectResult[0].RULEID.ToSafeString();
                        _RULE_NAME = selectResult[0].RULE_NAME.ToSafeString();
                        _ORDER_TYPE = selectResult[0].ORDER_TYPE.ToSafeString();
                        _SUBCONTRACT_TYPE = selectResult[0].SUBCONTRACT_TYPE.ToSafeString();
                        _TECHNOLOGY = selectResult[0].TECHNOLOGY.ToSafeString();
                        _EVENT_CODE = selectResult[0].EVENT_CODE.ToSafeString();
                        _ROOM_FLAG = selectResult[0].ROOM_FLAG.ToSafeString();


                        _INTERNET_PRICE = selectResult[0].INTERNET_PRICE.ToSafeString();
                        _VOIP_PRICE = selectResult[0].VOIP_PRICE.ToSafeString();
                        _PLAYBOX_PRICE = selectResult[0].PLAYBOX_PRICE.ToSafeString();
                        _MECH_PRICE = selectResult[0].MESH_PRICE.ToSafeString();

                        _EFFECTIVE_DATE = selectResult[0].EFFECTIVE_DATE.ToShortDateString();
                        _EXPIRE_DATE = selectResult[0].EXPIRE_DATE.ToShortDateString();
                        _CREATE_DATE = selectResult[0].CREATE_DATE.ToDateDisplayText();
                        _CREATE_BY = selectResult[0].CREATE_BY.ToSafeString();
                        _UPDATE_DATE = selectResult[0].UPDATED_DATE.ToDateDisplayText();
                        _UPDATE_BY = selectResult[0].UPDATED_BY.ToSafeString();
                        _SAME_DAY = selectResult[0].SAME_DAY.ToSafeString();
                        return Json(new
                        {
                            _RULDID = _RULDID,
                            _RULE_NAME = _RULE_NAME,
                            _ORDER_TYPE = _ORDER_TYPE,
                            _SUBCONTRACT_TYPE = _SUBCONTRACT_TYPE,
                            _TECHNOLOGY = _TECHNOLOGY,
                            _EVENT_CODE = _EVENT_CODE,
                            _ROOM_FLAG = _ROOM_FLAG,
                            _INTERNET_PRICE = _INTERNET_PRICE,
                            _VOIP_PRICE = _VOIP_PRICE,
                            _PLAYBOX_PRICE = _PLAYBOX_PRICE,
                            _MECH_PRICE = _MECH_PRICE,



                            _EFFECTIVE_DATE = _EFFECTIVE_DATE,
                            _EXPIRE_DATE = _EXPIRE_DATE,
                            _CREATE_DATE = _CREATE_DATE,
                            _CREATE_BY = _CREATE_BY,
                            _UPDATE_DATE = _UPDATE_DATE,
                            _UPDATE_BY = _UPDATE_BY,
                            _SAME_DAY = _SAME_DAY,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (TABLE == "T4")
                    {
                        var selectResult = result.CostInstallationtable4;
                        _RULDID = selectResult[0].RULEID.ToSafeString();
                        _RULE_NAME = selectResult[0].RULE_NAME.ToSafeString();

                        _SUBCONTRACT_TYPE = selectResult[0].SUBCONTRACT_TYPE.ToSafeString();
                        _TECHNOLOGY = selectResult[0].TECHNOLOGY.ToSafeString();
                        //R21.2 เพิ่ม ORDER_TYPE Table4
                        _ORDER_TYPE = selectResult[0].ORDER_TYPE.ToSafeString();

                        _REUSE_FLAG = selectResult[0].REUSE_FLAG.ToSafeString();
                        _DISTANCE_FROM = selectResult[0].DISTANCE_FROM.ToSafeString();
                        _DISTANCE_TO = selectResult[0].DISTANCE_TO.ToSafeString();
                        _INDOOR_PRICE = selectResult[0].INDOOR_PRICE.ToSafeString();
                        _OUTDOOR_PRICE = selectResult[0].OUTDOOR_PRICE.ToSafeString();
                        _TOTAL_PRICE = selectResult[0].TOTAL_PRICE.ToSafeString();



                        _EFFECTIVE_DATE = selectResult[0].EFFECTIVE_DATE.ToShortDateString();
                        _EXPIRE_DATE = selectResult[0].EXPIRE_DATE.ToShortDateString();
                        _CREATE_DATE = selectResult[0].CREATE_DATE.ToDateDisplayText();
                        _CREATE_BY = selectResult[0].CREATE_BY.ToSafeString();
                        _UPDATE_DATE = selectResult[0].UPDATED_DATE.ToDateDisplayText();
                        _UPDATE_BY = selectResult[0].UPDATED_BY.ToSafeString();
                        _SUBCONTRACT_LOCATION = selectResult[0].SUBCONTRACT_LOCATION.ToSafeString();
                        _COMPANY_NAME = selectResult[0].COMPANY_NAME.ToSafeString();
                        return Json(new
                        {
                            _RULDID = _RULDID,
                            _RULE_NAME = _RULE_NAME,

                            _SUBCONTRACT_TYPE = _SUBCONTRACT_TYPE,
                            _TECHNOLOGY = _TECHNOLOGY,
                            //R21.2 เพิ่ม ORDER_TYPE Table4
                            _ORDER_TYPE = _ORDER_TYPE,

                            _REUSE_FLAG = _REUSE_FLAG,
                            _DISTANCE_FROM = _DISTANCE_FROM,
                            _DISTANCE_TO = _DISTANCE_TO,
                            _INDOOR_PRICE = _INDOOR_PRICE,
                            _OUTDOOR_PRICE = _OUTDOOR_PRICE,
                            _TOTAL_PRICE = _TOTAL_PRICE,
                            _EFFECTIVE_DATE = _EFFECTIVE_DATE,
                            _EXPIRE_DATE = _EXPIRE_DATE,
                            _CREATE_DATE = _CREATE_DATE,
                            _CREATE_BY = _CREATE_BY,
                            _UPDATE_DATE = _UPDATE_DATE,
                            _UPDATE_BY = _UPDATE_BY,
                            _SUBCONTRACT_LOCATION = _SUBCONTRACT_LOCATION,
                            _COMPANY_NAME = _COMPANY_NAME,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (TABLE == "T5")
                    {
                        var selectResult = result.CostInstallationtable5;
                        _RULDID = selectResult[0].RULEID.ToSafeString();
                        _RULE_NAME = selectResult[0].RULE_NAME.ToSafeString();
                        _ORDER_TYPE = selectResult[0].ORDER_TYPE.ToSafeString();
                        _SUBCONTRACT_TYPE = selectResult[0].SUBCONTRACT_TYPE.ToSafeString();
                        _SUBCONTRACT_SUB_TYPE = selectResult[0].SUBCONTRACT_SUB_TYPE.ToSafeString();
                        _TECHNOLOGY = selectResult[0].TECHNOLOGY.ToSafeString();


                        _REUSE_FLAG = selectResult[0].REUSE_FLAG.ToSafeString();
                        _TOTAL_PRICE = selectResult[0].TOTAL_PRICE.ToSafeString();



                        _EFFECTIVE_DATE = selectResult[0].EFFECTIVE_DATE.ToShortDateString();
                        _EXPIRE_DATE = selectResult[0].EXPIRE_DATE.ToShortDateString();
                        _CREATE_DATE = selectResult[0].CREATE_DATE.ToDateDisplayText();
                        _CREATE_BY = selectResult[0].CREATE_BY.ToSafeString();
                        _UPDATE_DATE = selectResult[0].UPDATED_DATE.ToDateDisplayText();
                        _UPDATE_BY = selectResult[0].UPDATED_BY.ToSafeString();
                        return Json(new
                        {
                            _RULDID = _RULDID,
                            _RULE_NAME = _RULE_NAME,
                            _ORDER_TYPE = _ORDER_TYPE,
                            _SUBCONTRACT_TYPE = _SUBCONTRACT_TYPE,
                            _SUBCONTRACT_SUB_TYPE = _SUBCONTRACT_SUB_TYPE,
                            _TECHNOLOGY = _TECHNOLOGY,

                            _REUSE_FLAG = _REUSE_FLAG,

                            _TOTAL_PRICE = _TOTAL_PRICE,
                            _EFFECTIVE_DATE = _EFFECTIVE_DATE,
                            _EXPIRE_DATE = _EXPIRE_DATE,
                            _CREATE_DATE = _CREATE_DATE,
                            _CREATE_BY = _CREATE_BY,
                            _UPDATE_DATE = _UPDATE_DATE,
                            _UPDATE_BY = _UPDATE_BY,
                            //      _SUBCONTRACT_LOCATION = _SUBCONTRACT_LOCATION,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (TABLE == "T6")
                    {

                        var selectResult = result.CostInstallationtable6;
                        _RULDID = selectResult[0].RULEID.ToSafeString();
                        _RULE_NAME = selectResult[0].RULE_NAME.ToSafeString();
                        _ORDER_TYPE = selectResult[0].ORDER_TYPE.ToSafeString();

                        _TECHNOLOGY = selectResult[0].TECHNOLOGY.ToSafeString();



                        _TOTAL_PRICE = selectResult[0].TOTAL_PRICE.ToSafeString();



                        _EFFECTIVE_DATE = selectResult[0].EFFECTIVE_DATE.ToShortDateString();
                        _EXPIRE_DATE = selectResult[0].EXPIRE_DATE.ToShortDateString();
                        _CREATE_DATE = selectResult[0].CREATE_DATE.ToDateDisplayText();
                        _CREATE_BY = selectResult[0].CREATE_BY.ToSafeString();
                        _UPDATE_DATE = selectResult[0].UPDATED_DATE.ToDateDisplayText();
                        _UPDATE_BY = selectResult[0].UPDATED_BY.ToSafeString();
                        return Json(new
                        {
                            _RULDID = _RULDID,
                            _RULE_NAME = _RULE_NAME,
                            _ORDER_TYPE = _ORDER_TYPE,
                            _TECHNOLOGY = _TECHNOLOGY,
                            _TOTAL_PRICE = _TOTAL_PRICE,

                            _EFFECTIVE_DATE = _EFFECTIVE_DATE,
                            _EXPIRE_DATE = _EXPIRE_DATE,
                            _CREATE_DATE = _CREATE_DATE,
                            _CREATE_BY = _CREATE_BY,
                            _UPDATE_DATE = _UPDATE_DATE,
                            _UPDATE_BY = _UPDATE_BY,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                return null;
            }
            return null;
        }


        #region EXPORTEXCEL
        private string rptCriteria = " ACCESSNO: {0}  ORDERNO: {1}";
        private string rptName = "LastMileByDistance";


        public ActionResult _ExportAll([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            var searchEventModel = new JavaScriptSerializer().Deserialize<CostInstallationtable>(dataS);
            var bytes = DataExportAll(searchEventModel);
            return File(bytes, "application/excel", "Configuration-CostInstallation-All-Table" + ".xls");

        }

        private DataTable SetEntity<T>(List<T> data, string fileName, string TableName)
        {
            ////int col = 0;
            ////string _TableName = string.Empty;
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name);
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

            ////col = props.Count - 6;
            table.Columns.Remove("EFFECTIVE_DATE");
            table.Columns.Remove("EXPIRE_DATE");
            table.Columns.Remove("CREATE_DATE");
            table.Columns.Remove("UPDATED_DATE");
            table.Columns.Remove("RowNumber");
            table.Columns.Remove("CNT");
            //// table.Columns.Remove("SUBCONTRACT_LOCATION");
            if (TableName == "T1")
            {
                //// _TableName = "Table1";
                table.TableName = "Table1";
                table.Columns["RULEID"].Caption = "RUIE ID";
                table.Columns["RULE_NAME"].Caption = "RULE NAME";
                table.Columns["ORDER_TYPE"].Caption = "ORDER TYPE";
                table.Columns["SUBCONTRACT_TYPE"].Caption = "SUBCONTRACT TYPE";
                table.Columns["SUBCONTRACT_SUB_TYPE"].Caption = "SUBCONTRACT SUB TYPE";
                table.Columns["VENDOR_CODE"].Caption = "VENDOR CODE";
                table.Columns["TECHNOLOGY"].Caption = "TECHNOLOGY";
                table.Columns["TOTAL_PRICE"].Caption = "TOTAL PRICE";
                table.Columns["EFFECTIVE_DATE_TEXT"].Caption = "EFFECTIVE DATE";
                table.Columns["EXPIRE_DATE_TEXT"].Caption = "EXPIRE DATE";
                table.Columns["CREATE_DATE_TEXT"].Caption = "CREATE DATE";
                table.Columns["CREATE_BY"].Caption = "CREATE BY";
                table.Columns["UPDATED_DATE_TEXT"].Caption = "UPDATED DATE";
                table.Columns["UPDATED_BY"].Caption = "UPDATED BY";
            }
            if (TableName == "T2")
            {
                ////_TableName = "Table2";
                table.TableName = "Table2";
                table.Columns["RULEID"].Caption = "RUIE ID";
                table.Columns["RULE_NAME"].Caption = "RULE NAME";
                table.Columns["ORDER_TYPE"].Caption = "ORDER TYPE";
                table.Columns["SUBCONTRACT_TYPE"].Caption = "SUBCONTRACT TYPE";
                table.Columns["TECHNOLOGY"].Caption = "TECHNOLOGY";
                table.Columns["INTERNET_PRICE"].Caption = "INTERNET PRICE";
                table.Columns["VOIP_PRICE"].Caption = "VOIP PRICE";
                table.Columns["PLAYBOX_PRICE"].Caption = "PLAYBOX PRICE";
                table.Columns["MESH_PRICE"].Caption = "MESH PRICE";
                table.Columns["ADDRESS_ID"].Caption = "ADDRESS ID";
                table.Columns["EVENT_TYPE"].Caption = "EVENT TYPE";
                table.Columns["EFFECTIVE_DATE_TEXT"].Caption = "EFFECTIVE DATE";
                table.Columns["EXPIRE_DATE_TEXT"].Caption = "EXPIRE DATE";
                table.Columns["CREATE_DATE_TEXT"].Caption = "CREATE DATE";
                table.Columns["CREATE_BY"].Caption = "CREATE BY";
                table.Columns["UPDATED_DATE_TEXT"].Caption = "UPDATED DATE";
                table.Columns["UPDATED_BY"].Caption = "UPDATED BY";
                table.Columns["SAME_DAY"].Caption = "SAME DAY";
                ////   table.Columns.Remove("SUBCONTRACT_LOCATION");

            }
            if (TableName == "T3")
            {
                //// _TableName = "Table3";
                table.TableName = "Table3";
                table.Columns["RULEID"].Caption = "RUIE ID";
                table.Columns["RULE_NAME"].Caption = "RULE NAME";
                table.Columns["ORDER_TYPE"].Caption = "ORDER TYPE";
                table.Columns["SUBCONTRACT_TYPE"].Caption = "SUBCONTRACT TYPE";
                table.Columns["TECHNOLOGY"].Caption = "TECHNOLOGY";
                table.Columns["EVENT_CODE"].Caption = "EVENT CODE";
                table.Columns["ROOM_FLAG"].Caption = "ROOM_FLAG";
                ////  table.Columns["REUSE_FLAG"].Caption = "REUSE FLAG";
                table.Columns["INTERNET_PRICE"].Caption = "INTERNET PRICE";
                table.Columns["VOIP_PRICE"].Caption = "VOIP PRICE";
                table.Columns["PLAYBOX_PRICE"].Caption = "PLAYBOX PRICE";
                table.Columns["MESH_PRICE"].Caption = "MESH PRICE";
                table.Columns["EFFECTIVE_DATE_TEXT"].Caption = "EFFECTIVE DATE";
                table.Columns["EXPIRE_DATE_TEXT"].Caption = "EXPIRE DATE";
                table.Columns["CREATE_DATE_TEXT"].Caption = "CREATE DATE";
                table.Columns["CREATE_BY"].Caption = "CREATE BY";
                table.Columns["UPDATED_DATE_TEXT"].Caption = "UPDATED DATE";
                table.Columns["UPDATED_BY"].Caption = "UPDATED BY";
                table.Columns["SAME_DAY"].Caption = "SAME DAY";
                table.Columns["ROOM_FLAG"].Caption = "ROOM FLAG";
                ////  table.Columns.Remove("SUBCONTRACT_LOCATION");
            }
            if (TableName == "T4")
            {
                ////_TableName = "Table4";
                table.TableName = "Table4";
                table.Columns["RULEID"].Caption = "RUIE ID";
                table.Columns["RULE_NAME"].Caption = "RULE NAME";
                table.Columns["SUBCONTRACT_TYPE"].Caption = "SUBCONTRACT TYPE";
                table.Columns["TECHNOLOGY"].Caption = "TECHNOLOGY";
                //R21.2 เพิ่ม ORDER_TYPE Table4
                table.Columns["ORDER_TYPE"].Caption = "ORDER TYPE";
                table.Columns["REUSE_FLAG"].Caption = "REUSE FLAG";
                table.Columns["DISTANCE_FROM"].Caption = "DISTANCE FROM";
                table.Columns["DISTANCE_TO"].Caption = "DISTANCE TO";
                table.Columns["INDOOR_PRICE"].Caption = "INDOOR PRICE";
                table.Columns["OUTDOOR_PRICE"].Caption = "OUTDOOR PRICE";
                table.Columns["TOTAL_PRICE"].Caption = "TOTAL PRICE";
                table.Columns["EFFECTIVE_DATE_TEXT"].Caption = "EFFECTIVE DATE";
                table.Columns["EXPIRE_DATE_TEXT"].Caption = "EXPIRE DATE";
                table.Columns["CREATE_DATE_TEXT"].Caption = "CREATE DATE";
                table.Columns["CREATE_BY"].Caption = "CREATE BY";
                table.Columns["UPDATED_DATE_TEXT"].Caption = "UPDATED DATE";
                table.Columns["UPDATED_BY"].Caption = "UPDATED BY";
                table.Columns["SUBCONTRACT_LOCATION"].Caption = "LOCATION CODE";
                ////table.Columns["ORDER_TYPE"].Caption = "ORDER TYPE";
                ////table.Columns.Remove("ORDER_TYPE");
                table.Columns.Remove("SAME_DAY");
                table.Columns.Remove("COMPANY_NAME");
            }
            if (TableName == "T5")
            {
                ////_TableName = "Table5";
                table.TableName = "Table5";
                table.Columns["RULEID"].Caption = "RUIE ID";
                table.Columns["RULE_NAME"].Caption = "RULE NAME";
                table.Columns["ORDER_TYPE"].Caption = "ORDER TYPE";
                table.Columns["SUBCONTRACT_TYPE"].Caption = "SUBCONTRACT TYPE";
                table.Columns["SUBCONTRACT_SUB_TYPE"].Caption = "SUBCONTRACT SUB TYPE";
                table.Columns["REUSE_FLAG"].Caption = "REUSE FLAG";
                table.Columns["TOTAL_PRICE"].Caption = "TOTAL PRICE";
                table.Columns["TECHNOLOGY"].Caption = "TECHNOLOGY";
                table.Columns["EFFECTIVE_DATE_TEXT"].Caption = "EFFECTIVE DATE";
                table.Columns["EXPIRE_DATE_TEXT"].Caption = "EXPIRE DATE";
                table.Columns["CREATE_DATE_TEXT"].Caption = "CREATE DATE";
                table.Columns["CREATE_BY"].Caption = "CREATE BY";
                table.Columns["UPDATED_DATE_TEXT"].Caption = "UPDATED DATE";
                table.Columns["UPDATED_BY"].Caption = "UPDATED BY";
                //// table.Columns["SUBCONTRACT_LOCATION"].Caption = "SUBCONTRACT LOCATION";
                table.Columns.Remove("SAME_DAY");
                //// table.Columns.Remove("SUBCONTRACT_LOCATION");
            }
            if (TableName == "T6")
            {
                ////_TableName = "Table6";
                table.TableName = "Table6";
                table.Columns["RULEID"].Caption = "RUIE ID";
                table.Columns["RULE_NAME"].Caption = "RULE NAME";
                table.Columns["ORDER_TYPE"].Caption = "ORDER TYPE";
                table.Columns["TECHNOLOGY"].Caption = "TECHNOLOGY";
                table.Columns["TOTAL_PRICE"].Caption = "TOTAL PRICE";
                table.Columns["EFFECTIVE_DATE_TEXT"].Caption = "EFFECTIVE DATE";
                table.Columns["EXPIRE_DATE_TEXT"].Caption = "EXPIRE DATE";
                table.Columns["CREATE_DATE_TEXT"].Caption = "CREATE DATE";
                table.Columns["CREATE_BY"].Caption = "CREATE BY";
                table.Columns["UPDATED_DATE_TEXT"].Caption = "UPDATED DATE";
                table.Columns["UPDATED_BY"].Caption = "UPDATED BY";
                table.Columns.Remove("SAME_DAY");
                ////   table.Columns.Remove("SUBCONTRACT_LOCATION");

            }

            return table;
        }
        private byte[] DataExportAll(CostInstallationtable searchEventModel)
        {
            ////string filename = string.Empty;
            var ds = new DataSet();
            DataTable table = new DataTable();

            DataTable dtTable1 = table.Clone();
            DataTable dtTable2 = table.Clone();
            DataTable dtTable3 = table.Clone();
            DataTable dtTable4 = table.Clone();
            DataTable dtTable5 = table.Clone();
            DataTable dtTable6 = table.Clone();

            ////List<ConfigurationCostInstallationView> resultList = new List<ConfigurationCostInstallationView>();

            var data = SelectFbbCfgLov("CONFIG_COST_INS")
             .Where(d => d.LOV_VAL1 != null).ToList();
            var PageSize = SelectFbbCfgLov("PAGE_SIZE").FirstOrDefault(s => s.LOV_VAL1 != null);
            foreach (var d in data)
            {

                searchEventModel.TB_NAME = d.LOV_VAL1;
                searchEventModel.SUBCONTTYPE = "ALL";
                searchEventModel.PAGE_INDEX = "1";
                searchEventModel.PAGE_SIZE = PageSize != null ? PageSize.LOV_VAL1 : "9999";
                var result = GetCostInstallationTable(searchEventModel);

                if (d.LOV_VAL1 == "T1")
                {
                    var table1 = result.CostInstallationtable1;
                    dtTable1 = SetEntity<CostInstallationtable1>(table1, d.DISPLAY_VAL, d.LOV_VAL1);
                }
                if (d.LOV_VAL1 == "T2")
                {
                    var table2 = result.CostInstallationtable2;
                    dtTable2 = SetEntity<CostInstallationtable2>(table2, d.DISPLAY_VAL, d.LOV_VAL1);
                }
                if (d.LOV_VAL1 == "T3")
                {
                    var table3 = result.CostInstallationtable3;
                    dtTable3 = SetEntity<CostInstallationtable3>(table3, d.DISPLAY_VAL, d.LOV_VAL1);
                }
                if (d.LOV_VAL1 == "T4")
                {
                    var table4 = result.CostInstallationtable4;
                    dtTable4 = SetEntity<CostInstallationtable4>(table4, d.DISPLAY_VAL, d.LOV_VAL1);

                }
                if (d.LOV_VAL1 == "T5")
                {
                    var table5 = result.CostInstallationtable5;
                    dtTable5 = SetEntity<CostInstallationtable5>(table5, d.DISPLAY_VAL, d.LOV_VAL1);
                }
                if (d.LOV_VAL1 == "T6")
                {
                    var table6 = result.CostInstallationtable6;
                    dtTable6 = SetEntity<CostInstallationtable6>(table6, d.DISPLAY_VAL, d.LOV_VAL1);
                }
            }

            ds.Tables.Clear();
            ds.Tables.Add(dtTable1);
            ds.Tables.Add(dtTable2);
            ds.Tables.Add(dtTable3);
            ds.Tables.Add(dtTable4);
            ds.Tables.Add(dtTable5);
            ds.Tables.Add(dtTable6);
            string tempPath = System.IO.Path.GetTempPath();
            var data_ = GenerateALLExcel(ds, "ConfigCostIns", tempPath);
            return data_;
        }

        private byte[] GenerateALLExcel(DataSet dataToExcel, string fileName, string directoryPath)
        {
            _Logger.Info("GenerateCostInstallationExcel start");
            if (System.IO.File.Exists($"{directoryPath}\\{fileName}.xls"))
            { System.IO.File.Delete($"{directoryPath}\\{fileName}.xls"); }

            ////string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ////ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow = 2;
            int iHeaderRow = 0;
            string strRow = iRow.ToSafeString();
            ////string strHeader = iHeaderRow.ToSafeString();
            ////string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            ////string strColumn2 = string.Empty;
            ////int iCol = 11;
            int i = 0;
            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {

                var data = SelectFbbCfgLov("CONFIG_COST_INS").Where(d => d.LOV_VAL1 != null).ToList();

                foreach (var d in data)
                {
                    //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(d.DISPLAY_VAL);
                    //// ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("AA");

                    worksheet.Cells["A2:G2"].Merge = true;
                    worksheet.Cells["A2,G2"].LoadFromText(d.DISPLAY_VAL);
                    //// worksheet.Cells["A3:I3"].Merge = true;
                    //// worksheet.Cells["A3,I3"].LoadFromText("");
                    rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                    rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                    rangeReportDetail.Style.Font.Bold = true;
                    rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                    iRow = 5;
                    iHeaderRow = iRow + 1;
                    strRow = iRow.ToSafeString();
                    int iCol = 20;
                    rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                    rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                    rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.View.FreezePanes(iHeaderRow, 1);
                    strColumn1 = string.Format("A{0}", strRow);
                    ////  strColumn2 = string.Format("A{0}", strRow);
                    //Step 3 : Start loading datatable form A1 cell of worksheet.

                    worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel.Tables[i], true, TableStyles.None);
                    i++;
                    //Step 4 : (Optional) Set the file properties like title, author and subject
                    package.Workbook.Properties.Title = @"FBB Config";
                    package.Workbook.Properties.Author = "FBB";
                    package.Workbook.Properties.Subject = @"" + d.LOV_VAL1.ToSafeString();

                }
                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] datas = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return datas;
            }
        }

        public ActionResult ExportExcel([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            string filename = string.Empty;
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            string _TableName = string.Empty;
            string tempPath = System.IO.Path.GetTempPath();
            var PageSize = SelectFbbCfgLov("PAGE_SIZE").FirstOrDefault(s => s.LOV_VAL1 != null);
            var searchEventModel = new JavaScriptSerializer().Deserialize<CostInstallationtable>(dataS);
            searchEventModel.PAGE_INDEX = "1";
            searchEventModel.PAGE_SIZE = PageSize != null ? PageSize.LOV_VAL1 : "9999";
            var result = GetCostInstallationTable(searchEventModel);
            string TBNAME = searchEventModel.TB_NAME.ToSafeString();
            if (TBNAME == "T1")
            {
                //// var bytes = GeneratEntitytoExcel<CostInstallationtable1>(result.CostInstallationtable1, filename, TBNAME);

                _TableName = "Table1";
                filename = "ConfigCostTable1";
                var table = SetEntity<CostInstallationtable1>(result.CostInstallationtable1, filename, TBNAME);
                var bytes = GenerateCostInstallationExcel(table, "WorkSheet", tempPath, filename, _TableName, 20);
                return File(bytes, "application/excel", filename + ".xls");
            }
            if (TBNAME == "T2")
            {
                _TableName = "Table2";
                filename = "ConfigCostTable2";
                var table = SetEntity<CostInstallationtable2>(result.CostInstallationtable2, filename, TBNAME);
                var bytes = GenerateCostInstallationExcel(table, "WorkSheet", tempPath, filename, _TableName, 20);
                return File(bytes, "application/excel", filename + ".xls");
            }
            if (TBNAME == "T3")
            {
                _TableName = "Table3";
                filename = "ConfigCostTable3";
                var table = SetEntity<CostInstallationtable3>(result.CostInstallationtable3, filename, TBNAME);
                var bytes = GenerateCostInstallationExcel(table, "WorkSheet", tempPath, filename, _TableName, 20);
                return File(bytes, "application/excel", filename + ".xls");
            }
            if (TBNAME == "T4")
            {
                _TableName = "Table4";
                filename = "ConfigCostTable4";
                var table = SetEntity<CostInstallationtable4>(result.CostInstallationtable4, filename, TBNAME);
                var bytes = GenerateCostInstallationExcel(table, "WorkSheet", tempPath, filename, _TableName, 20);
                return File(bytes, "application/excel", filename + ".xls");
            }
            if (TBNAME == "T5")
            {
                _TableName = "Table5";
                filename = "ConfigCostTable5";
                var table = SetEntity<CostInstallationtable5>(result.CostInstallationtable5, filename, TBNAME);
                var bytes = GenerateCostInstallationExcel(table, "WorkSheet", tempPath, filename, _TableName, 20);
                return File(bytes, "application/excel", filename + ".xls");
            }
            if (TBNAME == "T6")
            {
                _TableName = "Table6";
                filename = "ConfigCostTable6";
                var table = SetEntity<CostInstallationtable6>(result.CostInstallationtable6, filename, TBNAME);
                var bytes = GenerateCostInstallationExcel(table, "WorkSheet", tempPath, filename, _TableName, 20);
                return File(bytes, "application/excel", filename + ".xls");
            }
            else
            {
                return null;
            }


        }

        private byte[] GenerateCostInstallationExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string TableName, int iCol)
        {
            _Logger.Info("GenerateCostInstallationExcel start");
            if (System.IO.File.Exists($"{directoryPath}\\{fileName}.xls"))
            { System.IO.File.Delete($"{directoryPath}\\{fileName}.xls"); }

            ////string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ////ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow = 2;
            int iHeaderRow = 0;
            string strRow = iRow.ToSafeString();
            ////string strHeader = iHeaderRow.ToSafeString();
            ////string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            ////string strColumn2 = string.Empty;
            ////int iCol = 11;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {

                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);
                //// ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("AA");

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(TableName);
                //// worksheet.Cells["A3:I3"].Merge = true;
                //// worksheet.Cells["A3,I3"].LoadFromText("");
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
                ////  strColumn2 = string.Format("A{0}", strRow);
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




        #endregion

    }
}
