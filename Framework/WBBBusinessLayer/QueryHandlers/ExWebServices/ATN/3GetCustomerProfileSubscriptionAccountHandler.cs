using RestSharp;
using System;
using System.Net;
using WBBBusinessLayer.Extension;
using WBBBusinessLayer.ServiceHelper;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices.ATN;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.ATN
{
    public class GetCustomerProfileSubScriptionAccountHandler : IQueryHandler<GetCustomerProfileSubScriptionAccountRequest, GetCustomerProfileSubScriptionAccountResult>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly ISkyLovHelper _skyLov;

        public GetCustomerProfileSubScriptionAccountHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            ISkyLovHelper skyLov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _skyLov = skyLov;
        }

        public GetCustomerProfileSubScriptionAccountResult Handle(GetCustomerProfileSubScriptionAccountRequest query)
        {
            InterfaceLogCommand log = null;
            var result = new GetCustomerProfileSubScriptionAccountResult();
            var success = "";
            var remark = "";
            try
            {
                var transactionId = string.IsNullOrEmpty(query.transactionId) ? query.msisdn : query.transactionId;
                log = InterfaceLogServiceHelper.StartInterfaceLog(
                    _uow,
                    _intfLog,
                    query,
                    transactionId,
                    "SubScriptionAccount",
                    "ATN",
                    query.msisdn,
                    "FBB",
                    "");

                var queryContractFbbUrl = _skyLov.GetSubscriptionAccountUrl(query.msisdn, query.status, query.filter);
                var client = new RestClient(queryContractFbbUrl);
                var request = new RestRequest(Method.GET);
                request.ReqHeader(nameof(query.channel), query.channel);
                request.ReqHeader(nameof(query.username), query.username);
                request.ReqQueryParameter(nameof(query.status), query.status);
                request.ReqQueryParameter(nameof(query.filter), query.filter);

                var response = client.Execute(request);
                if (!HttpStatusCode.OK.Equals(response?.StatusCode))
                {
                    success = "Failed";
                    remark = $"StatusCode: {response?.StatusCode.ToString()}, Content: {response?.Content}";
                    return result;
                }
                success = "Success";
                result = response?.Content?.JsonToObj<GetCustomerProfileSubScriptionAccountResult>();
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
