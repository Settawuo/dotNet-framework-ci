using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class LMDGenfileSubpaymentQueryHandler : IQueryHandler<LMDGenfileSubpaymentQuery, LMDGenfileSubpaymentReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LMDGenfileListCursor> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public LMDGenfileSubpaymentQueryHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<LMDGenfileListCursor> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
            _emailProcService = emailProcService;
        }

        public LMDGenfileSubpaymentReturn Handle(LMDGenfileSubpaymentQuery query)
        {
            var returnForm = new LMDGenfileSubpaymentReturn();
            try
            {
                var flag_check = new OracleParameter();
                { 
                    flag_check.OracleDbType = OracleDbType.Varchar2;
                    flag_check.Direction = ParameterDirection.Input;
                    flag_check.ParameterName = "flag_check";
                    flag_check.Value = query.flag_check;
                };
                

                var ret_code = new OracleParameter();
                {
                    ret_code.OracleDbType = OracleDbType.Decimal;
                    ret_code.Direction = ParameterDirection.Output;
                    ret_code.ParameterName = "ret_code";
                };

                var ret_msg = new OracleParameter();
                {
                    ret_msg.OracleDbType = OracleDbType.Varchar2;
                    ret_msg.Size = 2000;
                    ret_msg.Direction = ParameterDirection.Output;
                    ret_msg.ParameterName = "ret_msg";
                };

                var out_cursor = new OracleParameter();
                {
                    out_cursor.OracleDbType = OracleDbType.RefCursor;
                    out_cursor.Size = 2000;
                    out_cursor.Direction = ParameterDirection.Output;
                    out_cursor.ParameterName = "out_cursor";
                };



                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FIXED_ASSET_LASTMILE.p_fetch_data_subpayment",
                  new object[]
                  {
                      flag_check,
                      //result = result,                     
                      ret_code = ret_code,
                      ret_msg = ret_msg,
                      out_cursor

                  }).ToList();

                DataTable resp = new DataTable();
                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                //returnForm.ret_msgg = ret_msgg.Value != null ? ret_msgg.Value.ToString() : "-1";
                //returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                //returnForm.cur = executeResult;
                List<LMDGenfileListCursor> respList = new List<LMDGenfileListCursor>();
                if (executeResult[0] != null)
                {
                    returnForm.ret_code = executeResult[0].ToSafeString();
                }

                if (executeResult[1] != null)
                {
                    returnForm.ret_msg = executeResult[1].ToSafeString();
                }

                if (executeResult[2] != null)
                {
                    resp = (DataTable)executeResult[2];
                    respList = resp.DataTableToList<LMDGenfileListCursor>();
                    returnForm.out_cursor = respList;
                }

                return returnForm;

            }
            catch (Exception ex)
            {
            return null;

            }
        }
    }
}