using FBBConfig.Extensions;
using FBBConfig.Models;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.Account;

namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class AccountController_Bk : FBBConfigController
    {
        //
        // GET: /Account/Login
        private readonly IQueryProcessor _QueryProcessor;
       

        public AccountController_Bk(
            //ILDAPQueryProcessor ldapQueryProcessor,
            //ISSOQueryProcessor ssoQueryProcessor,
            IQueryProcessor queryProcessor,
            ILogger logger)
        {
            //_SSOQueryProcessor = ssoQueryProcessor;
            //_LDAPQueryProcessor = ldapQueryProcessor;
            _QueryProcessor = queryProcessor;
            base._Logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        [CustomActionFilter(LogType = "LogOnBySSO")]
        public ActionResult LogOnBySSO()
        {
            try
            {
                var ssoData = HttpContext.Request.Form;
                var ssoFields = LoadSSOFieldsFromPostData(ssoData);
                _Logger.Info(ssoFields.Token);
                _Logger.Info(ssoFields.UserName);

                //get profile
                _Logger.Info("Get User Model");
                var authenticatedUser = GetUser(ssoFields.UserName);
                if (null != authenticatedUser)
                {
                    authenticatedUser.AuthenticateType = AuthenticateType.SSO;
                    authenticatedUser.SSOFields = ssoFields;

                    base.CurrentUser = authenticatedUser;
                    //_Logger.Info("Get User Program Model");
                    //base.UserProgramPermission = GetUserProgramModel();                    
                    Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _Logger.Info("cannot log on by using sso.");
                    base.CurrentUser = new UserModel();
                    base.CurrentUser.AuthenticateType = AuthenticateType.SSO;

                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Session.Abandon();
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                _Logger.Info(ex.StackTrace);
            }

            return RedirectToAction("Logout", "Account");
        }


        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(string returnUrl)
        {
            //try
            //{
            //    var query = new SelectLovQuery
            //    {
            //        LOV_TYPE = "CONFIG_TEMP_LOGIN_PAGE"
            //    };
            //    var data = _QueryProcessor.Execute(query);

            //    //_Logger.Info(data.DumpToString("TemporaryLogin"));

            //    string flg = "N";
            //    if (data.Any())
            //        flg = data.FirstOrDefault().LOV_VAL1;

            //    if (flg == "Y")
            //    {
            //        ViewBag.ReturnUrl = returnUrl;
            //return View();
            //    }
            //    else
            //    {
            //        ViewBag.ReturnUrl = returnUrl;
            //        return RedirectToAction("TemporaryNotAvailable", "Account");
            //    }
            //    //if (data.Count < 0 || (bool)data[0] != true)
            //    //{
            //    //    ViewBag.ReturnUrl = returnUrl;
            //    //    return RedirectToAction("TemporaryNotAvailable", "Account");
            //    //}
            //    //else
            //    //{
            //    //    ViewBag.ReturnUrl = returnUrl;
            //    //    return View();
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    _Logger.Info(ex.GetErrorMessage());
            //    _Logger.Info(ex.StackTrace);

            //    return RedirectToAction("TemporaryNotAvailable", "Account");
            //}

            GetToken();



            return View();
        }


        #region login ids

        public void GetToken()
        {
            var url = $"{Configurations.IDS_URL}/oauth2/authorize";
            var request = new ApiClient(url);
            //request.Get<string>($"?client_id={Configurations.IDS_CLIENT_ID}&scope=openid&response_type=code&redirect_uri={Request.Url}{Configurations.IDS_Callback}");

        }



        public string GetProfile(string accessToken)
        {
            try
            {
                var url = $"{Configurations.IDS_URL}/oauth2/userinfo";
                var request = new ApiClient(url);

                var accessTokenHeader = new AccessTokenModel()
                {
                    scope = "openid",
                    access_token = accessToken,
                    id_token = accessToken,
                    token_type = "Bearer",
                    refresh_token = accessToken,
                    expires_in = 3600 // Assuming 1 hour expiration for the token

                };



                //var response = request.Post(accessToken);
                //return response;
                return "";
            }
            catch (Exception ex)
            {
                _Logger.Error("Error in GetProfile: " + ex.GetErrorMessage());
                return null;
            }
        }



        //Account/LogOnByIDS?code=e46598d3-cef9-3380-b68d-c9870c4e7407&session_state=8e593e4b225ad85cc42eba5e53bb5cf04070a4785c2484ea684be2f32d91c217.or5WeD-36cz9_fL-rISq4Q
        [HttpGet, Route("Account/LogOnByIDS")]
        //[AllowAnonymous]
        [CustomActionFilter(LogType = "LogOnByIDS")]
        public async Task<ActionResult> LogOnByIDS(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Home");
            }
                
            else
            {
                _Logger.Info("LogOnByIDS code: " + code);

                return RedirectToAction("Index", "Home");
            }
               
                





            //try
            //{
            //    if (string.IsNullOrEmpty(code))
            //    {
            //        return RedirectToAction("Index", "Employee");
            //    }
            //    #region IDS_Setting
            //    _Logger.Info("Start AccountController >> LogOnByIDS");
            //    var _httpRequest = _httpContextAccessor.HttpContext.Request;
            //    string _codeReq = _httpRequest.QueryString.ToSafeString();
            //    var Temp = _codeReq.Split('?');
            //    var Temp_Split = Temp[1].Split('&');
            //    var IDS_Code = code;

            //    string grant_type = string.Empty;
            //    string redirect_uri = string.Empty;

            //    string IDS_CLIENT_ID = _configuration["IDS_CLIENT_ID"];
            //    string IDS_CLIENT_SECRET = _configuration["IDS_CLIENT_SECRET"];
            //    string IDS_CLIENT = IDS_CLIENT_ID + ":" + IDS_CLIENT_SECRET;

            //    string IDS_Callback = _configuration["IDS_Callback"];
            //    string IDS_Grant_Type = _configuration["IDS_Grant_Type"];
            //    #endregion

            //    try
            //    {
            //        var IDS_CLIENTBytes = Encoding.UTF8.GetBytes(IDS_CLIENT);
            //        var IDS_Authorization = Convert.ToBase64String(IDS_CLIENTBytes);
            //        string ContentType = _configuration["IDS_Content_Type"];// base.GetApplicationConstantValueByConstantName(WebConstants.PMS.ADMDReqContentType);
            //        string Authorization = "Basic " + IDS_Authorization;// base.GetApplicationConstantValueByConstantName(WebConstants.PMS.BasicAuthenLoginWaitingList);

            //        var headerListData = new List<ListHeaderSendAPIModel>();
            //        headerListData.Add(new ListHeaderSendAPIModel()
            //        {
            //            key = WebConstants.PMS.ContentType,
            //            value = ContentType
            //        });

            //        headerListData.Add(new ListHeaderSendAPIModel()
            //        {
            //            key = WebConstants.PMS.Authorization,
            //            value = Authorization
            //        });

            //        AccessTokenModel model = new AccessTokenModel();

            //        model.grant_type = IDS_Grant_Type;
            //        model.code = code;
            //        model.redirect_uri = IDS_Callback;

            //        #region Access_Token_ENDPOINT
            //        string IDS_ENDPOINT = _configuration["IDS_ENDPOINT"];
            //        string IDS_TOKEN = _configuration["IDS_TOKEN"];
            //        string IDS_ENDPOINT_TOKEN = IDS_ENDPOINT + IDS_TOKEN;//base.GetApplicationConstantValueByConstantName(WebConstants.PMS.UrlADMDRedirect);
            //        var convertQueryToJson = JsonConvert.SerializeObject(model);
            //        _logger.LogInformation("[request] Authentication From FBB Portal ==> " + JsonConvert.SerializeObject(headerListData) + JsonConvert.SerializeObject(model));
            //        #endregion

            //        #region Get_Profile_ENDPOINT
            //        string IDS_GETPROFILE = _configuration["IDS_GETPROFILE"];
            //        string IDS_ENDPOINT_GETPROFILE = IDS_ENDPOINT + IDS_GETPROFILE;
            //        #endregion

            //        HttpClient client = new HttpClient();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        foreach (var header in headerListData)
            //        {
            //            if (!WebConstants.PMS.ContentType.Contains(header.key))
            //                client.DefaultRequestHeaders.Add(header.key, header.value);
            //        }

            //        var uriString = IDS_ENDPOINT_TOKEN;
            //        var httpContent = new FormUrlEncodedContent(new[]
            //        {
            //            new KeyValuePair<string, string>("grant_type", IDS_Grant_Type),
            //            new KeyValuePair<string, string>("code", code),
            //            new KeyValuePair<string, string>("redirect_uri", IDS_Callback)
            //        });
            //        HttpResponseMessage responseV = client.PostAsync(uriString, httpContent).Result;
            //        _logger.LogInformation($"[response] Get AccessToken ==> {JsonConvert.SerializeObject(responseV)}");
            //        if (responseV.IsSuccessStatusCode)
            //        {
            //            var result = responseV.Content.ReadAsStringAsync().Result;
            //            AccessTokenRespModel output = JsonConvert.DeserializeObject<AccessTokenRespModel>(result);
            //            if (output != null)
            //            {
            //                HttpContext.Session.SetString(WebConstants.SessionKeys.AccessToken, string.Format("{0}", output.access_token));
            //                HttpContext.Session.SetString(WebConstants.SessionKeys.IdToken, string.Format("{0}", output.id_token));
            //                var headerListDataGet = new List<ListHeaderSendAPIModel>();
            //                headerListDataGet.Add(new ListHeaderSendAPIModel()
            //                {
            //                    key = "Authorization",
            //                    value = "Bearer " + output.access_token

            //                });
            //                try
            //                {
            //                    HttpClient clientGet = new HttpClient();
            //                    foreach (var header in headerListDataGet)
            //                    {
            //                        if (!WebConstants.PMS.ContentType.Contains(header.key))
            //                            clientGet.DefaultRequestHeaders.Add(header.key, header.value);
            //                    }

            //                    var responseG = await clientGet.GetAsync(IDS_ENDPOINT_GETPROFILE);
            //                    _logger.LogInformation($"[response] Get User Profile ==> {JsonConvert.SerializeObject(responseG)}");
            //                    if (responseG.IsSuccessStatusCode)
            //                    {
            //                        if (responseG.StatusCode == HttpStatusCode.OK)
            //                        {
            //                            var resultG = responseG.Content.ReadAsStringAsync().Result;
            //                            ProfileRespModel outputG = JsonConvert.DeserializeObject<ProfileRespModel>(resultG);
            //                            if (outputG != null)
            //                            {
            //                                #region set Session for web
            //                                HttpContext.Session.SetString(WebConstants.SessionKeys.UserName, string.Format("{0}", outputG.username));
            //                                HttpContext.Session.SetString(WebConstants.SessionKeys.FirstNameLastName, string.Format("{0} {1}", outputG.firstname, outputG.lastname));
            //                                HttpContext.Session.SetString(WebConstants.SessionKeys.TransactionId, string.Format("{0}", DateTime.Now.ToString("ddMMyyyyhhmmsstt"))); // Must have : session ID
            //                                HttpContext.Session.SetString(WebConstants.SessionKeys.SubModuleName, string.Format("{0}", outputG.sub));
            //                                HttpContext.Session.SetString(WebConstants.SessionKeys.LocationCode, string.Format("{0}", outputG.location_code));
            //                                HttpContext.Session.SetString(WebConstants.SessionKeys.AuthenticateType, string.Format("{0}", FbbPortalEntity.PanelModels.AccountModels.AuthenticateType.IDS.ToString()));
            //                                #endregion set Session for web
            //                                SetApplicationServerSession(outputG.username);

            //                                var TempIdToken = HttpContext.Session.GetString(WebConstants.SessionKeys.IdToken);
            //                                base.IdToken = TempIdToken;
            //                                _logger.LogInformation("LoginIDS ==> " + base.IdToken);
            //                                var AuType = HttpContext.Session.GetString(WebConstants.SessionKeys.AuthenticateType);
            //                                base.AuthenType = AuType;
            //                                _logger.LogInformation("LoginIDS ==> " + base.AuthenType);
            //                            }
            //                        }
            //                        else if (responseG.StatusCode == HttpStatusCode.BadRequest)
            //                        {
            //                            var resultG = responseG.Content.ReadAsStringAsync().Result;
            //                            ProfileRespErrorModel outputG = JsonConvert.DeserializeObject<ProfileRespErrorModel>(result);
            //                            if (outputG != null)
            //                            {
            //                                _logger.LogInformation($"[Error] Authentication Fail ==> {JsonConvert.SerializeObject(resultG)}");
            //                                return RedirectToAction("IDSError", "Account");
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        _logger.LogInformation($"[Error] Authentication Fail ==> {JsonConvert.SerializeObject(responseG.Content.ReadAsStringAsync().Result)}");
            //                        return RedirectToAction("IDSError", "Account");
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    _logger.LogInformation(ex.RenderExceptionRequestAndResponse());
            //                    return RedirectToAction("IDSError", "Account");
            //                }
            //            }
            //        }
            //        else
            //        {
            //            _logger.LogInformation($"[Error] Authentication Fail ==> {JsonConvert.SerializeObject(responseV.Content.ReadAsStringAsync().Result)}");
            //            return RedirectToAction("IDSError", "Account");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogInformation(ex.RenderExceptionRequestAndResponse());
            //        return RedirectToAction("IDSError", "Account");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogInformation(ex.RenderExceptionRequestAndResponse());
            //    return RedirectToAction("IDSError", "Account");
            //}
            //finally
            //{
            //    _logger.LogInformation("End AccountController >> LogOnByIDS");
            //}
            //return GetRedirectUrl("START_MAIN_PAGE");
        }






        #endregion




        [HttpPost]
        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(LoginPanelModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Configurations.UseLDAP)
                {
                    var authenResultMessage = "";
                    if (AuthenLDAP(model.UserName, model.Password, out authenResultMessage))
                    {
                        var authenticatedUser = GetUser(model.UserName);
                        authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                        Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                        base.CurrentUser = authenticatedUser;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid UserName or Password.");
                    }
                }
                else
                {
                    // bypass authen
                    var authenticatedUser = GetUser(model.UserName);
                    if (null != authenticatedUser && authenticatedUser.ProgramModel != null)
                    {
                        authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                        Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                        base.CurrentUser = authenticatedUser;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", model.UserName + " not found.");
                    }
                }
            }

            return View(model);
        }

        private SSOFields LoadSSOFieldsFromPostData(NameValueCollection form)
        {
            var ssoFields = new SSOFields();

            ssoFields.Token = form["token"];

            var tokenValues = ssoFields.Token.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ssoFields.SessionID = (tokenValues.Length > 0) ? tokenValues[0] : null;
            ssoFields.UserName = (tokenValues.Length > 1) ? tokenValues[1] : null;
            ssoFields.GroupID = (tokenValues.Length > 2) ? tokenValues[2] : null;
            ssoFields.SubModuleIDInToken = (tokenValues.Length > 3) ? tokenValues[3] : null;
            ssoFields.ClientIP = (tokenValues.Length > 4) ? (tokenValues[4] == "null" ? null : tokenValues[4]) : null;

            ssoFields.RoleID = form["rid"];
            ssoFields.SubModuleID = form["sid"];
            ssoFields.RoleName = form["rn"];
            ssoFields.SubModuleName = form["sn"];
            ssoFields.FirstName = form["fn"];
            ssoFields.LastName = form["ln"];
            ssoFields.ThemeName = form["theme"];
            ssoFields.TemplateName = form["template"];
            ssoFields.EmployeeServiceWebRootUrl = form["host"];
            ssoFields.LocationCode = form["lc"];
            ssoFields.GroupLocation = form["gl"];
            ssoFields.DepartmentCode = form["dc"];
            ssoFields.SectionCode = form["sc"];
            ssoFields.PositionByJob = form["pt"];

            return ssoFields;
        }

        //[AuthorizeUser]
        //[CustomActionFilter(LogType = "Logout")]
        public ActionResult Logout()
        {
            var normalLogout = true;
            var sessionTimeOut = false;
            var isSSO = false;
            try
            {
                if (null != base.CurrentUser)
                {
                    if (base.CurrentUser.AuthenticateType == AuthenticateType.SSO)
                    {
                        var currentUser = base.CurrentUser;

                        using (var ssoService = new EmployeeServices.EmployeeServiceWebServiceV2Service())
                        {
                            _Logger.Info(string.Format("Decreasing SSO, Token:{0}", currentUser.SSOFields.Token));

                            try
                            {
                                var syncUserSessionResponse = ssoService.decreaseCounter(currentUser.SSOFields.Token);
                                if (syncUserSessionResponse.Message.ErrorCode == Constants.SSOReturnStatus.Success)
                                {
                                    _Logger.Info("SSO Decreasing SUCCESS " +
                                        string.Format("{0}:{1}",
                                            syncUserSessionResponse.Message.ErrorCode,
                                            syncUserSessionResponse.Message.ErrorMesg));
                                }
                                else
                                {
                                    _Logger.Info("SSO Decreasing FAIL " +
                                         string.Format("{0}:{1}",
                                             syncUserSessionResponse.Message.ErrorCode,
                                             syncUserSessionResponse.Message.ErrorMesg));
                                }
                            }
                            catch (TimeoutException tex)
                            {
                                _Logger.Info("SSO syncUserSession TIMEOUT " + tex.GetErrorMessage());
                            }
                            catch (Exception ex)
                            {
                                _Logger.Info("SSO syncUserSession ERROR " + ex.GetErrorMessage());
                            }

                        }

                        normalLogout = true;
                        isSSO = true;
                    }
                    else
                    {
                        normalLogout = true;
                        isSSO = false;
                    }

                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Session.Abandon();
                }
                else
                {
                    _Logger.Info("Session Time Out");

                    sessionTimeOut = true;
                    normalLogout = false;
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    Session.Abandon();
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                _Logger.Info(ex.StackTrace);

                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();
            }

            return RedirectToAction("LogoutSso", "Account",
                new { logout = normalLogout, sessionIsTimeOut = sessionTimeOut, sso = isSSO, });
        }

        [HttpGet]
        public ActionResult LogoutSso(bool logout, bool sessionIsTimeOut, bool sso)
        {
            ViewBag.NormalLogOut = logout;
            ViewBag.SessionTimeOut = sessionIsTimeOut;
            ViewBag.IsSSO = sso;
            return View();
        }

        public UserModel GetUser(string userName)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = userName,
            };

            var authenticatedUser = _QueryProcessor.Execute(userQuery);
            return authenticatedUser;
        }

        private HttpCookie CreateAuthenticatedCookie(string userName)
        {
            var authTicket = new FormsAuthenticationTicket(2, userName, DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "");

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(authTicket))
            { HttpOnly = true };

            return authCookie;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TemporaryNotAvailable()
        {
            return View();
        }

        public bool AuthenLDAP(string userName, string password, out string authenMessage)
        {
            var authLDAPQuery = new GetAuthenLDAPQuery
            {
                UserName = userName,
                Password = password,
                ProjectCode = Configurations.ProjectCodeLdapFBB,
            };

            var authenLDAPResult = _QueryProcessor.Execute(authLDAPQuery);
            authenMessage = "";
            return authenLDAPResult;
        }

        public ActionResult LogOnByPass()
        {
            string User = "ADMINPAYG";
            var authenticatedUser = GetUser(User);


            authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
            Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
            base.CurrentUser = authenticatedUser;

            return RedirectToAction("Index", "Home");

        }

    }
}
