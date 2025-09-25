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

    public class GetFBBPAYGExpireSimReportQueryHandler : IQueryHandler<GetListExpireSimDataQuery, List<LoadExpireSimS4GetDataQueryModel>>
    {

        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<LoadExpireSimS4GetDataQueryModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public GetFBBPAYGExpireSimReportQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<LoadExpireSimS4GetDataQueryModel> objService,
        IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public List<LoadExpireSimS4GetDataQueryModel> Handle(GetListExpireSimDataQuery query)
        {
            // Return
            var return_code = new OracleParameter();
            return_code.ParameterName = "return_code";
            return_code.OracleDbType = OracleDbType.BinaryFloat;
            return_code.Direction = ParameterDirection.Output;

            var return_msg = new OracleParameter();
            return_msg.ParameterName = "return_msg";
            return_msg.OracleDbType = OracleDbType.Varchar2;
            return_msg.Size = 2000;
            return_msg.Direction = ParameterDirection.Output;

            // Parameter
            var p_sheet_name = new OracleParameter();
            p_sheet_name.ParameterName = "p_sheet_name";
            p_sheet_name.OracleDbType = OracleDbType.Varchar2;
            p_sheet_name.Size = 2000;
            p_sheet_name.Direction = ParameterDirection.Input;

            var result_lookup_name_cur = new OracleParameter
            {
                ParameterName = "result_buffer_data",
                OracleDbType = OracleDbType.RefCursor,
                Direction = ParameterDirection.Output
            };


            #region Query Sim Data
            //InterfaceLogPayGCommand log3 = new InterfaceLogPayGCommand();
            //log3 = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [LoadFileSimCommand]", "call package : PKG_FBBPAYG_LOAD_SIM.p_load_file_sim", "LoadFileSimCommand", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {
                //p_load_file_sim
                var executeResult = _objService.SqlQuery(
                            string.Format(@""+ query.sheet_query + "", "WBB"));

                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log3, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
                var DATA_BUFFER = executeResult.Select(z => new LoadExpireSimS4GetDataQueryModel()
                {
                    data_buffer = z.data_buffer,

                })
                .ToList();

                return DATA_BUFFER;

            }
            catch (Exception ex)
            {
                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log3, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_insert_table : " + ex.GetErrorMessage());
                return null;
            }
            #endregion
        }

        
    }
}
