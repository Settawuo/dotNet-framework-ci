using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{

    public class CommandPerFormnceByRegion : ICommandHandler<CommmandReportByRegionCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_RPT_LOG> _FBB_RPT_LOG;



        public CommandPerFormnceByRegion(ILogger logger, IWBBUnitOfWork uow, IAirNetEntityRepository<string> objService,
            IEntityRepository<FBB_RPT_LOG> FBB_RPT_LOG)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _FBB_RPT_LOG = FBB_RPT_LOG;

        }
        public void Handle(CommmandReportByRegionCommand command)
        {
            try
            {
                _logger.Info("1");
                #region  Addd log
                if (command.Flag_Add_log_Rpt == "Add_R")
                {

                    var FBB_RPT_LOG_Model = new FBB_RPT_LOG();
                    FBB_RPT_LOG_Model.CREATED_BY = command.CREATED_BY.ToSafeString();
                    FBB_RPT_LOG_Model.CREATED_DATE = command.CREATED_DATE;
                    FBB_RPT_LOG_Model.REPORT_CODE = command.REPORT_CODE.ToSafeString();
                    FBB_RPT_LOG_Model.REPORT_DESC = command.REPORT_DESC.ToSafeString();
                    FBB_RPT_LOG_Model.REPORT_NAME = command.REPORT_NAME.ToSafeString();
                    FBB_RPT_LOG_Model.REPORT_PARAMETER = command.REPORT_PARAMETER.ToSafeString();
                    FBB_RPT_LOG_Model.REPORT_STATUS = command.REPORT_STATUS.ToSafeString();
                    FBB_RPT_LOG_Model.UPDATED_BY = command.CREATED_BY.ToSafeString();
                    FBB_RPT_LOG_Model.UPDATED_DATE = command.CREATED_DATE;
                    FBB_RPT_LOG_Model.REPORT_CONDITION = "BY_REQUEST";

                    _FBB_RPT_LOG.Create(FBB_RPT_LOG_Model);
                    _uow.Persist();

                    _logger.Info("Insert 2 Suess");

                    command.Return_Code = 1;
                    command.Return_Desc = "Success";

                    _logger.Info("Insert 3 Success");

                    //var ret_code = new OracleParameter();
                    //ret_code.OracleDbType = OracleDbType.Decimal;
                    //ret_code.Direction = ParameterDirection.Output;

                    var ret_report_status = new OracleParameter();
                    ret_report_status.OracleDbType = OracleDbType.Varchar2;
                    ret_report_status.Size = 2000;
                    ret_report_status.Direction = ParameterDirection.Output;

                    var v_error_msg = new OracleParameter();
                    v_error_msg.OracleDbType = OracleDbType.Varchar2;
                    v_error_msg.Size = 2000;
                    v_error_msg.Direction = ParameterDirection.Output;

                    var v_file_name = new OracleParameter();
                    v_file_name.OracleDbType = OracleDbType.Varchar2;
                    v_file_name.Size = 2000;
                    v_file_name.Direction = ParameterDirection.Output;

                    var outp = new List<object>();
                    var paramOut = outp.ToArray();

                    _logger.Info("4 Tinglee : " + command.CREATED_BY.ToSafeString() + " : " + command.REPORT_CODE.ToSafeString() + " : " + command.REPORT_PARAMETER.ToSafeString());

                    var executeResult = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_FBB_RPTPORT002.PROC_MAIN_FBB_RPTPORT002",
                    out paramOut,
                        new
                        {
                            p_create_by = command.CREATED_BY.ToSafeString(),
                            p_report_code = command.REPORT_CODE.ToSafeString(),
                            p_report_parameter = command.REPORT_PARAMETER.ToSafeString(),

                            //return 
                            ret_report_status = ret_report_status,
                            ret_file_path = v_error_msg,
                            ret_file_name = v_file_name
                        });
                    _logger.Info("ResultData" + executeResult.ToString());

                    command.Return_Code = 1;
                    command.Return_Desc = "Success";


                    FBB_RPT_LOG_Model.REPORT_STATUS = ret_report_status.Value.ToString();
                    FBB_RPT_LOG_Model.FILE_PATH = v_error_msg.Value.ToString();
                    FBB_RPT_LOG_Model.FILE_NAME = v_file_name.Value.ToString();
                    FBB_RPT_LOG_Model.UPDATED_BY = "WBB";
                    FBB_RPT_LOG_Model.UPDATED_DATE = System.DateTime.Now;
                    FBB_RPT_LOG_Model.REPORT_STATUS = ret_report_status.Value.ToString();
                    _FBB_RPT_LOG.Update(FBB_RPT_LOG_Model);
                    _uow.Persist();


                    _logger.Info("5  Success" + _uow);

                }
                #endregion


                else
                {

                    command.Return_Code = -1;
                    command.Return_Desc = "Not Success";


                    _logger.Info("Error occured when handle CaommandPerformanceByRegion Command" + command.REPORT_CODE + " And Error Fild" + command.REPORT_DESC);



                }


            }
            catch (Exception ex)
            {

                _logger.Info("Error occured when handle Region Command");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);



            }



        }
    }
}
