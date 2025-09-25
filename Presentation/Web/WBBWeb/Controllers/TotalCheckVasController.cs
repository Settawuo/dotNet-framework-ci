using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBEntity.PanelModels;
using WBBEntity.Extensions;
using WBBWeb.Extension;
using WBBWeb.Models;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBEntity.PanelModels.WebServiceModels;
namespace WBBWeb.Controllers
{
    public class TVasController : WBBController
    {
        

       private IQueryProcessor _queryProcessor;
       public TVasController(ILogger logger, 
            IQueryProcessor queryProcessor)
        {
            base.Logger = logger;
            _queryProcessor = queryProcessor;
        }

      
      
        //public JsonResult GetListPackageService(string PRODUCT_SUBTYPE)
        //{
        //    var query = new GetListPackageByServiceQuery
        //    {
        //        PRODUCT_SUBTYPE = PRODUCT_SUBTYPE
        //    };

        //    var list = _queryProcessor.Execute(query);
        //    if (!list.Whatever())
        //    {
        //        base.Logger.Info("Package Model Is Null");
        //        return Json(new List<PackageModel>(), JsonRequestBehavior.AllowGet);
        //    }

        //    if (PRODUCT_SUBTYPE != "WireBB")
        //    {
        //        list[0].S_GROUP = list.Where(p => p.PACKAGE_GROUP != null).Select(p => p.PACKAGE_GROUP).Distinct().ToList();
        //    }

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

    }
}
