using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class ResendToSFFSubmitFOAQueryHendler : IQueryHandler<ResendToSFFSubmitFOAQuery, ResultResendToSFFSubmitFOA>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<ListResendToSFFSubmitFOA> _submitOrderLog;
        private readonly IEntityRepository<ResultResendToSFFSubmitFOA> _updateStatus;

        public ResendToSFFSubmitFOAQueryHendler(
            ILogger logger,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<ListResendToSFFSubmitFOA> submitOrderLog,
            IEntityRepository<ResultResendToSFFSubmitFOA> updateStatus,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _cfgLov = cfgLov;
            _submitOrderLog = submitOrderLog;
            _updateStatus = updateStatus;
        }

        public ResultResendToSFFSubmitFOA Handle(ResendToSFFSubmitFOAQuery query)
        {

            var p_ws_sff = new OracleParameter();
            p_ws_sff.ParameterName = "p_ws_sff";
            p_ws_sff.OracleDbType = OracleDbType.RefCursor;
            p_ws_sff.Direction = ParameterDirection.Output;

            var p_return_code = new OracleParameter();
            p_return_code.ParameterName = "p_return_code";
            p_return_code.Size = 2000;
            p_return_code.OracleDbType = OracleDbType.Varchar2;
            p_return_code.Direction = ParameterDirection.Output;

            var p_return_message = new OracleParameter();
            p_return_message.ParameterName = "ret_msg";
            p_return_message.Size = 2000;
            p_return_message.OracleDbType = OracleDbType.Varchar2;
            p_return_message.Direction = ParameterDirection.Output;

            List<ListResendToSFFSubmitFOA> executeResults = _submitOrderLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.resend_sff",
                new
                {
                    p_ws_sff = p_ws_sff,
                    p_return_code = p_return_code,
                    p_return_message = p_return_message
                }).ToList();

            ResultResendToSFFSubmitFOA result = new ResultResendToSFFSubmitFOA();

            if (executeResults != null && executeResults.Count() > 0)
            {
                var resultSffLov = from item in _cfgLov.Get()
                                   where item.LOV_TYPE == "CREDENTIAL_SFF_FIXED_ASSET"
                                   select item;
                var urlSffEndpoint = resultSffLov.FirstOrDefault(item => item.LOV_NAME == "URL");
                var userSffCredential = resultSffLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
                var passwordSffCredential = resultSffLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();

                if (userSffCredential == null) return new ResultResendToSFFSubmitFOA() { RETURN_CODE = 8, RETURN_MSG = "Endpoint" };

                var _sff = new SFFServices.SFFServiceService();
                _sff.UseDefaultCredentials = true;

                if (urlSffEndpoint != null) _sff.Url = urlSffEndpoint.DISPLAY_VAL;

                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                _sff.Credentials = new NetworkCredential(userSffCredential.DISPLAY_VAL.ToSafeString(), passwordSffCredential.DISPLAY_VAL.ToSafeString());

                foreach (var item in executeResults)
                {
                    var request = new SFFServices.SffRequest();
                    request.Event = "evOMCreatePendingTaskFBB";

                    var paramArray = new SFFServices.Parameter[3];
                    var param0 = new SFFServices.Parameter();
                    var param1 = new SFFServices.Parameter();
                    var param2 = new SFFServices.Parameter();
                    var param3 = new SFFServices.Parameter();

                    param0.Name = "processType";
                    param0.Value = "NewRegister3GFMC";
                    param1.Name = "simSerial";
                    param1.Value = item.SIM_SN;
                    param2.Name = "fbbId";
                    param2.Value = param2.Value = item.ACCESS_NUMBER;

                    paramArray[0] = param0;
                    paramArray[1] = param1;
                    paramArray[2] = param2;

                    var paramList = new SFFServices.ParameterList();
                    paramList.Parameter = paramArray;

                    request.ParameterList = paramList;

                    var objResp = _sff.ExecuteService(request);

                    if (objResp != null)
                    {
                        if (objResp.ErrorMessage == null)
                        {
                            string returncode = "";
                            string retuenmsg = "";

                            var response = new SFFServices.SffResponse();
                            foreach (var itemData in objResp.ParameterList.Parameter)
                            {
                                if (itemData.Name == "result")
                                {
                                    returncode = itemData.Value;

                                }
                                else if (itemData.Name == "errorReason")
                                {
                                    retuenmsg = itemData.Value;
                                }
                            }

                            if (returncode == "Fail")
                            {
                                result.RETURN_CODE = -1;
                                result.RETURN_MSG = "Service Sff is " + retuenmsg;
                            }
                            else
                            {
                                this.UpdateStatusSubmitFOA(item.SIM_SN);
                                result.RETURN_CODE = 0;
                                result.RETURN_MSG = retuenmsg;
                            }
                        }
                    }
                }

            }
            else
            {
                result.RETURN_CODE = -1;
                result.RETURN_MSG = "Package Data Return Null!";
            }
            return result;
        }

        public void UpdateStatusSubmitFOA(string sim_sn)
        {

            var p_return_code = new OracleParameter();
            p_return_code.ParameterName = "p_return_code";
            p_return_code.Size = 2000;
            p_return_code.OracleDbType = OracleDbType.Varchar2;
            p_return_code.Direction = ParameterDirection.Output;

            var p_return_message = new OracleParameter();
            p_return_message.ParameterName = "p_return_message";
            p_return_message.Size = 2000;
            p_return_message.OracleDbType = OracleDbType.Varchar2;
            p_return_message.Direction = ParameterDirection.Output;

            var executeResult = _updateStatus.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.response_sff",
              new
              {
                  SIM_SN = sim_sn.ToSafeString(),
                  p_return_code = p_return_code,
                  p_return_message = p_return_message
              }).ToList();
        }
    }
}
