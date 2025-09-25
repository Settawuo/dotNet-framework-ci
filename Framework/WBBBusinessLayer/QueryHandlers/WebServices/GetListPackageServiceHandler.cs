using System.Collections.Generic;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetListPackageServiceHandler : IQueryHandler<GetListPackageByServiceQuery, List<PackageModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetListPackageServiceHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
        }

        public List<PackageModel> Handle(GetListPackageByServiceQuery query)
        {
            InterfaceLogCommand log = null;
            List<PackageModel> packages = new List<PackageModel>();

            try
            {
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                //log = GetPackageListHelper.StartInterfaceAirWfLog(_uow, _intfLog, query,
                //  query.TransactionID, "listPackageByService", "GetListPackageServiceHandler");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "listPackageByService", "GetListPackageServiceHandler", null, "FBB|" + query.FullUrl, "");

                packages = GetPackageListHelper.GetPackageList(_logger, query, _lov);

                System.TimeSpan ts = stopWatch.Elapsed;
                string SBNServiceListPackageElapsedTime = System.String.Format("{0}ms", ts.Milliseconds);
                ArrayOfPackageModelExtendTimeElaspedLog packageLog = new ArrayOfPackageModelExtendTimeElaspedLog()
                {
                    ArrayOfPackageModel = packages,
                    TimeElasped = SBNServiceListPackageElapsedTime
                };
                //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, packageLog, log, "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packageLog, log, "Success", "", "");
                _logger.Info("End call intefacelog");

                return packages;
            }
            catch (System.Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
                _logger.Info("ex message" + ex.Message + " error inner" + ex.InnerException);

                return packages;
            }
        }

    }

    public class ArrayOfPackageModelExtendTimeElaspedLog
    {
        public List<PackageModel> ArrayOfPackageModel { get; set; }
        public string TimeElasped { get; set; }
    }
}