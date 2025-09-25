using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBTransactionWorkflowReport
{
    public class FBBTransactionWorkflowReportJob
    {

        private Stopwatch _timer;
        private readonly ILogger logger;
        private readonly IQueryProcessor queryProcessor;
        //private FBBTransactionWorkflowQuery wkQ;
        private GetConnectionNasPAYGQuery pgCon;
        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();//Key Decrypt.
        protected string _userTemp;
        protected string _pwdTemp;

        public FBBTransactionWorkflowReportJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            this.logger = logger;
            this.queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                logger.Info("START : FBBTransactionWorkflowReportJob");
                ConnectionNasPAYGListResult retConnnectionNas = GetConnectionNas();
                _userTemp = EncryptionUtility.Decrypt(retConnnectionNas.NasTemp.Username, _Key);
                _pwdTemp = EncryptionUtility.Decrypt(retConnnectionNas.NasTemp.Password, _Key);
                logger.Info("Get Connection From PayG Success.");
                //==============================================================================
                //flag เปิด ปิด shareplex with HVR
                var flaghvr = GetFlagHVR();
                if(flaghvr == "Y")
                {
                    logger.Info(string.Format("Connect HVR FLAG :{0}", flaghvr));
                   var wkQ = new FBBTransactionWorkflowHVRQuery()
                    {
                       UserHost = _userTemp,
                       PassHost = _pwdTemp,
                       DomainHost = ConfigurationManager.AppSettings["DOMAIN_HOST"].ToSafeString(),
                       TargetArchivePath = ConfigurationManager.AppSettings["ARCHIVE_PATH"].ToSafeString(),
                       TargetDomainPath = ConfigurationManager.AppSettings["DOMAIN_PATH"].ToSafeString()
                   };
                    var result = queryProcessor.Execute(wkQ);
                    logger.Info("FBBTransactionWorkflowReportJob: ReturnCode = " + result.ReturnCode);
                    logger.Info("FBBTransactionWorkflowReportJob: ReturnMessage = " + result.ReturnMessage);
                }
                else
                {
                    logger.Info(string.Format("Connect SharePlex FLAG :{0}", flaghvr));
                   var wkQ = new FBBTransactionWorkflowQuery()
                    {
                        UserHost = _userTemp,
                        PassHost = _pwdTemp,
                        DomainHost = ConfigurationManager.AppSettings["DOMAIN_HOST"].ToSafeString(),
                        TargetArchivePath = ConfigurationManager.AppSettings["ARCHIVE_PATH"].ToSafeString(),
                        TargetDomainPath = ConfigurationManager.AppSettings["DOMAIN_PATH"].ToSafeString()
                    };
                    var result = queryProcessor.Execute(wkQ);
                    logger.Info("FBBTransactionWorkflowReportJob: ReturnCode = " + result.ReturnCode);
                    logger.Info("FBBTransactionWorkflowReportJob: ReturnMessage = " + result.ReturnMessage);

                }

                StopWatching("STOP : FBBTransactionWorkflowReportJob ");
            }
            catch (Exception ex)
            {
                logger.Info("Exceiption FBBTransactionWorkflowReportJob :" + string.Format(" is error on execute : {0}.",
                    ex.Message));
                logger.Info(ex.Message);
                StopWatching("STOP : FBBTransactionWorkflowReportJob ");
            }
        }

        public ConnectionNasPAYGListResult GetConnectionNas()
        {
            pgCon = new GetConnectionNasPAYGQuery();
            return queryProcessor.Execute(pgCon);
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
            logger.Info(string.Format("{0} take : {1}", process, _timer.Elapsed));
        }
        private string GetFlagHVR()
        {
            var query = new GetLovQuery()
            {
                LovType = "FBB_CONSTANT",
                LovName = "HVR_USE_FLAG"
            };
            var lov = queryProcessor.Execute(query);

            string flaghrv = (lov != null && lov.Any())
                 ? lov.FirstOrDefault()?.LovValue1 ?? "N"
                 : "N";
            return flaghrv;
        }
    }
}
