using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.FTTx;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class FTTxController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FTTxCommand> _fttxCommand;
        //
        // GET: /FTTx/
        public FTTxController(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<FTTxCommand> fttxCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _fttxCommand = fttxCommand;
        }

        [AuthorizeUserAttribute]
        public ActionResult FTTxCoverage()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            return View();
        }

        public JsonResult Read([DataSourceRequest] DataSourceRequest request, string region = "", string province = "", string amphur = "", string tumbon = "", string ownerProduct = "", string ownerType = "", string tower_th = "")
        {
            //if (region == "" && province == "" && amphur == "" && ownerProduct == "" && ownerType == "")
            //    return Json("");

            var query = new GetCoverageRegionQuery
            {
                Region = region,
                Province = province,
                Amphur = amphur,
                Tumbon = tumbon,
                OwnerProduct = ownerProduct,
                OwnerType = ownerType,
                tower_th = tower_th



            };
            var data = _queryProcessor.Execute(query).OrderByDescending(s => s.UPDATE_DATE);
            return Json(data.ToDataSourceResult(request));
        }

        public JsonResult FTTxCommand(string action = "", string ownerProduct = "", string ownerType = "", string province = "", string amphur = "", string tumbon = "",
            string province_en = "", string amphur_en = "", string tumbon_en = "", string zipcode = "",
            string groupAmphur = "", string oldOwnerProduct = "", string oldOwnerType = "", string tower_th = "", string tower_en = "",
            string Service_Type = "", string tagetdate_ex = "", string targetdate_in = "", string status = "", string lat = "", string lon = "", decimal FTTX = 0)
        {
            try
            {

                var command = new FTTxCommand
                {
                    Action = action,
                    OwnerProduct = ownerProduct,
                    OwnerType = ownerType,
                    Province = province,
                    Amphur = amphur,
                    Service_Type = Service_Type,

                    GroupAmphur = groupAmphur,
                    OldOwnerProduct = oldOwnerProduct,
                    OldOwnerType = oldOwnerType,
                    tower_th = tower_th,
                    tower_en = tower_en,
                    Fttx_id = FTTX,

                    Username = base.CurrentUser.UserName,

                    Tumbon = tumbon,
                    ProvinceEN = province_en,
                    AmphurEN = amphur_en,
                    TumbonEN = tumbon_en,

                    tagetdate_ex = tagetdate_ex,
                    targetdate_in = targetdate_in,
                    status = status,
                    lat = lat,
                    lon = lon,
                    zipcode = zipcode

                };

                ///  _fttxCommand.Handle(command);
                if ((command.Action == "Create" || command.Action == "Update" || command.Action == "Delete") && command.FlagDup != true)
                {
                    ///command.Action = "Package";
                    _fttxCommand.Handle(command);
                }

                if (command.FlagDup == true)
                    return Json("dup", JsonRequestBehavior.AllowGet);
                else
                    return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCountCoverage(string region = "", string province = "", string amphur = "", string tumbon = "", string ownerProduct = "", string ownerType = "", string tower_th = "")
        {
            //if (region == "" && province == "" && amphur == "" && ownerProduct == "" && ownerType == "")
            //    return Json(new CountCoverageModel { Total = 0, NSN = 0, SIMAT = 0 }, JsonRequestBehavior.AllowGet);

            var query = new GetCountCoverageQuery
            {
                Region = region,
                Province = province,
                Amphur = amphur,
                Tumbon = tumbon,
                OwnerProduct = ownerProduct,
                OwnerType = ownerType,
                tower_th = tower_th,


            };
            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcel(string region = "", string province = "", string amphur = "", string tumbon = "", string ownerProduct = "", string ownerType = "", string tower_th = "", string tower_en = "", string ServiceType = "", string tagetdate_ex = "", string targetdate_in = "", string status = "", string lat = "", string lon = "")
        {
            try
            {
                var query = new GetCoverageRegionQuery
                {
                    Region = region,
                    Province = province,
                    Amphur = amphur,
                    Tumbon = tumbon,
                    OwnerProduct = ownerProduct,
                    OwnerType = ownerType,
                    tower_th = tower_th,
                };
                var data = _queryProcessor.Execute(query);

                var dataExcel = (from r in data
                                 select new GridFTTxExcelModel
                                 {
                                     RegionCode = r.RegionCode,
                                     Province = r.Province,
                                     Amphur = r.Amphur,
                                     OwnerProduct = r.OwnerProduct,
                                     OwnerType = r.OwnerType,
                                     SERVICE_TYPE = r.SERVICE_TYPE,
                                     tower_th = r.tower_th,
                                     tower_en = r.tower_en,
                                     ONTARGET_DATE_EX = String.Format("{0:d/MM/yyyy}", r.ONTARGET_DATE_EX),
                                     ONTARGET_DATE_IN = String.Format("{0:d/MM/yyyy}", r.ONTARGET_DATE_IN),
                                     Tumbon = r.Tumbon
                                 }).ToList();

                var conditions = "";

                if (region != "") conditions = region + "_";
                if (province != "") conditions += province + "_";
                if (amphur != "") conditions += amphur + "_";
                if (ownerProduct != "") conditions += ownerProduct + "_";
                if (ownerType != "") conditions += ownerType + "_";
                /// if (ServiceType != "") conditions += +SERVICE_TYPE+ "_";
                if (ServiceType != "" && ServiceType != "กรุณาเลือก") conditions += ServiceType + "_";
                if (tower_th != "") conditions += tower_th + "_";
                if (tower_en != "") conditions += tower_en + "_";

                string filename = "FTTx_Coverage_" + conditions + String.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);

                var bytes = GenerateEntitytoExcel<GridFTTxExcelModel>(dataExcel, filename);

                return File(bytes, "application/excel", filename + ".xlsx");
            }
            catch (Exception ex)
            {
                return Json("Message: " + ex.Message + "InnerException: " + ex.InnerException, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
