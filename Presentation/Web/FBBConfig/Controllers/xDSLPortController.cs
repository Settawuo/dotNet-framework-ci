using FBBConfig.Extensions;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Account;

namespace FBBConfig.Controllers
{


    public class xDSLPortController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;

        public xDSLPortController(ILogger logger,
              IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        // GET: /xDSLPort/
        [AuthorizeUserAttribute]
        public ActionResult Index()
        {
            var query = new GetUserDataQuery
            {
                UserName = "thitimaw"
            };

            base.CurrentUser = _queryProcessor.Execute(query);

            ViewBag.User = base.CurrentUser;



            return View();
        }




    }
}
