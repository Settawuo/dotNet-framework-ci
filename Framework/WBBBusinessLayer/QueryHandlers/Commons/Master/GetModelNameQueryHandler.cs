using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetModelNameQueryHandler : IQueryHandler<GetModelNameQuery, bool>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CARDMODEL> _VA_FBB_CARDMODEL;

        public GetModelNameQueryHandler(ILogger logger,
            IEntityRepository<FBB_CARDMODEL> FBB_CARDMODEL)
        {
            _logger = logger;
            _VA_FBB_CARDMODEL = FBB_CARDMODEL;
        }

        public bool Handle(GetModelNameQuery query)
        {
            if (query.ResultModel == "User")
            {
                var validate_Modelname = _VA_FBB_CARDMODEL.Get(r => r.MODEL == query.ModelName).ToList();


                if (validate_Modelname.Count() > 0)
                {

                    return query.ResultBI = true;
                }
                else
                {

                    return query.ResultBI = false;
                }

            }
            return false;
        }
    }
}
