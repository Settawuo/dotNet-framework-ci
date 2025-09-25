using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public class GetPreRegisterQueryHandler : IQueryHandler<GetPreRegisterQuery, GetPreRegisterQueryData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PreRegisterModel> _objService;

        public GetPreRegisterQueryHandler(ILogger logger, IEntityRepository<PreRegisterModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public GetPreRegisterQueryData Handle(GetPreRegisterQuery query)
        {
            try
            {
                _logger.Info("GetPreRegisterQueryHandler Start");
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "list_info_by_monthly";
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                List<PreRegisterModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_LIST_INFO_BY_MONTHLY",

                            new
                            {
                                p_location_code = query.LocationCode,
                                p_asc_code = query.AscCode,
                                p_monthly_rpt = query.QMonth,

                                //out
                                return_code = ret_code,
                                return_message = ret_msg,
                                list_info_by_monthly = ret_data

                            }).ToList();

                GetPreRegisterQueryData success = new GetPreRegisterQueryData();
                success.PreRegisterModel = executeResult;
                success.ReturnCode = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                success.ReturnMessage = ret_msg.Value.ToSafeString();

                return success;

            }
            catch (Exception ex)
            {
                GetPreRegisterQueryData error = new GetPreRegisterQueryData();
                error.ReturnCode = -1;
                error.ReturnMessage = ex.Message;
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_LIST_INFO_BY_MONTHLY" + ex.Message);

                return error;
            }

        }

    }
}
