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
    class PayGTransMatdocLogQueryHandler : IQueryHandler<PayGTransMatdocLogQuery, PayGTransMatdocLogListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PayGTransMatdocLogQuery> _objService;

        public PayGTransMatdocLogQueryHandler(ILogger logger, IEntityRepository<PayGTransMatdocLogQuery> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public PayGTransMatdocLogListResult Handle(PayGTransMatdocLogQuery query)
        {

            PayGTransMatdocLogListResult result = new PayGTransMatdocLogListResult();

            try
            {
                _logger.Info("PayGTransMatdocLogQueryHandler : Start.");

                var f_pFileName = new OracleParameter();
                f_pFileName.OracleDbType = OracleDbType.Varchar2;
                f_pFileName.Value = query.pFileName;
                f_pFileName.Size = 100;
                f_pFileName.Direction = ParameterDirection.Input;

                var f_pFileDate = new OracleParameter();
                f_pFileDate.OracleDbType = OracleDbType.Date;
                f_pFileDate.Value = query.pFileDate;
                f_pFileDate.Direction = ParameterDirection.Input;

                var f_pMessage = new OracleParameter();
                f_pMessage.OracleDbType = OracleDbType.Varchar2;
                f_pMessage.Value = query.pMessage;
                f_pMessage.Size = 2000;
                f_pMessage.Direction = ParameterDirection.Input;

                var f_pLogDate = new OracleParameter();
                f_pLogDate.OracleDbType = OracleDbType.TimeStamp;
                f_pLogDate.Value = query.pLogDate;
                f_pLogDate.Direction = ParameterDirection.Input;

                var f_pFlagType = new OracleParameter();
                f_pFlagType.OracleDbType = OracleDbType.Varchar2;
                f_pFlagType.Value = query.pFlagType;
                f_pFlagType.Size = 1;
                f_pFlagType.Direction = ParameterDirection.Input;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                //var my_cur = new OracleParameter();
                //my_cur.ParameterName = "my_Cur";
                //my_cur.OracleDbType = OracleDbType.RefCursor;
                //my_cur.Direction = ParameterDirection.Output;

                //_objService.SqlQuery("delete from DIR_LIST_F; commit;");
                //List<PAYGTransAirnetFileList> data = new List<PAYGTransAirnetFileList>();
                //List<PAYGTransAirnetFileList> executeResult = _objService.ExecuteReadStoredProc("WBB.pkg_fbbpayg_airnet_inv_rec.p_gen_log_matdoc",
                _objService.ExecuteReadStoredProc("WBB.pkg_fbbpayg_airnet_inv_rec.p_gen_log_matdoc",
                            new
                            {
                                f_pFileName = f_pFileName,
                                f_pFileDate = f_pFileDate,
                                f_pMessage = f_pMessage,
                                f_pLogDate = f_pLogDate,
                                f_pFlagType = f_pFlagType,
                                ret_code = ret_code,
                                ret_msg = ret_msg
                            }).ToList();

                result.Return_Code = Convert.ToInt16(ret_code.Value.ToString());
                result.Return_Desc = ret_msg.Value.ToString();
                //result.Data = executeResult;
                return result;

            }
            catch (Exception ex)
            {
                _logger.Info("PayGTransMatdocLogQueryHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;
            }

        }
    }
}
