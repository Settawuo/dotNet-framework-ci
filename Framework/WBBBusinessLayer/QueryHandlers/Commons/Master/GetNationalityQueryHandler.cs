using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetNationalityQueryHandler : IQueryHandler<GetNationalityQuery, List<NationalityModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_NATIONALITY_MASTER> _natService;

        public GetNationalityQueryHandler(ILogger logger,
            IEntityRepository<FBB_NATIONALITY_MASTER> natService)
        {
            _logger = logger;
            _natService = natService;
        }

        public List<NationalityModel> Handle(GetNationalityQuery query)
        {
            var data = _natService.Get(n => n.STATUS.Equals("Y"));
            List<NationalityModel> result;

            if (query.CurrentCulture.IsThaiCulture())
            {
                result = data.Select(n => new NationalityModel
                {
                    Nationality = n.NATIONALITY_THA,
                    InterfaceSFF = n.INTERFACE_SFF,
                })
                .OrderBy(n => n.Nationality)
                .ToList();
            }
            else
            {
                result = data.Select(n => new NationalityModel
                {
                    Nationality = n.NATIONALITY_ENG,
                    InterfaceSFF = n.INTERFACE_SFF,
                })
                .OrderBy(n => n.Nationality)
                .ToList();
            }

            return result;
        }
    }
}
