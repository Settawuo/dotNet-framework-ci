using AIRNETEntity.Models;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetServicePlayboxHandler : IQueryHandler<GetServicePlayboxQuery, GetServicePlayboxModels>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> _AIR_FBB_NEW_PACKAGE_MASTER;
        private readonly IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> _AIR_SFF_SERVICE_CODE;
        private readonly IAirNetEntityRepository<AIR_FBB_PACKAGE_SERVICE_MASTER> _AIR_FBB_PACKAGE_SERVICE_MASTER;
        private readonly IAirNetEntityRepository<AIR_FBB_PACKAGE_MAPPING> _AIR_FBB_PACKAGE_MAPPING;

        public GetServicePlayboxHandler(ILogger logger, IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> AIR_FBB_NEW_PACKAGE_MASTER,
            IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> AIR_SFF_SERVICE_CODE,
            IAirNetEntityRepository<AIR_FBB_PACKAGE_SERVICE_MASTER> AIR_FBB_PACKAGE_SERVICE_MASTER,
            IAirNetEntityRepository<AIR_FBB_PACKAGE_MAPPING> AIR_FBB_PACKAGE_MAPPING)
        {
            _logger = logger;
            _AIR_FBB_NEW_PACKAGE_MASTER = AIR_FBB_NEW_PACKAGE_MASTER;
            _AIR_SFF_SERVICE_CODE = AIR_SFF_SERVICE_CODE;
            _AIR_FBB_PACKAGE_SERVICE_MASTER = AIR_FBB_PACKAGE_SERVICE_MASTER;
            _AIR_FBB_PACKAGE_MAPPING = AIR_FBB_PACKAGE_MAPPING;
        }

        public GetServicePlayboxModels Handle(GetServicePlayboxQuery query)
        {
            var result = GetServicePlayboxHelper.GetServicePlaybox(_logger, _AIR_FBB_NEW_PACKAGE_MASTER, _AIR_SFF_SERVICE_CODE, _AIR_FBB_PACKAGE_SERVICE_MASTER, _AIR_FBB_PACKAGE_MAPPING, query);
            return result;
        }
    }

    public static class GetServicePlayboxHelper
    {
        public static GetServicePlayboxModels GetServicePlaybox(ILogger logger,
            IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> AIR_FBB_NEW_PACKAGE_MASTER,
            IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> AIR_SFF_SERVICE_CODE,
            IAirNetEntityRepository<AIR_FBB_PACKAGE_SERVICE_MASTER> AIR_FBB_PACKAGE_SERVICE_MASTER,
            IAirNetEntityRepository<AIR_FBB_PACKAGE_MAPPING> AIR_FBB_PACKAGE_MAPPING,
            GetServicePlayboxQuery query)
        {
            var PRODUCT_CLASS = new string[] { "On-Top" };

            var mappingCode = from DD in AIR_FBB_PACKAGE_MAPPING.Get()
                              where DD.SFF_PROMOTION_CODE == query.SFF_PROMOTION_CODE
                              select DD.MAPPING_CODE;

            var sffPromoCode = from P in AIR_FBB_NEW_PACKAGE_MASTER.Get()
                               join DD in AIR_FBB_PACKAGE_MAPPING.Get()
                               on P.SFF_PROMOTION_CODE equals DD.SFF_PROMOTION_CODE
                               where DD.EFFECTIVE_DTM <= DateTime.Now &&
                               DD.EXPIRE_DTM >= DateTime.Now &&
                               mappingCode.Contains(DD.MAPPING_CODE) &&
                               P.PACKAGE_TYPE_DESC == "Ontop PBOX" &&
                               !(PRODUCT_CLASS.Contains((P.SFF_PRODUCT_CLASS ?? "On-Top Extra")))
                               select P.SFF_PROMOTION_CODE;

            var a = from M in AIR_FBB_PACKAGE_SERVICE_MASTER.Get()
                    join SS in AIR_SFF_SERVICE_CODE.Get()
                    on M.PACKAGE_SERVICE_NAME equals SS.PRODUCT_NAME
                    join P in AIR_FBB_NEW_PACKAGE_MASTER.Get()
                    on M.PACKAGE_SERVICE_CODE equals P.PACKAGE_SERVICE_CODE
                    where P.PACKAGE_TYPE_DESC == "Ontop PBOX" &&
                    sffPromoCode.Contains(P.SFF_PROMOTION_CODE) &&
                    !(PRODUCT_CLASS.Contains((P.SFF_PRODUCT_CLASS ?? "On-Top Extra"))) &&
                    SS.SERVICE_CODE == query.SERVICE_CODE

                    select new GetServicePlayboxModels
                    {
                        SFF_PROMOTION_CODE = P.SFF_PROMOTION_CODE,
                        SERVICE_PLAYBOX = SS.SERVICE_CODE,
                    };

            //and p.sff_promotion_code IN()

            if (a.Any())
                return a.FirstOrDefault();


            return new GetServicePlayboxModels();
        }
    }
}
