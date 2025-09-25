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
    public class GetCheckPremiumInstallQueryHandler : IQueryHandler<GetCheckPremiumInstallQuery, CheckPremiumInstallModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<CheckPremiumInstallModel> _checkPremiumInstallModel;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetCheckPremiumInstallQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<CheckPremiumInstallModel> checkPremiumInstallModel)
        {
            _logger = logger;
            _checkPremiumInstallModel = checkPremiumInstallModel;
            _intfLog = intfLog;
            _uow = uow;
        }

        public CheckPremiumInstallModel Handle(GetCheckPremiumInstallQuery query)
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

            CheckPremiumInstallModel ResultData = new CheckPremiumInstallModel();

            try
            {
                var executeResult = _checkPremiumInstallModel.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_CHECK_PREMIUM_INSTALL",
                     new
                     {
                         p_order_no = query.p_order_no,

                         ret_code = ret_code,
                         ret_msg = ret_msg
                     }).ToList();

                decimal ret = Decimal.Parse(ret_code.Value.ToSafeString());
                ResultData.ret_code = ret;
                ResultData.ret_msg = ret_msg.Value.ToSafeString();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }
            return ResultData;
        }
    }
}
