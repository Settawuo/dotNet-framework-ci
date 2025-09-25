using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;
using WBBSECURE = WBBBusinessLayer.Extension.Security;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class ProcessAddBundlingController : WBBController
    {
        //
        // GET: /ProcessAddBundling/

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CreateOrderChangePromotionDisCountCommand> _createOrderChangePromotionDisCountCommand;

        public ProcessAddBundlingController(IQueryProcessor queryProcessor,
            ICommandHandler<CreateOrderChangePromotionDisCountCommand> createOrderChangePromotionDisCountCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
            _createOrderChangePromotionDisCountCommand = createOrderChangePromotionDisCountCommand;
        }

        public ActionResult Index(string Data = "")
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("Index", "ProcessAddBundling", null, this.Request.Url.Scheme);

            if (Data != "")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                string NonMobileNo = "";
                string lang = "";
                if (DataTemps.Count() == 4)
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
                    ViewBag.NonMobileNo = NonMobileNo;
                }
                else
                {
                    // value in put ไม่ถูกต้อง
                }
            }

            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LanguagePage = "1"; }
            else
            { ViewBag.LanguagePage = "2"; }

            ViewBag.LabelFBBTR000 = GetScreenConfig("FBBOR000");
            ViewBag.LabelFBBTR001 = GetScreenConfig("FBBOR001");
            ViewBag.LabelFBBTR003 = GetScreenConfig("FBBOR003");
            ViewBag.LabelFBBOR015 = GetScreenConfig("FBBOR015");
            ViewBag.LabelFBBOR016 = GetScreenConfig("FBBOR016");
            ViewBag.LabelFBBTR016 = GetScreenConfig("FBBOR016");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");

            ViewBag.LabelFBBOR018 = GetScreenConfig("FBBOR018");

            return View();
        }

        [HttpPost]
        public ActionResult AddBundling(AddBundlingModels model)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("Index", "ProcessAddBundling", null, this.Request.Url.Scheme);

            if (SiteSession.CurrentUICulture.IsThaiCulture())
            { ViewBag.LanguagePage = "1"; }
            else
            { ViewBag.LanguagePage = "2"; }

            ViewBag.LabelFBBOR016 = GetScreenConfig("FBBOR016");
            ViewBag.LabelFBBTR016 = GetScreenConfig("FBBOR016");
            ViewBag.LabelLovScreen = GetScreenConfig("ALLPAGE");

            ViewBag.LabelFBBOR018 = GetScreenConfig("FBBOR018");

            return View(model);
        }

        [HttpPost]
        public JsonResult GetCheckRelate(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CHECKCOVERAGE_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;
            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                if (sffData.outMobileNumber != null && sffData.outMobileNumber.Length > 0)
                {
                    return Json(
                        new
                        {
                            data = new
                            {
                                status = "HaveRelate"
                            }
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(
                        new
                        {
                            data = new
                            {
                                status = "NoRelate"
                            }
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           status = "NoData"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ServiceQueryMassCommonAccountInfo(string mobileNo = "", string cardNo = "", string cardType = "", string line = "", string SubNetworkType = "")
        {
            // 17.6 Interface Log Add Url
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            string user = "";
            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            string InOption = line;

            mobileNo = DecryptStringAES(mobileNo, "fbbwebABCDEFGHIJ");
            if (cardNo != "" && cardType != "")
            {
                bool haveProfile = false;

                try
                {
                    var queryMassCommonAccountInfo = new evESeServiceQueryMassCommonAccountInfoQuery
                    {
                        inOption = "2",
                        inMobileNo = mobileNo,
                        inCardNo = cardNo,
                        inCardType = cardType,
                        Page = "ProcessAddBundling",
                        Username = user,
                        FullUrl = FullUrl
                    };
                    var massCommon = _queryProcessor.Execute(queryMassCommonAccountInfo);
                    if (massCommon.errorMessage == "")
                    {
                        haveProfile = true;
                    }

                }
                catch (Exception ex)
                {
                    return Json(
                        new
                        {
                            data = new
                            {
                                GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                                errorMessage = "No Profile"
                            }
                        }, JsonRequestBehavior.AllowGet);
                }


                if (haveProfile)
                {
                    var query = new evESeServiceQueryMassCommonAccountInfoForAddBundlingQuery
                    {
                        //inOption = "1",
                        inOption = InOption,//16.4 change use option2
                        inMobileNo = mobileNo,
                        inCardNo = cardNo,
                        inCardType = cardType,
                        Page = "Check Coverage",
                        Username = user,
                        FullUrl = FullUrl
                    };
                    var a = _queryProcessor.Execute(query);

                    if (line == "2")
                    {
                        var q = new WBBContract.Queries.WebServices.GetOwnerProductByNoQuery
                        {
                            No = mobileNo
                        };
                        var aa = _queryProcessor.Execute(q);
                        if (a.errorMessage.Trim() != "EB0001")
                        {
                            a.OwnerProduct = aa.Value;
                            a.PackageCode = aa.Value2;
                            a.outAddressId = aa.Value3;
                            a.outMobileSegment = a.outMobileSegment.ToUpper();
                        }
                    }

                    a.GUIDKEY = Guid.NewGuid().ToSafeString();
                    Session["CHECKCOVERAGE_SFFDATA"] = a;

                    return Json(
                        new
                        {
                            data = new
                            {
                                GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt(a.GUIDKEY),
                                errorMessage = a.errorMessage
                            }
                        }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(
                            new
                            {
                                data = new
                                {
                                    GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                                    errorMessage = "No Profile"
                                }
                            }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(
                new
                {
                    data = new
                    {
                        errorMessage = "No Profile"
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult evESQueryPersonalInformation(string mobileNo, string option)
        {
            // 17.6 Interface Log Add Url
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var query = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = mobileNo,
                    option = option,
                    FullUrl = FullUrl
                };
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                return Json(
                            new
                            {
                                data = new
                                {
                                    MainProductCd = "",
                                    status = "Error"
                                }
                            }, JsonRequestBehavior.AllowGet);
            }
            if (result != null && result.Count > 0)
            {
                string resultStatus = "";
                string MainProductCd = "";
                string resultStatusOnTop = "";
                string resultStatusOnTopExtra = "";
                int countProMain = 0;
                int countProOnTop = 0;
                int countProOnTopExtra = 0;

                List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData> tmpProMain = new List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData>();

                foreach (var item in result)
                {
                    List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData> ThisPromotions = GetSFFPromotionConfigBySFFPromotionCodeAndClassType(item.productCd, item.productClass);
                    if (ThisPromotions != null && ThisPromotions.Count > 0)
                    {
                        foreach (var ThisPromotion in ThisPromotions)
                        {
                            if (ThisPromotion.PROMOTION_CLASS == "Main")
                            {
                                tmpProMain.Add(ThisPromotion);
                                MainProductCd = item.productCd;
                                countProMain++;
                            }
                            else if (ThisPromotion.PROMOTION_CLASS == "On-Top")
                            {
                                countProOnTop++;
                            }
                            else if (ThisPromotion.PROMOTION_CLASS == "On-Top Extra")
                            {
                                countProOnTopExtra++;
                            }
                        }
                    }
                }

                if (tmpProMain != null && tmpProMain.Count > 1)
                {
                    DateTime tmStartDateMax = DateTime.Now;
                    int loopCount = 1;
                    foreach (var item in tmpProMain)
                    {
                        evESQueryPersonalInformationModel tmpProInSFF = result.FirstOrDefault(t => t.productCd == item.SFF_PROMOTION_CODE);
                        if (tmpProInSFF != null)
                        {
                            DateTime tmStartDate = DateTime.Now;
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                            System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");
                            if (DateTime.TryParseExact(tmpProInSFF.startDt, "dd/MM/yyyy HH:mm:ss", enUS, System.Globalization.DateTimeStyles.None, out tmStartDate))
                            {
                                if (loopCount == 1)
                                {
                                    tmStartDateMax = tmStartDate;
                                    MainProductCd = item.SFF_PROMOTION_CODE;
                                }
                                else
                                {
                                    if (tmStartDate > tmStartDateMax)
                                    {
                                        tmStartDateMax = tmStartDate;
                                        MainProductCd = item.SFF_PROMOTION_CODE;
                                    }
                                }
                            }
                        }
                        loopCount++;
                    }
                }

                if (countProMain == 0)
                {
                    resultStatus = "NoProMain";
                }
                if (countProOnTop > 0)
                {
                    resultStatusOnTop = "HaveOnTop";
                }
                if (countProOnTopExtra > 0)
                {
                    resultStatusOnTopExtra = "HaveOnTopExtra";
                }


                if (resultStatus == "NoProMain")
                {
                    return Json(
                            new
                            {
                                data = new
                                {
                                    MainProductCd = "",
                                    status = "NoProMain"
                                }
                            }, JsonRequestBehavior.AllowGet);
                }

                if (resultStatusOnTop == "HaveOnTop")
                {
                    return Json(
                            new
                            {
                                data = new
                                {
                                    MainProductCd = "",
                                    status = "HaveOnTop"
                                }
                            }, JsonRequestBehavior.AllowGet);
                }

                if (resultStatusOnTopExtra == "HaveOnTopExtra")
                {
                    return Json(
                          new
                          {
                              data = new
                              {
                                  MainProductCd = MainProductCd,
                                  status = "HaveOnTopExtra"
                              }
                          }, JsonRequestBehavior.AllowGet);
                }



                return Json(
                            new
                            {
                                data = new
                                {
                                    MainProductCd = MainProductCd,
                                    status = resultStatus

                                }
                            }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(
                            new
                            {
                                data = new
                                {
                                    MainProductCd = "",
                                    status = "NoProMain"
                                }
                            }, JsonRequestBehavior.AllowGet);
            }
        }

        public List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }
                //config = config.Where(a => a.Name == "L_DETAIL_DISCOUNT_SINGLE_BILL_1").ToList();
                var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
                var screenValue = new List<LovScreenValueModel>();
                if (langFlg == "TH")
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
                        DefaultValue = l.DefaultValue
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
                        DefaultValue = l.DefaultValue
                    }).ToList();
                }

                return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
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

        private string getPackageDescription(string lov_val)
        {
            var tmp = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("DESCRIPTION_BY_PACKAGE_GROUP") && l.Text.Equals(lov_val));
            if (tmp.Any())
            {
                if (base.GetCurrentCulture().IsThaiCulture())
                {
                    return tmp.FirstOrDefault().LovValue1;
                }
                else
                {
                    return tmp.FirstOrDefault().LovValue2;
                }
            }
            else
            {
                return "";
            }
        }

        private string GetLovByName(string LovName)
        {
            var tmp = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals(LovName));
            if (tmp.Any())
            {
                if (base.GetCurrentCulture().IsThaiCulture())
                {
                    return tmp.FirstOrDefault().LovValue1;
                }
                else
                {
                    return tmp.FirstOrDefault().LovValue2;
                }
            }
            else
            {
                return "";
            }
        }

        private List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData> GetSFFPromotionConfigBySFFPromotionCodeAndClassType(string p_SFF_PROMOTION_CODE, string p_PROMOTION_CLASS)
        {
            List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData> result = new List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData>();
            try
            {
                var query = new GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQuery()
                {
                    p_SFF_PROMOTION_CODE = p_SFF_PROMOTION_CODE,
                    p_PROMOTION_CLASS = p_PROMOTION_CLASS
                };
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        public string GetTablePackageOntopListByChange(string owner_product, string sff_promocode, string non_mobile, string address_id)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("Index", "ProcessAddBundling", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Column Header
            string L_DISCOUNT_PAGE_1 = "";
            string L_DISCOUNT_PAGE_2 = "";

            L_DISCOUNT_PAGE_1 = GetLovByName("L_DISCOUNT_PAGE_1");
            L_DISCOUNT_PAGE_2 = GetLovByName("L_DISCOUNT_PAGE_2");

            #endregion

            //string productSubtype = GetProcuctSubtypeByNonMobile(non_mobile);

            StringBuilder resulthtml = new StringBuilder();
            try
            {
                var query = new GetPackageOntopListBySFFPromoQuery()
                {
                    P_OWNER_PRODUCT = owner_product,
                    P_SFF_PROMOCODE = sff_promocode,
                    P_PRODUCT_SUBTYPE = "",
                    P_ADDRESS_ID = address_id,
                    FullUrl = FullUrl
                };

                var result = _queryProcessor.Execute(query);

                if (result != null && result.Any())
                {
                    #region PackageName & Description
                    string lblPkgGrp = string.Format("<h2><strong>{0}</strong></h2>", L_DISCOUNT_PAGE_1);
                    lblPkgGrp = lblPkgGrp + string.Format("<h4 style='margin-bottom: 0px;'>{0}</h4>", L_DISCOUNT_PAGE_1);

                    #endregion

                    string htmlTable = "<div class='panel panel-info'><div class='table-responsive'><table class='table table-hover'>{0}{1}</table></div></div>";
                    #region TableBody
                    string tableBody = "<tbody class='text-center promotionDetail2' style='font-size:18px;' id='dynamicRow0'>{0}</tbody>";
                    StringBuilder tblBodyContent = new StringBuilder();
                    var promotion_i = 1;
                    var selectbundling = "";
                    foreach (var detail in result)
                    {
                        if (promotion_i == 1)
                        {
                            selectbundling = "checked=''";
                        }
                        else
                        {
                            selectbundling = "";
                        }

                        string tblRow = "<tr style='cursor: pointer;' class='promotionDetail-table'>{0}</tr>";
                        string tblCell = "<th class='text-center'>" +
                            "<input name='SelectPackage' id='SelectPackage" + promotion_i.ToString() + "' " + selectbundling + " value='" + promotion_i.ToString() + "' type='radio'>" +
                            "<input id='MAPPING_CODE" + promotion_i.ToString() + "' value='" + detail.MAPPING_CODE + "' type='hidden'>" +
                            "<input id='PACKAGE_CODE" + promotion_i.ToString() + "' value='" + detail.PACKAGE_CODE + "' type='hidden'>" +
                            "<input id='PACKAGE_NAME" + promotion_i.ToString() + "' value='" + detail.TECHNOLOGY + "' type='hidden'>" +
                            "<input id='PACKAGE_GROUP" + promotion_i.ToString() + "' value='" + detail.PACKAGE_GROUP + "' type='hidden'>" +
                            "<input id='RECURRING_CHARGE" + promotion_i.ToString() + "' value='" + detail.RECURRING_CHARGE + "' type='hidden'>" +
                            "<input id='PRE_RECURRING_CHARGE" + promotion_i.ToString() + "' value='" + detail.PRE_RECURRING_CHARGE + "' type='hidden'>" +
                            "<input id='SFF_PROMOTION_BILL_THA" + promotion_i.ToString() + "' value='" + detail.SFF_PROMOTION_BILL_THA + "' type='hidden'>" +
                            "<input id='SFF_PROMOTION_BILL_ENG" + promotion_i.ToString() + "' value='" + detail.SFF_PROMOTION_BILL_ENG + "' type='hidden'>" +
                            "<input id='TECHNOLOGY" + promotion_i.ToString() + "' value='" + detail.TECHNOLOGY + "' type='hidden'>" +
                            "<input id='DOWNLOAD_SPEED" + promotion_i.ToString() + "' value='" + detail.DOWNLOAD_SPEED + "' type='hidden'>" +
                            "<input id='UPLOAD_SPEED" + promotion_i.ToString() + "' value='" + detail.UPLOAD_SPEED + "' type='hidden'>" +
                            "<input id='INITIATION_CHARGE" + promotion_i.ToString() + "' value='" + detail.INITIATION_CHARGE + "' type='hidden'>" +
                            "<input id='PRE_INITIATION_CHARGE" + promotion_i.ToString() + "' value='" + detail.PRE_INITIATION_CHARGE + "' type='hidden'>" +
                            "<input id='PACKAGE_TYPE" + promotion_i.ToString() + "' value='" + detail.PACKAGE_TYPE + "' type='hidden'>" +
                            "<input id='PRODUCT_SUBTYPE" + promotion_i.ToString() + "' value='" + detail.PRODUCT_SUBTYPE + "' type='hidden'>" +
                            "<input id='OWNER_PRODUCT" + promotion_i.ToString() + "' value='" + detail.OWNER_PRODUCT + "' type='hidden'>" +
                            "<input id='SFF_PROMOTION_CODE" + promotion_i.ToString() + "' value='" + detail.SFF_PROMOTION_CODE + "' type='hidden'>" +
                            "<input id='SEQ" + promotion_i.ToString() + "' value='" + detail.SEQ + "' type='hidden'>" +
                            "</th>" +
                            "<th>" +
                            "<strong>" + detail.TECHNOLOGY + "</strong>" +
                            "</th>";

                        tblRow = string.Format(tblRow, tblCell);
                        tblBodyContent.Append(tblRow);
                        promotion_i++;
                    }

                    tableBody = string.Format(tableBody, tblBodyContent);

                    #endregion

                    #region ColumnHeader
                    string tableHeader = "<thead class='text-center promotionTitle'>" +
                        "<tr style='background-image: linear-gradient(to bottom, #F5F5F5 0px, #E8E8E8 100%); background-repeat: repeat-x; border-top-left-radius: 3px; border-top-right-radius: 3px;'>" +
                        "<th class='text-center'><h4 style='margin-top:15px;'> &nbsp;</h4></th>" +
                        "<th><h4><strong>" + L_DISCOUNT_PAGE_1 + "</strong></h4></th>" +
                        "</tr></thead>";
                    #endregion

                    resulthtml.Append(lblPkgGrp + string.Format(htmlTable, tableHeader, tableBody) + "<p class='clearfix'></p>");

                }
                else
                {
                    resulthtml.Append("Nodata");
                }
                return resulthtml.ToString();
            }
            catch (Exception ex)
            {
                Logger.Info(ex.RenderExceptionMessage());
                resulthtml.Append("Nodata");
                return resulthtml.ToString();
            }

        }

        public string GetProcuctSubtypeByNonMobile(string NonMobile)
        {
            var result = "";
            var q = new WBBContract.Queries.WebServices.GetOwnerProductByNoQuery
            {
                No = NonMobile
            };
            var aa = _queryProcessor.Execute(q);
            if (aa != null)
            {
                result = aa.Value5;
            }

            return result;
        }

        public JsonResult CheckProfile(string internetNo = "", string idCardNo = "", string idCardType = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


            if ((idCardNo != "" && idCardType != ""))
            {
                var query = new evESeServiceQueryMassCommonAccountInfoQuery
                {
                    inOption = "2",
                    inMobileNo = internetNo,
                    inCardNo = idCardNo,
                    inCardType = idCardType,
                    Page = "CheckProfile",
                    Username = "",
                    ClientIP = ipAddress,
                    FullUrl = FullUrl
                };
                var a = _queryProcessor.Execute(query);
                if (a.outMobileSegment != null)
                {
                    a.outMobileSegment = a.outMobileSegment.ToUpper();
                }

                if (a.errorMessage == "")
                {
                    return Json(new { status = "Y" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "N" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "N" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string ConfirmChangBundling(string Mobile, string NonMobile, string ProductMainCD, string ProductOntopCD, string FlagDiscount)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("Index", "ProcessAddBundling", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var result = "";

            ChangePackageBundlingData changePackageBundlingData = new ChangePackageBundlingData();

            try
            {
                var query = new ChangePackageBundlingQuery()
                {
                    P_MOBILE = Mobile,
                    P_NON_MOBILE = NonMobile,
                    P_PRODUCT_MAIN_CD = ProductMainCD,
                    P_PROMOTION_ONTOP_CD = ProductOntopCD,
                    P_FLAG_DISCOUNT = FlagDiscount,
                    FullUrl = FullUrl
                };
                changePackageBundlingData = _queryProcessor.Execute(query);
                if (changePackageBundlingData != null && changePackageBundlingData.return_code == "0")
                {
                    if (SendToCreateOrderChangePromotion(changePackageBundlingData.ChangePackageBundlingList))
                    {
                        result = "Y";
                    }
                    else
                    {
                        result = "N";
                    }
                }
                else
                {
                    result = "N";
                }

            }
            catch (Exception ex)
            {
                result = "N";
            }

            return result;
        }

        private bool SendToCreateOrderChangePromotion(List<ChangePackageBundlingList> data)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("Index", "ProcessAddBundling", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            bool result = false;
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    var command = new CreateOrderChangePromotionDisCountCommand();

                    command.orderType = item.orderType;
                    command.mobileNo = item.mobileNo;
                    command.orderReson = item.orderReson;
                    command.orderChannel = item.orderChannel;
                    command.userName = item.userName;
                    command.chargeFeeFlag = item.chargeFeeFlag;
                    command.ascCode = item.ascCode;
                    command.locationCd = item.locationCd;
                    command.club900Mobile = item.club900Mobile;

                    command.projectName = item.projectName;
                    command.actionRelateMobile = item.actionRelateMobile;
                    command.relateMobileNo = item.relateMobileNo;
                    command.oldRelateMobile = item.oldRelateMobile;
                    command.referenceNo = item.referenceNo;
                    command.sourceSystem = item.sourceSystem;
                    command.mobileNumberContact = item.mobileNumberContact;

                    command.promotionCode = item.promotionCode;
                    command.actionStatus = item.actionStatus;
                    command.productSeq = item.productSeq;
                    command.promotionStartDt = item.promotionStartDt;
                    command.overRuleStartDate = item.overRuleStartDate;
                    command.waiveFlag = item.waiveFlag;
                    command.chargeType = item.chargeType;
                    command.orderItemReason = item.orderItemReason;
                    command.promotionClass = item.promotionClass;

                    // 17.6 Interface Log Add Url
                    command.FullUrl = FullUrl;

                    if (item.promotionCode_1 != null)
                    {
                        command.promotionCode_1 = item.promotionCode_1;
                        command.actionStatus_1 = item.actionStatus_1;
                        command.productSeq_1 = item.productSeq_1;
                        command.promotionStartDt_1 = item.promotionStartDt_1;
                        command.overRuleStartDate_1 = item.overRuleStartDate_1;
                        command.waiveFlag_1 = item.waiveFlag_1;
                        command.chargeType_1 = item.chargeType_1;
                        command.orderItemReason_1 = item.orderItemReason_1;
                        command.promotionClass_1 = item.promotionClass_1;
                    }

                    try
                    {
                        _createOrderChangePromotionDisCountCommand.Handle(command);
                        string ValidateFlag = command.validateFlag;
                        if (ValidateFlag == "Y")
                        {
                            result = true;
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return result;
        }

    }
}