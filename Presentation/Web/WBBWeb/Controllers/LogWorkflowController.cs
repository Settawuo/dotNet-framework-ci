using AIRNETEntity.PanelModels;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomHandleError]
    [IENoCache]
    [SessionExpireFilter]
    public class LogWorkflowController : WBBController
    {
        //
        // GET: /LogWorkflow/

        private readonly IQueryProcessor _queryProcessor;

        public LogWorkflowController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            ViewBag.User = base.CurrentUser;

            return View();
        }

        public ActionResult ReadSearchLogInterfaceData([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {

            if (string.IsNullOrEmpty(dataS)) return null;

            var searchrptModel = new JavaScriptSerializer().Deserialize<SearchAirnetInterfaceLogQuery>(dataS);
            var requestsort = request.Sorts.FirstOrDefault() ?? new SortDescriptor();
            searchrptModel.SortColumn = requestsort.Member;
            searchrptModel.OrderBy = !string.IsNullOrEmpty(requestsort.Member) ? (requestsort.SortDirection == ListSortDirection.Ascending ? "asc" : "desc") : string.Empty;
            searchrptModel.PageNo = request.Page;
            searchrptModel.RecordsPerPage = request.PageSize;

            var reportdatas = SearchInterfaceLogList(searchrptModel);

            request.Sorts = null;

            var result = reportdatas.ToDataSourceResult(request);
            result.Data = reportdatas;
            var reportTotal = reportdatas.FirstOrDefault();

            result.Total = reportTotal != null ? Convert.ToInt32(reportTotal.ALL_RECORDS) : 0;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<SearchAirnetInterfaceLog> SearchInterfaceLogList(SearchAirnetInterfaceLogQuery query)
        {
            try
            {
                var result = _queryProcessor.Execute(query);
                if (result.ReturnCode == "0")
                {
                    var returnList = result.AirInterfaceLogData;
                    return returnList;
                }
                else
                {
                    return new List<SearchAirnetInterfaceLog>();
                }

            }
            catch (Exception ex)
            {
                Logger.Info("Error when call SearchInterfaceLogList");
                Logger.Info(ex.GetErrorMessage());
                return new List<SearchAirnetInterfaceLog>();
            }

        }


    }
}
