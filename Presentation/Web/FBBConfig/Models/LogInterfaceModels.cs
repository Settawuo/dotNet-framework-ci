using System.Collections.Generic;
using System.Web.Mvc;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Models
{
    public class LogInterfaceView
    {
        public List<LogInterfaceModel> LogInterface { get; set; }
        public IEnumerable<SelectListItem> PACKAGE_NAME { get; set; }
        public IEnumerable<SelectListItem> METHOD_NAME { get; set; }
        public IEnumerable<SelectListItem> TABLE { get; set; }
        public IEnumerable<SelectListItem> FILENAME { get; set; }
    }

}