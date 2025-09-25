using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckPopupFMCQueryHandler : IQueryHandler<CheckPopupFMCQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<string> _objService;

        public CheckPopupFMCQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _objService = objService;
        }

        public string Handle(CheckPopupFMCQuery query)
        {
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "CheckPopupFMCQuery", "CheckPopupFMCQueryHandler", null, "FBB", "");
            string Result = "";
            try
            {
                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    ParameterName = "ret_code",
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var ret_msg = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    ParameterName = "ret_msg",
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_CHECK_POPUP_FMC",
                   new
                   {
                       //in 
                       p_sff_promotion_code = query.p_sff_promotion_code,
                       p_existing_mobile_flag = query.p_existing_mobile_flag,

                       /// Out
                       ret_code = ret_code,
                       ret_msg = ret_msg

                   });

                string returnCode = ret_code.Value == null ? "" : ret_code.Value.ToString();
                string returnMsg = ret_msg.Value == null ? "" : ret_msg.Value.ToString();
                if (returnMsg == "null")
                    returnMsg = "";

                Result = returnMsg;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, returnMsg, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Error", ex.Message, "");
            }

            return Result;
        }
    }
}
