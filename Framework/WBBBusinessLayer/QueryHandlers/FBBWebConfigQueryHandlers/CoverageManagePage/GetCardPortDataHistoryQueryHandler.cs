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
    public class GetCardPortDataHistoryQueryHandler : IQueryHandler<GetCardInfoPortDataPaneHistorylBQuery, List<portPanelHittory>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_PORT_NOTE> _FBB_PORT_NOTE_Service;


        public GetCardPortDataHistoryQueryHandler(ILogger logger,
                                               IEntityRepository<FBB_PORT_NOTE> FBB_PORT_NOTE_Service
                                             )
        {
            _logger = logger;
            _FBB_PORT_NOTE_Service = FBB_PORT_NOTE_Service;

        }



        public List<portPanelHittory> Handle(GetCardInfoPortDataPaneHistorylBQuery query)
        {
            var result = new List<portPanelHittory>();


            try
            {



                var temp = (from por_fbb in _FBB_PORT_NOTE_Service.Get()

                            where por_fbb.PORTID == query.PorID
                            select new portPanelHittory()
                            {
                                Datetime = por_fbb.CREATED_DATE,
                                User = por_fbb.CREATED_BY,
                                DESC = por_fbb.NOTE

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
