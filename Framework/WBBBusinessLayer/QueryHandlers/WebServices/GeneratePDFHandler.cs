using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GeneratePDFHandler : IQueryHandler<PDFDataQuery, PDFData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PDFData> _objService;

        public GeneratePDFHandler(ILogger logger, IEntityRepository<PDFData> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public PDFData Handle(PDFDataQuery query)
        {
            try
            {
                _logger.Info("GeneratePDFHandler Start");

                var orderNo = new OracleParameter();
                orderNo.ParameterName = "p_order_no";
                orderNo.OracleDbType = OracleDbType.Varchar2;
                orderNo.Direction = ParameterDirection.Input;
                orderNo.Value = query.orderNo;

                var language = new OracleParameter();
                language.ParameterName = "p_language";
                language.OracleDbType = OracleDbType.Varchar2;
                language.Direction = ParameterDirection.Input;
                language.Value = query.Language;

                var isShop = new OracleParameter();
                isShop.ParameterName = "p_is_shop";
                isShop.OracleDbType = OracleDbType.Varchar2;
                isShop.Direction = ParameterDirection.Input;
                isShop.Value = query.isShop;



                var ret_code = new OracleParameter();
                ret_code.ParameterName = "return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "return_message";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "str_pdf_html";
                ret_data.OracleDbType = OracleDbType.Clob;
                ret_data.Direction = ParameterDirection.Output;

                var ret_data_summary = new OracleParameter();
                ret_data_summary.ParameterName = "str_pdf_summary_html";
                ret_data_summary.OracleDbType = OracleDbType.Clob;
                ret_data_summary.Direction = ParameterDirection.Output;

                var ret_data_condition = new OracleParameter();
                ret_data_condition.ParameterName = "str_pdf_condition_html";
                ret_data_condition.OracleDbType = OracleDbType.Clob;
                ret_data_condition.Direction = ParameterDirection.Output;

                PDFData returnData = new PDFData();
                if (query.isEApp)
                {

                    Dictionary<string, object> executeResult = _objService.ExecuteStoredProcExecuteReader("WBB.PKG_FBB_GEN_PDF.PROC_GEN_PDF",

                                new object[]
                            {
                                orderNo,
                                language,
                                ret_code,
                                ret_msg,
                                ret_data

                            });


                    returnData.str_pdf_html = executeResult.FirstOrDefault(x => x.Key == "str_pdf_html").Value.ToString();
                    returnData.return_code = decimal.Parse(executeResult.FirstOrDefault(x => x.Key == "return_code").Value.ToString());
                    returnData.return_message = executeResult.FirstOrDefault(x => x.Key == "return_message").Value.ToString();

                }
                else
                {
                    if (query.pageNo == 1)
                    {
                        Dictionary<string, object> executeResult = _objService.ExecuteStoredProcExecuteReader("WBB.PKG_FBB_GEN_PDF.PROC_GEN_PDF_SUMMARY",

                                    new object[]
                            {
                                orderNo,
                                language,
                                isShop,
                                ret_code,
                                ret_msg,
                                ret_data_summary

                            });
                        returnData.str_pdf_html = executeResult.FirstOrDefault(x => x.Key == "str_pdf_summary_html").Value.ToString();
                        returnData.return_code = decimal.Parse(executeResult.FirstOrDefault(x => x.Key == "return_code").Value.ToString());
                        returnData.return_message = executeResult.FirstOrDefault(x => x.Key == "return_message").Value.ToString();
                    }
                    else
                    {
                        Dictionary<string, object> executeResult2 = _objService.ExecuteStoredProcExecuteReader("WBB.PKG_FBB_GEN_PDF.PROC_GEN_PDF_CONDITION",

                                    new object[]
                            {
                                orderNo,
                                language,
                                ret_code,
                                ret_msg,
                                ret_data_condition

                            });
                        returnData.str_pdf_html = executeResult2.FirstOrDefault(x => x.Key == "str_pdf_condition_html").Value.ToString();
                        returnData.return_code = decimal.Parse(executeResult2.FirstOrDefault(x => x.Key == "return_code").Value.ToString());
                        returnData.return_message = executeResult2.FirstOrDefault(x => x.Key == "return_message").Value.ToString();
                    }

                }

                return returnData;

            }
            catch (Exception ex)
            {
                PDFData error = new PDFData();
                error.return_code = -1;
                error.return_message = ex.Message;
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBB_GEN_PDF" + ex.Message);

                return error;
            }

        }

    }
}
