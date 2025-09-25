using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Minions;
using WBBContract.Queries.Commons.Account;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.Minions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.Account;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public class MinionController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<MinionProcMainCommand> _procMainCommand;
        private readonly ICommandHandler<MinionProcInstallAddressCommand> _procInsAddrCommand;
        private readonly ICommandHandler<MinionProcInstallContactCommand> _procInsContactCommand;
        private readonly ICommandHandler<MinionProcInstallPackageCommand> _procInsPackageCommand;
        private readonly ICommandHandler<MinionProcInstallSplitterCommand> _procInsSplitterCommand;
        private readonly ICommandHandler<MinionProcMessageLogCommand> _procMessageLogCommand;
        private readonly ICommandHandler<MinionExternalSmsSurveyFoaCommand> _proSmsSurveyFOACommand;
        private readonly ICommandHandler<WBBContract.Commands.CustRegisterCommand> _custRegCommand;
        //
        // GET: /Minion/
        public MinionController(IQueryProcessor queryProcessor
                                , ICommandHandler<MinionProcMainCommand> procMainCommand
                                , ICommandHandler<MinionProcInstallAddressCommand> procInsAddrCommand
                                , ICommandHandler<MinionProcInstallContactCommand> procInsContactCommand
                                , ICommandHandler<MinionProcInstallPackageCommand> procInsPackageCommand
                                , ICommandHandler<MinionProcInstallSplitterCommand> procInsSplitterCommand
                                , ICommandHandler<MinionProcMessageLogCommand> procMessageLogCommand
                                , ICommandHandler<MinionExternalSmsSurveyFoaCommand> procSmsSurveyFOACommand
                                , ICommandHandler<WBBContract.Commands.CustRegisterCommand> custRegCommand
                                , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _procMainCommand = procMainCommand;
            _procInsAddrCommand = procInsAddrCommand;
            _procInsContactCommand = procInsContactCommand;
            _procInsPackageCommand = procInsPackageCommand;
            _procMessageLogCommand = procMessageLogCommand;
            _procInsSplitterCommand = procInsSplitterCommand;
            _proSmsSurveyFOACommand = procSmsSurveyFOACommand;
            _custRegCommand = custRegCommand;
            base.Logger = logger;
        }

        #region Login Process

        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                var query = new SelectLovQuery
                {
                    LOV_TYPE = "CONFIG_TEMP_LOGIN_PAGE"
                };
                var data = _queryProcessor.Execute(query);


                string flg = "N";
                if (data.Any())
                {
                    var firstOrDefault = data.FirstOrDefault();
                    if (firstOrDefault != null) flg = firstOrDefault.LOV_VAL1;
                }

                if (flg == "Y")
                {
                    ViewBag.ReturnUrl = returnUrl;
                    var titleQuery = new GetLovQuery
                    {
                        LovType = "SCREEN",
                        LovName = "L_PAGE_MINIONL_TITLE",
                    };
                    var titleLogin = _queryProcessor.Execute(titleQuery);
                    ViewBag.configscreen = titleLogin;

                    return View();
                }

                ViewBag.ReturnUrl = returnUrl;

                return RedirectToAction("TemporaryNotAvailable", "Minion");
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                Logger.Info(ex.StackTrace);

                return RedirectToAction("TemporaryNotAvailable", "Minion");
            }
        }


        public ActionResult Logout(string flagout)
        {
            if (flagout == "Y")
            {
                UserModel authenticatedUser = null;
                base.CurrentUser = authenticatedUser;
            }
            return RedirectToAction("Login", "Minion");
        }


        [HttpPost]
        [AllowAnonymous]
        [CustomActionFilter(LogType = "Login")]
        public ActionResult Login(LoginPanelModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userName = model.UserName.Replace("\"", "").Replace("'", "");
                var passWord = model.Password.Replace("'", "").Replace("'", "");
                var grouIdQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "USER_TELESALE_GROUP_ID"
                };
                var groupId = _queryProcessor.Execute(grouIdQuery);

                var msgUserQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_FAILD"
                };
                var msgUser = _queryProcessor.Execute(msgUserQuery);

                var msgPassQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "MSG_USER_PASS_FAILD"
                };
                var msgPass = _queryProcessor.Execute(msgPassQuery);

                var cfgLDAPQuery = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "URLAuthenLDAP"
                };
                var cfgLDAP = _queryProcessor.Execute(cfgLDAPQuery);
                bool useLDAP = false;
                if (cfgLDAP != null && cfgLDAP.Count > 0)
                {
                    useLDAP = true;
                }

                if (useLDAP)
                {
                    UserModel authenticatedUser = null;
                    LovValueModel tmpGroup = null;
                    foreach (var item in groupId)
                    {
                        if (authenticatedUser == null || authenticatedUser.Groups == null)
                        {
                            tmpGroup = item;
                            authenticatedUser = GetUser(userName, item.LovValue1);
                        }
                    }

                    if (null != authenticatedUser && (null != authenticatedUser.UserName
                        && (null != authenticatedUser.Groups)
                        && (authenticatedUser.Groups[0] == Convert.ToDecimal(tmpGroup.LovValue1))))
                    {
                        string authenResultMessage;
                        if (AuthenLdap(userName, passWord, out authenResultMessage))
                        {
                            //var authenticatedUser = GetUser(userName);
                            authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                            Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                            base.CurrentUser = authenticatedUser;

                            return RedirectToAction("Index", "Minion");
                        }

                        ModelState.AddModelError("", msgPass[0].LovValue1);
                    }
                    else
                    {
                        ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                    }
                }
                else
                {
                    // bypass authen
                    UserModel authenticatedUser = null;
                    foreach (var item in groupId)
                    {
                        if (authenticatedUser == null || authenticatedUser.Groups == null)
                        {
                            authenticatedUser = GetUser(userName, item.LovValue1);
                        }
                    }
                    //Session["userName"] = userName;
                    if (null != authenticatedUser && (null != authenticatedUser.UserName && (null != authenticatedUser.Groups)))
                    {
                        authenticatedUser.AuthenticateType = AuthenticateType.LDAP;
                        Response.AppendCookie(CreateAuthenticatedCookie(authenticatedUser.UserName));
                        base.CurrentUser = authenticatedUser;

                        return RedirectToAction("Index", "Minion");
                    }

                    ModelState.AddModelError("", userName + " " + msgUser[0].LovValue1);
                }
            }
            var titleQuery = new GetLovQuery
            {
                LovType = "SCREEN",
                LovName = "L_PAGE_SALE_PORTAL_TITLE",
            };
            var titleLogin = _queryProcessor.Execute(titleQuery);
            ViewBag.configscreen = titleLogin;

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TemporaryNotAvailable()
        {
            return View();
        }

        public UserModel GetUser(string userName, string groupId)
        {
            var userQuery = new GetUserDataQuery
            {
                UserName = userName,
                GroupId = groupId
            };

            var authenticatedUser = _queryProcessor.Execute(userQuery);
            return authenticatedUser;
        }

        public bool AuthenLdap(string userName, string password, out string authenMessage)
        {
            var authLdapQuery = new GetAuthenLDAPQuery
            {
                UserName = userName,
                Password = password,
                ProjectCode = Configurations.ProjectCodeLdapFBB,
            };

            var authenLdapResult = _queryProcessor.Execute(authLdapQuery);
            authenMessage = "";

            return authenLdapResult;
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

        #endregion

        #region Business Process

        [IENoCache]
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
            {
                return RedirectToAction("Login", "Minion");
            }
            ViewBag.MinionServiceListMenu = GetMinionServiceListMenu();
            return View();
        }

        [IENoCache]
        public ActionResult IndexTest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListPackageByService(MinionGetListPackageByServiceQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListPackageByServiceQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListPackageByServiceQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception ex)
            {
                var jsonResult = Json(new { status = "", rawxml = ex.Message }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListPackageBySffPromo(MinionGetListPackageBySffPromoQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListPackageBySffPromoQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListPackageBySffPromoQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListPackageByChange(MinionGetListPackageByChangeQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListPackageByChangeQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListPackageByChangeQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetExteranlService(MinionGetExternalSoapServiceQuery model)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                //var model = new MinionGetExternalSoapServiceQuery
                //{
                //    UrlEnpoint = "http://10.252.64.129:6100/SFFWebService/SFFService",
                //    SoapHeader = @"soapenv:Header",
                //    RequestData = xml,
                //    ContentType = "text/xml",
                //    Charset = "utf-8",
                //    Method = "POST"
                //};

                #region Set Default Value

                if (string.IsNullOrEmpty(model.SoapHeader))
                {
                    model.SoapHeader = "SOAPAction";
                }

                if (string.IsNullOrEmpty(model.ContentType))
                {
                    model.ContentType = "text/xml";
                }

                if (string.IsNullOrEmpty(model.Charset))
                {
                    model.Charset = "utf-8";
                }

                #endregion Set Default Value

                model.RequestData = HttpUtility.HtmlDecode(model.RequestData);

                var query = _queryProcessor.Execute(model);

                query.ResponseData = XElement.Parse(query.ResponseData).ToString();

                var jsonResult = Json(new { query }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstallProfileProcMain(MinionProcMainCommand model, string xmlString)
        {
            try
            {
                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionProcMainCommand excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionProcMainCommand>(decodexml);
                }
                else
                {
                    model.FbbCustProfilesList = model.FbbCustProfilesList.Where(status => status.status != "D").ToList();
                    model.FbbCustContactList = model.FbbCustContactList.Where(status => status.status != "D").ToList();
                    model.FbbCustAddressList = model.FbbCustAddressList.Where(status => status.status != "D").ToList();
                    model.FbbCustPackageList = model.FbbCustPackageList.Where(status => status.status != "D").ToList();
                    model.FbbCustSplitterList = model.FbbCustSplitterList.Where(status => status.status != "D").ToList();
                    excModel = model;
                }
                _procMainCommand.Handle(excModel);

                var response = new MinionResponseCommandModel
                {
                    ReturnCode = excModel.Return_Code.ToSafeString(),
                    ReturnMessage = excModel.Return_Desc
                };

                var xml = SerializeObjectXml(response, false);
                var json = SerializeObjectJson(response);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstallAddress(MinionProcInstallAddressCommand model, string xmlString)
        {
            try
            {
                if (!ValidateModel(model))
                {
                    return Json(new { status = "Required", rawxml = "Required fields. !!!", rawjson = "" }, JsonRequestBehavior.AllowGet);
                }

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionProcInstallAddressCommand excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionProcInstallAddressCommand>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                _procInsAddrCommand.Handle(excModel);

                var response = new MinionResponseInsAddrCommandModel
                {
                    ReturnCode = excModel.Return_Code.ToSafeString(),
                    ReturnMessage = excModel.Return_Desc,
                    ReturnAddressRowId = excModel.Return_Address_RowId
                };

                var xml = SerializeObjectXml(response, false);
                var json = SerializeObjectJson(response);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private static bool ValidateModel<T>(T obj)
        {
            var isValidate = true;

            try
            {
                var ty = obj.GetType();
                var pinfo = ty.GetFilteredProperties();

                foreach (var prop in pinfo)
                {
                    if (prop.GetValue(obj, null) == null)
                    {
                        isValidate = false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return isValidate;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstallContact(MinionProcInstallContactCommand model, string xmlString)
        {
            if (!ValidateModel(model))
            {
                return Json(new { status = "Required", rawxml = "Required fields. !!!", rawjson = "" }, JsonRequestBehavior.AllowGet);
            }

            if (null == CurrentUser)
                return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

            MinionProcInstallContactCommand excModel;



            if (!string.IsNullOrWhiteSpace(xmlString))
            {
                var decodexml = HttpUtility.HtmlDecode(xmlString);
                excModel = DeserializeXml<MinionProcInstallContactCommand>(decodexml);
            }
            else
            {
                excModel = model;
            }
            _procInsContactCommand.Handle(excModel);

            var response = new MinionResponseInsContactCommandModel
            {
                ReturnCode = excModel.Return_Code.ToSafeString(),
                ReturnMessage = excModel.Return_Desc
            };

            var xml = SerializeObjectXml(response, false);
            var json = SerializeObjectJson(response);

            var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstallPackage(MinionProcInstallPackageCommand model, string xmlString)
        {
            try
            {
                if (!ValidateModel(model))
                {
                    return Json(new { status = "Required", rawxml = "Required fields. !!!", rawjson = "" }, JsonRequestBehavior.AllowGet);
                }

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionProcInstallPackageCommand excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionProcInstallPackageCommand>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                _procInsPackageCommand.Handle(excModel);

                var response = new MinionResponseInsPackageCommandModel
                {
                    ReturnCode = excModel.Return_Code.ToSafeString(),
                    ReturnMessage = excModel.Return_Desc
                };

                var xml = SerializeObjectXml(response, false);
                var json = SerializeObjectJson(response);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstallSplitter(MinionProcInstallSplitterCommand model, string xmlString)
        {
            try
            {
                if (!ValidateModel(model))
                {
                    return Json(new { status = "Required", rawxml = "Required fields. !!!", rawjson = "" }, JsonRequestBehavior.AllowGet);
                }

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionProcInstallSplitterCommand excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionProcInstallSplitterCommand>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                _procInsSplitterCommand.Handle(excModel);

                var response = new MinionResponseInsSplitterCommandModel
                {
                    ReturnCode = excModel.Return_Code.ToSafeString(),
                    ReturnMessage = excModel.Return_Desc
                };

                var xml = SerializeObjectXml(response, false);
                var json = SerializeObjectJson(response);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalSmsSurvey(MinionExternalSmsSurveyFoaCommand model, string xmlString)
        {
            try
            {
                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionExternalSmsSurveyFoaCommand excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionExternalSmsSurveyFoaCommand>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                _proSmsSurveyFOACommand.Handle(excModel);

                var response = excModel.Results;

                var xml = SerializeObjectXml(response, false);
                var json = SerializeObjectJson(response);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MessageLog(MinionProcMessageLogCommand model, string xmlString)
        {
            try
            {

                if (!ValidateModel(model))
                {
                    return Json(new { status = "Required", rawxml = "Required fields. !!!", rawjson = "" }, JsonRequestBehavior.AllowGet);
                }

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionProcMessageLogCommand excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionProcMessageLogCommand>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                _procMessageLogCommand.Handle(excModel);

                var response = new MinionResponseMessageLogCommandModel
                {
                    ReturnCode = excModel.Response_Code.ToSafeString(),
                    ReturnMessage = excModel.Response_Message
                };

                var xml = SerializeObjectXml(response, false);
                var json = SerializeObjectJson(response);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrderNew(MinionGetSaveOrderNew model)
        {
            try
            {
                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                var clientIp = string.Empty;
                var response = new MinionGetSaveOrderNewModel();

                model.RequestData = HttpUtility.HtmlDecode(model.RequestData);
                GetSaveOrderRespQuery excModel;

                if (!string.IsNullOrWhiteSpace(model.RequestData))
                {
                    var decodexml = HttpUtility.HtmlDecode(model.RequestData);
                    excModel = DeserializeXml<GetSaveOrderRespQuery>(decodexml);

                    //Set Value
                    clientIp = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(clientIp))
                    {
                        clientIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }

                    excModel.FullUrl = model.UrlEnpoint;
                    excModel.QuickWinPanelModel.ClientIP = clientIp;
                }
                else
                {
                    excModel = null;
                }

                #region Save Order New

                var query = new WBBEntity.PanelModels.WebServiceModels.SaveOrderResp();
                var querySaveOrderVoIpResp = new WBBEntity.PanelModels.WebServiceModels.SaveOrderResp();

                if (excModel != null && excModel.QuickWinPanelModel != null)
                {
                    if (!excModel.QuickWinPanelModel.CoveragePanelModel.CoverageResult.ToSafeString().ToLower().Contains("plan"))
                    {
                        query = _queryProcessor.Execute(excModel);
                        response.MinionSaveOrderResponse = query;

                        if (query.RETURN_CODE != 0)
                        {
                            Logger.Info(query.RETURN_MESSAGE);
                        }
                        else
                        {
                            // case register Playbox and VOIP
                            if (excModel.QuickWinPanelModel.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" || excModel.QuickWinPanelModel.SummaryPanelModel.TOPUP.ToSafeString() == "1")
                            {
                                if (excModel.QuickWinPanelModel.SummaryPanelModel.PackageModel.SelectPlayBox_Flag == "1" && excModel.QuickWinPanelModel.SummaryPanelModel.PackageModel.SelectVas_Flag == "1")
                                {
                                    excModel.QuickWinPanelModel.SummaryPanelModel.VOIP_FLAG = "1";
                                    querySaveOrderVoIpResp = _queryProcessor.Execute(excModel);
                                    response.MinionSaveOrderResponseVoIp = querySaveOrderVoIpResp;
                                    if (querySaveOrderVoIpResp.RETURN_CODE != 0)
                                    {
                                        Logger.Info(querySaveOrderVoIpResp.RETURN_MESSAGE);
                                    }
                                }
                            }
                        }
                    }

                    if (excModel.QuickWinPanelModel.ForCoverageResult != true)
                    {
                        #region set value

                        if (string.IsNullOrEmpty(excModel.QuickWinPanelModel.CoveragePanelModel.Address.L_MOOBAN))
                        {
                            excModel.QuickWinPanelModel.CoveragePanelModel.Address.L_MOOBAN = "-";
                        }
                        if (string.IsNullOrEmpty(excModel.QuickWinPanelModel.CoveragePanelModel.L_FLOOR_VILLAGE))
                        {
                            excModel.QuickWinPanelModel.CoveragePanelModel.L_FLOOR_VILLAGE = "1";
                        }

                        #endregion set value

                        #region Save register

                        // register customer
                        var coverageResultId = "";
                        if (null != excModel.QuickWinPanelModel.CoveragePanelModel)
                            coverageResultId = excModel.QuickWinPanelModel.CoveragePanelModel.RESULT_ID;

                        var command = new WBBContract.Commands.CustRegisterCommand
                        {
                            QuickWinPanelModel = excModel.QuickWinPanelModel,
                            CurrentCulture = SiteSession.CurrentUICulture,
                            InterfaceCode = query.RETURN_CODE.ToSafeString(),
                            InterfaceDesc = query.RETURN_MESSAGE,
                            InterfaceOrder = query.RETURN_IA_NO,
                            CoverageResultId = coverageResultId.ToSafeDecimal(),
                            ClientIP = clientIp
                        };
                        _custRegCommand.Handle(command);
                        response.ReturnIaNo = command.CustomerId;

                        //Register VOIP
                        if (excModel.QuickWinPanelModel.SummaryPanelModel.VAS_FLAG.ToSafeString() == "2" ||
                            excModel.QuickWinPanelModel.SummaryPanelModel.TOPUP.ToSafeString() == "1")
                        {
                            if (excModel.QuickWinPanelModel.SummaryPanelModel.PackageModel.SelectVas_Flag == "1")
                            {
                                if (querySaveOrderVoIpResp.RETURN_CODE == 0)
                                {
                                    var commandSaveOrderVoIpResp = new WBBContract.Commands.CustRegisterCommand
                                    {
                                        QuickWinPanelModel = excModel.QuickWinPanelModel,
                                        CurrentCulture = SiteSession.CurrentUICulture,
                                        InterfaceCode = querySaveOrderVoIpResp.RETURN_CODE.ToSafeString(),
                                        InterfaceDesc = querySaveOrderVoIpResp.RETURN_MESSAGE,
                                        InterfaceOrder = querySaveOrderVoIpResp.RETURN_IA_NO,
                                        CoverageResultId = coverageResultId.ToSafeDecimal(),
                                        ClientIP = clientIp
                                    };
                                    _custRegCommand.Handle(commandSaveOrderVoIpResp);
                                    response.ReturnIaNoVoIp = commandSaveOrderVoIpResp.CustomerId;
                                }
                            }
                        }

                        #endregion Save register

                    }
                }

                #endregion Save Order New

                //var query = _queryProcessor.Execute(excModel);

                //if (query != null)
                //{
                //    response.MinionSaveOrderResponse = query;

                //    //Customer Register
                //    if (excModel != null && excModel.QuickWinPanelModel != null)
                //    {
                //        var coverageResultId = "";
                //        if (null != excModel.QuickWinPanelModel.CoveragePanelModel)
                //            coverageResultId = excModel.QuickWinPanelModel.CoveragePanelModel.RESULT_ID;

                //        var command = new WBBContract.Commands.CustRegisterCommand
                //        {
                //            QuickWinPanelModel = excModel.QuickWinPanelModel,
                //            CurrentCulture = SiteSession.CurrentUICulture,
                //            //InterfaceCode = interfaceCode,
                //            //InterfaceDesc = interfaceDesc,
                //            //InterfaceOrder = interfaceOrder,
                //            CoverageResultId = coverageResultId.ToSafeDecimal(),
                //            ClientIP = clientIp
                //        };

                //        _custRegCommand.Handle(command);

                //        response.MinionCustomerRegisterResponse = command.CustomerId;
                //    }
                //}

                var xml = SerializeObjectXml(response, false);
                var json = SerializeObjectJson(response);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListProfileCustomer(MinionGetListProfileCustomerQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListProfileCustomerQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListProfileCustomerQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListProfileContact(MinionGetListProfileContactQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListProfileContactQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListProfileContactQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListProfileAddress(MinionGetListProfileAddressQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListProfileAddressQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListProfileAddressQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListProfilePackage(MinionGetListProfilePackageQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListProfilePackageQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListProfilePackageQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListProfileSplitter(MinionGetListProfileSplitterQuery model, string xmlString)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                MinionGetListProfileSplitterQuery excModel;

                if (!string.IsNullOrWhiteSpace(xmlString))
                {
                    var decodexml = HttpUtility.HtmlDecode(xmlString);
                    excModel = DeserializeXml<MinionGetListProfileSplitterQuery>(decodexml);
                }
                else
                {
                    excModel = model;
                }
                var query = _queryProcessor.Execute(excModel);

                var xml = SerializeObjectXml(query, false);
                var json = SerializeObjectJson(query);

                var jsonResult = Json(new { status = "", rawxml = xml, rawjson = json }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestSoap(MinionGetExternalSoapServiceQuery model)
        {
            try
            {

                if (null == base.CurrentUser)
                    return Json(new { status = "T" }, JsonRequestBehavior.AllowGet);

                //var model = new MinionGetExternalSoapServiceQuery
                //{
                //    UrlEnpoint = "http://10.252.64.129:6100/SFFWebService/SFFService",
                //    SoapHeader = @"soapenv:Header",
                //    RequestData = xml,
                //    ContentType = "text/xml",
                //    Charset = "utf-8",
                //    Method = "POST"
                //};

                #region Set Default Value

                if (string.IsNullOrEmpty(model.SoapHeader))
                {
                    model.SoapHeader = "SOAPAction";
                }

                if (string.IsNullOrEmpty(model.ContentType))
                {
                    model.ContentType = "text/xml";
                }

                if (string.IsNullOrEmpty(model.Charset))
                {
                    model.Charset = "utf-8";
                }

                #endregion Set Default Value

                model.RequestData = HttpUtility.HtmlDecode(model.RequestData);

                var query = _queryProcessor.Execute(model);

                query.ResponseData = XElement.Parse(query.ResponseData).ToString();

                var jsonResult = Json(new { query }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetRequestXml(string serviceId)
        {
            var query = new MinionGetMinionServiceQuery { ServiceId = serviceId.ToSafeDecimal() };

            var minionServiceMenu = _queryProcessor.Execute(query);

            var result = minionServiceMenu.FirstOrDefault() ?? new MinionGetMinionServiceQueryModel();

            var jsonResult = Json(new { status = "", rawxml = result.REQUET_SOAP_XML, result.DEV_SERVICE_MAIN_URL, result.STG_SERVICE_MAIN_URL, result.PRD_SERVICE_MAIN_URL }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        private static string SerializeObjectXml<T>(T toSerialize, bool removeNamepsaces = true)
        {
            if (removeNamepsaces)
            {
                var xmlSerializer = new XmlSerializer(toSerialize.GetType());

                using (var textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, toSerialize);
                    return textWriter.ToString();
                }
            }

            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(toSerialize.GetType());
            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, toSerialize, emptyNamepsaces);
                return stream.ToString();
            }
        }

        private static string SerializeObjectJson<T>(T toSerialize)
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            var json = serializer.Serialize(toSerialize);
            return json;
        }

        private static T DeserializeXml<T>(string xmlString)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xmlString))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        [HttpPost]
        public ActionResult GetXmlDefualt(string fileName)
        {

            string xml = System.IO.File.ReadAllText(Server.MapPath("~/Views/Minion/" + fileName + ".xml"));
            return Json(Content(xml));
        }

        #endregion

        #region Master Process

        public List<MinionGetMinionServiceQueryModel> GetMinionServiceListMenu()
        {
            var query = new MinionGetMinionServiceQuery { Flag = "MENU" };
            var minionServiceMenu = _queryProcessor.Execute(query);

            return minionServiceMenu ?? new List<MinionGetMinionServiceQueryModel>();
        }

        #endregion Master Process
    }
}
