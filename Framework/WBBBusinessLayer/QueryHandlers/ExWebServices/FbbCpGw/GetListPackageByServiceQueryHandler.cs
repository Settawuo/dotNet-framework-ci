using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public class GetListPackageByServiceQueryHandler : IQueryHandler<GetListPackageByServiceQuery, List<PackageGroupModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffChkProfLog;
        private readonly IEntityRepository<FBB_VSMP_LOG> _vsmpLog;
        private readonly IWBBUnitOfWork _uow;

        public GetListPackageByServiceQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffChkProfLog,
            IEntityRepository<FBB_VSMP_LOG> vsmpLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lov = lov;
            _intfLog = intfLog;
            _uow = uow;
            _sffChkProfLog = sffChkProfLog;
            _vsmpLog = vsmpLog;
        }

        public List<PackageGroupModel> Handle(GetListPackageByServiceQuery query)
        {
            InterfaceLogCommand log = null;
            var packageGroups = new List<PackageGroupModel>();
            var packages = new List<PackageModel>();

            try
            {
                //log = GetPackageListHelper.StartInterfaceAirWfLog(_uow, _intfLog, query,
                //   query.TransactionID, "listPackageByService", "GetPackageListByServiceQuery");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "GetListPackageByServiceQueryHandler", "listPackageByService", "", "FBB|" + query.FullUrl, "");

                packages = GetPackageListHelper.GetPackageList(_logger, query, _lov);

                _logger.Info("Get moddedPackages");
                var moddedPackages = (from t in packages
                                      where t.PRODUCT_SUBTYPE != "VOIP"
                                          && t.PACKAGE_TYPE != "Bundle"
                                      group t by new { t.OWNER_PRODUCT, t.MAPPING_CODE } into g
                                      //group t by new { t.OWNER_PRODUCT, t.PACKAGE_GROUP } into g
                                      select g);

                var moreModdedPacks = (from t in moddedPackages group t by t.First().PACKAGE_GROUP into g select g);

                var sffChkProfLog = (from t in _sffChkProfLog.Get()
                                     where t.TRANSACTION_ID == query.TransactionID
                                     select t).FirstOrDefault();

                var isAwnProduct = false;

                if (null != sffChkProfLog)
                {
                    var logProdNames = sffChkProfLog.OUTPRODUCTNAME.Split('/');

                    if (logProdNames.Any())
                    {
                        isAwnProduct = (from i_l in _lov.Get()
                                        where i_l.LOV_TYPE == "AWN_PRODUCT"
                                          && i_l.LOV_NAME == logProdNames.FirstOrDefault()
                                        select i_l).Any();
                    }
                }

                foreach (var package in moreModdedPacks)
                {
                    var packageGroup = new PackageGroupModel
                    {
                        OwnerProduct = package.First().Key.OWNER_PRODUCT,
                        PackgaeGroup = package.Key,
                        PackageItems = new List<PackageModel>(),
                    };

                    foreach (var packageItem in package)
                    {
                        // หา special offer
                        //var acquriedOfferQuery = new GetCustomerSpeOfferQuery
                        //{
                        //    //ReferenceID = query.TransactionID,
                        //    PackageGroup = package.First().Key.OWNER_PRODUCT,
                        //    IsAWNProduct = isAwnProduct,
                        //};

                        //var acqOffers = GetPackageListHelper
                        //                    .ValidateSpecialOffer(_lov,
                        //                        _sffChkProfLog,
                        //                        _vsmpLog,
                        //                        acquriedOfferQuery);

                        //packageItem.Each((pack, n) => pack.SpecialOffer = GetPackageListHelper.SpecialOfferList(_lov,
                        //                                                    acqOffers: acqOffers,
                        //                                                    packageGroup: package.First().Key.OWNER_PRODUCT,
                        //                                                    lang: query.Language));


                        //packageItem.SpecialOffer = GetPackageListHelper.SpecialOfferList(_lov,
                        //                                                    acqOffers: acqOffers,
                        //                                                    packageGroup: package.Key,
                        //                                                    lang: query.Language);

                        //packageItem.AccessMode = GetPackageListHelper
                        //    .ReverseAirnetWfAccessModeMapper(_logger, _lov,
                        //    package.Key.OWNER_PRODUCT, query.IsPartner, query.PartnerName);

                        packageGroup.PackageItems.AddRange(packageItem);
                    }

                    packageGroups.Add(packageGroup);
                }

                //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, packageGroups, log, "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packageGroups, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                if (null != log)
                {
                    _logger.Info("ex message" + ex.Message + " error inner" + ex.InnerException);
                    //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, ex, log,
                    //    "Failed", "inner" + ex.InnerException + " Message:"+ ex.Message);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
                }
                _logger.Info("ex message" + ex.Message + " error inner" + ex.InnerException);
                throw ex;
            }

            return packageGroups;
        }
    }

    public class GetListPackageSellRouterQueryHandler : IQueryHandler<WBBContract.Queries.WebServices.GetListPackageSellRouterQuery, GetListPackageSellRouterModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IAirNetEntityRepository<ListPackageSellRouterModel> _objService;

        public GetListPackageSellRouterQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IAirNetEntityRepository<ListPackageSellRouterModel> objService)
        {
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetListPackageSellRouterModel Handle(WBBContract.Queries.WebServices.GetListPackageSellRouterQuery query)
        {
            InterfaceLogCommand log = null;
            var getListPackageSellRouterModel = new GetListPackageSellRouterModel();
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "GetListPackageSellRouterQueryHandler", "GetListPackageSellRouterQuery", "", "FBB|" + query.FullUrl, "");
            try
            {
                var o_return_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    ParameterName = "o_return_code",
                    Direction = ParameterDirection.Output
                };

                var ioResults = new OracleParameter
                {
                    ParameterName = "ioResults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR905.LIST_PACKAGE_SELL_ROUTER",
                   new
                   {
                       p_mapping_project = query.P_MAPPING_PROJECT,
                       p_mapping_value = query.P_MAPPING_VALUE,

                       // Out
                       o_return_code = o_return_code,
                       ioresults = ioResults

                   }).ToList();

                getListPackageSellRouterModel.o_return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;
                getListPackageSellRouterModel.ListPackageSellRouter = executeResult;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                return getListPackageSellRouterModel;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
                return new GetListPackageSellRouterModel();
            }
        }
    }
}