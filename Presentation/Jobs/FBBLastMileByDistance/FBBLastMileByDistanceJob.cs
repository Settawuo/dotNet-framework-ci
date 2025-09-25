using System;
using System.Collections.Generic;
using System.Linq;

namespace FBBLastMileByDistance
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    class FBBLastMileByDistanceJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        public FBBLastMileByDistanceJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
             ICommandHandler<SendSmsCommand> SendSmsCommand
           )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
        }

        private List<string> QueryBuild()
        {
            try
            {
                var query = new LastMileByDistanceBatchQuery();
                errorMsg = query.ErrorMessage;
                return _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                SendSms();
                return null;
            }

        }
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("lastmilebydistance : " + _timer.Elapsed);
        }
        public void Execute()
        {
            _logger.Info("lastmilebydistance start.");
            StartWatching();
            try
            {
                var data = QueryBuild();

                if (data != null && data.Any())
                {

                    switch (data.FirstOrDefault())
                    {
                        case "1":
                            _logger.Info("lastmilebydistance : Fail.");
                            break;
                        default:
                            _logger.Info("lastmilebydistance  : Success.");
                            break;
                    }
                }
                else
                {
                    _logger.Info("lastmilebydistance : The process in Packages have a problem, please check.");
                    _logger.Info(string.Format("lastmilebydistance : {0}", errorMsg.ToSafeString()));
                }

                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("lastmilebydistance :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching();
                SendSms();
            }
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
                            command.FullUrl = "FBBLastMileByDistance";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBLastMileByDistance Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
