using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetRequestFormReportQueryHandlers : IQueryHandler<GetRequestFormReportQuery, RequestFormReturn>//List<ReportRequestFormListDetail>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReportRequestFormListDetail> _objService;

        public GetRequestFormReportQueryHandlers(ILogger logger, IEntityRepository<ReportRequestFormListDetail> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public RequestFormReturn Handle(GetRequestFormReportQuery query)
        {
            RequestFormReturn returnForm = new RequestFormReturn();

            try
            {

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "P_RETURN_CODE";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "P_RETURN_MESSAGE";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var p_page_count = new OracleParameter();
                p_page_count.ParameterName = "P_PAGE_COUNT";
                p_page_count.OracleDbType = OracleDbType.Decimal;
                p_page_count.Direction = ParameterDirection.Output;

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "P_RES_DATA";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;

                _logger.Info("StartPKG_FBBDORM_REPORT_REQUESTFORM");

                List<ReportRequestFormListDetail> executeResult = _objService.ExecuteReadStoredProc("PKG_FBBDORM_REPORT_REQUESTFORM.PROC_REPORT_REQUESTFORM",
                            new
                            {
                                P_DATE_FROM = query.P_DATE_FROM,
                                P_DATE_TO = query.P_DATE_TO,
                                P_REGION_CODE = query.P_REGION_CODE,
                                P_PROVINCE = query.P_PROVINCE,
                                P_PROCESS_STATUS = query.P_PROCESS_STATUS,
                                P_PAGE_INDEX = query.P_PAGE_INDEX,
                                P_PAGE_SIZE = query.P_PAGE_SIZE,


                                //  p_return_code code
                                p_page_count = p_page_count,
                                p_return_code = p_return_code,
                                p_return_message = p_return_message,
                                p_res_data = p_res_data

                            }).ToList();


                returnForm.P_RES_DATA = executeResult;
                returnForm.P_RETURN_CODE = p_return_code.Value != null ? p_return_code.Value.ToString() : "-1";
                returnForm.P_RETURN_MESSAGE = p_return_message.Value.ToString();
                returnForm.P_PAGE_COUNT = Int32.Parse(p_page_count.Value.ToString());

                _logger.Info("EndPKG_FBBDORM_REPORT_REQUESTFORM" + p_return_message.Value.ToString());


                return returnForm;

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBDORM_REPORT_REQUESTFORM handles : " + ex.Message);
                returnForm.P_RETURN_CODE = "-1";
                returnForm.P_RETURN_MESSAGE = ex.Message;
                return null;
            }

        }
    }
}
