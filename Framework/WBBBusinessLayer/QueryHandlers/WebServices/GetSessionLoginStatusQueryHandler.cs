using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetSessionLoginStatusQueryHandler : IQueryHandler<GetSessionLoginStatusQuery, SessionLoginStatusModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_SESSION_LOGIN> _fbbSessionLogin;
        private readonly IEntityRepository<object> _fbblovRepository;

        public GetSessionLoginStatusQueryHandler(
            ILogger logger,
            IEntityRepository<FBB_SESSION_LOGIN> fbbSessionLogin,
            IEntityRepository<object> fbblovRepository)
        {
            _logger = logger;
            _fbbSessionLogin = fbbSessionLogin;
            _fbblovRepository = fbblovRepository;
        }

        public SessionLoginStatusModel Handle(GetSessionLoginStatusQuery query)
        {
            var result = new SessionLoginStatusModel();
            try
            {
                _logger.Info("START_PROC_GET_SESSION_LOGIN_STATUS");

                var returnStatus = new OracleParameter
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
                //var returnobj1 = new OracleParameter
                //{
                //    ParameterName = "returnobj1",
                //    OracleDbType = OracleDbType.RefCursor,
                //    Direction = ParameterDirection.Output
                //};

                //var returnobj2 = new OracleParameter
                //{
                //    ParameterName = "returnobj2",
                //    OracleDbType = OracleDbType.RefCursor,
                //    Direction = ParameterDirection.Output
                //};

                //var obj1 = (List<FBB_CFG_LOV>) returnobj1.Value;
                //var obj2 = (List<FBB_CFG_LOV>) returnobj2.Value;

                //_fbblovRepository.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_SESSION_LOGIN.PROC_TEST_REFC",
                //    new object[]
                //    {
                //        returnobj1,
                //        returnobj2
                //    });

                //var result1 = ((OracleRefCursor)returnobj1.Value).GetDataReader();
                //var lov = _fbblovRepository.Translate(result1, "FBB_CFG_LOV") as List<FBB_CFG_LOV>;

                //var result2 = ((OracleRefCursor)returnobj2.Value).GetDataReader();
                //var lov2 = _fbblovRepository.Translate(result2, "FBB_CFG_LOV") as List<FBB_CFG_LOV>;
                #endregion

                object[] paramOut;
                _fbbSessionLogin.ExecuteStoredProc("WBB.PKG_FBB_SESSION_LOGIN.PROC_GET_SESSION_LOGIN_STATUS",
                    out paramOut,
                    new
                    {
                        p_in_CUST_INTERNET_NUM = query.CustInternetNum,
                        p_in_SESSION_ID = query.SessionId,

                        p_return_status = returnStatus,
                        p_return_code = returnCode,
                        p_return_message = returnMessage,
                        //catCur = returnobj1,
                        //prodCur = returnobj2
                    });

                result.ReturnStatus = returnStatus.Value != null ? Convert.ToInt32(returnStatus.Value.ToSafeString()) : -1;
                result.ReturnCode = returnCode.Value != null ? Convert.ToInt32(returnCode.Value.ToSafeString()) : -1;
                result.ReturnMessage = returnMessage.Value != null ? returnMessage.Value.ToSafeString() : "Error";



                _logger.Info("END_PROC_GET_SESSION_LOGIN_STATUS");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                result.ReturnStatus = -1;
                result.ReturnCode = -1;
                result.ReturnMessage = "Error call Session Login Command handles : " + ex.GetErrorMessage();
            }
            return result;
        }
    }
}
