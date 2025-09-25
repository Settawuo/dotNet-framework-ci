using LinqKit;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectZipCodeHandler : IQueryHandler<SelectZipCodeQuery, ZipCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectZipCodeHandler(ILogger logger, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }

        public ZipCodeModel Handle(SelectZipCodeQuery query)
        {
            var predicate = PredicateBuilder.True<FBB_ZIPCODE>();
            _logger.Info("start: " + predicate + "\r\n");
            _logger.Info("language: " + query.Language);
            predicate = predicate.And(t => t.LANG_FLAG == query.Language);
            _logger.Info("after lanuage: " + predicate);
            predicate = predicate.And(t => t.STATUS == "A");

            if (!string.IsNullOrEmpty(query.Province))
            {
                predicate = predicate.And(t => t.PROVINCE.ToUpper() == query.Province.ToUpper());
            }

            if (!string.IsNullOrEmpty(query.Aumphur))
            {
                predicate = predicate.And(t => t.AMPHUR.ToUpper() == query.Aumphur.ToUpper());
            }

            if (!string.IsNullOrEmpty(query.Tumbon))
            {
                predicate = predicate.And(t => t.TUMBON.ToUpper() == query.Tumbon.ToUpper());
            }

            if (!string.IsNullOrEmpty(query.PostalCode))
            {
                predicate = predicate.And(t => t.ZIPCODE.ToUpper() == query.PostalCode.ToUpper());
            }

            var rowZipcodes = _FBB_ZIPCODE.Get(predicate);
            //(from z in _FBB_ZIPCODE.Get()
            //               where z.LANG_FLAG == query.Language
            //                    && z.STATUS == "A"
            //               select z).Where(predicate);
            _logger.Info("rowZipcodes: " + rowZipcodes);
            var zipcodes = rowZipcodes
                .Select(z => new ZipCodeModel()
                {
                    Amphur = z.AMPHUR,
                    Province = z.PROVINCE,
                    Tumbon = z.TUMBON,
                    ZipCode = z.ZIPCODE,
                    ZipCodeId = z.ZIPCODE_ROWID,
                    RegionCode = z.REGION_CODE
                });
            _logger.Info("ZipCodeId: " + (zipcodes.FirstOrDefault() == null ? "" : zipcodes.FirstOrDefault().ZipCodeId));


            return zipcodes.FirstOrDefault();
        }
    }
}
