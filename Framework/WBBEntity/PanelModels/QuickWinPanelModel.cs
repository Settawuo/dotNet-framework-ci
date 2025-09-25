using System.Collections.Generic;
using System.Data;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBEntity.PanelModels
{
    public class QuickWinPanelModel : PanelModelBase
    {
        public QuickWinPanelModel()
        {
            this.CoveragePanelModel = new CoveragePanelModel();
            this.DisplayPackagePanelModel = new DisplayPackagePanelModel();
            this.CustomerRegisterPanelModel = new CustomerRegisterPanelModel();
            this.SummaryPanelModel = new SummaryPanelModel();
            this.DisplayVasPackagePanelModel = new DisplayVasPackagePanelModel();
            this.CoverageAreaResultModel = new CoverageAreaResultModel();
            this.MulitPlaybox = new List<MulitPlayboxModel>();
            this.PackagePromotionList = new List<PackagePromotionModel>();
            this.OfficerInfoPanelModel = new OfficerInfoPanelModel();
        }

        public CoveragePanelModel CoveragePanelModel { get; set; }
        public DisplayPackagePanelModel DisplayPackagePanelModel { get; set; }
        public CustomerRegisterPanelModel CustomerRegisterPanelModel { get; set; }
        public SummaryPanelModel SummaryPanelModel { get; set; }
        public bool? ForCoverageResult { get; set; }
        public DisplayVasPackagePanelModel DisplayVasPackagePanelModel { get; set; }
        public CoverageAreaResultModel CoverageAreaResultModel { get; set; }

        public List<PackageModel> ObjOwnerPackage { get; set; }



        public string outPrimaryContactFirstName { get; set; }
        public string outContactLastName { get; set; }
        public string outAmphur { get; set; }
        public string outBuildingName { get; set; }
        public string outFloor { get; set; }
        public string outHouseNumber { get; set; }
        public string outMoo { get; set; }
        public string outMooban { get; set; }
        public string outProvince { get; set; }
        public string outRoom { get; set; }
        public string outSoi { get; set; }
        public string outStreetName { get; set; }
        public string outBillLanguage { get; set; }
        public string outtumbol { get; set; }
        public string outBirthDate { get; set; }
        public string outEmail { get; set; }
        public string outparameter2 { get; set; }
        public string outAccountName { get; set; }
        public string outAccountNumber { get; set; }
        public string outServiceAccountNumber { get; set; }
        public string outBillingAccountNumber { get; set; }
        public string outProductName { get; set; }
        public string outDayOfServiceYear { get; set; }
        public string outRegisteredDate { get; set; }

        public string outAccountSubCategory { get; set; }
        public string outPostalCode { get; set; }
        public string outcardType { get; set; }

        public string IDCardNo { get; set; }
        public string IDCardType { get; set; }
        public string IDCardTypeENG { get; set; }

        public string no { get; set; }

        public decimal SffProfileLogID { get; set; }

        public string TopUp { get; set; }
        public string v_icon_pb { get; set; }
        public string v_check_pb { get; set; }
        public string v_add_playbox { get; set; }
        public string v_owner_product { get; set; }
        public string v_sff_main_promotionCD { get; set; }
        public string hdTransactionGuid { get; set; }
        public string v_PackageCode { get; set; }
        public string v_number_of_pb_number { get; set; }
        public string v_number_of_playbox { get; set; }
        public string address_flag { get; set; } // จาก map esri       
        public string CallfeascheckFlag { get; set; }
        public string TempBuildnameforlistpackage { get; set; }
        public string TriplePlayFlag { get; set; }
        public string SiteCode { get; set; }
        public string FlowFlag { get; set; }
        public string TransactionID { get; set; }
        public string outOwnerProduct { get; set; }
        public string outMobileNo { get; set; }
        public string outAccountCategory { get; set; }
        public string CATEGORY { get; set; }
        public string Register_device { get; set; }
        public string Browser_type { get; set; }
        public string old_relate_mobile { get; set; }
        public string relate_mobile_action { get; set; }
        public string mobileNumberContact { get; set; }
        public string UseMap { get; set; }
        public string PlugAndPlayFlow { get; set; }
        public string ClientIP { get; set; }
        public string SignaturePDF { get; set; }
        public string SignaturePDF2 { get; set; }

        //17.3 Splitter Management
        public string SplitterFlagFirstTime { get; set; }
        public string SplitterFlag { get; set; }
        public string ReservationId { get; set; }
        public string SpecialRemark { get; set; }
        public string TimeSlotMessage { get; set; }
        public string SplitterListStr { get; set; }
        public string SplitterTransactionId { get; set; }

        public string PruductCD_Content { get; set; }

        //Multi playbox
        public string RegisterPlayboxNumber { get; set; }
        public string IsMulitPlayboxSpeedPass { get; set; }
        public List<MulitPlayboxModel> MulitPlaybox { get; set; }

        //Entry Fee
        public string Entrancefee { get; set; }

        //17.7 Member get Member
        public string MemberGetMemberFlag { get; set; }
        public string RegisterChannel { get; set; }

        //17.9 Speed boost
        public List<PackagePromotionModel> PackagePromotionList { get; set; }
        public string ExistingFlag { get; set; }
        public DataTable CheckInOrder { get; set; }
        public string CheckXDSL { get; set; }
        public string PayMentTranID { get; set; }
        public string PayMentOrderID { get; set; }
        public string PayMentRecurringCharge { get; set; }
        public string PayMentRecurringChargeVAT { get; set; }
        public string PayMentTechnologyDisplay { get; set; }
        public string PayMentMethod { get; set; }
        public string PayMentPointType { get; set; }
        public string PayMentSelectPackage { get; set; }
        public string PayMentSelectOptionSelectInstallRouter { get; set; }
        public string ACCESS_MODE_SELECT { get; set; }
        public string SERVICE_CODE_SELECT { get; set; }
        public string AppointmentDateSelect { get; set; }

        public string HdResultId { get; set; }
        public string ZTEAddressID { get; set; }
        public string tempDslamJson { get; set; }
        public string v_package_subtype { get; set; }
        public string TopupMesh { get; set; }
        public string SessionId { get; set; }
        public string DataBypass_FlowFlag { get; set; }
        public string DataBypass_Casecode { get; set; }
        public string DataBypass_ShowTimeSlot { get; set; }
        public string DataBypass_CasecodeMessage { get; set; }
        public string DataBypass_BuildName { get; set; }
        public string DataBypass_BuildNo { get; set; }
        public string DataBypass_BuildNameSetup { get; set; }
        public string SffPromotiontCodeMeshSelect { get; set; }
        public int languageCulture { get; set; }
        public OfficerInfoPanelModel OfficerInfoPanelModel { get; set; }
        public List<PackageTopupInternetNotUse> PackageTopupInternetNotUseList { get; set; }
        public string cur_project_name_option { get; set; }
        public string cur_mobile_checkright { get; set; }
        public string cur_mobile_checkright_option { get; set; }
        public string cur_mobile_get_benefit { get; set; }
        public string cur_mobile_get_benefit_option { get; set; }
        public string AIChatBotBypass_Channel { get; set; }
        public string AIChatBotBypass_Flag { get; set; }
        public string PendingOrderFbss_Flag { get; set; }
        public string PendingOrderFbss_Change_Promotion_Flag { get; set; }
        public string SuperDuperProductName { get; set; }
        public string SuperDuperServiceName { get; set; }
        public string StaffPrivilegeBypass_TransactionID { get; set; }
        public string StaffPrivilegeBypass_Channel { get; set; }
        public string StaffPrivilegeBypass_Flag { get; set; }
        public string outServiceLevel { get; set; }
        public string TopupReplace { get; set; }
        public string count_atv { get; set; }
        public string sympton_code_pbreplace { get; set; }
        public string Is3bbCoverage { get; set; }

        //R22.11 Mesh with arpu
        public string MESH_ARPU_POINTS_SELECT { get; set; }
        public string MESH_ARPU_POINTS { get; set; }
        public string MESH_ARPU_FLAG_OPTION { get; set; }
        public string MESH_ARPU_FLAG_MESH { get; set; }

        public string OrderSubLine { get; set; }
        public string OrderSubID { get; set; }

        // R23.05.2023 Created: THOTST49
        public string MobileNoRegister { get; set; }
        public bool IsCheckConsentTerm { get; set; }
    }
}
