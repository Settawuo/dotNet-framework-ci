using System;
using System.Collections.Generic;
using System.Linq;
namespace FBBResendRevalue
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.FBBWebConfigCommands;
    using WBBContract.Queries.ExWebServices;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.ExWebServiceModels;
    //using CompositionRoot;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    public class FBBResendRevalueBatchJob
    {

        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private string _outErrorResult = string.Empty;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogCommand;
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }
        public FBBResendRevalueBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
            ICommandHandler<UpdateFbssFOAConfigTblCommand> intfLogCommand
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
            _intfLogCommand = intfLogCommand;
        }

        public void ResendRevalue()
        {
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_RESENDREVALUE_BATCH", "DATE_START").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_RESENDREVALUE_BATCH", "DATE_TO").FirstOrDefault();
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_RESENDREVALUE_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                string msg = "";
                int indexSuccess = 0;
                int indexError = 0;
                if (program_process.DISPLAY_VAL == "Y")
                {
                    var searchModel = new SubmitFOARevalueBatchQuery();
                    searchModel.internetNo = string.IsNullOrEmpty(searchModel.internetNo.ToSafeString()) ? "ALL" : searchModel.internetNo.ToSafeString();
                    searchModel.orderNo = string.IsNullOrEmpty(searchModel.orderNo.ToSafeString()) ? "ALL" : searchModel.orderNo.ToSafeString();
                    searchModel.companyCode = string.IsNullOrEmpty(searchModel.companyCode.ToSafeString()) ? "ALL" : searchModel.companyCode.ToSafeString();
                    searchModel.mainasset = string.IsNullOrEmpty(searchModel.mainasset.ToSafeString()) ? "ALL" : searchModel.mainasset.ToSafeString();
                    searchModel.action = string.IsNullOrEmpty(searchModel.action.ToSafeString()) ? "ALL" : searchModel.action.ToSafeString();
                    searchModel.status = "Pending";
                    searchModel.dateFrom = date_start.DISPLAY_VAL == "Y" ? date_start.VAL1.ToSafeString() : DateTime.Now.AddDays(-1).ToString("ddMMyyyy HHmmss");
                    searchModel.dateTo = date_to.DISPLAY_VAL == "Y" ? date_to.VAL1.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss");
                    var result = this.GetSubmitFOARevalue(searchModel);

                    var searchModelSeverError = new SubmitFOARevalueBatchQuery();
                    searchModelSeverError.internetNo = string.IsNullOrEmpty(searchModel.internetNo.ToSafeString()) ? "ALL" : searchModel.internetNo.ToSafeString();
                    searchModelSeverError.orderNo = string.IsNullOrEmpty(searchModel.orderNo.ToSafeString()) ? "ALL" : searchModel.orderNo.ToSafeString();
                    searchModelSeverError.companyCode = string.IsNullOrEmpty(searchModel.companyCode.ToSafeString()) ? "ALL" : searchModel.companyCode.ToSafeString();
                    searchModelSeverError.mainasset = string.IsNullOrEmpty(searchModel.mainasset.ToSafeString()) ? "ALL" : searchModel.mainasset.ToSafeString();
                    searchModelSeverError.action = string.IsNullOrEmpty(searchModel.action.ToSafeString()) ? "ALL" : searchModel.action.ToSafeString();
                    searchModelSeverError.status = "ERROR";
                    searchModelSeverError.dateFrom = date_start.DISPLAY_VAL == "Y" ? date_start.VAL1.ToSafeString() : DateTime.Now.AddDays(-1).ToString("ddMMyyyy HHmmss");
                    searchModelSeverError.dateTo = date_to.DISPLAY_VAL == "Y" ? date_to.VAL1.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss");
                    var resultError = this.GetSubmitFOARevalue(searchModelSeverError).Where(x => x.ERR_MSG == "Server Error").ToList();

                    var resultAll = result.Concat(resultError).ToList();

                    var LovQueryData = Get_FBSS_CONFIG_TBL_LOV("FBB_FOA_RESEND", "ACTION");
                    foreach (var item in resultAll)
                    {
                        string action = LovQueryData.Where(x => x.DISPLAY_VAL == item.ACTION).Select(x => x.VAL1).FirstOrDefault();
                        var query = new NewRegistForSubmitFOARevaluePendingQuery()
                        {
                            ACCESS_NUMBER = item.ACCESS_NUMBER,
                            ORDER_NO = item.ORDER_NO,
                            ORDER_TYPE = item.ORDER_TYPE,
                            RUN_GROUP = item.RUN_GROUP,
                            ACTION = action,
                            MAIN_ASSET = item.MAIN_ASSET,
                            SUB_NUMBER = item.SUB_NUMBER,
                            COM_CODE = item.COM_CODE,
                            DOC_DATE = item.DOC_DATE,
                            ERR_CODE = item.ERR_CODE,
                            ERR_MSG = item.ERR_MSG,
                            STATUS = item.STATUS,
                            TRANS_ID = item.TRANS_ID,
                            ITEM_TEXT = item.ITEM_TEXT
                        };
                        NewRegistForSubmitFOAResponse resultNewRegist = _queryProcessor.Execute(query);
                        if (resultNewRegist.result != "") indexSuccess += 1; else indexError += 1;

                        _logger.Info("Access No: " + item.ACCESS_NUMBER);
                    }


                    if (indexSuccess > 0 || indexError > 0)
                    {
                        msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                        msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";

                        _logger.Info("BatchResendRevalue  : " + msg);
                    }
                    else
                    {
                        _logger.Info("BatchResendRevalue  :  No data.");
                    }
                }
                else
                {
                    _logger.Info("BatchResendRevalue  : Program process : " + program_process.DISPLAY_VAL);
                }
                var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                {
                    con_type = "FBB_RESENDREVALUE_BATCH",
                    con_name = "DATE_START",
                    display_val = "Y",
                    val1 = DateTime.Now.ToString("ddMMyyyy HHmmss"),
                    val2 = msg,
                    flag = "EQUIP",
                    updated_by = "FBBResendRevalueBatchJob"
                };
                _intfLogCommand.Handle(queryUpdateDate);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
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
        public List<SubmitFOARevalue> GetSubmitFOARevalue(SubmitFOARevalueBatchQuery model)
        {
            try
            {
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                SendSms();
                return new List<SubmitFOARevalue>();
            }
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
                            command.FullUrl = "FBBResendRevalue";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBResendRevalue Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
