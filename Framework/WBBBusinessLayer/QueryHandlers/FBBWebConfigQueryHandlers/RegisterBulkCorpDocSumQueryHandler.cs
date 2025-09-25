using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class RegisterBulkCorpDocSumQueryHandler : IQueryHandler<RegisterBulkCorpDocSumQuery, ReturnDocumentSum>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DocumentSumList> _objService;

        public RegisterBulkCorpDocSumQueryHandler(ILogger logger, IEntityRepository<DocumentSumList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public ReturnDocumentSum Handle(RegisterBulkCorpDocSumQuery query)
        {
            try
            {
                var output_bulk_no = new OracleParameter();
                output_bulk_no.ParameterName = "output_bulk_no";
                output_bulk_no.OracleDbType = OracleDbType.Varchar2;
                output_bulk_no.Size = 2000;
                output_bulk_no.Direction = ParameterDirection.Output;

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

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "p_res_data";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SUMMARY_DETAIL",
                            new
                            {
                                p_bulk_no = query.Bulk_No,

                                output_bulk_no = output_bulk_no,
                                output_return_code = output_return_code,
                                output_return_message = output_return_message,
                                p_res_data = p_res_data

                            }).ToList();

                ReturnDocumentSum resultdata = new ReturnDocumentSum();
                resultdata.NewDocSumList = executeResult;

                var ret_bulk_no = output_bulk_no.Value.ToSafeString();

                if (null == ret_bulk_no || "" == ret_bulk_no)
                {
                    return null;
                }
                var out_code = output_return_code.Value.ToSafeString();
                if (out_code == null || out_code == "")
                {
                    return null;
                }
                var out_msg = output_return_message.Value.ToSafeString();

                resultdata.output_bulk_no = ret_bulk_no;
                resultdata.output_return_code = out_code;
                resultdata.output_return_message = out_msg;

                if (out_code == "0") // return 0 pass value to screen 
                {
                    _logger.Info("EndPROC_SUMMARY_DETAIL" + out_msg);
                    return resultdata;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SUMMARY_DETAIL output msg: " + out_msg);
                    return null;

                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error when WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SUMMARY_DETAIL output msg: " + ex.GetErrorMessage());
                return null;
            }
        }

    }
}
