using System.Web.Mvc;
using WBBBusinessLayer;

namespace WBBWeb.Controllers
{
    public class RabbitBaseController : Controller
    {
        public ILogger Logger { get; set; }
    }
}
