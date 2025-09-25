using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetCardPortDataQueryHandler : IQueryHandler<GetCardInfoPortPanelDataQuery, List<CoveragePortPanelGrid>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_CARDMODEL> _cardmodel;
        private readonly IEntityRepository<FBB_PORT_INFO> _FBB_PORT_INFO;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;


        public GetCardPortDataQueryHandler(ILogger logger,
                                                IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                IEntityRepository<FBB_CARDMODEL> cardmodel,
            IEntityRepository<FBB_PORT_INFO> FBB_PORT_INFO,
            IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL,
            IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA)
        {
            _logger = logger;
            _dslamInfo = dslamInfo;
            _cardInfo = cardInfo;
            _cardmodel = cardmodel;
            _FBB_PORT_INFO = FBB_PORT_INFO;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
        }


        private static string GetGrade(string value)
        {
            string fag;

            if (value == "Y") fag = "Data";
            else
            {
                fag = "Data+Voice";
            }

            return fag;
        }
        public List<CoveragePortPanelGrid> Handle(GetCardInfoPortPanelDataQuery query)
        {
            var result = new List<CoveragePortPanelGrid>();


            try
            {
                var temp = (from dl in _dslamInfo.Get().ToList()
                            join cardi in _cardInfo.Get().ToList() on dl.DSLAMID equals cardi.DSLAMID
                            join cardm in _cardmodel.Get().ToList() on cardi.CARDMODELID equals cardm.CARDMODELID
                            where dl.ACTIVEFLAG == "Y" && cardi.ACTIVEFLAG == "Y" && dl.DSLAMID == query.DSLAMID
                            orderby cardi.CARDNUMBER ascending
                            select new CoveragePortPanelGrid()
                            {
                                CARDID = cardi.CARDID,
                                CardModel = cardm.MODEL,
                                Number = cardi.CARDNUMBER,
                                CARModelID = cardm.CARDMODELID,
                                CardType = cardm.DATAONLY_FLAG,
                                Reseve = cardi.RESERVE_TECHNOLOGY,
                                Nodeid = dl.NODEID,
                                Building = cardi.BUILDING,

                                HasPort = (from PortInfo in _FBB_PORT_INFO.Get()
                                           where cardi.CARDID == PortInfo.CARDID && PortInfo.ACTIVEFLAG == "Y"
                                           select PortInfo.CARDID).ToList().Count > 0 ? true : false,

                                NodeID = dl.NODEID



                            }
                           );

                result = temp.ToList();

                //var start = 0;
                //foreach (var a in temp)
                //{
                //    a.Number = start;
                //    start++;

                //    result.Add(a);
                //}

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
            }


            return result;
        }
    }
}
