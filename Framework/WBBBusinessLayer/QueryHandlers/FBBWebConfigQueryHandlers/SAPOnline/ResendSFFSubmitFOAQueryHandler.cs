using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class ResendSFFSubmitFOAQueryHandler : IQueryHandler<ResendSFFSubmitFOAQuery, List<ListPendingSFFSubmitFOA>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ListPendingSFFSubmitFOA> _submitOrderLog;

        public ResendSFFSubmitFOAQueryHandler(
            ILogger logger,
            IEntityRepository<ListPendingSFFSubmitFOA> submitOrderLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _submitOrderLog = submitOrderLog;
        }

        public List<ListPendingSFFSubmitFOA> Handle(ResendSFFSubmitFOAQuery query)
        {

            var p_ws_sff = new OracleParameter();
            p_ws_sff.ParameterName = "p_ws_sff";
            p_ws_sff.OracleDbType = OracleDbType.RefCursor;
            p_ws_sff.Direction = ParameterDirection.Output;

            var p_return_code = new OracleParameter();
            p_return_code.ParameterName = "p_return_code";
            p_return_code.Size = 2000;
            p_return_code.OracleDbType = OracleDbType.Varchar2;
            p_return_code.Direction = ParameterDirection.Output;

            var p_return_message = new OracleParameter();
            p_return_message.ParameterName = "ret_msg";
            p_return_message.Size = 2000;
            p_return_message.OracleDbType = OracleDbType.Varchar2;
            p_return_message.Direction = ParameterDirection.Output;

            List<ListPendingSFFSubmitFOA> executeResult = _submitOrderLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.show_sff",
                new
                {
                    p_ws_sff = p_ws_sff,
                    p_return_code = p_return_code,
                    p_return_message = p_return_message
                }).ToList();

            return executeResult;
        }
    }

}
