using System;

namespace ReconcileStandardAddress
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBEntity.Extensions;

    public class ReconcileStandardAddressJob
    {
        public ILogger _logger;
        private readonly ICommandHandler<ReconcileStandardAddressCommand> _ReconcildData;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;

        //private string _attachFile;
        //public string[] _attachFiles;
        //public string[] _subjectFiles;

        public ReconcileStandardAddressJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<ReconcileStandardAddressCommand> ReconcildData)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _ReconcildData = ReconcildData;
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        public void ReconcileStandardAddress()
        {
            _logger.Info("Reconcile Standard Address");
            try
            {

                StartWatching();
                var command = new ReconcileStandardAddressCommand
                {
                };

                _ReconcildData.Handle(command);

                _logger.Info(string.Format("Reconcile Standard Address : {0}", command.Return_Message));
                StopWatching("Reconcile Standard Address");
            }
            catch (Exception ex)
            {
                _logger.Info("Reconcile Standard Address" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("Reconcile Standard Address");
                //throw ex;
            }
        }

        #region test code
        //public void PrepareAttachFile(string nasPath)
        //{
        //    _logger.Info("Prepare attachment file(s).");
        //    StartWatching();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(_attachFile))
        //        {
        //            var tempFiles = _attachFile.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //            var tmpAttachFile = "";

        //            foreach (var file in tempFiles)
        //            {
        //                tmpAttachFile += file + ',';
        //            }

        //            _attachFile = tmpAttachFile.TrimEnd(','); 
        //        }

        //        StopWatching("Sending an Email");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
        //            ex.GetErrorMessage()));
        //        _logger.Info(ex.RenderExceptionMessage());
        //        StopWatching("Prepare attachment file(s)");
        //        //throw ex;
        //    }
        //}


        //public void Sendmail(string Cause, string ErrorMessage, string ProcName, string ProcNameGetMail)
        //{
        //    _logger.Info("Sending an Email.");
        //    StartWatching();
        //    try
        //    {
        //        var command = new SendMailNotificationCommand
        //        {
        //            Cause = Cause,
        //            ErrorMessage = ErrorMessage,
        //            ProcName = ProcName,
        //            AttachFiles = _attachFiles,
        //            CreateUser = "Batch",
        //            ProcNameGetMail = ProcNameGetMail,
        //            SubSubject = _subjectFiles
        //        };

        //        _Sendmail.Handle(command);

        //        _logger.Info(string.Format("Sending an Email : {0}.",command.ReturnMessage));
        //        StopWatching("Sending an Email");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
        //            ex.GetErrorMessage()));
        //        _logger.Info(ex.RenderExceptionMessage());
        //        StopWatching("Sending an Email");
        //        //throw ex;
        //    }
        //}

        //public List<LovValueModel> GetLovList(string type, string name)
        //{
        //    _logger.Info("Get Lov List");
        //    StartWatching();

        //    try
        //    {
        //        var query = new GetLovQuery
        //        {
        //            LovType = type,
        //            LovName = name
        //        };

        //        var lov = _queryProcessor.Execute(query);
        //        StopWatching("Get Lov List");
        //        return lov;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info(ex.GetErrorMessage());
        //        _logger.Info(ex.RenderExceptionMessage());
        //        StopWatching("Get Lov List");
        //        return new List<LovValueModel>();
        //    }
        //}

        //public void DeleteOldData(string path, string fileName)
        //{          
        //    DateTime today = DateTime.Today;
        //    if (today.DayOfWeek == DayOfWeek.Monday && today.Day <= 7)
        //    {
        //        _logger.Info("Start process delete file.");
        //        StartWatching();
        //        var di = new DirectoryInfo(path);
        //        FileInfo fileInfo;
        //        int iCountDel = 0;

        //        if (di.Exists)
        //        {
        //            _logger.Info("Directory Info Exists");

        //            try
        //            {
        //                var fileList = (from file in Directory.EnumerateFiles(path, fileName) where !file.Contains(DateTime.Now.ToString("yyyyMMdd")) select file);

        //                foreach (string file in fileList)
        //                {
        //                    fileInfo = new FileInfo(file);
        //                    _logger.Info(string.Format("File {0} is deleted.", fileInfo.Name));
        //                    System.IO.File.Delete(file);
        //                    iCountDel++;
        //                }

        //                _logger.Info(string.Format("Deleted {0} file(s).", iCountDel.ToSafeString()));
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.Info(ex.GetErrorMessage());
        //                _logger.Info(ex.RenderExceptionMessage());
        //            }
        //        }
        //        else
        //        {
        //            _logger.Info("Directory Not Exists Or Cannot Access");
        //        }

        //        StopWatching("Process delete file");
        //    }            
        //}

        #endregion
    }
}
