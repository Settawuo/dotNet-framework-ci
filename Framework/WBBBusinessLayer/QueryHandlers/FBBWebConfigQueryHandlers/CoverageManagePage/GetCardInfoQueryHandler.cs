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
    public class GetCardInfoQueryHandler : IQueryHandler<GetCardInfoQuery, List<CardPanel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_CARDMODEL> _cardmodel;

        public GetCardInfoQueryHandler(ILogger logger,
                                                IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                IEntityRepository<FBB_CARDMODEL> cardmodel)
        {
            _logger = logger;
            _dslamInfo = dslamInfo;
            _cardInfo = cardInfo;
            _cardmodel = cardmodel;
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
        public List<CardPanel> Handle(GetCardInfoQuery query)
        {
            var result = new List<CardPanel>();


            try
            {

                var temp = (from dl in _dslamInfo.Get()
                            join cardi in _cardInfo.Get() on dl.DSLAMID equals cardi.DSLAMID
                            join cardm in _cardmodel.Get() on cardi.CARDMODELID equals cardm.CARDMODELID
                            where dl.ACTIVEFLAG == "Y" && cardi.ACTIVEFLAG == "Y" && dl.DSLAMID == query.DSLAMID
                            select new CardPanel()
                            {
                                Number = cardi.CARDNUMBER,
                                NodeId = dl.NODEID,
                                CardId = cardi.CARDID,
                                Model = cardm.MODEL,
                                Reserve = cardi.RESERVE_TECHNOLOGY,
                                CardType = GetGrade(cardm.DATAONLY_FLAG)
                            }
                           );

                result = temp.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
            }


            return result;
        }
    }
}
