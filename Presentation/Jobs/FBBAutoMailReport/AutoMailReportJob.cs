using FBBAutoMailReport.FileUploadServices;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;

namespace FBBAutoMailReport
{
    public class AutoMailReportJob
    {
        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;

        public AutoMailReportJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("AutoMailReportJob: Start");

                var query = new ReportAutoMailQuery()
                {
                    ReportId = "",
                    CreateBy = "Batch",
                    PathTempFile = ConfigurationManager.AppSettings["TARGET"].ToSafeString(),
                    DomainTempFile = ConfigurationManager.AppSettings["TARGET_DOMAIN"].ToSafeString(),
                    UserTempFile = ConfigurationManager.AppSettings["TARGET_USER"].ToSafeString(),
                    PassTempFile = ConfigurationManager.AppSettings["TARGET_PWD"].ToSafeString()
                };
                var result = _queryProcessor.Execute(query);

                _logger.Info("AutoMailReportJob: ReturnCode = " + result.ReturnCode);
                _logger.Info("AutoMailReportJob: ReturnMessage = " + result.ReturnMessage);
                StopWatching("AutoMailReportJob ");
            }
            catch (Exception ex)
            {
                _logger.Info("AutoMailReportJob :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("AutoMailReportJob ");
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


        public void TestUploadfile()
        {
            //Stream fs = File.OpenRead(@"D:\Movie\PEE MAK [2013] พี่มากพระโขนง\VIDEO_TS\VTS_01_1.VOB");

            //var query = new StreamQuery
            //{
            //    stream = fs
            //};
            //var result = _queryProcessor.Execute(query);

            using (var client = new FileUploadServiceClient())
            {
                Stream fileStream = null;

                try
                {
                    string rootPath = @"D:\Movie\PEE MAK [2013] พี่มากพระโขนง\VIDEO_TS";
                    string localDocumentPath = Path.Combine(rootPath, "VTS_01_1.VOB");
                    fileStream = new FileInfo(localDocumentPath).OpenRead();

                    client.UploadFile("", fileStream);

                    byte[] buffer = new byte[2048];
                    int bytesRead = fileStream.Read(buffer, 0, 2048);
                    while (bytesRead > 0)
                    {
                        fileStream.Write(buffer, 0, 2048);
                        bytesRead = fileStream.Read(buffer, 0, 2048);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }

        }
    }

}
