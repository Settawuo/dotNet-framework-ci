using System;
using System.Collections.Generic;
using System.Web.Http;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.PanelModels.WebServiceModels;
using WBBExternalAPI.Contacts;
using AIRNETEntity.StoredProc;
using WBBEntity.PanelModels;
using WBBContract.Queries.ExWebApi;
using WBBEntity.PanelModels.ExWebApiModel;
using WBBExternalAPI.Models.Response.FbbMasterDataController;
using WBBExternalAPI.Models.Request.FbbMasterDataController;

namespace WBBExternalAPI.Controllers
{
    public class FbbMasterDataController : ApiController
    {
        //R24.03 Max DisplayChangePro
        [HttpPost]
        public bool HealthCheck() => true;

        [HttpPost]
        public List<GetTableAirFbbModel> GetTableAirFbb(GetTableAirFbbQuery query) => CallBusinessLayer.ExecuteQuery<List<GetTableAirFbbModel>>(query);

        [HttpPost]
        public GetPackageSequenceModel DisplayChangePro(GetPackageSequenceQuery query) => CallBusinessLayer.ExecuteQuery<GetPackageSequenceModel>(query);//R24.03 Max kunlp885 DisplayChangePro

        [HttpPost]
        public DetailOrderFeeTopupReplaceModel DetailOrderFee(GetDetailOrderFeeTopupReplaceQuery query) => CallBusinessLayer.ExecuteQuery<DetailOrderFeeTopupReplaceModel>(query);//R24.03 Max kunlp885 DetailOrderFee

        public MeshCheckTechnologyModel CheckTechnology(GetMeshCheckTechnologyQuery query) => CallBusinessLayer.ExecuteQuery<MeshCheckTechnologyModel>(query);//R24.03 Max kunlp885 CheckTechnology
        //end R24.03 Max DisplayChangePro

        // R24.03 Max Sale portal
        [HttpPost]
        public AutoCheckCoverageModel GetListNoCoverage(GetListNoCoverageQuery query) => CallBusinessLayer.ExecuteQuery<AutoCheckCoverageModel>(query);

        [HttpPost]
        public List<AppointmentDisplayTrackingList> DisplayTracking(GetAppointmentTrackingQuery query) => CallBusinessLayer.ExecuteQuery<List<AppointmentDisplayTrackingList>>(query);

        [HttpPost]
        public ChangeCountInstallDateQueryModel CountChangeInstallDate(ChangeCountInstallDateQuery query) => CallBusinessLayer.ExecuteQuery<ChangeCountInstallDateQueryModel>(query);

        [HttpPost]
        public CheckOrderCancelModel CheckOrderCancel(CheckOrderCancelQuery query) => CallBusinessLayer.ExecuteQuery<CheckOrderCancelModel>(query);

        [HttpPost]
        public List<SalePortalLeaveMessageList> SalePortalLeaveMessage(SalePortalLeaveMessageQuery query) => CallBusinessLayer.ExecuteQuery<List<SalePortalLeaveMessageList>>(query);

        [HttpPost]
        public List<SalePortalLeaveMessageList> SalePortalLeaveMessageByRefferenceNo(SalePortalLeaveMessageByRefferenceNoQuery query) => CallBusinessLayer.ExecuteQuery<List<SalePortalLeaveMessageList>>(query);

        [HttpPost]
        public List<SalePortalLeaveMessageList> SalePortalLeaveMessageByRefferenceNoIM(SalePortalLeaveMessageByRefferenceNoQuery_IM query) => CallBusinessLayer.ExecuteQuery<List<SalePortalLeaveMessageList>>(query);

        [HttpPost]
        public WBBEntity.PanelModels.ExWebServiceModels.GetPreRegisterQueryData GetPreRegister(WBBContract.Queries.ExWebServices.FbbCpGw.GetPreRegisterQuery query) => CallBusinessLayer.ExecuteQuery<WBBEntity.PanelModels.ExWebServiceModels.GetPreRegisterQueryData>(query);

        private object ExecuteCommand(dynamic command)
        {
            Type commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic commandHandler = WBBExternalAPI.CompositionRoot.Bootstrapper.GetInstance(commandHandlerType);

            commandHandler.Handle(command);

            // Instead of returning the output property of a command, we just return the complete command.
            // There is some overhead in this, but is of course much easier than returning a part of the command.
            return command;
        }

        [HttpPost]
        public object InsertAppointment(InsertAppointmentTrackingCommand command) => ExecuteCommand(command);

        [HttpPost]
        public object UpdatePreregisterStatusCommand(UpdatePreregisterStatusCommand command) => ExecuteCommand(command);

        [HttpPost]
        public object UpdateFBBSaleportalPreRegisterByOrdermcCommand(UpdateFBBSaleportalPreRegisterByOrdermcCommand command) => ExecuteCommand(command);
        //end R24.03 Max Sale portal


        //R24.03 Max Check Change Package Ontop
        [HttpPost]
        public CheckChangePackageOntopModel CheckChangePackageOntop(CheckChangePackageOntopQuery query) => CallBusinessLayer.ExecuteQuery<CheckChangePackageOntopModel>(query);

        [HttpPost]
        public GetPathPdfModel GetPathPdf(GetPathPdfQuery query) => CallBusinessLayer.ExecuteQuery<GetPathPdfModel>(query);

        [HttpPost]
        public List<TrackingModel> GetListOrder(GetTrackingQuery query) => CallBusinessLayer.ExecuteQuery<List<TrackingModel>>(query);
        
        [HttpPost]
        public List<ListChangePackageModel> GetListChangePackage(WBBContract.WebService.GetListChangePackageQuery query) => CallBusinessLayer.ExecuteQuery<List<ListChangePackageModel>>(query);
        //end R24.03 Max Check Change Package Ontop



        // R24.04 Max Sale portal
        [HttpPost]
        public InstallLeaveMessageModel InstallLeaveMessage(InstallLeaveMessageQuery query) => CallBusinessLayer.ExecuteQuery<InstallLeaveMessageModel>(query);
        
        public PackageTopupInternetNotUseModel PackageTopupInternetNotUse(PackageTopupInternetNotUseQuery query) => CallBusinessLayer.ExecuteQuery<PackageTopupInternetNotUseModel>(query);
        //end R24.04 Max Sale portal

        [HttpGet]
        public GetDataFBBPreRegisterQueryResponse GetDataFBBPreRegister(GetDataFBBPreRegisterQueryRequest query)
        {
            GetDataFBBPreRegisterQueryResponse result = new GetDataFBBPreRegisterQueryResponse();
            var resultHandle = CallBusinessLayer.ExecuteQuery<GetDataFBBPreRegisterQueryModel>(new GetDataFBBPreRegisterQuery()
            {
                TRANSACTION_ID = DateTime.Now.ToString("yyyyMMddHHmmssfffff")

            });

            result.RESULT_CODE = resultHandle.RESULT_CODE;
            result.RESULT_DESC = resultHandle.RESULT_DESC;
            result.TRANSACTION_ID = resultHandle.TRANSACTION_ID;
            result.COUNT = resultHandle.COUNT;
            result.PRE_REGITER_LIST = resultHandle.PRE_REGITER_LIST;

            return result;
        }
    }
}