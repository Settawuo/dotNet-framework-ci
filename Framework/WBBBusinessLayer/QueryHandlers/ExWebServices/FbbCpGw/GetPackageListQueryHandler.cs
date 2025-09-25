using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public class GetPackageListQueryHandler : IQueryHandler<GetPackageListQuery, List<PackageGroupModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetPackageListQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lov = lov;
            _intfLog = intfLog;
            _uow = uow;
        }

        public List<PackageGroupModel> Handle(GetPackageListQuery query)
        {
            InterfaceLogCommand log = null;
            var packageGroups = new List<PackageGroupModel>();
            var packages = new List<PackageModel>();

            try
            {
                //log = GetPackageListHelper.StartInterfaceAirWfLog(_uow, _intfLog, query, query.TransactionID, "listPackage", "GetPackageListQuery");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "listPackage", "GetPackageListQuery", null, "FBB", "");

                packages = GetPackageListHelper.GetPackageListAll(_logger, query, _lov);

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
                        //packageItem.SpecialOffer = GetPackageListHelper
                        //    .SpecialOfferList(_lov,
                        //        acqOffers: new List<string>(),
                        //        packageGroup: packageItem.PACKAGE_GROUP,
                        //        lang: "");

                        packageGroup.PackageItems.Add(packageItem);
                    }

                    packageGroups.Add(packageGroup);
                }

                //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, packageGroups, log,
                //    "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packageGroups, log, "Success", "", "");
            }
            catch (System.Exception ex)
            {
                if (null != log)
                {
                    //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, packageGroups, log,
                    //    "Failed", ex.Message);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packageGroups, log, "Failed", ex.Message, "");
                }

                throw ex;
            }

            return packageGroups;
        }
    }
}