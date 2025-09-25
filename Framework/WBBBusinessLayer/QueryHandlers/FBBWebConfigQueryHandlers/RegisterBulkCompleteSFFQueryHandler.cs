using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class RegisterBulkCompleteSFFQueryHandler : IQueryHandler<RegisterBulkCompleteSFFQuery, ReturnCompletesff>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReturnCompletesff> _objService;

        public RegisterBulkCompleteSFFQueryHandler(ILogger logger, IEntityRepository<ReturnCompletesff> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public ReturnCompletesff Handle(RegisterBulkCompleteSFFQuery query)
        {
            try
            {
                var output_bulk_no = new OracleParameter();
                output_bulk_no.ParameterName = "output_bulk_no";
                output_bulk_no.OracleDbType = OracleDbType.Varchar2;
                output_bulk_no.Size = 2000;
                output_bulk_no.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "output_return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "output_return_message";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;



                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_COMPLETE_SFF_ORDER",
                            new
                            {
                                p_bulk_number = query.p_bulk_number,
                                p_result = query.p_result,
                                p_errorreason = query.p_errorreason,

                                //  return code
                                output_bulk_no = output_bulk_no,
                                ret_code = ret_code,
                                ret_msg = ret_msg


                            }).ToList();

                ReturnCompletesff ResultData = new ReturnCompletesff();

                var return_bulk_no = output_bulk_no.Value.ToString();
                var return_code = ret_code.Value.ToString();
                var return_msg = ret_msg.Value.ToString();

                if (null == return_bulk_no || "" == return_bulk_no)
                {
                    return null;
                }

                if (return_code == null || return_code == "")
                {
                    return null;
                }

                ResultData.output_bulk_no = return_bulk_no;
                ResultData.output_return_code = return_code;
                ResultData.output_return_message = return_msg;

                if (return_code == "0")
                {
                    _logger.Info("EndPROC_COMPLETE_SFF_ORDER" + return_msg);
                    return ResultData;

                }
                else
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_COMPLETE_SFF_ORDER output msg: " + return_msg);
                    return null;
                }


            }
            catch (Exception ex)
            {
                _logger.Info("Error when WBB.PKG_FBBBULK_CORP_REGISTER.PROC_COMPLETE_SFF_ORDER output msg: " + ex.InnerException);

                return null;
            }
        }
    }
}
