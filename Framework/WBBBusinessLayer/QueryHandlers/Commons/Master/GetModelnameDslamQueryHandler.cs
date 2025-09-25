using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetModelnameDslamQueryHandler : IQueryHandler<GetModelnameDslamQuery, bool>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _VA_FBB_CARDMODELDSAML;

        public GetModelnameDslamQueryHandler(ILogger logger,
            IEntityRepository<FBB_DSLAMMODEL> VA_FBB_CARDMODELDSAML)
        {
            _logger = logger;
            _VA_FBB_CARDMODELDSAML = VA_FBB_CARDMODELDSAML;
        }

        public bool Handle(GetModelnameDslamQuery query)
        {
            if (query.ResultNameCommandDslam == "User")
            {
                var validate_Modelname = _VA_FBB_CARDMODELDSAML.Get(r => r.MODEL == query.ModelName).ToList();


                if (validate_Modelname.Count() > 0)
                {

                    return true;
                }
                else
                {

                    return false;
                }

            }
            return false;
        }

        public IEntityRepository<FBB_DSLAMMODEL> VA_FBB_CARDMODELDSAML { get; set; }
    }
}
