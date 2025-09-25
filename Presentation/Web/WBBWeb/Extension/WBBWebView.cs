using System.Web.Mvc;
using WBBBusinessLayer;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Extension
{
    public abstract class WBBWebView<T> : WebViewPage<T>
    {
        public ILogger Logger { get { return Bootstrapper.GetInstance<ILogger>(); } }
    }
}