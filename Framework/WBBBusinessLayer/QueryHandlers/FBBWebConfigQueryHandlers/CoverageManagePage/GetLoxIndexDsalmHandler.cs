using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetLoxIndexDsalmHandler : IQueryHandler<GetLotStratIndex, decimal>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;
        private readonly IEntityRepository<FBB_CARDMODEL> _FBB_CARDMODEL;


        public GetLoxIndexDsalmHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
             IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL,
             IEntityRepository<FBB_CARDMODEL> FBB_CARDMODEL)
        {
            _logger = logger;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
            _FBB_CARDMODEL = FBB_CARDMODEL;
        }




        //        select  dm.slotstartindex  from wbb.fbb_dslammodel dm ,fbb_dslam_info dff ,fbb_cardmodel fbc
        //where 
        //dm.dslammodelid= dff.dslammodelid and dm.dslammodelid =fbc.cardmodelid  

        public decimal Handle(GetLotStratIndex query)
        {

            var dsamlammodelid = (from r in _FBB_DSLAM_INFO.Get()
                                  where r.CVRID == query.CVRID
                                  select r).ToList();


            //            select count(*)  from fbb_coveragearea c
            //where c.tie_flag = 'Y' and c.cvrid = 224; 
            //and c.activeflag = 'Y'


            var a = (from r in dsamlammodelid
                     join d in _FBB_DSLAMMODEL.Get() on r.DSLAMMODELID equals d.DSLAMMODELID
                     select d.SLOTSTARTINDEX).FirstOrDefault();






            return a;
        }

    }
}
