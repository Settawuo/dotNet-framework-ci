using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetMemberGetMemberQueryHandler : IQueryHandler<GetMemberGetMemberQuery, List<MemberGetMemberStatus>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<MemberGetMemberStatus> _objService;

        public GetMemberGetMemberQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<MemberGetMemberStatus> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _objService = objService;
        }

        public List<MemberGetMemberStatus> Handle(GetMemberGetMemberQuery query)
        {
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GetMemberGetMemberQuery", "GetMemberGetMemberQueryHandler", null, "FBB", "");

                var returnCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    ParameterName = "return_code",
                    Direction = ParameterDirection.Output
                };

                var returnMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    ParameterName = "return_message",
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var ioResults = new OracleParameter
                {
                    ParameterName = "ioResults",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                List<MemberGetMemberStatus> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBOR021.PROC_LIST_ORDER_STATUS",
                   new
                   {
                       //in 
                       p_language = query.Language,
                       p_referral_internet_no = query.InternetNo,

                       /// Out
                       return_code = returnCode,
                       return_message = returnMessage,
                       ioResults = ioResults

                   }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                return executeResult;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Error", ex.GetErrorMessage(), "");
                return new List<MemberGetMemberStatus>();
            }
        }

    }
}
