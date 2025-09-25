using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetCoverageAreaResultQueryHandler : IQueryHandler<GetCoverageAreaResultQuery, CoverageAreaResultModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _covresult;


        public GetCoverageAreaResultQueryHandler(ILogger logger, IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> covresult)
        {
            _logger = logger;
            _covresult = covresult;

        }

        public CoverageAreaResultModel Handle(GetCoverageAreaResultQuery query)
        {
            _logger.Info("Begin GetCoverageAreaResultQuery");
            var result = (from c in _covresult.Get()
                          where c.TRANSACTION_ID == query.TRANSACTION_ID
                          select new CoverageAreaResultModel()
                          {
                              ADDRRESS_TYPE = c.ADDRRESS_TYPE,
                              POSTAL_CODE = c.POSTAL_CODE,
                              SUB_DISTRICT_NAME = c.SUB_DISTRICT_NAME,
                              LANGUAGE = c.LANGUAGE,
                              BUILDING_NAME = c.BUILDING_NAME,
                              BUILDING_NO = c.BUILDING_NO,
                              PHONE_FLAG = c.PHONE_FLAG,
                              FLOOR_NO = c.FLOOR_NO,
                              ADDRESS_NO = c.ADDRESS_NO,
                              MOO = c.MOO,
                              SOI = c.SOI,
                              ROAD = c.ROAD,
                              LATITUDE = c.LATITUDE,
                              LONGITUDE = c.LONGITUDE,
                              UNIT_NO = c.UNIT_NO,
                              COVERAGE = c.COVERAGE,
                              ADDRESS_ID = c.ADDRESS_ID,
                              ACCESS_MODE_LIST = c.ACCESS_MODE_LIST,
                              PLANNING_SITE_LIST = c.PLANNING_SITE_LIST,
                              IS_PARTNER = c.IS_PARTNER,
                              PARTNER_NAME = c.PARTNER_NAME,
                              PREFIXNAME = c.PREFIXNAME,
                              FIRSTNAME = c.FIRSTNAME,
                              LASTNAME = c.LASTNAME,
                              CONTACTNUMBER = c.CONTACTNUMBER,
                              PRODUCTTYPE = c.PRODUCTTYPE,
                              ZIPCODE_ROWID = c.ZIPCODE_ROWID,
                              OWNER_PRODUCT = c.OWNER_PRODUCT,
                              TRANSACTION_ID = c.TRANSACTION_ID,
                              RESULTID = c.RESULTID
                          });

            _logger.Info("End GetCoverageAreaResultQuery");
            _logger.Info("GetCoverageAreaResultQuery Result:" + result.FirstOrDefault());
            return result.FirstOrDefault();
        }
    }

    public class GetCoverageAreaResultByResultIDQueryHandler : IQueryHandler<GetCoverageAreaResultByResultIDQuery, CoverageAreaResultModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _covresult;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;

        public GetCoverageAreaResultByResultIDQueryHandler(ILogger logger,
            IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> covresult,
            IEntityRepository<FBB_ZIPCODE> zipcode)
        {
            _logger = logger;
            _covresult = covresult;
            _zipcode = zipcode;
        }

        public CoverageAreaResultModel Handle(GetCoverageAreaResultByResultIDQuery query)
        {
            decimal ResultID = 0;
            ResultID = decimal.Parse(query.ResultID);
            var result = (from c in _covresult.Get()
                          join d in _zipcode.Get() on c.ZIPCODE_ROWID equals d.ZIPCODE_ROWID
                          where c.RESULTID == ResultID
                          select new CoverageAreaResultModel()
                          {
                              ADDRRESS_TYPE = c.ADDRRESS_TYPE,
                              POSTAL_CODE = c.POSTAL_CODE,
                              SUB_DISTRICT_NAME = c.SUB_DISTRICT_NAME,
                              LANGUAGE = c.LANGUAGE,
                              BUILDING_NAME = c.BUILDING_NAME,
                              BUILDING_NO = c.BUILDING_NO,
                              PHONE_FLAG = c.PHONE_FLAG,
                              FLOOR_NO = c.FLOOR_NO,
                              ADDRESS_NO = c.ADDRESS_NO,
                              MOO = c.MOO,
                              SOI = c.SOI,
                              ROAD = c.ROAD,
                              LATITUDE = c.LATITUDE,
                              LONGITUDE = c.LONGITUDE,
                              UNIT_NO = c.UNIT_NO,
                              COVERAGE = c.COVERAGE,
                              ADDRESS_ID = c.ADDRESS_ID,
                              ACCESS_MODE_LIST = c.ACCESS_MODE_LIST,
                              PLANNING_SITE_LIST = c.PLANNING_SITE_LIST,
                              IS_PARTNER = c.IS_PARTNER,
                              PARTNER_NAME = c.PARTNER_NAME,
                              PREFIXNAME = c.PREFIXNAME,
                              FIRSTNAME = c.FIRSTNAME,
                              LASTNAME = c.LASTNAME,
                              CONTACTNUMBER = c.CONTACTNUMBER,
                              PRODUCTTYPE = c.PRODUCTTYPE,
                              ZIPCODE_ROWID = c.ZIPCODE_ROWID,
                              OWNER_PRODUCT = c.OWNER_PRODUCT,
                              TRANSACTION_ID = c.TRANSACTION_ID,
                              RESULTID = c.RESULTID,
                              PROVINCE = d.PROVINCE,
                              AUMPHUR = d.AMPHUR
                          });
            return result.FirstOrDefault();
        }
    }
}
