using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetVoucherProjectDescriptionByGroupQueryHandler : IQueryHandler<GetVoucherProjectDescriptionByGroupQuery, List<FBB_VOUCHER_MASTER>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;
        private readonly IEntityRepository<FBB_VOUCHER_MASTER> _VoucherMasterTable;

        public GetVoucherProjectDescriptionByGroupQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog, IEntityRepository<FBB_VOUCHER_MASTER> VoucherMasterTable)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
            _VoucherMasterTable = VoucherMasterTable;
        }

        public List<FBB_VOUCHER_MASTER> Handle(GetVoucherProjectDescriptionByGroupQuery query)
        {
            List<FBB_VOUCHER_MASTER> data = _VoucherMasterTable.Get(t => t.VOUCHER_PROJECT_GROUP == query.voucher_project_group).ToList();
            return data;
        }
    }
}
