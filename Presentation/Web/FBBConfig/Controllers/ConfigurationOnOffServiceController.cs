using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class ConfigurationOnOffServiceController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateAddressIDCommand> _UpdateAddressIDCommand;



        public ConfigurationOnOffServiceController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<UpdateAddressIDCommand> updateAddressIDCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _UpdateAddressIDCommand = updateAddressIDCommand;
            //_SaveEditBuildingCommand = saveEditBuildingCommand;
            //_SaveEditAddBuildingCommand = saveEditAddBuildingCommand;
            //_SaveEditDormCommand = saveEditDormCommand;
        }
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();
            return View();
        }
        private void SetViewBagLov()
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == "ADMIN_FBBDORM_SCREEN").ToList();
            ViewBag.configscreen = LovDataScreen;
            ViewBag.DormConstant = GetFbbConstantModel(WebConstants.LovConfigName.DormConstants);

        }
        private List<FbbConstantModel> GetFbbConstantModel(string DormConstants)
        {
            var data = base.LovData
               .Where(l => l.Type.Equals(DormConstants))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }
        public ActionResult SearchDataSourceRequest([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (dataS != null && dataS != "")
            {
                var SearchPara = new JavaScriptSerializer().Deserialize<DormitorySearchPara>(dataS);
                var result = GetDataSearchModel(SearchPara);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }
        private List<ConfigurationAddressID> GetDataSearchModel(DormitorySearchPara SearchPara)
        {
            var User = base.CurrentUser;

            var query = new GetAWConfigurationAddressIDQuery()
            {
                DormitoryProvince = SearchPara.Province ?? "",
                DormitoryName = SearchPara.DormitoryName ?? "",
                User = User.UserName
            };
            List<ConfigurationAddressID> result = _queryProcessor.Execute(query);
            return result;
        }
    }
}
