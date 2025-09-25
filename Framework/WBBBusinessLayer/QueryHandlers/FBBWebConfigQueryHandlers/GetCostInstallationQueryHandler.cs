using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetCostInstallationQueryHandler : IQueryHandler<GetCostInstallationQuery, List<CostInstallation>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBSS_INSTALLATION_COST> _costInstall;

        public GetCostInstallationQueryHandler(ILogger logger, IEntityRepository<FBSS_INSTALLATION_COST> costInstall)
        {
            _logger = logger;
            _costInstall = costInstall;
        }

        public List<CostInstallation> Handle(GetCostInstallationQuery query)
        {
            //if (query.SERVICE == "")
            //{
            //    query.SERVICE = null;
            //}
            //if (query.VENDOR == "")
            //{
            //    query.VENDOR = null;
            //}
            //if (query.ORDER_TYPE == "")
            //{
            //    query.ORDER_TYPE = null;
            //}
            //if (query.INS_OPTION == "")
            //{
            //    query.INS_OPTION = null;
            //}
            //string _service = query.SERVICE ?? null;
            //string _vendor = query.VENDOR ?? null;
            //string _orderType = query.ORDER_TYPE ?? null;
            //string _insOption = query.INS_OPTION ?? null;

            var cost = (from c in _costInstall.Get()
                            //where (
                            //    c.SERVICE == _service ||
                            //    c.VENDOR == _vendor ||
                            //    c.ORDER_TYPE == _orderType
                            //) 
                        select c);

            if (!string.IsNullOrEmpty(query.SERVICE))
            {
                cost = cost.Where(c => c.SERVICE.ToUpper() == query.SERVICE.ToUpper());
            }
            if (!string.IsNullOrEmpty(query.VENDOR))
            {
                cost = cost.Where(c => c.VENDOR.ToUpper() == query.VENDOR.ToUpper());
            }
            if (!string.IsNullOrEmpty(query.ORDER_TYPE))
            {
                cost = cost.Where(c => c.ORDER_TYPE.ToUpper() == query.ORDER_TYPE.ToUpper());
            }
            if (!string.IsNullOrEmpty(query.INS_OPTION))
            {
                cost = cost.Where(c => c.INS_OPTION.ToUpper().Contains(query.INS_OPTION.ToUpper()));
            }

            List<CostInstallation> result = (from c in cost
                                             select new CostInstallation
                                             {
                                                 ID = c.ID,
                                                 SERVICE = c.SERVICE,
                                                 CUSTOMER = c.INS_OPTION,
                                                 CUSTOMER_NAME = c.VENDOR,
                                                 INTERNET_RATE = c.INTERNET,
                                                 PLAYBOX_RATE = c.PLAYBOX,
                                                 VOIP_RATE = c.VOIP,
                                                 ORDER_TYPE = c.ORDER_TYPE,
                                                 EFFECTIVE_DATE = c.EFFECTIVE_DATE,
                                                 EXPIRE_DATE = c.EXPIRE_DATE,
                                                 REMARK = c.REMARK,
                                                 LENGTH_FR = c.LENGTH_FR,
                                                 LENGTH_TO = c.LENGTH_TO,
                                                 OUT_DOOR_PRICE = c.OUT_DOOR_PRICE,
                                                 IN_DOOR_PRICE = c.IN_DOOR_PRICE,
                                                 CREATE_DATE = c.CREATE_DATE,
                                                 CREATE_BY = c.CREATE_BY,
                                                 UPDATED_DATE = c.UPDATED_DATE,
                                                 UPDATED_BY = c.UPDATED_BY,
                                                 ADDRESS_ID = c.ADDRESS_ID,
                                                 TOTAL_PRICE = c.TOTAL_PRICE
                                             }).ToList();
            return result;
        }

    }
}
