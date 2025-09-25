using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]

    public class ProfilePrePostPaidController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<StatLogCommand> _StatLogCommand;

        public ProfilePrePostPaidController(IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<CustRegisterCommand> custRegCommand,
            ICommandHandler<NotificationCommand> noticeCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ICommandHandler<StatLogCommand> StatLogCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _StatLogCommand = StatLogCommand;
            base.Logger = logger;
        }


        //
        // GET: /ProfilePrePostPaid/

        public ActionResult Index()
        {
            if (Session["EndProcessFlag"].ToSafeBoolean())
            {
                Session["PopupStatus"] = "Success";
                Session["EndProcessFlag"] = null;
            }
            else
                Session["PopupStatus"] = null;

            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            SaveStatlog("CUSTOMER", "CUSTOMER", ipAddress, "FBB chkPrePostPaid", "", "");

            ViewBag.User = base.CurrentUser;


            ViewBag.LabelFBBOR015 = GetProfilePrePostPaid();
            ViewBag.FbbConstant = GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);

            ViewBag.Vas = "";

            return View();
        }

        public void SaveStatlog(string username = "", string VisitType = "", string REQ_IPADDRESS = "", string SELECT_PAGE = "", string HOST = "", string LC = "")
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

        public List<LovScreenValueModel> GetProfilePrePostPaid()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CheckPrePostPaid);
            return screenData;
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




    }
}
