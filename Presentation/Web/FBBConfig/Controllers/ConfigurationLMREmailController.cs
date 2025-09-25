using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace FBBConfig.Controllers
{
    public class ConfigurationLMREmailController : FBBConfigController
    {

        private readonly ICommandHandler<ConfigurationLMREmailCommand> _ConfigurationLMREmailCommand;
        private readonly IQueryProcessor _queryProcessor;
        public ConfigurationLMREmailController(
             ILogger logger
            , IQueryProcessor queryProcessor
            , ICommandHandler<ConfigurationLMREmailCommand> ConfigurationLMREmailCommand

          )
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _ConfigurationLMREmailCommand = ConfigurationLMREmailCommand;
        }
        //
        // GET: /ConfigurationLMREmail/

        public ActionResult Index()
        {

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            ViewBag.UserGroupCHK = GetUserGroup();

            return View();
        }

        public ActionResult DisplayConfigList([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            try
            {

                var data = SelectFbbCfgLovLMR("MAIL_CONFIG", "FIXED_LASTMILE");

                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.Update_date_text = item.Update_date.ToDisplayText();
                    }

                    return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

                }

                return null;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

        private List<ConfigurationLMREmailModel> SelectFbbCfgLovLMR(string lov_type, string lov_val5)
        {
            var query = new GetLovConfigLMRQuery
            {
                LovType = lov_type,
                LovValue5 = lov_val5

            };
            return _queryProcessor.Execute(query);
        }

        public ActionResult UpdateLovLMREmail([DataSourceRequest] DataSourceRequest request, string Text, string id)
        {
            string msg = "";
            int number = 0;
            int IdLov = 0;
            try
            {
                if (null == base.CurrentUser)
                    return RedirectToAction("Logout", "Account");


                bool result = Int32.TryParse(id, out number);

                if (result)
                {
                    IdLov = Int32.Parse(id);
                }

                var command = new ConfigurationLMREmailCommand()
                {
                    updated_by = CurrentUser.UserName,
                    Text = Text,
                    Id = IdLov
                };
                _ConfigurationLMREmailCommand.Handle(command);
                if (command.ret_code == "0")
                {
                    msg = "Success. ";

                }
                else
                {
                    msg = "ERROR.";

                }
                return Json(
                   new
                   {
                       Code = command.ret_code,
                       message = msg,
                   }, JsonRequestBehavior.AllowGet
                   );
            }
            catch (Exception ex)
            {
                return Json(
                  new
                  {
                      Code = "-1",
                      message = "ERROR.",
                  }, JsonRequestBehavior.AllowGet
                  );
            }
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
    }
}
