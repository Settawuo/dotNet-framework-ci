using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    class GetAWPackageForMappingQueryHandler : IQueryHandler<GetAWPackageForMappingQuery, List<PackageTypeSearch>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<PackageTypeSearch> _PackageTypeSearch;


        public GetAWPackageForMappingQueryHandler(ILogger logger, IAirNetEntityRepository<PackageTypeSearch> PackageTypeSearch)
        {
            _logger = logger;
            _PackageTypeSearch = PackageTypeSearch;

        }

        public List<PackageTypeSearch> Handle(GetAWPackageForMappingQuery query)
        {
            try
            {
                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                List<PackageTypeSearch> executeResult = _PackageTypeSearch.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_PACKAGE_FOR_MAPPING_ALL",
                  new
                  {
                      p_package_code = query.PackageCode,
                      p_package_type = query.ProductType,

                      /// return //////
                      o_return_code = o_return_code,
                      ioResults = cursor

                  }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
