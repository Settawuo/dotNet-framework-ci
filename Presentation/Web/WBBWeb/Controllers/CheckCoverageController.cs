using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WBBBusinessLayer;
using WBBBusinessLayer.FBSSOrderServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBSS;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBSS;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBContract.Queries.WebServices.WTTX;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;
using WBBSECURE = WBBBusinessLayer.Extension.Security;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class CheckCoverageController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CoverageResultCommand> _covResultCommand;
        private readonly ICommandHandler<FBSSCoverageResultCommand> _fbssCovResultCommand;
        private readonly IQueryHandler<GetFBBFBSSCoverageAreaResultQuery, GetLeaveMsgReferenceNoCommand> _fbbfbssCoverageResult;


        public CheckCoverageController(IQueryProcessor queryProcessor
              , ICommandHandler<CoverageResultCommand> covResultCommand
              , ICommandHandler<FBSSCoverageResultCommand> fbssCovResultCommand
              , IQueryHandler<GetFBBFBSSCoverageAreaResultQuery, GetLeaveMsgReferenceNoCommand> fbbfbssCoverageResult
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _covResultCommand = covResultCommand;
            _fbssCovResultCommand = fbssCovResultCommand;
            _fbbfbssCoverageResult = fbbfbssCoverageResult;
            base.Logger = logger;
        }

        public JsonResult GetMuban(string province = "", string amphur = "", string tumbon = "", string langFlag = "", string sso = "")
        {
            var query = new SelectMubanQuery
            {
                province = province,
                aumphur = amphur,
                tumbon = tumbon,
                Language = langFlag, // 1 for thai or 2 for eng
                SSO = sso
            };

            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetProvince()
        {
            var provType = new List<DropdownModel>();
            try
            {
                provType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            //provType.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(provType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAmphur(string provinceFilter)
        {
            var amphType = new List<DropdownModel>();
            try
            {
                /// เพิ่ม เอา ปณ ออก 01/07/2015
                amphType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter)) && (!z.Amphur.Contains("ปณ") && !z.Amphur.Contains("PO")))
                    .GroupBy(z => z.Amphur)
                    .Select(z =>
                    {
                        var item = z.First();

                        return new DropdownModel { Text = item.Amphur, Value = item.Amphur };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            //amphType.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(amphType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTumbon(string provinceFilter, string amphurFilter)
        {
            var tumbType = new List<DropdownModel>();
            try
            {
                tumbType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter))
                                    && (string.IsNullOrEmpty(z.Amphur) || z.Amphur.Equals(amphurFilter)))
                    .GroupBy(z => z.Tumbon)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Tumbon, Value = item.Tumbon };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();
            }
            catch (Exception) { }

            //tumbType.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });

            return Json(tumbType, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetCoverageZipCode(string provinceFilter, string amphurFilter, string tumbonFilter, string sso)
        {
            Session["CoverageList"] = null;
            var coverageList = new List<DropdownModel>();
            var zipCodeList = new List<DropdownModel>();
            try
            {
                var amphurToFilter = "";
                int index1 = amphurFilter.IndexOf('(');
                if (index1 > 0)
                {
                    int index2 = amphurFilter.IndexOf(')');
                    amphurToFilter = amphurFilter.Remove(index1, index2 - index1 + 1);
                }
                else
                {
                    amphurToFilter = amphurFilter;
                }

                // find zip code.
                zipCodeList = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.Equals(provinceFilter))
                        && (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.Contains(amphurToFilter))
                        && (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.Equals(tumbonFilter)))
                    .Select(z => new DropdownModel { Text = z.ZipCode, Value = z.ZipCodeId, })
                    .OrderBy(o => o.Text)
                    .ToList();

                // find coverage ไม่ได้ใช้แล้ว
                foreach (var zip in zipCodeList)
                {
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    {
                        coverageList.AddRange(this.GetCoverageValueModel(zip.Value, provinceFilter, amphurFilter, sso)
                            .Select(c => new DropdownModel
                            {
                                Text = c.NodeNameTh,
                                Value = c.CvrId.ToSafeString(),
                                Value2 = c.Moo,
                                Value3 = c.Soi_Th,
                                Value4 = c.Road_Th,
                                Value5 = c.Zipcode,
                            })
                            .DistinctBy(c => c.Text)
                            .ToList());
                    }
                    else
                    {
                        coverageList.AddRange(this.GetCoverageValueModel(zip.Value, provinceFilter, amphurFilter, sso)
                            .Select(c => new DropdownModel
                            {
                                Text = c.NodeNameEn,
                                Value = c.CvrId.ToSafeString(),
                                Value2 = c.Moo,
                                Value3 = c.Soi_En,
                                Value4 = c.Road_En,
                                Value5 = c.Zipcode,
                            })
                            .DistinctBy(c => c.Text)
                            .ToList());
                    }
                }

                var listSym = (from cl in coverageList
                               where cl.Value == "-9"
                               select cl).DistinctBy(c => c.Text).ToList();

                coverageList = (from cl in coverageList
                                where cl.Value != "-9"
                                select cl).DistinctBy(p => p.Value).ToList();

                coverageList.AddRange(listSym);

                //coverageList = coverageList.DistinctBy(p => p.Value).ToList(); //distinct by boy

                Session["CoverageList"] = coverageList;

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    coverageList.Add(new DropdownModel { Text = "อื่น ๆ ระบุ", Value = "0" });
                }
                else
                {
                    coverageList.Add(new DropdownModel { Text = "Other", Value = "0" });
                }
            }
            catch (Exception) { }

            return Json(new { zipCodeList, coverageList }, JsonRequestBehavior.AllowGet);
            //return Json(new { zipCodeModel.ZipCode, zipCodeModel.ZipCodeId }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDefaultAddress(string key)
        {
            var data = ((List<DropdownModel>)(Session["CoverageList"])).Where(p => p.Value.Equals(key));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPortAvailable(int cvrid, string flagOnlineNo, string buildname)//FBB
        {
            var query = new GetPortAvaliableQuery
            {
                CVRID = cvrid,
                FLAGONLINENUMBER = flagOnlineNo,
                TOWER = buildname
            };

            var result = _queryProcessor.Execute(query);

            if (result.RETURN_CODE != 0)
                return Json(new { available = false, message = result.RETURN_DESC }, JsonRequestBehavior.AllowGet);

            return Json(new { available = result.SBNCheckCoverageData.AVALIABLE.ToYesNoFlgBoolean(), message = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAirnetWirelessCoverage(string cvrid, string coverageType, string floor, string latitude, string longitude, string sso = "")
        {
            var query = new GetAirnetWirelessCoverageQuery
            {
                CVRID = cvrid.ToSafeInteger(),
                LAT = latitude,
                LNG = longitude,
                COVERAGETYPE = coverageType,
                FLOOR = floor.ToSafeDecimal(),
                SSO = sso
            };

            var result = _queryProcessor.Execute(query);

            if (result.RETURN_CODE != 0)
                return Json(new { available = false, message = result.RETURN_DESC }, JsonRequestBehavior.AllowGet);

            return Json(new { available = result.SBNCheckCoverageData.AVALIABLE.ToYesNoFlgBoolean(), message = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCoverageRegion(string province, string amphur, string tambon, string language, string tower = "", string sso = "", string serviceType = "")
        {
            var query = new GetCoverageRegionQuery //physical file: ExWebServices > GetCoverageRegionQuery
            {
                Province = province,
                Aumphur = amphur,
                Tambon = tambon,
                Language = language,
                SSO = sso,
                ServiceType = serviceType
            };

            var result = _queryProcessor.Execute(query);

            if (result.RETURN_CODE != 0)
                return Json(new { available = false, message = result.RETURN_DESC, result.SBNCheckCoverageData.OWNER_PRODUCT }, JsonRequestBehavior.AllowGet);

            return Json(new { available = result.SBNCheckCoverageData.AVALIABLE.ToYesNoFlgBoolean(), message = "", result.SBNCheckCoverageData.OWNER_PRODUCT }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetCoverrageValueModel(string zipCodeId)
        {
            var coverage = new List<DropdownModel>();

            try
            {
                var query = new GetCoverageAreaQuery
                {
                    CurrentCulture = SiteSession.CurrentUICulture,
                    ZipCodeId = zipCodeId
                };

                var result = _queryProcessor.Execute(query);

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    coverage = result
                        .Select(c => new DropdownModel { Text = c.NodeNameTh, Value = c.CvrId.ToSafeString() })
                        .ToList();
                    coverage.Add(new DropdownModel { Text = "อื่น ๆ ระบุ", Value = "0" });
                }
                else
                {
                    coverage = result
                        .Select(c => new DropdownModel { Text = c.NodeNameEn, Value = c.CvrId.ToSafeString() })
                        .ToList();
                    coverage.Add(new DropdownModel { Text = "Other", Value = "0" });
                }
            }
            catch (Exception) { }

            return Json(coverage.OrderBy(o => o.Text), JsonRequestBehavior.AllowGet);
        }

        private List<CoverageValueModel> GetCoverageValueModel(string zipCodeId, string province, string amphur, string sso)
        {
            try
            {
                var query = new GetCoverageAreaQuery
                {
                    CurrentCulture = SiteSession.CurrentUICulture,
                    ZipCodeId = zipCodeId,
                    Province = province,
                    Amphur = amphur,
                    SSO = sso
                };

                var result = _queryProcessor.Execute(query);
                return result;
            }
            catch (Exception) { }

            return new List<CoverageValueModel>();
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetCoverrageRelValueModel(string coverageNodeName)
        {
            var covRelArea = new List<DropdownModel>();

            try
            {
                var query = new GetCoverageAreaRelQuery
                {
                    CurrentCulture = SiteSession.CurrentUICulture,
                    NodeName = coverageNodeName,
                };

                var result = _queryProcessor.Execute(query);

                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    covRelArea = result
                        .Select(c => new DropdownModel { Text = c.TowerNameTh, Value = c.CvrId.ToSafeString(), Value2 = c.Latitute, Value3 = c.Longitude })
                        .ToList();
                }
                else
                {
                    covRelArea = result
                        .Select(c => new DropdownModel { Text = c.TowerNameEn, Value = c.CvrId.ToSafeString(), Value2 = c.Latitute, Value3 = c.Longitude })
                        .ToList();
                }
            }
            catch (Exception) { }

            return Json(covRelArea, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertCoverageResult(string cvrid,
            string nodeName,
            string tower,
            string floor,
            string isOnlineNumber,
            string addressNo,
            string moo,
            string soi,
            string road,
            string coverageType,
            string coverageResult,
            string lat,
            string lng,
            string prodType,
            string zipCodeRowId,
            string owner,
            bool chkCoverage
            )
        {
            if (coverageType != "CONDOMINIUM")
                floor = "1";

            var command = new CoverageResultCommand
            {
                CVRID = cvrid.ToSafeDecimal(),
                NODENAME = nodeName,
                TOWER = tower,
                FLOOR = floor.ToSafeDecimal(),
                ISONLINENUMBER = isOnlineNumber,
                ADDRESS_NO = addressNo,
                MOO = moo.ToSafeDecimal(),
                SOI = soi,
                ROAD = road,
                COVERAGETYPE = coverageType,
                COVERAGERESULT = coverageResult,
                LATITUDE = lat,
                LONGITUDE = lng,
                PRODUCTTYPE = prodType,
                ZIPCODE_ROWID = zipCodeRowId,
                OWNER = owner
            };

            try
            {
                command.ActionType = ActionType.Insert;
                CoverageResultHandler(command);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }

            return Json(command.RESULT_ID, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FBSSCoverageResultCommand(string actionType = "", string addrType = "", string postCode = "", string subDistrict = "",
            string lang = "", string buildName = "", string buildNo = "", string phoneFlg = "", string floor = "", string addrNo = "",
            decimal moo = 0, string soi = "", string road = "", string lat = "", string lng = "", string coverage = "", string addrId = "",
            string accessMode = "", string planSite = "", string isPartner = "", string partner = "", string preName = "", string fName = "",
            string lName = "", string contactNo = "", string productType = "", string zipRowId = "", string owner = "", decimal resultId = 0,
            string interfaceId = "", decimal recode = 0, string remessage = "", string reorder = "",
            string email = "", string lineid = "", bool chkCoverage = false, string lcCode = "", string ascCode = "", string employeeID = "",
            string saleFirstname = "", string saleLastname = "", string locationName = "", string subRegion = "", string region = "", string ascName = "",
            string channelName = "", string saleChannel = "", string addressTypeDTL = "", string remark = "", string technology = "", string projectName = "")
        {
            var user = "CUSTOMER";
            try
            {
                if (base.CurrentUser != null)
                    user = base.CurrentUser.UserName;
            }
            catch (Exception ex)
            {
            }
            if (actionType == "Insert")
            {
                var command = new FBSSCoverageResultCommand
                {
                    ActionType = actionType.ParseEnum<ActionType>(),
                    ADDRRESS_TYPE = addrType,
                    POSTAL_CODE = postCode,
                    SUB_DISTRICT_NAME = subDistrict,
                    LANGUAGE = lang,
                    BUILDING_NAME = buildName,
                    BUILDING_NO = buildNo,
                    PHONE_FLAG = phoneFlg,
                    FLOOR_NO = floor,
                    ADDRESS_NO = addrNo,
                    MOO = moo,
                    SOI = soi,
                    ROAD = road,
                    LATITUDE = lat,
                    LONGITUDE = lng,
                    COVERAGE = coverage,
                    ADDRESS_ID = addrId,
                    ACCESS_MODE_LIST = accessMode,
                    PLANNING_SITE_LIST = planSite,
                    IS_PARTNER = isPartner,
                    PARTNER_NAME = partner,
                    PRODUCTTYPE = productType,
                    ZIPCODE_ROWID = zipRowId,
                    OWNER_PRODUCT = owner,
                    USERNAME = user,
                    TRANSACTION_ID = interfaceId,
                    LOCATION_CODE = lcCode,
                    ASC_CODE = ascCode,
                    EMPLOYEE_ID = employeeID,
                    SALE_FIRSTNAME = saleFirstname,
                    SALE_LASTNAME = saleLastname,
                    LOCATION_NAME = locationName,
                    SUB_REGION = subRegion,
                    REGION = region,
                    ASC_NAME = ascName,
                    CHANNEL_NAME = channelName,
                    SALE_CHANNEL = saleChannel,
                    //R21.2
                    ADDRESS_TYPE_DTL = addressTypeDTL,
                    REMARK = remark,
                    TECHNOLOGY = technology,
                    PROJECTNAME = projectName
                };
                if (chkCoverage)
                    command.ActionBy = "WEB COVERAGE";
                _fbssCovResultCommand.Handle(command);
                return Json(command.RESULTID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Update
                var command = new FBSSCoverageResultCommand
                {
                    ActionType = actionType.ParseEnum<ActionType>(),
                    USERNAME = user,
                    PREFIXNAME = preName,
                    FIRSTNAME = fName,
                    LASTNAME = lName,
                    EMAIL = email,
                    LINEID = lineid,
                    CONTACTNUMBER = contactNo,
                    RESULTID = resultId,
                    RETURN_CODE = recode,
                    RETURN_MESSAGE = remessage,
                    RETURN_ORDER = reorder,
                    TRANSACTION_ID = interfaceId,
                    PARTNER_NAME = partner,
                    ADDRRESS_TYPE = addrType,
                    ADDRESS_ID = addrId,
                    OWNER_PRODUCT = owner,
                    PRODUCTTYPE = productType,

                    COVERAGE = coverage,
                    BUILDING_NAME = buildName,
                    ADDRESS_NO = addrNo,
                    MOO = moo,
                    SOI = soi,
                    ROAD = road,
                    LOCATION_CODE = lcCode,
                    ASC_CODE = ascCode,
                    EMPLOYEE_ID = employeeID,
                    SALE_FIRSTNAME = saleFirstname,
                    SALE_LASTNAME = saleLastname,
                    LOCATION_NAME = locationName,
                    SUB_REGION = subRegion,
                    REGION = region,
                    ASC_NAME = ascName,
                    CHANNEL_NAME = channelName,
                    SALE_CHANNEL = saleChannel,
                    //R21.2
                    ADDRESS_TYPE_DTL = addressTypeDTL,
                    REMARK = remark,
                    TECHNOLOGY = technology,
                    PROJECTNAME = projectName
                };
                if (chkCoverage)
                    command.ActionBy = "WEB COVERAGE";
                _fbssCovResultCommand.Handle(command);
                return Json(command.RESULTID, JsonRequestBehavior.AllowGet);
            }
        }
        public GetLeaveMsgReferenceNoCommand FBSSCoverageResultQuery(decimal resultId = 0, string transactionId = "", string assetNumber = "", string caseId = "", string referenceNoStatus = "")
        {
            var query = new GetFBBFBSSCoverageAreaResultQuery
            {
                RESULTID = resultId,
                TRANSACTIONID = transactionId,
                ASSET_NUMBER = assetNumber,
                CASE_ID = caseId,
                REFERENCE_NO_STATUS = referenceNoStatus
            };
            var result = _fbbfbssCoverageResult.Handle(query);
            return result;
        }
        public void UpdateCoverageResult(string resultId, string titleName,
            string firstName, string lastName, string contactPhone, int returnCode, string returnMessage, string returnOrder)
        {
            try
            {
                var command = new CoverageResultCommand
                {
                    RESULT_ID = resultId.ToSafeDecimal(),
                    PREFIXNAME = titleName,
                    FIRSTNAME = firstName,
                    LASTNAME = lastName,
                    CONTACTNUMBER = contactPhone,
                    ReturnCode = returnCode,
                    ReturnMessage = returnMessage,
                    ReturnOrder = returnOrder
                };

                command.ActionType = ActionType.Update;
                CoverageResultHandler(command);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }

            //return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void CoverageResultHandler(CoverageResultCommand command)
        {
            _covResultCommand.Handle(command);
        }

        [HttpPost]
        //[OutputCache(Duration = 1800, VaryByParam = "mobileNo;cardNo;cardType;line;")]
        public JsonResult evESeServiceQueryMassCommonAccountInfo(string mobileNo = "", string cardNo = "", string cardType = "", string line = "", string SubNetworkType = "", string page = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            string user = "Customer";
            if (cardNo == "officer")  /// case officer
            {
                user = cardType;
                cardNo = "";
            }
            if (base.CurrentUser != null) user = base.CurrentUser.UserName;

            string InOption = "";
            if (SubNetworkType == "PREPAID")
            {
                InOption = "4";
            }
            else
            {
                InOption = "2";
            }

            mobileNo = DecryptStringAES(mobileNo, "fbbwebABCDEFGHIJ");

            if ((cardNo != "" && cardType != "") || user != "Customer")
            {

                bool haveProfile = false;
                if (user == "Customer")
                {
                    try
                    {

                        var query = new evESeServiceQueryMassCommonAccountInfoQuery
                        {
                            inOption = "2",
                            inMobileNo = mobileNo,
                            inCardNo = cardNo,
                            inCardType = cardType,
                            Page = "Check Coverage",
                            Username = user,
                            FullUrl = FullUrl
                        };
                        var massCommon = _queryProcessor.Execute(query);
                        if (massCommon.errorMessage == "")
                        {
                            haveProfile = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        return Json(
                            new
                            {
                                data = new
                                {
                                    GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                                    errorMessage = "No Profile"
                                }
                            }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    haveProfile = true;
                }

                if (haveProfile)
                {
                    evESeServiceQueryMassCommonAccountInfoQuery query = new evESeServiceQueryMassCommonAccountInfoQuery
                    {
                        //inOption = "1",
                        inOption = InOption,//16.4 change use option2
                        inMobileNo = mobileNo,
                        inCardNo = cardNo,
                        inCardType = cardType,
                        Page = "Check Coverage",
                        Username = user,
                        FullUrl = FullUrl
                    };
                    var a = _queryProcessor.Execute(query);
                    if (a != null)
                    {
                        if (line == "2")
                        {
                            if (a.errorMessage != null && a.errorMessage.Trim() != "EB0001")
                            {
                                evOMQueryListServiceAndPromotionByPackageTypeQuery query1 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                                {
                                    mobileNo = mobileNo,
                                    idCard = "",
                                    FullUrl = FullUrl
                                };
                                var aa = _queryProcessor.Execute(query1);

                                a.OwnerProduct = aa.v_owner_product;
                                a.PackageCode = aa.v_sff_main_promotionCD;
                                a.outAddressId = aa.addressId;
                                a.outMobileSegment = a.outMobileSegment == null ? "" : a.outMobileSegment.ToUpper();
                                a.outAccessType = aa.access_mode;
                            }

                            //var q = new WBBContract.Queries.WebServices.GetOwnerProductByNoQuery
                            //{
                            //    No = mobileNo
                            //};
                            //var aa = _queryProcessor.Execute(q);
                            //if (a.errorMessage != null && a.errorMessage.Trim() != "EB0001")
                            //{
                            //    a.OwnerProduct = aa.Value;
                            //    a.PackageCode = aa.Value2;
                            //    a.outAddressId = aa.Value3;
                            //    a.outMobileSegment = a.outMobileSegment == null ? "" : a.outMobileSegment.ToUpper();
                            //}
                        }

                        //R20.6 Add by Aware : Atipon
                        if (page == "changepro")
                        {
                            var queryOption4 = new evESeServiceQueryMassCommonAccountInfoQuery
                            {
                                inOption = "4",
                                inMobileNo = mobileNo,
                                inCardNo = cardNo,
                                inCardType = cardType,
                                Page = "Check Coverage",
                                Username = user,
                                FullUrl = FullUrl
                            };
                            var massOption4 = _queryProcessor.Execute(queryOption4);

                            if (massOption4 != null && (massOption4.projectName == "FMC" || massOption4.projectName == "Waiting FMC"))
                            {
                                a.outMobileNumber = massOption4.curMobileCheckRight;
                            }
                            else if (massOption4 != null && massOption4.projectName == "FMB1")
                            {
                                a.outMobileNumber = massOption4.curMobileGetBenefit;
                            }

                            a.curMobileCheckRight = massOption4.curMobileCheckRight;
                            a.curMobileCheckRightOption = massOption4.curMobileCheckRightOption;
                            a.curMobileGetBenefit = massOption4.curMobileGetBenefit;
                            a.curMobileGetBenefitOption = massOption4.curMobileGetBenefitOption;
                        }
                        //
                        //23.06 TopupIPCamera
                        if (FullUrl.IndexOf("TopupIPCamera") != -1)
                        {
                            var data = evESQueryPersonalInformationIPcamera(mobileNo, "2");
                            var result = data.Find(x => x.productPkg == "AIS Cloud IP Camera Fee");
                            ViewBag.TopupIPcameraPersonalInfo = result;
                        }
                        if (FullUrl.IndexOf("TopupMesh") != -1 || FullUrl.IndexOf("ExistingFibre") != -1 || FullUrl.IndexOf("TopupIPCamera") != -1)
                        {
                            evOMCheckDeviceContractQuery checkDeviceContractQuery = new evOMCheckDeviceContractQuery()
                            {
                                inCardNo = cardNo,
                                inCardType = cardType,
                                inMobileNo = mobileNo,
                                FullUrl = FullUrl
                            };
                            var checkDeviceContractData = _queryProcessor.Execute(checkDeviceContractQuery);
                            if (checkDeviceContractData.errorMessage == "")
                            {
                                a.contractFlagFbb = checkDeviceContractData.contractFlagFbb;
                                a.countContractFbb = checkDeviceContractData.countContractFbb;
                                a.fbbLimitContract = checkDeviceContractData.fbbLimitContract;
                                a.contractProfileCountFbb = checkDeviceContractData.contractProfileCountFbb;
                            }

                        }

                        a.GUIDKEY = Guid.NewGuid().ToSafeString();
                        Session["CHECKCOVERAGE_SFFDATA"] = a;
                        return Json(
                        new
                        {
                            data = new
                            {
                                GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt(a.GUIDKEY),
                                errorMessage = a.errorMessage
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(
                        new
                        {
                            data = new
                            {
                                GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                                errorMessage = "evESeServiceQueryMass return null"
                            }
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(
                        new
                        {
                            data = new
                            {
                                GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                                errorMessage = "No Profile"
                            }
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                base.Logger.Info("evESeServiceQueryMassCommonAccountInfo: cardNo or cardType is null");
                return Json(
                new
                {
                    data = new
                    {
                        GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt("XXX"),
                        errorMessage = "EB0001"
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckIsAIS(string mobileNo, string Page)
        {
            bool IsAis = false;
            string FullUrl = "";

            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            mobileNo = DecryptStringAES(mobileNo, "fbbwebABCDEFGHIJ");
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "2",
                inMobileNo = mobileNo,
                inCardNo = "",
                inCardType = "",
                Page = Page,
                Username = "Customer",
                FullUrl = FullUrl
            };
            var a = _queryProcessor.Execute(query);
            if (a != null)
            {
                if (!String.IsNullOrEmpty(a.errorMessage))
                {
                    IsAis = true;
                }
            }

            return Json(new { status = IsAis }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetProcessAisAirnet(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CHECKCOVERAGE_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;

            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            outProductName = sffData.outProductName,
                            vataddTripleplay = sffData.vataddTripleplay,
                            outMobileSegment = sffData.outMobileSegment,
                            outProvince = sffData.outProvince,
                            outAmphur = sffData.outAmphur,
                            outTumbol = sffData.outTumbol,
                            outMooban = sffData.outMooban,
                            outBuildingName = sffData.outBuildingName,
                            outFloor = sffData.outFloor,
                            outHouseNumber = sffData.outHouseNumber,
                            outMoo = sffData.outMoo,
                            outSoi = sffData.outSoi,
                            outStreetName = sffData.outStreetName,
                            outEmail = sffData.outEmail,
                            cardType = sffData.cardType,
                            outBillingAccountNumber = sffData.outBillingAccountNumber,
                            outAccountNumber = sffData.outAccountNumber,
                            outServiceAccountNumber = sffData.outServiceAccountNumber,
                            outDayOfServiceYear = sffData.outDayOfServiceYear,
                            outTitle = sffData.outTitle,
                            outBillingSystem = sffData.outBillingSystem,
                            outBirthDate = "", //sffData.outBirthDate,
                            outAccountName = "", //sffData.outAccountName,
                            outPrimaryContactFirstName = sffData.outPrimaryContactFirstName,
                            outContactLastName = sffData.outContactLastName,
                            SffProfileLogID = sffData.SffProfileLogID,
                            IsAWNProduct = sffData.IsAWNProduct,
                        },
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                new
                {
                    data = new
                    {
                        errorMessage = "ERROR"
                    }
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBindTriple(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CHECKCOVERAGE_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;
            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            outProvince = sffData.outProvince,
                            outAmphur = sffData.outAmphur,
                            outTumbol = sffData.outTumbol,
                            outBuildingName = sffData.outBuildingName,
                            outMooban = sffData.outMooban,
                            outFloor = sffData.outFloor,
                            outHouseNumber = sffData.outHouseNumber,
                            outMoo = sffData.outMoo,
                            outSoi = sffData.outSoi,
                            outStreetName = sffData.outStreetName,
                            outEmail = sffData.outEmail,
                            cardType = sffData.cardType,
                            outBillingAccountNumber = sffData.outBillingAccountNumber,
                            outAccountNumber = sffData.outAccountNumber,
                            outServiceAccountNumber = sffData.outServiceAccountNumber,
                            outProductName = sffData.outProductName,
                            outMobileSegment = sffData.outMobileSegment,
                            outDayOfServiceYear = sffData.outDayOfServiceYear,
                            outTitle = sffData.outTitle,
                            outBillingSystem = sffData.outBillingSystem,
                            outBirthDate = sffData.outBirthDate,
                            outAccountName = sffData.outAccountName,
                            outPrimaryContactFirstName = sffData.outPrimaryContactFirstName,
                            outContactLastName = sffData.outContactLastName,
                            SffProfileLogID = sffData.SffProfileLogID,
                            IsAWNProduct = sffData.IsAWNProduct,
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCheckProcess(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CHECKCOVERAGE_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;
            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            outMobileNumber = sffData.outMobileNumber,
                            outServiceMobileNo = sffData.outServiceMobileNo,
                            projectName = sffData.projectName,
                            checkPlayBox = sffData.checkPlayBox,
                            outPrimaryContactFirstName = sffData.outPrimaryContactFirstName,
                            outContactLastName = sffData.outContactLastName,
                            outAmphur = sffData.outAmphur,
                            outBuildingName = sffData.outBuildingName,
                            outFloor = sffData.outFloor,
                            outHouseNumber = sffData.outHouseNumber,
                            outMoo = sffData.outMoo,
                            outMooban = sffData.outMooban,
                            outProvince = sffData.outProvince,
                            outRoom = sffData.outRoom,
                            outSoi = sffData.outSoi,
                            outStreetName = sffData.outStreetName,
                            outBillLanguage = sffData.outBillLanguage,
                            outTumbol = sffData.outTumbol,
                            outBirthDate = sffData.outBirthDate,
                            outEmail = sffData.outEmail,
                            outparameter2 = sffData.outparameter2,
                            outAccountName = sffData.outAccountName,
                            outAccountNumber = sffData.outAccountNumber,
                            outServiceAccountNumber = sffData.outServiceAccountNumber,
                            outBillingAccountNumber = sffData.outBillingAccountNumber,
                            outProductName = sffData.outProductName,
                            outDayOfServiceYear = sffData.outDayOfServiceYear,
                            outRegisteredDate = sffData.outRegisteredDate,
                            cardType = sffData.cardType,
                            outAccountSubCategory = sffData.outAccountSubCategory,
                            outPostalCode = sffData.outPostalCode,
                            outTitle = sffData.outTitle,
                            SffProfileLogID = sffData.SffProfileLogID,
                            OwnerProduct = sffData.OwnerProduct,
                            PackageCode = sffData.PackageCode,
                            outFullAddress = sffData.outFullAddress,
                            vatAddress1 = sffData.vatAddress1,
                            vatAddress2 = sffData.vatAddress2,
                            vatAddress3 = sffData.vatAddress3,
                            vatAddress4 = sffData.vatAddress4,
                            vatAddress5 = sffData.vatAddress5,
                            vatPostalCd = sffData.vatPostalCd,
                            vatAddressFull = sffData.vatAddressFull,
                            outServiceLevel = sffData.outServiceLevel,
                            errorMessage = sffData.errorMessage,
                            contractFlagFbb = sffData.contractFlagFbb,
                            countContractFbb = sffData.countContractFbb,
                            fbbLimitContract = sffData.fbbLimitContract,
                            contractProfileCountFbb = sffData.contractProfileCountFbb
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCheckChanvePackageProcess(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CHECKCOVERAGE_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;
            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            outMobileNumber = sffData.outMobileNumber,
                            projectName = sffData.projectName,
                            checkPlayBox = sffData.checkPlayBox,
                            outPrimaryContactFirstName = sffData.outPrimaryContactFirstName,
                            outContactLastName = sffData.outContactLastName,
                            outAmphur = sffData.outAmphur,
                            outBuildingName = sffData.outBuildingName,
                            outFloor = sffData.outFloor,
                            outHouseNumber = sffData.outHouseNumber,
                            outMoo = sffData.outMoo,
                            outMooban = sffData.outMooban,
                            outProvince = sffData.outProvince,
                            outRoom = sffData.outRoom,
                            outSoi = sffData.outSoi,
                            outStreetName = sffData.outStreetName,
                            outBillLanguage = sffData.outBillLanguage,
                            outTumbol = sffData.outTumbol,
                            outBirthDate = "", //sffData.outBirthDate,
                            outEmail = sffData.outEmail,
                            outparameter2 = sffData.outparameter2,
                            outAccountName = sffData.outAccountName,
                            outAccountNumber = sffData.outAccountNumber,
                            outServiceAccountNumber = sffData.outServiceAccountNumber,
                            outBillingAccountNumber = sffData.outBillingAccountNumber,
                            outProductName = sffData.outProductName,
                            outDayOfServiceYear = sffData.outDayOfServiceYear,
                            cardType = sffData.cardType,
                            outAccountSubCategory = sffData.outAccountSubCategory,
                            outPostalCode = sffData.outPostalCode,
                            outTitle = sffData.outTitle,
                            SffProfileLogID = sffData.SffProfileLogID,
                            OwnerProduct = sffData.OwnerProduct,
                            PackageCode = sffData.PackageCode,
                            outFullAddress = sffData.outFullAddress,
                            vatAddress1 = sffData.vatAddress1,
                            vatAddress2 = sffData.vatAddress2,
                            vatAddress3 = sffData.vatAddress3,
                            vatAddress4 = sffData.vatAddress4,
                            vatAddress5 = sffData.vatAddress5,
                            vatPostalCd = sffData.vatPostalCd,
                            vatAddressFull = sffData.vatAddressFull,
                            outServiceLevel = sffData.outServiceLevel,
                            errorMessage = sffData.errorMessage,
                            outAccountCategory = sffData.outAccountCategory,
                            outAddressId = sffData.outAddressId,
                            projectOption = sffData.projectOption,
                            curMobileCheckRight = sffData.curMobileCheckRight,
                            curMobileCheckRightOption = sffData.curMobileCheckRightOption,
                            curMobileGetBenefit = sffData.curMobileGetBenefit,
                            curMobileGetBenefitOption = sffData.curMobileGetBenefitOption,
                            outAccessType = sffData.outAccessType
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCheck3G(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CHECKCOVERAGE_SFFDATA"] as WBBEntity.PanelModels.ExWebServiceModels.evESeServiceQueryMassCommonAccountInfoModel;

            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            outProductName = sffData.outProductName,
                            vataddTripleplay = sffData.vataddTripleplay,
                            outMobileSegment = sffData.outMobileSegment,
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCCCustInfoQuery(string mobileNo = "", string device_type = "", string browser_type = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            string transactionId = "";

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            transactionId = mobileNo + ipAddress;

            #endregion

            var query = new A_GetCCCustInfoQuery
            {
                MOBILE_NO = mobileNo,
                DEVICE_TYPE = device_type,
                BROWSER_TYPE = browser_type,
                TRANSACTION_ID = transactionId,
                FullUrl = FullUrl
            };

            var GetCCCustInfoModel = _queryProcessor.Execute(query);
            if (null != GetCCCustInfoModel)
            {
                if (null != GetCCCustInfoModel.MOBILE_SEGMENT)
                {
                    GetCCCustInfoModel.MOBILE_SEGMENT = GetCCCustInfoModel.MOBILE_SEGMENT.ToUpper();
                }
                GetCCCustInfoModel.GUIDKEY = new Guid().ToSafeString();
                Session["CCCUSTOMERINFO_SFFDATA"] = GetCCCustInfoModel;

                //20.6 Non-Res Register Residential
                string cardTypeFor = "";
                if (GetCCCustInfoModel.SUB_NETWORK_TYPE == "Post-paid")
                {
                    var personalInfoQuery = new evESQueryPersonalInformationQuery()
                    {
                        option = "1",
                        mobileNo = mobileNo,
                        FullUrl = FullUrl
                    };
                    var personalInfoData = _queryProcessor.Execute(personalInfoQuery);
                    if (personalInfoData.Count > 0)
                    {
                        string cardType = personalInfoData[0].idCardType;
                        var lovCardType = base.LovData.Where(p => p.Name == cardType && p.Type == WebConstants.LovConfigName.CardType);
                        LovValueModel model = new LovValueModel();
                        if (lovCardType.Any())
                        {
                            cardTypeFor = lovCardType.FirstOrDefault().LovValue3;
                            Session["CCCUSTOMERINFO_SFFDATA_CARDTYPE"] = personalInfoData[0].idCardType;
                            Session["CCCUSTOMERINFO_SFFDATA_IDCARD"] = personalInfoData[0].idCardNo;
                            GetCCCustInfoModel.CA_ID_CARD_NO = personalInfoData[0].idCardNo;
                            Session["CCCUSTOMERINFO_SFFDATA"] = GetCCCustInfoModel;
                        }
                    }
                }

                return Json(
                    new
                    {
                        data = new
                        {
                            GUIDKEY = WBBSECURE.WBBEncrypt.textEncrpyt(GetCCCustInfoModel.GUIDKEY),
                            STATE = GetCCCustInfoModel.MOBILE_NO_STATUS,
                            TYPE = GetCCCustInfoModel.SUB_NETWORK_TYPE,
                            //CARD_TYPE = cardType,
                            CARD_TYPE_F = cardTypeFor
                        }
                    }, JsonRequestBehavior.AllowGet);
            }

            return Json(
                    new
                    {
                        data = new
                        {
                            GUIDKEY = "",
                        }
                    }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCCCheckProcess(string key)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CCCUSTOMERINFO_SFFDATA"] as A_GetCCCustInfoModel;

            if (sffData.GUIDKEY == decryptedGuidKey)
            {
                return Json(
                    new
                    {
                        data = new
                        {
                            MOBILE_NO_STATUS = sffData.MOBILE_NO_STATUS,
                            MOBILE_SEGMENT = sffData.MOBILE_SEGMENT,
                            NETWORK_TYPE = sffData.NETWORK_TYPE,
                            SUB_NETWORK_TYPE = sffData.SUB_NETWORK_TYPE,
                            ID_CARD_NO = sffData.ID_CARD_NO,
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCCCustInfo(string key, string idCard, bool bypass = false)
        {
            var decryptedGuidKey = WBBSECURE.WBBDecrypt.textDecrpyt(key);

            var sffData =
                Session["CCCUSTOMERINFO_SFFDATA"] as A_GetCCCustInfoModel;

            if (sffData.GUIDKEY == decryptedGuidKey
                && (string.IsNullOrEmpty(idCard)
                    || (!string.IsNullOrEmpty(idCard) && (idCard == sffData.CA_ID_CARD_NO || bypass)))//แก้จาก ID_CARD_NO
                )
            {
                string MOBILE_SEGMENT = "";
                string NETWORK_TYPE = "";
                string DIFF_MONTH = "0";


                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                DateTime REGISTER_DATE = DateTime.Now;
                DateTime NOW_DATE = REGISTER_DATE;
                int DiffMonth = 0;
                if (sffData.MOBILE_SEGMENT != null)
                {
                    MOBILE_SEGMENT = WebSecurity.Encode(sffData.MOBILE_SEGMENT);
                }
                if (sffData.NETWORK_TYPE != null)
                {
                    NETWORK_TYPE = WebSecurity.Encode(sffData.NETWORK_TYPE);
                }
                if (sffData.REGISTER_DATE != null)
                {
                    DateTime.TryParseExact(sffData.REGISTER_DATE,
                            "yyyyMMdd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out REGISTER_DATE);
                    if (NOW_DATE > REGISTER_DATE)
                    {
                        DiffMonth = (NOW_DATE.Year - REGISTER_DATE.Year) * 12;
                        DiffMonth = DiffMonth + (NOW_DATE.Month - REGISTER_DATE.Month);
                    }
                    DIFF_MONTH = WebSecurity.Encode(DiffMonth.ToSafeString());
                }

                return Json(
                    new
                    {
                        data = new
                        {
                            STATE = sffData.MOBILE_NO_STATUS,
                            TYPE = sffData.SUB_NETWORK_TYPE,

                            V1 = MOBILE_SEGMENT,
                            V2 = NETWORK_TYPE,
                            V3 = DIFF_MONTH,
                            V4 = WebSecurity.Encode(bypass ? Session["CCCUSTOMERINFO_SFFDATA_IDCARD"] as string : ""),
                            V5 = WebSecurity.Encode(bypass ? Session["CCCUSTOMERINFO_SFFDATA_CARDTYPE"] as string : "")
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                   new
                   {
                       data = new
                       {
                           errorMessage = "ERROR"
                       }
                   }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMobileDataUsage(string mobileNo = "", string networkType = "")
        {
            MobileDataUsageModel DataUsageModel;
            var query = new GetMobileDataUsageQuery
            {
                MOBILE_NO = mobileNo,
                networkType = networkType
            };
            DataUsageModel = _queryProcessor.Execute(query);

            return Json(DataUsageModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOwnerByBuilding(string langFlag = "", string building = "")
        {
            var query = new GetOwnerByBuildingQuery
            {
                LanguageFlag = langFlag,
                Building = building
            };

            var a = _queryProcessor.Execute(query);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerSpeOffer(string orderRef = "", string technology = "", bool isNonMobile = false, decimal sffLogId = 0,
            string packageGroup = "", bool isAWNProduct = false)
        {
            var query = new GetCustomerSpeOfferQuery
            {
                ReferenceID = orderRef,
                //Technology = technology,
                IsNonMobile = isNonMobile,
                //SffChkProfLogID = sffLogId,
                PackageGroup = packageGroup,
                IsAWNProduct = isAWNProduct
            };

            var a = _queryProcessor.Execute(query);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFBSSFeasibilityCheck(
            string addrType = "",
            string postCode = "",
            string lang = "",
            string subDisName = "",
            string buildName = "",
            string buildNo = "",
            string phoneFlag = "",
            string latitude = "",
            string longitude = "",
            string unitNo = "",
            string floor = "",
            string aisAirNumber = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = aisAirNumber + ipAddress;

            #endregion

            var query = new GetFBSSFeasibilityCheck
            {
                AddressType = addrType,
                PostalCode = postCode,
                Language = lang,
                SubDistricName = subDisName,
                BuildingName = buildName,
                BuildingNo = buildNo,
                PhoneFlag = phoneFlag,
                Latitude = latitude,
                Longitude = longitude,
                UnitNo = unitNo,
                FloorNo = floor,
                TransactionId = transactionId,
                FullUrl = FullUrl
            };

            var coverageResult = _queryProcessor.Execute(query);

            var jsonResult = new JsonNetResult();
            jsonResult.Formatting = Formatting.Indented;
            jsonResult.Data = coverageResult;

            return jsonResult;
        }

        public void LoadBuildingChange()
        {
            HttpRuntime.Cache.Insert("ReLoadBuilding", "Y", null, DateTime.UtcNow.AddHours(6), Cache.NoSlidingExpiration);
        }

        public ActionResult GetBuildingChange(string province, string aumphur, string tumbon, string type, string language, string loc_code, string accessMode = "")
        {
            string CacheName = "BuildingData" + type + accessMode + loc_code;
            string ReLoadBuilding = "N";
            List<DropdownModel> a = new List<DropdownModel>();

            try
            {
                var query = new GetbuildingChangeQuery
                {
                    Province = "",
                    Aumphur = "",
                    Tumbon = "",
                    Typeaddress = type,
                    Language = language,
                    AccessMode = accessMode,
                    LOC_CODE = loc_code,
                    ReloadCache = "N"
                };

                if (null == HttpRuntime.Cache[CacheName] || null == HttpRuntime.Cache["ReLoadBuilding"])
                {
                    a = _queryProcessor.Execute(query).DistinctBy(t => t.Value4).ToList();
                    if (a != null && a.Count > 0)
                    {
                        if (language == "T")
                            a.Add(new DropdownModel { Text = "อื่น ๆ ระบุ", Value = "0" });
                        else
                            a.Add(new DropdownModel { Text = "Other", Value = "0" });

                        HttpRuntime.Cache.Insert(CacheName, a, null, DateTime.UtcNow.AddHours(6), Cache.NoSlidingExpiration);
                        HttpRuntime.Cache.Insert("ReLoadBuilding", "N", null, DateTime.UtcNow.AddHours(6), Cache.NoSlidingExpiration);
                    }
                }
                else
                {
                    ReLoadBuilding = HttpRuntime.Cache.Get("ReLoadBuilding") as string;
                    if (ReLoadBuilding == "Y")
                    {
                        query.ReloadCache = "Y";
                        a = _queryProcessor.Execute(query).DistinctBy(t => t.Value4).ToList();
                        if (a != null && a.Count > 0)
                        {
                            if (language == "T")
                                a.Add(new DropdownModel { Text = "อื่น ๆ ระบุ", Value = "0" });
                            else
                                a.Add(new DropdownModel { Text = "Other", Value = "0" });
                            HttpRuntime.Cache.Insert(CacheName, a, null, DateTime.UtcNow.AddHours(6), Cache.NoSlidingExpiration);
                            HttpRuntime.Cache.Insert("ReLoadBuilding", "N", null, DateTime.UtcNow.AddHours(6), Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        a = HttpRuntime.Cache.Get(CacheName) as List<DropdownModel>;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Error Call CheckCoverage/GetBuildingChange : " + "[Error Message : " + ex.Message + "][Error StackTrace : " + ex.StackTrace + "][Error InnerException : " + ex.InnerException + "]");
            }
            
            var jsonResult = new JsonNetResult();
            jsonResult.Formatting = Formatting.Indented;
            jsonResult.Data = a;

            return jsonResult;
        }

        public ActionResult GetBuildingAll(string type, string language)
        {
            var query = new GetbuildingAllQuery
            {
                Typeaddress = type,
                Language = language
            };

            var a = _queryProcessor.Execute(query).DistinctBy(t => t.Value4).ToList();

            if (language == "T")
                a.Add(new DropdownModel { Text = "อื่น ๆ ระบุ", Value = "0" });
            else
                a.Add(new DropdownModel { Text = "Other", Value = "0" });

            var jsonResult = new JsonNetResult();
            jsonResult.Formatting = Formatting.Indented;
            jsonResult.Data = a;

            return jsonResult;
        }

        public JsonResult GetBuildingNo(string postcode, string build_name, string language)
        {
            var query = new GetBuildingNoQuery
            {
                Buildname = build_name,
                Postcode = "",
                Language = language,
            };
            var a = _queryProcessor.Execute(query);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBuildingByBuildingNameAndNo(string build_name, string build_no)
        {
            var query = new GetBuildingByBuildingNameAndNoQuery
            {
                Buildname = build_name,
                Buildno = build_no
            };
            var a = _queryProcessor.Execute(query);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPrePostPaid(string msisdn, string orderRef, string cond = "")
        {
            bool chkPostPaid = false;
            string userName = "CUSTOMER";

            if (base.CurrentUser != null)
                userName = base.CurrentUser.UserName.ToSafeString();

            string refNo = orderRef;

            try
            {
                var query = new GetAisMobileServiceQuery()
                {
                    Msisdn = msisdn.ToSafeString(),
                    Opt1 = "",
                    Opt2 = "",
                    OrderDesc = "query sub",
                    OrderRef = refNo,
                    User = userName,
                    UserName = "FBB"
                };

                var result = _queryProcessor.Execute(query);

                //if (cond == "dbspeed")
                //{
                //    return Json(refNo, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                if (result.IsSuccess != null && result.Chm != null && result.State != null)
                {
                    if (result.IsSuccess.ToUpper() == "TRUE" && result.Chm == "0" && result.State == "1")
                        chkPostPaid = true;
                    else if (result.IsSuccess.ToUpper() == "TRUE" && result.Chm == "1" && result.State == "1")
                        chkPostPaid = false;
                }
                // }
            }
            catch (Exception ex)
            {
                Logger.Error("Error when call CheckPrePostPaid in CheckCoverageController : " + ex.InnerException);
            }

            return Json(chkPostPaid, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPrePostPaids(string msisdn, string orderRef, string cond = "")
        {
            string userName = "CUSTOMER";

            if (base.CurrentUser != null)
                userName = base.CurrentUser.UserName.ToSafeString();

            string refNo = orderRef;

            //try
            //{
            var query = new GetAisMobileServiceQuery()
            {
                Msisdn = msisdn.ToSafeString(),
                Opt1 = "",
                Opt2 = "",
                OrderDesc = "query sub",
                OrderRef = refNo,
                User = userName,
                UserName = "FBB"
            };

            var results = _queryProcessor.Execute(query);
            return Json(results, JsonRequestBehavior.AllowGet);

            //}
            //catch (Exception ex)
            //{
            //    Logger.Error("Error when call CheckPrePostPaid in CheckCoverageController : " + ex.InnerException);
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetP_Region(string rowid)
        {
            var query = new SelectSubRegionQuery()
            {
                rowid = rowid,
                currentculture = SiteSession.CurrentUICulture
            };

            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult Get_Region(string rowid)
        {
            var result = base.ZipCodeData(SiteSession.CurrentUICulture).SingleOrDefault(o => o.ZipCodeId == rowid);
            if (result != null)
            {
                return Json(result.RegionCode, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_ImagePOI(string LIST_COVERAGE_PICTURE)
        {
            CoveragePictureModel model = new JavaScriptSerializer().Deserialize<CoveragePictureModel>(LIST_COVERAGE_PICTURE);
            model.user = "FBBWEB";

            var query = new GetImagePOIQuery
            {
                model = model
            };

            ImageResultModel a = _queryProcessor.Execute(query);

            foreach (var temp in a.PicList)
            {
                temp.PICTURE_PATH = temp.PICTURE_PATH.Substring(temp.PICTURE_PATH.IndexOf("Coverage_Picture/") + "Coverage_Picture/".Length, (temp.PICTURE_PATH.Length) - (temp.PICTURE_PATH.IndexOf("Coverage_Picture/") + "Coverage_Picture/".Length));
            }
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAppointmentPlugAndPlay(string installation_Date, string access_Mode = "", string address_Id = "",
            string serviceCode = "", string district = "", string subDistrict = "", string province = "", string postalCode = "", int lineSelect = 1,
            long days = 0, string productSpecCode = "",
            bool isThai = true, string timeSlotId = "", bool smallSize = false, string AisAirNumber = "", string SubAccessMode = ""
            )
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var installationDate = new DateTime();
            var installationTime = Convert.ToInt32(DateTime.Now.Hour) * 60 + Convert.ToInt32(DateTime.Now.Minute);
            var strFBSSTimeSlot = "";

            try
            {
                #region Get IP Address Interface Log (Update 17.2)

                string transactionId = "";

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                transactionId = AisAirNumber + ipAddress;

                #endregion

                if (isThai)
                {
                    DateTimeFormatInfo thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, thDtfi);
                }
                else
                {
                    DateTimeFormatInfo usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, usDtfi);
                }

                /// check access_Mode for set FBSSTimeSlot

                // transform to null safe string commpare.
                var LovConstant = (from t in base.LovData
                                   where t.Type == "FBB_CONSTANT"
                                   && t.Name == "DEFAULT_TIMESLOT"
                                   && t.Text == access_Mode
                                   select t.LovValue1).ToList();

                if (LovConstant != null && LovConstant.Count > 0)
                {
                    strFBSSTimeSlot = LovConstant.FirstOrDefault().ToString();
                }

                /// End
                /// 
                var DayPlugandPlay = (from t in base.LovData
                                      where t.Type == "FBB_CONSTANT"
                                      && t.Name == "DAY_OF_PLUG_AND_PLAY"
                                      select t).ToList();

                days = long.Parse(DayPlugandPlay[0].LovValue1);
                var additiontime = DayPlugandPlay[0].LovValue2;

                installationTime = installationTime + Convert.ToInt32(additiontime);

                if (access_Mode == "XDSL")
                    access_Mode = "VDSL";
                if (serviceCode == "XDSL")
                    serviceCode = "VDSL";

                var query = new GetFBSSAppointment()
                {
                    AccessMode = access_Mode,
                    AddressId = address_Id,
                    Days = days,
                    ExtendingAttributes = "",
                    InstallationDate = installationDate.ToString("yyyy-MM-dd"),
                    ProductSpecCode = productSpecCode,
                    District = district,
                    Language = isThai ? "T" : "E",
                    Postal_Code = postalCode,
                    Province = province,
                    Service_Code = serviceCode,
                    SubDistrict = subDistrict,
                    LineSelect = (LineType)lineSelect,
                    Transaction_Id = transactionId,
                    SubAccessMode = SubAccessMode,
                    TaskType = "",
                    FullUrl = FullUrl
                };

                var data = _queryProcessor.Execute(query);

                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        var tempdata = data;
                        for (int i = 0; i < tempdata.Count; i++)
                        {
                            var capacity = tempdata[i].InstallationCapacity.Split('/')[0].ToSafeInteger();
                            var limitDate = tempdata[i].AppointmentDate.Value.Day + "/" + tempdata[i].AppointmentDate.Value.Month + "/" + (tempdata[i].AppointmentDate.Value.Year);
                            var limitTime = tempdata[i].TimeSlot.Split(':')[0].ToSafeInteger();
                            limitTime = limitTime * 60;

                            var installdate = installationDate.Day + "/" + installationDate.Month + "/" + installationDate.Year;
                            if (tempdata[i].TimeSlot == "08:00-10:00" || capacity <= 0 || (limitDate == installdate && installationTime > limitTime))
                            {
                                tempdata.RemoveAt(i--);
                            }
                        }

                        if (tempdata.Any())
                            return Json(
                                new
                                {
                                    AppointmentDate = tempdata[0],
                                    isPlugandPlay = true,
                                }
                                , JsonRequestBehavior.AllowGet);
                        else
                            return Json(
                                new
                                {
                                    AppointmentDate = "",
                                    isPlugandPlay = false,
                                }
                                , JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(
                                new
                                {
                                    AppointmentDate = "",
                                    isPlugandPlay = false,
                                }
                                , JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(
                                new
                                {
                                    AppointmentDate = "",
                                    isPlugandPlay = false,
                                }
                                , JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                base.Logger.Info(ex.RenderExceptionMessage());
                return Json(ex.Message);
            }


        }

        public WTTXInfoModel GetWTTXInfo(string gridId, string transactionId)
        {
            GetWTTXInfoQuery query = new GetWTTXInfoQuery()
            {
                grid_id = gridId,
                transaction_id = transactionId
            };
            WTTXInfoModel result = _queryProcessor.Execute(query);

            return result;
        }

        [HttpPost]
        public JsonResult GetCheckMobileOnlineQuery(string mobileNo, string fibreID, string locationCode)
        {
            string SubNetworkType = "";
            int DiffMonth = 0;
            string DiffMonthSTR = "0";
            OnlineQueryPersonalInfoResult PersonalInfo = new OnlineQueryPersonalInfoResult();
            List<OnlineQuerySpecialOfferResult> SpecialOffer = new List<OnlineQuerySpecialOfferResult>();
            OnlineQueryFibrenetInfoResult FibrenetInfo = new OnlineQueryFibrenetInfoResult();

            // Check session OfficerModel lost
            string ReloadToLogin = "N";
            if (locationCode != "")
            {
                if (null == Session["OfficerModel"])
                {
                    ReloadToLogin = "Y";
                }
            }

            if (ReloadToLogin == "N")
            {
                GetOnlineQueryMobileInfoModel mobileData = GetOnlineQueryMobileInfo(mobileNo, fibreID, locationCode);
                if (mobileData != null && mobileData.RESULT_CODE == "20000" && mobileData.PERSONAL_INFO != null)
                {
                    PersonalInfo = mobileData.PERSONAL_INFO;
                    SpecialOffer = mobileData.LIST_SPECIAL_OFFER;
                    PersonalInfo.ID_CARD_NO = Encrypt(mobileData.PERSONAL_INFO.ID_CARD_NO);
                    SubNetworkType = PersonalInfo.SUB_NETWORK_TYPE;
                    FibrenetInfo = mobileData.FIBRENET_INFO;//R21.10 MOU
                    FibrenetInfo.ID_CARD_NO = Encrypt(mobileData.FIBRENET_INFO.ID_CARD_NO);//R21.10 MOU

                    if (PersonalInfo.REGISTER_DATE != null && PersonalInfo.REGISTER_DATE.Length >= 10)
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                        DateTime NOW_DATE = DateTime.Now;
                        DateTime REGISTER_DATE = NOW_DATE;
                        string tmpRegisterDate = PersonalInfo.REGISTER_DATE;
                        tmpRegisterDate = tmpRegisterDate.Substring(0, 10); //14/01/2019
                        DateTime.TryParseExact(tmpRegisterDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out REGISTER_DATE);
                        if (NOW_DATE > REGISTER_DATE)
                        {
                            DiffMonth = (NOW_DATE.Year - REGISTER_DATE.Year) * 12;
                            DiffMonth = DiffMonth + (NOW_DATE.Month - REGISTER_DATE.Month);
                            DiffMonthSTR = DiffMonth.ToSafeString();
                        }
                    }

                    //R22.07
                    if (!string.IsNullOrEmpty(PersonalInfo.MOBILE_SEGMENT))
                    {
                        PersonalInfo.MOBILE_SEGMENT = PersonalInfo.MOBILE_SEGMENT.ToUpper();
                    }
                }
            }

            return Json(
                    new
                    {
                        PersonalInfo = PersonalInfo,
                        SpecialOffer = SpecialOffer,
                        SubNetworkType = SubNetworkType,
                        Diffmonth = DiffMonthSTR,
                        FibrenetInfo = FibrenetInfo,
                        ReloadToLogin = ReloadToLogin
                    }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCheckIDCardOnlineQuery(string mobileNo, string tmpDataID, string tmpDataIDType, string idCard, string idCardType, string SubNetworkType,
            bool StatusNonRes = false, string tmpFibrenetInfoDataID = "", string tmpFibrenetInfoDataIDType = "")
        {
            string checkID = "N";
            string tmpEnDataID = Decrypt(tmpDataID);

            string checkFibrenetID = "N";
            string tmpEnFibrenetInfoDataID = Decrypt(tmpFibrenetInfoDataID);

            if (SubNetworkType == "POSTPAID")
            {
                if (tmpEnDataID == idCard && tmpDataIDType == idCardType)
                {
                    checkID = "Y";
                }

                //R21.10 MOU
                if (tmpEnFibrenetInfoDataID == idCard && tmpFibrenetInfoDataIDType == idCardType)
                {
                    checkFibrenetID = "Y";
                }

            }
            else if (SubNetworkType == "PREPAID")
            {
                if (tmpEnDataID != "" && tmpDataIDType != "")
                {
                    checkID = "Y";
                }

                //R21.04 Issue MOU
                if (tmpEnFibrenetInfoDataID == idCard && tmpFibrenetInfoDataIDType == idCardType)
                {
                    checkFibrenetID = "Y";
                }
            }
            else
            {
                //R21.10 MOU
                if (tmpEnFibrenetInfoDataID == idCard && tmpFibrenetInfoDataIDType == idCardType)
                {
                    checkFibrenetID = "Y";
                }
            }

            if (StatusNonRes)
            {
                checkID = "Y";
                tmpEnDataID = idCard;
                tmpDataIDType = idCardType;
                checkFibrenetID = "Y";//R21.10
            }

            if (checkID == "Y" || checkFibrenetID == "Y")
            {
                tmpEnDataID = idCard;
                tmpDataIDType = idCardType;
            }

            return Json(
                    new
                    {
                        data = new
                        {
                            V1 = checkID,
                            V2 = WebSecurity.Encode(tmpEnDataID),
                            V3 = WebSecurity.Encode(tmpDataIDType),
                            V4 = checkFibrenetID
                        }
                    }, JsonRequestBehavior.AllowGet);
        }

        public GetOnlineQueryMobileInfoModel GetOnlineQueryMobileInfo(string MobileNo, string FibreID, string LocationCode)
        {
            GetOnlineQueryMobileInfoModel result = new GetOnlineQueryMobileInfoModel();
            GetOnlineQueryMobileInfoQuery reqOnline = new GetOnlineQueryMobileInfoQuery
            {
                FullUrl = "",
                Transaction_Id = MobileNo,
                Internet_No = MobileNo,
                Body = new OnlineQueryMobileInfoBody
                {
                    MOBILE_NO = MobileNo,
                    FIBRE_ID = FibreID,
                    ID_CARD = "",
                    ID_CARD_TYPE = "",
                    LOCATION_CODE = LocationCode
                }
            };

            result = _queryProcessor.Execute(reqOnline);

            if (result != null && result.PERSONAL_INFO != null && result.PERSONAL_INFO.ID_CARD_TYPE != null)
            {
                var lovCardType = base.LovData.Where(p => p.Name == result.PERSONAL_INFO.ID_CARD_TYPE && p.Type == WebConstants.LovConfigName.CardType);
                if (lovCardType != null && lovCardType.Count() > 0)
                {
                    result.PERSONAL_INFO.ID_CARD_TYPE_FOR = lovCardType.FirstOrDefault().LovValue3.ToSafeString();
                }
            }

            return result;
        }

        [HttpGet]
        public JsonResult GetcheckCoverageFTTR3BB(CheckCoverageSpecialRequest request)
        {
            string splitterJsonData = null;
            try
            {
                CheckCoverageSpecialResponse response = new CheckCoverageSpecialResponse();
                response.returnCode = "00009";
                response.returnMessage = "Call WBBService Failed.";
                var SpliterCheck = 0;
                response.splitterJson = null;

                CheckCoverage3bbQuery checkCoverage3bbQuery = new CheckCoverage3bbQuery
                {
                    latitude = request.latitude,
                    longitude = request.longitude,
                    TRANSACTION_ID = request.mobileno,
                };

                CheckCoverage3bbQueryModel queryData = _queryProcessor.Execute(checkCoverage3bbQuery);
                if (queryData != null && queryData.coverage == "ON_SERVICE")
                {
                    response.returnCode = queryData.returnCode.ToSafeString();
                    response.returnMessage = queryData.returnMessage.ToSafeString();
                    response.coverage = queryData.coverage.ToSafeString();
                    response.inServiceDate = queryData.inServiceDate.ToSafeString();
                    response.splitterCount = 0;
                    if (queryData.splitterList != null && queryData.splitterList.Count() > 0)
                    {
                        response.splitterCount = queryData.splitterList.Count();
                        var splitterList = queryData.splitterList.Select(t => new Models.CheckCoverage3bbSplitter
                        {
                            splitterCode = t.splitterCode.ToSafeString(),
                            splitterAlias = t.splitterAlias.ToSafeString(),
                            distance = t.distance.ToSafeString(),
                            splitterPort = t.splitterPort.ToSafeString(),
                            splitterLatitude = t.splitterLatitude.ToSafeString(),
                            splitterLongitude = t.splitterLongitude.ToSafeString(),

                        }).ToList();

                        //checkMostlyNearbyDistance
                        decimal LowestDistance = 0;
                        int lowestIndex = 0;
                        for (int i = 0; i < splitterList.Count; i++)
                        {
                            if (i == 0)
                            {
                                LowestDistance = splitterList[i].distance.ToSafeDecimal();
                            }
                            if (splitterList[i].distance.ToSafeDecimal() < LowestDistance)
                            {
                                LowestDistance = splitterList[i].distance.ToSafeDecimal();
                                lowestIndex = i;
                            }
                        }

                        var LowestDistancesplitterList = queryData.splitterList.Select(t => new Models.CheckCoverage3bbSplitter
                        {
                            splitterCode = t.splitterCode.ToSafeString(),
                            splitterAlias = t.splitterAlias.ToSafeString(),
                            distance = t.distance.ToSafeString(),
                            splitterPort = t.splitterPort.ToSafeString(),
                            splitterLatitude = t.splitterLatitude.ToSafeString(),
                            splitterLongitude = t.splitterLongitude.ToSafeString()
                        }).Where(t => t.distance == LowestDistance.ToSafeString()).ToList();

                        splitterJsonData = new JavaScriptSerializer().Serialize(LowestDistancesplitterList);
                        var lovConfig = _queryProcessor.Execute(new GetLovQuery
                        {
                            LovType = "FBB_CONSTANT",
                            LovName = "MY_AIS_KEY"
                        });
                        if (lovConfig != null && lovConfig.Count > 0)
                        {
                            string tmpK = lovConfig.FirstOrDefault().LovValue1.ToSafeString();
                            response.splitterJson = Encrypt(splitterJsonData, tmpK);
                        }

                    }

                }
                return Json(response.splitterJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                base.Logger.Info(ex.RenderExceptionMessage());
                return Json(ex.Message);
            }
        }
        //23.06 TOPUPIPCAMERA
        public List<evESQueryPersonalInformationModel> evESQueryPersonalInformationIPcamera(string mobileNo, string option)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var query = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = mobileNo,
                    option = option,
                    FullUrl = FullUrl
                };
                result = _queryProcessor.Execute(query);
            }
            catch { }

            return result;
        }
        [HttpPost]
        public ActionResult getAddressCustomer(string mobileNo, string option)
        {
            var data = evESQueryPersonalInformationIPcamera(mobileNo, option);
            var result = data.FirstOrDefault();
            return Json(
                new
                {
                    BuildingName = result.buildingName,
                    Amphur = result.amphur,
                    ProvinceName = result.provinceName,
                    Title = result.title,
                    Tumbol = result.tumbol,
                    ZipCode = result.zipCode,
                    FirstName = result.firstName,
                    LastName = result.lastName,
                    Moo = result.moo,
                    StreetName = result.streetName,
                    HouseNo = result.houseNo,
                    Mooban = result.mooban,
                    Floor = result.floor,
                    Soi = result.soi,
                    Room = result.room,
                }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult getPhoneNummberCustomer(string mobileNo = "", string cardNo = "", string cardType = "", string line = "", string SubNetworkType = "", string page = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            string user = "Customer";
            var query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "4",
                inMobileNo = mobileNo,
                inCardNo = cardNo,
                inCardType = cardType,
                Page = "IP_CAMERA",
                Username = user,
                FullUrl = FullUrl
            };
            var result = _queryProcessor.Execute(query);
            return Json(
                new
                {
                    ServiceMobileNo = result.outServiceMobileNo,
                }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetDeliveryIPCamera(string p_province = "")
        {
            var query = new GetDeliveryIPCameraQuery
            {
                province = p_province
            };
            var result = _queryProcessor.Execute(query);
            return Json(
                new
                {
                    ServiceMobileNo = result.RETURN_DELIVERY_CURROR.FirstOrDefault(),
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetContractProfileCountFbb(string mobileNo) => Json(evESQueryPersonalInformationIPcamera(mobileNo, "2").Select(s => s.productPkg).Where(w => w == "AIS Cloud IP Camera Fee").Count(), JsonRequestBehavior.AllowGet);

        public JsonResult GetListServiceAndPromotionByPackageType(string NonMobileNo, string IdCard)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var data = new evOMQueryListServiceAndPromotionByPackageTypeModel();
            var installAddress1 = new string[] { };
            try
            {
                var query2 = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                {
                    mobileNo = NonMobileNo,
                    idCard = IdCard,
                    FullUrl = FullUrl
                };

                data = _queryProcessor.Execute(query2);

                installAddress1 = data.v_installAddress1.Split(new char[] { ' ' }, 2);

            }
            catch { }

            return Json(
                new
                {
                    FullName = data.contactName,
                    InstallAddress = data.v_installAddress,
                    HouseNo = installAddress1[0],
                    BuildingName = installAddress1[1],
                    Tumbol = data.v_installAddress2,
                    Amphur = data.v_installAddress3,
                    ProvinceName = data.v_installAddress4,
                    ZipCode = data.v_installAddress5,
                }
        , JsonRequestBehavior.AllowGet); ;
        }
    }
}