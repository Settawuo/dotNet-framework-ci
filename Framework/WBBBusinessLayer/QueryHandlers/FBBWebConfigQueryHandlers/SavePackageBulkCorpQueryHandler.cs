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
    public class SavePackageBulkCorpQueryHandler : IQueryHandler<SavePackageBulkCorpQuery, returnregisterpkgData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<airregistpkgList> _objService;

        public SavePackageBulkCorpQueryHandler(ILogger logger, IEntityRepository<airregistpkgList> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public returnregisterpkgData Handle(SavePackageBulkCorpQuery query)
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

                var air_regist_package_array = new OracleParameter();
                air_regist_package_array.ParameterName = "air_regist_package_array";
                air_regist_package_array.OracleDbType = OracleDbType.RefCursor;
                air_regist_package_array.Direction = ParameterDirection.Output;


                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_AIR_REGIST_PACKAGE_ARRAY",
                            new
                            {
                                p_bulk_number = query.p_bulk_number,

                                //  return code
                                output_bulk_no = output_bulk_no,
                                output_return_code = output_return_code,
                                output_return_message = output_return_message,
                                air_regist_package_array = air_regist_package_array

                            }).ToList();


                returnregisterpkgData ResultData = new returnregisterpkgData();
                ResultData.airpkgList = executeResult;

                var ret_bulk_no = output_bulk_no.Value.ToString();

                if (null == ret_bulk_no || "" == ret_bulk_no)
                {
                    return null;
                }
                var out_code = output_return_code.Value.ToString();
                if (out_code == null || out_code == "")
                {
                    return null;
                }
                var out_msg = output_return_message.Value.ToString();

                ResultData.output_bulk_no = ret_bulk_no;
                ResultData.output_return_code = out_code;
                ResultData.output_return_message = out_msg;

                if (out_code == "0") // return 0 pass value to screen 
                {
                    _logger.Info("EndPROC_AIR_REGIST_PACKAGE_ARRAY" + out_msg);
                    return ResultData;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_AIR_REGIST_PACKAGE_ARRAY output msg: " + out_msg);
                    return null;

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_AIR_REGIST_PACKAGE_ARRAY" + ex.Message);

                return null;
            }
        }

    }
}
