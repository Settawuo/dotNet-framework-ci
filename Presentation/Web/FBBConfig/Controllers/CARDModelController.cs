using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.Commons.Master;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class CARDModelController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveCardModelCommand> _COMMAND_CARDMODEL;

        public CARDModelController(ILogger logger,
             IQueryProcessor queryProcessor,
            ICommandHandler<SaveCardModelCommand> COMMAND_CARDMODEL)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _COMMAND_CARDMODEL = COMMAND_CARDMODEL;
        }

        // GET: /CARDModel/
        [AuthorizeUserAttribute]
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            return View();
        }

        public JsonResult ReadCardModel([DataSourceRequest] DataSourceRequest request)
        {
            var query = new GetCardModelQuery();

            var result = _queryProcessor.Execute(query).OrderByDescending(x => x.CARDMODELID);

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult Editing_Popup()
        {
            return View();
        }

        public JsonResult Add_CardModel(string ResultCommand, string Model, string Band, decimal maxport,
        decimal RESERVEPORTSPARE, decimal PORTSTARTINDEX, string DATAONLY_FLAG, decimal CARDMODELID)
        {
            Session.Remove("returnDesc");
            Session.Remove("Return_Code");

            var CREATED_DATE = DateTime.Now.Date;

            var SaveGradModelCommand = new SaveCardModelCommand
            {

                CARDMODELID = CARDMODELID,
                CREATED_BY = base.CurrentUser.UserName.ToString(),
                CREATED_DATE = CREATED_DATE,
                UPDATED_BY = base.CurrentUser.UserName.ToString(),
                UPDATED_DATE = CREATED_DATE,
                MODEL = Model,
                BRAND = Band,
                POSTSTARTINDEX = PORTSTARTINDEX,

                MAXSLOT = maxport,
                //RESERVE = RESERVEPORTSPARE,
                RESERVEPORTSPARE = RESERVEPORTSPARE == 0 ? 1 : RESERVEPORTSPARE,
                DATAONLY_FLANG = DATAONLY_FLAG ?? "N",
                ACTIVEFLAG = "Y",
                ResultCommand = ResultCommand
            };

            try
            {

                if (SaveGradModelCommand.ResultCommand == "ADD")
                {
                    _COMMAND_CARDMODEL.Handle(SaveGradModelCommand);


                }
                else if (SaveGradModelCommand.ResultCommand == "UPDATE")
                {

                    _COMMAND_CARDMODEL.Handle(SaveGradModelCommand);

                }
                else if (SaveGradModelCommand.ResultCommand == "DELETE")
                {

                    _COMMAND_CARDMODEL.Handle(SaveGradModelCommand);

                }

            }

            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            Session["returnDesc"] = SaveGradModelCommand.Return_Desc;
            Session["Return_Code"] = SaveGradModelCommand.Return_Code;
            return Json(SaveGradModelCommand.ResultCommand, JsonRequestBehavior.AllowGet);

        }

        public JsonResult setdataTransition()
        {
            string resultDesc = "";
            string returnCode = "";

            resultDesc = Session["returnDesc"].ToString();
            returnCode = Session["Return_Code"].ToString();

            var listStrt = new List<string>();
            listStrt.Add(resultDesc);
            listStrt.Add(returnCode);

            return Json(listStrt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsUID_Available(string modelname = "", string Falg = "")
        {

            var IsUID_Available_user = new GetModelNameQuery
            {
                ModelName = modelname,
                ResultModel = "User"
            };

            var result = _queryProcessor.Execute(IsUID_Available_user);


            return Json(IsUID_Available_user.ResultBI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CARDModel()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var query = new GetUserDataQuery
            {
                UserName = "thitimaw"
            };

            base.CurrentUser = _queryProcessor.Execute(query);
            ViewBag.User = base.CurrentUser;
            return View();
        }

    }
}
