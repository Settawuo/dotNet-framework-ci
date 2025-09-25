using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectAirPanelQueryHandler : IQueryHandler<SelectAirPanelQuery, List<AirPanelModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _FBB_APCOVERAGE;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SelectAirPanelQueryHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> FBB_APCOVERAGE, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
            _FBB_APCOVERAGE = FBB_APCOVERAGE;
        }

        public List<AirPanelModel> Handle(SelectAirPanelQuery query)
        {
            List<ProvincModel> resultprovince;
            List<AumphurModel> resultaumphur;
            List<TumbonModel> resulttumbon;
            List<AirPanelModel> finalresult = new List<AirPanelModel>();

            if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {
                //var resultregion = (from r in _FBB_APCOVERAGE.Get()
                //                    where r.ZONE == query.region
                //                    group r by r.ZONE into g
                //                    orderby g.Key
                //                    select new AirPanelModel
                //                    {
                //                        region = g.Key

                //                    }).ToList();

                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.OrderBy(ss => ss.region).ToList();

                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region && r.ACTIVE_FLAG == "Y"
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.ACTIVE_FLAG == "Y"
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;

            }
            else if (query.region != "" && query.region != "not" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {

                //var resultregion = (from r in _FBB_APCOVERAGE.Get()
                //                    where r.ZONE == query.region
                //                    group r by r.ZONE into g
                //                    orderby g.Key
                //                    select new AirPanelModel
                //                    {
                //                        region = g.Key

                //                    }).ToList();

                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    where r.ZONE == query.region && r.ACTIVE_FLAG == "Y"
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();


                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region && r.ACTIVE_FLAG == "Y"
                                      //&& r.PROVINCE == query.province
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.ACTIVE_FLAG == "Y"
                                         //&& r.DISTRICT == query.aumphur
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;
            }
            else if (query.region != "" && query.province != "" && query.province != "not" && query.aumphur == "" && query.tumbon == "")
            {
                //var resultregion = (from r in _FBB_APCOVERAGE.Get()
                //                    where r.ZONE == query.region
                //                    group r by r.ZONE into g
                //                    orderby g.Key
                //                    select new AirPanelModel
                //                    {
                //                        region = g.Key

                //                    }).ToList();

                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    where r.ZONE == query.region && r.ACTIVE_FLAG == "Y"
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();

                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region
                                      && r.PROVINCE == query.province && r.ACTIVE_FLAG == "Y"
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.ACTIVE_FLAG == "Y"
                                         //&& r.DISTRICT == query.aumphur
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;
            }
            else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon == "")
            {
                //var resultregion = (from r in _FBB_APCOVERAGE.Get()
                //                    where r.ZONE == query.region
                //                    group r by r.ZONE into g
                //                    orderby g.Key
                //                    select new AirPanelModel
                //                    {
                //                        region = g.Key

                //                    }).ToList();

                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    where r.ZONE == query.region && r.ACTIVE_FLAG == "Y"
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();

                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region
                                      && r.PROVINCE == query.province && r.ACTIVE_FLAG == "Y"
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province
                                         && r.DISTRICT == query.aumphur && r.ACTIVE_FLAG == "Y"
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;
            }
            else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon != "")
            {
                //var resultregion = (from r in _FBB_APCOVERAGE.Get()
                //                    where r.ZONE == query.region
                //                    group r by r.ZONE into g
                //                    orderby g.Key
                //                    select new AirPanelModel
                //                    {
                //                        region = g.Key

                //                    }).ToList();

                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    where r.ZONE == query.region && r.ACTIVE_FLAG == "Y"
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();

                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region
                                      && r.PROVINCE == query.province && r.ACTIVE_FLAG == "Y"
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province
                                         && r.DISTRICT == query.aumphur && r.ACTIVE_FLAG == "Y"
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur
                                            && r.SUB_DISTRICT == query.tumbon && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;
            }
            else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon == "not")
            {
                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    where r.ZONE == query.region && r.ACTIVE_FLAG == "Y"
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();

                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region
                                      && r.PROVINCE == query.province && r.ACTIVE_FLAG == "Y"
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.ACTIVE_FLAG == "Y"
                                         //&& r.DISTRICT == query.aumphur
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;
            }
            else if (query.region != "" && query.province == "not" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    where r.ZONE == query.region && r.ACTIVE_FLAG == "Y"
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();

                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region
                                      && r.PROVINCE != "กรุงเทพ" && r.ACTIVE_FLAG == "Y"
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.ACTIVE_FLAG == "Y"
                                         //&& r.DISTRICT == query.aumphur
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;
            }
            else if (query.region == "not" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregion = (from r in _FBB_APCOVERAGE.Get()
                                    join ifo in _FBB_CFG_LOV.Get() on r.ZONE equals ifo.LOV_NAME
                                    where r.ZONE != "BKK" && r.ACTIVE_FLAG == "Y"
                                    select new AirPanelModel
                                    {
                                        region = r.ZONE,
                                        regiondisplay = ifo.LOV_VAL2
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();

                foreach (var regionin in resultregion)
                {
                    resultprovince = (from r in _FBB_APCOVERAGE.Get()
                                      where r.ZONE == regionin.region && r.ACTIVE_FLAG == "Y"
                                      //&& r.PROVINCE != "กรุงเทพ"
                                      group r by r.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        resultaumphur = (from r in _FBB_APCOVERAGE.Get()
                                         where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.ACTIVE_FLAG == "Y"
                                         //&& r.DISTRICT == query.aumphur
                                         group r by r.DISTRICT into g
                                         orderby g.Key
                                         select new AumphurModel
                                         {
                                             aumphur = g.Key

                                         }).ToList();
                        foreach (var aumphurin in resultaumphur)
                        {
                            resulttumbon = (from r in _FBB_APCOVERAGE.Get()
                                            where r.ZONE == regionin.region && r.PROVINCE == provincein.province && r.DISTRICT == aumphurin.aumphur && r.ACTIVE_FLAG == "Y"
                                            select new TumbonModel
                                            {
                                                tumbon = r.SUB_DISTRICT,
                                                createdate = r.CREATED_DATE

                                            }).OrderByDescending(a => a.createdate).ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphur;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }


                return finalresult;
            }


            return new List<AirPanelModel>();
        }
    }
}
