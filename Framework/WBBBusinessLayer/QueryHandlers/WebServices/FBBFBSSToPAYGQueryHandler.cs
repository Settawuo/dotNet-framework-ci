using System;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class FBBFBSSToPAYGQueryHandler : IQueryHandler<FBBFBSSToPAYGQuery, FBBFBSSToPAYGModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBFBSSToPAYGModel> _Fbbtopayg;
        private readonly IWBBUnitOfWork _uow;

        public FBBFBSSToPAYGQueryHandler(ILogger logger,
            IEntityRepository<FBBFBSSToPAYGModel> Fbbtopayg,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _Fbbtopayg = Fbbtopayg;
            _uow = uow;
        }

        public FBBFBSSToPAYGModel Handle(FBBFBSSToPAYGQuery query)
        {
            FBBFBSSToPAYGModel res = new FBBFBSSToPAYGModel();
            try
            {

            }
            catch (Exception err)
            {

            }
            return res;
        }
    }
}
