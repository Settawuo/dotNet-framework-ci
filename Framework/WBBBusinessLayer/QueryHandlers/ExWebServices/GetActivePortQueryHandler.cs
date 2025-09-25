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
    public class GetActivePortQueryHandler : IQueryHandler<GetActivePortQuery, SBNCheckCoverageResponse>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfoService;
        private readonly IEntityRepository<FBB_PORT_NOTE> _portNoteService;
        protected Stopwatch timer;

        public GetActivePortQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            _logger = logger;
            _uow = uow;
            _portInfoService = portInfoService;
            _portNoteService = portNoteService;
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

        public SBNCheckCoverageResponse Handle(GetActivePortQuery query)
        {
            var response = new SBNCheckCoverageResponse();
            try
            {
                StartWatch();
                ExWebServiceHelper.LogActivePort(query.CURRENTPORTID,
                    query.REFF_USER, query.REFF_KEY, _uow, _portInfoService, _portNoteService);
                StopWatch("LogActivePort");

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
