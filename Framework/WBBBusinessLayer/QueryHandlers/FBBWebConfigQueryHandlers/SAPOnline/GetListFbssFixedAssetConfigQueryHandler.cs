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
    public class GetListFbssFixedAssetConfigQueryHandler : IQueryHandler<GetListFbssFixedAssetConfigQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _fixedassetconfig;

        public GetListFbssFixedAssetConfigQueryHandler(ILogger logger, IEntityRepository<FBSS_FIXED_ASSET_CONFIG> fixedassetconfig)
        {
            _logger = logger;
            _fixedassetconfig = fixedassetconfig;
        }

        public List<LovModel> Handle(GetListFbssFixedAssetConfigQuery query)
        {
            string _param1 = query.Param1 ?? null;
            string _paramCondition = "";
            if (query.DDLName == "ProductName") _paramCondition = "EVA5";
            else if (query.DDLName == "CompanyCode") _paramCondition = "COM_CODE";
            else if (query.DDLName == "ServiceName") _paramCondition = "SEVICE_NAME";
            else if (query.DDLName == "Plant") _paramCondition = "COM_CODE";
            else if (query.DDLName == "PHASE") _paramCondition = "PHASE";
            List<LovModel> result = new List<LovModel>();

            try
            {
                var resultFixed = (from f in _fixedassetconfig.Get()
                                   where (f.PROGRAM_NAME == _paramCondition)
                                   select f);

                if (query.DDLName == "ProductName" || query.DDLName == "CompanyCode" || query.DDLName == "ServiceName")
                    result = (from p in resultFixed
                              select new LovModel
                              { LOV_NAME = p.COM_CODE, LOV_VAL1 = p.COM_CODE }).ToList();

                else if (query.DDLName == "Plant")
                {
                    if (string.IsNullOrEmpty(_param1))
                        result = (from p in resultFixed
                                  select new LovModel { LOV_NAME = p.ASSET_CLASS_GI, LOV_VAL1 = p.ASSET_CLASS_GI }).ToList();
                    else
                        result = (from p in resultFixed
                                  where (p.COM_CODE == _param1)
                                  select new LovModel { LOV_NAME = p.ASSET_CLASS_GI, LOV_VAL1 = p.ASSET_CLASS_GI }).ToList();

                }
                else if (query.DDLName == "PHASE")
                {
                    if (string.IsNullOrEmpty(_param1))
                        result = (from p in resultFixed
                                  select new LovModel { LOV_NAME = p.ASSET_CLASS_GI, LOV_VAL1 = p.ASSET_CLASS_GI }).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetListFbssFixedAssetConfigQueryHandler");
            }

            return result;
        }
    }
}
