using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    //class TranMatDocGetFBSSLogQueryHandler
    //{
    //}

    public class TranMatDocGetFBSSLogQueryHandler : IQueryHandler<TranMatDocGetFBSSLogQuery, List<GetTranMatDocGetFBSSLogQuery>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<GetTranMatDocGetFBSSLogQuery> _objService;

        public TranMatDocGetFBSSLogQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<GetTranMatDocGetFBSSLogQuery> objService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
        }

        public List<GetTranMatDocGetFBSSLogQuery> Handle(TranMatDocGetFBSSLogQuery query)
        {
            // Return Code
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "FILE_NAME";
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            // Return Message
            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "FLAG_TYPE";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            // Input
            var filename_log = new OracleParameter();
            filename_log.ParameterName = "filename";
            filename_log.Size = 2000;
            filename_log.OracleDbType = OracleDbType.Varchar2;
            filename_log.Direction = ParameterDirection.Input;
            filename_log.Value = query.inbound_filename;
            // Input
            var comDayCallback_log = new OracleParameter();
            filename_log.ParameterName = "date";
            filename_log.Size = 2000;
            filename_log.OracleDbType = OracleDbType.Varchar2;
            filename_log.Direction = ParameterDirection.Input;
            filename_log.Value = query.inbound_comDayCallback;

            try
            {
                //p_load_file_sim
                //  var executeResult = _objService.SqlQuery(
                //      string.Format(@"select FILE_NAME,FLAG_TYPE from wbb.fbss_load_file_log WHERE FILE_NAME like '%" + query.inbound_filename + "%' AND file_date >= trunc(sysdate - +query.comDayCallback) and file_date <= trunc(sysdate)  AND FLAG_TYPE = 'Y'", "WBB"));


                //var executeResult = _objService.SqlQuery($@"
                //SELECT FILE_NAME, FLAG_TYPE 
                //FROM wbb.fbss_load_file_log 
                //WHERE FILE_NAME LIKE '%{query.inbound_filename}%' 
                //  AND file_date >= trunc(sysdate - {query.inbound_comDayCallback}) 
                //  AND file_date <= trunc(sysdate)  
                //  AND FLAG_TYPE = 'Y'");




                var executeResult = _objService.SqlQuery($@"
                SELECT FILE_NAME, FLAG_TYPE 
                FROM wbb.fbss_load_file_log 
                WHERE FILE_NAME LIKE '%{query.inbound_filename}%' 
                AND file_date >= trunc(sysdate - {query.inbound_comDayCallback}) 
                AND file_date <= trunc(sysdate)");


                // string.Format(@"select FILE_NAME,FLAG_TYPE from wbb.fbss_load_file_log WHERE FILE_NAME like '%" + query.inbound_filename + "%'  AND FLAG_TYPE = 'Y'", "WBB"));
                //select file_name from wbb.fbss_load_file_log where file_name like 'MATDOC-%' and file_date >= trunc(sysdate - DayCallBack) and file_date <= trunc(sysdate) and flag_type = 'Y'

                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log3, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
                var DATA_BUFFER = executeResult.Select(z => new GetTranMatDocGetFBSSLogQuery()
                {
                    file_name = z.file_name,
                    flag_type = z.flag_type

                })
                .ToList();

                return DATA_BUFFER;

            }
            catch (Exception ex)
            {
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_insert_table : " + ex.GetErrorMessage());
                return null;
            }
            
        }
    }
}
