using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectModelNameHandler : IQueryHandler<SelectModelNameQuery, List<ModelNameCardModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CARDMODEL> _FBB_CardModelService;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODELService;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOVLService;
        private readonly IEntityRepository<FBB_CARD_INFO> _FBB_FBB_CARD_INFO_Service;

        public SelectModelNameHandler(ILogger logger, IEntityRepository<FBB_CARDMODEL> FBB_CARDMODEL,
            IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODELService,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOVLService,
            IEntityRepository<FBB_CARD_INFO> FBB_FBB_CARD_INFO_Service)
        {
            _logger = logger;
            _FBB_CardModelService = FBB_CARDMODEL;
            _FBB_DSLAMMODELService = FBB_DSLAMMODELService;
            _FBB_CFG_LOVLService = FBB_CFG_LOVLService;
            _FBB_FBB_CARD_INFO_Service = FBB_FBB_CARD_INFO_Service;
        }


        public List<ModelNameCardModel> Handle(SelectModelNameQuery query)
        {

            if (query.DlasmBran == "DSALAM")
            {

                return (from r in _FBB_DSLAMMODELService.Get()
                        where r.ACTIVEFLAG == "Y"
                        orderby r.BRAND descending



                        select new ModelNameCardModel
                        {
                            Band = r.BRAND

                        }).Distinct().ToList();


            }
            else if (query.Reserve == "Reserve")
            {



                //select cf.lov_name,cf.display_val  from FBB_CFG_LOV cf
                //where cf.lov_type like '%RESERVE_TECH%'
                //and cf.activeflag = 'Y'
                //order by cf.order_by;
                return (from r in _FBB_CFG_LOVLService.Get()
                        where r.ACTIVEFLAG == "Y" && r.LOV_TYPE == "RESERVE_TECH"
                        orderby r.ORDER_BY descending



                        select new ModelNameCardModel

                        {
                            DISPLAY_VAL = r.LOV_NAME,
                            reserve = r.LOV_NAME

                        }).Distinct().ToList();

            }

            else if (query.Reserve == "MaxCardModel")
            {



                //select cf.lov_name,cf.display_val  from FBB_CFG_LOV cf
                //where cf.lov_type like '%RESERVE_TECH%'
                //and cf.activeflag = 'Y'
                //order by cf.order_by;




            }

            else if (query.CardModel == "ModelNmaeSearchID")
            {

                return (from r in _FBB_CardModelService.Get()
                        where r.ACTIVEFLAG == "Y" && query.CardModelidID == r.CARDMODELID
                        orderby r.MODEL descending



                        select new ModelNameCardModel
                        {

                            CardType = r.DATAONLY_FLAG

                        }).Distinct().ToList();



            }
            else if (query.CardModel == "ModelNmae")
            {

                return (from r in _FBB_CardModelService.Get()
                        where r.ACTIVEFLAG == "Y"
                        orderby r.MODEL descending



                        select new ModelNameCardModel
                        {
                            Model = r.MODEL,
                            CardType = r.DATAONLY_FLAG,
                            CardModelID = r.CARDMODELID
                        }).Distinct().ToList();



            }


            else if (query.CardModel == "ModelDropdown")
            {

                return (from r in _FBB_CardModelService.Get()
                        where r.ACTIVEFLAG == "Y"
                        orderby r.MODEL descending



                        select new ModelNameCardModel
                        {
                            Model = r.MODEL,

                        }).Distinct().ToList();



            }



            return (from r in _FBB_CardModelService.Get()
                    where r.ACTIVEFLAG == "Y"
                    orderby r.BRAND descending



                    select new ModelNameCardModel
                    {
                        Band = r.BRAND

                    }).Distinct().ToList();





        }
    }
}
