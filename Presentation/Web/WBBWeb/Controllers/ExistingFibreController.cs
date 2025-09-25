using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{

    public partial class ProcessController : WBBController
    {
        //[HttpPost]
        public ActionResult RedirectExistingFibreMain()
        {
            try
            {
                QuickWinPanelModel model = (QuickWinPanelModel)TempData["QuickWinPanelModel"];
                ViewBag.PBOXCount = model.CustomerRegisterPanelModel.pbox_count;
                return View("ExistingFibre/Home", model);
                //return View("ExistingFibre/New_Main");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Check_Session()
        {
            string _Result = Session["CONTRACTMOBILENO"].ToSafeString();
            if (_Result != null)
            {
                if (_Result != "")
                {
                    _Result = "1";
                }
                else
                {
                    _Result = "0";
                }
            }
            else
            {
                _Result = "0";
            }

            return Json(new { Result = _Result });
        }

        public ActionResult ExistingFibre(string Data = "")
        {
            if (Data != "")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                //string channel = "";
                string NonMobileNo = "";
                string lang = "";
                //string transactionId = "";
                string GetIdCardStatus = "";
                if (DataTemps.Count() > 0)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            //if (DataTemp[0].ToSafeString() == "channel")
                            //{
                            //    channel = DataTemp[1].ToSafeString();
                            //}
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                NonMobileNo = DataTemp[1].ToSafeString();
                                //NonMobileNo = "8800010059";
                                ViewBag.NonMobileNo = NonMobileNo;
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                lang = DataTemp[1].ToSafeString();
                                if (lang == "TH")
                                {
                                    ViewBag.LanguagePage = "1";
                                }
                                else
                                {
                                    ViewBag.LanguagePage = "2";
                                }
                            }
                            //if (DataTemp[0].ToSafeString() == "transactionId")
                            //{
                            //    transactionId = DataTemp[1].ToSafeString();
                            //}
                        }
                        else
                        {
                            // value in put ไม่ถูกต้อง
                            CheckInput = false;
                            break;

                        }
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    CheckInput = false;

                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "")
                    {
                        /// CheckShow menu
                        ViewBag.CheckShowMenuChangeService = CheckShowMenuChangeService(NonMobileNo, IdCard);
                        ViewBag.CheckPendingOrder = CheckPendingOrder(NonMobileNo, "TOPUP_MESH");
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        ViewBag.PageShow = "Mobile";
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    ViewBag.PageShow = "LoginFail";
                }
            }

            Session["FullUrl"] = this.Url.Action("ExistingFibre", "Process", null, this.Request.Url.Scheme);

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
            ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");
            ViewBag.LabelLovScreenFBBWEB031 = GetScreenConfig("FBBWEB031");
            ViewBag.Function = "ExistingFibre";
            ViewBag.SetAction = @"/Process/ExistingFibre";
            return View("ExistingFibre/New_Main");
        }

        [HttpPost]
        public ActionResult ExistingFibre(QuickWinPanelModel model, string Data = "", string LcCode = "", string ASCCode = "", string EmployeeID = "", string outType = "", string outSubType = "", string outEmpName = "", string isOfficer = "", string pageNo = "",
           string outTitle = "", string outCompanyName = "", string outDistChn = "", string outChnSales = "", string outShopType = "", string outOperatorClass = "", string outASCTitleThai = "", string outASCPartnerName = "", string outMemberCategory = "", string outPosition = "",
            string outLocationRegion = "", string outLocationSubRegion = "", string THFirstName = "", string THLastName = "", string outLocationProvince = "")
        {
            ViewBag.LabelFBBOR041 = GetScreenConfig("FBBOR041");
            if (Data != "")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                //string channel = "";
                string NonMobileNo = "";
                string lang = "";
                //string transactionId = "";
                string GetIdCardStatus = "";
                if (DataTemps.Count() > 0)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            //if (DataTemp[0].ToSafeString() == "channel")
                            //{
                            //    channel = DataTemp[1].ToSafeString();
                            //}
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                NonMobileNo = DataTemp[1].ToSafeString();
                                //NonMobileNo = "8800010059";
                                ViewBag.NonMobileNo = NonMobileNo;
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                lang = DataTemp[1].ToSafeString();
                                if (lang == "TH")
                                {
                                    ViewBag.LanguagePage = "1";
                                }
                                else
                                {
                                    ViewBag.LanguagePage = "2";
                                }
                            }
                            //if (DataTemp[0].ToSafeString() == "transactionId")
                            //{
                            //    transactionId = DataTemp[1].ToSafeString();
                            //}
                        }
                        else
                        {
                            // value in put ไม่ถูกต้อง
                            CheckInput = false;
                            break;

                        }
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    CheckInput = false;

                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "")
                    {
                        /// CheckShow menu
                        ViewBag.CheckShowMenuChangeService = CheckShowMenuChangeService(NonMobileNo, IdCard);
                        ViewBag.CheckPendingOrder = CheckPendingOrder(NonMobileNo, "TOPUP_MESH");
                        ViewBag.IdCard = IdCard;
                        ViewBag.CardType = CardType;
                        ViewBag.PageShow = "Mobile";
                    }
                    else
                    {
                        // Login fail
                        IdCard = "";
                        CardType = "";
                        ViewBag.PageShow = "LoginFail";
                    }
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                    ViewBag.LanguagePage = "1";
                    ViewBag.PageShow = "LoginFail";
                }
                Session["FullUrl"] = this.Url.Action("ExistingFibre", "Process", null, this.Request.Url.Scheme);

                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
                ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");
                ViewBag.LabelLovScreenFBBWEB031 = GetScreenConfig("FBBWEB031");
                ViewBag.Function = "ExistingFibre";
                ViewBag.SetAction = @"/Process/ExistingFibre";
                ViewBag.PBOXCount = GetNumberPbox(NonMobileNo, IdCard);

                return View("ExistingFibre/Home");
            }
            else if (isOfficer == "Y" && pageNo == "1")
            {
                Session["FullUrl"] = this.Url.Action("ExistingFibre", "Process", null, this.Request.Url.Scheme);

                ViewBag.LcCode = LcCode;
                ViewBag.ASCCode = ASCCode;
                ViewBag.EmployeeID = EmployeeID;
                ViewBag.outType = outType;
                ViewBag.outSubType = outSubType;
                ViewBag.outEmpName = outEmpName;
                ViewBag.isOfficer = isOfficer;

                ViewBag.outTitle = outTitle;
                ViewBag.outCompanyName = outCompanyName;
                ViewBag.outDistChn = outDistChn;
                ViewBag.outChnSales = outChnSales;
                ViewBag.outShopType = outShopType;
                ViewBag.outOperatorClass = outOperatorClass;
                ViewBag.outASCTitleThai = outASCTitleThai;
                ViewBag.outASCPartnerName = outASCPartnerName;
                ViewBag.outMemberCategory = outMemberCategory;
                ViewBag.outPosition = outPosition;
                ViewBag.outLocationRegion = outLocationRegion;
                ViewBag.outLocationSubRegion = outLocationSubRegion;
                ViewBag.THFirstName = THFirstName;
                ViewBag.THLastName = THLastName;
                //R21.5 Pool Villa
                ViewBag.outLocationProvince = outLocationProvince;

                ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
                ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
                ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
                ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
                ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
                ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
                ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
                ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");
                ViewBag.LabelLovScreenFBBWEB031 = GetScreenConfig("FBBWEB031");
                ViewBag.Function = "ExistingFibre";
                ViewBag.SetAction = @"/Process/ExistingFibre";
                ViewBag.isOfficer = isOfficer;

                return View("ExistingFibre/New_Main");
            }
            else
            {
                List<LovValueModel> config = base.LovData.Where(l => l.Name == "MAPPING_ACC_CATEGORY" && l.Type == "FBB_CONSTANT" && l.LovValue1 == model.outAccountCategory).ToList();
                if (config.Any())
                {
                    model.CATEGORY = config.Select(i => i.LovValue2).FirstOrDefault();
                }
                else
                {
                    model.CATEGORY = "";
                }

                ViewBag.LabelFBBTR016 = GetChangePromotionScreenConfig();
                ViewBag.LabelFBBOR016 = GetChangePromotionPageCode();
                ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");
                ViewBag.OverruleDropdown = GetDropDownConfig("OVERRULE");
                ViewBag.LabelLovScreenFBBWEB031 = GetScreenConfig("FBBWEB031");
                Session["FullUrl"] = this.Url.Action("ExistingFibreMain", "Process", null, this.Request.Url.Scheme);

                if (isOfficer == "Y")
                {
                    ViewBag.LcCode = LcCode;
                    ViewBag.ASCCode = ASCCode;
                    ViewBag.EmployeeID = EmployeeID;
                    ViewBag.outType = outType;
                    ViewBag.outSubType = outSubType;
                    ViewBag.outEmpName = outEmpName;
                    ViewBag.isOfficer = isOfficer;

                    ViewBag.outTitle = outTitle;
                    ViewBag.outCompanyName = outCompanyName;
                    ViewBag.outDistChn = outDistChn;
                    ViewBag.outChnSales = outChnSales;
                    ViewBag.outShopType = outShopType;
                    ViewBag.outOperatorClass = outOperatorClass;
                    ViewBag.outASCTitleThai = outASCTitleThai;
                    ViewBag.outASCPartnerName = outASCPartnerName;
                    ViewBag.outMemberCategory = outMemberCategory;
                    ViewBag.outPosition = outPosition;
                    ViewBag.outLocationRegion = outLocationRegion;
                    ViewBag.outLocationSubRegion = outLocationSubRegion;
                    ViewBag.THFirstName = THFirstName;
                    ViewBag.THLastName = THLastName;
                    ViewBag.outLocationProvince = outLocationProvince;

                    var CustomerData = GetCustomerInfo(model.outAccountNumber);
                    if (CustomerData != null)
                    {
                        var ListCardType = GetConfigByType("ID_CARD_TYPE");
                        string CardTypeName = "";
                        if (CustomerData != null && ListCardType.Count() > 0)
                        {
                            if (Session[WBBWeb.Extension.WebConstants.SessionKeys.CurrentUICulture].ToString() == "1")
                            {
                                // ค้นหา IDCardType เป็น ภาษาไทย
                                CardTypeName = ListCardType.Where(t => t.Text == CustomerData.idCardType).Count() > 0 ? ListCardType.Where(t => t.Text == CustomerData.idCardType).FirstOrDefault().Value : "";
                            }
                            else
                            {
                                CardTypeName = ListCardType.Where(t => t.Text == CustomerData.idCardType).Count() > 0 ? ListCardType.Where(t => t.Text == CustomerData.idCardType).FirstOrDefault().Value2 : "";
                            }
                        }
                        model.IDCardType = CardTypeName;
                        model.IDCardNo = CustomerData.idCardNum;

                        // R20.7 รอบต้นเดือน
                        string CardTypeNameENG = "";
                        CardTypeNameENG = ListCardType.Where(t => t.Text == CustomerData.idCardType).Count() > 0 ? ListCardType.Where(t => t.Text == CustomerData.idCardType).FirstOrDefault().Text : "";
                        model.IDCardTypeENG = CardTypeNameENG.ToString();

                        var FullUrl = Session["FullUrl"].ToSafeString();
                        /// CheckShow menu
                        ViewBag.CheckShowMenuChangeService = CheckShowMenuChangeService(model.CoveragePanelModel.P_MOBILE, model.IDCardNo, FullUrl);
                        ViewBag.CheckPendingOrder = CheckPendingOrder(model.CoveragePanelModel.P_MOBILE, "TOPUP_MESH", "");
                    }
                }

                //R20.7 Add by Aware : Atipon > Check Pending Order FBSS
                var url = this.Url.Action("ExistingFibreMain", "Process", null, this.Request.Url.Scheme);

                model.PendingOrderFbss_Flag = GetCheckPendingOrderFbss(model.CoveragePanelModel.P_MOBILE, url);
                model.CustomerRegisterPanelModel.pbox_count = GetNumberPbox(model.CoveragePanelModel.P_MOBILE, model.IDCardNo);
                ViewBag.PBOXCount = model.CustomerRegisterPanelModel.pbox_count;
                ViewBag.QuickWinPanelModel = model;

                TempData["QuickWinPanelModel"] = model;

                #region GetLastLoginDate
                var _GetSessionLoginDateQuery = new GetSessionLoginDateQuery
                {

                    CustInternetNum = model.CoveragePanelModel.P_MOBILE,
                };
                var _Result = _queryProcessor.Execute(_GetSessionLoginDateQuery);

                ViewBag.LoginDate = _Result.ReturnDate;
                Login_Log(model);
                #endregion

                return View("ExistingFibre/Home", model);
            }
        }

        public ActionResult MenuOfficer(string LcCode = "", string ASCCode = "", string EmployeeID = ""
            , string outType = "", string outSubType = "", string outMobileNo = "", string PartnerName = "", string outLocationEmailByRegion = "", string outEmpName = ""
            , string outTitle = "", string outCompanyName = "", string outDistChn = "", string outChnSales = "", string outShopType = ""
            , string outOperatorClass = "", string outASCTitleThai = "", string outASCPartnerName = "", string outMemberCategory = "", string outPosition = ""
            , string outLocationRegion = "", string outLocationSubRegion = "", string THFirstName = "", string THLastName = "", string outLocationProvince = ""
            )
        {
            if (LcCode == "" || null == Session["secIDS"])
            {
                return RedirectToAction("Index", "Officer");
            }

            ViewBag.LcCode = LcCode;
            ViewBag.ASCCode = ASCCode;
            ViewBag.EmployeeID = EmployeeID;
            ViewBag.outType = outType;
            ViewBag.outSubType = outSubType;
            ViewBag.outMobileNo = outMobileNo;
            ViewBag.PartnerName = PartnerName;
            ViewBag.outLocationEmailByRegion = outLocationEmailByRegion;
            ViewBag.outEmpName = outEmpName;
            //20.3
            ViewBag.outTitle = outTitle;
            ViewBag.outCompanyName = outCompanyName;
            ViewBag.outDistChn = outDistChn;
            ViewBag.outChnSales = outChnSales;
            ViewBag.outShopType = outShopType;
            ViewBag.outOperatorClass = outOperatorClass;
            ViewBag.outASCTitleThai = outASCTitleThai;
            ViewBag.outASCPartnerName = outASCPartnerName;
            ViewBag.outMemberCategory = outMemberCategory;
            ViewBag.outPosition = outPosition;
            ViewBag.outLocationRegion = outLocationRegion;
            ViewBag.outLocationSubRegion = outLocationSubRegion;
            ViewBag.THFirstName = THFirstName;
            ViewBag.THLastName = THLastName;
            //R21.5 Pool Villa
            ViewBag.outLocationProvince = outLocationProvince;

            var processController = DependencyResolver.Current.GetService<ProcessController>();
            ViewBag.LabelLovScreenFBBWEB031 = processController.GetScreenConfig("FBBWEB031");
            ViewBag.SetAction = @"/Process/ExistingFibre";
            return View("ExistingFibre/Menu_Officer");
        }

        public void Login_Log(QuickWinPanelModel model)
        {
            var sessionId = SessionManagement.GetSessionId();
            var requestCommand = new SessionLoginCommand
            {
                CustInternetNum = model.CoveragePanelModel.P_MOBILE,
                SessionId = sessionId
            };
            _sessionLoginCommand.Handle(requestCommand);
        }

        [HttpPost]
        public ActionResult LogMenu(string mobile, string aisnumber, string id_card, string menu)
        {

            ExistingLogMenu _ExistingLogMenu = new ExistingLogMenu();
            _ExistingLogMenu.internetNo = aisnumber;
            _ExistingLogMenu.mobileNo = mobile;
            _ExistingLogMenu.menu = menu;

            StartInterface_Existing(_ExistingLogMenu, "LogMenu", aisnumber, id_card, menu);
            return Json(new { result = true, }, JsonRequestBehavior.AllowGet);
        }

        private InterfaceLogCommand StartInterface_Existing<T>(T query, string methodName, string transactionId, string idCardNo, string INTERFACE_NODE)
        {

            string FullUrl = "";
            if (Session["FullUrl"] != null)
            {
                FullUrl = Session["FullUrl"].ToSafeString();
                FullUrl = "FBB" + "|" + FullUrl;
            }

            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = "ExistingMenu",
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = FullUrl,
                CREATED_BY = "FBBWEB",
                REQUEST_STATUS = "Success",
                UPDATED_BY = "FBBWEB",
                UPDATED_DATE = DateTime.Now,
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        public List<MenuAuthorize> GetListMenuAuthorize(string PARTNER_TYPE = "", string PARTNER_SUBTYPE = "")
        {
            List<MenuAuthorize> ResultData = new List<MenuAuthorize>();
            var _GetListMenuAuthorizeQuery = new GetListMenuAuthorizeQuery
            {
                P_PARTNER_TYPE = PARTNER_TYPE,
                P_PARTNER_SUBTYPE = PARTNER_SUBTYPE
            };
            var _Result = _queryProcessor.Execute(_GetListMenuAuthorizeQuery);
            if (_Result.ListMenuAuthorize != null && _Result.ListMenuAuthorize.Count > 0)
            {
                ResultData = _Result.ListMenuAuthorize;
            }
            return ResultData;

        }

        public ActionResult test()
        {
            ViewBag.LabelLovScreenFBBWEB031 = GetScreenConfig("FBBWEB031");
            return View("ExistingFibre/test");
        }

        public ActionResult BypassCheckCoverage(QuickWinPanelModel model)
        {
            /// case map
            if (Session["ESRI_BYPASSCOVERAGEMODEL"] != null)
            {
                model = Session["ESRI_BYPASSCOVERAGEMODEL"] as QuickWinPanelModel;
            }
            else
            {
                if (!string.IsNullOrEmpty(model.HdResultId))
                {
                    CoverageAreaResultModel dataCoverageAreaResult = new CoverageAreaResultModel();
                    dataCoverageAreaResult = GetCoverageAreaResultByResultID(model.HdResultId);
                    if (dataCoverageAreaResult != null && dataCoverageAreaResult.RESULTID != null)
                    {
                        model.CoveragePanelModel.RESULT_ID = dataCoverageAreaResult.RESULTID.ToSafeString();
                        model.CoverageAreaResultModel = dataCoverageAreaResult;
                        model.CoveragePanelModel.Address.L_ZIPCODE = dataCoverageAreaResult.ZIPCODE_ROWID.ToSafeString();
                        model.CoveragePanelModel.Address.ZIPCODE_ID = dataCoverageAreaResult.ZIPCODE_ROWID.ToSafeString();
                        if (!string.IsNullOrEmpty(model.DataBypass_BuildName))
                            model.DataBypass_BuildNameSetup = model.DataBypass_BuildName.Replace("+", " ") + " " + model.DataBypass_BuildNo.ToSafeString();
                    }
                }
            }

            Session["ESRI_BYPASSCOVERAGEMODEL"] = model;

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            ViewBag.LabelFBBTR000 = GetGeneralScreenConfig();
            ViewBag.LabelFBBTR001 = GetCoverageScreenConfig();
            ViewBag.LabelFBBTR002 = GetDisplayPackageScreenConfig();
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.LabelFBBTR004 = GetSummaryScreenConfig();
            ViewBag.LabelFBBTR010 = GetVas_Select_Package_ScreenConfig();
            ViewBag.LabelFBBORV11 = GetVasPopUpScreenConfig();
            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.LabelFBBORV25 = GetSelectRouter();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.FbbException = GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
            ViewBag.Fbb_Service = GetDisplay_Select_Type_Service();
            ViewBag.Version = GetVersion();
            ViewBag.Vas = "";
            ViewBag.User = base.CurrentUser;
            ViewBag.ContentPlaybox = GetLovConfigBytype("NEW_CONTENT_PLAYBOX");
            var newmodel = new QuickWinPanelModel();
            var data = base.LovData
                .Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("B_CLOSE"))
                .FirstOrDefault();

            if (data != null)
            {
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    if (data.LovValue1 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue1.ToString();
                    }
                }
                else
                {
                    if (data.LovValue2 != null)
                    {
                        ViewBag.LCLOSE = data.LovValue2.ToString();
                    }
                }
            }
            else
            {
                ViewBag.LCLOSE = "";
            }
            ViewBag.CheckcoveragBypassflag = "true";
            newmodel.ClientIP = ipAddress;
            return View("New_SearchProfilePrePostPaid", newmodel);
        }

        public CoverageAreaResultModel GetCoverageAreaResultByResultID(string ResultID)
        {
            CoverageAreaResultModel data = new CoverageAreaResultModel();
            GetCoverageAreaResultByResultIDQuery query = new GetCoverageAreaResultByResultIDQuery
            {
                ResultID = ResultID
            };
            data = _queryProcessor.Execute(query);
            return data;
        }

        //R20.7 Add by Aware : Atipon > Check Pending Order FBSS
        private string GetCheckPendingOrderFbss(string non_mobile, string url)
        {
            var CheckPendingFlag = "";

            List<LovValueModel> CheckPendingOrderFbssFlag = base.LovData.Where(l => l.Name == "CHECK_PENDING_ORDER_FLAG" && l.Type == "CONFIG").ToList();

            if (CheckPendingOrderFbssFlag.Any())
            {
                CheckPendingFlag = CheckPendingOrderFbssFlag.Select(i => i.LovValue1).FirstOrDefault();
            }

            if (CheckPendingFlag == "Y")
            {
                var query = new CheckPendingOrderFbssQuery()
                {
                    InteretNo = non_mobile,
                    FullUrl = url
                };

                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    return result.PendingOrderFbss_Flag.ToSafeString();
                }
            }


            return "";
        }

        public string GetNumberPbox(string NonMobileNo, string IdCard)
        {
            int result = 0;
            var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
            {
                mobileNo = NonMobileNo,
                idCard = IdCard,
                FullUrl = ""
            };

            var data = _queryProcessor.Execute(query2);

            if (data != null)
            {
                result = data.v_number_of_pb_number;
            }
            return result.ToSafeString();
        }
    }

    public class ExistingLogMenu
    {
        public string internetNo { get; set; }
        public string mobileNo { get; set; }
        public string menu { get; set; }
    }
}