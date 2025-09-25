using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetDSLAMModelQueryHandler : IQueryHandler<GetDSLAMModelQuery, List<FBB_DSLAMMODEL>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _dslamModel;

        public GetDSLAMModelQueryHandler(ILogger logger, IEntityRepository<FBB_DSLAMMODEL> dslamModel)
        {
            _logger = logger;
            _dslamModel = dslamModel;
        }


        public List<FBB_DSLAMMODEL> Handle(GetDSLAMModelQuery query)
        {
            // todo have to where cvrid 
            var listdslamModeldata = (from dslamModel in _dslamModel.Get()
                                      where dslamModel.ACTIVEFLAG == "Y"
                                      select dslamModel).ToList();

            return listdslamModeldata;
        }
    }
}
