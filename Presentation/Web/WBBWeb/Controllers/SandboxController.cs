using System.Collections.Generic;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices.FBSS;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBWeb.Controllers
{
    public class SandboxController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;

        public SandboxController(IQueryProcessor queryProcessor, ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
        }

        public FBSSCoverageResult GetFBSSFeasibilityCheck(GetFBSSFeasibilityCheck query)
        {
            return _queryProcessor.Execute(query);
        }

        public List<FBSSChangedAddressInfo> GetFBSSChangedBuilding(GetFBSSChangedBuilding query)
        {
            return _queryProcessor.Execute(query);
        }

        public List<FBSSTimeSlot> GetFBSSAppointment(GetFBSSAppointment query)
        {
            return _queryProcessor.Execute(query);
        }

    }
}
