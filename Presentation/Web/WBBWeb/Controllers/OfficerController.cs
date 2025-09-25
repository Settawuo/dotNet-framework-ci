using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBContract.QueryModels.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class OfficerController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public OfficerController(IQueryProcessor queryProcessor, ILogger logger, ICommandHandler<InterfaceLogCommand> intfLogCommand)
        {
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            base.Logger = logger;
        }

        public ActionResult Index()
        {
            Session["OfficerModel"] = null;
            Session["FullUrl"] = this.Url.Action("Index", "Officer", null, this.Request.Url.Scheme);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            ViewBag.labelFBBORV00 = GetScreenConfig("FBBORV00");
            ViewBag.LabelFBBTR003 = GetCustRegisterScreenConfig();
            ViewBag.IDSConfig = GetScreenConfig("OFFICER_IDS");
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            ViewBag.GetRedirectIDS = GetRedirectIDS().Select(s => s.SERVICE_PROVIDER_NAME);

            return View();

        }
        public List<LovScreenValueModel> GetScreenConfig(string page)
        {
            try
            {
                List<LovValueModel> config = null;

                config = base.LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN")
                        && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(page))).ToList();

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

        public List<LovScreenValueModel> GetCustRegisterScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
            return screenData;
        }

        private List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
            => base.LovData.Where(l => l.Type.Equals(fbbConstType))
                                       .Select(l => new FbbConstantModel
                                       {
                                           Field = l.Name,
                                           Validation = l.LovValue1,
                                           SubValidation = l.LovValue2
                                       }).ToList();

        [HttpPost]
        public JsonResult CheckSeibel(string LocationCode = "", string ASCCode = "")
        {
            Session["OfficerModel"] = null;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            string FullUrl = "";
            if (Session["FullUrl"] == null)
            {
                FullUrl = this.Url.Action("Index", "Officer", null, this.Request.Url.Scheme);
            }
            else
            {
                FullUrl = Session["FullUrl"].ToSafeString();
            }
            return GetSeibelData(LocationCode, ASCCode, FullUrl);
        }

        [HttpPost]
        public JsonResult GetSeibelData(string LocationCode = "", string ASCCode = "", string FullUrl = "")
        {
            string errorMsg = "";
            var _lov = base.LovData;
            string CALL_CCSM_FLAG = _lov.FirstOrDefault(t => t.Name == "CALL_CCSM_FLAG").LovValue1.ToSafeString();

            if (LocationCode != "")
            {
                if (ASCCode != "")
                {
                    var query = new GetSeibelInfoQuery()
                    {
                        ASCCode = ASCCode,
                        Inparam1 = "IVR",
                        Transaction_Id = ASCCode,
                        FullURL = FullUrl
                    };

                    var result = _queryProcessor.Execute(query);
                    errorMsg = result.outErrorMessage;
                    if (result.outLocationCode.ToSafeString() != "")
                    {
                        if (result.outLocationCode.ToSafeString() == LocationCode || (CALL_CCSM_FLAG == "Y" && result.outMemberCategory == "AIS FBB Sales Promoter"))
                        {
                            var query2 = new GetSeibelInfoQuery()
                            {
                                LocationCode = LocationCode,
                                Inparam1 = "",
                                Transaction_Id = LocationCode,
                                FullURL = FullUrl
                            };
                            var result2 = _queryProcessor.Execute(query2);
                            result2.outMobileNo = result.outMobileNo;
                            result2.outLocationEmailByRegion = CALL_CCSM_FLAG == "Y" ? result2.outSubRegion :
                                LovData.Where(l => l.Type == "FBB_CONSTANT"
                                                   && l.Name == "EMAIL_BY_REGION"
                                                   && l.LovValue1 == result2.outRegion
                                                   && l.LovValue2 == result2.outSubRegion)
                                    .Select(l => l.Text)
                                    .FirstOrDefault();
                            result2.outASCPartnerName = result.outASCPartnerName;
                            result2.outASCTitleThai = result.outASCTitleThai;
                            result2.outMemberCategory = result.outMemberCategory;
                            result2.outPosition = result.outPosition;
                            //R21.5 Pool Villa
                            result2.outLocationProvince = GetProvincePoolVilla(result2.addressLocationList);
                            return Json(result2, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var query2 = new GetSeibelInfoQuery()
                            {
                                LocationCode = LocationCode,
                                Inparam1 = "",
                                Transaction_Id = LocationCode,
                                FullURL = FullUrl
                            };
                            var result2 = _queryProcessor.Execute(query2);
                            if (!string.IsNullOrEmpty(result2.outLocationCode))
                            {
                                result2.outMobileNo = result.outMobileNo;
                                result2.outLocationEmailByRegion = CALL_CCSM_FLAG == "Y" ? result2.outSubRegion :
                                    LovData.Where(l => l.Type == "FBB_CONSTANT"
                                                       && l.Name == "EMAIL_BY_REGION"
                                                       && l.LovValue1 == result2.outRegion
                                                       && l.LovValue2 == result2.outSubRegion)
                                        .Select(l => l.Text)
                                        .FirstOrDefault();
                                result2.outASCPartnerName = result.outASCPartnerName;
                                result2.outASCTitleThai = result.outASCTitleThai;
                                result2.outMemberCategory = result.outMemberCategory;
                                result2.outPosition = result.outPosition;
                                //R21.5 Pool Villa
                                result2.outLocationProvince = GetProvincePoolVilla(result2.addressLocationList);
                                return Json(result2, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                errorMsg = "LocationCode from ASCCode not match your LocationCode";
                            }

                        }
                    }
                }
                else
                {

                    var query = new GetSeibelInfoQuery()
                    {
                        LocationCode = LocationCode,
                        Transaction_Id = LocationCode,
                        FullURL = FullUrl
                    };

                    var result = _queryProcessor.Execute(query);
                    result.outLocationEmailByRegion = CALL_CCSM_FLAG == "Y" ? result.outSubRegion :
                        LovData.Where(l => l.Type == "FBB_CONSTANT"
                                           && l.Name == "EMAIL_BY_REGION"
                                           && l.LovValue1 == result.outRegion
                                           && l.LovValue2 == result.outSubRegion)
                            .Select(l => l.Text)
                            .FirstOrDefault();
                    //R21.5 Pool Villa
                    result.outLocationProvince = GetProvincePoolVilla(result.addressLocationList);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


            }
            else if (LocationCode == "" && ASCCode != "")
            {
                var query = new GetSeibelInfoQuery()
                {
                    ASCCode = ASCCode,
                    Inparam1 = "IVR",
                    Transaction_Id = ASCCode,
                    FullURL = FullUrl
                };

                var result = _queryProcessor.Execute(query);
                errorMsg = result.outErrorMessage;
                if (result.outLocationCode.ToSafeString() != "")
                {
                    var query2 = new GetSeibelInfoQuery()
                    {
                        LocationCode = result.outLocationCode.ToSafeString(),
                        Inparam1 = "",
                        Transaction_Id = LocationCode,
                        FullURL = FullUrl
                    };
                    var result2 = _queryProcessor.Execute(query2);
                    result2.outMobileNo = result.outMobileNo;
                    result2.outLocationEmailByRegion = CALL_CCSM_FLAG == "Y" ? result.outSubRegion :
                        LovData.Where(l => l.Type == "FBB_CONSTANT"
                                           && l.Name == "EMAIL_BY_REGION"
                                           && l.LovValue1 == result2.outRegion
                                           && l.LovValue2 == result2.outSubRegion)
                            .Select(l => l.Text)
                            .FirstOrDefault();
                    result2.outASCPartnerName = result.outASCPartnerName;
                    result2.outASCTitleThai = result.outASCTitleThai;
                    result2.outMemberCategory = result.outMemberCategory;
                    result2.outPosition = result.outPosition;
                    return Json(result2, JsonRequestBehavior.AllowGet);
                }
            }

            var errormodel = new SeibelResultModel();
            errormodel.outStatus = "Error";
            errormodel.outErrorMessage = errorMsg;
            return Json(errormodel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void LogInInterfaceLog(string transactionId)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = "LoginOfficer",
                SERVICE_NAME = "LoginOfficer",
                IN_ID_CARD_NO = transactionId,
                IN_XML_PARAM = "",
                INTERFACE_NODE = "",
                CREATED_BY = "FBBWEB",
            };

            _intfLogCommand.Handle(dbIntfCmd);
        }

        private string GetProvincePoolVilla(List<SeibelAddressLocation> data)
        {
            string result = "";
            if (data != null)
            {
                foreach (var i in data)
                {
                    if (i.outAddressType.Equals("LOCATION_ADDR"))
                    {
                        if (!string.IsNullOrEmpty(i.outProvince))
                        {
                            result = i.outProvince.Replace("จังหวัด ", "");
                            return result;
                        }
                    }
                }
            }
            return result;
        }

        //R23.08 IDS Login officer
        public List<GetRedirectIDSQueryModel> GetRedirectIDS() => _queryProcessor.Execute(new GetRedirectIDSQuery());

        public ActionResult SubmitOfficerForm(string loginOption)
        {
            var queryIDS = GetRedirectIDS().Where(w => w.SERVICE_PROVIDER_NAME == loginOption).FirstOrDefault();
            Session["secIDS"] = queryIDS.CLIENT_SECRET;
            Session["urlIDS"] = queryIDS.FULL_URL_IDS;
            Session["FullUrl"] = this.Url.Action("Index", "Officer", null, this.Request.Url.Scheme);
            return Redirect(queryIDS.FULL_URL_IDS);
        }

        public ActionResult MenuOfficerAuthen(string code)
        {
            LogInInterfaceLog(code);
            ViewBag.IDSConfig = GetIDSConfig();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
            var url = Session["urlIDS"]?.ToString();
            var secret = Session["secIDS"]?.ToString();
            if (!string.IsNullOrEmpty(code))
            {
                ViewBag.IDSProfile = GetAccessToken(code, url, secret);
            }
            return View("MenuOfficerAuthen");
        }

        private List<LovScreenValueModel> GetIDSConfig()
            => base.LovData.Where(w => w.LovValue5 == "OFFICER_IDS")
                .Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = SiteSession.CurrentUICulture.IsThaiCulture() ? l.LovValue1 : l.LovValue2,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue
                }).ToList();

        //kong,Max IDS Webservice 23.08 GetAccessToken() and GetProfile()
        [HttpPost]
        public JsonResult GetAccessToken(string code, string url, string secret)
        {
            var result = new GetProfileQueryModel { error = "" };
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    result.error = "not_code";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var clientId = CutUri("client_id=", url);
                var callbackUrl = CutUri("redirect_uri=", url);
                var authorizationValue = GetAuthorizationValue(clientId, secret);
                var request = BuildAccessTokenQuery(authorizationValue, code, callbackUrl);
                var res = _queryProcessor.Execute(request);


                if (res != null && !string.IsNullOrEmpty(res.access_token))
                {
                    result = ProcessAccessToken(res.access_token);


                }
                else
                {
                    result.error = "no_token";
                }
            }
            catch (Exception ex)
            {
                result = HandleException(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string CutUri(string word, string url)
        {
            int startIdx = url.IndexOf(word) + word.Length;
            int endIdx = url.IndexOf(url.Contains('&') ? "&" : "%26", startIdx);
            string result = url.Substring(startIdx, endIdx == -1 ? url.Length - startIdx : endIdx - startIdx);
            return result;
        }

        private string GetAuthorizationValue(string clientId, string secret) => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

        private GetAccessTokenQuery BuildAccessTokenQuery(string authorization, string code, string callbackUrl)
            => new GetAccessTokenQuery
            {
                authorization = authorization,
                code = code,
                grant_type = "authorization_code",
                redirect_uri = callbackUrl
            };

        //private GetProfileQueryModel ProcessAccessToken(string accessToken) => CheckProfileIssue(GetProfile(accessToken));
        private GetProfileQueryModel ProcessAccessToken(string accessToken)
        {
            var checkprofileissue = GetProfile(accessToken);




            var vo_query = new GetUserProfileQuery
            {
                userName = checkprofileissue.username,
                authToken = accessToken
            };
            GetUserProfileQueryModel getlocationcode_vo = new GetUserProfileQueryModel();
            getlocationcode_vo = _queryProcessor.Execute(vo_query);
            
            if (getlocationcode_vo.aisEmployeeHierarchy != null)
            {
                if (getlocationcode_vo.aisEmployeeHierarchy[0].locationCode != null)
                {
                    checkprofileissue.location_code = getlocationcode_vo.aisEmployeeHierarchy[0].locationCode;
                }
                if (getlocationcode_vo.aisEmployeeHierarchy[0].ascCode != null)
                {
                    checkprofileissue.asc_code = getlocationcode_vo.aisEmployeeHierarchy[0].ascCode;
                }
            }




            var profileresult = CheckProfileIssue(checkprofileissue);

            var channel_sales_query = new GetChannelSalesCodeQuery();

            //Check if location code from ids and vo is null    #backoffice case
            if (getlocationcode_vo.aisEmployeeHierarchy != null)
            {
                    if (string.IsNullOrEmpty(checkprofileissue.location_code)
                 && string.IsNullOrEmpty(getlocationcode_vo.aisEmployeeHierarchy[0].locationCode))
                    {
                        profileresult.error = "is_backoffice";
                    }
            }
            else
            {
                if (string.IsNullOrEmpty(checkprofileissue.location_code))
                {
                    profileresult.error = "is_backoffice";
                }
            }

            // Check if location is alphabet letter not number    #backoffice case
            if (getlocationcode_vo.aisEmployeeHierarchy != null)
            {
                if (!string.IsNullOrEmpty(getlocationcode_vo.aisEmployeeHierarchy[0].locationCode))
                {
                    if (getlocationcode_vo.aisEmployeeHierarchy[0].locationCode.Any(x => char.IsLetter(x)))
                    {
                        profileresult.error = "is_backoffice";
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(checkprofileissue.location_code))
                {
                    if (checkprofileissue.location_code.Any(x => char.IsLetter(x)))
                    {
                        profileresult.error = "is_backoffice";
                    }

                }
            }
            // OutSource Case
            if (string.IsNullOrEmpty(profileresult.pincode) && string.IsNullOrEmpty(profileresult.asc_code) || checkprofileissue.pincode == "00000000")
            {
                profileresult.error = "is_outsource";
            }


            if (!string.IsNullOrEmpty(profileresult.pincode) && string.IsNullOrEmpty(profileresult.asc_code))
            {
                channel_sales_query.userType = "Staff";
            }
            if (!string.IsNullOrEmpty(profileresult.asc_code) && string.IsNullOrEmpty(profileresult.pincode))
            {
                channel_sales_query.userType = "Partner";
            }
            if (!string.IsNullOrEmpty(profileresult.pincode) && !string.IsNullOrEmpty(profileresult.asc_code))
            {
                channel_sales_query.userType = "Staff";
            }

            channel_sales_query.channelSalesCode = profileresult.channel_salescode;
            GetChannelSalesCodeQueryModel checkChannelSalesCode = new GetChannelSalesCodeQueryModel();
            //Check Special or Not Special
            checkChannelSalesCode = _queryProcessor.Execute(channel_sales_query);

            if (checkChannelSalesCode.is_special != null)
            {
                profileresult.is_special = checkChannelSalesCode.is_special;
            }

            return profileresult;
        }

        private GetProfileQueryModel GetProfile(string token) => _queryProcessor.Execute(new GetProfileQuery { authorization = token });

        private GetProfileQueryModel CheckProfileIssue(GetProfileQueryModel profile)
        {
            if (string.IsNullOrEmpty(profile?.username))
            {
                profile.error = "no_profile";
            }
            //if (string.IsNullOrEmpty(profile.location_code) || profile.location_code.Contains("null"))
            //{
            //    profile.error = "location_code_null";
            //}// waiting deploy 21sep23

            if (string.IsNullOrEmpty(profile.asc_code) || profile.asc_code.Contains("null"))
            {

                profile.asc_code = "";
            }
            else// waiting deploy 21sep23
            {
                if (string.IsNullOrEmpty(profile.location_code) || profile.location_code.Contains("null"))
                {
                    profile.error = "location_code_null";
                }
            }
            if (string.IsNullOrEmpty(profile.pincode) || profile.pincode.Contains("null"))
            {
                profile.pincode = "";
            }
            if (string.IsNullOrEmpty(profile.asc_code) && string.IsNullOrEmpty(profile.pincode))
            {
                profile.error = "disable";
            }
            if (string.IsNullOrEmpty(profile.error))
            {
                if (!string.IsNullOrEmpty(profile.pincode))
                {
                    var seibelJson = CheckSeibel(profile.location_code, profile.asc_code);
                    ViewBag.CheckSeibelIDS = seibelJson.Data;
                    var seibelData = seibelJson.Data as SeibelResultModel;
                    if (!string.IsNullOrEmpty(seibelData.outChnSalesCode))
                    {
                        profile.channel_salescode = seibelData.outChnSalesCode;
                    }
                    profile.error = "has_pincode";
                    //ViewBag.CheckSeibelIDS = Json(new SeibelResultModel(), JsonRequestBehavior.AllowGet).Data;// waiting deploy 21sep23
                }
                else// waiting deploy 21sep23
                {
                    var seibelJson = CheckSeibel(profile.location_code, profile.asc_code);
                    ViewBag.CheckSeibelIDS = seibelJson.Data;
                    var seibelData = seibelJson.Data as SeibelResultModel;
                    profile.error = "has_outLcCode";
                    if (!string.IsNullOrEmpty(seibelData.outChnSalesCode))
                    {
                        profile.channel_salescode = seibelData.outChnSalesCode;
                    }
                    if (!string.IsNullOrEmpty(seibelData?.outLocationCode))
                    {
                        profile.error = "has_outLcCode";
                        profile.error_description = seibelData.outLocationCode;
                        if (int.TryParse(seibelData.outLocationCode, out _))
                        {
                            profile.error = "has_NumLcCode";
                        }
                    }
                    if (!string.IsNullOrEmpty(seibelData.outPosition))
                    {
                        profile.error = "has_ascList";
                    }
                }
            }
            return profile;
        }

        private GetProfileQueryModel HandleException(Exception ex) => new GetProfileQueryModel { error = ex.Message, error_description = ex.Source };
        //endR23.08 IDS Login officer
        
    }

}
