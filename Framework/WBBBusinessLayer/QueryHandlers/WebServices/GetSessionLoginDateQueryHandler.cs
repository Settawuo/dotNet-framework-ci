using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers
{
    public class GetSessionLoginDateQueryHandler : IQueryHandler<GetSessionLoginDateQuery, SessionLoginStatusModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_SESSION_LOGIN> _fbbSessionLogin;
        private readonly IEntityRepository<object> _fbblovRepository;

        public GetSessionLoginDateQueryHandler(
            ILogger logger,
            IEntityRepository<FBB_SESSION_LOGIN> fbbSessionLogin,
            IEntityRepository<object> fbblovRepository)
        {
            _logger = logger;
            _fbbSessionLogin = fbbSessionLogin;
            _fbblovRepository = fbblovRepository;
        }
        public SessionLoginStatusModel Handle(GetSessionLoginDateQuery query)
        {
            var result = new SessionLoginStatusModel();
            try
            {
                _logger.Info("START_PROC_GET_SESSION_LOGIN_DATE");

                var p_return_date = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var returnCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var returnMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                #region Output Multiple Cursor
                #endregion

                object[] paramOut;
                _fbbSessionLogin.ExecuteStoredProc("WBB.PKG_FBB_SESSION_LOGIN.PROC_GET_SESSION_LOGIN_DATE",
                    out paramOut,
                    new
                    {
                        p_in_CUST_INTERNET_NUM = query.CustInternetNum,

                        p_return_date = p_return_date,
                        p_return_code = returnCode,
                        p_return_message = returnMessage,
                        //catCur = returnobj1,
                        //prodCur = returnobj2
                    });

                result.ReturnDate = p_return_date.Value != null ? p_return_date.Value.ToSafeString() : "";
                result.ReturnCode = returnCode.Value != null ? Convert.ToInt32(returnCode.Value.ToSafeString()) : -1;
                result.ReturnMessage = returnMessage.Value != null ? returnMessage.Value.ToSafeString() : "Error";



                _logger.Info("END_PROC_GET_SESSION_LOGIN_DATE");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                result.ReturnDate = "";
                result.ReturnCode = -1;
                result.ReturnMessage = "Error call Session Login Command handles : " + ex.GetErrorMessage();
            }
            return result;
        }
    }
}
