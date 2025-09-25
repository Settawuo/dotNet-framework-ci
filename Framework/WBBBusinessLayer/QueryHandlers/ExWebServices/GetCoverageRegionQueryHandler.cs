using System;
using System.Diagnostics;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetCoverageRegionQueryHandler : IQueryHandler<GetCoverageRegionQuery, SBNCheckCoverageResponse>
    {
        private readonly ILogger _logger;
        //private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _covService;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;
        protected Stopwatch timer;

        public GetCoverageRegionQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGE_REGION> covService,
            IEntityRepository<FBB_ZIPCODE> zipcode)
        {
            _logger = logger;
            _covService = covService;
            _zipcode = zipcode;
        }

        private void StartWatch()
        {
            timer = Stopwatch.StartNew();
        }

        private void StopWatch(string actionName)
        {
            timer.Stop();
            _logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }

        public SBNCheckCoverageResponse Handle(GetCoverageRegionQuery query)
        {
            var response = new SBNCheckCoverageResponse();
            //var found = false;

            try
            {
                StartWatch();
                response = GetCoverageRegionHelper.GetCoverageRegion(_logger, _covService, _zipcode, query);

                StopWatch("Finding Coverage Region");
            }
            catch (Exception ex)
            {
                response.RETURN_CODE = -1;
                response.RETURN_DESC = ex.GetErrorMessage();
            }

            return response;
        }
    }

    public static class GetCoverageRegionHelper
    {
        public static SBNCheckCoverageResponse GetCoverageRegion(ILogger logger,
            IEntityRepository<FBB_COVERAGE_REGION> covService,
            IEntityRepository<FBB_ZIPCODE> zipcode,
            GetCoverageRegionQuery query)
        {
            var response = new SBNCheckCoverageResponse();


            var zipCodeRowId = (from z in zipcode.Get()
                                where z.PROVINCE == query.Province && z.AMPHUR == query.Aumphur && z.TUMBON == query.Tambon
                                select z.ZIPCODE_ROWID).FirstOrDefault();

            var q = "zipcode_rowid_th";

            if (query.Language == "2")
                q = "zipcode_rowid_en";

            logger.Info("CheckCoverageRegion: " + q + " = " + zipCodeRowId);

            var found1 = covService.SqlQuery(string.Format(@"
                                    select * from fbb_coverage_region cr 
                                    where cr.{0} = '{1}' and coverage_status = 'ON_SITE'
                                    and cr.activeflag = 'Y' and cr.service_type = '{2}'
                                    and (cr.tower_th = '' or cr.tower_th is null)
                                    and (cr.tower_en = '' or cr.tower_en is null)
                                ", q, zipCodeRowId, query.ServiceType));

            if (query.SSO == "")
                found1 = found1.Where(a => a.ONTARGET_DATE_EX <= DateTime.Now.Date);
            else
                found1 = found1.Where(a => a.ONTARGET_DATE_IN <= DateTime.Now.Date);

            var f = found1.Select(l => l.OWNER_PRODUCT);
            var found = f.FirstOrDefault();

            if (!string.IsNullOrEmpty(found)) //found
            {
                response.SBNCheckCoverageData.OWNER_PRODUCT = found.ToString();
                response.SBNCheckCoverageData.AVALIABLE = found.Any().ToYesNoFlgString();
                response.RETURN_CODE = 0;
                response.RETURN_DESC = "";
            }
            else
            {
                if (query.ServiceType == "OTHER") //ถ้า other มาเช็คด้วย group amphur ต่อ
                {
                    var listGroupAmphur = (from z in zipcode.Get()
                                           where z.PROVINCE == query.Province
                                           && z.AMPHUR == query.Aumphur
                                           select z);

                    string groupAmphur = listGroupAmphur.Any() ? listGroupAmphur.FirstOrDefault().GROUP_AMPHUR : "";

                    logger.Info("CheckCoverageRegion finding by group amphur: " + groupAmphur);

                    var found2 = covService.SqlQuery(string.Format(@"
                                    select* from fbb_coverage_region cr 
                                    where cr.zipcode_rowid_th = '{0}' and coverage_status = 'ON_SITE'
                                    and cr.activeflag = 'Y' and cr.service_type = 'OTHER'
                                    and (cr.tower_th = '' or cr.tower_th is null)
                                    and (cr.tower_en = '' or cr.tower_en is null)
                                ", groupAmphur));

                    if (query.SSO == "")
                        found2 = found2.Where(a => a.ONTARGET_DATE_EX <= DateTime.Now.Date);
                    else
                        found2 = found2.Where(a => a.ONTARGET_DATE_IN <= DateTime.Now.Date);

                    var f2 = found2.Select(l => l.OWNER_PRODUCT);
                    var founds = f2.FirstOrDefault();

                    if (!string.IsNullOrEmpty(founds))
                    {
                        response.SBNCheckCoverageData.OWNER_PRODUCT = founds.ToString();
                        response.SBNCheckCoverageData.AVALIABLE = founds.Any().ToYesNoFlgString();
                        response.RETURN_CODE = 0;
                        response.RETURN_DESC = "";
                    }
                    else
                    {
                        response.SBNCheckCoverageData.OWNER_PRODUCT = "";
                        response.SBNCheckCoverageData.AVALIABLE = "N";
                        response.RETURN_CODE = -1;
                        response.RETURN_DESC = "Coverage Region is Null";
                    }
                }
                else
                {
                    response.SBNCheckCoverageData.OWNER_PRODUCT = "";
                    response.SBNCheckCoverageData.AVALIABLE = "N";
                    response.RETURN_CODE = -1;
                    response.RETURN_DESC = "Coverage Region is Null";
                }
            }

            return response;
        }
    }
}
