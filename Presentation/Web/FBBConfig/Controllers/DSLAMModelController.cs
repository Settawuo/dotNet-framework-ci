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
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Master;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class DSLAMModelController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<DSLAMModelCommand> _COMMAND_DSLAMMODEL;

        public DSLAMModelController(ILogger logger,
             IQueryProcessor queryProcessor,
             ICommandHandler<DSLAMModelCommand> COMMAND_DSLAMMODEL)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _COMMAND_DSLAMMODEL = COMMAND_DSLAMMODEL;
        }

        // GET: /DSLAMModel/

        public JsonResult ReadDSLAMModel([DataSourceRequest] DataSourceRequest request) //Read data
        {
            var query = new GetDSLAMModelQuery();
            {
                //UserName = base.CurrentUser.UserName
            };
            var result = _queryProcessor.Execute(query).OrderByDescending(s => s.DSLAMMODELID); //Result + Sorting Data from new to old.

            return Json(result.ToDataSourceResult(request));
        }

        [AuthorizeUserAttribute]
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            return View();
        }

        public ActionResult Editing_Popup()
        {
            return View();
        }

        //public ActionResult EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    return null;
        //   //return Json(productService.Read().ToDataSourceResult(request));
        //}

        public JsonResult Add_DSALAMModel(string ResultCommand, string Model, string Band, string SH_BRAND,
        decimal StartIndex222, decimal MaxSlot, decimal DSLAMMODELID)
        {
            Session.Remove("returnDesc");
            Session.Remove("Return_Code");

            var CREATED_DATE = DateTime.Now.Date;

            var SaveDSLAMModelCommand = new DSLAMModelCommand
            {
                DSLAMMODELID = DSLAMMODELID,
                CREATED_BY = base.CurrentUser.UserName.ToString(),
                CREATED_DATE = CREATED_DATE,
                UPDATED_BY = base.CurrentUser.UserName.ToString(),
                UPDATED_DATE = CREATED_DATE,
                MODEL = Model,
                BRAND = Band,
                SLOTSTARTINDEX = StartIndex222,
                SH_BRAND = SH_BRAND,
                MAXSLOT = MaxSlot == 0 ? 1 : MaxSlot,
                ACTIVEFLAG = "Y",
                ResultCommand = ResultCommand

            };

            try
            {

                if (SaveDSLAMModelCommand.ResultCommand == "ADD")
                {
                    _COMMAND_DSLAMMODEL.Handle(SaveDSLAMModelCommand);

                }
                else if (SaveDSLAMModelCommand.ResultCommand == "UPDATE")
                {

                    _COMMAND_DSLAMMODEL.Handle(SaveDSLAMModelCommand);

                }
                else if (SaveDSLAMModelCommand.ResultCommand == "DELETE")
                {

                    _COMMAND_DSLAMMODEL.Handle(SaveDSLAMModelCommand);

                }

            }

            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            Session["returnDesc"] = SaveDSLAMModelCommand.Return_Desc;
            Session["Return_Code"] = SaveDSLAMModelCommand.Return_Code;
            return Json(SaveDSLAMModelCommand.ResultCommand, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DataDesc()
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

        private void COMMAND_DSLAMMODEL(DSLAMModelCommand SaveDSLAMModelCommand)
        {
            throw new NotImplementedException();
        }

    }
}
