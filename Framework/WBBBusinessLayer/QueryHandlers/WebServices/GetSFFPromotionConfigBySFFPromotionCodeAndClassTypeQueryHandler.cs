using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    class GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryHandler : IQueryHandler<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQuery, List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_SFF_PROMOTION_CONFIG> _apifo;

        public GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryHandler(ILogger logger, IEntityRepository<FBB_SFF_PROMOTION_CONFIG> apifo)
        {
            _logger = logger;
            _apifo = apifo;
        }

        public List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData> Handle(GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQuery query)
        {
            var result = new List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData>();
            try
            {
                var qq = (from info in _apifo.Get()
                          where info.SFF_PROMOTION_CODE == query.p_SFF_PROMOTION_CODE
                          select new GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData()
                          {
                              SFF_PROMOTION_CODE = info.SFF_PROMOTION_CODE,
                              PROMOTION_CLASS = info.PROMOTION_CLASS,
                              TYPE_PROMOTION = info.TYPE_PROMOTION,
                              FLAG = info.FLAG
                          });
                result = qq.ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}
