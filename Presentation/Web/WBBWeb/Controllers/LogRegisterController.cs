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
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomHandleError]
    [IENoCache]
    [SessionExpireFilter]
    // [AuthorizeReportAttribute]
    public class LogRegisterController : WBBController
    {
        //
        // GET: /LogRegister/
        private readonly IQueryProcessor _queryProcessor;

        public LogRegisterController(IQueryProcessor queryProcessor)
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

        public ActionResult ReadSearchLogRegisterData([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            //if (null == base.CurrentUser)
            //{
            //    return this.Json(new DataSourceResult
            //    {
            //        Errors = "Timeout"
            //    });
            //}

            if (string.IsNullOrEmpty(dataS)) return null;

            var searchrptModel = new JavaScriptSerializer().Deserialize<SearchInterfaceLogQuery>(dataS);
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

        public JsonResult GetMethodName()
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetMethodNameDB();
                DropdownModel dropdownModel = new DropdownModel();
                dropdownModel.Text = "All";
                dropdownModel.Value = "";
                CONTACT.Insert(0, dropdownModel);
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        private List<SearchInterfaceLog> SearchInterfaceLogList(SearchInterfaceLogQuery query)
        {
            try
            {
                var result = _queryProcessor.Execute(query);
                if (result.ReturnCode == "0")
                {
                    var returnList = result.InterfaceLogData;
                    return returnList;
                }
                else
                {
                    return new List<SearchInterfaceLog>();
                }

            }
            catch (Exception ex)
            {
                Logger.Info("Error when call SearchInterfaceLogList");
                Logger.Info(ex.GetErrorMessage());
                return new List<SearchInterfaceLog>();
            }
        }

        private List<DropdownModel> GetMethodNameDB()
        {
            try
            {
                var query = new GetInterfaceLogMethodNameQuery();

                var returnList = _queryProcessor.Execute(query);
                List<DropdownModel> ListData = new List<DropdownModel>();
                foreach (var item in returnList)
                {
                    DropdownModel dataitem = new DropdownModel()
                    {
                        Text = item,
                        Value = item
                    };
                    ListData.Add(dataitem);
                }

                return ListData;
            }
            catch (Exception ex)
            {
                Logger.Info("Error when call SearchInterfaceLogList");
                Logger.Info(ex.GetErrorMessage());
                return new List<DropdownModel>();
            }
        }

    }
}
