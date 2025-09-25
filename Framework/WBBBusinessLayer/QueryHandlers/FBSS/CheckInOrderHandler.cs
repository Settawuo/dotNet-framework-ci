using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBSS;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.FBSS
{
    public class CheckInOrderHandler : IQueryHandler<CheckInOrderQuery, CheckInOrderModel>
    {
        private readonly ILogger _logger;
        //private readonly IFBSSEntityRepository<CheckInOrderModel> _output;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        public CheckInOrderHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            //_output = output;
            _lov = lov;

        }

        public CheckInOrderModel Handle(CheckInOrderQuery query)
        {
            var lovStrQ = new FBB_CFG_LOV();
            lovStrQ = (from z in _lov.Get()
                       where z.LOV_VAL1 == query.queryString && z.LOV_NAME == "ORDER_TYPE" && z.LOV_VAL5 == "CHECKIN" && z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "SCREEN"
                       select z).FirstOrDefault();

            CheckInOrderModel ret = new CheckInOrderModel();
            try
            {
                //ret.Data = _output.ExecuteToDataTable(Encoding.UTF8.GetString(lovStrQ.IMAGE_BLOB), "FBSS_DATA");
                ret.Data = new DataTable();
            }
            catch (Exception ex)
            {
                ret.Data = new DataTable();
            }

            return ret;
        }
    }
}
