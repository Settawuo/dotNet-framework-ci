using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.ExWebServices.ATN;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class evOMServiceIVRCheckBlackListQueryHandler : IQueryHandler<evOMServiceIVRCheckBlackListQuery, evOMServiceIVRCheckBlackListModel>
    {
        private const string CONF_ENABLE_APIMAPPINGSKY = "ENABLE_EVOMSERVICEIVRCHECKBLACKLIST_TOBE_APIMAPPINGSKY";
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetCustomerProfileRiskWatchlistRequest, GetCustomerProfileRiskWatchlistResult> _cpRiskWatch;

        public evOMServiceIVRCheckBlackListQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IWBBUnitOfWork uow,
            IQueryHandler<GetCustomerProfileRiskWatchlistRequest, GetCustomerProfileRiskWatchlistResult> cpRiskWatch)
        {
            _logger = logger;
            _lovService = lovService;
            _sffLog = sffLog;
            _intfLog = intfLog;
            _uow = uow;
            _cpRiskWatch = cpRiskWatch;
        }

        public evOMServiceIVRCheckBlackListModel Handle(evOMServiceIVRCheckBlackListQuery query)
        {
            var confApiMappingSky = _lovService.Get().Where(x => x.LOV_NAME == CONF_ENABLE_APIMAPPINGSKY).FirstOrDefault() ?? new FBB_CFG_LOV();
            if (confApiMappingSky?.ACTIVEFLAG == "Y")
            {
                return HandleBySky(query);
            }
            return HandleBySff(query);
        }

        private evOMServiceIVRCheckBlackListModel HandleBySky(evOMServiceIVRCheckBlackListQuery query)
        {
            InterfaceLogCommand log = null;
            var model = new evOMServiceIVRCheckBlackListModel();
            var success = "";
            var remark = "";

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.inCardNo, "evOMServiceIVRCheckBlackListQueryHandler", "evOMServiceIVRCheckBlackList", query.inCardNo, "FBB", "");

                var req = new GetCustomerProfileRiskWatchlistRequest
                {
                    key_name = "IDCardNo",
                    key_value = query.inCardNo,
                };
                var res = _cpRiskWatch.Handle(req);

                if (!ResultBaseATN.DefSuccessCodes.Contains(res?.resultCode))
                {
                    success = "Failed";
                    model.ErrorMessage = res?.resultDescription;
                    if (string.IsNullOrEmpty(model.ErrorMessage))
                    {
                        model.ErrorMessage = res?.developerMessage?.Trim().Split(':')?.FirstOrDefault();
                    }
                }
                //success
                model.returnFlag = res?.resultData?.customerRisk?.watchlistStatus ?? string.Empty;
                success = "Success";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error when call {GetType().Name} : {ex.ToSafeString()}");
                model.ErrorMessage = "Error Exception";
                success = "Failed";
                remark = ex.Message;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, success, remark, "");
            }
            return model;
        }

        public evOMServiceIVRCheckBlackListModel HandleBySff(evOMServiceIVRCheckBlackListQuery query)
        {
            InterfaceLogCommand log = null;
            var model = new evOMServiceIVRCheckBlackListModel();

            try
            {
                var request = new SFFServices.SffRequest();
                request.Event = "evOMServiceIVRCheckBlackList";

                var paramArray = new SFFServices.Parameter[2];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();


                //param0.Name = "orderType";
                //param0.Value = "New Registration";
                //param1.Name = "IDCardNo";
                //param1.Value = "9679312563295";                

                param0.Name = "orderType";
                param0.Value = "New Registration";
                param1.Name = "IDCardNo";
                param1.Value = query.inCardNo;


                paramArray[0] = param0;
                paramArray[1] = param1;


                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                _logger.Info("Call evOMServiceIVRCheckBlackList SFF");
                _logger.Info("inIDCardNo: " + query.inCardNo);

                //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog, request, "",
                //    "evOMServiceIVRCheckBlackListQueryHandler", "evOMServiceIVRCheckBlackList", query.inCardNo);
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, "", "evOMServiceIVRCheckBlackListQueryHandler", "evOMServiceIVRCheckBlackList", query.inCardNo, "FBB", "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);

                    if (data != null)
                    {
                        _logger.Info(data.ErrorMessage);
                        if (data.ErrorMessage != null)
                        {
                            model.ErrorMessage = data.ErrorMessage;
                            var errSp = data.ErrorMessage.Trim().Split(':');
                            model.ErrorMessage = errSp[0];
                        }

                        foreach (var a in data.ParameterList.Parameter)
                        {
                            if (a.Name == "returnFlag") model.returnFlag = a.Value;
                        }

                    }
                }

                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                //    "Success", model.ErrorMessage.ToSafeString());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.ErrorMessage.ToSafeString(), "");
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.GetErrorMessage();
                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                //    "Failed", model.ErrorMessage.ToSafeString());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", model.ErrorMessage.ToSafeString(), "");
            }

            return model;
        }
    }
}
