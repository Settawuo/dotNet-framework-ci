using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class GetMaterialCodeQueryHandler : IQueryHandler<GetMaterialCodeQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPAYG_VENDOR> _material;

        public GetMaterialCodeQueryHandler(ILogger logger, IEntityRepository<FBBPAYG_VENDOR> material)
        {
            _logger = logger;
            _material = material;
        }

        public List<LovModel> Handle(GetMaterialCodeQuery query)
        {
            List<LovModel> result = new List<LovModel>();
            try
            {
                var resultMaterial = (from m in _material.Get() select m);

                result = (from v in resultMaterial
                          select new LovModel
                          {
                              LOV_NAME = v.MATERIAL_CODE,
                              LOV_VAL1 = v.MATERIAL_CODE
                          }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetMaterialCodeQueryHandler");
            }

            return result;
        }
    }
}
