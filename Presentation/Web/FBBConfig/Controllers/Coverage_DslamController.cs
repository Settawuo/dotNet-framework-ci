using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class Coverage_DslamController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CoverageDSLAMBuildingCommand> _command;
        private readonly ICommandHandler<DSLAMRetockCommand> _restockCommand;
        //
        // GET: /Coverage_Dslam/
        public Coverage_DslamController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<CoverageDSLAMBuildingCommand> command,
            ICommandHandler<DSLAMRetockCommand> restockCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _command = command;
            _restockCommand = restockCommand;
        }

        [AuthorizeUserAttribute]
        public ActionResult Index(decimal cvrId = 0, string nodeNameTH = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            ViewBag.NodeNameTH = nodeNameTH;

            var q = new GetCoverageAreaByCVRQuery
            {
                CVRID = cvrId
            };

            var model = _queryProcessor.Execute(q);
            return View(model);
        }

        public JsonResult Read([DataSourceRequest] DataSourceRequest request, decimal cvrId)
        {
            var query = new GetDSLAMInfoQuery
            {
                CVRID = cvrId
            };

            var data = _queryProcessor.Execute(query);
            return Json(data.ToDataSourceResult(request));
        }

        public JsonResult GetIPRANPort(string mc)
        {
            var data = new List<DropdownModel>();
            foreach (var a in mc.Split(','))
            {
                data.Add(new DropdownModel { Text = a, Value = a });
            }

            data.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDSLAMMaster(string regionCode)
        {
            var query = new GetDSLAMAndExistingQuery
            {
                Existing = false,
                RegionCode = regionCode
            };

            var data = _queryProcessor.Execute(query);
            data.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExistingDSLAM(decimal cvrId)
        {
            var query = new GetDSLAMAndExistingQuery
            {
                Existing = true,
                CVRID = cvrId
            };

            var data = _queryProcessor.Execute(query);
            data.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModelAndBrand(string regionCode, string lotNo)
        {
            var query = new GetModelAndBrandQuery
            {
                RegionCode = regionCode,
                LotNo = lotNo
            };

            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CoverageDSLAMBuildingCommand(string action, decimal cvrId = 0, decimal dslamId = 0, string type = "", string multiBuilding = "",
             string nodeId = "", decimal dslamNo = 0, decimal contactId = 0, decimal cvrRelationId = -1, string nodeNameTH = "", string buildingCode = "")
        {
            try
            {
                var command = new CoverageDSLAMBuildingCommand
                {
                    Action = action,

                    ContactID = contactId,

                    CVRID = cvrId,
                    DSLAMID = dslamId,
                    Type = type,
                    BuildingUse = multiBuilding,
                    NodeId = nodeId,
                    DSLAMNo = dslamNo,

                    CVRRelationID = cvrRelationId,
                    NodeNameTH = nodeNameTH,
                    BuildingCode = buildingCode,

                    Username = base.CurrentUser.UserName
                };

                _command.Handle(command);

                if (command.FlagDup != "" && command.FlagDup != null)
                {
                    return Json(command.FlagDup.Substring(0, command.FlagDup.Length - 2), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBuilding(decimal contactId = 0, bool label = true)
        {
            var query = new GetCoverageAreaBuildingQuery
            {
                ContactId = contactId,
                NotIn = false
            };

            var data = _queryProcessor.Execute(query);
            if (label)
                data.Insert(0, new BuildingPanel { Text = "กรุณาเลือก", Value = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaxDL(decimal cvrId)
        {
            var query = new GetMaxDLQuery
            {
                CVRID = cvrId
            };

            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadGridRestock([DataSourceRequest] DataSourceRequest request, decimal cvrId)
        {
            var query = new GetGridDSLAMRestockQuery
            {
                CVRID = cvrId
            };

            var data = _queryProcessor.Execute(query);
            return Json(data.ToDataSourceResult(request));
        }

        public JsonResult DSLAMRetockCommand(string action = "", decimal dslamId = 0, string nodeId = "", string nodeTH = "")
        {
            try
            {
                var command = new DSLAMRetockCommand
                {
                    Action = action,
                    DSLAMID = dslamId,
                    Username = base.CurrentUser.UserName,
                    NodeID = nodeId,
                    NodeTH = nodeTH
                };
                _restockCommand.Handle(command);

                if (command.FlagNot == true)
                    return Json("N", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
