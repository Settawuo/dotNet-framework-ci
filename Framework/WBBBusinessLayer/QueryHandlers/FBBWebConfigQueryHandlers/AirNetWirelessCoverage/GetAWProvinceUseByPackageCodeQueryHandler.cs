using AIRNETEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWProvinceUseByPackageCodeQueryHandler : IQueryHandler<GetAWProvinceUseByPackageCodeQuery, List<ProvinceTable>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_LOCATION> _airpackagelocation;

        public GetAWProvinceUseByPackageCodeQueryHandler(ILogger logger, IAirNetEntityRepository<AIR_PACKAGE_LOCATION> airpackagelocation)
        {
            _logger = logger;
            _airpackagelocation = airpackagelocation;
        }

        public List<ProvinceTable> Handle(GetAWProvinceUseByPackageCodeQuery query)
        {

            List<ProvinceTable> result = new List<ProvinceTable>();
            try
            {
                var ttp = (from c in _airpackagelocation.Get()
                           where c.PACKAGE_CODE == query.PackageCode
                           && c.ADDRESS_TYPE == null
                           select new ProvinceTable()
                           {
                               ProvinceSelect = false,
                               ProvinceName = c.PROVINCE,
                               SubRegion = c.REGION,
                               EffectiveDtm = c.EFFECTIVE_DTM,
                               ExpireDtm = c.EXPIRE_DTM

                           }).Distinct().OrderBy(t => t.ProvinceName);
                result = ttp.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("GetAWProvinceUseByPackageCodeQuery " + ex.Message);
            }

            return result;
        }
    }
}
