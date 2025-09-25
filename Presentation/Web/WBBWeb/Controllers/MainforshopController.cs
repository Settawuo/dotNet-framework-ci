using System.Linq;
using System.Web.Mvc;

namespace WBBWeb.Controllers
{
    public class MainforshopController : WBBController
    {
        //
        // GET: /Mainforshop/

        public ActionResult Index()
        {
            ViewBag.UrlScreenMainforshop = base.LovData.Where(l => l.Type == "SCREEN").ToList();

            return View();
        }

    }
}
