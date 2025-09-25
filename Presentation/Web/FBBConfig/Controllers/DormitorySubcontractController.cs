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
    public class DormitorySubcontractController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateDormitorySubcontrachCommand> _UpdateDormitorySubcontrachCommand;



        public DormitorySubcontractController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<UpdateDormitorySubcontrachCommand> updateDormitorySubcontrachCommand
          )
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _UpdateDormitorySubcontrachCommand = updateDormitorySubcontrachCommand;
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
        private List<ConfigurationDormitorySubcontract> GetDataSearchModel(DormitorySearchFibrenetID SearchPara)
        {
            var User = base.CurrentUser;

            var query = new GetAWConfigurationDormitorySubcontractQuery()
            {
                Region = SearchPara.Region ?? "",
                DormitoryProvince = SearchPara.Province ?? "",
                DormitoryName = SearchPara.DormitoryName ?? "",
                User = User.UserName,

                SubContractFlag = SearchPara.FibrenetIDFlag
            };
            List<ConfigurationDormitorySubcontract> result = _queryProcessor.Execute(query);
            return result;
        }
        public string EditSubcontractData(string DormitoryTH, string SubcontractCode, string SubcontractNameTH, string SubcontractNameEN, string Price, string Index)
        {
            var User = base.CurrentUser;
            var command = new UpdateDormitorySubcontrachCommand()
            {

                DORMITORY_NAME_TH = DormitoryTH,
                SUB_CONTRACT_LOCATION_CODE = SubcontractCode,
                SUB_CONTRACT_NAME_TH = SubcontractNameTH,
                SUB_CONTRACT_NAME_EN = SubcontractNameEN,
                PRICE_INSTALL = Price,
                User = User.UserName
            };
            _UpdateDormitorySubcontrachCommand.Handle(command);
            if (command.Result == 0)
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
            var searchModel = new JavaScriptSerializer().Deserialize<DormitorySearchFibrenetID>(dataS);
            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            string rptCriteria = criteriaModel.CRITERIA;
            string rptName = criteriaModel.REPORT;
            string rptDate = criteriaModel.REPORT_DATE;

            List<ConfigurationDormitorySubcontract> listall = GetDataSearchModel(searchModel);

            string filename = GetExcelNameWithDateTime("ConfigurationSubcontract");
            string[] headerCol = { "G_DORMITORY_ID", "G_REGION", "G_PROVINCE", "G_DORMITORY_NAME", "G_SUBCONTRACT_CODE", "G_SUBCONTRACT_NAME", "G_SUBCONTRACT_NAME", "G_PRICE" };
            int[] hideCol = { 2, 1, 0 };
            bool[] getLov2 = { true, true, true, true, true, false, true, true };
            var bytes = GenerateEntitytoExcel<ConfigurationDormitorySubcontract>(listall, filename, headerCol, "FBBDORM_ADMIN_SCREEN", "ADMIN_FBBDORM001", rptName, rptCriteria, rptDate, hideCol, getLov2);


            return File(bytes, "application/excel", filename + ".xlsx");

        }
    }
}
