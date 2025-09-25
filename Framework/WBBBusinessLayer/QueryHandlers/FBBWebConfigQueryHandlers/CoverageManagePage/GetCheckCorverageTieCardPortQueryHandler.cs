using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{



    public class GetCheckCorverageTieCardPortQueryHandler : IQueryHandler<GetCheckCorverageTieCardPortQuery, bool>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;



        public GetCheckCorverageTieCardPortQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA)
        {
            _logger = logger;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
        }



        public bool Handle(GetCheckCorverageTieCardPortQuery query)
        {



            var checkTie_flag = (from r in _FBB_COVERAGEAREA.Get()
                                 where r.TIE_FLAG == "Y" && r.CVRID == query.CVRID && r.ACTIVEFLAG == "Y"
                                 select r);


            if (checkTie_flag.Any())
            {

                query.Corverage_Tie = true;
                return true;
            }
            else
            {

                query.Corverage_Tie = false;
                return false;
            }






            ///  return false;
        }

    }
}
