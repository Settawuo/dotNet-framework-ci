using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigModels;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBWeb.Controllers.ConfigLovs;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class CampaignController : WBBController
    {
        // GET: /Campaign/
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveCampaignmessageCommand> _saveCampaignCommand;
        private readonly ICommandHandler<SaveMemberGetMemberCommand> _saveMemberGetMemberCommand;
        private ConfigLovHelpers _configLovHelpers;
        public CampaignController(IQueryProcessor queryProcessor
              , ICommandHandler<SaveCampaignmessageCommand> saveCampaignCommand
              , ICommandHandler<SaveMemberGetMemberCommand> saveMemberGetMemberCommand
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _saveCampaignCommand = saveCampaignCommand;
            _saveMemberGetMemberCommand = saveMemberGetMemberCommand;
            base.Logger = logger;
        }
        public ActionResult Neighbor()
        {
            var LovScreenFBBOR021 = GetScreenConfig("FBBOR021");
            ViewBag.LabelFBBOR021 = LovScreenFBBOR021;

            CampaignModel model = new CampaignModel();
            model.FRIENDS = new List<FriendModel>();
            int maxMember = 0;
            var totalMemberList = base.LovData.Where(l => l.LovValue5 == "FBBOR021" && l.Type == "FBB_CONSTANT" && l.Name == "L_NO_FRIENDS").ToList();
            if (totalMemberList != null && totalMemberList.Count > 0)
            {
                foreach (var item in totalMemberList)
                {
                    int tmpMaxMember = Convert.ToInt16(item.LovValue1);
                    if (maxMember < tmpMaxMember)
                    {
                        maxMember = tmpMaxMember;
                    }
                }

                for (int i = 0; i < maxMember; i++)
                {
                    var friendModel = new FriendModel();
                    model.FRIENDS.Add(friendModel);
                }
            }

            ViewBag.TotalMember = maxMember.ToString();

            if (LovScreenFBBOR021 != null && LovScreenFBBOR021.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR021.Any(c => c.Name == "L_REGISTER_AIS_FIBRE") ? LovScreenFBBOR021.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE").DisplayValue : "";
            }
            ViewBag.UrlRef = "/Campaign/Neighbor";
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            return View(model);
        }

        public JsonResult GetNoFriend()
        {
            var LovNoFriend = new List<DropdownModel>();
            try
            {
                var total = base.LovData
                        .Where(l => l.LovValue5 == "FBBOR021" & l.Type == "FBB_CONSTANT" & l.Name == "L_NO_FRIENDS")
                        .FirstOrDefault();

                if (total != null)
                {
                    int a = total.LovValue1.ToSafeInteger();
                    for (int i = 1; i <= a; i++)
                    {
                        DropdownModel t = new DropdownModel();
                        t.Text = i.ToSafeString();
                        t.Value = i.ToSafeString();
                        LovNoFriend.Add(t);
                    }
                }
                // LovNoFriend = GetDropDownConfig("FBBOR021", "FBB_CONSTANT", "L_NO_FRIENDS");
            }
            catch (Exception) { }

            return Json(LovNoFriend, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveRegister(CampaignModel Model)
        {
            string SaveStatus = "";
            string RefNo = "";
            string Language = "";

            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                Language = "T";
            }
            else
            {
                Language = "E";
            }
            List<FbbCampaignNeighborArrayModel> ListFr = new List<FbbCampaignNeighborArrayModel>();

            foreach (var item in Model.FRIENDS)
            {
                FbbCampaignNeighborArrayModel fr = new FbbCampaignNeighborArrayModel();
                fr.language = Language.ToSafeString();
                fr.service_speed = "";
                fr.cust_name = item.NAME.ToSafeString();
                fr.cust_surname = item.SURNAME.ToSafeString();
                fr.contact_mobile_no = item.CONTACT_MOBILE.ToSafeString();
                fr.is_ais_mobile = item.MOBILE_IS_AIS.ToSafeString();
                fr.contact_email = item.EMAIL.ToSafeString();
                fr.address_type = item.SELECT_ADDRESS_TYPE.ToSafeString();
                fr.building_name = item.BUILDING.ToSafeString();
                fr.house_no = item.HOUSE_NO.ToSafeString();
                fr.soi = item.SOI.ToSafeString();
                fr.road = item.ROAD.ToSafeString();
                fr.sub_district = item.SUB_DISTRICT.ToSafeString();
                fr.district = item.DISTRICT.ToSafeString();
                fr.province = item.PROVINCE.ToSafeString();
                fr.postal_code = item.POSTAL_CODE.ToSafeString();
                fr.contact_time = item.CONTACT_TIME.ToSafeString();

                ListFr.Add(fr);
            }

            var LovCampaignProjectName = GetLovList("FBB_CONSTANT", "CAMPAIGN_PROJECT_NAME").Where(l => l.Text == "NGN").FirstOrDefault();
            var FullUrl = this.Url.Action("neighbor", "campaign", null, this.Request.Url.Scheme);

            var Command = new SaveCampaignmessageCommand
            {
                p_referral_name = Model.EMP_NAME.ToSafeString(),
                p_referral_surname = Model.EMP_SURNAME.ToSafeString(),
                p_referral_staff_id = Model.EMP_CODE.ToSafeString(),
                p_referral_internet_no = Model.INTERNET_CODE.ToSafeString(),
                p_referral_contact_mobile_no = Model.MOBILE_CALLBACK.ToSafeString(),
                p_referral_neighbor_no = Model.TOTAL_FRIEND.ToSafeString(),
                p_rec_campaign_netghbor = ListFr,

                p_channal = LovCampaignProjectName.LovValue1.ToSafeString(),
                p_campaign = LovCampaignProjectName.LovValue2.ToSafeString(),
                p_url = FullUrl.ToSafeString()
            };
            _saveCampaignCommand.Handle(Command);
            if (Command.return_code == 0)
            {
                SaveStatus = "Y";
                RefNo = Command.return_msg;

            }
            else
            {
                SaveStatus = "N";
            }

            return Json(new { SaveStatus, RefNo }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContactTime()
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("FBBOR021", "FBB_CONSTANT", "L_CONTACT_TIME");
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetProvince()
        {
            var provType = new List<DropdownModel>();
            try
            {
                provType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(provType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAmphur(string provinceFilter)
        {
            var amphType = new List<DropdownModel>();
            try
            {
                /// เพิ่ม เอา ปณ ออก 01/07/2015
                amphType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter)) && (!z.Amphur.Contains("ปณ") && !z.Amphur.Contains("PO")))
                    .GroupBy(z => z.Amphur)
                    .Select(z =>
                    {
                        var item = z.First();

                        return new DropdownModel { Text = item.Amphur, Value = item.Amphur };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(amphType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTumbon(string provinceFilter, string amphurFilter)
        {
            var tumbType = new List<DropdownModel>();
            try
            {
                tumbType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter))
                                    && (string.IsNullOrEmpty(z.Amphur) || z.Amphur.Equals(amphurFilter)))
                    .GroupBy(z => z.Tumbon)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Tumbon, Value = item.Tumbon };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            //tumbType.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });

            return Json(tumbType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoverageZipCode(string provinceFilter, string amphurFilter, string tumbonFilter, string sso)
        {
            var zipCodeList = new List<DropdownModel>();
            try
            {
                var amphurToFilter = "";
                int index1 = amphurFilter.IndexOf('(');
                if (index1 > 0)
                {
                    int index2 = amphurFilter.IndexOf(')');
                    amphurToFilter = amphurFilter.Remove(index1, index2 - index1 + 1);
                }
                else
                {
                    amphurToFilter = amphurFilter;
                }

                // find zip code.
                zipCodeList = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.Equals(provinceFilter))
                        && (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.Contains(amphurToFilter))
                        && (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.Equals(tumbonFilter)))
                    .Select(z => new DropdownModel { Text = z.ZipCode, Value = z.ZipCodeId, })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(new { zipCodeList }, JsonRequestBehavior.AllowGet);
        }

        private List<DropdownModel> GetDropDownConfig(string pageCode, string type, string name)
        {
            return base.LovData
                        .Where(l => l.LovValue5 == pageCode & l.Type == type & l.Name == name)
                        .Select(l => new DropdownModel
                        {
                            Text = SiteSession.CurrentUICulture.IsThaiCulture() ? l.LovValue1 : l.LovValue2,
                            Value = SiteSession.CurrentUICulture.IsThaiCulture() ? l.LovValue1 : l.LovValue2,
                        })
                        .ToList();
        }

        private List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = base.LovData.Where(l => l.Type == "SCREEN").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }
                //config = config.Where(a => a.Name == "L_DETAIL_DISCOUNT_SINGLE_BILL_1").ToList();
                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
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

        private List<CoverageValueModel> GetCoverageValueModel(string zipCodeId, string province, string amphur, string sso)
        {
            try
            {
                var query = new GetCoverageAreaQuery
                {
                    CurrentCulture = SiteSession.CurrentUICulture,
                    ZipCodeId = zipCodeId,
                    Province = province,
                    Amphur = amphur,
                    SSO = sso
                };

                var result = _queryProcessor.Execute(query);
                return result;
            }
            catch (Exception) { }

            return new List<CoverageValueModel>();
        }

        private string IdCard = ""; //R22.06
        private string CardType = ""; //R22.06
        //R22.06
        private string GetInfoByNonMobileNo(string NonMobileNo = "")
        {
            var FullUrl = this.Url.Action("membergetgember", "campaign", null, this.Request.Url.Scheme);
            var user = (CurrentUser != null) ? CurrentUser.UserName : string.Empty;

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "2",
                inMobileNo = NonMobileNo,
                Page = "Member Get Member",
                Username = user,
                ClientIP = ipAddress,
                FullUrl = FullUrl
            };
            var a = _queryProcessor.Execute(query);
            if (a != null)
            {
                var CustomerData = GetCustomerInfo(a.outAccountNumber);
                if (CustomerData != null && CustomerData.idCardNum != "")
                {
                    IdCard = CustomerData.idCardNum;
                    CardType = CustomerData.idCardType;

                    return "";
                }
                else
                {
                    IdCard = "";
                    CardType = "";
                    return "NodataCustomer";
                }
            }
            else
            {
                return "NodataCustomer";
            }
        }

        //R22.06
        private evAMQueryCustomerInfoModel GetCustomerInfo(string accntNo = "")
        {
            var query = new evAMQueryCustomerInfoQuery
            {
                accntNo = accntNo
            };
            evAMQueryCustomerInfoModel a = _queryProcessor.Execute(query);

            return a;
        }

        public JsonResult CheckMobilePromotion(string mobile)
        {
            bool check = false;
            string msg = "";
            try
            {
                var query = new evOMQueryListServiceAndPromotionByNeighborQuery
                {
                    mobileNo = mobile
                };

                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    if (result.resultFlag == "Y")
                    {
                        var promotion_code = base.LovData.Where(x => x.Type == "FBB_CONSTANT" && x.Name == "L_PROMOTION_CODE" && x.LovValue5 == "FBBOR021").ToList();
                        foreach (var item in result.productCDContent)
                        {
                            if (promotion_code.Any(x => x.LovValue1.Contains(item)))
                            {
                                check = true;
                            }
                        }
                    }
                    else if (result.ErrorMessage == "{Service or Promotion} Not Found.")
                    {
                        msg = "L_VALID_PACKAGE";
                    }
                    else
                    {
                        msg = "L_VALID_INTERNET_NO";
                    }

                }

                if (check == false && msg == "")
                    msg = "L_VALID_PACKAGE";
            }
            catch (Exception) { }

            return Json(new { status = check, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        //17.7 Member get Member
        [HttpGet]
        public ActionResult MemberGetMember(string status, string Data = "")
        {
            string Language = "";
            //R22.06 By Pass MemberGetMember
            if (Data != "" && Data != "Officer")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                string NonMobileNo = "";
                string lang = "";
                string GetIdCardStatus = "";
                string timeStamp = "";
                string cardType = "";
                string cardNo = "";

                string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ClientIP))
                {
                    ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (DataTemps.Count() > 1)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                NonMobileNo = DataTemp[1].ToSafeString();
                                ViewBag.NonMobileNo = NonMobileNo;
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                lang = DataTemp[1].ToSafeString();
                                if (lang == "TH")
                                {
                                    Language = "1";
                                }
                                else
                                {
                                    Language = "2";
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "timeStamp")
                            {
                                timeStamp = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardType")
                            {
                                cardType = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardNo")
                            {
                                cardNo = DataTemp[1].ToSafeString();
                            }
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

                string TransactionID = NonMobileNo + ClientIP;

                //InterfaceLogCommand log = null;
                //log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Language: " + lang + "\r\n timeStamp: " + timeStamp, "/process/ChangePackagePromotion", TransactionID, "", "ChangePackagePromotionGET");

                //EndInterface("", log, TransactionID, "Success", "");

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = true;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "")
                    {
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
                    ViewBag.Language = "1";
                    ViewBag.PageShow = "LoginFail";
                }
            }

            //R22.06 Issue MGM
            var CheckLanguage = string.IsNullOrEmpty(Language) ? (SiteSession.CurrentUICulture.IsThaiCulture() ? "1" : "2") : Language;
            int intLanguage = Int16.Parse(CheckLanguage);
            SiteSession.CurrentUICulture = intLanguage;
            Session["CurrentUICulture"] = intLanguage;

            //R22.05 Modify Member get Member
            ViewBag.Language = CheckLanguage;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.LabelFBBOR022 = GetScreenConfig("FBBOR022");
            ViewBag.LabelFBBOR021 = GetScreenConfig("FBBOR021");
            ViewBag.LabelFBBTR001 = GetScreenConfig(WebConstants.LovConfigName.CoveragePageCode);
            ViewBag.LabelFBBTR003 = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
            ViewBag.LabelFBBOR015 = GetScreenConfig(WebConstants.LovConfigName.CheckPrePostPaid);
            ViewBag.LabelFBBOR019 = GetScreenConfig(WebConstants.LovConfigName.PaymentPageCode);

            #region member
            var member = new MemberGetMemberModel();
            var totalMember = base.LovData.FirstOrDefault(l => l.Type.Equals("FBB_CONSTANT") && l.Name.Equals("NEIGHBOR_TOTAL")) ?? new LovValueModel();
            if (!string.IsNullOrEmpty(totalMember.Text))
            {
                var total = Convert.ToInt16(totalMember.Text);
                for (int i = 0; i < total; i++)
                {
                    var cust = new CustomerNeighbor();
                    member.CustomerNeighborList.Add(cust);
                }
            }
            ViewBag.TotalMember = totalMember.Text;
            #endregion

            #region expriedDate
            var lovExpriedDate = base.LovData.FirstOrDefault(l => l.Type.Equals("FBB_CONSTANT") && l.Name.Equals("MEMBER_GET_MEMBER_EXPIRED_DATE"));
            var expriedDate = lovExpriedDate != null ? lovExpriedDate.Text : string.Empty;
            var expried = false;
            if (!string.IsNullOrEmpty(expriedDate))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                DateTime date = DateTime.ParseExact(expriedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (DateTime.Now.Date >= date.Date) expried = true;
            }

            ViewBag.Expried = expried;
            #endregion

            return View(member);
        }
        //R22.06 By Pass MemberGetMember
        [HttpPost]
        public ActionResult MemberGetMember(string Data = "")
        {
            string Language = "";
            //R22.06 By Pass MemberGetMember
            if (Data != "" && Data != "Officer")
            {
                bool CheckInput = true;
                string DataDec = Decrypt(Data);
                string[] DataTemps = DataDec.Split('&');
                string NonMobileNo = "";
                string lang = "";
                string GetIdCardStatus = "";
                string timeStamp = "";
                string cardType = "";
                string cardNo = "";

                string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ClientIP))
                {
                    ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (DataTemps.Count() > 1)
                {
                    foreach (var item in DataTemps)
                    {
                        string[] DataTemp = item.Split('=');
                        if (DataTemp != null && DataTemp.Count() == 2)
                        {
                            if (DataTemp[0].ToSafeString() == "mobileNo")
                            {
                                NonMobileNo = DataTemp[1].ToSafeString();
                                ViewBag.NonMobileNo = NonMobileNo;
                            }
                            if (DataTemp[0].ToSafeString() == "lang")
                            {
                                lang = DataTemp[1].ToSafeString();
                                if (lang == "TH")
                                {
                                    Language = "1";
                                }
                                else
                                {
                                    Language = "2";
                                }
                            }
                            if (DataTemp[0].ToSafeString() == "timeStamp")
                            {
                                timeStamp = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardType")
                            {
                                cardType = DataTemp[1].ToSafeString();
                            }
                            if (DataTemp[0].ToSafeString() == "cardNo")
                            {
                                cardNo = DataTemp[1].ToSafeString();
                            }
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

                string TransactionID = NonMobileNo + ClientIP;

                //InterfaceLogCommand log = null;
                //log = StartInterface("DataEncrypt: " + Data + "\r\n NonMobileNo: " + NonMobileNo + "\r\n Language: " + lang + "\r\n timeStamp: " + timeStamp, "/process/ChangePackagePromotion", TransactionID, "", "ChangePackagePromotionGET");

                //EndInterface("", log, TransactionID, "Success", "");

                if (cardType == "" || cardNo == "")
                {
                    CheckInput = true;
                }
                else
                {
                    List<FbbConstantModel> cardtypeDatas = GetFbbConstantModel("ID_CARD_TYPE");
                    if (cardtypeDatas != null && cardtypeDatas.Count > 0)
                    {
                        List<FbbConstantModel> cardtypeData = cardtypeDatas.Where(t => t.Field == cardType).ToList();
                        if (cardtypeData == null || cardtypeData.Count == 0)
                        {
                            CheckInput = false;
                        }
                    }
                }

                if (CheckInput)
                {
                    GetIdCardStatus = GetInfoByNonMobileNo(NonMobileNo);
                    if (GetIdCardStatus == "")
                    {
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
                    ViewBag.Language = "T";
                    ViewBag.PageShow = "LoginFail";
                }
            }

            //R22.06 Issue MGM
            var CheckLanguage = string.IsNullOrEmpty(Language) ? (SiteSession.CurrentUICulture.IsThaiCulture() ? "1" : "2") : Language;
            int intLanguage = Int16.Parse(CheckLanguage);
            SiteSession.CurrentUICulture = intLanguage;
            Session["CurrentUICulture"] = intLanguage;

            //R22.05 Modify Member get Member
            ViewBag.Language = CheckLanguage;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.LabelFBBOR022 = GetScreenConfig("FBBOR022");
            ViewBag.LabelFBBOR021 = GetScreenConfig("FBBOR021");
            ViewBag.LabelFBBTR001 = GetScreenConfig(WebConstants.LovConfigName.CoveragePageCode);
            ViewBag.LabelFBBTR003 = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
            ViewBag.LabelFBBOR015 = GetScreenConfig(WebConstants.LovConfigName.CheckPrePostPaid);
            ViewBag.LabelFBBOR019 = GetScreenConfig(WebConstants.LovConfigName.PaymentPageCode);

            #region member
            var member = new MemberGetMemberModel();
            var totalMember = base.LovData.FirstOrDefault(l => l.Type.Equals("FBB_CONSTANT") && l.Name.Equals("NEIGHBOR_TOTAL")) ?? new LovValueModel();
            if (!string.IsNullOrEmpty(totalMember.Text))
            {
                var total = Convert.ToInt16(totalMember.Text);
                for (int i = 0; i < total; i++)
                {
                    var cust = new CustomerNeighbor();
                    member.CustomerNeighborList.Add(cust);
                }
            }
            ViewBag.TotalMember = totalMember.Text;
            #endregion

            #region expriedDate
            var lovExpriedDate = base.LovData.FirstOrDefault(l => l.Type.Equals("FBB_CONSTANT") && l.Name.Equals("MEMBER_GET_MEMBER_EXPIRED_DATE"));
            var expriedDate = lovExpriedDate != null ? lovExpriedDate.Text : string.Empty;
            var expried = false;
            if (!string.IsNullOrEmpty(expriedDate))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                DateTime date = DateTime.ParseExact(expriedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (DateTime.Now.Date >= date.Date) expried = true;
            }

            ViewBag.Expried = expried;
            #endregion

            return View(member);
        }

        [HttpGet]
        public JsonResult GetPathBannerPicture()
        {
            string pathpicture = "";
            string path = "";
            try
            {
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    path = "/Content/images/postpaid/new_mgm_banner_th.jpg";
                }
                else
                {
                    path = "/Content/images/postpaid/new_mgm_banner_en.jpg";
                }
                //var result = GetScreenConfig("FBBOR022");
                //var path = result.Where(c => c.Name == lov_name).Select(p => p.DisplayValueJing).First();
                pathpicture = path.ToSafeString().Trim();

            }
            catch (Exception ex)
            { }
            return Json(pathpicture, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SubmitMemberGetMember(MemberGetMemberModel model)
        {
            string saveStatus = "N";
            string refNo = "";

            try
            {
                var listFr = new List<MemberGetMemberArrayModel>();
                foreach (var item in model.CustomerNeighborList)
                {
                    var fr = new MemberGetMemberArrayModel
                    {
                        language = model.Language.ToSafeString(),
                        cust_name = item.Name.ToSafeString(),
                        cust_surname = item.Surname.ToSafeString(),
                        contact_mobile_no = item.ContactNo.ToSafeString(),
                        is_ais_mobile = item.IsAisMobile.ToSafeString(),
                        contact_email = string.Empty, //item.Email.ToSafeString(),
                        address_type = item.SelectAddressType.ToSafeString(),
                        building_name = item.Building.ToSafeString(),
                        house_no = item.HouseNo.ToSafeString(),
                        soi = item.Soi.ToSafeString(),
                        road = item.Road.ToSafeString(),
                        sub_district = item.SubDistrict.ToSafeString(),
                        district = item.District.ToSafeString(),
                        province = item.Province.ToSafeString(),
                        postal_code = item.PostalCode.ToSafeString(),
                        contact_time = item.ContactTime.ToSafeString(),
                        lineId = string.Empty, //item.LineId.ToSafeString(),
                        voucher_desc = item.Rewards.ToSafeString(),
                        full_address = string.Empty //item.FullAddress.ToSafeString()
                    };

                    listFr.Add(fr);
                }

                var LovCampaignProjectName = GetLovList("FBB_CONSTANT", "CAMPAIGN_PROJECT_NAME").Where(l => l.Text == "MGM").FirstOrDefault();
                var FullUrl = this.Url.Action("membergetgember", "campaign", null, this.Request.Url.Scheme);

                var command = new SaveMemberGetMemberCommand
                {
                    p_referral_name = model.NeighborName.ToSafeString(),
                    p_referral_surname = model.NeighborSurname.ToSafeString(),
                    p_referral_internet_no = model.InternetNo.ToSafeString(),
                    p_referral_contact_mobile_no = model.MobileNo.ToSafeString(),
                    p_referral_neighbor_no = model.TotalFriend.ToSafeString(),
                    p_rec_campaign_mgm = listFr,

                    p_channal = LovCampaignProjectName.LovValue1.ToSafeString(),
                    p_campaign = LovCampaignProjectName.LovValue2.ToSafeString(),
                    p_url = FullUrl.ToSafeString()
                };
                _saveMemberGetMemberCommand.Handle(command);
                if (command.return_code == 0)
                {
                    saveStatus = "Y";
                    refNo = command.return_msg;

                }
            }
            catch (Exception)
            {

            }

            return Json(new { saveStatus, refNo }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetContactTimeNeighbor()
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig("FBBOR022", "FBB_CONSTANT", "NEIGHBOR_CONTACT_TIME");
            }
            catch (Exception) { }

            return Json(CONTACT, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRewardsNeighbor()
        {
            var rewards = new List<DropdownModel>();
            try
            {
                rewards = GetDropDownConfig("FBBOR022", "FBB_CONSTANT", "NEIGHBOR_REWARDS");
            }
            catch (Exception)
            { }

            return Json(rewards, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckInternetNumber(string internetNo)
        {
            string check = "0";
            string mobileNumber = "";
            string mobileNumberShow = "";
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "4",
                inMobileNo = internetNo,
                inCardNo = string.Empty,
                inCardType = string.Empty,
                Page = "Member Get Member",
                Username = string.Empty,
                FullUrl = string.Empty
            };

            var resultQueryMassCommonAccountInfo = _queryProcessor.Execute(query);

            if ((resultQueryMassCommonAccountInfo != null) &&
                (string.IsNullOrEmpty(resultQueryMassCommonAccountInfo.outErrorMessage)))
            {
                check = "1";
                mobileNumber = resultQueryMassCommonAccountInfo.outServiceMobileNo;
                mobileNumberShow = !string.IsNullOrEmpty(resultQueryMassCommonAccountInfo.outServiceMobileNo) ?
                    resultQueryMassCommonAccountInfo.outServiceMobileNo.Substring(0, 3)
                    + "-xxx-" + resultQueryMassCommonAccountInfo.outServiceMobileNo.Substring(6, 4) : "";
            }

            return Json(new { check, mobileNumber, mobileNumberShow }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string custInternetNum, string custIdCardNum, string custIdCardType)
        {
            //R22.05 Modify Member get Member
            var FullUrl = this.Url.Action("membergetgember", "campaign", null, this.Request.Url.Scheme);
            var user = (CurrentUser != null) ? CurrentUser.UserName : string.Empty;

            var loginStatus = "0";
            var fullname = "";
            var firstName = "";//R22.05
            var lastName = "";//R22.05
            var outServiceMobileNo = "";

            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "2",
                inMobileNo = custInternetNum,
                inCardNo = custIdCardNum,
                inCardType = custIdCardType,
                Page = "Member Get Member",
                Username = user,
                FullUrl = FullUrl
            };

            var resultQueryMassCommonAccountInfo = _queryProcessor.Execute(query);

            if ((resultQueryMassCommonAccountInfo != null) &&
                (string.IsNullOrEmpty(resultQueryMassCommonAccountInfo.outErrorMessage)))
            {
                loginStatus = "1";
                fullname = resultQueryMassCommonAccountInfo.outAccountName ?? "";
                firstName = resultQueryMassCommonAccountInfo.outPrimaryContactFirstName ?? ""; //R22.05
                lastName = resultQueryMassCommonAccountInfo.outContactLastName ?? ""; //R22.05
            }

            if (loginStatus == "1")
            {
                var queryMass = new evESeServiceQueryMassCommonAccountInfoQuery
                {
                    inOption = "4",
                    inMobileNo = custInternetNum,
                    inCardNo = string.Empty,
                    inCardType = string.Empty,
                    Page = "Member Get Member",
                    Username = user,
                    FullUrl = FullUrl
                };

                var resultQueryMassCommonAccountInfo2 = _queryProcessor.Execute(queryMass);
                if ((resultQueryMassCommonAccountInfo2 != null) &&
                (string.IsNullOrEmpty(resultQueryMassCommonAccountInfo2.outErrorMessage)))
                {
                    outServiceMobileNo = resultQueryMassCommonAccountInfo2.outServiceMobileNo;
                }

            }

            return Json(new { loginStatus, firstName, lastName, fullname, outServiceMobileNo }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNeighbor(string language, string internetNo)
        {
            //R22.05 Modify Member get Member
            var status = "0";

            try
            {
                var query = new GetMemberGetMemberQuery
                {
                    Language = language,
                    InternetNo = internetNo
                };
                var resultQuery = _queryProcessor.Execute(query) ?? new List<MemberGetMemberStatus>();

                if (resultQuery.Count > 0)
                {
                    status = "1";
                    var result = resultQuery.Select(x => x.referral_contact_mobile_no).ToList().Distinct();
                }

                return Json(new { status, member = resultQuery }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status, member = new List<MemberGetMemberStatus>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetSummaryAmountCallIM(string language, string internetNo)
        {
            //R22.06 MGM Call New WS IM Show Summary Amount Privileges
            var FullUrl = this.Url.Action("membergetmember", "campaign", null, this.Request.Url.Scheme);
            string ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ClientIP))
            {
                ClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            string TransactionID = internetNo + ClientIP;

            var outSummaryAmount = "";

            try
            {
                var LovQueryOption = base.LovData.Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "Query_Option")).Select(s => s.LovValue1).FirstOrDefault();
                var LovChildCampaignCode = base.LovData.Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "ChildCampaignCode")).Select(s => s.LovValue1).FirstOrDefault();
                var LovInParameter1 = base.LovData.Where(l => (l.Type == "FBB_CONSTANT" && l.Name == "InParameter1")).Select(s => s.LovValue1).FirstOrDefault();

                var query = new GetIMQueryCampaignContactQuery
                {
                    TransactionId = TransactionID,
                    FullUrl = FullUrl,
                    p_queryoption = LovQueryOption,
                    p_mobilenumber = internetNo.ToSafeString(),
                    p_childcampaigncode = LovChildCampaignCode,
                    p_inparameter1 = LovInParameter1
                };

                var resultQuery = _queryProcessor.Execute(query);

                if (resultQuery != null)
                {
                    var query2 = new GetDetailMemberGerMemberQuery
                    {
                        p_internet_no = internetNo.ToSafeString(),
                        p_values1 = resultQuery.VALUE_1.ToSafeString(),
                        p_ContactListInfo = resultQuery.CONTACT_LIST_INFO.ToSafeString(),
                        p_language = language.ToSafeString()
                    };

                    var resultQuery2 = _queryProcessor.Execute(query2);

                    if (resultQuery2 != null)
                        outSummaryAmount = resultQuery2.detail.ToSafeString();
                }

                return Json(new { outSummaryAmount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { outSummaryAmount }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Logout()
        {
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        struct MyObj
        {
            public string output1 { get; set; }
            public string output2 { get; set; }
            public string output3 { get; set; }
            public string output4 { get; set; }
            public string output5 { get; set; }
        }

        [HttpPost]
        public JsonResult CheckEmpCode(string empCode = "")
        {
            GetOfficerInfoModel result = new GetOfficerInfoModel();
            //string result2Query = "";
            //string URL = "http://emmobile.ais.co.th/api/fbb/profilefbbon90day";
            //string X_Token = "N=fbbon90day";
            //string urlParameters = "?Key1=";
            try
            {
                GetOfficerInfoQuery query = new GetOfficerInfoQuery()
                {
                    EMP_CODE = empCode
                };

                #region R24.10 Inactive OM Service

                ////Before Develop R24.10 Inactive OM Service
                //GetEmployeeProfileByPINModel result1Query = _queryProcessor.Execute(query);
                //result.result1 = result1Query.NewDataSet.Table.USERID;
                //result.THFirstName = result1Query.NewDataSet.Table.THFIRSTNAME;
                //result.THLastName = result1Query.NewDataSet.Table.THLASTNAME;

                ////After Develop R24.10 Inactive OM Service
                result = new GetOfficerInfoModel();

                #endregion R24.10 Inactive OM Service

                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(URL);
                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-Token", X_Token);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //var content = new FormUrlEncodedContent(new[]
                //    {
                //        new KeyValuePair<string, string>("Key1", empCode.PadLeft(8, '0'))
                //    });


                //var resultData = client.PostAsync(urlParameters, content).Result;
                //if (resultData.IsSuccessStatusCode)
                //{
                //    string resultContent = resultData.Content.ReadAsStringAsync().Result;
                //    var resultObj = JsonConvert.DeserializeObject<MyObj>(resultContent);
                //    if (resultObj.output1 == "000")
                //    {
                //        result1Query = "Y";
                //        result2Query = resultObj.output5;
                //    }
                //}

            }
            catch (Exception ex)
            {
                result = new GetOfficerInfoModel();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<LovValueModel> GetLovList(string type, string name)
        {
            Logger.Info("Get parameter from Lov. " + name);
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

    }
}
