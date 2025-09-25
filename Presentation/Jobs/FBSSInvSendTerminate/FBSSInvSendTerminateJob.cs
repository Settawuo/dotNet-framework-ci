using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace FBSSInvSendTerminate
{
    public class FBSSInvSendTerminateJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IQueryProcessor _queryFixedAssetConfig;
        public FBSSInvSendTerminateJob(
             ILogger logger,
             IQueryProcessor queryProcessor,
              ICommandHandler<SendSmsCommand> SendSmsCommand,
           IQueryProcessor queryFixedAssetConfig
           )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
            _queryFixedAssetConfig = queryFixedAssetConfig;
        }
        public void Execute()
        {
            _logger.Info("Start Execute FBSSInvSendTerminate Log.");

            try
            {
                StartWatching();
                //DateTime dt = DateTime.Now;
                //string tem_date = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                var query = new GetFixedAssetConfigQuery()
                {
                    Program = "Flag_RollbackSAP"
                };
                var _FbssConfig = _queryFixedAssetConfig.Execute(query).FirstOrDefault();

                if (_FbssConfig.DISPLAY_VAL == "Y")
                {
                    var queryInv = new FBSSInvSendTerminateQuery()
                    {
                        p_term_date = "",

                    };

                    var result = _queryProcessor.Execute(queryInv);

                }
                else
                {
                    //ตัวใหม่
                    var queryInv = new FBSSInvSendTerminateS4HANAQuery()
                    {
                        p_term_date = "",

                    };

                    var result = _queryProcessor.Execute(queryInv);
                }


            }
            catch (Exception ex)
            {
                _logger.Info("Error FBSSInvSendTerminate Log :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("FBSSInvSendTerminate Log take");
                SendSms();
            }
            StopWatching("StopWatching FBSSInvSendTerminate Log take");

        }
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string Message)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} : {1}", Message, _timer.Elapsed));

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
                            command.FullUrl = "FBSSInvSendTerminate";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBSSInvSendTerminate Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
