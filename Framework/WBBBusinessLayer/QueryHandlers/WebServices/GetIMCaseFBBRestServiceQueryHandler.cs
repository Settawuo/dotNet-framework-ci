using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetIMCaseFBBRestServiceQueryHandler : IQueryHandler<GetIMCaseFBBRestServiceQuery, GetIMCaseFBBRestServiceModel>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<object> _objService;

        public GetIMCaseFBBRestServiceQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _objService = objService;
        }

        public GetIMCaseFBBRestServiceModel Handle(GetIMCaseFBBRestServiceQuery query)
        {
            var StatusCode = "";
            var StatusMessage = "";
            var OrderNo = "";

            var result = new GetIMCaseFBBRestServiceModel();

            var apiCongif = (from l in _cfgLov.Get()
                             where l.LOV_NAME == "WEB_SERVICE_IM"
                             && l.ACTIVEFLAG == "Y"
                             select new { l.LOV_VAL1, l.LOV_VAL3 });
            query.Request_Url = apiCongif.FirstOrDefault().LOV_VAL1.ToSafeString();
            string useSecurityProtocol = apiCongif.FirstOrDefault().LOV_VAL3.ToSafeString();

            var client = new RestClient(query.Request_Url);
            var request = new RestRequest();
            request.Method = Method.PUT;

            var objBody = new DetailCalliMTopupReplace
            {
                AssetNo = query.Body.AssetNo.ToSafeString(),
                UserID = query.Body.UserID.ToSafeString(),
                InteractionType = query.Body.InteractionType.ToSafeString(),
                SocialAccountID = query.Body.SocialAccountID.ToSafeString(),
                SocialName = query.Body.SocialName.ToSafeString(),
                CustomerName = query.Body.CustomerName.ToSafeString(),
                ConfirmFlag = query.Body.ConfirmFlag.ToLower().ToSafeString(),
                SymptomCode = query.Body.SymptomCode.ToSafeString(),
                Symptom = query.Body.Symptom.ToSafeString(),
                ContactNo = query.Body.ContactNo.ToSafeString(),
                Channel = query.Body.Channel.ToSafeString(),
                SymptomName = query.Body.SymptomName.ToSafeString(),
                Priority = query.Body.Priority.ToSafeString(),
                TopicName = query.Body.TopicName.ToSafeString(),
                PlayboxAdditionalService = query.Body.PlayboxAdditionalService.ToSafeString(),
                SerialNo = query.Body.SerialNo.ToSafeString(),
                FixedLineNo = query.Body.FixedLineNo.ToSafeString(),
                TimeSelected = query.Body.TimeSelected.ToSafeString(),
                DateSelected = query.Body.DateSelected.ToSafeString(),
                ReasonOver24Hr = query.Body.ReasonOver24Hr.ToSafeString(),
                ContactFlag = query.Body.ContactFlag.ToLower().ToSafeString(),
                CommentFromCustomer = query.Body.CommentFromCustomer.ToSafeString(),
                AccessType = query.Body.AccessType.ToSafeString(),
                OnlineStatus = query.Body.OnlineStatus.ToSafeString(),
                LocationAddress = query.Body.LocationAddress.ToSafeString(),
                Package = query.Body.Package.ToSafeString(),
                PackageOnTop = query.Body.PackageOnTop.ToSafeString(),
                StartedUsingDateTime = query.Body.StartedUsingDateTime.ToSafeString(),
                IPAddress = query.Body.IPAddress.ToSafeString(),
                IPvSix = query.Body.IPvSix.ToSafeString(),
                IPType = query.Body.IPType.ToSafeString(),
                NodeName = query.Body.NodeName.ToSafeString(),
                AddressID = query.Body.AddressID.ToSafeString(),
                ReservedId = query.Body.ReservedId.ToSafeString()
            };

            GetIMCaseFBBRestServiceModel getIMCaseFBBRestServiceResult = new GetIMCaseFBBRestServiceModel();
            string BodyStr = JsonConvert.SerializeObject(objBody);
            query.BodyJson = BodyStr;
            request.AddParameter("application/json", query.BodyJson, ParameterType.RequestBody);

            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "IMCaseFBBRestService", "GetIMCaseFBBRestServiceQueryHandler", null, "FBB", "");
            var log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, BodyStr, query.TransactionId, "IMCaseFBBRestService", "GetIMCaseFBBRestServiceQueryHandler (Service IM)", null, "FBB", "");
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // execute the request
                //useSecurityProtocol = "Y";
                if (useSecurityProtocol == "Y")
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;
                }

                var response = client.Execute(request);

                var content = response.Content; //raw content as string

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    getIMCaseFBBRestServiceResult = JsonConvert.DeserializeObject<GetIMCaseFBBRestServiceModel>(response.Content) ?? new GetIMCaseFBBRestServiceModel();
                    if (getIMCaseFBBRestServiceResult != null)
                    {
                        StatusCode = getIMCaseFBBRestServiceResult.ResultCode == "01" ? "Success" : "Failed";
                        StatusMessage = getIMCaseFBBRestServiceResult.ResultCode == "01" ? "Success" : getIMCaseFBBRestServiceResult.ResultDesc;
                        OrderNo = GenOrderNo("AIR", "RP", "REPLACE");
                    }
                }

                if (!string.IsNullOrEmpty(getIMCaseFBBRestServiceResult.ResultCode))
                {
                    result.ResultCode = getIMCaseFBBRestServiceResult.ResultCode.ToSafeString();
                    result.ResultDesc = StatusMessage;
                    result.ResultOrderNo = OrderNo;

                    StatusCode = "Success";
                    StatusMessage = "Success";
                }
                else
                {
                    result.ResultCode = "-1";
                    result.ResultDesc = "No Data.";

                    StatusCode = "Failed";
                    StatusMessage = "No Data.";
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = "-1";
                result.ResultDesc = ex.GetBaseException().ToString();

                StatusCode = "Failed";
                StatusMessage = ex.GetBaseException().ToString();
                _logger.Info("GetIMCaseFBBRestServiceQueryHandler Exception " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, StatusMessage, "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getIMCaseFBBRestServiceResult, log2, StatusCode, StatusMessage, "");
            }

            return result;
        }

        public string GenOrderNo(string company_abbr, string order_type, string user_name)
        {
            string error_msg = "";
            string order_no = "";

            try
            {
                var P_COMPANY_ABBR = new OracleParameter();
                P_COMPANY_ABBR.ParameterName = "P_COMPANY_ABBR";
                P_COMPANY_ABBR.OracleDbType = OracleDbType.Varchar2;
                P_COMPANY_ABBR.Direction = ParameterDirection.Input;
                P_COMPANY_ABBR.Value = company_abbr;

                var P_ORDER_TYPE = new OracleParameter();
                P_ORDER_TYPE.ParameterName = "P_ORDER_TYPE";
                P_ORDER_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_ORDER_TYPE.Direction = ParameterDirection.Input;
                P_ORDER_TYPE.Value = order_type;

                var P_USER_NAME = new OracleParameter();
                P_USER_NAME.ParameterName = "P_USER_NAME";
                P_USER_NAME.OracleDbType = OracleDbType.Varchar2;
                P_USER_NAME.Direction = ParameterDirection.Input;
                P_USER_NAME.Value = user_name;

                var ERROR_MSG = new OracleParameter();
                ERROR_MSG.ParameterName = "ERROR_MSG";
                ERROR_MSG.OracleDbType = OracleDbType.Varchar2;
                ERROR_MSG.Size = 2000;
                ERROR_MSG.Direction = ParameterDirection.Output;

                var ORDER_NO = new OracleParameter();
                ORDER_NO.ParameterName = "ORDER_NO";
                ORDER_NO.OracleDbType = OracleDbType.Varchar2;
                ORDER_NO.Size = 2000;
                ORDER_NO.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR004.PROC_GEN_ORDER_NO",
                     new object[]
                     {
                         P_COMPANY_ABBR,
                         P_ORDER_TYPE,
                         P_USER_NAME,
                         //return
                         ERROR_MSG,
                         ORDER_NO
                     });

                if (result != null)
                {
                    error_msg = result[0] != null ? result[0].ToSafeString() : "Error";
                    order_no = result[1] != null ? result[1].ToSafeString() : "Error";
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return order_no;
        }

        public class IMCaseFBBRestServiceBody
        {
            public IMCaseFBBRestServiceParameterList imcasefbbrestservicerequest { get; set; }
        }

        public class IMCaseFBBRestServiceParameterList
        {
            public IMCaseFBBRestServiceParameters[] Parameter { get; set; }
        }

        public class IMCaseFBBRestServiceParameters
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public List<IMCaseFBBRestServiceParameter>[] Values { get; set; }
        }

        public class IMCaseFBBRestServiceParameter
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
