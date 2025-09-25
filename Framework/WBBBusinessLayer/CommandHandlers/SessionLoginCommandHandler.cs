using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SessionLoginCommandHandler : ICommandHandler<SessionLoginCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_SESSION_LOGIN> _fbbSessionLogin;

        public SessionLoginCommandHandler(IEntityRepository<FBB_SESSION_LOGIN> fbbSessionLogin, ILogger logger)
        {
            _fbbSessionLogin = fbbSessionLogin;
            _logger = logger;
        }

        public void Handle(SessionLoginCommand command)
        {
            try
            {
                _logger.Info("START_PROC_SAVE_SESSION_LOGIN");

                var retCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var retMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };
                object[] paramOut;
                _fbbSessionLogin.ExecuteStoredProc("WBB.PKG_FBB_SESSION_LOGIN.PROC_SAVE_SESSION_LOGIN",
                    out paramOut,
                    new
                    {
                        p_in_CUST_INTERNET_NUM = command.CustInternetNum,
                        p_in_SESSION_ID = command.SessionId,
                        //// return 
                        p_return_code = retCode,
                        p_return_message = retMessage,
                    });

                command.ReturnCode = retCode.Value != null ? Convert.ToInt32(retCode.Value.ToSafeString()) : -1;
                command.ReturnDesc = retMessage.Value != null ? retMessage.Value.ToSafeString() : "Error";

                _logger.Info("END_PROC_SAVE_SESSION_LOGIN");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ReturnCode = -1;
                command.ReturnDesc = "Error call Session Login Command handles : " + ex.GetErrorMessage();
            }

        }
    }

}
