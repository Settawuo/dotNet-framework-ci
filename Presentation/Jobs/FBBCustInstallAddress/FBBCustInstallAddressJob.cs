using System;
using System.Diagnostics;


namespace FBBCustInstallAddress
{
    using System.Threading;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.FBBWebConfigCommands;
    using WBBEntity.Extensions;

    public class FBBCustInstallAddressJob
    {
        private string errorMsg = string.Empty;
        public ILogger _logger;
        private readonly ICommandHandler<FBBCustInstallAddressCommand> _reconcile;
        private readonly ICommandHandler<FBBCustInstallDeleteCommand> _reconcile2;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private Stopwatch _timer;
        private string _outErrorResult = string.Empty;

        public FBBCustInstallAddressJob(
            ILogger logger,
            ICommandHandler<FBBCustInstallAddressCommand> reconcile,
            ICommandHandler<FBBCustInstallDeleteCommand> reconcile2,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        {
            _logger = logger;
            _reconcile2 = reconcile2;
            _reconcile = reconcile;
            _sendMail = sendMail;

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

        public void ExecuteJob_new()
        {
            StartWatching();
            this.ReconcileJob_new();
            StopWatching("close new.");
        }

        private void ReconcileJob_new()
        {
            _logger.Info(" Start : FBBCustInstallDeleteCommand");
            var command = new FBBCustInstallDeleteCommand { };
            _reconcile2.Handle(command);


            // Loop Call Agian
            if (command.RES_MESSAGE.Trim() == "The underlying provider failed on Open.")
            {
                int re_call = 10;
                for (int i = 1; i <= re_call; i++)
                {
                    if (command.RES_MESSAGE.Trim() == "The underlying provider failed on Open.")
                    {
                        // call agian
                        _logger.Info(" Call Agian Round [" + i.ToSafeString() + "] : FBBCustInstallDeleteCommand");
                        Thread.Sleep(2000); // delay
                        _reconcile2.Handle(command);
                    }
                    else
                    {
                        i = re_call; // stop
                        _logger.Info("Message From Procedure : RES_CODE[ " + command.RES_CODE + " ]" + " RES_MESSAGE[ " + command.RES_MESSAGE + " ]");

                    }
                }
            }
            else
            {
                _logger.Info("Message From Procedure : RES_CODE[ " + command.RES_CODE + " ]" + " RES_MESSAGE[ " + command.RES_MESSAGE + " ]");
            }

            // do _reconcile2 success only
            if (command.RES_CODE == "0")
            {
                _logger.Info(" End : FBBCustInstallDeleteCommand");
            }
            else if (command.RES_CODE == "3")
            {
                _logger.Info(" Error : FBBCustInstallDeleteCommand Exception : (" + command.RES_CODE + ") " + command.RES_MESSAGE);
            }
            else
            {
                _logger.Info(" Error : FBBCustInstallDeleteCommand Exception : (" + command.RES_CODE + ") " + command.RES_MESSAGE);
                QueryDataToSendMail(command);
            }
        }

        public void ExecuteJob()
        {
            StartWatching();
            this.ReconcileJob();
            StopWatching("close.");
        }

        private void ReconcileJob()
        {
            _logger.Info(" Stat : FBBCustInstallAddressCommand");
            _reconcile.Handle(new FBBCustInstallAddressCommand());
            _logger.Info(" End : FBBCustInstallAddressCommand");

        }

        public void QueryDataToSendMail(FBBCustInstallDeleteCommand command)
        {
            //var list = GetDataFOALogError();
            //TODO: GET Subject and detail 
            string strFrom = "";
            string strTo = "";
            string strCC = "";
            string strBCC = "";
            string IPMailServer = "";
            string FromPass = "";// Fixed Code scan : string FromPassword = "";
            string Port = "";
            string Domaim = "";
            string strSubject = "Job FBBCustInstallDelete Error [" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "]";

            try
            {
                string strBody = GenBodyEmailTable(command);

                string[] sendResult = Sendmail("SEND_EMAIL_JOB_FBBCUSTINSTALLDELETE", "", strFrom, strTo, strCC,
                    strBCC, strSubject, strBody, IPMailServer, FromPass, Port, Domaim);
                _logger.Info("SendEmail Success");
            }
            catch (Exception ex)
            {
                _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());
            }
        }

        public string[] Sendmail(string processname, string createuser, string sendfrom, string sendto, string sendcc, string sendbcc
            , string subject, string body, string ip_mail_server, string frompass, string port, string domain)
        {
            _logger.Info("Sending an Email.");

            string[] result = new string[2];

            StartWatching();
            try
            {
                // select * from FBB_EMAIL_PROCESSING where PROCESS_NAME = 'SEND_EMAIL_JOB_FBBCUSTINSTALLDELETE'
                var command = new SendMailBatchNotificationCommand
                {
                    ProcessName = processname,
                    CreateUser = createuser,
                    SendTo = "Dummy",
                    SendFrom = sendfrom,
                    Subject = subject,
                    Body = body,
                    FromPassword = frompass,
                    Port = port,
                    Domaim = domain,
                    IPMailServer = ip_mail_server
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
                _outErrorResult = "Sending an Email" + string.Format(" is error on execute : {0}.",
                                      ex.GetErrorMessage());
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage());

                StopWatching("Sending an Email");
            }

            return result;
        }

        public string GenBodyEmailTable(FBBCustInstallDeleteCommand command)
        {
            string body = "";
            body += "<strong style='color: red;'>Error</strong>";
            body += "<br/><br/>";
            body += "<strong>Job : </strong>" + "FBBCustInstallAddress";
            body += "<br/><br/>";
            body += "<strong>Package Name : </strong>" + "WBB.pkg_fbb_cust_install_delete.p_get_ins_addr";
            body += "<br/><br/>";
            body += "<strong >Exception (" + command.RES_CODE + ") : </strong><strong  style='color: red;'>" + command.RES_MESSAGE + "</strong>";
            body += "<br/><br/>";
            body += "<strong>เวลา : </strong>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            return body;
        }

    }
}
