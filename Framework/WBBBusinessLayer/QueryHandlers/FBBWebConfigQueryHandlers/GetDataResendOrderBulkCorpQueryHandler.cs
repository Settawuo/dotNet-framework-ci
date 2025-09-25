using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetDataResendOrderBulkCorpQueryHandler : IQueryHandler<GetDataResendOrderBulkCorpQuery, ResendOrderBulkCorpModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ResendOrderBulkCorpModel> _objServiceSubj;
        private readonly IEntityRepository<object> _fbblovRepository;

        public GetDataResendOrderBulkCorpQueryHandler(ILogger logger, IEntityRepository<ResendOrderBulkCorpModel> objServiceSubj
             , IEntityRepository<object> fbblovRepository)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _fbblovRepository = fbblovRepository;
        }

        public ResendOrderBulkCorpModel Handle(GetDataResendOrderBulkCorpQuery query)
        {
            List<ResendOrderBulkCorpModel> executeResult = new List<ResendOrderBulkCorpModel>();
            ResendOrderBulkCorpModel executeResults = new ResendOrderBulkCorpModel();

            try
            {
                var p_bulk_number = new OracleParameter();
                p_bulk_number.ParameterName = "p_bulk_number";
                p_bulk_number.Size = 2000;
                p_bulk_number.OracleDbType = OracleDbType.Varchar2;
                p_bulk_number.Direction = ParameterDirection.Input;
                p_bulk_number.Value = query.p_bulk_number;

                var output_return_code = new OracleParameter();
                output_return_code.ParameterName = "output_return_code";
                output_return_code.OracleDbType = OracleDbType.Int64;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.ParameterName = "output_return_message";
                output_return_message.Size = 2000;
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Direction = ParameterDirection.Output;

                var p_message = new OracleParameter();
                p_message.ParameterName = "p_message";
                p_message.OracleDbType = OracleDbType.RefCursor;
                p_message.Direction = ParameterDirection.Output;

                var p_resend_tech = new OracleParameter();
                p_resend_tech.ParameterName = "p_resend_tech";
                p_resend_tech.OracleDbType = OracleDbType.RefCursor;
                p_resend_tech.Direction = ParameterDirection.Output;

                var p_package_main = new OracleParameter();
                p_package_main.ParameterName = "p_package_main";
                p_package_main.OracleDbType = OracleDbType.RefCursor;
                p_package_main.Direction = ParameterDirection.Output;

                var p_package_discount = new OracleParameter();
                p_package_discount.ParameterName = "p_package_discount";
                p_package_discount.OracleDbType = OracleDbType.RefCursor;
                p_package_discount.Direction = ParameterDirection.Output;


                _logger.Info("StartPKG_FBBBULK_CORP_REGISTER.PROC_RESEND_BULK_NUMBER ");

                var result = _fbblovRepository.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_RESEND_BULK_NUMBER",
                      new object[]
                      {
                          p_bulk_number,
                          //return code    
                          output_return_code      ,
                          output_return_message   ,
                          p_message,
                          p_resend_tech         ,
                          p_package_main   ,
                          p_package_discount

                      });

                if (result != null)
                {
                    executeResults.output_return_code = result[0].ToSafeString();
                    executeResults.output_return_message = result[1].ToSafeString();

                    DataTable resend_msg = (DataTable)result[2];
                    List<ResendPmsg> RESEND_MSG = resend_msg.DataTableToList<ResendPmsg>();
                    executeResults.p_message = RESEND_MSG;

                    DataTable resend_tech = (DataTable)result[3];
                    List<ResendTech> P_RESEND_TECH = resend_tech.DataTableToList<ResendTech>();
                    executeResults.p_resend_tech = P_RESEND_TECH;

                    DataTable package_main = (DataTable)result[4];
                    List<ResendPackMain> P_PACKAGE_MAIN = package_main.DataTableToList<ResendPackMain>();
                    executeResults.p_package_main = P_PACKAGE_MAIN;

                    DataTable package_discount = (DataTable)result[5];
                    List<ResendPackOntopDis> P_PACKAGE_DISCOUNT = package_discount.DataTableToList<ResendPackOntopDis>();
                    executeResults.p_package_discount = P_PACKAGE_DISCOUNT;

                    _logger.Info("EndPKG_FBBBULK_CORP_REGISTER.PROC_RESEND_BULK_NUMBER " + executeResults.output_return_message);
                }
                else
                {
                    _logger.Info("EndPKG_FBBBULK_CORP_REGISTER.PROC_RESEND_BULK_NUMBER : result is null");

                    return null;
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBBULK_CORP_REGISTER.PROC_RESEND_BULK_NUMBER handles : " + ex.Message);

                //executeResults.output_return_code = "-1";
                //executeResults.output_return_message = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
