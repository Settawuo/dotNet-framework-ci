using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Commands.FBSS;
using WBBContract.Commands.WebServices;
using WBBContract.Commands.WebServices.FBSS;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.FBBShareplex;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBSS;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBEntity.Extensions;
using WBBEntity.FBBShareplexModels;
using WBBEntity.FBSSModels;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.ShareplexModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBExternalService.Customization;
using WBBExternalService.Models;
using WBBExternalService.Models.Request;
using WBBExternalService.Models.Response;

namespace WBBExternalService
{
    public class FbbCpGwInterface : IFbbCpGwInterface
    {
        private ILogger _logger;
        private IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CoverageResultCommand> _covResultCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<WBBContract.Commands.ExWebServices.FbbCpGw.CustRegisterCommand> _custRegCommand;
        private readonly ICommandHandler<AisWiFiRegCommand> _aisWiFiRegCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly ICommandHandler<GeneratePdfCommand> _genPdfCommand;
        private readonly ICommandHandler<NotificationCommand> _notiCommand;
        private readonly ICommandHandler<FBSSCoverageResultCommand> _fbssCoverageCommand;
        private readonly ICommandHandler<ReserveTimeSlotCommand> _reserveTimeSlotCommand;
        private readonly ICommandHandler<SaveLeavemessageCommand> _saveLeavemessageCommand;
        private readonly ICommandHandler<SetCSNoteCommand> _setCSNoteCommand;
        private readonly ICommandHandler<SetCustomerVerificationCommand> _setCustomerVerificationCommand;
        private readonly ICommandHandler<SavePendingDeductionCommand> _savePendingDeductionCommand;
        private readonly ICommandHandler<MicrositeWSCommand> _micrositeWSCommand;
        private readonly ICommandHandler<MicrositeActionCommand> _micrositeActionCommand;
        private readonly ICommandHandler<PermissionUserCommand> _permissionuserCommand;
        private readonly ICommandHandler<InsertCoverageRusultCommand> _insertCoverageRusultCommand;
        private readonly ICommandHandler<UpdateCoverageRusultCommand> _updateCoverageRusultCommand;
        private readonly ICommandHandler<TransferFileToStorageCommand> _transferFileToStorageCommand;
        private readonly ICommandHandler<SendMailFTTHNotificationCommand> _sendMail;
        private readonly List<string> _listOfAcceptedLangCode = new List<string> { "THA", "ENG" };
        private readonly string _installationDateFormat = "yyyy-MM-dd";
        private readonly string _reserveTimeSlotDateFormat = "yyyy-MM-dd HH:mm:ss";


        public FbbCpGwInterface(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<CoverageResultCommand> covResultCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<WBBContract.Commands.ExWebServices.FbbCpGw.CustRegisterCommand> custRegCommand,
            ICommandHandler<AisWiFiRegCommand> aisWiFiRegCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ICommandHandler<GeneratePdfCommand> genPdfCommand,
            ICommandHandler<NotificationCommand> notiCommand,
            ICommandHandler<FBSSCoverageResultCommand> fbssCoverageCommand,
            ICommandHandler<ReserveTimeSlotCommand> reserveTimeSlotCommand,
            ICommandHandler<SaveLeavemessageCommand> saveLeavemessageCommand,
            ICommandHandler<SetCSNoteCommand> setCSNoteCommand,
            ICommandHandler<SavePendingDeductionCommand> savePendingDeductionCommand,
            ICommandHandler<SetCustomerVerificationCommand> setCustomerVerificationCommand,
            ICommandHandler<MicrositeWSCommand> micrositeWSCommand,
            ICommandHandler<MicrositeActionCommand> micrositeActionCommand,
            ICommandHandler<PermissionUserCommand> permissionuserCommand,
            ICommandHandler<InsertCoverageRusultCommand> insertCoverageRusultCommand,
            ICommandHandler<UpdateCoverageRusultCommand> updateCoverageRusultCommand,
            ICommandHandler<TransferFileToStorageCommand> transferFileToStorageCommand,
            ICommandHandler<SendMailFTTHNotificationCommand> sendMail)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _covResultCommand = covResultCommand;
            _intfLogCommand = intfLogCommand;
            _custRegCommand = custRegCommand;
            _aisWiFiRegCommand = aisWiFiRegCommand;
            _mailLogCommand = mailLogCommand;
            _genPdfCommand = genPdfCommand;
            _notiCommand = notiCommand;
            _fbssCoverageCommand = fbssCoverageCommand;
            _reserveTimeSlotCommand = reserveTimeSlotCommand;
            _saveLeavemessageCommand = saveLeavemessageCommand;
            _setCSNoteCommand = setCSNoteCommand;
            _setCustomerVerificationCommand = setCustomerVerificationCommand;
            _savePendingDeductionCommand = savePendingDeductionCommand;
            _micrositeWSCommand = micrositeWSCommand;
            _micrositeActionCommand = micrositeActionCommand;
            _permissionuserCommand = permissionuserCommand;
            _insertCoverageRusultCommand = insertCoverageRusultCommand;
            _updateCoverageRusultCommand = updateCoverageRusultCommand;
            _transferFileToStorageCommand = transferFileToStorageCommand;
            _sendMail = sendMail;
        }

        #region Check
        public CheckSFFProfileResponse CheckSFFProfile(GetMassCommonAccountQuery query)
        {
            query.inOption = "1";
            //query.inCardType = "ID_CARD";
            query.Page = "Check Coverage";
            query.Username = "FbbCpGwInterface";

            InterfaceLogCommand log = null;

            var result = new evESeServiceQueryMassCommonAccountInfoModel();

            try
            {
                log = StartInterface<GetMassCommonAccountQuery>(query, "CheckSFFProfile",
                    query.inMobileNo, query.inCardNo);

                query.ReferenceID = query.TransactionID;

                result = _queryProcessor.Execute(query);

                // ถ้าเป็นเบอร์ mobile (มีเลข 0 นำหน้า)ให้เรียก vsmp ด้วย
                if (query.inMobileNo.First() == '0')
                {
                    var vsmpQuery = new GetAisMobileServiceQuery
                    {
                        TransactionId = query.TransactionID,
                        Msisdn = query.inMobileNo.ToSafeString(),
                        Opt1 = "",
                        Opt2 = "",
                        OrderDesc = "query sub",
                        OrderRef = query.TransactionID,
                        User = "CPGWPlus",
                        UserName = "FBBMOB"
                    };

                    // ไม่ต้อง return
                    _queryProcessor.Execute(vsmpQuery);
                }

                var extractedSffBuilding = ExtractSffBuilding(result.outBuildingName);
                var success = new CheckSFFProfileResponse()
                {
                    Result = (string.IsNullOrEmpty(result.errorMessage) ? Constants.Result.Success : Constants.Result.Failed),

                    Province = result.outProvince,
                    Amphur = result.outAmphur,
                    Tumbol = result.outTumbol,
                    PostalCode = result.outPostalCode,
                    HouseNumber = result.outHouseNumber,
                    Mooban = result.outMooban,
                    Moo = result.outMoo,
                    Soi = result.outSoi,
                    StreetName = result.outStreetName,

                    BuildingName = extractedSffBuilding.BuildingName,
                    Tower = extractedSffBuilding.TowerName,

                    Floor = result.outFloor,
                    PrimaryContactFirstName = result.outPrimaryContactFirstName,
                    ContactLastName = result.outContactLastName,
                    BirthDate = result.outBirthDate,
                    Email = result.outEmail,

                    ErrorCode = (string.IsNullOrEmpty(result.errorMessage) ? "" : Constants.ErrorCode.External),
                    ErrorReason = (string.IsNullOrEmpty(result.errorMessage) ? "" : Constants.CheckSFFInternetProfileErrorMessage.ProfileNotFound(isLocalLanguage: true)),
                };

                EndInterface<CheckSFFProfileResponse>(success, log, query.TransactionID,
                        success.Result, success.ErrorReason);

                return success;
            }
            catch (System.Exception ex)
            {
                var error = new CheckSFFProfileResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<CheckSFFProfileResponse>(error, log, query.TransactionID,
                    error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public CheckCoverageResponse CheckCoverage(GetCoverageResultQuery query)
        {
            InterfaceLogCommand log = null;
            var coverageResult = false;
            var packageList = new List<PackageGroupModel>();
            try
            {
                log = StartInterface<GetCoverageResultQuery>(query, "CheckCoverage",
                    query.TransactionID, query.IDCardNo);

                var isLocalLanguage = query.Language.ToCultureCode().IsThaiCulture();

                // หา owner product
                var ownerProduct = _queryProcessor.Execute(query);

                // บันทึก log coverage result
                if (!string.IsNullOrEmpty(ownerProduct[0].ToSafeString()))
                    coverageResult = true;

                if (query.BuildingType != "CONDOMINIUM")
                    query.Floor = "1";

                // insert transaction id ด้วย
                var command = new CoverageResultCommand
                {
                    TRANSACTION_ID = query.TransactionID,
                    CVRID = ownerProduct[1].ToSafeDecimal(),
                    NODENAME = query.BuildingName,
                    TOWER = query.Tower,
                    FLOOR = query.Floor.ToSafeDecimal(),
                    ISONLINENUMBER = query.OnlineNumberFlag,
                    ADDRESS_NO = query.HouseNo,
                    MOO = query.Moo.ToSafeDecimal(),
                    SOI = query.Soi,
                    ROAD = query.Road,
                    COVERAGETYPE = query.BuildingType,
                    COVERAGERESULT = coverageResult.ToYesNoFlgString(),
                    LATITUDE = ownerProduct[3].ToSafeString(),
                    LONGITUDE = ownerProduct[4].ToSafeString(),
                    PRODUCTTYPE = ownerProduct[0].ToSafeString(),
                    ZIPCODE_ROWID = ownerProduct[2].ToSafeString(),
                    OWNER = ownerProduct[0].ToSafeString(),
                    ActionType = ActionType.Insert,

                    ActionBy = "MOBILE CUSTOMER",
                };

                _covResultCommand.Handle(command);


                if (!string.IsNullOrEmpty(ownerProduct[0]))
                {
                    foreach (var owp in ownerProduct[0].Split(','))
                    {
                        // ย้ายไปเนื่องจากเปลี่ยน technology เป็น product subtype
                        //var acquriedOfferQuery = new GetCustomerSpeOfferQuery
                        //{
                        //    ReferenceID = query.TransactionID,
                        //    Technology = owp,
                        //};

                        //var acqOffers = _queryProcessor.Execute(acquriedOfferQuery);

                        var listPackageQuery = new WBBContract.Queries.ExWebServices.FbbCpGw.GetListPackageByServiceQuery
                        {
                            TransactionID = query.TransactionID,
                            Language = query.Language,
                            //AcquiredOffers = acqOffers,
                            P_NETWORK_TYPE = "",
                            P_OWNER_PRODUCT = owp,
                            P_PACKAGE_CODE = "",
                            P_PACKAGE_FOR = "PUBLIC",
                            P_PRODUCT_SUBTYPE = "",
                            P_SERVICE_DAY = "",
                        };

                        //var listPacksLog = StartInterface<GetPackageListByServiceQuery>(listPackageQuery, "CheckCoverage",
                        //    query.TransactionID, query.IDCardNo);

                        packageList.AddRange(_queryProcessor.Execute(listPackageQuery));

                        //EndInterface<string>("", listPacksLog, query.TransactionID,
                        //    "Success", "");
                    }
                }

                var success = new CheckCoverageResponse
                {
                    Result = coverageResult ? Constants.Result.Success : Constants.Result.Failed,
                    ErrorCode = coverageResult ? "" : Constants.ErrorCode.External,
                    ErrorReason = coverageResult ? string.Empty : Constants.CheckCoverageErrorMessage.OutOfCoverage(isLocalLanguage),
                    PackageModels = packageList,
                };

                EndInterface<CheckCoverageResponse>(success, log, query.TransactionID,
                                                        success.Result, success.ErrorReason);

                return success;
            }
            catch (System.Exception ex)
            {
                var error = new CheckCoverageResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<CheckCoverageResponse>(error, log, query.TransactionID,
                   error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public CheckSFFInternetProfileResponse CheckSFFInternetProfile(GetSFFInternetProfileQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<GetSFFInternetProfileQuery>(query, "CheckSFFInternetProfile",
                    query.TransactionID, query.IDCardNo);

                var result = _queryProcessor.Execute(query);

                if (null != result)
                {
                    if (result.ReturnCode == "001")
                    {
                        var aisWiFiRegCommand = new AisWiFiRegCommand
                        {
                            CheckChangePromotionModel = result,
                            OrderNo = result.OrderNo,
                            IDCardNo = query.IDCardNo,
                            InternetNo = query.InternetNo,
                            TransactionID = query.TransactionID,
                        };

                        Task.Run(() => _aisWiFiRegCommand.Handle(aisWiFiRegCommand));
                    }

                    var success = new CheckSFFInternetProfileResponse
                    {
                        Result = (result.ReturnCode == "001" ? Constants.Result.Success : Constants.Result.Failed),
                        ErrorCode = (result.ReturnCode == "001" ? "" : Constants.ErrorCode.External),
                        ErrorReason = result.ReturnCode,
                    };

                    EndInterface<CheckSFFInternetProfileResponse>(success, log, query.TransactionID,
                                                                    success.Result, success.ErrorReason);

                    return success;
                }
                else
                {
                    var error = new CheckSFFInternetProfileResponse
                    {
                        Result = Constants.Result.Failed,
                        ErrorCode = Constants.ErrorCode.External,
                        ErrorReason = Constants.CheckSFFInternetProfileErrorMessage.ProfileNotFound(isLocalLanguage: true),
                    };

                    EndInterface<CheckSFFInternetProfileResponse>(error, log, query.TransactionID,
                    error.Result, error.ErrorReason);

                    return error;
                }
            }
            catch (System.Exception ex)
            {
                var error = new CheckSFFInternetProfileResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<CheckSFFInternetProfileResponse>(error, log, query.TransactionID,
                    error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public CheckBlacklistResponse CheckBlacklist(GetBlackListQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                var validationContext = new ValidationContext(query, null, null);
                Validator.ValidateObject(query, validationContext);

                log = StartInterface<GetBlackListQuery>(query,
                            "CheckBlackist",
                            query.TransactionID,
                            query.IDCardNo);

                var blacklistQuery = new evOMServiceIVRCheckBlackListQuery
                {
                    inCardNo = query.IDCardNo
                };

                var result = _queryProcessor.Execute(blacklistQuery);

                if (null != result)
                {
                    var success = new CheckBlacklistResponse
                    {
                        Result = result.returnFlag,
                        ErrorCode = "",
                        ErrorReason = result.ErrorMessage,
                    };

                    switch (result.returnFlag)
                    {
                        case "A":
                            success.ErrorReason = "Sorry! This ID No. is black lists & registered more than no. of mobile limit";
                            break;
                        case "Y":
                            success.ErrorReason = "Sorry! This ID No. is black lists";
                            break;
                        case "O":
                            success.ErrorReason = "Sorry! This ID No. is registered more than no. of mobile limit";
                            break;
                        case "N":
                            success.ErrorReason = "";
                            break;
                        case "I":
                            success.ErrorReason = "Sorry this ID Card is invalid";
                            break;
                        default:
                            break;
                    }

                    EndInterface<CheckBlacklistResponse>(success, log, "", success.Result, success.ErrorReason);

                    return success;
                }
                else
                {
                    var error = new CheckBlacklistResponse
                    {
                        Result = Constants.Result.Failed,
                        ErrorCode = Constants.ErrorCode.Internal,
                        ErrorReason = Constants.ErrorReason.Standard,
                    };

                    EndInterface<CheckBlacklistResponse>(error, log, "", error.Result, error.ErrorReason);

                    return error;
                }
            }
            catch (Exception ex)
            {
                var error = new CheckBlacklistResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<CheckBlacklistResponse>(error, log, "", error.Result, ex.GetErrorMessage());

                return error;
            }
        }
        #endregion

        #region List Or Query
        public ListBuildVillResponse ListBuildingVillage(GetListBuildingVillageQuery query)
        {

            InterfaceLogCommand log = null;
            var buildingVillageModelList = new List<ListBuildingVillageModel>();

            try
            {
                log = StartInterface<GetListBuildingVillageQuery>(query, "ListBuildingVillage",
                   query.TransactionID, "");

                buildingVillageModelList = _queryProcessor.Execute(query);

                var success = new ListBuildVillResponse();

                if (buildingVillageModelList.Count() != 0)
                {
                    success.Result = Constants.Result.Success;
                    success.BuildingVillageModelList = buildingVillageModelList;
                }
                else
                {
                    success.Result = Constants.Result.Failed;
                    success.ErrorReason = "No data found.";
                }
                EndInterface<ListBuildVillResponse>(success, log, query.TransactionID,
                        success.Result, success.ErrorReason);

                return success;
            }
            catch (System.Exception ex)
            {
                var error = new ListBuildVillResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<ListBuildVillResponse>(error, log, query.TransactionID,
                   error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public ListPackageResponse ListPackages(GetListPackageQuery query)
        {
            InterfaceLogCommand log = null;
            var getPackageListQuery = new GetPackageListQuery();

            getPackageListQuery.P_NETWORK_TYPE = "";
            getPackageListQuery.P_OWNER_PRODUCT = "";
            getPackageListQuery.P_PACKAGE_CODE = "";
            getPackageListQuery.P_PACKAGE_FOR = "PUBLIC";
            getPackageListQuery.P_PRODUCT_SUBTYPE = "";
            getPackageListQuery.P_SERVICE_DAY = "";
            getPackageListQuery.TransactionID = query.TransactionID;

            try
            {
                log = StartInterface<GetPackageListQuery>(getPackageListQuery, "ListPackages",
                    query.TransactionID, "");

                var list = (from t in _queryProcessor.Execute(getPackageListQuery)
                            select t.PackageItems).FirstOrDefault();


                var success = new ListPackageResponse
                {
                    Result = Constants.Result.Success,
                    Packages = list.Select(t => new CpPackageModel { PACKAGE_NAME = t.PACKAGE_NAME }).ToList(),
                };

                EndInterface<ListPackageResponse>(success, log, query.TransactionID,
                    success.Result, success.ErrorReason);

                return success;
            }
            catch (System.Exception ex)
            {
                var error = new ListPackageResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<ListPackageResponse>(error, log, "",
                    error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public ListPackageByServiceResponse ListPackagesByService(GetPackageListByServiceQuery query)
        {
            InterfaceLogCommand log = null;

            try
            {
                log = StartInterface<GetPackageListByServiceQuery>(query, "ListPackagesByService",
                    query.TransactionID, "");

                var owenerProduction = _queryProcessor.Execute(new GetMappingSbnOwnerProd
                {
                    FBSSAccessModeInfo = query.ListAccessMode.Select(t => new FBSSAccessModeInfo
                    {
                        AccessMode = t,
                    }).ToList(),
                    IsPartner = query.IsPartner,
                    PartnerName = query.PartnerName,
                });

                if (string.IsNullOrEmpty(owenerProduction))
                {
                    var error = new ListPackageByServiceResponse
                    {
                        Result = Constants.Result.Failed,
                        ErrorCode = Constants.ErrorCode.External,
                        ErrorReason = "Package Not Found",
                    };

                    EndInterface<ListPackageByServiceResponse>(error, log, query.TransactionID,
                    error.Result, error.ErrorReason);

                    return error;
                }

                var listPackageQuery = new WBBContract.Queries.ExWebServices.FbbCpGw.GetListPackageByServiceQuery
                {
                    TransactionID = query.TransactionID,
                    Language = "EN",
                    P_NETWORK_TYPE = "",
                    P_OWNER_PRODUCT = owenerProduction,
                    P_PACKAGE_CODE = "",
                    P_PACKAGE_FOR = "PUBLIC",
                    P_PRODUCT_SUBTYPE = "",
                    P_SERVICE_DAY = "",

                    IsPartner = query.IsPartner.ToYesNoFlgBoolean(),
                    PartnerName = query.PartnerName,
                };

                var result = _queryProcessor.Execute(listPackageQuery);

                var success = new ListPackageByServiceResponse
                {
                    Result = Constants.Result.Success,
                    PackageModels = result.ToList(),
                };

                EndInterface<ListPackageByServiceResponse>(success, log, query.TransactionID,
                    success.Result, success.ErrorReason);

                return success;
            }
            catch (System.Exception ex)
            {
                var error = new ListPackageByServiceResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<ListPackageByServiceResponse>(error, log, "",
                    error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public ListFBSSBuildingResponse ListFBSSBuilding(GetListFBSSBuildingQuery query)
        {
            InterfaceLogCommand log = null;
            var buildingVillageModelList = new List<FBSSBuildingModel>();

            try
            {
                log = StartInterface<GetListFBSSBuildingQuery>(query, "ListFBSSBuilding",
                   query.TransactionID, "");

                buildingVillageModelList = _queryProcessor.Execute(query);

                var success = new ListFBSSBuildingResponse
                {
                    Result = Constants.Result.Success,
                    BuildingVillageModelList = buildingVillageModelList,
                };

                EndInterface<ListFBSSBuildingResponse>(success, log, query.TransactionID,
                        success.Result, success.ErrorReason);

                return success;
            }
            catch (System.Exception ex)
            {
                var error = new ListFBSSBuildingResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<ListFBSSBuildingResponse>(error, log, query.TransactionID,
                   error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public CoverageResultEnquiryResponse CoverageResultEnquiry(CoverageResultEnquiryCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                log = StartInterface<CoverageResultEnquiryCommand>(command, "CoverageResultEnquiry",
                   command.TransactionID, "");

                var lovConfig = _queryProcessor.Execute(new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                });

                var tempNameTH = lovConfig.Where(a => a.Name == "ESRI_REPLACE_NAME_TH").Select(a => new { a.LovValue1, a.LovValue2 });
                var tempNameEN = lovConfig.Where(a => a.Name == "ESRI_REPLACE_NAME_EN").Select(a => new { a.LovValue1, a.LovValue2 });


                if (command.Language.ToSafeString() == "T") // TH
                {


                    var tumtemp = from a in tempNameTH
                                  where a.LovValue1 == command.SubDistrictName.ToSafeString()
                                  select a.LovValue2;

                    if (tumtemp.FirstOrDefault() != null && tumtemp.FirstOrDefault() != "") { command.SubDistrictName = tumtemp.FirstOrDefault(); }

                }
                else // EN
                {

                    var tumtemp = from a in tempNameEN
                                  where a.LovValue1.ToUpper() == command.SubDistrictName.ToSafeString().ToUpper()
                                  select a.LovValue2;

                    var twmpwesubdistrict = _queryProcessor.Execute(new SelectTumbonQuery { REGION_CODE = "", PROVINCE = "", AUMPHUR = "", Lang_Flag = "Y" });

                    var wesubdistrict = twmpwesubdistrict.Where(a => a.LOV_NAME.ToUpper() == command.SubDistrictName.ToUpper()).Select(a => a.LOV_NAME).FirstOrDefault();

                    if (tumtemp.FirstOrDefault() != null && tumtemp.FirstOrDefault() != "")
                    {
                        command.SubDistrictName = tumtemp.FirstOrDefault();
                    }
                    else
                    {
                        if (wesubdistrict != null && wesubdistrict != "")
                        {
                            command.SubDistrictName = wesubdistrict;

                        }
                    }
                }


                var zipCodeRowIdQuery = new SelectZipCodeQuery
                {
                    Tumbon = command.SubDistrictName,
                    PostalCode = command.PostalCode,
                    Language = command.Language.ToCultureCode().IsEngCulture().ToYesNoFlgString()
                };

                var zipCodeRowId = _queryProcessor.Execute(zipCodeRowIdQuery);

                if (null == zipCodeRowId)
                {
                    if (command.Language.ToSafeString() == "T") // TH
                    {
                        //SelectDistrict
                        var districttemp = from a in tempNameTH
                                           where a.LovValue1 == command.District.ToSafeString()
                                           select a.LovValue2;

                        if (districttemp.FirstOrDefault() != null && districttemp.FirstOrDefault() != "") { command.District = districttemp.FirstOrDefault(); }

                        //SelectProvince

                        var provincetemp = from a in tempNameTH
                                           where a.LovValue1 == command.Province.ToSafeString()
                                           select a.LovValue2;

                        if (provincetemp.FirstOrDefault() != null && provincetemp.FirstOrDefault() != "") { command.Province = provincetemp.FirstOrDefault(); }

                    }
                    else
                    {
                        //SelectDistrict

                        var districttemp = from a in tempNameEN
                                           where a.LovValue1.ToUpper() == command.District.ToSafeString().ToUpper()
                                           select a.LovValue2;

                        var twmpwedistrict = _queryProcessor.Execute(new SelectAmphurQuery { REGION_CODE = "", PROVINCE = "", Lang_Flag = "Y" });

                        var wedistrict = twmpwedistrict.Where(a => a.LOV_NAME.ToUpper() == command.District.ToUpper()).Select(a => a.LOV_NAME).FirstOrDefault();

                        if (districttemp.FirstOrDefault() != null && districttemp.FirstOrDefault() != "")
                        {
                            command.District = districttemp.FirstOrDefault();
                        }
                        else
                        {
                            if (wedistrict != null && wedistrict != "")
                            {
                                command.District = wedistrict;

                            }
                        }

                        //SelectProvince

                        var provincetemp = from a in tempNameEN
                                           where a.LovValue1.ToUpper() == command.Province.ToSafeString().ToUpper()
                                           select a.LovValue2;

                        var twmpweprovince = _queryProcessor.Execute(new SelectProvinceQuery { REGION_CODE = "", Lang_Flag = "Y" });

                        var weprovince = twmpweprovince.Where(a => a.LOV_NAME.ToUpper() == command.Province.ToUpper()).Select(a => a.LOV_NAME).FirstOrDefault();

                        if (provincetemp.FirstOrDefault() != null && provincetemp.FirstOrDefault() != "")
                        {
                            command.Province = provincetemp.FirstOrDefault();
                        }
                        else
                        {
                            if (weprovince != null && weprovince != "")
                            {
                                command.Province = weprovince;

                            }
                        }
                    }

                    zipCodeRowIdQuery = new SelectZipCodeQuery
                    {
                        Language = command.Language.ToCultureCode().IsEngCulture().ToYesNoFlgString(),
                        Tumbon = command.SubDistrictName,
                        Aumphur = command.District,
                        Province = command.Province
                    };

                    zipCodeRowId = _queryProcessor.Execute(zipCodeRowIdQuery);
                }

                GetMappingSbnOwnerProd mapProdTypeQuery = null;
                var prodTypeStr = "";

                switch (command.Coverage)
                {
                    case "YES":
                        mapProdTypeQuery = new GetMappingSbnOwnerProd
                        {
                            FBSSAccessModeInfo = command.AccessModeList,
                            IsPartner = command.IsPartner,
                            PartnerName = command.PartnerName,
                        };
                        break;
                    case "PLAN":
                        mapProdTypeQuery = new GetMappingSbnOwnerProd
                        {
                            FBSSAccessModeInfo = new List<FBSSAccessModeInfo>() { command.PlanningSite },
                            IsPartner = command.IsPartner,
                            PartnerName = command.PartnerName,
                        };
                        break;
                    default:
                        break;
                }

                if (null != mapProdTypeQuery)
                    prodTypeStr = _queryProcessor.Execute(mapProdTypeQuery);

                var coveragecmd = new FBSSCoverageResultCommand
                {
                    ACCESS_MODE_LIST = command.AccessModeList.DumpToXml(),
                    ADDRESS_NO = command.AddressNo,
                    ADDRESS_ID = command.AddressId,
                    ADDRRESS_TYPE = command.AddressType,
                    BUILDING_NAME = command.BuildingName,
                    BUILDING_NO = command.BuildingNo,
                    COVERAGE = command.Coverage,
                    FLOOR_NO = command.FloorNo,
                    IS_PARTNER = command.IsPartner,
                    LANGUAGE = command.Language,
                    LATITUDE = command.Latitude,
                    LONGITUDE = command.Longitude,
                    PARTNER_NAME = command.PartnerName,
                    PHONE_FLAG = command.PhoneFlag,
                    PLANNING_SITE_LIST = command.PlanningSite.DumpToXml(),
                    POSTAL_CODE = command.PostalCode,
                    SUB_DISTRICT_NAME = command.SubDistrictName,
                    TRANSACTION_ID = command.TransactionID,
                    UNIT_NO = command.UnitNo,
                    PRODUCTTYPE = prodTypeStr,
                    OWNER_PRODUCT = prodTypeStr,
                    ZIPCODE_ROWID = (zipCodeRowId == null ? "" : zipCodeRowId.ZipCodeId),
                    ActionType = WBBContract.Commands.ActionType.Insert,
                    ActionBy = "FBBMOB",
                    ActionDate = DateTime.Now,
                    LOCATION_CODE = command.LocationCode,
                    ASC_CODE = command.AscCode,
                    EMPLOYEE_ID = command.EmployeeID,
                    SALE_FIRSTNAME = command.SaleFirstname,
                    SALE_LASTNAME = command.SaleLastname,
                    LOCATION_NAME = command.LocationName,
                    SUB_REGION = command.SubRegion,
                    REGION = command.Region,
                    ASC_NAME = command.AscName,
                    CHANNEL_NAME = command.ChannelName,
                    SALE_CHANNEL = command.SaleChannel,

                    //onservice special
                    COVERAGEAREA = command.coverageArea,
                    STATUS = command.status,
                    SUBSTATUS = command.subStatus,
                    CONTACTEMAIL = command.contactEmail,
                    CONTACTTEL = command.contactTel,
                    GROUPOWNER = command.groupOwner,
                    CONTACTNAME = command.contactName,
                    NETWORKPROVIDER = command.networkProvider,
                    FTTHDISPLAYMESSAGE = command.ftthDisplayMessage,
                    WTTXDISPLAYMESSAGE = command.wttxDisplayMessage
                };

                _fbssCoverageCommand.Handle(coveragecmd);

                var success = new CoverageResultEnquiryResponse
                {
                    Result = Constants.Result.Success,
                };

                EndInterface<CoverageResultEnquiryResponse>(success, log, command.TransactionID,
                      success.Result, success.ErrorReason);
                
                return success;
            }
            catch (Exception ex)
            {
                var error = new CoverageResultEnquiryResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<CoverageResultEnquiryResponse>(error, log, command.TransactionID,
                   error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public ListDuplicateOrderResponse ListDuplicateOrder(GetOrderDuplicateQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                var validationContext = new ValidationContext(query, null, null);
                Validator.ValidateObject(query, validationContext);

                log = StartInterface<GetOrderDuplicateQuery>(query, "ListDuplicateOrder",
                    query.TransactionID, query.IDCardNo);

                if (!_listOfAcceptedLangCode.Contains(query.Language))
                {
                    throw new ValidationException("Language Code Must Be THA, ENG");
                }

                var checkOrderDuplicationQuery = new GetOrderDupQuery
                {
                    p_id_card = query.IDCardNo,
                    p_eng_flag = query.Language.ToCultureCode().IsEngCulture().ToYesNoFlgString(),
                };

                var result = _queryProcessor.Execute(checkOrderDuplicationQuery);

                var success = new ListDuplicateOrderResponse
                {
                    DuplicatedOrders = result,
                    Result = Constants.Result.Success,
                    ErrorCode = "",
                    ErrorReason = "",
                };

                EndInterface<ListDuplicateOrderResponse>(success, log, "", success.Result, success.ErrorReason);

                return success;
            }
            catch (Exception ex)
            {
                var error = new ListDuplicateOrderResponse
                {
                    DuplicatedOrders = new List<OrderDupModel>(),
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<ListDuplicateOrderResponse>(error, log, "", error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public GetSubContractTimeSlotResponse GetSubContractTimeSlot(GetSubContractTimeSlotQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                if (!query.InstallationDate.IsDateStringCorrectedFormat(_installationDateFormat))
                {
                    throw new ValidationException("Installation Date Is Null Or Not In Format (yyyy-MM-dd)");
                }

                if (!_listOfAcceptedLangCode.Contains(query.Language))
                {
                    throw new ValidationException("Language Code Must Be THA, ENG");
                }

                log = StartInterface<GetSubContractTimeSlotQuery>(query, "GetSubContractTimeSlot",
                   query.TransactionID, "");

                var isThai = query.Language.ToCultureCode().IsThaiCulture();

                var getFbssAppointmentQuery = new GetFBSSAppointment()
                {
                    Days = query.Days,
                    ExtendingAttributes = "",
                    InstallationDate = query.InstallationDate,
                    District = query.District,
                    Language = (isThai ? "T" : "E"),
                    Postal_Code = query.PostalCode,
                    Province = query.Province,
                    Service_Code = query.ServiceCode,
                    SubDistrict = query.SubDistrict,
                    LineSelect = LineType.Line1,
                    SubAccessMode = "",
                    Transaction_Id = ""
                };

                var result = _queryProcessor.Execute(getFbssAppointmentQuery);

                var success = new GetSubContractTimeSlotResponse
                {
                    TimeSlots = result,
                    Result = Constants.Result.Success,
                    ErrorCode = "",
                    ErrorReason = "",
                };

                EndInterface<GetSubContractTimeSlotResponse>(success, log, "", success.Result, success.ErrorReason);

                return success;
            }
            catch (Exception ex)
            {
                var error = new GetSubContractTimeSlotResponse
                {
                    TimeSlots = new List<FBSSTimeSlot>(),
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<GetSubContractTimeSlotResponse>(error, log, "", error.Result, ex.GetErrorMessage());

                return error;
            }
        }
        #endregion

        #region Register
        public RegisterOutOfCoverageResponse RegisterOutOfCoverage(GetRegOutOfCoverageResultQuery query)
        {
            InterfaceLogCommand log = null;
            var custNameArray = query.CustName.Split(' ');
            var custFirstName = "";
            var custLastName = "";

            string building_name = "";
            string building_no = "";
            string floor_no = "";
            string address_no = "";
            string moo = "";
            string soi = "";
            string road = "";
            string tumbon = "";
            string amphur = "";
            string province = "";
            string postal_code = "";
            try
            {
                log = StartInterface<GetRegOutOfCoverageResultQuery>(query, "RegisterOutOfCoverage",
                    query.TransactionID, query.IDCardNo);

                if (custNameArray.Length > 0)
                {
                    custFirstName = custNameArray[0];
                    custLastName = string.Join(" ", custNameArray.Skip(1));
                }

                var saveOrderRes = _queryProcessor.Execute(query);

                if (saveOrderRes.Count <= 0)
                    throw new SaveOrderResWasNoValue();

                // update coverage result command.
                var command = new FBSSCoverageResultCommand
                {
                    RESULTID = saveOrderRes[0].ToSafeDecimal(),
                    PREFIXNAME = "127",
                    FIRSTNAME = custFirstName,
                    LASTNAME = custLastName,
                    CONTACTNUMBER = query.ContactMobileNo,
                    ActionType = ActionType.Update,
                    EMAIL = query.EmailAddress,
                    LINEID = query.LineId,
                    ADDRESS_TYPE_DTL = query.AddressTypeDTL,
                    REMARK = query.Remark,
                    TECHNOLOGY = query.Technology,
                    PROJECTNAME = query.Projectname,

                    RETURN_CODE = saveOrderRes[1].ToSafeInteger(),
                    RETURN_MESSAGE = saveOrderRes[2],
                    RETURN_ORDER = saveOrderRes[3]
                };

                _fbssCoverageCommand.Handle(command);

                // get data coverageareResult
                var getDataCoverageAreaResultQuery = new GetDataCoverageAreaResultQuery()
                {
                    RESULTID = saveOrderRes[0].ToSafeDecimal()
                };
                var resultCoverageArea = _queryProcessor.Execute(getDataCoverageAreaResultQuery);

                // call leavemessage
                if (resultCoverageArea != null  && !string.IsNullOrEmpty(resultCoverageArea.COVERAGE_SUBSTATUS)) //&& resultCoverageArea.COVERAGE_STATUS.Equals("ON_SERVICE_SPECIAL")
                {

                    var OnServiceSpecialQuery = new InstallLeaveMessageQuery()
                    {
                        p_result_id = saveOrderRes[0].ToSafeString(),
                        p_status = resultCoverageArea.COVERAGE_STATUS.ToSafeString()
                    };
                    _queryProcessor.Execute(OnServiceSpecialQuery);

                    // config ftth onservice special lov
                    var getLov = Get_FBB_CFG_LOV("FTTH_ON_SERVICE_SPECIAL", "").ToList(); 
                    // get substatus 
                    var getLovVal_substatus = getLov.Where(p => p.Name == "SUB_STATUS" && p.LovValue1 == resultCoverageArea.COVERAGE_SUBSTATUS).Select(o => o.LovValue1).FirstOrDefault() ?? string.Empty;

                    //send email coverageareResult
                    if (getLovVal_substatus.Equals(resultCoverageArea.COVERAGE_SUBSTATUS))
                    {
                       
                        building_name = string.IsNullOrEmpty(resultCoverageArea.BUILDING_NAME) ? "-" : resultCoverageArea.BUILDING_NAME.ToSafeString();
                        building_no = string.IsNullOrEmpty(resultCoverageArea.BUILDING_NO) ? "-" : resultCoverageArea.BUILDING_NO.ToSafeString();
                        floor_no = string.IsNullOrEmpty(resultCoverageArea.FLOOR_NO) ? "-" : resultCoverageArea.FLOOR_NO.ToSafeString();
                        address_no = string.IsNullOrEmpty(resultCoverageArea.ADDRESS_NO) ? "-" : resultCoverageArea.ADDRESS_NO.ToSafeString();
                        moo = resultCoverageArea.MOO == 0 ? "-" : resultCoverageArea.MOO.ToSafeString();
                        soi = string.IsNullOrEmpty(resultCoverageArea.SOI) ? "-" : resultCoverageArea.SOI.ToSafeString();
                        road = string.IsNullOrEmpty(resultCoverageArea.ROAD) ? "-" : resultCoverageArea.ROAD.ToSafeString();
                        tumbon =  string.IsNullOrEmpty(resultCoverageArea.TUMBON) ? "-" : resultCoverageArea.TUMBON.ToSafeString();
                        amphur = string.IsNullOrEmpty(resultCoverageArea.AMPHUR) ? "-" : resultCoverageArea.AMPHUR.ToSafeString();
                        province = string.IsNullOrEmpty(resultCoverageArea.PROVINCE) ? "-" : resultCoverageArea.PROVINCE.ToSafeString();
                        postal_code = string.IsNullOrEmpty(resultCoverageArea.POSTAL_CODE) ? "-" : resultCoverageArea.POSTAL_CODE.ToSafeString();

                        string strBody = "";
                        if (resultCoverageArea.LANGUAGE.Equals("T"))
                        {
                            //get addr_t
                            var getLovVal_addrT = getLov.Where(p => p.Name == "ADDR_T").Select(o => o.LovValue1).FirstOrDefault() ?? string.Empty;
                            if (resultCoverageArea.ADDRRESS_TYPE.ToUpper().Equals("B"))
                            {
                                strBody = string.Format(getLovVal_addrT,
                                                   building_no,
                                                   building_name,
                                                   floor_no,
                                                   moo,
                                                   road,
                                                   soi,
                                                   tumbon,
                                                   amphur,
                                                   province,
                                                   postal_code);
                                
                            }
                            else
                            {
                                strBody = string.Format(getLovVal_addrT,
                                                     address_no,
                                                     building_name,
                                                     floor_no,
                                                     moo,
                                                     road,
                                                     soi,
                                                     tumbon,
                                                     amphur,
                                                     province,
                                                     postal_code);
                            }
                            
                        }
                        else if (resultCoverageArea.LANGUAGE.Equals("E"))
                        {
                            //get addr_e
                            var getLovVal_addrE = getLov.Where(p => p.Name == "ADDR_E").Select(o => o.LovValue1).FirstOrDefault() ?? string.Empty;
                            if (resultCoverageArea.ADDRRESS_TYPE.ToUpper().Equals("B"))
                            {
                                strBody = string.Format(getLovVal_addrE,
                                                   building_no,
                                                   building_name,
                                                   floor_no,
                                                   moo,
                                                   road,
                                                   soi,
                                                   tumbon,
                                                   amphur,
                                                   province,
                                                   postal_code);
                            }
                            else
                            {
                                strBody = string.Format(getLovVal_addrE,
                                                     address_no,
                                                     building_name,
                                                     floor_no,
                                                     moo,
                                                     road,
                                                     soi,
                                                     tumbon,
                                                     amphur,
                                                     province,
                                                     postal_code);
                            }                               
                        }
                        resultCoverageArea.COVERAGE_CONTACTNAME = string.IsNullOrEmpty(resultCoverageArea.COVERAGE_CONTACTNAME) ? "ช่องทางการขาย" : resultCoverageArea.COVERAGE_CONTACTNAME;
                        // get body email
                        var getLovVal_email = getLov.Where(p => p.Name == "EMAIL").Select(o => o.LovValue1).FirstOrDefault() ?? string.Empty;
                        var tempBody = string.Format(getLovVal_email, resultCoverageArea.COVERAGE_CONTACTNAME, custFirstName, query.ContactMobileNo, strBody); 

                        // get subject email
                        var getLovVal_subjectemail = getLov.Where(p => p.Name == "SUBJECT_EMAIL").Select(o => o.LovValue1).FirstOrDefault() ?? string.Empty;

                        // check null email of coverage_contactemail
                        if (!string.IsNullOrEmpty(resultCoverageArea.COVERAGE_CONTACTEMAIL))
                        {
                            var sendmailFTTHcommand = new SendMailFTTHNotificationCommand
                            {
                                ProcessName = "SEND_MAIL_ONSERVICE_SPECIAL_FTTH",
                                Subject = getLovVal_subjectemail,
                                Body = tempBody,
                                SendTo = resultCoverageArea.COVERAGE_CONTACTEMAIL
                            };
                            _sendMail.Handle(sendmailFTTHcommand);
                        }                        
                    }
                }

                //var coveragecmd = new FBSSCoverageResultCommand
                //{
                //    TRANSACTION_ID = query.TransactionID,

                //    PREFIXNAME = query.Language.ToCultureCode().IsThaiCulture() ? "คุณ" : "",
                //    FIRSTNAME = custFirstName,
                //    LASTNAME = custLastName,
                //    CONTACTNUMBER = query.ContactMobileNo,
                //    RETURN_CODE = saveOrderRes[1].ToSafeInteger(),
                //    RETURN_MESSAGE = saveOrderRes[2],
                //    RETURN_ORDER = saveOrderRes[3],

                //    ActionType = WBBContract.Commands.ActionType.Update,
                //    ActionBy = "FBBMOB",
                //    ActionDate = DateTime.Now,
                //};

                //_fbssCoverageCommand.Handle(coveragecmd);

                if (saveOrderRes[1] == "0")
                {
                    var success = new RegisterOutOfCoverageResponse
                    {
                        Result = Constants.Result.Success,
                    };

                    EndInterface<RegisterOutOfCoverageResponse>(success, log, query.TransactionID,
                        success.Result, success.ErrorReason);

                    return success;
                }
                else
                {
                    var error = new RegisterOutOfCoverageResponse
                    {
                        Result = Constants.Result.Failed,
                        ErrorCode = Constants.ErrorCode.External,
                        ErrorReason = saveOrderRes[2],
                    };

                    EndInterface<RegisterOutOfCoverageResponse>(error, log, query.TransactionID,
                        error.Result, error.ErrorReason);

                    return error;
                }
            }
            catch (System.Exception ex)
            {
                var error = new RegisterOutOfCoverageResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<RegisterOutOfCoverageResponse>(error, log, query.TransactionID,
                    error.Result, ex.GetErrorMessage());

                return error;
            }
        }
        public List<LovValueModel> Get_FBB_CFG_LOV(string LOV_TYPE, string LOV_NAME)
        {
            var query = new GetLovQuery()
            {
                LovType = LOV_TYPE,
                LovName = LOV_NAME,
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);

            return _FbbCfgLov;
        }
        public RegisterInCoverageResponse RegisterInCoverage(GetRegResultQuery query)
        {
            InterfaceLogCommand log = null;

            log = StartInterface<GetRegResultQuery>(query,
                "RegisterInCoverage",
                query.TransactionID, query.IDCardNo);

            var regResultQuery = new GetRegResultCoreQuery
            {
                RegResultType = RegResultType.CRM,
                TransactionID = query.TransactionID,

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
                LocationCode = query.LocationCode,
                ASCCode = query.ASCCode,
                StaffID = query.StaffID,
                SaleRep = query.SaleRep,
            };

            var saveOrderRes = new CustRegisterInfoModel();

            var response = RegisterInCoverageCore(regResultQuery, out saveOrderRes);

            // update coverage result command.
            var command = new CoverageResultCommand
            {
                RESULT_ID = saveOrderRes.RegisterResult.CoverageResultId.ToSafeDecimal(),
                PREFIXNAME = "127",
                FIRSTNAME = query.CustFirstName,
                LASTNAME = query.CustLastName,
                CONTACTNUMBER = query.ContactMobileNo,
                ActionType = ActionType.Update,

                ReturnCode = saveOrderRes.RegisterResult.ReturnCode.ToSafeInteger(),
                ReturnMessage = saveOrderRes.RegisterResult.ReturnMessage,
                ReturnOrder = saveOrderRes.RegisterResult.ReturnIANO
            };

            _covResultCommand.Handle(command);

            EndInterface<RegisterInCoverageResponse>(response,
                log, query.TransactionID,
                response.Result, response.ErrorReason);

            return response;
        }

        public RegisterInCoverageResponse FBSSRegisterInCoverage(GetFBSSRegResultQuery query)
        {
            InterfaceLogCommand log = null;

            log = StartInterface<GetFBSSRegResultQuery>(query,
                "FBSSRegisterInCoverage",
                query.TransactionID, query.IDCardNo);

            try
            {
                // cancel the order if any cancel order reports
                if (query.CancelOrders != null
                    && query.CancelOrders.OrderNo.Any())
                {
                    var cancelOrderAirnetQuery = new GetCancelOrderQuery
                    {
                        ID_Card_No = query.IDCardNo,
                        ListOrder = query.CancelOrders.OrderNo,
                    };

                    _queryProcessor.Execute(cancelOrderAirnetQuery);
                }

                var regResultQuery = new GetRegResultCoreQuery
                {
                    RegResultType = RegResultType.FBSS,
                    TransactionID = query.TransactionID,

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
                    LocationCode = query.LocationCode,
                    ASCCode = query.ASCCode,
                    StaffID = query.StaffID,
                    SaleRep = query.SaleRep,
                    PhoneFlag = query.PhoneFlag,
                    TimeSlot = query.TimeSlot,
                    InstallCapacity = query.InstallCapacity,
                    AddressId = query.AddressId,
                    IsPartner = query.IsPartner,
                    PartnerName = query.PartnerName,
                    OrderRef = query.OrderRef,
                };

                var saveOrderRes = new CustRegisterInfoModel();

                var response = RegisterInCoverageCore(regResultQuery, out saveOrderRes);

                EndInterface<RegisterInCoverageResponse>(response,
                    log, query.TransactionID,
                    response.Result, response.ErrorReason);

                return response;
            }
            catch (Exception ex)
            {
                var error = new RegisterInCoverageResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                if (ex is ArgumentNullException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<RegisterInCoverageResponse>(error, log, "", error.Result, ex.GetErrorMessage());

                return error;
            }

        }

        private RegisterInCoverageResponse RegisterInCoverageCore(GetRegResultCoreQuery query, out CustRegisterInfoModel saveOrderRes)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<GetRegResultCoreQuery>(query, "RegisterInCoverage",
                                                            query.TransactionID, query.IDCardNo);

                var isLocalLanguage = query.Language.ToCultureCode().IsThaiCulture();

                var mainPackBill = (from t in query.SelectPackage
                                    where t.PACKAGE_TYPE.IsMainPack()
                                    select t).FirstOrDefault();

                if (mainPackBill == null)
                    throw new MainPackageNotFoundException(Constants
                        .QuickWinModelBuilderErrorMessage
                        .MainPackIsNull(isLocalLanguage));

                saveOrderRes = _queryProcessor.Execute(query);

                // update coverage result command.
                //var command = new CoverageResultCommand
                //{
                //    RESULT_ID = saveOrderRes.RegisterResult.CoverageResultId.ToSafeDecimal(),
                //    PREFIXNAME = "127",
                //    FIRSTNAME = query.CustFirstName,
                //    LASTNAME = query.CustLastName,
                //    CONTACTNUMBER = query.ContactMobileNo,
                //    ActionType = ActionType.Update,

                //    ReturnCode = saveOrderRes.RegisterResult.ReturnCode.ToSafeInteger(),
                //    ReturnMessage = saveOrderRes.RegisterResult.ReturnMessage,
                //    ReturnOrder = saveOrderRes.RegisterResult.ReturnIANO
                //};

                //_covResultCommand.Handle(command);

                if (saveOrderRes.RegisterResult.ReturnCode == "0")
                //if (true)
                {
                    var doctype = (!string.IsNullOrEmpty(query.LocationCode)
                          || !string.IsNullOrEmpty(query.ASCCode)
                          || !string.IsNullOrEmpty(query.StaffID)
                          || !string.IsNullOrEmpty(query.SaleRep)) ?
                          "STAFF" : // it's suck
                              "RES";

                    saveOrderRes.DocType = doctype;

                    // do build the quickwin model.
                    var qwModel = QuickWinPanelBuilder(query, mainPackBill, isLocalLanguage, doctype);

                    // register a customer
                    var custRegCommand = new WBBContract.Commands.ExWebServices.FbbCpGw.CustRegisterCommand
                    {
                        CustRegisterInfoModel = saveOrderRes,
                        CoveragePanelModel = qwModel.CoveragePanelModel,
                        TransactionID = query.TransactionID,
                        PhoneFlag = query.PhoneFlag,
                        TimeSlot = query.TimeSlot,
                        InstallCapacity = query.InstallCapacity,
                        OrderRefId = query.OrderRef.ToSafeDecimal(),
                        TimeSlotID = query.TimeSlotID,
                        Guid = query.Guid,
                    };
                    _custRegCommand.Handle(custRegCommand);

                    // insert a mail log 
                    //var tempCustRowId = "0C0C17A7E43852A7E05365A0FC0A43D9";
                    var insertMailLogCmd = new MailLogCommand
                    {
                        CustomerId = custRegCommand.CustRegisterInfoModel.RegisterResult.CustomerRowId,
                    };
                    _mailLogCommand.Handle(insertMailLogCmd);

                    // do the generate a pdf file to nas path
                    string uploadFileWebPath = Configurations.UploadFilePath;
                    string uploadFileAppPath = Configurations.UploadFileTempPath;
                    string fontPath = Configurations.FontFolder;
                    string imagePath = Configurations.ImageFolder;

                    System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
                    string filename = "Request"
                                            + DateTime.Now.ToString("ddMMyy", format)
                                            + "_"
                                            + insertMailLogCmd.RunningNo.ToSafeString();

                    var genPdfCmd = new GeneratePdfCommand
                    {
                        CurrentUICulture = query.Language.ToCultureCode(),
                        DirectoryPath = @uploadFileWebPath,
                        DirectoryTempPath = @uploadFileAppPath,
                        FileName = filename,
                        Model = qwModel,
                        FontFolderPath = fontPath,
                        ImageFolderPath = imagePath,
                    };
                    _genPdfCommand.Handle(genPdfCmd);

                    var directoryPath = genPdfCmd.PdfPath;

                    // do the send an email
                    if (!string.IsNullOrEmpty(query.ContactEmail))
                    {
                        var sendMailCmd = new NotificationCommand
                        {
                            CustomerId = custRegCommand.CustRegisterInfoModel.RegisterResult.CustomerRowId,
                            CurrentCulture = query.Language.ToCultureCode(),
                            RunningNo = insertMailLogCmd.RunningNo,
                            EmailModel = new EmailModel
                            {
                                MailTo = query.ContactEmail,
                                FilePath = @directoryPath,
                            },
                        };
                        _notiCommand.Handle(sendMailCmd);
                    }

                    //var tempOrderNo = "AIR-NW-201501-000028";
                    var success = new RegisterInCoverageResponse
                    {
                        Result = Constants.Result.Success,
                        IANO = custRegCommand.CustRegisterInfoModel.RegisterResult.ReturnIANO.ToOrderNo(),
                    };

                    EndInterface<RegisterInCoverageResponse>(success, log, query.TransactionID,
                                                                success.Result, success.ErrorReason);

                    return success;
                }
                else
                {
                    var error = new RegisterInCoverageResponse
                    {
                        Result = Constants.Result.Failed,
                        ErrorCode = Constants.ErrorCode.External,
                        ErrorReason = saveOrderRes.RegisterResult.ReturnMessage,
                    };

                    EndInterface<RegisterInCoverageResponse>(error, log, query.TransactionID,
                    error.Result, error.ErrorReason);

                    return error;
                }
            }
            catch (System.Exception ex)
            {
                saveOrderRes = new CustRegisterInfoModel();

                var error = new RegisterInCoverageResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                if (ex is MainPackageNotFoundException)
                {
                    error.ErrorCode = Constants.ErrorCode.External;
                    error.ErrorReason = ex.Message;
                }

                EndInterface<RegisterInCoverageResponse>(error, log, query.TransactionID,
                    error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public ReserveTimeSlotResponse ReserveTimeSlot(ReserveTimeSlotQuery query)
        {
            InterfaceLogCommand log = null;
            try
            {
                if (!query.ReserveDateTime.IsDateStringCorrectedFormat(_reserveTimeSlotDateFormat))
                {
                    throw new Exception("ReserveDateTime not in correted format requred format is yyyy-MM-dd HH:mm:ss");
                }

                var reserveTimeSlot = new ReserveTimeSlotCommand
                {
                    TimeSlotId = query.TimeSlotID,
                    ReserveDTM = query.ReserveDateTime,
                    IdReserve = new Guid(query.Guid),
                };

                _reserveTimeSlotCommand.Handle(reserveTimeSlot);

                if (reserveTimeSlot.Return_Code != -1)
                {
                    var success = new ReserveTimeSlotResponse
                    {
                        Result = Constants.Result.Success,
                        ErrorCode = "",
                        ErrorReason = "",
                    };

                    EndInterface<ReserveTimeSlotResponse>(success, log, "", success.Result, success.ErrorReason);

                    return success;
                }
                else
                {
                    var error = new ReserveTimeSlotResponse
                    {
                        Result = Constants.Result.Failed,
                        ErrorCode = Constants.ErrorCode.External,
                        ErrorReason = reserveTimeSlot.Return_Message,
                    };

                    EndInterface<ReserveTimeSlotResponse>(error, log, "", error.Result, error.ErrorReason);

                    return error;
                }
            }
            catch (Exception ex)
            {
                var error = new ReserveTimeSlotResponse
                {
                    Result = Constants.Result.Failed,
                    ErrorCode = Constants.ErrorCode.Internal,
                    ErrorReason = Constants.ErrorReason.Standard,
                };

                if (ex is System.ComponentModel.DataAnnotations.ValidationException)
                {
                    error.ErrorReason = ex.Message;
                }

                EndInterface<ReserveTimeSlotResponse>(error, log, "", error.Result, ex.GetErrorMessage());

                return error;
            }
        }

        public ListImageResponse GetPicture(GetListImagePOIQuery query)
        {
            InterfaceLogCommand log = null;
            ListImageResponse Listimageresult = new ListImageResponse();
            try
            {
                log = StartInterface<GetListImagePOIQuery>(query, "GetPicture", query.TransactionID, "");

                var lovConfig = _queryProcessor.Execute(new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                });

                var tempNameTH = lovConfig.Where(a => a.Name == "ESRI_REPLACE_NAME_TH").Select(a => new { a.LovValue1, a.LovValue2 });
                var tempNameEN = lovConfig.Where(a => a.Name == "ESRI_REPLACE_NAME_EN").Select(a => new { a.LovValue1, a.LovValue2 });

                if (query.Language.ToSafeString() == "1") // TH
                {


                    var tumtemp = from a in tempNameTH
                                  where a.LovValue1 == query.SubDistrict.ToSafeString()
                                  select a.LovValue2;

                    if (tumtemp.FirstOrDefault() != null && tumtemp.FirstOrDefault() != "") { query.SubDistrict = tumtemp.FirstOrDefault(); }


                }
                else // EN
                {

                    var tumtemp = from a in tempNameEN
                                  where a.LovValue1.ToUpper() == query.SubDistrict.ToSafeString().ToUpper()
                                  select a.LovValue2;

                    var twmpwesubdistrict = _queryProcessor.Execute(new SelectTumbonQuery { REGION_CODE = "", PROVINCE = "", AUMPHUR = "", Lang_Flag = "Y" });

                    var wesubdistrict = twmpwesubdistrict.Where(a => a.LOV_NAME.ToUpper() == query.SubDistrict.ToUpper()).Select(a => a.LOV_NAME).FirstOrDefault();

                    if (tumtemp.FirstOrDefault() != null && tumtemp.FirstOrDefault() != "")
                    {
                        query.SubDistrict = tumtemp.FirstOrDefault();
                    }
                    else
                    {
                        if (wesubdistrict != null && wesubdistrict != "")
                        {
                            query.SubDistrict = wesubdistrict;

                        }
                    }

                }

                CoveragePictureModel temp = new CoveragePictureModel();

                temp.language = query.Language;
                temp.site_code = query.SiteCode;
                temp.sub_district = query.SubDistrict;
                temp.transaction_id = query.TransactionID;
                temp.zip_code = query.ZipCode;
                temp.user = "ESRI";

                var cancelOrderAirnetQuery = new GetImagePOIQuery
                {
                    model = temp
                };

                var result = _queryProcessor.Execute(cancelOrderAirnetQuery);


                Listimageresult.ReturnCode = result.ReturnCode;
                Listimageresult.ReturnMassage = result.ReturnMassage;

                if (result.PicList.Count() != 0)
                {
                    Listimageresult.PicList = new List<PicList>();
                    foreach (var a in result.PicList)
                    {
                        PicList tee = new PicList();
                        tee.url = a.PICTURE_PATH;
                        Listimageresult.PicList.Add(tee);
                    }
                }
                EndInterface<ListImageResponse>(Listimageresult, log, "", "Success", Listimageresult.ReturnMassage);
                return Listimageresult;
            }
            catch (Exception ex)
            {
                EndInterface<ListImageResponse>(Listimageresult, log, "", "FAILED", ex.GetErrorMessage() + "internal:" + ex.InnerException);
                return Listimageresult;
            }

        }
        #endregion

        #region Preregister
        public CreateOrderPreRegisterResponse CreateOrderPreRegister(CreateOrderPreRegisterModel command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<CreateOrderPreRegisterModel>(command, "CreateOrderPreRegister", command.ContactMobileNo, command.ContactMobileNo);

                var Command = new SaveLeavemessageCommand
                {
                    p_language = command.Language,
                    p_service_speed = command.ServiceSpeed,
                    p_cust_name = command.CustName,
                    p_cust_surname = command.CustSurname,
                    p_contact_mobile_no = command.ContactMobileNo,
                    p_is_ais_mobile = command.IsAisMobile,
                    p_contact_email = command.ContactEmail,
                    p_address_type = command.AddressType,
                    p_building_name = command.BuildingName,
                    p_house_no = command.HouseNo,
                    p_soi = command.Soi,
                    p_road = command.Road,
                    p_sub_district = command.SubDistrict,
                    p_district = command.District,
                    p_province = command.Province,
                    p_postal_code = command.PostalCode,
                    p_contact_time = command.ContactTime,
                    p_location_code = command.LocationCode,
                    p_asc_code = command.AscCode,
                    p_channel = command.Channel.ToSafeString(),

                    p_in_coverage = command.InCoverage,
                    p_playbox_flag = command.PlayboxFlag,
                    p_latitude = command.Latitude,
                    p_longitude = command.Longitude,
                    p_url = "",

                    p_internet_no = "",
                    p_line_id = "",
                    p_voucher_desc = "",
                    p_campaign_project_name = command.CampaignProjectName.ToSafeString(),
                    p_full_address = command.Full_Address,
                    p_relate_mobile_no = command.RelateMobileNo.ToSafeString(),
                    p_fbb_percent_discount = command.FBBDiscount.ToSafeString(),
                    p_order_mc_no = command.OrderMcNo.ToSafeString(),
                    p_address_mc = command.AddressMc.ToSafeString(),

                    //012024_Add Parameter
                    p_coveragearea = command.CoverageArea,
                    p_networkprovider = command.NetworkProvider,
                    p_region = command.Region,
                    p_coveragesubstatus = command.CoverageSubstatus,
                    p_coveragegroupowner = command.CoverageGroupOwner,
                    p_coveragecontactname = command.CoverageContactName,
                    p_coveragecontactemail = command.CoverageContactEmail,
                    p_coveragecontacttel = command.CoverageContactTel,
                    p_coveragestatus = command.CoverageStatus,
                    p_coverage = "null"
                };
                _saveLeavemessageCommand.Handle(Command);
                if (Command.return_code == 0)
                {
                    var success = new CreateOrderPreRegisterResponse
                    {
                        ReturnCode = 0,
                        ReturnMessage = Command.return_msg
                    };
                    EndInterface<CreateOrderPreRegisterResponse>(success, log, command.ContactMobileNo, "Success", "");
                    return success;

                }
                else
                {
                    var error = new CreateOrderPreRegisterResponse
                    {
                        ReturnCode = -1,
                        ReturnMessage = Command.return_msg
                    };
                    EndInterface<CreateOrderPreRegisterResponse>(error, log, command.TransactionID, error.ReturnMessage, error.ReturnMessage);

                    return error;
                }
            }
            catch (System.Exception ex)
            {
                var error = new CreateOrderPreRegisterResponse
                {
                    ReturnCode = -1,
                    ReturnMessage = "CreateOrderPreRegister error"
                };

                EndInterface<CreateOrderPreRegisterResponse>(error, log, command.TransactionID, ex.Message, error.ReturnMessage);

                return error;
            }
        }

        public QueryOrderPreRegisterResponse QueryOrderPreRegister(GetPreRegisterQuery query)
        {
            InterfaceLogCommand log = null;
            var OrderPreRegisterList = new GetPreRegisterQueryData();

            try
            {
                log = StartInterface<GetPreRegisterQuery>(query, "QueryOrderPreRegister",
                   query.TransactionID, "");

                OrderPreRegisterList = _queryProcessor.Execute(query);

                QueryOrderPreRegisterResponse success = new QueryOrderPreRegisterResponse();
                success.ReturnCode = OrderPreRegisterList.ReturnCode;
                success.ReturnMessage = OrderPreRegisterList.ReturnMessage;

                List<outPreRegisterModel> tempPreRegisterList = new List<outPreRegisterModel>();

                foreach (var PreRegister in OrderPreRegisterList.PreRegisterModel)
                {
                    outPreRegisterModel outPreRegister = new outPreRegisterModel();
                    outPreRegister.CustName = PreRegister.cust_name;
                    outPreRegister.CustSurname = PreRegister.cust_surname;
                    outPreRegister.OrderDate = PreRegister.order_date;
                    outPreRegister.OrderStatus = PreRegister.order_status;
                    outPreRegister.OrderResult = PreRegister.order_result;

                    tempPreRegisterList.Add(outPreRegister);
                }
                success.PreRegisterModel = tempPreRegisterList;

                EndInterface<QueryOrderPreRegisterResponse>(success, log, query.TransactionID,
                        "Success", "");

                return success;


            }
            catch (System.Exception ex)
            {
                QueryOrderPreRegisterResponse error = new QueryOrderPreRegisterResponse();
                error.ReturnMessage = ex.Message;
                error.ReturnCode = -1;

                //EndInterface<ListFBSSBuildingResponse>(error, log, query.TransactionID,
                //   ex.Message, ex.GetErrorMessage());

                return error;
            }
        }

        public CheckPremiumFlagResponse CheckPremiumFlag(CheckPremiumFlagQuery query)
        {
            InterfaceLogCommand log = null;
            string LogMsgDetail = "";
            bool HaveData = false;
            CheckPremiumFlagResponse Response = new CheckPremiumFlagResponse();
            Response.AccessMode = query.AccessMode;
            Response.RecurringCharge = query.RecurringCharge;
            Response.LocationCode = query.LocationCode;

            log = StartInterface(query,
                "CheckPremiumFlag",
                query.TransactionID, "");
            try
            {
                PremiumAreaModel premiumAreaModel = GetPremiumArea(query.SubDistrict, query.District, query.Province, query.PostalCode, query.Language);

                if (premiumAreaModel != null && premiumAreaModel.ReturnCode == "0" && premiumAreaModel.ReturnPremiumConfig != null && premiumAreaModel.ReturnPremiumConfig.Count > 0)
                {
                    List<ReturnPremiumConfigData> returnPremiumConfigDataList = new List<ReturnPremiumConfigData>();
                    foreach (var itempremiumAreaModel in premiumAreaModel.ReturnPremiumConfig)
                    {

                        ReturnPremiumConfigData returnPremiumConfigData = new ReturnPremiumConfigData()
                        {
                            Region = itempremiumAreaModel.Region,
                            DistrictEn = itempremiumAreaModel.DistrictEn,
                            DistrictTh = itempremiumAreaModel.DistrictTh,
                            ProvinceEn = itempremiumAreaModel.ProvinceEn,
                            ProvinceTh = itempremiumAreaModel.ProvinceTh,
                            SubdistrictEn = itempremiumAreaModel.SubdistrictEn,
                            SubdistrictTh = itempremiumAreaModel.SubdistrictTh,
                            Postcode = itempremiumAreaModel.Postcode
                        };
                        returnPremiumConfigDataList.Add(returnPremiumConfigData);
                    }
                    Response.ReturnCode = premiumAreaModel.ReturnCode;
                    Response.ReturnMessage = premiumAreaModel.ReturnMessage;
                    Response.ReturnPremiumConfig = returnPremiumConfigDataList;

                    CheckPremiumFlagModel checkPremiumFlagData = CheckPremiumFlagData(query.RecurringCharge, query.LocationCode, query.AccessMode, query.PartnerSubtype, query.MemoFlag, query.TransactionID);
                    if (checkPremiumFlagData != null && checkPremiumFlagData.ReturnCode == "0" && checkPremiumFlagData.ReturnPremiumTimeSlot != null && checkPremiumFlagData.ReturnPremiumTimeSlot.Count > 0)
                    {
                        List<ReturnPremiumTimeSlotData> returnPremiumTimeSlotDataList = new List<ReturnPremiumTimeSlotData>();
                        foreach (var itemPremiumTimeSlot in checkPremiumFlagData.ReturnPremiumTimeSlot)
                        {
                            ReturnPremiumTimeSlotData returnPremiumTimeSlotData = new ReturnPremiumTimeSlotData()
                            {
                                AccessMode = itemPremiumTimeSlot.AccessMode.ToSafeString(),
                                AssignRule = itemPremiumTimeSlot.assignrule.ToSafeString(),
                                NumberOfDay = itemPremiumTimeSlot.numberofday.ToSafeString(),
                                NumberOfDisplay = itemPremiumTimeSlot.numberofdisplay.ToSafeString(),
                                NumberOfHour = itemPremiumTimeSlot.numberofhour.ToSafeString(),
                                NumberOfShift = itemPremiumTimeSlot.numberofshift.ToSafeString(),
                                PartnerSubtype = itemPremiumTimeSlot.PartnerSubtype.ToSafeString(),
                                ShiftType = itemPremiumTimeSlot.shifttype.ToSafeString()
                            };
                            returnPremiumTimeSlotDataList.Add(returnPremiumTimeSlotData);
                        }
                        //Response.ReturnCode = checkPremiumFlagData.ReturnCode;
                        //Response.ReturnMessage = checkPremiumFlagData.ReturnMessage;
                        Response.SubAccessMode = checkPremiumFlagData.SubAccessMode;
                        Response.RecurringCharge = checkPremiumFlagData.RecurringCharges;
                        Response.LocationCode = checkPremiumFlagData.LocationCodes;
                        Response.AccessMode = checkPremiumFlagData.AccessModes;
                        Response.ReturnPremiumTimeSlot = returnPremiumTimeSlotDataList;
                    }

                    HaveData = true;
                }

                if (!HaveData)
                {
                    Response.ReturnCode = "-1";
                    Response.ReturnMessage = "No data found.";
                }

                EndInterface(Response, log, query.TransactionID,
                            "Success", LogMsgDetail);
            }
            catch (Exception ex)
            {
                Response.ReturnCode = "-1";
                Response.ReturnMessage = ex.Message;
                Response.SubAccessMode = "";
                EndInterface(Response, log, query.TransactionID,
                            "Error", ex.Message);
            }
            return Response;
        }

        public CheckTimeSlotbySubTypeResponse CheckTimeSlotbySubType(CheckTimeSlotbySubTypeQuery query)
        {
            InterfaceLogCommand log = null;
            bool HaveData = false;
            CheckTimeSlotbySubTypeResponse Response = new CheckTimeSlotbySubTypeResponse();
            log = StartInterface(query,
                "CheckTimeSlotbySubType",
                query.TransactionID, "");
            try
            {
                CheckTimeslotBySubtypeModel checkTimeslotBySubtypeData = CheckTimeslotBySubtypeData(query.PartnerSubtype, query.AccessMode, query.TransactionID);
                if (checkTimeslotBySubtypeData != null && checkTimeslotBySubtypeData.ReturnCode == "0"
                    && checkTimeslotBySubtypeData.ReturnConfigTimeslot != null && checkTimeslotBySubtypeData.ReturnConfigTimeslot.Count > 0)
                {
                    List<ReturnConfigTimeslotData> returnConfigTimeslotDataList = new List<ReturnConfigTimeslotData>();
                    foreach (var itemConfigTimeslot in checkTimeslotBySubtypeData.ReturnConfigTimeslot)
                    {
                        ReturnConfigTimeslotData returnConfigTimeslotData = new ReturnConfigTimeslotData
                        {
                            AccessMode = itemConfigTimeslot.AccessMode,
                            AssignRule = itemConfigTimeslot.assignrule,
                            NumberOfDay = itemConfigTimeslot.numberofday,
                            NumberOfDisplay = itemConfigTimeslot.numberofdisplay,
                            NumberOfHour = itemConfigTimeslot.numberofhour,
                            NumberOfShift = itemConfigTimeslot.numberofshift,
                            PartnerSubtype = itemConfigTimeslot.PartnerSubtype,
                            ShiftType = itemConfigTimeslot.shifttype
                        };
                        returnConfigTimeslotDataList.Add(returnConfigTimeslotData);
                    }
                    Response.ReturnCode = checkTimeslotBySubtypeData.ReturnCode;
                    Response.ReturnMessage = checkTimeslotBySubtypeData.ReturnMessage;
                    Response.ReturnConfigTimeslot = returnConfigTimeslotDataList;
                    HaveData = true;
                }

                if (!HaveData)
                {
                    Response.ReturnCode = "-1";
                    Response.ReturnMessage = "No data found.";
                }

                EndInterface(Response, log, query.TransactionID,
                            "Success", Response.ReturnMessage);
            }
            catch (Exception ex)
            {
                Response.ReturnCode = "-1";
                Response.ReturnMessage = ex.Message;
                EndInterface(Response, log, query.TransactionID,
                            "Error", ex.Message);
            }
            return Response;
        }

        public CheckFMCPackageResponse CheckFMCPackage(CheckFMCPackageQuery query)
        {

            InterfaceLogCommand log = null;
            bool HaveData = false;
            bool HavePriceExclVat = false;
            string priceExclVat = "";
            CheckFMCPackageResponse Response = new CheckFMCPackageResponse();
            log = StartInterface(query,
                "CheckFMCPackage",
                query.TransactionID, "");
            try
            {
                var personalInformationQuery = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = query.MobileNo,
                    option = "2",
                    FullUrl = "FbbCpGwInterface"
                };
                List<evESQueryPersonalInformationModel> personalInformationResult = _queryProcessor.Execute(personalInformationQuery);
                if (personalInformationResult != null && personalInformationResult.Count > 0)
                {
                    foreach (var itemPersonalInformation in personalInformationResult)
                    {
                        if (itemPersonalInformation.productClass.ToSafeString() == "Main")
                        {
                            priceExclVat = itemPersonalInformation.priceExclVat.ToSafeString();
                            HavePriceExclVat = true;
                        }
                    }
                    if (HavePriceExclVat)
                    {
                        evESeServiceQueryMassCommonAccountInfoQuery evESeServiceQueryMassCommonAccountInfoQuery = new evESeServiceQueryMassCommonAccountInfoQuery
                        {
                            inOption = "2",
                            inMobileNo = query.MobileNo,
                            Page = "CheckFMCPackage",
                            Username = "WBBExternal",
                            ClientIP = "",
                            FullUrl = "FbbCpGwInterface"
                        };
                        evESeServiceQueryMassCommonAccountInfoModel MassCommonAccountInfoResult = _queryProcessor.Execute(evESeServiceQueryMassCommonAccountInfoQuery);
                        if (MassCommonAccountInfoResult != null && MassCommonAccountInfoResult.outErrorMessage != null && MassCommonAccountInfoResult.outErrorMessage == "")
                        {
                            string tmpProjectName = string.IsNullOrEmpty(MassCommonAccountInfoResult.projectName) ? "" : MassCommonAccountInfoResult.projectName;
                            string tmpPriceExclVat = string.IsNullOrEmpty(priceExclVat) ? "" : priceExclVat;
                            CheckFMCPackageModel checkFMCPackageData = CheckFMCPackageData(tmpPriceExclVat, tmpProjectName, query.SFFPromotionCode);

                            if (checkFMCPackageData != null && checkFMCPackageData.ReturnCode == "0")
                            {
                                Response.ReturnCode = checkFMCPackageData.ReturnCode;
                                Response.ReturnMessage = checkFMCPackageData.ReturnMessage;
                                Response.ProjectName = checkFMCPackageData.ProjectName;
                                Response.OntopPackage = checkFMCPackageData.OntopPackage;
                                HaveData = true;
                            }
                        }
                    }
                }

                if (!HaveData)
                {
                    Response.ReturnCode = "-1";
                    Response.ReturnMessage = "No data found.";
                }

                EndInterface(Response, log, query.TransactionID,
                            "Success", Response.ReturnMessage);
            }
            catch (Exception ex)
            {
                Response.ReturnCode = "-1";
                Response.ReturnMessage = ex.Message;
                EndInterface(Response, log, query.TransactionID,
                            "Error", ex.Message);
            }
            return Response;
        }
        #endregion

        #region R19.12 12/2019
        public QueryOrderByASCEmpIdResponse QueryOrderByASCEmpId(GetOrderByASCEmpIdQuery query)
        {
            QueryOrderByASCEmpIdResponse response = new QueryOrderByASCEmpIdResponse();
            List<OrderDetail> responseOrderDetail = new List<OrderDetail>();
            try
            {
                // get config 
                var getLovConfig = Get_FBB_CFG_LOV("FBB_CONSTANT", "HVR_USE_FLAG").ToList();
                // get flag
                var flagHVR = getLovConfig.Where(p => p.LovValue1 == "Y").Select(o => o.LovValue1).FirstOrDefault() ?? "N";

                GetOrderByASCEmpIdModel GetOrderByASCEmpIdResult = new GetOrderByASCEmpIdModel();

                if (flagHVR == "Y")
                {
                    GetOrderByASCEmpIdDataHVRQuery queryData = new GetOrderByASCEmpIdDataHVRQuery()
                    {
                        TransactionID = query.TransactionID.ToSafeString(),
                        ASCCode = query.ASCCode.ToSafeString(),
                        EmployeeId = query.EmployeeId.ToSafeString(),
                        DateFrom = query.DateFrom.ToSafeString(),
                        DateTo = query.DateTo.ToSafeString(),
                        LocationCode = query.LocationCode.ToSafeString()
                    };

                    GetOrderByASCEmpIdResult = _queryProcessor.Execute(queryData);
                }
                else
                {
                    GetOrderByASCEmpIdDataQuery queryData = new GetOrderByASCEmpIdDataQuery()
                    {
                        TransactionID = query.TransactionID.ToSafeString(),
                        ASCCode = query.ASCCode.ToSafeString(),
                        EmployeeId = query.EmployeeId.ToSafeString(),
                        DateFrom = query.DateFrom.ToSafeString(),
                        DateTo = query.DateTo.ToSafeString(),
                        LocationCode = query.LocationCode.ToSafeString()
                    };

                    GetOrderByASCEmpIdResult = _queryProcessor.Execute(queryData);
                }
                
                if (GetOrderByASCEmpIdResult != null && GetOrderByASCEmpIdResult.ReturnCode == "0")
                {
                    if (GetOrderByASCEmpIdResult.ReturnOrderDetail != null && GetOrderByASCEmpIdResult.ReturnOrderDetail.Count > 0)
                    {
                        responseOrderDetail = GetOrderByASCEmpIdResult.ReturnOrderDetail.Select(t => new OrderDetail()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            RegisterDate = t.RegisterDate.ToSafeString(),
                            CustomerName = t.CustomerName.ToSafeString(),
                            PrivilegeNo = t.PrivilegeNo.ToSafeString(),
                            MainPackage = t.MainPackage.ToSafeString(),
                            NotifyDetail = t.NotifyDetail.ToSafeString(),
                            NoteDetail = t.NoteDetail.ToSafeString(),
                            OrderExpireDate = t.OrderExpireDate.ToSafeString(),
                            AppointmentDate = t.AppointmentDate.ToSafeString(),
                            CancelDate = t.CancelDate.ToSafeString(),
                            RegisterChannel = t.RegisterChannel.ToSafeString(),
                            OrderStatus = t.OrderStatus
                        }).ToList();
                    }

                    response = new QueryOrderByASCEmpIdResponse()
                    {
                        ReturnCode = GetOrderByASCEmpIdResult.ReturnCode.ToSafeString(),
                        ReturnMessage = GetOrderByASCEmpIdResult.ReturnMessage.ToSafeString(),
                        TotalKeyIn = GetOrderByASCEmpIdResult.TotalKeyIn.ToSafeString(),
                        TotalComplete = GetOrderByASCEmpIdResult.TotalComplete.ToSafeString(),
                        TotalCancel = GetOrderByASCEmpIdResult.TotalCancel.ToSafeString(),
                        ReturnOrderDetail = responseOrderDetail
                    };
                }
                else
                {
                    response = new QueryOrderByASCEmpIdResponse()
                    {
                        ReturnCode = "-1",
                        ReturnMessage = "No data found.",
                        TotalKeyIn = "",
                        TotalComplete = "",
                        TotalCancel = "",
                        ReturnOrderDetail = new List<OrderDetail>()
                    };
                }
            }
            catch (Exception ex)
            {
                response = new QueryOrderByASCEmpIdResponse()
                {
                    ReturnCode = "-1",
                    ReturnMessage = ex.Message,
                    TotalKeyIn = "",
                    TotalComplete = "",
                    TotalCancel = "",
                    ReturnOrderDetail = new List<OrderDetail>()
                };
            }

            return response;
        }

        public QueryOrderDetailResponse QueryOrderDetail(GetOrderDetailQuery query)
        {
            QueryOrderDetailResponse response = new QueryOrderDetailResponse();

            try
            {
                // get config
                var getLovConfig = Get_FBB_CFG_LOV("FBB_CONSTANT", "HVR_USE_FLAG").ToList();
                // get flag
                var flagHVR = getLovConfig.Where(p => p.LovValue1 == "Y").Select(o => o.LovValue1).FirstOrDefault() ?? "N";

                GetOrderDetailModel GetOrderDetailResult = new GetOrderDetailModel();

                if (flagHVR == "Y")
                {
                    GetOrderDetailDataHVRQuery queryData = new GetOrderDetailDataHVRQuery()
                    {
                        TransactionID = query.TransactionID.ToSafeString(),
                        CustomerName = query.CustomerName.ToSafeString(),
                        CustomerLastName = query.CustomerLastName.ToSafeString(),
                        ListOrder = query.ListOrder,
                        ListCardNo = query.ListCardNo,
                        ListNonMobileNo = query.ListNonMobileNo,
                        ListContactMobileNo = query.ListContactMobileNo
                    };

                    GetOrderDetailResult = _queryProcessor.Execute(queryData);
                }
                else
                {
                    GetOrderDetailDataQuery queryData = new GetOrderDetailDataQuery()
                    {
                        TransactionID = query.TransactionID.ToSafeString(),
                        CustomerName = query.CustomerName.ToSafeString(),
                        CustomerLastName = query.CustomerLastName.ToSafeString(),
                        ListOrder = query.ListOrder,
                        ListCardNo = query.ListCardNo,
                        ListNonMobileNo = query.ListNonMobileNo,
                        ListContactMobileNo = query.ListContactMobileNo
                    };

                    GetOrderDetailResult = _queryProcessor.Execute(queryData);
                }
                
                if (GetOrderDetailResult != null)
                {
                    List<OrderCustomer> responseOrderCustomerData = new List<OrderCustomer>();
                    List<OrderPackage> responseOrderPackageData = new List<OrderPackage>();
                    List<OrderContact> responseOrderContactData = new List<OrderContact>();
                    List<OrderBillMedia> responseOrderBillMediaData = new List<OrderBillMedia>();
                    List<BillingAddressItem> responseBillingAddressData = new List<BillingAddressItem>();
                    List<InstallAddressItem> responseInstallAddressData = new List<InstallAddressItem>();
                    List<OrderDocument> responseOrderDocumentData = new List<OrderDocument>();
                    List<ForOfficer> responseForOfficerData = new List<ForOfficer>();

                    if (GetOrderDetailResult.ReturnOrderCustomer != null && GetOrderDetailResult.ReturnOrderCustomer.Count > 0)
                    {
                        responseOrderCustomerData = GetOrderDetailResult.ReturnOrderCustomer.Select(t => new OrderCustomer()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            CustomerName = t.CustomerName.ToSafeString(),
                            CustomerId = t.CustomerId.ToSafeString(),
                            RegisterDate = t.RegisterDate.ToSafeString(),
                            OrderStatus = t.OrderStatus.ToSafeString(),
                            PrivilegeNo = t.PrivilegeNo.ToSafeString(),
                            NonMobileNo = t.NonMobileNo.ToSafeString(),
                            RegisterChannel = t.RegisterChannel.ToSafeString(),
                            NotifyDetail = t.NotifyDetail.ToSafeString(),
                            NoteDetail = t.NoteDetail.ToSafeString(),
                            OrderExpireDate = t.OrderExpireDate.ToSafeString(),
                            CancelDate = t.CancelDate.ToSafeString()
                        }).ToList();
                    }

                    if (GetOrderDetailResult.ReturnOrderPackage != null && GetOrderDetailResult.ReturnOrderPackage.Count > 0)
                    {
                        responseOrderPackageData = GetOrderDetailResult.ReturnOrderPackage.Select(t => new OrderPackage()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            SFFPromotionCode = t.SFFPromotionCode.ToSafeString(),
                            SFFWordInStatementTha = t.SFFWordInStatementTha.ToSafeString(),
                            SFFWordInStatementEng = t.SFFWordInStatementEng.ToSafeString(),
                            PackageType = t.PackageType.ToSafeString(),
                            PackageTypeDesc = t.PackageTypeDesc.ToSafeString()
                        }).ToList();
                    }

                    if (GetOrderDetailResult.ReturnOrderContact != null && GetOrderDetailResult.ReturnOrderContact.Count > 0)
                    {
                        responseOrderContactData = GetOrderDetailResult.ReturnOrderContact.Select(t => new OrderContact()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            AppointmentDate = t.AppointmentDate.ToSafeString(),
                            TimeSlot = t.TimeSlot.ToSafeString(),
                            ContactNo = t.ContactNo.ToSafeString(),
                            WaitingInstallDate = t.WaitingInstallDate.ToSafeString(),
                            WaitingTimeSlot = t.WaitingTimeSlot.ToSafeString()
                        }).ToList();
                    }

                    if (GetOrderDetailResult.ReturnOrderBillMedia != null && GetOrderDetailResult.ReturnOrderBillMedia.Count > 0)
                    {
                        responseOrderBillMediaData = GetOrderDetailResult.ReturnOrderBillMedia.Select(t => new OrderBillMedia()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            BillMedia = t.BillMedia.ToSafeString(),
                            MobileNo = t.MobileNo.ToSafeString()
                        }).ToList();
                    }

                    if (GetOrderDetailResult.ReturnBillingAddress != null && GetOrderDetailResult.ReturnBillingAddress.Count > 0)
                    {
                        responseBillingAddressData = GetOrderDetailResult.ReturnBillingAddress.Select(t => new BillingAddressItem()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            BillingAddress = t.BillingAddress.ToSafeString()
                        }).ToList();
                    }

                    if (GetOrderDetailResult.ReturnInstallAddress != null && GetOrderDetailResult.ReturnInstallAddress.Count > 0)
                    {
                        responseInstallAddressData = GetOrderDetailResult.ReturnInstallAddress.Select(t => new InstallAddressItem()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            InstallAddress = t.InstallAddress.ToSafeString()
                        }).ToList();
                    }

                    if (GetOrderDetailResult.ReturnOrderDocument != null && GetOrderDetailResult.ReturnOrderDocument.Count > 0)
                    {
                        responseOrderDocumentData = GetOrderDetailResult.ReturnOrderDocument.Select(t => new OrderDocument()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            FileName = t.FileName.ToSafeString()
                        }).ToList();
                    }

                    if (GetOrderDetailResult.ReturnForOfficer != null && GetOrderDetailResult.ReturnForOfficer.Count > 0)
                    {
                        responseForOfficerData = GetOrderDetailResult.ReturnForOfficer.Select(t => new ForOfficer()
                        {
                            OrderNo = t.OrderNo.ToSafeString(),
                            LocationName = t.LocationName.ToSafeString(),
                            ASCCode = t.ASCCode.ToSafeString(),
                            EmployeeId = t.EmployeeId.ToSafeString(),
                            CSNote = t.CSNote.ToSafeString()
                        }).ToList();
                    }

                    response = new QueryOrderDetailResponse()
                    {
                        ReturnCode = GetOrderDetailResult.ReturnCode.ToSafeString(),
                        ReturnMessage = GetOrderDetailResult.ReturnMessage.ToSafeString(),
                        ReturnOrderCustomer = responseOrderCustomerData,
                        ReturnOrderPackage = responseOrderPackageData,
                        ReturnOrderContact = responseOrderContactData,
                        ReturnOrderBillMedia = responseOrderBillMediaData,
                        ReturnBillingAddress = responseBillingAddressData,
                        ReturnInstallAddress = responseInstallAddressData,
                        ReturnOrderDocument = responseOrderDocumentData,
                        ReturnForOfficer = responseForOfficerData
                    };
                }
                else
                {
                    response = new QueryOrderDetailResponse()
                    {
                        ReturnCode = "-1",
                        ReturnMessage = "No data found.",
                    };
                }
            }
            catch (Exception)
            {
                response = new QueryOrderDetailResponse()
                {
                    ReturnCode = "-1",
                    ReturnMessage = "No data found.",
                };
            }
            return response;
        }

        public UpdateDocumentOrCSNoteByNotifyOrderResponse UpdateDocumentOrCSNoteByNotifyOrder(UpdateDocumentOrCSNoteByNotifyOrderCommand command)
        {
            UpdateDocumentOrCSNoteByNotifyOrderResponse response = new UpdateDocumentOrCSNoteByNotifyOrderResponse();
            try
            {
                bool checkSaveSCNote = true;
                if (command.CSNote != null && command.CSNote != "")
                {
                    SetCSNoteCommand commandCSNote = new SetCSNoteCommand()
                    {
                        in_order_no = command.OrderNo.ToSafeString(),
                        in_cs_note = command.CSNote.ToSafeString(),
                        in_p_user = command.UserName.ToSafeString()
                    };

                    _setCSNoteCommand.Handle(commandCSNote);

                    if (commandCSNote.out_return_code != "0")
                    {
                        checkSaveSCNote = false;
                    }
                    response.ReturnCode = commandCSNote.out_return_code;
                    response.ReturnMessage = commandCSNote.out_return_error;
                }

                if (checkSaveSCNote && command.ListFile != null && command.ListFile.Count > 0)
                {
                    bool checkFileName = true;
                    foreach (var item in command.ListFile)
                    {
                        if (item.FileName.ToSafeString() == "")
                        {
                            checkFileName = false;
                        }
                    }

                    if (checkFileName)
                    {
                        GenerateFilenamesForWorkflowQuery query = new GenerateFilenamesForWorkflowQuery()
                        {
                            in_order_no = command.OrderNo.ToSafeString(),
                            in_filenames = command.ListFile.Select(t => new in_filenames_data { FILE_NAME = t.FileName }).ToList(),
                            in_user_name = command.UserName.ToSafeString()
                        };
                        GenerateFilenamesForWorkflowModel executeResult = _queryProcessor.Execute(query);
                        if (executeResult != null && executeResult.out_return_code == "0" && executeResult.out_result != null && executeResult.out_result.Count > 0)
                        {
                            response.ReturnListFileName = executeResult.out_result.Select(t => new ReturnListFileNameData { FileName = t.file_name }).ToList();
                        }
                        response.ReturnCode = executeResult.out_return_code;
                        response.ReturnMessage = "";
                    }
                }

                if (checkSaveSCNote && ((!String.IsNullOrEmpty(command.CustomerPurge)) || (!String.IsNullOrEmpty(command.ExceptEntryFee)) || (!String.IsNullOrEmpty(command.SecondInstallation))))
                {
                    SetCustomerVerificationCommand commandSetCustomerVerification = new SetCustomerVerificationCommand()
                    {
                        OrderNo = command.OrderNo.ToSafeString(),
                        CustomerPurge = command.CustomerPurge.ToSafeString(),
                        ExceptEntryFee = command.ExceptEntryFee.ToSafeString(),
                        SecondInstallation = command.SecondInstallation.ToSafeString()
                    };

                    _setCustomerVerificationCommand.Handle(commandSetCustomerVerification);

                    if (commandSetCustomerVerification.return_code != "0")
                    {
                        checkSaveSCNote = false;
                    }

                    response.ReturnCode = commandSetCustomerVerification.return_code;
                    response.ReturnMessage = commandSetCustomerVerification.return_msg;

                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = "-1";
                response.ReturnMessage = ex.Message;
            }

            return response;
        }

        public QuestionCustomerInsightResponse QueryQuestionCustomerInsight(QuestionCustomerInsightQuery query)
        {
            QuestionCustomerInsightResponse response = new QuestionCustomerInsightResponse();
            response.ListQuestion = new List<ListQuestionData>();
            response.ListAnswer = new List<ListAnswerData>();
            response.ListSubAnswer = new List<ListSubAnswerData>();

            GetQuestionByChannelQuery queryData = new GetQuestionByChannelQuery()
            {
                MobileNo = query.TransactionID.ToSafeString(),
                p_channel = query.Channel.ToSafeString(),
                p_order_type = query.OrderType.ToSafeString(),
                p_technology = query.Technology.ToSafeString()
            };

            try
            {
                GetQuestionByChannelModel queryResult = _queryProcessor.Execute(queryData);

                if (queryResult != null)
                {
                    if (queryResult.questionDatas != null && queryResult.questionDatas.Count > 0)
                    {
                        response.ListQuestion = queryResult.questionDatas.Select(p => new ListQuestionData
                        {
                            group_id = p.GROUP_ID.ToSafeString(),
                            group_name_th = p.GROUP_NAME_EN.ToSafeString(),
                            group_name_en = p.GROUP_NAME_EN.ToSafeString(),
                            group_seq = p.GROUP_SEQ.ToSafeString(),
                            question_id = p.QUESTION_ID.ToSafeString(),
                            question_seq = p.QUESTION_SEQ.ToSafeString(),
                            question_th = p.QUESTION_TH.ToSafeString(),
                            question_en = p.QUESTION_EN.ToSafeString(),
                            question_desc_th = p.QUESTION_DESC_TH.ToSafeString(),
                            question_desc_en = p.QUESTION_DESC_EN.ToSafeString(),
                            require_answer_flag = p.REQUIRE_ANSWER_FLAG.ToSafeString(),
                            check_action_flag = p.CHECK_ACTION_FLAG.ToSafeString(),
                            channel = p.CHANNEL.ToSafeString(),
                            technology = p.TECHNOLOGY.ToSafeString()
                        }).ToList();
                    }
                    if (queryResult.answerDatas != null && queryResult.answerDatas.Count > 0)
                    {
                        response.ListAnswer = queryResult.answerDatas.Select(p => new ListAnswerData
                        {
                            question_id = p.QUESTION_ID.ToSafeString(),
                            answer_id = p.ANSWER_ID.ToSafeString(),
                            answer_seq = p.ANSWER_SEQ.ToSafeString(),
                            answer_th = p.ANSWER_TH.ToSafeString(),
                            answer_en = p.ANSWER_EN.ToSafeString(),
                            parent_answer_id = p.PARENT_ANSWER_ID.ToSafeString(),
                            action_wfm = p.ACTION_WFM.ToSafeString(),
                            action_foa = p.ACTION_FOA.ToSafeString(),
                            display_type = p.DISPLAY_TYPE.ToSafeString(),
                            action = p.ACTION.ToSafeString(),
                            value = p.VALUE.ToSafeString()
                        }).ToList();
                    }
                    if (queryResult.subAnswerDatas != null && queryResult.subAnswerDatas.Count > 0)
                    {
                        response.ListSubAnswer = queryResult.subAnswerDatas.Select(p => new ListSubAnswerData
                        {
                            answer_id = p.ANSWER_ID.ToSafeString(),
                            answer_value_th = p.ANSWER_VALUE_TH.ToSafeString(),
                            answer_value_en = p.ANSWER_VALUE_EN.ToSafeString(),
                        }).ToList();
                    }
                    response.ReturnCode = queryResult.ret_code;
                    response.ReturnMessage = queryResult.ret_message;
                }
                else
                {
                    response.ReturnCode = "-1";
                    response.ReturnMessage = "No data found.";
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = "-1";
                response.ReturnMessage = ex.Message;
            }

            return response;
        }

        public FBBPendingDeductionResponse FBBPendingDeduction(FBBPendingDeductionModel command)
        {
            FBBPendingDeductionResponse response = new FBBPendingDeductionResponse();

            SavePendingDeductionCommand commandEx = new SavePendingDeductionCommand()
            {
                p_transaction_id = command.TransactionID.ToSafeString(),
                p_mobile_no = command.MobileNo.ToSafeString(),
                p_ba_no = command.BANo.ToSafeString(),
                p_paid_amt = command.PaidAMT.ToSafeString(),
                p_channel = command.Channel,
                p_merchant_id = command.MerchantID.ToSafeString(),
                p_payment_method_id = command.PaymentMethodId.ToSafeString()
            };

            _savePendingDeductionCommand.Handle(commandEx);

            response.ReturnCode = commandEx.ret_code;
            response.ReturnMessage = commandEx.ret_message;

            return response;
        }
        #endregion

        #region R20.9
        public ListMaxMeshInstallmentRespond ListMaxMeshInstallment(ListMaxMeshInstallmentQuery query)
        {
            var response = new ListMaxMeshInstallmentRespond();
            var lovConfig = _queryProcessor.Execute(new GetLovQuery
            {
                LovType = "FBB_MESH_CONFIG",

            });

            try
            {
                var queryResult = new ListMaxMeshInstallmentRespond();
                List<ListMaxMeshArray> lstrest = new List<ListMaxMeshArray>();

                switch (query.OPTION)
                {
                    case "0":
                        {
                            string[] arr = { "MAX_MESH_NEW", "MAX_MESH_EX" };
                            var lov = lovConfig.Where(w => arr.Contains(w.Name));
                            foreach (var v in lov)
                            {
                                var res = new ListMaxMeshArray();
                                res.EVENT = (v.Name == "MAX_MESH_NEW" ? "NEW" : "EXISTING");
                                res.MAXMESH = v.LovValue1;
                                lstrest.Add(res);
                            }

                            break;
                        }
                    case "1":
                        {
                            var lov = lovConfig.Where(w => w.Name == "MAX_MESH_NEW");
                            foreach (var v in lov)
                            {
                                var res = new ListMaxMeshArray();
                                res.EVENT = "NEW";
                                res.MAXMESH = v.LovValue1;
                                lstrest.Add(res);
                            }
                            break;
                        }
                    case "2":
                        {
                            var lov = lovConfig.Where(w => w.Name == "MAX_MESH_EX");
                            foreach (var v in lov)
                            {
                                var res = new ListMaxMeshArray();
                                res.EVENT = "EXISTING";
                                res.MAXMESH = v.LovValue1;
                                lstrest.Add(res);
                            }
                            break;
                        }
                }

                //GetQuestionByChannelModel queryResult = _queryProcessor.Execute(queryData);

                if (queryResult != null)
                {
                    response.ReturnCode = "0";
                    response.ReturnMessage = "";
                    response.ListMaxMeshArray = lstrest;
                }
                else
                {
                    response.ReturnCode = "-1";
                    response.ReturnMessage = "No data found.";
                    response.ListMaxMeshArray = null;
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = "-1";
                response.ReturnMessage = ex.Message;
                response.ListMaxMeshArray = null;
            }

            return response;
        }
        #endregion

        #region R21.2
        //MicrositeWS
        public MicrositeWSResponse MicrositeWS(MicrositeWSModel command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<MicrositeWSModel>(command, "MicrositeWS", command.OrderNo, "");

                var Command = new MicrositeWSCommand
                {
                    P_ORDER_NO = command.OrderNo,
                    P_USER = command.User,
                    P_SEGMENT = command.Segment,
                    P_NUMBER_OF_USER = command.NumberOfUser,
                    P_RESIDENTIAL = command.Residential,
                    P_TYPE_OF_USER = command.TypeOfUser,
                    P_PACKAGE_NAME = command.PackageName,
                    P_PACKAGE_CODE = command.PackageCode,
                    P_SPEED = command.Speed,
                    P_PLAYBOX_BUNDLE = command.PlayboxBundle,
                    P_PLAYBOX_ADDON = command.PlayboxAddon,
                    P_ROUTER_BUNDLE = command.RouterBundle,
                    P_ROUTER_ADDON = command.RouterAddon,
                    P_PRICE = command.Price,
                    P_FIRST_NAME = command.FirstName,
                    P_LAST_NAME = command.LastName,
                    P_TELEPHONE = command.Telephone,
                    P_EMAIL = command.Email,
                    P_ADDRESS = command.Adress,
                    P_SUB_DISTRICT = command.SubDistrict,
                    P_DISTRICT = command.District,
                    P_PROVINCE = command.Province,
                    P_ZIPCODE = command.Zipcode,
                    P_MEDIA_STREAMING = command.MediaStreaming,
                    P_USER_JOURNEY = command.UserJourney,
                    P_CID = command.CID,
                    P_STATUS_COMPLETE = command.StatusComplete
                };
                _micrositeWSCommand.Handle(Command);
                if (Command.p_return_code == "0")
                {
                    var success = new MicrositeWSResponse
                    {
                        ReturnCode = Command.p_return_code,
                        ReturnMessage = Command.p_return_message
                    };
                    EndInterface<MicrositeWSResponse>(success, log, command.OrderNo, "Success", "");
                    return success;

                }
                else
                {
                    var error = new MicrositeWSResponse
                    {
                        ReturnCode = Command.p_return_code,
                        ReturnMessage = Command.p_return_message
                    };
                    EndInterface<MicrositeWSResponse>(error, log, command.OrderNo, error.ReturnMessage, error.ReturnMessage);

                    return error;
                }
            }
            catch (System.Exception ex)
            {
                var error = new MicrositeWSResponse
                {
                    ReturnCode = "-1",
                    ReturnMessage = "MicrositeWS error"
                };
                EndInterface<MicrositeWSResponse>(error, log, command.OrderNo, ex.Message, error.ReturnMessage);
                return error;
            }
        }

        //MicrositeAction
        public MicrositeActionResponse MicrositeAction(MicrositeActionModel command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<MicrositeActionModel>(command, "MicrositeAction", command.OrderNo, "");

                var Command = new MicrositeActionCommand
                {
                    P_ORDER_NO = command.OrderNo,
                    P_ORDER_CHANNEL = command.OrderChannel,
                    P_IS_CONTACT_CUSTOMER = command.IsContactCustomer,
                    P_IS_IN_COVERAGE = command.IsInCoverage,
                    P_USER_ACTION = command.UserAction,
                    P_DATE_ACTION = command.DateAction,
                    P_ORDER_PRE_REGISTER = command.OrderPreRegister,
                    P_STATUS_ORDER = command.StatusOrder,
                    P_REMARK_NOTE = command.RemarkNote
                };
                _micrositeActionCommand.Handle(Command);
                if (Command.p_return_code == "0")
                {
                    var success = new MicrositeActionResponse
                    {
                        ReturnCode = Command.p_return_code,
                        ReturnMessage = Command.p_return_message
                    };
                    EndInterface<MicrositeActionResponse>(success, log, command.OrderNo, "Success", "");
                    return success;

                }
                else
                {
                    var error = new MicrositeActionResponse
                    {
                        ReturnCode = Command.p_return_code,
                        ReturnMessage = Command.p_return_message
                    };
                    EndInterface<MicrositeActionResponse>(error, log, command.OrderNo, error.ReturnMessage, error.ReturnMessage);

                    return error;
                }
            }
            catch (System.Exception ex)
            {
                var error = new MicrositeActionResponse
                {
                    ReturnCode = "-1",
                    ReturnMessage = "MicrositeAction error"
                };
                EndInterface<MicrositeActionResponse>(error, log, command.OrderNo, ex.Message, error.ReturnMessage);
                return error;
            }
        }

        //InsertCoverageRusult
        public InsertCoverageRusultResponse InsertCoverageRusult(InsertCoverageRusultModel command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<InsertCoverageRusultModel>(command, "InsertCoverageRusult", command.Channel, "");

                if (string.IsNullOrEmpty(command.Channel.ToSafeString()) || string.IsNullOrEmpty(command.AddressType.ToSafeString()) || string.IsNullOrEmpty(command.PostalCode.ToSafeString())
                    || string.IsNullOrEmpty(command.District.ToSafeString()) || string.IsNullOrEmpty(command.SubDistrict.ToSafeString()) || string.IsNullOrEmpty(command.Language.ToSafeString())
                    || string.IsNullOrEmpty(command.AddressNo.ToSafeString()) || string.IsNullOrEmpty(command.CoverageFlag.ToSafeString()) || string.IsNullOrEmpty(command.IsPartner.ToSafeString())
                    || string.IsNullOrEmpty(command.FirstName.ToSafeString()) || string.IsNullOrEmpty(command.LastName.ToSafeString()) || string.IsNullOrEmpty(command.ContactNumber.ToSafeString()))
                {
                    var error = new InsertCoverageRusultResponse
                    {
                        ReturnCode = "-1",
                        ReturnMessage = "Missing Parameter"
                    };
                    EndInterface<InsertCoverageRusultResponse>(error, log, command.Channel, error.ReturnMessage, error.ReturnMessage);

                    return error;
                }
                else
                {
                    var Command = new InsertCoverageRusultCommand
                    {
                        p_channel = command.Channel,
                        p_address_type = command.AddressType,
                        p_postal_code = command.PostalCode,
                        p_district = command.District,
                        p_sub_district = command.SubDistrict,
                        p_language = command.Language,
                        p_building_name = command.BuildingName,
                        p_building_no = command.BuildingNo,
                        p_phone_flag = command.PhoneFlag,
                        p_floor_no = command.FloorNo,
                        p_address_no = command.AddressNo,
                        p_moo = command.Moo,
                        p_soi = command.Soi,
                        p_road = command.Road,
                        p_latitude = command.Latitude,
                        p_longitude = command.Longitude,
                        p_unit_no = command.UnitNo,
                        p_coverage_flag = command.CoverageFlag,
                        p_address_id = command.AddressID,
                        p_is_partner = command.IsPartner,
                        p_partner_name = command.PartnerName,
                        p_firstname = command.FirstName,
                        p_lastname = command.LastName,
                        p_contactnumber = command.ContactNumber,
                        p_producttype = command.ProductType,
                        p_owner_product = command.OwnerProduct,
                        p_splitter_name = command.SplitterName,
                        p_distance = command.Distance,
                        p_contact_email = command.ContactEmail,
                        p_contact_line_id = command.ContactLineID,
                        p_location_code = command.LocationCode,
                        p_asc_code = command.AscCode,
                        p_employee_id = command.EmployeeID,
                        p_location_name = command.LocationName,
                        p_sub_region = command.SubRegion,
                        p_region_name = command.RegionName,
                        p_asc_name = command.AscName,
                        p_sale_name = command.SaleName,
                        p_channel_name = command.ChannelName,
                        p_sale_channel = command.SaleChannel,
                        P_ADDRESS_TYPE_DTL = command.AddressTypeDTL,
                        P_REMARK = command.Remark,
                        P_TECHNOLOGY = command.Technology,
                        P_PROJECTNAME = command.ProjectName,
                        //R24.01 Add coverage
                        P_CoverageArea = command.CoverageArea,
                        P_NetworkProvider = command.NetworkProvider,
                        P_Region = command.Region,
                        P_CoverageStatus = command.CoverageStatus,
                        P_CoverageSubstatus = command.CoverageSubstatus,
                        P_CoverageGroupOwner = command.CoverageGroupOwner,
                        P_CoverageContactName = command.CoverageContactName,
                        P_CoverageContactEmail = command.CoverageContactEmail,
                        P_CoverageContactTel = command.CoverageContactTel
                    };
                    _insertCoverageRusultCommand.Handle(Command);
                    if (Command.ret_code == "0")
                    {
                        var success = new InsertCoverageRusultResponse
                        {
                            ReturnCode = Command.ret_code,
                            ReturnMessage = Command.ret_message
                        };
                        EndInterface<InsertCoverageRusultResponse>(success, log, command.Channel, "Success", "");
                        return success;

                    }
                    else
                    {
                        var error = new InsertCoverageRusultResponse
                        {
                            ReturnCode = Command.ret_code,
                            ReturnMessage = Command.ret_message
                        };
                        EndInterface<InsertCoverageRusultResponse>(error, log, command.Channel, error.ReturnMessage, error.ReturnMessage);

                        return error;
                    }
                }
            }
            catch (System.Exception ex)
            {
                var error = new InsertCoverageRusultResponse
                {
                    ReturnCode = "-1",
                    ReturnMessage = "InsertCoverageRusult error"
                };
                EndInterface<InsertCoverageRusultResponse>(error, log, command.Channel, ex.Message, error.ReturnMessage);
                return error;
            }
        }

        //UpdateCoverageRusult
        public UpdateCoverageRusultResponse UpdateCoverageResult(UpdateCoverageRusultModel command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<UpdateCoverageRusultModel>(command, "UpdateCoverageRusult", command.Channel, "");

                if (string.IsNullOrEmpty(command.OrderNo.ToSafeString()) || string.IsNullOrEmpty(command.Channel.ToSafeString()) || string.IsNullOrEmpty(command.StatusPlan.ToSafeString())
                    || string.IsNullOrEmpty(command.UserVerify.ToSafeString()) || string.IsNullOrEmpty(command.FlagVerify.ToSafeString()) || string.IsNullOrEmpty(command.DateVerify.ToSafeString()))
                {
                    UpdateCoverageRusultResponse error = new UpdateCoverageRusultResponse
                    {
                        ReturnCode = "-1",
                        ReturnMessage = "Missing Parameter"
                    };
                    EndInterface<UpdateCoverageRusultResponse>(error, log, command.Channel, error.ReturnMessage, error.ReturnMessage);

                    return error;
                }
                else
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime DateVerify = new DateTime();
                    if (DateTime.TryParseExact(command.DateVerify, "dd/MM/yyyy HH:mm:ss", provider, DateTimeStyles.None, out DateVerify))
                    {
                        UpdateCoverageRusultCommand Command = new UpdateCoverageRusultCommand
                        {
                            p_order_no = command.OrderNo,
                            p_channel = command.Channel,
                            p_status_plan = command.StatusPlan,
                            p_user_verify = command.UserVerify,
                            p_flag_verify = command.FlagVerify,
                            p_date_verify = command.DateVerify,
                            p_remark = command.Remark
                        };
                        _updateCoverageRusultCommand.Handle(Command);
                        if (Command.o_return_code == "0")
                        {
                            UpdateCoverageRusultResponse success = new UpdateCoverageRusultResponse
                            {
                                ReturnCode = Command.o_return_code,
                                ReturnMessage = Command.o_return_message
                            };
                            EndInterface<UpdateCoverageRusultResponse>(success, log, command.Channel, "Success", "");
                            return success;

                        }
                        else
                        {
                            UpdateCoverageRusultResponse error = new UpdateCoverageRusultResponse
                            {
                                ReturnCode = Command.o_return_code,
                                ReturnMessage = Command.o_return_message
                            };
                            EndInterface<UpdateCoverageRusultResponse>(error, log, command.Channel, error.ReturnMessage, error.ReturnMessage);

                            return error;
                        }
                    }
                    else
                    {
                        UpdateCoverageRusultResponse error = new UpdateCoverageRusultResponse
                        {
                            ReturnCode = "-1",
                            ReturnMessage = "Missing Parameter DateVerify DateTime Format."
                        };
                        EndInterface<UpdateCoverageRusultResponse>(error, log, command.Channel, error.ReturnMessage, error.ReturnMessage);

                        return error;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UpdateCoverageRusultResponse error = new UpdateCoverageRusultResponse
                {
                    ReturnCode = "-1",
                    ReturnMessage = "UpdateCoverageRusult error"
                };
                EndInterface<UpdateCoverageRusultResponse>(error, log, command.Channel, ex.Message, error.ReturnMessage);
                return error;
            }
        }


        //PermissionUserACIM
        public PermissionUserResponse PermissionuserACIM(PermissionUserModel command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterfaceACIM<PermissionUserModel>(command, "PermissionUserACIM", command.TRANSACTION_NO, "");

                //เช็ค Action ว่าใช่ A, U, D ไหม
                if (command.ACTION == "A" || command.ACTION == "U" || command.ACTION == "D" || command.ACTION == "")
                {
                    //check conditions here
                    List<DETAIL_USER> list = (from x in command.USER_ARRAY
                                              where x.USER_NAME != "" && x.ROLE != "" && command.TRANSACTION_NO != "" && command.ACTION != ""
                                              select x).ToList();
                    //เช็คว่า USER_NAME, ROLE, TRANSACTION_NO, ACTION เป็น null ไหม
                    if (list.Count == command.USER_ARRAY.Count)
                    {
                        // List
                        var Command = new PermissionUserCommand
                        {
                            P_TRANSACTION_NO = command.TRANSACTION_NO,
                            P_ACTION = command.ACTION,
                            P_FBBOR045_ACIM_ARRAY = list
                        };
                        _permissionuserCommand.Handle(Command);

                        if (Command.P_RETURN_CODE.ToSafeString() == "20000")
                        {
                            Command.IO_PROCESS_FAIL = null;
                            var success = new PermissionUserResponse
                            {
                                ReturnCode = Command.P_RETURN_CODE.ToSafeString(),
                                ReturnMessage = Command.P_RETURN_MESSAGE.ToSafeString(),
                                USER_ARRAY = Command.IO_PROCESS_FAIL
                            };
                            EndInterfaceACIM<PermissionUserResponse>(success, log, success.ReturnCode, success.ReturnMessage);
                            return success;

                        }
                        else
                        {
                            var error = new PermissionUserResponse
                            {
                                ReturnCode = Command.P_RETURN_CODE.ToSafeString(),
                                ReturnMessage = Command.P_RETURN_MESSAGE.ToSafeString(),
                                USER_ARRAY = Command.IO_PROCESS_FAIL.ToList()
                            };
                            EndInterfaceACIM<PermissionUserResponse>(error, log, error.ReturnCode, error.ReturnMessage);

                            return error;
                        }
                    }
                    else
                    {
                        var error = new PermissionUserResponse
                        {
                            ReturnCode = "40001",
                            ReturnMessage = "Missing Parameter"
                        };

                        EndInterfaceACIM<PermissionUserResponse>(error, log, error.ReturnCode, error.ReturnMessage);

                        return error;
                    }
                }
                else
                {
                    var error = new PermissionUserResponse
                    {
                        ReturnCode = "40002",
                        ReturnMessage = "Invalid Parameter"
                    };
                    EndInterfaceACIM<PermissionUserResponse>(error, log, error.ReturnCode, error.ReturnMessage);

                    return error;
                }


            }
            catch (System.Exception ex)
            {
                var error = new PermissionUserResponse
                {
                    ReturnCode = "50000",
                    ReturnMessage = "System Error"
                };
                EndInterfaceACIM<PermissionUserResponse>(error, log, error.ReturnCode, error.ReturnMessage);
                return error;
            }
        }
        #endregion

        #region R21.8

        //PatchAdressESRI
        public PatchAdressESRIResponse PatchAdressESRI(GetPatchAdressESRIQuery query)
        {
            InterfaceLogCommand log = null;
            PatchAdressESRIResponse result = new PatchAdressESRIResponse();
            PatchAdressESRIModel dataResult = new PatchAdressESRIModel();
            try
            {
                log = StartInterface<GetPatchAdressESRIQuery>(query, "PatchAdressESRI", "", "");

                if (string.IsNullOrEmpty(query.Lang) || string.IsNullOrEmpty(query.SubDistrict) ||
                    string.IsNullOrEmpty(query.District) || string.IsNullOrEmpty(query.Province))
                {
                    string strMESSAGE = ""; string strMESSAGELang = "";

                    if (string.IsNullOrEmpty(query.Lang))
                        strMESSAGELang = "Lang";

                    strMESSAGE += strMESSAGELang == "" ? "" : strMESSAGELang;

                    if (string.IsNullOrEmpty(query.SubDistrict))
                        strMESSAGE += strMESSAGE == "" ? "SubDistrict" : ", SubDistrict";

                    if (string.IsNullOrEmpty(query.District))
                        strMESSAGE += strMESSAGE == "" ? "District" : ", District";

                    if (string.IsNullOrEmpty(query.Province))
                        strMESSAGE += strMESSAGE == "" ? "Province" : ", Province";

                    strMESSAGE += " Value is empty.";

                    result = new PatchAdressESRIResponse
                    {
                        RETURN_CODE = "-1",
                        RETURN_MESSAGE = strMESSAGE,
                        list_address = null
                    };
                    EndInterface<PatchAdressESRIResponse>(result, log, "", result.RETURN_MESSAGE, result.RETURN_MESSAGE);
                    return result;
                }
                else
                {
                    dataResult = _queryProcessor.Execute(query);

                    if (dataResult.RETURN_CODE == "0")
                    {
                        result = new PatchAdressESRIResponse
                        {
                            RETURN_CODE = dataResult.RETURN_CODE,
                            RETURN_MESSAGE = dataResult.RETURN_MESSAGE,
                            list_address = dataResult.list_address
                        };
                        EndInterface<PatchAdressESRIResponse>(result, log, query.Province, "Success", result.RETURN_MESSAGE);
                        return result;

                    }
                    else
                    {
                        result = new PatchAdressESRIResponse
                        {
                            RETURN_CODE = dataResult.RETURN_CODE,
                            RETURN_MESSAGE = dataResult.RETURN_MESSAGE,
                            list_address = dataResult.list_address
                        };
                        EndInterface<PatchAdressESRIResponse>(result, log, query.Province, "Error", result.RETURN_MESSAGE);

                        return result;
                    }
                }
            }
            catch (System.Exception ex)
            {
                result = new PatchAdressESRIResponse
                {
                    RETURN_CODE = "-1",
                    RETURN_MESSAGE = "PatchAdressESRI error",
                    list_address = null
                };
                EndInterface<PatchAdressESRIResponse>(result, log, "", ex.Message, result.RETURN_MESSAGE);
                return result;
            }
        }

        #endregion

        public QueryLOVForWebResponse QueryLOVForWeb(QueryLOVForWebQuery query)
        {
            QueryLOVForWebModel queryResult = _queryProcessor.Execute(query);
            QueryLOVForWebResponse response = new QueryLOVForWebResponse();
            if (queryResult != null)
            {
                response = new QueryLOVForWebResponse()
                {
                    ReturnCode = queryResult.ReturnCode,
                    ReturnMessage = queryResult.ReturnMessage,
                    LIST_LOV_CUR = queryResult.LIST_LOV_CUR
                };
            }
            else
            {
                response = new QueryLOVForWebResponse()
                {
                    ReturnCode = "-1",
                    ReturnMessage = "No Data"
                };
            }

            return response;
        }

        #region Call 3BB Service

        public CheckCoverageSpecialResponse CheckCoverageSpecial(CheckCoverageSpecialRequest request)
        {
            CheckCoverageSpecialResponse response = new CheckCoverageSpecialResponse();
            response.returnCode = "00009";
            response.returnMessage = "Call WBBService Failed.";

            CheckCoverage3bbQuery checkCoverage3bbQuery = new CheckCoverage3bbQuery
            {
                latitude = request.latitude,
                longitude = request.longitude,
                TRANSACTION_ID = request.mobileno
            };

            CheckCoverage3bbQueryModel queryData = _queryProcessor.Execute(checkCoverage3bbQuery);
            if (queryData != null)
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
                        splitterLongitude = t.splitterLongitude.ToSafeString()
                    }).ToList();
                    string splitterJsonData = new JavaScriptSerializer().Serialize(splitterList);
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
            return response;
        }

        #endregion

        #region R22.09

        public CheckCoverageForWorkflowResponse CheckCoverageForWorkflow(CheckCoverageForWorkflowRequest request)
        {

            InterfaceLogCommand log3bb = StartInterface(request, "CheckCoverageForWorkflow", request.transactionId.ToSafeString(), "");
            CheckCoverageForWorkflowResponse result = new CheckCoverageForWorkflowResponse();

            try
            {
                if (request != null && !string.IsNullOrEmpty(request.latitude) && !string.IsNullOrEmpty(request.longitude))
                {

                    //Step 1: Call MapService
                    var queryCheckCoverageMap = new CheckCoverageMapServiceQuery
                    {
                        latitude = request.latitude.ToString(),
                        longitude = request.longitude.ToString(),
                        transactionId = request.transactionId.ToString(),
                        FullUrl = "FBB"
                    };

                    CheckCoverageMapServiceDataModel dataCheckCoverageMap = _queryProcessor.Execute(queryCheckCoverageMap);

                    if (dataCheckCoverageMap != null && dataCheckCoverageMap.returnCode == "0")
                    {
                        if (dataCheckCoverageMap.status == "ON_SERVICE" && dataCheckCoverageMap.splitterList.Count > 0)
                        {
                            List<SPLITTER_INFO> splitterCheckCoverageList = dataCheckCoverageMap.splitterList.ConvertAll(x => new SPLITTER_INFO
                            {
                                Splitter_Name = x.Name.ToSafeString(),
                                Distance = 0,
                                Distance_Type = string.Empty,
                                Resource_Type = string.Empty
                            });

                            //Step 2: Call FBSS ResQuery
                            var queryZTEResQuery = new ZTEResQueryQuery
                            {
                                PRODUCT = "FTTH",
                                LISTOFSPLITTER = splitterCheckCoverageList.ToArray(),
                                TRANSACTION_ID = request.transactionId.ToSafeString(),
                                PHONE_FLAGE = string.Empty,
                                LISTOFDSLAM = null,
                                ADDRESS_ID = string.Empty,
                                FullUrl = "FBB"
                            };

                            var dataZTEResQuery = _queryProcessor.Execute(queryZTEResQuery);

                            if (dataZTEResQuery != null && dataZTEResQuery.RESULT_SPLITTERLIST.Length > 0)
                            {
                                List<ResultSplitList> splitterZTEList = dataZTEResQuery.RESULT_SPLITTERLIST
                                    .Select(item => new ResultSplitList()
                                    {
                                        SPLITTER_NO = item.SPLITTER_NO,
                                        RESULT_CODE = item.RESULT_CODE, //"1",
                                        RESULT_DESCRIPTION = item.RESULT_DESCRIPTION
                                    }).ToList();

                                //Step 3: filter SplitterZTEList RESULT_CODE 1
                                List<CheckCoverageForWorkflowSplitter> filterSplitterZTEList = new List<CheckCoverageForWorkflowSplitter>();
                                foreach (var i in splitterZTEList)
                                {
                                    if (i.RESULT_CODE == "1")
                                    {
                                        var tmpSplitterlist = dataCheckCoverageMap.splitterList.Where(w => w.Name == i.SPLITTER_NO)
                                            .Select(item => new CheckCoverageForWorkflowSplitter()
                                            {
                                                distance = item.Distance.ToSafeString(),
                                                splitterAlias = string.Empty,
                                                splitterCode = item.Name.ToSafeString(),
                                                splitterLatitude = item.Lat.ToSafeString(),
                                                splitterLongitude = item.Lon.ToSafeString(),
                                                splitterPort = string.Empty
                                            }).FirstOrDefault();
                                        filterSplitterZTEList.Add(tmpSplitterlist);
                                    }
                                }

                                result.splitterList = new List<CheckCoverageForWorkflowSplitter>();

                                //Step 4: Call FBSS QueryPort
                                bool chkCallQueryPort = true;
                                foreach (var j in filterSplitterZTEList)
                                {
                                    var queryPort = new QueryPortQuery
                                    {
                                        RESOURCE_NO = j.splitterCode.ToSafeString(),
                                        RESOURCE_TYPE = "SPLITTER",
                                        SERVICE_STATE = string.Empty,
                                        TRANSACTION_ID = request.transactionId.ToSafeString(),
                                        FullUrl = "FBB"
                                    };
                                    var dataqueryPort = _queryProcessor.Execute(queryPort);

                                    if (dataqueryPort.return_code == "0" && dataqueryPort.Data != null)
                                    {
                                        if (dataqueryPort.Data.RESULT_CODE == "0" && dataqueryPort.Data.QueryPortNoList.Count > 0)
                                        {
                                            var tmpSplitterlist2 = dataqueryPort.Data.QueryPortNoList
                                            .Select(item => new CheckCoverageForWorkflowSplitter()
                                            {
                                                distance = j.distance.ToSafeString(),
                                                splitterAlias = dataqueryPort.Data.RESOURCE_ALIAS.ToSafeString(),
                                                splitterCode = j.splitterCode.ToSafeString(),
                                                splitterLatitude = j.splitterLatitude.ToSafeString(), //dataqueryPort.Data.RESOURCE_LATITUDE.ToSafeString(),
                                                splitterLongitude = j.splitterLongitude.ToSafeString(),//dataqueryPort.Data.RESOURCE_LONGITUDE.ToSafeString(),
                                                splitterPort = item.PORT_NO
                                            }).FirstOrDefault();
                                            result.splitterList.Add(tmpSplitterlist2);
                                        }
                                    }
                                    else
                                    {
                                        //case => dataqueryPort.return_code == "-1" ตก catch call QuertPort ไม่ได้
                                        chkCallQueryPort = false;
                                    }
                                }

                                if (result.splitterList.Count > 0)
                                {
                                    //ON_SERVICE=อยู่ในพื้นที่ให้บริการ
                                    result.returnCode = "00000";
                                    result.returnMessage = "Success";
                                    result.inServiceDate = "";
                                    result.coverage = dataCheckCoverageMap.status.ToSafeString();
                                }
                                else if (chkCallQueryPort == false)
                                {
                                    //Can't connect database
                                    result.returnCode = "50301";
                                    result.returnMessage = "Can't connect database";
                                    result.inServiceDate = "";
                                    result.coverage = "";
                                    result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                                }
                                else
                                {
                                    //ไม่มี Splitter Port
                                    result.returnCode = "00000";
                                    result.returnMessage = "Success";
                                    result.inServiceDate = "";
                                    result.coverage = "OUT_OF_SERVICE";
                                    result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                                }
                            }
                            else
                            {
                                //Data not found
                                result.returnCode = "50204";
                                result.returnMessage = "Data not found";
                                result.inServiceDate = "";
                                result.coverage = "";
                                result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                            }
                        }
                        else if (dataCheckCoverageMap.status == "PLAN")
                        {
                            //PLAN=มีแผนจะเปิดให้บริการ
                            result.returnCode = "00000";
                            result.returnMessage = "Success";
                            result.inServiceDate = dataCheckCoverageMap.inserviceDate.ToSafeString();
                            result.coverage = dataCheckCoverageMap.status.ToSafeString();
                            result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                        }
                        else if (dataCheckCoverageMap.status == "OUT_OF_SERVICE"
                            || dataCheckCoverageMap.status == "ON_DEMAND"
                            || dataCheckCoverageMap.status == "RESTRICTION_VILLAGE_PERMISSION"
                            || dataCheckCoverageMap.status == "RESTRICTION_CABLE_UNDERGROUND")
                        {
                            //OUT_OF_SERVICE=ไม่อยู่ในพื้นที่ให้บริการ
                            //ON_DEMAND=พื้นที่เบาบาง แต่ให้บริการได้
                            //RESTRICTION_VILLAGE_PERMISSION=ติด permission ของหมู่บ้าน
                            //RESTRICTION_CABLE_UNDERGROUND=ติด Cable Underground ของหมู่บ้าน

                            result.returnCode = "00000";
                            result.returnMessage = "Success";
                            result.inServiceDate = "";
                            result.coverage = dataCheckCoverageMap.status.ToSafeString();
                            result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                        }
                        else if (dataCheckCoverageMap.status == "ON_SERVICE" && dataCheckCoverageMap.splitterList.Count == 0)
                        {
                            //OUT_OF_SERVICE
                            result.returnCode = "00000";
                            result.returnMessage = "Success";
                            result.inServiceDate = "";
                            result.coverage = "OUT_OF_SERVICE";
                            result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                        }
                        else
                        {
                            //UNDEFINED=กรณี Error
                            result.returnCode = "50403";
                            result.returnMessage = "Response Error";
                            result.inServiceDate = "";
                            result.coverage = "";
                            result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                        }
                    }
                    else if (dataCheckCoverageMap != null && dataCheckCoverageMap.returnCode == "1")
                    {
                        //1 = lat หรือ long เป็นค่าว่าง
                        string strRequireField = "";
                        if (string.IsNullOrEmpty(request.latitude)) { strRequireField += "Require field - ( latitude )"; }
                        else if (string.IsNullOrEmpty(request.longitude)) { strRequireField += "Require field - ( longitude )"; }

                        result.returnCode = "50101";
                        result.returnMessage = strRequireField;
                        result.inServiceDate = "";
                        result.coverage = "";
                        result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                    }
                    else if (dataCheckCoverageMap != null && dataCheckCoverageMap.returnCode == "-1")
                    {
                        //-1 = Query Error
                        result.returnCode = "50301";
                        result.returnMessage = "Can't connect database";
                        result.inServiceDate = "";
                        result.coverage = "";
                        result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                    }
                    else if (dataCheckCoverageMap.returnCode == "-1")
                    {
                        //Data not found
                        result.returnCode = "50204";
                        result.returnMessage = "Data not found";
                        result.inServiceDate = "";
                        result.coverage = "";
                        result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                    }
                    else
                    {
                        //-2 = System Error
                        result.returnCode = "59999";
                        result.returnMessage = "System error";
                        result.inServiceDate = "";
                        result.coverage = dataCheckCoverageMap.status.ToSafeString();
                        result.splitterList = new List<CheckCoverageForWorkflowSplitter>();
                    }

                    EndInterface(result, log3bb, request.transactionId.ToSafeString(), "Success", "");
                    return result;
                }
                else
                {
                    //Format incorreact
                    result.returnCode = "50102";
                    result.returnMessage = "Format incorreact";
                    result.inServiceDate = "";
                    result.coverage = "";
                    result.splitterList = new List<CheckCoverageForWorkflowSplitter>();

                    EndInterface(result, log3bb, request.transactionId.ToSafeString(), "Success", "");
                    return result;
                }

            }
            catch (Exception ex)
            {
                //Authorization failed
                result.returnCode = "401";
                result.returnMessage = ex.Message;
                result.inServiceDate = "";
                result.coverage = "";
                result.splitterList = new List<CheckCoverageForWorkflowSplitter>();

                EndInterface(result, log3bb, request.transactionId.ToSafeString(), "Failed", ex.Message);
                return result;
            }
        }

        #endregion

        #region R23 Call TransferFileToStorage On Prim

        public TransferFileToStorageResponse TransferFileToStorage(TransferFileToStorageRequest request)
        {
            TransferFileToStorageResponse result = new TransferFileToStorageResponse();

            List<FileListData> requestFileListData = request.FileList.Select(d => new FileListData
            {
                OrderNo = d.OrderNo,
                Action = d.Action,
                FileName = d.FileName,
                DataFile = d.DataFile
            }).ToList();

            TransferFileToStorageCommand command = new TransferFileToStorageCommand()
            {
                Option = request.Option,
                OrderNo = request.OrderNo,
                FileList = requestFileListData
            };

            try
            {
                if (request.Option == "2")
                {
                    if (request.FileList.Count > 0 && request.FileList != null)
                    {
                        if (request.FileList.getMoreThanOnceRepeated(z => new { z.OrderNo, z.Action, z.FileName }).ToList().Count > 0)
                        {
                            command.FileList = requestFileListData.getListNonDuplicated(z => new { z.OrderNo, z.Action, z.FileName }).ToList();
                        }

                        _transferFileToStorageCommand.Handle(command);

                        result.ResultCode = command.Return_code;
                        result.ResultDesc = command.Return_message;
                        result.TRANSACTION_ID = command.Transaction_id;
                        result.OrderNo = null;
                        result.FileName = null;
                        result.DataFile = null;
                        result.FileList = command.FileList;
                    }
                }
                else if (request.Option == "12")
                {
                    if (request.OrderNo != "" || request.OrderNo != null)
                    {
                        _transferFileToStorageCommand.Handle(command);

                        result.ResultCode = command.Return_code;
                        result.ResultDesc = command.Return_message;
                        result.TRANSACTION_ID = command.Transaction_id;
                        result.OrderNo = null;
                        result.FileName = null;
                        result.DataFile = null;
                        result.FileList = command.FileList;
                    }
                }
                else
                {
                    result.ResultCode = "40002";
                    result.ResultDesc = "Event not found";
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = "50001";
                result.ResultDesc = ex.Message;
            }

            return result;
        }

        #endregion

        #region private zone forever alone
        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = query.GetType().Name,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "CPGWPlus",
                CREATED_BY = "FBBMOB",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private InterfaceLogCommand StartInterfaceACIM<T>(T query, string methodName, string transactionId, string idCardNo)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = "PKG_FBBOR045",
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "miniEAI",
                CREATED_BY = "WBB_APP",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd, string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogCommand.Handle(dbIntfCmd);
        }

        private void EndInterfaceACIM<T>(T output, InterfaceLogCommand dbIntfCmd, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "20000") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = reason;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            _intfLogCommand.Handle(dbIntfCmd);
        }
        private QuickWinPanelModel QuickWinPanelBuilder(GetRegResultCoreQuery query, PackageModel mainPackBill, bool isLocalLanguage, string docType)
        {
            var lovConfig = _queryProcessor.Execute(new GetLovQuery
            {
                LovType = "SCREEN",
            });

            var mainPackBillDesc = (isLocalLanguage ?
                                    mainPackBill.SFF_PROMOTION_BILL_THA :
                                    mainPackBill.SFF_PROMOTION_BILL_ENG).ToSafeString();

            var ontopPackBill = (from t in query.SelectPackage
                                 where t.PACKAGE_TYPE.IsOnTopPack()
                                 select t)
                                 .FirstOrDefault();

            var summaryPackModel = new SummaryPanelModel
            {
                PDFPackageModel = new PDFPackageModel
                {
                    PDF_L_MAIN_PACKAGE = mainPackBillDesc,
                    PDF_L_VALUE_LABEL_PAKAGENAME = mainPackBill.PACKAGE_NAME,

                },
            };

            if (ontopPackBill != null)
            {
                var pdfOntopText = PdfOntopString(lovConfig, isLocalLanguage, ontopPackBill.TECHNOLOGY);

                summaryPackModel.PDFPackageModel.PDF_L_ONTOPLIST = new List<string>
                        {
                            pdfOntopText,
                        };

                var preInitCharge = ontopPackBill.PRE_INITIATION_CHARGE.GetValueOrDefault();
                var initCharge = ontopPackBill.INITIATION_CHARGE.GetValueOrDefault();

                summaryPackModel.PDFPackageModel.PDF_L_VALUE_LABEL_TIDTUN = pdfOntopText;

                summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_TIDTUN = preInitCharge.ToSafeString();

                summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNT = preInitCharge.ToSafeString();

                if (ontopPackBill.PRODUCT_SUBTYPE.IsSWiFi())
                    summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNT =
                        (preInitCharge - initCharge).ToSafeString();

                summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM1 =
                    (preInitCharge - summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNT.ToSafeDecimal())
                    .ToSafeString();
            }

            summaryPackModel.PDFPackageModel.PDF_L_VALUE_LABEL_PAKAGENAME = mainPackBill.PACKAGE_NAME;

            summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_PAKAGE = mainPackBill.RECURRING_CHARGE.ToSafeString();

            summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM2 = mainPackBill.RECURRING_CHARGE.ToSafeString();

            summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_4DAY =
                decimal.Round((mainPackBill.RECURRING_CHARGE /
                        DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) * 4).GetValueOrDefault(), 2, MidpointRounding.AwayFromZero).ToSafeString();

            summaryPackModel.PDFPackageModel.PDF_L_LABEL_DETAIL_ALLFIRSTBILL =
                (summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM1.ToSafeDecimal() +
                summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM2.ToSafeDecimal() +
                summaryPackModel.PDFPackageModel.PDF_L_VALUE_DETAIL_4DAY.ToSafeDecimal()).ToSafeString();

            var model = new QuickWinPanelModel
            {
                CustomerRegisterPanelModel = new CustomerRegisterPanelModel
                {

                    L_TITLE = isLocalLanguage ? "คุณ" : "",
                    L_TITLE_CODE = isLocalLanguage ? "127" : "",

                    L_FIRST_NAME = query.CustFirstName,
                    L_LAST_NAME = query.CustLastName,
                    L_CARD_TYPE = isLocalLanguage ? "บัตรประชาชน" : "ID_CARD",
                    L_CARD_NO = query.IDCardNo,

                    L_BIRTHDAY = query.CustBirthDate,
                    L_HOME_PHONE = query.ContactHomeNo,
                    L_MOBILE = query.ContactMobileNo,
                    L_SPECIFIC_TIME = query.PreferInstallTime,
                    L_EMAIL = query.ContactEmail,
                    L_INSTALL_DATE = query.PreferInstallDate,
                    CateType = "R",
                    SubCateType = "T",
                    DocType = docType,

                    L_LOC_CODE = query.LocationCode,
                    L_ASC_CODE = query.ASCCode,
                    L_STAFF_ID = query.StaffID,
                    L_SALE_REP = query.SaleRep,

                    AddressPanelModelSendDoc = new AddressPanelModel
                    {
                        L_PROVINCE = query.BillProvince,
                        L_TUMBOL = query.BillSubDistrict,
                        L_AMPHUR = query.BillDistrict,
                        L_MOOBAN = query.BillVillage,
                        L_MOO = query.BillMoo,
                        L_SOI = query.BillSoi,
                        L_ROAD = query.BillRoad,
                        L_ZIPCODE = query.BillZipCode,
                        L_HOME_NUMBER_2 = query.BillHouseNo,
                        L_FLOOR = query.BillFloor,
                        L_BUILD_NAME = query.BillBuilding + query.BillTower,
                    },

                    AddressPanelModelSetup = new AddressPanelModel
                    {
                        L_PROVINCE = query.InstallProvince,
                        L_TUMBOL = query.InstallSubDistrict,
                        L_AMPHUR = query.InstallDistrict,
                        L_MOOBAN = query.InstallVillage,
                        L_MOO = query.InstallMoo,
                        L_SOI = query.InstallSoi,
                        L_ROAD = query.InstallRoad,
                        L_ZIPCODE = query.InstallZipCode,
                        L_HOME_NUMBER_2 = query.InstallHouseNo,
                        L_FLOOR = query.InstallFloor,
                        L_BUILD_NAME = query.InstallBuilding + query.InstallTower,
                    },
                },

                CoveragePanelModel = new CoveragePanelModel
                {
                    PRODUCT_SUBTYPE = mainPackBill.PRODUCT_SUBTYPE,
                    Address = new AddressPanelModel
                    {
                        AddressId = query.AddressId.ToSafeString(),
                    },
                    AccessMode = mainPackBill.ACCESS_MODE,
                    ServiceCode = mainPackBill.SERVICE_CODE,
                },

                SummaryPanelModel = summaryPackModel,
            };
            return model;
        }

        private string PdfOntopString(IEnumerable<LovValueModel> lovConfig, bool isLocalLang, string onIntTech)
        {
            var detail6 = lovConfig
                            .Where(t => t.Name == "L_SUMM_DETAIL6")
                            .Select(t => isLocalLang ? t.LovValue1 : t.LovValue2).FirstOrDefault();
            var router = lovConfig
                            .Where(t => t.Name == "L_ROUTER")
                            .Select(t => isLocalLang ? t.LovValue1 : t.LovValue2).FirstOrDefault();

            var pdfOntopListStringFormat = "{0} {1} {2}";

            if (onIntTech.IsFTTx())
            {
                router = lovConfig
                            .Where(t => t.Name == "L_ROUTER_ONU")
                            .Select(t => isLocalLang ? t.LovValue1 : t.LovValue2).FirstOrDefault();
            }

            return string.Format(pdfOntopListStringFormat, detail6, onIntTech, router);
        }

        private struct SffBuilding
        {
            public string BuildingName { get; set; }
            public string TowerName { get; set; }
        }

        private SffBuilding ExtractSffBuilding(string sffBuilding)
        {
            var extracted = new SffBuilding();

            var arkarnWords = new List<string>
            {
                "ตึก", "อาคาร","building","Building"
            };

            if (string.IsNullOrEmpty(sffBuilding))
                return extracted;

            var astrn = arkarnWords
                .Select(t => sffBuilding.Contains(t) ? sffBuilding.Substring(0, sffBuilding.IndexOf(t))
                    + "|" + sffBuilding.Substring(sffBuilding.IndexOf(t) + t.Length) : "")
                    .Where(t => !string.IsNullOrEmpty(t)).FirstOrDefault();


            if (!string.IsNullOrEmpty(astrn))
            {
                var strns = astrn.Split('|');
                extracted.BuildingName = strns[0];
                extracted.TowerName = strns[1];
            }
            else
            {
                var strns = sffBuilding.Split(' ');

                var potTower = strns.Skip(Math.Max(0, strns.Count() - 1)).Take(strns.Count()).FirstOrDefault();

                if (potTower.Count() <= 3)
                {
                    extracted.BuildingName = string.Join(" ", strns.Take(strns.Count() - 1));
                    extracted.TowerName = potTower;
                }
                else
                {
                    extracted.BuildingName = sffBuilding;
                }
            }

            return extracted;
        }

        private PremiumAreaModel GetPremiumArea(string SubDistrict, string District, string Province, string PostalCode, string Language)
        {
            // get config 
            var getLovConfig = Get_FBB_CFG_LOV("FBB_CONSTANT", "HVR_USE_FLAG").ToList();
            // get flag
            var flagHVR = getLovConfig.Where(p => p.LovValue1 == "Y").Select(o => o.LovValue1).FirstOrDefault() ?? "N";

            PremiumAreaModel premiumAreaModel = new PremiumAreaModel();

            if (flagHVR == "Y")
            {
                GetPremiumAreaHVRQuery query = new GetPremiumAreaHVRQuery()
                {
                    SubDistrict = SubDistrict.ToSafeString(),
                    District = District.ToSafeString(),
                    Province = Province.ToSafeString(),
                    PostalCode = PostalCode.ToSafeString(),
                    Language = Language.ToSafeString()
                };

                premiumAreaModel = _queryProcessor.Execute(query);
            }
            else
            {
                GetPremiumAreaQuery query = new GetPremiumAreaQuery()
                {
                    SubDistrict = SubDistrict.ToSafeString(),
                    District = District.ToSafeString(),
                    Province = Province.ToSafeString(),
                    PostalCode = PostalCode.ToSafeString(),
                    Language = Language.ToSafeString()
                };

                premiumAreaModel = _queryProcessor.Execute(query);
            }
            
            return premiumAreaModel;
        }

        private CheckPremiumFlagModel CheckPremiumFlagData(string RecurringCharge, string LocationCode, string AccessMode, string PartnerSubtype, string MemoFlag, string TransactionID)
        {
            CheckPremiumFlagDataQuery query = new CheckPremiumFlagDataQuery()
            {
                RecurringCharge = RecurringCharge.ToSafeString(),
                LocationCode = LocationCode.ToSafeString(),
                AccessMode = AccessMode.ToSafeString(),
                PartnerSubtype = PartnerSubtype.ToSafeString(),
                MemoFlag = MemoFlag.ToSafeString(),
                TransactionID = TransactionID.ToSafeString()
            };
            CheckPremiumFlagModel checkPremiumFlagData = _queryProcessor.Execute(query);
            return checkPremiumFlagData;
        }

        private CheckTimeslotBySubtypeModel CheckTimeslotBySubtypeData(string partnersubtype, string accessmode, string transactionID)
        {
            CheckTimeslotBySubtypeDataQuery query = new CheckTimeslotBySubtypeDataQuery()
            {
                partnersubtype = partnersubtype.ToSafeString(),
                accessmode = accessmode.ToSafeString(),
                TransactionID = transactionID.ToSafeString()
            };
            CheckTimeslotBySubtypeModel checkTimeslotBySubtypeData = _queryProcessor.Execute(query);
            return checkTimeslotBySubtypeData;
        }

        private CheckFMCPackageModel CheckFMCPackageData(string p_mobile_price_excl_vat, string p_project_name, string p_sff_promotion_code)
        {
            CheckFMCPackageDataQuery query = new CheckFMCPackageDataQuery()
            {
                p_mobile_price_excl_vat = p_mobile_price_excl_vat.ToSafeString(),
                p_project_name = p_project_name.ToSafeString(),
                p_sff_promotion_code = p_sff_promotion_code.ToSafeString()
            };
            CheckFMCPackageModel checkFMCPackageData = _queryProcessor.Execute(query);
            return checkFMCPackageData;
        }

        private static string Encrypt(string textToEncrypt, string key = "")
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.ECB;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 0x80;
                rijndaelCipher.BlockSize = 0x80;

                String pass = null;
                pass = padString(key);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(pass);

                rijndaelCipher.Key = pwdBytes;
                rijndaelCipher.IV = pwdBytes;

                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
                byte[] encrypt = transform.TransformFinalBlock(plainText, 0, plainText.Length);
                return HexEncodingToString(encrypt);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static string Decrypt(string textToDecrypt, string key = "")
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.ECB;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 0x80;
                rijndaelCipher.BlockSize = 0x80;

                byte[] encryptedData = HexToBytes(textToDecrypt);
                //byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
                String pass = null;
                pass = padString(key);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(pass);


                rijndaelCipher.Key = pwdBytes;
                rijndaelCipher.IV = pwdBytes;


                byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return Encoding.UTF8.GetString(plainText);

            }
            catch (Exception ex)
            {
                return "";
                throw new Exception(ex.Message);
            }
        }

        private static String padString(String str)
        {
            int slen = (str.Length % 16);
            int i = (16 - slen);
            if ((i > 0) && (i < 16))
            {
                StringBuilder buf = new StringBuilder(str.Length + i);
                buf.Insert(0, str);
                for (i = (16 - slen); i > 0; i--)
                {
                    buf.Append(" ");
                }
                return buf.ToString();
            }
            else
            {
                return str;
            }
        }

        private static string HexEncodingToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }

        private static byte[] HexToBytes(string str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }

        #endregion
    }
}