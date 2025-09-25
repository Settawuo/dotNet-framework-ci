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
    public class GetAWCQueryHandler : IQueryHandler<GetAWCQuery, List<AWCSearchlist>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _apcoverage;
        private readonly IEntityRepository<FBB_AP_INFO> _apifo;

        public GetAWCQueryHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> apcoverage, IEntityRepository<FBB_AP_INFO> apifo)
        {
            _logger = logger;
            _apcoverage = apcoverage;
            _apifo = apifo;
        }

        public List<AWCSearchlist> Handle(GetAWCQuery query)
        {
            ////----Count------
            //select count(a.ap_id) Total_AP,count(b.appid) Total_Coverage
            //from fbb_ap_info a,fbb_apcoverage b
            //where a.active_flag='Y'
            //and b.active_flag='Y'
            //and (b.zone='' or b.province='' or b.district='ธัญบุรี' or b.sub_district='' or a.ap_name like '%%')
            //and a.site_id=b.appid;


            //------Search--------
            //select a.ap_id,b.appid,a.ap_name,b.province,b.district,b.sub_district
            //from fbb_ap_info a,fbb_apcoverage b
            //where a.active_flag='Y'
            //and b.active_flag='Y'
            //and b.zone='' or 
            //   (b.province='ปทุมธานี' and b.district='' and b.sub_district='') or 
            //   a.ap_name like ''
            //and a.site_id=b.appid;

            var searchresult = new List<AWCSearchlist>();
            try
            {
                //var ttp = (from c2 in _apcoverage.Get()
                //                             join ifo2 in _apifo.Get() on c2.APPID equals ifo2.SITE_ID
                //                             where ifo2.ACTIVE_FLAG == "Y" && c2.ACTIVE_FLAG == "Y" && c2.ZONE == query.region
                //                             || (c2.PROVINCE == query.province && c2.DISTRICT == query.aumphur && c2.SUB_DISTRICT == query.tumbon)
                //                             || ifo2.AP_NAME.ToLower().Contains(query.APname.ToLower())
                //                             select c2.APPID
                //                            ).Count();
                //var ttc = (from c2 in _apcoverage.Get()
                //           join ifo2 in _apifo.Get() on c2.APPID equals ifo2.SITE_ID
                //           where ifo2.ACTIVE_FLAG == "Y" && c2.ACTIVE_FLAG == "Y" && c2.ZONE == query.region
                //           || (c2.PROVINCE == query.province && c2.DISTRICT == query.aumphur && c2.SUB_DISTRICT == query.tumbon)
                //           || ifo2.AP_NAME.ToLower().Contains(query.APname.ToLower())
                //           select ifo2.AP_ID
                //                             ).Count();
                //var result = (from c in _apcoverage.Get()
                //              join  ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID                             
                //              where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region 
                //              || (c.PROVINCE == query.province && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon) 
                //              || ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                //              select new AWCSearchlist()
                //              {
                //                  province = c.PROVINCE,
                //                  aumphur = c.DISTRICT,
                //                  tumbon = c.SUB_DISTRICT,
                //                  APname = ifo.AP_NAME,
                //                  ap_id = ifo.AP_ID,
                //                  app_id = c.APPID,
                //                  site_id = ifo.SITE_ID,
                //                  TotalAP = ttp,
                //                  TotalCoverage = ttc
                //             });
                ////1
                if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////2
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////3
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////4
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///5
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////6
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////7
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////8
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }

                ////9
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///10
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }///11
                else if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                //12
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///13
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///14
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////15
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }

                ///16
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname != "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                               ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur &&
                                  ifo.AP_NAME.ToLower().Contains(query.APname.ToLower())
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                //17
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////2
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon == "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region.Trim()
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////3
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////4
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.DISTRICT == query.aumphur
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///5
                else if (query.region != "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.SUB_DISTRICT == query.tumbon
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////6
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon == "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////7
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////8
                else if (query.region == "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.PROVINCE == query.province && c.SUB_DISTRICT == query.tumbon
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }

                ////9
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///10
                else if (query.region == "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.DISTRICT == query.aumphur && c.SUB_DISTRICT == query.tumbon
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }///11
                else if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                //12
                else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon == "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.ZONE == query.region && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///13
                else if (query.region == "" && query.province != "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.PROVINCE == query.province && c.DISTRICT == query.aumphur
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ///14
                else if (query.region != "" && query.province == "" && query.aumphur != "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.DISTRICT == query.aumphur
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }
                ////15
                else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon != "" && query.APname == "")
                {
                    var ttp = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province
                               select c.APPID).Distinct().Count();
                    var ttc = (from c in _apcoverage.Get()
                               join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                               where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province
                               select ifo.AP_ID).Count();

                    var result = (from c in _apcoverage.Get()
                                  join ifo in _apifo.Get() on c.APPID equals ifo.SITE_ID
                                  where ifo.ACTIVE_FLAG == "Y" && c.ACTIVE_FLAG == "Y" && c.SUB_DISTRICT == query.tumbon && c.ZONE == query.region && c.PROVINCE == query.province
                                  select new AWCSearchlist()
                                  {
                                      province = c.PROVINCE,
                                      aumphur = c.DISTRICT,
                                      tumbon = c.SUB_DISTRICT,
                                      APname = ifo.AP_NAME,
                                      ap_id = ifo.AP_ID,
                                      app_id = c.APPID,
                                      site_id = ifo.SITE_ID,
                                      TotalAP = ttp,
                                      TotalCoverage = ttc,
                                      updatedate = ifo.UPDATED_DATE
                                  });
                    searchresult = result.ToList();
                }


            }




            catch (Exception)
            {
                throw;
            }

            return searchresult;
            ;
        }
    }
}
