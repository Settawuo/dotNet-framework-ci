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
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class CoverageManagementController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<DSLAMInfoCommand> _dslaminfo;
        private readonly ICommandHandler<CoverageAreaCommand> _coverageAreaCommand;
        private readonly ICommandHandler<CoverageAreaBuildingCommand> _coverageAreaBuildingCommand;

        public CoverageManagementController(ILogger logger,
                    IQueryProcessor queryProcessor,
                    ICommandHandler<DSLAMInfoCommand> dslaminfo,
                    ICommandHandler<CoverageAreaCommand> coverageAreaCommand,
                    ICommandHandler<CoverageAreaBuildingCommand> coverageAreaBuildingCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _dslaminfo = dslaminfo;
            _coverageAreaCommand = coverageAreaCommand;
            _coverageAreaBuildingCommand = coverageAreaBuildingCommand;
        }

        //[AuthorizeUserAttribute]
        public ActionResult Index()
        {
            var query = new GetUserDataQuery
            {
                UserName = "thitimaw"
            };

            base.CurrentUser = _queryProcessor.Execute(query);

            ViewBag.User = base.CurrentUser;



            var lstDslamModel = GetListDslamModel();
            var lstBuildingCodeModel = GetListBuildingCode();


            DslamPanel dslampanel = new DslamPanel();

            dslampanel.Brand = "";
            if (lstBuildingCodeModel != null) { dslampanel.BuildingCode = lstBuildingCodeModel.FirstOrDefault().BuildingCode; } else { dslampanel.BuildingCode = ""; }
            dslampanel.BuildingUse = "A";
            dslampanel.Code = "1";
            dslampanel.CurrentPort = 1;
            dslampanel.DLRuningNumber = "";
            if (lstDslamModel != null) { dslampanel.DSLAMModel = lstDslamModel.FirstOrDefault().MODEL; } else { dslampanel.DSLAMModel = ""; }
            dslampanel.IPRANPort = "";
            //dslampanel.Lot = "Lot01";
            dslampanel.MC = "";
            dslampanel.NodeId = "";
            dslampanel.Number = 1;
            dslampanel.RegionDSLAM = "";

            ViewBag.DefaultPopup = dslampanel;

            return View();
        }

        #region Grid Coverage


        public JsonResult Read_CoverageSite([DataSourceRequest] DataSourceRequest request, string amphur, string ipranCode, string locationCode, string nodeName, string nodeStatus, string port, string province, string regionCode)
        {
            var query = new WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage.GetCoverageAreaQuery
            {
                Amphur = amphur,
                IpRanCode = ipranCode,
                LocationCode = locationCode,
                NodeName = nodeName,
                NodeStaus = nodeStatus,
                Port = port,
                Province = province,
                RegionCode = regionCode
            };

            var coverageSite = _queryProcessor.Execute(query);
            if (coverageSite.CoverageAreaPanel != null)
            {
                var data = coverageSite.CoverageAreaPanel.ToList().DistinctBy(x => x.ContactId);
                coverageSite.TotalSite = data.Count().ToString();
                coverageSite.CoverageAreaPanel = new List<CoverageAreaPanel>();
                coverageSite.CoverageAreaPanel = (List<CoverageAreaPanel>)data.ToList();
                Session["CoverageSite"] = coverageSite;
                return Json(coverageSite.CoverageAreaPanel.ToDataSourceResult(request));
            }
            else
            {
                return Json("");
            }



        }

        public JsonResult Get_EditCoverageSite(string contactId)
        {
            decimal? cid = contactId.ToSafeDecimal();

            var query = new WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage.GetEditCoverageAreaQuery
            {
                ContactId = cid
            };

            var data = _queryProcessor.Execute(query);
            Session["EditCoverageSite"] = data;
            return Json(data);

        }

        public JsonResult GetCoverageSiteTotal()
        {
            var coverageSite = (CoverageSitePanelModel)Session["CoverageSite"];
            if (coverageSite == null)
            {
                coverageSite = new CoverageSitePanelModel();
            }
            return Json(coverageSite);
        }
        #endregion

        #region Grid Coverage Info
        public ActionResult Read_CoverageInfo([DataSourceRequest] DataSourceRequest request)
        {

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create_CoverageInfo([DataSourceRequest] DataSourceRequest request, CoverageAreaPanel model)
        {
            model.CreateBy = base.CurrentUser.UserName;

            var command = new CoverageAreaCommand
            {
                ActionType = ActionType.Insert,
                UpdateCoverageType = WBBContract.Commands.FBBWebConfigCommands.UpdateCoverageType.CoverageInformation,
                CoverageAreaPanel = model
            };
            _coverageAreaCommand.Handle(command);

            //Session["returnCoverage"] = command.Return_Code.ToString() + ":" + command.Return_Desc.ToString();//0 cannot save , 1 success ,-1 error
            Session["returnCoverage"] = command;
            return Json(command, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update_CoverageInfo([DataSourceRequest] DataSourceRequest request, CoverageAreaPanel model)
        {
            model.CreateBy = base.CurrentUser.UserName;
            var command = new CoverageAreaCommand
            {
                ActionType = ActionType.Update,
                UpdateCoverageType = WBBContract.Commands.FBBWebConfigCommands.UpdateCoverageType.CoverageInformation,
                CoverageAreaPanel = model
            };
            _coverageAreaCommand.Handle(command);

            //Session["returnCoverage"] = command.Return_Code.ToString() + ":" + command.Return_Desc.ToString();//0 cannot save , 1 success ,-1 error
            Session["returnCoverage"] = command;
            return Json(command, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy_CoverageInfo([DataSourceRequest] DataSourceRequest request, CoverageAreaPanel model)
        {
            model.CreateBy = base.CurrentUser.UserName;

            var command = new CoverageAreaCommand
            {
                ActionType = ActionType.Delete,
                UpdateCoverageType = WBBContract.Commands.FBBWebConfigCommands.UpdateCoverageType.CoverageInformation,
                FlagDelectAll = false,
                CoverageAreaPanel = model
            };
            _coverageAreaCommand.Handle(command);

            //Session["returnCoverage"] = command.Return_Code.ToString() + ":" + command.Return_Desc.ToString();//0 cannot save , 1 success ,-1 error
            Session["returnCoverage"] = command;
            return Json(command, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Grid DSLAM
        public ActionResult Read_DSLAM([DataSourceRequest] DataSourceRequest request)
        {

            var query = new WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage.GetDSLAMInfoQuery
            {
                CVRID = 16
            };

            var result = _queryProcessor.Execute(query);

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create_DSLAM([DataSourceRequest] DataSourceRequest request, DslamPanel model)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update_DSLAM([DataSourceRequest] DataSourceRequest request, DslamPanel model)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy_DSLAM([DataSourceRequest] DataSourceRequest request, DslamPanel model)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Grid Building
        public ActionResult Read_Building([DataSourceRequest] DataSourceRequest request, string contactId)
        {
            if (!string.IsNullOrEmpty(contactId))
            {
                var query = new WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage.GetCoverageAreaBuildingQuery
                {
                    ContactId = Convert.ToDecimal(contactId)
                };
                var getBuilding = _queryProcessor.Execute(query);
                return Json(getBuilding, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create_Building([DataSourceRequest] DataSourceRequest request, BuildingPanel model)
        {
            var command = new CoverageAreaBuildingCommand
            {
                ActionType = ActionType.Insert,
                BUILDING = model.Tower,
                BUILDING_EN = model.TowerEN,
                BUILDING_TH = model.TowerTH,
                CONTACT_ID = model.ContactId,
                CREATED_BY = base.CurrentUser.UserName
            };
            _coverageAreaBuildingCommand.Handle(command);

            //Session["returnCoverage"] = command.Return_Code.ToString() + ":" + command.Return_Desc.ToString();//0 cannot save , 1 success ,-1 error
            Session["returnCoverage"] = command;
            return Json(command, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update_Building([DataSourceRequest] DataSourceRequest request, BuildingPanel model)
        {
            var command = new CoverageAreaBuildingCommand
            {
                ActionType = ActionType.Update,
                BUILDING = model.Tower,
                BUILDING_EN = model.TowerEN,
                BUILDING_TH = model.TowerTH,
                CONTACT_ID = model.ContactId,
                CREATED_BY = base.CurrentUser.UserName
            };
            _coverageAreaBuildingCommand.Handle(command);
            return Json(command.Return_Desc);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy_Building([DataSourceRequest] DataSourceRequest request, BuildingPanel model)
        {
            var command = new CoverageAreaBuildingCommand
            {
                ActionType = ActionType.Delete,
                BUILDING = model.Tower,
                BUILDING_EN = model.TowerEN,
                BUILDING_TH = model.TowerTH,
                CONTACT_ID = model.ContactId,
                CREATED_BY = base.CurrentUser.UserName
            };
            _coverageAreaBuildingCommand.Handle(command);
            return Json(command.Return_Desc);
        }

        public ActionResult setdataTransition()
        {
            string result = Session["returnCoverage"].ToString();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Grid Port
        public ActionResult Read_Port([DataSourceRequest] DataSourceRequest request, string id)
        {
            var listPort = new List<PortPanel>();
            var port = new PortPanel();
            port.PortType = "type1" + id;
            port.PortStatus = "available" + id;
            port.PortNumber = 1;
            listPort.Add(port);

            port = new PortPanel();
            port.PortType = "type2" + id;
            port.PortStatus = "available" + id;
            port.PortNumber = 2;
            listPort.Add(port);

            port = new PortPanel();
            port.PortType = "type1" + id;
            port.PortStatus = "available" + id;
            port.PortNumber = 3;
            listPort.Add(port);

            return Json(listPort.ToDataSourceResult(request));
        }
        #endregion

        #region Grid Card
        public ActionResult Read_Card([DataSourceRequest] DataSourceRequest request)
        {
            var listCardPanel = new List<CardPanel>();
            var cardPanel = new CardPanel();

            cardPanel.Number = 1;
            cardPanel.Model = "model";
            cardPanel.CardType = "cardtype";
            cardPanel.Reserve = "reserve";
            cardPanel.NodeId = "nodeid";
            listCardPanel.Add(cardPanel);

            cardPanel = new CardPanel();
            cardPanel.Number = 2;
            cardPanel.Model = "model2";
            cardPanel.CardType = "cardtype2";
            cardPanel.Reserve = "reserve2";
            cardPanel.NodeId = "nodeid2";
            listCardPanel.Add(cardPanel);

            cardPanel = new CardPanel();
            cardPanel.Number = 3;
            cardPanel.Model = "model3";
            cardPanel.CardType = "cardtype3";
            cardPanel.Reserve = "reserve3";
            cardPanel.NodeId = "nodeid3";
            listCardPanel.Add(cardPanel);

            return Json(listCardPanel.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create_Card([DataSourceRequest] DataSourceRequest request, CardPanel model)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update_Card([DataSourceRequest] DataSourceRequest request, CardPanel model)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy_Card([DataSourceRequest] DataSourceRequest request, CardPanel model)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetCoverageStatus(bool showAll, int removeIndex)
        {
            var dropDown = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.CoverageStatus))
                .Select(l => new DropdownModel
                {
                    Text = l.Text,
                    Value = l.Name
                }).ToList();
            if (!showAll)
            {
                dropDown.RemoveAt(removeIndex);
            }


            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNodeType()
        {
            var dropDown = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.CoverageType))
                .Select(l => new DropdownModel
                {
                    Text = l.Text,
                    Value = l.Name
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegionCode()
        {
            var dropDown = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.RegionCode))
                .Select(l => new DropdownModel
                {
                    Text = l.Text,
                    Value = l.Name
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPortUtilization()
        {
            var dropDown = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.PortUtilization))
                .Select(l => new DropdownModel
                {
                    Text = l.Text,
                    Value = l.LovValue1.ToSafeString(),
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDefaultPortUtilization()
        {
            var dropDown = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.PortUtilization) && !string.IsNullOrEmpty(l.DefaultValue) && l.DefaultValue.Equals("Y"))
                .Select(l => new DropdownModel
                {
                    Text = l.Text,
                    Value = l.LovValue1,
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        #region Get Dslam Model
        public List<FBB_DSLAMMODEL> GetListDslamModel()
        {
            var query = new GetDSLAMModelQuery
            {

            };

            var dslam = _queryProcessor.Execute(query).ToList();
            return dslam;
        }

        public JsonResult GetDslamModel()
        {
            var dslam = GetListDslamModel().Select(t => new DropdownModel
            {
                Text = t.MODEL,
                Value = t.DSLAMMODELID.ToSafeString()
            }).ToList();
            return Json(dslam, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Building Code
        public List<CoverageAreaPanel> GetListBuildingCode()
        {
            List<CoverageAreaPanel> lstModel = new List<CoverageAreaPanel>();

            lstModel.Add(new CoverageAreaPanel { BuildingCode = "A" });
            lstModel.Add(new CoverageAreaPanel { BuildingCode = "B" });
            lstModel.Add(new CoverageAreaPanel { BuildingCode = "C" });
            lstModel.Add(new CoverageAreaPanel { BuildingCode = "D" });
            lstModel.Add(new CoverageAreaPanel { BuildingCode = "E" });

            return lstModel;
        }

        public JsonResult GetBuildingCode(string contactId)
        {

            if (!string.IsNullOrEmpty(contactId))
            {
                var query = new WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage.GetCoverageAreaBuildingQuery
                {
                    ContactId = Convert.ToDecimal(contactId)
                };
                var getBuilding = _queryProcessor.Execute(query);
                var data = getBuilding.Select(t => new DropdownModel
                {
                    Text = t.Tower.ToSafeString(),
                    Value = t.Tower.ToSafeString()
                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Get Region & Lot DSLAM
        public List<GridDSLAMModel> GetListRegionDSLAM()
        {

            List<GridDSLAMModel> lstModel = new List<GridDSLAMModel>();

            lstModel.Add(new GridDSLAMModel { Region = "A", LotNo = "Lot01" });
            lstModel.Add(new GridDSLAMModel { Region = "A", LotNo = "Lot02" });
            lstModel.Add(new GridDSLAMModel { Region = "A", LotNo = "Lot03" });
            lstModel.Add(new GridDSLAMModel { Region = "B", LotNo = "Lot01" });
            lstModel.Add(new GridDSLAMModel { Region = "C", LotNo = "Lot01" });

            return lstModel;
        }

        public JsonResult GetRegionDSLAM()
        {

            return Json(GetListRegionDSLAM(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLot(string region)
        {
            //var data = GetListRegionDSLAM().Where(l => l.Region.Equals(region))
            return Json(GetListRegionDSLAM(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Building Use
        public List<BuildingPanel> GetListBuildingUse()
        {
            List<BuildingPanel> lstModel = new List<BuildingPanel>();

            lstModel.Add(new BuildingPanel { Tower = "A", TowerEN = "A", TowerTH = "A" });
            lstModel.Add(new BuildingPanel { Tower = "B", TowerEN = "B", TowerTH = "B" });
            lstModel.Add(new BuildingPanel { Tower = "C", TowerEN = "C", TowerTH = "C" });
            lstModel.Add(new BuildingPanel { Tower = "D", TowerEN = "D", TowerTH = "D" });
            lstModel.Add(new BuildingPanel { Tower = "E", TowerEN = "E", TowerTH = "E" });

            return lstModel;
        }

        public JsonResult GetBuildingUse()
        {
            var data = GetListBuildingUse().Select(t => new DropdownModel
            {
                Text = t.Tower,
                Value = t.Tower.ToSafeString()
            }).ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Card Info
        public List<CardPanel> GetListCardInfo(string dslamId)
        {
            var query = new WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage.GetCardInfoQuery()
            {
                DSLAMID = Convert.ToDecimal(dslamId)
            };

            var cardInfo = _queryProcessor.Execute(query);
            return cardInfo;
        }

        public JsonResult GetCardInfo(string dslamId)
        {
            var data = GetListCardInfo(dslamId).Select(t => new DropdownModel
            {
                Text = t.CardId.ToSafeString(),
                Value = t.Number.ToSafeString()
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public JsonResult SaveCoverageArea(CoverageAreaPanel coverage)
        {
            coverage.CreateBy = base.CurrentUser.UserName;

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

        [HttpPost]
        public JsonResult DeleteCoverageArea(CoverageAreaPanel coverage, bool deleteAll)
        {
            coverage.CreateBy = base.CurrentUser.UserName;

            var command = new CoverageAreaCommand()
            {
                ActionType = ActionType.Delete,
                UpdateCoverageType = UpdateCoverageType.None,
                FlagDelectAll = deleteAll,
                CoverageAreaPanel = coverage
            };

            _coverageAreaCommand.Handle(command);
            return Json(command);
        }

        public ActionResult Download()
        {
            var coverageSite = (CoverageSitePanelModel)Session["CoverageSite"];
            if (coverageSite == null)
            {
                coverageSite = new CoverageSitePanelModel();
            }

            string filename = "Coverage_" + String.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);

            var bytes = GenerateEntitytoExcel<CoverageAreaPanel>(coverageSite.CoverageAreaPanel, filename);
            return File(bytes, "application/excel", filename + ".xlsx");
            //return View();
        }

        public JsonResult CheckCompleteFlag(decimal cvrId)
        {
            var query = new GetCheckCompleteFlagQuery()
            {
                CVRId = cvrId
            };

            var result = _queryProcessor.Execute(query);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //test save
        //[HttpPost]
        //public JsonResult SubmitInfo(CoverageSitePanelModel dslamId)
        //{
        //    var response = new ZipCodeModel()
        //    {
        //        Amphur = "TTT"
        //    };

        //    return Json(dslamId);
        //}

        //[HttpPost]
        //public JsonResult SaveDSLAMInfo(DslamInfoPanel dslam)
        //{
        //    //var command = new DSLAMInfoCommand()
        //    //{
        //    //    //ACTIVEFLAG = dslam.ACTIVEFLAG,
        //    //    //DSLAMID = dslam.DSLAMID,
        //    //    //DSLAMMODELID = dslam.DSLAMMODELID,
        //    //    //DSLAMNUMBER = dslam.DSLAMNUMBER,
        //    //    //LOT_NUMBER = dslam.lo
        //    //};

        //    return Json("");
        //}      

    }
}
