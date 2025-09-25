using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.ExWebServices;
namespace WBBWeb.Controllers
{
    public class TotalVasController : WBBController
    {


        private IQueryProcessor _queryProcessor;
        public TotalVasController(ILogger logger,
             IQueryProcessor queryProcessor)
        {
            base.Logger = logger;
            _queryProcessor = queryProcessor;
        }



        public JsonResult CheckMobilePhoneOneloveAIS(string mobile, string Language, string ResultID, string SffProfileLogID_H)
        {
            var Ipquery = new GetWSAISMOBILESeviceQuery
            {
                Msisdn = mobile,
                LanguageSender = Language,
                ResultID = ResultID,
                SffProfileLogID = SffProfileLogID_H,
                User = base.CurrentUser.UserName.ToString()
            };

            string sesultdata = _queryProcessor.Execute(Ipquery);
            base.Logger.Info("sesultdata ::::::::::::::::::::::" + sesultdata);
            return Json(sesultdata, JsonRequestBehavior.AllowGet);
        }



    }
}
