using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class evESeServiceQueryMassCommonAccountInfoModel
    {
        public string GUIDKEY { get; set; }

        public string outAssetStatus { get; set; }
        public string outAccountStatus { get; set; }
        public string outPrimaryContactFirstName { get; set; }
        public string outContactLastName { get; set; }
        public string outMobileSegment { get; set; }
        public string outAssetId { get; set; }
        public string outBillingAccountId { get; set; }
        public string outBillingAccountName { get; set; }
        public string outBillingAccountNumber { get; set; }
        public string outAccountId { get; set; }
        public string outAccountName { get; set; }
        public string outAccountNumber { get; set; }
        public string outServiceAccountId { get; set; }
        public string outServiceAccountName { get; set; }
        public string outProductName { get; set; }
        public string outMobileServiceType { get; set; }
        public string outStatusReason { get; set; }
        public string outStatusDate { get; set; }
        public string outRegisteredDate { get; set; }
        public string outCustomerValueSegment { get; set; }
        public string outSegmentEffectiveDate { get; set; }
        public string outSegmentExpiredDate { get; set; }
        public string outSuspendType { get; set; }
        public string outMobileNumberRegion { get; set; }
        public string outFullAddress { get; set; }
        public string outAmphur { get; set; }
        public string outBuildingName { get; set; }
        public string outCountry { get; set; }
        public string outFloor { get; set; }
        public string outHouseNumber { get; set; }
        public string outMoo { get; set; }
        public string outMooban { get; set; }
        public string outPostalCode { get; set; }
        public string outProvince { get; set; }
        public string outRoom { get; set; }
        public string outSoi { get; set; }
        public string outStreetName { get; set; }
        public string outHierarchyBilling { get; set; }
        public string outBillCycle { get; set; }
        public string outBillStyle { get; set; }
        public string outBillLanguage { get; set; }
        public string outBillMedia { get; set; }
        public string outEmail { get; set; }
        public string outSMSBillTo { get; set; }
        public string outFaxBillTo { get; set; }
        public string outMediaTypeCode { get; set; }
        public string outItemisationFlagGPRS { get; set; }
        public string outItemisationFlagLocal { get; set; }
        public string outItemisationFlagNR { get; set; }
        public string outItemisationFlagSMS { get; set; }
        public string outItemisationFlagTransactions { get; set; }
        public string outItemisationFlagVAS { get; set; }
        public string outProject { get; set; }
        public string outPrintBill { get; set; }
        public string outBillName { get; set; }
        public string outAccountCategory { get; set; }
        public string outAccountSubCategory { get; set; }
        public string outBAInvoicingCompany { get; set; }
        public string outSpecialAccount { get; set; }
        public Nullable<decimal> outAccountCreditLimit { get; set; }
        public string outAccountType { get; set; }
        public Nullable<decimal> outAvailableCreditLimit { get; set; }
        public string outEffectiveUntil { get; set; }
        public string outMOIAccountNumber { get; set; }
        public string outMOIVerifiedStatus { get; set; }
        public string outparameter1 { get; set; }
        public string outparameter2 { get; set; }
        public string outparameter3 { get; set; }
        public string outparameter7 { get; set; }
        public string outparameter8 { get; set; }
        public string outErrorMessage { get; set; }
        public string errorMessage { get; set; }
        public string outPreOperatorCd { get; set; }
        public string outMigrateDate { get; set; }
        public string outTumbol { get; set; }
        public string outBirthDate { get; set; }
        public string cardType { get; set; }
        public string outServiceAccountNumber { get; set; }
        public string outServiceYear { get; set; }
        public string outDayOfServiceYear { get; set; }
        public string OwnerProduct { get; set; }
        public string PackageCode { get; set; }
        public string outTitle { get; set; }
        public Nullable<decimal> SffProfileLogID { get; set; }
        public bool IsAWNProduct { get; set; }
        public string outBillingSystem { get; set; }
        public string checkPlayBox { get; set; }

        //15.7  add output parameter

        public string projectName { get; set; }
        public string vatAddress1 { get; set; }
        public string vatAddress2 { get; set; }
        public string vatAddress3 { get; set; }
        public string vatAddress4 { get; set; }
        public string vatAddress5 { get; set; }
        public string vatAddressFull { get; set; }
        public string vatPostalCd { get; set; }
        public string vataddTripleplay { get; set; }


        //16.3
        /// <summary>
        /// return เบอร์ 08 กรณีส่งค่าเข้ามาด้วยเบอร์ 88
        /// </summary>
        public string outMobileNumber { get; set; }

        //16.6
        public string outServiceMobileNo { get; set; }

        //16.9
        public string outAddressId { get; set; }

        //20.3 Service Level
        public string outServiceLevel { get; set; }
        public string outPaGroup { get; set; }

        //R20.6 ChangePromotionCheckRight
        public string outInstanceNameFBBrowType { get; set; }
        public string outInstanceNameFBBnonMobileNumber { get; set; }

        //R20.6 Add by Aware : Atipon
        public string projectOption { get; set; }
        public string curMobileCheckRight { get; set; }
        public string curMobileCheckRightOption { get; set; }
        public string curMobileGetBenefit { get; set; }
        public string curMobileGetBenefitOption { get; set; }

        public string countContractFbb { get; set; }
        public string contractFlagFbb { get; set; }
        public string fbbLimitContract { get; set; }
        public string contractProfileCountFbb { get; set; }

        //R24.06
        public string outIPCamera3bbAcountNumber { get; set; }
        public string outIPCamera3BBAccountUuid { get; set; }
        public string outIPCamera3bbErrorMessage { get; set; }

        public string outAccessType { get; set; }
    }

    public class evOMServiceCheckChangePromotionModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string OrderNo { get; set; }
    }

    public class evOMServiceConfirmChangePromotionModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }

        public string SuccessFlag { get; set; }
    }

    public class SFFInternetProfile
    {
        public string InternetNo { get; set; }
        public string IDCardNo { get; set; }
        public string ServiceCode { get; set; }
    }

    public class evOMServiceIVRCheckBlackListModel
    {
        public string ErrorMessage { get; set; }
        public string returnFlag { get; set; }

    }
    /*Jang*/
    public class evOMQueryListServiceAndPromotionByPackageTypeModel
    {

        public string resultFlag { get; set; }
        public string ErrorMessage { get; set; }

        public string v_installAddress { get; set; }
        public string v_installAddress1 { get; set; }
        public string v_installAddress2 { get; set; }
        public string v_installAddress3 { get; set; }
        public string v_installAddress4 { get; set; }
        public string v_installAddress5 { get; set; }
        public string v_sff_main_promotionCD { get; set; }
        public int v_number_of_pb_number { get; set; }
        public string v_owner_product { get; set; }
        public string L_NUMBER_OF_PLAYBOX { get; set; }
        public string productCDContent { get; set; }
        // V 17.6
        public bool checkHavePlayBox { get; set; }
        // V 17.7
        public bool replacePlayboxCheckProductMainNotUse { get; set; }

        // V 17.9
        public List<promotionModel> ListPromotion { get; set; }

        // V 18.10
        public string v_package_subtype { get; set; }
        public string addressId { get; set; }
        public string contactName { get; set; }
        public string contactMobilePhone { get; set; }
        public string flowFlag { get; set; }

        // V 20.1
        public string access_mode { get; set; }

        // V 20.5
        public List<serviceModel> ListService { get; set; }

        // V 20.7
        public string errorValueNull { get; set; }

        // V 21.5
        public string startDate { get; set; }

        // V 22.4 //R22.04 WTTx
        public string gridId { get; set; }

        //R23.08
        public string userAccount { get; set; }
    }

    public class evESQueryPersonalInformationModel
    {
        /// <summary>
        /// [OPTION:1,6,7]
        /// รหัสบัตรประชาชน
        /// </summary>
        public string idCardNo { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Mobile No
        /// </summary>
        public string mobileNo { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// Customer Account
        /// </summary>
        public string customerAccount { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Row_id ของ CA
        /// </summary>
        public string customerAccountRowId { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// segment
        /// </summary>
        public string mobileSegment { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// Register Date (DD/MM/YYYY HH24:MI:SS)
        /// </summary>
        public string registerDt { get; set; }
        /// <summary>
        /// [OPTION:1,6]
        /// อีเมล์
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// [OPTION:1,2,4]
        /// รอบบิล
        /// </summary>
        public string billCycle { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// Birth Day (DD/MM/YYYY)
        /// </summary>
        public string birthDay { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Contract Home Phone
        /// </summary>
        public string houseTelNo { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Contract Mobile Phone
        /// </summary>
        public string officeTelNo { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// familyAuth
        /// </summary>
        public string familyAuth { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// singleAuth
        /// </summary>
        public string singleAuth { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// CA Account No
        /// </summary>
        public string caAccountNo { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// BA Account No
        /// </summary>
        public string baAccountNo { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// Previous Operator Code Ex. 01,02,04
        /// </summary>
        public string preOperatorCd { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// Migrate Date (DD/MM/YYYY HH24:MI:SS)
        /// </summary>
        public string migrateDt { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// invoicingCompany Ex..AIS , AWN
        /// </summary>
        public string invoicingCompany { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// GPRS Type Ex. Time-Based, Volume-Based
        /// </summary>
        public string gprsType { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// GPRS Start Date (DD/MM/YYYY HH24:MI:SS)
        /// </summary>
        public string gprsStartDate { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Customer Account Title
        /// </summary>
        public string caAccountTitle { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Billing Account Title
        /// </summary>
        public string billAccountTitle { get; set; }
        /// <summary>
        /// [OPTION:1,5]
        /// Billing Account Name
        /// </summary>
        public string billAccountName { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Network Type
        /// </summary>
        public string networkType { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Customer Type
        /// </summary>
        public string customerType { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Yes = BOS / No = Not BOS
        /// </summary>
        public string isBOSSystem { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Charge Type Ex. Post-paid, Pre-paid, Hybrid-Post
        /// </summary>
        public string chargeType { get; set; }
        /// <summary>
        /// [OPTION:1,6,7]
        /// Loan flag (Y/N)
        /// </summary>
        public string loanFlg { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// วันที่ Migrate to BOS dd/MM/yyyy HH:mm:ss
        /// </summary>
        public string migrateToBosDt { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// BA Account No เก่า ก่อน Migrate to BOS
        /// </summary>
        public string baAccountNoRef { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Status Of Mobile Ex. ‘Active’, 'Suspend'
        /// </summary>
        public string mobileStatus { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Status Reason Of Mobile Ex. '1314-Suspend No PI'
        /// </summary>
        public string mobileStatusReason { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Unknown???
        /// </summary>
        public string paGroup { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Hybrid Rule เช่น 1, 2, 3 เป็นต้น
        /// 1:Selective Service Hybrid Rule
        /// 2:Hit Credit Limit
        /// 3:Hit Threshold
        /// </summary>
        public string hybridRule { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Promotion Name
        /// </summary>
        public string promotionName { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Promotion Class
        /// </summary>
        public string productClass { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Promotion Group
        /// </summary>
        public string produuctGroup { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Promotion Package Group
        /// </summary>
        public string productPkg { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Promotion Code
        /// </summary>
        public string productCd { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// Promotion End date (DD/MM/YYYY HH24:MI:SS)
        /// </summary>
        public string endDt { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Promotion Short Name(Thai) 
        /// </summary>
        public string shortNameThai { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Promotion Short Name(Eng)
        /// </summary>
        public string shortNameEng { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Start Date (DD/MM/YYYY HH24:MI:SS)
        /// </summary>
        public string startDt { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// Description Thai
        /// </summary>
        public string descThai { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// Description English
        /// </summary>
        public string descEng { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Word In Statement Thai
        /// </summary>
        public string inStatementThai { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Word In Statement Eng
        /// </summary>
        public string inStatementEng { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Price Type
        /// </summary>
        public string priceType { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// Product Sequence
        /// </summary>
        public string productSeq { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// Monthly Fee
        /// </summary>
        public decimal monthlyFee { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// Next Bill Cycle
        /// </summary>
        public string nextBillCycle { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// BOS Product ID
        /// </summary>
        public string bosId { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// BV Point(BV Promotion)
        /// </summary>
        public string bvPoint { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// BV Description(BV Promotion)
        /// </summary>
        public string bvDescription { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// CRM Flag
        /// </summary>
        public string crmFlg { get; set; }
        /// <summary>
        /// [OPTION:2,3]
        /// Payment Mode
        /// </summary>
        public string paymentMode { get; set; }
        /// <summary>
        /// [OPTION:2]
        /// Price Exclude Vat
        /// </summary>
        public decimal priceExclVat { get; set; }

        /// <summary>
        /// [OPTION:4]
        /// Bill Media
        /// </summary>
        public string billMedia { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Bill Language (THA = Thai, ENU = English)
        /// </summary>
        public string billLanguage { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Email Bill To
        /// </summary>
        public string emailBillTo { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// SMS Bill To
        /// </summary>
        public string smsBillTo { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Credit Card Holder Name
        /// </summary>
        public string creditCardName { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Credit Card number
        /// </summary>
        public string creditCardNo { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Example VISA, Mastercard, etc.
        /// </summary>
        public string creditCardType { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Credit Card Issuing Bank
        /// </summary>
        public string creditCardBankCd { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Expire Month 
        /// </summary>
        public string creditCardExpMonth { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Expire Year 
        /// </summary>
        public string creditCardExpYear { get; set; }
        /// <summary>
        /// [OPTION:4]
        /// Credit Card reference id from PCI
        /// </summary>
        public string creditCardRefID { get; set; }

        /// <summary>
        /// [OPTION:5,6,7]
        /// House Number
        /// </summary>
        public string houseNo { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Moo
        /// </summary>
        public string moo { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Moo ban
        /// </summary>
        public string mooban { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Room
        /// </summary>
        public string room { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Floor
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Building Name
        /// </summary>
        public string buildingName { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Soi
        /// </summary>
        public string soi { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Street Name
        /// </summary>
        public string streetName { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Tumbol
        /// </summary>
        public string tumbol { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Province Name
        /// </summary>
        public string provinceName { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// Amphur
        /// </summary>
        public string amphur { get; set; }
        /// <summary>
        /// [OPTION:5,6,7]
        /// ZipCode
        /// </summary>
        public string zipCode { get; set; }

        /// <summary>
        /// [OPTION:6]
        /// Project Name “Identification” is เป็นข้อมูลที่ลูกค้ามาแสดงตนผ่านช่องทาง identification
        /// </summary>
        public string projectName { get; set; }
        /// <summary>
        /// [OPTION:6,7]
        /// Status Of Mobile
        /// </summary>
        public string statusCd { get; set; }
        /// <summary>
        /// [OPTION:6,7]
        /// Moi status
        /// -Change
        /// -NotVerify
        /// -Others
        /// -Pending
        /// -Pending - Second
        /// -Valid - All
        /// -Valid - Diff Name
        /// </summary>
        public string moiStatus { get; set; }
        /// <summary>
        /// [OPTION:6]
        /// ID Card Type
        /// </summary>
        public string idCardTypeCd { get; set; }
        /// <summary>
        /// [OPTION:6]
        /// title
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// [OPTION:6,7]
        /// หมายเลขโทรศัพท์อื่นๆ
        /// </summary>
        public string mobileNoOther { get; set; }
        /// <summary>
        /// [OPTION:6,7]
        /// เบอร์โทรศัพท์
        /// </summary>
        public string phoneNo { get; set; }
        /// <summary>
        /// [OPTION:6,7]
        /// เบอร์โทรศัพท์ที่ทำงาน
        /// </summary>
        public string officePhoneNo { get; set; }

        /// <summary>
        /// [OPTION:7]
        /// ชื่อ
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// [OPTION:7]
        /// นามสกุล
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// [OPTION:7]
        /// title
        /// </summary>
        //public string Title { get; set; }
        /// <summary>
        /// [OPTION:7]
        /// อีเมล์
        /// </summary>
        //public string Email { get; set; }
        /// <summary>
        /// [OPTION:7]
        /// ช่องทางการแสดงตน 
        /// ตัวอย่าง Change Charge TypeBatch,Change Charge TypeOnline,Change Mobile NumberOnline,Change Owner for SupBatch,
        /// Change Owner for SupOnline,Change OwnerBatch,Change OwnerOnline,MOBILE_APP,USSD,Web
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// [OPTION:7]
        /// Path ที่เก็บรูปตอน Iden
        /// </summary>
        public string urlPicture { get; set; }
        /// <summary>
        /// [OPTION:1]
        /// Customer card type
        /// </summary>
        public string idCardType { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class promotionModel
    {
        public string productClass { get; set; }
        public string productType { get; set; }
        public string productCD { get; set; }

        //17.9 Speed boost
        public string endDate { get; set; }
        public string startDate { get; set; }
        public string productStatus { get; set; }
    }

    public class serviceModel
    {
        public string productType { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string accessType { get; set; }
        public string networkProvider { get; set; }
        public string addressId { get; set; }
        public string gridId { get; set; } //R22.04 WTTx
    }

    public class CheckChangePromotionModelLine4
    {
        public string returnCode { get; set; }
        public string existFlag { get; set; }
        public string productCd { get; set; }
        public string firstActDate { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string countFN { get; set; }
    }

    public class ConfirmChangePromotionModelLine4
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }

        public string SuccessFlag { get; set; }
    }

    public class CheckNewRegisProspectQueryModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string blackListFlag { get; set; }
        public string locationFlag { get; set; }
        public string ascFlag { get; set; }
        public string errorCode { get; set; }
    }

    public class evOMCheckDeviceContractModel
    {
        public string countContractFbb { get; set; }
        public string contractFlagFbb { get; set; }
        public string fbbLimitContract { get; set; }
        public string errorMessage { get; set; }
        public string countContract { get; set; }
        public string contractExpireDt { get; set; }
        public string contractFlag { get; set; }
        public string sameNumber { get; set; }
        public string returnCode { get; set; }
        public string blackListFlag { get; set; }
        public string limitContract { get; set; }
        public string contractProfileCount { get; set; }
        public string idCardNo { get; set; }
        public string remainLimitMobile { get; set; }
        public string contractExpireDtFbb { get; set; }
        public string contractProfileCountFbb { get; set; }
        public string sameFbbNumber { get; set; }
    }

    public class evFBBGenerateFBBNoModel
    {
        public string FBBNo { get; set; }
        public string errorMessage { get; set; }
    }

    public class evOMQueryContractModel
    {
        public List<evOMQueryContractData> evOMQueryContractDatas { get; set; }
        public string errorMessage { get; set; }
    }

    public class evOMQueryContractData
    {
        public string tdmContractId { get; set; }
        public string contractNo { get; set; }
        public string penalty { get; set; }
        public string duration { get; set; }
    }
}
