using System.Collections.Generic;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCustomerSpeOfferQueryHandler : IQueryHandler<GetCustomerSpeOfferQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffProfLog;
        private readonly IEntityRepository<FBB_VSMP_LOG> _vsmpLog;

        public GetCustomerSpeOfferQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffProfLog,
            IEntityRepository<FBB_VSMP_LOG> vsmpLog)
        {
            _logger = logger;
            _lov = lov;
            _sffProfLog = sffProfLog;
            _vsmpLog = vsmpLog;
        }

        public List<string> Handle(GetCustomerSpeOfferQuery query)
        {
            _logger.Info(query.DumpToXml());
            return GetPackageListHelper.ValidateSpecialOffer(_lov, _sffProfLog, _vsmpLog, query);
        }
    }
}
