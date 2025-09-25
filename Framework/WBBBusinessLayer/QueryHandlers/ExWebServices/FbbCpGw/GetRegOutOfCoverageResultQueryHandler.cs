using System.Collections.Generic;
using System.Linq;
using System.Net;
using WBBBusinessLayer.SBNV2WebService;
using WBBContract;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public class GetRegOutOfCoverageResultQueryHandler : IQueryHandler<GetRegOutOfCoverageResultQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RESULT> _coverageAreaRes;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _coverageAreaResFBSS;

        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _coverageBuilding;

        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffChkProfLog;

        public GetRegOutOfCoverageResultQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> coverageArea,
            IEntityRepository<FBB_COVERAGEAREA_RESULT> coverageAreaRes,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_COVERAGEAREA_BUILDING> coverageBuilding,
            IEntityRepository<FBB_ZIPCODE> zipcode,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffChkProfLog,
            IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> coverageAreaResFBSS)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _coverageAreaRes = coverageAreaRes;
            _lov = lov;
            _coverageBuilding = coverageBuilding;
            _zipcode = zipcode;
            _sffChkProfLog = sffChkProfLog;
            _coverageAreaResFBSS = coverageAreaResFBSS;
        }

        public List<string> Handle(GetRegOutOfCoverageResultQuery query)
        {
            var custNameArray = query.CustName.Split(' ');
            var custFirstName = "";
            var custLastName = "";

            if (custNameArray.Length > 0)
            {
                custFirstName = custNameArray[0];
                custLastName = string.Join(" ", custNameArray.Skip(1));
            }

            var isThai = query.Language.ToCultureCode().IsThaiCulture();
            var result = new List<string>();

            _logger.Info("Begin Find Coverage result from TRANSACTION_ID");
            var coverageAreaRow = (from t in _coverageAreaResFBSS.Get()
                                   where t.TRANSACTION_ID == query.TransactionID
                                   select new
                                   {
                                       t.ADDRESS_ID,
                                       t.RESULTID,
                                       t.LATITUDE,
                                       t.LONGITUDE,
                                       t.ZIPCODE_ROWID,
                                       t.COVERAGE
                                   }).FirstOrDefault();

            _logger.Info("record result: " + coverageAreaRow);
            var cvrResultId = "";
            var lat = "";
            var lng = "";
            var zipcodeRowId = "";

            if (null != coverageAreaRow)
            {
                cvrResultId = coverageAreaRow.RESULTID.ToSafeString();
                lat = coverageAreaRow.LATITUDE.ToSafeString();
                lng = coverageAreaRow.LONGITUDE.ToSafeString();
                zipcodeRowId = coverageAreaRow.ZIPCODE_ROWID.ToSafeString();
            }

            result.Add(cvrResultId);

            var EngFlag = "N";
            if (query.Language.ToCultureCode().IsEngCulture())
            {
                EngFlag = "Y";
            }

            var SBNStatus = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();

            if (SBNStatus.LOV_VAL1 == "NEW")
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                #region New SBNWebService

                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.saveOrderNew(
                            CUSTOMER_TYPE: "R",
                            CUSTOMER_SUBTYPE: "T",
                            TITLE_CODE: "127",
                            FIRST_NAME: custFirstName.ToSafeString(),
                            LAST_NAME: custLastName.ToSafeString(),
                            CONTACT_TITLE_CODE: "127",
                            CONTACT_FIRST_NAME: custFirstName.ToSafeString(),
                            CONTACT_LAST_NAME: custLastName.ToSafeString(),
                            ID_CARD_TYPE_DESC: "บัตรประชาชน",
                            ID_CARD_NO: "",
                            TAX_ID: "",
                            GENDER: "Female",
                            BIRTH_DATE: "01/01/1999",
                            MOBILE_NO: query.ContactMobileNo.ToSafeString(),
                            MOBILE_NO_2: "",
                            HOME_PHONE_NO: "",
                            EMAIL_ADDRESS: query.EmailAddress.ToSafeString(),
                            CONTACT_TIME: "08.00 - 19.00",
                            NATIONALITY_DESC: "THAI",
                            CUSTOMER_REMARK: "",
                            HOUSE_NO: "",
                            MOO_NO: "",
                            BUILDING: "",
                            FLOOR: "",
                            ROOM: "",
                            MOOBAN: "",
                            SOI: "",
                            ROAD: "",
                            ZIPCODE_ROWID: zipcodeRowId,
                            LATITUDE: lat,
                            LONGTITUDE: lng,
                            ASC_CODE: "",
                            EMPLOYEE_ID: "",
                            LOCATION_CODE: "",
                            SALE_REPRESENT: "",
                            CS_NOTE: "",
                            WIFI_ACCESS_POINT: "N",
                            INSTALL_STATUS: "N",
                            COVERAGE: "N",
                            EXISTING_AIRNET_NO: "",
                            GSM_MOBILE_NO: "",
                            CONTACT_NAME_1: custFirstName,
                            CONTACT_NAME_2: custLastName,
                            CONTACT_MOBILE_NO_1: query.ContactMobileNo.ToSafeString(),
                            CONTACT_MOBILE_NO_2: "",
                            CONDO_FLOOR: "",
                            CONDO_ROOF_TOP: "N",
                            CONDO_BALCONY: "N",
                            BALCONY_NORTH: "N",
                            BALCONY_SOUTH: "N",
                            BALCONY_EAST: "N",
                            BALCONY_WAST: "N",
                            HIGH_BUILDING: "N",
                            HIGH_TREE: "N",
                            BILLBOARD: "N",
                            EXPRESSWAY: "N",
                            ADDRESS_TYPE_WIRE: "บ้านเดี่ยว",
                            ADDRESS_TYPE: "บ้านเดี่ยว",
                            FLOOR_NO: "",
                            HOUSE_NO_BL: "",
                            MOO_NO_BL: "",
                            MOOBAN_BL: "",
                            BUILDING_BL: "",
                            FLOOR_BL: "",
                            ROOM_BL: "",
                            SOI_BL: "",
                            ROAD_BL: "",
                            ZIPCODE_ROWID_BL: zipcodeRowId,
                            HOUSE_NO_VT: "",
                            MOO_NO_VT: "",
                            MOOBAN_VT: "",
                            BUILDING_VT: "",
                            FLOOR_VT: "",
                            ROOM_VT: "",
                            SOI_VT: "",
                            ROAD_VT: "",
                            ZIPCODE_ROWID_VT: "",
                            CVR_ID: "",
                            CVR_NODE: "",
                            CVR_TOWER: "",
                            SITE_CODE: "",
                            RELATE_MOBILE: "",
                            RELATE_NON_MOBILE: "",
                            SFF_CA_NO: "",
                            SFF_SA_NO: "",
                            SFF_BA_NO: "",
                            NETWORK_TYPE: "",
                            SERVICE_DAY: "0",
                            EXPECT_INSTALL_DATE: "",
                            FTTX_VENDOR: "",
                            INSTALL_NOTE: "",
                            PHONE_FLAG: "",
                            TIME_SLOT: "",
                            INSTALLATION_CAPACITY: "",
                            ADDRESS_ID: "",
                            ACCESS_MODE: "",
                            ENG_FLAG: EngFlag,
                            EVENT_CODE: "",
                            INSTALLADDRESS1: "",
                            INSTALLADDRESS2: "",
                            INSTALLADDRESS3: "",
                            INSTALLADDRESS4: "",
                            INSTALLADDRESS5: "",
                            PBOX_COUNT: "",
                            CONVERGENCE_FLAG: "",
                            TIME_SLOT_ID: "",
                            GIFT_VOUCHER: "",
                            SUB_LOCATION_ID: "",
                            SUB_CONTRACT_NAME: "",
                            INSTALL_STAFF_ID: "",
                            INSTALL_STAFF_NAME: "",
                            FLOW_FLAG: "",
                            LINE_ID: query.LineId.ToSafeString(),
                            RELATE_PROJECT_NAME: "",
                            PLUG_AND_PLAY_FLAG: "",
                            RESERVED_ID: "",
                            JOB_ORDER_TYPE: "",
                            ASSIGN_RULE: "",
                            OLD_ISP: "",
                            SPLITTER_FLAG: "",
                            RESERVED_PORT_ID: "",
                            SPECIAL_REMARK: "",
                            ORDER_NO: "",
                            SOURCE_SYSTEM: "",
                            BILL_MEDIA: "",
                            PRE_ORDER_NO: "",
                            VOUCHER_DESC: "",
                            CAMPAIGN_PROJECT_NAME: "",
                            PRE_ORDER_CHANEL: "",
                            RENTAL_FLAG: "",
                            DEV_PROJECT_CODE: "",
                            DEV_BILL_TO: "",
                            DEV_PO_NO: "",
                            PARTNER_TYPE: "",
                            PARTNER_SUBTYPE: "",
                            MOBILE_BY_ASC: "",
                            LOCATION_NAME: "",
                            PAYMENTMETHOD: "",
                            TRANSACTIONID_IN: "",
                            TRANSACTIONID: "",
                            SUB_ACCESS_MODE: "",
                            REQUEST_SUB_FLAG: "",
                            PREMIUM_FLAG: "",
                            RELATE_MOBILE_SEGMENT: "",
                            REF_UR_NO: "",
                            LOCATION_EMAIL_BY_REGION: "",
                            SALE_STAFF_NAME: "",
                            DOPA_FLAG: "",
                            SERVICE_YEAR: "",
                            REQUIRE_CS_VERIFY_DOC: "",
                            FACERECOG_FLAG: "",
                            SPECIAL_ACCOUNT_NAME: "",
                            SPECIAL_ACCOUNT_NO: "",
                            SPECIAL_ACCOUNT_ENDDATE: "",
                            SPECIAL_ACCOUNT_GROUP_EMAIL: "",
                            SPECIAL_ACCOUNT_FLAG: "",
                            EXISTING_MOBILE_FLAG: "",
                            PRE_SURVEY_DATE: "",
                            PRE_SURVEY_TIMESLOT: "",
                            REGISTER_CHANNEL: "FBBWF",
                            AUTO_CREATE_PROSPECT_FLAG: "N",
                            ORDER_VERIFY: "",
                            WAITING_INSTALL_DATE: "",
                            WAITING_TIME_SLOT: "",
                            SALE_CHANNEL: "",
                            OWNER_PRODUCT: "",
                            PACKAGE_FOR: "",
                            SFF_PROMOTION_CODE: "",
                            REGION: "",
                            PROVINCE: "",
                            DISTRICT: "",
                            SUB_DISTRICT: "",
                            SERENADE_FLAG: "",
                            FMPA_FLAG: "",
                            CVM_FLAG: "",
                            ORDER_RELATE_CHANGE_PRO: "",
                            COMPANY_NAME: "",
                            DISTRIBUTION_CHANNEL: "",
                            CHANNEL_SALES_GROUP: "",
                            SHOP_TYPE: "",
                            SHOP_SEGMENT: "",
                            ASC_NAME: "",
                            ASC_MEMBER_CATEGORY: "",
                            ASC_POSITION: "",
                            LOCATION_REGION: "",
                            LOCATION_SUB_REGION: "",
                            EMPLOYEE_NAME: "",
                            CUSTOMERPURGE: "",
                            EXCEPTENTRYFEE: "",
                            SECONDINSTALLATION: "",
                            AMENDMENT_FLAG: "",
                            SERVICE_LEVEL: "",
                            FIRST_INSTALL_DATE: "",
                            FIRST_TIME_SLOT: "",
                            LINE_TEMP_ID: "",
                            FMC_SPECIAL_FLAG: "",
                            NON_RES_FLAG: "",
                            CRITERIA_MOBILE: "",
                            REMARK_FOR_SUBCONTRACT: "",
                            MESH_COUNT: "",
                            ONLINE_FLAG: "",
                            PRIVILEGE_POINT: "",
                            PRIVILEGE_STAFF: "",
                            SPECIAL_SKILL: "",
                            TDM_CONTRACT_ID: "",
                            TDM_RULE_ID: "",
                            TDM_PENALTY_ID: "",
                            TDM_PENALTY_GROUP_ID: "",
                            DURATION: "",
                            CONTRACT_FLAG: "",
                            NON_MOBILE_NO: "",
                            REGIS_PAYMENT_ID: "",
                            REGIS_PAYMENTDATE: "",
                            REGIS_PAYMENTMETHOD: "",
                            //R23.05 CheckFraud
                            CEN_FRAUD_FLAG: "",
                            VERIFY_REASON_CEN_FRAUD: "",
                            FRAUD_SCORE: "",
                            //end R23.05 CheckFraud
                            //R23.07
                            DELIVERY_METHOD: "",
                            DIY_FLAG: "",

                            AIR_REGIST_PACKAGE_ARRAY: new List<airRegistPackageRecord>().ToArray(),
                            AIR_REGIST_FILE_ARRAY: new List<airRegistFileRecord>().ToArray(),
                            AIR_REGIST_SPLITTER_ARRAY: new List<airRegistSplitterRecord>().ToArray(),
                            AIR_REGIST_CPE_SERIAL_ARRAY: new List<airRegistCpeSerialRecord>().ToArray(),
                            AIR_REGIST_CUST_INSIGHT_ARRAY: new List<airRegistCustInsightRecord>().ToArray(),
                            AIR_REGIST_DCONTRACT_ARRAY: new List<airRegistDcontractRecord>().ToArray(),
                            AIR_REGIST_CROSS_NETWORK_ARRAY: new List<airRegistCrossnetworkRecord>().ToArray(),
                            //R23.05 CheckFraud
                            AIR_FRAUD_REASON_ARRAY: new List<airFraudReasonRecord>().ToArray()
                        //end R23.05 CheckFraud
                        );

                    result.Add(data.RETURN_CODE.ToSafeString());
                    result.Add(data.RETURN_MESSAGE);
                    result.Add(data.RETURN_ORDER_NO);
                }
                #endregion
            }
            else
            {
                #region Old SBNWebService
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.saveOrderNew(
                            CUSTOMER_TYPE: "R",
                            CUSTOMER_SUBTYPE: "T",
                            TITLE_CODE: "127",
                            FIRST_NAME: custFirstName.ToSafeString(),
                            LAST_NAME: custLastName.ToSafeString(),
                            CONTACT_TITLE_CODE: "127",
                            CONTACT_FIRST_NAME: custFirstName.ToSafeString(),
                            CONTACT_LAST_NAME: custLastName.ToSafeString(),
                            ID_CARD_TYPE_DESC: "บัตรประชาชน",
                            ID_CARD_NO: "",
                            TAX_ID: "",
                            GENDER: "Female",
                            BIRTH_DATE: "01/01/1999",
                            MOBILE_NO: query.ContactMobileNo.ToSafeString(),
                            MOBILE_NO_2: "",
                            HOME_PHONE_NO: "",
                            EMAIL_ADDRESS: query.EmailAddress.ToSafeString(),
                            CONTACT_TIME: "08.00 - 19.00",
                            NATIONALITY_DESC: "THAI",
                            CUSTOMER_REMARK: "",
                            HOUSE_NO: "",
                            MOO_NO: "",
                            BUILDING: "",
                            FLOOR: "",
                            ROOM: "",
                            MOOBAN: "",
                            SOI: "",
                            ROAD: "",
                            ZIPCODE_ROWID: zipcodeRowId,
                            LATITUDE: lat,
                            LONGTITUDE: lng,
                            ASC_CODE: "",
                            EMPLOYEE_ID: "",
                            LOCATION_CODE: "",
                            SALE_REPRESENT: "",
                            CS_NOTE: "",
                            WIFI_ACCESS_POINT: "N",
                            INSTALL_STATUS: "N",
                            COVERAGE: "N",
                            EXISTING_AIRNET_NO: "",
                            GSM_MOBILE_NO: "",
                            CONTACT_NAME_1: custFirstName,
                            CONTACT_NAME_2: custLastName,
                            CONTACT_MOBILE_NO_1: query.ContactMobileNo,
                            CONTACT_MOBILE_NO_2: "",
                            CONDO_FLOOR: "",
                            CONDO_ROOF_TOP: "N",
                            CONDO_BALCONY: "N",
                            BALCONY_NORTH: "N",
                            BALCONY_SOUTH: "N",
                            BALCONY_EAST: "N",
                            BALCONY_WAST: "N",
                            HIGH_BUILDING: "N",
                            HIGH_TREE: "N",
                            BILLBOARD: "N",
                            EXPRESSWAY: "N",
                            ADDRESS_TYPE_WIRE: "บ้านเดี่ยว",
                            ADDRESS_TYPE: "บ้านเดี่ยว",
                            FLOOR_NO: "",
                            HOUSE_NO_BL: "",
                            MOO_NO_BL: "",
                            MOOBAN_BL: "",
                            BUILDING_BL: "",
                            FLOOR_BL: "",
                            ROOM_BL: "",
                            SOI_BL: "",
                            ROAD_BL: "",
                            ZIPCODE_ROWID_BL: zipcodeRowId,
                            HOUSE_NO_VT: "",
                            MOO_NO_VT: "",
                            MOOBAN_VT: "",
                            BUILDING_VT: "",
                            FLOOR_VT: "",
                            ROOM_VT: "",
                            SOI_VT: "",
                            ROAD_VT: "",
                            ZIPCODE_ROWID_VT: "",
                            CVR_ID: "",
                            CVR_NODE: "",
                            CVR_TOWER: "",
                            SITE_CODE: "",
                            RELATE_MOBILE: "",
                            RELATE_NON_MOBILE: "",
                            SFF_CA_NO: "",
                            SFF_SA_NO: "",
                            SFF_BA_NO: "",
                            NETWORK_TYPE: "",
                            SERVICE_DAY: "0",
                            EXPECT_INSTALL_DATE: "",
                            FTTX_VENDOR: "",
                            INSTALL_NOTE: "",
                            PHONE_FLAG: "",
                            TIME_SLOT: "",
                            INSTALLATION_CAPACITY: "",
                            ADDRESS_ID: "",
                            ACCESS_MODE: "",
                            ENG_FLAG: EngFlag,
                            EVENT_CODE: "",
                            INSTALLADDRESS1: "",
                            INSTALLADDRESS2: "",
                            INSTALLADDRESS3: "",
                            INSTALLADDRESS4: "",
                            INSTALLADDRESS5: "",
                            PBOX_COUNT: "",
                            CONVERGENCE_FLAG: "",
                            TIME_SLOT_ID: "",
                            GIFT_VOUCHER: "",
                            SUB_LOCATION_ID: "",
                            SUB_CONTRACT_NAME: "",
                            INSTALL_STAFF_ID: "",
                            INSTALL_STAFF_NAME: "",
                            FLOW_FLAG: "",
                            LINE_ID: query.LineId.ToSafeString(),
                            RELATE_PROJECT_NAME: "",
                            PLUG_AND_PLAY_FLAG: "",
                            RESERVED_ID: "",
                            JOB_ORDER_TYPE: "",
                            ASSIGN_RULE: "",
                            OLD_ISP: "",
                            SPLITTER_FLAG: "",
                            RESERVED_PORT_ID: "",
                            SPECIAL_REMARK: "",
                            ORDER_NO: "",
                            SOURCE_SYSTEM: "",
                            BILL_MEDIA: "",
                            PRE_ORDER_NO: "",
                            VOUCHER_DESC: "",
                            CAMPAIGN_PROJECT_NAME: "",
                            PRE_ORDER_CHANEL: "",
                            RENTAL_FLAG: "",
                            DEV_PROJECT_CODE: "",
                            DEV_BILL_TO: "",
                            DEV_PO_NO: "",
                            PARTNER_TYPE: "",
                            PARTNER_SUBTYPE: "",
                            MOBILE_BY_ASC: "",
                            LOCATION_NAME: "",
                            PAYMENTMETHOD: "",
                            TRANSACTIONID_IN: "",
                            TRANSACTIONID: "",
                            SUB_ACCESS_MODE: "",
                            REQUEST_SUB_FLAG: "",
                            PREMIUM_FLAG: "",
                            RELATE_MOBILE_SEGMENT: "",
                            REF_UR_NO: "",
                            LOCATION_EMAIL_BY_REGION: "",
                            SALE_STAFF_NAME: "",
                            DOPA_FLAG: "",
                            SERVICE_YEAR: "",
                            REQUIRE_CS_VERIFY_DOC: "",
                            FACERECOG_FLAG: "",
                            SPECIAL_ACCOUNT_NAME: "",
                            SPECIAL_ACCOUNT_NO: "",
                            SPECIAL_ACCOUNT_ENDDATE: "",
                            SPECIAL_ACCOUNT_GROUP_EMAIL: "",
                            SPECIAL_ACCOUNT_FLAG: "",
                            EXISTING_MOBILE_FLAG: "",
                            PRE_SURVEY_DATE: "",
                            PRE_SURVEY_TIMESLOT: "",
                            REGISTER_CHANNEL: "FBBWF",
                            AUTO_CREATE_PROSPECT_FLAG: "N",
                            ORDER_VERIFY: "",
                            WAITING_INSTALL_DATE: "",
                            WAITING_TIME_SLOT: "",
                            SALE_CHANNEL: "",
                            OWNER_PRODUCT: "",
                            PACKAGE_FOR: "",
                            SFF_PROMOTION_CODE: "",
                            REGION: "",
                            PROVINCE: "",
                            DISTRICT: "",
                            SUB_DISTRICT: "",
                            SERENADE_FLAG: "",
                            FMPA_FLAG: "",
                            CVM_FLAG: "",

                        ORDER_RELATE_CHANGE_PRO: "",

                        COMPANY_NAME: "",
                            DISTRIBUTION_CHANNEL: "",
                            CHANNEL_SALES_GROUP: "",
                            SHOP_TYPE: "",
                            SHOP_SEGMENT: "",
                            ASC_NAME: "",
                            ASC_MEMBER_CATEGORY: "",
                            ASC_POSITION: "",
                            LOCATION_REGION: "",
                            LOCATION_SUB_REGION: "",
                            EMPLOYEE_NAME: "",
                            CUSTOMERPURGE: "",
                            EXCEPTENTRYFEE: "",
                            SECONDINSTALLATION: "",
                            AMENDMENT_FLAG: "",
                            SERVICE_LEVEL: "",
                            FIRST_INSTALL_DATE: "",
                            FIRST_TIME_SLOT: "",
                            LINE_TEMP_ID: "",
                            FMC_SPECIAL_FLAG: "",
                            NON_RES_FLAG: "",
                            CRITERIA_MOBILE: "",
                            REMARK_FOR_SUBCONTRACT: "",
                            MESH_COUNT: "",
                            ONLINE_FLAG: "",
                            PRIVILEGE_POINT: "",
                            PRIVILEGE_STAFF: "",
                            SPECIAL_SKILL: "",
                            TDM_CONTRACT_ID: "",
                            TDM_RULE_ID: "",
                            TDM_PENALTY_ID: "",
                            TDM_PENALTY_GROUP_ID: "",
                            DURATION: "",
                            CONTRACT_FLAG: "",
                            NON_MOBILE_NO: "",
                            REGIS_PAYMENT_ID: "",
                            REGIS_PAYMENTDATE: "",
                            REGIS_PAYMENTMETHOD: "",
                            //R23.05 CheckFraud
                            CEN_FRAUD_FLAG: "",
                            VERIFY_REASON_CEN_FRAUD: "",
                            FRAUD_SCORE: "",
                            //end R23.05 CheckFraud
                            //R23.07
                            DELIVERY_METHOD: "",
                            DIY_FLAG: "",

                            AIR_REGIST_PACKAGE_ARRAY: new List<airRegistPackageRecord>().ToArray(),
                            AIR_REGIST_FILE_ARRAY: new List<airRegistFileRecord>().ToArray(),
                            AIR_REGIST_SPLITTER_ARRAY: new List<airRegistSplitterRecord>().ToArray(),
                            AIR_REGIST_CPE_SERIAL_ARRAY: new List<airRegistCpeSerialRecord>().ToArray(),
                            AIR_REGIST_CUST_INSIGHT_ARRAY: new List<airRegistCustInsightRecord>().ToArray(),
                            AIR_REGIST_DCONTRACT_ARRAY: new List<airRegistDcontractRecord>().ToArray(),
                            AIR_REGIST_CROSS_NETWORK_ARRAY: new List<airRegistCrossnetworkRecord>().ToArray(),
                            //R23.05 CheckFraud
                            AIR_FRAUD_REASON_ARRAY: new List<airFraudReasonRecord>().ToArray()
                        //end R23.05 CheckFraud
                        );

                    result.Add(data.RETURN_CODE.ToSafeString());
                    result.Add(data.RETURN_MESSAGE);
                    result.Add(data.RETURN_SALE_ORDER);
                }
                #endregion

            }
            return result;
        }

        private static airRegistPackageRecord[] CreateAirnetPackageRecord(List<PackageModel> packageList)
        {
            airRegistPackageRecord[] airregists = packageList.Select(o => new airRegistPackageRecord()
            {

                faxFlag = "",
                tempIa = o.MAPPING_PRODUCT.ToSafeString(),
                homeIp = "",
                homePort = "",
                iddFlag = "",
                packageCode = o.PACKAGE_CODE.ToSafeString(),
                packageType = o.PACKAGE_TYPE.ToSafeString(),
                productSubtype = o.PRODUCT_SUBTYPE.ToSafeString(),
            }).ToArray();

            return airregists;
        }

        private List<string> aFindLatLng(string transactionId)
        {
            var latlngRow = (from t in _coverageAreaRes.Get()
                             where t.TRANSACTION_ID == transactionId
                             select new { t.LATITUDE, t.LONGITUDE }).FirstOrDefault();

            var latlngList = new List<string>();

            if (null != latlngRow)
            {
                latlngList.Add(latlngRow.LATITUDE);
                latlngList.Add(latlngRow.LONGITUDE);
            }
            else
            {
                latlngList.Add("");
                latlngList.Add("");
            }

            return latlngList;
        }

        private List<string> aFindCvrId(string transactionId)
        {
            var coverageAreaRow = (from t in _coverageAreaRes.Get()
                                   where t.TRANSACTION_ID == transactionId
                                   select new { t.CVRID, t.RESULTID }).FirstOrDefault();

            var coverageAreaList = new List<string>();

            if (null != coverageAreaRow)
            {
                coverageAreaList.Add(coverageAreaRow.CVRID.ToSafeString());
                coverageAreaList.Add(coverageAreaRow.RESULTID.ToSafeString());
            }
            else
            {
                coverageAreaList.Add("");
                coverageAreaList.Add("");
            }

            return coverageAreaList;
        }

        private string aFindZipCodeRowId(string transactionId)
        {
            var zipcode = (from t in _coverageAreaRes.Get()
                           where t.TRANSACTION_ID == transactionId
                           select t.ZIPCODE_ROWID).FirstOrDefault();
            return "";
        }
    }
}