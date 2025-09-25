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
    public class GetVendorCostInstallationQueryHandler : IQueryHandler<GetVendorCostInstallationQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _vendor;
        private readonly IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> _vendorfull;

        public GetVendorCostInstallationQueryHandler(ILogger logger
                                                    , IEntityRepository<FBSS_FIXED_ASSET_CONFIG> vendor
                                                    , IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> vendorfull)
        {
            _logger = logger;
            _vendor = vendor;
            _vendorfull = vendorfull;
        }

        public List<LovModel> Handle(GetVendorCostInstallationQuery query)
        {
            List<LovModel> result = new List<LovModel>();
            try
            {
                // "Insert" => Full Vendor List , "Update", other => Vendor List
                //if (query.VENDER_MODE == "Full")
                //{

                var resultVendorfull = (from v in _vendorfull.Get() select v);

                result = (from v in resultVendorfull
                          select new LovModel
                          {
                              LOV_NAME = v.SUB_CONTRACTOR_NAME_EN,
                              LOV_VAL1 = v.STORAGE_LOCATION
                          }).ToList();
                //}
                //else
                //{
                //var resultVendor = (from v in vendorfull.Get()
                //                        where (
                //                            v.PROGRAM_NAME == "SUB_CONTACT"
                //                            &&
                //                            v.ASSET_CLASS_GI != "CS AIRNET"
                //                        )
                //                        select v);

                //result = (from v in resultVendor
                //          select new LovModel
                //          {
                //              LOV_NAME = v.ASSET_CLASS_GI,
                //              LOV_VAL1 = v.COM_CODE
                //          }).ToList();
                //}
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetVendorCostInstallationQueryHandler");
            }

            return result;
        }
    }
}
