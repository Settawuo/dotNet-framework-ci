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
    public class GetAWPackageForMappingUseQueryHandler : IQueryHandler<GetAWPackageForMappingUseQuery, List<PackageTypeUse>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<PackageTypeUse> _PackageTypeUse;


        public GetAWPackageForMappingUseQueryHandler(ILogger logger, IAirNetEntityRepository<PackageTypeUse> PackageTypeUse)
        {
            _logger = logger;
            _PackageTypeUse = PackageTypeUse;

        }

        public List<PackageTypeUse> Handle(GetAWPackageForMappingUseQuery query)
        {
            try
            {
                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                List<PackageTypeUse> executeResult = _PackageTypeUse.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_PACKAGE_FOR_MAPPING_USE",
                  new
                  {
                      p_package_code = query.PackageCode,

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
