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
    public class ConfigurationAddressIDController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateAddressIDCommand> _UpdateAddressIDCommand;



        public ConfigurationAddressIDController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<UpdateAddressIDCommand> updateAddressIDCommand)
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
        public ActionResult SearchDataSourceRequest([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (dataS != null && dataS != "")
            {
                var SearchPara = new JavaScriptSerializer().Deserialize<DormitorySearchFibrenetID>(dataS);
                var result = GetDataSearchModel(SearchPara);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }
        private List<ConfigurationAddressID> GetDataSearchModel(DormitorySearchFibrenetID SearchPara)
        {
            var User = base.CurrentUser;

            var query = new GetAWConfigurationAddressIDQuery()
            {
                Region = SearchPara.Region ?? "",
                DormitoryProvince = SearchPara.Province ?? "",
                DormitoryName = SearchPara.DormitoryName ?? "",
                User = User.UserName,
                FibrenetIDFlag = SearchPara.FibrenetIDFlag
            };
            List<ConfigurationAddressID> result;
            result = _queryProcessor.Execute(query);
            return result;
        }


        public string EditAddressIDData(string DORMITORY_NAME_TH, string DORMITORY_NO_TH, string ADDRESS_ID, string Index)
        {
            var User = base.CurrentUser;
            var command = new UpdateAddressIDCommand()
            {
                DORMITORY_NAME_TH = DORMITORY_NAME_TH,
                DORMITORY_NO_TH = DORMITORY_NO_TH,
                ADDRESS_ID = ADDRESS_ID,
                User = User.UserName
            };
            _UpdateAddressIDCommand.Handle(command);
            if (command.Result == "0")
            {
                return "Success";
            }
            else
            {

                return "Fail";
            }
        }

        public ActionResult ExportReport(string dataS, string criteria)
        {
            List<ConfigurationAddressID> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<DormitorySearchFibrenetID>(dataS);
            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            string rptCriteria = criteriaModel.CRITERIA;
            string rptName = criteriaModel.REPORT;
            string rptDate = criteriaModel.REPORT_DATE;

            listall = GetDataSearchModel(searchModel);

            string filename = GetExcelNameWithDateTime("ConfigurationAddressID");
            string[] headerCol = { "G_REGION", "G_DORMITORY_ID", "G_PROVINCE", "G_DORMITORY_NAME", "L_BUILDING_NAME", "G_ADDRESS_ID" };
            int[] hideCol = { 1, 0 };
            var bytes = GenerateEntitytoExcel<ConfigurationAddressID>(listall, filename, headerCol, "FBBDORM_ADMIN_SCREEN", "ADMIN_FBBDORM001", rptName, rptCriteria, rptDate, hideCol, null);

            return File(bytes, "application/excel", filename + ".xlsx");

        }
    }
}