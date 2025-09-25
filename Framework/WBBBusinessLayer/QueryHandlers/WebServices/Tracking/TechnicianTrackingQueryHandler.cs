using System;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices.Tracking;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices.Tracking
{
    public class TechnicianTrackingQueryHandler : IQueryHandler<TechnicianTrackingQuery, TechnicianTrackingModel>
    {
        private const string ENABLE_CALLBACK = "ENABLE_CALLBACK";

        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IQueryHandler<GetEncryptDecryptQuery, GetEncryptDecryptModel> _endeHandler;
        private readonly IQueryHandler<GetWfmfUrlQuery, GetWfmfUrlModel> _wfmfHander;

        public TechnicianTrackingQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IQueryHandler<GetEncryptDecryptQuery, GetEncryptDecryptModel> endeHandler,
            IQueryHandler<GetWfmfUrlQuery, GetWfmfUrlModel> wfmfHander)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _cfgLov = cfgLov;
            _endeHandler = endeHandler;
            _wfmfHander = wfmfHander;
        }

        public TechnicianTrackingModel Handle(TechnicianTrackingQuery query)
        {
            var result = new TechnicianTrackingModel();
            InterfaceLogCommand log = null;
            var success = "Success";
            var remark = "";
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.fibrenetId, "TechnicianTracking", GetType().Name, null, "FBB", "");

                var wfmfUrl = GetWfmfUrl(query.fibrenetId);
                if (string.IsNullOrEmpty(wfmfUrl)) return null;

                var req = new GetEncryptDecryptQuery
                {
                    IsEncoded = true,
                    ToEncryptText = Convert.ToBase64String(Encoding.UTF8.GetBytes($"idCard={query.idcard}"))
                };
                var res = _endeHandler.Handle(req);
                var track = new TechnicianTrackingModel
                {
                    wfmfurl = wfmfUrl,
                    p_type = "DISTANCE",
                    p_language = query.language,
                    p_option = "0",
                    p_channel = "WEB_REGISTER",
                    p_refId = GenRefId(),
                    p_urlback = isCallbackEnable()
                                ? $"{query.prefixUrl}/Tracking/back/" + res?.EncryptResult
                                : string.Empty
                };
                return track;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred '{GetType().Name}.Handle': {ex.ToString()}");
                success = "Failed";
                remark = ex.Message;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, success, remark, "");
            }
            return result;
        }

        public bool isCallbackEnable()
        {
            var conf = _cfgLov
                   .Get(t => t.LOV_TYPE == "CONFIG_TRACKING" && t.LOV_NAME == ENABLE_CALLBACK)
                   .FirstOrDefault();
            return conf?.ACTIVEFLAG == "Y";
        }

        public string GetWfmfUrl(string fibrenetId = "")
        {
            if (string.IsNullOrEmpty(fibrenetId)) return string.Empty;

            var query = new GetWfmfUrlQuery
            {
                fbbId = fibrenetId
            };
            var result = _wfmfHander.Handle(query);
            if (result?.resultData?.Any() == true)
            {
                var url = result?.resultData.LastOrDefault(x => x.fbbId == fibrenetId)?.WMWFURL ?? string.Empty;
                return url;
            }
            return string.Empty;
        }

        private string GenRefId(string prefix = null, int num = -1)
        {
            prefix = prefix ?? string.Empty;
            num = (num < 0) ? new Random().Next(1, 1000) : num;
            var datetime = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            var fragment = num.ToString("D7");
            return $"{prefix}{datetime}{fragment}";
        }
    }
}