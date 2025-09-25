using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FBBConfig.Extensions;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class ConfigurationAutoMailReportController : FBBConfigController
    {

        private readonly ICommandHandler<ConfigurationAutoMailCommand> _saveCommand;
        //
        // GET: /AutoMailReport/
        private readonly IQueryProcessor _queryProcessor;

        public ConfigurationAutoMailReportController(ILogger logger,
              IQueryProcessor queryProcessor
              , ICommandHandler<ConfigurationAutoMailCommand> saveCommand)
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _saveCommand = saveCommand;
        }

        public ActionResult Index(string saveStatus = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = CurrentUser;
            ViewBag.SaveStatus = saveStatus;
            return View();
        }

        public ActionResult ConfigurationInformation(string reportId)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            var configModel = new ConfigurationReportModel();

            ViewBag.User = CurrentUser;
            ViewBag.Username = CurrentUser.UserName;
            if (!string.IsNullOrEmpty(reportId))
            {
                var query = new GetConfigurationReportByIdQuery
                {
                    ReportId = reportId
                };
                configModel = _queryProcessor.Execute(query);
            }


            return View(configModel);
        }

        public ActionResult SaveConfigurationInformation(ConfigurationReportModel model)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            string saveStatus = "N";
            string errMsg = "";


            var command = new ConfigurationAutoMailCommand
            {
                report_id = model.REPORT_ID,
                report_name = model.REPORT_NAME,
                scheduler = model.SCHEDULER,
                day_of_week = model.DAY_OF_WEEK,
                email_to = model.EMAIL_TO,
                email_from = model.EMAIL_FROM,
                email_cc = model.EMAIL_CC,
                email_subject = model.EMAIL_SUBJECT,
                email_content = model.EMAIL_CONTENT,
                email_to_admin = model.EMAIL_TO_ADMIN,
                active_flag = model.ACTIVE_FLAG,
                report_type = model.REPORT_TYPE,
                created_by = CurrentUser.UserName
            };
            foreach (var item in model.ConfigurationQueryList)
            {
                if (item.QUERY_ID != 0 && item.QUERY_TYPE != "D")
                {
                    var config = new ConfigurationQueryArrayModel
                    {
                        query_id = item.QUERY_ID,
                        sheet_name = item.SHEET_NAME,
                        query_1 = item.QUERY_1,
                        query_2 = item.QUERY_2,
                        query_3 = item.QUERY_3,
                        query_4 = item.QUERY_4,
                        query_5 = item.QUERY_5,
                        query_type = item.QUERY_TYPE
                    };
                    command.ConfigurationQueryList.Add(config);
                }

            }

            _saveCommand.Handle(command);
            if (command.return_code == 0)
            {
                saveStatus = "Y";
                errMsg = command.return_msg;
            }

            return Json(new {saveStatus, errMsg}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReport(string reportId)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            string saveStatus = "N";
            string errMsg = "";

            var command = new ConfigurationAutoMailCommand
            {
                report_id = Convert.ToDecimal(reportId),
                report_type = "D",
                ConfigurationQueryList = new List<ConfigurationQueryArrayModel>()
            };

            _saveCommand.Handle(command);
            if (command.return_code == 0)
            {
                saveStatus = "Y";
                errMsg = command.return_msg;
            }

            return Json(new {saveStatus, errMsg}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetScheduler(string type)
        {
            var data = new List<LovModel>
            {
                new LovModel {DISPLAY_VAL = "Daily", LOV_NAME = "", LOV_VAL1 = "Daily"},
                new LovModel {DISPLAY_VAL = "Weekly", LOV_NAME = "", LOV_VAL1 = "Weekly"}
            };
            if (type == "SEARCH")
            {
                data.Insert(0, new LovModel { DISPLAY_VAL = "All", LOV_NAME = "", LOV_VAL1 = "All" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDayOfWeek(string type)
        {
            var data = new List<LovModel>
            {
                new LovModel {DISPLAY_VAL = "Monday", LOV_NAME = "", LOV_VAL1 = "Monday"},
                new LovModel {DISPLAY_VAL = "Tuesday", LOV_NAME = "", LOV_VAL1 = "Tuesday"},
                new LovModel {DISPLAY_VAL = "Wednesday", LOV_NAME = "", LOV_VAL1 = "Wednesday"},
                new LovModel {DISPLAY_VAL = "Thursday", LOV_NAME = "", LOV_VAL1 = "Thursday"},
                new LovModel {DISPLAY_VAL = "Friday", LOV_NAME = "", LOV_VAL1 = "Friday"},
                new LovModel {DISPLAY_VAL = "Saturday", LOV_NAME = "", LOV_VAL1 = "Saturday"},
                new LovModel {DISPLAY_VAL = "Sunday", LOV_NAME = "", LOV_VAL1 = "Sunday"}
            };
            if (type == "SEARCH")
            {
                data.Insert(0, new LovModel { DISPLAY_VAL = "All", LOV_NAME = "", LOV_VAL1 = "All" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportRead([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (!string.IsNullOrEmpty(dataS))
            {
                var searchrptModel = new JavaScriptSerializer().Deserialize<GetConfigurationReportQuery>(dataS);
                var requestsort = request.Sorts.FirstOrDefault() ?? new SortDescriptor();
                searchrptModel.SortColumn = requestsort.Member;
                searchrptModel.SortBy = !string.IsNullOrEmpty(requestsort.Member) ? (requestsort.SortDirection == ListSortDirection.Ascending ? "asc" : "desc") : string.Empty;
                searchrptModel.PageNo = request.Page;
                searchrptModel.RecordsPerPage = request.PageSize;
                var reportdatas = GetDataReport(searchrptModel);
                request.Sorts = null;

                var result = reportdatas.ToDataSourceResult(request);
                result.Data = reportdatas;
                var reportTotal = reportdatas.FirstOrDefault();

                result.Total = reportTotal != null ? Convert.ToInt32(reportTotal.ALL_RECORDS) : 0;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public List<ConfigurationReportModel> GetDataReport(GetConfigurationReportQuery searchrptModel)
        {
            List<ConfigurationReportModel> listResult = _queryProcessor.Execute(searchrptModel);
            return listResult;
        }

        public ActionResult CheckQueryReport(string query)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            string status = "N";
            string errMsg = "";

            var checkQuery = new CheckQueryReportQuery
            {
                Query = query
            };

            var check = _queryProcessor.Execute(checkQuery);
            status = check.Status;
            errMsg = check.Message;

            return Json(new {status, errMsg}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Resend(string reportId, string createBy)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            string status = "N";
            string errMsg = "";

            var query = new ReportAutoMailQuery()
            {
                ReportId = reportId,
                CreateBy = CurrentUser.UserName,
                PathTempFile = Configurations.TARGET,
                DomainTempFile = Configurations.TARGET_DOMAIN,
                UserTempFile = Configurations.TARGET_USER,
                PassTempFile = Configurations.TARGET_PWD
            };

            var check = _queryProcessor.Execute(query);
            if (check.ReturnCode == "0")
                status = "Y";



            return Json(new {status, errMsg}, JsonRequestBehavior.AllowGet);
        }

    }
}
