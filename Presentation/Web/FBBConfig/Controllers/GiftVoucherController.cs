using FBBConfig.Extensions;
using FBBConfig.Solid.CompositionRoot;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace FBBConfig.Controllers
{
    [IENoCache(Order = 1)]
    public class GiftVoucherController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        //private readonly IEntityRepository<FBB_VOUCHER_PIN> _VoucherTable;
        //private readonly IEntityRepository<FBB_VOUCHER_MASTER> _VoucherMasterTable;
        //private readonly IEntityRepository<FBB_CFG_LOV> _AllLov;
        private readonly ICommandHandler<GenerateGiftVoucherPINsCommand> _genPINCommandHandler;
        private readonly ICommandHandler<CreateNewVoucherProjectCommand> _createVoucherProjectCommandHandler;

        public GiftVoucherController(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<GenerateGiftVoucherPINsCommand> genPINCommandHandler,
            ICommandHandler<CreateNewVoucherProjectCommand> createVoucherProjectCommandHandler)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _genPINCommandHandler = genPINCommandHandler;
            _createVoucherProjectCommandHandler = createVoucherProjectCommandHandler;
        }

        //
        // GET: /GiftVoucher/
        [AuthorizeUserAttribute]
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            var LovDataScreen = base.LovData.Where(p => p.Type == "SCREEN").ToList();
            ViewBag.configscreen = LovDataScreen;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            ViewBag.User = base.CurrentUser;
            return View();
        }
        private List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
        {
            var data = base.LovData
               .Where(l => l.Type.Equals(fbbConstType))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }

        public ActionResult ShowGiftVoucherPINs([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchMapVoucherModel = new JavaScriptSerializer().Deserialize<GiftVoucherStr>(dataS);
                //var result = GetDataModel(searchMapVoucherModel);
                var result = GetPINLot(searchMapVoucherModel);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        //private List<GiftVoucherPINModels> GetDataModel(GiftVoucherStr genGiftVoucherParam)
        //{
        //    DateTime dttmp;
        //    Nullable<DateTime> dtstartdate = DateTime.TryParseExact(genGiftVoucherParam.start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp : (DateTime?)null;
        //    Nullable<DateTime> dtexpireddate = DateTime.TryParseExact(genGiftVoucherParam.expired_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp : (DateTime?)null;
        //    int inttmp = 0;
        //    int pintypetmp = Int32.TryParse(genGiftVoucherParam.pin_type, out inttmp) ? inttmp : 0;
        //    int pinlengthtmp = Int32.TryParse(genGiftVoucherParam.pin_length, out inttmp) ? inttmp : 12;
        //    int positiontmp = Int32.TryParse(genGiftVoucherParam.fixedPosition, out inttmp) ? inttmp - 1 : 0;
        //    int amountpuntmp = Int32.TryParse(genGiftVoucherParam.AmountPINs, out inttmp) ? inttmp : 0;

        //    var query = new GiftVoucherQuery()
        //    {
        //        pin_type = pintypetmp,
        //        pin_length = pinlengthtmp,
        //        exceptedChar = genGiftVoucherParam.exceptedChar,
        //        fixedChar = genGiftVoucherParam.fixedChar,
        //        fixedPosition = positiontmp,

        //        start_date = dtstartdate,
        //        expired_date = dtexpireddate,

        //        AmountPINs = amountpuntmp
        //    };
        //    List<GiftVoucherPINModels>  result = new List<GiftVoucherPINModels>();
        //    result = _queryProcessor.Execute(query);
        //    return result;
        //}

        public ActionResult ExportData(string dataS)
        {
            var searchMapVoucherModel = new JavaScriptSerializer().Deserialize<GiftVoucherStr>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            List<FBB_VOUCHER_PIN> listall = GetPINLot(searchMapVoucherModel);

            string filename = "Gift_Voucher_PIN";

            var bytes = GenerateEntitytoExcel<FBB_VOUCHER_PIN>(listall, filename);

            return File(bytes, "application/excel", filename + ".xlsx");
        }
        public JsonResult CheckGenSuccess(GiftVoucherStr genGiftVoucherParam)
        {
            long longtmp = 0;
            long amount = long.TryParse(genGiftVoucherParam.AmountPINs, out longtmp) ? longtmp : 0;
            var listPIN = GetPINLot(genGiftVoucherParam);
            if (listPIN.Count == amount)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        public List<FBB_VOUCHER_PIN> GetPINLot(GiftVoucherStr genGiftVoucherParam)
        {
            long longtmp = 0;
            long voucher_master_id = long.TryParse(genGiftVoucherParam.voucher_master_id, out longtmp) ? longtmp : 0;
            long lot = long.TryParse(genGiftVoucherParam.lot, out longtmp) ? longtmp : 0;

            var query = new GetGiftVoucherPINsQuery()
            {
                VOUCHER_MASTER_ID = voucher_master_id,
                Lot = lot,
                VOUCHER_PIN = null
            };
            var result = _queryProcessor.Execute(query);
            return result;
        }

        public JsonResult GenPINs(GiftVoucherStr genGiftVoucherParam)
        {
            DateTime dttmp;
            Nullable<DateTime> dtstartdate = DateTime.TryParseExact(genGiftVoucherParam.start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp : (DateTime?)null;
            Nullable<DateTime> dtexpireddate = DateTime.TryParseExact(genGiftVoucherParam.expired_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp : (DateTime?)null;
            int inttmp = 0;
            int pintypetmp = Int32.TryParse(genGiftVoucherParam.pin_type, out inttmp) ? inttmp : 0;
            int pinlengthtmp = Int32.TryParse(genGiftVoucherParam.pin_length, out inttmp) ? inttmp : 12;
            int amountpuntmp = Int32.TryParse(genGiftVoucherParam.AmountPINs, out inttmp) ? inttmp : 0;
            long longtmp = 0;
            long voucher_master_id = long.TryParse(genGiftVoucherParam.voucher_master_id, out longtmp) ? longtmp : 0;
            long lot = long.TryParse(genGiftVoucherParam.lot, out longtmp) ? longtmp : 0;
            genGiftVoucherParam.exceptedChar = string.IsNullOrEmpty(genGiftVoucherParam.exceptedChar) ? "" : genGiftVoucherParam.exceptedChar;

            var command = new GenerateGiftVoucherPINsCommand()
            {
                pin_type = pintypetmp,
                pin_length = pinlengthtmp,
                exceptedChar = genGiftVoucherParam.exceptedChar,
                fixedChar = genGiftVoucherParam.fixedChar,

                start_date = dtstartdate,
                expired_date = dtexpireddate,

                AmountPINs = amountpuntmp,

                voucher_master_id = voucher_master_id,
                lot = lot
            };
            _genPINCommandHandler.Handle(command);
            return Json(command.lot, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLastLot(GiftVoucherStr genGiftVoucherParam)
        {
            long longtmp = 0;
            long voucher_master_id = long.TryParse(genGiftVoucherParam.voucher_master_id, out longtmp) ? longtmp : 0;
            var query = new GetLastGiftVoucherLotByVoucherMasterIDQuery()
            {
                Voucher_Master_ID = voucher_master_id
            };
            long result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult VoucherProjectGroupDropDownList()
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            var LovData = masterController.GetLovList("FBB_CONSTANT").Where(p => p.LovValue5 == "FBBOR014" && p.Name == "VOUCHER_PROJECT_GROUP").OrderBy(p => p.Id).ToList();

            List<SelectListItem> result = new List<SelectListItem>();
            if (LovData.Any())
                result.Insert(0, new SelectListItem { Text = "กรุณาเลือก", Value = "", Selected = true });
            else
                result.Insert(0, new SelectListItem { Text = "", Value = "กรุณาเลือก" });
            foreach (LovValueModel tmp in LovData)
            {
                SelectListItem ListItem = new SelectListItem()
                {
                    Text = tmp.LovValue1,
                    Value = tmp.LovValue2
                };
                result.Add(ListItem);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PINTypeDropDownList()
        {
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            var LovData = masterController.GetLovList("FBB_CONSTANT").Where(p => p.LovValue5 == "FBBOR014" && p.Name == "VOUCHER_PIN_TYPE").OrderBy(p => p.Id).ToList();

            List<SelectListItem> result = new List<SelectListItem>();
            if (LovData.Any())
                result.Insert(0, new SelectListItem { Text = "กรุณาเลือก", Value = "", Selected = true });
            else
                result.Insert(0, new SelectListItem { Text = "", Value = "กรุณาเลือก" });
            int i = 0;
            foreach (LovValueModel tmp in LovData)
            {
                SelectListItem ListItem = new SelectListItem()
                {
                    Text = tmp.LovValue1,
                    Value = i.ToString()
                };
                result.Add(ListItem);
                i++;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVoucherProjectCode(string ProjectGroup, string ID)
        {
            long ltmp;
            if (long.TryParse(ID, out ltmp))
            {
                var query = new GetVoucherProjectDescriptionByGroupQuery()
                {
                    voucher_project_group = ProjectGroup
                };
                var dataList = _queryProcessor.Execute(query);
                var result = dataList.Where(t => t.VOUCHER_MASTER_ID == long.Parse(ID)).ToList();
                return Json(result[0].VOUCHER_PROJECT_CODE, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult VoucherDescriptionDropDownList(string ProjectGroup)
        {
            if (!string.IsNullOrEmpty(ProjectGroup))
            {
                var query = new GetVoucherProjectDescriptionByGroupQuery()
                {
                    voucher_project_group = ProjectGroup
                };
                var dataList = _queryProcessor.Execute(query);
                List<SelectListItem> result = new List<SelectListItem>();
                result.Insert(0, new SelectListItem { Text = "กรุณาเลือก", Value = "", Selected = true });
                foreach (FBB_VOUCHER_MASTER tmp in dataList)
                {
                    SelectListItem ListItem = new SelectListItem()
                    {
                        Text = tmp.VOUCHER_PROJECT_DES,
                        Value = tmp.VOUCHER_MASTER_ID.ToString()
                    };
                    result.Add(ListItem);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            List<SelectListItem> Emptyresult = new List<SelectListItem>();
            Emptyresult.Insert(0, new SelectListItem { Text = "กรุณาเลือก", Value = "" });
            return Json(Emptyresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateNewVoucherProject(string ProjectGroup, string ProjectDesc)
        {
            try
            {
                var query = new GetVoucherProjectDescriptionByGroupQuery()
                {
                    voucher_project_group = ProjectGroup
                };
                var dataList = _queryProcessor.Execute(query);
                long runningNo = dataList.Count() + 1;
                DateTime dt = DateTime.Now;
                string CurrYear = dt.ToString("yy");
                string CurrMonth = dt.ToString("MM");
                string ProjectCode = ProjectGroup + CurrYear + CurrMonth + runningNo.ToString("00000");

                var command = new CreateNewVoucherProjectCommand()
                {
                    voucher_project_group = ProjectGroup,
                    voucher_project_code = ProjectCode,
                    voucher_project_des = ProjectGroup + ": " + ProjectDesc
                };
                _createVoucherProjectCommandHandler.Handle(command);
                return Json(command.resultMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class GiftVoucherStr
    {
        public string pin_type { get; set; }
        public string pin_length { get; set; }
        public string exceptedChar { get; set; }
        public string fixedChar { get; set; }
        public string fixedPosition { get; set; }
        public string start_date { get; set; }
        public string expired_date { get; set; }
        public string AmountPINs { get; set; }
        public string voucher_master_id { get; set; }
        public string lot { get; set; }
    }
}
