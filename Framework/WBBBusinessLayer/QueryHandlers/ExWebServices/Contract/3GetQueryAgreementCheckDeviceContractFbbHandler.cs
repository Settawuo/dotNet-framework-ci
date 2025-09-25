using RestSharp;
using System;
using System.Net;
using WBBBusinessLayer.Extension;
using WBBBusinessLayer.ServiceHelper;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices.Contract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.Contract
{
    public class GetQueryAgreementCheckDeviceContractFbbHandler : IQueryHandler<GetQueryAgreementCheckDeviceContractFbbRequest, GetQueryAgreementCheckDeviceContractFbbResult>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly ISkyLovHelper _skyLov;

        public GetQueryAgreementCheckDeviceContractFbbHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            ISkyLovHelper skyLov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _skyLov = skyLov;
        }

        public GetQueryAgreementCheckDeviceContractFbbResult Handle(GetQueryAgreementCheckDeviceContractFbbRequest query)
        {
            InterfaceLogCommand log = null;
            var result = new GetQueryAgreementCheckDeviceContractFbbResult();
            var success = "";
            var remark = "";
            try
            {
                var transactionId = string.IsNullOrEmpty(query.transactionId) ? query.fibrenetId : query.transactionId;
                log = InterfaceLogServiceHelper.StartInterfaceLog(
                    _uow,
                    _intfLog,
                    query,
                    transactionId,
                    "CheckDeviceContractFbb",
                    "ATN",
                    query.idCardNo,
                    "FBB",
                    "");

                var queryContractFbbUrl = _skyLov.GetCheckContractDeviceFbbUrl();
                var client = new RestClient(queryContractFbbUrl);
                var request = new RestRequest(Method.POST);
                request.AddJsonBody(query);

                var response = client.Execute(request);
                if (!HttpStatusCode.OK.Equals(response?.StatusCode))
                {
                    success = "Failed";
                    remark = $"StatusCode: {response?.StatusCode.ToString()}, Content: {response?.Content}";
                    return result;
                }
                success = "Success";
                result = response?.Content?.JsonToObj<GetQueryAgreementCheckDeviceContractFbbResult>();
            }
            catch (Exception ex)
            {
                success = "Failed";
                remark = ex.GetBaseException()?.Message ?? ex.Message;
                _logger.Error($"Error when call {GetType().Name} : {ex.ToSafeString()}");
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, success, remark, "");
            }
            return result;
        }
    }
}
