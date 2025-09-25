using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetDSLAMInfoQueryHandler : IQueryHandler<GetDSLAMInfoQuery, List<DslamPanel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfo;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageAreaRelation;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        //private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetDSLAMInfoQueryHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                           IEntityRepository<FBB_PORT_INFO> portInfo,
                                                            IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                            IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageAreaRelation,
                                                            IEntityRepository<FBB_COVERAGEAREA> coverageArea)
        {
            _logger = logger;
            _dslamInfo = dslamInfo;
            _portInfo = portInfo;
            _cardInfo = cardInfo;
            _coverageAreaRelation = coverageAreaRelation;
            _coverageArea = coverageArea;
        }



        //public List<DslamPanel> Handle(GetDSLAMInfoQuery query)
        //{
        //    List<DslamPanel> lstdslampanel = new List<DslamPanel>();
        //    try
        //    {
        //        #region sql
        //        //select r.cvrrelationid,c.cvrid,di.nodeid,di.dslamnumber,c.buildingcode,r.towername_en
        //        //,(select count(p.portid)
        //        //from FBB_DSLAM_INFO D, 
        //        //Fbb_Card_Info CI,FBB_PORT_INFO p 
        //        //where  d.dslamid =ci.dslamid
        //        //and  ci.cardid = p.cardid 
        //        //and d.activeflag = 'Y' 
        //        //and CI.ACTIVEFLAG = 'Y' 
        //        //and p.activeflag = 'Y'
        //        //and d.dslamid = di.dslamid) as CurrentPort
        //        //from 
        //        //fbb_coveragearea_relation r ,
        //        //FBB_COVERAGEAREA c,
        //        //Fbb_Dslam_Info di
        //        //where c.cvrid = r.cvrid
        //        //and r.dslamid = di.dslamid
        //        //and di.activeflag = 'Y'
        //        //and c.cvrid = @CVRID
        //        #endregion


        //        decimal aa = System.Convert.ToDecimal(query.CVRID);
        //        var temp = (from r in _coverageAreaRelation.Get()
        //                    join c in _coverageArea.Get() on r.CVRID equals c.CVRID
        //                    join d in _dslamInfo.Get() on r.DSLAMID equals d.DSLAMID
        //                    where d.ACTIVEFLAG.Equals("Y")
        //                    && c.CVRID == aa
        //                    select new
        //                    {                               
        //                        d.NODEID,
        //                        d.DSLAMNUMBER,
        //                        c.BUILDINGCODE,
        //                        r.TOWERNAME_EN,
        //                        d.DSLAMID
        //                    }).ToList();

        //        var x = from r in temp
        //                where r.

        //        var tt = temp.Select(o => o.DSLAMID);
        //        var temp2 = (from c in _cardInfo.Get()
        //                     join d in _dslamInfo.Get() on c.DSLAMID equals d.DSLAMID
        //                     join p in _portInfo.Get() on c.CARDID equals p.CARDID
        //                     where c.ACTIVEFLAG.Equals("Y")
        //                     && p.ACTIVEFLAG.Equals("Y")
        //                     && d.ACTIVEFLAG.Equals("Y")
        //                     && tt.Contains(d.DSLAMID)
        //                     group new { d } by new
        //                     {
        //                         d.DSLAMID
        //                     } into grp
        //                     select new { ID = grp.Key.DSLAMID, Count = grp.Count() }).ToList();

        //        var temp3 = from a in temp
        //                  join b in temp2 on a.DSLAMID equals b.ID
        //                    select new DslamPanel()
        //                  {


        //                      NodeId =  a.NODEID,
        //                      Number = Convert.ToInt32(a.DSLAMNUMBER),
        //                      BuildingCode = a.BUILDINGCODE,
        //                      BuildingUse = a.TOWERNAME_EN,
        //                      CurrentPort =    b.Count

        //                  };

        //        //var subdata = from c in _cardInfo.Get()
        //        //                  join d in _dslamInfo.Get() on c.DSLAMID equals d.DSLAMID
        //        //                  join p in _portInfo.Get() on c.CARDID equals p.CARDID
        //        //                  where c.ACTIVEFLAG.Equals("Y")
        //        //                  && p.ACTIVEFLAG.Equals("Y")
        //        //                  && d.ACTIVEFLAG.Equals("Y")
        //        //                  select new { p.PORTID };
        //        //int CurrentPort = subdata.Any() ? subdata.Count(): 0;

        //        //var data = from r in _coverageAreaRelation.Get()
        //        //           join c in _coverageArea.Get() on r.CVRID equals c.CVRID
        //        //           join d in _dslamInfo.Get() on r.DSLAMID equals d.DSLAMID
        //        //           where d.ACTIVEFLAG.Equals("Y")
        //        //           && c.CVRID == Int32.Parse(query.CVRID)
        //        //           select new
        //        //            {
        //        //                r.CVRRELATIONID,
        //        //                c.CVRID,
        //        //                d.NODEID,
        //        //                d.DSLAMNUMBER,
        //        //                c.BUILDINGCODE,
        //        //                r.TOWERNAME_EN,
        //        //                CurrentPort
        //        //            };
        //        //var returnData = data.Select(l => new DslamPanel
        //        //{

        //        // NodeId            = l.NODEID,
        //        // Number            = Convert.ToInt32(l.DSLAMNUMBER),
        //        // CurrentPort       = l.CurrentPort,
        //        // //Code              = l.BUILDINGCODE,
        //        // BuildingCode      = l.BUILDINGCODE,
        //        // BuildingUse       = l.TOWERNAME_EN
        //        // //DLRuningNumber    = l.
        //        // //MC                = l
        //        // //IPRANPort         = l.
        //        // //RegionDSLAM       = l.
        //        // //Lot               = l.
        //        // //DSLAMModel        = l.
        //        // //Brand              = l

        //        //});
        //        lstdslampanel = temp3.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.GetErrorMessage());
        //    }
        //    return lstdslampanel;
        //}

        public List<DslamPanel> Handle(GetDSLAMInfoQuery query)
        {
            #region sql
            //select r.cvrrelationid,co.cvrid,co.REGION_CODE,di.dslamid,di.nodeid,di.dslamnumber,co.buildingcode,r.towername_en
            //,(select count(p.portid)
            //from FBB_DSLAM_INFO D, 
            //Fbb_Card_Info CI,FBB_PORT_INFO p 
            //where  d.dslamid =ci.dslamid
            //and  ci.cardid = p.cardid 
            //and d.activeflag = 'Y' 
            //and CI.ACTIVEFLAG = 'Y' 
            //and p.activeflag = 'Y'
            //and d.dslamid = di.dslamid) as CurrentPort
            //from 
            //fbb_coveragearea_relation r ,
            //FBB_COVERAGEAREA co,
            //Fbb_Dslam_Info di
            //where co.cvrid = r.cvrid
            //and r.dslamid = di.dslamid
            //and di.activeflag = 'Y'
            //and co.cvrid =16
            #endregion
            var mainQuery = from co in _coverageArea.Get()
                            join r in _coverageAreaRelation.Get() on co.CVRID equals r.CVRID
                            join di in _dslamInfo.Get() on r.DSLAMID equals di.DSLAMID
                            where co.CVRID == query.CVRID && di.ACTIVEFLAG == "Y" && r.ACTIVEFLAG == "Y" && co.ACTIVEFLAG == "Y"
                            select new
                            {
                                co,
                                r,
                                di
                            };

            var subQuery = from d in _dslamInfo.Get()
                           join ci in _cardInfo.Get() on d.DSLAMID equals ci.DSLAMID
                           join p in _portInfo.Get() on ci.CARDID equals p.CARDID
                           where d.ACTIVEFLAG == "Y" && ci.ACTIVEFLAG == "Y" && p.ACTIVEFLAG == "Y"
                           select new { d, p };

            var result = from m in mainQuery
                         orderby m.di.DSLAMNUMBER
                         select new DslamPanel
                         {
                             //m.r.CVRRELATIONID,
                             //m.co.CVRID,
                             //m.co.REGION_CODE,
                             //m.di.DSLAMID,
                             //m.di.NODEID,
                             //m.di.DSLAMNUMBER,
                             //m.co.BUILDINGCODE,
                             //m.r.TOWERNAME_EN,
                             DSLAMID = m.di.DSLAMID,
                             Number = m.di.DSLAMNUMBER,
                             NodeId = m.di.NODEID,
                             RegionDSLAM = m.co.REGION_CODE,
                             BuildingUse = m.r.TOWERNAME_EN,
                             BuildingCode = m.co.BUILDINGCODE,
                             CurrentPort = (from s in subQuery where s.d.DSLAMID == m.di.DSLAMID select s.p.PORTID).Count(),
                             CVRRelationID = m.r.CVRRELATIONID
                         };

            return result.ToList();
        }
    }
}
