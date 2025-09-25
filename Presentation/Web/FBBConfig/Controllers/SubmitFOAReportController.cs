using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;


/* change history
 *ch0001 21/01/2020 --Update Status-- เพิ่ม function SetDDLEditStatus
 *ch0002 09/03/2020 --Update Status-- เพื่ม Update status data SubmitFOAEquipment
 */

namespace FBBConfig.Controllers
{
    public class SubmitFOAReportController : FBBConfigController
    {
        #region Properties
        private readonly IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> _queryProcessorGoodsMovementHandler;
        private readonly ICommandHandler<UpdateSubmitFoaErrorLogCommand> _UpdateSubmitFoaError;
        private readonly ICommandHandler<SendMailLastMileNotificationCommand> _sendMail;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFOAResendStatusCommand> _updateFOAStatusTypeCommand;
        private readonly ICommandHandler<SumitFOAErrorLogCommand> _SumitFOAErrorLogCommand;
        static SubmitFOAEquipmentListReturn resultEquip;
        static IEnumerable<SubmitFOAEquipment> newResultList;

        ////private readonly IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> _tran;
        #endregion

        #region Constructor

        public SubmitFOAReportController(ILogger logger
                                       , IQueryProcessor queryProcessor
                                       , ICommandHandler<UpdateFOAResendStatusCommand> updateFOAStatusTypeCommand
            , ICommandHandler<SumitFOAErrorLogCommand> SumitFOAErrorLogCommand
            , ICommandHandler<SendMailLastMileNotificationCommand> sendMail)

        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _updateFOAStatusTypeCommand = updateFOAStatusTypeCommand;
            _sendMail = sendMail;
            _SumitFOAErrorLogCommand = SumitFOAErrorLogCommand;
        }

        #endregion

        #region ActionResult

        public ActionResult Configuration(string page)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLovV2("FBBPAYG_SCREEN", page);
            ViewBag.UserMainAsset = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "USERMAINASSET").FirstOrDefault();

            return null;
        }

        private void SetViewBagLovV2(string screenType, string LovValue5)
        {
            var query = new GetLovV2Query()
            {
                LovType = screenType,
                LovVal5 = LovValue5

            };
            var LovDataScreen = _queryProcessor.Execute(query).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        public ActionResult Index()
        {
            resultEquip = null;
            this.Configuration("REPORTSLADTL");
            Session["TempSearchCriteria"] = null;
            return View();
        }

        public ActionResult ClearListEquip()
        {
            resultEquip = null;
            newResultList = null;
            return Json("Success");
        }

        // Search Equipment
        public ActionResult SubmitFOAEquipmentReportAsync([DataSourceRequest] DataSourceRequest request, string firstCheck, string Resend, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                //string activetflag = "";
                var searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAEquipmentReportQuery>(dataS);
                Session["TempSearchCriteria"] = searchModel;
                //var resuftlov = Get_FBSS_CONFIG_TBL_LOV("RESEND_ORDER", "PAGESIZE").FirstOrDefault(l => l.ACTIVEFLAG == "Y");
                //activetflag = resuftlov is null ? "N" : "Y";
                if (searchModel.companyCode == "ALL" && searchModel.dateFrom == "" && searchModel.dateTo == "" && searchModel.errormessage == "ALL" && searchModel.internetNo == "ALL" && searchModel.materialCode == "ALL" && searchModel.orderNo == "ALL" && searchModel.orderType == "ALL" &&
                         searchModel.plant == "ALL" && searchModel.productName == "ALL" && searchModel.serviceName == "" && searchModel.status == "ALL" && searchModel.storLocation == "ALL" && searchModel.subcontractorCode == "ALL" && searchModel.productOwner == "ALL")
                {
                    resultEquip = null;
                    newResultList = null;
                    return null;
                }

                #region Resend order Deploy 18/11/2021(1)
                //if (activetflag == "N")  //กรณี activeflag = n
                //{
                //    newResultList = null;
                //    resultEquip = GetSubmitFOAEquipment(searchModel);
                //    newResultList = GetSubmitFOAEquipmentConvertCurToIEnumerable(resultEquip.cur);
                //}
                //else if (activetflag == "Y" && firstCheck == "EQUIPMENT") //check = Y ให้ load ใหม่ทุกครั้ง
                //{
                //resultEquip = GetSubmitFOAEquipment(searchModel);
                //newResultList = GetSubmitFOAEquipmentConvertCurToIEnumerable(resultEquip.cur);
                // }
                #endregion

                if (firstCheck == "EQUIPMENT") //ค้นหาเพื่อเก็บเข้า List
                {
                    resultEquip = GetSubmitFOAEquipment(searchModel);
                    newResultList = GetSubmitFOAEquipmentConvertCurToIEnumerable(resultEquip.cur);
                }
                else if (Resend == "Resend")
                {
                    searchModel = Session["TempSearchCriteria"] as SubmitFOAEquipmentReportQuery;
                    newResultList = null;
                    resultEquip = GetSubmitFOAEquipment(searchModel);
                    newResultList = GetSubmitFOAEquipmentConvertCurToIEnumerable(resultEquip.cur);
                }

                if (resultEquip is null) return null;

                //เรียงข้อมูลใหม่ ให้เป็น group// begin
                //if (activetflag == "Y" && firstCheck == "EQUIPMENT")
                //    newResultList = GetSubmitFOAEquipmentConvertCurToIEnumerable(resultEquip.cur);
                #region group begin old

                ////var resultEquip = this.GetSubmitFOAEquipment(searchModel);
                //List<SubmitFOAEquipment> newResultList = new List<SubmitFOAEquipment>();
                //foreach (var item in resultEquip.cur)
                //{
                //    SubmitFOAEquipment newResult = new SubmitFOAEquipment();

                //    var checkList = newResultList.Any(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                //        && x.SUBCONTRACT_CODE == item.SUBCONTRACT_CODE && x.SUBCONTRACT_NAME == item.SUBCONTRACT_NAME
                //        && x.PRODUCT_NAME == item.PRODUCT_NAME && x.ORDER_TYPE == item.ORDER_TYPE && x.SUBMIT_DATE == item.SUBMIT_DATE
                //        && x.STATUS == item.STATUS);

                //    if (!checkList)
                //    {
                //        newResult.ACCESS_NUMBER = item.ACCESS_NUMBER;
                //        newResult.ORDER_NO = item.ORDER_NO;
                //        newResult.SUBCONTRACT_CODE = item.SUBCONTRACT_CODE;
                //        newResult.SUBCONTRACT_NAME = item.SUBCONTRACT_NAME;
                //        newResult.PRODUCT_NAME = item.PRODUCT_NAME;
                //        newResult.ORDER_TYPE = item.ORDER_TYPE;
                //        newResult.SUBMIT_DATE = item.SUBMIT_DATE;
                //        newResult.SN = item.SN == "" || item.SN == null ? "&nbsp;" : item.SN;
                //        newResult.MATERIAL_CODE = item.MATERIAL_CODE == "" || item.MATERIAL_CODE == null ? "&nbsp;" : item.MATERIAL_CODE;
                //        newResult.COMPANY_CODE = item.COMPANY_CODE == "" || item.COMPANY_CODE == null ? "&nbsp;" : item.COMPANY_CODE;
                //        newResult.PLANT = item.PLANT == "" || item.PLANT == null ? "&nbsp;" : item.PLANT;
                //        newResult.STORAGE_LOCATION = item.STORAGE_LOCATION == "" || item.STORAGE_LOCATION == null ? "&nbsp;" : item.STORAGE_LOCATION;
                //        newResult.SN_TYPE = item.SN_TYPE == "" || item.SN_TYPE == null ? "&nbsp;" : item.SN_TYPE;
                //        newResult.MOVEMENT_TYPE = item.MOVEMENT_TYPE == "" || item.MOVEMENT_TYPE == null ? "&nbsp;" : item.MOVEMENT_TYPE;
                //        newResult.ERR_CODE = item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE;
                //        newResult.ERR_MSG = item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG;
                //        newResult.MOVEMENT_TYPE_OUT = item.MOVEMENT_TYPE_OUT == "" || item.MOVEMENT_TYPE_OUT == null ? "&nbsp;" : item.MOVEMENT_TYPE_OUT;
                //        newResult.MOVEMENT_TYPE_IN = item.MOVEMENT_TYPE_IN == "" || item.MOVEMENT_TYPE_IN == null ? "&nbsp;" : item.MOVEMENT_TYPE_IN;
                //        newResult.STATUS = item.STATUS;
                //        newResult.SERVICE_NAME = item.SERVICE_NAME;
                //        newResult.REJECT_REASON = item.REJECT_REASON;
                //        newResult.OLT_NAME = item.OLT_NAME;
                //        newResult.BUILDING_NAME = item.BUILDING_NAME;
                //        newResult.MOBILE_CONTACT = item.MOBILE_CONTACT;
                //        newResultList.Add(newResult);
                //    }
                //    else
                //    {
                //        var whereResult = newResultList.First(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                //        && x.SUBCONTRACT_CODE == item.SUBCONTRACT_CODE && x.SUBCONTRACT_NAME == item.SUBCONTRACT_NAME
                //        && x.PRODUCT_NAME == item.PRODUCT_NAME && x.ORDER_TYPE == item.ORDER_TYPE && x.SUBMIT_DATE == item.SUBMIT_DATE
                //        && x.STATUS == item.STATUS);

                //        whereResult.SN = whereResult.SN + "<br/>" + (item.SN == "" || item.SN == null ? "&nbsp;" : item.SN);
                //        whereResult.MATERIAL_CODE = whereResult.MATERIAL_CODE + "<br/>" + (item.MATERIAL_CODE == "" || item.MATERIAL_CODE == null ? "&nbsp;" : item.MATERIAL_CODE);
                //        whereResult.COMPANY_CODE = whereResult.COMPANY_CODE + "<br/>" + (item.COMPANY_CODE == "" || item.COMPANY_CODE == null ? "&nbsp;" : item.COMPANY_CODE);
                //        whereResult.PLANT = whereResult.PLANT + "<br/>" + (item.PLANT == "" || item.PLANT == null ? "&nbsp;" : item.PLANT);
                //        whereResult.STORAGE_LOCATION = whereResult.STORAGE_LOCATION + "<br/>" + (item.STORAGE_LOCATION == "" || item.STORAGE_LOCATION == null ? "&nbsp;" : item.STORAGE_LOCATION);
                //        whereResult.SN_TYPE = whereResult.SN_TYPE + "<br/>" + (item.SN_TYPE == "" || item.SN_TYPE == null ? "&nbsp;" : item.SN_TYPE);
                //        whereResult.MOVEMENT_TYPE = whereResult.MOVEMENT_TYPE + "<br/>" + (item.MOVEMENT_TYPE == "" || item.MOVEMENT_TYPE == null ? "&nbsp;" : item.MOVEMENT_TYPE);
                //        whereResult.ERR_CODE = whereResult.ERR_CODE + "<br/>" + (item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE);
                //        whereResult.ERR_MSG = whereResult.ERR_MSG + "<br/>" + (item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG);
                //        whereResult.SERVICE_NAME = whereResult.SERVICE_NAME + "<br/>" + (item.SERVICE_NAME == "" || item.SERVICE_NAME == null ? "&nbsp;" : item.SERVICE_NAME);
                //        whereResult.REJECT_REASON = whereResult.REJECT_REASON + "<br/>" + (item.REJECT_REASON == "" || item.REJECT_REASON == null ? "&nbsp;" : item.REJECT_REASON);
                //        whereResult.OLT_NAME = whereResult.OLT_NAME + "<br/>" + (item.OLT_NAME == "" || item.OLT_NAME == null ? "&nbsp;" : item.OLT_NAME);
                //        whereResult.BUILDING_NAME = whereResult.BUILDING_NAME + "<br/>" + (item.BUILDING_NAME == "" || item.BUILDING_NAME == null ? "&nbsp;" : item.BUILDING_NAME);
                //        whereResult.MOBILE_CONTACT = whereResult.MOBILE_CONTACT + "<br/>" + (item.MOBILE_CONTACT == "" || item.MOBILE_CONTACT == null ? "&nbsp;" : item.MOBILE_CONTACT);
                //        whereResult.MOVEMENT_TYPE_OUT = whereResult.MOVEMENT_TYPE_OUT + "<br/>" + (item.MOVEMENT_TYPE_OUT == "" || item.MOVEMENT_TYPE_OUT == null ? "&nbsp;" : item.MOVEMENT_TYPE_OUT);
                //        whereResult.MOVEMENT_TYPE_IN = whereResult.MOVEMENT_TYPE_IN + "<br/>" + (item.MOVEMENT_TYPE_IN == "" || item.MOVEMENT_TYPE_IN == null ? "&nbsp;" : item.MOVEMENT_TYPE_IN);
                //    }
                //}
                #endregion
                //end
                if (newResultList.Any())
                {
                    #region Resend order Deploy 18/11/2021(2)
                    //if (resuftlov is null)
                    //{
                    //	decimal _total = resultEquip.cur[0].CNT;
                    //	return Json(new
                    //	{
                    //		Data = newResultList,
                    //		Total = _total,
                    //	});
                    //}
                    //else
                    //{
                    //	return Json(newResultList.ToDataSourceResult(request));
                    //}
                    #endregion

                    return Json(newResultList.ToDataSourceResult(request));
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        #region Group Equipment by Linq
        private IEnumerable<SubmitFOAEquipment> GetSubmitFOAEquipmentConvertCurToIEnumerable(List<SubmitFOAEquipment> cur)
        {

            //order by o.access_number, o.order_no , o.foa_submit_date
            var query = from item in cur.AsEnumerable()
                        orderby item.ACCESS_NUMBER, item.ORDER_NO, item.SUBMIT_DATE
                        select new
                        {
                            ACCESS_NUMBER = item.ACCESS_NUMBER,
                            ORDER_NO = item.ORDER_NO,
                            SUBCONTRACT_CODE = item.SUBCONTRACT_CODE,
                            SUBCONTRACT_NAME = item.SUBCONTRACT_NAME,
                            PRODUCT_NAME = item.PRODUCT_NAME,
                            ORDER_TYPE = item.ORDER_TYPE,
                            SUBMIT_DATE = item.SUBMIT_DATE,
                            SN = item.SN == "" || item.SN == null ? "&nbsp;" : item.SN,
                            MATERIAL_CODE = item.MATERIAL_CODE == "" || item.MATERIAL_CODE == null ? "&nbsp;" : item.MATERIAL_CODE,
                            COMPANY_CODE = item.COMPANY_CODE == "" || item.COMPANY_CODE == null ? "&nbsp;" : item.COMPANY_CODE,
                            PLANT = item.PLANT == "" || item.PLANT == null ? "&nbsp;" : item.PLANT,
                            STORAGE_LOCATION = item.STORAGE_LOCATION == "" || item.STORAGE_LOCATION == null ? "&nbsp;" : item.STORAGE_LOCATION,
                            SN_TYPE = item.SN_TYPE == "" || item.SN_TYPE == null ? "&nbsp;" : item.SN_TYPE,
                            MOVEMENT_TYPE = item.MOVEMENT_TYPE == "" || item.MOVEMENT_TYPE == null ? "&nbsp;" : item.MOVEMENT_TYPE,
                            ERR_CODE = item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE,
                            ERR_MSG = item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG,
                            MOVEMENT_TYPE_OUT = item.MOVEMENT_TYPE_OUT == "" || item.MOVEMENT_TYPE_OUT == null ? "&nbsp;" : item.MOVEMENT_TYPE_OUT,
                            MOVEMENT_TYPE_IN = item.MOVEMENT_TYPE_IN == "" || item.MOVEMENT_TYPE_IN == null ? "&nbsp;" : item.MOVEMENT_TYPE_IN,
                            STATUS = item.STATUS,
                            SERVICE_NAME = item.SERVICE_NAME,
                            REJECT_REASON = item.REJECT_REASON,
                            OLT_NAME = item.OLT_NAME,
                            BUILDING_NAME = item.BUILDING_NAME,
                            MOBILE_CONTACT = item.MOBILE_CONTACT,
                            PROCUCT_OWNER = item.PRODUCT_OWNER,
                            MAIN_PROMO_CODE = item.MAIN_PROMO_CODE,
                            TRANS_ID = item.TRANS_ID,
                            TEAM_ID = item.TEAM_ID

                        };

            var retSubmitFOAEquipment = query.GroupBy(cc => new
            {
                cc.ACCESS_NUMBER,
                cc.ORDER_NO,
                cc.SUBCONTRACT_CODE,
                cc.SUBCONTRACT_NAME,
                cc.PRODUCT_NAME,
                cc.ORDER_TYPE,
                cc.SUBMIT_DATE,
                cc.STATUS,
                cc.SERVICE_NAME,
                cc.REJECT_REASON,
                cc.OLT_NAME,
                cc.BUILDING_NAME,
                cc.MOBILE_CONTACT,
                cc.PROCUCT_OWNER
            }).Select(dd =>
                new SubmitFOAEquipment
                {
                    ACCESS_NUMBER = dd.Key.ACCESS_NUMBER,
                    ORDER_NO = dd.Key.ORDER_NO,
                    SUBCONTRACT_CODE = dd.Key.SUBCONTRACT_CODE,
                    SUBCONTRACT_NAME = dd.Key.SUBCONTRACT_NAME,
                    PRODUCT_NAME = dd.Key.PRODUCT_NAME,
                    ORDER_TYPE = dd.Key.ORDER_TYPE,
                    SUBMIT_DATE = dd.Key.SUBMIT_DATE,
                    SN = string.Join("<br/>", dd.Select(s => s.SN).ToList()),
                    MATERIAL_CODE = string.Join("<br/>", dd.Select(s => s.MATERIAL_CODE).ToList()),
                    COMPANY_CODE = string.Join("<br/>", dd.Select(s => s.COMPANY_CODE).ToList()),
                    PLANT = string.Join("<br/>", dd.Select(s => s.PLANT).ToList()),
                    STORAGE_LOCATION = string.Join("<br/>", dd.Select(s => s.STORAGE_LOCATION).ToList()),
                    SN_TYPE = string.Join("<br/>", dd.Select(s => s.SN_TYPE).ToList()),
                    MOVEMENT_TYPE = string.Join("<br/>", dd.Select(s => s.MOVEMENT_TYPE).ToList()),
                    ERR_CODE = string.Join("<br/>", dd.Select(s => s.ERR_CODE).ToList()),
                    ERR_MSG = string.Join("<br/>", dd.Select(s => s.ERR_MSG).ToList()),
                    MOVEMENT_TYPE_OUT = string.Join("<br/>", dd.Select(s => s.MOVEMENT_TYPE_OUT).ToList()),
                    MOVEMENT_TYPE_IN = string.Join("<br/>", dd.Select(s => s.MOVEMENT_TYPE_IN).ToList()),
                    STATUS = dd.Key.STATUS,
                    SERVICE_NAME = dd.Key.SERVICE_NAME,
                    REJECT_REASON = dd.Key.REJECT_REASON,
                    OLT_NAME = dd.Key.OLT_NAME,
                    BUILDING_NAME = dd.Key.BUILDING_NAME,
                    MOBILE_CONTACT = dd.Key.MOBILE_CONTACT,
                    PRODUCT_OWNER = dd.Key.PROCUCT_OWNER,

                    TRANS_ID = string.Join("<br/>", dd.Select(s => s.TRANS_ID).ToList()),
                    MAIN_PROMO_CODE = string.Join("<br/>", dd.Select(s => s.MAIN_PROMO_CODE).ToList()),
                    TEAM_ID = string.Join("<br/>", dd.Select(s => s.TEAM_ID).ToList())
                });


            return retSubmitFOAEquipment.ToList();

        }

        #endregion

        // Search Installation
        public ActionResult SubmitFOAInstallationReportAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAInstallationReportQuery>(dataS);
                var resultInstall = this.GetSubmitFOAInstallation(searchModel);

                return Json(resultInstall.ToDataSourceResult(request));
            }
            else
            {
                return null;
            }
        }

        //begin 16.07.2018 main asset
        public ActionResult SubmitFOAMainAssetAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAMainAssetQuery>(dataS);
                searchModel.internetNo = string.IsNullOrEmpty(searchModel.internetNo.ToSafeString()) ? "ALL" : searchModel.internetNo.ToSafeString();
                searchModel.orderNo = string.IsNullOrEmpty(searchModel.orderNo.ToSafeString()) ? "ALL" : searchModel.orderNo.ToSafeString();
                searchModel.companyCode = string.IsNullOrEmpty(searchModel.companyCode.ToSafeString()) ? "ALL" : searchModel.companyCode.ToSafeString();
                searchModel.assetClass = string.IsNullOrEmpty(searchModel.assetClass.ToSafeString()) ? "ALL" : searchModel.assetClass.ToSafeString();
                searchModel.status = string.IsNullOrEmpty(searchModel.status.ToSafeString()) ? "ALL" : searchModel.status.ToSafeString();
                var resultMainAsset = this.GetSubmitFOAMainAsset(searchModel);

                return Json(resultMainAsset.ToDataSourceResult(request));
            }
            else
            {
                return null;
            }
        }
        //end 16.07.2018 main asset
        //begin 05.11.2018 revalue
        public ActionResult SubmitFOARevalueAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchModel = new JavaScriptSerializer().Deserialize<SubmitFOARevalueQuery>(dataS);
                searchModel.internetNo = string.IsNullOrEmpty(searchModel.internetNo.ToSafeString()) ? "ALL" : searchModel.internetNo.ToSafeString();
                searchModel.orderNo = string.IsNullOrEmpty(searchModel.orderNo.ToSafeString()) ? "ALL" : searchModel.orderNo.ToSafeString();
                searchModel.companyCode = string.IsNullOrEmpty(searchModel.companyCode.ToSafeString()) ? "ALL" : searchModel.companyCode.ToSafeString();
                searchModel.mainasset = string.IsNullOrEmpty(searchModel.mainasset.ToSafeString()) ? "ALL" : searchModel.mainasset.ToSafeString();
                searchModel.action = string.IsNullOrEmpty(searchModel.action.ToSafeString()) ? "ALL" : searchModel.action.ToSafeString();
                searchModel.status = string.IsNullOrEmpty(searchModel.status.ToSafeString()) ? "ALL" : searchModel.status.ToSafeString();
                searchModel.errormessage = string.IsNullOrEmpty(searchModel.errormessage.ToSafeString()) ? "ALL" : searchModel.errormessage.ToSafeString();
                //เรียงข้อมูลใหม่ ให้เป็น group// begin
                List<SubmitFOARevalue> newResultList = new List<SubmitFOARevalue>();
                var result = this.GetSubmitFOARevalue(searchModel);
                foreach (var item in result)
                {
                    SubmitFOARevalue newResult = new SubmitFOARevalue();

                    var checkList = newResultList.Any(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                        && x.STATUS == item.STATUS);

                    if (!checkList)
                    {
                        newResult.ACCESS_NUMBER = item.ACCESS_NUMBER;
                        newResult.ORDER_NO = item.ORDER_NO;
                        newResult.RUN_GROUP = item.RUN_GROUP == "" || item.RUN_GROUP == null ? "&nbsp;" : item.RUN_GROUP;
                        newResult.ACTION = item.ACTION == "" || item.ACTION == null ? "&nbsp;" : item.ACTION;
                        newResult.MAIN_ASSET = item.MAIN_ASSET == "" || item.MAIN_ASSET == null ? "&nbsp;" : item.MAIN_ASSET;
                        newResult.SUB_NUMBER = item.SUB_NUMBER == "" || item.SUB_NUMBER == null ? "&nbsp;" : item.SUB_NUMBER;
                        newResult.COM_CODE = item.COM_CODE == "" || item.COM_CODE == null ? "&nbsp;" : item.COM_CODE;
                        newResult.DOC_DATE = item.DOC_DATE == "" || item.DOC_DATE == null ? "&nbsp;" : item.DOC_DATE;
                        newResult.ERR_CODE = item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE;
                        newResult.ERR_MSG = item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG;
                        newResult.STATUS = item.STATUS;
                        newResult.PRODUCT_OWNER = item.PRODUCT_OWNER;
                        newResultList.Add(newResult);
                    }
                    else
                    {
                        var whereResult = newResultList.First(x => x.ACCESS_NUMBER == item.ACCESS_NUMBER && x.ORDER_NO == item.ORDER_NO
                         && x.STATUS == item.STATUS);

                        whereResult.RUN_GROUP = whereResult.RUN_GROUP + "<br/>" + (item.RUN_GROUP == "" || item.RUN_GROUP == null ? "&nbsp;" : item.RUN_GROUP);
                        whereResult.ACTION = whereResult.ACTION + "<br/>" + (item.ACTION == "" || item.ACTION == null ? "&nbsp;" : item.ACTION);
                        whereResult.MAIN_ASSET = whereResult.MAIN_ASSET + "<br/>" + (item.MAIN_ASSET == "" || item.MAIN_ASSET == null ? "&nbsp;" : item.MAIN_ASSET);
                        whereResult.SUB_NUMBER = whereResult.SUB_NUMBER + "<br/>" + (item.SUB_NUMBER == "" || item.SUB_NUMBER == null ? "&nbsp;" : item.SUB_NUMBER);
                        whereResult.COM_CODE = whereResult.COM_CODE + "<br/>" + (item.COM_CODE == "" || item.COM_CODE == null ? "&nbsp;" : item.COM_CODE);
                        whereResult.DOC_DATE = whereResult.DOC_DATE + "<br/>" + (item.DOC_DATE == "" || item.DOC_DATE == null ? "&nbsp;" : item.DOC_DATE);
                        whereResult.ERR_CODE = whereResult.ERR_CODE + "<br/>" + (item.ERR_CODE == "" || item.ERR_CODE == null ? "&nbsp;" : item.ERR_CODE);
                        whereResult.ERR_MSG = whereResult.ERR_MSG + "<br/>" + (item.ERR_MSG == "" || item.ERR_MSG == null ? "&nbsp;" : item.ERR_MSG);

                    }
                }

                //end



                return Json(newResultList.ToDataSourceResult(request));
                //return null;
            }
            else
            {
                return null;
            }
        }
        //end  05.11.2018 revalue
        // Get ProductList
        public ActionResult GetProductList([DataSourceRequest] DataSourceRequest request, GetProductListQuery model)
        {
            var result = _queryProcessor.Execute(model);
            return Json(result.InstallList.ToDataSourceResult(request));
        }

        #endregion

        #region Method private

        // Get LOV ViewBag
        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        #endregion

        #region Method public

        // Get Equipment from package
        public SubmitFOAEquipmentListReturn GetSubmitFOAEquipment(SubmitFOAEquipmentReportQuery model)
        {
            try
            {
                var result = _queryProcessor.Execute(model);



                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new SubmitFOAEquipmentListReturn();
            }
        }

        // Get Installation from package
        public List<SubmitFOAInstallation> GetSubmitFOAInstallation(SubmitFOAInstallationReportQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<SubmitFOAInstallation>();
            }
        }

        // Get Mainasset from package
        public List<SubmitFOAMainAsset> GetSubmitFOAMainAsset(SubmitFOAMainAssetQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<SubmitFOAMainAsset>();
            }
        }
        public List<SubmitFOARevalue> GetSubmitFOARevalue(SubmitFOARevalueQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<SubmitFOARevalue>();
            }
        }
        //// Get All Equipment from package 
        ////public List<SubmitFOAEquipment> GetSubmitFOA(SubmitFOAEquipmentReportQuery model)
        ////{
        ////    try
        ////    {
        ////        return _queryProcessor.Execute(model);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        _Logger.Info(ex.GetErrorMessage());
        ////        return new List<SubmitFOAEquipment>();
        ////    }
        ////}

        public SubmitFOAEquipmentListReturn GetSubmitFOA(SubmitFOAEquipmentReportQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new SubmitFOAEquipmentListReturn();
            }
        }

        // Group Equipment Multi
        public List<SubmitFOAEquipmentReport> GroupEquipment(List<SubmitFOAEquipment> dataS)
        {
            var result = new List<SubmitFOAEquipmentReport>();

            if (dataS.Count > 0)
            {

                foreach (var item in dataS.Where(w => w.STATUS != null && (w.STATUS.ToUpper() == "ERROR" || w.STATUS.ToUpper() == "PENDING")).GroupBy(g => new { g.ACCESS_NUMBER, g.ORDER_NO }))

                {
                    var EquipList = new SubmitFOAEquipmentReport();
                    //try
                    //{
                    var SubmitFOAEquipmentData = dataS.Where(p => p.ACCESS_NUMBER == item.FirstOrDefault().ACCESS_NUMBER && p.ORDER_NO == item.FirstOrDefault().ORDER_NO)
                                                       .Select(p =>
                                                       {
                                                           return new SubmitFOAEquipment
                                                           {
                                                               ACCESS_NUMBER = p.ACCESS_NUMBER,
                                                               ORDER_NO = p.ORDER_NO,
                                                               SERIAL_NUMBER = p.SN,
                                                               ORDER_TYPE = p.ORDER_TYPE,
                                                               SUBCONTRACT_CODE = p.SUBCONTRACT_CODE,
                                                               SUBCONTRACT_NAME = p.SUBCONTRACT_NAME,
                                                               PRODUCT_NAME = p.PRODUCT_NAME,
                                                               SERVICE_NAME = p.SERVICE_NAME,
                                                               SUBMIT_FLAG = p.SUBMIT_FLAG,
                                                               REJECT_REASON = p.REJECT_REASON,
                                                               SUBMIT_DATE = p.SUBMIT_DATE,
                                                               OLT_NAME = p.OLT_NAME,
                                                               BUILDING_NAME = p.BUILDING_NAME,
                                                               MOBILE_CONTACT = p.MATERIAL_CODE,
                                                               SN = p.SN,
                                                               MATERIAL_CODE = p.MATERIAL_CODE,
                                                               COMPANY_CODE = p.COMPANY_CODE,
                                                               PLANT = p.PLANT,
                                                               STORAGE_LOCATION = p.STORAGE_LOCATION,
                                                               SN_TYPE = p.SN_TYPE,
                                                               MOVEMENT_TYPE = p.MOVEMENT_TYPE,
                                                               MOVEMENT_TYPE_OUT = p.MOVEMENT_TYPE_OUT,
                                                               MOVEMENT_TYPE_IN = p.MOVEMENT_TYPE_IN,
                                                               STATUS = p.STATUS,
                                                               ERR_CODE = p.ERR_CODE,
                                                               ERR_MSG = p.ERR_MSG,
                                                               PRODUCT_OWNER = p.PRODUCT_OWNER,
                                                               MAIN_PROMO_CODE = p.MAIN_PROMO_CODE,
                                                               TRANS_ID = p.TRANS_ID,
                                                               TEAM_ID = p.TEAM_ID
                                                           };
                                                       }).ToList();

                    EquipList.ACCESS_NUMBER = item.FirstOrDefault().ACCESS_NUMBER;
                    EquipList.ORDER_NO = item.FirstOrDefault().ORDER_NO;
                    EquipList.SERIAL_NUMBER = item.FirstOrDefault().SN;
                    EquipList.ORDER_TYPE = item.FirstOrDefault().ORDER_TYPE;
                    EquipList.SUBCONTRACT_CODE = item.FirstOrDefault().SUBCONTRACT_CODE;
                    EquipList.SUBCONTRACT_NAME = item.FirstOrDefault().SUBCONTRACT_NAME;
                    EquipList.PRODUCT_NAME = item.FirstOrDefault().PRODUCT_NAME;
                    EquipList.SERVICE_NAME = item.FirstOrDefault().SERVICE_NAME;
                    EquipList.SUBMIT_FLAG = item.FirstOrDefault().SUBMIT_FLAG;
                    EquipList.REJECT_REASON = item.FirstOrDefault().REJECT_REASON;
                    EquipList.SUBMIT_DATE = item.FirstOrDefault().SUBMIT_DATE;
                    EquipList.OLT_NAME = item.FirstOrDefault().OLT_NAME;
                    EquipList.BUILDING_NAME = item.FirstOrDefault().BUILDING_NAME;
                    EquipList.MOBILE_CONTACT = item.FirstOrDefault().MATERIAL_CODE;
                    EquipList.SN = item.FirstOrDefault().SN;
                    EquipList.MATERIAL_CODE = item.FirstOrDefault().MATERIAL_CODE;
                    EquipList.COMPANY_CODE = item.FirstOrDefault().COMPANY_CODE;
                    EquipList.PLANT = item.FirstOrDefault().PLANT;
                    EquipList.STORAGE_LOCATION = item.FirstOrDefault().STORAGE_LOCATION;
                    EquipList.SN_TYPE = item.FirstOrDefault().SN_TYPE;
                    EquipList.MOVEMENT_TYPE = item.FirstOrDefault().MOVEMENT_TYPE;
                    EquipList.STATUS = item.FirstOrDefault().STATUS;
                    EquipList.ERR_CODE = item.FirstOrDefault().ERR_CODE;
                    EquipList.ERR_MSG = item.FirstOrDefault().ERR_MSG;
                    EquipList.ListEquipment = SubmitFOAEquipmentData;
                    EquipList.MOVEMENT_TYPE_OUT = item.FirstOrDefault().MOVEMENT_TYPE_OUT;
                    EquipList.MOVEMENT_TYPE_IN = item.FirstOrDefault().MOVEMENT_TYPE_IN;
                    EquipList.PRODUCT_OWNER = item.FirstOrDefault().PRODUCT_OWNER;
                    EquipList.MAIN_PROMO_CODE = item.FirstOrDefault().MAIN_PROMO_CODE;
                    EquipList.TEAM_ID = item.FirstOrDefault().TEAM_ID;
                    EquipList.TRANS_ID = item.FirstOrDefault().TRANS_ID;
                    result.Add(EquipList);

                    //}
                    //catch
                    //{

                    //}
                }
            }

            return result;
        }

        public List<SubmitFOAInstallationReport> GroupInstallation(List<SubmitFOAInstallation> dataS)
        {
            var result = new List<SubmitFOAInstallationReport>();

            if (dataS.Count > 0)
            {

                foreach (var item in dataS.Where(w => w.TRAN_STATUS != null && (w.TRAN_STATUS.ToUpper() == "ERROR" || w.TRAN_STATUS.ToUpper() == "PENDING")).GroupBy(g => new { g.ACCESS_NUMBER, g.ORDER_NO }))


                {
                    var EquipList = new SubmitFOAInstallationReport();
                    //try
                    //{
                    var SubmitFOAEquipmentData = dataS.Where(p => p.ACCESS_NUMBER == item.FirstOrDefault().ACCESS_NUMBER && p.ORDER_NO == item.FirstOrDefault().ORDER_NO)
                                                       .Select(p =>
                                                       {
                                                           return new SubmitFOAInstallation
                                                           {
                                                               ACCESS_NUMBER = p.ACCESS_NUMBER,
                                                               ASSET_CODE = p.ASSET_CODE,
                                                               SUB_NUMBER = p.SUB_NUMBER,
                                                               PRODUCT_NAME = p.PRODUCT_NAME,
                                                               SERVICE_NAME = p.SERVICE_NAME,
                                                               INSTALLATION_COST = p.INSTALLATION_COST,
                                                               ORDER_DATE = p.ORDER_DATE,
                                                               MODIFY_DATE = p.MODIFY_DATE,
                                                               TRAN_STATUS = p.TRAN_STATUS,
                                                               ORDER_NO = p.ORDER_NO,
                                                               PRODUCT_OWNER = p.PRODUCT_OWNER,
                                                               MAIN_PROMO_CODE = p.MAIN_PROMO_CODE,
                                                               TRANS_ID = p.TRANS_ID,
                                                               TEAM_ID = p.TEAM_ID

                                                           };
                                                       }).ToList();

                    EquipList.ACCESS_NUMBER = item.FirstOrDefault().ACCESS_NUMBER;
                    EquipList.ASSET_CODE = item.FirstOrDefault().ASSET_CODE;
                    EquipList.SUB_NUMBER = item.FirstOrDefault().SUB_NUMBER;
                    EquipList.PRODUCT_NAME = item.FirstOrDefault().PRODUCT_NAME;
                    EquipList.SERVICE_NAME = item.FirstOrDefault().SERVICE_NAME;
                    EquipList.INSTALLATION_COST = item.FirstOrDefault().INSTALLATION_COST;
                    EquipList.ORDER_DATE = item.FirstOrDefault().ORDER_DATE;
                    EquipList.MODIFY_DATE = item.FirstOrDefault().MODIFY_DATE;
                    EquipList.TRAN_STATUS = item.FirstOrDefault().TRAN_STATUS;

                    EquipList.ListInstallation = SubmitFOAEquipmentData;

                    result.Add(EquipList);

                }
            }

            return result;
        }



        #endregion

        #region JsonResult
        // Resend To Select
        [HttpPost]
        public JsonResult ResendToSelectSubmitFOAMA(List<SubmitFOAMainAsset> model)
        {
            string msg = "";
            int indexSuccess = 0;
            int indexError = 0;
            foreach (var item in model)
            {
                var query = new NewRegistForSubmitFOAMainAssetQuery()
                {
                    p_ORDER_NO = item.ORDER_NO,
                    p_INTERNET_NO = item.ACCESS_NUMBER,
                    p_COM_CODE_OLD = item.COM_CODE_OLD,
                    p_COM_CODE_NEW = item.COM_CODE_NEW,
                    p_ASSET_CLASS = item.ASSET_CLASS_GI,
                    p_COSTCENTER = item.COST_CENTER,
                    p_PRODUCT = item.PRODUCT_NAME,
                    p_USER_CODE = base.CurrentUser.UserId.ToSafeString(),
                    ORDER_TYPE = item.ORDER_TYPE,
                    SUBCONTRACT_NAME = item.SUBCONTRACT_NAME,
                    FOA_SUBMIT_DATE = item.FOA_SUBMIT_DATE,
                    UserName = base.CurrentUser.UserName.ToSafeString()
                };

                var resultNewRegist = _queryProcessor.Execute(query);
                if (resultNewRegist.result != "" && resultNewRegist.result != "-1") indexSuccess += 1;
                else indexError += 1;
            }

            if (indexSuccess > 0 || indexError > 0)
            {
                msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
            }

            return Json(msg);
        }
        // Resend To Select
        [HttpPost]
        public async Task<JsonResult> ResendToSelectSubmitFOA(List<SubmitFOAEquipment> dataS, List<ReplaceEquipmentModel> dataRe)
        {
            // 1.GetSubmitFOA from View
            var searchModel = dataS;////new JavaScriptSerializer().Deserialize<List<SubmitFOAEquipment>>(dataS);
            var replaceModel = dataRe; //// new JavaScriptSerializer().Deserialize<ReplaceEquipmentModel>(dataRe);
                                       // Group Multi


            var groupEquipmentModel = GroupEquipment(searchModel);

            string msg = "";
            int indexSuccess = 0;
            int indexError = 0;
       
            NewRegistForSubmitFOAResponse resultNewRegist = new NewRegistForSubmitFOAResponse();

            NewRegistForSubmitFOAS4HANAResponse resultNewRegists4hana = new NewRegistForSubmitFOAS4HANAResponse();
            if (groupEquipmentModel.Any())
            {

                var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();

                if (resultFixAssConfig.DISPLAY_VAL == "Y")
                {

                    foreach (var resultlist in groupEquipmentModel.Where(w => w.STATUS.ToUpper().Trim() == "ERROR" || w.STATUS.ToUpper().Trim() == "PENDING"))
                    {
                        // 2.GetProductList
                        var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                        var resultProductList = _queryProcessor.Execute(queryProductList);




                        var _main = new NewRegistForSubmitFOAQuery();
                        var _product = new List<NewRegistFOAProductList>();

                        // 3.ResendSubmitFOA (STATUS = ERROR Only)
                        string _subcontractorCode = "";
                        string _postDate = "";
                        if (replaceModel != null)
                        {
                            var _replace = replaceModel.Where(w => w.access_no == resultlist.ACCESS_NUMBER && w.order_no == resultlist.ORDER_NO).ToList();
                            if (_replace.Any())
                            {
                                _subcontractorCode = _replace.FirstOrDefault().subcontractorCode;
                                _postDate = _replace.FirstOrDefault().postDate;
                            }
                        }
                        // Set _main
                        _main = new NewRegistForSubmitFOAQuery()
                        {
                            Access_No = resultProductList.AccessNo.ToSafeString(),
                            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                            SubcontractorCode = !string.IsNullOrEmpty(_subcontractorCode) ?
                                                _subcontractorCode.ToSafeString() : resultProductList.SubcontractorCode.ToSafeString(),
                            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                            ProductName = resultProductList.ProductName.ToSafeString(),
                            OrderType = resultProductList.OrderType.ToSafeString(),
                            SubmitFlag = "RESEND",
                            RejectReason = resultProductList.RejectReason.ToSafeString(),
                            FOA_Submit_date = resultProductList.FOA_Submit_date.ToSafeString(),
                            OLT_NAME = resultProductList.OLT_NAME.ToSafeString(),
                            BUILDING_NAME = resultProductList.BUILDING_NAME.ToSafeString(),
                            Mobile_Contact = resultProductList.Mobile_Contact.ToSafeString(),

                            Post_Date = _postDate,
                            Address_ID = resultProductList.ADDRESS_ID.ToSafeString(),
                            ORG_ID = resultProductList.ORG_ID.ToSafeString(),
                            Reuse_Flag = resultProductList.REUSE_FLAG.ToSafeString(),
                            Event_Flow_Flag = resultProductList.EVENT_FLOW_FLAG.ToSafeString(),

                            //18.07.02
                            UserName = base.CurrentUser.UserName.ToSafeString(),
                            //

                            Subcontract_Type = resultProductList.SUBCONTRACT_TYPE.ToSafeString(),
                            Subcontract_Sub_Type = resultProductList.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                            Request_Sub_Flag = resultProductList.REQUEST_SUB_FLAG.ToSafeString(),
                            Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString(),

                            Product_Owner = resultProductList.PRODUCT_OWNER.ToSafeString(),
                            Main_Promo_Code = resultProductList.MAIN_PROMO_CODE.ToSafeString(),
                            Team_ID = resultProductList.TEAM_ID.ToSafeString()
                        };

                        // Set _product

                        //เพิ่ม Flag Rollback for productlist (01/11/2021)
                        var rollback = Get_FBSS_CONFIG_TBL_LOV("RESEND_ORDER", "ROLLBACK").FirstOrDefault(l => l.ACTIVEFLAG == "Y");

                        var rollbacks = rollback != null ? rollback.ACTIVEFLAG : "N";
                        if (rollbacks.ToSafeString().ToUpper() == "Y")
                        {
                            foreach (var equipmentlist in resultProductList.ProductList)
                            {
                                var equipmentListFormView = new List<SubmitFOAEquipment>();

                                string _materialCode = "";
                                string _companyCode = "";
                                string _plant = "";
                                string _storLocation = "";
                                if (replaceModel != null)
                                {
                                    var _replace = replaceModel.Where(w => w.access_no == resultlist.ACCESS_NUMBER && w.order_no == resultlist.ORDER_NO && w.serial_number == equipmentlist.SerialNumber).ToList();
                                    if (_replace.Any())
                                    {
                                        _materialCode = _replace.FirstOrDefault().materialCode;
                                        _companyCode = _replace.FirstOrDefault().companyCode;
                                        _plant = _replace.FirstOrDefault().plant;
                                        _storLocation = _replace.FirstOrDefault().storLocation;
                                    }
                                }

                                // Get Equipment from View Edit
                                equipmentListFormView = resultlist.ListEquipment.Where(x => x.SN == equipmentlist.SerialNumber).Select(p =>
                                {
                                    return new SubmitFOAEquipment()
                                    {
                                        SN = p.SN.ToSafeString(),
                                        MATERIAL_CODE = p.MATERIAL_CODE.ToSafeString(),
                                        COMPANY_CODE = p.COMPANY_CODE.ToSafeString(),
                                        PLANT = p.PLANT.ToSafeString(),
                                        STORAGE_LOCATION = p.STORAGE_LOCATION.ToSafeString(),
                                        SN_TYPE = p.SN_TYPE.ToSafeString(),
                                        //MOVEMENT_TYPE = p.MOVEMENT_TYPE
                                        MOVEMENT_TYPE = (p.MOVEMENT_TYPE != null) ? p.MOVEMENT_TYPE : p.MOVEMENT_TYPE_IN
                                    };
                                }).ToList();

                                if (equipmentListFormView.Any())
                                {
                                    // Assign Value from View Edit
                                    var equipment = equipmentListFormView.Select(e =>
                                    {
                                        return new NewRegistFOAProductList()
                                        {
                                            SerialNumber = e.SN,
                                            MaterialCode = e.MATERIAL_CODE,
                                            CompanyCode = e.COMPANY_CODE,
                                            Plant = e.PLANT,
                                            StorageLocation = e.STORAGE_LOCATION,
                                            SNPattern = e.SN_TYPE,
                                            MovementType = e.MOVEMENT_TYPE
                                        };
                                    }).First();
                                    // New approach using GetLovV2Query
                                    var query = new GetLovV2Query()
                                    {
                                        LovType = "SN_PATT",
                                        LovVal5 = "FIXED_LASTMILE"
                                    };
                                    var snpatternwhere = _queryProcessor.Execute(query).FirstOrDefault(p => p.Name.ToUpper() == equipment.SNPattern.ToUpper());
                                    _product.Add(new NewRegistFOAProductList()
                                    {
                                        SerialNumber = equipment.SerialNumber,
                                        MaterialCode = !string.IsNullOrEmpty(_materialCode) ?
                                                                  _materialCode.ToSafeString() : equipment.MaterialCode,
                                        CompanyCode = !string.IsNullOrEmpty(_companyCode) ?
                                                                  _companyCode.ToSafeString() : equipment.CompanyCode,
                                        Plant = !string.IsNullOrEmpty(_plant) ?
                                                            _plant.ToSafeString() : equipment.Plant,
                                        StorageLocation = !string.IsNullOrEmpty(_storLocation) ?
                                                                      _storLocation.ToSafeString() : equipment.StorageLocation,
                                        SNPattern = (snpatternwhere != null) ? snpatternwhere.Text : "",
                                        MovementType = equipment.MovementType
                                    });
                                }
                                else
                                {
                                    _product.Add(new NewRegistFOAProductList()
                                    {
                                        SerialNumber = equipmentlist.SerialNumber,
                                        MaterialCode = equipmentlist.MaterialCode,
                                        CompanyCode = equipmentlist.CompanyCode,
                                        Plant = equipmentlist.Plant,
                                        StorageLocation = equipmentlist.StorageLocation,
                                        SNPattern = equipmentlist.SNPattern,
                                        MovementType = equipmentlist.MovementType
                                    });
                                }
                            }

                        }


                        _main.ProductList = _product;

                        var _services = new NewRegistFOAServiceList()
                        {
                            ServiceName = resultProductList.ServiceName != null ? resultProductList.ServiceName : ""
                        };

                        List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                        var subStr = _services.ServiceName.Split(',');
                        foreach (var service in subStr)
                        {
                            _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                        }
                        _main.ServiceList = _service;
                        resultNewRegist = _queryProcessor.Execute(_main);
                        insertErrorLog<SubmitFOAEquipmentReport>(resultlist, $"5. NewRegistForSubmitFOAResponse", resultlist.ACCESS_NUMBER.ToString());
                        ////if (resultNewRegist.result != "") indexSuccess += 1;
                        ////else indexError += 1;
                        if (resultNewRegist.new_response != "9999" && resultNewRegist.new_response != "999") indexSuccess += 1;
                        else indexError += 1;

                    }

                }
                else
                {

                    var masterList = new List<SubmitFOAResenorderdata>();

                    foreach (var resultlist in groupEquipmentModel.Where(w => w.STATUS.ToUpper().Trim() == "ERROR" || w.STATUS.ToUpper().Trim() == "PENDING"))
                    {

                        foreach (var ListEquipmentresult in resultlist.ListEquipment)
                        {
                            var data = new SubmitFOAResenorderdata
                            {
                                SerialNumber = ListEquipmentresult.SN,
                                trans_id = ListEquipmentresult.TRANS_ID,
                                MaterialCode = ListEquipmentresult.MATERIAL_CODE,
                                CompanyCode = ListEquipmentresult.COMPANY_CODE,
                                Plant = ListEquipmentresult.PLANT,
                                StorageLocation = ListEquipmentresult.STORAGE_LOCATION,
                            };

                            masterList.Add(data);
                        }
                    }



                    //var equipments = groupEquipmentModel
                    //    .Where(w => w.STATUS.ToUpper().Trim() == "ERROR" || w.STATUS.ToUpper().Trim() == "PENDING")
                    //    .SelectMany(g => g.ListEquipment)
                    //    .ToList();


                    //     var allTransIds = equipments
                    //.SelectMany(item =>
                    //  item.TRANS_ID == null
                    // ? new List<string> { null }
                    // : item.TRANS_ID
                    // .Split(new[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries)
                    // .Select(t => t.Trim()).ToList()
                    // ).Distinct().ToList();


                    //     var groupedBySN = equipments.GroupBy(item => item.SN).ToList();

                    //     var groupedMasterList = groupedBySN
                    //         .Select((group, index) =>
                    //         {
                    //             var firstItem = group.First();


                    //             string transIdForThisItem = index < allTransIds.Count ? allTransIds[index] : "";
                    //             return new SubmitFOAResenorderdata
                    //             {
                    //                 SerialNumber = firstItem.SN,
                    //                 trans_id = transIdForThisItem,
                    //                 MaterialCode = firstItem.MATERIAL_CODE,
                    //                 CompanyCode = firstItem.COMPANY_CODE,
                    //                 Plant = firstItem.PLANT,
                    //                 StorageLocation = firstItem.STORAGE_LOCATION
                    //             };
                    //         })
                    //         .ToList();



                    var ResultModel = new ResendOrderGetData();

                    var query = new SubmitFOAResendOrderNewQuery()
                    {
                        tab_name = "EQUIPMENT",
                        list_p_get_oder = masterList
                    };
                    ResultModel = _queryProcessor.Execute(query);

                    if (ResultModel.ret_code != "" && ResultModel.ret_code != "-1") indexSuccess += 1;
                    else indexError += 1;

                }

            }


            if (indexSuccess > 0 || indexError > 0)
            {
               

                msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + " ";

                msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + " ";

            }

            //if (  indexSuccess > 0 || indexError > 0)
            //{
            //    int totalSuccess = count_success + indexSuccess;
            //    int totalError = count_err + indexError;

            // msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "Success";
            //    msg = indexError > 0 ? msg + "Error " + resultNewRegist.new_ErrorMsg.ToString() : msg + "Error";



            //}

            return Json(msg);
        }



        public async Task<JsonResult> ResendToSelectSubmitFOAIN(List<SubmitFOAInstallation> dataS, List<ReplaceEquipmentModel> dataRe)
        {
            // 1.GetSubmitFOA from View
            var searchModel = dataS;////new JavaScriptSerializer().Deserialize<List<SubmitFOAEquipment>>(dataS);
            var replaceModel = dataRe; //// new JavaScriptSerializer().Deserialize<ReplaceEquipmentModel>(dataRe);
                                       // Group Multi

            var groupEquipmentModel = GroupInstallation(searchModel);

            //var groupEquipmentModel = GroupInstallation(searchModel);

            string msg = "";
            int indexSuccess = 0;
            int indexError = 0;

            NewRegistForSubmitFOAResponse resultNewRegist = new NewRegistForSubmitFOAResponse();

            NewRegistForSubmitFOAS4HANAResponse resultNewRegists4hana = new NewRegistForSubmitFOAS4HANAResponse();


            if (groupEquipmentModel.Any())
            {
                var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();

                if (resultFixAssConfig.DISPLAY_VAL == "Y")
                {
                    foreach (var resultlist in groupEquipmentModel.Where(w => w.TRAN_STATUS.ToUpper().Trim() == "ERROR" || w.TRAN_STATUS.ToUpper().Trim() == "PENDING"))
                    {
                        // 2.GetProductList
                        var orderNo = resultlist.ListInstallation.Any() ? resultlist.ListInstallation.Select(item => item.ORDER_NO).FirstOrDefault() : null;

                        var queryProductList = new GetProductListQuery() { OrderNo = orderNo, AccessNo = resultlist.ACCESS_NUMBER };
                        var resultProductList = _queryProcessor.Execute(queryProductList);




                        var _main = new NewRegistForSubmitFOAQuery();
                        var _product = new List<NewRegistFOAProductList>();

                        // 3.ResendSubmitFOA (STATUS = ERROR Only)
                        string _subcontractorCode = "";
                        string _postDate = "";
                        if (replaceModel != null)
                        {
                            var _replace = replaceModel.Where(w => w.access_no == resultlist.ACCESS_NUMBER && w.order_no == orderNo).ToList();
                            if (_replace.Any())
                            {
                                _subcontractorCode = _replace.FirstOrDefault().subcontractorCode;
                                _postDate = _replace.FirstOrDefault().postDate;
                            }
                        }
                        // Set _main
                        _main = new NewRegistForSubmitFOAQuery()
                        {
                            Access_No = resultProductList.AccessNo.ToSafeString(),
                            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                            SubcontractorCode = !string.IsNullOrEmpty(_subcontractorCode) ?
                                                _subcontractorCode.ToSafeString() : resultProductList.SubcontractorCode.ToSafeString(),
                            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                            ProductName = resultProductList.ProductName.ToSafeString(),
                            OrderType = resultProductList.OrderType.ToSafeString(),
                            SubmitFlag = "RESEND",
                            RejectReason = resultProductList.RejectReason.ToSafeString(),
                            FOA_Submit_date = resultProductList.FOA_Submit_date.ToSafeString(),
                            OLT_NAME = resultProductList.OLT_NAME.ToSafeString(),
                            BUILDING_NAME = resultProductList.BUILDING_NAME.ToSafeString(),
                            Mobile_Contact = resultProductList.Mobile_Contact.ToSafeString(),

                            Post_Date = _postDate,
                            Address_ID = resultProductList.ADDRESS_ID.ToSafeString(),
                            ORG_ID = resultProductList.ORG_ID.ToSafeString(),
                            Reuse_Flag = resultProductList.REUSE_FLAG.ToSafeString(),
                            Event_Flow_Flag = resultProductList.EVENT_FLOW_FLAG.ToSafeString(),

                            //18.07.02
                            UserName = base.CurrentUser.UserName.ToSafeString(),
                            //

                            Subcontract_Type = resultProductList.SUBCONTRACT_TYPE.ToSafeString(),
                            Subcontract_Sub_Type = resultProductList.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                            Request_Sub_Flag = resultProductList.REQUEST_SUB_FLAG.ToSafeString(),
                            Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString(),

                            Product_Owner = resultProductList.PRODUCT_OWNER.ToSafeString(),
                            Main_Promo_Code = resultProductList.MAIN_PROMO_CODE.ToSafeString(),
                            Team_ID = resultProductList.TEAM_ID.ToSafeString()
                        };

                        // Set _product

                        //เพิ่ม Flag Rollback for productlist (01/11/2021)
                        var rollback = Get_FBSS_CONFIG_TBL_LOV("RESEND_ORDER", "ROLLBACK").FirstOrDefault(l => l.ACTIVEFLAG == "Y");

                        var rollbacks = rollback != null ? rollback.ACTIVEFLAG : "N";
                        if (rollbacks.ToSafeString().ToUpper() == "Y")
                        {
                            foreach (var equipmentlist in resultProductList.ProductList)
                            {
                                var equipmentListFormView = new List<SubmitFOAEquipment>();

                                string _materialCode = "";
                                string _companyCode = "";
                                string _plant = "";
                                string _storLocation = "";
                                if (replaceModel != null)
                                {
                                    var _replace = replaceModel.Where(w => w.access_no == resultlist.ACCESS_NUMBER && w.order_no == orderNo && w.serial_number == equipmentlist.SerialNumber).ToList();
                                    if (_replace.Any())
                                    {
                                        _materialCode = _replace.FirstOrDefault().materialCode;
                                        _companyCode = _replace.FirstOrDefault().companyCode;
                                        _plant = _replace.FirstOrDefault().plant;
                                        _storLocation = _replace.FirstOrDefault().storLocation;
                                    }
                                }

                                // Get Equipment from View Edit
                                //equipmentListFormView = resultlist.ListEquipment.Where(x => x.SN == equipmentlist.SerialNumber).Select(p =>
                                //{
                                //    return new SubmitFOAEquipment()
                                //    {
                                //        SN = p.SN.ToSafeString(),
                                //        MATERIAL_CODE = p.MATERIAL_CODE.ToSafeString(),
                                //        COMPANY_CODE = p.COMPANY_CODE.ToSafeString(),
                                //        PLANT = p.PLANT.ToSafeString(),
                                //        STORAGE_LOCATION = p.STORAGE_LOCATION.ToSafeString(),
                                //        SN_TYPE = p.SN_TYPE.ToSafeString(),
                                //        //MOVEMENT_TYPE = p.MOVEMENT_TYPE
                                //        MOVEMENT_TYPE = (p.MOVEMENT_TYPE != null) ? p.MOVEMENT_TYPE : p.MOVEMENT_TYPE_IN
                                //    };
                                //}).ToList();

                                if (equipmentListFormView.Any())
                                {
                                    // Assign Value from View Edit
                                    var equipment = equipmentListFormView.Select(e =>
                                    {
                                        return new NewRegistFOAProductList()
                                        {
                                            SerialNumber = e.SERIAL_NUMBER,
                                            MaterialCode = e.MATERIAL_CODE,
                                            CompanyCode = e.COMPANY_CODE,
                                            Plant = e.PLANT,
                                            StorageLocation = e.STORAGE_LOCATION,
                                            SNPattern = e.SN_TYPE,
                                            MovementType = e.MOVEMENT_TYPE
                                        };
                                    }).First();
                                    // New approach using GetLovV2Query
                                    var query = new GetLovV2Query()
                                    {
                                        LovType = "SN_PATT",
                                        LovVal5 = "FIXED_LASTMILE"
                                    };
                                    var snpatternwhere = _queryProcessor.Execute(query).FirstOrDefault(p => p.Name.ToUpper() == equipment.SNPattern.ToUpper());
                                    _product.Add(new NewRegistFOAProductList()
                                    {
                                        SerialNumber = equipment.SerialNumber,
                                        MaterialCode = !string.IsNullOrEmpty(_materialCode) ?
                                                                  _materialCode.ToSafeString() : equipment.MaterialCode,
                                        CompanyCode = !string.IsNullOrEmpty(_companyCode) ?
                                                                  _companyCode.ToSafeString() : equipment.CompanyCode,
                                        Plant = !string.IsNullOrEmpty(_plant) ?
                                                            _plant.ToSafeString() : equipment.Plant,
                                        StorageLocation = !string.IsNullOrEmpty(_storLocation) ?
                                                                      _storLocation.ToSafeString() : equipment.StorageLocation,
                                        SNPattern = (snpatternwhere != null) ? snpatternwhere.Text : "",
                                        MovementType = equipment.MovementType
                                    });
                                }
                                else
                                {
                                    _product.Add(new NewRegistFOAProductList()
                                    {
                                        SerialNumber = equipmentlist.SerialNumber,
                                        MaterialCode = equipmentlist.MaterialCode,
                                        CompanyCode = equipmentlist.CompanyCode,
                                        Plant = equipmentlist.Plant,
                                        StorageLocation = equipmentlist.StorageLocation,
                                        SNPattern = equipmentlist.SNPattern,
                                        MovementType = equipmentlist.MovementType
                                    });
                                }
                            }

                        }
                        else
                        {
                            foreach (SubmitFOAProduct equipmentlist in resultProductList.ProductList.AsEnumerable())
                            {
                                var equipmentListFormView = new List<SubmitFOAEquipment>();
                                string _materialCode = "";
                                string _companyCode = "";
                                string _plant = "";
                                string _storLocation = "";
                                if (replaceModel != null)
                                {
                                    var _replace = replaceModel.Where(w => w.access_no == resultlist.ACCESS_NUMBER && w.order_no == orderNo && w.serial_number == equipmentlist.SerialNumber).ToList();
                                    if (_replace.Any())
                                    {
                                        _materialCode = _replace.FirstOrDefault().materialCode;
                                        _companyCode = _replace.FirstOrDefault().companyCode;
                                        _plant = _replace.FirstOrDefault().plant;
                                        _storLocation = _replace.FirstOrDefault().storLocation;
                                    }
                                }

                                // Get Equipment from View Edit
                                //equipmentListFormView = resultlist.ListEquipment.Where(x => x.SN == equipmentlist.SerialNumber).Select(p =>
                                //{
                                //    return new SubmitFOAEquipment()
                                //    {
                                //        SN = p.SN.ToSafeString(),
                                //        MATERIAL_CODE = p.MATERIAL_CODE.ToSafeString(),
                                //        COMPANY_CODE = p.COMPANY_CODE.ToSafeString(),
                                //        PLANT = p.PLANT.ToSafeString(),
                                //        STORAGE_LOCATION = p.STORAGE_LOCATION.ToSafeString().Contains("") ? p.STORAGE_LOCATION.ToSafeString().Trim() : p.STORAGE_LOCATION.ToSafeString(),
                                //        SN_TYPE = p.SN_TYPE.ToSafeString(),
                                //        //MOVEMENT_TYPE = p.MOVEMENT_TYPE
                                //        MOVEMENT_TYPE = (p.MOVEMENT_TYPE != null) ? p.MOVEMENT_TYPE : p.MOVEMENT_TYPE_IN
                                //    };
                                //}).ToList();

                                if (equipmentListFormView.Any())
                                {
                                    // Assign Value from View Edit
                                    var equipment = equipmentListFormView.Select(e =>
                                    {
                                        return new NewRegistFOAProductList()
                                        {
                                            SerialNumber = e.SN,
                                            MaterialCode = e.MATERIAL_CODE,
                                            CompanyCode = e.COMPANY_CODE,
                                            Plant = e.PLANT,
                                            StorageLocation = e.STORAGE_LOCATION,
                                            SNPattern = e.SN_TYPE,
                                            MovementType = e.MOVEMENT_TYPE
                                        };
                                    }).First();
                                    var query = new GetLovV2Query()
                                    {
                                        LovType = "SN_PATT",
                                        LovVal5 = "FIXED_LASTMILE"
                                    };
                                    var snpatternwhere = _queryProcessor.Execute(query).FirstOrDefault(p => p.Name.ToUpper() == equipment.SNPattern.ToUpper());
                                    _product.Add(new NewRegistFOAProductList()
                                    {
                                        SerialNumber = equipment.SerialNumber,
                                        MaterialCode = !string.IsNullOrEmpty(_materialCode) ?
                                                                  _materialCode.ToSafeString() : equipment.MaterialCode,
                                        CompanyCode = !string.IsNullOrEmpty(_companyCode) ?
                                                                  _companyCode.ToSafeString() : equipment.CompanyCode,
                                        Plant = !string.IsNullOrEmpty(_plant) ?
                                                            _plant.ToSafeString() : equipment.Plant,
                                        StorageLocation = !string.IsNullOrEmpty(_storLocation) ?
                                                                      _storLocation.ToSafeString() : equipment.StorageLocation,
                                        SNPattern = (snpatternwhere != null) ? snpatternwhere.Text : "",
                                        MovementType = equipment.MovementType
                                    });
                                }
                                else
                                {
                                    _product.Add(new NewRegistFOAProductList()
                                    {
                                        SerialNumber = equipmentlist.SerialNumber,
                                        MaterialCode = equipmentlist.MaterialCode,
                                        CompanyCode = equipmentlist.CompanyCode,
                                        Plant = equipmentlist.Plant,
                                        StorageLocation = equipmentlist.StorageLocation,
                                        SNPattern = equipmentlist.SNPattern,
                                        MovementType = equipmentlist.MovementType
                                    });
                                }
                            }

                        }

                        _main.ProductList = _product;

                        var _services = new NewRegistFOAServiceList()
                        {
                            ServiceName = resultProductList.ServiceName != null ? resultProductList.ServiceName : ""
                        };

                        List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                        var subStr = _services.ServiceName.Split(',');
                        foreach (var service in subStr)
                        {
                            _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                        }
                        _main.ServiceList = _service;
                        resultNewRegist = _queryProcessor.Execute(_main);
                        //insertErrorLog<SubmitFOAEquipmentReport>(resultlist, $"5. NewRegistForSubmitFOAResponse", resultlist.ACCESS_NUMBER.ToString());

                        ////if (resultNewRegist.result != "") indexSuccess += 1;
                        ////else indexError += 1;
                        if (resultNewRegist.new_response != "9999" && resultNewRegist.new_response != "999") indexSuccess += 1;
                        else indexError += 1;



                    }
                }
                else
                {


                    //var masterList = new List<SubmitFOAResenorderdata>();

                    //foreach (var resultlist in groupEquipmentModel.Where(w => w.TRAN_STATUS.ToUpper().Trim() == "PENDING" || w.TRAN_STATUS.ToUpper().Trim() == "ERROR"))
                    //{

                    //    foreach (var ListInstallationresult in resultlist.ListInstallation)
                    //    {
                    //        var data = new SubmitFOAResenorderdata
                    //        {
                    //            SerialNumber = "",
                    //            trans_id = ListInstallationresult.TRANS_ID,
                    //            MaterialCode = "",
                    //            CompanyCode = "",
                    //            Plant = "",
                    //            StorageLocation = "",
                    //        };

                    //        masterList.Add(data);

                    //    }
                    //}



                    var masterList = new List<SubmitFOAResenorderdata>();
                    var addedTransIds = new HashSet<string>();

                    foreach (var result in groupEquipmentModel
                                .Where(w => w.TRAN_STATUS.ToUpper().Trim() == "PENDING" || w.TRAN_STATUS.ToUpper().Trim() == "ERROR"))
                    {
                        foreach (var install in result.ListInstallation)
                        {
                            if (!string.IsNullOrWhiteSpace(install.TRANS_ID) && !addedTransIds.Contains(install.TRANS_ID))
                            {
                                var data = new SubmitFOAResenorderdata
                                {
                                    SerialNumber = "",
                                    trans_id = install.TRANS_ID,
                                    MaterialCode = "",
                                    CompanyCode = "",
                                    Plant = "",
                                    StorageLocation = ""
                                };

                                masterList.Add(data);
                                addedTransIds.Add(install.TRANS_ID);
                            }
                        }
                    }



                    var ResultModel = new ResendOrderGetData();


                    var query = new SubmitFOAResendOrderNewQuery()
                    {
                        tab_name = "INSTALLATION",
                        list_p_get_oder = masterList
                    };
                    ResultModel = _queryProcessor.Execute(query);

                    if (ResultModel.ret_code != "" && ResultModel.ret_code != "-1") indexSuccess += 1;
                    else indexError += 1;

                }



            }
            if (indexSuccess > 0 || indexError > 0)
            {


                msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + " ";
                msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + " ";

            }

            return Json(msg);
        }



        // Resend All (Update 15/11/2017)
        public JsonResult ResendPendingAllSubmitFOA(string dataS = "", string dataRe = "")
        {
            string msg = "";
            int indexSuccess = 0;
            int indexError = 0;

            if (!string.IsNullOrEmpty(dataS))
            {
                // 1.GetSubmitFOA
                var searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAEquipmentReportQuery>(dataS);
                var replaceModel = new JavaScriptSerializer().Deserialize<ReplaceEquipmentModel>(dataRe);

                SubmitFOAEquipmentListReturn resultSubmitFOAListTemp = this.GetSubmitFOA(searchModel);
                List<SubmitFOAEquipment> resultSubmitFOAList = resultSubmitFOAListTemp.cur.Where(p => p.STATUS != "COMPLETE")
                //List<SubmitFOAEquipment> resultSubmitFOAList = resultSubmitFOAListTemp.cur.Where(p => p.STATUS != "COMPLETE" && p.STATUS != "ERROR")
                                                .Select(p =>
                                                {
                                                    return new SubmitFOAEquipment
                                                    {
                                                        ACCESS_NUMBER = p.ACCESS_NUMBER,
                                                        ORDER_NO = p.ORDER_NO,
                                                        ORDER_TYPE = p.ORDER_TYPE,
                                                        SUBCONTRACT_CODE = p.SUBCONTRACT_CODE,
                                                        SUBCONTRACT_NAME = p.SUBCONTRACT_NAME,
                                                        PRODUCT_NAME = p.PRODUCT_NAME,
                                                        SERVICE_NAME = p.SERVICE_NAME,
                                                        SUBMIT_FLAG = p.SUBMIT_FLAG,
                                                        REJECT_REASON = p.REJECT_REASON,
                                                        SUBMIT_DATE = p.SUBMIT_DATE,
                                                        OLT_NAME = p.OLT_NAME,
                                                        BUILDING_NAME = p.BUILDING_NAME,
                                                        MOBILE_CONTACT = p.MATERIAL_CODE,
                                                        SN = p.SN,
                                                        MATERIAL_CODE = p.MATERIAL_CODE,
                                                        COMPANY_CODE = p.COMPANY_CODE,
                                                        PLANT = p.PLANT,
                                                        STORAGE_LOCATION = p.STORAGE_LOCATION,
                                                        SN_TYPE = p.SN_TYPE,
                                                        MOVEMENT_TYPE = p.MOVEMENT_TYPE_IN,
                                                        STATUS = p.STATUS,
                                                        ERR_CODE = p.ERR_CODE,
                                                        ERR_MSG = p.ERR_MSG,
                                                        ADDRESS_ID = p.ADDRESS_ID.ToSafeString(),
                                                        ORG_ID = p.ORG_ID.ToSafeString(),
                                                        REUSE_FLAG = p.REUSE_FLAG.ToSafeString(),
                                                        EVENT_FLOW_FLAG = p.EVENT_FLOW_FLAG.ToSafeString(),
                                                        SUBCONTRACT_TYPE = p.SUBCONTRACT_TYPE.ToSafeString(),
                                                        SUBCONTRACT_SUB_TYPE = p.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                                                        REQUEST_SUB_FLAG = p.REQUEST_SUB_FLAG.ToSafeString(),
                                                        SUB_ACCESS_MODE = p.SUB_ACCESS_MODE.ToSafeString()
                                                    };
                                                }).ToList();

                string orderno = "", accessnum = "";
                if (resultSubmitFOAList.Any())
                {
                    var _mainList = new List<NewRegistForSubmitFOAQuery>();
                    var _mainListFOA4HANA = new List<NewRegistForSubmitFOA4HANAQuery>();

                    foreach (var resultlist in resultSubmitFOAList)
                    {
                        // 2.GetProductList
                        var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                        var resultProductList = _queryProcessor.Execute(queryProductList);



                        // 3.ResendSubmitFOA
                        ////if (resultlist.STATUS == "Pending")
                        //{

                        var _main = new NewRegistForSubmitFOAQuery();
                        var _product = new List<NewRegistFOAProductList>();

                        if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                        {
                            _main = new NewRegistForSubmitFOAQuery()
                            {
                                Access_No = resultProductList.AccessNo.ToSafeString(),
                                OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                                SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                                SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                                ProductName = resultProductList.ProductName.ToSafeString(),
                                OrderType = resultProductList.OrderType.ToSafeString(),
                                SubmitFlag = "WEB_RESEND_PENDING",//"RESEND",
                                RejectReason = resultProductList.RejectReason.ToSafeString(),
                                FOA_Submit_date = resultProductList.FOA_Submit_date.ToSafeString(),
                                OLT_NAME = resultProductList.OLT_NAME.ToSafeString(),
                                BUILDING_NAME = resultProductList.BUILDING_NAME.ToSafeString(),
                                Mobile_Contact = resultProductList.Mobile_Contact.ToSafeString(),

                                Post_Date = replaceModel.postDate,
                                Address_ID = resultProductList.ADDRESS_ID.ToSafeString(),
                                ORG_ID = resultProductList.ORG_ID.ToSafeString(),
                                Reuse_Flag = resultProductList.REUSE_FLAG.ToSafeString(),
                                Event_Flow_Flag = resultProductList.EVENT_FLOW_FLAG.ToSafeString(),
                                UserName = base.CurrentUser.UserName.ToSafeString(),
                                Subcontract_Type = resultProductList.SUBCONTRACT_TYPE.ToSafeString(),
                                Subcontract_Sub_Type = resultProductList.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                                Request_Sub_Flag = resultProductList.REQUEST_SUB_FLAG.ToSafeString(),
                                Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString()
                            };

                            _product = resultProductList.ProductList.Select(p =>
                            {
                                return new NewRegistFOAProductList()
                                {
                                    SerialNumber = p.SerialNumber.ToSafeString(),
                                    MaterialCode = p.MaterialCode.ToSafeString(),
                                    CompanyCode = p.CompanyCode.ToSafeString(),
                                    Plant = p.Plant.ToSafeString(),
                                    StorageLocation = p.StorageLocation.ToSafeString(),
                                    SNPattern = p.SNPattern.ToSafeString(),
                                    MovementType = p.MovementType.ToSafeString()
                                };
                            }).ToList();
                            ////}

                            _main.ProductList = _product;

                            var _services = new NewRegistFOAServiceList()
                            {
                                ServiceName = resultProductList.ServiceName != null ? resultProductList.ServiceName : ""
                            };

                            List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                            var subStr = _services.ServiceName.Split(',');
                            foreach (var service in subStr)
                            {
                                _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                            }
                            _main.ServiceList = _service;

                            _mainList.Add(_main);
                            orderno = resultlist.ORDER_NO;
                            accessnum = resultlist.ACCESS_NUMBER;
                        }



                    }
                    foreach (var item in _mainList)
                    {
                        NewRegistForSubmitFOAResponse resultNewRegist = _queryProcessor.Execute(item);
                        ////if (resultNewRegist.result == "0" || resultNewRegist.result == "000" || resultNewRegist.result == "") indexSuccess += 1;
                        if (resultNewRegist.result != "") indexSuccess += 1; else indexError += 1;
                    }

                }
            }

            if (indexSuccess > 0 || indexError > 0)
            {
                msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
            }

            return Json(msg);
        }




        public JsonResult ResendPendingAllSubmitFOARevalue(string dataS = "")
        {
            string msg = "";
            int indexSuccess = 0;
            int indexError = 0;

            if (!string.IsNullOrEmpty(dataS))
            {
                var searchModel = new JavaScriptSerializer().Deserialize<SubmitFOARevalueQuery>(dataS);
                searchModel.internetNo = string.IsNullOrEmpty(searchModel.internetNo.ToSafeString()) ? "ALL" : searchModel.internetNo.ToSafeString();
                searchModel.orderNo = string.IsNullOrEmpty(searchModel.orderNo.ToSafeString()) ? "ALL" : searchModel.orderNo.ToSafeString();
                searchModel.companyCode = string.IsNullOrEmpty(searchModel.companyCode.ToSafeString()) ? "ALL" : searchModel.companyCode.ToSafeString();
                searchModel.mainasset = string.IsNullOrEmpty(searchModel.mainasset.ToSafeString()) ? "ALL" : searchModel.mainasset.ToSafeString();
                searchModel.action = string.IsNullOrEmpty(searchModel.action.ToSafeString()) ? "ALL" : searchModel.action.ToSafeString();
                searchModel.status = string.IsNullOrEmpty(searchModel.status.ToSafeString()) ? "ALL" : searchModel.status.ToSafeString();

                var result = this.GetSubmitFOARevalue(searchModel);
                // 1.GetSubmitFOA
                #region comment
                //SubmitFOAEquipmentReportQuery q = new SubmitFOAEquipmentReportQuery();
                //q.dateFrom = searchModel.dateFrom;
                //q.dateTo = searchModel.dateTo;
                //q.internetNo = "ALL";
                //q.orderNo = "ALL";
                //q.orderType = "ALL";
                //q.productName = "ALL";
                //q.serviceName = "";
                //q.subcontractorCode = "ALL";
                //q.companyCode = "ALL";
                //q.materialCode = "ALL";
                //q.plant = "ALL";
                //q.storLocation = "ALL";
                //q.status = "PENDING";
                //List<string> ordertype = new List<string>();
                //ordertype.Add("TERMINATION");
                //ordertype.Add("RETURNED");
                //List<SubmitFOAEquipment> resultSubmitFOAListTemp = new List<SubmitFOAEquipment>();
                //resultSubmitFOAListTemp = this.GetSubmitFOA(q).Where(x=> result.Select(a=>a.ACCESS_NUMBER).Contains(x.ACCESS_NUMBER) && ordertype.Contains(x.ORDER_TYPE)).ToList();
                //List<SubmitFOAEquipment> resultSubmitFOAList = resultSubmitFOAListTemp.Where(p => p.STATUS != "COMPLETE" && p.STATUS != "ERROR")
                //                                                .Select(p =>
                //                                                {
                //                                                    return new SubmitFOAEquipment
                //                                                    {
                //                                                        ACCESS_NUMBER = p.ACCESS_NUMBER,
                //                                                        ORDER_NO = p.ORDER_NO,
                //                                                        ORDER_TYPE = p.ORDER_TYPE,
                //                                                        SUBCONTRACT_CODE = p.SUBCONTRACT_CODE,
                //                                                        SUBCONTRACT_NAME = p.SUBCONTRACT_NAME,
                //                                                        PRODUCT_NAME = p.PRODUCT_NAME,
                //                                                        SERVICE_NAME = p.SERVICE_NAME,
                //                                                        SUBMIT_FLAG = p.SUBMIT_FLAG,
                //                                                        REJECT_REASON = p.REJECT_REASON,
                //                                                        SUBMIT_DATE = p.SUBMIT_DATE,
                //                                                        OLT_NAME = p.OLT_NAME,
                //                                                        BUILDING_NAME = p.BUILDING_NAME,
                //                                                        MOBILE_CONTACT = p.MATERIAL_CODE,
                //                                                        SN = p.SN,
                //                                                        MATERIAL_CODE = p.MATERIAL_CODE,
                //                                                        COMPANY_CODE = p.COMPANY_CODE,
                //                                                        PLANT = p.PLANT,
                //                                                        STORAGE_LOCATION = p.STORAGE_LOCATION,
                //                                                        SN_TYPE = p.SN_TYPE,
                //                                                        MOVEMENT_TYPE = p.MOVEMENT_TYPE_IN,
                //                                                        STATUS = p.STATUS,
                //                                                        ERR_CODE = p.ERR_CODE,
                //                                                        ERR_MSG = p.ERR_MSG,
                //                                                        ADDRESS_ID = p.ADDRESS_ID.ToSafeString(),
                //                                                        ORG_ID = p.ORG_ID.ToSafeString(),
                //                                                        REUSE_FLAG = p.REUSE_FLAG.ToSafeString(),
                //                                                        EVENT_FLOW_FLAG = p.EVENT_FLOW_FLAG.ToSafeString(),
                //                                                    };
                //                                                }).ToList();

                //string orderno = "", accessnum = "";
                //if (resultSubmitFOAList.Any())
                //{
                //    var _mainList = new List<NewRegistForSubmitFOAQuery>();
                //    foreach (var resultlist in resultSubmitFOAList)
                //    {
                //        // 2.GetProductList
                //        var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                //        var resultProductList = _queryProcessor.Execute(queryProductList);

                //        var _main = new NewRegistForSubmitFOAQuery();
                //        var _product = new List<NewRegistFOAProductList>();

                //        // 3.ResendSubmitFOA
                //        //if (resultlist.STATUS == "Pending")
                //        //{
                //        if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                //        {
                //            _main = new NewRegistForSubmitFOAQuery()
                //            {
                //                Access_No = resultProductList.AccessNo.ToSafeString(),
                //                OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                //                SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                //                SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                //                ProductName = resultProductList.ProductName.ToSafeString(),
                //                OrderType = resultProductList.OrderType.ToSafeString(),
                //                SubmitFlag = "RESEND",
                //                RejectReason = resultProductList.RejectReason.ToSafeString(),
                //                FOA_Submit_date = resultProductList.FOA_Submit_date.ToSafeString(),
                //                OLT_NAME = resultProductList.OLT_NAME.ToSafeString(),
                //                BUILDING_NAME = resultProductList.BUILDING_NAME.ToSafeString(),
                //                Mobile_Contact = resultProductList.Mobile_Contact.ToSafeString(),

                //                Post_Date = "",
                //                Address_ID = resultProductList.ADDRESS_ID.ToSafeString(),
                //                ORG_ID = resultProductList.ORG_ID.ToSafeString(),
                //                Reuse_Flag = resultProductList.REUSE_FLAG.ToSafeString(),
                //                Event_Flow_Flag = resultProductList.EVENT_FLOW_FLAG.ToSafeString(),
                //                UserName = base.CurrentUser.UserName.ToSafeString()
                //            };

                //            _product = resultProductList.ProductList.Select(p =>
                //            {
                //                return new NewRegistFOAProductList()
                //                {
                //                    SerialNumber = p.SerialNumber.ToSafeString(),
                //                    MaterialCode = p.MaterialCode.ToSafeString(),
                //                    CompanyCode = p.CompanyCode.ToSafeString(),
                //                    Plant = p.Plant.ToSafeString(),
                //                    StorageLocation = p.StorageLocation.ToSafeString(),
                //                    SNPattern = p.SNPattern.ToSafeString(),
                //                    MovementType = p.MovementType.ToSafeString()
                //                };
                //            }).ToList();
                //            //}

                //            _main.ProductList = _product;

                //            var _services = new NewRegistFOAServiceList()
                //            {
                //                ServiceName = resultProductList.ServiceName != null ? resultProductList.ServiceName : ""
                //            };

                //            List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                //            var subStr = _services.ServiceName.Split(',');
                //            foreach (var service in subStr)
                //            {
                //                _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                //            }
                //            _main.ServiceList = _service;

                //            _mainList.Add(_main);
                //            orderno = resultlist.ORDER_NO;
                //            accessnum = resultlist.ACCESS_NUMBER;
                //        }
                //    }


                //    foreach (var item in _mainList)
                //    {
                //        NewRegistForSubmitFOAResponse resultNewRegist = _queryProcessor.Execute(item);
                //        //if (resultNewRegist.result == "0" || resultNewRegist.result == "000" || resultNewRegist.result == "") indexSuccess += 1;
                //        if (resultNewRegist.result != "") indexSuccess += 1; else indexError += 1;
                //    }

                //}  
                #endregion
                var LovQueryData = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "ACTION");
                foreach (var item in result)
                {
                    string action = LovQueryData.Where(x => x.DISPLAY_VAL == item.ACTION).Select(x => x.VAL1).FirstOrDefault();
                    var query = new NewRegistForSubmitFOARevaluePendingQuery()
                    {
                        ACCESS_NUMBER = item.ACCESS_NUMBER,
                        ORDER_NO = item.ORDER_NO,
                        ORDER_TYPE = item.ORDER_TYPE,
                        RUN_GROUP = item.RUN_GROUP,
                        ACTION = action,
                        MAIN_ASSET = item.MAIN_ASSET,
                        SUB_NUMBER = item.SUB_NUMBER,
                        COM_CODE = item.COM_CODE,
                        DOC_DATE = item.DOC_DATE,
                        ERR_CODE = item.ERR_CODE,
                        ERR_MSG = item.ERR_MSG,
                        STATUS = item.STATUS,
                        TRANS_ID = item.TRANS_ID,
                        ITEM_TEXT = item.ITEM_TEXT
                    };
                    NewRegistForSubmitFOAResponse resultNewRegist = _queryProcessor.Execute(query);
                    if (resultNewRegist.result != "") indexSuccess += 1; else indexError += 1;
                }
            }

            if (indexSuccess > 0 || indexError > 0)
            {
                msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
            }

            return Json(msg);
        }

        //ch0002 Start
        [HttpPost]
        public JsonResult UpdateStatusSubmitFOAEquipment(string dataS)
        {
            string MSG = string.Empty;
            string code = string.Empty;

            try
            {
                var updateModel = new JavaScriptSerializer().Deserialize<UpdateStatusSubmitFOAEquipmentModel>(dataS);
                if (updateModel != null)
                {
                    string[] sn = updateModel.SERIAL_NO.Split(',');
                    string[] com_code = updateModel.COM_CODE.Split(',');
                    for (int i = 0; i < sn.Count(); i++)
                    {
                        var addresult = new UpdateFOAResendStatusCommand
                        {
                            P_TRANS_ID = updateModel.TRANS_ID,
                            P_ORDER_NO = updateModel.ORDER_NO,
                            //P_SERIAL_NO = updateModel.SERIAL_NO,
                            P_SERIAL_NO = sn[i],
                            P_INTERNET_NO = updateModel.INTERNET_NO,
                            P_SUBNUMBER = updateModel.SUBNUMBER,
                            P_ASSET_CODE = updateModel.ASSET_CODE,
                            P_MATERIAL_DOC = updateModel.MATERIAL_DOC,
                            P_DOC_YEAR = updateModel.DOC_YEAR,
                            //P_COM_CODE = updateModel.COM_CODE,
                            P_COM_CODE = com_code[i],
                            P_ERR_CODE = "0000",
                            P_ERR_MSG = "",
                            P_REMARK = updateModel.REMARK
                        };
                        _updateFOAStatusTypeCommand.Handle(addresult);
                        if (addresult.ret_code != null)
                        {
                            code = addresult.ret_code.ToSafeString();
                            MSG = addresult.ret_msg.ToSafeString();
                        }
                    }
                    return Json(new
                    {
                        code = code,
                        msg = MSG
                    }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new
                    {
                        code = "-1",
                        msg = "Data is not correct, Please Check Data or Contact System Admin"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string _msg = string.Empty;
                _msg = "Please Contact System Admin";
                return Json(new { code = "-1", msg = _msg }, JsonRequestBehavior.AllowGet);
            }



        }
        //end ch0002

        // Set DDL Status
        public JsonResult SetDDLStatus()
        {
            var model = new List<LovModel>();
            model.Add(new LovModel() { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            model.Add(new LovModel() { LOV_NAME = "COMPLETE", LOV_VAL1 = "COMPLETE" });
            model.Add(new LovModel() { LOV_NAME = "PENDING", LOV_VAL1 = "Pending" });
            model.Add(new LovModel() { LOV_NAME = "ERROR", LOV_VAL1 = "ERROR" });
            var LovData = model.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set DDL ProductName
        public JsonResult SetDDLProductName(string grid = "")
        {
            var queryProductName = new GetListFbssFixedAssetConfigQuery() { DDLName = "ProductName" };
            var LovProductNameData = _queryProcessor.Execute(queryProductName);
            var LovData = LovProductNameData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set DDL OrderType
        public JsonResult SetDDLOrderType()
        {
            var LovQueryData = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "ORDER_TYPE");
            var LovData = LovQueryData.Select(p => { return new { LOV_NAME = p.VAL1, LOV_VAL1 = p.VAL1 }; }).ToList();
            LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            //var model = new List<LovModel>();
            //model.Add(new LovModel() { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            //model.Add(new LovModel() { LOV_NAME = "JOIN", LOV_VAL1 = "JOIN" });
            //model.Add(new LovModel() { LOV_NAME = "NEW", LOV_VAL1 = "NEW" });
            //model.Add(new LovModel() { LOV_NAME = "MA", LOV_VAL1 = "MA" });
            //model.Add(new LovModel() { LOV_NAME = "HUMANERROR", LOV_VAL1 = "HUMANERROR" });
            ////model.Add(new LovModel() { LOV_NAME = "RENEW", LOV_VAL1 = "RENEW" });
            //var LovData = model.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set DDL CompanyCode
        public JsonResult SetDDLCompanyCode(string grid)
        {
            var queryCompanyCode = new GetListFbssFixedAssetConfigQuery() { DDLName = "CompanyCode" };
            var LovCompanyCodeData = _queryProcessor.Execute(queryCompanyCode);
            var LovData = LovCompanyCodeData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set DDL ServiceName
        public JsonResult SetDDLServiceName()
        {
            var queryServiceName = new GetListFbssFixedAssetConfigQuery() { DDLName = "ServiceName" };
            var LovServiceNameData = _queryProcessor.Execute(queryServiceName);
            var LovData = LovServiceNameData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "" });
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set DDL SubcontractorCode
        public JsonResult SetDDLSubcontractorCode(string grid)
        {
            var querySubcontractorCode = new GetListFbssFoaVendorCodeQuery();
            var LovSubcontractorCodeData = _queryProcessor.Execute(querySubcontractorCode);
            var LovData = LovSubcontractorCodeData.Select(p => { return new { LOV_NAME = p.LOV_NAME.ToUpper(), LOV_VAL1 = p.LOV_VAL1.ToUpper() }; })
                .Distinct()
                //  .OrderBy(x => x.LOV_NAME.PadLeft(20, '0'))
                .OrderBy(x => x.LOV_NAME)
                .ToList();

            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }

            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set DDL Plant
        public JsonResult SetDDLPlant(string CompanyCode, string grid)
        {
            var queryPlant = new GetListFbssFixedAssetConfigQuery() { DDLName = "Plant", Param1 = CompanyCode };
            var LovPlantData = _queryProcessor.Execute(queryPlant);
            var LovData = LovPlantData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();

            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                if (string.IsNullOrEmpty(CompanyCode)) LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }

            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        // Set DDL MaterialCode
        public JsonResult SetDDLMaterialCode(string grid)
        {
            var queryMaterial = new GetMaterialCodeQuery();
            var LovMaterialData = _queryProcessor.Execute(queryMaterial);
            var LovData = LovMaterialData.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }
        // Set DDL SubcontractorCode
        public JsonResult SetDDLSNPattern(string grid)
        {
            var query = new GetLovV2Query()
            {
                LovType = "SN_PATT",
                LovVal5 = "FIXED_LASTMILE"

            };
            var result = _queryProcessor.Execute(query).ToList();
            var resultLovData = result.Select(p => { return new { LOV_NAME = p.Name, LOV_VAL1 = p.LovValue1 }; }).ToList();
            if (grid == "grid")
            {
                resultLovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                resultLovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(resultLovData, JsonRequestBehavior.AllowGet);
        }

        // main asset
        public JsonResult SetDDLCostCenter(string grid)
        {
            var LovQueryData = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "COST_CENTER");
            var LovData = LovQueryData.Select(p => { return new { LOV_NAME = p.VAL1, LOV_VAL1 = p.VAL1 }; }).ToList();
            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetDDLAssetClass(string grid)
        {
            var LovQueryData = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "ASSET_CLASS");
            var LovData = LovQueryData.Select(p => { return new { LOV_NAME = p.VAL1, LOV_VAL1 = p.VAL1 }; }).ToList();
            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetDDLMainAssetStatus(string grid)
        {
            var LovQueryData = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "STATUS");
            var LovData = LovQueryData.Select(p => { return new { LOV_NAME = p.VAL1, LOV_VAL1 = p.VAL1 }; }).ToList();
            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetDDLAction(string grid)
        {
            var LovQueryData = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "ACTION");
            var LovData = LovQueryData.Select(p => { return new { LOV_NAME = p.DISPLAY_VAL, LOV_VAL1 = p.VAL1 }; }).ToList();
            if (grid == "grid")
            {
                LovData.Insert(0, new { LOV_NAME = "", LOV_VAL1 = "-- Please Select One --" });
            }
            else
            {
                LovData.Insert(0, new { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            }
            return Json(LovData, JsonRequestBehavior.AllowGet);

        }

        // Set DDL ProductOwner
        public JsonResult SetDDLProductOwner()
        {
            var data = Get_CONFIG_LOV("DROPDOWNLIST", "Product Owner").Where(d => d.LovValue1 != null).ToList();

            //var LovQueryData = _queryProcessor.Execute(query).ToList();
            //var LovData = LovQueryData.Select(p => { return new { LOV_NAME = p.LovValue1, LOV_VAL1 = p.LovValue1 }; }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public List<LovValueModel> Get_CONFIG_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetLovQuery()
            {
                LovType = _CON_TYPE,
                LovName = _CON_NAME
            };
            var _ConfigLov = _queryProcessor.Execute(query).OrderBy(p => p.OrderBy).ToList();

            return _ConfigLov;
        }

        public List<LovModel> GET_FBSS_FIXED_ASSET_CONFIG(string product_name)
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = product_name
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }

        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME
            };
            var _FbssConfig = _queryProcessor.Execute(query).OrderBy(p => p.ORDER_BY).ToList();

            return _FbssConfig;
        }

        //ch0001 start
        public JsonResult SetDDLEditStatus()
        {
            var model = new List<LovModel>();
            model.Add(new LovModel() { LOV_NAME = "-- Select All --", LOV_VAL1 = "ALL" });
            model.Add(new LovModel() { LOV_NAME = "COMPLETE", LOV_VAL1 = "COMPLETE" });
            model.Add(new LovModel() { LOV_NAME = "ERROR", LOV_VAL1 = "ERROR" });
            var LovData = model.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 }; }).ToList();
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }
        //ch0001 end

        //end
        #endregion

        #region Not Use

        public JsonResult ResendSubmitFOA(string main, string product)//NewRegistForSubmitFOAQuery model, 
        {
            var _main = new JavaScriptSerializer().Deserialize<NewRegistForSubmitFOAQuery>(main);
            var _product = new JavaScriptSerializer().Deserialize<List<NewRegistFOAProductList>>(product);
            _main.ProductList = _product;

            var _services = new JavaScriptSerializer().Deserialize<NewRegistFOAServiceList>(main);
            List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
            var subStr = _services.ServiceName.Split(',');
            foreach (var service in subStr)
            {
                _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
            }
            _main.ServiceList = _service;

            //var resend = new ResendSubmitFOA() { main = _main, product = _product };
            //return null;
            return Json(_queryProcessor.Execute(_main));
        }


        // Not use
        //public ActionResult SubmitFOAReportAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        //{
        //    if (dataS != null && dataS != "")
        //    {
        //        var searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAReportQuery>(dataS);
        //        var result = this.GetSubmitFOA(searchModel);

        //        return Json(result.ToDataSourceResult(request));
        //        //return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        // Not use
        //public List<SubmitFOAReport> GetSubmitFOA(SubmitFOAReportQuery model)
        //{
        //    try
        //    {
        //        return _queryProcessor.Execute(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _Logger.Info(ex.GetErrorMessage());
        //        return new List<SubmitFOAReport>();
        //    }
        //}

        #endregion

        #region Export Excel

        public ActionResult Export_equip_Report(string dataS)
        {
            SubmitFOAEquipmentReportQuery searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAEquipmentReportQuery>(dataS);
            searchModel.page_index = 1;
            searchModel.page_size = decimal.MaxValue;
            string activetflag = "";
            var resuftlov = Get_FBSS_CONFIG_TBL_LOV("RESEND_ORDER", "PAGESIZE").FirstOrDefault(l => l.ACTIVEFLAG == "Y");
            activetflag = resuftlov is null ? "N" : "Y";
            var result = GetSubmitFOAEquipment(searchModel);
            if (activetflag == "Y")
            {
                result.cur = (from item in result.cur
                              orderby item.ACCESS_NUMBER, item.ORDER_NO, item.SUBMIT_DATE
                              select item).ToList();
            }
            string filename = GetExcelName("Equip_Report");


            //// var resultcur = GetSubmitFOAEquipmentConvertCurToIEnumerable(result.cur.ToList(), activetflag);
            var bytes = Generate_equip_Report<SubmitFOAEquipment>(result.cur, filename, "report_Equip", searchModel);
            return File(bytes, "application/excel", filename + ".xls");
        }

        public ActionResult Export_Installation_Report(string dataS)
        {
            SubmitFOAInstallationReportQuery searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAInstallationReportQuery>(dataS);
            SubmitFOAEquipmentReportQuery searchModel_display = new JavaScriptSerializer().Deserialize<SubmitFOAEquipmentReportQuery>(dataS);
            var result = GetSubmitFOAInstallation(searchModel);
            string filename = GetExcelName("Installation_Report");

            var bytes = Generate_installation_Report<SubmitFOAInstallation>(result, filename, "report_Installation", searchModel, searchModel_display);
            return File(bytes, "application/excel", filename + ".xls");
        }
        //main asset
        public ActionResult Export_MainAsset_Report(string dataS)
        {
            SubmitFOAMainAssetQuery searchModel = new JavaScriptSerializer().Deserialize<SubmitFOAMainAssetQuery>(dataS);
            searchModel.internetNo = string.IsNullOrEmpty(searchModel.internetNo.ToSafeString()) ? "ALL" : searchModel.internetNo.ToSafeString();
            searchModel.orderNo = string.IsNullOrEmpty(searchModel.orderNo.ToSafeString()) ? "ALL" : searchModel.orderNo.ToSafeString();
            searchModel.companyCode = string.IsNullOrEmpty(searchModel.companyCode.ToSafeString()) ? "ALL" : searchModel.companyCode.ToSafeString();
            searchModel.assetClass = string.IsNullOrEmpty(searchModel.assetClass.ToSafeString()) ? "ALL" : searchModel.assetClass.ToSafeString();
            searchModel.status = string.IsNullOrEmpty(searchModel.status.ToSafeString()) ? "ALL" : searchModel.status.ToSafeString();
            var result = GetSubmitFOAMainAsset(searchModel);
            string filename = GetExcelName("MainAsset_Report");

            var bytes = Generate_MainAsset_Report<SubmitFOAMainAsset>(result, filename, "report_mainasset", searchModel);
            return File(bytes, "application/excel", filename + ".xls");
        }
        //
        //revalue
        public ActionResult Export_Revalue_Report(string dataS)
        {
            SubmitFOARevalueQuery searchModel = new JavaScriptSerializer().Deserialize<SubmitFOARevalueQuery>(dataS);
            searchModel.internetNo = string.IsNullOrEmpty(searchModel.internetNo.ToSafeString()) ? "ALL" : searchModel.internetNo.ToSafeString();
            searchModel.orderNo = string.IsNullOrEmpty(searchModel.orderNo.ToSafeString()) ? "ALL" : searchModel.orderNo.ToSafeString();
            searchModel.companyCode = string.IsNullOrEmpty(searchModel.companyCode.ToSafeString()) ? "ALL" : searchModel.companyCode.ToSafeString();
            searchModel.action = string.IsNullOrEmpty(searchModel.action.ToSafeString()) ? "ALL" : searchModel.action.ToSafeString();
            searchModel.status = string.IsNullOrEmpty(searchModel.status.ToSafeString()) ? "ALL" : searchModel.status.ToSafeString();
            searchModel.status = string.IsNullOrEmpty(searchModel.status.ToSafeString()) ? "ALL" : searchModel.status.ToSafeString();
            searchModel.mainasset = string.IsNullOrEmpty(searchModel.mainasset.ToSafeString()) ? "ALL" : searchModel.mainasset.ToSafeString();
            var result = GetSubmitFOARevalue(searchModel);
            string filename = GetExcelName("Revalue_Report");

            var bytes = Generate_Revalue_Report<SubmitFOARevalue>(result, filename, "report_revalue", searchModel);
            return File(bytes, "application/excel", filename + ".xls");
        }
        //
        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }
        // main asset
        public byte[] Generate_MainAsset_Report<T>(List<T> data, string fileName, string LovValue5, SubmitFOAMainAssetQuery searchModel)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            table.Columns.Add("ACCESS NO", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER NO", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER TYPE", System.Type.GetType("System.String"));
            table.Columns.Add("FLAG TYPE", System.Type.GetType("System.String"));
            table.Columns.Add("SUBCONTACT CODE", System.Type.GetType("System.String"));
            table.Columns.Add("SUBCONTACT NAME", System.Type.GetType("System.String"));
            table.Columns.Add("PRODUCT NAME", System.Type.GetType("System.String"));
            table.Columns.Add("COMPANY CODE", System.Type.GetType("System.String"));
            table.Columns.Add("ASSET CODE", System.Type.GetType("System.String"));
            table.Columns.Add("COST CENTER", System.Type.GetType("System.String"));
            table.Columns.Add("SUBMIT DATE", System.Type.GetType("System.String"));

            object[] values = new object[table.Columns.Count];
            foreach (T item in data)
            {
                values[0] = props["ACCESS_NUMBER"].GetValue(item);
                values[1] = props["ORDER_NO"].GetValue(item);
                values[2] = props["ORDER_TYPE"].GetValue(item);
                values[3] = props["FLAG_TYPE"].GetValue(item);
                values[4] = props["SUBCONTRACT_CODE"].GetValue(item);
                values[5] = props["SUBCONTRACT_NAME"].GetValue(item);
                values[6] = props["PRODUCT_NAME"].GetValue(item);
                values[7] = props["COM_CODE_NEW"].GetValue(item);
                values[8] = props["ASSET_CLASS_GI"].GetValue(item);
                values[9] = props["COST_CENTER"].GetValue(item);
                values[10] = props["FOA_SUBMIT_DATE"].GetValue(item);

                table.Rows.Add(values);
            }
            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateMainAssetExcel(table, "WorkSheet", tempPath, fileName, LovValue5, searchModel);
            return data_;
        }
        //
        // revalue 
        public byte[] Generate_Revalue_Report<T>(List<T> data, string fileName, string LovValue5, SubmitFOARevalueQuery searchModel)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            table.Columns.Add("ACCESS NO", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER NO", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER TYPE", System.Type.GetType("System.String"));
            table.Columns.Add("RUN GROUP", System.Type.GetType("System.String"));
            table.Columns.Add("ACTION", System.Type.GetType("System.String"));
            table.Columns.Add("MAIN ASSET", System.Type.GetType("System.String"));
            table.Columns.Add("SUB NUMBER", System.Type.GetType("System.String"));
            table.Columns.Add("COMPANY CODE", System.Type.GetType("System.String"));
            table.Columns.Add("DOC DATE", System.Type.GetType("System.String"));
            table.Columns.Add("ERROR CODE", System.Type.GetType("System.String"));
            table.Columns.Add("ERROR MESSAGE", System.Type.GetType("System.String"));
            table.Columns.Add("PRODUCT OWNER", System.Type.GetType("System.String"));

            object[] values = new object[table.Columns.Count];
            foreach (T item in data)
            {
                values[0] = props["ACCESS_NUMBER"].GetValue(item);
                values[1] = props["ORDER_NO"].GetValue(item);
                values[2] = props["ORDER_TYPE"].GetValue(item);
                values[3] = props["RUN_GROUP"].GetValue(item);
                values[4] = props["ACTION"].GetValue(item);
                values[5] = props["MAIN_ASSET"].GetValue(item);
                values[6] = props["SUB_NUMBER"].GetValue(item);
                values[7] = props["COM_CODE"].GetValue(item);
                values[8] = props["DOC_DATE"].GetValue(item);
                values[9] = props["ERR_CODE"].GetValue(item);
                values[10] = props["ERR_MSG"].GetValue(item);
                values[11] = props["PRODUCT_OWNER"].GetValue(item);
                table.Rows.Add(values);
            }
            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateRevalueExcel(table, "WorkSheet", tempPath, fileName, LovValue5, searchModel);
            return data_;
        }
        //
        //
        public byte[] Generate_installation_Report<T>(List<T> data, string fileName, string LovValue5, SubmitFOAInstallationReportQuery searchModel, SubmitFOAEquipmentReportQuery searchModel_display)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            //ACCESS NO	ASSET CODE	SUB NUMBER	PRODUCT NAME	INSTALLATION COST	ORDER DATE	MODIFY DATE	STATUS
            table.Columns.Add("ACCESS NO", System.Type.GetType("System.String"));
            table.Columns.Add("ASSET COD", System.Type.GetType("System.String"));
            table.Columns.Add("SUB NUMBER", System.Type.GetType("System.String"));
            table.Columns.Add("PRODUCT NAME", System.Type.GetType("System.String"));
            table.Columns.Add("INSTALLATION COST", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER DATE", System.Type.GetType("System.String"));
            table.Columns.Add("MODIFY DATE", System.Type.GetType("System.String"));
            table.Columns.Add("STATUS", System.Type.GetType("System.String"));
            table.Columns.Add("PRODUCT OWNER", System.Type.GetType("System.String"));
            table.Columns.Add("TRANS ID", System.Type.GetType("System.String"));

            object[] values = new object[table.Columns.Count];
            foreach (T item in data)
            {
                values[0] = props["ACCESS_NUMBER"].GetValue(item);
                values[1] = props["ASSET_CODE"].GetValue(item);
                values[2] = props["SUB_NUMBER"].GetValue(item);
                values[3] = props["PRODUCT_NAME"].GetValue(item);
                values[4] = props["INSTALLATION_COST"].GetValue(item);
                values[5] = props["ORDER_DATE"].GetValue(item);
                values[6] = props["MODIFY_DATE"].GetValue(item);
                values[7] = props["TRAN_STATUS"].GetValue(item);
                values[8] = props["PRODUCT_OWNER"].GetValue(item);
                values[9] = props["TRANS_ID"].GetValue(item);
                table.Rows.Add(values);
            }
            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateinstallationExcel(table, "WorkSheet", tempPath, fileName, LovValue5, searchModel, searchModel_display);
            return data_;
        }

        public byte[] Generate_equip_Report<T>(List<T> data, string fileName, string LovValue5, SubmitFOAEquipmentReportQuery searchModel)
        {
            _Logger.Info("GenerateSaleTrackingEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            //ACCESS NO	ORDER NUMBER	SUBCONTRACTOR CODE	SUBCONTRACTOR NAME	PRODUCT NAME	ORDER TYPE	SUBMIT DATE	
            //SERIAL NUMBER	MATERIAL CODE	COMPANY CODE	PLANT	STORAGE LOCATION	SN PATTERN MOVEMENT 
            //ERROR CODE ERROR MESSAGE
            table.Columns.Add("ACCESS NO", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER NUMBER", System.Type.GetType("System.String"));
            table.Columns.Add("SUBCONTRACTOR CODE", System.Type.GetType("System.String"));
            table.Columns.Add("SUBCONTRACTOR NAME", System.Type.GetType("System.String"));
            table.Columns.Add("PRODUCT NAME", System.Type.GetType("System.String"));
            table.Columns.Add("ORDER TYPE", System.Type.GetType("System.String"));
            table.Columns.Add("SUBMIT DATE", System.Type.GetType("System.String"));
            table.Columns.Add("SERIAL NUMBER", System.Type.GetType("System.String"));
            table.Columns.Add("MATERIAL CODE", System.Type.GetType("System.String"));
            table.Columns.Add("COMPANY CODE", System.Type.GetType("System.String"));
            table.Columns.Add("PLANT", System.Type.GetType("System.String"));
            table.Columns.Add("STORAGE LOCATION", System.Type.GetType("System.String"));
            table.Columns.Add("SN PATTERN", System.Type.GetType("System.String"));
            table.Columns.Add("MOVEMENT TYPE", System.Type.GetType("System.String"));
            table.Columns.Add("ERROR CODE", System.Type.GetType("System.String"));
            table.Columns.Add("ERROR MESSAGE", System.Type.GetType("System.String"));
            table.Columns.Add("STATUS", System.Type.GetType("System.String"));
            table.Columns.Add("MAIN ASSET", System.Type.GetType("System.String"));
            table.Columns.Add("MATERIAL DOC", System.Type.GetType("System.String"));
            table.Columns.Add("DOC YEAR", System.Type.GetType("System.String"));
            table.Columns.Add("REMARK", System.Type.GetType("System.String"));
            table.Columns.Add("CHANGE STATUS", System.Type.GetType("System.String"));
            table.Columns.Add("PRODUCT OWNER", System.Type.GetType("System.String"));
            table.Columns.Add("TRANS ID", System.Type.GetType("System.String"));

            object[] values = new object[table.Columns.Count];
            foreach (T item in data)
            {
                string errmsg = props["ERR_MSG"].GetValue(item).ToSafeString();
                string storelocat = props["STORAGE_LOCATION"].GetValue(item).ToSafeString();
                string substringmsg = string.Empty;
                if (errmsg.Contains("current plant") && errmsg.Contains("current sloc."))
                {
                    substringmsg = errmsg.Substring(errmsg.Length - 4, 4);
                }
                else
                {
                    substringmsg = "";
                }

                values[0] = props["ACCESS_NUMBER"].GetValue(item);
                values[1] = props["ORDER_NO"].GetValue(item);
                values[2] = props["SUBCONTRACT_CODE"].GetValue(item);
                values[3] = props["SUBCONTRACT_NAME"].GetValue(item);
                values[4] = props["PRODUCT_NAME"].GetValue(item);
                values[5] = props["ORDER_TYPE"].GetValue(item);
                values[6] = props["SUBMIT_DATE"].GetValue(item);
                values[7] = props["SN"].GetValue(item);
                values[8] = props["MATERIAL_CODE"].GetValue(item);
                values[9] = props["COMPANY_CODE"].GetValue(item);
                values[10] = props["PLANT"].GetValue(item);

                if (substringmsg == "")
                {
                    values[11] = storelocat;
                }
                else
                {
                    values[11] = substringmsg;
                }

                values[12] = props["SN_TYPE"].GetValue(item);
                values[13] = props["MOVEMENT_TYPE_OUT"].GetValue(item);
                values[14] = props["ERR_CODE"].GetValue(item);
                values[15] = props["ERR_MSG"].GetValue(item);
                values[16] = props["STATUS"].GetValue(item);
                values[17] = "";
                values[18] = "";
                values[19] = "";
                values[20] = "";
                values[21] = "";
                values[22] = props["PRODUCT_OWNER"].GetValue(item);
                values[23] = props["TRANS_ID"].GetValue(item);
                table.Rows.Add(values);
            }
            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateequipExcel(table, "WorkSheet", tempPath, fileName, LovValue5, searchModel);
            return data_;
        }

        private byte[] GenerateinstallationExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5, SubmitFOAInstallationReportQuery searchModel, SubmitFOAEquipmentReportQuery searchModel_display)
        {
            _Logger.Info("GenerateSaleTrackingExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 9;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText("Report Name  : INSTALLATION");

                worksheet.Cells["A3:D3"].Merge = true;
                worksheet.Cells["A3,D3"].LoadFromText("Access No : " + searchModel_display.internetNo);
                worksheet.Cells["E3:H3"].Merge = true;
                worksheet.Cells["E3,H3"].LoadFromText("Order Number : " + searchModel_display.orderNo);
                worksheet.Cells["I3:L3"].Merge = true;
                worksheet.Cells["I3,L3"].LoadFromText("Status : " + searchModel_display.status);

                worksheet.Cells["A4:D4"].Merge = true;
                worksheet.Cells["A4,D4"].LoadFromText("Product Name : " + searchModel_display.productName);
                worksheet.Cells["E4:H4"].Merge = true;
                worksheet.Cells["E4,H4"].LoadFromText("Order Type : " + searchModel_display.orderType);
                worksheet.Cells["I4:L4"].Merge = true;
                worksheet.Cells["I4,L4"].LoadFromText("Company Code : " + searchModel_display.companyCode);

                worksheet.Cells["A5:D5"].Merge = true;
                worksheet.Cells["A5,D5"].LoadFromText("Service Name : " + searchModel_display.serviceName);
                worksheet.Cells["E5:H5"].Merge = true;
                worksheet.Cells["E5,H5"].LoadFromText("Subcontractor Code : " + searchModel_display.subcontractorCode);
                worksheet.Cells["I5:L5"].Merge = true;
                worksheet.Cells["I5,L5"].LoadFromText("Plant : " + searchModel_display.plant);

                worksheet.Cells["A6:D6"].Merge = true;
                worksheet.Cells["A6,D6"].LoadFromText("Material Code : " + searchModel_display.materialCode);
                worksheet.Cells["E6:H6"].Merge = true;
                worksheet.Cells["E6,H6"].LoadFromText("Storage Location : " + searchModel_display.storLocation);

                worksheet.Cells["A7:D7"].Merge = true;
                worksheet.Cells["A7,D7"].LoadFromText("Order Create From : " + searchModel_display.dateFrom);
                worksheet.Cells["E7:H7"].Merge = true;
                worksheet.Cells["E7,H7"].LoadFromText("Order Create To : " + searchModel_display.dateTo);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 7, 9]; //[2, 1, 4, 4];
                                                                         //rangeReportDetail = worksheet.SelectedRange[]
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 8;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }

        private byte[] GenerateMainAssetExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5, SubmitFOAMainAssetQuery searchModel)
        {
            _Logger.Info("GenerateMainAssetExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 8;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText("Report Name  : MainAsset");

                worksheet.Cells["A3:D3"].Merge = true;
                worksheet.Cells["A3,D3"].LoadFromText("Access No : " + searchModel.internetNo);
                worksheet.Cells["E3:H3"].Merge = true;
                worksheet.Cells["E3,H3"].LoadFromText("Order Number : " + searchModel.orderNo);
                worksheet.Cells["I3:L3"].Merge = true;
                worksheet.Cells["I3,L3"].LoadFromText("Status : " + searchModel.status);

                worksheet.Cells["A4:D4"].Merge = true;
                worksheet.Cells["A4,D4"].LoadFromText("Company Code : " + searchModel.companyCode);
                worksheet.Cells["E4:H4"].Merge = true;
                worksheet.Cells["E4,H4"].LoadFromText("Asset Class : " + searchModel.assetClass);

                worksheet.Cells["A5:D5"].Merge = true;
                worksheet.Cells["A5,D5"].LoadFromText("Order Create From : " + searchModel.dateFrom);
                worksheet.Cells["E5:H5"].Merge = true;
                worksheet.Cells["E5,H5"].LoadFromText("Order Create To : " + searchModel.dateTo);

                rangeReportDetail = worksheet.SelectedRange[2, 1, 7, 9]; //[2, 1, 4, 4];
                                                                         //rangeReportDetail = worksheet.SelectedRange[]
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 8;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }


        private byte[] GenerateRevalueExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5, SubmitFOARevalueQuery searchModel)
        {
            _Logger.Info("GenerateRevalueExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 12;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText("Report Name  : Revalue");

                worksheet.Cells["A3:D3"].Merge = true;
                worksheet.Cells["A3,D3"].LoadFromText("Access No : " + searchModel.internetNo);
                worksheet.Cells["E3:H3"].Merge = true;
                worksheet.Cells["E3,H3"].LoadFromText("Order Number : " + searchModel.orderNo);
                worksheet.Cells["I3:L3"].Merge = true;
                worksheet.Cells["I3,L3"].LoadFromText("Status : " + searchModel.status);

                worksheet.Cells["A4:D4"].Merge = true;
                worksheet.Cells["A4,D4"].LoadFromText("Company Code : " + searchModel.companyCode);
                worksheet.Cells["E4:H4"].Merge = true;
                worksheet.Cells["E4,H4"].LoadFromText("Main Asset : " + searchModel.mainasset);
                worksheet.Cells["I4:L4"].Merge = true;
                worksheet.Cells["I4,L4"].LoadFromText("Action : " + searchModel.action);

                worksheet.Cells["A5:D5"].Merge = true;
                worksheet.Cells["A5,D5"].LoadFromText("Order Create From : " + searchModel.dateFrom);
                worksheet.Cells["E5:H5"].Merge = true;
                worksheet.Cells["E5,H5"].LoadFromText("Order Create To : " + searchModel.dateTo);

                rangeReportDetail = worksheet.SelectedRange[2, 1, 7, 9]; //[2, 1, 4, 4];
                                                                         //rangeReportDetail = worksheet.SelectedRange[]
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 12;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }

        private byte[] GenerateequipExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5, SubmitFOAEquipmentReportQuery searchModel)
        {
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 16;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText("Report Name  : EQUIPMENT");

                worksheet.Cells["A3:D3"].Merge = true;
                worksheet.Cells["A3,D3"].LoadFromText("Access No : " + searchModel.internetNo);
                worksheet.Cells["E3:H3"].Merge = true;
                worksheet.Cells["E3,H3"].LoadFromText("Order Number : " + searchModel.orderNo);
                worksheet.Cells["I3:L3"].Merge = true;
                worksheet.Cells["I3,L3"].LoadFromText("Status : " + searchModel.status);

                worksheet.Cells["A4:D4"].Merge = true;
                worksheet.Cells["A4,D4"].LoadFromText("Product Name : " + searchModel.productName);
                worksheet.Cells["E4:H4"].Merge = true;
                worksheet.Cells["E4,H4"].LoadFromText("Order Type : " + searchModel.orderType);
                worksheet.Cells["I4:L4"].Merge = true;
                worksheet.Cells["I4,L4"].LoadFromText("Company Code : " + searchModel.companyCode);

                worksheet.Cells["A5:D5"].Merge = true;
                worksheet.Cells["A5,D5"].LoadFromText("Service Name : " + searchModel.serviceName);
                worksheet.Cells["E5:H5"].Merge = true;
                worksheet.Cells["E5,H5"].LoadFromText("Subcontractor Code : " + searchModel.subcontractorCode);
                worksheet.Cells["I5:L5"].Merge = true;
                worksheet.Cells["I5,L5"].LoadFromText("Plant : " + searchModel.plant);

                worksheet.Cells["A6:D6"].Merge = true;
                worksheet.Cells["A6,D6"].LoadFromText("Material Code : " + searchModel.materialCode);
                worksheet.Cells["E6:H6"].Merge = true;
                worksheet.Cells["E6,H6"].LoadFromText("Storage Location : " + searchModel.storLocation);

                worksheet.Cells["A7:D7"].Merge = true;
                worksheet.Cells["A7,D7"].LoadFromText("Order Create From : " + searchModel.dateFrom);
                worksheet.Cells["E7:H7"].Merge = true;
                worksheet.Cells["E7,H7"].LoadFromText("Order Create To : " + searchModel.dateTo);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 7, 9]; //[2, 1, 4, 4];
                                                                         //rangeReportDetail = worksheet.SelectedRange[]
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 8;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }

        #endregion

        #region ResendByFile 20200803


        public ActionResult resendbyfile_Save(HttpPostedFileBase SubmitFOAresedbyfile)    //HttpPostedFileBase
        {
            if (SubmitFOAresedbyfile != null)
            {
                try
                {
                    // foreach (var file in SubmitFOAresedbyfile)
                    // {
                    if (Path.GetExtension(SubmitFOAresedbyfile.FileName).ToLower() == ".csv")
                    {
                        // Read bytes from http input stream
                        BinaryReader b = new BinaryReader(SubmitFOAresedbyfile.InputStream);
                        byte[] binData = b.ReadBytes(SubmitFOAresedbyfile.ContentLength);
                        //LastMileFileModel.csv = System.Text.Encoding.UTF8.GetString(binData);
                        SubmitFOAByFileModel.csv = System.Text.Encoding.Default.GetString(binData);
                    }
                    else if (Path.GetExtension(SubmitFOAresedbyfile.FileName).ToLower() == ".xls" || Path.GetExtension(SubmitFOAresedbyfile.FileName).ToLower() == ".xlsx")
                    {
                        //=------------Read Data to DataTable
                        GetDataTableFromExcel(SubmitFOAresedbyfile);
                        //=------------Read Data to DataTable
                    }

                    else
                    {
                        var modelResponse = new { status = false, message = "Please upload .csv .xls .xlxs file extension", fileName = SubmitFOAresedbyfile.FileName };
                        return Json(modelResponse, "text/plain");
                    }
                    // }
                }
                catch (Exception e)
                {
                    var modelResponse = new { status = false, message = e.GetErrorMessage(), fileName = "" };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }


        public ActionResult resendbyfile_Remove(string[] SubmitFOAresedbyfile)
        {
            if (SubmitFOAresedbyfile != null)
            {
                try
                {
                    SubmitFOAByFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }


        private void GetDataTableFromExcel(HttpPostedFileBase File)
        {
            DataTable tbl = new DataTable();
            TempDataTableResendOrder.dt = tbl;
            try
            {
                //   FileInfo File = new FileInfo(path);
                using (var pck = new ExcelPackage(File.InputStream))
                {
                    var workbook = pck.Workbook;
                    var ws = pck.Workbook.Worksheets.First();

                    bool hasHeader = true;
                    TempDataTableResendOrder.dt.Columns.Add("ACCESS_NO");
                    TempDataTableResendOrder.dt.Columns.Add("ORDER_NO");
                    TempDataTableResendOrder.dt.Columns.Add("SUBCONTRACTOR_CODE");
                    TempDataTableResendOrder.dt.Columns.Add("SUBCONTRACTOR_NAME");
                    TempDataTableResendOrder.dt.Columns.Add("PRODUCT_NAME");
                    TempDataTableResendOrder.dt.Columns.Add("ORDER_TYPE");
                    TempDataTableResendOrder.dt.Columns.Add("SUBMIT_DATE");
                    TempDataTableResendOrder.dt.Columns.Add("SERIAL_NUMBER");
                    TempDataTableResendOrder.dt.Columns.Add("MATERIAL_CODE");
                    TempDataTableResendOrder.dt.Columns.Add("COMPANY_CODE");
                    TempDataTableResendOrder.dt.Columns.Add("PLANT");
                    TempDataTableResendOrder.dt.Columns.Add("STORAGE_LOCATION");
                    TempDataTableResendOrder.dt.Columns.Add("SN_PATTERN");
                    TempDataTableResendOrder.dt.Columns.Add("MOVEMENT_TYPE");
                    TempDataTableResendOrder.dt.Columns.Add("ERROR_CODE");
                    TempDataTableResendOrder.dt.Columns.Add("ERROR_MESSAGE");
                    TempDataTableResendOrder.dt.Columns.Add("STATUS");
                    TempDataTableResendOrder.dt.Columns.Add("MAIN_ASSET");
                    TempDataTableResendOrder.dt.Columns.Add("MATERIAL_DOC");
                    TempDataTableResendOrder.dt.Columns.Add("DOC_YEAR");
                    TempDataTableResendOrder.dt.Columns.Add("REMARK");
                    TempDataTableResendOrder.dt.Columns.Add("CHANGE_STATUS");
                    TempDataTableResendOrder.dt.Columns.Add("PRODUCT_OWNER");
                    TempDataTableResendOrder.dt.Columns.Add("TRANS_ID");


                    var startRow = hasHeader ? 9 : 1;
                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        var row = TempDataTableResendOrder.dt.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                        TempDataTableResendOrder.dt.Rows.Add(row);
                    }

                    // return TempDataTableWriteOff.dt;

                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                // return TempDataTableWriteOff.dt;
            }
        }


        private List<ResendOrderFileList> ConvertDataTableToList()
        {
            List<ResendOrderFileList> Datafile = new List<ResendOrderFileList>();
            // string p_UserName = base.CurrentUser.UserName.ToString();
            try
            {

                foreach (DataRow row in TempDataTableResendOrder.dt.Rows)
                {
                    try
                    {

                        var model = new ResendOrderFileList
                        {


                            ACCESS_NO = row.ItemArray[0].ToSafeString(),

                            ORDER_NO = row.ItemArray[1].ToSafeString(),
                            SUBCONTRACTOR_CODE = row.ItemArray[2].ToSafeString(),
                            SUBCONTRACTOR_NAME = row.ItemArray[3].ToSafeString(),
                            PRODUCT_NAME = row.ItemArray[4].ToSafeString(),
                            ORDER_TYPE = row.ItemArray[5].ToSafeString(),
                            SUBMIT_DATE = row.ItemArray[6].ToSafeString(),
                            SERIAL_NUMBER = row.ItemArray[7].ToSafeString(),
                            MATERIAL_CODE = row.ItemArray[8].ToSafeString(),
                            COMPANY_CODE = row.ItemArray[9].ToSafeString(),
                            PLANT = row.ItemArray[10].ToSafeString(),
                            STORAGE_LOCATION = row.ItemArray[11].ToSafeString(),
                            SN_PATTERN = row.ItemArray[12].ToSafeString(),
                            MOVEMENT_TYPE = row.ItemArray[13].ToSafeString(),
                            ERROR_CODE = row.ItemArray[14].ToSafeString(),
                            ERROR_MESSAGE = row.ItemArray[15].ToSafeString(),
                            STATUS = row.ItemArray[16].ToSafeString(),
                            MAIN_ASSET = row.ItemArray[17].ToSafeString(),
                            MATERIAL_DOC = row.ItemArray[18].ToSafeString(),
                            DOC_YEAR = row.ItemArray[19].ToSafeString(),
                            REMARK = row.ItemArray[20].ToSafeString(),
                            CHANGE_STATUS = row.ItemArray[21].ToSafeString(),
                            PRODUCT_OWNER = row.ItemArray[22].ToSafeString(),
                            TRANS_ID = row.ItemArray[23].ToSafeString(),

                        };



                        Datafile.Add(model);
                    }
                    catch (Exception ex)
                    {
                        _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                    }

                }
                // Clear data 
                DataTable tbl = new DataTable();
                TempDataTableResendOrder.dt = tbl;
                // Clear
                return Datafile;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                return null;
            }

        }



        private List<ResendOrderFileListIN> ConvertDataTableToListin()
        {
            List<ResendOrderFileListIN> Datafilein = new List<ResendOrderFileListIN>();
            // string p_UserName = base.CurrentUser.UserName.ToString();
            try
            {

                foreach (DataRow row in TempDataTableResendOrder.dt.Rows)
                {
                    try
                    {

                        var model = new ResendOrderFileListIN
                        {


                            ACCESS_NO = row.ItemArray[0].ToSafeString(),

                            ASSET_CODE = row.ItemArray[1].ToSafeString(),
                            SUBCONTRACT_CODE = row.ItemArray[2].ToSafeString(),
                            SUBCONTRACT_NAME = row.ItemArray[3].ToSafeString(),
                            INSTALLATION_COST = row.ItemArray[4].ToSafeString(),
                            ORDER_DATE = row.ItemArray[5].ToSafeString(),
                            MODIFY_DATE = row.ItemArray[6].ToSafeString(),
                            TRAN_STATUS = row.ItemArray[7].ToSafeString(),
                            PRODUCT_OWNER = row.ItemArray[8].ToSafeString(),
                            TRANS_ID = row.ItemArray[9].ToSafeString(),
                        };



                        Datafilein.Add(model);
                    }
                    catch (Exception ex)
                    {
                        _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                    }

                }
                // Clear data 
                DataTable tbl = new DataTable();
                TempDataTableResendOrder.dt = tbl;
                // Clear
                return Datafilein;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error ConvertDataTableToList:" + ex.GetErrorMessage());
                return null;
            }

        }


        public async Task<string> SubmitFOAReportResendByFile([DataSourceRequest] DataSourceRequest request, string fileName, string EmailUser)
        {
            SubmitFOAEquipmentReportQuery model = new SubmitFOAEquipmentReportQuery();
            SubmitFOAReportQuery modelReportQuery = new SubmitFOAReportQuery();

            var listreplace = new List<ReplaceEquipmentModel>();
            List<string> accessNo = new List<string>();
            List<string> Result = new List<string>();
            int count_success = 0, count_err = 0;
            List<ResendOrderFileList> Models = ConvertDataTableToList();
            if (string.IsNullOrEmpty(SubmitFOAByFileModel.csv))
            {
                // return Json("Please upload file", JsonRequestBehavior.AllowGet);
            }
            try
            {
                // Group Multi
                string sum_response_results = "";
                string[] lines = null;
                Task<string> results;
                var lineCSV = SubmitFOAByFileModel.csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lineCSV.Length > 0 && (Models == null || Models.Count == 0))
                {

                    lines = lineCSV;

                    #region get data CSV by line

                    foreach (var item in lines)
                    {
                        var values = item.Split(',', '|');
                        string chkData = values[0].ToSafeString();

                        if (chkData.Length == 10 && values != null)
                        {
                            string ACCESS_NUMBER = string.Empty;
                            string ORDER_NO = string.Empty;

                            string SUBCONTRACTOR_CODE = string.Empty;
                            string SUBCONTRACTOR_NAME = string.Empty;
                            string PRODUCT_NAME = string.Empty;
                            string ORDER_TYPE = string.Empty;
                            string SUBMIT_DATE = string.Empty;
                            string SERIAL_NUMBER = string.Empty;
                            string MATERIAL_CODE = string.Empty;
                            string COMPANY_CODE = string.Empty;
                            string PLANT = string.Empty;
                            string STORAGE_LOCATION = string.Empty;
                            string SN_PATTERN = string.Empty;
                            string MOVEMENT_TYPE = string.Empty;
                            string ERROR_CODE = string.Empty;
                            string ERROR_MESSAGE = string.Empty;
                            string STATUS = string.Empty;
                            string MAIN_ASSET = string.Empty;
                            string MAT_DOC = string.Empty;
                            string DOC_YEAR = string.Empty;
                            string REMARK = string.Empty;
                            string CH_STATUS = string.Empty;
                            string PRODUCT_OWNER = string.Empty;
                            string TRANS_ID = string.Empty;
                            for (int i = 0; i <= values.Count() - 1; i++)
                            {
                                if (i == 0)
                                {
                                    ACCESS_NUMBER = values[i];
                                    //   accNbr = "8972268010";


                                }
                                if (i == 1)
                                {
                                    ORDER_NO = values[i];
                                }
                                if (i == 2)
                                {
                                    SUBCONTRACTOR_CODE = values[i];
                                }
                                if (i == 3)
                                {
                                    SUBCONTRACTOR_NAME = values[i];
                                }
                                if (i == 4)
                                {
                                    PRODUCT_NAME = values[i];
                                }
                                if (i == 5)
                                {
                                    ORDER_TYPE = values[i];
                                }
                                if (i == 6)
                                {
                                    SUBMIT_DATE = values[i];
                                }
                                if (i == 7)
                                {
                                    SERIAL_NUMBER = values[i];
                                }
                                if (i == 8)
                                {
                                    MATERIAL_CODE = values[i];
                                }
                                if (i == 9)
                                {
                                    COMPANY_CODE = values[i];
                                }
                                if (i == 10)
                                {
                                    PLANT = values[i];
                                }
                                if (i == 11)
                                {
                                    STORAGE_LOCATION = values[i];
                                }
                                if (i == 12)
                                {
                                    SN_PATTERN = values[i];
                                }
                                if (i == 13)
                                {
                                    MOVEMENT_TYPE = values[i];
                                }
                                if (i == 14)
                                {
                                    ERROR_CODE = values[i];
                                }
                                if (i == 15)
                                {
                                    ERROR_MESSAGE = values[i];
                                }
                                if (i == 16)
                                {
                                    STATUS = values[i];
                                }
                                if (i == 17)
                                {
                                    MAIN_ASSET = values[i];
                                }
                                if (i == 18)
                                {
                                    MAT_DOC = values[i];
                                }
                                if (i == 19)
                                {
                                    DOC_YEAR = values[i];
                                }
                                if (i == 20)
                                {
                                    REMARK = values[i];
                                }
                                if (i == 21)
                                {
                                    CH_STATUS = values[i];
                                }
                                if (i == 22)
                                {
                                    PRODUCT_OWNER = values[i];
                                }
                                if (i == 23)
                                {
                                    TRANS_ID = values[i];
                                }

                            }

                            if (STATUS.ToUpper().Trim() == "ERROR" || STATUS.ToUpper().Trim() == "COMPLETE")
                            {
                                ReplaceEquipmentModel replaceModel = new ReplaceEquipmentModel();
                                replaceModel.materialCode = MATERIAL_CODE;
                                replaceModel.subcontractorCode = SUBCONTRACTOR_CODE;
                                replaceModel.companyCode = COMPANY_CODE;
                                replaceModel.postDate = "";
                                replaceModel.storLocation = STORAGE_LOCATION;
                                replaceModel.plant = PLANT;
                                replaceModel.serial_number = SERIAL_NUMBER;
                                replaceModel.access_no = ACCESS_NUMBER;
                                replaceModel.order_no = ORDER_NO;
                                replaceModel.ch_status = STATUS;
                                replaceModel.product_owner = PRODUCT_OWNER;
                                replaceModel.Tarns_id = TRANS_ID;
                                listreplace.Add(replaceModel);
                            }
                        }

                    }

                    #endregion
                    int a = 0, k = 0;
                    #region Resend
                    foreach (var re in listreplace.Where(w => w.ch_status.ToUpper().Trim() != "COMPLETE").GroupBy(g => new { g.access_no, g.order_no }))
                    {
                        a++;
                        model.orderNo = re.FirstOrDefault().order_no;
                        model.internetNo = re.FirstOrDefault().access_no;
                        model.status = "ALL";
                        model.productName = "ALL";
                        model.orderType = "ALL";
                        model.companyCode = "ALL";
                        model.serviceName = "";
                        model.subcontractorCode = "ALL";
                        model.errormessage = "ALL";
                        model.plant = "ALL";
                        model.materialCode = "ALL";
                        model.storLocation = "ALL";
                        model.dateFrom = "";
                        model.dateTo = "";
                        model.productOwner = "ALL";
                        model.page_index = 1;
                        model.page_size = 1000;

                        modelReportQuery.accessNo = re.FirstOrDefault().access_no;
                        modelReportQuery.orderNo = re.FirstOrDefault().order_no;
                        modelReportQuery.status = "ALL";
                        modelReportQuery.productName = "ALL";
                        modelReportQuery.orderType = "ALL";
                        modelReportQuery.companyCode = "ALL";
                        modelReportQuery.serviceName = "ALL";
                        modelReportQuery.subcontractorCode = "ALL";
                        modelReportQuery.plant = "ALL";
                        modelReportQuery.materialCode = "ALL";
                        modelReportQuery.storLocation = "ALL";
                        modelReportQuery.dateFrom = "";
                        modelReportQuery.dateTo = "";
                        modelReportQuery.productOwner = "ALL";

                        #region ResendToSelectSubmitFOA

                        var resultEquip = GetSubmitFOAEquipment(model);
                        insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"1. GetSubmitFOAEquipment", re.FirstOrDefault().access_no.ToSafeString());

                        List<SubmitFOAEquipment> data = resultEquip.cur;
                        insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", re.FirstOrDefault().access_no.ToSafeString());
                        insertErrorLog(model, "3.CH_STATUS=" + re.FirstOrDefault().ch_status, re.FirstOrDefault().access_no.ToSafeString());

                        try
                        {
                            if (data.Count > 0)
                            {
                                var res_result = await ResendToSelectSubmitFOA(data, listreplace);
                                // accessNo.Add(re.FirstOrDefault().access_no);



                                if (res_result.Data.ToString() == "")
                                {
                                    Result.Add("COMPLETE");
                                    count_success++;
                                }
                                else if (res_result.Data.ToString() == "Success 1 Order. ")
                                {
                                    Result.Add(res_result.Data.ToString());
                                    count_success++;
                                }
                                else
                                {
                                    Result.Add(res_result.Data.ToString());
                                    count_err++;
                                }
                                insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count > 0", re.FirstOrDefault().access_no.ToSafeString());
                            }
                            else
                            {
                                insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count < 0", re.FirstOrDefault().access_no.ToSafeString());
                            }
                        }

                        catch (Exception Ex)
                        {
                            _Logger.Info(Ex.Message.ToSafeString() + "Error Call WEBSERVICE");
                            insertErrorLog(model, Ex.Message.ToString(), re.FirstOrDefault().access_no.ToSafeString());
                        }
                        #endregion
                    }
                    #endregion

                    #region Complete
                    foreach (var re in listreplace.Where(w => w.ch_status.ToUpper().Trim() == "COMPLETE"))
                    {
                        k++;
                        model.orderNo = re.order_no;
                        model.internetNo = re.access_no;
                        model.status = "ALL";
                        model.productName = "ALL";
                        model.orderType = "ALL";
                        model.companyCode = "ALL";
                        model.serviceName = "";
                        model.subcontractorCode = "ALL";
                        model.errormessage = "ALL";
                        model.plant = "ALL";
                        model.materialCode = "ALL";
                        model.storLocation = "ALL";
                        model.dateFrom = "";
                        model.dateTo = "";
                        model.productOwner = "ALL";
                        model.page_index = 1;
                        model.page_size = 1000;

                        modelReportQuery.accessNo = re.access_no;
                        modelReportQuery.orderNo = re.order_no;
                        modelReportQuery.status = "ALL";
                        modelReportQuery.productName = "ALL";
                        modelReportQuery.orderType = "ALL";
                        modelReportQuery.companyCode = "ALL";
                        modelReportQuery.serviceName = "ALL";
                        modelReportQuery.subcontractorCode = "ALL";
                        modelReportQuery.plant = "ALL";
                        modelReportQuery.materialCode = "ALL";
                        modelReportQuery.storLocation = "ALL";
                        modelReportQuery.dateFrom = "";
                        modelReportQuery.dateTo = "";
                        modelReportQuery.productOwner = "ALL";

                        #region ResendToSelectSubmitFOA

                        var resultEquip = GetSubmitFOAEquipment(model);
                        insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"1. GetSubmitFOAEquipment", re.access_no.ToSafeString());

                        List<SubmitFOAEquipment> data = resultEquip.cur;
                        insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", re.access_no.ToSafeString());
                        insertErrorLog(model, "3.CH_STATUS=" + re.ch_status, re.access_no.ToSafeString());

                        try
                        {
                            accessNo.Add(re.access_no);
                            Result.Add("COMPLETE");
                            count_success++;
                            var addresult = new UpdateFOAResendStatusCommand
                            {
                                P_TRANS_ID = data.FirstOrDefault().TRANS_ID.ToSafeString(),
                                P_ORDER_NO = re.order_no.ToSafeString(),

                                P_SERIAL_NO = re.serial_number.ToSafeString(),
                                P_INTERNET_NO = re.access_no.ToSafeString(),
                                P_SUBNUMBER = "COMPLETE",
                                P_ASSET_CODE = re.access_no.ToSafeString(),
                                P_MATERIAL_DOC = string.Empty,
                                P_DOC_YEAR = string.Empty,

                                P_COM_CODE = re.companyCode.ToSafeString(),
                                P_ERR_CODE = "0000",
                                P_ERR_MSG = "",
                                P_REMARK = string.Empty
                            };
                            _updateFOAStatusTypeCommand.Handle(addresult);
                            insertErrorLog<UpdateFOAResendStatusCommand>(addresult, $"4. CH_STATUS Complete UpdateFOAResendStatusCommand", re.access_no.ToSafeString());
                        }
                        catch (Exception Ex)
                        {
                            _Logger.Info(Ex.Message.ToSafeString() + re.access_no.ToSafeString());
                            insertErrorLog(model, Ex.Message.ToString(), re.access_no.ToSafeString());
                        }

                        #endregion
                    }
                    #endregion

                    TempData["accessNo"] = accessNo;
                    TempData["Result"] = Result;
                    sum_response_results = "Access Number : " + accessNo.Count + "|" + "Success : " + count_success + "|Error : " + count_err + "";
                }
                else if (Models.Count > 0 || Models != null)
                {
                    var sum_response_result = IMPORTBYEXCEL(Models, fileName, EmailUser);
                    sum_response_results += sum_response_result.Result.ToSafeString();
                }
                else
                {
                    return "Blank .csv .xls .xlsx file";
                }

                // send Email
                results = sendEmailImportStatus(fileName, EmailUser, TempData["accessNo"] as List<string>, TempData["Result"] as List<string>, sum_response_results);
                return results.Result + "|" + sum_response_results;
            }
            catch (Exception Ex)
            {
                _Logger.Info(Ex.Message.ToSafeString() + "ERROR IMPORT FILE");
                return Ex.Message.ToSafeString() + "ERROR IMPORT FILE";
            }
        }



        public async Task<string> SubmitFOAReportResendByFileIN([DataSourceRequest] DataSourceRequest request, string fileName, string EmailUser)
        {
            SubmitFOAInstallationReportQuery model = new SubmitFOAInstallationReportQuery();
            SubmitFOAReportQuery modelReportQuery = new SubmitFOAReportQuery();

            var listreplace = new List<ReplaceEquipmentModel>();
            List<string> accessNo = new List<string>();
            List<string> Result = new List<string>();
            int count_success = 0, count_err = 0;
            List<ResendOrderFileListIN> Models = ConvertDataTableToListin();
            if (string.IsNullOrEmpty(SubmitFOAByFileModel.csv))
            {
                // return Json("Please upload file", JsonRequestBehavior.AllowGet);
            }
            try
            {
                // Group Multi
                string sum_response_results = "";
                string[] lines = null;
                Task<string> results;
                var lineCSV = SubmitFOAByFileModel.csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lineCSV.Length > 0 && (Models == null || Models.Count == 0))
                {

                    lines = lineCSV;

                    #region get data CSV by line
                    List<ReplaceEquipmentModel> listreplaces = new List<ReplaceEquipmentModel>();

                    foreach (var item in lines)
                    {


                        var values = item.Split(',', '|');
                        string chkData = values[0].ToSafeString();

                        if (chkData.Length == 10 && values != null)
                        {

                            string ACCESS_NUMBER = string.Empty;
                            string ASSET_CODE = string.Empty;

                            string SUB_NUMBER = string.Empty;
                            string PRODUCT_NAME = string.Empty;
                            string INSTALLATION_COST = string.Empty;
                            string ORDER_DATE = string.Empty;
                            string MODIFY_DATE = string.Empty;
                            string TRAN_STATUS = string.Empty;
                            string PRODUCT_OWNER = string.Empty;

                            string TRANS_ID = string.Empty;
                            for (int i = 0; i <= values.Count() - 1; i++)
                            {
                                if (i == 0)
                                {
                                    ACCESS_NUMBER = values[i];
                                    //   accNbr = "8972268010";


                                }
                                if (i == 1)
                                {
                                    ASSET_CODE = values[i];
                                }
                                if (i == 2)
                                {
                                    SUB_NUMBER = values[i];
                                }
                                if (i == 3)
                                {
                                    PRODUCT_NAME = values[i];
                                }
                                if (i == 4)
                                {
                                    INSTALLATION_COST = values[i];
                                }
                                if (i == 5)
                                {
                                    ORDER_DATE = values[i];
                                }
                                if (i == 6)
                                {
                                    MODIFY_DATE = values[i];
                                }
                                if (i == 7)
                                {
                                    TRAN_STATUS = values[i];
                                }
                                if (i == 8)
                                {
                                    PRODUCT_OWNER = values[i];
                                }

                                if (i == 9)
                                {
                                    TRANS_ID = values[i];
                                }
                            }




                            if (TRAN_STATUS.ToUpper().Trim() == "ERROR" || TRAN_STATUS.ToUpper().Trim() == "COMPLETE")
                            {
                                var replaceModel = new ReplaceEquipmentModel
                                {
                                    postDate = "",
                                    access_no = ACCESS_NUMBER,
                                    product_owner = PRODUCT_OWNER,
                                    Tarns_id = TRANS_ID,
                                    ch_status = TRAN_STATUS
                                };

                                listreplaces.Add(replaceModel);
                            }


                            //    if (TRAN_STATUS.ToUpper().Trim() == "ERROR" || TRAN_STATUS.ToUpper().Trim() == "COMPLETE")
                            //    {
                            //        ReplaceEquipmentModel replaceModel = new ReplaceEquipmentModel();
                            //        replaceModel.postDate = "";
                            //        replaceModel.access_no = ACCESS_NUMBER;
                            //        replaceModel.product_owner = PRODUCT_OWNER;
                            //        replaceModel.Tarns_id = TRANS_ID;
                            //        replaceModel.ch_status = TRAN_STATUS;
                            //        listreplace.Add(replaceModel);
                            //    }
                        }

                    }

                    #endregion
                    int a = 0, k = 0;
                    #region Resend
                    foreach (var re in listreplace.Where(w => w.ch_status.ToUpper().Trim() != "COMPLETE").GroupBy(g => new { g.access_no, g.order_no }))
                    {
                        a++;
                        model.orderNo = re.FirstOrDefault().order_no;
                        model.internetNo = re.FirstOrDefault().access_no;
                        model.status = "ALL";
                        model.productName = "ALL";
                        model.serviceName = "";
                        model.dateFrom = "";
                        model.dateTo = "";
                        model.productOwner = "ALL";


                        modelReportQuery.accessNo = re.FirstOrDefault().access_no;
                        modelReportQuery.orderNo = "ALL";
                        modelReportQuery.status = "ALL";
                        modelReportQuery.productName = "ALL";
                        modelReportQuery.orderType = "ALL";
                        modelReportQuery.companyCode = "ALL";
                        modelReportQuery.serviceName = "ALL";
                        modelReportQuery.subcontractorCode = "ALL";
                        modelReportQuery.plant = "ALL";
                        modelReportQuery.materialCode = "ALL";
                        modelReportQuery.storLocation = "ALL";
                        modelReportQuery.dateFrom = "";
                        modelReportQuery.dateTo = "";
                        modelReportQuery.productOwner = "ALL";
                        #region ResendToSelectSubmitFOA

                        var resultEquip = GetSubmitFOAInstallation(model);
                        //insertErrorLog<GetSubmitFOAInstallation>(model, $"1. GetSubmitFOAEquipment", re.FirstOrDefault().access_no.ToSafeString());
                        List<SubmitFOAInstallation> data = new List<SubmitFOAInstallation>();
                        if (listreplaces != null && listreplaces.Any())
                        {
                            data = listreplaces.Select(r => new SubmitFOAInstallation
                            {
                                ACCESS_NUMBER = r.access_no,
                                ORDER_NO = r.order_no,
                                PRODUCT_OWNER = r.product_owner,
                                TRANS_ID = r.Tarns_id
                            }).ToList();
                        }
                        else
                        {
                            // กรณีไม่มีข้อมูล อาจใส่ default หรือปล่อยว่างไว้ก็ได้
                        }

                        //List<SubmitFOAInstallation> data = resultEquip.cus;
                        // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", re.FirstOrDefault().access_no.ToSafeString());
                        insertErrorLog(model, "3.CH_STATUS=" + re.FirstOrDefault().ch_status, re.FirstOrDefault().access_no.ToSafeString());

                        try
                        {
                            if (data.Count > 0)
                            {
                                var res_result = await ResendToSelectSubmitFOAIN(data, listreplaces);
                                accessNo.Add(re?.FirstOrDefault()?.access_no ?? "-");

                                if (res_result.Data.ToString() == "")
                                {
                                    Result.Add("COMPLETE");
                                    count_success++;

                                }
                                else if (res_result.Data.ToString().Trim() == "Success 1 Order.")
                                {
                                    Result.Add(res_result.Data.ToString());
                                    count_success++;

                                }
                                else
                                {
                                    Result.Add(res_result.Data.ToString());
                                    count_err++;

                                }
                                //insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count > 0", re.FirstOrDefault().access_no.ToSafeString());
                            }
                            else
                            {
                                // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count < 0", re.FirstOrDefault().access_no.ToSafeString());
                            }
                        }

                        catch (Exception Ex)
                        {
                            _Logger.Info(Ex.Message.ToSafeString() + "Error Call WEBSERVICE");
                            insertErrorLog(model, Ex.Message.ToString(), re.FirstOrDefault().access_no.ToSafeString());
                        }
                        #endregion
                    }
                    #endregion

                    #region Complete
                    foreach (var re in listreplace.Where(w => w.ch_status.ToUpper().Trim() == "COMPLETE"))
                    {
                        k++;
                        model.orderNo = re.order_no;
                        model.internetNo = re.access_no;
                        model.status = "ALL";
                        model.productName = "ALL";
                        model.serviceName = "";
                        model.dateFrom = "";
                        model.dateTo = "";
                        model.productOwner = "ALL";


                        modelReportQuery.accessNo = re.access_no;
                        modelReportQuery.orderNo = re.order_no;
                        modelReportQuery.status = "ALL";
                        modelReportQuery.productName = "ALL";
                        modelReportQuery.orderType = "ALL";
                        modelReportQuery.companyCode = "ALL";
                        modelReportQuery.serviceName = "ALL";
                        modelReportQuery.subcontractorCode = "ALL";
                        modelReportQuery.plant = "ALL";
                        modelReportQuery.materialCode = "ALL";
                        modelReportQuery.storLocation = "ALL";
                        modelReportQuery.dateFrom = "";
                        modelReportQuery.dateTo = "";
                        modelReportQuery.productOwner = "ALL";

                        #region ResendToSelectSubmitFOA

                        var resultEquip = GetSubmitFOAInstallation(model);
                        //  insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"1. GetSubmitFOAEquipment", re.access_no.ToSafeString());
                        List<SubmitFOAInstallation> data = new List<SubmitFOAInstallation>();
                        // List<SubmitFOAEquipment> data = resultEquip.cur;
                        //insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", re.access_no.ToSafeString());
                        insertErrorLog(model, "3.CH_STATUS=" + re.ch_status, re.access_no.ToSafeString());

                        try
                        {
                            accessNo.Add(re.access_no);
                            Result.Add("COMPLETE");
                            count_success++;
                            var addresult = new UpdateFOAResendStatusCommand
                            {
                                P_ORDER_NO = re.order_no.ToSafeString(),

                                P_SERIAL_NO = re.serial_number.ToSafeString(),
                                P_INTERNET_NO = re.access_no.ToSafeString(),
                                P_SUBNUMBER = "COMPLETE",
                                P_ASSET_CODE = re.access_no.ToSafeString(),
                                P_MATERIAL_DOC = string.Empty,
                                P_DOC_YEAR = string.Empty,

                                P_COM_CODE = re.companyCode.ToSafeString(),
                                P_ERR_CODE = "0000",
                                P_ERR_MSG = "",
                                P_REMARK = string.Empty
                            };
                            _updateFOAStatusTypeCommand.Handle(addresult);
                            insertErrorLog<UpdateFOAResendStatusCommand>(addresult, $"4. CH_STATUS Complete UpdateFOAResendStatusCommand", re.access_no.ToSafeString());
                        }
                        catch (Exception Ex)
                        {
                            _Logger.Info(Ex.Message.ToSafeString() + re.access_no.ToSafeString());
                            insertErrorLog(model, Ex.Message.ToString(), re.access_no.ToSafeString());
                        }

                        #endregion
                    }
                    #endregion

                    TempData["accessNo"] = accessNo;
                    TempData["Result"] = Result;
                    sum_response_results = "Access Number : " + accessNo.Count + "|" + "Success : " + count_success + "|Error : " + count_err + "";
                }
                else if (Models.Count > 0 || Models != null)
                {
                    var sum_response_result = IMPORTBYEXCELIN(Models, fileName, EmailUser);
                    sum_response_results += sum_response_result.Result.ToSafeString();
                }
                else
                {
                    return "Blank .csv .xls .xlsx file";
                }

                // send Email
                results = sendEmailImportStatus(fileName, EmailUser, TempData["accessNo"] as List<string>, TempData["Result"] as List<string>, sum_response_results);
                return results.Result + "|" + sum_response_results;
            }
            catch (Exception Ex)
            {
                _Logger.Info(Ex.Message.ToSafeString() + "ERROR IMPORT FILE");
                return Ex.Message.ToSafeString() + "ERROR IMPORT FILE";
            }
        }



        public async Task<string> IMPORTBYEXCEL(List<ResendOrderFileList> dataFile, string fileName, string Emailuser)
        {
            SubmitFOAEquipmentReportQuery model = new SubmitFOAEquipmentReportQuery();
            SubmitFOAReportQuery modelReportQuery = new SubmitFOAReportQuery();

            var listreplace = new List<ReplaceEquipmentModel>();
            List<string> accessNo = new List<string>();
            List<string> Result = new List<string>();
            int count_success = 0, count_err = 0;

            // List<ResendOrderFileList> Models = ConvertDataTableToList();
            string ACCESS_NUMBER = string.Empty;
            string ORDER_NO = string.Empty;
            string code = string.Empty;
            string MSG = string.Empty;
            string SUBCONTRACTOR_CODE = string.Empty;
            string SUBCONTRACTOR_NAME = string.Empty;
            string PRODUCT_NAME = string.Empty;
            string ORDER_TYPE = string.Empty;
            string SUBMIT_DATE = string.Empty;
            string SERIAL_NUMBER = string.Empty;
            string MATERIAL_CODE = string.Empty;
            string COMPANY_CODE = string.Empty;
            string PLANT = string.Empty;
            string STORAGE_LOCATION = string.Empty;
            string SN_PATTERN = string.Empty;
            string MOVEMENT_TYPE = string.Empty;
            string ERROR_CODE = string.Empty;
            string ERROR_MESSAGE = string.Empty;
            string STATUS = string.Empty;
            string MAIN_ASSET = string.Empty;
            string MAT_DOC = string.Empty;
            string DOC_YEAR = string.Empty;
            string REMARK = string.Empty;
            string CH_STATUS = string.Empty;
            string PRODUCT_OWNER = string.Empty;
            string TRANS_ID = string.Empty;

            List<ReplaceEquipmentModel> listreplaces = new List<ReplaceEquipmentModel>();


            //foreach (var re in dataFile.Where(w => w.CHANGE_STATUS.ToUpper().Trim() != "COMPLETE" && w.STATUS.ToUpper() == "ERROR" 
            //    || w.CHANGE_STATUS.ToUpper().Trim() != "COMPLETE" && w.STATUS.ToUpper() == "PENDING"))
            //    {
            //    ReplaceEquipmentModel replaceModel = new ReplaceEquipmentModel();
            //    replaceModel.materialCode = re.MATERIAL_CODE;
            //    replaceModel.subcontractorCode = re.SUBCONTRACTOR_CODE;
            //    replaceModel.companyCode = re.COMPANY_CODE;
            //    replaceModel.postDate = "";
            //    replaceModel.storLocation = re.STORAGE_LOCATION;
            //    replaceModel.plant = re.PLANT;
            //    replaceModel.serial_number = re.SERIAL_NUMBER;
            //    replaceModel.access_no = re.ACCESS_NO;
            //    replaceModel.order_no = re.ORDER_NO;
            //    replaceModel.ch_status = re.CHANGE_STATUS;
            //    replaceModel.product_owner = re.PRODUCT_OWNER;
            //    replaceModel.Tarns_id = re.TRANS_ID;
            //    listreplace.Add(replaceModel);

            //}


            foreach (var re in dataFile.Where(w =>
                w.CHANGE_STATUS.ToUpper().Trim() != "COMPLETE" &&
                (w.STATUS.ToUpper() == "ERROR" || w.STATUS.ToUpper() == "PENDING")))
            {
                ReplaceEquipmentModel replaceModel = new ReplaceEquipmentModel
                {
                    materialCode = re.MATERIAL_CODE,
                    subcontractorCode = re.SUBCONTRACTOR_CODE,
                    companyCode = re.COMPANY_CODE,
                    postDate = "",
                    storLocation = re.STORAGE_LOCATION,
                    plant = re.PLANT,
                    serial_number = re.SERIAL_NUMBER,
                    access_no = re.ACCESS_NO,
                    order_no = re.ORDER_NO,
                    ch_status = re.STATUS,
                    product_owner = re.PRODUCT_OWNER,
                    Tarns_id = re.TRANS_ID

                };

                listreplaces.Add(replaceModel);
            }





            var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();

            if (resultFixAssConfig.DISPLAY_VAL == "Y")
            {

                #region Resend
                foreach (var re in dataFile.Where(w => w.CHANGE_STATUS.ToUpper().Trim() != "COMPLETE").GroupBy(g => new { g.ACCESS_NO, g.ORDER_NO }))
                {
                    CH_STATUS = re.FirstOrDefault().CHANGE_STATUS;
                    ORDER_NO = re.FirstOrDefault().ORDER_NO;
                    SERIAL_NUMBER = re.FirstOrDefault().SERIAL_NUMBER;
                    ACCESS_NUMBER = re.FirstOrDefault().ACCESS_NO;
                    MAIN_ASSET = re.FirstOrDefault().MAIN_ASSET;
                    MAT_DOC = re.FirstOrDefault().MATERIAL_DOC;
                    DOC_YEAR = re.FirstOrDefault().DOC_YEAR;
                    COMPANY_CODE = re.FirstOrDefault().COMPANY_CODE;
                    REMARK = re.FirstOrDefault().REMARK;
                    PRODUCT_OWNER = re.FirstOrDefault().PRODUCT_OWNER;


                    model.orderNo = re.FirstOrDefault().ORDER_NO;
                    model.internetNo = re.FirstOrDefault().ACCESS_NO;
                    model.status = "ALL";
                    model.productName = "ALL";
                    model.orderType = "ALL";
                    model.companyCode = "ALL";
                    model.serviceName = "";
                    model.subcontractorCode = "ALL";
                    model.errormessage = "ALL";
                    model.plant = "ALL";
                    model.materialCode = "ALL";
                    model.storLocation = "ALL";
                    model.dateFrom = "";
                    model.dateTo = "";
                    model.productOwner = "ALL";
                    model.page_index = 1;
                    model.page_size = 1000;


                    modelReportQuery.accessNo = re.FirstOrDefault().ACCESS_NO;
                    modelReportQuery.orderNo = re.FirstOrDefault().ORDER_NO;
                    modelReportQuery.status = "ALL";
                    modelReportQuery.productName = "ALL";
                    modelReportQuery.orderType = "ALL";
                    modelReportQuery.companyCode = "ALL";
                    modelReportQuery.serviceName = "ALL";
                    modelReportQuery.subcontractorCode = "ALL";
                    modelReportQuery.plant = "ALL";
                    modelReportQuery.materialCode = "ALL";
                    modelReportQuery.storLocation = "ALL";
                    modelReportQuery.dateFrom = "";
                    modelReportQuery.dateTo = "";
                    modelReportQuery.productOwner = re.FirstOrDefault().PRODUCT_OWNER;


                    var resultEquip = GetSubmitFOAEquipment(model);
                    insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"1. GetSubmitFOAEquipment", ACCESS_NUMBER.ToString());

                    List<SubmitFOAEquipment> data = new List<SubmitFOAEquipment>();



                    if (listreplaces != null && listreplaces.Any())
                    {

                        data = listreplaces.Select(r => new SubmitFOAEquipment
                        {
                            MATERIAL_CODE = r.materialCode,
                            COMPANY_CODE = r.companyCode,
                            STORAGE_LOCATION = r.storLocation,
                            PLANT = r.plant,
                            SN = r.serial_number,
                            ACCESS_NUMBER = r.access_no,
                            ORDER_NO = r.order_no,
                            PRODUCT_OWNER = r.product_owner,
                            TRANS_ID = r.Tarns_id,

                            STATUS = r.ch_status
                        }).ToList();


                    }
                    else
                    {
                        data = resultEquip.cur;
                    }


                    insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", ACCESS_NUMBER.ToString());
                    insertErrorLog(model, "3.CH_STATUS=" + CH_STATUS, ACCESS_NUMBER);

                    try
                    {
                        if (data.Count > 0)
                        {
                            var res_result = await ResendToSelectSubmitFOA(data, listreplaces);
                            insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count > 0", ACCESS_NUMBER.ToString());
                            //data.Clear();



                            if (res_result.Data.ToString() == "")
                            {
                                Result.Add("COMPLETE");

                                count_success++;
                            }
                            else if (res_result.Data.ToString() == "Success 1 Order. ")
                            {
                                Result.Add(res_result.Data.ToString());
                                count_success++;

                            }
                            else
                            {
                                Result.Add(res_result.Data.ToString());
                                count_err++;



                            }
                        }
                        else
                        {
                            // log error
                            insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count < 0 ", ACCESS_NUMBER.ToString());
                        }
                    }
                    catch (Exception Ex)
                    {

                        _Logger.Info(Ex.Message.ToSafeString() + "Error Call WEBSERVICE");
                        insertErrorLog(model, Ex.Message.ToString(), ACCESS_NUMBER);

                    }

                }
                #endregion

                #region Complete
                foreach (var re in dataFile.Where(w => w.CHANGE_STATUS.ToUpper().Trim() == "COMPLETE"))
                {
                    CH_STATUS = re.CHANGE_STATUS;
                    ORDER_NO = re.ORDER_NO;
                    SERIAL_NUMBER = re.SERIAL_NUMBER;
                    ACCESS_NUMBER = re.ACCESS_NO;
                    MAIN_ASSET = re.MAIN_ASSET;
                    MAT_DOC = re.MATERIAL_DOC;
                    DOC_YEAR = re.DOC_YEAR;
                    COMPANY_CODE = re.COMPANY_CODE;
                    REMARK = re.REMARK;


                    model.orderNo = re.ORDER_NO;
                    model.internetNo = re.ACCESS_NO;
                    model.status = "ALL";
                    model.productName = "ALL";
                    model.orderType = "ALL";
                    model.companyCode = "ALL";
                    model.serviceName = "";
                    model.subcontractorCode = "ALL";
                    model.errormessage = "ALL";
                    model.plant = "ALL";
                    model.materialCode = "ALL";
                    model.storLocation = "ALL";
                    model.dateFrom = "";
                    model.dateTo = "";
                    model.page_index = 1;
                    model.page_size = 1000;


                    modelReportQuery.accessNo = re.ACCESS_NO;
                    modelReportQuery.orderNo = re.ORDER_NO;
                    modelReportQuery.status = "ALL";
                    modelReportQuery.productName = "ALL";
                    modelReportQuery.orderType = "ALL";
                    modelReportQuery.companyCode = "ALL";
                    modelReportQuery.serviceName = "ALL";
                    modelReportQuery.subcontractorCode = "ALL";
                    modelReportQuery.plant = "ALL";
                    modelReportQuery.materialCode = "ALL";
                    modelReportQuery.storLocation = "ALL";
                    modelReportQuery.dateFrom = "";
                    modelReportQuery.dateTo = "";

                    insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"1. GetSubmitFOAEquipment", ACCESS_NUMBER.ToString());
                    var resultEquip = GetSubmitFOAEquipment(model);
                    insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", ACCESS_NUMBER.ToString());
                    insertErrorLog(model, "3.CH_STATUS=" + CH_STATUS, ACCESS_NUMBER);

                    try
                    {
                        accessNo.Add(re.ACCESS_NO);
                        Result.Add("COMPLETE");
                        count_success++;
                        var addresult = new UpdateFOAResendStatusCommand
                        {
                            P_TRANS_ID = resultEquip.cur.FirstOrDefault().TRANS_ID.ToSafeString(),
                            P_ORDER_NO = ORDER_NO.ToSafeString(),

                            P_SERIAL_NO = SERIAL_NUMBER.ToSafeString(),
                            P_INTERNET_NO = ACCESS_NUMBER.ToSafeString(),
                            P_SUBNUMBER = "COMPLETE",
                            P_ASSET_CODE = MAIN_ASSET.ToSafeString(),
                            P_MATERIAL_DOC = MAT_DOC.ToSafeString(),
                            P_DOC_YEAR = DOC_YEAR.ToSafeString(),

                            P_COM_CODE = COMPANY_CODE.ToSafeString(),
                            P_ERR_CODE = "0000",
                            P_ERR_MSG = "",
                            P_REMARK = REMARK
                        };
                        _updateFOAStatusTypeCommand.Handle(addresult);
                        insertErrorLog<UpdateFOAResendStatusCommand>(addresult, $"4. CH_STATUS Complete UpdateFOAResendStatusCommand", ACCESS_NUMBER.ToString());

                        //if (addresult.ret_code != null)
                        //{
                        //    code = addresult.ret_code.ToSafeString();
                        //    MSG = addresult.ret_msg.ToSafeString();
                        //}
                    }
                    catch (Exception Ex)
                    {

                        _Logger.Info(Ex.Message.ToSafeString() + ACCESS_NUMBER);
                        insertErrorLog(model, Ex.Message.ToString(), ACCESS_NUMBER);
                    }

                }
                #endregion
            }
            else
            {
                List<SubmitFOAEquipment> data = new List<SubmitFOAEquipment>();



                if (listreplaces != null && listreplaces.Any())
                {

                    data = listreplaces.Select(r => new SubmitFOAEquipment
                    {
                        MATERIAL_CODE = r.materialCode,
                        COMPANY_CODE = r.companyCode,
                        STORAGE_LOCATION = r.storLocation,
                        PLANT = r.plant,
                        SN = r.serial_number,
                        ACCESS_NUMBER = r.access_no,
                        ORDER_NO = r.order_no,
                        PRODUCT_OWNER = r.product_owner,
                        TRANS_ID = r.Tarns_id,

                        STATUS = r.ch_status
                    }).ToList();


                }
                else
                {
                    data = resultEquip.cur;
                }


                insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", ACCESS_NUMBER.ToString());
                insertErrorLog(model, "3.CH_STATUS=" + CH_STATUS, ACCESS_NUMBER);

                try
                {
                    if (data.Count > 0)
                    {
                        var res_result = await ResendToSelectSubmitFOA(data, listreplaces);
                        insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count > 0", ACCESS_NUMBER.ToString());
                        //data.Clear();



                        if (res_result.Data.ToString() == "")
                        {
                            Result.Add("COMPLETE");

                            count_success++;
                        }
                        else if (res_result.Data.ToString().Trim() == "Success 1 Order.")
                        {
                            Result.Add(res_result.Data.ToString());
                            count_success++;

                        }
                        else
                        {
                            Result.Add(res_result.Data.ToString());
                            count_err++;



                        }
                    }
                    else
                    {
                        // log error
                        insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count < 0 ", ACCESS_NUMBER.ToString());
                    }
                }
                catch (Exception Ex)
                {

                    _Logger.Info(Ex.Message.ToSafeString() + "Error Call WEBSERVICE");
                    insertErrorLog(model, Ex.Message.ToString(), ACCESS_NUMBER);

                }
            }





            TempData["accessNo"] = accessNo;
            TempData["Result"] = Result;
            return "Access Number : " + accessNo.Count + "|" + "Success : " + count_success + "|Error : " + count_err + "";
        }



        public async Task<string> IMPORTBYEXCELIN(List<ResendOrderFileListIN> dataFilein, string fileName, string Emailuser)
        {
            SubmitFOAInstallationReportQuery model = new SubmitFOAInstallationReportQuery();
            SubmitFOAReportQuery modelReportQuery = new SubmitFOAReportQuery();

            var listreplace = new List<ReplaceEquipmentModel>();
            List<string> accessNo = new List<string>();
            List<string> Result = new List<string>();
            int count_success = 0, count_err = 0;
            // List<ResendOrderFileList> Models = ConvertDataTableToList();
            string ACCESS_NUMBER = string.Empty;
            string ASSET_CODE = string.Empty;

            string SUB_NUMBER = string.Empty;
            string PRODUCT_NAME = string.Empty;
            string INSTALLATION_COST = string.Empty;
            string ORDER_DATE = string.Empty;
            string MODIFY_DATE = string.Empty;
            string TRAN_STATUS = string.Empty;
            string PRODUCT_OWNER = string.Empty;

            string STORAGE_LOCATION = string.Empty;
            string COMPANY_CODE = string.Empty;
            string SERIAL_NUMBER = string.Empty;
            string MATERIAL_CODE = string.Empty;
            string CH_STATUS = string.Empty;
            string ORDER_NO = string.Empty;
            string PLANT = string.Empty;
            string SUBCONTRACTOR_CODE = string.Empty;
            string TRANS_ID = string.Empty;

            //foreach (var re in dataFilein.Where(w => w.TRAN_STATUS.ToUpper().Trim() != "COMPLETE" && w.TRAN_STATUS.ToUpper() == "ERROR"
            //    || w.TRAN_STATUS.ToUpper().Trim() != "COMPLETE" && w.TRAN_STATUS.ToUpper() == "PENDING"))
            //{
            //    ReplaceEquipmentModel replaceModel = new ReplaceEquipmentModel();

            //    replaceModel.postDate = "";

            //    replaceModel.access_no = re.ACCESS_NO;

            //    replaceModel.product_owner = re.PRODUCT_OWNER;


            //    listreplace.Add(replaceModel);
            //}

            List<ReplaceEquipmentModel> listreplaces = new List<ReplaceEquipmentModel>();

            foreach (var re in dataFilein.Where(w =>
                w.TRAN_STATUS.ToUpper().Trim() != "COMPLETE" &&
                (w.TRAN_STATUS.ToUpper() == "ERROR" || w.TRAN_STATUS.ToUpper() == "PENDING")))
            {
                ReplaceEquipmentModel replaceModel = new ReplaceEquipmentModel
                {
                    postDate = "",
                    access_no = re.ACCESS_NO,
                    product_owner = re.PRODUCT_OWNER,
                    Tarns_id = re.TRANS_ID,
                    ch_status = re.TRAN_STATUS,
                    order_no = re.ORDER_NO
                };

                listreplaces.Add(replaceModel); // ✅ ใช้ listreplaces ให้ตรงกับที่ประกาศ
            }

            var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();

            if (resultFixAssConfig.DISPLAY_VAL == "Y")
            {
                #region Resend
                foreach (var re in dataFilein.Where(w => w.TRAN_STATUS.ToUpper().Trim() != "COMPLETE").GroupBy(g => new { g.ACCESS_NO }))
                {
                    ACCESS_NUMBER = re.FirstOrDefault().ACCESS_NO;
                    ORDER_NO = re.FirstOrDefault().ORDER_NO;



                    PRODUCT_OWNER = re.FirstOrDefault().PRODUCT_OWNER;


                    model.orderNo = "ALL";
                    model.internetNo = re.FirstOrDefault().ACCESS_NO;
                    model.status = "ALL";
                    model.productName = "ALL";
                    model.productName = "ALL";
                    model.serviceName = "";
                    model.dateFrom = "";
                    model.dateTo = "";
                    model.productOwner = "ALL";


                    modelReportQuery.accessNo = re.FirstOrDefault().ACCESS_NO;
                    modelReportQuery.orderNo = "ALL";
                    modelReportQuery.status = "ALL";
                    modelReportQuery.productName = "ALL";
                    modelReportQuery.orderType = "ALL";
                    modelReportQuery.companyCode = "ALL";
                    modelReportQuery.serviceName = "ALL";
                    modelReportQuery.subcontractorCode = "ALL";
                    modelReportQuery.plant = "ALL";
                    modelReportQuery.materialCode = "ALL";
                    modelReportQuery.storLocation = "ALL";
                    modelReportQuery.dateFrom = "";
                    modelReportQuery.dateTo = "";
                    modelReportQuery.productOwner = re.FirstOrDefault().PRODUCT_OWNER;



                    var resultEquip = GetSubmitFOAInstallation(model);
                    // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"1. GetSubmitFOAEquipment", ACCESS_NUMBER.ToString());

                    List<SubmitFOAInstallation> data = new List<SubmitFOAInstallation>();


                    if (listreplaces != null && listreplaces.Any())
                    {
                        data = listreplaces.Select(r => new SubmitFOAInstallation
                        {
                            TRAN_STATUS = r.ch_status,
                            TRANS_ID = r.Tarns_id
                        }).ToList();
                    }
                    else
                    {
                        data = resultEquip;

                    }
                    // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", ACCESS_NUMBER.ToString());
                    insertErrorLog(model, "3.CH_STATUS=" + CH_STATUS, ACCESS_NUMBER);

                    try
                    {
                        if (data.Count > 0)
                        {
                            var res_result = await ResendToSelectSubmitFOAIN(data, listreplace);
                            // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count > 0", ACCESS_NUMBER.ToString());





                            accessNo.Add(re.FirstOrDefault().ACCESS_NO);
                            if (res_result.Data.ToString() == "")
                            {
                                Result.Add("COMPLETE");
                                count_success++;
                            }
                            else if (res_result.Data.ToString() == "Success 1 Order. ")
                            {
                                Result.Add(res_result.Data.ToString());
                                count_success++;
                            }
                            else
                            {
                                Result.Add(res_result.Data.ToString());
                                count_err++;
                            }
                        }
                        else
                        {
                            // log error
                            // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count < 0 ", ACCESS_NUMBER.ToString());
                        }
                    }
                    catch (Exception Ex)
                    {

                        data.Clear();
                        _Logger.Info(Ex.Message.ToSafeString() + "Error Call WEBSERVICE");
                        // insertErrorLog(model, Ex.Message.ToString(), ACCESS_NUMBER);
                    }

                }
                #endregion

                #region Complete
                foreach (var re in dataFilein.Where(w => w.TRAN_STATUS.ToUpper().Trim() == "COMPLETE"))
                {



                    model.internetNo = re.ACCESS_NO;
                    model.status = "ALL";
                    model.productName = "ALL";
                    model.serviceName = "";



                    modelReportQuery.accessNo = re.ACCESS_NO;
                    modelReportQuery.status = "ALL";
                    modelReportQuery.productName = "ALL";
                    modelReportQuery.orderType = "ALL";
                    modelReportQuery.companyCode = "ALL";
                    modelReportQuery.serviceName = "ALL";
                    modelReportQuery.subcontractorCode = "ALL";
                    modelReportQuery.plant = "ALL";
                    modelReportQuery.materialCode = "ALL";
                    modelReportQuery.storLocation = "ALL";
                    modelReportQuery.dateFrom = "";
                    modelReportQuery.dateTo = "";

                    // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"1. GetSubmitFOAEquipment", ACCESS_NUMBER.ToString());
                    var resultEquip = GetSubmitFOAInstallation(model);
                    // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"2. resultEquip.cur", ACCESS_NUMBER.ToString());
                    insertErrorLog(model, "3.CH_STATUS=" + CH_STATUS, ACCESS_NUMBER);

                    try
                    {
                        accessNo.Add(re.ACCESS_NO);
                        Result.Add("COMPLETE");
                        count_success++;
                        var addresult = new UpdateFOAResendStatusCommand
                        {
                            P_ORDER_NO = ORDER_NO.ToSafeString(),

                            P_SERIAL_NO = SERIAL_NUMBER.ToSafeString(),
                            P_INTERNET_NO = ACCESS_NUMBER.ToSafeString(),
                            P_SUBNUMBER = "COMPLETE",

                            P_COM_CODE = COMPANY_CODE.ToSafeString(),
                            P_ERR_CODE = "0000",
                            P_ERR_MSG = ""
                        };
                        _updateFOAStatusTypeCommand.Handle(addresult);
                        insertErrorLog<UpdateFOAResendStatusCommand>(addresult, $"4. CH_STATUS Complete UpdateFOAResendStatusCommand", ACCESS_NUMBER.ToString());

                        //if (addresult.ret_code != null)
                        //{
                        //    code = addresult.ret_code.ToSafeString();
                        //    MSG = addresult.ret_msg.ToSafeString();
                        //}
                    }
                    catch (Exception Ex)
                    {

                        _Logger.Info(Ex.Message.ToSafeString() + ACCESS_NUMBER);
                        insertErrorLog(model, Ex.Message.ToString(), ACCESS_NUMBER);
                    }

                }
                #endregion
            }
            else
            {

                var resultEquip = GetSubmitFOAInstallation(model);

                List<SubmitFOAInstallation> data = new List<SubmitFOAInstallation>();


                if (listreplaces != null && listreplaces.Any())
                {
                    data = listreplaces.Select(r => new SubmitFOAInstallation
                    {
                        TRAN_STATUS = r.ch_status,
                        TRANS_ID = r.Tarns_id
                    }).ToList();
                }
                else
                {
                    data = resultEquip;

                }
                insertErrorLog(model, "3.CH_STATUS=" + CH_STATUS, ACCESS_NUMBER);

                try
                {
                    if (data.Count > 0)
                    {
                        var res_result = await ResendToSelectSubmitFOAIN(data, listreplace);

                        if (res_result.Data.ToString() == "")
                        {
                            Result.Add("COMPLETE");
                            count_success++;
                        }
                        else if (res_result.Data.ToString().Trim() == "Success 1 Order.")
                        {
                            Result.Add(res_result.Data.ToString());
                            count_success++;
                        }
                        else
                        {
                            Result.Add(res_result.Data.ToString());
                            count_err++;
                        }
                    }
                    else
                    {
                        //log error;
                        // insertErrorLog<SubmitFOAEquipmentReportQuery>(model, $"4.data.Count < 0 ", ACCESS_NUMBER.ToString());
                    }
                }
                catch (Exception Ex)
                {

                    data.Clear();
                    _Logger.Info(Ex.Message.ToSafeString() + "Error Call WEBSERVICE");
                    // insertErrorLog(model, Ex.Message.ToString(), ACCESS_NUMBER);
                }
            }







            TempData["accessNo"] = accessNo;
            TempData["Result"] = Result;
            return "Access Number : " + accessNo.Count + "|" + "Success : " + count_success + "|Error : " + count_err + "";
        }




        private async Task<string> sendEmailImportStatus(string fileName, string Emailuser, List<string> accessNo, List<string> Result, string sum)
        {
            try
            {
                _Logger.Info("Start SendEmail");
                string result = "";
                string body = "";

                body = EmailTemplate(fileName, accessNo, Result, sum);

                string sendto = string.Empty;
                if (Emailuser != "" || Emailuser != null)
                {
                    sendto = Emailuser;
                }
                else
                {
                    sendto = CurrentUser.Email.ToSafeString();
                }

                var command = new SendMailLastMileNotificationCommand
                {
                    ProcessName = "SEND_EMAIL_RESEND_ORDER",
                    Subject = "Resend Import File Success:",
                    Body = body,
                    // msAttachFiles = files,
                    SendTo = sendto
                    // SendTo = CurrentUser.Email.ToSafeString()
                };
                _sendMail.Handle(command);

                _Logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                if (command.ReturnMessage == "Success.")
                {
                    result = "Success";
                }
                else
                {
                    result = command.ReturnMessage;
                }

                return result;

            }
            catch (Exception Ex)
            {

                _Logger.Info("Fail:" + Ex.Message.ToSafeString());
                return null;
            }
        }
        public string EmailTemplate(string fileName, List<string> accessNo, List<string> Result, string sum)
        {

            try
            {
                //--
                string tempPath = Path.GetTempPath();
                string DateNow = DateTime.Now.ToString("DD/MM/YYYY").ToString();
                StringBuilder tempBody = new StringBuilder();
                CultureInfo ThaiCulture = new CultureInfo("th-TH");
                CultureInfo UsaCulture = new CultureInfo("en-US");


                #region tempBody

                tempBody.Append("<p style='font-weight:bolder;'>เรียน..." + CurrentUser.UserFullNameInThai + "</p>");
                tempBody.Append("<br/>");



                tempBody.Append("<br/>");
                tempBody.Append("<span>File:" + fileName + " is resend Success.");
                tempBody.Append("</span>");
                tempBody.Append("<br/>");

                tempBody.Append("</span>");
                tempBody.Append("<br/>");

                tempBody.Append("<span>" + sum.Replace("|", ", "));

                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                for (int i = 0; i < Result.Count; i++)
                {
                    if (Result[i].ToUpper().Contains("ERROR"))
                    {
                        tempBody.Append("<span>" + accessNo[i] + " " + Result[i].Replace("|", ", "));
                        tempBody.Append("</span>");
                        tempBody.Append("<br/>");
                    }
                }

                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<span>Thanks.");
                tempBody.Append("</span>");
                tempBody.Append("<br/>");


                #endregion
                string body = "";
                body = tempBody.ToSafeString();

                return body;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error ExportFile: " + ex.GetErrorMessage());
                return ex.GetErrorMessage();
            }
        }

        #endregion

        //R31.05.2021 for resend by File
        [HttpPost]
        public JsonResult CheckRollBack()
        {
            var rollback = Get_FBSS_CONFIG_TBL_LOV("RESEND_ORDER", "ROLLBACK").Select(x => x.ACTIVEFLAG).FirstOrDefault();
            return Json(rollback.ToSafeString().ToUpper());
        }

        private void insertErrorLog<T>(T dataS, string _msg, string access_number)
        {
            var error = new SumitFOAErrorLogCommand
            {
                access_number = access_number,
                in_xml_foa = dataS.DumpToXml(),
                created_date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                updated_date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                updated_desc = _msg

            };


            _SumitFOAErrorLogCommand.Handle(error);
        }

    }
}
