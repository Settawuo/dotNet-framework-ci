using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Minions;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace DelectInterfaceLogARC
{
    public class DelectInterfaceLogARCJob
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

        public DelectInterfaceLogARCJob(
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

        public string Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("DelectInterfaceLogARCJob: Start");

                //DelectInterfaceLogARCModel result = new DelectInterfaceLogARCModel();
                //var query = new DelectInterfaceLogARCQuery { };
                //result = _queryProcessor.Execute(query);
                var result = "1";

                Console.WriteLine("Start send mail");
                QueryDataToSendMail(result);
                Console.WriteLine("End send mail");
                
                _logger.Info("Done");
                StopWatching("DelectInterfaceLogARCJob");
                return "1";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                _logger.Info("DelectInterfaceLogARCJob :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("DelectInterfaceLogARCJob");
                return "";
            }
        }
        
        public void QueryDataToSendMail(string result)
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
                string strSubject = ""; // P_SUBJECT

                string strBody = "Dear All<br>";
                //P_BODY_H <br> +
                //P_BODY_RESULT <br> +
                //P_BODY_SUMMARY <br> +
                //P_BODY_SIGNATURE <br> ;

                string[] sendResult = Sendmail("SEND_EMAIL_DELECT_INTERFACE_LOG_ARC_BATCH", "BATCH", strFrom, strTo, strCC,
                    strBCC, strSubject, strBody, IPMailServer, FromPassword, Port, Domaim);
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
                errorAttFile.fileName = "DelectInterfaceLogARC_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";
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
