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
    public class DeleteRegisterBulkCorpQueryHandler : IQueryHandler<DeleteRegisterBulkCorpQuery, returnDelExcel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<returnDelExcel> _objService;

        public DeleteRegisterBulkCorpQueryHandler(ILogger logger, IEntityRepository<returnDelExcel> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public returnDelExcel Handle(DeleteRegisterBulkCorpQuery query)
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


                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_BACK_EXCEL",
                            new
                            {
                                p_user = query.p_bulk_number,


                                //  return code
                                output_bulk_no = output_bulk_no,
                                output_return_code = output_return_code,
                                output_return_message = output_return_message

                            }).ToList();

                returnDelExcel Resultdata = new returnDelExcel();

                var ret_bulk_no = output_bulk_no.Value.ToString();
                Resultdata.output_bulk_no = ret_bulk_no;

                if (null == ret_bulk_no || "" == ret_bulk_no)
                {
                    return null;
                }

                var ret_code = output_return_code.Value.ToString();
                Resultdata.output_return_code = ret_code;

                if (ret_code == null || ret_code == "")
                {
                    return null;
                }
                var ret_msg = output_return_message.Value.ToString();
                Resultdata.output_return_message = ret_msg;

                if (ret_code == "0") // return 0 pass value to screen 
                {
                    _logger.Info("EndPROC_BACK_EXCEL " + output_return_message);
                    return Resultdata;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_BACK_EXCEL output msg: " + output_return_message);
                    return null;

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_BACK_EXCEL" + ex.Message);

                return null;
            }
        }

    }
}
