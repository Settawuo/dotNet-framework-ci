using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;

namespace WBBWeb.Controllers
{
    public class LeavemessageOnlineController : WBBController
    {
        //
        // GET: /LeavemessageOnline/

        #region Properties

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SaveLeavemessageCommand> _saveLeavemessageCommand;

        #endregion

        #region Constuctor

        public LeavemessageOnlineController(IQueryProcessor queryProcessor
              , ICommandHandler<SaveLeavemessageCommand> saveLeavemessageCommand
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _saveLeavemessageCommand = saveLeavemessageCommand;
            base.Logger = logger;
        }

        #endregion

        [HttpPost]
        public JsonResult Index(string Language = "",
                                    string ServiceSpeed = "",
                                    string CustName = "",
                                    string CustSurname = "",
                                    string ContactMobileNO = "",
                                    string IsAisMobile = "",
                                    string ContactEmail = "",
                                    string AddressType = "",
                                    string BuildingName = "",
                                    string HouseNo = "",
                                    string Soi = "",
                                    string Road = "",
                                    string SubDistrict = "",
                                    string District = "",
                                    string Province = "",
                                    string PostalCode = "",
                                    string ContactTime = "",
                                    string CampainProjectName = "",
                                    string TransactionID = "",
                                    string Channel = "",
                                    string LinkURL = "")
        {
            string URL = "";
            string VOUCHER_DESC = "";
            string RENTAL_FLAG = "";
            string LINE_ID = "";
            string FULL_ADDRESS = "";
            string LOCATION_CHECK_COVERAGE = "";
            string INTERNET_NO = "";
            string ASC_CODE = "";
            string LOCATION_CODE = "";
            string SaveStatus = "N";

            //if (LinkURL != "")
            //{
            URL = LinkURL;
            //}
            //else
            //{
            //    URL = this.Url.Action("", "LeavemessageOnline", null, this.Request.Url.Scheme);
            //}

            if (CustName != "")
            {
                var Command = new SaveLeavemessageCommand
                {
                    p_language = Language,
                    p_service_speed = ServiceSpeed,
                    p_cust_name = CustName,
                    p_cust_surname = CustSurname,
                    p_contact_mobile_no = ContactMobileNO,
                    p_is_ais_mobile = IsAisMobile,
                    p_contact_email = ContactEmail,
                    p_address_type = AddressType,
                    p_building_name = BuildingName,
                    p_house_no = HouseNo,
                    p_soi = Soi,
                    p_road = Road,
                    p_sub_district = SubDistrict,
                    p_district = District,
                    p_province = Province,
                    p_postal_code = PostalCode,
                    p_contact_time = ContactTime,
                    p_location_code = "",
                    p_asc_code = "",
                    p_channel = Channel,
                    p_internet_no = "",
                    p_line_id = "",
                    p_voucher_desc = "",
                    p_campaign_project_name = CampainProjectName,
                    p_rental_flag = "",
                    p_location_check_coverage = "",
                    p_full_address = "",
                    p_url = URL

                };
                _saveLeavemessageCommand.Handle(Command);
                if (Command.return_code == 0)
                {
                    SaveStatus = "Y";
                }
            }
            return Json(new { SaveStatus = SaveStatus }, JsonRequestBehavior.AllowGet);
        }

    }
}