using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset
{
    public class HistoryLogCommandHandler : ICommandHandler<HistoryLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<string> _objService;

        public HistoryLogCommandHandler(ILogger logger,
            IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> historyLog,
            IWBBUnitOfWork uow,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _historyLog = historyLog;
            _objService = objService;
            _uow = uow;
        }

        public void Handle(HistoryLogCommand command)
        {
            var log = HistoryLogHelper.Log(_uow, _historyLog, command);
        }

    }

    public static class HistoryLogHelper
    {
        public static FBB_FIXED_ASSET_HISTORY_LOG Log(
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> historyLog,
            HistoryLogCommand command)
        {
            try
            {
                var data = new FBB_FIXED_ASSET_HISTORY_LOG();

                if (!command.HISTORY_ID.Equals(0)) data.HISTORY_ID = command.HISTORY_ID;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var in_foa = new OracleParameter();
                in_foa.OracleDbType = OracleDbType.Clob;
                in_foa.Value = command.IN_FOA;
                in_foa.Direction = ParameterDirection.Input;

                var installation = new OracleParameter();
                installation.OracleDbType = OracleDbType.Clob;
                installation.Value = command.INSTALLATION;
                installation.Direction = ParameterDirection.Input;

                var in_sap = new OracleParameter();
                in_sap.OracleDbType = OracleDbType.Clob;
                in_sap.Value = command.IN_SAP;
                in_sap.Direction = ParameterDirection.Input;

                var out_sap = new OracleParameter();
                out_sap.OracleDbType = OracleDbType.Clob;
                out_sap.Value = command.OUT_SAP;
                out_sap.Direction = ParameterDirection.Input;

                var out_foa = new OracleParameter();
                out_foa.OracleDbType = OracleDbType.Clob;
                out_foa.Value = command.OUT_FOA;
                out_foa.Direction = ParameterDirection.Input;

                var p_return_code = new OracleParameter();
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Size = 2000;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Size = 2000;
                p_return_message.Direction = ParameterDirection.Output;

                var p_out_history_id = new OracleParameter();
                p_out_history_id.OracleDbType = OracleDbType.Varchar2;
                p_out_history_id.Size = 2000;
                p_out_history_id.Direction = ParameterDirection.Output;


                if (command.ActionBy.ToUpper().Equals("IN_FOA"))
                {
                    data.IN_FOA = command.IN_FOA;
                    data.REQUEST_STATUS = "Pending";
                    data.CREATED_BY = command.CREATED_BY;
                    data.CREATED_DATE = DateTime.Now;
                    historyLog.Create(data);
                    uow.Persist();
                    data.HISTORY_ID = data.HISTORY_ID;
                }
                else
                {
                    var executeResult = historyLog.ExecuteStoredProc("WBB.pkg_fbb_foa_order_management.foa_history_log",
                        out paramOut,
                        new
                        {
                            p_event = command.ActionBy.ToUpper(),
                            p_history_id = command.HISTORY_ID,
                            p_trans_id = command.TRANSACTION_ID,
                            p_in_foa = in_foa,
                            p_out_foa = out_foa,
                            p_installation = installation,
                            p_in_sap = in_sap,
                            p_out_sap = out_sap,
                            p_msg = command.REQUEST_STATUS,
                            p_create_by = command.CREATED_BY,
                            //// return 
                            p_return_code = p_return_code,
                            p_return_message = p_return_message,
                            p_out_history_id = p_out_history_id
                        });
                }
                //else if (command.ActionBy.ToUpper().Equals("INSTALLATION"))
                //{
                //    data.HISTORY_ID = command.HISTORY_ID;
                //    data.TRANSACTION_ID = command.TRANSACTION_ID;
                //    data.INSTALLATION = command.INSTALLATION;
                //    var his = historyLog.Get(a => a.HISTORY_ID == command.HISTORY_ID);
                //}
                //else if (command.ActionBy.ToUpper().Equals("IN_SAP"))
                //{
                //    data.IN_SAP = command.IN_SAP;
                //    historyLog.Update(data);
                //}
                //else if (command.ActionBy.ToUpper().Equals("OUT_SAP"))
                //{
                //    data.OUT_SAP = command.OUT_SAP;
                //    historyLog.Update(data);
                //}
                //else if (command.ActionBy.ToUpper().Equals("OUT_FOA"))
                //{
                //    data.OUT_FOA = command.OUT_FOA;
                //    data.REQUEST_STATUS = "Success";
                //    historyLog.Update(data);
                //}
                //else if (command.ActionBy.ToUpper().Equals("ERROR"))
                //{
                //    data.REQUEST_STATUS = command.REQUEST_STATUS;
                //    historyLog.Update(data);
                //}
                //else
                //{
                //    data.REQUEST_STATUS = "HistoryLogCommand Error";
                //    throw new Exception("HistoryLogCommand Error");
                //}
                return data;
            }
            catch (Exception ex)
            {
                return new FBB_FIXED_ASSET_HISTORY_LOG();
            }
        }
    }

}
