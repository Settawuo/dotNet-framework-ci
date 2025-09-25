using System;
using System.Diagnostics;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetAvaliablePortQueryHandler : IQueryHandler<GetAvaliablePortQuery, SBNCheckCoverageResponse>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfoService;
        private readonly IEntityRepository<FBB_PORT_NOTE> _portNoteService;
        protected Stopwatch timer;

        public GetAvaliablePortQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            _logger = logger;
            _portInfoService = portInfoService;
            _portNoteService = portNoteService;
            _uow = uow;
        }

        private void StartWatch()
        {
            timer = Stopwatch.StartNew();
        }

        private void StopWatch(string actionName)
        {
            timer.Stop();
            _logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }

        public SBNCheckCoverageResponse Handle(GetAvaliablePortQuery query)
        {
            var response = new SBNCheckCoverageResponse();
            try
            {
                StartWatch();
                ExWebServiceHelper.LogAvaliablePort(query.CURRENTPORTID,
                    query.REFF_USER.ToSafeString(), query.REFF_KEY.ToSafeString(), _uow, _portInfoService, _portNoteService);
                StopWatch("LogAvaliablePort");

                response.RETURN_CODE = 0;
                response.RETURN_DESC = "";
            }
            catch (Exception ex)
            {
                response.RETURN_CODE = -1;
                response.RETURN_DESC = ex.GetErrorMessage();
            }


            return response;
        }
    }
}
