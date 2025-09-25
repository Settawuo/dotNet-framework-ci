using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using WBBBusinessLayer;
using WBBBusinessLayer.CommandHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBQueryOrderPendingPayment
{
    public class FBBQueryOrderPendingPaymentJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SavePaymentLogCommand> _savePaymentLogCommand;
        private readonly ICommandHandler<UpdatOrderPaymentStatusCommand> _updatOrderPaymentStatusCommand;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<CustRegisterJobCommand> _custRegJobCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly ICommandHandler<UpdateFileNameCommand> _updateFileNameCommand;
        private readonly ICommandHandler<NotificationCommand> _noticeCommand;
        private readonly ICommandHandler<CreateOrderMeshPromotionCommand> _createOrderMeshPromotionCommand;
        private Stopwatch _timer;

        public FBBQueryOrderPendingPaymentJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SavePaymentLogCommand> savePaymentLogCommand,
            ICommandHandler<UpdatOrderPaymentStatusCommand> updatOrderPaymentStatusCommand,
            ICommandHandler<SendSmsCommand> sendSmsCommand,
            ICommandHandler<CustRegisterJobCommand> custRegJobCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ICommandHandler<UpdateFileNameCommand> updateFileNameCommand,
            ICommandHandler<NotificationCommand> noticeCommand,
            ICommandHandler<CreateOrderMeshPromotionCommand> createOrderMeshPromotionCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _savePaymentLogCommand = savePaymentLogCommand;
            _updatOrderPaymentStatusCommand = updatOrderPaymentStatusCommand;
            _sendSmsCommand = sendSmsCommand;
            _custRegJobCommand = custRegJobCommand;
            _mailLogCommand = mailLogCommand;
            _updateFileNameCommand = updateFileNameCommand;
            _noticeCommand = noticeCommand;
            _createOrderMeshPromotionCommand = createOrderMeshPromotionCommand;
        }

        public GetORDPendingPaymentModel GetORDPendingPayment(string payment_method)
        {
            GetORDPendingPaymentQuery query = new GetORDPendingPaymentQuery()
            {
                p_payment_method = payment_method
            };
            GetORDPendingPaymentModel Results = new GetORDPendingPaymentModel();
            try
            {
                Results = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {

            }
            return Results;
        }

        public CreateOrderPendingPaymentModel CreateOrderPendingPayment(string payment_method = "")
        {
            var query = new CreateOrderPendingPaymentQuery()
            {
                payment_method = payment_method,
                UpdateBy = "BATCH"
            };

            try
            {
                var results = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw;
            }
            return new CreateOrderPendingPaymentModel();
        }


        public GetORDPendingPaymentTimeOutModel GetORDPendingPaymentTimeOut()
        {
            GetORDPendingPaymentTimeOutQuery query = new GetORDPendingPaymentTimeOutQuery();
            GetORDPendingPaymentTimeOutModel Results = new GetORDPendingPaymentTimeOutModel();
            try
            {
                Results = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {

            }
            return Results;
        }

        public GetListORDDetailCreateModel GetListORDDetailCreate(string orderID)
        {
            GetListORDDetailCreateModel data = new GetListORDDetailCreateModel();

            GetListORDDetailCreateQuery query = new GetListORDDetailCreateQuery
            {
                p_order_id = orderID
            };
            data = _queryProcessor.Execute(query);

            return data;

        }

        public void SavePaymentLog(SavePaymentLogModel model)
        {
            var commamd = new SavePaymentLogCommand
            {
                p_action = model.ACTION,
                p_payment_order_id = model.PAYMENT_ORDER_ID,
                p_process_name = model.PROCESS_NAME,
                p_endpoint = model.ENDPOINT,

                p_req_project_code = model.REQ_PROJECT_CODE,
                p_req_command = model.REQ_COMMAND,
                p_req_sid = model.REQ_SID,
                p_req_redirect_url = model.REQ_REDIRECT_URL,
                p_req_merchant_id = model.REQ_MERCHANT_ID,
                p_req_order_id = model.REQ_ORDER_ID,
                p_req_currency = model.REQ_CURRENCY,
                p_req_purchase_amt = model.REQ_PURCHASE_AMT,
                p_req_payment_method = model.REQ_PAYMENT_METHOD,
                p_req_product_desc = model.REQ_PRODUCT_DESC,
                p_req_ref1 = model.REQ_REF1,
                p_req_ref2 = model.REQ_REF2,
                p_req_ref3 = model.REQ_REF3,
                p_req_ref4 = model.REQ_REF4,
                p_req_ref5 = model.REQ_REF5,
                p_req_integrity_str = model.REQ_INTEGRITY_STR,
                p_req_sms_flag = model.REQ_SMS_FLAG,
                p_req_sms_mobile = model.REQ_SMS_MOBILE,
                p_req_mobile_no = model.REQ_MOBILE_NO,
                p_req_token_key = model.REQ_TOKEN_KEY,
                p_req_order_expire = model.REQ_ORDER_EXPIRE,
                p_req_tran_id = model.REQ_TRAN_ID,

                p_resp_status = model.RESP_STATUS,
                p_resp_resp_code = model.RESP_RESP_CODE,
                p_resp_resp_desc = model.RESP_RESP_DESC,
                p_resp_sale_id = model.RESP_SALE_ID,
                p_resp_endpoint_url = model.RESP_ENDPOINT_URL,
                p_resp_detail1 = model.RESP_DETAIL1,
                p_resp_detail2 = model.RESP_DETAIL2,
                p_resp_detail3 = model.RESP_DETAIL3,

                p_post_status = model.POST_STATUS,
                p_post_resp_code = model.POST_RESP_CODE,
                p_post_resp_desc = model.POST_RESP_DESC,
                p_post_tran_id = model.POST_TRAN_ID,
                p_post_sale_id = model.POST_SALE_ID,
                p_post_order_id = model.POST_ORDER_ID,
                p_post_currency = model.POST_CURRENCY,
                p_post_exchange_rate = model.POST_EXCHANGE_RATE,
                p_post_purchase_amt = model.POST_PURCHASE_AMT,
                p_post_amount = model.POST_AMOUNT,
                p_post_inc_customer_fee = model.POST_INC_CUSTOMER_FEE,
                p_post_exc_customer_fee = model.POST_EXC_CUSTOMER_FEE,
                p_post_payment_status = model.POST_PAYMENT_STATUS,
                p_post_payment_code = model.POST_PAYMENT_CODE,
                p_post_order_expire_date = model.POST_ORDER_EXPIRE_DATE,

                Transaction_Id = "FBBQueryOrderPendingPayment",
                FullUrl = ""
            };
            _savePaymentLogCommand.Handle(commamd);
        }

        public bool CheckOrderPaymentStatus()
        {
            bool CheckOrderPayment = false;
            string CheckOrderPaymentMsg = "";
            var query = new CheckOrderPaymentStatusQuery();
            CheckOrderPaymentMsg = _queryProcessor.Execute(query);
            if (CheckOrderPaymentMsg == "Y")
            {
                CheckOrderPayment = true;
            }
            return CheckOrderPayment;
        }

        public void UpdatOrderPaymentStatus(string status)
        {
            var commamd = new UpdatOrderPaymentStatusCommand
            {
                Status = status
            };
            _updatOrderPaymentStatusCommand.Handle(commamd);
        }

        public SaveOrderResp GetSaveOrderResp(GetListORDDetailCreateModel model)
        {
            GetSaveOrderRespJobQuery query = new GetSaveOrderRespJobQuery();
            if (model.ODRDetailCustomerList != null && model.ODRDetailCustomerList.Count > 0)
            {
                foreach (var item in model.ODRDetailCustomerList)
                {
                    query.CUSTOMER_TYPE = item.customer_type.ToSafeString();
                    query.CUSTOMER_SUBTYPE = item.customer_subtype.ToSafeString();
                    query.TITLE_CODE = item.title_code.ToSafeString();
                    query.FIRST_NAME = item.first_name.ToSafeString();
                    query.LAST_NAME = item.last_name.ToSafeString();
                    query.CONTACT_TITLE_CODE = item.contact_title_code.ToSafeString();
                    query.CONTACT_FIRST_NAME = item.contact_first_name.ToSafeString();
                    query.CONTACT_LAST_NAME = item.contact_last_name.ToSafeString();
                    query.ID_CARD_TYPE_DESC = item.id_card_type_desc.ToSafeString();
                    query.ID_CARD_NO = item.id_card_no.ToSafeString();
                    query.TAX_ID = item.tax_id.ToSafeString();
                    query.GENDER = item.gender.ToSafeString();
                    query.BIRTH_DATE = item.birth_date.ToSafeString();
                    query.MOBILE_NO = item.mobile_no.ToSafeString();
                    query.MOBILE_NO_2 = item.mobile_no_2.ToSafeString();
                    query.HOME_PHONE_NO = item.home_phone_no.ToSafeString();
                    query.EMAIL_ADDRESS = item.email_address.ToSafeString();
                    query.CONTACT_TIME = item.contact_time.ToSafeString();
                    query.NATIONALITY_DESC = item.nationality_desc.ToSafeString();
                    query.CUSTOMER_REMARK = item.customer_remark.ToSafeString();
                    query.HOUSE_NO = item.house_no.ToSafeString();
                    query.MOO_NO = item.moo_no.ToSafeString();
                    query.BUILDING = item.building.ToSafeString();
                    query.FLOOR = item.floor.ToSafeString();
                    query.ROOM = item.room.ToSafeString();
                    query.MOOBAN = item.mooban.ToSafeString();
                    query.SOI = item.soi.ToSafeString();
                    query.ROAD = item.road.ToSafeString();
                    query.ZIPCODE_ROWID = item.zipcode_rowid.ToSafeString();
                    query.LATITUDE = item.latitude.ToSafeString();
                    query.LONGTITUDE = item.longtitude.ToSafeString();
                    query.ASC_CODE = item.asc_code.ToSafeString();
                    query.EMPLOYEE_ID = item.employee_id.ToSafeString();
                    query.LOCATION_CODE = item.location_code.ToSafeString();
                    query.SALE_REPRESENT = item.sale_represent.ToSafeString();
                    query.CS_NOTE = item.cs_note.ToSafeString();
                    query.WIFI_ACCESS_POINT = item.wifi_access_point.ToSafeString();
                    query.INSTALL_STATUS = item.install_status.ToSafeString();
                    query.COVERAGE = item.coverage.ToSafeString();
                    query.EXISTING_AIRNET_NO = item.existing_airnet_no.ToSafeString();
                    query.GSM_MOBILE_NO = item.gsm_mobile_no.ToSafeString();
                    query.CONTACT_NAME_1 = item.contact_name_1.ToSafeString();
                    query.CONTACT_NAME_2 = item.contact_name_2.ToSafeString();
                    query.CONTACT_MOBILE_NO_1 = item.contact_mobile_no_1.ToSafeString();
                    query.CONTACT_MOBILE_NO_2 = item.contact_mobile_no_2.ToSafeString();
                    query.CONDO_FLOOR = item.condo_floor.ToSafeString();
                    query.CONDO_ROOF_TOP = item.condo_roof_top.ToSafeString();
                    query.CONDO_BALCONY = item.condo_balcony.ToSafeString();
                    query.BALCONY_NORTH = item.balcony_north.ToSafeString();
                    query.BALCONY_SOUTH = item.balcony_south.ToSafeString();
                    query.BALCONY_EAST = item.balcony_east.ToSafeString();
                    query.BALCONY_WAST = item.balcony_wast.ToSafeString();
                    query.HIGH_BUILDING = item.high_building.ToSafeString();
                    query.HIGH_TREE = item.high_tree.ToSafeString();
                    query.BILLBOARD = item.billboard.ToSafeString();
                    query.EXPRESSWAY = item.expressway.ToSafeString();
                    query.ADDRESS_TYPE_WIRE = item.address_type_wire.ToSafeString();
                    query.ADDRESS_TYPE = item.address_type.ToSafeString();
                    query.FLOOR_NO = item.floor_no.ToSafeString();
                    query.HOUSE_NO_BL = item.house_no_bl.ToSafeString();
                    query.MOO_NO_BL = item.moo_no_bl.ToSafeString();
                    query.BUILDING_BL = item.building_bl.ToSafeString();
                    query.FLOOR_BL = item.floor_bl.ToSafeString();
                    query.ROOM_BL = item.room_bl.ToSafeString();
                    query.MOOBAN_BL = item.mooban_bl.ToSafeString();
                    query.SOI_BL = item.soi_bl.ToSafeString();
                    query.ROAD_BL = item.road_bl.ToSafeString();
                    query.ZIPCODE_ROWID_BL = item.zipcode_rowid_bl.ToSafeString();
                    query.HOUSE_NO_VT = item.house_no_vt.ToSafeString();
                    query.MOO_NO_VT = item.moo_no_vt.ToSafeString();
                    query.BUILDING_VT = item.building_vt.ToSafeString();
                    query.FLOOR_VT = item.floor_vt.ToSafeString();
                    query.ROOM_VT = item.room_vt.ToSafeString();
                    query.MOOBAN_VT = item.mooban_vt.ToSafeString();
                    query.SOI_VT = item.soi_vt.ToSafeString();
                    query.ROAD_VT = item.road_vt.ToSafeString();
                    query.ZIPCODE_ROWID_VT = item.zipcode_rowid_vt.ToSafeString();
                    query.CVR_ID = item.cvr_id.ToSafeString();
                    query.CVR_NODE = item.cvr_node.ToSafeString();
                    query.CVR_TOWER = item.cvr_tower.ToSafeString();
                    query.RELATE_MOBILE = item.relate_mobile.ToSafeString();
                    query.RELATE_NON_MOBILE = item.relate_non_mobile.ToSafeString();
                    query.SFF_CA_NO = item.sff_ca_no.ToSafeString();
                    query.SFF_SA_NO = item.sff_sa_no.ToSafeString();
                    query.SFF_BA_NO = item.sff_ba_no.ToSafeString();
                    query.NETWORK_TYPE = item.network_type.ToSafeString();
                    query.SERVICE_DAY = item.service_day.ToSafeString();
                    query.EXPECT_INSTALL_DATE = item.expect_install_date.ToSafeString();
                    query.SERVICE_DAYSpecified = item.service_dayspecified; // xxx
                    query.FTTX_VENDOR = item.fttx_vendor.ToSafeString();
                    query.INSTALL_NOTE = item.install_note.ToSafeString();
                    query.PHONE_FLAG = item.phone_flag.ToSafeString();
                    query.TIME_SLOT = item.time_slot.ToSafeString();
                    query.INSTALLATION_CAPACITY = item.installation_capacity.ToSafeString();
                    query.ADDRESS_ID = item.address_id.ToSafeString();
                    query.ACCESS_MODE = item.access_mode.ToSafeString();
                    query.ENG_FLAG = item.eng_flag.ToSafeString();
                    query.EVENT_CODE = item.event_code.ToSafeString();
                    query.INSTALLADDRESS1 = item.installaddress1.ToSafeString();
                    query.INSTALLADDRESS2 = item.installaddress2.ToSafeString();
                    query.INSTALLADDRESS3 = item.installaddress3.ToSafeString();
                    query.INSTALLADDRESS4 = item.installaddress4.ToSafeString();
                    query.INSTALLADDRESS5 = item.installaddress5.ToSafeString();
                    query.PBOX_COUNT = item.pbox_count.ToSafeString();
                    query.CONVERGENCE_FLAG = item.convergence_flag.ToSafeString();
                    query.TIME_SLOT_ID = item.time_slot_id.ToSafeString();
                    query.GIFT_VOUCHER = item.gift_voucher.ToSafeString();
                    query.SUB_LOCATION_ID = item.sub_location_id.ToSafeString();
                    query.SUB_CONTRACT_NAME = item.sub_contract_name.ToSafeString();
                    query.INSTALL_STAFF_ID = item.install_staff_id.ToSafeString();
                    query.INSTALL_STAFF_NAME = item.install_staff_name.ToSafeString();
                    query.FLOW_FLAG = item.flow_flag.ToSafeString();
                    query.SITE_CODE = item.site_code.ToSafeString();
                    query.LINE_ID = item.line_id.ToSafeString();
                    query.RELATE_PROJECT_NAME = item.relate_project_name.ToSafeString();
                    query.PLUG_AND_PLAY_FLAG = item.plug_and_play_flag.ToSafeString();
                    query.RESERVED_ID = item.reserved_id.ToSafeString();
                    query.JOB_ORDER_TYPE = item.job_order_type.ToSafeString();
                    query.ASSIGN_RULE = item.assign_rule.ToSafeString();
                    query.OLD_ISP = item.old_isp.ToSafeString();
                    query.SPLITTER_FLAG = item.splitter_flag.ToSafeString();
                    query.RESERVED_PORT_ID = item.reserved_port_id.ToSafeString();
                    query.SPECIAL_REMARK = item.special_remark.ToSafeString();
                    query.ORDER_NO = item.order_no.ToSafeString();
                    query.SOURCE_SYSTEM = item.source_system.ToSafeString();
                    query.BILL_MEDIA = item.bill_media.ToSafeString();
                    query.PRE_ORDER_NO = item.pre_order_no.ToSafeString();
                    query.VOUCHER_DESC = item.voucher_desc.ToSafeString();
                    query.CAMPAIGN_PROJECT_NAME = item.campaign_project_name.ToSafeString();
                    query.PRE_ORDER_CHANEL = item.pre_order_chanel.ToSafeString();
                    query.RENTAL_FLAG = item.rental_flag.ToSafeString();
                    query.DEV_PROJECT_CODE = item.dev_project_code.ToSafeString();
                    query.DEV_BILL_TO = item.dev_bill_to.ToSafeString();
                    query.DEV_PO_NO = item.dev_po_no.ToSafeString();
                    query.PARTNER_TYPE = item.partner_type.ToSafeString();
                    query.PARTNER_SUBTYPE = item.partner_subtype.ToSafeString();
                    query.MOBILE_BY_ASC = item.mobile_by_asc.ToSafeString();
                    query.LOCATION_NAME = item.location_name.ToSafeString();
                    query.PAYMENTMETHOD = item.paymentmethod.ToSafeString();
                    query.TRANSACTIONID_IN = item.transactionid_in.ToSafeString();
                    query.TRANSACTIONID = item.transactionid.ToSafeString();
                    query.SUB_ACCESS_MODE = item.sub_access_mode.ToSafeString();
                    query.REQUEST_SUB_FLAG = item.request_sub_flag.ToSafeString();
                    query.PREMIUM_FLAG = item.premium_flag.ToSafeString();
                    query.RELATE_MOBILE_SEGMENT = item.relate_mobile_segment.ToSafeString();
                    query.REF_UR_NO = item.ref_ur_no.ToSafeString();
                    query.LOCATION_EMAIL_BY_REGION = item.location_email_by_region.ToSafeString();
                    //20.3
                    //Report channel
                    query.COMPANY_NAME = item.company_name.ToSafeString();
                    query.DISTRIBUTION_CHANNEL = item.distribution_channel.ToSafeString();
                    query.CHANNEL_SALES_GROUP = item.channel_sales_group.ToSafeString();
                    query.SHOP_TYPE = item.shop_type.ToSafeString();
                    query.SHOP_SEGMENT = item.shop_segment.ToSafeString();
                    query.ASC_NAME = item.asc_name.ToSafeString();
                    query.ASC_MEMBER_CATEGORY = item.asc_member_category.ToSafeString();
                    query.ASC_POSITION = item.asc_position.ToSafeString();
                    query.LOCATION_REGION = item.location_region.ToSafeString();
                    query.LOCATION_SUB_REGION = item.location_sub_region.ToSafeString();
                    query.EMPLOYEE_NAME = item.employee_name.ToSafeString();
                    //Service Level
                    query.SERVICE_LEVEL = item.service_level.ToSafeString();
                    query.AMENDMENT_FLAG = item.amendment_flag.ToSafeString();
                    //Sale digitize + Other
                    query.ORDER_RELATE_CHANGE_PRO = "";
                    query.CUSTOMERPURGE = item.customerpurge.ToSafeString();
                    query.EXCEPTENTRYFEE = item.exceptentryfee.ToSafeString();
                    query.SECONDINSTALLATION = item.secondinstallation.ToSafeString();
                    query.FIRST_INSTALL_DATE = item.first_install_date.ToSafeString();
                    query.FIRST_TIME_SLOT = item.first_time_slot.ToSafeString();
                }
            }
            if (model.ODRDetailPackageList != null && model.ODRDetailPackageList.Count() > 0)
            {
                List<REGIST_PACKAGE> RegistPackageList = new List<REGIST_PACKAGE>();
                foreach (var item in model.ODRDetailPackageList)
                {
                    REGIST_PACKAGE RegistPackage = new REGIST_PACKAGE
                    {
                        faxFlag = item.fax_flag.ToSafeString(),
                        homeIp = item.home_ip.ToSafeString(),
                        homePort = item.home_port.ToSafeString(),
                        iddFlag = item.idd_flag.ToSafeString(),
                        mobileForward = item.mobile_forward.ToSafeString(),
                        packageCode = item.package_code.ToSafeString(),
                        packagePrice = item.package_price,
                        packagePriceSpecified = true,
                        packageType = item.package_type.ToSafeString(),
                        pboxExt = item.pbox_ext.ToSafeString(),
                        productSubtype = item.product_subtype.ToSafeString(),
                        tempIa = item.temp_ia.ToSafeString()
                    };
                    RegistPackageList.Add(RegistPackage);
                }
                query.REGIST_PACKAGE_LIST = RegistPackageList;
            }

            if (model.ODRDetailFileList != null && model.ODRDetailFileList.Count() > 0)
            {
                List<REGIST_FILE> RegistFileList = new List<REGIST_FILE>();
                foreach (var item in model.ODRDetailFileList)
                {
                    REGIST_FILE RegistFile = new REGIST_FILE
                    {
                        fileName = item.file_name.ToSafeString()
                    };
                    RegistFileList.Add(RegistFile);
                }
                query.REGIST_FILE_LIST = RegistFileList;
            }

            if (model.ODRDetailSplitterList != null && model.ODRDetailSplitterList.Count() > 0)
            {
                List<REGIST_SPLITTER> RegistSplitterList = new List<REGIST_SPLITTER>();
                foreach (var item in model.ODRDetailSplitterList)
                {
                    REGIST_SPLITTER RegistSplitter = new REGIST_SPLITTER
                    {
                        distance = item.distance,
                        distanceSpecified = true,
                        distanceType = item.distance_type.ToSafeString(),
                        resourceType = item.resource_type.ToSafeString(),
                        splitterName = item.splitter_name.ToSafeString()
                    };
                    RegistSplitterList.Add(RegistSplitter);
                }
                query.REGIST_SPLITTER_LIST = RegistSplitterList;
            }

            if (model.ODRDetailCPEList != null && model.ODRDetailCPEList.Count() > 0)
            {
                List<REGIST_CPE_SERIAL> RegistCPEList = new List<REGIST_CPE_SERIAL>();
                foreach (var item in model.ODRDetailCPEList)
                {
                    REGIST_CPE_SERIAL RegistCPE = new REGIST_CPE_SERIAL
                    {
                        cpeType = item.cpe_type.ToSafeString(),
                        macAddress = item.mac_address.ToSafeString(),
                        serialNo = item.serial_no.ToSafeString()
                    };
                    if (RegistCPE.serialNo != null && RegistCPE.serialNo != "")
                    {
                        RegistCPEList.Add(RegistCPE);
                    }
                }

                query.REGIST_CPE_SERIAL_LIST = RegistCPEList;
            }

            var data = _queryProcessor.Execute(query);
            return data;
        }

        public string RegisterCustomer(GetListORDDetailCreateModel model)
        {
            CustRegisterJobCommand command = new CustRegisterJobCommand()
            {
                RETURN_IA_NO = model.return_ia_no,
                RETURN_ORDER_NO = model.return_order_no,
                TRANSACTIONID_IN = model.ODRDetailCustomerList[0].transactionid_in,
                TRANSACTIONID = model.ODRDetailCustomerList[0].transactionid,
                PAYMENTMETHOD = model.ODRDetailCustomerList[0].paymentmethod,
                PLUG_AND_PLAY_FLAG = model.ODRDetailCustomerList[0].plug_and_play_flag,
                ClientIP = "",
                FullUrl = ""
            };
            _custRegJobCommand.Handle(command);

            return command.CUSTOMERID;
        }

        public string RegisterCustomerMesh(string SffOrder, string oderID, string transactionID, string paymentMethod)
        {
            CustRegisterJobCommand command = new CustRegisterJobCommand()
            {
                RETURN_IA_NO = SffOrder,
                RETURN_ORDER_NO = SffOrder,
                TRANSACTIONID_IN = oderID,
                TRANSACTIONID = transactionID,
                PAYMENTMETHOD = paymentMethod,
                PLUG_AND_PLAY_FLAG = "",
                REGISTER_TYPE = "TOPUP_MESH",
                ClientIP = "",
                FullUrl = ""
            };
            _custRegJobCommand.Handle(command);

            return command.CUSTOMERID;
        }

        public void GenPDFAndSendEmail(string custRowId, string L_CARD_NO, string OrderNo, string L_CONTACT_PHONE, string mailTo, bool IsThaiCulture, string user, string pass, string ip, string imagepathimer)
        {
            int CurrentUICulture = 2;
            string langPDFAPP = "E";
            if (IsThaiCulture)
            {
                CurrentUICulture = 1;
                langPDFAPP = "T";
            }

            #region PDF

            var running_no = InsertMailLog(custRowId);

            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
            string filename = "Request" + DateTime.Now.ToString("ddMMyy", format) + "_" + running_no.ToSafeString();
            string directoryPathApp = "";

            @directoryPathApp = GeneratePDFApp(L_CARD_NO, OrderNo, langPDFAPP, L_CONTACT_PHONE, user, pass, ip, imagepathimer);

            #endregion PDF

            if (mailTo != "")
            {
                string filePathAppNASTemp = "";
                if (directoryPathApp != "")
                {
                    filePathAppNASTemp = directoryPathApp.Substring(2);
                }

                string filePathAppNAS = "";

                if (filePathAppNASTemp != "")
                {
                    filePathAppNAS = "\\\\" + ip + filePathAppNASTemp.Replace(filePathAppNASTemp.Split('\\')[0], "");
                }

                var command = new NotificationCommand
                {
                    CustomerId = custRowId,
                    CurrentCulture = CurrentUICulture,
                    RunningNo = running_no,
                    EmailModel = new EmailModel
                    {
                        MailTo = mailTo,
                        FilePath = "",
                        FilePath2 = filePathAppNAS,
                    },
                    ImpersonateUser = user,
                    ImpersonatePass = pass,
                    ImpersonateIP = ip
                };

                _noticeCommand.Handle(command);
            }
        }

        public List<LovValueModel> GetLov(string LovType = "", string LovName = "")
        {
            GetLovQuery query = new GetLovQuery
            {
                LovName = LovName,
                LovType = LovType
            };
            return _queryProcessor.Execute(query);
        }

        public decimal InsertMailLog(string customerId)
        {
            var command = new MailLogCommand
            {
                CustomerId = customerId,
            };

            _mailLogCommand.Handle(command);

            return command.RunningNo;
        }

        private Object thisLock = new Object();
        public void SendSMS(string mobileNo, string mainCode, bool IsThaiCulture, string msgtxt)
        {

            string[] strMainCode = null;
            if (!string.IsNullOrEmpty(mainCode))
            {
                strMainCode = mainCode.Split('|');
            }
            lock (thisLock)
            {
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                List<string> promotionCode = new List<string>();
                if (strMainCode != null)
                {
                    foreach (string mCode in strMainCode)
                    {
                        if (!string.IsNullOrEmpty(mCode))
                        {
                            promotionCode.Add(mCode);
                        }

                    }
                }

                var air_old_list = new List<AIR_CHANGE_OLD_PACKAGE_ARRAY>();
                foreach (var code in promotionCode)
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        var air = new AIR_CHANGE_OLD_PACKAGE_ARRAY()
                        {
                            enddt = "",
                            productSeq = "",
                            sffPromotionCode = code,
                            startdt = ""
                        };
                        air_old_list.Add(air);
                    }
                }

                List<ListShortNameModel> result = new List<ListShortNameModel>();

                if (air_old_list != null && air_old_list.Count > 0)
                {

                    var query = new GetListShortNamePackageQuery()
                    {
                        id_card_no = "",
                        airChangePromotionCode_List = air_old_list,
                        transaction_id = "",
                        FullUrl = ""
                    };
                    result = _queryProcessor.Execute(query);
                }

                var command = new SendSmsCommand();
                command.FullUrl = "";
                command.Source_Addr = "AISFIBRE";
                command.Destination_Addr = mobileNo;
                // Update 17.2
                command.Transaction_Id = "";
                var packageMainName = "";

                if (result != null && result.Count > 0)
                {
                    foreach (var name in result)
                    {
                        if (name.package_class == "MAIN")  //Main
                        {
                            if (IsThaiCulture)
                            {
                                packageMainName = name.package_Short_Name_TH;
                            }
                            else
                            {
                                packageMainName = name.package_Short_Name_EN;
                            }

                            msgtxt = msgtxt.Replace("{Package}", packageMainName);

                            command.Message_Text = msgtxt;
                            if (!string.IsNullOrEmpty(command.Message_Text))
                            {
                                _sendSmsCommand.Handle(command);
                                //Thread.Sleep(15000);
                            }
                            command.Message_Text = "";
                            msgtxt = "";

                        }
                    }

                }
                else
                {
                    command.Message_Text = msgtxt;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _sendSmsCommand.Handle(command);
                        //Thread.Sleep(15000);
                    }
                    command.Message_Text = "";
                    msgtxt = "";
                }

            }

        }

        private void UpdateFileName(string orderNo, string filename, string AisAirNumber)
        {

            var commamd = new UpdateFileNameCommand
            {
                OrderNo = orderNo,
                FileName = filename,
                Transaction_Id = "",
                FullUrl = ""
            };
            _updateFileNameCommand.Handle(commamd);

        }

        private string GeneratePDFApp(string CardNo, string orderNo, string Language, string contactNo, string user, string pass, string ip, string imagepathimer)
        {
            try
            {

                var html = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

                Byte[] bytes = null;

                PDFDataQuery query = new PDFDataQuery();
                query.orderNo = orderNo;
                query.Language = Language;
                query.isEApp = true;
                var htmlFromPackage = QueryGeneratePDF(query);

                html = html + htmlFromPackage;

                html = html.Replace("{Sign}", "");

                try
                {
                    bytes = htmlToPDF(html);
                }
                catch (Exception ex1)
                {
                    throw new Exception("htmlToPDF error. Error : " + ex1.Message);
                }

                bytes = PdfSecurity.SetPasswordPdf(bytes, CardNo);

                var queryName = new GetFormatFileNameEAPPQuery
                {
                    ID_CardNo = CardNo,
                };

                var result = _queryProcessor.Execute(queryName);

                string fileName = result.file_name;
                string yearweek = (DateTime.Now.Year.ToString());
                string monthyear = (DateTime.Now.Month.ToString("00"));

                string imagepathimerTemp = Path.Combine(imagepathimer, yearweek + monthyear);

                imagepathimer = imagepathimerTemp;

                _logger.Info("GeneratePDFApp : imagepathimer = " + imagepathimer);

                string pathfileImpesontae = "";

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    System.IO.Directory.CreateDirectory(@imagepathimer);

                    pathfileImpesontae = Path.Combine(imagepathimer, fileName + ".pdf");

                    _logger.Info("GeneratePDFApp : pathfileImpesontae = " + pathfileImpesontae);

                    if (bytes != null)
                    {
                        try
                        {
                            PdfSecurity.WriteFile(pathfileImpesontae, bytes);
                        }
                        catch (Exception ex2)
                        {
                            throw new Exception("PdfSecurity.WriteFile is Error : " + ex2.Message);
                        }

                        UpdateFileName(orderNo, pathfileImpesontae, contactNo);

                    }
                    else
                    {
                        _logger.Info("GeneratePDFApp : bytes file is null.");
                    }
                }

                return pathfileImpesontae;
            }
            catch (Exception ex)
            {
                _logger.Info("GeneratePDFApp : " + ex.GetBaseException());
                return "";
            }
        }

        private string QueryGeneratePDF(PDFDataQuery query)
        {
            var pdfData = new PDFData();

            try
            {
                pdfData = _queryProcessor.Execute(query);
                return pdfData.str_pdf_html;
            }
            catch (System.Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        private Byte[] htmlToPDF(string html)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var doc = new Document())
                {

                    doc.SetMargins(doc.LeftMargin / 4, doc.RightMargin / 4, doc.TopMargin, doc.BottomMargin);

                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {

                        doc.Open();
                        //doc.NewPage();

                        using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                        {

                            using (var sr = new StringReader(html))
                            {
                                //Path to our font
                                string fonttttts = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "fonts", "tahoma.ttf");
                                LogMsg("FontPath : " + fonttttts);
                                //Register the font with iTextSharp
                                try
                                {
                                    iTextSharp.text.FontFactory.Register(fonttttts);
                                    iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);
                                }
                                catch
                                {
                                    fonttttts = Path.Combine("D:\\Batch", "FBBQueryOrderPendingPayment", "Content", "fonts", "tahoma.ttf"); // Fix
                                    LogMsg("FontPathFix : " + fonttttts);
                                    iTextSharp.text.FontFactory.Register(fonttttts);
                                    iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);
                                }

                                //Create a new stylesheet
                                iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();
                                //Set the default body font to our registered font's internal name
                                ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FACE, "Tahoma");
                                //Set the default encoding to support Unicode characters
                                ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.ENCODING, BaseFont.IDENTITY_H);
                                ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONTSIZE, "4");

                                Dictionary<string, object> providers = new Dictionary<string, object>();

                                providers.Add(HTMLWorker.IMG_PROVIDER, new ImageThing(doc));

                                ////Parse our HTML using the stylesheet created above
                                List<IElement> list = HTMLWorker.ParseToList(sr, ST, providers);
                                //htmlWorker.Parse(sr);
                                try
                                {
                                    foreach (var element in list)
                                    {
                                        doc.Add(element);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Info("htmlToPDF (step loop element) : " + ex.GetBaseException());
                                }
                            }
                        }

                        doc.Close();
                    }
                }
                return ms.ToArray();
            }

        }

        private Byte[] htmlToPDF(string[] html)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var doc = new Document())
                {

                    doc.SetMargins(doc.LeftMargin / 2, doc.RightMargin / 2, doc.TopMargin, doc.BottomMargin);

                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {

                        doc.Open();

                        using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                        {
                            for (int i = 0; i < html.Count(); i++)
                            {
                                using (var sr = new StringReader(html[i]))
                                {
                                    string fonttttts = Directory.GetCurrentDirectory() + "\\Content\\fonts\\tahoma.ttf";
                                    iTextSharp.text.FontFactory.Register(fonttttts);
                                    iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);

                                    //Create a new stylesheet
                                    iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FACE, "Tahoma");
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.ENCODING, BaseFont.IDENTITY_H);
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONTSIZE, "4");

                                    Dictionary<string, object> providers = new Dictionary<string, object>();

                                    providers.Add(HTMLWorker.IMG_PROVIDER, new ImageThing(doc));

                                    List<IElement> list = HTMLWorker.ParseToList(sr, ST, providers);
                                    try
                                    {
                                        foreach (var element in list)
                                        {
                                            doc.Add(element);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                                if (i != html.Count() - 1)
                                    doc.NewPage();
                            }

                        }

                        doc.Close();
                    }
                }
                return ms.ToArray();
            }

        }

        public void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        public void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        public void LogMsg(string Msg)
        {
            _logger.Info(Msg);
        }

        public ReleaseTimeSlotModel ReleaseTimeSlot(string RESERVED_ID, string ORDER_ID)
        {
            ReleaseTimeSlotQuery query = new ReleaseTimeSlotQuery
            {
                RESERVED_ID = RESERVED_ID,
                ORDER_ID = ORDER_ID
            };
            return _queryProcessor.Execute(query);
        }

        public ResReleaseModel ResRelease(string RES_RESERVATION_ID, string ORDER_ID)
        {
            ResReleaseQuery query = new ResReleaseQuery
            {
                RES_RESERVATION_ID = RES_RESERVATION_ID,
                ORDER_ID = ORDER_ID
            };
            return _queryProcessor.Execute(query);
        }

        public GetOrderChangeServiceModel GetOrderChangeService(string internetNo, string paymentOrderID)
        {
            GetOrderChangeServiceQuery changeServiceQuery = new GetOrderChangeServiceQuery()
            {
                p_internet_no = internetNo,
                p_payment_order_id = paymentOrderID
            };

            GetOrderChangeServiceModel result = _queryProcessor.Execute(changeServiceQuery);

            return result;
        }

        public CreateOrderMeshPromotionResult CreateOrderMeshPromotion(GetOrderChangeServiceModel data, string NonMobileNo)
        {
            CreateOrderMeshPromotionResult result = new CreateOrderMeshPromotionResult();
            string FullUrl = "";

            // Get IP Address
            string ipAddress = "";

            CreateOrderMeshPromotionCommand createOrderMeshPromotionCommand = new CreateOrderMeshPromotionCommand()
            {
                GetOrderChangeService = data,
                NonMobileNo = NonMobileNo,
                FullUrl = FullUrl,
                client_ip = ipAddress
            };

            _createOrderMeshPromotionCommand.Handle(createOrderMeshPromotionCommand);

            if (createOrderMeshPromotionCommand.ERROR_MSG == "")
            {
                result.ret_code = createOrderMeshPromotionCommand.VALIDATE_FLAG.ToSafeString();
                result.ret_message = createOrderMeshPromotionCommand.ERROR_MSG.ToSafeString();
                result.order_no = createOrderMeshPromotionCommand.sffOrderNo.ToSafeString();
            }

            return result;
        }

        public GetMeshCustomerProfileModel GetMeshCustomerProfile(string internetNo, string paymentOrderID)
        {
            GetMeshCustomerProfileModel result = null;
            GetMeshCustomerProfileQuery query = new GetMeshCustomerProfileQuery()
            {
                p_internet_no = internetNo,
                p_payment_order_id = paymentOrderID
            };
            result = _queryProcessor.Execute(query);
            return result;
        }

        private Object MeshthisLock = new Object();

        public void MeshSendSMS(string mobileNo, string[] msgtxts)
        {
            string FullUrl = "";

            #region Get IP Address Interface Log : Edit 2017-01-30

            string transactionId = "";

            // Get IP Address
            string ipAddress = "";

            transactionId = ipAddress + "|";

            #endregion

            lock (MeshthisLock)
            {
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                foreach (var item in msgtxts)
                {
                    var command = new SendSmsCommand();
                    command.FullUrl = FullUrl;
                    command.Source_Addr = "AISFIBRE";
                    command.Destination_Addr = mobileNo;
                    command.Transaction_Id = transactionId;

                    command.Message_Text = item;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _sendSmsCommand.Handle(command);
                    }
                    command.Message_Text = "";
                }
            }
        }

        #region R20.9

        public string SendSMS(string mobileNo, string mainCode, bool IsThaiCulture, string msgtxt, string internetNo)
        {
            var resultsms = string.Empty;
            string[] strMainCode = null;
            if (!string.IsNullOrEmpty(mainCode))
            {
                strMainCode = mainCode.Split('|');
            }
            lock (thisLock)
            {
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                List<string> promotionCode = new List<string>();
                if (strMainCode != null)
                {
                    foreach (string mCode in strMainCode)
                    {
                        if (!string.IsNullOrEmpty(mCode))
                        {
                            promotionCode.Add(mCode);
                        }

                    }
                }

                var air_old_list = new List<AIR_CHANGE_OLD_PACKAGE_ARRAY>();
                foreach (var code in promotionCode)
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        var air = new AIR_CHANGE_OLD_PACKAGE_ARRAY()
                        {
                            enddt = "",
                            productSeq = "",
                            sffPromotionCode = code,
                            startdt = ""
                        };
                        air_old_list.Add(air);
                    }
                }

                List<ListShortNameModel> result = new List<ListShortNameModel>();

                if (air_old_list != null && air_old_list.Count > 0)
                {

                    var query = new GetListShortNamePackageQuery()
                    {
                        id_card_no = "",
                        airChangePromotionCode_List = air_old_list,
                        transaction_id = "",
                        FullUrl = ""
                    };
                    result = _queryProcessor.Execute(query);
                }

                var command = new SendSmsCommand();
                command.FullUrl = "";
                command.Source_Addr = "AISFIBRE";
                command.Destination_Addr = mobileNo;
                // Update 17.2
                command.Transaction_Id = internetNo;
                var packageMainName = "";

                if (result != null && result.Count > 0)
                {
                    foreach (var name in result)
                    {
                        if (name.package_class == "MAIN")  //Main
                        {
                            if (IsThaiCulture)
                            {
                                packageMainName = name.package_Short_Name_TH;
                            }
                            else
                            {
                                packageMainName = name.package_Short_Name_EN;
                            }

                            msgtxt = msgtxt.Replace("{Package}", packageMainName);

                            command.Message_Text = msgtxt;
                            if (!string.IsNullOrEmpty(command.Message_Text))
                            {
                                _sendSmsCommand.Handle(command);
                                //Thread.Sleep(15000);
                            }
                            command.Message_Text = "";
                            msgtxt = "";

                        }
                    }

                }
                else
                {
                    command.Message_Text = msgtxt;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _sendSmsCommand.Handle(command);
                        //Thread.Sleep(15000);
                    }
                    command.Message_Text = "";
                    msgtxt = "";
                }

                if (string.IsNullOrEmpty(resultsms))
                {
                    resultsms = command.return_status;
                }
            }

            return resultsms;
        }

        public string MeshSendSMS(string mobileNo, string[] msgtxts, string internetNo)
        {
            var resultsms = string.Empty;

            lock (MeshthisLock)
            {
                if (mobileNo.Substring(0, 2) != "66")
                {
                    if (mobileNo.Substring(0, 1) == "0")
                    {
                        mobileNo = "66" + mobileNo.Substring(1);
                    }
                }

                foreach (var item in msgtxts)
                {
                    var command = new SendSmsCommand();
                    command.FullUrl = "";
                    command.Source_Addr = "AISFIBRE";
                    command.Destination_Addr = mobileNo;
                    command.Transaction_Id = internetNo;

                    command.Message_Text = item;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _sendSmsCommand.Handle(command);

                    }
                    command.Message_Text = "";

                    if (string.IsNullOrEmpty(resultsms))
                    {
                        resultsms = command.return_status;
                    }
                }
            }

            return resultsms;
        }

        public bool CheckSMSFlag(string orderId, string status, string updateby, string ContactMobile, string fullUrl)
        {
            SMSFlagRegisterPendingQuery query = new SMSFlagRegisterPendingQuery()
            {
                Action = "Check",
                OrderId = orderId,
                Status = status,
                UpdateBy = updateby,
                Mobile_No = ContactMobile,
                FullUrl = fullUrl,
            };

            SMSFlagRegisterPendingModel result = new SMSFlagRegisterPendingModel();

            try
            {
                result = _queryProcessor.Execute(query);
                if (result != null && result.SendSMS_Flag != "Y") return true;
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateSMSFlag(string orderId, string status, string updateby, string ContactMobile, string fullUrl)
        {
            SMSFlagRegisterPendingQuery query = new SMSFlagRegisterPendingQuery()
            {
                Action = "Update",
                OrderId = orderId,
                Status = status,
                SMS_Flag = "Y",
                UpdateBy = updateby,
                Mobile_No = ContactMobile,
                FullUrl = fullUrl,
            };

            SMSFlagRegisterPendingModel result = new SMSFlagRegisterPendingModel();

            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }

    public class PdfSecurity
    {
        public static byte[] SetPasswordPdf(byte[] dataBytes, string keyPassword)
        {
            //SET PDF PASSWORD
            var pdfPwd = GetPassword(keyPassword ?? string.Empty);
            var pdfbyte = PdfSettingPassword(dataBytes, pdfPwd);

            return pdfbyte;
        }

        public static void WriteFile(string path, byte[] dataBytes)
        {
            if (dataBytes == null) return;

            using (var f = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                f.Write(dataBytes, 0, dataBytes.Length);
                f.Close();
            }
        }

        private static byte[] PdfSettingPassword(byte[] inputBytes, string pdfPassword)
        {
            byte[] resultBytes;

            if (!string.IsNullOrEmpty(pdfPassword))
            {
                using (var input = new MemoryStream(inputBytes))
                {
                    using (var output = new MemoryStream())
                    {
                        var reader = new PdfReader(input);
                        PdfEncryptor.Encrypt(reader, output, true, pdfPassword, pdfPassword,
                            PdfWriter.ALLOW_SCREENREADERS);

                        resultBytes = output.ToArray();
                    }
                }
            }
            else
            {
                resultBytes = inputBytes;
            }

            return resultBytes;

        }

        private static string GetPassword(string contactMobile)
        {
            const int resultLength = 4; //Fixed รหัส Passsword 4 ตัวท้าย
            var result = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(contactMobile))
                {
                    if (contactMobile.Length >= resultLength)
                    {
                        result = contactMobile.Substring(contactMobile.Length - resultLength, resultLength);
                    }
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }
    }

    public class ImageThing : IImageProvider
    {
        //Store a reference to the main document so that we can access the page size and margins
        private Document MainDoc = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc"></param>
        public ImageThing(Document doc)
        {
            this.MainDoc = doc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="attrs"></param>
        /// <param name="chain"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        Image IImageProvider.GetImage(string src, IDictionary<string, string> attrs, ChainedProperties chain, IDocListener doc)
        {
            //Prepend the src tag with our path. NOTE, when using HTMLWorker.IMG_PROVIDER, HTMLWorker.IMG_BASEURL gets ignored unless you choose to implement it on your own

            // Local image file
            //src = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + src;

            //Get the image. NOTE, this will attempt to download/copy the image, you'd really want to sanity check here
            Image img = null;
            if (src.IndexOf("data:image/png;base64,") < 0)
            {
                img = Image.GetInstance(src);
                //Make sure we got something
                if (img == null)
                    return null;
                img.ScalePercent(40);
            }
            else
            {
                Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(src.Replace("data:image/png;base64,", "")));
                img = Image.GetInstance(bitmapData);
                //Make sure we got something
                if (img == null)
                    return null;
                img.ScalePercent(40);
            }
            //If the downloaded image is bigger than either width and/or height then shrink it
            //if (img.Width > usableW || img.Height > usableH)
            //{
            //    img.ScaleToFit(usableW, usableH);
            //    img.ScaleAbsolute(img.Width, img.Height);
            //}

            //return our image
            return img;
        }

        public string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }

    }
}
