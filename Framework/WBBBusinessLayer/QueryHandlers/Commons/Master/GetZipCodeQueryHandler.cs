using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetZipCodeQueryHandler : IQueryHandler<GetZipCodeQuery, List<ZipCodeModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipCodeService;

        public GetZipCodeQueryHandler(ILogger logger, IEntityRepository<FBB_ZIPCODE> zipCodeService)
        {
            _logger = logger;
            _zipCodeService = zipCodeService;
        }

        public List<ZipCodeModel> Handle(GetZipCodeQuery query)
        {
            //var data = _zipCodeService.Get(z => z.LANG_FLAG.Equals(langFlag)
            //    && z.STATUS.Equals("A"));
            var data = _zipCodeService.SqlQuery(
                            string.Format(@"select * from {0}.FBB_ZIPCODE z
                            where z.STATUS = 'A'", "WBB"));

            var zipcodes = data.Select(z => new ZipCodeModel()
            {
                ZipCodeId = z.ZIPCODE_ROWID,
                IsThai = (z.LANG_FLAG == "N"),
                ZipCode = z.ZIPCODE,
                Province = z.PROVINCE,
                Amphur = z.AMPHUR,
                Tumbon = z.TUMBON,
                RegionCode = z.REGION_CODE
            })
                         .OrderBy(z => z.Province)
                         .ThenBy(z => z.Amphur)
                         .ThenBy(z => z.Tumbon)
                         .ToList();

            return zipcodes;

            //using (var service = new SBNServices.SBNWebServiceService())
            //{
            //    var data = service.listAllZipcodeNew(langFlag);

            //    if (data.RETURN_CODE != 0)
            //    {
            //        _logger.Info(data.RETURN_MESSAGE);
            //        return new List<ZipCodeModel>();
            //    }
            //    if (!data.AllZipcodeArrayNews.Any())
            //    {
            //        _logger.Info("Zipcode is null");
            //        return new List<ZipCodeModel>();
            //    }

            //    var zipcodes = data.AllZipcodeArrayNews
            //        .Select(z => new ZipCodeModel()
            //        {
            //            ZipCodeId = z.ZIPCODE_ROWID,
            //            IsThai = langFlag.ToYesNoFlgBoolean(),
            //            ZipCode = z.ZIPCODE,
            //            Province = z.PROVINCE,
            //            Amphur = z.AMPHUR,
            //            Tumbon = z.TUMBON
            //        })
            //             .OrderBy(z => z.Province)
            //             .ThenBy(z => z.Amphur)
            //             .ThenBy(z => z.Tumbon)
            //             .ToList();
            //    return zipcodes;
            //}
        }
    }
}