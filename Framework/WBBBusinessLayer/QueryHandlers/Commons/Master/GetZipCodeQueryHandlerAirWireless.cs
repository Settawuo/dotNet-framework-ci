using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetZipCodeQueryHandlerAirWireless : IQueryHandler<GetZipCodeAirQuery, List<ZipCodeModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipCodeService;

        public GetZipCodeQueryHandlerAirWireless(ILogger logger, IEntityRepository<FBB_ZIPCODE> zipCodeService)
        {
            _logger = logger;
            _zipCodeService = zipCodeService;
        }

        public List<ZipCodeModel> Handle(GetZipCodeAirQuery query)
        {
            var langFlag = (query.CurrentCulture.IsThaiCulture() ? "N" : "Y");
            var regioncode = query.Regioncode;
            var langFlagBool = langFlag.ToYesNoFlgBoolean();

            //var data = _zipCodeService.Get(z => z.LANG_FLAG.Equals(langFlag)
            //    && z.STATUS.Equals("A"));
            var data = _zipCodeService.SqlQuery(
                            string.Format(@"select * from {0}.FBB_ZIPCODE z
                            where region_code='{1}' and z.LANG_FLAG = '{2}' and z.STATUS = 'A'", "WBB", regioncode, langFlag));

            var zipcodes = data.Select(z => new ZipCodeModel()
            {
                ZipCodeId = z.ZIPCODE_ROWID,
                IsThai = langFlagBool,
                ZipCode = z.ZIPCODE,
                Province = z.PROVINCE,
                Amphur = z.AMPHUR,
                Tumbon = z.TUMBON
            })
                         .OrderBy(z => z.Province)
                         .ThenBy(z => z.Amphur)
                         .ThenBy(z => z.Tumbon)
                         .ToList();

            return zipcodes;
        }
    }
}
