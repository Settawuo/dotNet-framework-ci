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
    public class SendRegisterBulkCorpToWorkFlowQueryHandler : IQueryHandler<SendRegisterBulkCorpToWorkFlowQuery, returnDetailWorkflowData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DetailWorkflow> _objService;

        public SendRegisterBulkCorpToWorkFlowQueryHandler(ILogger logger, IEntityRepository<DetailWorkflow> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public returnDetailWorkflowData Handle(SendRegisterBulkCorpToWorkFlowQuery query)
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

                var p_call_workflow = new OracleParameter();
                p_call_workflow.ParameterName = "p_call_workflow";
                p_call_workflow.OracleDbType = OracleDbType.RefCursor;
                p_call_workflow.Direction = ParameterDirection.Output;


                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_DETAIL_WORKFLOW",
                            new
                            {
                                p_bulk_no = query.p_bulk_number,

                                output_bulk_no = output_bulk_no,
                                output_return_code = output_return_code,
                                output_return_message = output_return_message,
                                p_call_workflow = p_call_workflow

                            }).ToList();

                returnDetailWorkflowData ResultData = new returnDetailWorkflowData();
                ResultData.WorkflowList = executeResult;

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

                ResultData.output_bulk_no = ret_bulk_no;
                ResultData.output_return_code = out_code;
                ResultData.output_return_message = out_msg;

                if (out_code == "0") // return 0 pass value to screen 
                {
                    _logger.Info("EndPROC_DETAIL_WORKFLOW" + out_msg);
                    return ResultData;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_DETAIL_WORKFLOW output msg: " + out_msg);
                    return null;

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_DETAIL_WORKFLOW" + ex.Message);

                return null;
            }
        }
    }
}
