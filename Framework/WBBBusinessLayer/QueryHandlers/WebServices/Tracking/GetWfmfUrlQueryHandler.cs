using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices.Tracking;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices.Tracking
{
    public class GetWfmfUrlQueryHandler : IQueryHandler<GetWfmfUrlQuery, GetWfmfUrlModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetWfmfUrlQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _cfgLov = cfgLov;
        }

        public GetWfmfUrlModel Handle(GetWfmfUrlQuery query)
        {
            InterfaceLogCommand log = null;
            GetWfmfUrlModel result = null;
            var success = "Success";
            var remark = "";
            try
            {
                result = new GetWfmfUrlModel();
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.fbbId, "GetWfmfUrl", GetType().Name, null, "FBB", "");

                var urlConf = _cfgLov.Get(x => x.LOV_NAME == "URL_WMFURL").FirstOrDefault();

                if (string.IsNullOrEmpty(urlConf?.LOV_VAL1))
                {
                    throw new InvalidOperationException($"Error FBB_CFG_LOV on LOV_NAME == 'URL_WMFURL' should not be null or empty.");
                }
                var authen = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(urlConf?.LOV_VAL2 + ":" + urlConf?.LOV_VAL3));
                if (urlConf?.LOV_VAL5 == "Y")
                {
                    ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                }

                var request = (HttpWebRequest)WebRequest.Create(urlConf?.LOV_VAL1);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add("Authorization", $"Basic {authen}");
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var json = query.Obj2Json();
                    streamWriter.Write(json);
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var temp = streamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(temp))
                    {
                        result = temp.Json2Obj<GetWfmfUrlModel>();
                    }
                }
                if (result?.resultCode != "20000")
                {
                    success = "Failed";
                    remark = result?.developerMessage;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred {GetType().Name}.Handle : {ex.ToString()}");
                success = "Failed";
                remark = ex.Message;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, success, remark, "");
            }
            return result;
        }
    }
}
