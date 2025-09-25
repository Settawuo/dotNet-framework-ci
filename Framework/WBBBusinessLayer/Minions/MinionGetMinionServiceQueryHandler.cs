using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Minions;
using WBBData.Repository;
using WBBEntity.Minions;
using WBBEntity.Models;

namespace WBBBusinessLayer.Minions
{
    public class MinionGetMinionServiceQueryHandler :
        IQueryHandler<MinionGetMinionServiceQuery, List<MinionGetMinionServiceQueryModel>>
    {
        private readonly IEntityRepository<FBB_MINION_SERVICE> _objMinionService;

        public MinionGetMinionServiceQueryHandler(
            IEntityRepository<FBB_MINION_SERVICE> objMinionService)
        {
            _objMinionService = objMinionService;
        }

        public List<MinionGetMinionServiceQueryModel> Handle(MinionGetMinionServiceQuery query)
        {
            var listMinion = new List<MinionGetMinionServiceQueryModel>();
            try
            {
                List<FBB_MINION_SERVICE> lstMinionServices;

                IOrderedQueryable<FBB_MINION_SERVICE> result;
                switch (query.Flag)
                {
                    case "MENU":
                        result = from minion in _objMinionService.Get()
                                 where minion.ACTIVE_FLAG == "Y"
                                 orderby minion.SERVICE_MAIN_ID
                                 select minion;

                        lstMinionServices = result.ToList();

                        listMinion.AddRange(lstMinionServices.Select(itemService => new MinionGetMinionServiceQueryModel
                        {
                            ACTIVE_FLAG = itemService.ACTIVE_FLAG,
                            CREATED_BY = itemService.CREATED_BY,
                            CREATED_DATE = itemService.CREATED_DATE,
                            DEV_SERVICE_MAIN_URL = itemService.DEV_SERVICE_MAIN_URL,
                            PRD_SERVICE_MAIN_URL = itemService.PRD_SERVICE_MAIN_URL,
                            REQUET_SOAP_XML = "",
                            SERVICE_ID = itemService.SERVICE_ID,
                            SERVICE_MAIN_ID = itemService.SERVICE_MAIN_ID,
                            SERVICE_MAIN_NAME = itemService.SERVICE_MAIN_NAME,
                            SERVICE_PARENT_ID = itemService.SERVICE_PARENT_ID,
                            SERVICE_PARENT_NAME = itemService.SERVICE_PARENT_NAME,
                            STG_SERVICE_MAIN_URL = itemService.STG_SERVICE_MAIN_URL,
                            UPDATED_BY = itemService.UPDATED_BY,
                            UPDATED_DATE = itemService.UPDATED_DATE
                        }));

                        break;
                    default:

                        result = from minion in _objMinionService.Get()
                                 where minion.ACTIVE_FLAG == "Y"
                                 orderby minion.SERVICE_MAIN_ID
                                 select minion;

                        if (query.ServiceId != null && query.ServiceId > 0)
                        {
                            lstMinionServices = result.Where(item => item.SERVICE_ID == query.ServiceId).ToList();
                        }
                        else
                        {
                            lstMinionServices = result.ToList();
                        }

                        listMinion.AddRange(lstMinionServices.Select(itemService => new MinionGetMinionServiceQueryModel
                        {
                            ACTIVE_FLAG = itemService.ACTIVE_FLAG,
                            CREATED_BY = itemService.CREATED_BY,
                            CREATED_DATE = itemService.CREATED_DATE,
                            DEV_SERVICE_MAIN_URL = itemService.DEV_SERVICE_MAIN_URL,
                            PRD_SERVICE_MAIN_URL = itemService.PRD_SERVICE_MAIN_URL,
                            REQUET_SOAP_XML = itemService.REQUET_SOAP_XML,
                            SERVICE_ID = itemService.SERVICE_ID,
                            SERVICE_MAIN_ID = itemService.SERVICE_MAIN_ID,
                            SERVICE_MAIN_NAME = itemService.SERVICE_MAIN_NAME,
                            SERVICE_PARENT_ID = itemService.SERVICE_PARENT_ID,
                            SERVICE_PARENT_NAME = itemService.SERVICE_PARENT_NAME,
                            STG_SERVICE_MAIN_URL = itemService.STG_SERVICE_MAIN_URL,
                            UPDATED_BY = itemService.UPDATED_BY,
                            UPDATED_DATE = itemService.UPDATED_DATE
                        }));
                        break;
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listMinion;
        }
    }
}
