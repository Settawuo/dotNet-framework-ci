namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    using System.Linq;
    using System.Collections.Generic;
    using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
    using WBBContract;
    using WBBContract.Queries.ExWebServices.FbbCpGw;
    using WBBContract.Queries.WebServices;
    using WBBData.Repository;
    using WBBEntity.Models;
    using WBBEntity.PanelModels.WebServiceModels;
    using WBBContract.Commands;
    using WBBData.DbIteration;
    using System;

    public class GetPackageListByServiceQueryHandler : IQueryHandler<GetPackageListByServiceQuery, List<PackageGroupModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetPackageListByServiceQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lov = lov;
            _intfLog = intfLog;
            _uow = uow;
        }

        public List<PackageGroupModel> Handle(GetPackageListByServiceQuery query)
        {
            InterfaceLogCommand log = null;
            var packageGroups = new List<PackageGroupModel>();
            var packages = new List<PackageModel>();

            try
            {
                log = GetPackageListHelper.StartInterfaceAirWfLog(_uow, _intfLog, query, query.TransactionID, "listPackageByService", "GetPackageListByServiceQuery");

                packages = GetPackageListHelper.GetPackageList(_logger, query);

                var moddedPackages = (from t in packages
                                      where t.PRODUCT_SUBTYPE != "VOIP"
                                          && t.PACKAGE_TYPE != "Bundle"
                                      group t by new { t.OWNER_PRODUCT, t.PACKAGE_GROUP } into g
                                      select g);

                foreach (var package in moddedPackages)
                {
                    var packageGroup = new PackageGroupModel
                    {
                        OwnerProduct = package.Key.OWNER_PRODUCT,
                        PackgaeGroup = package.Key.PACKAGE_GROUP,
                        PackageItems = new List<PackageModel>(),
                    };

                    foreach (var packageItem in package)
                    {
                        packageItem.SpecialOffer = GetPackageListHelper.FindSpecialOffer(_lov, packageItem.OWNER_PRODUCT);
                        packageGroup.PackageItems.Add(packageItem);
                    }

                    packageGroups.Add(packageGroup);
                }

                GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, packageGroups, log,
                    "Success", "");

            }
            catch (Exception ex)
            {
                if (null != log)
                {
                    GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, packageGroups, log,
                        "Failed", ex.Message);
                }

                throw ex;
            }

            return packageGroups;
        }
    }
}