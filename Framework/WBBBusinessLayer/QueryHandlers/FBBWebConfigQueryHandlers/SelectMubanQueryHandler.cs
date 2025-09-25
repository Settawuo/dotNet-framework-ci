using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectMubanQueryHandler : IQueryHandler<SelectMubanQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;


        public SelectMubanQueryHandler(ILogger logger,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
            IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION)
        {
            _logger = logger;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
        }

        public List<LovModel> Handle(SelectMubanQuery query)
        {
            return SelectMubanHelper.GetMuban(_logger, _FBB_ZIPCODE, _FBB_COVERAGE_REGION, query);
        }
    }

    public static class SelectMubanHelper
    {
        public static List<LovModel> GetMuban(ILogger logger,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
            IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION,
            SelectMubanQuery query)
        {
            IEnumerable<FBB_COVERAGE_REGION> result = null;

            var zipCodeRowId = (from z in FBB_ZIPCODE.Get()
                                where z.PROVINCE == query.province && z.AMPHUR == query.aumphur && z.TUMBON == query.tumbon
                                select z.ZIPCODE_ROWID).FirstOrDefault();

            var q = "";

            if (query.Language == "1") // thai
            {
                q = "zipcode_rowid_th";
                logger.Info("GetMuban: " + q + " = " + zipCodeRowId);

                result = (from v in FBB_COVERAGE_REGION.Get()
                          where v.SERVICE_TYPE == "VILLAGE" && v.ACTIVEFLAG == "Y"
                          && v.ZIPCODE_ROWID_TH == zipCodeRowId && v.COVERAGE_STATUS == "ON_SITE"
                          select v);

                if (query.SSO == "")
                    result = result.Where(a => a.ONTARGET_DATE_EX <= DateTime.Now.Date);
                else
                    result = result.Where(a => a.ONTARGET_DATE_IN <= DateTime.Now.Date);

                var outcome = (from v in result
                               select new LovModel
                               {
                                   LOV_NAME = v.TOWER_TH,
                                   DISPLAY_VAL = v.TOWER_TH,
                               }).ToList();

                return outcome;
            }
            else  // eng
            {
                q = "zipcode_rowid_en";
                logger.Info("GetMuban: " + q + " = " + zipCodeRowId);


                result = (from v in FBB_COVERAGE_REGION.Get()
                          where v.SERVICE_TYPE == "VILLAGE" && v.ACTIVEFLAG == "Y"
                          && v.ZIPCODE_ROWID_EN == zipCodeRowId && v.COVERAGE_STATUS == "ON_SITE"
                          select v);

                if (query.SSO == "")
                    result = result.Where(a => a.ONTARGET_DATE_EX <= DateTime.Now.Date);
                else
                    result = result.Where(a => a.ONTARGET_DATE_IN <= DateTime.Now.Date);

                var outcome = (from v in result
                               select new LovModel
                               {
                                   LOV_NAME = v.TOWER_EN,
                                   DISPLAY_VAL = v.TOWER_EN,
                               }).ToList();

                return outcome;
            }
        }
    }
}

