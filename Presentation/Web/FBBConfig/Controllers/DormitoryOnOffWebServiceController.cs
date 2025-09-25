using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace FBBConfig.Controllers
{
    public class DormitoryOnOffWebServiceController : FBBConfigController
    {

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CheckStatusOnOffQuery> _CheckStatusOnOffQuery;
        private readonly ICommandHandler<UpdateOnOffServiceCommand> _UpdateOnOffServiceCommand;

        public DormitoryOnOffWebServiceController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<CheckStatusOnOffQuery> checkStatusOnOffQuery,
            ICommandHandler<UpdateOnOffServiceCommand> uppdateOnOffServiceCommand
          )
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _CheckStatusOnOffQuery = checkStatusOnOffQuery;
            _UpdateOnOffServiceCommand = uppdateOnOffServiceCommand;
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
            var LovDataScreen = base.LovData.Where(p => p.Type == "FBBDORM_ADMIN_SCREEN" && p.LovValue5 == "ADMIN_FBBDORM001").ToList();
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
        private List<ConfigurationOnOffServices> GetDataSearchModel(DormitorySearchPara SearchPara)
        {
            var User = base.CurrentUser;

            var query = new GetAWConfigurationOnOffServicetQuery()
            {
                User = User.UserName,
                Region = SearchPara.Region ?? "",
                DormitoryName = SearchPara.DormitoryName ?? "",
                BuildingNo = SearchPara.Building ?? "",
                DormitoryProvince = SearchPara.Province ?? ""


            };
            List<ConfigurationOnOffServices> result = _queryProcessor.Execute(query);
            if (result != null && result.Count > 0)
            {
                int i = 0;
                foreach (var item in result)
                {
                    result[i].StatusOld = item.Status;
                    i++;
                }

            }
            return result;
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

        public ActionResult UpdateOnOffServices(List<ConfigurationOnOffServices> saveModels)
        {
            string item = "0";

            if (saveModels != null)
            {
                foreach (var saveModel in saveModels)
                {
                    EditOnOffServices(saveModel.dormitory_name, saveModel.BUILDING, saveModel.Status, saveModel.StatusOld);
                }
            }
            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        private string EditOnOffServices(string DORMITORY_NAME_TH, string DORMITORY_NO_TH, string Status, string StatusOld, string p5, string p6)
        {
            throw new NotImplementedException();
        }

        public string EditOnOffServices(string DORMITORY_NAME_TH, string DORMITORY_NO_TH, string Status, string StatusOld)
        {
            var User = base.CurrentUser;
            if (Status != StatusOld)
            {
                var query = new CheckStatusOnOffQuery()
                {
                    p_dormitory_name = DORMITORY_NAME_TH,
                    p_building = DORMITORY_NO_TH
                };
                var result = _queryProcessor.Execute(query);
                if (result.result == 0)
                {
                    var command = new UpdateOnOffServiceCommand()
                    {
                        p_dormitory_name = DORMITORY_NAME_TH,
                        p_building = DORMITORY_NO_TH,
                        p_status = Status,
                        User = User.UserName
                    };
                    _UpdateOnOffServiceCommand.Handle(command);

                }
            }
            return "Succes";
        }


    }
}
