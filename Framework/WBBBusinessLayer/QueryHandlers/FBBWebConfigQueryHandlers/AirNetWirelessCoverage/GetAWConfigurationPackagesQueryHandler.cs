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
    public class GetAWConfigurationPackagesQueryHandler : IQueryHandler<GetAWConfigurationPackagesQuery, List<NewPackageMaster>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<NewPackageMaster> _NewPackageMaster;


        public GetAWConfigurationPackagesQueryHandler(ILogger logger, IAirNetEntityRepository<NewPackageMaster> NewPackageMaster)
        {
            _logger = logger;
            _NewPackageMaster = NewPackageMaster;

        }

        public List<NewPackageMaster> Handle(GetAWConfigurationPackagesQuery query)
        {
            var searchresult = new List<NewPackageMaster>();
            try
            {
                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                List<NewPackageMaster> executeResult = _NewPackageMaster.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_NEW_PACKAGE_MASTER",
                  new
                  {
                      p_sff_product_code = query.PromotionCode,
                      p_sff_promo_name_thai = query.PromotionNameThai,
                      p_sff_promo_name_eng = query.PromotionNameEng,

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