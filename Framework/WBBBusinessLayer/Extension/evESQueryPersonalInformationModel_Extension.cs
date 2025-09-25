using System.Collections.Generic;
using System.Linq;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public static class evESQueryPersonalInformationModel_Extension
    {
        //mobilePackageCurrent(Main) to evESQueryPersonalInformation, Option2
        public static evESQueryPersonalInformationModel ToEvESQueryPersonalInformationModelOption2(this CustomerProfileMobilePackageCurrentMain main,
            GetCustomerProfileSubScriptionProfileItem subScriptionProfile)
        {
            _ = decimal.TryParse(main.priceIncludeVat, out var _monthlyFee);
            _ = decimal.TryParse(main.priceExcludeVat, out var _priceExcludeVat);
            return new evESQueryPersonalInformationModel
            {
                billCycle = main.nextBillDate,
                ////bosId = "",
                ////bvDescription = "",
                ////bvPoint = "",
                crmFlg = main.crmFlag,
                descEng = main.descriptionEN,
                descThai = main.descriptionTH,
                ////deviceContractFlg = main.deviceContractFlg,
                ////dlRate = main.downLoadSpeed,
                endDt = main.productExpireTime,
                inStatementEng = main.billItemDescriptionEN,
                inStatementThai = main.billItemDescriptionTH,
                ////integrationName = main.IntegrationNameTH,
                ////maxCounterMain = subScriptionProfile.maxCounterMain,
                ////maxCounterOntop = subScriptionProfile.maxCounterOntop,
                monthlyFee = _monthlyFee,
                ////netFlexiFlg = main.netFlexiFlg,
                nextBillCycle = main.nextBillDate,
                paymentMode = main.paymentMode,
                ////phxProductId = main.productOfferPriceId,
                priceExclVat = _priceExcludeVat,
                ////priceInclVat = main.priceIncludeVat,
                priceType = main.priceType,
                ////pro5gflg = main.pro5gflg,
                ////productAcctnCat = main.productAcctnCat,
                productCd = main.productCd,
                productClass = main.productClass,
                productPkg = main.productPackage,
                productSeq = main.gvProductSequenceId,
                produuctGroup = main.offeringGroup,
                promotionName = main.name,
                ////prorateFlg = main.prorateFlag,
                ////remainCounterMain = subScriptionProfile.maxCounterMain - subScriptionProfile.counterMain,
                ////remainCounterOntop = subScriptionProfile.maxCounterOntop - subScriptionProfile.counterOntop,
                ////remark  = "",
                shortNameEng = main.shortenedNameEN,
                shortNameThai = main.shortenedNameTH,
                startDt = main.productEffectiveTime,
                ////ulRate = main.upLoadSpeed,
                ////url = ""
            };
        }

        //mobilePackageCurrent(OnTop) to evESQueryPersonalInformation, Option2
        public static List<evESQueryPersonalInformationModel> ToEvESQueryPersonalInformationModelOption2(this List<CustomerProfileMobilePackageCurrentOntop> listOntop,
            GetCustomerProfileSubScriptionProfileItem subScriptionProfile)
        {
            var results = listOntop.Select(x =>
            {
                _ = decimal.TryParse(x.priceIncludeVat, out var _monthlyFee);
                _ = decimal.TryParse(x.priceExcludeVat, out var _priceExcludeVat);
                return new evESQueryPersonalInformationModel
                {
                    billCycle = x.nextBillDate,
                    ////bosId = "",
                    ////bvDescription = "",
                    ////bvPoint = "",
                    crmFlg = x.crmFlag,
                    descEng = x.descriptionEN,
                    descThai = x.descriptionTH,
                    ////deviceContractFlg = x.deviceContractFlg,
                    ////dlRate = x.downLoadSpeed,
                    endDt = x.productExpireTime,
                    inStatementEng = x.billItemDescriptionEN,
                    inStatementThai = x.billItemDescriptionTH,
                    ////integrationName = x.IntegrationNameTH,
                    ////maxCounterMain = subScriptionProfile.maxCounterMain,
                    ////maxCounterOntop = subScriptionProfile.maxCounterOntop,
                    monthlyFee = _monthlyFee,
                    ////netFlexiFlg = x.netFlexiFlg,
                    nextBillCycle = x.nextBillDate,
                    paymentMode = x.paymentMode,
                    ////phxProductId = x.productOfferPriceId,
                    priceExclVat = _priceExcludeVat,
                    ////priceInclVat = x.priceIncludeVat,
                    priceType = x.priceType,
                    ////pro5gflg = x.pro5gflg,
                    ////productAcctnCat = x.productAcctnCat,
                    productCd = x.productCd,
                    productClass = x.productClass,
                    productPkg = x.productPackage,
                    productSeq = x.gvProductSequenceId,
                    produuctGroup = x.offeringGroup,
                    promotionName = x.name,
                    ////prorateFlg = x.prorateFlag,
                    ////remainCounterMain = subScriptionProfile.maxCounterMain - subScriptionProfile.counterMain,
                    ////remainCounterOntop = subScriptionProfile.maxCounterOntop - subScriptionProfile.counterOntop,
                    ////remark  = "",
                    shortNameEng = x.shortenedNameEN,
                    shortNameThai = x.shortenedNameTH,
                    startDt = x.productEffectiveTime,
                    ////ulRate = x.upLoadSpeed,
                    ////url = ""
                };
            }).ToList();
            return results ?? new List<evESQueryPersonalInformationModel>();
        }

        //serviceProfile to evESQueryPersonalInformation, Option3
        public static List<evESQueryPersonalInformationModel> ToEvESQueryPersonalInformationModelOption3(this List<GetCustomerProfileServiceProfileItem> serviceProfiles)
        {
            var results = serviceProfiles.Select(x =>
            {
                _ = decimal.TryParse(x.priceIncludeVat, out var _monthlyFee);
                _ = decimal.TryParse(x.priceExcludeVat, out var _priceExcludeVat);
                return new evESQueryPersonalInformationModel
                {
                    descEng = x.descriptionEN,
                    descThai = x.descriptionTH,
                    endDt = x.endDateTime,
                    inStatementEng = x.billItemDescriptionEN,
                    inStatementThai = x.billItemDescriptionTH,
                    paymentMode = x.paymentMode,
                    priceType = x.priceType,
                    productCd = x.productCd,
                    productClass = x.productClass,
                    productPkg = x.productPackage,
                    promotionName = x.serviceName,
                    shortNameEng = x.shortenedNameEN,
                    shortNameThai = x.shortenedNameTH,
                    startDt = x.startDateTime,
                    monthlyFee = _monthlyFee,
                    priceExclVat = _priceExcludeVat
                    ////productGroup = x.offeringGroup,
                    ////integrationName = x.IntegrationNameTH,
                    ////netFlexiFlg = x.netFlexiFlg,
                    ////url = ""
                };
            }).ToList();
            return results ?? new List<evESQueryPersonalInformationModel>();
        }

        //subscriptionAccount to evESQueryPersonalInformation, OnOption4
        public static List<evESQueryPersonalInformationModel> ToEvESQueryPersonalInformationModelOption4(this GetCustomerProfileSubscriptionAccountItem subscriptionAccount)
        {
            var results = subscriptionAccount?.billingAccount.Select(x =>
            {
                return new evESQueryPersonalInformationModel
                {
                    ////paymentMethod = x.paymentMethod,
                    billCycle = x.billDisplay,
                    creditCardNo = x.creditCardNo,
                    creditCardType = x.creditCardType,
                    creditCardBankCd = x.creditCardBankCd,
                    creditCardExpMonth = x.creditCardExpMonth,
                    creditCardExpYear = x.creditCardExpYear,
                    ////bankName = x.bankName,
                    ////bankAccntNumber = x.bankAccntNumber,
                    billMedia = x.billMedia,
                    billLanguage = x.billLanguage,
                    emailBillTo = x.emailBillTo,
                    smsBillTo = x.smsBillTo,
                    creditCardName = x.creditCardName,
                    creditCardRefID = x.creditCardRefID,
                    ////firstRegEStatement = x.firstRegEStatement,
                    ////itemLocalFlg = x.itemLocalFlg,
                    ////itemFax = x.itemFax,
                    ////itemSmsFlg = x.itemSmsFlg,
                    ////itemVasFlg = x.itemVasFlg,
                    ////itemGprsFlg = x.itemGprsFlg,
                    ////itemNrFlg = x.itemNrFlg,
                    ////itemTransFlg = x.itemTransFlg,
                    ////itemWaiveFlg = x.itemWaiveFlg,
                    ////itemStartDt = x.itemStartDt,
                    ////itemEndDt = x.itemEndDt,
                    ////cdrRequestDt = x.cdrRequestDt,
                    ////itemEmail = x.itemEmail,
                    ////GroupBill = x.groupBill,
                    ////MailGroupFlag = x.mailGroupFlag,
                    ////MailGroupAddress = x.mailGroupAddress,
                    ////MailGroupStatementCurrency = x.mailGroupStatementCurrency,
                    ////MailGroupLanguage = x.mailGroupLanguage,
                    ////MailGroupName = x.mailGroupName,
                    ////MailGroupDelivery = x.mailGroupDelivery,
                    ////MailGroupStatementStyle = x.mailGroupStatementStyle,
                    ////smsContactMobileNo = x.smsContactNo,
                    ////wtReqFlg = x.wtReqFlg,
                    ////wtReqDt = x.wtReqDt,
                    ////bankNameCd = x.bankNameCd,
                };
            }).ToList();
            return results ?? new List<evESQueryPersonalInformationModel>();
        }

        //subscriptionAccount+subScriptionProfile to evESQueryPersonalInformation, OnOption5
        public static List<evESQueryPersonalInformationModel> ToEvESQueryPersonalInformationModelOption5(this GetCustomerProfileSubscriptionAccountItem subscriptionAccount,
            GetCustomerProfileSubScriptionProfileItem subScriptionProfile)
        {
            var results = subscriptionAccount?
                        .customerAccount?.Select((x, i) =>
                        {
                            var cbill = subscriptionAccount.billingAccount.ElementAtOrDefault(i) ?? new CustomerProfileBillingAccount();
                            var chold = subscriptionAccount.subscriptionHolder.ElementAtOrDefault(i) ?? new CustomerProfileSubscriptionHolder();
                            var caddr = cbill?.address?.FirstOrDefault() ?? new CustomerProfileCustomerAddress();
                            return new evESQueryPersonalInformationModel
                            {
                                idCardNo = x.idCardNo,
                                buildingName = caddr?.building,
                                houseNo = caddr?.building,
                                room = caddr?.room,
                                tumbol = caddr?.tumbol,
                                moo = caddr?.moo,
                                streetName = caddr?.street,
                                ////mooBan = caddr?.mooban,
                                floor = caddr?.floor,
                                soi = caddr?.soi,
                                amphur = caddr?.amphur,
                                provinceName = caddr?.province,
                                zipCode = caddr?.zipCode,
                                ////engFlg = caddr?.engFlag,
                                ////caNo = subScriptionProfile?.caId,
                                ////baNo = subScriptionProfile?.baId,
                                ////saNo = subScriptionProfile?.saId,
                                ////subCategory = x.accountSubCategory,
                                ////category = x.accountCategory,
                                billAccountName = cbill.billAccountName,
                                title = x.title,
                                firstName = chold?.contactFirstname,
                                lastName = chold?.contactLastname,
                                idCardType = x.idCardType,
                                chargeType = subScriptionProfile?.chargeType,
                            };
                        })
                        .ToList();
            return results ?? new List<evESQueryPersonalInformationModel>();
        }

        ////public static evESQueryPersonalInformationModel ToEvESQueryPersonalInformationModel(this GetCustomerProfileSubScriptionProfileItem ssProfile)
        ////{
        ////    /**
        ////     * update ampping parameter here
        ////     **/
        ////    return new evESQueryPersonalInformationModel
        ////    {
        ////        //promotionName = "",
        ////        //productClass = "",
        ////        //produuctGroup = "",
        ////        //productPkg = "",
        ////        //productCd = "",
        ////        //endDt = "",
        ////        //shortNameThai = "",
        ////        //shortNameEng = "",
        ////        //startDt = "",
        ////        //descThai = "",
        ////        //descEng = "",
        ////        //inStatementThai = "",
        ////        //inStatementEng = "",
        ////        //priceType = "",
        ////        //productSeq = "",
        ////        ////monthlyFee = _monthlyFee,
        ////        //nextBillCycle = "",
        ////        //crmFlg = "",
        ////        //paymentMode = "",
        ////        ////priceExclVat = _priceExcludeVat
        ////    };
        ////}
        ////
        ////
        //////evESQueryPersonalInformation, OnOption5
        ////public static void UpdateSsProfile(this evESQueryPersonalInformationModel result, ref GetCustomerProfileSubScriptionProfileItem ssProfile)
        ////{
        ////    /**
        ////     * update ampping parameter here
        ////     **/
        ////    ////subscription - account    idCardNo
        ////    ////subscription - account    billingAccount.address.building
        ////    ////subscription - account    billingAccount.address.building
        ////    ////subscription - account    billingAccount.address.room
        ////    ////subscription - account    billingAccount.address.tumbol
        ////    ////subscription - account    billingAccount.address.moo
        ////    ////subscription - account    billingAccount.address.street
        ////    ////subscription - account    billingAccount.address.mooban
        ////    ////subscription - account    billingAccount.address.floor
        ////    ////subscription - account    billingAccount.address.soi
        ////    ////subscription - account    billingAccount.address.amphur
        ////    ////subscription - account    billingAccount.address.province
        ////    ////subscription - account    billingAccount.address.zipCode
        ////    ////subscription - account    billingAccount.address.engFlag
        ////    ////subScriptionProfile subScriptionProfile.caId
        ////    ////subScriptionProfile subScriptionProfile.baId
        ////    ////subScriptionProfile subScriptionProfile.saId
        ////    ////subscription - account    customerAccount.accountCategory
        ////    ////subscription - account    customerAccount.accountSubCategory
        ////    ////- -
        ////    ////subscription - account    customerAccount.title
        ////    ////subscription - account    subscriptionHolder.contactFirstname
        ////    ////subscription - account    subscriptionHolder.contactLastname
        ////    ////subscription - account    customerAccount.idCardTypeDesc
        ////    ////subScriptionProfile subScriptionProfile.chargeType
        ////}
    }
}
