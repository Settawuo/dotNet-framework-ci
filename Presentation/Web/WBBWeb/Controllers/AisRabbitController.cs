using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Master;

namespace WBBWeb.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class AisRabbitController : RabbitBaseController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<RegAisRabbitCommand> _regAirRabbit;
        //
        // GET: /AisRabbit/
        public AisRabbitController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<RegAisRabbitCommand> regAirRabbit)
        {
            base.Logger = logger;
            _queryProcessor = queryProcessor;
            _regAirRabbit = regAirRabbit;
        }

        public ActionResult Index()
        {
            var query = new GetLovRabbitQuery();
            ViewBag.Lov = _queryProcessor.Execute(query);

            return View();
        }

        public JsonResult Register(string airnetID, string idCard, string email)
        {
            base.Logger.Info("Register => AirnetID: " + airnetID + " IDCard: " + idCard + " Email: " + email);

            var regAirRabbitModel = new RegAisRabbitCommand()
            {
                IdCard = idCard,
                Non_Mobile = airnetID,
                Email = email
            };

            _regAirRabbit.Handle(regAirRabbitModel);
            return Json(regAirRabbitModel.Return_Desc, JsonRequestBehavior.AllowGet);
        }

    }
}
