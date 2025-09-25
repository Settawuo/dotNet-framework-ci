using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.Extensions;
using WBBRepositoryService;
using System.Diagnostics;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetSIMAXCoverageQueryHandler : IQueryHandler<GetSIMAXCoverageQuery, SBNCheckCoverageResponse>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepositoryService<FBB_CFG_LOV> _lovService;
        protected Stopwatch timer;

        public GetSIMAXCoverageQueryHandler(ILogger logger,
            IEntityRepositoryService<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
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

        public SBNCheckCoverageResponse Handle(GetSIMAXCoverageQuery query)
        {
            var response = new SBNCheckCoverageResponse();
            var found = false;

            try
            {
                StartWatch();
                found = _lovService.Get(l => l.LOV_TYPE.Equals("SIMAX_COVERAGE")
                    && l.LOV_VAL1.Equals(query.Province)
                    && l.LOV_VAL2.Equals(query.Aumphur)
                    && l.ACTIVEFLAG.ToYesNoFlgBoolean())
                    .Any();
                StopWatch("Finding SIMAX Coverage.");

                response.SBNCheckCoverageData.AVALIABLE = found.ToYesNoFlgString();
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
