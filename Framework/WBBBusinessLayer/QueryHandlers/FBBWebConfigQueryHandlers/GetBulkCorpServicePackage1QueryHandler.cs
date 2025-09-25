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
    public class GetBulkCorpServicePackage1QueryHandler : IQueryHandler<GetBulkCorpServicePackage1Query, List<ReturnServicePackageList>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ReturnServicePackageList> _objService;

        public GetBulkCorpServicePackage1QueryHandler(ILogger logger, IAirNetEntityRepository<ReturnServicePackageList> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<ReturnServicePackageList> Handle(GetBulkCorpServicePackage1Query query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "output_return_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "output_return_message";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var res_data = new OracleParameter();
                res_data.ParameterName = "p_res_package_service_1";
                res_data.OracleDbType = OracleDbType.RefCursor;
                res_data.Direction = ParameterDirection.Output;

                List<ReturnServicePackageList> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_QUERY_PACKAGE.PROD_PACKAGE_SERVICE_1",
                            new
                            {
                                Accnt_Cat = query.AccntCat,


                                //  return code
                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                res_data = res_data

                            }).ToList();

                if (ret_code.Value.ToString() == "0")
                {
                    return executeResult;

                }
                else
                {
                    _logger.Info(ret_code + " " + ret_msg);
                    return null;
                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                return null;
            }
        }
    }
}
