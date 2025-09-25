using System.Web.Mvc;

namespace FBBConfig.Extensions
{
    public class RouteBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return new RouteValues
            {
                Action = controllerContext.RouteData.Values["action"].ToString(),
                Controller = controllerContext.RouteData.Values["controller"].ToString(),
            };
        }
    }
}