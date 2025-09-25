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
    public class GetAWCAllQueryHandler : IQueryHandler<GetAWCAllQuery, List<AWCexportResultlist>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _apcoverage;
        private readonly IEntityRepository<FBB_AP_INFO> _apifo;

        public GetAWCAllQueryHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> apcoverage, IEntityRepository<FBB_AP_INFO> apifo)
        {
            _logger = logger;
            _apcoverage = apcoverage;
            _apifo = apifo;
        }

        public List<AWCexportResultlist> Handle(GetAWCAllQuery query)
        {
            var searchresult = new List<AWCexportResultlist>();
            try
            {
                if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT

                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////2
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT

                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////3
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT

                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////4
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///5
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////6
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////7
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////8
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }

                ////9
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///10
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }///11
                else if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                //12
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///13
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///14
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////15
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }

                ///16
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                //17
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////2
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon == "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region.Trim()
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////3
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////4
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///5
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////6
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname == "")
                {

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////7
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////8
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }

                ////9
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///10
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }///11
                else if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                //12
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///13
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ///14
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
                ////15
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {


                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province
                                  select new AWCexportResultlist()
                                  {
                                      AP_Name = ifo.AP_NAME,
                                      Sector = ifo.SECTOR,
                                      IP_Address = ifo.IP_ADDRESS,
                                      Status = ifo.STATUS,
                                      Implement_Phase = ifo.IMPLEMENT_PHASE,
                                      Implement_date = ifo.IMPLEMENT_DATE,
                                      On_service_date = ifo.ON_SERVICE_DATE,
                                      PO_Number = ifo.PO_NUMBER,
                                      AP_Company = ifo.AP_COMPANY,
                                      AP_Lot = ifo.AP_LOT,
                                      Base_L2 = c.BASEL2,
                                      Site_Name = c.SITENAME,
                                      Zone = c.ZONE,
                                      Province = c.PROVINCE,
                                      Aumphur = c.DISTRICT,
                                      Tumbon = c.SUB_DISTRICT,
                                      Lat = c.LAT,
                                      Lon = c.LNG,
                                      VLAN = c.VLAN,
                                      subnet_mask_26 = c.SUBNET_MASK_26,
                                      gateway = c.GATEWAY,
                                      ap_comment = c.AP_COMMENT,
                                      tower_type = c.TOWER_TYPE,
                                      tower_height = c.TOWER_HEIGHT
                                  }).OrderBy(c => c.AP_Name);
                    searchresult = result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return searchresult;
        }
    }
}
