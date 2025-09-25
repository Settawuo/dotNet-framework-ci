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
    public class GetBulkCorpOntopDiscountPackageQueryHandler : IQueryHandler<GetBulkCorpOntopDiscountPackageQuery, List<ReturnPackageList>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ReturnPackageList> _objService;

        public GetBulkCorpOntopDiscountPackageQueryHandler(ILogger logger, IAirNetEntityRepository<ReturnPackageList> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<ReturnPackageList> Handle(GetBulkCorpOntopDiscountPackageQuery query)
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
                res_data.ParameterName = "p_res_package_discount";
                res_data.OracleDbType = OracleDbType.RefCursor;
                res_data.Direction = ParameterDirection.Output;

                List<ReturnPackageList> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_QUERY_PACKAGE.PROD_ONTOP_DISCOUNT",
                            new
                            {
                                Accnt_Cat = query.AccntCat,
                                Tech_no = query.Techno,
                                Pack_Code = query.PackCode,
                                MainPack_Code = query.MainPackCode,

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
