using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class InstallLeaveMessageQueryHandler : IQueryHandler<InstallLeaveMessageQuery, InstallLeaveMessageModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<InstallLeaveMessageModel> _objSubJ;

        public InstallLeaveMessageQueryHandler(ILogger logger, IEntityRepository<InstallLeaveMessageModel> objSubJ)
        {
            _logger = logger;
            _objSubJ = objSubJ;
        }

        public InstallLeaveMessageModel Handle(InstallLeaveMessageQuery query)
        {
            InstallLeaveMessageModel leave = null;
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "RET_CODE";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "RET_MESSAGE";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                if (query.p_status.Equals("ON_SERVICE_SPECIAL"))
                {
                    var executeResult = _objSubJ.ExecuteStoredProc("WBB.PKG_FBB_AUTO_CHECK_COVERAGE.PROC_INS_LEAVE_MESSAGE_ONS",
                    new
                    {
                        p_result_id = query.p_result_id,
                        ret_code,
                        ret_message
                    });
                }
                else
                {
                    var executeResult = _objSubJ.ExecuteStoredProc("WBB.PKG_FBB_AUTO_CHECK_COVERAGE.PROC_INS_LEAVE_MESSAGE",
                    new
                    {
                        p_result_id = query.p_result_id,
                        ret_code,
                        ret_message
                    });
                }

                string code = ret_code.Value.ToString();
                string message = ret_message.Value.ToString();
                _logger.Info("PROC_INS_LEAVE_MESSAGE  " + code + " : " + message);
                leave = new InstallLeaveMessageModel()
                {
                    ret_code = code,
                    ret_message = message
                };

            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                leave = new InstallLeaveMessageModel()
                {
                    ret_code = "-1",
                    ret_message = ex.Message
                };
                _logger.Info("EXCEPTION PROC_INS_LEAVE_MESSAGE  " + ex.Message);
            }

            return leave;
        }
    }
}
