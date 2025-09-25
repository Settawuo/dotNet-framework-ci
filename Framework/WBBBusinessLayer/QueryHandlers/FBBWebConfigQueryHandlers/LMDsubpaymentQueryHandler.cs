using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class LMDsubpaymentQueryHandler : IQueryHandler<LMDsubpaymentQuery, LMDsubpaymentReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LMDsubpaymentQuery> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public LMDsubpaymentQueryHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<LMDsubpaymentQuery> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
            _emailProcService = emailProcService;
        }

        public LMDsubpaymentReturn Handle(LMDsubpaymentQuery query)
        {
            LMDsubpaymentReturn lM = null;
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("Start PKG_FIXED_ASSET_LASTMILE");

                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_genfile_subpayment",
                out paramOut,
                  new
                  {
                      //result = result,                     
                      ret_code = ret_code,
                      ret_msg = ret_msg

                  });

                lM = new LMDsubpaymentReturn
                {
                    ret_code = ret_code.Value.ToSafeString(),
                    ret_msg = ret_msg.Value.ToSafeString()
                };
                _logger.Info("End PKG_FIXED_ASSET_LASTMILE");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                lM = new LMDsubpaymentReturn
                {
                    ret_code = "-1",
                    ret_msg = ex.GetErrorMessage()
                };
            }

            return lM;
        }
    }
}