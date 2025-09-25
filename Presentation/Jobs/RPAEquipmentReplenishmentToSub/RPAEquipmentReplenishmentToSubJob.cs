using System;
using System.Configuration;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBShareplex;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels;
using WBBContract.Queries.Commons.Masters;
using System.Collections.Generic;
using System.Linq;
using WBBEntity.PanelModels.WebServiceModels;
using WBBContract.Queries.FBBHVR;


namespace RPAEquipmentReplenishmentToSub
{
    public class RPAEquipmentReplenishmentToSubJob
    {
        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;

        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        protected string _userTemp;
        protected string _pwdTemp;
        protected string _userTarget;
        protected string _pwdTarget;

        public RPAEquipmentReplenishmentToSubJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("RPAEquipmentReplenishmentToSubJob: Start");
                var ConnectionNas = GetConnectionNasPAYG();

                _userTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Username, _Key);
                _pwdTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Password, _Key);
                _userTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Username, _Key);
                _pwdTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Password, _Key);

                RPAEquipmentReplenishmentToSubModel result = new RPAEquipmentReplenishmentToSubModel();

                // R24.07 Edit DB Shareplex to HVR
                var flagHVR = GetFlagHVR(); // get config flag shareplex or hvr

                if (flagHVR == "Y")
                {
                    _logger.Info("Connect HVR FLAG");

                    var query = new RPAEquipmentReplenishmentToSubHVRQuery()
                    {
                        PathTempFile = ConfigurationManager.AppSettings["TARGET"].ToSafeString(),
                        DomainTempFile = ConfigurationManager.AppSettings["TARGET_DOMAIN"].ToSafeString(),
                        UserTempFile = _userTemp,
                        PassTempFile = _pwdTemp,
                        TargetArchivePathFile = ConfigurationManager.AppSettings["TARGET_ARCHIVE_PATH"].ToSafeString(),
                        TargetArchiveDomainFile = ConfigurationManager.AppSettings["TARGET_ARCHIVE_DOMAIN"].ToSafeString(),
                        TargetArchiveUserFile = _userTarget,
                        TargetArchivePassFile = _pwdTarget
                    };
                    result = _queryProcessor.Execute(query);
                }
                else
                {
                    _logger.Info("Connect SharePlex FLAG");

                    var query = new RPAEquipmentReplenishmentToSubQuery()
                    {
                        PathTempFile = ConfigurationManager.AppSettings["TARGET"].ToSafeString(),
                        DomainTempFile = ConfigurationManager.AppSettings["TARGET_DOMAIN"].ToSafeString(),
                        UserTempFile = _userTemp,
                        PassTempFile = _pwdTemp,
                        TargetArchivePathFile = ConfigurationManager.AppSettings["TARGET_ARCHIVE_PATH"].ToSafeString(),
                        TargetArchiveDomainFile = ConfigurationManager.AppSettings["TARGET_ARCHIVE_DOMAIN"].ToSafeString(),
                        TargetArchiveUserFile = _userTarget,
                        TargetArchivePassFile = _pwdTarget
                    };
                    result = _queryProcessor.Execute(query);
                }

                _logger.Info("RPAEquipmentReplenishmentToSubJob: ReturnCode = " + result.ReturnCode);
                _logger.Info("RPAEquipmentReplenishmentToSubJob: ReturnMessage = " + result.ReturnMessage);
                StopWatching("RPAEquipmentReplenishmentToSubJob ");
            }
            catch (Exception ex)
            {
                _logger.Info("RPAEquipmentReplenishmentToSubJob :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("RPAEquipmentReplenishmentToSubJob ");
            }
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", process, _timer.Elapsed));
        }

        private ConnectionNasPAYGListResult GetConnectionNasPAYG()
        {
            var query = new GetConnectionNasPAYGQuery();
            var ss = _queryProcessor.Execute(query);
            return ss;
        }

        private string GetFlagHVR()
        {
            var query = new GetLovQuery()
            {
                LovType = "FBB_CONSTANT",
                LovName = "HVR_USE_FLAG"
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);
            
            string flagHVR = (_FbbCfgLov != null && _FbbCfgLov.Any())
                 ? _FbbCfgLov.FirstOrDefault()?.LovValue1 ?? "N"
                 : "N";
            return flagHVR;
        }
    }
}
