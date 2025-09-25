using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using WBBBusinessLayer.SBNV2WebService;
using WBBContract;
using WBBContract.Commands;
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
    public class GetSaveOrderRespQueryHandler : IQueryHandler<GetSaveOrderRespQuery, SaveOrderResp>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private static ILogger _loggers;

        public GetSaveOrderRespQueryHandler(ILogger logger,
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
            _loggers = logger;
        }

        public SaveOrderResp Handle(GetSaveOrderRespQuery query)
        {
            InterfaceLogCommand log = null;
            var response = new SaveOrderResp();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.QuickWinPanelModel.CoveragePanelModel.P_MOBILE + query.QuickWinPanelModel.ClientIP, "saveOrderNew", "GetSaveOrderRespQuery", query.QuickWinPanelModel.CustomerRegisterPanelModel.L_CARD_NO, "FBB|" + query.FullUrl, "WEB");

                #region make parameter
                var quickWinModel = query.QuickWinPanelModel;

                // check null.
                if (null == quickWinModel)
                    throw new Exception("Model is null.");
                if (null == quickWinModel.CoveragePanelModel)
                    throw new Exception("CoveragePanelModel is null.");
                if (null == quickWinModel.DisplayPackagePanelModel)
                    throw new Exception("DisplayPackagePanelModel is null.");
                if (null == quickWinModel.CustomerRegisterPanelModel)
                    throw new Exception("CustomerRegisterPanelModel is null.");
                if (null == quickWinModel.SummaryPanelModel)
                    throw new Exception("SummaryPanelModel is null.");

                var cpm = quickWinModel.CoveragePanelModel;
                var dpm = quickWinModel.DisplayPackagePanelModel;
                var crpm = quickWinModel.CustomerRegisterPanelModel;
                var spm = quickWinModel.SummaryPanelModel;
                var langFlag = (query.CurrentCulture.IsThaiCulture() ? "THA" : "ENG");
                var oim = quickWinModel.OfficerInfoPanelModel;

                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1") { crpm.L_TITLE_CODE = "127"; }

                if (crpm.L_TITLE_CODE.ToSafeString() == "")
                {
                    if (crpm.CateType.ToSafeString().Equals("R"))
                        crpm.L_TITLE_CODE = "188";
                    if (crpm.CateType.ToSafeString().Equals("G"))
                        crpm.L_TITLE_CODE = "949";
                    if (crpm.CateType.ToSafeString().Equals("B"))
                        crpm.L_TITLE_CODE = "948";
                }

                if ((crpm.CateType.ToSafeString().Equals("G") || crpm.CateType.ToSafeString().Equals("B")) && crpm.L_TITLE_CODE == "127")
                {
                    if (crpm.CateType.ToSafeString().Equals("G"))
                        crpm.L_TITLE_CODE = "949";
                    if (crpm.CateType.ToSafeString().Equals("B"))
                        crpm.L_TITLE_CODE = "948";
                }

                var title = crpm.L_TITLE_CODE.ToSafeString();

                var firstName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_FIRST_NAME : crpm.L_GOVERNMENT_NAME);
                var lastName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_LAST_NAME : "");

                var cTitle = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_TITLE_CODE.ToSafeString() : "");

                var tempcontact = crpm.L_CONTACT_PERSON.ToSafeString().Split();
                var ctemtfirst = "";
                var ctemtlast = "";
                if (tempcontact.Count() > 1)
                {
                    ctemtfirst = tempcontact[0];
                    for (var i = 1; i < tempcontact.Count(); i++)
                    {
                        ctemtlast = ctemtlast + tempcontact[i] + " ";
                    }
                }
                else
                {
                    ctemtfirst = tempcontact[0];
                    ctemtlast = ".";
                }

                var cFirstName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_FIRST_NAME : ctemtfirst);
                var cLastName = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_LAST_NAME : ctemtlast.Trim());

                var phoneNo = crpm.L_MOBILE.ToSafeString();
                var cPhoneNo = crpm.L_MOBILE.ToSafeString();

                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                {

                    if (crpm.L_HOME_PHONE == null)
                        crpm.L_HOME_PHONE = "";
                    if (crpm.L_OR == null)
                        crpm.L_OR = "";
                }


                if (query.QuickWinPanelModel.ForCoverageResult == true)
                {
                    title = cpm.L_FIRST_LAST.ToSafeString();
                    firstName = cpm.L_FIRST_NAME.ToSafeString();
                    lastName = cpm.L_LAST_NAME.ToSafeString();

                    cTitle = title;
                    cFirstName = firstName;
                    cLastName = lastName;

                    phoneNo = cpm.L_CONTACT_PHONE.ToSafeString();
                    cPhoneNo = phoneNo;
                }

                var cardNo = "";
                var taxId = "";
                if (crpm.CateType.ToSafeString().Equals("R"))
                {
                    cardNo = crpm.L_CARD_NO.ToSafeString();
                }
                else
                {
                    taxId = crpm.L_CARD_NO.ToSafeString();
                }

                var cusNat = (crpm.CateType.ToSafeString().Equals("R") ? crpm.L_NATIONALITY : "");
                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1") { cusNat = "THAI"; }
                var contactName = (crpm.CateType.ToSafeString().Equals("R") ? "" : crpm.L_CONTACT_PERSON);

                var addressTypeWire = "";
                if (spm.VAS_FLAG.ToSafeString() != "2" && spm.TOPUP.ToSafeString() == "")
                {
                    if (!string.IsNullOrEmpty(cpm.BuildingType) && cpm.BuildingType.Equals("V"))
                    {
                        addressTypeWire = "HOUSE";
                        if (!string.IsNullOrEmpty(crpm.L_TYPE_ADDR))
                        {
                            FBB_CFG_LOV lovValue = null;
                            if (query.CurrentCulture.IsThaiCulture())
                                addressTypeWire = crpm.L_TYPE_ADDR;
                            else
                                lovValue = _lovService.Get(l => l.LOV_VAL2 == crpm.L_TYPE_ADDR).FirstOrDefault();

                            if (null != lovValue)
                                addressTypeWire = lovValue.LOV_VAL1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(cpm.BuildingType) && cpm.BuildingType.Equals("O"))
                    {
                        addressTypeWire = "OTHER_SPECIFIC";
                    }
                    else if (!string.IsNullOrEmpty(cpm.BuildingType) && cpm.BuildingType.Equals("B"))
                    {
                        addressTypeWire = "CONDO";
                    }
                    else
                    {
                        addressTypeWire = "HOUSE";
                        if (!string.IsNullOrEmpty(crpm.L_TYPE_ADDR))
                        {
                            FBB_CFG_LOV lovValue = null;
                            if (query.CurrentCulture.IsThaiCulture())
                                addressTypeWire = crpm.L_TYPE_ADDR;
                            else
                                lovValue = _lovService.Get(l => l.LOV_VAL2 == crpm.L_TYPE_ADDR).FirstOrDefault();

                            if (null != lovValue)
                                addressTypeWire = lovValue.LOV_VAL1;
                        }
                    }
                }

                var packageCode = "";
                if (spm.VAS_FLAG.ToSafeString() != "2" && spm.TOPUP.ToSafeString() == "")
                {
                    if (spm.PackageModel.PACKAGE_CODE.ToSafeString().Equals("Default"))
                    {
                        var lovValue = _lovService.Get(l => l.LOV_TYPE.Equals("DEFAULT_PACKAGE")).FirstOrDefault();
                        if (null != lovValue)
                        {

                            packageCode = lovValue.LOV_VAL1;
                        }
                    }
                    else
                    {
                        packageCode = spm.PackageModel.PACKAGE_CODE.ToSafeString();
                    }
                }

                #region condo
                var CONDO_ROOF_TOP = "";
                var CONDO_BALCONY = "";
                var BALCONY_NORTH = "";
                var BALCONY_SOUTH = "";
                var BALCONY_EAST = "";
                var BALCONY_WAST = "";
                var HIGH_BUILDING = "";
                var HIGH_TREE = "";
                var BILLBOARD = "";
                var EXPRESSWAY = "";
                if (spm.VAS_FLAG.ToSafeString() != "2")
                {
                    // condo
                    CONDO_ROOF_TOP = (string.IsNullOrEmpty(crpm.L_TOP_TERRACE) ? "N" : "Y");
                    CONDO_BALCONY = (string.IsNullOrEmpty(crpm.L_TERRACE) ? "N" : "Y");
                    BALCONY_NORTH = (string.IsNullOrEmpty(crpm.L_NORTH) ? "N" : "Y");
                    BALCONY_SOUTH = (string.IsNullOrEmpty(crpm.L_SOUTH) ? "N" : "Y");
                    BALCONY_EAST = (string.IsNullOrEmpty(crpm.L_EAST) ? "N" : "Y");
                    BALCONY_WAST = (string.IsNullOrEmpty(crpm.L_WEST) ? "N" : "Y");
                    HIGH_BUILDING = (string.IsNullOrEmpty(crpm.L_BUILDING) ? "N" : "Y");
                    HIGH_TREE = (string.IsNullOrEmpty(crpm.L_TREE) ? "N" : "Y");

                    // house
                    BILLBOARD = (string.IsNullOrEmpty(crpm.L_BILLBOARD) ? "N" : "Y");
                    EXPRESSWAY = (string.IsNullOrEmpty(crpm.L_EXPRESSWAY) ? "N" : "Y");

                    try
                    {
                        //if (!string.IsNullOrEmpty(cpm.Address.L_MOOBAN))
                        if (!string.IsNullOrEmpty(cpm.BuildingType) && cpm.BuildingType.Equals("V"))
                        {
                            var homeArea = crpm.L_HOUSE_AREA.ToSafeString();

                            var homeAreaSplitValues = homeArea.Split('|');
                            if (homeAreaSplitValues.Length > 3)
                            {
                                HIGH_BUILDING = homeAreaSplitValues[0].HaveValue().ToYesNoFlgString();
                                HIGH_TREE = homeAreaSplitValues[1].HaveValue().ToYesNoFlgString();
                                BILLBOARD = homeAreaSplitValues[2].HaveValue().ToYesNoFlgString();
                                EXPRESSWAY = homeAreaSplitValues[3].HaveValue().ToYesNoFlgString();
                            }
                        }
                        else if (!string.IsNullOrEmpty(cpm.BuildingType) && cpm.BuildingType.Equals("B"))
                        {
                            var condoType = crpm.L_BUILD_CONDO.ToSafeString();
                            var condoDirection = crpm.L_TERRACE_DIRECTION.ToSafeString();
                            var condoLimit = crpm.L_NUM_OF_FLOOR.ToSafeString();
                            var condoArea = crpm.L_CONDO_AREA.ToSafeString();

                            var condoTypeSplitValues = condoType.Split('|');
                            if (condoTypeSplitValues.Length > 1)
                            {
                                CONDO_ROOF_TOP = condoTypeSplitValues[0].HaveValue().ToYesNoFlgString();
                                CONDO_BALCONY = condoTypeSplitValues[1].HaveValue().ToYesNoFlgString();
                            }

                            var condoDirectionSplitValues = condoDirection.Split('|');
                            if (condoDirectionSplitValues.Length > 3)
                            {
                                BALCONY_NORTH = condoDirectionSplitValues[0].HaveValue().ToYesNoFlgString();
                                BALCONY_SOUTH = condoDirectionSplitValues[1].HaveValue().ToYesNoFlgString();
                                BALCONY_EAST = condoDirectionSplitValues[2].HaveValue().ToYesNoFlgString();
                                BALCONY_WAST = condoDirectionSplitValues[3].HaveValue().ToYesNoFlgString();
                            }

                            var condoAreaSplitValues = condoArea.Split('|');
                            if (condoAreaSplitValues.Length > 1)
                            {
                                HIGH_BUILDING = condoAreaSplitValues[0].HaveValue().ToYesNoFlgString();
                                HIGH_TREE = condoAreaSplitValues[1].HaveValue().ToYesNoFlgString();
                            }
                        }
                        else
                        {
                            var homeArea = crpm.L_HOUSE_AREA.ToSafeString();

                            var homeAreaSplitValues = homeArea.Split('|');
                            if (homeAreaSplitValues.Length > 3)
                            {
                                HIGH_BUILDING = homeAreaSplitValues[0].HaveValue().ToYesNoFlgString();
                                HIGH_TREE = homeAreaSplitValues[1].HaveValue().ToYesNoFlgString();
                                BILLBOARD = homeAreaSplitValues[2].HaveValue().ToYesNoFlgString();
                                EXPRESSWAY = homeAreaSplitValues[3].HaveValue().ToYesNoFlgString();
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        _logger.Info("Index out rage was occured.");
                    }
                }
                #endregion

                var floorNo = (spm.VAS_FLAG.ToSafeString().Equals("2") ? "" : cpm.Address.L_FLOOR);
                var condoFloor = crpm.L_NUM_OF_FLOOR.ToSafeString();

                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                    condoFloor = crpm.AddressPanelModelVat.L_FLOOR;

                var birthDateString = string.Empty;
                var birthDate = new DateTime();

                if (string.IsNullOrEmpty(crpm.L_BIRTHDAY)) crpm.L_BIRTHDAY = "0/0/";

                var date = DateTime.TryParseExact(crpm.L_BIRTHDAY.ToSafeString(), "dd/MM/yyyy",
                                                    CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);


                if (query.CurrentCulture.IsThaiCulture())
                {
                    if (spm.VAS_FLAG.ToSafeString() != "2" && spm.TOPUP.ToSafeString() == "")
                    {
                        if (birthDate > DateTime.MinValue.AddYears(543))
                            birthDateString = birthDate.AddYears(-543).ToDateDisplayText();
                    }
                    else
                    {
                        if (birthDate > DateTime.MinValue)
                            birthDateString = birthDate.ToDateDisplayText();
                    }
                }
                else
                {
                    if (birthDate > DateTime.MinValue)
                        birthDateString = birthDate.ToDateDisplayText();
                }
                // Case Leap Year 29/02
                var split_BD = crpm.L_BIRTHDAY.ToSafeString().Split('/');
                if (!date && split_BD.Length > 2)
                {
                    if (split_BD.Any())
                    {
                        if (query.CurrentCulture.IsThaiCulture())
                        {
                            if (split_BD[2] != "")
                            {
                                if (DateTime.IsLeapYear((split_BD[2].ToSafeInteger() - 543)))
                                {
                                    if (split_BD[1] == "02")
                                        if (split_BD[0] == "29")
                                        {
                                            int tmpYear = split_BD[2].ToSafeInteger();
                                            if (tmpYear > 2450)
                                            {
                                                tmpYear = tmpYear - 543;
                                            }

                                            birthDate = new DateTime(tmpYear, 2, 29);
                                            birthDateString = birthDate.ToString("dd/MM/yyyy");
                                            crpm.L_BIRTHDAY = birthDateString;

                                        }
                                }
                            }
                            else
                            {
                                birthDateString = "";
                                crpm.L_BIRTHDAY = birthDateString;
                            }
                        }
                        else
                        {
                            if (split_BD[2] != "")
                            {
                                if (DateTime.IsLeapYear(split_BD[2].ToSafeInteger()))
                                {
                                    if (split_BD[1] == "02")
                                        if (split_BD[0] == "29")
                                        {
                                            birthDate = new DateTime(split_BD[2].ToSafeInteger(), 2, 29);
                                            birthDateString = birthDate.ToString("dd/MM/yyyy");
                                            crpm.L_BIRTHDAY = birthDateString;
                                        }
                                }
                            }
                            else
                            {
                                birthDateString = "";
                                crpm.L_BIRTHDAY = birthDateString;
                            }
                        }
                    }
                    else
                    {
                        birthDateString = "";
                        crpm.L_BIRTHDAY = birthDateString;
                    }

                }


                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                {
                    if (crpm.SubCateType != "")
                    {
                        crpm.SubCateType = crpm.SubCateType.Substring(0, 1);
                    }
                    crpm.AddressPanelModelSetup.ZIPCODE_ID = crpm.AddressPanelModelSetup.L_ZIPCODE.ToSafeString();
                    crpm.AddressPanelModelSendDoc.ZIPCODE_ID = crpm.AddressPanelModelSendDoc.L_ZIPCODE.ToSafeString();
                    crpm.AddressPanelModelVat.ZIPCODE_ID = crpm.AddressPanelModelVat.L_ZIPCODE.ToSafeString();

                    dpm.WIFIAccessPoint = "N";
                }

                if (crpm.CateType.ToSafeString().Equals("R"))
                {
                    crpm.AddressPanelModelVat.L_HOME_NUMBER_2 = "";
                    crpm.AddressPanelModelVat.L_SOI = "";
                    crpm.AddressPanelModelVat.L_MOO = "";
                    crpm.AddressPanelModelVat.L_MOOBAN = "";
                    crpm.AddressPanelModelVat.L_BUILD_NAME = "";
                    crpm.AddressPanelModelVat.L_FLOOR = "";
                    crpm.AddressPanelModelVat.L_ROOM = "";
                    crpm.AddressPanelModelVat.L_ROAD = "";
                    crpm.AddressPanelModelVat.ZIPCODE_ID = "";
                }

                if (cpm.SffServiceYear.ToSafeString() == "" || cpm.SffServiceYear.ToSafeString() == "null")
                {
                    cpm.SffServiceYear = "0";
                }

                if (cpm.CVR_TOWER.ToSafeString() == "-")
                {
                    cpm.CVR_TOWER = "";
                }

                //do R17.6 Multiple Playbox
                var packageModelList = new List<PackageModel>();
                if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                {
                    if ((spm.PackageModel.SelectVas_Flag == "1" && spm.VOIP_FLAG == "1")
                        || (spm.PackageModel.SelectPlayBox_Flag == "0" && spm.PackageModel.SelectVas_Flag == "1"))
                    {
                        //register VOIP Only
                        packageModelList = spm.PackageModelList.Where(item => item.PRODUCT_SUBTYPE == "VOIP").ToList();
                    }
                    else
                    {
                        //register Playbox Only

                        // remove package VOIP from list package
                        spm.PackageModelList.RemoveAll(item => item.PRODUCT_SUBTYPE == "VOIP");

                        if (!string.IsNullOrEmpty(query.QuickWinPanelModel.RegisterPlayboxNumber) &&
                            Convert.ToInt16(query.QuickWinPanelModel.RegisterPlayboxNumber) > 0)
                        {
                            // register Playbox 2,3,....
                            for (var i = 0; i < Convert.ToInt16(query.QuickWinPanelModel.RegisterPlayboxNumber); i++)
                            {
                                var iExt = i.ToSafeString();
                                var playboxItem =
                                    query.QuickWinPanelModel.MulitPlaybox.SingleOrDefault(
                                        item => item.RowNumber == iExt) ?? new MulitPlayboxModel();
                                var ext =
                                    playboxItem.InstallProductSubType.Substring(
                                        playboxItem.InstallProductSubType.Length - 4);
                                foreach (
                                    var packageModel in
                                        spm.PackageModelList.Where(item => item.SERVICE_CODE == playboxItem.ServiceCode)
                                    )
                                {
                                    packageModel.PLAYBOX_EXT = ext;
                                    packageModelList.Add(packageModel);
                                }
                            }
                        }
                        else
                        {
                            //register Playbox Main

                            //remove package Playbox 2,3,....  from list package
                            spm.PackageModelList.RemoveAll(
                                item =>
                                    !string.IsNullOrEmpty(item.MAPPING_PRODUCT) &&
                                    item.MAPPING_PRODUCT.Substring(0, 1) == "E");

                            packageModelList = spm.PackageModelList;
                        }

                    }
                }
                else
                {
                    packageModelList = spm.PackageModelList;
                    packageModelList = packageModelList.OrderBy(p => p.PACKAGE_SERVICE_CODE).ThenBy(p => p.PACKAGE_TYPE).ToList();
                }

                var temp = packageModelList;

                //R18.1 FTTB Sell Router
                string[] routerPackageCode = null;
                if (!string.IsNullOrEmpty(crpm.RouterFlag) && crpm.RouterFlag == "S")
                {
                    routerPackageCode = (from z in _lov.Get()
                                         where z.LOV_TYPE == "ROUTER_FIBRE" && z.DISPLAY_VAL == "SELECT_GROUP" && z.ACTIVEFLAG == "Y"
                                         select z.LOV_VAL1).ToArray();
                }

                var airregists = temp.Select(o => new airRegistPackageRecord()
                {
                    faxFlag = o.FAX_FLAG.ToSafeString(),
                    tempIa = o.PACKAGE_SERVICE_CODE.ToSafeString(),
                    homeIp = o.VOIP_IP.ToSafeString(),
                    homePort = "",
                    iddFlag = o.IDD_FLAG.ToSafeString(),
                    packageCode = o.SFF_PROMOTION_CODE.ToSafeString(),
                    packageType = o.PACKAGE_TYPE.ToSafeString(),
                    productSubtype = o.PRODUCT_SUBTYPE.ToSafeString(),
                    mobileForward = o.MOBILE_FORWARD.ToSafeString(),
                    pboxExt = o.PLAYBOX_EXT.ToSafeString(),
                    //R18.1 FTTB Sell Router
                    packagePrice = (routerPackageCode != null)
                        ? (routerPackageCode.Contains(o.SFF_PROMOTION_CODE.ToSafeString())
                            ? o.PRICE_CHARGE.GetValueOrDefault()
                            : 0)
                        : 0,
                    packagePriceSpecified = true,
                    packageCount = o.PACKAGE_COUNT
                }).ToArray();

                List<airRegistCpeSerialRecord> airCPEtmp;
                if (cpm.WTTX_COVERAGE_RESULT == "YES")
                {
                    if (crpm.WTTx_Info != null)
                    {
                        airCPEtmp = crpm.WTTx_Info.ConvertAll(l => new airRegistCpeSerialRecord()
                        {
                            serialNo = l.SN.ToSafeString(),
                            cpeType = l.cpe_type.ToSafeString(),
                            macAddress = l.CPE_MAC_ADDR.ToSafeString(),

                            //20.4
                            statusDesc = l.STATUS_DESC.ToSafeString(),
                            modelName = l.CPE_MODEL_NAME.ToSafeString(),
                            companyCode = l.CPE_COMPANY_CODE.ToSafeString(),
                            cpePlant = l.CPE_PLANT.ToSafeString(),
                            storageLocation = l.CPE_STORAGE_LOCATION.ToSafeString(),
                            materialCode = l.CPE_MATERIAL_CODE.ToSafeString(),
                            registerDate = l.REGISTER_DATE.ToSafeString(),
                            fibrenetId = l.FIBRENET_ID.ToSafeString(),
                            snPattern = l.SN_PATTERN.ToSafeString(),
                            shipTo = l.SHIP_TO.ToSafeString(),
                            warrantyStartDate = l.WARRANTY_START_DATE.ToSafeString(),
                            warrantyEndDate = l.WARRANTY_END_DATE.ToSafeString()
                        });
                    }
                    else
                    {
                        airCPEtmp = new List<airRegistCpeSerialRecord>();
                    }
                }
                else
                {
                    if (crpm.CPE_Info != null)
                    {
                        airCPEtmp = crpm.CPE_Info.ConvertAll(l => new airRegistCpeSerialRecord()
                        {
                            serialNo = l.SN.ToSafeString(),
                            cpeType = l.cpe_type.ToSafeString(),
                            macAddress = l.CPE_MAC_ADDR.ToSafeString(),

                            //20.4
                            statusDesc = l.STATUS_DESC.ToSafeString(),
                            modelName = l.CPE_MODEL_NAME.ToSafeString(),
                            companyCode = l.CPE_COMPANY_CODE.ToSafeString(),
                            cpePlant = l.CPE_PLANT.ToSafeString(),
                            storageLocation = l.CPE_STORAGE_LOCATION.ToSafeString(),
                            materialCode = l.CPE_MATERIAL_CODE.ToSafeString(),
                            registerDate = l.REGISTER_DATE.ToSafeString(),
                            fibrenetId = l.FIBRENET_ID.ToSafeString(),
                            snPattern = l.SN_PATTERN.ToSafeString(),
                            shipTo = l.SHIP_TO.ToSafeString(),
                            warrantyStartDate = l.WARRANTY_START_DATE.ToSafeString(),
                            warrantyEndDate = l.WARRANTY_END_DATE.ToSafeString()
                        });
                    }
                    else
                    {
                        airCPEtmp = new List<airRegistCpeSerialRecord>();
                    }
                }


                var airCPE = airCPEtmp.ToArray();


                #region mobile,ca,sa,ba
                var CA_ID = "";
                var SA_ID = "";
                var BA_ID = "";
                var P_AIS_MOBILE = "";
                var P_AIS_NONMOBILE = "";
                var Productname = "";
                var ServiceYear = "0";

                if (cpm.P_MOBILE.ToSafeString() != "")
                {
                    cpm.P_MOBILE = cpm.P_MOBILE.Replace("|", "");
                    if (cpm.P_MOBILE != "")
                    {
                        if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) == "0")
                        {
                            P_AIS_NONMOBILE = "";
                            P_AIS_MOBILE = cpm.P_MOBILE.ToSafeString();
                        }
                        else if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) != "0")
                        {
                            P_AIS_MOBILE = "";
                            P_AIS_NONMOBILE = cpm.P_MOBILE.ToSafeString();
                        }
                    }
                }

                //R21.10 MOU
                if (cpm.SAVEORDER_MOU_FLAG == "Y")
                {
                    P_AIS_NONMOBILE = crpm.FIBRE_ID.ToSafeString();
                }

                if (spm.VAS_FLAG.ToSafeString() == "1" || (spm.VAS_FLAG.ToSafeString() == "0" && spm.TOPUP.ToSafeString() == ""))
                {
                    Productname = cpm.SffProductName.ToSafeString();
                    ServiceYear = cpm.SffServiceYear.ToSafeString();


                    if (cpm.P_MOBILE.ToSafeString() == "")
                    {
                        CA_ID = "";
                        SA_ID = "";
                        BA_ID = "";
                    }
                    else
                    {
                        if (quickWinModel.CoveragePanelModel.ChargeType == "PREPAID")
                        {
                            CA_ID = "";
                            SA_ID = "";
                            BA_ID = "";
                        }
                        else if (cpm.BillingSystem.ToSafeString() == "BOS" && quickWinModel.CoveragePanelModel.ChargeType == "POSTPAID")
                        {
                            if (quickWinModel.CoveragePanelModel.BundlingSpecialFlag == "Y" || quickWinModel.CoveragePanelModel.BundlingMainFlag == "Y")
                            {
                                CA_ID = cpm.CA_ID.ToSafeString(); ;
                                SA_ID = cpm.SA_ID.ToSafeString(); ;
                                BA_ID = "";
                            }
                            else
                            {
                                CA_ID = "";
                                SA_ID = "";
                                BA_ID = "";
                            }
                        }
                        else
                        {
                            CA_ID = cpm.CA_ID.ToSafeString();
                            SA_ID = cpm.SA_ID.ToSafeString();
                            BA_ID = "";
                        }

                    }
                }
                else if (spm.VAS_FLAG.ToSafeString() == "2" || spm.TOPUP.ToSafeString() == "1")
                {
                    Productname = cpm.SffProductName;
                    ServiceYear = cpm.SffServiceYear;
                    CA_ID = cpm.CA_ID;
                    SA_ID = cpm.SA_ID;
                    BA_ID = cpm.BA_ID;
                }
                else if (spm.VAS_FLAG.ToSafeString() == "3")
                {
                    if (cpm.BillingSystem.ToSafeString() == "BOS" && quickWinModel.CoveragePanelModel.ChargeType == "POSTPAID")
                    {
                        if (quickWinModel.CoveragePanelModel.BundlingSpecialFlag == "Y" || quickWinModel.CoveragePanelModel.BundlingMainFlag == "Y")
                        {
                            CA_ID = cpm.CA_ID.ToSafeString(); ;
                            SA_ID = cpm.SA_ID.ToSafeString(); ;
                            BA_ID = "";
                        }
                        else
                        {
                            CA_ID = "";
                            SA_ID = "";
                            BA_ID = "";
                        }
                    }
                    else
                    {
                        CA_ID = cpm.CA_ID.ToSafeString();//cpm.CA_ID.ToSafeString();
                        SA_ID = cpm.SA_ID.ToSafeString();//cpm.SA_ID.ToSafeString();
                        BA_ID = "";
                    }
                }
                #endregion

                #region install date
                var tempdatefinal = crpm.FBSSTimeSlot.AppointmentDate.ToDisplayText("dd/MM/yyyy");
                #endregion


                #region vendor
                var vendor = "";
                if (quickWinModel.CoverageAreaResultModel.IS_3BB_COVERAGE)
                {
                    // Cass 3BB
                    string referenceId = "";

                    if (quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST != null
                        && quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST.Count > 0)
                    {
                        referenceId = quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST[0].referenceId;
                    }

                    vendor = "3BB_" + referenceId;
                }
                else
                {
                    if (spm.VAS_FLAG == "2" || spm.TOPUP == "1")
                    {
                        if (spm.PackageModelList.Any())
                        {
                            var listowner = _lovService.Get().Where(t => t.LOV_NAME == "MAPPING_OWNER_PRODUCT").Select(l => l.LOV_VAL3).ToList();

                            if (listowner.Contains(spm.PackageModelList[0].OWNER_PRODUCT.ToSafeString()))
                            {
                                vendor = spm.PackageModelList[0].OWNER_PRODUCT.ToSafeString();
                            }

                        }
                    }
                    else
                    {
                        PackageModel PackageMain = null;
                        if (spm.PackageModelList.Any())
                            PackageMain = spm.PackageModelList.FirstOrDefault(t => t.PACKAGE_TYPE == "1");


                        if (PackageMain != null)
                        {
                            if (PackageMain.PRODUCT_SUBTYPE == "FTTx" || PackageMain.PRODUCT_SUBTYPE == "FTTR")
                            {
                                if (cpm.P_FTTX_VENDOR.ToSafeString().Contains("|"))
                                {
                                    var tempowner = cpm.P_FTTX_VENDOR.Split('|');
                                    var listowner = _lovService.Get().Where(t => t.LOV_NAME == "MAPPING_OWNER_PRODUCT").Select(l => l.LOV_VAL3).ToList();
                                    foreach (var a in tempowner)
                                    {
                                        if (listowner.Contains(a))
                                        {
                                            vendor = a;
                                        }

                                    }
                                }
                                else if (PackageMain.PRODUCT_SUBTYPE == "FTTR")
                                {
                                    vendor = PackageMain.OWNER_PRODUCT.ToSafeString();
                                }
                                else
                                {
                                    vendor = PackageMain.OWNER_PRODUCT.ToSafeString();
                                }
                            }
                            else if (PackageMain.PRODUCT_SUBTYPE == "WTTx")
                            {
                                vendor = "AWN";
                            }
                        }
                    }
                }
                #endregion

                #region OWNER_PRODUCT Case 3BB
                if (quickWinModel.CoverageAreaResultModel.IS_3BB_COVERAGE)
                {
                    string referenceId = "";

                    if (quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST != null
                        && quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST.Count > 0)
                    {
                        referenceId = quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST[0].referenceId;
                    }

                    cpm.SAVEORDER_OWNER_PRODUCT = "3BB_" + referenceId;
                }
                #endregion

                //// phone flag
                if (cpm.L_HAVE_FIXED_LINE.ToSafeString() == "")
                {
                    cpm.L_HAVE_FIXED_LINE = "N";
                }

                var EngFlag = "N";
                if (query.CurrentCulture.IsEngCulture())
                {
                    EngFlag = "Y";
                }

                if (tempdatefinal == "-")
                {
                    tempdatefinal = "";
                }
                var temp2 = crpm.ListImageFile;
                var airImage = temp2.Select(o => new airRegistFileRecord()
                {
                    fileName = o.FileName

                }).ToArray();

                var email = cpm.L_CONTACT_EMAIL.ToSafeString();
                var lineid = cpm.L_CONTACT_LINE_ID.ToSafeString();
                var flowflag = quickWinModel.FlowFlag.ToSafeString();
                var sitecode = quickWinModel.SiteCode.ToSafeString();
                List<airRegistSplitterRecord> splittertmp;
                if (quickWinModel.CoverageAreaResultModel.SPLITTER_LIST != null)
                {
                    splittertmp = quickWinModel.CoverageAreaResultModel.SPLITTER_LIST.ConvertAll(x => new airRegistSplitterRecord
                    {
                        splitterName = x.Splitter_Name.ToSafeString(),
                        distance = x.Distance,
                        distanceType = x.Distance_Type.ToSafeString(),
                        distanceSpecified = true,
                        resourceType = "SPLITTER"
                    });
                }
                else if (quickWinModel.CoverageAreaResultModel.RESOURCE_LIST != null)
                {
                    splittertmp = quickWinModel.CoverageAreaResultModel.RESOURCE_LIST.ConvertAll(x => new airRegistSplitterRecord
                    {
                        splitterName = x.Dslam_Name.ToSafeString(),
                        distance = 0,
                        distanceType = "",
                        distanceSpecified = true,
                        resourceType = "DSLAM"
                    });
                }
                else if (quickWinModel.CoverageAreaResultModel.IS_3BB_COVERAGE
                    && quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST != null
                    && quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST.Count > 0)
                {
                    splittertmp = quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST.ConvertAll(x => new airRegistSplitterRecord
                    {
                        splitterName = x.splitterCode.ToSafeString(),
                        distance = x.distance.ToSafeDecimal(),
                        distanceType = "",
                        resourceType = "SPLITTER"
                    }).ToList();
                }
                else
                {
                    splittertmp = new List<airRegistSplitterRecord>();
                }
                var airSplitter = splittertmp.ToArray();

                // Splitter 3BB
                List<airRegistCrossnetworkRecord> crossnetworkRecord = new List<airRegistCrossnetworkRecord>();
                if (quickWinModel.CoverageAreaResultModel.IS_3BB_COVERAGE && quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST != null
                    && quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST.Count > 0)
                {
                    // Set Splitter 3BB
                    SPLITTER_3BB_RESERVED Splitter3bbReserved = quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST[0];
                    crossnetworkRecord = quickWinModel.CoverageAreaResultModel.SPLITTER_3BB_RESERVED_LIST
                        .Select(t => new airRegistCrossnetworkRecord()
                        {
                            distance = t.distance.ToSafeString(),
                            ishome = t.isHome.ToSafeString(),
                            mdfname = t.mdfName.ToSafeString(),
                            mdfport = t.mdfPort.ToSafeString(),
                            referenceid = t.referenceId.ToSafeString(),
                            splitteralias = t.splitterAlias.ToSafeString(),
                            splittercode = t.splitterCode.ToSafeString(),
                            splitterlatitude = t.splitterLatitude.ToSafeString(),
                            splitterlongitude = t.splitterLongitude.ToSafeString(),
                            splitterport = t.splitterPort.ToSafeString(),
                            threebbFlag = "Y"
                        }).ToList();
                }

                //TODO: Splitter Management
                var splitterFlag = string.Empty;
                var reservedPortId = string.Empty;
                var spacialRemark = string.Empty;
                if (!string.IsNullOrEmpty(query.QuickWinPanelModel.FlowFlag)
                    && query.QuickWinPanelModel.CoveragePanelModel.AccessMode == "FTTH")
                {
                    splitterFlag = query.QuickWinPanelModel.SplitterFlag;
                    reservedPortId = query.QuickWinPanelModel.ReservationId;

                    var configNoteForCs =
                        _lovService.Get().FirstOrDefault(
                            item => item.LOV_NAME == "SaveOrderNew" &&
                                    item.LOV_VAL1 == query.QuickWinPanelModel.SplitterFlagFirstTime &&
                                    item.LOV_VAL2 == query.QuickWinPanelModel.SplitterFlag) ?? new FBB_CFG_LOV();
                    spacialRemark = configNoteForCs.LOV_VAL3;
                }
                else if (query.QuickWinPanelModel.CoveragePanelModel.AccessMode == "VDSL")
                {
                    splitterFlag = query.QuickWinPanelModel.SplitterFlag;
                    reservedPortId = query.QuickWinPanelModel.ReservationId;
                    var configNoteForCs =
                        _lovService.Get().FirstOrDefault(
                            item => item.LOV_NAME == "SaveOrderNew" &&
                                    item.DISPLAY_VAL == "SPECIAL_REMARK_FTTB" &&
                                    item.LOV_VAL1 == query.QuickWinPanelModel.SplitterFlagFirstTime
                    ) ?? new FBB_CFG_LOV();
                    spacialRemark = configNoteForCs.LOV_VAL3;
                }
                else if (query.QuickWinPanelModel.CoveragePanelModel.AccessMode == "WTTx")
                {
                    splitterFlag = query.QuickWinPanelModel.SplitterFlag;
                    reservedPortId = query.QuickWinPanelModel.ReservationId;
                    var configNoteForCs =
                        _lovService.Get().FirstOrDefault(
                            item => item.LOV_NAME == "SaveOrderNew" &&
                                    item.DISPLAY_VAL == "SPECIAL_REMARK_WTTX"
                    ) ?? new FBB_CFG_LOV();
                    spacialRemark = configNoteForCs.LOV_VAL1;
                }
                //TODO: eStateMent
                //18.2 eBill
                var lovEstatement = (from z in _lov.Get()
                                     where z.LOV_NAME == "ESTATEMENT_STATUS" && z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "FBB_CONSTANT"
                                     select z).FirstOrDefault() ?? new FBB_CFG_LOV();

                var billMedia = string.Empty;
                if (!string.IsNullOrEmpty(query.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag))
                {
                    if (query.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag == "0")
                    {
                        billMedia = lovEstatement.LOV_VAL2;
                    }
                    else if (query.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag == "1")
                    {
                        billMedia = lovEstatement.LOV_VAL3;
                    }
                    else if (query.QuickWinPanelModel.CustomerRegisterPanelModel.EBillFlag == "2")
                    {
                        billMedia = lovEstatement.LOV_VAL1;
                    }
                }

                #endregion

                string[] inputparam = (from z in _lov.Get()
                                       where z.LOV_NAME == "SERENADE_MOBILE_SEGMENT" && z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "FBB_CONSTANT"
                                       select z.LOV_VAL1).ToArray();

                bool IsSerenade = false;
                string mSeg = quickWinModel.CoveragePanelModel.MobileSegment != null ? quickWinModel.CoveragePanelModel.MobileSegment.ToUpper() : "";
                string NetworkType = quickWinModel.CoveragePanelModel.NetworkType != null ? quickWinModel.CoveragePanelModel.NetworkType.ToUpper() : "";

                string mSegPre = quickWinModel.CoveragePanelModel.Mobile_Segment != null ? quickWinModel.CoveragePanelModel.Mobile_Segment.ToUpper() : "";
                if (inputparam.Contains(mSeg) || inputparam.Contains(mSegPre))
                    IsSerenade = true;
                string CType = quickWinModel.CoveragePanelModel.ChargeType;

                //17.7 MGM
                var MemberGetMemberInfo = quickWinModel.CoveragePanelModel.CoverageMemberGetMember;
                var reffernce_no = MemberGetMemberInfo.RefferenceNo.ToSafeString();
                var voucher_desc = MemberGetMemberInfo.VoucherDesc.ToSafeString();
                var campaign_project_name = MemberGetMemberInfo.CampaignProjectName.ToSafeString();
                var channel = quickWinModel.RegisterChannel.ToSafeString();
                var dev_project_code = quickWinModel.CustomerRegisterPanelModel.p_dev_project_code.ToSafeString();
                var dev_bill_to = quickWinModel.CustomerRegisterPanelModel.p_dev_bill_to.ToSafeString();
                var dev_price = quickWinModel.CustomerRegisterPanelModel.p_dev_price.ToSafeString();
                var dev_po_no = quickWinModel.CustomerRegisterPanelModel.PO_NO.ToSafeString();

                //18.6 SCPE

                if (crpm.SCPE_USE_LOC_CODE == "Y")
                {
                    crpm.L_LOC_CODE = "";
                }
                if (crpm.SCPE_USE_LOC_CODE == "N")
                {
                    crpm.L_LOC_CODE = crpm.SCPE_LOC_CODE;
                    crpm.SCPE_LOC_CODE = "";
                }

                if (crpm.SCPE_USE_ASC_CODE == "Y")
                {
                    crpm.L_ASC_CODE = "";
                }
                if (crpm.SCPE_USE_ASC_CODE == "N")
                {
                    crpm.L_ASC_CODE = crpm.SCPE_ASC_CODE;
                    crpm.SCPE_ASC_CODE = "";
                }

                string PayMentMethod = quickWinModel.PayMentMethod.ToSafeString();
                string PayMentOrderID = quickWinModel.PayMentOrderID.ToSafeString();
                string PayMentTranID = quickWinModel.PayMentTranID.ToSafeString();

                var userReferencetransaction = quickWinModel.SessionId;

                #region SBNService
                SBNSaveOrder(response, cpm, dpm, crpm, title,
                    firstName, lastName, cTitle, cFirstName,
                    cLastName, lineid, email, flowflag, sitecode, phoneNo, cPhoneNo, cardNo, taxId,
                    cusNat, addressTypeWire, CONDO_ROOF_TOP, CONDO_BALCONY,
                    BALCONY_NORTH, BALCONY_SOUTH, BALCONY_EAST, BALCONY_WAST,
                    HIGH_BUILDING, HIGH_TREE, BILLBOARD, EXPRESSWAY, floorNo,
                    condoFloor, birthDateString, airregists, CA_ID, SA_ID,
                    BA_ID, P_AIS_MOBILE, P_AIS_NONMOBILE, Productname,
                    ServiceYear, tempdatefinal, vendor, "", EngFlag, airImage, airSplitter, airCPE, _lov,
                    CType, IsSerenade, spm.RESERVED_ID, splitterFlag, reservedPortId, spacialRemark, string.Empty, string.Empty, billMedia,
                    reffernce_no, voucher_desc, campaign_project_name, channel, dev_project_code, dev_bill_to, dev_price, dev_po_no,
                    PayMentMethod, PayMentOrderID, PayMentTranID, userReferencetransaction, oim, crossnetworkRecord.ToArray());
                #endregion

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response, log, "Success", "", "");

            }
            catch (Exception ex)
            {

                _logger.Info(string.Format("saveOrderNewError = {0}", ex.GetBaseException()));

                response.RETURN_CODE = -1;
                response.RETURN_MESSAGE = "Error Before Call Airnet Service " + ex.GetErrorMessage();
                response.RETURN_IA_NO = "";

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "RETURN_CODE : " + response.RETURN_CODE + "ErrorMessage : " + ex.GetErrorMessage(), "");
                }
            }

            return response;
        }

        private static void SBNSaveOrder(SaveOrderResp response,
            CoveragePanelModel cpm,
            DisplayPackagePanelModel dpm,
            CustomerRegisterPanelModel crpm,
            string title, string firstName, string lastName,
            string cTitle, string cFirstName, string cLastName,
            string lineId, string email, string flowFlag, string siteCode,
            string phoneNo, string cPhoneNo, string cardNo,
            string taxId, string cusNat, string addressTypeWire,
            string CONDO_ROOF_TOP, string CONDO_BALCONY, string BALCONY_NORTH,
            string BALCONY_SOUTH, string BALCONY_EAST, string BALCONY_WAST,
            string HIGH_BUILDING, string HIGH_TREE, string BILLBOARD, string EXPRESSWAY,
            string floorNo, string condoFloor, string birthDateString,
            airRegistPackageRecord[] airregists, string CA_ID, string SA_ID,
            string BA_ID, string P_AIS_MOBILE, string P_AIS_NONMOBILE,
            string Productname, string ServiceYear, string tempdatefinal,
            string vendor, string installNote, string engFlag, airRegistFileRecord[] airImage, airRegistSplitterRecord[] airSplitter, airRegistCpeSerialRecord[] airCPE, IEntityRepository<FBB_CFG_LOV> _lov,
            string ChargeType, bool IsSerenade, string RESERVED_ID,
            string SPLITTER_FLAG, string RESERVED_PORT_ID, string SPECIAL_REMARK, string ORDER_NO, string SOURCE_SYSTEM, string BILL_MEDIA,
            string reffernce_no, string voucher_desc, string campaign_project_name, string channel, string dev_project_code, string dev_bill_to, string dev_price, string dev_po_no,
            string PayMentMethod, string PayMentOrderID, string PayMentTranID, string userReferencetransaction, OfficerInfoPanelModel oim, airRegistCrossnetworkRecord[] airCrossnetwork)
        {
            _loggers.Info(string.Format("step 2  NEW"));

            string tmp_mail = email;
            tmp_mail = string.IsNullOrEmpty(tmp_mail) ? crpm.L_EMAIL.ToSafeString() : email;
            #region newSNBService
            //ถ้าเป็นเบอร์ prepaid
            if (ChargeType == "PREPAID")
            {
                //Productname = "3G|PREPAID";
                //เป็น Serenade
                if (IsSerenade)
                {
                    Productname = cpm.NetworkType.ToSafeString() + "|PREPAID|SERENADE";
                }
                else
                {
                    Productname = cpm.NetworkType.ToSafeString() + "|PREPAID";
                }
            }
            //ถ้าเป็นเบอร์ postpaid
            else if (ChargeType == "POSTPAID")
            {
                //เป็น Serenade
                if (IsSerenade)
                {
                    Productname = cpm.NetworkType.ToSafeString() + "|POSTPAID|SERENADE";
                }
                else
                {
                    Productname = cpm.NetworkType.ToSafeString() + "|POSTPAID";
                }
            }
            else
                Productname = cpm.NetworkType.ToSafeString();

            _loggers.Info(string.Format("step 3  Productname = {0}", Productname));

            string strPlug_and_play_flag = "";
            if (!string.IsNullOrEmpty(crpm.L_EVENT_CODE.ToSafeString()))
            {
                strPlug_and_play_flag = crpm.Plug_and_play_flag.ToSafeString();
            }

            //R18.1 FTTB Sell Router
            if (!string.IsNullOrEmpty(crpm.RouterFlag))
            {
                if (crpm.RouterFlag == "S")
                {
                    strPlug_and_play_flag = "4";
                }
                else if (crpm.RouterFlag == "M")
                {
                    strPlug_and_play_flag = "3";
                }
            }

            _loggers.Info(string.Format("step 4  RouterFlag = {0}", crpm.RouterFlag));

            if (crpm.CateType.ToSafeString().Equals("R"))
            {
                _loggers.Info(string.Format("step 5  CateType = {0}", crpm.RouterFlag));

                crpm.AddressPanelModelVat.L_HOME_NUMBER_2 = crpm.AddressPanelModelSendDocIDCard.L_HOME_NUMBER_2;
                crpm.AddressPanelModelVat.L_SOI = crpm.AddressPanelModelSendDocIDCard.L_SOI;
                crpm.AddressPanelModelVat.L_MOO = crpm.AddressPanelModelSendDocIDCard.L_MOO;
                crpm.AddressPanelModelVat.L_MOOBAN = crpm.AddressPanelModelSendDocIDCard.L_MOOBAN;
                crpm.AddressPanelModelVat.L_BUILD_NAME = crpm.AddressPanelModelSendDocIDCard.L_BUILD_NAME;
                crpm.AddressPanelModelVat.L_FLOOR = crpm.AddressPanelModelSendDocIDCard.L_FLOOR;
                crpm.AddressPanelModelVat.L_ROOM = crpm.AddressPanelModelSendDocIDCard.L_ROOM;
                crpm.AddressPanelModelVat.L_ROAD = crpm.AddressPanelModelSendDocIDCard.L_ROAD;
                crpm.AddressPanelModelVat.ZIPCODE_ID = crpm.AddressPanelModelSendDocIDCard.ZIPCODE_ID;
            }

            List<airRegistCustInsightRecord> custInsightRecord = new List<airRegistCustInsightRecord>();
            if (crpm.ListCustomerInsight != null && crpm.ListCustomerInsight.Count > 0)
            {
                custInsightRecord = crpm.ListCustomerInsight.Select(p => new airRegistCustInsightRecord
                {
                    groupId = p.GROUP_ID.ToSafeString(),
                    groupNameTh = p.GROUP_NAME_TH.ToSafeString(),
                    groupNameEn = p.GROUP_NAME_EN.ToSafeString(),
                    questionId = p.QUESTION_ID.ToSafeString(),
                    questionTh = p.QUESTION_TH.ToSafeString(),
                    questionEn = p.QUESTION_EN.ToSafeString(),
                    answerId = p.ANSWER_ID.ToSafeString(),
                    answerTh = p.ANSWER_TH.ToSafeString(),
                    answerEn = p.ANSWER_EN.ToSafeString(),
                    answerValueTh = p.ANSWER_VALUE_TH.ToSafeString(),
                    answerValueEn = p.ANSWER_VALUE_EN.ToSafeString(),
                    parentAnswerId = p.PARENT_ANSWER_ID.ToSafeString(),
                    actionWfm = p.ACTION_WFM.ToSafeString(),
                    actionFoa = p.ACTION_FOA.ToSafeString()
                }).ToList();
            }

            List<airRegistDcontractRecord> dcontractRecord = new List<airRegistDcontractRecord>();
            if (crpm.ListDcontract != null && crpm.ListDcontract.Count > 0)
            {
                dcontractRecord = crpm.ListDcontract.Select(p => new airRegistDcontractRecord
                {
                    productSubtype = p.PRODUCT_SUBTYPE.ToSafeString(),
                    pboxExt = p.PBOX_EXT.ToSafeString(),
                    tdmContractId = p.TDM_CONTRACT_ID.ToSafeString(),
                    tdmRuleId = p.TDM_RULE_ID.ToSafeString(),
                    tdmPenaltyId = p.TDM_PENALTY_ID.ToSafeString(),
                    tdmPenaltyGroupId = p.TDM_PENALTY_GROUP_ID.ToSafeString(),
                    duration = p.DURATION.ToSafeString(),
                    contractFlag = p.CONTRACT_FLAG.ToSafeString(),
                    deviceCount = p.DEVICE_COUNT.ToSafeString()
                }).ToList();
            }
            //R23.05 CheckFraud
            List<airFraudReasonRecord> fraudReason = new List<airFraudReasonRecord>();
            if (!string.IsNullOrEmpty(crpm.CHECK_FRAUD_INFO.FLAG_GO_NOGO))
            {
                if (crpm.CHECK_FRAUD_INFO.FRAUD_REASONS != null && crpm.CHECK_FRAUD_INFO.FRAUD_REASONS.Count > 0)
                {
                    fraudReason = crpm.CHECK_FRAUD_INFO.FRAUD_REASONS.Select(f => new airFraudReasonRecord
                    {
                        reasons = f.REASON ?? "",
                        score = f.SCORE.ToSafeDecimal()
                        //scoreSpecified = false
                    }).ToList();
                }
            }
            //end R23.05 CheckFraud
            _loggers.Info(string.Format("step 6  CateType = {0}", crpm.RouterFlag));

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;

            using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
            {
                service.Timeout = 600000;

                string tmpUrl = (from r in _lov.Get()
                                 where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                                 select r.LOV_VAL1).FirstOrDefault().ToSafeString();
                if (tmpUrl != "")
                {
                    service.Url = tmpUrl;
                }

                //convert 
                int dev_price_convert = 0;
                if (!string.IsNullOrEmpty(dev_price))
                {
                    _loggers.Info(string.Format("step 6.1  dev_price = {0}", dev_price));
                    dev_price_convert = dev_price.ToSafeInteger();
                }

                // Case WTTx
                string NON_MOBILE_NO = "";
                if (cpm.AccessMode.ToSafeString() == "WTTx")
                {
                    NON_MOBILE_NO = P_AIS_NONMOBILE.ToSafeString();
                }
                //TODO: for log Change pack fieldwork R19.5
                service.Credentials = new NetworkCredential(userReferencetransaction, "");
                try
                {
                    //var test = crpm.CHECK_FRAUD_INFO;
                    //var test2 = fraudReason;
                    var data = service.saveOrderNew(
                    CUSTOMER_TYPE: crpm.CateType.ToSafeString(),
                    CUSTOMER_SUBTYPE: crpm.SubCateType.ToSafeString(),
                    TITLE_CODE: title.ToSafeString(),
                    FIRST_NAME: firstName.ToSafeString().Trim(),
                    LAST_NAME: lastName.ToSafeString().Trim(),
                    CONTACT_TITLE_CODE: cTitle.ToSafeString(),
                    CONTACT_FIRST_NAME: cFirstName.ToSafeString().Trim(),
                    CONTACT_LAST_NAME: cLastName.ToSafeString().Trim(),
                    ID_CARD_TYPE_DESC: crpm.L_CARD_TYPE.ToSafeString(),
                    ID_CARD_NO: cardNo.ToSafeString(),
                    TAX_ID: taxId.ToSafeString(),
                    GENDER: crpm.L_GENDER.ToSafeString(),
                    BIRTH_DATE: birthDateString.ToSafeString(),
                    MOBILE_NO: phoneNo.ToSafeString(),
                    MOBILE_NO_2: crpm.L_OR.ToSafeString(),
                    HOME_PHONE_NO: crpm.L_HOME_PHONE.ToSafeString(),
                    EMAIL_ADDRESS: tmp_mail,
                    CONTACT_TIME: crpm.L_SPECIFIC_TIME.ToSafeString(),
                    NATIONALITY_DESC: cusNat.ToSafeString(),
                    CUSTOMER_REMARK: crpm.L_REMARK.ToSafeString(),

                    HOUSE_NO: crpm.AddressPanelModelSetup.L_HOME_NUMBER_2.ToSafeString(),
                    MOO_NO: crpm.AddressPanelModelSetup.L_MOO.ToSafeString(),
                    BUILDING: crpm.AddressPanelModelSetup.L_BUILD_NAME.ToSafeString(),
                    FLOOR: crpm.AddressPanelModelSetup.L_FLOOR.ToSafeString(),
                    ROOM: crpm.AddressPanelModelSetup.L_ROOM.ToSafeString(),
                    MOOBAN: crpm.AddressPanelModelSetup.L_MOOBAN.ToSafeString(),
                    SOI: crpm.AddressPanelModelSetup.L_SOI.ToSafeString(),
                    ROAD: crpm.AddressPanelModelSetup.L_ROAD.ToSafeString(),
                    ZIPCODE_ROWID: crpm.AddressPanelModelSetup.ZIPCODE_ID.ToSafeString(),

                    LATITUDE: cpm.L_LAT.ToSafeString(),
                    LONGTITUDE: cpm.L_LONG.ToSafeString(),
                    ASC_CODE: crpm.L_ASC_CODE.ToSafeString(),
                    EMPLOYEE_ID: crpm.L_STAFF_ID.ToSafeString(),
                    LOCATION_CODE: crpm.L_LOC_CODE.ToSafeString(),
                    SALE_REPRESENT: crpm.L_SALE_REP.ToSafeString(),
                    CS_NOTE: crpm.L_FOR_CS_TEAM.ToSafeString(),
                    WIFI_ACCESS_POINT: dpm.WIFIAccessPoint.ToSafeString(),
                    INSTALL_STATUS: "N",
                    COVERAGE: cpm.L_RESULT.ToSafeString(),
                    EXISTING_AIRNET_NO: "",
                    GSM_MOBILE_NO: "",
                    CONTACT_NAME_1: cFirstName.ToSafeString(),
                    CONTACT_NAME_2: cLastName.ToSafeString(),
                    CONTACT_MOBILE_NO_1: cPhoneNo.ToSafeString(),
                    CONTACT_MOBILE_NO_2: crpm.L_OR.ToSafeString(),
                    CONDO_FLOOR: condoFloor.ToSafeString(),
                    CONDO_ROOF_TOP: CONDO_ROOF_TOP.ToSafeString(),
                    CONDO_BALCONY: CONDO_BALCONY.ToSafeString(),
                    BALCONY_NORTH: BALCONY_NORTH.ToSafeString(),
                    BALCONY_SOUTH: BALCONY_SOUTH.ToSafeString(),
                    BALCONY_EAST: BALCONY_EAST.ToSafeString(),
                    BALCONY_WAST: BALCONY_WAST.ToSafeString(),
                    HIGH_BUILDING: HIGH_BUILDING.ToSafeString(),
                    HIGH_TREE: HIGH_TREE.ToSafeString(),
                    BILLBOARD: BILLBOARD.ToSafeString(),
                    EXPRESSWAY: EXPRESSWAY.ToSafeString(),
                    ADDRESS_TYPE_WIRE: addressTypeWire.ToSafeString(),
                    ADDRESS_TYPE: crpm.L_TYPE_ADDR.ToSafeString(),
                    FLOOR_NO: floorNo.ToSafeString(),

                    HOUSE_NO_BL: crpm.AddressPanelModelSendDoc.L_HOME_NUMBER_2.ToSafeString(),
                    MOO_NO_BL: crpm.AddressPanelModelSendDoc.L_MOO.ToSafeString(),
                    MOOBAN_BL: crpm.AddressPanelModelSendDoc.L_MOOBAN.ToSafeString(),
                    BUILDING_BL: crpm.AddressPanelModelSendDoc.L_BUILD_NAME.ToSafeString(),
                    FLOOR_BL: crpm.AddressPanelModelSendDoc.L_FLOOR.ToSafeString(),
                    ROOM_BL: crpm.AddressPanelModelSendDoc.L_ROOM.ToSafeString(),
                    SOI_BL: crpm.AddressPanelModelSendDoc.L_SOI.ToSafeString(),
                    ROAD_BL: crpm.AddressPanelModelSendDoc.L_ROAD.ToSafeString(),
                    ZIPCODE_ROWID_BL: crpm.AddressPanelModelSendDoc.ZIPCODE_ID.ToSafeString(),

                    HOUSE_NO_VT: crpm.AddressPanelModelVat.L_HOME_NUMBER_2.ToSafeString(),
                    MOO_NO_VT: crpm.AddressPanelModelVat.L_MOO.ToSafeString(),
                    MOOBAN_VT: crpm.AddressPanelModelVat.L_MOOBAN.ToSafeString(),
                    BUILDING_VT: crpm.AddressPanelModelVat.L_BUILD_NAME.ToSafeString(),
                    FLOOR_VT: crpm.AddressPanelModelVat.L_FLOOR.ToSafeString(),
                    ROOM_VT: crpm.AddressPanelModelVat.L_ROOM.ToSafeString(),
                    SOI_VT: crpm.AddressPanelModelVat.L_SOI.ToSafeString(),
                    ROAD_VT: crpm.AddressPanelModelVat.L_ROAD.ToSafeString(),
                    ZIPCODE_ROWID_VT: crpm.AddressPanelModelVat.ZIPCODE_ID.ToSafeString(),

                    CVR_ID: "",
                    CVR_NODE: "",
                    CVR_TOWER: "",

                    SITE_CODE: siteCode.ToSafeString(),

                    RELATE_MOBILE: P_AIS_MOBILE.ToSafeString(),
                    RELATE_NON_MOBILE: P_AIS_NONMOBILE.ToSafeString(),
                    SFF_CA_NO: CA_ID.ToSafeString(),
                    SFF_SA_NO: SA_ID.ToSafeString(),
                    //SFF_BA_NO: BA_ID.ToSafeString(),
                    //R23.04
                    //SFF_BA_NO: crpm.BILL_ADDRESS_NO.ToSafeString() ?? BA_ID.ToSafeString(), before fix R.24.10_29102024 chats885
                    SFF_BA_NO: string.IsNullOrEmpty(crpm.BILL_ADDRESS_NO.ToSafeString()) ? BA_ID.ToSafeString() : crpm.BILL_ADDRESS_NO.ToSafeString(), //after fix R.24.10_29102024 chats885
                    NETWORK_TYPE: Productname.ToSafeString(),
                    SERVICE_DAY: ServiceYear.ToSafeString(),
                    EXPECT_INSTALL_DATE: tempdatefinal.ToSafeString(),

                    FTTX_VENDOR: vendor.ToSafeString(),
                    INSTALL_NOTE: installNote.ToSafeString(),

                    PHONE_FLAG: cpm.L_HAVE_FIXED_LINE.ToSafeString(),
                    TIME_SLOT: crpm.FBSSTimeSlot.TimeSlot.ToSafeString(),
                    INSTALLATION_CAPACITY: crpm.FBSSTimeSlot.InstallationCapacity.ToSafeString(),
                    ADDRESS_ID: cpm.Address.AddressId.ToSafeString(),
                    ACCESS_MODE: cpm.AccessMode.ToSafeString(),

                    ENG_FLAG: engFlag.ToSafeString(),
                    EVENT_CODE: crpm.L_EVENT_CODE.ToSafeString(),
                    INSTALLADDRESS1: crpm.installAddress1.ToSafeString(),
                    INSTALLADDRESS2: crpm.installAddress2.ToSafeString(),
                    INSTALLADDRESS3: crpm.installAddress3.ToSafeString(),
                    INSTALLADDRESS4: crpm.installAddress4.ToSafeString(),
                    INSTALLADDRESS5: crpm.installAddress5.ToSafeString(),
                    PBOX_COUNT: crpm.pbox_count.ToSafeString(),
                    CONVERGENCE_FLAG: crpm.convergence_flag.ToSafeString(),
                    TIME_SLOT_ID: crpm.FBSSTimeSlot.TimeSlotId.ToSafeString(),

                    GIFT_VOUCHER: crpm.L_VOUCHER_PIN.ToSafeString(),
                    SUB_LOCATION_ID: crpm.AddressPanelModelVat.SUB_LOCATION_ID.ToSafeString(),
                    SUB_CONTRACT_NAME: crpm.AddressPanelModelVat.SUB_CONTRACT_NAME.ToSafeString(),
                    INSTALL_STAFF_ID: crpm.AddressPanelModelVat.INSTALL_STAFF_ID.ToSafeString(),
                    INSTALL_STAFF_NAME: crpm.AddressPanelModelVat.INSTALL_STAFF_NAME.ToSafeString(),
                    FLOW_FLAG: flowFlag.ToSafeString(),

                    LINE_ID: lineId.ToSafeString(),
                    RELATE_PROJECT_NAME: crpm.Project_name.ToSafeString(),
                    PLUG_AND_PLAY_FLAG: strPlug_and_play_flag,

                    RESERVED_ID: RESERVED_ID,
                    JOB_ORDER_TYPE: crpm.JOB_ORDER_TYPE,
                    ASSIGN_RULE: crpm.ASSIGN_RULE,
                    OLD_ISP: crpm.L_OLD_ISP.ToSafeString(),

                    SPLITTER_FLAG: SPLITTER_FLAG,
                    RESERVED_PORT_ID: RESERVED_PORT_ID,
                    SPECIAL_REMARK: SPECIAL_REMARK,
                    ORDER_NO: ORDER_NO,
                    SOURCE_SYSTEM: SOURCE_SYSTEM,

                    BILL_MEDIA: BILL_MEDIA,

                    PRE_ORDER_NO: reffernce_no.ToSafeString(),
                    VOUCHER_DESC: voucher_desc.ToSafeString(),
                    CAMPAIGN_PROJECT_NAME: campaign_project_name.ToSafeString(),
                    PRE_ORDER_CHANEL: channel.ToSafeString(),

                    RENTAL_FLAG: crpm.RentalFlag.ToSafeString(),
                    DEV_PROJECT_CODE: dev_project_code.ToSafeString(),
                    DEV_BILL_TO: dev_bill_to.ToSafeString(),
                    DEV_PO_NO: dev_po_no.ToSafeString(),

                    PARTNER_TYPE: crpm.outType.ToSafeString(),
                    PARTNER_SUBTYPE: crpm.outSubType.ToSafeString(),
                    MOBILE_BY_ASC: crpm.outMobileNo.ToSafeString(),
                    LOCATION_NAME: crpm.PartnerName.ToSafeString(),
                    PAYMENTMETHOD: PayMentMethod,
                    TRANSACTIONID_IN: PayMentOrderID,
                    TRANSACTIONID: PayMentTranID,

                    SUB_ACCESS_MODE: crpm.SUB_ACCESS_MODE.ToSafeString(),
                    REQUEST_SUB_FLAG: crpm.REQUEST_SUB_FLAG.ToSafeString(),
                    PREMIUM_FLAG: crpm.PREMIUM_FLAG.ToSafeString(),
                    RELATE_MOBILE_SEGMENT: crpm.RELATE_MOBILE_SEGMENT.ToSafeString(),
                    REF_UR_NO: crpm.REF_UR_NO.ToSafeString(),
                    LOCATION_EMAIL_BY_REGION: crpm.LOCATION_EMAIL_BY_REGION.ToSafeString(),
                    SALE_STAFF_NAME: crpm.EMP_NAME.ToSafeString(),

                    DOPA_FLAG: crpm.FlagDopaSubmit.ToSafeString(),
                    SERVICE_YEAR: crpm.SffServiceYear.ToSafeString(),
                    REQUIRE_CS_VERIFY_DOC: crpm.FlagVarifyDocuments.ToSafeString(),
                    FACERECOG_FLAG: crpm.FlagFaceRecognitionSubmit.ToSafeString(),

                    SPECIAL_ACCOUNT_NAME: crpm.SpecialAccountName.ToSafeString(),
                    SPECIAL_ACCOUNT_NO: crpm.SpecialAccountNo.ToSafeString(),
                    SPECIAL_ACCOUNT_ENDDATE: crpm.SpecialAccountEnddate.ToSafeString(),
                    SPECIAL_ACCOUNT_GROUP_EMAIL: crpm.SpecialAccountGroupEmail.ToSafeString(),
                    SPECIAL_ACCOUNT_FLAG: crpm.SpecialAccountFlag.ToSafeString(),

                    EXISTING_MOBILE_FLAG: crpm.Existing_Mobile.ToSafeString(),
                    PRE_SURVEY_DATE: crpm.PreSurveyDate.ToSafeString(),
                    PRE_SURVEY_TIMESLOT: crpm.PreSurveyTimeslot.ToSafeString(),

                    REGISTER_CHANNEL: crpm.RegisterChannelSaveOrder.ToSafeString(),
                    //// no CheckFraud
                    //AUTO_CREATE_PROSPECT_FLAG: crpm.AutoCreateProspectFlag.ToSafeString(),
                    //R23.05 CheckFraud
                    AUTO_CREATE_PROSPECT_FLAG: crpm.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG == null || crpm.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG == "" ? 
                    (crpm.AutoCreateProspectFlag == "Y" ? crpm.AutoCreateProspectFlag : "N") : crpm.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG,
                    ////end R23.05 CheckFraud
                    ORDER_VERIFY: crpm.OrderVerify.ToSafeString(),

                    WAITING_INSTALL_DATE: "",
                    WAITING_TIME_SLOT: "",

                    SALE_CHANNEL: cpm.SAVEORDER_SALE_CHANNEL.ToSafeString(),
                    OWNER_PRODUCT: cpm.SAVEORDER_OWNER_PRODUCT.ToSafeString(),
                    PACKAGE_FOR: cpm.SAVEORDER_PACKAGE_FOR.ToSafeString(),
                    SFF_PROMOTION_CODE: "",
                    REGION: cpm.SAVEORDER_REGION.ToSafeString(),
                    PROVINCE: cpm.SAVEORDER_PROVINCE.ToSafeString(),
                    DISTRICT: cpm.SAVEORDER_DISTRICT.ToSafeString(),
                    SUB_DISTRICT: cpm.SAVEORDER_SUB_DISTRICT.ToSafeString(),
                    SERENADE_FLAG: cpm.SAVEORDER_SERENADE_FLAG.ToSafeString(),
                    FMPA_FLAG: cpm.SAVEORDER_FMPA_FLAG.ToSafeString(),
                    CVM_FLAG: cpm.SAVEORDER_CVM_FLAG.ToSafeString(),

                    ORDER_RELATE_CHANGE_PRO: "",
                    COMPANY_NAME: (oim.outTitle.ToSafeString() + " " + oim.outCompanyName.ToSafeString()).Trim(),
                    DISTRIBUTION_CHANNEL: oim.outDistChn.ToSafeString(),
                    CHANNEL_SALES_GROUP: oim.outChnSales.ToSafeString(),
                    SHOP_TYPE: oim.outShopType.ToSafeString(),
                    SHOP_SEGMENT: oim.outOperatorClass.ToSafeString(),
                    ASC_NAME: oim.outASCTitleThai.ToSafeString() + oim.outASCPartnerName.ToSafeString(),
                    ASC_MEMBER_CATEGORY: oim.outMemberCategory.ToSafeString(),
                    ASC_POSITION: oim.outPosition.ToSafeString(),
                    LOCATION_REGION: oim.outLocationRegion.ToSafeString(),
                    LOCATION_SUB_REGION: oim.outLocationSubRegion.ToSafeString(),
                    EMPLOYEE_NAME: (oim.THFirstName.ToSafeString() + " " + oim.THLastName.ToSafeString()).Trim(),

                    CUSTOMERPURGE: "",
                    EXCEPTENTRYFEE: "",
                    SECONDINSTALLATION: "",
                    AMENDMENT_FLAG: crpm.ServiceLevel_Flag.ToSafeString(),
                    SERVICE_LEVEL: crpm.ServiceLevel.ToSafeString(),

                    FIRST_INSTALL_DATE: crpm.FBSSTimeSlot.FirstInstallDate.ToSafeString(),
                    FIRST_TIME_SLOT: crpm.FBSSTimeSlot.FirstTimeSlot.ToSafeString(),

                    LINE_TEMP_ID: crpm.LINE_TEMP_ID.ToSafeString(),
                    FMC_SPECIAL_FLAG: cpm.SAVEORDER_FMC_SPECIAL_FLAG.ToSafeString(),
                    NON_RES_FLAG: crpm.Non_Res_Flag.ToSafeString(),
                    CRITERIA_MOBILE: P_AIS_MOBILE.ToSafeString(),

                    REMARK_FOR_SUBCONTRACT: crpm.Remark_For_Subcontract.ToSafeString(),
                    MESH_COUNT: crpm.mesh_count,
                    ONLINE_FLAG: crpm.Online_Flag.ToSafeString(),
                    PRIVILEGE_POINT: "",
                    PRIVILEGE_STAFF: crpm.StaffPrivilegeBypass_TransactionID.ToSafeString(),
                    SPECIAL_SKILL: "",
                    TDM_CONTRACT_ID: crpm.TDMContractId.ToSafeString(),
                    TDM_RULE_ID: crpm.TDMRuleId.ToSafeString(),
                    TDM_PENALTY_ID: crpm.TDMPenaltyId.ToSafeString(),
                    TDM_PENALTY_GROUP_ID: crpm.TDMPenaltyGroupId.ToSafeString(),
                    DURATION: crpm.Duration.ToSafeString(),
                    CONTRACT_FLAG: crpm.ContractFlag.ToSafeString(),

                    NON_MOBILE_NO: NON_MOBILE_NO,
                    REGIS_PAYMENT_ID: "",
                    REGIS_PAYMENTDATE: "",
                    REGIS_PAYMENTMETHOD: "",
                    //////No CheckFraud
                    //CEN_FRAUD_FLAG: "",
                    //VERIFY_REASON_CEN_FRAUD: "",
                    //FRAUD_SCORE: "",
                    //R23.05 CheckFraud
                    CEN_FRAUD_FLAG: crpm.CHECK_FRAUD_INFO.CEN_FRAUD_FLAG.ToSafeString(),
                    VERIFY_REASON_CEN_FRAUD: crpm.CHECK_FRAUD_INFO.VERIFY_REASON.ToSafeString(),
                    FRAUD_SCORE: crpm.CHECK_FRAUD_INFO.FRAUD_SCORE.ToSafeString(),
                    //end R23.05 CheckFraud
                    //R23.07
                    DELIVERY_METHOD: crpm.DELIVERY_METHOD,
                    DIY_FLAG: crpm.DIY_FLAG,

                    AIR_REGIST_PACKAGE_ARRAY: airregists,
                    AIR_REGIST_FILE_ARRAY: airImage,
                    AIR_REGIST_SPLITTER_ARRAY: airSplitter,
                    AIR_REGIST_CPE_SERIAL_ARRAY: airCPE,
                    AIR_REGIST_CUST_INSIGHT_ARRAY: custInsightRecord.ToArray(),
                    AIR_REGIST_DCONTRACT_ARRAY: dcontractRecord.ToArray(),
                    AIR_REGIST_CROSS_NETWORK_ARRAY: airCrossnetwork,
                    // NO CheckFraud
                    //AIR_FRAUD_REASON_ARRAY: fraudReason.ToArray()
                    ////R23.05 CheckFraud
                    AIR_FRAUD_REASON_ARRAY: fraudReason.ToArray()
                    ////end R23.05 CheckFraud
                );

                    response.RETURN_CODE = data.RETURN_CODE;
                    response.RETURN_MESSAGE = data.RETURN_MESSAGE;
                    response.RETURN_IA_NO = data.RETURN_SALE_ORDER;
                    response.RETURN_ORDER_NO = data.RETURN_ORDER_NO;

                    _loggers.Info(string.Format("step 7  RETURN_CODE = {0}", data.RETURN_CODE));
                }
                catch (Exception ex)
                {
                    response.RETURN_CODE = -1;
                    response.RETURN_MESSAGE = "Exception msg : " + ex.Message;
                    _loggers.Info(string.Format("step 8  ErrorMessage = {0}", ex.Message));
                }
            }

            #endregion
        }

        private QuickWinPanelModel ForTest()
        {
            var q = new QuickWinPanelModel();
            var cpm = q.CoveragePanelModel;
            var dpm = q.DisplayPackagePanelModel;
            var crpm = q.CustomerRegisterPanelModel;
            var spm = q.SummaryPanelModel;

            crpm.CateType = "R";
            crpm.SubCateType = "R";
            crpm.L_TITLE = "คุณ";
            crpm.L_FIRST_NAME = "จอน";
            crpm.L_LAST_NAME = "นี่";
            crpm.L_CONTACT_PERSON = "คุณ จอน นี่";

            crpm.L_CARD_TYPE = "ID_CARD";
            crpm.L_CARD_NO = "1100600103254";
            crpm.L_GENDER = "M";
            crpm.L_BIRTHDAY = "01/01/1987";
            crpm.L_MOBILE = "0857320657";
            crpm.L_OR = "0857320657";

            crpm.L_HOME_PHONE = "-";
            crpm.L_EMAIL = "game@mail.com";
            crpm.L_SPECIFIC_TIME = "08:00-19:00";
            crpm.L_NATIONALITY = "ไทย";
            crpm.L_REMARK = "จอนนี่";
            crpm.AddressPanelModelSetup.L_HOME_NUMBER_1 = "11";
            crpm.AddressPanelModelSetup.L_HOME_NUMBER_2 = "12";
            crpm.AddressPanelModelSetup.L_MOO = "2";
            crpm.AddressPanelModelSetup.L_BUILD_NAME = "เกมทาวเวอร์";
            crpm.AddressPanelModelSetup.L_FLOOR = "23";
            crpm.AddressPanelModelSetup.L_ROOM = "104";
            crpm.AddressPanelModelSetup.L_MOOBAN = "";
            crpm.AddressPanelModelSetup.L_SOI = "";
            crpm.AddressPanelModelSetup.L_ROAD = "";
            crpm.AddressPanelModelSetup.ZIPCODE_ID = "6A3C56EE181B61C0E0440000BEA816B7";

            cpm.L_LAT = "10.12215";
            cpm.L_LONG = "100.32326";
            crpm.L_ASC_CODE = "";
            crpm.L_STAFF_ID = "38867";
            crpm.L_LOC_CODE = "";
            crpm.L_SALE_REP = "";
            crpm.L_FOR_CS_TEAM = "";
            cpm.L_RESULT = "Y";
            cpm.L_FLOOR_CONDO = "4-8 ชั้น";
            crpm.L_TOP_TERRACE = ""; ;
            crpm.L_TERRACE = ""; ;
            crpm.L_NORTH = "เหนือ";
            crpm.L_SOUTH = ""; ;
            crpm.L_EAST = ""; ;
            crpm.L_WEST = ""; ;

            crpm.L_BUILDING = "อาคารสูง";
            crpm.L_TREE = "";
            crpm.L_BILLBOARD = "";
            crpm.L_EXPRESSWAY = "";
            cpm.Address.L_BUILD_NAME = "เอทาวน์";
            cpm.L_BUILD_NAME = "เกมทาวเวอร์";

            crpm.L_TYPE_ADDR = "";
            cpm.L_FLOOR_CONDO = "23";

            cpm.CVRID = "3";
            cpm.CVR_NODE = "คอนโดลุมพินี สุขุมวิท 77";
            cpm.CVR_TOWER = "CONDOMINIUM";

            return q;
        }
    }

    public class GetSaveOrderRespJobQueryHandler : IQueryHandler<GetSaveOrderRespJobQuery, SaveOrderResp>
    {
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetSaveOrderRespJobQueryHandler(IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> fbb_CFG_LOV)
        {
            _intfLog = intfLog;
            _uow = uow;
            _FBB_CFG_LOV = fbb_CFG_LOV;
        }

        public SaveOrderResp Handle(GetSaveOrderRespJobQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MOBILE_NO + query.ClientIP, "saveOrderNewJob", "GetSaveOrderRespJobQuery", query.ID_CARD_NO, "FBB|" + query.FullUrl, "WEB");
            var response = new SaveOrderResp();

            try
            {
                List<airRegistPackageRecord> airRegists = new List<airRegistPackageRecord>();

                if (query.REGIST_PACKAGE_LIST != null && query.REGIST_PACKAGE_LIST.Count > 0)
                {
                    airRegists = query.REGIST_PACKAGE_LIST.Select(o => new airRegistPackageRecord()
                    {
                        faxFlag = o.faxFlag.ToSafeString(),
                        tempIa = o.tempIa.ToSafeString(),
                        homeIp = o.homeIp.ToSafeString(),
                        homePort = o.homePort.ToSafeString(),
                        iddFlag = o.iddFlag.ToSafeString(),
                        packageCode = o.packageCode.ToSafeString(),
                        packageType = o.packageType.ToSafeString(),
                        productSubtype = o.productSubtype.ToSafeString(),
                        mobileForward = o.mobileForward.ToSafeString(),
                        packagePrice = o.packagePrice,
                        pboxExt = o.pboxExt,
                        packagePriceSpecified = o.packagePriceSpecified
                    }).ToList();
                }

                List<airRegistFileRecord> airImage = new List<airRegistFileRecord>();

                if (query.REGIST_FILE_LIST != null && query.REGIST_FILE_LIST.Count > 0)
                {
                    airImage = query.REGIST_FILE_LIST.Select(o => new airRegistFileRecord()
                    {
                        fileName = o.fileName

                    }).ToList();
                }

                List<airRegistSplitterRecord> airSplitter = new List<airRegistSplitterRecord>();

                if (query.REGIST_SPLITTER_LIST != null && query.REGIST_SPLITTER_LIST.Count > 0)
                {
                    airSplitter = query.REGIST_SPLITTER_LIST.Select(o => new airRegistSplitterRecord
                    {
                        splitterName = o.splitterName.ToSafeString(),
                        distance = o.distance,
                        distanceType = o.distanceType.ToSafeString(),
                        resourceType = o.resourceType.ToSafeString(),
                        distanceSpecified = o.distanceSpecified
                    }).ToList();
                }

                List<airRegistCpeSerialRecord> airCPE = new List<airRegistCpeSerialRecord>();

                if (query.REGIST_CPE_SERIAL_LIST != null && query.REGIST_CPE_SERIAL_LIST.Count > 0)
                {
                    airCPE = query.REGIST_CPE_SERIAL_LIST.Select(o => new airRegistCpeSerialRecord()
                    {
                        serialNo = o.serialNo.ToSafeString(),
                        cpeType = o.cpeType.ToSafeString(),
                        macAddress = o.macAddress.ToSafeString(),

                        //20.4
                        statusDesc = o.status_desc.ToSafeString(),
                        modelName = o.model_name.ToSafeString(),
                        companyCode = o.company_code.ToSafeString(),
                        cpePlant = o.cpeType.ToSafeString(),
                        storageLocation = o.storage_location.ToSafeString(),
                        materialCode = o.material_code.ToSafeString(),
                        registerDate = o.register_date.ToSafeString(),
                        fibrenetId = o.fibrenet_id.ToSafeString(),
                        snPattern = o.sn_pattern.ToSafeString(),
                        shipTo = o.ship_to.ToSafeString(),
                        warrantyStartDate = o.warranty_start_date.ToSafeString(),
                        warrantyEndDate = o.warranty_end_date.ToSafeString()

                    }).ToList();
                }

                List<airRegistCustInsightRecord> custInsightRecord = new List<airRegistCustInsightRecord>();
                List<airRegistDcontractRecord> dcontractRecord = new List<airRegistDcontractRecord>();
                List<airRegistCrossnetworkRecord> crossnetworkRecord = new List<airRegistCrossnetworkRecord>();
                //R23.05 CheckFraud
                List<airFraudReasonRecord> fraudReason = new List<airFraudReasonRecord>();
                //end R23.05 CheckFraud
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNV2WebService.AIRInterfaceWorkflowServiceService())
                {
                    service.Timeout = 600000;

                    string tmpUrl = (from r in _FBB_CFG_LOV.Get()
                                     where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                                     select r.LOV_VAL1).FirstOrDefault().ToSafeString();
                    if (tmpUrl != "")
                    {
                        service.Url = tmpUrl;
                    }

                    var data = service.saveOrderNew(

                        CUSTOMER_TYPE: query.CUSTOMER_TYPE.ToSafeString(),
                        CUSTOMER_SUBTYPE: query.CUSTOMER_SUBTYPE.ToSafeString(),
                        TITLE_CODE: query.TITLE_CODE.ToSafeString(),
                        FIRST_NAME: query.FIRST_NAME.ToSafeString().Trim(),
                        LAST_NAME: query.LAST_NAME.ToSafeString().Trim(),
                        CONTACT_TITLE_CODE: query.CONTACT_TITLE_CODE.ToSafeString(),
                        CONTACT_FIRST_NAME: query.CONTACT_FIRST_NAME.ToSafeString().Trim(),
                        CONTACT_LAST_NAME: query.CONTACT_LAST_NAME.ToSafeString().Trim(),
                        ID_CARD_TYPE_DESC: query.ID_CARD_TYPE_DESC.ToSafeString(),
                        ID_CARD_NO: query.ID_CARD_NO.ToSafeString(),
                        TAX_ID: query.TAX_ID.ToSafeString(),
                        GENDER: query.GENDER.ToSafeString(),
                        BIRTH_DATE: query.BIRTH_DATE.ToSafeString(),
                        MOBILE_NO: query.MOBILE_NO.ToSafeString(),
                        MOBILE_NO_2: query.MOBILE_NO_2.ToSafeString(),
                        HOME_PHONE_NO: query.HOME_PHONE_NO.ToSafeString(),
                        EMAIL_ADDRESS: query.EMAIL_ADDRESS.ToSafeString(),
                        CONTACT_TIME: query.CONTACT_TIME.ToSafeString(),
                        NATIONALITY_DESC: query.NATIONALITY_DESC.ToSafeString(),
                        CUSTOMER_REMARK: query.CUSTOMER_REMARK.ToSafeString(),

                        HOUSE_NO: query.HOUSE_NO.ToSafeString(),
                        MOO_NO: query.MOO_NO.ToSafeString(),
                        BUILDING: query.BUILDING.ToSafeString(),
                        FLOOR: query.FLOOR.ToSafeString(),
                        ROOM: query.ROOM.ToSafeString(),
                        MOOBAN: query.MOOBAN.ToSafeString(),
                        SOI: query.SOI.ToSafeString(),
                        ROAD: query.ROAD.ToSafeString(),
                        ZIPCODE_ROWID: query.ZIPCODE_ROWID.ToSafeString(),

                        LATITUDE: query.LATITUDE.ToSafeString(),
                        LONGTITUDE: query.LONGTITUDE.ToSafeString(),
                        ASC_CODE: query.ASC_CODE.ToSafeString(),
                        EMPLOYEE_ID: query.EMPLOYEE_ID.ToSafeString(),
                        LOCATION_CODE: query.LOCATION_CODE.ToSafeString(),
                        SALE_REPRESENT: query.SALE_REPRESENT.ToSafeString(),
                        CS_NOTE: query.CS_NOTE.ToSafeString(),
                        WIFI_ACCESS_POINT: query.WIFI_ACCESS_POINT.ToSafeString(),
                        INSTALL_STATUS: query.INSTALL_STATUS.ToSafeString(),
                        COVERAGE: query.COVERAGE.ToSafeString(),
                        EXISTING_AIRNET_NO: query.EXISTING_AIRNET_NO.ToSafeString(),
                        GSM_MOBILE_NO: query.GSM_MOBILE_NO.ToSafeString(),
                        CONTACT_NAME_1: query.CONTACT_NAME_1.ToSafeString(),
                        CONTACT_NAME_2: query.CONTACT_NAME_2.ToSafeString(),
                        CONTACT_MOBILE_NO_1: query.CONTACT_MOBILE_NO_1.ToSafeString(),
                        CONTACT_MOBILE_NO_2: query.CONTACT_MOBILE_NO_2.ToSafeString(),
                        CONDO_FLOOR: query.CONDO_FLOOR.ToSafeString(),
                        CONDO_ROOF_TOP: query.CONDO_ROOF_TOP.ToSafeString(),
                        CONDO_BALCONY: query.CONDO_BALCONY.ToSafeString(),
                        BALCONY_NORTH: query.BALCONY_NORTH.ToSafeString(),
                        BALCONY_SOUTH: query.BALCONY_SOUTH.ToSafeString(),
                        BALCONY_EAST: query.BALCONY_EAST.ToSafeString(),
                        BALCONY_WAST: query.BALCONY_WAST.ToSafeString(),
                        HIGH_BUILDING: query.HIGH_BUILDING.ToSafeString(),
                        HIGH_TREE: query.HIGH_TREE.ToSafeString(),
                        BILLBOARD: query.BILLBOARD.ToSafeString(),
                        EXPRESSWAY: query.EXPRESSWAY.ToSafeString(),
                        ADDRESS_TYPE_WIRE: query.ADDRESS_TYPE_WIRE.ToSafeString(),
                        ADDRESS_TYPE: query.ADDRESS_TYPE.ToSafeString(),
                        FLOOR_NO: query.FLOOR_NO.ToSafeString(),

                        HOUSE_NO_BL: query.HOUSE_NO_BL.ToSafeString(),
                        MOO_NO_BL: query.MOO_NO_BL.ToSafeString(),
                        MOOBAN_BL: query.MOOBAN_BL.ToSafeString(),
                        BUILDING_BL: query.BUILDING_BL.ToSafeString(),
                        FLOOR_BL: query.FLOOR_BL.ToSafeString(),
                        ROOM_BL: query.ROOM_BL.ToSafeString(),
                        SOI_BL: query.SOI_BL.ToSafeString(),
                        ROAD_BL: query.ROAD_BL.ToSafeString(),
                        ZIPCODE_ROWID_BL: query.ZIPCODE_ROWID_BL.ToSafeString(),

                        HOUSE_NO_VT: query.HOUSE_NO_VT.ToSafeString(),
                        MOO_NO_VT: query.MOO_NO_VT.ToSafeString(),
                        MOOBAN_VT: query.MOOBAN_VT.ToSafeString(),
                        BUILDING_VT: query.BUILDING_VT.ToSafeString(),
                        FLOOR_VT: query.FLOOR_VT.ToSafeString(),
                        ROOM_VT: query.ROOM_VT.ToSafeString(),
                        SOI_VT: query.SOI_VT.ToSafeString(),
                        ROAD_VT: query.ROAD_VT.ToSafeString(),
                        ZIPCODE_ROWID_VT: query.ZIPCODE_ROWID_VT.ToSafeString(),

                        CVR_ID: query.CVR_ID.ToSafeString(),
                        CVR_NODE: query.CVR_NODE.ToSafeString(),
                        CVR_TOWER: query.CVR_TOWER.ToSafeString(),

                        SITE_CODE: query.SITE_CODE.ToSafeString(),

                        RELATE_MOBILE: query.RELATE_MOBILE.ToSafeString(),
                        RELATE_NON_MOBILE: query.RELATE_NON_MOBILE.ToSafeString(),
                        SFF_CA_NO: query.SFF_CA_NO.ToSafeString(),
                        SFF_SA_NO: query.SFF_SA_NO.ToSafeString(),
                        SFF_BA_NO: query.SFF_BA_NO.ToSafeString(),
                        NETWORK_TYPE: query.NETWORK_TYPE.ToSafeString(),
                        SERVICE_DAY: query.SERVICE_DAY.ToSafeString(),
                        EXPECT_INSTALL_DATE: query.EXPECT_INSTALL_DATE.ToSafeString(),
                        FTTX_VENDOR: query.FTTX_VENDOR.ToSafeString(),
                        INSTALL_NOTE: query.INSTALL_NOTE.ToSafeString(),

                        PHONE_FLAG: query.PHONE_FLAG.ToSafeString(),
                        TIME_SLOT: query.TIME_SLOT.ToSafeString(),
                        INSTALLATION_CAPACITY: query.INSTALLATION_CAPACITY.ToSafeString(),
                        ADDRESS_ID: query.ADDRESS_ID.ToSafeString(),
                        ACCESS_MODE: query.ACCESS_MODE.ToSafeString(),

                        ENG_FLAG: query.ENG_FLAG.ToSafeString(),
                        EVENT_CODE: query.EVENT_CODE.ToSafeString(),
                        INSTALLADDRESS1: query.INSTALLADDRESS1.ToSafeString(),
                        INSTALLADDRESS2: query.INSTALLADDRESS2.ToSafeString(),
                        INSTALLADDRESS3: query.INSTALLADDRESS3.ToSafeString(),
                        INSTALLADDRESS4: query.INSTALLADDRESS4.ToSafeString(),
                        INSTALLADDRESS5: query.INSTALLADDRESS5.ToSafeString(),
                        PBOX_COUNT: query.PBOX_COUNT.ToSafeString(),
                        CONVERGENCE_FLAG: query.CONVERGENCE_FLAG.ToSafeString(),
                        TIME_SLOT_ID: query.TIME_SLOT_ID.ToSafeString(),

                        GIFT_VOUCHER: query.GIFT_VOUCHER.ToSafeString(),
                        SUB_LOCATION_ID: query.SUB_LOCATION_ID.ToSafeString(),
                        SUB_CONTRACT_NAME: query.SUB_CONTRACT_NAME.ToSafeString(),
                        INSTALL_STAFF_ID: query.INSTALL_STAFF_ID.ToSafeString(),
                        INSTALL_STAFF_NAME: query.INSTALL_STAFF_NAME.ToSafeString(),

                        FLOW_FLAG: query.FLOW_FLAG.ToSafeString(),

                        LINE_ID: query.LINE_ID.ToSafeString(),
                        RELATE_PROJECT_NAME: query.RELATE_PROJECT_NAME.ToSafeString(),
                        PLUG_AND_PLAY_FLAG: query.PLUG_AND_PLAY_FLAG.ToSafeString(),

                        RESERVED_ID: query.RESERVED_ID.ToSafeString(),

                        JOB_ORDER_TYPE: query.JOB_ORDER_TYPE.ToSafeString(),
                        ASSIGN_RULE: query.ASSIGN_RULE.ToSafeString(),
                        OLD_ISP: query.OLD_ISP.ToSafeString(),

                        SPLITTER_FLAG: query.SPLITTER_FLAG.ToSafeString(),
                        RESERVED_PORT_ID: query.RESERVED_PORT_ID.ToSafeString(),
                        SPECIAL_REMARK: query.SPECIAL_REMARK.ToSafeString(),
                        ORDER_NO: query.ORDER_NO.ToSafeString(),
                        SOURCE_SYSTEM: query.SOURCE_SYSTEM.ToSafeString(),
                        BILL_MEDIA: query.BILL_MEDIA.ToSafeString(),

                        PRE_ORDER_NO: query.PRE_ORDER_NO.ToSafeString(),
                        VOUCHER_DESC: query.VOUCHER_DESC.ToSafeString(),
                        CAMPAIGN_PROJECT_NAME: query.CAMPAIGN_PROJECT_NAME.ToSafeString(),
                        PRE_ORDER_CHANEL: query.PRE_ORDER_CHANEL.ToSafeString(),

                        RENTAL_FLAG: query.RENTAL_FLAG.ToSafeString(),
                        DEV_PROJECT_CODE: query.DEV_PROJECT_CODE.ToSafeString(),
                        DEV_BILL_TO: query.DEV_BILL_TO.ToSafeString(),

                        DEV_PO_NO: query.DEV_PO_NO.ToSafeString(),

                        PARTNER_TYPE: query.PARTNER_TYPE.ToSafeString(),
                        PARTNER_SUBTYPE: query.PARTNER_SUBTYPE.ToSafeString(),
                        MOBILE_BY_ASC: query.MOBILE_BY_ASC.ToSafeString(),
                        LOCATION_NAME: query.LOCATION_NAME.ToSafeString(),
                        PAYMENTMETHOD: query.PAYMENTMETHOD.ToSafeString(),
                        TRANSACTIONID_IN: query.TRANSACTIONID_IN.ToSafeString(),
                        TRANSACTIONID: query.TRANSACTIONID.ToSafeString(),

                        SUB_ACCESS_MODE: query.SUB_ACCESS_MODE.ToSafeString(),
                        REQUEST_SUB_FLAG: query.REQUEST_SUB_FLAG.ToSafeString(),
                        PREMIUM_FLAG: query.PREMIUM_FLAG.ToSafeString(),
                        RELATE_MOBILE_SEGMENT: query.RELATE_MOBILE_SEGMENT.ToSafeString(),
                        REF_UR_NO: query.REF_UR_NO.ToSafeString(),
                        LOCATION_EMAIL_BY_REGION: query.LOCATION_EMAIL_BY_REGION.ToSafeString(),
                        SALE_STAFF_NAME: query.SALE_STAFF_NAME.ToSafeString(),
                        DOPA_FLAG: "",
                        SERVICE_YEAR: query.SERVICE_YEAR.ToSafeString(),
                        REQUIRE_CS_VERIFY_DOC: query.REQUIRE_CS_VERIFY_DOC.ToSafeString(),
                        FACERECOG_FLAG: query.FACERECOG_FLAG.ToSafeString(),
                        SPECIAL_ACCOUNT_NAME: "",
                        SPECIAL_ACCOUNT_NO: "",
                        SPECIAL_ACCOUNT_ENDDATE: "",
                        SPECIAL_ACCOUNT_GROUP_EMAIL: "",
                        SPECIAL_ACCOUNT_FLAG: query.SPECIAL_ACCOUNT_FLAG.ToSafeString(),
                        EXISTING_MOBILE_FLAG: "",
                        PRE_SURVEY_DATE: "",
                        PRE_SURVEY_TIMESLOT: "",
                        REGISTER_CHANNEL: "FBBWF",
                        AUTO_CREATE_PROSPECT_FLAG: "Y",
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

                        ORDER_RELATE_CHANGE_PRO: query.ORDER_RELATE_CHANGE_PRO.ToSafeString(),
                        COMPANY_NAME: query.COMPANY_NAME.ToSafeString(),
                        DISTRIBUTION_CHANNEL: query.DISTRIBUTION_CHANNEL.ToSafeString(),
                        CHANNEL_SALES_GROUP: query.CHANNEL_SALES_GROUP.ToSafeString(),
                        SHOP_TYPE: query.SHOP_TYPE.ToSafeString(),
                        SHOP_SEGMENT: query.SHOP_SEGMENT.ToSafeString(),
                        ASC_NAME: query.ASC_NAME.ToSafeString(),
                        ASC_MEMBER_CATEGORY: query.ASC_MEMBER_CATEGORY.ToSafeString(),
                        ASC_POSITION: query.ASC_POSITION.ToSafeString(),
                        LOCATION_REGION: query.LOCATION_REGION.ToSafeString(),
                        LOCATION_SUB_REGION: query.LOCATION_SUB_REGION.ToSafeString(),
                        EMPLOYEE_NAME: query.EMPLOYEE_NAME.ToSafeString(),
                        CUSTOMERPURGE: query.CUSTOMERPURGE.ToSafeString(),
                        EXCEPTENTRYFEE: query.EXCEPTENTRYFEE.ToSafeString(),
                        SECONDINSTALLATION: query.SECONDINSTALLATION.ToSafeString(),
                        AMENDMENT_FLAG: query.AMENDMENT_FLAG.ToSafeString(),
                        SERVICE_LEVEL: query.SERVICE_LEVEL.ToSafeString(),

                        FIRST_INSTALL_DATE: query.FIRST_INSTALL_DATE.ToSafeString(),
                        FIRST_TIME_SLOT: query.FIRST_TIME_SLOT.ToSafeString(),
                        LINE_TEMP_ID: query.LINE_TEMP_ID.ToSafeString(),
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

                        NON_MOBILE_NO: query.NON_MOBILE_NO.ToSafeString(),
                        REGIS_PAYMENT_ID: query.REGIS_PAYMENT_ID.ToSafeString(),
                        REGIS_PAYMENTDATE: query.REGIS_PAYMENTDATE.ToSafeString(),
                        REGIS_PAYMENTMETHOD: query.REGIS_PAYMENTMETHOD.ToSafeString(),
                        //R23.05 CheckFraud
                        CEN_FRAUD_FLAG: "",
                        VERIFY_REASON_CEN_FRAUD: "",
                        FRAUD_SCORE: "",
                        //end R23.05 CheckFraud
                        //R23.07
                        DELIVERY_METHOD: "",
                        DIY_FLAG: "",

                        AIR_REGIST_PACKAGE_ARRAY: airRegists.ToArray(),
                        AIR_REGIST_FILE_ARRAY: airImage.ToArray(),
                        AIR_REGIST_SPLITTER_ARRAY: airSplitter.ToArray(),
                        AIR_REGIST_CPE_SERIAL_ARRAY: airCPE.ToArray(),
                        AIR_REGIST_CUST_INSIGHT_ARRAY: custInsightRecord.ToArray(),
                        AIR_REGIST_DCONTRACT_ARRAY: dcontractRecord.ToArray(),
                        AIR_REGIST_CROSS_NETWORK_ARRAY: crossnetworkRecord.ToArray(),
                        //R23.05 CheckFraud
                        AIR_FRAUD_REASON_ARRAY: fraudReason.ToArray()
                    //end R23.05 CheckFraud
                    );

                    response.RETURN_CODE = data.RETURN_CODE;
                    response.RETURN_MESSAGE = data.RETURN_MESSAGE;
                    response.RETURN_IA_NO = data.RETURN_SALE_ORDER;
                    response.RETURN_ORDER_NO = data.RETURN_ORDER_NO;
                }
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                response.RETURN_CODE = -1;
                response.RETURN_MESSAGE = "Error Before Call Airnet Service " + ex.GetErrorMessage();
                response.RETURN_IA_NO = "";
                response.RETURN_ORDER_NO = "";
            }

            return response;
        }
    }
}
