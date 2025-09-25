using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
    public class RelatedMobileServiceATNHandler : IQueryHandler<RelatedMobileServiceATNQuery, RelatedMobileServiceATNModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public RelatedMobileServiceATNHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
        }

        public RelatedMobileServiceATNModel Handle(RelatedMobileServiceATNQuery query)
        {
            InterfaceLogCommand log = null;
            RelatedMobileServiceATNModel result = new RelatedMobileServiceATNModel();
            List<resultDataATN> resultListDataATN = new List<resultDataATN>();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.username, "RelatedMobileServiceATN", "RelatedMobileServiceATNHandler", "", "FBB", "");

                //Config Text By Lov
                var configBillCycle = _lov.Get(l => l.LOV_TYPE == ("CONFIG") && l.LOV_NAME == ("L_BILLING_ADDR_BILL_CYCLE") && l.ACTIVEFLAG == ("Y")).ToList();
                var configBillMedia = _lov.Get(l => l.LOV_TYPE == ("SCREEN") && l.LOV_NAME == ("L_BILLING_ADDR_BILL_MEDIA") && l.ACTIVEFLAG == ("Y")).ToList();

                //Config Condition
                var configCondition = _lov.Get(l => l.LOV_TYPE == ("CONFIG") && l.LOV_NAME == ("CONDITION_FILTER_OUT") && l.ACTIVEFLAG == ("Y")).ToList();

                //Config Service URL
                var configAtn = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("Athena_Related_URL") && l.ACTIVEFLAG == ("Y")).FirstOrDefault();
                if (configAtn != null && configAtn.LOV_VAL1.ToSafeString() != "" && configAtn.LOV_VAL2.ToSafeString() == "Y")
                {
                    //R23.08 Check RELATED or NON RELATED and Get URL
                    string tmpUrl = configAtn.LOV_VAL1.ToSafeString();

                    if (query.mode == "RELATED" && !string.IsNullOrEmpty(query.mobileNo))
                    {
                        tmpUrl = tmpUrl + "mobileNo=" + query.mobileNo;
                    }
                    else
                    {
                        tmpUrl = tmpUrl + "idCardNo=" + query.idCardNo;
                    }

                    var client = new RestClient(tmpUrl);
                    var request = new RestRequest();
                    request.Method = Method.GET;
                    request.AddHeader("channel", !string.IsNullOrEmpty(query.channel) ? query.channel.ToSafeString() : "FBB");
                    request.AddHeader("username", query.username.ToSafeString());

                    //trust certificate
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                    //execute
                    var response = client.Execute(request);
                    var content = response.Content;

                    if (response.StatusCode == HttpStatusCode.OK) //200
                    {
                        ServiceATNIntraModel resultATNIntra = null;
                        resultATNIntra = JsonConvert.DeserializeObject<ServiceATNIntraModel>(response.Content) ?? new ServiceATNIntraModel();

                        if (resultATNIntra != null && resultATNIntra.resultCode == "20000")
                        {
                            if (resultATNIntra.resultData != null)
                            {
                                if (query.mode == "RELATED")
                                {
                                    #region Call Service ATN mobileNo
                                    foreach (var item in resultATNIntra.resultData.listDefaultBa)
                                    {
                                        #region Filter Condition
                                        if (configCondition.Where(c => c.LOV_VAL1.Contains("negoFlag")).FirstOrDefault().LOV_VAL2.ToSafeString() == item.negoFlag) continue;

                                        bool PkgFilter = false;
                                        foreach (var p in item.productPackage)
                                        {
                                            foreach (var pGroup in configCondition.Where(c => c.LOV_VAL1.Contains("productGroup")))
                                            {
                                                if (pGroup.LOV_VAL2.ToSafeString() == p.productGroup)
                                                {
                                                    foreach (var pPkg in configCondition.Where(c => c.LOV_VAL1.Contains("productPkg")))
                                                    {
                                                        if (pPkg.LOV_VAL2.ToSafeString() == p.productPkg)
                                                        {
                                                            PkgFilter = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (PkgFilter) continue;
                                        #endregion Filter Condition

                                        resultDataATN dataATN = new resultDataATN();

                                        dataATN.BaNo = item.baNo.ToSafeString();
                                        dataATN.MobileNo = item.mobile.FirstOrDefault().mobileNo.ToSafeString();
                                        dataATN.MobileNoDisplay = ConvertMobileDisplay(item.mobile.FirstOrDefault().mobileNo.ToSafeString());
                                        dataATN.MobileNoGroup = item.mobile.FirstOrDefault().mobileNo.ToSafeString();
                                        dataATN.MobileNoGroupDisplay = ConvertMobileDisplay(item.mobile.FirstOrDefault().mobileNo.ToSafeString());

                                        dataATN.HomeId = item.billAddress.FirstOrDefault().houseNo.ToSafeString();
                                        dataATN.Moo = item.billAddress.FirstOrDefault().moo.ToSafeString();
                                        dataATN.Mooban = item.billAddress.FirstOrDefault().mooban.ToSafeString();
                                        dataATN.Building = item.billAddress.FirstOrDefault().building.ToSafeString();
                                        dataATN.Floor = item.billAddress.FirstOrDefault().floor.ToSafeString();
                                        dataATN.Room = item.billAddress.FirstOrDefault().room.ToSafeString();
                                        dataATN.Soi = item.billAddress.FirstOrDefault().soi.ToSafeString();
                                        dataATN.Street = item.billAddress.FirstOrDefault().street.ToSafeString();
                                        dataATN.Tumbon = item.billAddress.FirstOrDefault().tumbol.ToSafeString();
                                        dataATN.Ampur = item.billAddress.FirstOrDefault().amphur.ToSafeString();
                                        dataATN.Province = item.billAddress.FirstOrDefault().province.ToSafeString();
                                        dataATN.ZipCode = item.billAddress.FirstOrDefault().zipCode.ToSafeString();

                                        dataATN.BillCycle = item.billCycle;
                                        dataATN.BillMedia = item.billMedia;
                                        dataATN.BillCycleInfo = !string.IsNullOrEmpty(item.billDisplay) && query.languageCode == "1" ?
                                            configBillCycle.Where(l => l.LOV_VAL3.Contains(item.billDisplay)).FirstOrDefault()?.LOV_VAL1.ToSafeString() ?? item.billDisplay.ToSafeString() :
                                            configBillCycle.Where(l => l.LOV_VAL3.Contains(item.billDisplay)).FirstOrDefault()?.LOV_VAL2.ToSafeString() ?? item.billDisplay.ToSafeString();
                                        dataATN.ChannelViewBill = !string.IsNullOrEmpty(item.billMedia) && query.languageCode == "1" ?
                                            configBillMedia.Where(l => l.LOV_VAL3.Contains(item.billMedia)).FirstOrDefault()?.LOV_VAL1.ToSafeString() ?? item.billMedia.ToSafeString() :
                                            configBillMedia.Where(l => l.LOV_VAL3.Contains(item.billMedia)).FirstOrDefault()?.LOV_VAL2.ToSafeString() ?? item.billMedia.ToSafeString();

                                        resultListDataATN.Add(dataATN);
                                    }
                                    #endregion Call Service ATN mobileNo
                                }
                                else
                                {
                                    #region Call Service ATN NO RELATE By idCardNo
                                    foreach (var item in resultATNIntra.resultData.listDefaultBa)
                                    {
                                        #region Filter Condition
                                        if (configCondition.Where(c => c.LOV_VAL1.Contains("negoFlag")).FirstOrDefault().LOV_VAL2.ToSafeString() == item.negoFlag) continue;

                                        bool PkgFilter = false;
                                        foreach (var p in item.productPackage)
                                        {
                                            foreach (var pGroup in configCondition.Where(c => c.LOV_VAL1.Contains("productGroup")))
                                            {
                                                if (pGroup.LOV_VAL2.ToSafeString() == p.productGroup)
                                                {
                                                    foreach (var pPkg in configCondition.Where(c => c.LOV_VAL1.Contains("productPkg")))
                                                    {
                                                        if (pPkg.LOV_VAL2.ToSafeString() == p.productPkg)
                                                        {
                                                            PkgFilter = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (PkgFilter) continue;
                                        #endregion Filter Condition

                                        resultDataATN dataATN = new resultDataATN();

                                        foreach (var m in item.mobile)
                                        {
                                            dataATN.MobileNo = string.IsNullOrEmpty(dataATN.MobileNo) ? m.mobileNo : dataATN.MobileNo;
                                            dataATN.MobileNoDisplay = string.IsNullOrEmpty(dataATN.MobileNoDisplay) ? ConvertMobileDisplay(m.mobileNo) : dataATN.MobileNoDisplay;
                                            dataATN.MobileNoGroup = string.IsNullOrEmpty(dataATN.MobileNoGroup) ? m.mobileNo : dataATN.MobileNoGroup + ", " + m.mobileNo;
                                            dataATN.MobileNoGroupDisplay = string.IsNullOrEmpty(dataATN.MobileNoGroupDisplay) ? ConvertMobileDisplay(m.mobileNo) : dataATN.MobileNoGroupDisplay + ", " + ConvertMobileDisplay(m.mobileNo);
                                        }

                                        dataATN.BaNo = item.baNo.ToSafeString();
                                        dataATN.HomeId = item.billAddress.FirstOrDefault().houseNo.ToSafeString();
                                        dataATN.Moo = item.billAddress.FirstOrDefault().moo.ToSafeString();
                                        dataATN.Mooban = item.billAddress.FirstOrDefault().mooban.ToSafeString();
                                        dataATN.Building = item.billAddress.FirstOrDefault().building.ToSafeString();
                                        dataATN.Floor = item.billAddress.FirstOrDefault().floor.ToSafeString();
                                        dataATN.Room = item.billAddress.FirstOrDefault().room.ToSafeString();
                                        dataATN.Soi = item.billAddress.FirstOrDefault().soi.ToSafeString();
                                        dataATN.Street = item.billAddress.FirstOrDefault().street.ToSafeString();
                                        dataATN.Ampur = item.billAddress.FirstOrDefault().amphur.ToSafeString();
                                        dataATN.Tumbon = item.billAddress.FirstOrDefault().tumbol.ToSafeString();
                                        dataATN.ZipCode = item.billAddress.FirstOrDefault().zipCode.ToSafeString();
                                        dataATN.Province = item.billAddress.FirstOrDefault().province.ToSafeString();

                                        dataATN.BillCycle = item.billCycle;
                                        dataATN.BillMedia = item.billMedia;
                                        dataATN.BillCycleInfo = !string.IsNullOrEmpty(item.billDisplay) && query.languageCode == "1" ?
                                            configBillCycle.Where(l => l.LOV_VAL3.Contains(item.billDisplay)).FirstOrDefault()?.LOV_VAL1.ToSafeString() ?? item.billDisplay.ToSafeString() :
                                            configBillCycle.Where(l => l.LOV_VAL3.Contains(item.billDisplay)).FirstOrDefault()?.LOV_VAL2.ToSafeString() ?? item.billDisplay.ToSafeString();
                                        dataATN.ChannelViewBill = !string.IsNullOrEmpty(item.billMedia) && query.languageCode == "1" ?
                                            configBillMedia.Where(l => l.LOV_VAL3.Contains(item.billMedia)).FirstOrDefault()?.LOV_VAL1.ToSafeString() ?? item.billMedia.ToSafeString() :
                                            configBillMedia.Where(l => l.LOV_VAL3.Contains(item.billMedia)).FirstOrDefault()?.LOV_VAL2.ToSafeString() ?? item.billMedia.ToSafeString();

                                        resultListDataATN.Add(dataATN);
                                    }
                                    #endregion Call Service ATN NO RELATE By idCardNo
                                }

                                if (resultListDataATN != null && resultListDataATN.Count > 0)
                                {
                                    result.returnCode = "0";
                                    result.returnMessage = "Success";
                                    result.returnRelated = query.mode.ToSafeString();
                                    result.resultData = resultListDataATN;
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n" + "- Call By " + query.mode.ToSafeString() + " \r\n", log, "Success", "", "");
                                }
                                else
                                {
                                    result.returnCode = "-2";
                                    result.returnMessage = "resultData null";
                                    result.returnRelated = query.mode.ToSafeString();
                                    result.resultData = null;
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Success", "resultData null", "");
                                }
                            }
                            else
                            {
                                result.returnCode = "-2";
                                result.returnMessage = "resultData null";
                                result.returnRelated = query.mode.ToSafeString();
                                result.resultData = null;
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Success", "resultData null", "");
                            }
                        }
                        else
                        {
                            result.returnCode = "-1";
                            result.returnMessage = "Failed : " + resultATNIntra.resultCode + " " + resultATNIntra.resultDescription;
                            result.returnRelated = string.Empty;
                            result.resultData = null;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound) //404
                    {
                        ServiceATNIntraModel resultATNIntra = null;
                        resultATNIntra = JsonConvert.DeserializeObject<ServiceATNIntraModel>(response.Content) ?? new ServiceATNIntraModel();

                        #region R23.08 New Response no data from ATN
                        if (resultATNIntra.resultCode == "40401")
                        {
                            result.returnCode = "-2";
                            result.returnMessage = "resultData null";
                            result.returnRelated = query.mode.ToSafeString();
                            result.resultData = null;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Success", "resultData null", "");
                        }
                        else
                        {
                            result.returnCode = "-1";
                            result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(resultATNIntra.resultCode) ? resultATNIntra.resultCode.ToSafeString() + " " + resultATNIntra.resultDescription.ToSafeString() : "response is null");
                            result.returnRelated = string.Empty;
                            result.resultData = null;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                        }
                        #endregion R23.08 New Response no data from ATN

                        #region R23.08 Old Version: ReCall Service ATN By idCardNo
                        ////Check NO_RELATE Check ID Card
                        //if (resultATNIntra.resultCode == "40401")
                        //{
                        //    string tmpUrlNoRelate = configAtn.LOV_VAL1.ToSafeString();
                        //    tmpUrlNoRelate = tmpUrlNoRelate + "idCardNo=" + query.idCardNo; //R23.08 Call ATN ReCheck NO RELATE By ID Card

                        //    var clientNoRelate = new RestClient(tmpUrlNoRelate);
                        //    var requestNoRelate = new RestRequest();
                        //    requestNoRelate.Method = Method.GET;
                        //    requestNoRelate.AddHeader("channel", !string.IsNullOrEmpty(query.channel) ? query.channel.ToSafeString() : "FBB");
                        //    requestNoRelate.AddHeader("username", query.username.ToSafeString());

                        //    //execute
                        //    ServicePointManager.Expect100Continue = true;
                        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        //    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                        //    var responseNoRelate = clientNoRelate.Execute(requestNoRelate);
                        //    var contentNoRelate = responseNoRelate.Content;

                        //    if (responseNoRelate.StatusCode == HttpStatusCode.OK)
                        //    {
                        //        ServiceATNIntraModel resultATNIntraNoRelate = null;
                        //        resultATNIntraNoRelate = JsonConvert.DeserializeObject<ServiceATNIntraModel>(responseNoRelate.Content) ?? new ServiceATNIntraModel();

                        //        if (resultATNIntraNoRelate != null && resultATNIntraNoRelate.resultCode == "20000")
                        //        {
                        //            #region Call Service ATN NO RELATE By idCardNo
                        //            foreach (var item in resultATNIntraNoRelate.resultData.listDefaultBa)
                        //            {
                        //                #region Filter Condition
                        //                if (configCondition.Where(c => c.LOV_VAL1.Contains("negoFlag")).FirstOrDefault().LOV_VAL2.ToSafeString() == item.negoFlag) continue;

                        //                bool PkgFilter = false;
                        //                foreach (var p in item.productPackage)
                        //                {
                        //                    foreach (var pGroup in configCondition.Where(c => c.LOV_VAL1.Contains("productGroup")))
                        //                    {
                        //                        if (pGroup.LOV_VAL2.ToSafeString() == p.productGroup)
                        //                        {
                        //                            foreach (var pPkg in configCondition.Where(c => c.LOV_VAL1.Contains("productPkg")))
                        //                            {
                        //                                if (pPkg.LOV_VAL2.ToSafeString() == p.productPkg)
                        //                                {
                        //                                    PkgFilter = true;
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }

                        //                if (PkgFilter) continue;
                        //                #endregion Filter Condition

                        //                resultDataATN dataATN = new resultDataATN();

                        //                foreach (var m in item.mobile)
                        //                {
                        //                    dataATN.MobileNo = string.IsNullOrEmpty(dataATN.MobileNo) ? m.mobileNo : dataATN.MobileNo;
                        //                    dataATN.MobileNoDisplay = string.IsNullOrEmpty(dataATN.MobileNoDisplay) ? ConvertMobileDisplay(m.mobileNo) : dataATN.MobileNoDisplay;
                        //                    dataATN.MobileNoGroup = string.IsNullOrEmpty(dataATN.MobileNoGroup) ? m.mobileNo : dataATN.MobileNoGroup + ", " + m.mobileNo;
                        //                    dataATN.MobileNoGroupDisplay = string.IsNullOrEmpty(dataATN.MobileNoGroupDisplay) ? ConvertMobileDisplay(m.mobileNo) : dataATN.MobileNoGroupDisplay + ", " + ConvertMobileDisplay(m.mobileNo);
                        //                }

                        //                dataATN.BaNo = item.baNo.ToSafeString();
                        //                dataATN.HomeId = item.billAddress.FirstOrDefault().houseNo.ToSafeString();
                        //                dataATN.Moo = item.billAddress.FirstOrDefault().moo.ToSafeString();
                        //                dataATN.Mooban = item.billAddress.FirstOrDefault().mooban.ToSafeString();
                        //                dataATN.Building = item.billAddress.FirstOrDefault().building.ToSafeString();
                        //                dataATN.Floor = item.billAddress.FirstOrDefault().floor.ToSafeString();
                        //                dataATN.Room = item.billAddress.FirstOrDefault().room.ToSafeString();
                        //                dataATN.Soi = item.billAddress.FirstOrDefault().soi.ToSafeString();
                        //                dataATN.Street = item.billAddress.FirstOrDefault().street.ToSafeString();
                        //                dataATN.Ampur = item.billAddress.FirstOrDefault().amphur.ToSafeString();
                        //                dataATN.Tumbon = item.billAddress.FirstOrDefault().tumbol.ToSafeString();
                        //                dataATN.ZipCode = item.billAddress.FirstOrDefault().zipCode.ToSafeString();
                        //                dataATN.Province = item.billAddress.FirstOrDefault().province.ToSafeString();

                        //                dataATN.BillCycle = item.billCycle;
                        //                dataATN.BillMedia = item.billMedia;
                        //                dataATN.BillCycleInfo = !string.IsNullOrEmpty(item.billDisplay) && query.languageCode == "1" ?
                        //                    configBillCycle.Where(l => l.LOV_VAL3.Contains(item.billDisplay)).FirstOrDefault()?.LOV_VAL1.ToSafeString() ?? item.billDisplay.ToSafeString() :
                        //                    configBillCycle.Where(l => l.LOV_VAL3.Contains(item.billDisplay)).FirstOrDefault()?.LOV_VAL2.ToSafeString() ?? item.billDisplay.ToSafeString();
                        //                dataATN.ChannelViewBill = !string.IsNullOrEmpty(item.billMedia) && query.languageCode == "1" ?
                        //                    configBillMedia.Where(l => l.LOV_VAL3.Contains(item.billMedia)).FirstOrDefault()?.LOV_VAL1.ToSafeString() ?? item.billMedia.ToSafeString() :
                        //                    configBillMedia.Where(l => l.LOV_VAL3.Contains(item.billMedia)).FirstOrDefault()?.LOV_VAL2.ToSafeString() ?? item.billMedia.ToSafeString();

                        //                resultListDataATN.Add(dataATN);
                        //            }
                        //            #endregion Call Service ATN NO RELATE By idCardNo

                        //            if (resultListDataATN != null && resultListDataATN.Count > 0)
                        //            {
                        //                result.returnCode = "0";
                        //                result.returnMessage = "Success";
                        //                result.returnRelated = query.mode.ToSafeString();
                        //                result.resultData = resultListDataATN;
                        //                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + contentNoRelate + " \r\n" + "- Call By idCardNo for " + query.mode.ToSafeString() + " \r\n", log, "Success", "", "");
                        //            }
                        //            else
                        //            {
                        //                result.returnCode = "-2";
                        //                result.returnMessage = "resultData null";
                        //                result.returnRelated = query.mode.ToSafeString();
                        //                result.resultData = null;
                        //                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + contentNoRelate + " \r\n", log, "Success", "resultData null", "");
                        //            }
                        //        }
                        //        else
                        //        {
                        //            result.returnCode = "-1";
                        //            result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(resultATNIntraNoRelate.resultCode) ? resultATNIntraNoRelate.resultCode.ToSafeString() + " " + resultATNIntraNoRelate.resultDescription.ToSafeString() : "response is null");
                        //            result.returnRelated = string.Empty;
                        //            result.resultData = null;
                        //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + contentNoRelate + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                        //        }
                        //    }
                        //    else if (responseNoRelate.StatusCode == HttpStatusCode.NotFound) //404
                        //    {
                        //        ServiceATNIntraModel resultATNIntraNoRelate = null;
                        //        resultATNIntraNoRelate = JsonConvert.DeserializeObject<ServiceATNIntraModel>(responseNoRelate.Content) ?? new ServiceATNIntraModel();

                        //        if (resultATNIntraNoRelate.resultCode == "40401")
                        //        {
                        //            result.returnCode = "-2";
                        //            result.returnMessage = "resultData null";
                        //            result.returnRelated = query.mode.ToSafeString();
                        //            result.resultData = null;
                        //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + contentNoRelate + " \r\n", log, "Success", "resultData null", "");
                        //        }
                        //        else
                        //        {
                        //            result.returnCode = "-1";
                        //            result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(resultATNIntraNoRelate.resultCode) ? resultATNIntraNoRelate.resultCode.ToSafeString() + " " + resultATNIntraNoRelate.resultDescription.ToSafeString() : "response is null");
                        //            result.returnRelated = string.Empty;
                        //            result.resultData = null;
                        //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + contentNoRelate + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                        //        }
                        //    }
                        //    else if (responseNoRelate.StatusCode == HttpStatusCode.GatewayTimeout) //504
                        //    {
                        //        result.returnCode = "-1";
                        //        result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(responseNoRelate.StatusCode.ToSafeString()) ? (int)HttpStatusCode.GatewayTimeout + " " + responseNoRelate.StatusCode.ToSafeString() + " " + responseNoRelate.Content.ToSafeString() : "response is null");
                        //        result.returnRelated = string.Empty;
                        //        result.resultData = null;
                        //        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + contentNoRelate + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                        //    }
                        //    else
                        //    {
                        //        result.returnCode = "-1";
                        //        result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(responseNoRelate.StatusCode.ToSafeString()) ? (int)responseNoRelate.StatusCode + " " + responseNoRelate.StatusCode.ToSafeString() + " " + responseNoRelate.Content.ToSafeString() : "response is null");
                        //        result.returnRelated = string.Empty;
                        //        result.resultData = null;
                        //        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + contentNoRelate + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                        //    }
                        //}
                        //else
                        //{
                        //    result.returnCode = "-1";
                        //    result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(resultATNIntra.resultCode) ? resultATNIntra.resultCode.ToSafeString() + " " + resultATNIntra.resultDescription.ToSafeString() : "response is null");
                        //    result.returnRelated = string.Empty;
                        //    result.resultData = null;
                        //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                        //}
                        #endregion R23.08 Old Version: ReCall Service ATN By idCardNo
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized) //401
                    {
                        result.returnCode = "-1";
                        result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(response.StatusCode.ToSafeString()) ? (int)HttpStatusCode.Unauthorized + " " + response.StatusCode.ToSafeString() : "response is null");
                        result.returnRelated = string.Empty;
                        result.resultData = null;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden) //403
                    {
                        result.returnCode = "-1";
                        result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(response.StatusCode.ToSafeString()) ? (int)HttpStatusCode.Forbidden + " " + response.StatusCode.ToSafeString() : "response is null");
                        result.returnRelated = string.Empty;
                        result.resultData = null;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError) //500
                    {
                        result.returnCode = "-1";
                        result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(response.StatusCode.ToSafeString()) ? (int)HttpStatusCode.InternalServerError + " " + response.StatusCode.ToSafeString() + " " + response.Content.ToSafeString() : "response is null");
                        result.returnRelated = string.Empty;
                        result.resultData = null;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                    }
                    else if (response.StatusCode == HttpStatusCode.GatewayTimeout) //504
                    {
                        result.returnCode = "-1";
                        result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(response.StatusCode.ToSafeString()) ? (int)HttpStatusCode.GatewayTimeout + " " + response.StatusCode.ToSafeString() + " " + response.Content.ToSafeString() : "response is null");
                        result.returnRelated = string.Empty;
                        result.resultData = null;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                    }
                    else
                    {
                        result.returnCode = "-1";
                        result.returnMessage = "Failed : " + (!string.IsNullOrEmpty(response.StatusCode.ToSafeString()) ? (int)response.StatusCode + " " + response.StatusCode.ToSafeString() + " " + response.Content.ToSafeString() : "response is null");
                        result.returnRelated = string.Empty;
                        result.resultData = null;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, " \r\n" + "- Return For Web: " + " \r\n" + ConvertToJson(result) + " \r\n" + "- Call Service ATN list-defaultba: " + " \r\n" + content + " \r\n", log, "Failed", result.returnMessage.ToSafeString(), "");
                    }
                }
                else
                {
                    result.returnCode = "-1";
                    result.returnMessage = "Failed : API Endpoint";
                    result.returnRelated = string.Empty;
                    result.resultData = null;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ConvertToJson(result), log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                result.returnCode = "-1";
                result.returnMessage = "Failed : " + ex.Message;
                result.returnRelated = string.Empty;
                result.resultData = null;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ConvertToJson(result), log, "Failed", ex.GetBaseException().ToString(), "");
            }

            return result;
        }

        private string ConvertMobileDisplay(string mobileNo)
        {
            string resultMobileNo = mobileNo; string pattern; string replacement;
            try
            {
                pattern = mobileNo.Length == 10 ? @"^(\d{3})(\d{3})(\d{4})$" : @"^(\+?66|0)(\d{1,2})(\d{3})(\d{4})$";
                replacement = mobileNo.Length == 10 ? @"$1-XXX-$3" : @"$1$2-XXX-$4";
                resultMobileNo = Regex.Replace(mobileNo, pattern, replacement);
            }
            catch (Exception)
            {
                resultMobileNo = mobileNo;
            }
            return resultMobileNo;
        }

        private string ConvertToJson<T>(T value)
        {
            if (value == null)
            {
                return "";
            }

            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
