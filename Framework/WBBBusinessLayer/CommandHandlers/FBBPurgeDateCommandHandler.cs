using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class FBBPurgeDateCommandHandler : ICommandHandler<FBBPurgeDateCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPurgeGetDateTBL> _objService;
        private readonly IEntityRepository<object> _objServiceRe;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _interfaceLog;

        public FBBPurgeDateCommandHandler(ILogger logger, IEntityRepository<FBBPurgeGetDateTBL> objService, IEntityRepository<object> objServiceRe
            , IWBBUnitOfWork uow
            , IEntityRepository<FBB_INTERFACE_LOG_PAYG> interfaceLog)
        {
            _logger = logger;
            _objService = objService;
            _objServiceRe = objServiceRe;
            _uow = uow;
            _interfaceLog = interfaceLog;
        }

        public void Handle(FBBPurgeDateCommand command)
        {
            _logger.Info("CALL FBBPurgeDateCommandHandler START");
            InterfaceLogPayGCommand log = null;
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _interfaceLog, "WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data", ""
                , "FBBPurgeDateCommand", "FBBPurgeDateCommandHandler", "", "FBB", "Batch PurgeData");
            try
            {
                List<FBBPurgeGetDateTBL> DATAs = new List<FBBPurgeGetDateTBL>();
                List<returnErrorFBBPurgeGetDateTBL> ReturnPurge = new List<returnErrorFBBPurgeGetDateTBL>();

                var out_ref_cur = new OracleParameter();
                out_ref_cur.ParameterName = "out_ref_cur";
                out_ref_cur.OracleDbType = OracleDbType.RefCursor;
                out_ref_cur.Direction = ParameterDirection.Output;

                _logger.Info("CALL WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data START");
                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data",
                       new
                       {
                           //OUT
                           out_ref_cur = out_ref_cur
                       }).ToList();

                _logger.Info("CALL WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data END");


                _logger.Info("LOOP WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data START");

                if (executeResult != null)
                {
                    foreach (var item in executeResult)
                    {
                        var ret_code = new OracleParameter
                        {
                            ParameterName = "ret_code",
                            OracleDbType = OracleDbType.Decimal,
                            Direction = ParameterDirection.Output
                        };

                        var p_con_id = new OracleParameter
                        {
                            ParameterName = "p_con_id",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = ParameterDirection.Input,
                            Value = item.CON_ID.ToSafeString()
                        };

                        var ret_msg = new OracleParameter
                        {
                            ParameterName = "ret_msg",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = ParameterDirection.Output,
                            Size = 2000
                        };

                        _logger.Info("CALL WBB.PKG_FBBPAYG_PURGE_DATA.p_purge_data CON_TYPE: " + item.CON_TYPE + " CON_NAME: " + item.CON_NAME + " START");
                        var PurgeResult = _objServiceRe.ExecuteStoredProc("WBB.PKG_FBBPAYG_PURGE_DATA.p_purge_data",
                           new
                           {
                               //IN
                               p_con_id = p_con_id,
                               //OUT
                               ret_code = ret_code,
                               ret_msg = ret_msg
                           });
                        _logger.Info("CALL WBB.PKG_FBBPAYG_PURGE_DATA.p_purge_data END");
                        command.ret_code = ret_code.Value == null ? "0" : ret_code.Value.ToString();
                        command.ret_msg = ret_msg.Value == null ? "" : ret_msg.Value.ToString();

                        _logger.Info("Call WBB.PKG_FBBPAYG_PURGE_DATA.p_purge_data: ret_code: " + command.ret_code + " ret_msg: " + command.ret_msg + " CON_TYPE: " + item.CON_TYPE + " CON_NAME: " + item.CON_NAME);
                        if (command.ret_code.ToSafeInteger() != 0)
                        {
                            ReturnPurge.Add(new returnErrorFBBPurgeGetDateTBL
                            {
                                CON_ID = item.CON_ID.ToSafeString(),
                                CON_NAME = item.CON_NAME,
                                CON_TYPE = item.CON_TYPE,
                                ret_code = command.ret_code,
                                ret_msg = command.ret_msg
                            });
                        }
                    }

                    _logger.Info("LOOP WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data END");
                    if (ReturnPurge.Count > 0)
                    {
                        command.iserror = ReturnPurge;
                    }
                }
                else
                {
                    _logger.Info("LOOP WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data : Data not found.");
                    _logger.Info("LOOP WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data END");
                    //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _interfaceLog, command, log, "Data not found.", "", "");
                }



                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _interfaceLog, command, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info("CALL ERROR MSG : " + ex.Message);
                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _interfaceLog, command, log, "Failed", ex.Message, "");
            }
        }
    }
}
