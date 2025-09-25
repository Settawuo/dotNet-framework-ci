using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;
namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public class LeavemessageController : WBBController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveLeavemessageCommand> _saveLeavemessageCommand;
        private readonly ICommandHandler<SaveLeavemessageIICommand> _saveLeavemessageIICommand;
        private readonly ICommandHandler<SaveLeavemessageIMCommand> _saveLeavemessageIMCommand;
        private readonly ICommandHandler<UpdatePreregisterStatusPackageCommand> _updatePregisterCommand;

        private readonly ICommandHandler<GetLeaveMsgReferenceNoCommand> _getLeaveMsgRefCommand;
        //
        // GET: /Leavemessage/
        //new leavmessage
        private readonly ICommandHandler<SaveLeavemessagePNCommand> _saveLeavemessagePNCommand;
        #endregion

        #region Constuctor

        public LeavemessageController(IQueryProcessor queryProcessor
              , ICommandHandler<SaveLeavemessageCommand> saveLeavemessageCommand
              , ICommandHandler<SaveLeavemessageIICommand> saveLeavemessageIICommand
              , ICommandHandler<SaveLeavemessageIMCommand> saveLeavemessageIMCommand
              , ICommandHandler<UpdatePreregisterStatusPackageCommand> updatePregisterCommand
              , ICommandHandler<GetLeaveMsgReferenceNoCommand> getLeaveMsgRefCommand
              //partner
              , ICommandHandler<SaveLeavemessagePNCommand> saveLeavemessagePNCommand
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _saveLeavemessageCommand = saveLeavemessageCommand;
            _saveLeavemessageIICommand = saveLeavemessageIICommand;
            _saveLeavemessageIMCommand = saveLeavemessageIMCommand;
            _updatePregisterCommand = updatePregisterCommand;
            _getLeaveMsgRefCommand = getLeaveMsgRefCommand;
            //partner
            _saveLeavemessagePNCommand = saveLeavemessagePNCommand;
            base.Logger = logger;
        }

        #endregion

        #region ActionResult

        public ActionResult Index(string SaveStatus = "", string RefNo = "", string locationData = "", string locationWTTX = "", string WTTxFull = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR021 = GetScreenConfig("FBBOR021");
            ViewBag.LabelFBBOR021 = LovScreenFBBOR021;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR021 != null && LovScreenFBBOR021.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR021.Any(c => c.Name == "L_REGISTER_AIS_FIBRE") ? LovScreenFBBOR021.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR021.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_TH.jpg") ? LovScreenFBBOR021.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR021.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_EN.jpg") ? LovScreenFBBOR021.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage";

            if (locationData != "")
            {
                ViewBag.LocationData = locationData;
            }
            //20.2
            if (WTTxFull != "")
            {
                ViewBag.WTTXFull = true;
            }
            if (locationWTTX != "")
            {
                ViewBag.LocationWTTX = locationWTTX;
            }

            ViewBag.FullUrl = this.Url.Action("", "LeaveMessage", null, this.Request.Url.Scheme);

            return View();
        }

        public ActionResult Expired()
        {
            return View();
        }

        // LeavemessageIII
        public ActionResult ADN1(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR029 = GetScreenConfig("FBBOR029");
            ViewBag.LabelFBBOR029 = LovScreenFBBOR029;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR029 != null && LovScreenFBBOR029.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR029.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_III") ? LovScreenFBBOR029.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_III").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR029.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_III_TH.jpg") ? LovScreenFBBOR029.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_III_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR029.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_III_EN.jpg") ? LovScreenFBBOR029.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_III_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/ADN1";

            var expired = LovScreenFBBOR029.FirstOrDefault(item => item.Name == "LEAVEMESSAGEIII_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR029.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEIII_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("ADN1", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("ADN1");
        }

        // LeavemessageIV
        public ActionResult ADN2(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR030 = GetScreenConfig("FBBOR030");
            ViewBag.LabelFBBOR030 = LovScreenFBBOR030;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR030 != null && LovScreenFBBOR030.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR030.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_IV") ? LovScreenFBBOR030.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_IV").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR030.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IV_TH.jpg") ? LovScreenFBBOR030.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IV_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR030.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IV_EN.jpg") ? LovScreenFBBOR030.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IV_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/ADN2";

            var expired = LovScreenFBBOR030.FirstOrDefault(item => item.Name == "LEAVEMESSAGEIV_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR030.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEIV_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("ADN2", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("ADN2");
        }

        // LeavemessageV
        public ActionResult ADN3(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR031 = GetScreenConfig("FBBOR031");
            ViewBag.LabelFBBOR031 = LovScreenFBBOR031;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR031 != null && LovScreenFBBOR031.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR031.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_V") ? LovScreenFBBOR031.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_V").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR031.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_V_TH.jpg") ? LovScreenFBBOR031.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_V_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR031.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_V_EN.jpg") ? LovScreenFBBOR031.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_V_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/ADN3";

            var expired = LovScreenFBBOR031.FirstOrDefault(item => item.Name == "LEAVEMESSAGEV_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR031.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEV_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("ADN3", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("ADN3");
        }

        // LeavemessageVII
        public ActionResult ADN4(string SaveStatus = "", string RefNo = "", string LocationData = "")
        {
            if (LocationData != "")
            {
                ViewBag.LocationData = LocationData;
            }

            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR033 = GetScreenConfig("FBBOR033");
            ViewBag.LabelFBBOR033 = LovScreenFBBOR033;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR033 != null && LovScreenFBBOR033.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR033.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_VII") ? LovScreenFBBOR033.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_VII").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR033.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VII_TH.jpg") ? LovScreenFBBOR033.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VII_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR033.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VII_EN.jpg") ? LovScreenFBBOR033.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VII_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/ADN4";

            var expired = LovScreenFBBOR033.FirstOrDefault(item => item.Name == "LEAVEMESSAGEVII_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR033.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEVII_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("ADN4", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("ADN4");
        }

        // LeavemessageVIII
        public ActionResult LINE1(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR034 = GetScreenConfig("FBBOR034");
            ViewBag.LabelFBBOR034 = LovScreenFBBOR034;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR034 != null && LovScreenFBBOR034.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR034.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_VIII") ? LovScreenFBBOR034.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_VIII").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR034.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VIII_TH.jpg") ? LovScreenFBBOR034.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VIII_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR034.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VIII_EN.jpg") ? LovScreenFBBOR034.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VIII_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/LINE1";

            var expired = LovScreenFBBOR034.FirstOrDefault(item => item.Name == "LEAVEMESSAGEVIII_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR034.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEVIII_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("LINE1", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("LINE1");
        }

        // LeavemessageII
        public ActionResult LINE2(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR027 = GetScreenConfig("FBBOR027");
            ViewBag.LabelFBBOR027 = LovScreenFBBOR027;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR027 != null && LovScreenFBBOR027.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR027.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_II") ? LovScreenFBBOR027.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_II").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR027.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_II_TH.jpg") ? LovScreenFBBOR027.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_II_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR027.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_II_EN.jpg") ? LovScreenFBBOR027.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_II_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/LINE2";

            var expired = LovScreenFBBOR027.FirstOrDefault(item => item.Name == "LEAVEMESSAGEII_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR027.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEII_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("LINE2", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("LINE2");
        }

        // LeavemessageVI
        public ActionResult LINE3(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR032 = GetScreenConfig("FBBOR032");
            ViewBag.LabelFBBOR032 = LovScreenFBBOR032;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR032 != null && LovScreenFBBOR032.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR032.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_VI") ? LovScreenFBBOR032.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_VI").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR032.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VI_TH.jpg") ? LovScreenFBBOR032.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VI_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR032.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VI_EN.jpg") ? LovScreenFBBOR032.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_VI_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/LINE3";

            var expired = LovScreenFBBOR032.FirstOrDefault(item => item.Name == "LEAVEMESSAGEVI_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR032.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEVI_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("LINE3", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("LINE3");
        }

        // LeavemessageI
        public ActionResult Event1(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR026 = GetScreenConfig("FBBOR026");

            ViewBag.LabelFBBOR026 = LovScreenFBBOR026;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR026 != null && LovScreenFBBOR026.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR026.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_I") ? LovScreenFBBOR026.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_I").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR026.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_I_TH.jpg") ? LovScreenFBBOR026.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_I_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR026.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_I_EN.jpg") ? LovScreenFBBOR026.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_I_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/Event1";

            var expired = LovScreenFBBOR026.FirstOrDefault(item => item.Name == "LEAVEMESSAGEI_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR026.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEI_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("Event1", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("Event1");
        }

        // LeavemessageIX
        public ActionResult Event2(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR035 = GetScreenConfig("FBBOR035");
            ViewBag.LabelFBBOR035 = LovScreenFBBOR035;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR035 != null && LovScreenFBBOR035.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR035.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_IX") ? LovScreenFBBOR035.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_IX").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR035.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IX_TH.jpg") ? LovScreenFBBOR035.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IX_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR035.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IX_EN.jpg") ? LovScreenFBBOR035.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_IX_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/Event2";

            var expired = LovScreenFBBOR035.FirstOrDefault(item => item.Name == "LEAVEMESSAGEIX_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR035.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEIX_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("Event2", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("Event2");
        }

        // LeavemessageX
        public ActionResult Event3(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR036 = GetScreenConfig("FBBOR036");
            ViewBag.LabelFBBOR036 = LovScreenFBBOR036;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR036 != null && LovScreenFBBOR036.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR036.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_X") ? LovScreenFBBOR036.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_X").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR036.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_X_TH.jpg") ? LovScreenFBBOR036.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_X_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR036.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_X_EN.jpg") ? LovScreenFBBOR036.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_X_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/Event3";

            var expired = LovScreenFBBOR036.FirstOrDefault(item => item.Name == "LEAVEMESSAGEX_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR036.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEX_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("Event3", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("Event3");
        }

        // LeavemessageA
        public ActionResult Special1(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR037 = GetScreenConfig("FBBOR037");
            ViewBag.LabelFBBOR037 = LovScreenFBBOR037;
            var LabelFBBOR028 = GetScreenConfig("FBBOR028");
            ViewBag.LabelFBBOR028 = LabelFBBOR028;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR037 != null && LovScreenFBBOR037.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR037.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_A") ? LovScreenFBBOR037.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_A").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR037.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_A_TH.jpg") ? LovScreenFBBOR037.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_A_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR037.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_A_EN.jpg") ? LovScreenFBBOR037.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_A_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/Special1";

            var expired = LovScreenFBBOR037.FirstOrDefault(item => item.Name == "LEAVEMESSAGEA_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR037.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEA_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("Special1", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("Special1");
        }

        // LeavemessageB
        public ActionResult Special2(string SaveStatus = "", string RefNo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR038 = GetScreenConfig("FBBOR038");
            ViewBag.LabelFBBOR038 = LovScreenFBBOR038;

            var LabelFBBOR028 = GetScreenConfig("FBBOR028");
            ViewBag.LabelFBBOR028 = LabelFBBOR028;

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR038 != null && LovScreenFBBOR038.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR038.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_B") ? LovScreenFBBOR038.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_B").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR038.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_B_TH.jpg") ? LovScreenFBBOR038.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_B_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR038.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_B_EN.jpg") ? LovScreenFBBOR038.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_B_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/Special2";

            var expired = LovScreenFBBOR038.FirstOrDefault(item => item.Name == "LEAVEMESSAGEB_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR038.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEB_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("Special2", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("Special2");
        }

        // Leavemessagethailandpost
        public ActionResult Thailandpost(string SaveStatus = "", string RefNo = "", string Ref = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }

            if (Ref != "")
            {
                ViewBag.Ref = Ref;
            }

            var LovScreenFBBOR043 = GetScreenConfig("FBBOR043");
            ViewBag.LabelFBBOR043 = LovScreenFBBOR043;

            var LabelFBBOR028 = GetScreenConfig("FBBOR028");
            ViewBag.LabelFBBOR028 = LabelFBBOR028;

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR043 != null && LovScreenFBBOR043.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR043.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_XX") ? LovScreenFBBOR043.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_XX").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR043.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_TH.jpg") ? LovScreenFBBOR043.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR043.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_EN.jpg") ? LovScreenFBBOR043.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/Thailandpost";

            var expired = LovScreenFBBOR043.FirstOrDefault(item => item.Name == "LEAVEMESSAGEXX_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR043.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEXX_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("Thailandpost", "LeaveMessage", null, this.Request.Url.Scheme);

            return View("Thailandpost");
        }

        //LeaveMessageIM
        public ActionResult IndexIM(string SaveStatus = "", string RefNo = "", string SentIMStatusCode = "", string locationData = "", string buildingName = "", string houseNo = "", string soi = "", string road = "", string province = "", string district = "", string subDistrict = "", string postcode = "", string buildNo = "", string floor = "", string moo = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                ViewBag.SentIMStatusCode = SentIMStatusCode;
                Session["LeavemessageSaveStatus"] = "Y";
            }
            var LovScreenFBBOR039 = GetScreenConfig("FBBOR039");
            ViewBag.LabelFBBOR039 = LovScreenFBBOR039;
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR039 != null && LovScreenFBBOR039.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR039.Any(c => c.Name == "L_REGISTER_AIS_FIBRE") ? LovScreenFBBOR039.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR039.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_TH.jpg") ? LovScreenFBBOR039.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR039.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_EN.jpg") ? LovScreenFBBOR039.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage";

            if (locationData != "")
            {
                ViewBag.LocationData = locationData;
            }

            ViewBag.FullUrl = this.Url.Action("", "LeaveMessage/IndexIM", null, this.Request.Url.Scheme);
            ViewBag.buildingName = buildingName;
            ViewBag.houseNo = houseNo;
            ViewBag.soi = soi;
            ViewBag.road = road;
            ViewBag.province = province;
            ViewBag.district = district;
            ViewBag.subDistrict = subDistrict;
            ViewBag.postcode = postcode;
            ViewBag.buildNo = buildNo;
            ViewBag.floor = floor;
            ViewBag.moo = moo;
            return View();
        }

        //LeaveMessagePartnert
        public ActionResult Partner(string SaveStatus = "", string RefNo = "", string Ref = "", string Show = "")
        {
            if (SaveStatus != "")
            {
                ViewBag.SaveStatus = SaveStatus;
                ViewBag.RefNo = RefNo;
                //Session["LeavemessageSaveStatus"] = "Y";
            }

            if (Ref != "")
            {
                //ViewBag.Ref = Ref;
                Session["Partner_Ref"] = Ref;
                ViewBag.Ref = Session["Partner_Ref"];
            }
            ViewBag.Ref = Session["Partner_Ref"];
            if (Show != "")
            {
                //ViewBag.Show = Show;
                Session["Partner_Show"] = Show;
                ViewBag.Show = Session["Partner_Show"];
            }
            ViewBag.Show = Session["Partner_Show"];

            var LovScreenFBBOR044 = GetScreenConfig("FBBOR044");
            ViewBag.LabelFBBOR044 = LovScreenFBBOR044;

            var LabelFBBOR028 = GetScreenConfig("FBBOR028");
            ViewBag.LabelFBBOR028 = LabelFBBOR028;

            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            if (LovScreenFBBOR044 != null && LovScreenFBBOR044.Count > 0)
            {
                ViewBag.TitleText = LovScreenFBBOR044.Any(c => c.Name == "L_REGISTER_AIS_FIBRE_XX") ? LovScreenFBBOR044.FirstOrDefault(c => c.Name == "L_REGISTER_AIS_FIBRE_XX").DisplayValue : "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR044.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_TH.jpg") ? LovScreenFBBOR044.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_TH.jpg").Blob : "";
                }
                else
                {
                    ViewBag.LeaveMessagePic = LovScreenFBBOR044.Any(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_EN.jpg") ? LovScreenFBBOR044.FirstOrDefault(c => c.Name == "L_LEAVE_MESSAGE_PIC" && c.DisplayValue == "AISFIBRE_600_XX_EN.jpg").Blob : "";
                }
            }
            ViewBag.UrlRef = "/LeaveMessage/Partner?Show='" + Show + "'&Ref='" + Ref + "'";

            var expired = LovScreenFBBOR044.FirstOrDefault(item => item.Name == "LEAVEMESSAGEXX_EXPIRED" && item.DisplayValueJing.ToDate().GetValueOrDefault().Date > DateTime.Now.Date);
            if (expired == null)
            {
                var messageExpired =
                    LovScreenFBBOR044.FirstOrDefault(item => item.Name == "L_LEAVEMESSAGEXX_EXPIRED_MESSAGE") ??
                    new LovScreenValueModel();
                ViewBag.MessageExpired = messageExpired.DisplayValue;

                return View("Expired");
            }

            ViewBag.FullUrl = this.Url.Action("Partner", "LeaveMessage", new { Show = Show, Ref = Ref }, this.Request.Url.Scheme);

            return View("Partner");
        }


        // Save Leavemessage
        public ActionResult SaveRegister(LeavemessagePanelModel Model, string full_url)
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

            var FullUrl = full_url.ToSafeString() != null ? full_url.ToSafeString() : "";

            var Command = new SaveLeavemessageCommand
            {
                p_language = Language,
                p_service_speed = Model.SELECT_SERVICE,
                p_cust_name = Model.NAME,
                p_cust_surname = Model.SURNAME,
                p_contact_mobile_no = Model.CONTACT_MOBILE,
                p_is_ais_mobile = Model.MOBILE_IS_AIS,
                p_contact_email = Model.EMAIL,
                p_address_type = Model.SELECT_ADDRESS_TYPE,
                p_building_name = Model.BUILDING,
                p_house_no = Model.HOUSE_NO,
                p_soi = Model.SOI,
                p_road = Model.ROAD,
                p_sub_district = Model.SUB_DISTRICT,
                p_district = Model.DISTRICT,
                p_province = Model.PROVINCE,
                p_postal_code = Model.POSTAL_CODE,
                p_contact_time = Model.CONTACT_TIME,
                p_location_code = "",
                p_asc_code = "",
                p_channel = "FBB_WEB",
                p_internet_no = "",
                p_line_id = "",
                p_voucher_desc = "",
                p_campaign_project_name = "Leave Message",
                p_rental_flag = Model.SELECT_TYPE_BUILD,
                p_location_check_coverage = Model.LOCATION.ToSafeString(),
                p_full_address = Model.FULL_ADDRESS,
                p_url = FullUrl.ToSafeString(),
                p_wttx_full = Model.WTTX_FULL,
                p_latitude = Model.WTTX_LOCATION != null ? Model.WTTX_LOCATION.Split(',')[0] : null,
                p_longitude = Model.WTTX_LOCATION != null ? Model.WTTX_LOCATION.Split(',')[1] : null
            };
            _saveLeavemessageCommand.Handle(Command);
            if (Command.return_code == 0)
            {
                SaveStatus = "Y";
                RefNo = Command.return_msg;
            }
            else
            {
                SaveStatus = "N";
            }

            return RedirectToAction("Index", new { SaveStatus = SaveStatus, RefNo = RefNo });
        }

        // Save Leavemessage I-X
        public ActionResult SaveRegisterII(LeavemessageIIPanelModel Model, string full_url, string mobile_is_ais_special = "")
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

            string p_latitude = "";
            string p_longitude = "";
            if (!string.IsNullOrEmpty(Model.LOCATION))
            {
                string[] tmpLocation = Model.LOCATION.Split(',');
                if (tmpLocation != null && tmpLocation.Count() == 2)
                {
                    p_latitude = tmpLocation[0].ToSafeString();
                    p_longitude = tmpLocation[1].ToSafeString();
                }
            }
            var mobile_is_ais = string.IsNullOrEmpty(mobile_is_ais_special.ToSafeString()) ? Model.MOBILE_IS_AIS : Model.MOBILE_IS_AIS + mobile_is_ais_special;
            var LovCampaignProjectName = GetLovList("FBB_CONSTANT", "CAMPAIGN_PROJECT_NAME");
            var FullUrl = full_url.ToSafeString() != null ? full_url.ToSafeString() : "";
            var ViewsRedirect = string.IsNullOrEmpty(full_url.ToSafeString()) ? "Index" : full_url.ToSafeString().Split('/')[full_url.ToSafeString().Split('/').Count() - 1];
            var LovChannal = LovCampaignProjectName.Where(l => l.Text == ViewsRedirect).FirstOrDefault().LovValue1;
            var LovCampaign = LovCampaignProjectName.Where(l => l.Text == ViewsRedirect).FirstOrDefault().LovValue2;

            var Command = new SaveLeavemessageIICommand
            {
                p_language = Language,
                p_service_speed = Model.SELECT_SERVICE,
                p_cust_name = Model.NAME,
                p_cust_surname = Model.SURNAME,
                p_contact_mobile_no = Model.CONTACT_MOBILE,
                //p_is_ais_mobile = Model.MOBILE_IS_AIS,
                p_is_ais_mobile = mobile_is_ais,
                p_contact_email = Model.EMAIL,
                p_address_type = Model.SELECT_ADDRESS_TYPE,
                p_building_name = Model.BUILDING,
                p_house_no = Model.HOUSE_NO,
                p_soi = Model.SOI,
                p_road = Model.ROAD,
                p_sub_district = Model.SUB_DISTRICT,
                p_district = Model.DISTRICT,
                p_province = Model.PROVINCE,
                p_postal_code = Model.POSTAL_CODE,
                p_contact_time = Model.CONTACT_TIME,
                p_rental_flag = Model.SELECT_TYPE_BUILD,

                p_location_code = Model.LOCATION_CODE.ToSafeString(),
                p_asc_code = Model.ASC_CODE.ToSafeString(),
                p_emp_id = Model.EMP_ID.ToSafeString(),
                p_sales_rep = Model.SALES_REP.ToSafeString(),

                p_channal = LovChannal.ToSafeString(),
                p_campaign = LovCampaign.ToSafeString(),
                p_url = FullUrl.ToSafeString(),
                p_latitude = p_latitude,
                p_longitude = p_longitude

            };
            _saveLeavemessageIICommand.Handle(Command);
            if (Command.return_code == 0)
            {
                SaveStatus = "Y";
                RefNo = Command.return_message;
            }
            else
            {
                SaveStatus = "N";
            }

            return RedirectToAction(ViewsRedirect, new { SaveStatus = SaveStatus, RefNo = RefNo });
        }

        // Save Leavemessage Channel IM
        public ActionResult SaveRegisterIM(LeavemessageIMPanelModel Model, string full_url, string status_im_update)
        {
            string SaveStatus = "N";
            string SentIMStatusCode = "";
            string RefNo = "";
            string Language = "";
            string referenceNoStatus = "";

            try
            {

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    Language = "T";
                }
                else
                {
                    Language = "E";
                }

                var FullUrl = full_url.ToSafeString() != null ? full_url.ToSafeString() : "";

                var Command = new SaveLeavemessageIMCommand
                {
                    p_language = Language,
                    p_service_speed = Model.SELECT_SERVICE,
                    p_cust_name = Model.NAME,
                    p_cust_surname = Model.SURNAME,
                    p_contact_mobile_no = Model.CONTACT_MOBILE,
                    p_is_ais_mobile = Model.MOBILE_IS_AIS,
                    p_contact_email = Model.EMAIL,
                    p_address_type = Model.SELECT_ADDRESS_TYPE,
                    p_building_name = Model.BUILDING,
                    p_house_no = Model.HOUSE_NO,
                    p_soi = Model.SOI,
                    p_road = Model.ROAD,
                    p_sub_district = Model.SUB_DISTRICT,
                    p_district = Model.DISTRICT,
                    p_province = Model.PROVINCE,
                    p_postal_code = Model.POSTAL_CODE,
                    p_contact_time = Model.CONTACT_TIME,
                    p_location_code = "",
                    p_asc_code = "",
                    p_channel = "IM_CALL_CENTER",
                    p_internet_no = "",
                    p_line_id = "",
                    p_voucher_desc = "",
                    p_campaign_project_name = "IM_CALL_CENTER",
                    p_rental_flag = Model.SELECT_TYPE_BUILD,
                    p_location_check_coverage = Model.LOCATION.ToSafeString(),
                    p_full_address = Model.FULL_ADDRESS,
                    p_url = FullUrl.ToSafeString(),
                    p_asset_number = Model.ASSET_NUMBER,
                    p_service_case_id = Model.SERVICE_CASE_ID,
                    p_building_no = Model.BUILDING_NO,
                    p_floor = Model.FLOOR,
                    p_moo = Model.MOO
                };
                _saveLeavemessageIMCommand.Handle(Command);
                //if (true)
                if (Command.return_code == 0)
                {
                    SaveStatus = "Y";
                    RefNo = Command.return_msg;
                }
                referenceNoStatus = Command.return_code.ToString();
            }
            catch (Exception ex)
            {
                referenceNoStatus = "-2";
            }

            try
            {
                //request GetLeaveMsgReferenceNo
                var Command = new GetLeaveMsgReferenceNoCommand();
                Command.referenceNoStatus = referenceNoStatus;
                Command.referenceNo = RefNo;
                Command.caseID = Model.SERVICE_CASE_ID;
                Command.contactMobileNo = Model.CONTACT_MOBILE;
                Command.customerName = Model.NAME;
                Command.customerLastName = Model.SURNAME;
                Command.inService = "YES";
                Command.addressAmphur = Model.DISTRICT;
                Command.addressBuilding = Model.BUILDING_NO;
                Command.addressFloor = Model.FLOOR;
                Command.addressMoo = Model.MOO;
                Command.addressMooban = Model.BUILDING;
                Command.addressNo = Model.HOUSE_NO;
                Command.addressPostCode = Model.POSTAL_CODE;
                Command.addressProvince = Model.PROVINCE;
                Command.addressRoad = Model.ROAD;
                Command.addressSoi = Model.SOI;
                Command.addressTumbol = Model.SUB_DISTRICT;
                string address_type = "";
                if (Model.SELECT_ADDRESS_TYPE == "B")
                {
                    address_type = Model.PRODUCT_TYPE_BUILDING;
                }
                else if (Model.SELECT_ADDRESS_TYPE == "V")
                {
                    address_type = Model.PRODUCT_TYPE_HOME;
                }
                Command.productType = address_type;
                _getLeaveMsgRefCommand.Handle(Command);
                SentIMStatusCode = Command.Return_Code;
            }
            catch (Exception ex)
            {
                SentIMStatusCode = "003";
            }


            if (SaveStatus == "Y" && SentIMStatusCode != "001")
            {
                var Command = new UpdatePreregisterStatusPackageCommand
                {
                    p_refference_no = RefNo,
                    p_status = status_im_update
                };
                _updatePregisterCommand.Handle(Command);
            }
            return RedirectToAction("IndexIM", new { SaveStatus = SaveStatus, RefNo = RefNo, SentIMStatusCode = SentIMStatusCode });
        }

        // Save Leavemessage I-X
        public ActionResult SaveRegisterPartner(LeavemessageIIPanelModel Model, string full_url, string mobile_is_ais_special = "")
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

            string p_latitude = "";
            string p_longitude = "";
            if (!string.IsNullOrEmpty(Model.LOCATION))
            {
                string[] tmpLocation = Model.LOCATION.Split(',');
                if (tmpLocation != null && tmpLocation.Count() == 2)
                {
                    p_latitude = tmpLocation[0].ToSafeString();
                    p_longitude = tmpLocation[1].ToSafeString();
                }
            }
            var mobile_is_ais = string.IsNullOrEmpty(mobile_is_ais_special.ToSafeString()) ? Model.MOBILE_IS_AIS : Model.MOBILE_IS_AIS + mobile_is_ais_special;
            var LovCampaignProjectName = GetLovList("FBB_CONSTANT", "CAMPAIGN_PROJECT_NAME");
            var FullUrl = full_url.ToSafeString() != null ? full_url.ToSafeString() : "";
            var ViewsRedirect = string.IsNullOrEmpty(full_url.ToSafeString()) ? "Index" : full_url.ToSafeString().Split('/')[full_url.ToSafeString().Split('/').Count() - 1].Substring(0, 7);
            var LovChannal = LovCampaignProjectName.Where(l => l.Text == ViewsRedirect).FirstOrDefault().LovValue1;
            //var LovCampaign = LovCampaignProjectName.Where(l => l.Text == ViewsRedirect).FirstOrDefault().LovValue2;

            var Command = new SaveLeavemessagePNCommand
            {
                p_language = Language,
                p_service_speed = Model.SELECT_SERVICE,
                p_cust_name = Model.NAME,
                p_cust_surname = Model.SURNAME,
                p_contact_mobile_no = Model.CONTACT_MOBILE,
                //p_is_ais_mobile = Model.MOBILE_IS_AIS,
                p_is_ais_mobile = mobile_is_ais,
                p_contact_email = Model.EMAIL,
                p_address_type = Model.SELECT_ADDRESS_TYPE,
                p_building_name = Model.BUILDING,
                p_house_no = Model.HOUSE_NO,
                p_soi = Model.SOI,
                p_road = Model.ROAD,
                p_sub_district = Model.SUB_DISTRICT,
                p_district = Model.DISTRICT,
                p_province = Model.PROVINCE,
                p_postal_code = Model.POSTAL_CODE,
                p_contact_time = Model.CONTACT_TIME,
                p_rental_flag = Model.SELECT_TYPE_BUILD,

                p_location_code = Model.LOCATION_CODE.ToSafeString().Trim(),
                p_asc_code = Model.ASC_CODE.ToSafeString(),
                p_emp_id = Model.EMP_ID.ToSafeString(),
                p_sales_rep = Model.SALES_REP.ToSafeString(),

                p_channal = LovChannal.ToSafeString(),
                p_campaign = Model.COMPANY_NAME.ToSafeString().Trim(),
                p_url = FullUrl.ToSafeString(),
                p_latitude = p_latitude,
                p_longitude = p_longitude

            };
            _saveLeavemessagePNCommand.Handle(Command);
            if (Command.return_code == 0)
            {
                SaveStatus = "Y";
                RefNo = Command.return_message;
            }
            else
            {
                SaveStatus = "N";
            }

            return RedirectToAction(ViewsRedirect, new { SaveStatus = SaveStatus, RefNo = RefNo });
        }
        #endregion

        #region JsonResult

        public JsonResult GetServiceSpeed()
        {
            var SERVICE = new List<DropdownModel>();
            try
            {
                SERVICE = GetDropDownConfig("FBBOR021", "FBB_CONSTANT", "L_SERVICE_SPEED");
            }
            catch (Exception) { }

            return Json(SERVICE, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContactTime(string pageCode, string name)
        {
            var CONTACT = new List<DropdownModel>();
            try
            {
                CONTACT = GetDropDownConfig(pageCode, "FBB_CONSTANT", name);
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

        #endregion

        #region method private

        private List<DropdownModel> GetDropDownConfig(string pageCode, string type, string name)
        {
            return base.LovData
                        .Where(l => l.LovValue5 == pageCode & l.Type == type & l.Name == name)
                        .Select(l => new DropdownModel
                        {
                            Text = SiteSession.CurrentUICulture.IsThaiCulture() ? l.LovValue1 : l.LovValue2,
                            Value = l.Text,
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

        #endregion

    }
}
