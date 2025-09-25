namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using WBBBusinessLayer.Extension;
    using WBBBusinessLayer.SBNV2WebService;
    using WBBContract;
    using WBBContract.Queries.ExWebServices.FbbCpGw;
    using WBBData.Repository;
    using WBBEntity.Extensions;
    using WBBEntity.Models;
    using WBBEntity.PanelModels.ExWebServiceModels;
    using WBBEntity.PanelModels.WebServiceModels;

    public class GetRegResultQueryHandler : IQueryHandler<GetRegResultCoreQuery, CustRegisterInfoModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RESULT> _coverageAreaRes;

        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _coverageBuilding;

        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffChkProfLog;

        public GetRegResultQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> coverageArea,
            IEntityRepository<FBB_COVERAGEAREA_RESULT> coverageAreaRes,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_COVERAGEAREA_BUILDING> coverageBuilding,
            IEntityRepository<FBB_ZIPCODE> zipcode,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffChkProfLog)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _coverageAreaRes = coverageAreaRes;
            _lov = lov;
            _coverageBuilding = coverageBuilding;
            _sffChkProfLog = sffChkProfLog;
            _zipcode = zipcode;
        }

        public CustRegisterInfoModel Handle(GetRegResultCoreQuery query)
        {
            var custReginfoModel = new CustRegisterInfoModel();
            if (query.RegResultType == RegResultType.FBSS)
            {
                custReginfoModel = FBSSSaveOrder(query, custReginfoModel);
            }
            else if (query.RegResultType == RegResultType.CRM)
            {
                custReginfoModel = CRMSaveOrder(query, custReginfoModel);
            }

            return custReginfoModel;
        }

        private CustRegisterInfoModel CRMSaveOrder(GetRegResultCoreQuery query, CustRegisterInfoModel custReginfoModel)
        {
            var isThai = query.Language.ToCultureCode().IsThaiCulture();

            var latlngList = FbbCpGwCustRegisterHelper.FindLatLng(_coverageAreaRes, query.TransactionID);

            var sffComradeList = FbbCpGwCustRegisterHelper.FindSffComrades(_lov, _sffChkProfLog, query.MobileNo,
                                                                            query.IDCardNo, query.TransactionID);

            var coverageAreaList = FbbCpGwCustRegisterHelper.FindCvrId(_coverageAreaRes, query.TransactionID);

            var cvrId = "";
            var cvrResultId = "";

            if (coverageAreaList.Count == 2)
            {
                cvrId = coverageAreaList[0];
                cvrResultId = coverageAreaList[1];
            }

            var installZipCodeId = FbbCpGwCustRegisterHelper.FindZipCodeRowID(_zipcode,
                                                    query.InstallProvince, query.InstallDistrict, query.InstallSubDistrict)
                                                    .ToSafeString();

            var billZipCodeId = FbbCpGwCustRegisterHelper.FindZipCodeRowID(_zipcode,
                                    query.BillProvince, query.BillDistrict, query.BillSubDistrict)
                                    .ToSafeString();

            var sffCaNo = "";
            var sffSaNo = "";
            var sffBaNo = "";
            var networkType = "";
            var serviceDay = 0;

            if (sffComradeList.Count == 5)
            {
                sffCaNo = sffComradeList[0].ToSafeString();
                sffSaNo = sffComradeList[1].ToSafeString();
                sffBaNo = sffComradeList[2].ToSafeString();
                networkType = sffComradeList[3].ToSafeString();
                serviceDay = sffComradeList[4].ToSafeInteger();
            }

            var isNonMobile = FbbCpGwCustRegisterHelper.IsNonMobile(query.MobileNo);

            var SBNStatus = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();

            if (SBNStatus.LOV_VAL1 == "NEW")
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                #region NewSBNService
                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.saveOrderNew(
                            CUSTOMER_TYPE: "R",
                            CUSTOMER_SUBTYPE: "T",
                            TITLE_CODE: "127",
                            FIRST_NAME: query.CustFirstName.ToSafeString(),
                            LAST_NAME: query.CustLastName.ToSafeString(),
                            CONTACT_TITLE_CODE: "127",
                            CONTACT_FIRST_NAME: query.CustFirstName.ToSafeString(),
                            CONTACT_LAST_NAME: query.CustLastName.ToSafeString(),
                            ID_CARD_TYPE_DESC: isThai ? "บัตรประชาชน" : "ID_CARD",
                            ID_CARD_NO: query.IDCardNo.ToSafeString(),
                            TAX_ID: "",
                            GENDER: "F",
                            BIRTH_DATE: FbbCpGwCustRegisterHelper.ToDateString(isThai, query.CustBirthDate).ToSafeString(),
                            MOBILE_NO: query.ContactMobileNo.ToSafeString(),
                            MOBILE_NO_2: "",
                            HOME_PHONE_NO: query.ContactHomeNo.ToSafeString(),
                            EMAIL_ADDRESS: query.ContactEmail.ToSafeString(),
                            CONTACT_TIME: query.PreferInstallTime.ToSafeString(),
                            NATIONALITY_DESC: "THAI",
                            CUSTOMER_REMARK: "",
                            HOUSE_NO: query.InstallHouseNo.ToSafeString(),
                            MOO_NO: query.InstallMoo.ToSafeString(),
                            BUILDING: (query.InstallBuilding + query.InstallTower).ToSafeString(),
                            FLOOR: query.InstallFloor.ToSafeString(),
                            ROOM: "",
                            MOOBAN: query.InstallVillage.ToSafeString(),
                            SOI: query.InstallSoi.ToSafeString(),
                            ROAD: query.InstallRoad.ToSafeString(),
                            ZIPCODE_ROWID: installZipCodeId,
                            LATITUDE: "",
                            LONGTITUDE: "",
                            ASC_CODE: query.ASCCode.ToSafeString(),
                            EMPLOYEE_ID: query.StaffID.ToSafeString(),
                            LOCATION_CODE: query.LocationCode.ToSafeString(),
                            SALE_REPRESENT: query.SaleRep.ToSafeString(),
                            CS_NOTE: "",
                            WIFI_ACCESS_POINT: FbbCpGwCustRegisterHelper.HaveWifiAccessPoint(_lov, query.SelectPackage).ToSafeString(),
                            INSTALL_STATUS: "N",
                            COVERAGE: "Y",
                            EXISTING_AIRNET_NO: "",
                            GSM_MOBILE_NO: "",
                            CONTACT_NAME_1: "",
                            CONTACT_NAME_2: "",
                            CONTACT_MOBILE_NO_1: query.ContactHomeNo,
                            CONTACT_MOBILE_NO_2: query.ContactMobileNo,
                            CONDO_FLOOR: "",
                            CONDO_ROOF_TOP: "",
                            CONDO_BALCONY: "",
                            BALCONY_NORTH: "",
                            BALCONY_SOUTH: "",
                            BALCONY_EAST: "",
                            BALCONY_WAST: "",
                            HIGH_BUILDING: "",
                            HIGH_TREE: "",
                            BILLBOARD: "",
                            EXPRESSWAY: "",
                            ADDRESS_TYPE_WIRE: !string.IsNullOrEmpty(query.InstallBuilding) ? "อาคาร" : "หมู่บ้าน",
                            ADDRESS_TYPE: "",
                            FLOOR_NO: query.InstallFloor.ToSafeString(),
                            HOUSE_NO_BL: query.BillHouseNo.ToSafeString(),
                            MOO_NO_BL: query.BillMoo.ToSafeString(),
                            MOOBAN_BL: query.BillVillage.ToSafeString(),
                            BUILDING_BL: (query.BillBuilding + query.BillTower).ToSafeString(),
                            FLOOR_BL: query.BillFloor.ToSafeString(),
                            ROOM_BL: "",
                            SOI_BL: query.BillSoi.ToSafeString(),
                            ROAD_BL: query.BillRoad.ToSafeString(),
                            ZIPCODE_ROWID_BL: billZipCodeId,
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
                            SITE_CODE: query.SiteCode.ToSafeString(),
                            RELATE_MOBILE: isNonMobile ? "" : query.MobileNo.ToSafeString(),
                            RELATE_NON_MOBILE: isNonMobile ? query.MobileNo.ToSafeString() : "",
                            SFF_CA_NO: sffCaNo,
                            SFF_SA_NO: sffSaNo,
                            SFF_BA_NO: sffBaNo,
                            NETWORK_TYPE: networkType,
                            SERVICE_DAY: serviceDay.ToSafeString(),
                            EXPECT_INSTALL_DATE: FbbCpGwCustRegisterHelper.ToDateString(isThai, query.PreferInstallDate.ToSafeString()).ToSafeString(),
                            FTTX_VENDOR: FbbCpGwCustRegisterHelper.FindVendor(_coverageAreaRes, query.SelectPackage, query.TransactionID).ToSafeString(),
                            INSTALL_NOTE: FbbCpGwCustRegisterHelper.FindInstallNote(_coverageArea, _coverageBuilding, query.SelectPackage, isThai, cvrId, query.InstallTower).ToSafeString(),
                            PHONE_FLAG: "",
                            TIME_SLOT: "",
                            INSTALLATION_CAPACITY: "",
                            ADDRESS_ID: "",
                            ACCESS_MODE: "",
                            ENG_FLAG: query.Language.ToCultureCode().IsEngCulture().ToYesNoFlgString(),
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
                            FLOW_FLAG: query.FlowFlag.ToSafeString(),
                            LINE_ID: query.LineId.ToSafeString(),
                            RELATE_PROJECT_NAME: "",
                            PLUG_AND_PLAY_FLAG: "",
                            RESERVED_ID: query.RESERVED_ID,
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

                            AIR_REGIST_PACKAGE_ARRAY: query.SelectPackage.ConvertAll(x => new airRegistPackageRecord
                            {
                                faxFlag = x.FAX_FLAG.ToSafeString(),
                                homeIp = "",
                                homePort = "",
                                iddFlag = x.IDD_FLAG.ToSafeString(),
                                mobileForward = x.MOBILE_FORWARD.ToSafeString(),
                                packageCode = x.PACKAGE_CODE.ToSafeString(),
                                packagePrice = 0,
                                packagePriceSpecified = false,
                                packageType = x.PACKAGE_TYPE.ToSafeString(),
                                pboxExt = x.PLAYBOX_EXT.ToSafeString(),
                                productSubtype = x.PRODUCT_SUBTYPE.ToSafeString(),
                                tempIa = x.TEMP_IA.ToSafeString()
                            }).ToArray(),
                            AIR_REGIST_FILE_ARRAY: new List<airRegistFileRecord>().ToArray(),
                            AIR_REGIST_SPLITTER_ARRAY: query.Splitter.ConvertAll(x => new airRegistSplitterRecord
                            {
                                splitterName = x.Splitter_Name,
                                distance = x.Distance,
                                distanceType = x.Distance_Type,
                                distanceSpecified = true
                            }).ToArray(),
                            AIR_REGIST_CPE_SERIAL_ARRAY: new List<airRegistCpeSerialRecord>().ToArray(),
                            AIR_REGIST_CUST_INSIGHT_ARRAY: new List<airRegistCustInsightRecord>().ToArray(),
                            AIR_REGIST_DCONTRACT_ARRAY: new List<airRegistDcontractRecord>().ToArray(),
                            AIR_REGIST_CROSS_NETWORK_ARRAY: new List<airRegistCrossnetworkRecord>().ToArray(),
                            //R23.05 CheckFraud
                            AIR_FRAUD_REASON_ARRAY: new List<airFraudReasonRecord>().ToArray()
                        //end R23.05 CheckFraud
                        );

                    custReginfoModel = CreateCustRegInfoModel(
                        query,
                        cvrId: cvrId,
                        airReturnCode: data.RETURN_CODE.ToSafeString(),
                        airReturnMessage: data.RETURN_MESSAGE,
                        airReturnOrder: data.RETURN_SALE_ORDER,
                        installZipCodeId: installZipCodeId,
                        billZipCodeId: billZipCodeId,
                        coverageResultId: cvrResultId,
                        sffCaNo: sffCaNo,
                        sffSaNo: sffSaNo,
                        sffBaNo: sffBaNo,
                        isNonMobile: isNonMobile,
                        networkType: networkType,
                        serviceYear: serviceDay.ToSafeString());

                    custReginfoModel.ASCCode = query.ASCCode.ToSafeString();
                    custReginfoModel.StaffID = query.StaffID.ToSafeString();
                    custReginfoModel.LocationCode = query.LocationCode.ToSafeString();
                    custReginfoModel.SaleRep = query.SaleRep.ToSafeString();
                }
                #endregion
            }
            else
            {
                #region OldSBNService
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                //using (var service = new SBNWebService.SBNWebServiceService())
                //{
                //    service.Timeout = 600000;
                //    var data = service.saveOrderNew(
                //            CUSTOMER_TYPE: "R",
                //            CUSTOMER_SUBTYPE: "T",
                //            TITLE_CODE: "127",
                //            FIRST_NAME: query.CustFirstName.ToSafeString(),
                //            LAST_NAME: query.CustLastName.ToSafeString(),
                //            CONTACT_TITLE_CODE: "127",
                //            CONTACT_FIRST_NAME: query.CustFirstName.ToSafeString(),
                //            CONTACT_LAST_NAME: query.CustLastName.ToSafeString(),
                //            ID_CARD_TYPE_DESC: isThai ? "บัตรประชาชน" : "ID_CARD",
                //            ID_CARD_NO: query.IDCardNo.ToSafeString(),
                //            TAX_ID: "",
                //            GENDER: "F",
                //            BIRTH_DATE:
                //                FbbCpGwCustRegisterHelper.ToDateString(isThai, query.CustBirthDate)
                //                .ToSafeString(),
                //            MOBILE_NO: query.ContactMobileNo.ToSafeString(),
                //            MOBILE_NO_2: "",
                //            HOME_PHONE_NO: query.ContactHomeNo.ToSafeString(),
                //            EMAIL_ADDRESS: query.ContactEmail.ToSafeString(),
                //            CONTACT_TIME: query.PreferInstallTime.ToSafeString(),
                //            NATIONALITY_DESC: "THAI",
                //            CUSTOMER_REMARK: "",

                //            HOUSE_NO: query.InstallHouseNo.ToSafeString(),
                //            MOO_NO: query.InstallMoo.ToSafeString(),
                //            BUILDING: (query.InstallBuilding + query.InstallTower).ToSafeString(),
                //            FLOOR: query.InstallFloor.ToSafeString(),
                //            ROOM: "",
                //            MOOBAN: query.InstallVillage.ToSafeString(),
                //            SOI: query.InstallSoi.ToSafeString(),
                //            ROAD: query.InstallRoad.ToSafeString(),
                //            ZIPCODE_ROWID: installZipCodeId,

                //            LATITUDE: "",//latlngList[0].ToSafeString(),
                //            LONGTITUDE: "",//latlngList[1].ToSafeString(),
                //            ASC_CODE: query.ASCCode.ToSafeString(),
                //            EMPLOYEE_ID: query.StaffID.ToSafeString(),
                //            LOCATION_CODE: query.LocationCode.ToSafeString(),
                //            SALE_REPRESENT: query.SaleRep.ToSafeString(),
                //            CS_NOTE: "",
                //            WIFI_ACCESS_POINT:
                //                FbbCpGwCustRegisterHelper.HaveWifiAccessPoint(_lov, query.SelectPackage)
                //                .ToSafeString(),
                //            INSTALL_STATUS: "N",
                //            COVERAGE: "Y",
                //            EXISTING_AIRNET_NO: "",
                //            GSM_MOBILE_NO: "",
                //            CONTACT_NAME_1: "",
                //            CONTACT_NAME_2: "",
                //            CONTACT_MOBILE_NO_1: query.ContactHomeNo,
                //            CONTACT_MOBILE_NO_2: query.ContactMobileNo,
                //            CONDO_FLOOR: "",
                //            CONDO_ROOF_TOP: "",
                //            CONDO_BALCONY: "",
                //            BALCONY_NORTH: "",
                //            BALCONY_SOUTH: "",
                //            BALCONY_EAST: "",
                //            BALCONY_WAST: "",
                //            HIGH_BUILDING: "",
                //            HIGH_TREE: "",
                //            BILLBOARD: "",
                //            EXPRESSWAY: "",
                //            ADDRESS_TYPE_WIRE: !string.IsNullOrEmpty(query.InstallBuilding) ? "อาคาร" : "หมู่บ้าน",
                //        //FbbCpGwCustRegisterHelper.FindAddressTypeWire(_coverageAreaRes, query.TransactionID)
                //        //.ToSafeString(),
                //            ADDRESS_TYPE: "",

                //            FLOOR_NO: query.InstallFloor.ToSafeString(),
                //            HOUSE_NO_BL: query.BillHouseNo.ToSafeString(),
                //            MOO_NO_BL: query.BillMoo.ToSafeString(),
                //            BUILDING_BL: (query.BillBuilding + query.BillTower).ToSafeString(),
                //            FLOOR_BL: query.BillFloor.ToSafeString(),
                //            ROOM_BL: "",
                //            MOOBAN_BL: query.BillVillage.ToSafeString(),
                //            SOI_BL: query.BillSoi.ToSafeString(),
                //            ROAD_BL: query.BillRoad.ToSafeString(),
                //            ZIPCODE_ROWID_BL: billZipCodeId,

                //            HOUSE_NO_VT: "",
                //            MOO_NO_VT: "",
                //            BUILDING_VT: "",
                //            FLOOR_VT: "",
                //            ROOM_VT: "",
                //            MOOBAN_VT: "",
                //            SOI_VT: "",
                //            ROAD_VT: "",
                //            ZIPCODE_ROWID_VT: "",

                //            // FBSS
                //            CVR_ID: "",//cvrId.ToSafeString(),
                //            CVR_NODE: "",//query.InstallBuilding.ToSafeString(),
                //            CVR_TOWER: "",//query.InstallTower.ToSafeString(),
                //            SITE_CODE: "",
                //        // FBSS

                //            RELATE_MOBILE: isNonMobile ? "" : query.MobileNo.ToSafeString(),
                //            RELATE_NON_MOBILE: isNonMobile ? query.MobileNo.ToSafeString() : "",
                //            SFF_CA_NO: sffCaNo,
                //            SFF_SA_NO: sffSaNo,
                //            SFF_BA_NO: sffBaNo,
                //            NETWORK_TYPE: networkType,
                //            SERVICE_DAY: serviceDay,
                //            EXPECT_INSTALL_DATE: FbbCpGwCustRegisterHelper.ToDateString(isThai, query.PreferInstallDate.ToSafeString()).ToSafeString(),
                //            SERVICE_DAYSpecified: true,
                //            FTTX_VENDOR:
                //                FbbCpGwCustRegisterHelper.FindVendor(_coverageAreaRes,
                //                    query.SelectPackage, query.TransactionID)
                //                .ToSafeString(),
                //            INSTALL_NOTE: FbbCpGwCustRegisterHelper.FindInstallNote(_coverageArea,
                //                    _coverageBuilding, query.SelectPackage, isThai, cvrId, query.InstallTower)
                //                .ToSafeString(),

                //            // FBSS
                //            PHONE_FLAG: "",
                //            TIME_SLOT: "",
                //            INSTALLATION_CAPACITY: "",
                //            ADDRESS_ID: "",
                //            ACCESS_MODE: "",
                //            ENG_FLAG: query.Language.ToCultureCode().IsEngCulture().ToYesNoFlgString(),
                //            EVENT_CODE: "",
                //            INSTALLADDRESS1: "",
                //            INSTALLADDRESS2: "",
                //            INSTALLADDRESS3: "",
                //            INSTALLADDRESS4: "",
                //            INSTALLADDRESS5: "",
                //            PBOX_COUNT: "",
                //            CONVERGENCE_FLAG: "",
                //            TIME_SLOT_ID: "",
                //            AIR_REGIST_PACKAGE_ARRAY: FbbCpGwCustRegisterHelper.CreateAirnetPackageRecord(query.SelectPackage),
                //            air_regist_file_array: new List<AirRegistFileRecord>().ToArray()
                //        );

                //    //result.Add(data.RETURN_CODE.GetValueOrDefault().ToSafeString());
                //    //result.Add(data.RETURN_MESSAGE);
                //    //result.Add(data.RETURN_IA_NO);

                //    // do create the CustRegisterinfoModel

                //    custReginfoModel = CreateCustRegInfoModel(
                //        query,
                //        cvrId: cvrId,
                //        airReturnCode: data.RETURN_CODE.GetValueOrDefault().ToSafeString(),
                //        airReturnMessage: data.RETURN_MESSAGE,
                //        airReturnOrder: data.RETURN_SALE_ORDER,
                //        installZipCodeId: installZipCodeId,
                //        billZipCodeId: billZipCodeId,
                //        coverageResultId: cvrResultId,
                //        sffCaNo: sffCaNo,
                //        sffSaNo: sffSaNo,
                //        sffBaNo: sffBaNo,
                //        isNonMobile: isNonMobile,
                //        networkType: networkType,
                //        serviceYear: serviceDay.ToSafeString());

                //    custReginfoModel.ASCCode = query.ASCCode.ToSafeString();
                //    custReginfoModel.StaffID = query.StaffID.ToSafeString();
                //    custReginfoModel.LocationCode = query.LocationCode.ToSafeString();
                //    custReginfoModel.SaleRep = query.SaleRep.ToSafeString();
                //}
                #endregion

            }

            return custReginfoModel;
        }

        private CustRegisterInfoModel FBSSSaveOrder(GetRegResultCoreQuery query, CustRegisterInfoModel custReginfoModel)
        {
            var isThai = query.Language.ToCultureCode().IsThaiCulture();
            //var result = new List<string>();
            //var latlngList = FbbCpGwCustRegisterHelper.FindLatLng(_coverageAreaRes, query.TransactionID);
            var sffComradeList = FbbCpGwCustRegisterHelper.FindSffComrades(_lov, _sffChkProfLog, query.MobileNo, query.IDCardNo, query.TransactionID);
            //var coverageAreaList = FbbCpGwCustRegisterHelper.FindCvrId(_coverageAreaRes, query.TransactionID);
            //var cvrId = coverageAreaList[0];
            //var cvrResultId = coverageAreaList[1];
            //result.Add(cvrResultId);
            var installZipCodeId = FbbCpGwCustRegisterHelper.FindZipCodeRowID(_zipcode, query.InstallProvince, query.InstallDistrict, query.InstallSubDistrict).ToSafeString();
            var billZipCodeId = FbbCpGwCustRegisterHelper.FindZipCodeRowID(_zipcode, query.BillProvince, query.BillDistrict, query.BillSubDistrict).ToSafeString();

            var sffCaNo = sffComradeList[0].ToSafeString();
            var sffSaNo = sffComradeList[1].ToSafeString();
            var sffBaNo = sffComradeList[2].ToSafeString();
            var networkType = sffComradeList[3].ToSafeString();
            var serviceDay = sffComradeList[4].ToSafeInteger();

            var isNonMobile = FbbCpGwCustRegisterHelper.IsNonMobile(query.MobileNo);

            var mainPack = (from t in query.SelectPackage
                            where t.PACKAGE_TYPE.IsMainPack()
                            select t).FirstOrDefault();

            var accessMode = mainPack.ACCESS_MODE;

            var internetOntops = (from t in query.SelectPackage
                                  where !t.PACKAGE_TYPE.IsMainPack()
                                   && !t.PRODUCT_SUBTYPE.IsNonInternetPackage()
                                  select t).ToList();

            var nonInternetOntops = (from t in query.SelectPackage
                                     where !t.PACKAGE_TYPE.IsMainPack()
                                      && t.PRODUCT_SUBTYPE.IsNonInternetPackage()
                                     orderby t.PRODUCT_SUBTYPE
                                     group t by t.PRODUCT_SUBTYPE into g
                                     select g).ToList();

            var orderedSelectedPackage = new List<PackageModel>();
            orderedSelectedPackage.Add(mainPack);
            orderedSelectedPackage.AddRange(internetOntops);

            if (nonInternetOntops.Any())
            {
                var enquirePackageQuery = new GetListPackageByServiceQuery
                {
                    P_OWNER_PRODUCT = mainPack.OWNER_PRODUCT,
                    P_PRODUCT_SUBTYPE = "",
                    P_NETWORK_TYPE = "",
                    P_SERVICE_DAY = "0",
                    P_PACKAGE_FOR = "PUBLIC",
                    P_PACKAGE_CODE = mainPack.PACKAGE_CODE,
                };

                var enquirePackage = GetPackageListHelper
                                        .GetPackageList(_logger, enquirePackageQuery, _lov)
                                        .Where(t => t.PRODUCT_SUBTYPE != "VOIP" && t.PACKAGE_TYPE != "Bundle");

                foreach (var nonInternetPack in nonInternetOntops)
                {
                    orderedSelectedPackage.AddRange(enquirePackage
                                            .Where(t => t.PRODUCT_SUBTYPE == nonInternetPack.Key)
                                            .OrderByDescending(t => t.PRODUCT_SUBTYPE3));
                }

            }

            var SBNStatus = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();

            if (SBNStatus.LOV_VAL1 == "NEW")
            {
                #region newSBNService
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;
                    var data = service.saveOrderNew(
                           CUSTOMER_TYPE: "R",
                           CUSTOMER_SUBTYPE: "T",
                           TITLE_CODE: "127",
                           FIRST_NAME: query.CustFirstName.ToSafeString(),
                           LAST_NAME: query.CustLastName.ToSafeString(),
                           CONTACT_TITLE_CODE: "127",
                           CONTACT_FIRST_NAME: query.CustFirstName.ToSafeString(),
                           CONTACT_LAST_NAME: query.CustLastName.ToSafeString(),
                           ID_CARD_TYPE_DESC: isThai ? "บัตรประชาชน" : "ID_CARD",
                           ID_CARD_NO: query.IDCardNo.ToSafeString(),
                           TAX_ID: "",
                           GENDER: "F",
                           BIRTH_DATE: FbbCpGwCustRegisterHelper.ToDateString(isThai, query.CustBirthDate).ToSafeString(),
                           MOBILE_NO: query.ContactMobileNo.ToSafeString(),
                           MOBILE_NO_2: "",
                           HOME_PHONE_NO: query.ContactHomeNo.ToSafeString(),
                           EMAIL_ADDRESS: query.ContactEmail.ToSafeString(),
                           CONTACT_TIME: query.PreferInstallTime.ToSafeString(),
                           NATIONALITY_DESC: "THAI",
                           CUSTOMER_REMARK: "",
                           HOUSE_NO: query.InstallHouseNo.ToSafeString(),
                           MOO_NO: query.InstallMoo.ToSafeString(),
                           BUILDING: (query.InstallBuilding + query.InstallTower).ToSafeString(),
                           FLOOR: query.InstallFloor.ToSafeString(),
                           ROOM: "",
                           MOOBAN: query.InstallVillage.ToSafeString(),
                           SOI: query.InstallSoi.ToSafeString(),
                           ROAD: query.InstallRoad.ToSafeString(),
                           ZIPCODE_ROWID: installZipCodeId,
                           LATITUDE: "",
                           LONGTITUDE: "",
                           ASC_CODE: query.ASCCode.ToSafeString(),
                           EMPLOYEE_ID: query.StaffID.ToSafeString(),
                           LOCATION_CODE: query.LocationCode.ToSafeString(),
                           SALE_REPRESENT: query.SaleRep.ToSafeString(),
                           CS_NOTE: "",
                           WIFI_ACCESS_POINT: FbbCpGwCustRegisterHelper.HaveWifiAccessPoint(_lov, query.SelectPackage).ToSafeString(),
                           INSTALL_STATUS: "N",
                           COVERAGE: "Y",
                           EXISTING_AIRNET_NO: "",
                           GSM_MOBILE_NO: "",
                           CONTACT_NAME_1: "",
                           CONTACT_NAME_2: "",
                           CONTACT_MOBILE_NO_1: query.ContactHomeNo,
                           CONTACT_MOBILE_NO_2: query.ContactMobileNo,
                           CONDO_FLOOR: "",
                           CONDO_ROOF_TOP: "",
                           CONDO_BALCONY: "",
                           BALCONY_NORTH: "",
                           BALCONY_SOUTH: "",
                           BALCONY_EAST: "",
                           BALCONY_WAST: "",
                           HIGH_BUILDING: "",
                           HIGH_TREE: "",
                           BILLBOARD: "",
                           EXPRESSWAY: "",
                           ADDRESS_TYPE_WIRE: !string.IsNullOrEmpty(query.InstallBuilding) ? "อาคาร" : "หมู่บ้าน",
                           ADDRESS_TYPE: "",
                           FLOOR_NO: query.InstallFloor.ToSafeString(),
                           HOUSE_NO_BL: query.BillHouseNo.ToSafeString(),
                           MOO_NO_BL: query.BillMoo.ToSafeString(),
                           MOOBAN_BL: query.BillVillage.ToSafeString(),
                           BUILDING_BL: (query.BillBuilding + query.BillTower).ToSafeString(),
                           FLOOR_BL: query.BillFloor.ToSafeString(),
                           ROOM_BL: "",
                           SOI_BL: query.BillSoi.ToSafeString(),
                           ROAD_BL: query.BillRoad.ToSafeString(),
                           ZIPCODE_ROWID_BL: billZipCodeId,
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
                           SITE_CODE: query.SiteCode.ToSafeString(),
                           RELATE_MOBILE: isNonMobile ? "" : query.MobileNo.ToSafeString(),
                           RELATE_NON_MOBILE: isNonMobile ? query.MobileNo.ToSafeString() : "",
                           SFF_CA_NO: sffCaNo,
                           SFF_SA_NO: sffSaNo,
                           SFF_BA_NO: sffBaNo,
                           NETWORK_TYPE: networkType,
                           SERVICE_DAY: serviceDay.ToSafeString(),
                           EXPECT_INSTALL_DATE: FbbCpGwCustRegisterHelper.ToDateString(isThai, query.PreferInstallDate.ToSafeString()).ToSafeString(),
                           FTTX_VENDOR: FbbCpGwCustRegisterHelper.FindVendor(_coverageAreaRes, query.SelectPackage, query.TransactionID).ToSafeString(),
                           INSTALL_NOTE: "",
                           PHONE_FLAG: "",
                           TIME_SLOT: "",
                           INSTALLATION_CAPACITY: "",
                           ADDRESS_ID: "",
                           ACCESS_MODE: "",
                           ENG_FLAG: query.Language.ToCultureCode().IsEngCulture().ToYesNoFlgString(),
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
                           FLOW_FLAG: query.FlowFlag.ToSafeString(),
                           LINE_ID: query.LineId.ToSafeString(),
                           RELATE_PROJECT_NAME: "",
                           PLUG_AND_PLAY_FLAG: "",
                           RESERVED_ID: query.RESERVED_ID,
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

                            AIR_REGIST_PACKAGE_ARRAY: query.SelectPackage.ConvertAll(x => new airRegistPackageRecord
                            {
                                faxFlag = x.FAX_FLAG.ToSafeString(),
                                homeIp = "",
                                homePort = "",
                                iddFlag = x.IDD_FLAG.ToSafeString(),
                                mobileForward = x.MOBILE_FORWARD.ToSafeString(),
                                packageCode = x.PACKAGE_CODE.ToSafeString(),
                                packagePrice = 0,
                                packagePriceSpecified = false,
                                packageType = x.PACKAGE_TYPE.ToSafeString(),
                                pboxExt = x.PLAYBOX_EXT.ToSafeString(),
                                productSubtype = x.PRODUCT_SUBTYPE.ToSafeString(),
                                tempIa = x.TEMP_IA.ToSafeString()
                            }).ToArray(),
                           AIR_REGIST_FILE_ARRAY: new List<airRegistFileRecord>().ToArray(),
                           AIR_REGIST_SPLITTER_ARRAY: query.Splitter.ConvertAll(x => new airRegistSplitterRecord
                           {
                               splitterName = x.Splitter_Name,
                               distance = x.Distance,
                               distanceType = x.Distance_Type,
                               distanceSpecified = true
                           }).ToArray(),
                           AIR_REGIST_CPE_SERIAL_ARRAY: new List<airRegistCpeSerialRecord>().ToArray(),
                           AIR_REGIST_CUST_INSIGHT_ARRAY: new List<airRegistCustInsightRecord>().ToArray(),
                           AIR_REGIST_DCONTRACT_ARRAY: new List<airRegistDcontractRecord>().ToArray(),
                           AIR_REGIST_CROSS_NETWORK_ARRAY: new List<airRegistCrossnetworkRecord>().ToArray(),
                           //R23.05 CheckFraud
                           AIR_FRAUD_REASON_ARRAY: new List<airFraudReasonRecord>().ToArray()
                    //end R23.05 CheckFraud
                    );

                    //result.Add(data.RETURN_CODE.GetValueOrDefault().ToSafeString());
                    //result.Add(data.RETURN_MESSAGE);
                    //result.Add(data.RETURN_IA_NO);

                    // do create the CustRegisterinfoModel
                    custReginfoModel = CreateCustRegInfoModel(
                        query,
                        cvrId: "",
                        airReturnCode: data.RETURN_CODE.ToSafeString(),
                        airReturnMessage: data.RETURN_MESSAGE,
                        airReturnOrder: data.RETURN_SALE_ORDER,
                        installZipCodeId: installZipCodeId,
                        billZipCodeId: billZipCodeId,
                        coverageResultId: "",
                        sffCaNo: sffCaNo,
                        sffSaNo: sffSaNo,
                        sffBaNo: sffBaNo,
                        isNonMobile: isNonMobile,
                        networkType: networkType,
                        serviceYear: serviceDay.ToSafeString());

                    custReginfoModel.ASCCode = query.ASCCode.ToSafeString();
                    custReginfoModel.StaffID = query.StaffID.ToSafeString();
                    custReginfoModel.LocationCode = query.LocationCode.ToSafeString();
                    custReginfoModel.SaleRep = query.SaleRep.ToSafeString();
                }
                #endregion
            }
            else
            {
                #region oldSBNService

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                //using (var service = new SBNWebService.SBNWebServiceService())
                // {
                //     service.Timeout = 600000;
                //     var data = service.saveOrderNew(
                //         CUSTOMER_TYPE: "R",
                //         CUSTOMER_SUBTYPE: "T",
                //         TITLE_CODE: "127",
                //         FIRST_NAME: query.CustFirstName.ToSafeString(),
                //         LAST_NAME: query.CustLastName.ToSafeString(),
                //         CONTACT_TITLE_CODE: "127",
                //         CONTACT_FIRST_NAME: query.CustFirstName.ToSafeString(),
                //         CONTACT_LAST_NAME: query.CustLastName.ToSafeString(),
                //         ID_CARD_TYPE_DESC: isThai ? "บัตรประชาชน" : "ID_CARD",
                //         ID_CARD_NO: query.IDCardNo.ToSafeString(),
                //         TAX_ID: "",
                //         GENDER: "F",
                //         BIRTH_DATE: FbbCpGwCustRegisterHelper.ToDateString(isThai, query.CustBirthDate).ToSafeString(),
                //         MOBILE_NO: query.ContactMobileNo.ToSafeString(),
                //         MOBILE_NO_2: "",
                //         HOME_PHONE_NO: query.ContactHomeNo.ToSafeString(),
                //         EMAIL_ADDRESS: query.ContactEmail.ToSafeString(),
                //         CONTACT_TIME: query.PreferInstallTime.ToSafeString(),
                //         NATIONALITY_DESC: "THAI",
                //         CUSTOMER_REMARK: "",

                //         HOUSE_NO: query.InstallHouseNo.ToSafeString(),
                //         MOO_NO: query.InstallMoo.ToSafeString(),
                //         BUILDING: (query.InstallBuilding + query.InstallTower).ToSafeString(),
                //         FLOOR: query.InstallFloor.ToSafeString(),
                //         ROOM: "",
                //         MOOBAN: query.InstallVillage.ToSafeString(),
                //         SOI: query.InstallSoi.ToSafeString(),
                //         ROAD: query.InstallRoad.ToSafeString(),
                //         ZIPCODE_ROWID: installZipCodeId,

                //         LATITUDE: "",//latlngList[0].ToSafeString(),
                //         LONGTITUDE: "",//latlngList[1].ToSafeString(),
                //         ASC_CODE: query.ASCCode.ToSafeString(),
                //         EMPLOYEE_ID: query.StaffID.ToSafeString(),
                //         LOCATION_CODE: query.LocationCode.ToSafeString(),
                //         SALE_REPRESENT: query.SaleRep.ToSafeString(),
                //         CS_NOTE: "",
                //         WIFI_ACCESS_POINT:
                //             FbbCpGwCustRegisterHelper.HaveWifiAccessPoint(_lov, query.SelectPackage)
                //             .ToSafeString(),
                //         INSTALL_STATUS: "N",
                //         COVERAGE: "Y",
                //         EXISTING_AIRNET_NO: "",
                //         GSM_MOBILE_NO: "",
                //         CONTACT_NAME_1: "",
                //         CONTACT_NAME_2: "",
                //         CONTACT_MOBILE_NO_1: query.ContactHomeNo,
                //         CONTACT_MOBILE_NO_2: query.ContactMobileNo,
                //         CONDO_FLOOR: "",
                //         CONDO_ROOF_TOP: "",
                //         CONDO_BALCONY: "",
                //         BALCONY_NORTH: "",
                //         BALCONY_SOUTH: "",
                //         BALCONY_EAST: "",
                //         BALCONY_WAST: "",
                //         HIGH_BUILDING: "",
                //         HIGH_TREE: "",
                //         BILLBOARD: "",
                //         EXPRESSWAY: "",
                //         ADDRESS_TYPE_WIRE: !string.IsNullOrEmpty(query.InstallBuilding) ? "อาคาร" : "หมู่บ้าน",
                //         //FbbCpGwCustRegisterHelper.FindAddressTypeWire(_coverageAreaRes, query.TransactionID)
                //         //.ToSafeString(),
                //         ADDRESS_TYPE: "",

                //         FLOOR_NO: query.InstallFloor.ToSafeString(),
                //         HOUSE_NO_BL: query.BillHouseNo.ToSafeString(),
                //         MOO_NO_BL: query.BillMoo.ToSafeString(),
                //         BUILDING_BL: (query.BillBuilding + query.BillTower).ToSafeString(),
                //         FLOOR_BL: query.BillFloor.ToSafeString(),
                //         ROOM_BL: "",
                //         MOOBAN_BL: query.BillVillage.ToSafeString(),
                //         SOI_BL: query.BillSoi.ToSafeString(),
                //         ROAD_BL: query.BillRoad.ToSafeString(),
                //         ZIPCODE_ROWID_BL: billZipCodeId,

                //         HOUSE_NO_VT: "",
                //         MOO_NO_VT: "",
                //         BUILDING_VT: "",
                //         FLOOR_VT: "",
                //         ROOM_VT: "",
                //         MOOBAN_VT: "",
                //         SOI_VT: "",
                //         ROAD_VT: "",
                //         ZIPCODE_ROWID_VT: "",

                //         // FBSS
                //         CVR_ID: "",//cvrId.ToSafeString(),
                //         CVR_NODE: "",//query.InstallBuilding.ToSafeString(),
                //         CVR_TOWER: "",//query.InstallTower.ToSafeString(),
                //         SITE_CODE: "",
                //         // FBSS

                //         RELATE_MOBILE: isNonMobile ? "" : query.MobileNo.ToSafeString(),
                //         RELATE_NON_MOBILE: isNonMobile ? query.MobileNo.ToSafeString() : "",
                //         SFF_CA_NO: sffCaNo,
                //         SFF_SA_NO: sffSaNo,
                //         SFF_BA_NO: sffBaNo,
                //         NETWORK_TYPE: networkType,
                //         SERVICE_DAY: serviceDay,
                //         EXPECT_INSTALL_DATE: FbbCpGwCustRegisterHelper.ToDateString(isThai, query.PreferInstallDate.ToSafeString()).ToSafeString(),
                //         SERVICE_DAYSpecified: true,
                //         FTTX_VENDOR: FbbCpGwCustRegisterHelper
                //                         .FindVendorByPartner(_lov, query.PartnerName)
                //                         .ToSafeString(),

                //         INSTALL_NOTE: "",
                //         //FbbCpGwCustRegisterHelper.FindInstallNote(_coverageArea,
                //         //        _coverageBuilding, query.SelectPackage, isThai, cvrId, query.InstallTower)
                //         //    .ToSafeString(),

                //         // FBSS
                //         PHONE_FLAG: query.PhoneFlag.ToSafeString(),
                //         TIME_SLOT: query.TimeSlot.ToSafeString(),
                //         INSTALLATION_CAPACITY: query.InstallCapacity.ToSafeString(),
                //         ADDRESS_ID: query.AddressId.ToSafeString(),
                //         ACCESS_MODE: accessMode.ToSafeString(),
                //         ENG_FLAG: query.Language.ToCultureCode().IsEngCulture().ToYesNoFlgString(),
                //         EVENT_CODE: "",
                //         INSTALLADDRESS1: "",
                //         INSTALLADDRESS2: "",
                //         INSTALLADDRESS3: "",
                //         INSTALLADDRESS4: "",
                //         INSTALLADDRESS5: "",
                //         PBOX_COUNT: "",
                //         CONVERGENCE_FLAG: "",
                //         TIME_SLOT_ID: "",
                //         AIR_REGIST_PACKAGE_ARRAY: FbbCpGwCustRegisterHelper.CreateAirnetPackageRecord(orderedSelectedPackage),
                //         air_regist_file_array: new List<AirRegistFileRecord>().ToArray()
                //     );

                //     //result.Add(data.RETURN_CODE.GetValueOrDefault().ToSafeString());
                //     //result.Add(data.RETURN_MESSAGE);
                //     //result.Add(data.RETURN_IA_NO);

                //     // do create the CustRegisterinfoModel

                //     custReginfoModel = CreateCustRegInfoModel(
                //         query,
                //         cvrId: "",
                //         airReturnCode: data.RETURN_CODE.GetValueOrDefault().ToSafeString(),
                //         airReturnMessage: data.RETURN_MESSAGE,
                //         airReturnOrder: data.RETURN_SALE_ORDER,
                //         installZipCodeId: installZipCodeId,
                //         billZipCodeId: billZipCodeId,
                //         coverageResultId: "",
                //         sffCaNo: sffCaNo,
                //         sffSaNo: sffSaNo,
                //         sffBaNo: sffBaNo,
                //         isNonMobile: isNonMobile,
                //         networkType: networkType,
                //         serviceYear: serviceDay.ToSafeString());

                //     custReginfoModel.ASCCode = query.ASCCode.ToSafeString();
                //     custReginfoModel.StaffID = query.StaffID.ToSafeString();
                //     custReginfoModel.LocationCode = query.LocationCode.ToSafeString();
                //     custReginfoModel.SaleRep = query.SaleRep.ToSafeString();
                // }
                #endregion
            }

            return custReginfoModel;
        }

        private CustRegisterInfoModel CreateCustRegInfoModel(GetRegResultCoreQuery query,
            string cvrId, string airReturnCode, string airReturnMessage, string airReturnOrder,
            string installZipCodeId, string billZipCodeId, string coverageResultId,
            string sffCaNo, string sffSaNo, string sffBaNo, bool isNonMobile, string networkType,
            string serviceYear)
        {
            return new CustRegisterInfoModel
            {
                CvrId = cvrId,
                AirReturnCode = airReturnCode,
                AirReturnMessage = airReturnMessage,
                AirReturnOrder = airReturnOrder,
                InstallZipCodeId = installZipCodeId,
                BillZipCodeId = billZipCodeId,
                CoverageResultId = coverageResultId,
                SffCaNo = sffCaNo,
                SffSaNo = sffSaNo,
                SffBaNo = sffBaNo,
                IsNonMobile = isNonMobile,
                NetworkType = networkType,
                ServiceYear = serviceYear,

                IDCardNo = query.IDCardNo,
                MobileNo = query.MobileNo,
                Language = query.Language,
                CustFirstName = query.CustFirstName,
                CustLastName = query.CustLastName,
                CustBirthDate = query.CustBirthDate,
                ContactMobileNo = query.ContactMobileNo,
                ContactHomeNo = query.ContactHomeNo,
                ContactEmail = query.ContactEmail,
                PreferInstallDate = query.PreferInstallDate,
                PreferInstallTime = query.PreferInstallTime,
                InstallHouseNo = query.InstallHouseNo,
                InstallMoo = query.InstallMoo,
                InstallBuilding = query.InstallBuilding,
                InstallTower = query.InstallTower,
                InstallFloor = query.InstallFloor,
                InstallVillage = query.InstallVillage,
                InstallSoi = query.InstallSoi,
                InstallRoad = query.InstallRoad,
                InstallProvince = query.InstallProvince,
                InstallDistrict = query.InstallDistrict,
                InstallSubDistrict = query.InstallSubDistrict,
                InstallZipCode = query.InstallZipCode,
                BillHouseNo = query.BillHouseNo,
                BillMoo = query.BillMoo,
                BillBuilding = query.BillBuilding,
                BillTower = query.BillTower,
                BillFloor = query.BillFloor,
                BillVillage = query.BillVillage,
                BillSoi = query.BillSoi,
                BillRoad = query.BillRoad,
                BillProvince = query.BillProvince,
                BillDistrict = query.BillDistrict,
                BillSubDistrict = query.BillSubDistrict,
                BillZipCode = query.BillZipCode,
                SelectPackage = query.SelectPackage,
                RegisterResult = new RegisterResult
                {
                    CoverageResultId = coverageResultId,
                    ReturnCode = airReturnCode,
                    ReturnIANO = airReturnOrder,
                    ReturnMessage = airReturnMessage,
                }
            };
        }
    }
}
