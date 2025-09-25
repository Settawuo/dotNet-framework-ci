using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
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
    public class GetSeibelInfoQueryHandler : IQueryHandler<GetSeibelInfoQuery, SeibelResultModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;

        public GetSeibelInfoQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
        }

        public SeibelResultModel Handle(GetSeibelInfoQuery query)
        {
            string outASCPatnerName, outBusinessType, outCharacteristic, outErrorMessage, outFullAddress, outLocationCode,
                   outMainFax, outMainPhone, outMobileNo, outOperatorClass, outParam1, outParam10, outParam11, outParam12,
                   outParam13, outParam14, outParam15, outParam16, outParam17, outParam18, outParam19, outParam2, outParam20,
                   outParam21, outParam22, outParam23, outParam24, outParam25, outParam3, outParam4, outParam5, outParam6,
                   outParam7, outParam8, outParam9, outPartnerName, outProvince, outRegion, outStatus, outSubRegion, outSubType,
                   outTitle, outType, outWTName;

            var resultnop = "";
            var modelResult = new SeibelResultModel();
            InterfaceLogCommand log = null;

            try
            {

                string CALL_CCSM_FLAG = "";
                string URL_CCSM = "";

                CALL_CCSM_FLAG = (from z in _lov.Get()
                                  where z.LOV_NAME == "CALL_CCSM_FLAG" && z.ACTIVEFLAG == "Y"
                                  select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                URL_CCSM = (from z in _lov.Get()
                            where z.LOV_NAME == "URL_CCSM" && z.ACTIVEFLAG == "Y"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                if (CALL_CCSM_FLAG == "Y")
                {
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, "locationCode:" + query.LocationCode + ", ASCCode:" + query.ASCCode, query.Transaction_Id, "GetSeibelInfoQuery", "Call_CCSM", "", "FBB|" + query.FullURL, "");

                    modelResult.outASCPartnerName = "";
                    modelResult.outBusinessType = "";
                    modelResult.outCharacteristic = "";
                    modelResult.outErrorMessage = "";
                    modelResult.outFullAddress = "";
                    modelResult.outLocationCode = "";
                    modelResult.outMainFax = "";
                    modelResult.outMainPhone = "";
                    modelResult.outMobileNo = "";
                    modelResult.outOperatorClass = "";
                    modelResult.outPartnerName = "";
                    modelResult.outProvince = "";
                    modelResult.outRegion = "";
                    modelResult.outStatus = "";
                    modelResult.outSubRegion = "";
                    modelResult.outSubType = "";
                    modelResult.outTitle = "";
                    modelResult.outType = "";
                    modelResult.outWTName = "";

                    //20.3
                    modelResult.outDistChn = "";
                    modelResult.outChnSales = "";
                    modelResult.outShopType = "";
                    modelResult.outASCTitleThai = "";
                    modelResult.outPosition = "";
                    modelResult.outMemberCategory = "";
                    modelResult.outLocationName = "";

                    modelResult.outChnSalesCode = "";

                    string strPara = "";
                    if (query.LocationCode != null && query.LocationCode != "")
                    {
                        strPara = "?filter=(&(inLocationCode=" + query.LocationCode + ")(inEvent=evLocationInfo)(inSource=FBB))";
                    }
                    if (query.ASCCode != null && query.ASCCode != "")
                    {
                        strPara = "?filter=(&(inASCCode=" + query.ASCCode + ")(inEvent=evASCInfo)(inSource=FBB))";
                    }

                    string URL = URL_CCSM;
                    URL = URL + strPara;
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var resultData = client.GetAsync(URL).Result;
                    if (resultData.IsSuccessStatusCode)
                    {
                        string resultContent = resultData.Content.ReadAsStringAsync().Result;
                        resultnop = resultContent;
                        DataSet myDataSet = ReadDataFromJson(resultContent);
                        if (myDataSet != null && myDataSet.Tables != null && myDataSet.Tables.Count > 0)
                        {
                            DataTable dt = myDataSet.Tables[0];
                            DataColumnCollection columnsCheck = dt.Columns;
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DataRow dr = dt.Rows[0];
                                string dc = "";
                                if (columnsCheck.Contains("outStatus"))
                                    dc = dr["outStatus"].ToSafeString();
                                modelResult.outStatus = dc;

                                if (columnsCheck.Contains("resultDescription"))
                                    modelResult.outErrorMessage = dr["resultDescription"].ToSafeString();

                                if (dc == "0000")
                                {

                                    DataTable dtResultLocation = myDataSet.Tables["LocationList"];
                                    DataColumnCollection ResultLocationColumns = dtResultLocation.Columns;
                                    if (dtResultLocation != null && dtResultLocation.Rows != null && dtResultLocation.Rows.Count > 0)
                                    {
                                        DataRow drResult = dtResultLocation.Rows[0];

                                        if (ResultLocationColumns.Contains("outASCPartnerName"))
                                            modelResult.outASCPartnerName = drResult["outASCPartnerName"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outBusinessType"))
                                            modelResult.outBusinessType = drResult["outBusinessType"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outCharacteristic"))
                                            modelResult.outCharacteristic = drResult["outCharacteristic"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outFullAddress"))
                                            modelResult.outFullAddress = drResult["outFullAddress"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outLocationCode"))
                                            modelResult.outLocationCode = drResult["outLocationCode"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outLocationFax"))
                                            modelResult.outMainFax = drResult["outLocationFax"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outLocationPhoneNo"))
                                            modelResult.outMainPhone = drResult["outLocationPhoneNo"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outMobileNo"))
                                            modelResult.outMobileNo = drResult["outMobileNo"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outOperatorClass"))
                                            modelResult.outOperatorClass = drResult["outOperatorClass"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outPartnerName"))
                                            modelResult.outPartnerName = drResult["outPartnerName"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outProvince"))
                                            modelResult.outProvince = drResult["outProvince"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outSubType"))
                                            modelResult.outSubType = drResult["outSubType"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outTitle"))
                                            modelResult.outTitle = drResult["outTitle"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outType"))
                                            modelResult.outType = drResult["outType"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outWTName"))
                                            modelResult.outWTName = drResult["outWTName"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outLocationRegion"))
                                            modelResult.outRegion = drResult["outLocationRegion"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outLocationSubRegion"))
                                            modelResult.outSubRegion = drResult["outLocationSubRegion"].ToSafeString();
                                        //20.3 Channel Report
                                        if (ResultLocationColumns.Contains("outCompanyName"))
                                            modelResult.outCompanyName = drResult["outCompanyName"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outDistChn"))
                                            modelResult.outDistChn = drResult["outDistChn"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outChnSales"))
                                            modelResult.outChnSales = drResult["outChnSales"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outShopType"))
                                            modelResult.outShopType = drResult["outShopType"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outLocationName"))
                                            modelResult.outLocationName = drResult["outLocationName"].ToSafeString();
                                        if (ResultLocationColumns.Contains("outChnSalesCode"))
                                            modelResult.outChnSalesCode = drResult["outChnSalesCode"].ToSafeString();
                                    }

                                    DataTable dtResultAsc = myDataSet.Tables["ASCList"];
                                    if (dtResultAsc != null && dtResultAsc.Rows != null && dtResultAsc.Rows.Count > 0)
                                    {
                                        DataColumnCollection ResultAscColumns = dtResultAsc.Columns;
                                        DataRow drResult = dtResultAsc.Rows[0];

                                        if (ResultAscColumns.Contains("outASCPartnerName"))
                                            modelResult.outASCPartnerName = drResult["outASCPartnerName"].ToSafeString();
                                        if (ResultAscColumns.Contains("outMobileNo"))
                                            modelResult.outMobileNo = drResult["outMobileNo"].ToSafeString();
                                        if (ResultAscColumns.Contains("outMemberCategory"))
                                            modelResult.outMemberCategory = drResult["outMemberCategory"].ToSafeString();
                                        if (ResultAscColumns.Contains("outASCTitleThai"))
                                            modelResult.outASCTitleThai = drResult["outASCTitleThai"].ToSafeString();
                                        if (ResultAscColumns.Contains("outPosition"))
                                            modelResult.outPosition = drResult["outPosition"].ToSafeString();
                                    }

                                    //R21.5 Pool Villa
                                    DataTable dtResultAddrLocation = myDataSet.Tables["addressLocationList"];
                                    if (dtResultAddrLocation != null && dtResultAddrLocation.Rows != null && dtResultAddrLocation.Rows.Count > 0)
                                    {
                                        DataRow drResult;
                                        DataColumnCollection ResultAddrLocationColumns = dtResultAddrLocation.Columns;
                                        List<SeibelAddressLocation> SeibelAddressLocationList = new List<SeibelAddressLocation>();

                                        for (int i = 0; i < dtResultAddrLocation.Rows.Count; i++)
                                        {
                                            SeibelAddressLocation seibelAddressLocation = new SeibelAddressLocation();
                                            drResult = dtResultAddrLocation.Rows[i];

                                            if (ResultAddrLocationColumns.Contains("outAddressID"))
                                                seibelAddressLocation.outAddressID = drResult["outAddressID"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outAddressType"))
                                                seibelAddressLocation.outAddressType = drResult["outAddressType"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outHouseNo"))
                                                seibelAddressLocation.outHouseNo = drResult["outHouseNo"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outMoo"))
                                                seibelAddressLocation.outMoo = drResult["outMoo"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outMooban"))
                                                seibelAddressLocation.outMooban = drResult["outMooban"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outBuilding"))
                                                seibelAddressLocation.outBuilding = drResult["outBuilding"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outFloor"))
                                                seibelAddressLocation.outFloor = drResult["outFloor"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outRoom"))
                                                seibelAddressLocation.outRoom = drResult["outRoom"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outSoi"))
                                                seibelAddressLocation.outSoi = drResult["outSoi"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outStreet"))
                                                seibelAddressLocation.outStreet = drResult["outStreet"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outProvince"))
                                                seibelAddressLocation.outProvince = drResult["outProvince"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outAmphur"))
                                                seibelAddressLocation.outAmphur = drResult["outAmphur"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outTumbol"))
                                                seibelAddressLocation.outTumbol = drResult["outTumbol"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outZipcode"))
                                                seibelAddressLocation.outZipcode = drResult["outZipcode"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outCountry"))
                                                seibelAddressLocation.outCountry = drResult["outCountry"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outFullAddress"))
                                                seibelAddressLocation.outFullAddress = drResult["outFullAddress"].ToSafeString();
                                            if (ResultAddrLocationColumns.Contains("outAddressLastUpd"))
                                                seibelAddressLocation.outAddressLastUpd = drResult["outAddressLastUpd"].ToSafeString();

                                            SeibelAddressLocationList.Add(new SeibelAddressLocation()
                                            {
                                                outAddressID = seibelAddressLocation.outAddressID,
                                                outAddressType = seibelAddressLocation.outAddressType,
                                                outHouseNo = seibelAddressLocation.outHouseNo,
                                                outMoo = seibelAddressLocation.outMoo,
                                                outMooban = seibelAddressLocation.outMooban,
                                                outBuilding = seibelAddressLocation.outBuilding,
                                                outFloor = seibelAddressLocation.outFloor,
                                                outRoom = seibelAddressLocation.outRoom,
                                                outSoi = seibelAddressLocation.outSoi,
                                                outStreet = seibelAddressLocation.outStreet,
                                                outProvince = seibelAddressLocation.outProvince,
                                                outAmphur = seibelAddressLocation.outAmphur,
                                                outTumbol = seibelAddressLocation.outTumbol,
                                                outZipcode = seibelAddressLocation.outZipcode,
                                                outCountry = seibelAddressLocation.outCountry,
                                                outFullAddress = seibelAddressLocation.outFullAddress,
                                                outAddressLastUpd = seibelAddressLocation.outAddressLastUpd
                                            });
                                        }
                                        modelResult.addressLocationList = SeibelAddressLocationList;
                                    }

                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", modelResult.outErrorMessage, "");
                                    return modelResult;
                                }
                                else
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", modelResult.outErrorMessage, "");
                                    return modelResult;
                                }

                            }
                            else
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", modelResult.outErrorMessage, "");
                                return modelResult;
                            }
                        }
                        else
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", modelResult.outErrorMessage, "");
                            return modelResult;
                        }
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", "Connect Service Fail", "");
                        return modelResult;
                    }
                }
                else
                {
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, "locationCode:" + query.LocationCode + ", ASCCode:" + query.ASCCode, query.Transaction_Id, "GetSeibelInfoQuery", "SiebelServices.AIS_spcDRM_spcChannel_spcInfo", "", "FBB|" + query.FullURL, "");

                    using (var service = new SiebelService.AIS_spcDRM_spcChannel_spcInfo())
                    {
                        string KeyData = "";
                        string UrlData = "";

                        KeyData = (from z in _lov.Get()
                                   where z.LOV_NAME == "sobs" && z.ACTIVEFLAG == "Y"
                                   select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                        UrlData = (from z in _lov.Get()
                                   where z.LOV_NAME == "SiebelService" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Endpoint"
                                   select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                        service.Url = EncryptionUtility.Decrypt(UrlData, KeyData);

                        var result = service.AISDRMChannelInfo("", "", query.Inparam1, "", query.LocationCode, query.ASCCode, ""
                        , out outASCPatnerName, out outBusinessType, out outCharacteristic, out outErrorMessage, out outFullAddress
                        , out outLocationCode, out outMainFax, out outMainPhone, out outMobileNo, out outOperatorClass, out outParam1
                        , out outParam10, out outParam11, out outParam12, out outParam13, out outParam14, out outParam15, out outParam16
                        , out outParam17, out outParam18, out outParam19, out outParam2, out outParam20, out outParam21, out outParam22, out outParam23
                        , out outParam24, out outParam25, out outParam3, out outParam4, out outParam5, out outParam6, out outParam7, out outParam8
                        , out outParam9, out outPartnerName, out outProvince, out outRegion, out outStatus, out outSubRegion, out outSubType, out outTitle, out outType
                        , out outWTName
                        );

                        resultnop = result;

                        #region resultmodel

                        modelResult.outASCPartnerName = outASCPatnerName;
                        modelResult.outBusinessType = outBusinessType;
                        modelResult.outCharacteristic = outCharacteristic;
                        modelResult.outErrorMessage = outErrorMessage;
                        modelResult.outFullAddress = outFullAddress;
                        modelResult.outLocationCode = outLocationCode;
                        modelResult.outMainFax = outMainFax;
                        modelResult.outMainPhone = outMainPhone;
                        modelResult.outMobileNo = outMobileNo;
                        modelResult.outOperatorClass = outOperatorClass;
                        modelResult.outPartnerName = outPartnerName;
                        modelResult.outProvince = outProvince;
                        modelResult.outRegion = outRegion;
                        modelResult.outStatus = outStatus;
                        modelResult.outSubRegion = outSubRegion;
                        modelResult.outSubType = outSubType;
                        modelResult.outTitle = outTitle;
                        modelResult.outType = outType;
                        modelResult.outWTName = outWTName;
                        //20.3 Channel Report
                        modelResult.outDistChn = "";
                        modelResult.outChnSales = "";
                        modelResult.outShopType = "";
                        modelResult.outASCTitleThai = "";
                        modelResult.outPosition = "";
                        #endregion

                        //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, modelResult, log,
                        //                    "Success", outErrorMessage);
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, modelResult, log, "Success", outErrorMessage, "");

                        if (outStatus != null || outStatus != "")
                        {
                            if (outStatus == "Error")
                            {
                                return modelResult;
                            }
                            else
                            {
                                return modelResult;
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultnop, log, "Failed", ex.GetErrorMessage(), "");
                modelResult.outStatus = "Error";
                return modelResult;
            }

            modelResult.outStatus = "Error";
            return modelResult;

        }

        private DataSet ReadDataFromJson(string jsonString, XmlReadMode mode = XmlReadMode.Auto)
        {
            //// Note:Json convertor needs a json with one node as root
            jsonString = "{ \"rootNode\": {" + jsonString.Trim().TrimStart('{').TrimEnd('}') + @"} }";
            //// Now it is secure that we have always a Json with one node as root 
            var xd = JsonConvert.DeserializeXmlNode(jsonString);

            //// DataSet is able to read from XML and return a proper DataSet
            var result = new DataSet();
            result.ReadXml(new XmlNodeReader(xd), mode);
            return result;
        }

    }

}
