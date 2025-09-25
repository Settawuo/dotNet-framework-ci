using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace FBBConfig.Controllers
{
    public class ConfigurationSubContractEmailInfoController : FBBConfigController
    {
        private readonly ICommandHandler<UpdateSubContractEmailInfoCommand> _ICommandConfigurationSubContractmailCommand;

        private readonly IQueryProcessor _queryProcessor;
        public ConfigurationSubContractEmailInfoController(
             ILogger logger
            , IQueryProcessor queryProcessor
            , ICommandHandler<UpdateSubContractEmailInfoCommand> IComConfigurationSubContractmailCommand

          )
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _ICommandConfigurationSubContractmailCommand = IComConfigurationSubContractmailCommand;
        }

        public ActionResult Index()
        {

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = CurrentUser;
            ViewBag.UserGroupCHK = GetUserGroup();

            var query = new GetLovV2Query()
            {
                LovType = "FBB_SUBCONTRACT_SCREEN",
                LovVal5 = "FBB_SUBCONTRACT_EMAIL_INFO"
            };
            ViewBag.ListScreen = _queryProcessor.Execute(query).ToList();
            //ViewBag.ListScreen = base.LovData.Where(p => p.Type == "FBB_SUBCONTRACT_SCREEN" && p.LovValue5 == "FBB_SUBCONTRACT_EMAIL_INFO").ToList();
            return View();
        }

        public ActionResult getConfigurationSubContractEmailInfo([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {

            try
            {
                if (null == base.CurrentUser)
                    return RedirectToAction("Logout", "Account");


                var search = new JavaScriptSerializer().Deserialize<SearchSubContractEmailInfoModel>(dataS);
                string p_subcontract_name = search.subcontract_name == "" ? "ALL" : search.subcontract_name;
                string p_subcontract_code = search.subcontract_code == "" ? "ALL" : search.subcontract_code;
                string p_storage = search.storage == "" ? "ALL" : search.storage;
                string p_Action_flag = search.action_flag == "" ? "ALL" : search.action_flag;
                var query = new GetSubContractEmailInfoQuery()
                {
                    p_subcontract_name = p_subcontract_name,
                    p_subcontract_code = p_subcontract_code,
                    p_storage = p_storage,
                    p_Action_flag = p_Action_flag
                };
                var result = _queryProcessor.Execute(query);

                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        item.MODIFY_DT_TEXT = item.MODIFY_DT.ToDisplayText();
                        item.CREATE_DT_TEXT = item.CREATE_DT.ToDisplayText();
                    }

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        public ActionResult ConfirmUpdateSubContractEmailInfo(
            string rowid, string sub_contrac_email, string sub_contrac_for_mail, string phase
            )
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");


            var ConfigurationSubContractmailCommand = new UpdateSubContractEmailInfoCommand
            {
                p_row_id = rowid,
                p_subcontrac_email = sub_contrac_email,
                p_subcontract_for_email = sub_contrac_for_mail,
                p_phase = phase,

            };
            _ICommandConfigurationSubContractmailCommand.Handle(ConfigurationSubContractmailCommand);
            string message = "";
            string Code = "";
            if (ConfigurationSubContractmailCommand.ret_code == "0")
            {
                message = "Update Success.";
                Code = "0";
            }
            else
            {
                message = "update not success.";
                Code = "-1";
            }
            return Json(new
            {
                Code = Code,
                message = message
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SelectSubContractorCode(string text)
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
        public JsonResult SelectSubContractorName(string text)
        {
            var query = new SelectSubContractorNameQuery
            {
                p_name = "",
                p_code = "",
                p_code_distinct = false
            };
            var data = _queryProcessor.Execute(query);
            data = data.Where(p => p.SUB_CONTRACTOR_NAME.Contains(text)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getStorage(string text)
        {
            var resule = new GetStorageSubContractWfmQuery
            {
                p_storage_location = text
            };
            var data = _queryProcessor.Execute(resule);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetDDLActionFlag()
        {

            var data = SelectFbbCfgLov("FBB_SUBCONTRACT_SCREEN", "FBB_SUBCONTRACT_EMAIL_INFO");
            data = data.Where(x => x.LOV_VAL3 == "DDL_FLAG").ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
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

        private List<LovModel> SelectFbbCfgLov(string lov_type, string lov_val5)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = lov_type,
                LOV_VAL5 = lov_val5
            };
            return _queryProcessor.Execute(query);
        }
    }
}
