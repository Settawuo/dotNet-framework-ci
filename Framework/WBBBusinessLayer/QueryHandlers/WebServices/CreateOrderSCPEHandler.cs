using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WBBBusinessLayer.CommandHandlers;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CreateOrderSCPEHandler : IQueryHandler<CreateOrderSCPEQuery, CreateOrderSCPEModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<GetListORDDetailCreateQuery, GetListORDDetailCreateModel> _objGetListORDDetail;
        private readonly IQueryHandler<GetSaveOrderRespJobQuery, SaveOrderResp> _getSaveOrderRespJob;
        private readonly ICommandHandler<CustRegisterJobCommand> _custRegJobCommand;
        private readonly ICommandHandler<NotificationCommand> _noticeCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly IQueryHandler<PDFDataQuery, PDFData> _pdfData;
        private readonly IQueryHandler<GetListShortNamePackageQuery, List<ListShortNameModel>> _GetListShortNamePackage;
        private readonly ICommandHandler<SendSmsCommand> _SendSms;
        private readonly IQueryHandler<GetFormatFileNameEAPPQuery, FileFormatModel> _GetFormatFileNameEAPP;
        private readonly ICommandHandler<UpdateFileNameCommand> _UpdateFileName;
        private readonly IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> _GetConfigReqPayment;
        private readonly IQueryHandler<CheckOrderPendingCreateQuery, CheckOrderPendingCreateModel> _queryCheckOrderCreateHandler;
        private readonly IQueryHandler<SMSFlagRegisterPendingQuery, SMSFlagRegisterPendingModel> _smsFlagHandler;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        private string FullUrlStr;
        public CreateOrderSCPEHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IQueryHandler<GetListORDDetailCreateQuery, GetListORDDetailCreateModel> objGetListORDDetail, IQueryHandler<GetSaveOrderRespJobQuery, SaveOrderResp> getSaveOrderRespJob, ICommandHandler<CustRegisterJobCommand> custRegJobCommand, ICommandHandler<NotificationCommand> noticeCommand, ICommandHandler<MailLogCommand> mailLogCommand, IQueryHandler<PDFDataQuery, PDFData> pdfData, IQueryHandler<GetListShortNamePackageQuery, List<ListShortNameModel>> getListShortNamePackage, ICommandHandler<SendSmsCommand> sendSms, IQueryHandler<GetFormatFileNameEAPPQuery, FileFormatModel> getFormatFileNameEAPP, ICommandHandler<UpdateFileNameCommand> updateFileName, IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> getConfigReqPayment, IQueryHandler<CheckOrderPendingCreateQuery, CheckOrderPendingCreateModel> queryCheckOrderCreateHandler, IQueryHandler<SMSFlagRegisterPendingQuery, SMSFlagRegisterPendingModel> smsFlagHandler, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objGetListORDDetail = objGetListORDDetail;
            _getSaveOrderRespJob = getSaveOrderRespJob;
            _custRegJobCommand = custRegJobCommand;
            _noticeCommand = noticeCommand;
            _mailLogCommand = mailLogCommand;
            _pdfData = pdfData;
            _GetListShortNamePackage = getListShortNamePackage;
            _SendSms = sendSms;
            _GetFormatFileNameEAPP = getFormatFileNameEAPP;
            _UpdateFileName = updateFileName;
            _GetConfigReqPayment = getConfigReqPayment;
            _queryCheckOrderCreateHandler = queryCheckOrderCreateHandler;
            _smsFlagHandler = smsFlagHandler;
            _lovService = lovService;
        }

        public CreateOrderSCPEModel Handle(CreateOrderSCPEQuery query)
        {
            InterfaceLogCommand log = null;
            //TODO: ใส่ค่าให้ทีหลัง
            var resultModel = new CreateOrderSCPEModel();

            try
            {
                query.FullUrl += "/SCPE";

                FullUrlStr = query.FullUrl;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.txn_id, "CreateOrderSCPE", "CreateOrderSCPEHandler", "", "FBB|" + query.FullUrl, "");

                //Check Order create ?
                if (CheckOrderCreated(query))
                {
                    resultModel.RESULT_CODE = "0";
                    resultModel.RESULT_DESC = "The order has been created.";

                    return resultModel;
                }

                //var results = new paymentLog();
                var errorMsg = "";
                var isCreateOrder = false;

                //TODO: Get from database
                var lovImpersonate = GetLovByTypeName("FBB_CONSTANT", "Impersonate_App");
                var lovMsgSentData = GetLovByLovVal5("FBBOR028");

                var ImpersonateVar = lovImpersonate != null ? lovImpersonate.FirstOrDefault() : new LovValueModel();
                string user = ImpersonateVar.LovValue1;
                string pass = ImpersonateVar.LovValue2;
                string ip = ImpersonateVar.LovValue3;
                string Impersonate = ImpersonateVar.LovValue4;

                var MsgSentData = lovMsgSentData != null ? lovMsgSentData.Where(l => l.Name.Equals("SMS_MESSAGE")).FirstOrDefault() : new LovValueModel();
                string MsgSentTH = MsgSentData.LovValue1;
                string MsgSentEN = MsgSentData.LovValue2;

                //TODO: get method payment table 
                var paymentMethod = GetConfigReqPayment(query);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Save For SCPE
                _logger.Info("Save For SCPE");

                GetListORDDetailCreateModel dataORDDetailCreate = new GetListORDDetailCreateModel();
                _logger.Info("GetListORDDetailCreate order_id :" + query.OrderId);
                dataORDDetailCreate = GetListORDDetailCreate(query.OrderId);
                if (dataORDDetailCreate != null)
                {
                    string eng_flag = dataORDDetailCreate.ODRDetailCustomerList[0].eng_flag;
                    bool isThai = true;
                    if (eng_flag == "Y")
                        isThai = false;

                    /// Fix paymentmethod if not return CheckPayment
                    dataORDDetailCreate.ODRDetailCustomerList[0].paymentmethod = paymentMethod;
                    dataORDDetailCreate.ODRDetailCustomerList[0].transactionid = query.txn_id;
                    _logger.Info("GetListORDDetailCreate order_id :" + query.OrderId + " IsOK");
                    /// SaveOrder

                    SaveOrderResp saveOrderResp = new SaveOrderResp();
                    _logger.Info("GetSaveOrderResp order_id :" + query.OrderId);
                    saveOrderResp = GetSaveOrderResp(dataORDDetailCreate);

                    if (saveOrderResp != null && saveOrderResp.RETURN_ORDER_NO != null && saveOrderResp.RETURN_ORDER_NO != "")
                    {
                        dataORDDetailCreate.return_ia_no = saveOrderResp.RETURN_IA_NO;
                        dataORDDetailCreate.return_order_no = saveOrderResp.RETURN_ORDER_NO;
                        _logger.Info("GetSaveOrderResp order_id :" + query.OrderId + " IsOK");
                        isCreateOrder = true;

                        resultModel.RESULT_CODE = "0";
                        resultModel.RESULT_DESC = "Success";
                    }
                    else
                    {
                        dataORDDetailCreate.return_ia_no = "";
                        dataORDDetailCreate.return_order_no = "";
                        _logger.Info("GetSaveOrderResp order_id :" + query.OrderId + " IsOK");
                        isCreateOrder = false;

                        resultModel.RESULT_CODE = "-1";
                        resultModel.RESULT_DESC = "GetSaveOrderResp faild";
                    }

                    // register customer
                    string customerRowID = "";

                    if (isCreateOrder)
                    {
                        _logger.Info("RegisterCustomer order_id :" + query.OrderId);
                        customerRowID = RegisterCustomer(dataORDDetailCreate);
                        if (customerRowID == null || customerRowID == "")
                        {
                            _logger.Info("RegisterCustomer order_id :" + query.OrderId + " IsOK");
                            isCreateOrder = false;

                            resultModel.RESULT_CODE = "0";
                            resultModel.RESULT_DESC = "GetSaveOrderResp Success but RegisterCustomer faild";
                        }
                        else
                        {
                            _logger.Info("RegisterCustomer order_id :" + query.OrderId + " IsOK");
                            // SendEmail

                            string EmailAddress = "";
                            if (!string.IsNullOrEmpty(dataORDDetailCreate.ODRDetailCustomerList[0].email_address))
                            {
                                _logger.Info("Have SendEmail order_id :" + query.OrderId);
                                EmailAddress = dataORDDetailCreate.ODRDetailCustomerList[0].email_address;
                            }
                            _logger.Info("GenPDFAndSendEmail order_id :" + query.OrderId);
                            GenPDFAndSendEmail(customerRowID, dataORDDetailCreate.ODRDetailCustomerList[0].id_card_no, saveOrderResp.RETURN_ORDER_NO, dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, EmailAddress, isThai, user, pass, ip, Impersonate);


                            // SendSMS

                            string mainCode = "";
                            if (dataORDDetailCreate.ODRDetailPackageList.Count > 0 && isCreateOrder)
                            {
                                for (int i = 0; i <= dataORDDetailCreate.ODRDetailPackageList.Count - 1; i++)
                                {
                                    if (!string.IsNullOrEmpty(dataORDDetailCreate.ODRDetailPackageList[i].sff_promotion_code) && dataORDDetailCreate.ODRDetailPackageList[i].package_type == "Main")
                                    {
                                        mainCode += "|" + dataORDDetailCreate.ODRDetailPackageList[i].sff_promotion_code;
                                    }
                                }
                            }
                            string MsgSent = "";
                            if (isThai)
                                MsgSent = MsgSentTH;
                            else
                                MsgSent = MsgSentEN;

                            _logger.Info("SendSMS order_id :" + query.OrderId);
                            if (CheckSMSFlag(query.OrderId, "Success", dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, query.FullUrl))
                            {
                                var resultsms = SendSMS(dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, mainCode, isThai, MsgSent);
                                if (resultsms == "Success")
                                {
                                    UpdateSMSFlag(query.OrderId, "Success", dataORDDetailCreate.ODRDetailCustomerList[0].mobile_no, query.FullUrl);
                                }
                            }
                        }
                    }
                }
                else
                {
                    resultModel.RESULT_CODE = "-1";
                    resultModel.RESULT_DESC = "GetListORDDetailCreate faild";
                }

                _logger.Info("End Save For SCPE");
            }
            catch (Exception ex)
            {
                resultModel.RESULT_CODE = "-1";
                resultModel.RESULT_DESC = ex.Message;
                _logger.Info("Error call CreateOrderSCPEHandler : " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultModel, log, resultModel.RESULT_CODE == "0" ? "Success" : "Error", resultModel.RESULT_DESC, "");
            }

            return resultModel;
        }


        private bool CheckOrderCreated(CreateOrderSCPEQuery query)
        {
            var checkOrderCreate = _queryCheckOrderCreateHandler.Handle(new CheckOrderPendingCreateQuery() { OrderId = query.OrderId, UpdateBy = query.FullUrl });
            if (checkOrderCreate != null && checkOrderCreate.OrderCreated == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private List<LovValueModel> GetLovByTypeName(string type = "", string name = "")
        {
            List<FBB_CFG_LOV> loveList = null;
            if (!string.IsNullOrEmpty(type))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == type && lov.LOV_NAME == name)
                     .OrderBy(o => o.ORDER_BY).ToList();
                }
                else
                {
                    loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE == type)
                     .OrderBy(o => o.ORDER_BY).ToList();
                }

            }
            else if (!string.IsNullOrEmpty(name))
            {
                loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME == name)
                     .OrderBy(o => o.ORDER_BY).ToList();
            }

            var loveValueModelList = loveList.ToList().Select(l => new LovValueModel
            {
                Text = l.DISPLAY_VAL,
                Type = l.LOV_TYPE,
                Name = l.LOV_NAME,
                Id = l.LOV_ID,
                ParId = l.PAR_LOV_ID.ToSafeDecimal(),
                LovValue1 = l.LOV_VAL1,
                LovValue2 = l.LOV_VAL2,
                LovValue3 = l.LOV_VAL3,
                LovValue4 = l.LOV_VAL4,
                LovValue5 = l.LOV_VAL5,
                OrderBy = l.ORDER_BY,
                DefaultValue = l.DEFAULT_VALUE,
            }).ToList();

            return loveValueModelList;
        }

        private List<LovValueModel> GetLovByLovVal5(string lov_val5 = "")
        {
            List<FBB_CFG_LOV> loveList = null;
            if (!string.IsNullOrEmpty(lov_val5))
            {
                loveList = _lovService
                     .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_VAL5 == lov_val5)
                     .OrderBy(o => o.ORDER_BY).ToList();
            }
            var loveValueModelList = loveList.ToList().Select(l => new LovValueModel
            {
                Text = l.DISPLAY_VAL,
                Type = l.LOV_TYPE,
                Name = l.LOV_NAME,
                Id = l.LOV_ID,
                ParId = l.PAR_LOV_ID.ToSafeDecimal(),
                LovValue1 = l.LOV_VAL1,
                LovValue2 = l.LOV_VAL2,
                LovValue3 = l.LOV_VAL3,
                LovValue4 = l.LOV_VAL4,
                LovValue5 = l.LOV_VAL5,
                OrderBy = l.ORDER_BY,
                DefaultValue = l.DEFAULT_VALUE,
            }).ToList();

            return loveValueModelList;
        }

        private GetListORDDetailCreateModel GetListORDDetailCreate(string orderID)
        {
            GetListORDDetailCreateModel data = new GetListORDDetailCreateModel();

            GetListORDDetailCreateQuery query = new GetListORDDetailCreateQuery
            {
                p_order_id = orderID
            };

            data = _objGetListORDDetail.Handle(query);

            return data;
        }

        private SaveOrderResp GetSaveOrderResp(GetListORDDetailCreateModel model)
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

            var data = _getSaveOrderRespJob.Handle(query);
            return data;
        }

        private string RegisterCustomer(GetListORDDetailCreateModel model)
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

        private void GenPDFAndSendEmail(string custRowId, string L_CARD_NO, string OrderNo, string L_CONTACT_PHONE, string mailTo, bool IsThaiCulture, string user, string pass, string ip, string imagepathimer)
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

        private string QueryGeneratePDF(PDFDataQuery query)
        {
            var pdfData = new PDFData();
            try
            {
                pdfData = _pdfData.Handle(query);
                return pdfData.str_pdf_html;
            }
            catch (System.Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        private decimal InsertMailLog(string customerId)
        {
            var command = new MailLogCommand
            {
                CustomerId = customerId,
            };

            _mailLogCommand.Handle(command);

            return command.RunningNo;
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
            _UpdateFileName.Handle(commamd);
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

                var result = _GetFormatFileNameEAPP.Handle(queryName);

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
                        doc.NewPage();

                        using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                        {

                            using (var sr = new StringReader(html))
                            {
                                var serverMap = HttpContext.Current.Server.MapPath("~");

                                //Path to our font
                                string fonttttts = Path.Combine(serverMap, "App_Content", "Fonts", "tahoma.ttf");
                                _logger.Info("FontPath : " + fonttttts);
                                //Register the font with iTextSharp
                                try
                                {
                                    iTextSharp.text.FontFactory.Register(fonttttts);
                                    iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);
                                }
                                catch
                                {
                                    fonttttts = Path.Combine("D:\\Batch", "FBBQueryOrderPendingPayment", "Content", "Fonts", "tahoma.ttf"); // Fix
                                    _logger.Info("FontPathFix : " + fonttttts);
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

        private Object thisLock = new Object();

        private string SendSMS(string mobileNo, string mainCode, bool IsThaiCulture, string msgtxt)
        {
            var resultsms = string.Empty;
            var tempMobileNo = mobileNo;

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
                    result = _GetListShortNamePackage.Handle(query);
                }

                var source_addr_list = GetLovByTypeName("CONFIG", "SENDER_SMS");
                var source_addr = "";

                if (source_addr_list != null)
                {
                    source_addr = source_addr_list.Any() ? source_addr_list.FirstOrDefault().LovValue1 : "AISFIBRE";
                }
                else
                {
                    source_addr = "AISFIBRE";
                }

                var command = new SendSmsCommand();
                command.FullUrl = FullUrlStr;
                command.Source_Addr = source_addr;
                command.Destination_Addr = mobileNo;
                command.Transaction_Id = tempMobileNo;

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
                                _SendSms.Handle(command);
                                //Thread.Sleep(15000);
                            }
                            command.Message_Text = "";
                            msgtxt = "";

                            if (string.IsNullOrEmpty(resultsms))
                            {
                                resultsms = command.return_status;
                            }
                        }
                    }

                }
                else
                {
                    command.Message_Text = msgtxt;
                    if (!string.IsNullOrEmpty(command.Message_Text))
                    {
                        _SendSms.Handle(command);
                        //Thread.Sleep(15000);
                    }
                    command.Message_Text = "";
                    msgtxt = "";

                    if (string.IsNullOrEmpty(resultsms))
                    {
                        resultsms = command.return_status;
                    }
                }



            }

            return resultsms;
        }

        private string GetConfigReqPayment(CreateOrderSCPEQuery query)
        {
            var payment_method_id = "";

            GetConfigReqPaymentQuery getConfigReqPaymentQuery = new GetConfigReqPaymentQuery()
            {
                p_product_name = "",
                p_service_name = query.Channel,
                p_transaction_id = query.txn_id,
                p_non_mobile_no = query.IdCardNo
            };

            GetConfigReqPaymentModel getConfigReqPaymentData = _GetConfigReqPayment.Handle(getConfigReqPaymentQuery);

            if (getConfigReqPaymentData.ret_code == "0" && getConfigReqPaymentData.list_config_req_payment != null && getConfigReqPaymentData.list_config_req_payment.Count > 0)
            {
                List<ConfigReqPaymentData> configReqPaymentDatas = getConfigReqPaymentData.list_config_req_payment;

                payment_method_id = (configReqPaymentDatas.FirstOrDefault(item => item.attr_name == "payment_method_id") ?? new ConfigReqPaymentData()).attr_value;
            }

            return payment_method_id;
        }

        private bool CheckSMSFlag(string orderId, string status, string ContactMobile, string fullUrl)
        {
            var result = _smsFlagHandler.Handle(new SMSFlagRegisterPendingQuery()
            {
                Action = "Check",
                OrderId = orderId,
                Status = status,
                Mobile_No = ContactMobile,
                FullUrl = fullUrl
            });

            if (result != null && result.SendSMS_Flag != "Y")
            {
                return true;
            }

            return false;
        }

        private void UpdateSMSFlag(string orderId, string status, string ContactMobile, string fullUrl)
        {
            _smsFlagHandler.Handle(new SMSFlagRegisterPendingQuery()
            {
                Action = "Update",
                OrderId = orderId,
                Status = status,
                SMS_Flag = "Y",
                Mobile_No = ContactMobile,
                FullUrl = fullUrl
            });
        }
    }
}
