using System;
using System.Linq;
using System.Net;
//using WBBBusinessLayer.SBNWebService;

using WBBBusinessLayer.SBNNewWebService;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetSaveOrderBulkCorpQueryHandler : IQueryHandler<GetSaveBulkCorpOrderNewQuery, SaveOrderResp>// IQuery<GetSaveBulkCorpOrderNewQuery>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetSaveOrderBulkCorpQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lovService = lovService;
            _uow = uow;
            _lov = lov;
        }

        public SaveOrderResp Handle(GetSaveBulkCorpOrderNewQuery query)
        {
            InterfaceLogCommand log = null;
            var response = new SaveOrderResp();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.GetBulkCorpRegister.P_CALL_WORKFLOW[0].MOBILE_NO +
                    "GetSaveBulkCorpOrderNewQuery", "GetSaveBulkCorpOrderNewQuery", "saveOrderNew", null, "FBB", "FBBBULK001");

                //Package
                var arr_pack = query.GetBulkCorpRegister.AIR_REGIST_PACKAGE_ARRAY;
                var airregists = arr_pack.Select(o => new airRegistPackageRecord()
                {
                    faxFlag = o.FAX_FLAG.ToSafeString(),
                    tempIa = o.TEMP_IA.ToSafeString(),
                    homeIp = o.HOME_IP.ToSafeString(),
                    homePort = o.HOME_PORT.ToSafeString(),
                    iddFlag = o.IDD_FLAG.ToSafeString(),
                    packageCode = o.PACKAGE_CODE.ToSafeString(),
                    packagePrice = o.PACKAGE_PRICE,
                    packageType = o.PACKAGE_TYPE.ToSafeString(),
                    productSubtype = o.PRODUCT_SUBTYPE.ToSafeString(),
                    mobileForward = o.MOBILE_FORWARD.ToSafeString()

                }).ToArray();

                //File
                var temp2 = query.GetBulkCorpRegister.AIR_REGIST_FILE_ARRAY;
                var AirImage = temp2.Select(o => new airRegistFileRecord()// AirRegistFileRecord()
                {
                    fileName = o.PATH_FILE.ToSafeString() //FileName

                });

                var airImage = temp2.Select(o => new airRegistFileRecord()
                {
                    fileName = o.PATH_FILE.ToSafeString()

                }).ToArray();

                var temp3 = query.GetBulkCorpRegister.AIR_REGIST_SPLITTER_ARRAY;
                var airSplitter = temp3.Select(o => new airRegistSplitterRecord()
                {
                    splitterName = o.SPLITTER_NAME.ToSafeString(),
                    distance = o.DISTANCE,
                    distanceType = o.DISTANCE_TYPE.ToSafeString(),
                    resourceType = o.RESOURCE_TYPE.ToSafeString()
                }).ToArray();

                var temp4 = query.GetBulkCorpRegister.AIR_REGIST_CPE_SERIAL_ARRAY;
                var airCPE = temp4.Select(o => new airRegistCpeSerialRecord()
                {
                    cpeType = o.CPE_TYPE.ToSafeString(),
                    serialNo = o.SERIAL_NO.ToSafeString(),
                    macAddress = o.MAC_ADDRESS.ToSafeString()
                }).ToArray();

                #region test
                //List<airRegistPackageRecord> pak1 = new List<airRegistPackageRecord>();
                //airRegistPackageRecord pac2 = new airRegistPackageRecord();
                //pac2.faxFlag = "";
                //pac2.tempIa = "P00521";
                //pac2.homeIp = "";
                //pac2.homePort = "";
                //pac2.iddFlag = "";
                //pac2.packageCode = "001197";
                //pac2.packageType = "Main";
                //pac2.productSubtype = "WireBB";
                //pac2.mobileForward = "";
                //pak1.Add(pac2);

                //pac2 = new airRegistPackageRecord();
                //pac2.faxFlag = "";
                //pac2.tempIa = "P00521";
                //pac2.homeIp = "";
                //pac2.homePort = "";
                //pac2.iddFlag = "";
                //pac2.packageCode = "001133";
                //pac2.packageType = "Ontop";
                //pac2.productSubtype = "WireBB";
                //pac2.mobileForward = "";
                //pak1.Add(pac2);

                //pac2 = new airRegistPackageRecord();
                //pac2.faxFlag = "";
                //pac2.tempIa = "P00521";
                //pac2.homeIp = "";
                //pac2.homePort = "";
                //pac2.iddFlag = "";
                //pac2.packageCode = "001139";
                //pac2.packageType = "Ontop";
                //pac2.productSubtype = "WireBB";
                //pac2.mobileForward = "";

                //pak1.Add(pac2);
                #endregion

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNNewWebService.SBNWebServiceService())
                {
                    service.Timeout = 600000;
                    // Thread.Sleep(2000);

                    var data = service.saveOrderNew(
                    CUSTOMER_TYPE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CUSTOMER_TYPE.ToSafeString(),
                    CUSTOMER_SUBTYPE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CUSTOMER_SUBTYPE.ToSafeString(),
                    TITLE_CODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().TITLE_CODE.ToSafeString(),
                    FIRST_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().FIRST_NAME.ToSafeString(),
                    LAST_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().LAST_NAME.ToSafeString(),
                    CONTACT_TITLE_CODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_TITLE_CODE.ToSafeString(),
                    CONTACT_FIRST_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_FIRST_NAME.ToSafeString(),
                    CONTACT_LAST_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_LAST_NAME.ToSafeString(),
                    ID_CARD_TYPE_DESC: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ID_CARD_TYPE_DESC.ToSafeString(),
                    ID_CARD_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ID_CARD_NO.ToSafeString(),
                    TAX_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().TAX_ID.ToSafeString(),
                    GENDER: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().GENDER.ToSafeString(),
                    BIRTH_DATE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BIRTH_DATE.ToSafeString(),
                    MOBILE_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOBILE_NO.ToSafeString(),
                    MOBILE_NO_2: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOBILE_NO_2.ToSafeString(),
                    HOME_PHONE_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().HOME_PHONE_NO.ToSafeString(),
                    EMAIL_ADDRESS: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().EMAIL_ADDRESS.ToSafeString(),
                    CONTACT_TIME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_TIME.ToSafeString(),
                    NATIONALITY_DESC: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().NATIONALITY_DESC.ToSafeString(),
                    CUSTOMER_REMARK: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CUSTOMER_REMARK.ToSafeString(),

                    HOUSE_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().HOUSE_NO.ToSafeString(),
                    MOO_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOO_NO.ToSafeString(),
                    BUILDING: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BUILDING.ToSafeString(),
                    FLOOR: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().FLOOR.ToSafeString(),
                    ROOM: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ROOM.ToSafeString(),
                    MOOBAN: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOOBAN.ToSafeString(),
                    SOI: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SOI.ToSafeString(),
                    ROAD: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ROAD.ToSafeString(),
                    ZIPCODE_ROWID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ZIPCODE_ROWID.ToSafeString(),

                    LATITUDE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().LATITUDE.ToSafeString(),
                    LONGTITUDE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().LONGTITUDE.ToSafeString(),
                    ASC_CODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ASC_CODE.ToSafeString(),
                    EMPLOYEE_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().EMPLOYEE_ID.ToSafeString(),
                    LOCATION_CODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().LOCATION_CODE.ToSafeString(),
                    SALE_REPRESENT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SALE_REPRESENT.ToSafeString(),
                    CS_NOTE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CS_NOTE.ToSafeString(),
                    WIFI_ACCESS_POINT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().WIFI_ACCESS_POINT.ToSafeString(), // todo
                    INSTALL_STATUS: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALL_STATUS.ToSafeString(),
                    COVERAGE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().COVERAGE.ToSafeString(),
                    EXISTING_AIRNET_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().EXISTING_AIRNET_NO.ToSafeString(),
                    GSM_MOBILE_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().GSM_MOBILE_NO.ToSafeString(),
                    CONTACT_NAME_1: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_NAME_1.ToSafeString(),
                    CONTACT_NAME_2: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_NAME_2.ToSafeString(),
                    CONTACT_MOBILE_NO_1: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_MOBILE_NO_1.ToSafeString(),
                    CONTACT_MOBILE_NO_2: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONTACT_MOBILE_NO_2.ToSafeString(),
                    CONDO_FLOOR: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONDO_FLOOR.ToSafeString(),
                    CONDO_ROOF_TOP: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONDO_ROOF_TOP.ToSafeString(),
                    CONDO_BALCONY: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONDO_BALCONY.ToSafeString(),
                    BALCONY_NORTH: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BALCONY_NORTH.ToSafeString(),
                    BALCONY_SOUTH: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BALCONY_SOUTH.ToSafeString(),
                    BALCONY_EAST: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BALCONY_EAST.ToSafeString(),
                    BALCONY_WAST: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BALCONY_WAST.ToSafeString(),
                    HIGH_BUILDING: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().HIGH_BUILDING.ToSafeString(),
                    HIGH_TREE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().HIGH_TREE.ToSafeString(),
                    BILLBOARD: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BILLBOARD.ToSafeString(),
                    EXPRESSWAY: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().EXPRESSWAY.ToSafeString(),
                    ADDRESS_TYPE_WIRE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ADDRESS_TYPE_WIRE.ToSafeString(),
                    ADDRESS_TYPE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ADDRESS_TYPE.ToSafeString(),
                    FLOOR_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().FLOOR_NO.ToSafeString(),

                    HOUSE_NO_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().HOUSE_NO_BL.ToSafeString(),
                    MOO_NO_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOO_NO_BL.ToSafeString(),
                    BUILDING_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BUILDING_BL.ToSafeString(),
                    FLOOR_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().FLOOR_BL.ToSafeString(),
                    ROOM_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ROOM_BL.ToSafeString(),
                    MOOBAN_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOOBAN_BL.ToSafeString(),
                    SOI_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SOI_BL.ToSafeString(),
                    ROAD_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ROAD_BL.ToSafeString(),
                    ZIPCODE_ROWID_BL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ZIPCODE_ROWID_BL.ToSafeString(),

                    HOUSE_NO_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().HOUSE_NO_VT.ToSafeString(),
                    MOO_NO_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOO_NO_VT.ToSafeString(),
                    BUILDING_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BUILDING_VT.ToSafeString(),
                    FLOOR_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().FLOOR_VT.ToSafeString(),
                    ROOM_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ROOM_VT.ToSafeString(),
                    MOOBAN_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOOBAN_VT.ToSafeString(),
                    SOI_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SOI_VT.ToSafeString(),
                    ROAD_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ROAD_VT.ToSafeString(),
                    ZIPCODE_ROWID_VT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ZIPCODE_ROWID_VT.ToSafeString(),

                    // Update 15.3
                    CVR_ID: "", //cpm.CVRID.ToSafeString(),
                    CVR_NODE: "", //cpm.CVR_NODE.ToSafeString(),
                    CVR_TOWER: "", //cpm.CVR_TOWER.ToSafeString(),
                                   // Update 15.3

                    RELATE_MOBILE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().RELATE_MOBILE.ToSafeString(),
                    RELATE_NON_MOBILE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().RELATE_NON_MOBILE.ToSafeString(),
                    SFF_CA_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SFF_CA_NO.ToSafeString(),
                    SFF_SA_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SFF_SA_NO.ToSafeString(),
                    SFF_BA_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SFF_BA_NO.ToSafeString(),
                    NETWORK_TYPE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().NETWORK_TYPE.ToSafeString(),
                    SERVICE_DAY: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SERVICE_DAY.ToSafeInteger(),
                    EXPECT_INSTALL_DATE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().EXPECT_INSTALL_DATE.ToSafeString(),
                    SERVICE_DAYSpecified: true,//query.GetBulkCorpRegister.SERVICE_DAY.ToSafeBoolean(),
                    FTTX_VENDOR: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().FTTX_VENDOR.ToSafeString(),
                    INSTALL_NOTE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALL_NOTE.ToSafeString(),

                    // Update 15.3
                    PHONE_FLAG: "",
                    TIME_SLOT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().TIME_SLOT.ToSafeString(),
                    INSTALLATION_CAPACITY: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALLATION_CAPACITY.ToSafeString(),
                    ADDRESS_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ADDRESS_ID.ToSafeString(),
                    ACCESS_MODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ACCESS_MODE.ToSafeString(),
                    // Update 15.3

                    ENG_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ENG_FLAG.ToSafeString(),
                    EVENT_CODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().EVENT_CODE.ToSafeString(),
                    INSTALLADDRESS1: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALLADDRESS1.ToSafeString(),
                    INSTALLADDRESS2: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALLADDRESS2.ToSafeString(),
                    INSTALLADDRESS3: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALLADDRESS3.ToSafeString(),
                    INSTALLADDRESS4: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALLADDRESS4.ToSafeString(),
                    INSTALLADDRESS5: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALLADDRESS5.ToSafeString(),
                    PBOX_COUNT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PBOX_COUNT.ToSafeString(),
                    CONVERGENCE_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CONVERGENCE_FLAG.ToSafeString(),
                    TIME_SLOT_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().TIME_SLOT_ID.ToSafeString(),
                    // Update 16.1
                    GIFT_VOUCHER: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().GIFT_VOUCHER.ToSafeString(),
                    SUB_LOCATION_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SUB_LOCATION_ID.ToSafeString(),
                    SUB_CONTRACT_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SUB_CONTRACT_NAME.ToSafeString(),
                    INSTALL_STAFF_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALL_STAFF_ID.ToSafeString(),
                    INSTALL_STAFF_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().INSTALL_STAFF_NAME.ToSafeString(),

                    FLOW_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().FLOW_FLAG.ToSafeString(),
                    SITE_CODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SITE_CODE.ToSafeString(),

                    LINE_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().LINE_ID.ToSafeString(),
                    RELATE_PROJECT_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().RELATE_PROJECT_NAME.ToSafeString(),
                    PLUG_AND_PLAY_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PLUG_AND_PLAY_FLAG.ToSafeString(),

                    //update16.04
                    //p_REC_CPE_LIST = CPE,
                    RESERVED_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().RESERVED_ID.ToSafeString(),
                    //update16.11
                    JOB_ORDER_TYPE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().JOB_ORDER_TYPE.ToSafeString(),
                    ASSIGN_RULE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ASSIGN_RULE.ToSafeString(),
                    OLD_ISP: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().OLD_ISP.ToSafeString(),
                    //17.3 Splitter Management
                    SPLITTER_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SPLITTER_FLAG.ToSafeString(),
                    RESERVED_PORT_ID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().RESERVED_PORT_ID.ToSafeString(),
                    SPECIAL_REMARK: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SPECIAL_REMARK.ToSafeString(),
                    ORDER_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().ORDER_NO.ToSafeString(),
                    SOURCE_SYSTEM: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SOURCE_SYSTEM.ToSafeString(),
                    BILL_MEDIA: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().BILL_MEDIA.ToSafeString(),

                    //17.7
                    PRE_ORDER_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PRE_ORDER_NO.ToSafeString(),
                    VOUCHER_DESC: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().VOUCHER_DESC.ToSafeString(),
                    CAMPAIGN_PROJECT_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().CAMPAIGN_PROJECT_NAME.ToSafeString(),
                    PRE_ORDER_CHANEL: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PRE_ORDER_CHANEL.ToSafeString(),

                    //17.9
                    RENTAL_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().RENTAL_FLAG.ToSafeString(),
                    DEV_PROJECT_CODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().DEV_PROJECT_CODE.ToSafeString(),
                    DEV_BILL_TO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().DEV_BILL_TO.ToSafeString(),
                    //DEV_PRICE:0,
                    //DEV_PRICESpecified:true,

                    //18.7
                    PARTNER_TYPE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PARTNER_TYPE.ToSafeString(),
                    PARTNER_SUBTYPE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PARTNER_SUBTYPE.ToSafeString(),
                    MOBILE_BY_ASC: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().MOBILE_BY_ASC.ToSafeString(),
                    LOCATION_NAME: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().LOCATION_NAME.ToSafeString(),
                    PAYMENTMETHOD: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PAYMENTMETHOD.ToSafeString(),
                    TRANSACTIONID_IN: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().TRANSACTIONID_IN.ToSafeString(),
                    TRANSACTIONID: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().TRANSACTIONID.ToSafeString(),

                    //18.8
                    SUB_ACCESS_MODE: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().SUB_ACCESS_MODE.ToSafeString(),
                    REQUEST_SUB_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().REQUEST_SUB_FLAG.ToSafeString(),
                    PREMIUM_FLAG: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().PREMIUM_FLAG.ToSafeString(),
                    RELATE_MOBILE_SEGMENT: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().RELATE_MOBILE_SEGMENT.ToSafeString(),
                    REF_UR_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().REF_UR_NO.ToSafeString(),
                    LOCATION_EMAIL_BY_REGION: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().LOCATION_EMAIL_BY_REGION.ToSafeString(),
                    SALE_STAFF_NAME: "",

                    //19.1
                    DOPA_FLAG: "",
                    SERVICE_YEAR: "",
                    REQUIRE_CS_VERIFY_DOC: "",
                    FACERECOG_FLAG: "",

                    SPECIAL_ACCOUNT_NAME: "",
                    SPECIAL_ACCOUNT_NO: "",
                    SPECIAL_ACCOUNT_ENDDATE: "",
                    SPECIAL_ACCOUNT_GROUP_EMAIL: "",
                    SPECIAL_ACCOUNT_FLAG: "",

                    //19.3
                    EXISTING_MOBILE_FLAG: "",

                    //19.4
                    PRE_SURVEY_DATE: "",
                    PRE_SURVEY_TIMESLOT: "",

                    //19.7
                    REGISTER_CHANNEL: "FBBWF",
                    AUTO_CREATE_PROSPECT_FLAG: "N",
                    ORDER_VERIFY: "",

                    //19.10
                    TRANSACTION_LOG_ID: "",
                    WAITING_INSTALL_DATE: "",
                    WAITING_TIME_SLOT: "",

                    DEV_PO_NO: query.GetBulkCorpRegister.P_CALL_WORKFLOW.FirstOrDefault().DEV_PO_NO.ToSafeString(),
                    AIR_REGIST_PACKAGE_ARRAY: airregists,
                    air_regist_file_array: airImage,
                    AIR_REGIST_SPLITTER_ARRAY: airSplitter,
                    AIR_REGIST_CPE_SERIAL_ARRAY: airCPE

                   );


                    if (data != null)
                    {
                        response.RETURN_CODE = 0;
                        response.RETURN_MESSAGE = data.RETURN_MESSAGE;
                        response.RETURN_IA_NO = data.RETURN_SALE_ORDER;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "FBBBULK001");
                    }
                    else
                    {
                        response.RETURN_CODE = -1;
                        response.RETURN_MESSAGE = "Webservice WorkFlow data null";
                        response.RETURN_IA_NO = "";

                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", "Webservice WorkFlow data null", "FBBBULK001");
                    }

                    TimeSpan ts = stopWatch.Elapsed;
                    string SBNServiceListPackageElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    _logger.Info("SBNWebServiceService.saveOrderNew elapsed time is " + SBNServiceListPackageElapsedTime);

                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "FBBBULK001");
                }

                response.RETURN_CODE = -1;
                response.RETURN_MESSAGE = "Error Before Call Airnet Service " + ex.GetErrorMessage();
                response.RETURN_IA_NO = "";
            }

            return response;
        }


    }
}
