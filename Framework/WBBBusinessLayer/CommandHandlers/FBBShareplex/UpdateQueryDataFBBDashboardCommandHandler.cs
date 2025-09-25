using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBShareplex;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBBusinessLayer.CommandHandlers.FBBShareplex
{
    public class UpdateQueryDataFBBDashboardCommandHandler : ICommandHandler<UpdateQueryDataFBBDashboardCommand>
    {
        private readonly ILogger _logger;
        private readonly IFBBShareplexEntityRepository<UpdateQueryDataFBBDashboardCommand> _fbbShareplexRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public UpdateQueryDataFBBDashboardCommandHandler(ILogger logger,
            IFBBShareplexEntityRepository<UpdateQueryDataFBBDashboardCommand> fbbShareplexRepository,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _fbbShareplexRepository = fbbShareplexRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(UpdateQueryDataFBBDashboardCommand command)
        {
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start UpdateQueryDataFBBDashboardCommandHandler");

                var executeResultsDynamic = this.GetType().GetMethod(command.func, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[] { command });
                //var execute1 = QueryFBBDashboardRegRegion(command);
                //var execute2 = QueryFBBDashboardRegProv(command);
                //var execute3 = QueryFBBDashboardRegDis(command);
                //var execute4 = QueryFBBDashboardRegSubdis(command);
                //var execute5 = QueryFBBDashboardRegTeam(command);
                //var execute6 = QueryFBBDashboardCovRegion(command);
                //var execute7 = QueryFBBDashboardCovProv(command);
                //var execute8 = QueryFBBDashboardCovDis(command);
                //var execute9 = QueryFBBDashboardCovSubdis(command);
                //var execute10 = QueryFBBDashboardCovTeam(command);
                //var execute11 = QueryFBBDashboardRegPack(command);
                //var execute12 = QueryFBBDashboardCovCust(command);
                //var execute13 = QueryFBBDashboardRegCust(command);

                _logger.Info("End UpdateQueryDataFBBDashboardCommandHandler." + command.func + " output msg: " + executeResultsDynamic.GetValue("ret_message", String.Empty));
            }
            catch (Exception ex)
            {
                _logger.Info("Error UpdateQueryDataFBBDashboardCommandHandler output msg: " + ex.Message);
            }
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardRegRegion(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_REG_REGION.QUERY_REG_REGION_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_REG_REGION_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_reg_region.query_reg_region_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_REG_REGION.QUERY_REG_REGION_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_REG_REGION.QUERY_REG_REGION_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardRegProv(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_REG_PROV.QUERY_REG_PROV_MAIN​");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_REG_PROV_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_reg_prov.query_reg_prov_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_REG_PROV.QUERY_REG_PROV_MAIN​ output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_REG_PROV.QUERY_REG_PROV_MAIN​" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardRegDis(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_REG_DIS.QUERY_REG_DIS_MAIN​");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_REG_DIS_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_reg_dis.query_reg_dis_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_REG_DIS.QUERY_REG_DIS_MAIN​ output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_REG_DIS.QUERY_REG_DIS_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardRegSubdis(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_REG_SUBDIS.QUERY_REG_SUBDIS_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_REG_SUBDIS_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_reg_subdis.query_reg_subdis_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_REG_SUBDIS.QUERY_REG_SUBDIS_MAIN​ output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_REG_SUBDIS.QUERY_REG_SUBDIS_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardRegTeam(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_REG_TEAM.QUERY_REG_TEAM_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_REG_TEAM_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_reg_team.query_reg_team_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_REG_TEAM.QUERY_REG_TEAM_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_REG_TEAM.QUERY_REG_TEAM_MAIN​" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardCovRegion(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_COV_REGION.QUERY_COV_REGION_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_COV_REGION_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_cov_region.query_cov_region_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_COV_REGION.QUERY_COV_REGION_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_COV_REGION.QUERY_COV_REGION_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardCovProv(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_COV_PROV.QUERY_COV_PROV_MAIN​");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_COV_PROV_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_cov_prov.query_cov_prov_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_COV_PROV.QUERY_COV_PROV_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_COV_PROV.QUERY_COV_PROV_MAIN​" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardCovDis(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_COV_DIS.QUERY_COV_DIS_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_COV_DIS_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_cov_dis.query_cov_dis_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_COV_DIS.QUERY_COV_DIS_MAIN​ output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_COV_DIS.QUERY_COV_DIS_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardCovSubdis(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_COV_SUBDIS.QUERY_COV_SUBDIS_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_COV_SUBDIS_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_cov_subdis.query_cov_subdis_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_COV_SUBDIS.QUERY_COV_SUBDIS_MAIN​ output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_COV_SUBDIS.QUERY_COV_SUBDIS_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardCovTeam(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_COV_TEAM.QUERY_COV_TEAM_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_COV_TEAM_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_cov_team.query_cov_team_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_COV_TEAM.QUERY_COV_TEAM_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_COV_TEAM.QUERY_COV_TEAM_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardRegPack(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_REG_PACK.QUERY_REG_PACK_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_REG_PACK_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_reg_pack.query_reg_pack_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_REG_PACK.QUERY_REG_PACK_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_REG_PACK.QUERY_REG_PACK_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardCovCust(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_COV_CUST.QUERY_COV_CUST_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_COV_CUST_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_cov_cust.query_cov_cust_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_COV_CUST.QUERY_COV_CUST_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_COV_CUST.QUERY_COV_CUST_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }

        private QueryDataFBBDashboardModel QueryFBBDashboardRegCust(UpdateQueryDataFBBDashboardCommand command)
        {
            InterfaceLogCommand log = null;
            QueryDataFBBDashboardModel executeResults = new QueryDataFBBDashboardModel();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_DASHBOARD_REG_CUST.QUERY_REG_CUST_MAIN");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var result = new List<string>();
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "QUERY_REG_CUST_MAIN", "QueryDataFBBDashboard", "", "FBB", "QueryDataFBBDashboard.exe");

                var execute = _fbbShareplexRepository.ExecuteStoredProc("fbbadm.pkg_fbb_dashboard_reg_cust.query_reg_cust_main",
                out paramOut,
                  new
                  {
                      ret_code = ret_code,
                      ret_message = ret_message
                  });

                result.Add(ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1");
                result.Add(ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed");

                executeResults.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "-1";
                executeResults.ret_message = ret_message.Value != null ? ret_message.Value.ToSafeString() : "Failed";

                _logger.Info(string.Format("package -> {0} : {1}", ret_code.Value.ToSafeString(), ret_message.Value.ToSafeString()));
                _logger.Info("End FBBADM.PKG_FBB_DASHBOARD_REG_CUST.QUERY_REG_CUST_MAIN output msg: " + executeResults.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info("Error call service FBBADM.PKG_FBB_DASHBOARD_REG_CUST.QUERY_REG_CUST_MAIN" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }
    }
}
