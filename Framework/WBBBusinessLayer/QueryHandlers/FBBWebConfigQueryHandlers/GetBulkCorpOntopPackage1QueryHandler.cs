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
    public class GetBulkCorpOntopPackage1QueryHandler : IQueryHandler<GetBulkCorpOntopPackage1Query, List<ReturnOntopPackageList>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ReturnOntopPackageList> _objService;

        public GetBulkCorpOntopPackage1QueryHandler(ILogger logger, IAirNetEntityRepository<ReturnOntopPackageList> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<ReturnOntopPackageList> Handle(GetBulkCorpOntopPackage1Query query)
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
                res_data.ParameterName = "p_res_package_ontop_1";
                res_data.OracleDbType = OracleDbType.RefCursor;
                res_data.Direction = ParameterDirection.Output;

                List<ReturnOntopPackageList> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_QUERY_PACKAGE.PROD_PACKAGE_ONTOP_1",
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
