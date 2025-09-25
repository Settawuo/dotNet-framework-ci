using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWZipCodeProvinceQueryHandler : IQueryHandler<GetAWZipCodeProvinceQuery, List<ZipCodeProvince>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _fbbZipCode;

        public GetAWZipCodeProvinceQueryHandler(ILogger logger, IEntityRepository<FBB_ZIPCODE> fbbZipCode)
        {
            _logger = logger;
            _fbbZipCode = fbbZipCode;
        }

        public List<ZipCodeProvince> Handle(GetAWZipCodeProvinceQuery query)
        {
            List<ZipCodeProvince> result;
            var ttp = (from c in _fbbZipCode.Get()
                       where c.LANG_FLAG == "N"
                       && c.STATUS == "A"
                       && query.RegionNames.Contains(c.SUB_REGION)
                       select new ZipCodeProvince()
                       {
                           ProvinceSelect = false,
                           ProvinceName = c.PROVINCE,
                           SUB_REGION = c.SUB_REGION

                       }).Distinct().OrderBy(t => t.ProvinceName);
            result = ttp.ToList();

            return result;
        }
    }
}
