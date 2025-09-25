using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetRoomNumberQueryhandler : IQueryHandler<GetRoomNumberQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBB_DORM;

        public GetRoomNumberQueryhandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBB_DORM)
        {
            _logger = logger;
            _FBB_DORM = FBB_DORM;
        }

        public List<DropdownModel> Handle(GetRoomNumberQuery query)
        {
            string[] temp = query.filteroom.Split(':');

            var name = temp[0];
            var no = temp[1];

            if (query.language == "TH")
            {

                return (from r in _FBB_DORM.Get()
                        where r.DORMITORY_NAME_TH == name && r.DORMITORY_NO_TH == no
                        select new DropdownModel
                        {
                            Text = r.NUMBER_OF_ROOM,
                            Value = r.NUMBER_OF_ROOM,

                        }).ToList();
            }
            else
            {
                return (from r in _FBB_DORM.Get()
                        where r.DORMITORY_NAME_EN == temp[0] && r.DORMITORY_NO_EN == temp[1]
                        select new DropdownModel
                        {
                            Text = r.NUMBER_OF_ROOM,
                            Value = r.NUMBER_OF_ROOM,

                        }).ToList();
            }
        }
    }
}
