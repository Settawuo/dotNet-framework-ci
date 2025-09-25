using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFbssConfigTBLQueryHandler : IQueryHandler<GetFbssConfigTBLQuery, List<FbssConfigTBL>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSS_CONFIG_TBL> _configTBL;

        public GetFbssConfigTBLQueryHandler(ILogger logger, IEntityRepository<FBSS_CONFIG_TBL> configTBL)
        {
            _logger = logger;
            _configTBL = configTBL;
        }

        public List<FbssConfigTBL> Handle(GetFbssConfigTBLQuery query)
        {
            var config = (from c in _configTBL.Get() select c);

            if (query.CON_TYPE != null)
            {
                if (query.CON_TYPE != "")
                {
                    config = (from c in config where (c.CON_TYPE.ToUpper() == query.CON_TYPE.ToUpper()) select c);
                }
            }
            if (query.VAL4 != null)
            {
                if (query.VAL4 != "")
                {
                    config = (from c in config where (c.VAL4.ToUpper() == query.VAL4.ToUpper()) select c);
                }
            }
            if (query.CON_ID != 0)
            {
                config = (from c in config where (c.CON_ID == query.CON_ID) select c);
            }
            //
            if (!string.IsNullOrEmpty(query.CON_NAME))
            {
                config = (from c in config where (c.CON_NAME == query.CON_NAME) select c);
            }

            List<FbssConfigTBL> result = (from c in config
                                          select new FbssConfigTBL
                                          {
                                              CON_ID = c.CON_ID,
                                              CON_TYPE = c.CON_TYPE,
                                              CON_NAME = c.CON_NAME,
                                              DISPLAY_VAL = c.DISPLAY_VAL,
                                              VAL1 = c.VAL1,
                                              VAL2 = c.VAL2,
                                              VAL3 = c.VAL3,
                                              VAL4 = c.VAL4,
                                              VAL5 = c.VAL5,
                                              ACTIVEFLAG = c.ACTIVEFLAG,
                                              ORDER_BY = (c.ORDER_BY == null) ? 0 : c.ORDER_BY,
                                              DEFAULT_VALUE = c.DEFAULT_VALUE,
                                              CREATED_BY = c.CREATED_BY,
                                              CREATED_DATE = c.CREATED_DATE,
                                              UPDATED_BY = c.UPDATED_BY,
                                              UPDATED_DATE = c.UPDATED_DATE
                                          }).OrderBy(p => p.CON_TYPE)
                        .OrderBy(p => p.ORDER_BY)
                        .OrderBy(p => p.UPDATED_DATE)
                        .ToList();
            return result;
        }

    }
}
