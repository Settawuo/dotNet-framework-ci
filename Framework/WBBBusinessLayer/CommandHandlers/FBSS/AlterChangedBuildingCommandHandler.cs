using AIRNETEntity.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using WBBBusinessLayer.Extension;
using WBBBusinessLayer.FBSSOrderServices;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands.WebServices.FBSS;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class AlterChangedBuildingCommandHandler : ICommandHandler<AlterChangedBuildingCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetUnitOfWork _uowAir;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _listBV;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _coverageResult;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_FTTR> _air_package_fttr;
        private readonly IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> _queryProcessor;
        private readonly MemoryCache cache = MemoryCache.Default;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public AlterChangedBuildingCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IAirNetUnitOfWork uowAir,
            IEntityRepository<FBB_FBSS_LISTBV> listBV,
            IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> coverageResult,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
            IEntityRepository<FBB_CFG_LOV> lov,
            IAirNetEntityRepository<AIR_PACKAGE_FTTR> air_package_fttr,
            IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel> queryProcessor,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _uowAir = uowAir;
            _listBV = listBV;
            _coverageResult = coverageResult;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _lov = lov;
            _air_package_fttr = air_package_fttr;
            _queryProcessor = queryProcessor;
            _intfLog = intfLog;
        }

        public void Handle(AlterChangedBuildingCommand command)
        {
            bool IsError = false;
            var newonly = command.FBSSChangedAddressInfos.Where(t => t.ChangedAction == "N" && (t.AddressType != "O" && t.AddressType != "D")).ToList();
            int numInsert = 0;
            int numUpdate = 0;
            int numDelete = 0;

            // neww case "N":
            foreach (var neww in newonly)
            {
                try
                {
                    #region Get Ziprowid
                    var ziprow = (from r in _FBB_ZIPCODE.Get()
                                  where r.ZIPCODE == neww.PostalCode && r.TUMBON == neww.SUBDISTRICT && r.STATUS == "A"
                                  select r.ZIPCODE_ROWID).ToList().FirstOrDefault();
                    #endregion
                    if (ziprow != null)
                    {
                        var insertBv = (from t in _listBV.Get() where t.ADDRESS_ID == neww.AddressId && t.LANGUAGE == neww.Language select t).FirstOrDefault();

                        if (null == insertBv)
                        {
                            #region R21.1 Check FTTR
                            var N_chkFTTR = checkFTTR(neww.AddressType.ToSafeString(), neww.ACCESS_MODE.ToSafeString());
                            var N_fttrType = getFTTR_TYPE(N_chkFTTR, neww.FTTR_FLAG.ToSafeString());
                            #endregion

                            var newBv = new FBB_FBSS_LISTBV
                            {
                                ACTIVE_FLAG = "Y",//changedAdrs.ChangedAction.ToSafeString(),
                                ADDRESS_ID = neww.AddressId,
                                ADDRESS_TYPE = neww.AddressType.ToSafeString(),
                                BUILDING_NAME = neww.BuildingName,
                                BUILDING_NO = neww.BuildingNo,
                                LANGUAGE = neww.Language.ToSafeString(),
                                POSTAL_CODE = neww.PostalCode,
                                CREATED_BY = command.ActionBy,
                                CREATED_DATE = DateTime.Now,
                                SUB_DISTRICT = neww.SUBDISTRICT,
                                ACCESS_MODE = neww.ACCESS_MODE,
                                SITE_CODE = neww.SITE_CODE,
                                PARTNER = neww.PARTNER,
                                LATITUDE = neww.LATITUDE,
                                LONGTITUDE = neww.LONGTITUDE,

                                //R21.1 
                                FTTR_FLAG = neww.FTTR_FLAG.ToSafeString(),
                                SPECIFIC_TEAM_1 = neww.SPECIFIC_TEAM_1.ToSafeString(),
                                SPECIFIC_TEAM_2 = neww.SPECIFIC_TEAM_2.ToSafeString(),
                                FTTR_TYPE = N_fttrType.ToSafeString()
                            };

                            _listBV.Create(newBv);

                            #region check and insert
                            var coverageResult = new FBSSCoverageResult();

                            coverageResult = FBSSFeasCheck(neww.AddressType.ToSafeString(), neww.PostalCode, neww.SUBDISTRICT, neww.Language.ToSafeString(), neww.BuildingName
                               , neww.BuildingNo, "N", "5", "", "", "");

                            insertCoverageResult(neww.AddressType.ToSafeString(), neww.PostalCode, neww.SUBDISTRICT, neww.Language.ToSafeString(), neww.BuildingName
                               , neww.BuildingNo, "N", "5", "5", 0, "", "", "", "", coverageResult.Coverage, coverageResult.AddressId, coverageResult.AccessModeList.DumpToXml()
                               , coverageResult.PlanningSite.DumpToXml(), coverageResult.IsPartner, coverageResult.PartnerName, "", ziprow, "", "NEW", "FBBCONFIG");

                            numInsert++;
                            #endregion

                            //R21.1 FTTR => Insert Update Table AIR_PACKAGE_FTTR
                            if (N_chkFTTR)
                            {
                                var chkDupAirPackageFTTR = checkDupAirPackageFTTR(neww.AddressId.ToSafeString());
                                if (!chkDupAirPackageFTTR)
                                    insertAirPackageFTTR(neww.AddressId.ToSafeString(), "FTTR", "AWN", command.ActionBy);
                                else
                                    updateAirPackageFTTR(neww.AddressId.ToSafeString(), command.ActionBy);
                            }
                        }
                        else
                        { //R21.1 
                            #region R21.1 Check FTTR
                            var N2_chkFTTR = checkFTTR(neww.AddressType.ToSafeString(), neww.ACCESS_MODE.ToSafeString());
                            var N2_fttrType = getFTTR_TYPE(N2_chkFTTR, neww.FTTR_FLAG.ToSafeString());
                            #endregion
                            //R21.1 FTTR => Insert Update Table AIR_PACKAGE_FTTR
                            if (N2_chkFTTR)
                            {
                                var chkDupAirPackageFTTR = checkDupAirPackageFTTR(neww.AddressId.ToSafeString());
                                if (!chkDupAirPackageFTTR)
                                    insertAirPackageFTTR(neww.AddressId.ToSafeString(), "FTTR", "AWN", command.ActionBy);
                                else
                                    updateAirPackageFTTR(neww.AddressId.ToSafeString(), command.ActionBy);
                            }
                        }
                    }
                    else
                    {
                        command.errormsg += " Insert Data AddressId : " + neww.AddressId + " Error ZIPCODE_ROWID : The ZIPCODE_ROWID field is required.";
                    }
                }
                catch (Exception ex)
                {
                    command.errormsg = " Insert Data AddressId : " + neww.AddressId + " Error " + ex.Message;
                    IsError = true;
                }

            }
            if (!IsError)
            {
                _uow.Persist();
            }

            foreach (var changedAdrs in command.FBSSChangedAddressInfos.Where(t => t.AddressType != "D"))
            {
                try
                {
                    if (!IsAddressNeedUpdate(changedAdrs))
                        continue;
                    if (changedAdrs.AddressType.ToSafeString() != "O")
                    {
                        switch (changedAdrs.ChangedAction)
                        {
                            //// newly
                            //case "N":
                            //    var insertBv = (from t in _listBV.Get() where t.ADDRESS_ID == changedAdrs.AddressId select t)
                            //       .FirstOrDefault();
                            //    if (null == insertBv)
                            //    {
                            //        var newBv = new FBB_FBSS_LISTBV
                            //        {
                            //            ACTIVE_FLAG = "Y",//changedAdrs.ChangedAction.ToSafeString(),
                            //            ADDRESS_ID = changedAdrs.AddressId,
                            //            ADDRESS_TYPE = changedAdrs.AddressType.ToSafeString(),
                            //            BUILDING_NAME = changedAdrs.BuildingName,
                            //            BUILDING_NO = changedAdrs.BuildingNo,
                            //            LANGUAGE = changedAdrs.Language.ToSafeString(),
                            //            POSTAL_CODE = changedAdrs.PostalCode,
                            //            CREATED_BY = command.ActionBy,
                            //            CREATED_DATE = DateTime.Now,
                            //            SUB_DISTRICT = changedAdrs.SUBDISTRICT
                            //        };

                            //        _listBV.Create(newBv);
                            //    }
                            //    break;

                            // modified
                            case "M":
                                var updateBv = (from t in _listBV.Get() where t.ADDRESS_ID == changedAdrs.AddressId && t.LANGUAGE == changedAdrs.Language select t).FirstOrDefault();

                                // Insert
                                if (null == updateBv)
                                {
                                    #region Get Ziprowid
                                    var ziprow = (from r in _FBB_ZIPCODE.Get()
                                                  where r.ZIPCODE == changedAdrs.PostalCode && r.TUMBON == changedAdrs.SUBDISTRICT && r.STATUS == "A"
                                                  select r.ZIPCODE_ROWID
                                            ).ToList().FirstOrDefault();

                                    #endregion

                                    if (ziprow != null)
                                    {
                                        #region R21.1 Check FTTR
                                        var M_chkFTTR = checkFTTR(changedAdrs.AddressType.ToSafeString(), changedAdrs.ACCESS_MODE.ToSafeString());
                                        var M_fttrType = getFTTR_TYPE(M_chkFTTR, changedAdrs.FTTR_FLAG.ToSafeString());
                                        #endregion

                                        var newUpdateBv = new FBB_FBSS_LISTBV
                                        {
                                            ACTIVE_FLAG = "Y",//changedAdrs.ChangedAction.ToSafeString(),
                                            ADDRESS_ID = changedAdrs.AddressId,
                                            ADDRESS_TYPE = changedAdrs.AddressType.ToSafeString(),
                                            BUILDING_NAME = changedAdrs.BuildingName,
                                            BUILDING_NO = changedAdrs.BuildingNo,
                                            LANGUAGE = changedAdrs.Language.ToSafeString(),
                                            POSTAL_CODE = changedAdrs.PostalCode,
                                            CREATED_BY = command.ActionBy,
                                            CREATED_DATE = DateTime.Now,
                                            SUB_DISTRICT = changedAdrs.SUBDISTRICT,
                                            ACCESS_MODE = changedAdrs.ACCESS_MODE,
                                            SITE_CODE = changedAdrs.SITE_CODE,
                                            PARTNER = changedAdrs.PARTNER,
                                            LATITUDE = changedAdrs.LATITUDE,
                                            LONGTITUDE = changedAdrs.LONGTITUDE,

                                            //R21.1
                                            FTTR_FLAG = changedAdrs.FTTR_FLAG.ToSafeString(),
                                            SPECIFIC_TEAM_1 = changedAdrs.SPECIFIC_TEAM_1.ToSafeString(),
                                            SPECIFIC_TEAM_2 = changedAdrs.SPECIFIC_TEAM_2.ToSafeString(),
                                            FTTR_TYPE = M_fttrType.ToSafeString()
                                        };

                                        _listBV.Create(newUpdateBv);

                                        #region Check and insert
                                        var coverageResult = new FBSSCoverageResult();
                                        coverageResult = FBSSFeasCheck(changedAdrs.AddressType.ToSafeString(), changedAdrs.PostalCode, changedAdrs.SUBDISTRICT, changedAdrs.Language.ToSafeString(), changedAdrs.BuildingName
                                            , changedAdrs.BuildingNo, "N", "5", "", "", "");

                                        insertCoverageResult(changedAdrs.AddressType.ToSafeString(), changedAdrs.PostalCode, changedAdrs.SUBDISTRICT, changedAdrs.Language.ToSafeString(), changedAdrs.BuildingName
                                            , changedAdrs.BuildingNo, "N", "5", "5", 0, "", "", "", "", coverageResult.Coverage, coverageResult.AddressId, coverageResult.AccessModeList.DumpToXml()
                                            , coverageResult.PlanningSite.DumpToXml(), coverageResult.IsPartner, coverageResult.PartnerName, "", ziprow, "", "UPDATE Force Insert", "FBBCONFIG");

                                        numInsert++;
                                        #endregion

                                        //R21.1 FTTR => Insert Update Table AIR_PACKAGE_FTTR
                                        if (M_chkFTTR)
                                        {
                                            var chkDupAirPackageFTTR = checkDupAirPackageFTTR(changedAdrs.AddressId.ToSafeString());
                                            if (!chkDupAirPackageFTTR)
                                                insertAirPackageFTTR(changedAdrs.AddressId.ToSafeString(), "FTTR", "AWN", command.ActionBy);
                                            else
                                                updateAirPackageFTTR(changedAdrs.AddressId.ToSafeString(), command.ActionBy);
                                        }
                                    }
                                    else
                                    {
                                        command.errormsg += " Insert Data AddressId : " + changedAdrs.AddressId + " Error ZIPCODE_ROWID : The ZIPCODE_ROWID field is required.";
                                    }
                                    break;
                                }

                                // Update
                                #region R21.1 Check FTTR
                                var M2_chkFTTR = checkFTTR(changedAdrs.AddressType.ToSafeString(), changedAdrs.ACCESS_MODE.ToSafeString());
                                var M2_fttrType = getFTTR_TYPE(M2_chkFTTR, changedAdrs.FTTR_FLAG.ToSafeString());
                                #endregion
                                //updateBv.ACTIVE_FLAG = "Y";//changedAdrs.ChangedAction.ToSafeString();
                                updateBv.ADDRESS_ID = changedAdrs.AddressId;
                                updateBv.ADDRESS_TYPE = changedAdrs.AddressType.ToSafeString();
                                updateBv.BUILDING_NAME = changedAdrs.BuildingName;
                                updateBv.BUILDING_NO = changedAdrs.BuildingNo;
                                updateBv.LANGUAGE = changedAdrs.Language.ToSafeString();
                                updateBv.POSTAL_CODE = changedAdrs.PostalCode;
                                updateBv.UPDATED_BY = command.ActionBy;
                                updateBv.UPDATED_DATE = DateTime.Now;
                                updateBv.SUB_DISTRICT = changedAdrs.SUBDISTRICT;
                                updateBv.ACCESS_MODE = changedAdrs.ACCESS_MODE;
                                updateBv.SITE_CODE = changedAdrs.SITE_CODE;
                                updateBv.PARTNER = changedAdrs.PARTNER;
                                updateBv.LATITUDE = changedAdrs.LATITUDE;
                                updateBv.LONGTITUDE = changedAdrs.LONGTITUDE;
                                //R21.1
                                updateBv.FTTR_FLAG = changedAdrs.FTTR_FLAG.ToSafeString();
                                updateBv.SPECIFIC_TEAM_1 = changedAdrs.SPECIFIC_TEAM_1.ToSafeString();
                                updateBv.SPECIFIC_TEAM_2 = changedAdrs.SPECIFIC_TEAM_2.ToSafeString();
                                updateBv.FTTR_TYPE = M2_fttrType.ToSafeString();

                                if (string.IsNullOrEmpty(updateBv.CREATED_BY))
                                    updateBv.CREATED_BY = command.ActionBy;
                                if (updateBv.CREATED_DATE == null)
                                    updateBv.CREATED_DATE = DateTime.Now;

                                _listBV.Update(updateBv, _listBV.GetByKey(updateBv.LISTBV_ID));

                                #region Check and insert
                                var coverageResult2 = new FBSSCoverageResult();
                                coverageResult2 = FBSSFeasCheck(changedAdrs.AddressType.ToSafeString(), changedAdrs.PostalCode, changedAdrs.SUBDISTRICT, changedAdrs.Language.ToSafeString(), changedAdrs.BuildingName
                                    , changedAdrs.BuildingNo, "N", "5", "", "", "");

                                #region Get Ziprowid
                                var ziprow2 = (from r in _FBB_ZIPCODE.Get()
                                               where r.ZIPCODE == changedAdrs.PostalCode && r.TUMBON == changedAdrs.SUBDISTRICT && r.STATUS == "A"
                                               select r.ZIPCODE_ROWID
                                        ).ToList().FirstOrDefault();

                                #endregion

                                if (ziprow2 != null)
                                {
                                    insertCoverageResult(changedAdrs.AddressType.ToSafeString(), changedAdrs.PostalCode, changedAdrs.SUBDISTRICT, changedAdrs.Language.ToSafeString(), changedAdrs.BuildingName
                                        , changedAdrs.BuildingNo, "N", "5", "5", 0, "", "", "", "", coverageResult2.Coverage, coverageResult2.AddressId, coverageResult2.AccessModeList.DumpToXml()
                                        , coverageResult2.PlanningSite.DumpToXml(), coverageResult2.IsPartner, coverageResult2.PartnerName, "", ziprow2, "", "Normal UPDATE", "FBBCONFIG");

                                    numUpdate++;
                                }
                                else
                                {
                                    command.errormsg += " Update Data AddressId : " + changedAdrs.AddressId + " Error ZIPCODE_ROWID : The ZIPCODE_ROWID field is required.";
                                }
                                #endregion

                                //R21.1 FTTR => Insert Update Table AIR_PACKAGE_FTTR
                                if (M2_chkFTTR)
                                {
                                    var chkDupAirPackageFTTR = checkDupAirPackageFTTR(changedAdrs.AddressId.ToSafeString());
                                    if (!chkDupAirPackageFTTR)
                                        insertAirPackageFTTR(changedAdrs.AddressId.ToSafeString(), "FTTR", "AWN", command.ActionBy);
                                    else
                                        updateAirPackageFTTR(changedAdrs.AddressId.ToSafeString(), command.ActionBy);
                                }

                                break;

                            // deleted
                            case "D":
                                var deleteBv = (from t in _listBV.Get() where t.ADDRESS_ID == changedAdrs.AddressId && t.LANGUAGE == changedAdrs.Language select t).FirstOrDefault();

                                if (null == deleteBv)
                                    break;

                                //R21.1 Check FTTR
                                var D_chkFTTR = checkFTTR(changedAdrs.AddressType.ToSafeString(), changedAdrs.ACCESS_MODE.ToSafeString());
                                var D_fttrType = getFTTR_TYPE(D_chkFTTR, changedAdrs.FTTR_FLAG.ToSafeString());

                                // Update 
                                deleteBv.ACTIVE_FLAG = "N";//changedAdrs.ChangedAction.ToSafeString();
                                deleteBv.UPDATED_BY = command.ActionBy;
                                deleteBv.UPDATED_DATE = DateTime.Now;
                                //R21.1
                                deleteBv.FTTR_FLAG = changedAdrs.FTTR_FLAG.ToSafeString();
                                deleteBv.SPECIFIC_TEAM_1 = changedAdrs.SPECIFIC_TEAM_1.ToSafeString();
                                deleteBv.SPECIFIC_TEAM_2 = changedAdrs.SPECIFIC_TEAM_2.ToSafeString();
                                deleteBv.FTTR_TYPE = D_fttrType.ToSafeString();

                                if (string.IsNullOrEmpty(deleteBv.CREATED_BY))
                                    deleteBv.CREATED_BY = command.ActionBy;
                                if (deleteBv.CREATED_DATE == null)
                                    deleteBv.CREATED_DATE = DateTime.Now;

                                _listBV.Update(deleteBv, _listBV.GetByKey(deleteBv.LISTBV_ID));

                                #region Check and insert
                                var coverageResult3 = new FBSSCoverageResult();
                                coverageResult3 = FBSSFeasCheck(changedAdrs.AddressType.ToSafeString(), changedAdrs.PostalCode, changedAdrs.SUBDISTRICT, changedAdrs.Language.ToSafeString(), changedAdrs.BuildingName
                                    , changedAdrs.BuildingNo, "N", "5", "", "", "");

                                #region Get Ziprowid
                                var ziprow3 = (from r in _FBB_ZIPCODE.Get()
                                               where r.ZIPCODE == changedAdrs.PostalCode && r.TUMBON == changedAdrs.SUBDISTRICT && r.STATUS == "A"
                                               select r.ZIPCODE_ROWID
                                        ).ToList().FirstOrDefault();

                                #endregion
                                if (ziprow3 != null)
                                {
                                    insertCoverageResult(changedAdrs.AddressType.ToSafeString(), changedAdrs.PostalCode, changedAdrs.SUBDISTRICT, changedAdrs.Language.ToSafeString(), changedAdrs.BuildingName
                                        , changedAdrs.BuildingNo, "N", "5", "5", 0, "", "", "", "", coverageResult3.Coverage, coverageResult3.AddressId, coverageResult3.AccessModeList.DumpToXml()
                                        , coverageResult3.PlanningSite.DumpToXml(), coverageResult3.IsPartner, coverageResult3.PartnerName, "", ziprow3, "", "DELETE", "FBBCONFIG");

                                    numDelete++;
                                }
                                else
                                {
                                    command.errormsg += " Update Data AddressId : " + changedAdrs.AddressId + " Error ZIPCODE_ROWID : The ZIPCODE_ROWID field is required.";
                                }
                                #endregion
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    command.errormsg += " Update Data AddressId : " + changedAdrs.AddressId + " Error " + ex.Message;
                    IsError = true;
                }
            }
            command.numRecInsert = numInsert.ToSafeString();
            command.numRecUpdate = numUpdate.ToSafeString();
            command.numRecDelete = numDelete.ToSafeString();
            if (!IsError)
            {
                _uow.Persist();
            }
        }

        private bool IsAddressNeedUpdate(FBSSChangedAddressInfo changedAdrs)
        {
            // check if data need update or not
            var exist = (from t in _listBV.Get() where t.ADDRESS_ID == changedAdrs.AddressId select t).Any();

            if (exist)
            {
                if (changedAdrs.ChangedAction == "N")
                    return false;
            }
            else
            {
                if (changedAdrs.ChangedAction == "D")
                    //|| changedAdrs.ChangedAction == "M")
                    return false;
            }

            return true;
        }

        private FBSSCoverageResult FBSSFeasCheck(string AddressType,
            string PostalCode, string SubDistricName, string Language, string BuildingName,
            string BuildingNo, string PhoneFlag, string FloorNo, string Latitude,
            string Longitude, string UnitNo)
        {
            int countException = 0;
            var addressId = "";
            addressModeInfo[] accessmodeList = null;
            planSite planingSite = null;
            var isPartner = "";
            var partneName = "";
            var resultCode = "";
            var resultDesc = "";

            var coverageResult = new FBSSCoverageResult();

            //R21.2 Endpoint FBSSOrderServices.OrderService
            var endpointFbssOrderServices = (from l in _lov.Get() where l.LOV_NAME == "FBSS_ORDER_SERVICES" select l.LOV_VAL1).ToList().FirstOrDefault();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            repeat:
            try
            {
                #region R24.10 Call Access Token FBSS
                string accessToken = string.Empty;
                string channel = FBSSAccessToken.channelFBB.ToUpper();
                accessToken = (string)cache.Get(channel); //Get cache

                if (string.IsNullOrEmpty(accessToken))
                {
                    string clientId = string.Empty;
                    string clientSecret = string.Empty;
                    string grantType = string.Empty;
                    var loveConfigList = _lov.Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBSS_Authen")).ToList();
                    if (loveConfigList != null && loveConfigList.Count() > 0)
                    {
                        clientId = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL1 : "";
                        clientSecret = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL2 : "";
                        grantType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel) != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == channel).LOV_VAL4 : "";
                    }

                    var getToken = new GetTokenFbbQuery()
                    {
                        Channel = channel,
                        ParamGetoken = new ParametersGetoken()
                        {
                            client_id = clientId,
                            client_secret = clientSecret,
                            grant_type = grantType
                        }
                    };

                    var responseGetToken = _queryProcessor.Handle(getToken);
                    accessToken = (string)cache.Get(channel);
                }

                //log access token
                var logToken = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, accessToken, "FBB", "AlterChangedBuildingCommandToken", "AlterChangedBuildingCommandHandlerToken", "FBB", "FBB", "FBB");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", logToken, "Failed", "Access Token is Null", "");
                }
                #endregion
                using (var service = new FBSSAccessToken.CustomOrderService(accessToken))
                {
                    if (!string.IsNullOrEmpty(endpointFbssOrderServices)) service.Url = endpointFbssOrderServices;

                    var result = service.feasibilityCheck(
                        AddressType.ToSafeString().ParseEnum<addressType>(),
                        true,
                        PostalCode.ToSafeString(),
                        SubDistricName.ToSafeString(),
                        Language.ToSafeString().ParseEnum<language>(),
                        true,
                        BuildingName.ToSafeString().ToUpperInvariant(),
                        BuildingNo.ToSafeString().ToUpperInvariant(),
                        PhoneFlag.ToSafeString().ParseEnum<yn>(),
                        true,
                        FloorNo.ToSafeString(),
                        Latitude.ToSafeDecimal(), // changed
                        Latitude.ToSafeDecimal() > 0,
                        Longitude.ToSafeDecimal(), // changed
                        Longitude.ToSafeDecimal() > 0,
                        UnitNo.ToSafeString(),
                        out addressId,
                        out accessmodeList,
                        out planingSite,
                        out isPartner,
                        out partneName,
                        out resultCode,
                        out resultDesc);

                    coverageResult = new FBSSCoverageResult
                    {
                        AccessModeList = accessmodeList
                        .Select(t => new FBSSAccessModeInfo
                        {
                            AccessMode = (t == null ? "" : t.ACCESS_MODE),
                            InserviceDate = DateTime.Now,
                        }).ToList(),

                        AddressId = addressId,
                        Coverage = (string.IsNullOrEmpty(result) ? "NO" : result),
                        IsPartner = isPartner,
                        PartnerName = partneName,
                        PlanningSite = new FBSSAccessModeInfo
                        {
                            AccessMode = (planingSite == null ? "" : planingSite.ACCESS_MODE),
                            InserviceDate = (planingSite == null ? new DateTime() : planingSite.INSERVICE_DATE.ToFBSSDate()),
                        },
                    };

                    coverageResult.InterfaceLogId = 0;

                    return coverageResult;
                }
            }
            catch (WebException webEx)
            {
                //R24.10 Call Access Token FBSS Exception 
                if ((webEx.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)) && (countException == 0))
                {
                    countException++;
                    cache.Remove(FBSSAccessToken.channelFBB.ToUpper());
                    webEx = null;
                    goto repeat;
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void insertCoverageResult(string ADDRRESS_TYPE, string POSTAL_CODE, string SUB_DISTRICT_NAME,
            string LANGUAGE, string BUILDING_NAME, string BUILDING_NO, string PHONE_FLAG, string FLOOR_NO,
            string ADDRESS_NO, decimal MOO, string ROAD, string SOI, string LATITUDE, string LONGITUDE,
            string COVERAGE, string ADDRESS_ID, string ACCESS_MODE_LIST, string PLANNING_SITE_LIST, string IS_PARTNER,
            string PARTNER_NAME, string PRODUCTTYPE, string ZIPCODE_ROWID, string OWNER_PRODUCT,
            string TRANSACTION_ID, string ActionBy)
        {
            #region Insert FBB_COVERAGEAREA_RESULT

            var data = new FBB_FBSS_COVERAGEAREA_RESULT();
            data.ADDRRESS_TYPE = ADDRRESS_TYPE;
            data.POSTAL_CODE = POSTAL_CODE;
            data.SUB_DISTRICT_NAME = SUB_DISTRICT_NAME;
            data.LANGUAGE = LANGUAGE;
            data.BUILDING_NAME = BUILDING_NAME;
            data.BUILDING_NO = BUILDING_NO;
            data.PHONE_FLAG = PHONE_FLAG;
            data.FLOOR_NO = FLOOR_NO;
            data.ADDRESS_NO = ADDRESS_NO;
            data.MOO = MOO;
            data.SOI = SOI;
            data.ROAD = ROAD;
            data.LATITUDE = LATITUDE;
            data.LONGITUDE = LONGITUDE;
            data.COVERAGE = COVERAGE;
            data.ADDRESS_ID = ADDRESS_ID;
            data.ACCESS_MODE_LIST = ACCESS_MODE_LIST;
            data.PLANNING_SITE_LIST = PLANNING_SITE_LIST;
            data.IS_PARTNER = IS_PARTNER;
            data.PARTNER_NAME = PARTNER_NAME;
            data.PRODUCTTYPE = PRODUCTTYPE;
            data.ZIPCODE_ROWID = ZIPCODE_ROWID;
            data.OWNER_PRODUCT = OWNER_PRODUCT;

            data.TRANSACTION_ID = TRANSACTION_ID;
            data.CREATED_BY = string.IsNullOrEmpty(ActionBy) ? "CUSTOMER" : ActionBy;
            data.CREATED_DATE = DateTime.Now;

            _coverageResult.Create(data);
            _uow.Persist();

            #endregion Insert FBB_COVERAGEAREA_RESULT
        }

        private bool checkDupAirPackageFTTR(string ADDRESS_ID)
        {
            var dupAddressId = (from t in _air_package_fttr.Get() where t.ADDRESS_ID == ADDRESS_ID select t).FirstOrDefault();

            if (dupAddressId != null) return true; // Case Dup
            else return false;// Case New
        }

        private void insertAirPackageFTTR(string ADDRESS_ID, string PRODUCT_SUBTYPE, string OWNER_PRODUCT, string UPD_BY)
        {
            var lov_expire_dtm_fttr = (from t in _lov.Get() where t.LOV_TYPE == "FBB_CONFIG" && t.LOV_NAME == "EXPIRE_DTM_FTTR" select t).FirstOrDefault();

            DateTime upd_dtm = DateTime.Now;

            DateTime effective_dtm = new DateTime();
            DateTime.TryParseExact(upd_dtm.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out effective_dtm);

            DateTime expire_dtm_fttr = new DateTime();
            DateTime.TryParseExact(lov_expire_dtm_fttr.LOV_VAL1, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out expire_dtm_fttr);

            // Case New
            var data = new AIR_PACKAGE_FTTR();
            data.ADDRESS_ID = ADDRESS_ID.ToSafeString();
            data.SFF_PROMOTION_CODE = "";
            data.SFF_PROMOTION_BILL_THA = "";
            data.SFF_PROMOTION_BILL_ENG = "";
            data.PRODUCT_SUBTYPE = PRODUCT_SUBTYPE.ToSafeString();
            data.OWNER_PRODUCT = OWNER_PRODUCT.ToSafeString();
            data.CUSTOMER_TYPE = "";
            data.EFFECTIVE_DTM = effective_dtm;
            data.EXPIRE_DTM = expire_dtm_fttr;
            data.UPD_DTM = upd_dtm;
            data.UPD_BY = UPD_BY.ToSafeString();

            _air_package_fttr.Create(data);
            _uowAir.Persist();
        }

        private void updateAirPackageFTTR(string ADDRESS_ID, string UPD_BY)
        {
            var lov_expire_dtm_fttr = (from t in _lov.Get() where t.LOV_TYPE == "FBB_CONFIG" && t.LOV_NAME == "EXPIRE_DTM_FTTR" select t).FirstOrDefault();

            DateTime upd_dtm = DateTime.Now;

            DateTime effective_dtm = new DateTime();
            DateTime.TryParseExact(upd_dtm.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out effective_dtm);

            DateTime expire_dtm_fttr = new DateTime();
            DateTime.TryParseExact(lov_expire_dtm_fttr.LOV_VAL1, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out expire_dtm_fttr);

            var data = (from t in _air_package_fttr.Get() where t.ADDRESS_ID == ADDRESS_ID select t).FirstOrDefault();

            string oldEXPIRE_DTM = data.EXPIRE_DTM.ToDateDisplayText();
            string newEXPIRE_DTM = expire_dtm_fttr.Date.ToString("dd/MM/yyyy");

            string oldUPD_DTM = data.UPD_DTM.ToDateDisplayText();
            string newUPD_DTM = upd_dtm.Date.ToString("dd/MM/yyyy");

            if (!(oldEXPIRE_DTM == newEXPIRE_DTM && oldUPD_DTM == newUPD_DTM && data.UPD_BY == UPD_BY.ToSafeString()))
            {
                data.EXPIRE_DTM = expire_dtm_fttr;
                data.UPD_DTM = upd_dtm;
                data.UPD_BY = UPD_BY.ToSafeString();

                _air_package_fttr.Update(data);
                _uowAir.Persist();
            }
        }

        private bool checkFTTR(string address_type, string access_mode)
        {
            var lov_address_type_fttr = (from r in _lov.Get() where r.LOV_TYPE == "FBB_CONFIG" && r.LOV_NAME == "ADDRESS_TYPE_FTTR" select r).ToList().FirstOrDefault();
            var ADDRESS_TYPE_FTTR = lov_address_type_fttr.LOV_VAL1.ToSafeString(); //B
            var ACCESS_MODE_FTTR = lov_address_type_fttr.LOV_VAL2.ToSafeString(); // FTTH

            if (address_type == ADDRESS_TYPE_FTTR && access_mode == ACCESS_MODE_FTTR) return true;
            else return false;
        }

        private string getFTTR_TYPE(bool chkFTTR, string FTTR_FLAG)
        {
            var fttr_type = (from r in _lov.Get()
                             where r.LOV_TYPE == "FTTR_TYPE" && r.LOV_NAME == FTTR_FLAG
                             select r.LOV_VAL1
                                    ).ToList().FirstOrDefault();
            if (chkFTTR && fttr_type != null) return fttr_type;
            else return "";
        }

    }
}
