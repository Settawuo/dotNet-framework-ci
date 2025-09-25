using System;
using System.Collections.Generic;
using System.Linq;

namespace FBBPayGCheckLoadFile
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    public class FBBPayGCheckLoadFileJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        public FBBPayGCheckLoadFileJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
        }
        public void Execute()
        {
            _logger.Info("PAYG start Check load file.");
            StartWatching();
            try
            {
                var data = QueryBuild();

                if (data != null && data.Any())
                {
                    switch (data.FirstOrDefault())
                    {
                        case "1":
                            _logger.Info("PAYG Check load file : Fail.");
                            break;
                        case "2":
                            SendSms(2);
                            _logger.Info("PAYG Check load file  : Pass with Error.");
                            break;
                        case "3":
                            SendSms(3);
                            _logger.Info("PAYG Check load file  :Case not have some file and load file failed");
                            break;
                        default:
                            _logger.Info("PAYG Check load file  : Success.");
                            break;
                    }
                }
                else
                {
                    _logger.Info("PAYG Check load file : The process in Packages have a problem, please check.");
                    _logger.Info(string.Format("PAYGCheck load file : {0}", errorMsg.ToSafeString()));
                }

                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("Check load file :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching();
                SendSms();
                throw ex;
            }
        }

        private List<string> QueryBuild()
        {
            var query = new PAYGCheckLoadFileQuery();
            errorMsg = query.ErrorMessage;
            return _queryProcessor.Execute(query);
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("PAYG start load file to table take : " + _timer.Elapsed);
        }
        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                SendSms();
                _logger.Info("Error GetLovList : " + ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }
        public void SendSms(int ret_code)
        {
            string strMobile = GetLovList("FBB_SMS", "mobile_no")[0].Text;
            string strMessage = GetLovList("FBB_SMS", "message")[0].LovValue1;
            var msg = GetLovList("FBB_SMS", "SMS_MESSAGES").FirstOrDefault();
            string message = "";
            switch (ret_code)
            {
                case 1:
                    _logger.Info("PAYG Check load file : Case 1");
                    message = msg.LovValue1.ToSafeString();
                    break;
                case 2:
                    _logger.Info("PAYG Check load file : Case 2");
                    message = msg.LovValue2.ToSafeString();
                    break;
                case 3:
                    _logger.Info("PAYG Check load file : Case 3");
                    message = msg.LovValue3.ToSafeString();
                    break;
                default:
                    _logger.Info("PAYG Check load file : Case default");
                    message = msg.LovValue4.ToSafeString();
                    break;
            }
            //List<string> mess = new List<string>();
            if (!string.IsNullOrEmpty(strMobile))
            {
                var mobile = strMobile.Split(',');
                //if (!string.IsNullOrEmpty(strMessage))
                //{
                //    mess = Split(strMessage, 100).ToList();
                //}
                foreach (var item in mobile)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        //foreach (var m in mess)
                        //{
                        var command = new SendSmsCommand();
                        command.FullUrl = "FBBPayGCheckLoadFile";
                        command.Source_Addr = "FBBBATCH";
                        command.Destination_Addr = item;
                        command.Transaction_Id = item;
                        command.Message_Text = message;
                        _sendSmsCommand.Handle(command);
                        //Thread.Sleep(15000);
                        //}

                    }

                }

            }

        }
        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }
        public void SendSms()
        {
            var getMobile = Get_FBSS_CONFIG_TBL_LOV("FBB_MOBILE_ERROR_BATCH", "MOBILE_SMS").FirstOrDefault();
            if (getMobile != null)
            {
                if (!string.IsNullOrEmpty(getMobile.VAL1) && getMobile.DISPLAY_VAL == "Y")
                {
                    var mobile = getMobile.VAL1.Split(',');

                    foreach (var item in mobile)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var command = new SendSmsCommand();
                            command.FullUrl = "FBBPayGCheckLoadFile";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBPayGCheckLoadFile Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
