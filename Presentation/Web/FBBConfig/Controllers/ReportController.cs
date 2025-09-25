using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries.Report;
using WBBContract.Queries.FBBWebConfigQueries.ReportPortAssignment;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class ReportController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<PortAssignmentCommand> _portAssignmentCommand;
        private readonly ICommandHandler<SummaryPortCommand> _SummaryPortCommand;
        private readonly ICommandHandler<ReportActiveCommand> _ReportActiveCommand;
        private readonly ICommandHandler<CommmandReportByRegionCommand> _Pefremennt_By_Region;

        public ReportController(ILogger logger, IQueryProcessor queryProcessor,
            ICommandHandler<PortAssignmentCommand> portAssignmentCommand,
            ICommandHandler<SummaryPortCommand> SummaryPortCommand,
            ICommandHandler<ReportActiveCommand> ReportActiveCommand,
            ICommandHandler<CommmandReportByRegionCommand> Pefremennt_By_Region)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _portAssignmentCommand = portAssignmentCommand;
            _SummaryPortCommand = SummaryPortCommand;
            _ReportActiveCommand = ReportActiveCommand;
            _Pefremennt_By_Region = Pefremennt_By_Region;
        }

        [AuthorizeUserAttribute]
        public ActionResult RptIndex()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            return View();
        }

        public IEnumerable<FBB_RPT_LOG> GetRPTLogs(string Username)
        {

            var query = new GetRptLogQuery
            {
                UserName = Username
            };
            var result = _queryProcessor.Execute(query);

            return result;
        }

        public ActionResult RPTLogs_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetRPTLogs("").ToDataSourceResult(request));
        }
        #region RptPerformanceByRegion Page
        [AuthorizeUserAttribute]
        public ActionResult RptPerformanceByRegion()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;


            return View();
        }



        public JsonResult Getprogram_description()
        {
            var query = new GetPrograme_Name
            {
                Progrmer_Code = "FBB_RPTPORT002"
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Add_RptPerformanceByRegion(string REPORT_NAME, string REPORT_DESC, string REPORT_PARAMETER, string REPORT_STATUS)
        {


            var Date_Today = System.DateTime.Now;

            var commmandPrefermeanRegion = new CommmandReportByRegionCommand
            {
                CREATED_BY = base.CurrentUser.UserName.ToString(),
                CREATED_DATE = Date_Today,
                Des_Code = REPORT_DESC,
                Flag_Add_log_Rpt = "Add_R",
                REPORT_DESC = REPORT_DESC.ToString(),
                REPORT_NAME = REPORT_NAME,
                REPORT_PARAMETER = REPORT_PARAMETER.ToString(),
                REPORT_STATUS = REPORT_STATUS,
                REPORT_CODE = "FBB_RPTPORT002"

            };

            _Pefremennt_By_Region.Handle(commmandPrefermeanRegion);

            return Json(null, JsonRequestBehavior.AllowGet);

        }

        #endregion

        //private List<FBB_DSLAM_INFO> GetDSLAMInfoModel(string nodeName)
        //{
        //    var query = new GetDSLAMInfoQuery
        //    {
        //        NodeName = nodeName
        //    };

        //    var result = _queryProcessor.Execute(query);
        //    return result;
        //}

        //private List<string> GetCoverrageAreaModel()
        //{
        //    var query = new GetCoverageAreaReportQuery
        //    {

        //    };

        //    var result = _queryProcessor.Execute(query);
        //    return null;
        //}   

        #region My Report
        [AuthorizeUserAttribute]
        public ActionResult MyReport()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            return View();
        }
        protected internal virtual FilePathResult File(string fileName, string contentType, string fileDownloadName)
        {
            return new FilePathResult(fileName, contentType) { FileDownloadName = fileDownloadName };
        }

        public JsonResult ReadMyReport([DataSourceRequest] DataSourceRequest request)
        {
            var query = new GetRptLogQuery
            {
                UserName = base.CurrentUser.UserName
            };
            var result = _queryProcessor.Execute(query);
            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult download(string filename)
        {
            string temppath = Configurations.dowloadReportPath;
            byte[] fileBytes = System.IO.File.ReadAllBytes(temppath + filename);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

        }

        #endregion

        #region Assignment
        [AuthorizeUserAttribute]
        public ActionResult Assignment()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            return View();
        }

        public JsonResult SearchRptAssignment(string nodeNameTH = "", string nodeId = "")
        {
            var param = nodeNameTH + "|" + nodeId;
            var portAssignment = new PortAssignmentCommand()
            {
                ReportParam = param,
                Report_Code = "FBB_RPTPORT003",
                Create_By = base.CurrentUser.UserName
            };

            _portAssignmentCommand.Handle(portAssignment);

            return Json(portAssignment, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNodeNameTH()
        {
            var query = new GetNodeNameTHQuery
            {
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNodeID(string nodeNameTH = "")
        {
            var query = new GetNodeIDQuery
            {
                NodeNameTH = nodeNameTH
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Summary Port
        [AuthorizeUserAttribute]
        public ActionResult SummaryPort()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            return View();
        }

        public JsonResult SearchSummary(string nodeNameTH = "", string nodeId = "")
        {
            var param = nodeNameTH + "|" + nodeId;
            var summary = new SummaryPortCommand()
            {
                ReportParam = param,
                Report_Code = "FBB_RPTPORT004",
                Create_By = base.CurrentUser.UserName
            };

            _SummaryPortCommand.Handle(summary);

            return Json(summary, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNodeNameTH2()
        {
            var query = new GetNodeNameTHQuery
            {
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new DropdownModel { Text = "All", Value = "All" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNodeID2(string nodeNameTH = "")
        {
            var query = new GetNodeIDQuery
            {
                NodeNameTH = nodeNameTH
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new DropdownModel { Text = "All", Value = "All" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Report Active Subs by Region
        [AuthorizeUserAttribute]
        public ActionResult ReportActive()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            return View();
        }

        public JsonResult SearchActive(string region = "", string datefrom = "", string dateto = "")
        {
            var param = region + "|" + datefrom + "|" + dateto;
            var summary = new ReportActiveCommand()
            {
                ReportParam = param,
                Report_Code = "FBB_RPTPORT005",
                Create_By = base.CurrentUser.UserName
            };

            _ReportActiveCommand.Handle(summary);

            return Json(summary, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
