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
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetBulkCorpSearchBulkCorpQueryHandler : IQueryHandler<GetBulkCorpSearchBulkCorpQuery, List<SearchBulkCorpList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SearchBulkCorpList> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public GetBulkCorpSearchBulkCorpQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<SearchBulkCorpList> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public List<SearchBulkCorpList> Handle(GetBulkCorpSearchBulkCorpQuery query)
        {
            try
            {
                _logger.Info("GetBulkCorpSearchBulkCorpQueryHandler Start");
                var output_return_code = new OracleParameter();
                output_return_code.ParameterName = "OUTPUT_return_code";
                output_return_code.OracleDbType = OracleDbType.Decimal;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.ParameterName = "OUTPUT_return_message";
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Size = 2000;
                output_return_message.Direction = ParameterDirection.Output;

                var p_search_order_number = new OracleParameter();
                p_search_order_number.ParameterName = "p_SEARCH_ORDER_NUMBER";
                p_search_order_number.OracleDbType = OracleDbType.RefCursor;
                p_search_order_number.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();


                List<SearchBulkCorpList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SEARCH_BULK_NUMBER",
                    new
                    {
                        p_bulk_number = query.P_BULK_NUMBER,
                        p_ca_number = query.P_CA_NUMBER,
                        p_tax_id = query.P_TAX_ID,

                        // return code
                        output_return_code = output_return_code,
                        output_return_message = output_return_message,
                        p_search_order_number = p_search_order_number

                    }).ToList();

                if (output_return_code.Value.ToSafeString() == "0") // return 0 pass value to screen 
                {
                    _logger.Info("End WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SEARCH_BULK_NUMBER output msg: " + query.output_return_message);
                    return executeResult;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SEARCH_BULK_NUMBER output msg: " + output_return_message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SEARCH_BULK_NUMBER" + ex.Message);
                return null;
            }

        }
    }
}
