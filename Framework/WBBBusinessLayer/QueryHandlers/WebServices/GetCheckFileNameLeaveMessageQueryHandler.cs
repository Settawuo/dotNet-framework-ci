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
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetCheckFileNameLeaveMessageQueryHandler : IQueryHandler<GetCheckFileNameLeaveMessageQuery, CheckFileNameLeaveMessageModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<CheckFileNameLeaveMessageModel> _checkFileNameLeaveMessageModel;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetCheckFileNameLeaveMessageQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<CheckFileNameLeaveMessageModel> checkFileNameLeaveMessageModel)
        {
            _logger = logger;
            _checkFileNameLeaveMessageModel = checkFileNameLeaveMessageModel;
            _intfLog = intfLog;
            _uow = uow;
        }

        public CheckFileNameLeaveMessageModel Handle(GetCheckFileNameLeaveMessageQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var cursor = new OracleParameter();
            cursor.OracleDbType = OracleDbType.RefCursor;
            cursor.Direction = ParameterDirection.Output;

            CheckFileNameLeaveMessageModel ResultData = new CheckFileNameLeaveMessageModel();

            try
            {
                var executeResult = _checkFileNameLeaveMessageModel.ExecuteReadStoredProc("WBB.PKG_FBBOR021.PROC_CHECK_FILE_NAME",
                     new
                     {
                         p_file_name = query.p_file_name.ToSafeString(),
                         p_user_name = query.p_user_name.ToSafeString(),

                         return_code = ret_code,
                         return_message = ret_msg
                     }).ToList();

                decimal ret = Decimal.Parse(ret_code.Value.ToSafeString());
                ResultData.return_code = ret;
                ResultData.return_message = ret_msg.Value.ToSafeString();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                ResultData.return_code = -1;
                ResultData.return_message = ex.GetErrorMessage().ToSafeString();
            }
            return ResultData;
        }
    }
}
