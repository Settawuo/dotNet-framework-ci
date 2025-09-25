using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWCconfigQueryHandler : IQueryHandler<GetAWCconfigQuey, List<AWCconfig>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_AP_INFO> _apifo;

        public GetAWCconfigQueryHandler(ILogger logger, IEntityRepository<FBB_AP_INFO> apifo)
        {
            _logger = logger;
            _apifo = apifo;
        }

        public List<AWCconfig> Handle(GetAWCconfigQuey query)
        {
            var result = new List<AWCconfig>();
            try
            {
                var qq = (from info in _apifo.Get()
                          select new AWCconfig()
                          {
                              AP_Name = info.AP_NAME,
                              AP_ID = info.AP_ID,
                              Sector = info.SECTOR,
                              ACTIVE_FLAGINFO = info.ACTIVE_FLAG,
                              Site_id = info.SITE_ID,
                              updatedate = info.UPDATED_DATE,
                              user = info.UPDATED_BY,
                              ip_address = info.IP_ADDRESS,
                              status = info.STATUS,
                              implement_phase = info.IMPLEMENT_PHASE,
                              implement_date = info.IMPLEMENT_DATE,
                              onservice_date = info.ON_SERVICE_DATE,
                              po_number = info.PO_NUMBER,
                              ap_lot = info.AP_LOT,
                              ap_company = info.AP_COMPANY
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
