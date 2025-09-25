using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class PdfOnWebHandler : IQueryHandler<PdfOnWebQuery, PdfOnWebModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PdfOnWebData> _objService;
        //private readonly IWBBUnitOfWork _uow;
        //private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public PdfOnWebHandler(ILogger logger, IEntityRepository<PdfOnWebData> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public PdfOnWebModel Handle(PdfOnWebQuery query)
        {
            //InterfaceLogCommand log = null;
            PdfOnWebModel result = new PdfOnWebModel();
            try
            {
                _logger.Info("GeneratePDFHandler Start");

                //input
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


                //output
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "return_message";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var LIST_PDF_CUR = new OracleParameter();
                LIST_PDF_CUR.ParameterName = "LIST_PDF_CUR";
                LIST_PDF_CUR.OracleDbType = OracleDbType.RefCursor;
                LIST_PDF_CUR.Direction = ParameterDirection.Output;

                List<PdfOnWebData> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_GEN_PDF.PROC_GENERATE_PDF",

                            new
                            {
                                //in
                                orderNo,
                                language,
                                isShop,

                                //out
                                ret_code,
                                ret_msg,
                                LIST_PDF_CUR

                            }).ToList();


                result.LIST_PDF_CUR = executeResult;
                result.return_code = ret_code.Value.ToSafeString() != "null" ? decimal.Parse(ret_code.Value.ToSafeString()) : 0;
                result.return_message = ret_msg.Value != null ? ret_msg.Value.ToSafeString() : "";

                //log
                //if (result.return_code == 0)
                //{
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                //}
                //else
                //{
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", "resultExecute is null", "");
                //}


                return result;

            }
            catch (Exception ex)
            {
                PdfOnWebModel error = new PdfOnWebModel();
                error.return_code = -1;
                error.return_message = ex.Message;
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBB_GEN_PDF" + ex.Message);

                return error;
            }

        }
    }
}
