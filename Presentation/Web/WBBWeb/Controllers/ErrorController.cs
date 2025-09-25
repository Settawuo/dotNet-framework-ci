using System.Web.Mvc;
using WBBBusinessLayer;

namespace iWorkflowsWeb.Controllers
{
    public class ErrorController : Controller
    {
        public ErrorController(ILogger logger)
        {
            //base._Logger = logger;
        }

        public ViewResult Index()
        {
            return View("Error");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View("PageNotFound");
        }

        public ActionResult TimeOutPage()
        {
            return View("TimeOutPage");
        }

        public ActionResult ContinueSession()
        {
            return new EmptyResult();
        }

        public ActionResult HttpError404(string message)
        {
            return RedirectToAction("Index", "Process");
        }

        public ActionResult HttpError500(string message)
        {
            return RedirectToAction("Index", "Process");
        }

        public ActionResult General(string message)
        {
            return RedirectToAction("Index", "Process");
        }
    }
}
