using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFixedAssetResultQueryHandler : IQueryHandler<GetFixedAssetResultQuery, List<FixedAssetInventoryModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FixedAssetInventoryModel> _objService;
        public GetFixedAssetResultQueryHandler(ILogger logger, IEntityRepository<FixedAssetInventoryModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<FixedAssetInventoryModel> Handle(GetFixedAssetResultQuery query)
        {

            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var cur = new OracleParameter();
            cur.ParameterName = "cur";
            cur.OracleDbType = OracleDbType.RefCursor;
            cur.Direction = ParameterDirection.Output;
            List<FixedAssetInventoryModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FIXED_ASSET_INVENTORY_RPT.p_fetch_asset",
                     new
                     {
                         p_order_type = query.p_order_type,
                         p_product_name = query.p_product_name,
                         //   p_service_name  = query.p_service_name,
                         p_ord_dt_from = query.p_ord_dt_from,
                         p_ord_dt_to = query.p_ord_dt_to,

                         //  return code
                         ret_code = ret_code,
                         cur = cur,

                     }).ToList();



            return executeResult;
        }

    }
}
