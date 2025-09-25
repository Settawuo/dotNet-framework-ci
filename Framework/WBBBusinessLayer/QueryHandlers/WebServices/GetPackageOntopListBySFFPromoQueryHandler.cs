using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPackageOntopListBySFFPromoQueryHandler : IQueryHandler<GetPackageOntopListBySFFPromoQuery, List<PackageModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_SFF_PROMOTION_MAPPING> _promotionMapping;
        private readonly IAirNetEntityRepository<PackageModel> _objService;
        private readonly IEntityRepository<object> _obj;

        public GetPackageOntopListBySFFPromoQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_SFF_PROMOTION_MAPPING> promotionMapping,
            IEntityRepository<FBB_CFG_LOV> lov,
            IAirNetEntityRepository<PackageModel> objService,
            IEntityRepository<object> obj)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
            _promotionMapping = promotionMapping;
            _objService = objService;
            _obj = obj;
        }

        public List<PackageModel> Handle(GetPackageOntopListBySFFPromoQuery query)

        {
            InterfaceLogCommand log = null;
            List<PackageModel> packages = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_SFF_PROMOCODE, "LIST_PACKAGE_ONTOP_BY_CHANGE", "GetPackageOntopListBySFFPromoQueryHandler", null, "FBB|" + query.FullUrl, "");

                List<PackageModel> tmpPackages = new List<PackageModel>();

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                var a = (
                         from pm in _promotionMapping.Get()
                         where pm.SFF_PROMOTION_CODE == query.P_SFF_PROMOCODE
                         select pm).ToList();

                if (a != null && a.Count > 0)
                {

                    var aa = a.FirstOrDefault();

                    string ProductSubtype = GetPackageListHelper.GetProductSubtype(_obj, query.P_OWNER_PRODUCT.ToSafeString(), query.P_ADDRESS_ID.ToSafeString());
                    //List<PackageModel> packages = GetPackageListHelper.GetPackageOntopListbySFFPromo(_logger, query, _lov);
                    packages = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR910.LIST_PACKAGE_ADD_BUNDLING",
               new
               {
                   p_owner_product = query.P_OWNER_PRODUCT.ToSafeString(),
                   p_product_subtype = ProductSubtype,
                   p_package_for = "PUBLIC",
                   p_sff_promotion_code = query.P_SFF_PROMOCODE.ToSafeString(),

                   // return code
                   o_return_code = o_return_code,
                   ioResults = ioResults

               }).ToList();

                    foreach (var item in packages.Where(p => p.SFF_PROMOTION_CODE == aa.SFF_PROMOTION_CODE_1))
                    {
                        tmpPackages.Add(item);
                    }
                    foreach (var item in packages.Where(p => p.SFF_PROMOTION_CODE == aa.SFF_PROMOTION_CODE_2))
                    {
                        tmpPackages.Add(item);
                    }
                    foreach (var item in packages.Where(p => p.SFF_PROMOTION_CODE == aa.SFF_PROMOTION_CODE_3))
                    {
                        tmpPackages.Add(item);
                    }
                    foreach (var item in packages.Where(p => p.SFF_PROMOTION_CODE == aa.SFF_PROMOTION_CODE_4))
                    {
                        tmpPackages.Add(item);
                    }
                    foreach (var item in packages.Where(p => p.SFF_PROMOTION_CODE == aa.SFF_PROMOTION_CODE_5))
                    {
                        tmpPackages.Add(item);
                    }
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, tmpPackages, log, "Success", "", "");

                return tmpPackages;
            }
            catch (System.Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.Message, "");
                return null;
            }


        }

    }
}
