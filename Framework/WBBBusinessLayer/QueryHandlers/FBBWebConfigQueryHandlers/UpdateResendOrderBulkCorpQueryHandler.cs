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
    public class UpdateResendOrderBulkCorpQueryHandler : IQueryHandler<UpdateResendOrderBulkCorpQuery, Resendreturndata>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<Resendreturndata> _objService;

        public UpdateResendOrderBulkCorpQueryHandler(ILogger logger, IEntityRepository<Resendreturndata> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public Resendreturndata Handle(UpdateResendOrderBulkCorpQuery query)
        {
            try
            {
                var p_bulk_number_return = new OracleParameter();
                p_bulk_number_return.ParameterName = "p_bulk_number_return";
                p_bulk_number_return.OracleDbType = OracleDbType.Varchar2;
                p_bulk_number_return.Size = 2000;
                p_bulk_number_return.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "output_return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "output_return_message";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;



                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_RESEND_UPDATE",
                            new
                            {
                                bulk_number = query.bulk_number,
                                p_user = query.p_user,

                                p_no = query.p_no,
                                p_order_number = query.p_order_number,
                                p_installaddress1 = query.p_installaddress1,
                                p_installaddress2 = query.p_installaddress2,
                                p_installaddress3 = query.p_installaddress3,
                                p_installaddress4 = query.p_installaddress4,
                                p_installaddress5 = query.p_installaddress5,
                                p_latitude = query.p_latitude,
                                p_longitude = query.p_longitude,
                                p_install_date = query.p_install_date,
                                p_file_name = query.p_file_name,

                                //  return code
                                p_bulk_number_return = p_bulk_number_return,
                                ret_code = ret_code,
                                ret_msg = ret_msg


                            }).ToList();

                Resendreturndata ResultData = new Resendreturndata();

                var return_bulk_no = p_bulk_number_return.Value.ToString();
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

                ResultData.p_bulk_number_return = return_bulk_no;
                ResultData.output_return_code = return_code;
                ResultData.output_return_message = return_msg;

                if (return_code == "0")
                {
                    _logger.Info("EndPROC_RESEND_UPDATE" + return_msg);
                    return ResultData;

                }
                else
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_RESEND_UPDATE output msg: " + return_msg);
                    return null;
                }


            }
            catch (Exception ex)
            {
                _logger.Info("Error when WBB.PKG_FBBBULK_CORP_REGISTER.PROC_RESEND_UPDATE output msg: " + ex.InnerException);

                return null;
            }
        }
    }
}
