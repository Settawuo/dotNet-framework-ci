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
    public class LoadSimS4CommandHandler : ICommandHandler<LoadSimS4DataInsertCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public LoadSimS4CommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
           IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public void Handle(LoadSimS4DataInsertCommand command)
        {
            #region Parameter
            // Return Code
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.Size = 2000;
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            // Return Message
            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            // Input Parameter
            var darr = new OracleParameter();
            darr.ParameterName = "darr";
            ret_msg.Size = 8000;
            darr.OracleDbType = OracleDbType.Varchar2;
            darr.Direction = ParameterDirection.Input;
            darr.Value = command.psp_file;

            var tab_name = new OracleParameter();
            tab_name.ParameterName = "tab_name";
            ret_msg.Size = 2000;
            tab_name.OracleDbType = OracleDbType.Varchar2;
            tab_name.Direction = ParameterDirection.Input;
            tab_name.Value = command.pt_name;
            #endregion

            #region Insert FBBPAYG_SIM
            //InterfaceLogPayGCommand log3 = new InterfaceLogPayGCommand();
            //log3 = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [LoadFileSimCommand]", "call package : PKG_FBBPAYG_LOAD_SIM.p_load_file_sim", "LoadFileSimCommand", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {
                var executeResults = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_SIM.p_insert_sim_table",
                    new
                    {
                        // Parameter Input
                        darr,
                        tab_name,
                        // Parameter Output
                        ret_code,
                        ret_msg
                    });

                //Return
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value == null ? "" : ret_msg.Value.ToSafeString();

                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log3, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
            }
            catch (Exception ex)
            {
                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log3, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_insert_sim_table : " + ex.GetErrorMessage());
            }
            #endregion
        }
    }

    public class LoadSimS4DataUpdateFlagCommandHandler : ICommandHandler<LoadSimS4DataUpdateFlagCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public LoadSimS4DataUpdateFlagCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public void Handle(LoadSimS4DataUpdateFlagCommand command)
        {
            #region Parameter
            // Return Code
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Int32;
            ret_code.Direction = ParameterDirection.Output;

            // Return Message
            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            var p_flag_fbss = new OracleParameter();
            p_flag_fbss.ParameterName = "p_flag_fbss";
            p_flag_fbss.Size = 2000;
            p_flag_fbss.OracleDbType = OracleDbType.Varchar2;
            p_flag_fbss.Direction = ParameterDirection.Input;
            p_flag_fbss.Value = command.flag_fbss;

            var p_serial = new OracleParameter();
            p_serial.ParameterName = "p_serial";
            p_serial.Size = 2000;
            p_serial.OracleDbType = OracleDbType.Varchar2;
            p_serial.Direction = ParameterDirection.Input;
            p_serial.Value = command.serial;

            var p_status = new OracleParameter();
            p_status.ParameterName = "p_status";
            p_status.Size = 2000;
            p_status.OracleDbType = OracleDbType.Varchar2;
            p_status.Direction = ParameterDirection.Input;
            p_status.Value = command.status;
            #endregion

            #region Update Flag FBBPAYG_SIM
            //InterfaceLogPayGCommand log3 = new InterfaceLogPayGCommand();
            //log3 = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [LoadFileSimCommand]", "call package : PKG_FBBPAYG_LOAD_SIM.p_load_file_sim", "LoadFileSimCommand", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {
                var executeResults = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_SIM.p_update_flag_sim",
                    new
                    {
                        // Parameter Input
                        p_flag_fbss,
                        p_serial,
                        p_status,
                        // Parameter Output
                        ret_code,
                        ret_msg
                    });

                //Return
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value == null ? "" : ret_msg.Value.ToSafeString();

                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log3, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
            }
            catch (Exception ex)
            {
                //InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log3, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_update_flag_sim : " + ex.GetErrorMessage());
            }
            #endregion
        }
    }

    public class InsertGenLogCommandHandler : ICommandHandler<InsertGenLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;

        public InsertGenLogCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
        }

        public void Handle(InsertGenLogCommand command)
        {
            #region Parameter
            // Return Message
            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            // Input
            var p_fn_name = new OracleParameter();
            p_fn_name.ParameterName = "fn_name";
            p_fn_name.Size = 2000;
            p_fn_name.OracleDbType = OracleDbType.Varchar2;
            p_fn_name.Direction = ParameterDirection.Input;
            p_fn_name.Value = command.pfn_name;

            var p_f_name = new OracleParameter();
            p_f_name.ParameterName = "f_name";
            p_f_name.Size = 2000;
            p_f_name.OracleDbType = OracleDbType.Varchar2;
            p_f_name.Direction = ParameterDirection.Input;
            p_f_name.Value = command.pf_name;

            var p_t_name = new OracleParameter();
            p_t_name.ParameterName = "t_name";
            p_t_name.Size = 2000;
            p_t_name.OracleDbType = OracleDbType.Varchar2;
            p_t_name.Direction = ParameterDirection.Input;
            p_t_name.Value = command.pt_name;

            var p_in_xml = new OracleParameter();
            p_in_xml.ParameterName = "in_xml";
            p_in_xml.Size = 2000;
            p_in_xml.OracleDbType = OracleDbType.Varchar2;
            p_in_xml.Direction = ParameterDirection.Input;
            p_in_xml.Value = command.pin_xml;

            var p_out_ret = new OracleParameter();
            p_out_ret.ParameterName = "out_ret";
            p_out_ret.Size = 2000;
            p_out_ret.OracleDbType = OracleDbType.Varchar2;
            p_out_ret.Direction = ParameterDirection.Input;
            p_out_ret.Value = command.pout_ret;


            var p_exc_det = new OracleParameter();
            p_exc_det.ParameterName = "exc_det";
            p_exc_det.Size = 2000;
            p_exc_det.OracleDbType = OracleDbType.Varchar2;
            p_exc_det.Direction = ParameterDirection.Input;
            p_exc_det.Value = command.pexc_det;

            var p_out_xml = new OracleParameter();
            p_out_xml.ParameterName = "out_xml";
            p_out_xml.Size = 2000;
            p_out_xml.OracleDbType = OracleDbType.Varchar2;
            p_out_xml.Direction = ParameterDirection.Input;
            p_out_xml.Value = command.pout_xml;

            var p_row_cnt = new OracleParameter();
            p_row_cnt.ParameterName = "row_cnt";
            p_row_cnt.Size = 2000;
            p_row_cnt.OracleDbType = OracleDbType.Varchar2;
            p_row_cnt.Direction = ParameterDirection.Input;
            p_row_cnt.Value = command.prow_cnt;
            #endregion

            #region Insert FBB_INTERFACE_LOG_PAYG
            try
            {
                var executeResults = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_SIM.p_gen_log",
                    new
                    {
                        // Parameter Input
                        p_fn_name,
                        p_f_name,
                        p_t_name,
                        p_in_xml,
                        p_out_ret,
                        p_exc_det,
                        p_out_xml,
                        p_row_cnt,
                        // Parameter Output
                        ret_msg
                    });

                //Return
                command.pret_msg = ret_msg.Value == null ? "" : ret_msg.Value.ToSafeString();
            }
            catch (Exception ex)
            {
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_gen_log : " + ex.GetErrorMessage());
            }
            #endregion
        }
    }

    public class InsertLoadFileLogCommandHandler : ICommandHandler<InsertLoadFileLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;

        public InsertLoadFileLogCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
        }

        public void Handle(InsertLoadFileLogCommand command)
        {
            #region Parameter
            // Return Code
            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Int32;
            ret_code.Direction = ParameterDirection.Output;

            // Return Message
            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            // Input
            var filename_log = new OracleParameter();
            filename_log.ParameterName = "filename_log";
            filename_log.Size = 2000;
            filename_log.OracleDbType = OracleDbType.Varchar2;
            filename_log.Direction = ParameterDirection.Input;
            filename_log.Value = command.filename;

            var filedate = new OracleParameter();
            filedate.ParameterName = "filedate";
            filedate.OracleDbType = OracleDbType.Date;
            filedate.Direction = ParameterDirection.Input;
            filedate.Value = command.filedate;

            var messege_log = new OracleParameter();
            messege_log.ParameterName = "messege_log";
            messege_log.Size = 2000;
            messege_log.OracleDbType = OracleDbType.Varchar2;
            messege_log.Direction = ParameterDirection.Input;
            messege_log.Value = command.message;

            var flag_type = new OracleParameter();
            flag_type.ParameterName = "flag_type";
            flag_type.Size = 3;
            flag_type.OracleDbType = OracleDbType.Varchar2;
            flag_type.Direction = ParameterDirection.Input;
            flag_type.Value = string.IsNullOrEmpty(command.flag_type) ? "N" : command.flag_type.ToUpper();

            #endregion

            #region Insert fbss_load_file_log
            try
            {
                var executeResults = _objService.ExecuteStoredProc("WBB.PKG_FBBPAYG_LOAD_SIM.p_insert_load_file_log",
                    new
                    {
                        // Parameter Input
                        filename_log,
                        filedate,
                        messege_log,
                        flag_type,
                        // Parameter Output
                        ret_code,
                        ret_msg
                    });

                //Return
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value == null ? "" : ret_msg.Value.ToSafeString();
                
            }
            catch (Exception ex)
            {
                _logger.Info("PKG_FBBPAYG_LOAD_SIM.p_insert_load_file_log : " + ex.GetErrorMessage());
            }
            #endregion
        }
    }
}
