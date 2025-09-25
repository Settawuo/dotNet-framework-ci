using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckPendingOrderFbssHandler : IQueryHandler<CheckPendingOrderFbssQuery, CheckPendingOrderFbssModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IQueryHandler<QueryOrderQuery, QueryOrderModel> _queryOrder;
        private readonly IWBBUnitOfWork _uow;

        public CheckPendingOrderFbssHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<FBB_CFG_LOV> lov, IQueryHandler<QueryOrderQuery, QueryOrderModel> queryOrder, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _queryOrder = queryOrder;
            _uow = uow;
        }

        public CheckPendingOrderFbssModel Handle(CheckPendingOrderFbssQuery query)
        {
            InterfaceLogCommand log = null;

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.InteretNo, "CheckPendingOrderFbss", "CheckPendingOrderFbssHandler", null, "FBB|" + query.FullUrl, "");
            var messageResult = "Success";
            var messageDescription = "Success";

            var checkPendingOrderFbssModel = new CheckPendingOrderFbssModel();
            try
            {
                _logger.Info("Start CheckPendingOrderFbss");

                checkPendingOrderFbssModel.PendingOrderFbss_Flag = "N";

                var statusConfig = (from z in _lov.Get()
                                    where z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "CONFIG"
                                    && z.LOV_NAME == "STATUS_CHECK_ORDER_PENDING_FBSS"
                                    select z.LOV_VAL1).ToList();

                var dayConfig = (from z in _lov.Get()
                                 where z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "CONFIG"
                                 && z.LOV_NAME == "DAY_OF_CHECK_ORDER_PENDING_FBSS"
                                 select z.LOV_VAL1).FirstOrDefault();

                var orderTypeConfig = (from z in _lov.Get()
                                       where z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "CONFIG"
                                       && z.LOV_NAME == "ORDER_TYPE_CHECK_ORDER_PENDING_FBSS"
                                       orderby z.ORDER_BY ascending
                                       select z.LOV_VAL1).ToList();

                var internet = new FIBRENetID()
                {
                    FIBRENET_ID = query.InteretNo,
                    START_DATE = DateTime.Now.AddDays(-Convert.ToInt32(dayConfig)).ToString("dd/MM/yyyy") + " 00:00:00",
                    END_DATE = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                //Loop Order type
                foreach (var item in orderTypeConfig)
                {
                    _logger.Info("CheckPendingOrderFbss OrderType = " + item);

                    var queryOrder = new QueryOrderQuery()
                    {
                        ORDER_TYPE = item,
                        FIBRENetID_List = new List<FIBRENetID>() { internet },
                        FullUrl = query.FullUrl
                    };

                    var result = _queryOrder.Handle(queryOrder);
                    if (result != null && result.RESULT == "0" && result.Order_Details_List != null)
                    {
                        //No Order list,then next order type
                        if (result.Order_Details_List.Count <= 0) continue;

                        //Check TRANSACTION_STATE
                        var orderDetail = result.Order_Details_List.FirstOrDefault(order => statusConfig.Contains(order.TRANSACTION_STATE));
                        //TRANSACTION_STATE is Completed ,then next order type
                        if (orderDetail != null) continue;

                        //TRANSACTION_STATE not Completed ,have order pending
                        checkPendingOrderFbssModel.PendingOrderFbss_Flag = "Y";
                        //end process check
                        return checkPendingOrderFbssModel;
                    }
                }



            }
            catch (Exception ex)
            {
                _logger.Info("Error CheckPendingOrderFbss : " + ex.StackTrace);

                messageResult = "Failed";
                messageDescription = ex.Message;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkPendingOrderFbssModel, log, messageResult, messageDescription, "");

                _logger.Info("End CheckPendingOrderFbss");
            }

            return checkPendingOrderFbssModel;
        }

    }
}
