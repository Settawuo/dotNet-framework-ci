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
    class PAYGTransAirnetQueryHandler : IQueryHandler<PAYGTransAirnetQuery, PAYGTransAirnetListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PAYGTransAirnetFileList> _objService;

        public PAYGTransAirnetQueryHandler(ILogger logger, IEntityRepository<PAYGTransAirnetFileList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public PAYGTransAirnetListResult Handle(PAYGTransAirnetQuery query)
        {

            PAYGTransAirnetListResult result = new PAYGTransAirnetListResult();

            try
            {
                _logger.Info("PAYGTransAirnetQueryHandler : Start.");

                var f_list = new OracleParameter();
                f_list.OracleDbType = OracleDbType.Varchar2;
                f_list.Value = query.f_list;
                f_list.Direction = ParameterDirection.Input;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var my_cur = new OracleParameter();
                my_cur.ParameterName = "my_Cur";
                my_cur.OracleDbType = OracleDbType.RefCursor;
                my_cur.Direction = ParameterDirection.Output;

                //_objService.SqlQuery("delete from DIR_LIST_F; commit;");
                //List<PAYGTransAirnetFileList> data = new List<PAYGTransAirnetFileList>();
                List<PAYGTransAirnetFileList> executeResult = _objService.ExecuteReadStoredProc("WBB.pkg_fbbpayg_airnet_inv_rec.p_fetch_data",
                            new
                            {
                                f_list = f_list,
                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                my_cur = my_cur
                            }).ToList();

                result.Return_Code = Convert.ToInt16(ret_code.Value.ToString());
                result.Return_Desc = ret_msg.Value.ToString();
                result.Data = executeResult;
                return result;

            }
            catch (Exception ex)
            {
                _logger.Info("PAYGTransAirnetQueryHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;
            }

        }

    }
}
