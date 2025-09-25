using System;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBEntity.Extensions;

namespace WBBWeb.Controllers
{
    public class VasSummaryController : WBBController
    {
        private IQueryProcessor _queryProcessor;

        public VasSummaryController(ILogger logger, IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public JsonResult GetAisMobile(string msisdn, string orderRef, string cond = "")
        {
            bool returnValue = false;
            string userName = "CUSTOMER";

            if (base.CurrentUser != null)
                userName = base.CurrentUser.UserName.ToSafeString();

            string refNo = orderRef + DateTime.Now.ToString("HHmmss");

            try
            {
                var query = new GetAisMobileServiceQuery()
                {
                    Msisdn = msisdn.ToSafeString(),
                    Opt1 = "",
                    Opt2 = "",
                    OrderDesc = "query sub",
                    OrderRef = refNo,
                    User = userName,
                    UserName = "FBB"
                };

                var result = _queryProcessor.Execute(query);

                if (cond == "dbspeed")
                {
                    return Json(refNo, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (result.IsSuccess != null && result.Chm != null && result.State != null)
                    {
                        if (result.IsSuccess.ToUpper() == "TRUE" && result.Chm == "0" && result.State == "1")
                            returnValue = true;
                        else
                            returnValue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error when call GetAisMobile in VasSummaryController : " + ex.InnerException);
            }

            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

    }
}
