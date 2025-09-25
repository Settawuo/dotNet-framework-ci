using System;
using System.Linq;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public class CheckCardByLaserController : WBBController
    {
        //
        // GET: /LoveTest/

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<NotificationCommand> _noticeCommand;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<CustRegisterCommand> _custRegCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<UpdateFileNameCommand> _updateFileNameCommand;
        private readonly ICommandHandler<SavePaymentLogCommand> _savePaymentLogCommand;
        private readonly ICommandHandler<CustRegisterJobCommand> _custRegJobCommand;

        public CheckCardByLaserController(IQueryProcessor queryProcessor,
            ICommandHandler<NotificationCommand> noticeCommand,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
            ICommandHandler<CustRegisterCommand> custRegCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<UpdateFileNameCommand> UpdateFileNameCommand,
            ICommandHandler<SavePaymentLogCommand> savePaymentLogCommand,
            ICommandHandler<CustRegisterJobCommand> custRegJobCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _noticeCommand = noticeCommand;
            _sendSmsCommand = SendSmsCommand;
            _custRegCommand = custRegCommand;
            _mailLogCommand = mailLogCommand;
            _intfLogCommand = intfLogCommand;
            _updateFileNameCommand = UpdateFileNameCommand;
            _savePaymentLogCommand = savePaymentLogCommand;
            _custRegJobCommand = custRegJobCommand;
            base.Logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CallWebService()
        {
            // Fix ไว้ก่อน เดี๋ยวกลับมาแก้ไข
            SiteSession.CurrentUICulture = 1;

            CheckCardByLaserMappingModel result;
            try
            {
                var lang = "";

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    lang = "T";
                }
                else
                {
                    lang = "E";
                }

                var resultlov = base.LovData.Where(lov => lov.Type == "DOPA_CHK_IDCARD");

                var result_xssborigin = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-origin") ?? new LovValueModel();
                var result_xssbserviceorigin = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-service-origin") ?? new LovValueModel();
                var result_xssborderchannel = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-order-channel") ?? new LovValueModel();
                var result_xssbversion = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-version") ?? new LovValueModel();
                var result_Url = resultlov.FirstOrDefault(lov => lov.Name == "url") ?? new LovValueModel();
                var result_Method = resultlov.FirstOrDefault(lov => lov.Name == "command") ?? new LovValueModel();

                var uuid = GenerateRandomNo();
                var transaction = DateTime.Now.ToString("yyyyMMddHHmmfff");

                var result_xssbtransactionid = transaction + uuid;

                var query = new CheckCardByLaserQuery
                {
                    xssborigin = result_xssborigin.LovValue1,
                    xssbserviceorigin = result_xssbserviceorigin.LovValue1,
                    xssbtransactionid = result_xssbtransactionid,
                    xssborderchannel = result_xssborderchannel.LovValue1,
                    xssbversion = result_xssbversion.LovValue1,
                    Url = result_Url.LovValue1,
                    Method = result_Method.LovValue1,
                    Lang = lang,
                    Body = new CheckCardByLaserBody
                    {
                        firstName = "อติพนธ์",
                        lastName = "วิภาสมงคล",
                        idCardNo = "1209700588021",
                        laserID = "JT3104550269",
                        birthday = "25380205"
                    }
                };

                result = _queryProcessor.Execute(query);
            }
            catch (Exception)
            {
                result = new CheckCardByLaserMappingModel();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
    }
}
