using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.WebServices.Tracking;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices.Tracking
{
    public class GetEncryptDecryptQueryHandler : IQueryHandler<GetEncryptDecryptQuery, GetEncryptDecryptModel>
    {
        private const string DEFAULT_KEYWORD = "Web Register";
        private const string ENCODE_KEYWORD = "ENCODE_KEYWORD";

        private readonly string _keywordEncrypt;

        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IQueryHandler<GetLovQuery, List<LovValueModel>> _queryHandler;
        public GetEncryptDecryptQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IQueryHandler<GetLovQuery, List<LovValueModel>> queryHandler)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _queryHandler = queryHandler;
            _keywordEncrypt = GetEncodeKeyword();
        }

        public GetEncryptDecryptModel Handle(GetEncryptDecryptQuery query)
        {
            InterfaceLogCommand log = null;
            GetEncryptDecryptModel result = null;
            var success = "Success";
            var remark = "";
            try
            {
                result = new GetEncryptDecryptModel();
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, string.Empty, "GetEncryptDecrypt", GetType().Name, null, "FBB", "");

                if (!string.IsNullOrEmpty(query.ToEncryptText))
                {
                    var toEncode = query.IsEncoded ? Encoding.UTF8.GetString(Convert.FromBase64String(query.ToEncryptText))
                                                   : query.ToEncryptText;
                    result.EncryptResult = Encrypt(toEncode, _keywordEncrypt);
                }
                if (!string.IsNullOrEmpty(query.ToDecryptText))
                {
                    result.DecryptResult = Decrypt(query.ToDecryptText, _keywordEncrypt);
                }
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

        public string Encrypt(string text, string keyword)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(keyword)) throw new ArgumentNullException(nameof(keyword));

            var encrypted = UTF8Encoding.UTF8.GetBytes($"{text}.{keyword}");
            return Convert.ToBase64String(encrypted);
        }
        public string Decrypt(string text, string keyword)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(keyword)) throw new ArgumentNullException(nameof(keyword));

            var encrypted = Convert.FromBase64String(text);
            var plaintext = UTF8Encoding.UTF8.GetString(encrypted);

            return plaintext.Replace($".{keyword}", string.Empty);
        }

        public string GetEncodeKeyword()
        {
            var query = new GetLovQuery
            {
                LovType = "CONFIG_TRACKING",
                LovName = ENCODE_KEYWORD
            };
            var lov = _queryHandler.Handle(query);
            var conf = lov?.FirstOrDefault();
            if (conf != null)
            {
                return conf.ActiveFlag == "Y" ? conf.LovValue1 : DEFAULT_KEYWORD;
            }
            return DEFAULT_KEYWORD;
        }
    }
}
