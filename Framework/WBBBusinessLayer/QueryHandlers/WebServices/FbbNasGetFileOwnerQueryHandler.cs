using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class FbbNasGetFileOwnerQueryHandler : IQueryHandler<FbbNasGetFileOwnerQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<string> _objService;

        public FbbNasGetFileOwnerQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _objService = objService;
        }

        public string Handle(FbbNasGetFileOwnerQuery query)
        {
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "FbbNasGetFileOwnerQuery", "FbbNasGetFileOwnerQueryHandler", null, "FBB", "");
            string result = null;
            try
            {

                var ret_file_owner = new OracleParameter();
                ret_file_owner.OracleDbType = OracleDbType.Varchar2;
                ret_file_owner.ParameterName = "ret_file_owner";
                ret_file_owner.Size = 2000;
                ret_file_owner.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Int32;
                ret_code.ParameterName = "ret_code";
                ret_code.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.P_FBB_NAS_GET_FILEOWNER",
                    out paramOut,
                   new
                   {
                       //in 
                       p_FILE_NAME = query.file_name,

                       //out
                       ret_file_owner = ret_file_owner,
                       ret_code = ret_code
                   });

                if (ret_code.Value.ToString() == "0")
                {
                    result = ret_file_owner.Value.ToString();
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ret_code, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Error", ex.Message, "");
            }

            return result;
        }
    }
}
