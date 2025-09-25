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
    public class GetPackageListBySFFPromoQueryHandler : IQueryHandler<GetPackageListBySFFPromoQuery, List<PackageModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetPackageListBySFFPromoQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
        }

        public List<PackageModel> Handle(GetPackageListBySFFPromoQuery query)

        {
            InterfaceLogCommand log = null;

            try
            {
                //log = GetPackageListHelper.StartInterfaceAirWfLog(_uow, _intfLog, query, query.TransactionID, "list_package_by_sffpromo", "GetListPackageBySFFPromoHandler");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "list_package_by_sffpromo", "GetPackageListBySFFPromoQueryHandler", null, "FBB|" + query.FullUrl, "");

                var packages = GetPackageListHelper.GetPackageListbySFFPromo(_logger, query, _lov);

                //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, packages, log, "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packages, log, "Success", "", "");

                return packages;
            }
            catch (System.Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.Message, "");

                throw ex;
            }


        }

    }
}
