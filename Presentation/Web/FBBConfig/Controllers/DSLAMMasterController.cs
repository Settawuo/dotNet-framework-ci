using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.DSLAMMasterPage;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class DSLAMMasterController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CreateDSLAMMasterCommand> _createDSLAMMasterCommand;
        private readonly ICommandHandler<DeleteDSLAMMasterCommand> _deleteDSLAMMasterCommand;

        public DSLAMMasterController(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<CreateDSLAMMasterCommand> createDSLAMMasterCommand,
            ICommandHandler<DeleteDSLAMMasterCommand> deleteDSLAMMasterCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _createDSLAMMasterCommand = createDSLAMMasterCommand;
            _deleteDSLAMMasterCommand = deleteDSLAMMasterCommand;
        }

        // GET: /DSLAMMaster/
        [AuthorizeUserAttribute]
        public ActionResult Index(string saveFlag = "")
        {
            Session["SaveFlag"] = saveFlag;

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            return View();
        }

        public JsonResult ReadDSLAMMaster([DataSourceRequest] DataSourceRequest request)
        {
            var query = new GetDSLAMMasterQuery
            {
            };
            var data = _queryProcessor.Execute(query);
            return Json(data.ToDataSourceResult(request));
        }

        public JsonResult GetDSLAMModel()
        {
            var query = new GetDSLAMModelQuery { };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCardModel()
        {
            var query = new GetCardModelQuery { };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateDSLAMMaster(string data)
        {
            try
            {
                DSLAMMasterModel model = new JavaScriptSerializer().Deserialize<DSLAMMasterModel>(data);
                model.Username = base.CurrentUser.UserName;

                var command = new CreateDSLAMMasterCommand
                {
                    DSLAMMasterModel = model
                };
                _createDSLAMMasterCommand.Handle(command);

                if (command.FlagDup == true)
                    return Json("dup", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetModelMaxSlot(string model)
        {
            var query = new GetModelMaxSlotQuery
            {
                Model = model
            };
            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDSLAMMaster(string lot, string regionCode)
        {
            try
            {
                var command = new DeleteDSLAMMasterCommand
                {
                    Lot = lot,
                    RegionCode = regionCode,
                    Username = base.CurrentUser.UserName
                };
                _deleteDSLAMMasterCommand.Handle(command);

                if (command.FlagNot == true)
                    return Json("not", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDeleteMin(string lot, string regionCode)
        {
            try
            {
                var query = new GetDeleteMinQuery
                {
                    Lot = lot,
                    RegionCode = regionCode
                };

                var data = _queryProcessor.Execute(query);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateDSLAMMaster(string lot, string regionCode, decimal oldNo, decimal newNo)
        {
            try
            {
                decimal diff = newNo - oldNo;
                if (diff > 0)
                {
                    var model = new DSLAMMasterModel
                    {
                        Username = base.CurrentUser.UserName,
                        Region = regionCode,
                        Lot = lot,
                        DSLAMNo = diff,
                    };

                    var command = new CreateDSLAMMasterCommand
                    {
                        DSLAMMasterModel = model,
                        FlagUpdate = true,
                        OldNo = oldNo,
                        NewNo = newNo
                    };
                    _createDSLAMMasterCommand.Handle(command);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else if (diff < 0)
                {
                    var command = new DeleteDSLAMMasterCommand
                    {
                        Lot = lot,
                        RegionCode = regionCode,
                        FlagUpdate = true,
                        Loop = Math.Abs(diff),
                        OldNo = oldNo,
                        NewNo = newNo,
                        Username = base.CurrentUser.UserName
                    };
                    _deleteDSLAMMasterCommand.Handle(command);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }

        }

    }
}
