using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DormPAYGReport
{
    using System.Diagnostics;
    using System.IO;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands.FBBWebConfigCommands;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.Extensions;

    public class DormPAYGReportJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        //private readonly ICommandHandler<SendMailBatchCommand> _Sendmail;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;

        private bool isSendMail = false;
        private List<SendMailDetailModel> sendMailModel = new List<SendMailDetailModel>();

        public DormPAYGReportJob(
            ILogger logger,
            IQueryProcessor queryProcessor)/*,
            ICommandHandler<SendMailBatchCommand> Sendmail)*/
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            //_Sendmail = Sendmail;
        }

        public void Execute()
        {
            ExecuteMaster();
            ExecuteDTL();
            ExecuteZipCode();
        }

        public void ExecuteMaster()
        {
            _logger.Info("DormPAYGReport Extract Dormitort Master");
            StartWatching();
            try
            {
                var data = QueryBuildMaster();

                if (data != null && data.Any())
                {
                    switch (data.FirstOrDefault())
                    {
                        case "-1": //Fail call Packages
                            errorMsg = data[1].ToSafeString();
                            _logger.Info("DormPAYGReport Extract Dormitort Master : The process in Packages have a problem, please check.");
                            _logger.Info(errorMsg);

                            isSendMail = true;
                            PrepareDataForSendMail("Master", "ERROR", errorMsg);
                            break;
                        case "1": // Fail : Packages EXCEPTION
                            errorMsg = string.Format("Packages EXCEPTION : {0}.", data[1].ToSafeString());
                            _logger.Info("DormPAYGReport Extract Dormitort Master : Fail.");
                            _logger.Info(errorMsg);

                            isSendMail = true;
                            PrepareDataForSendMail("Master", "FAIL", errorMsg);
                            break;
                        case "0": //Success
                            _logger.Info("DormPAYGReport Extract Dormitort Master : Success.");
                            break;
                    }
                }

                StopWatching("Master");
            }
            catch (Exception ex)
            {
                _logger.Info("DormPAYGReport Extract Dormitort Master :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());

                isSendMail = true;
                PrepareDataForSendMail("Master", "ERROR", ex.GetErrorMessage());
                StopWatching("Master");
            }
        }

        private List<string> QueryBuildMaster()
        {
            try
            {
                var command = new GetExtractDormitoryMaster();

                var result = _queryProcessor.Execute(command);
                errorMsg = command.ErrorMessage;
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.RenderExceptionMessage());
                return null;
            }
        }

        public void ExecuteDTL()
        {
            _logger.Info("DormPAYGReport Extract Dormitort DTL.");
            StartWatching();
            try
            {
                var data = QueryBuildDTL();

                if (data != null && data.Any())
                {
                    switch (data.FirstOrDefault())
                    {
                        case "-1": //Fail call Packages
                            errorMsg = data[1].ToSafeString();
                            _logger.Info("DormPAYGReport Extract Dormitort DTL : The process in Packages have a problem, please check.");
                            _logger.Info(errorMsg);

                            isSendMail = true;
                            PrepareDataForSendMail("DTL", "ERROR", errorMsg);
                            break;
                        case "1": // Fail : Packages EXCEPTION
                            errorMsg = string.Format("Packages EXCEPTION : {0}.", data[1].ToSafeString());
                            _logger.Info("DormPAYGReport Extract Dormitort DTL : Fail.");
                            _logger.Info(errorMsg);

                            isSendMail = true;
                            PrepareDataForSendMail("DTL", "FAIL", errorMsg);
                            break;
                        case "0": //Success
                            _logger.Info("DormPAYGReport Extract Dormitort DTL : Success.");
                            break;
                    }
                }

                StopWatching("DTL");
            }
            catch (Exception ex)
            {
                _logger.Info("DormPAYGReport Extract Dormitort DTL :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());

                isSendMail = true;
                PrepareDataForSendMail("DTL", "ERROR", ex.GetErrorMessage());

                StopWatching("DTL");
            }
        }

        private List<string> QueryBuildDTL()
        {
            try
            {
                var query = new GetExtractDormitoryDTL();
                var result = _queryProcessor.Execute(query);
                errorMsg = query.ErrorMessage;
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.RenderExceptionMessage());
                return null;
            }
        }

        public void ExecuteZipCode()
        {
            _logger.Info("DormPAYGReport Extract Dormitort ZipCode.");
            StartWatching();
            try
            {
                var data = QueryBuildZipCode();

                if (data != null && data.Any())
                {
                    switch (data.FirstOrDefault())
                    {
                        case "-1": //Fail call Packages
                            errorMsg = data[1].ToSafeString();
                            _logger.Info("DormPAYGReport Extract Dormitort ZipCode : The process in Packages have a problem, please check.");
                            _logger.Info(errorMsg);

                            isSendMail = true;
                            PrepareDataForSendMail("ZipCode", "ERROR", errorMsg);
                            break;
                        case "1": // Fail : Packages EXCEPTION
                            errorMsg = string.Format("Packages EXCEPTION : {0}.", data[1].ToSafeString());
                            _logger.Info("DormPAYGReport Extract Dormitort ZipCode : Fail.");
                            _logger.Info(errorMsg);

                            isSendMail = true;
                            PrepareDataForSendMail("ZipCode", "FAIL", errorMsg);
                            break;
                        case "0": //Success
                            _logger.Info("DormPAYGReport Extract Dormitort ZipCode : Success.");
                            break;
                    }
                }

                StopWatching("ZipCode");
            }
            catch (Exception ex)
            {
                _logger.Info("DormPAYGReport Extract Dormitort ZipCode :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());

                isSendMail = true;
                PrepareDataForSendMail("ZipCode", "ERROR", ex.GetErrorMessage());

                StopWatching("ZipCode");
            }
        }

        private List<string> QueryBuildZipCode()
        {
            try
            {
                var query = new GetExtractDormitoryZipCode();
                var result = _queryProcessor.Execute(query);
                errorMsg = query.ErrorMessage;
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.RenderExceptionMessage());
                return null;
            }
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("DormPAYGReport Extract Dormitort {0} take : {1}", mode, _timer.Elapsed));
        }

        public void PrepareDataForSendMail(string mode, string result, string errormsg)
        {
            var model = new SendMailDetailModel
            {
                Project = "DormPAYGReport",
                Mode = string.Format("DormPAYGReport Extract Dormitort {0}", mode),
                Result = result,
                ErrorMsg = errormsg
            };

            sendMailModel.Add(model);
        }

        public void SendMail()
        {
            if (!isSendMail)
                return;

            StartWatching();
            _logger.Info("DormPAYGReport Extract Dormitort start sending an Email.");
            var command = new SendMailBatchCommand
            {
                SendMail = sendMailModel
            };

            try
            {
                //_Sendmail.Handle(command);
                _logger.Info("DormPAYGReport Extract Dormitort Sending an Email : Success.");

                StopWatching("SendMail");
            }
            catch (Exception ex)
            {
                _logger.Info(string.Format("DormPAYGReport Extract Dormitort Sending an Email is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                _logger.Info(command.Return_Message);
                StopWatching("SendMail");
            }
        }

        /// <summary>
        /// Delete file in pervious month.
        /// </summary>
        public void DeleteFileInFirstDateOfMonth()
        {
            DateTime today = DateTime.Today;
            DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);

            //if (today.Day != firstOfMonth.Day)
            //{
            //    return;
            //}

            DateTime deleteMonth = new DateTime(today.Year, today.Month, 1);
            string deleteFileDate;
            if (deleteMonth.Month < deleteMonth.AddMonths(-2).Month)
            {
                deleteFileDate = string.Format("{0}{1}", today.Year - 1, deleteMonth.AddMonths(-2).Month.ToString("00"));
            }
            else
            {
                deleteFileDate = string.Format("{0}{1}", today.Year, deleteMonth.AddMonths(-2).Month.ToString("00"));
            }

            string tempPath = ConfigurationManager.AppSettings["nasPath"];
            string tempFile = "FBSS_DORM_{0}_info_{1}*.*";
            string fileDelete = string.Empty;
            string[] fileList;
            FileInfo fileInfo;
            int iCountDel = 0;

            string[] modeList = { "MASTER", "DTL", "ZIPCODE" };

            _logger.Info(string.Format("Start process Delete file on {0}.", today.ToDateDisplayText()));

            if (System.IO.Directory.Exists(tempPath))
            {
                foreach (string mode in modeList)
                {
                    fileDelete = string.Format(tempFile, mode, deleteFileDate);
                    fileList = System.IO.Directory.GetFiles(tempPath, fileDelete);

                    foreach (string file in fileList)
                    {
                        fileInfo = new FileInfo(file);
                        _logger.Info(string.Format("File {0} is deleted.", fileInfo.Name));
                        System.IO.File.Delete(file);
                        iCountDel++;
                    }
                }

                _logger.Info(string.Format("Deleted file total {0}.", iCountDel.ToSafeString()));

            }
            else
            {
                _logger.Info(string.Format("Path {0} not exists.", tempPath));
            }

            _logger.Info("End process Delete file");
        }
    }
}
