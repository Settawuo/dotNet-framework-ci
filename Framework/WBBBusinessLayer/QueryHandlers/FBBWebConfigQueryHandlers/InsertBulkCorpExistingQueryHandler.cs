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
    class InsertBulkCorpExistingQueryHandler : IQueryHandler<InsertBulkCorpExistingQuery, retInsertBulkCorpExisting>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<retInsertBulkCorpExisting> _objService;

        public InsertBulkCorpExistingQueryHandler(ILogger logger, IEntityRepository<retInsertBulkCorpExisting> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public retInsertBulkCorpExisting Handle(InsertBulkCorpExistingQuery query)
        {
            try
            {
                var output_bulk_no = new OracleParameter();
                output_bulk_no.ParameterName = "output_bulk_no";
                output_bulk_no.OracleDbType = OracleDbType.Varchar2;
                output_bulk_no.Size = 2000;
                output_bulk_no.Direction = ParameterDirection.Output;

                var output_account_category = new OracleParameter();
                output_account_category.ParameterName = "output_account_category";
                output_account_category.OracleDbType = OracleDbType.Varchar2;
                output_account_category.Size = 2000;
                output_account_category.Direction = ParameterDirection.Output;

                var output_account_sub_category = new OracleParameter();
                output_account_sub_category.ParameterName = "output_account_sub_category";
                output_account_sub_category.OracleDbType = OracleDbType.Varchar2;
                output_account_sub_category.Size = 2000;
                output_account_sub_category.Direction = ParameterDirection.Output;

                var output_return_code = new OracleParameter();
                output_return_code.ParameterName = "output_return_code";
                output_return_code.OracleDbType = OracleDbType.Varchar2;
                output_return_code.Size = 2000;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.ParameterName = "output_return_message";
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Size = 2000;
                output_return_message.Direction = ParameterDirection.Output;


                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_EXISTING_INSERT",
                            new
                            {
                                p_user = query.p_user,
                                p_asc_code = query.p_asc_code,
                                p_employee_id = query.p_employee_id,
                                p_location_code = query.p_location_code,
                                p_bulk_number = query.p_bulk_number,

                                //  return code
                                output_bulk_no = output_bulk_no,
                                output_account_category = output_account_category,
                                output_account_sub_category = output_account_sub_category,
                                output_return_code = output_return_code,
                                output_return_message = output_return_message

                            }).ToList();

                retInsertBulkCorpExisting ResultData = new retInsertBulkCorpExisting();

                var ret_bulk_no = output_bulk_no.Value.ToString();
                ResultData.output_bulk_no = ret_bulk_no;

                if (null == ret_bulk_no || "" == ret_bulk_no)
                {
                    return null;
                }

                var ret_accnt_cate = output_account_category.Value.ToString();
                ResultData.output_account_category = ret_accnt_cate;
                if (null == ret_accnt_cate || "" == ret_accnt_cate)
                {
                    return null;
                }

                var ret_accnt_sub_cate = output_account_sub_category.Value.ToString();
                ResultData.output_account_sub_category = ret_accnt_sub_cate;
                if (null == ret_accnt_sub_cate || "" == ret_accnt_sub_cate)
                {
                    return null;
                }

                var ret_code = output_return_code.Value.ToString();
                ResultData.output_return_code = ret_code;
                if (ret_code == null || ret_code == "")
                {
                    return null;
                }

                var ret_msg = output_return_message.Value.ToString();
                ResultData.output_return_message = ret_msg;


                if (ret_code == "0") // return 0 pass value to screen 
                {
                    _logger.Info("EndPROC_EXISTING_INSERT " + output_return_message);
                    return ResultData;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_EXISTING_INSERT output msg: " + output_return_message);
                    return null;

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_EXISTING_INSERT" + ex.Message);

                return null;
            }
        }

    }
}
