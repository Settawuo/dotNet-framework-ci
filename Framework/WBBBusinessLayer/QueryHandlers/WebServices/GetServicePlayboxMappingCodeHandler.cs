using AIRNETEntity.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetServicePlayboxMappingCodeHandler : IQueryHandler<GetServicePlayboxMappingCodeQuery, GetServicePlayboxMappingCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        private readonly IAirNetEntityRepository<AIR_FBB_PACKAGE_SERVICE_MASTER> _AIR_FBB_PACKAGE_SERVICE_MASTER;
        private readonly IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> _AIR_FBB_NEW_PACKAGE_MASTER;
        private readonly IAirNetEntityRepository<AIR_FBB_PACKAGE_MAPPING> _AIR_FBB_PACKAGE_MAPPING;
        private readonly IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> _AIR_SFF_SERVICE_CODE;

        public GetServicePlayboxMappingCodeHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IAirNetEntityRepository<AIR_FBB_PACKAGE_SERVICE_MASTER> AIR_FBB_PACKAGE_SERVICE_MASTER,
            IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> AIR_FBB_NEW_PACKAGE_MASTER,
            IAirNetEntityRepository<AIR_FBB_PACKAGE_MAPPING> AIR_FBB_PACKAGE_MAPPING,
            IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> AIR_SFF_SERVICE_CODE)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _AIR_FBB_PACKAGE_SERVICE_MASTER = AIR_FBB_PACKAGE_SERVICE_MASTER;
            _AIR_FBB_NEW_PACKAGE_MASTER = AIR_FBB_NEW_PACKAGE_MASTER;
            _AIR_FBB_PACKAGE_MAPPING = AIR_FBB_PACKAGE_MAPPING;
            _AIR_SFF_SERVICE_CODE = AIR_SFF_SERVICE_CODE;
        }

        public GetServicePlayboxMappingCodeModel Handle(GetServicePlayboxMappingCodeQuery query)
        {
            var result = new GetServicePlayboxMappingCodeModel
            {
                RETURN_CODE = 1,
                RETURN_DESC = "BEGIN",
                GOTO_TOPUP = "N",
                LIST_DATA = new List<ServicePlayboxMappingCodeModel>()
            };

            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.INTERNET_NO, "GET_PLAYBOX_MAPPING_CODE", "GetServicePlayboxMappingCodeHandler", query.INTERNET_NO, query.INTERNET_NO, "WEB");
            try
            {
                if (query.HAVE_PLAY_FLAG != "Y")
                {
                    result.LIST_DATA = GetServicePlayboxMappingCodeHelper.GetServicePlayboxMappingCode(_logger, _AIR_FBB_NEW_PACKAGE_MASTER, _AIR_SFF_SERVICE_CODE, _AIR_FBB_PACKAGE_SERVICE_MASTER, _AIR_FBB_PACKAGE_MAPPING, query);

                    if (result.LIST_DATA.Any())
                    {
                        var rowData = result.LIST_DATA.FirstOrDefault();
                        if (rowData.PRICE_CHARGE <= 0)
                        {
                            result.GOTO_TOPUP = "Y";
                        }
                    }
                }

                result.RETURN_CODE = 0;
                result.RETURN_DESC = "Success";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                result = new GetServicePlayboxMappingCodeModel
                {
                    RETURN_CODE = -1,
                    RETURN_DESC = ex.Message,
                    GOTO_TOPUP = "N",
                    LIST_DATA = new List<ServicePlayboxMappingCodeModel>()
                };

                if (log != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return result;
        }

        public static class GetServicePlayboxMappingCodeHelper
        {
            public static List<ServicePlayboxMappingCodeModel> GetServicePlayboxMappingCode(ILogger logger,
                IAirNetEntityRepository<AIR_FBB_NEW_PACKAGE_MASTER> AIR_FBB_NEW_PACKAGE_MASTER,
                IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> AIR_SFF_SERVICE_CODE,
                IAirNetEntityRepository<AIR_FBB_PACKAGE_SERVICE_MASTER> AIR_FBB_PACKAGE_SERVICE_MASTER,
                IAirNetEntityRepository<AIR_FBB_PACKAGE_MAPPING> AIR_FBB_PACKAGE_MAPPING,
                GetServicePlayboxMappingCodeQuery query)
            {
                var PRODUCT_CLASS = new string[] { "On-Top" };

                var mappingCode = from DD in AIR_FBB_PACKAGE_MAPPING.Get()
                                  where DD.SFF_PROMOTION_CODE == query.SFF_PROMOTION_CODE
                                  select DD.MAPPING_CODE;

                var sffPromoCode = from P in AIR_FBB_NEW_PACKAGE_MASTER.Get()
                                   join DD in AIR_FBB_PACKAGE_MAPPING.Get()
                                   on P.SFF_PROMOTION_CODE equals DD.SFF_PROMOTION_CODE
                                   where mappingCode.Contains(DD.MAPPING_CODE) &&
                                   DD.EFFECTIVE_DTM <= DateTime.Now &&
                                   DD.EXPIRE_DTM >= DateTime.Now &&
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
                        SS.SERVICE_CODE == "P15053283"

                        select new ServicePlayboxMappingCodeModel
                        {
                            SFF_PROMOTION_CODE = P.SFF_PROMOTION_CODE,
                            SFF_PRODUCT_NAME = P.SFF_PRODUCT_NAME,
                            SERVICE_PLAYBOX = SS.SERVICE_CODE,
                            PRICE_CHARGE = P.PRICE_CHARGE,
                            SFF_PRODUCT_CLASS = P.SFF_PRODUCT_CLASS
                        };

                if (a.Any())
                {
                    return a.ToList();
                }

                return new List<ServicePlayboxMappingCodeModel>();
            }
        }
    }
}
