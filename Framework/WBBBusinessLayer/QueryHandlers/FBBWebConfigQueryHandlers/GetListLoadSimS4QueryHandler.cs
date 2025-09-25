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
    public class GetListLoadSimS4QueryHandler : IQueryHandler<GetListSimDataQuery, List<LoadSimS4GetDataQueryModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<LoadSimS4GetDataQueryModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public GetListLoadSimS4QueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<LoadSimS4GetDataQueryModel> objService,
        IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public List<LoadSimS4GetDataQueryModel> Handle(GetListSimDataQuery query)
        {
            // Return
            var return_code = new OracleParameter();
            return_code.ParameterName = "ret_code";
            return_code.OracleDbType = OracleDbType.Int32;
            return_code.Direction = ParameterDirection.Output;

            var return_msg = new OracleParameter();
            return_msg.ParameterName = "ret_msg";
            return_msg.OracleDbType = OracleDbType.Varchar2;
            return_msg.Size = 2000;
            return_msg.Direction = ParameterDirection.Output;

            // Parameter
            var p_process_name = new OracleParameter();
            p_process_name.ParameterName = "p_process_name";
            p_process_name.OracleDbType = OracleDbType.Varchar2;
            p_process_name.Size = 2000;
            p_process_name.Direction = ParameterDirection.Input;
            p_process_name.Value = query.p_sheet_name;

            var result_lookup_name_cur = new OracleParameter
            {
                ParameterName = "result_load_sim_data_cur",
                OracleDbType = OracleDbType.RefCursor,
                Direction = ParameterDirection.Output
            };


            #region Query Sim Data
            //InterfaceLogPayGCommand log3 = new InterfaceLogPayGCommand();
            //log3 = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [LoadFileSimCommand]", "call package : PKG_FBBPAYG_LOAD_SIM.p_load_file_sim", "LoadFileSimCommand", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {
                //p_load_file_sim
                var executeResult = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_FBBPAYG_LOAD_SIM.p_get_sim_data",
                    new
                    {
                        p_process_name,
                        return_code,
                        return_msg,
                        result_lookup_name_cur
                    }).ToList();

                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log3, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
                return executeResult;
                
            }
            catch (Exception ex)
            {
                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log3, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_get_sim_data : " + ex.GetErrorMessage());
                return null;
            }
            #endregion
        }
    }
}
