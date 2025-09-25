using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetSubcontracttttQueryHandler : IQueryHandler<GetSubcontractQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBB_DORM;

        public GetSubcontracttttQueryHandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBB_DORM)
        {
            _logger = logger;
            _FBB_DORM = FBB_DORM;
        }

        public List<DropdownModel> Handle(GetSubcontractQuery query)
        {
            if (query.language == "TH")
            {
                return (from r in _FBB_DORM.Get()
                        select new DropdownModel
                        {
                            Text = r.SUB_CONTRACT_NAME_TH,
                            Value = r.SUB_CONTRACT_NAME_TH,
                        }).ToList();
            }
            else
            {
                return (from r in _FBB_DORM.Get()
                        select new DropdownModel
                        {
                            Text = r.SUB_CONTRACT_NAME_EN,
                            Value = r.SUB_CONTRACT_NAME_EN,
                        }).ToList();
            }

        }
    }
}
