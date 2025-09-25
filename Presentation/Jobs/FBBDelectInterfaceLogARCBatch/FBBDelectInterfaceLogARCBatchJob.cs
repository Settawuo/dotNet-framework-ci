using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBDelectInterfaceLogARCBatch
{
    public class FBBDelectInterfaceLogARCBatchJob
    {
        #region Properties

        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        private string _outErrorResult = string.Empty;
        private int _numCompleted, _numCancelWf, _numCancelFbbs;

        #endregion

        #region Constructor

        public FBBDelectInterfaceLogARCBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _sendMail = sendMail;
        }

        #endregion

        #region Public Methods

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("FBBDelectInterfaceLogARCBatchJob: Start");

                DelectInterfaceLogARCModel result = new DelectInterfaceLogARCModel();
                var query = new DelectInterfaceLogARCQuery { };
                result = _queryProcessor.Execute(query);
                var dataSendmail = result.P_EMAIL.FirstOrDefault();

                Console.WriteLine("Start send mail");
                QueryDataToSendMail(dataSendmail);
                Console.WriteLine("End send mail");

                _logger.Info("Done");
                StopWatching("FBBDelectInterfaceLogARCBatchJob");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                _logger.Info("FBBDelectInterfaceLogARCBatchJob :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("FBBDelectInterfaceLogARCBatchJob");
            }
        }

#pragma warning disable CS0246 // The type or namespace name 'DataSendEmailDelectInterface' could not be found (are you missing a using directive or an assembly reference?)
        public void QueryDataToSendMail(DataSendEmailDelectInterface dataSendmail)
#pragma warning restore CS0246 // The type or namespace name 'DataSendEmailDelectInterface' could not be found (are you missing a using directive or an assembly reference?)
        {
            //TODO: GET Subject and detail 
            string strFrom = "";
            string strTo = "";
            string strCC = "";
            string strBCC = "";
            string IPMailServer = "";

            try
            {
                int numTotal = _numCompleted + _numCancelWf + _numCancelFbbs;
                string FromPass = "";// Fixed Code scan : string FromPassword = "";
                string Port = "";
                string Domaim = "";
                string strSubject = dataSendmail.P_SUBJECT.ToSafeString();

                string strBody =
                dataSendmail.P_BODY_H.ToSafeString() + "<br />" +
                dataSendmail.P_BODY_RESULT.ToSafeString() + "<br />" +
                dataSendmail.P_BODY_SUMMARY.ToSafeString() + "<br />" +
                dataSendmail.P_BODY_SIGNATURE.ToSafeString() + "<br />";

                string[] sendResult = Sendmail("SEND_EMAIL_INTERFACE_LOG", "BATCH", strFrom, strTo, strCC,
                    strBCC, strSubject, strBody, IPMailServer, FromPass, Port, Domaim);
            }
            catch (Exception ex)
            {
                _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());
            }
        }

        public string[] Sendmail(string processname, string createuser, string sendfrom, string sendto, string sendcc, string sendbcc
            , string subject, string body, string ip_mail_server, string frompass, string port, string domain)
        {
            List<MemoryStreamAttachFiles> files = new List<MemoryStreamAttachFiles>();
            if (string.IsNullOrEmpty(_outErrorResult))
            {
                files = null;
            }
            else
            {
                MemoryStreamAttachFiles errorAttFile = new MemoryStreamAttachFiles();
                errorAttFile.file = new MemoryStream(Encoding.UTF8.GetBytes(_outErrorResult));
                errorAttFile.fileName = "FBBDelectInterfaceLogARCBatch_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";
                files.Add(errorAttFile);
            }

            _logger.Info("Sending an Email : Start.");

            string[] result = new string[2];

            StartWatching();
            try
            {
                var command = new SendMailBatchNotificationCommand
                {
                    ProcessName = processname,
                    CreateUser = createuser,
                    SendTo = sendto,
                    SendFrom = sendfrom,
                    Subject = subject,
                    Body = body,
                    FromPassword = frompass,
                    Port = port,
                    Domaim = domain,
                    IPMailServer = ip_mail_server,
                    msAttachFiles = files
                };


                _sendMail.Handle(command);

                _logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                StopWatching("Sending an Email");

                if (command.ReturnMessage == "Success.")
                {
                    result[0] = "0";
                    result[1] = "";
                }
                else
                {
                    result[0] = "-1";
                    result[1] = command.ReturnMessage;
                }

            }
            catch (Exception ex)
            {
                _outErrorResult += "Sending an Email" + string.Format(" is error on execute : {0}.",
                                      ex.GetErrorMessage()) + Environment.NewLine;
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage());

                StopWatching("Sending an Email");
            }

            return result;
        }

        #endregion

        #region Private Methods

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", process, _timer.Elapsed));
        }

        #endregion
    }
}
