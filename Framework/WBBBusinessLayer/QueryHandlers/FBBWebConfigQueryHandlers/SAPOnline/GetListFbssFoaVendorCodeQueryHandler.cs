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
    public class GetListFbssFoaVendorCodeQueryHandler : IQueryHandler<GetListFbssFoaVendorCodeQuery, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSS_FOA_VENDOR_CODE> _fbssfoavendorcode;

        public GetListFbssFoaVendorCodeQueryHandler(ILogger logger, IEntityRepository<FBSS_FOA_VENDOR_CODE> fbssfoavendorcode)
        {
            _logger = logger;
            _fbssfoavendorcode = fbssfoavendorcode;
        }

        public List<LovModel> Handle(GetListFbssFoaVendorCodeQuery query)
        {
            List<LovModel> result = new List<LovModel>();

            try
            {
                var resultFixed = (from f in _fbssfoavendorcode.Get()
                                   select f).Distinct();

                result = (from p in resultFixed
                          select new LovModel { LOV_NAME = p.VENDOR_CODE, LOV_VAL1 = p.VENDOR_CODE }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetListFbssFoaVendorCodeQueryHandler");
            }

            return result;
        }
    }
}
