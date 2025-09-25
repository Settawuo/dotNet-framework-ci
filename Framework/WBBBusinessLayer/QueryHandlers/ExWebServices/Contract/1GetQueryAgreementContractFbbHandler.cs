using RestSharp;
using System;
using System.Collections.Generic;
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
    public class GetQueryAgreementContractFbbHandler : IQueryHandler<GetQueryAgreementContractFbbRequest, List<GetQueryAgreementContractFbbResult>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly ISkyLovHelper _skyLov;

        public GetQueryAgreementContractFbbHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            ISkyLovHelper skyLov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _skyLov = skyLov;
        }

        public List<GetQueryAgreementContractFbbResult> Handle(GetQueryAgreementContractFbbRequest query)
        {
            InterfaceLogCommand log = null;
            var results = new List<GetQueryAgreementContractFbbResult>();
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
                    "QueryContractFbb",
                    "ATN",
                    query.idCardNo,
                    "FBB",
                    "");

                var queryContractFbbUrl = _skyLov.GetQueryContractFbbUrl();
                var client = new RestClient(queryContractFbbUrl);
                var request = new RestRequest(Method.POST);
                request.AddJsonBody(query);

                var response = client.Execute(request);
                if (!HttpStatusCode.OK.Equals(response?.StatusCode))
                {
                    success = "Failed";
                    remark = $"StatusCode: {response?.StatusCode.ToString()}, Content: {response?.Content}";
                    return results;
                }
                success = "Success";
                if (!string.IsNullOrEmpty(response?.Content))
                {
                    if (response?.Content?.StartsWith("{") == true)
                    {
                        var result = response?.Content?.JsonToObj<GetQueryAgreementContractFbbResult>();
                        results.Add(result);
                    }
                    else
                    {
                        results = response?.Content?.JsonToObj<List<GetQueryAgreementContractFbbResult>>();
                    }
                }
            }
            catch (Exception ex)
            {
                success = "Failed";
                remark = ex.GetBaseException()?.Message ?? ex.Message;
                _logger.Error($"Error when call {GetType().Name} : {ex.ToSafeString()}");
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, results, log, success, remark, "");
            }
            return results;
        }
    }
}
