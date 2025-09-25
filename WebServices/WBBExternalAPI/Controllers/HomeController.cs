using System.Web.Mvc;

namespace WBBExternalAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "External Web API";

            return View();
        }
    }
}
