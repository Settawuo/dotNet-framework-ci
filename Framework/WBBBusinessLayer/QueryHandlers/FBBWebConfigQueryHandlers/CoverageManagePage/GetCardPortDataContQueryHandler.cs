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
    public class GetCardPortDataContQueryHandler : IQueryHandler<GetCardInfoPortDataPanelBQuery, List<CoveragePortPanel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_PORT_INFO> _FBB_PORT_INFOService;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV_Service;


        public GetCardPortDataContQueryHandler(ILogger logger,
                                                IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV_Service,
                                                IEntityRepository<FBB_PORT_INFO> FBB_PORT_INFOService)
        {
            _logger = logger;
            _FBB_PORT_INFOService = FBB_PORT_INFOService;
            _FBB_CFG_LOV_Service = FBB_CFG_LOV_Service;

        }



        public List<CoveragePortPanel> Handle(GetCardInfoPortDataPanelBQuery query)
        {
            var result = new List<CoveragePortPanel>();


            try
            {




                var temp = (from fbb_cvg in _FBB_CFG_LOV_Service.Get().ToList()
                            join fbb_port in _FBB_PORT_INFOService.Get().ToList() on fbb_cvg.LOV_VAL1 equals fbb_port.PORTSTATUSID.ToString()

                            where fbb_port.ACTIVEFLAG == "Y" && fbb_port.CARDID == query.CardID && fbb_cvg.LOV_TYPE == "PORTSTATUS"
                            orderby fbb_port.PORTNUMBER
                            select new CoveragePortPanel()
                            {
                                PortNumber = fbb_port.PORTNUMBER,
                                PortStatus = fbb_cvg.DISPLAY_VAL,
                                PortType = fbb_port.PORTTYPE,
                                PortID = fbb_port.PORTID

                            }
                            );

                result = temp.ToList();
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
            }


            return null;
        }
    }
}
