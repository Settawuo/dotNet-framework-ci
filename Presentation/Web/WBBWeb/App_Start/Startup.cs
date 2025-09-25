using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("FBBWEB-SignalR", typeof(WBBWeb.App_Start.Startup))]
namespace WBBWeb.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}