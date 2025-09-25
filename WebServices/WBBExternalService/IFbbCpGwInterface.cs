using System.ServiceModel;
using WBBContract.Commands.ExWebServices;
using WBBContract.Commands.WebServices.FBSS;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBExternalService.Models;
using WBBExternalService.Models.Request;
using WBBExternalService.Models.Response;

namespace WBBExternalService
{
    [ServiceContract]
    public interface IFbbCpGwInterface
    {
        [OperationContract]
        CheckSFFProfileResponse CheckSFFProfile(GetMassCommonAccountQuery query);

        [OperationContract]
        ListBuildVillResponse ListBuildingVillage(GetListBuildingVillageQuery query);

        [OperationContract]
        CheckCoverageResponse CheckCoverage(GetCoverageResultQuery query);

        [OperationContract]
        RegisterOutOfCoverageResponse RegisterOutOfCoverage(GetRegOutOfCoverageResultQuery query);

        [OperationContract]
        RegisterInCoverageResponse RegisterInCoverage(GetRegResultQuery query);

        [OperationContract]
        RegisterInCoverageResponse FBSSRegisterInCoverage(GetFBSSRegResultQuery query);

        [OperationContract]
        ListPackageResponse ListPackages(GetListPackageQuery query);

        [OperationContract]
        ListPackageByServiceResponse ListPackagesByService(GetPackageListByServiceQuery query);

        [OperationContract]
        CheckSFFInternetProfileResponse CheckSFFInternetProfile(GetSFFInternetProfileQuery query);

        [OperationContract]
        ListFBSSBuildingResponse ListFBSSBuilding(GetListFBSSBuildingQuery query);

        [OperationContract]
        CoverageResultEnquiryResponse CoverageResultEnquiry(CoverageResultEnquiryCommand command);

        [OperationContract]
        CheckBlacklistResponse CheckBlacklist(GetBlackListQuery query);

        [OperationContract]
        ListDuplicateOrderResponse ListDuplicateOrder(GetOrderDuplicateQuery query);

        [OperationContract]
        GetSubContractTimeSlotResponse GetSubContractTimeSlot(GetSubContractTimeSlotQuery query);

        [OperationContract]
        ReserveTimeSlotResponse ReserveTimeSlot(ReserveTimeSlotQuery query);

        [OperationContract]
        ListImageResponse GetPicture(GetListImagePOIQuery query);

        [OperationContract]
        CreateOrderPreRegisterResponse CreateOrderPreRegister(CreateOrderPreRegisterModel command);

        [OperationContract]
        QueryOrderPreRegisterResponse QueryOrderPreRegister(GetPreRegisterQuery query);

        [OperationContract]
        CheckPremiumFlagResponse CheckPremiumFlag(CheckPremiumFlagQuery query);

        [OperationContract]
        CheckTimeSlotbySubTypeResponse CheckTimeSlotbySubType(CheckTimeSlotbySubTypeQuery query);

        [OperationContract]
        CheckFMCPackageResponse CheckFMCPackage(CheckFMCPackageQuery query);

        [OperationContract]
        QueryOrderByASCEmpIdResponse QueryOrderByASCEmpId(GetOrderByASCEmpIdQuery query);

        [OperationContract]
        QueryOrderDetailResponse QueryOrderDetail(GetOrderDetailQuery query);

        [OperationContract]
        UpdateDocumentOrCSNoteByNotifyOrderResponse UpdateDocumentOrCSNoteByNotifyOrder(UpdateDocumentOrCSNoteByNotifyOrderCommand command);

        [OperationContract]
        QuestionCustomerInsightResponse QueryQuestionCustomerInsight(QuestionCustomerInsightQuery query);

        [OperationContract]
        FBBPendingDeductionResponse FBBPendingDeduction(FBBPendingDeductionModel command);

        [OperationContract]
        ListMaxMeshInstallmentRespond ListMaxMeshInstallment(ListMaxMeshInstallmentQuery query);

        [OperationContract]
        MicrositeWSResponse MicrositeWS(MicrositeWSModel command);

        [OperationContract]
        MicrositeActionResponse MicrositeAction(MicrositeActionModel command);

        [OperationContract]
        InsertCoverageRusultResponse InsertCoverageRusult(InsertCoverageRusultModel command);

        [OperationContract]
        UpdateCoverageRusultResponse UpdateCoverageResult(UpdateCoverageRusultModel command);

        [OperationContract]
        PermissionUserResponse PermissionuserACIM(PermissionUserModel command);

        [OperationContract]
        PatchAdressESRIResponse PatchAdressESRI(GetPatchAdressESRIQuery query);

        [OperationContract]
        QueryLOVForWebResponse QueryLOVForWeb(QueryLOVForWebQuery query);

        [OperationContract]
        CheckCoverageSpecialResponse CheckCoverageSpecial(CheckCoverageSpecialRequest request);

        [OperationContract]
        CheckCoverageForWorkflowResponse CheckCoverageForWorkflow(CheckCoverageForWorkflowRequest request);

        [OperationContract]
        TransferFileToStorageResponse TransferFileToStorage(TransferFileToStorageRequest request);
    }
}