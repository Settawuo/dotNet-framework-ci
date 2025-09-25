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
    public class GetAWCEditQueryHandler : IQueryHandler<GetAWCEditQuery, AWCinformation>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _coverage;
        private readonly IEntityRepository<FBB_AP_INFO> _ifo;

        public GetAWCEditQueryHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> coverage, IEntityRepository<FBB_AP_INFO> ifo)
        {
            _logger = logger;
            _coverage = coverage;
            _ifo = ifo;
        }

        public AWCinformation Handle(GetAWCEditQuery query)
        {
            var awcedit = new AWCinformation();
            var id = Convert.ToDecimal(query.site_id);
            if (query.site_id != "")
            {
                List<AWCconfig> listapconfig;
                var qq = (from info in _ifo.Get()
                          where info.SITE_ID == id && info.ACTIVE_FLAG == "Y"
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
                              implement_date = info.IMPLEMENT_DATE,//String.Format("{dd/MM/yyyy}", info.IMPLEMENT_DATE),
                              implement_phase = info.IMPLEMENT_PHASE,
                              po_number = info.PO_NUMBER,
                              ap_company = info.AP_COMPANY,
                              ap_lot = info.AP_LOT,
                              onservice_date = info.ON_SERVICE_DATE//String.Format("{dd/MM/yyyy}", info.ON_SERVICE_DATE)

                          });
                listapconfig = qq.ToList();

                var tt = (from u in _coverage.Get()
                          where u.APPID == id && u.ACTIVE_FLAG == "Y"
                          select new AWCinformation()
                          {
                              Base_L2 = u.BASEL2,
                              Site_Name = u.SITENAME,
                              Aumphur = u.DISTRICT,
                              Tumbon = u.SUB_DISTRICT,
                              Province = u.PROVINCE,
                              Zone = u.ZONE,
                              Lat = u.LAT,
                              Lon = u.LNG,
                              ACTIVE_FLAGAPPC = u.ACTIVE_FLAG,
                              APP_ID = u.APPID,
                              ap_comment = u.AP_COMMENT,
                              gateway = u.GATEWAY,
                              VLAN = u.VLAN,
                              tower_height = u.TOWER_HEIGHT,
                              tower_type = u.TOWER_TYPE,
                              subnet_mask_26 = u.SUBNET_MASK_26

                          }).ToList();

                awcedit = tt[0];
                awcedit.oldmodelpage1 = query.oldmodelpage1;
                awcedit.apmodel = listapconfig;
                return awcedit;
            }
            return awcedit;
        }
    }
}
