using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace FBBConfig.Controllers
{
    public class DormMasterDetailController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveDormMasterDetailCommand> _SaveDormMasterDetailCommand;
        public DormMasterDetailController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<SaveDormMasterDetailCommand> SaveDormMasterDetailCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _SaveDormMasterDetailCommand = SaveDormMasterDetailCommand;
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
        public JsonResult SelectDormitoryName(string State = "Out of Service")
        {
            var query = new SelectDormitoryNameQuery
            {
                State = State
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectDormitoryBuilding(string State = "Out of Service", string DormName = "")
        {
            var query = new SelectDormitoryBuildingQuery
            {
                State = State,
                DormitoryName = DormName
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
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
        public JsonResult SaveDormDetailMaster(string dormitory_name_th, string dormitory_no,
       string addressid, string SubcontractCode, string SubcontractTH, string SubcontractEN)
        {
            try
            {
                // AdminDormitoryMasterModel DormMaster = new AdminDormitoryMasterModel();

                var LoginUser = base.CurrentUser;
                var command = new SaveDormMasterDetailCommand()
                {
                    SaveDT_dormitory_name = dormitory_name_th,
                    SaveDT_dormitory_no_th = dormitory_no,
                    SaveDT_addressid = addressid,
                    SaveDT_subcontract_code = SubcontractCode,
                    SaveDT_subcontractTH = SubcontractTH,
                    SaveDT_subcontractEN = SubcontractEN,
                    SaveDT_User = LoginUser.UserName
                };

                _SaveDormMasterDetailCommand.Handle(command);


                return Json(command.SaveDT_subcontract_code, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //base.Logger.Info(ex.Message.ToString());
                throw;
            }

        }
    }
}
