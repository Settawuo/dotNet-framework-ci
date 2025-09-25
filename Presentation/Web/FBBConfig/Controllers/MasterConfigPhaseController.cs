using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.ComponentModel;
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
    public class MasterConfigPhaseController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdatePaidStatusDataCommand> _UpdatePaidStatusCommand;
        private readonly ICommandHandler<ImportPaidStatusCommand> _ImportPaidStatusCommand;
        private readonly ICommandHandler<UpdateOLTStatusCommand> _UpdateOLTStatusCommand;

        public MasterConfigPhaseController(ILogger logger,
              IQueryProcessor queryProcessor,
              ICommandHandler<UpdatePaidStatusDataCommand> updatePaidStatusCommand,
              ICommandHandler<ImportPaidStatusCommand> importPaidStatusCommand,
              ICommandHandler<UpdateOLTStatusCommand> updateOLTStatusCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _UpdatePaidStatusCommand = updatePaidStatusCommand;
            _ImportPaidStatusCommand = importPaidStatusCommand;
            _UpdateOLTStatusCommand = updateOLTStatusCommand;
        }

        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;

            SetViewBagLovV2("FBB_MASTERCONFIG_PHASE");
            return View();
        }

        private void SetViewBagLovV2(string screenType)
        {
            var query = new GetLovQuery()
            {
                LovType = screenType
            };

            var LovDataScreen = _queryProcessor.Execute(query).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        private void SetViewBagLov(string screenType)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        // dropdown 1
        public JsonResult p_get_orgid()
        {
            var query = new FBBPAYG_DropdownSUBSONTRACTQuery
            {
                PackageName = "wbb.PKG_FBB_REPORT_SUBCONTRACT",
                ProcName = "P_GET_ORGID_SUBCONTRACT",
                CurName = "dropdown_cur"
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "ALL", VALUE = "-1" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // dropdown 2
        public JsonResult p_get_nameth_subcontract()
        {
            var query = new FBBPAYG_DropdownSUBSONTRACTQuery
            {
                PackageName = "wbb.PKG_FBB_REPORT_SUBCONTRACT",
                ProcName = "P_GET_NAMETH_SUBCONTRACT",
                CurName = "dropdown_cur"
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "ALL", VALUE = "-1" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // dropdown 3
        public JsonResult p_get_storage_subcontract()
        {
            var query = new FBBPAYG_DropdownSUBSONTRACTQuery
            {
                PackageName = "wbb.PKG_FBB_REPORT_SUBCONTRACT",
                ProcName = "P_GET_STORAGE_SUBCONTRACT",
                CurName = "dropdown_cur"
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "ALL", VALUE = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // dropdown 4
        public JsonResult p_get_phase_subcontract()
        {
            var query = new FBBPAYG_DropdownSUBSONTRACTQuery
            {
                PackageName = "wbb.PKG_FBB_REPORT_SUBCONTRACT",
                ProcName = "P_GET_PHASE_SUBCONTRACT",
                CurName = "dropdown_cur"
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "ALL", VALUE = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReportSUBSONTRACT_Async([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (request.Sorts.Count == 0) // order by UPDATED_DATE desc
            {
                Kendo.Mvc.SortDescriptor _sort = new Kendo.Mvc.SortDescriptor();
                _sort.Member = "ORG_ID";
                _sort.SortDirection = ListSortDirection.Ascending;
                request.Sorts.Add(_sort);
            }
            var searchEventModel = new JavaScriptSerializer().Deserialize<SupContractorReportList>(dataS);
            var query = new SupContractorReportQuery
            {
                ORG_ID = searchEventModel.ORG_ID.ToString(),
                SUB_CONTRACTOR_NAME_TH = searchEventModel.SUB_CONTRACTOR_NAME_TH,
                STORAGE_LOCATION = searchEventModel.STORAGE_LOCATION,
                PHASE = searchEventModel.PHASE,
                REQUEST_BY = base.CurrentUser.UserName.ToSafeString()
            };
            var result = _queryProcessor.Execute(query);
            return Json(result.ToDataSourceResult(request));
        }

    }
}
