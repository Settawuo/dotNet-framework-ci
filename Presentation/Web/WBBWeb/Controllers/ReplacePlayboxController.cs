using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;


namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class ReplacePlayboxController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<StatLogCommand> _StatLogCommand;
        //
        // GET: /ReplacePlaybox/

        public ReplacePlayboxController(IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<StatLogCommand> StatLogCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _StatLogCommand = StatLogCommand;
            base.Logger = logger;
        }

        public ActionResult Index(string Status = "")
        {
            Session["FullUrl"] = this.Url.Action("Index", "ReplacePlaybox", null, this.Request.Url.Scheme);
            Session["CONTRACTMOBILENO"] = null;
            ViewBag.User = base.CurrentUser;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FBBREPLACE = GetGeneralScreenFBBREPLACE();

            if (Status != "")
            {
                ViewBag.Status = Status;
            }

            //R21.7 Flag Open/Close
            string flagOpen = base.LovData.Where(l => l.Name == "WEB_REPLACE_PLAYBOX").Select(i => i.LovValue1).FirstOrDefault();
            if (flagOpen == "N")
            {
                return View("NotFound");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ReplacePlaybox(ReplacePlayboxModel Model)
        {
            //R21.7 Flag Open/Close
            string flagOpen = base.LovData.Where(l => l.Name == "WEB_REPLACE_PLAYBOX").Select(i => i.LovValue1).FirstOrDefault();
            if (flagOpen == "N")
            {
                return View("NotFound");
            }
            else
            {
                if (Session["CONTRACTMOBILENO"] == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    Model.ContractMobileNO = Session["CONTRACTMOBILENO"].ToSafeString();
                }

                ViewBag.FBBREPLACE = GetGeneralScreenFBBREPLACE();

                return View(Model);
            }
        }

        [HttpPost]
        public JsonResult GetContractMobileNo(string mobileNo, string cardNo, string cardType)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            string user = "";
            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            var contractMobileNo = "";
            Session["CONTRACTMOBILENO"] = null;

            if (String.IsNullOrEmpty(cardNo) || String.IsNullOrEmpty(cardType))
            {
                base.Logger.Info("GetContractMobileNo: cardNo or cardType is null");
                return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
            }

            bool haveProfile = false;
            try
            {
                var query = new evESeServiceQueryMassCommonAccountInfoQuery
                {
                    inOption = "2",
                    inMobileNo = mobileNo,
                    inCardNo = cardNo,
                    inCardType = cardType,
                    Page = "Change Package Promotion",
                    Username = "USER",
                    ClientIP = ipAddress,
                    FullUrl = FullUrl
                };
                var massCommon = _queryProcessor.Execute(query);
                if (massCommon.errorMessage == "")
                {
                    haveProfile = true;
                }

            }
            catch (Exception ex)
            {
                base.Logger.Info(ex.GetErrorMessage());
                return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
            }
            if (haveProfile)
            {
                try
                {
                    var query = new evESeServiceQueryMassCommonAccountInfoQuery
                    {
                        inOption = "4",
                        inMobileNo = mobileNo,
                        inCardNo = cardNo,
                        inCardType = cardType,
                        Page = "Change Package Promotion",
                        Username = "USER",
                        ClientIP = ipAddress,
                        FullUrl = FullUrl
                    };
                    var massCommon = _queryProcessor.Execute(query);

                    contractMobileNo = massCommon.outServiceMobileNo;

                    Session["CONTRACTMOBILENO"] = contractMobileNo;

                    return Json(new { data = contractMobileNo.Remove(0, 6).Insert(0, "XXXXXX") }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    base.Logger.Info(ex.GetErrorMessage());
                    return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { data = contractMobileNo }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckHavePlaybox(string mobileNo, string cardNo)
        {
            bool CheckHavePlaybox = false;
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = mobileNo + ipAddress;

            #endregion
            var query = new evOMQueryListServiceAndPromotionByPackageTypeQuery
            {
                mobileNo = mobileNo,
                idCard = cardNo,
                FullUrl = FullUrl
            };

            var model = _queryProcessor.Execute(query);
            CheckHavePlaybox = model.checkHavePlayBox;


            return Json(new { CheckHavePlaybox = CheckHavePlaybox }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ReplacePlayboxCheckProductMainNotUse(string mobileNo, string cardNo)
        {
            bool ReplacePlayboxCheckProductMainNotUse = false;
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = mobileNo + ipAddress;

            #endregion
            var query = new evOMQueryListServiceAndPromotionByPackageTypeQuery
            {
                mobileNo = mobileNo,
                idCard = cardNo,
                FullUrl = FullUrl
            };

            var model = _queryProcessor.Execute(query);
            ReplacePlayboxCheckProductMainNotUse = model.replacePlayboxCheckProductMainNotUse;


            return Json(new { ReplacePlayboxCheckProductMainNotUse = ReplacePlayboxCheckProductMainNotUse }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveReplacePlaybox(ReplacePlayboxModel Model)
        {
            if (Session["CONTRACTMOBILENO"] == null)
            {
                return RedirectToAction("Index");
            }

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            List<LovValueModel> config = base.LovData.Where(l => l.Type == "FBB_SCREEN" && l.LovValue5 == "FBBREPLACE").ToList();
            string L_POP_UP_SUCCESS_01 = "";
            string L_POP_UP_SUCCESS_02 = "";
            string L_POP_UP_SUCCESS_03 = "";
            string L_POP_UP_ERROR_01 = "";
            string L_POP_UP_ERROR_02 = "";
            if (Session[WBBWeb.Extension.WebConstants.SessionKeys.CurrentUICulture].ToString() == "1")
            {
                L_POP_UP_SUCCESS_01 = config.Where(t => t.Name == "L_POP_UP_SUCCESS_01").Select(t => new { t.LovValue1 }).FirstOrDefault().LovValue1.ToSafeString();
                L_POP_UP_SUCCESS_02 = config.Where(t => t.Name == "L_POP_UP_SUCCESS_02").Select(t => new { t.LovValue1 }).FirstOrDefault().LovValue1.ToSafeString();
                L_POP_UP_SUCCESS_03 = config.Where(t => t.Name == "L_POP_UP_SUCCESS_03").Select(t => new { t.LovValue1 }).FirstOrDefault().LovValue1.ToSafeString();
                L_POP_UP_ERROR_01 = config.Where(t => t.Name == "L_POP_UP_ERROR_01").Select(t => new { t.LovValue1 }).FirstOrDefault().LovValue1.ToSafeString();
                L_POP_UP_ERROR_02 = config.Where(t => t.Name == "L_POP_UP_ERROR_02").Select(t => new { t.LovValue1 }).FirstOrDefault().LovValue1.ToSafeString();
            }
            else
            {
                L_POP_UP_SUCCESS_01 = config.Where(t => t.Name == "L_POP_UP_SUCCESS_01").Select(t => new { t.LovValue2 }).FirstOrDefault().LovValue2.ToSafeString();
                L_POP_UP_SUCCESS_02 = config.Where(t => t.Name == "L_POP_UP_SUCCESS_02").Select(t => new { t.LovValue2 }).FirstOrDefault().LovValue2.ToSafeString();
                L_POP_UP_SUCCESS_03 = config.Where(t => t.Name == "L_POP_UP_SUCCESS_03").Select(t => new { t.LovValue2 }).FirstOrDefault().LovValue2.ToSafeString();
                L_POP_UP_ERROR_01 = config.Where(t => t.Name == "L_POP_UP_ERROR_01").Select(t => new { t.LovValue2 }).FirstOrDefault().LovValue2.ToSafeString();
                L_POP_UP_ERROR_02 = config.Where(t => t.Name == "L_POP_UP_ERROR_02").Select(t => new { t.LovValue2 }).FirstOrDefault().LovValue2.ToSafeString();
            }

            string Status = "";
            ProcSiebelModel result = new ProcSiebelModel();

            try
            {

                ProcSiebelQuery query = new ProcSiebelQuery
                {
                    P_INTERNET_ID = Model.AisAirNumber,
                    P_CONTACT_MOBILE = Model.ContractMobileNO,
                    FullUrl = FullUrl,
                    client_ip = ipAddress
                };
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                Status = L_POP_UP_ERROR_01 + "|" + L_POP_UP_ERROR_02;
            }

            if (result != null && result.OUTPUT_RETURN_CODE == "0")
            {
                try
                {
                    SiebelCreateTroubleTicketQuery querySiebel = new SiebelCreateTroubleTicketQuery
                    {
                        InProblemDate_End = result.InProblemDate_End,
                        InMooban = result.InMooban,
                        InMobileNumber = result.InMobileNumber,
                        InIndoor = result.InIndoor,
                        InDestModel = result.InDestModel,
                        InParam1 = result.InParam1,
                        InFloor = result.InFloor,
                        InAssetId = result.InAssetId,
                        InTumbol = result.InTumbol,
                        InRefArea = result.InRefArea,
                        InCurrentSignalLevel = result.InCurrentSignalLevel,
                        InStreet = result.InStreet,
                        InParam2 = result.InParam2,
                        InDestMobileNumber = result.InDestMobileNumber,
                        InAccountId = result.InAccountId,
                        InUsedCountry = result.InUsedCountry,
                        InChannel = result.InChannel,
                        InBuilding = result.InBuilding,
                        InSoi = result.InSoi,
                        InParam3 = result.InParam3,
                        InModel = result.InModel,
                        InSubCategory = result.InSubCategory,
                        InProvince = result.InProvince,
                        InProblemDate = result.InProblemDate,
                        InPath = result.InPath,
                        InDescription = result.InDescription,
                        InOption = result.InOption,
                        InMaxSignalLevel = result.InMaxSignalLevel,
                        InHouseNumber = result.InHouseNumber,
                        InProductName = result.InProductName,
                        InDestBrand = result.InDestBrand,
                        InAmphur = result.InAmphur,
                        InParam5 = result.InParam5,
                        InParam4 = result.InParam4,
                        InContactId = result.InContactId,
                        InSymptomNote = result.InSymptomNote,
                        InOtherContactPhone = result.InOtherContactPhone,
                        InOperatorName = result.InOperatorName,
                        InBrand = result.InBrand,
                        InContentProvider = result.InContentProvider,
                        InCategory = result.InCategory,
                        FullURL = FullUrl

                    };

                    var resultSiebel = _queryProcessor.Execute(querySiebel);
                    if (resultSiebel.OutResult == "Success")
                    {
                        Status = L_POP_UP_SUCCESS_01 + "|" + L_POP_UP_SUCCESS_02 + "|" + L_POP_UP_SUCCESS_03;
                    }
                    else
                    {
                        Status = L_POP_UP_ERROR_01 + "|" + L_POP_UP_ERROR_02;
                    }
                }
                catch (Exception ex)
                {
                    Status = L_POP_UP_ERROR_01 + "|" + L_POP_UP_ERROR_02;
                }

            }
            else
            {
                Status = L_POP_UP_ERROR_01 + "|" + L_POP_UP_ERROR_02;
            }

            return RedirectToAction("Index", new { Status = Status });
        }

        private void SaveStatlog(string username = "", string VisitType = "", string REQ_IPADDRESS = "", string SELECT_PAGE = "", string HOST = "", string LC = "")
        {
            try
            {
                var statcommand = new StatLogCommand
                {
                    Username = username,
                    VisitType = VisitType,
                    REQ_IPADDRESS = REQ_IPADDRESS,
                    SelectPage = SELECT_PAGE,
                    HOST = HOST,
                    LC = LC
                };

                _StatLogCommand.Handle(statcommand);
                Logger.Info("Statlogww: " + statcommand.ReturnDesc);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }
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

        private List<LovScreenValueModel> GetGeneralScreenFBBREPLACE()
        {
            List<LovValueModel> config = null;

            config = base.LovData.Where(l => l.Type == "FBB_SCREEN" && l.LovValue5 == "FBBREPLACE").ToList();
            List<LovScreenValueModel> screenValue;
            if (Session[WBBWeb.Extension.WebConstants.SessionKeys.CurrentUICulture].ToString() == "1")
            {
                screenValue = config.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue1,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue,
                    Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                    DisplayValueJing = l.Text
                }).ToList();
            }
            else
            {
                screenValue = config.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue2,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue,
                    Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                    DisplayValueJing = l.Text
                }).ToList();
            }

            return screenValue;
        }
    }
}
