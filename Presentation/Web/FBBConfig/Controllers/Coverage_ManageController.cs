using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class Coverage_ManageController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CoverageAreaBuildingCommand> _coverageAreaBuildingCommand;
        private readonly ICommandHandler<CoverageAreaCommand> _coverageAreaCommand;
        //
        // GET: /Coverage_Search/
        public Coverage_ManageController(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<CoverageAreaBuildingCommand> coverageAreaBuildingCommand,
            ICommandHandler<CoverageAreaCommand> coverageAreaCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _coverageAreaBuildingCommand = coverageAreaBuildingCommand;
            _coverageAreaCommand = coverageAreaCommand;
        }

        [AuthorizeUserAttribute]
        public ActionResult Index(decimal contactId = 0)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            var a = GetDataForEdit(contactId, true);

            return View(a);
        }

        private CoverageSitePanelModel GetDataForEdit(decimal contactId, bool typeForGet)
        {
            var query = new GetEditCoverageAreaQuery
            {
                ContactId = contactId,
                GetForEdit = typeForGet
            };

            return _queryProcessor.Execute(query);
        }

        public JsonResult ReadBuilding(decimal contactId)
        {
            var query = new GetCoverageAreaBuildingQuery
            {
                ContactId = contactId,
                NotIn = false
            };

            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadBuildingToGrid([DataSourceRequest] DataSourceRequest request, decimal contactId)
        {
            var query = new GetCoverageAreaBuildingQuery
            {
                ContactId = contactId,
                NotIn = false
            };

            var data = _queryProcessor.Execute(query);
            return Json(data.ToDataSourceResult(request));
        }

        public JsonResult SaveBuilding(string action, string building, string buildingTH, string buildingEN, string installNote, decimal contactId, string refKey)
        {
            var command = new CoverageAreaBuildingCommand()
            {
                BUILDING = building,
                BUILDING_EN = buildingEN,
                BUILDING_TH = buildingTH,
                INSTALLNOTE = installNote,
                CONTACT_ID = contactId,
                ActionType = action == "Create" ? ActionType.Insert : ActionType.Update,
                RefKey = refKey,
                CREATED_BY = base.CurrentUser.UserName
            };

            _coverageAreaBuildingCommand.Handle(command);

            return Json(command, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteBuilding(decimal contactId, string building, string refKey)
        {
            var command = new CoverageAreaBuildingCommand
            {
                CONTACT_ID = contactId,
                BUILDING = building,
                ActionType = ActionType.Delete,
                CREATED_BY = base.CurrentUser.UserName,
                RefKey = refKey
            };

            _coverageAreaBuildingCommand.Handle(command);

            return Json(command, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadCoverageArea(decimal contactId)
        {
            return Json(GetDataForEdit(contactId, false), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadCoverageAreaToGrid([DataSourceRequest] DataSourceRequest request, decimal contactId)
        {
            var data = GetDataForEdit(contactId, false).CoverageAreaPanel;
            return Json(data.ToDataSourceResult(request));
        }

        public JsonResult SaveCoverageAreaInformation(decimal contactId, string building, string status, bool tieFlag)
        {
            var coverage = new CoverageAreaPanel();
            coverage.ContactId = contactId;
            coverage.BuildingCode = building;
            coverage.CreateBy = base.CurrentUser.UserName;
            coverage.TieFlag = tieFlag;
            coverage.Status = status;

            var command = new CoverageAreaCommand()
            {
                ActionType = ActionType.Insert,
                UpdateCoverageType = UpdateCoverageType.None,
                FlagDelectAll = false,
                CoverageAreaPanel = coverage
            };

            _coverageAreaCommand.Handle(command);

            return Json(command, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCovarageAreaInformation(string cvrId, decimal contactId, string building, string status, string onTargetDateIn, string onTargetDateEx, bool completeFlag, bool tieFlag)
        {
            var coverage = new CoverageAreaPanel();
            coverage.CVRId = string.IsNullOrEmpty(cvrId) ? 0 : Convert.ToDecimal(cvrId);
            coverage.ContactId = contactId;
            coverage.BuildingCode = building;
            coverage.CreateBy = base.CurrentUser.UserName;

            if (onTargetDateIn != "")
                coverage.OnTargetDateIn = DateTime.ParseExact(onTargetDateIn, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;

            if (onTargetDateEx != "")
                coverage.OnTargetDateEx = DateTime.ParseExact(onTargetDateEx, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;

            coverage.ConfigComplete = completeFlag;
            coverage.TieFlag = tieFlag;
            coverage.Status = status;

            var command = new CoverageAreaCommand()
            {
                ActionType = ActionType.Update,
                UpdateCoverageType = UpdateCoverageType.CoverageInformation,
                FlagDelectAll = false,
                CoverageAreaPanel = coverage
            };

            _coverageAreaCommand.Handle(command);

            return Json(command, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCoverageAreaInformation(string cvrId, decimal contactId, string status)
        {
            var coverage = new CoverageAreaPanel();
            coverage.CVRId = string.IsNullOrEmpty(cvrId) ? 0 : Convert.ToDecimal(cvrId);
            coverage.ContactId = contactId;
            coverage.CreateBy = base.CurrentUser.UserName;
            coverage.Status = status;

            var command = new CoverageAreaCommand()
            {
                ActionType = ActionType.Delete,
                UpdateCoverageType = UpdateCoverageType.None,
                FlagDelectAll = false,
                CoverageAreaPanel = coverage
            };

            _coverageAreaCommand.Handle(command);

            return Json(command, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCoverageArea(CoverageAreaPanel coverage)
        {
            coverage.CreateBy = base.CurrentUser.UserName;

            var latDecimal = Convert.ToDecimal(coverage.Lat);
            var longDecimal = Convert.ToDecimal(coverage.Long);
            coverage.Lat = String.Format("{0:0.000000}", latDecimal);
            coverage.Long = String.Format("{0:0.000000}", longDecimal);

            var command = new CoverageAreaCommand()
            {
                ActionType = ActionType.Insert,
                UpdateCoverageType = UpdateCoverageType.None,
                FlagDelectAll = false,
                CoverageAreaPanel = coverage
            };

            _coverageAreaCommand.Handle(command);
            return Json(command);
        }

        [HttpPost]
        public JsonResult UpdateCoverageArea(CoverageAreaPanel coverage)
        {
            coverage.CreateBy = base.CurrentUser.UserName;

            var latDecimal = Convert.ToDecimal(coverage.Lat);
            var longDecimal = Convert.ToDecimal(coverage.Long);
            coverage.Lat = String.Format("{0:0.000000}", latDecimal);
            coverage.Long = String.Format("{0:0.000000}", longDecimal);

            var command = new CoverageAreaCommand()
            {
                ActionType = ActionType.Update,
                UpdateCoverageType = (UpdateCoverageType)coverage.UpdateCoverageType,
                FlagDelectAll = false,
                CoverageAreaPanel = coverage
            };

            _coverageAreaCommand.Handle(command);
            return Json(command);
        }

        public JsonResult DeleteCoverageArea(decimal contactId)
        {
            var coverage = new CoverageAreaPanel();
            coverage.ContactId = contactId;
            coverage.CreateBy = base.CurrentUser.UserName;

            var command = new CoverageAreaCommand()
            {
                ActionType = ActionType.Delete,
                UpdateCoverageType = UpdateCoverageType.None,
                FlagDelectAll = true,
                CoverageAreaPanel = coverage
            };

            _coverageAreaCommand.Handle(command);
            return Json(command);
        }

        public JsonResult GetBuilding(decimal contactId = 0, bool label = true)
        {
            var query = new GetCoverageAreaBuildingQuery
            {
                ContactId = contactId,
                NotIn = true
            };

            var data = _queryProcessor.Execute(query);
            if (label)
                data.Insert(0, new BuildingPanel { Text = "กรุณาเลือก", Value = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectCoverageStatus(string type, string status, bool label = true)
        {
            var query = new SelectCoverageStatusQuery
            {
                LOV_TYPE = type,
                Status = status
            };

            var data = _queryProcessor.Execute(query);
            if (label)
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCompleteFlag(string cvrId)
        {
            var query = new GetCheckCompleteFlagQuery()
            {
                CVRId = string.IsNullOrEmpty(cvrId) ? 0 : Convert.ToDecimal(cvrId)
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
