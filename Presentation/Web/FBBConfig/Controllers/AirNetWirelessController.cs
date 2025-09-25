using FBBConfig.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Account;

namespace FBBConfig.Controllers
{
    public class AirNetWirelessController : FBBConfigController
    {
         private readonly IQueryProcessor _queryProcessor;
         private readonly IQueryProcessor _lo;
         public AirNetWirelessController(ILogger logger,
              IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }
      

        public ActionResult AirNetWireless()
        {
            return View();
        }


       


        //[AuthorizeUserAttribute]
        //public ActionResult RptPerformanceByRegion()
        //{
        //    var query = new GetUserDataQuery
        //    {
        //        UserName = "thitimaw"
        //    };

        //    base.CurrentUser = _queryProcessor.Execute(query);

        //    ViewBag.User = base.CurrentUser;
        //    return View();
        //}



      
    }
}
