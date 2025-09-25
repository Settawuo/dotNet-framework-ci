using System;
using System.Configuration;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBShareplex;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace GenReportReconcileBatch
{
    public class GenReportReconcileBatchJob
    {
        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;

        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        protected string _userTemp;
        protected string _pwdTemp;
        protected string _userTarget;
        protected string _pwdTarget;

        public GenReportReconcileBatchJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("GenReportReconcileBatchJob: Start");
                var ConnectionNas = GetConnectionNasPAYG();
                _userTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Username, _Key);
                _pwdTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Password, _Key);
                _userTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Username, _Key);
                _pwdTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Password, _Key);

                var query = new GenReportReconcileQuery()
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
                var result = _queryProcessor.Execute(query);

                _logger.Info("GenReportReconcileBatchJob: ReturnCode = " + result.ReturnCode);
                _logger.Info("GenReportReconcileBatchJob: ReturnMessage = " + result.ReturnMessage);
                StopWatching("GenReportReconcileBatchJob ");
            }
            catch (Exception ex)
            {
                _logger.Info("GenReportReconcileBatchJob :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("GenReportReconcileBatchJob ");
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
    }
}

