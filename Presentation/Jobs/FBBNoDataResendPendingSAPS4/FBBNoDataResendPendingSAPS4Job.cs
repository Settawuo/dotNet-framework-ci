using System;
using System.Collections.Generic;
using System.Linq;
    using Newtonsoft.Json.Linq;

namespace FBBNoDataResendPendingSAPS4
{
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using WBBBusinessLayer;
    using WBBBusinessLayer.FBSSOrderServices;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.FBBWebConfigCommands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.ExWebServices;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBContract.Queries.WebServices;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.ExWebServiceModels;
    //using CompositionRoot;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using WBBEntity.PanelModels.WebServiceModels;

    public class FBBNoDataResendPendingSAPS4Job
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogCommand;
        private readonly IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> _queryProcessorGoodsMovementHandler;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }
        public FBBNoDataResendPendingSAPS4Job(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateFbssFOAConfigTblCommand> intfLogCommand
            , IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> queryProcessorGoodsMovementHandle)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _queryProcessorGoodsMovementHandler = queryProcessorGoodsMovementHandle;
        }


        public void ResendPendingS4()
        {
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBBNoDataResendPendingSAPS4_Batch", "DATE_START").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBBNoDataResendPendingSAPS4_Batch", "DATE_TO").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var date_diff = Get_FBSS_CONFIG_TBL_LOV("FBBNoDataResendPendingSAPS4_Batch", "DATE_DIFF").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var date_to_diff = Get_FBSS_CONFIG_TBL_LOV("FBBNoDataResendPendingSAPS4_Batch", "DATE_TO_DIFF").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();

                var Check_DateDiff = date_diff.DISPLAY_VAL.ToSafeString() == "Y" ? date_diff.VAL1.ToSafeInteger(): 1;
                var Check_DateToDiff = date_to_diff.DISPLAY_VAL.ToSafeString() == "Y" ? date_to_diff.VAL1.ToSafeInteger(): 1;

                _logger.Info("DATE_START : DISPLAY_VAL :: " + date_start.DISPLAY_VAL + " VAL_1 :: " + date_start.VAL1);
                _logger.Info("DATE_TO : DISPLAY_VAL :: " + date_to.DISPLAY_VAL + " VAL_1 :: " + date_to.VAL1);
                _logger.Info("DATE_DIFF : DISPLAY_VAL :: " + date_diff.DISPLAY_VAL + " VAL_1 :: " + date_diff.VAL1);
                _logger.Info("DATE_TO_DIFF : DISPLAY_VAL :: " + date_to_diff.DISPLAY_VAL + " VAL_1 :: " + date_to_diff.VAL1);
                try
                {

                    ///////////////  DATE_TO
                    string c_DATE_TO = string.Empty;
                    DateTime parsedDateTo = DateTime.Now;
                    if (date_to.DISPLAY_VAL == "Y")
                    {
                        DateTime.TryParseExact(date_to.VAL1, "ddMMyyyy HHmmss", null, System.Globalization.DateTimeStyles.None, out parsedDateTo);
                        c_DATE_TO = parsedDateTo.ToString("ddMMyyyy HHmmss");
                    }
                    else
                    {
                        parsedDateTo = DateTime.Now.AddDays(-Check_DateToDiff);
                        c_DATE_TO = parsedDateTo.ToString("ddMMyyyy HHmmss");
                    }

                    ///////////////  DATE_START
                    DateTime parsedDateStart;
                    string c_DATE_START = string.Empty;
                    if (date_start.DISPLAY_VAL == "Y")
                    {
                        DateTime.TryParseExact(date_start.VAL1, "ddMMyyyy HHmmss", null, System.Globalization.DateTimeStyles.None, out parsedDateStart);
                        c_DATE_START = parsedDateStart.ToString("ddMMyyyy HHmmss");
                    }
                    else
                    {
                        parsedDateStart = parsedDateTo.AddDays(-Check_DateDiff);
                        c_DATE_START = parsedDateStart.ToString("ddMMyyyy HHmmss");
                    }


                    _logger.Info("DATE_START :: " + c_DATE_START);
                    _logger.Info("DATE_TO :: " + c_DATE_TO);

                    ////////////////// UPDATE DISPLAY_VAL DATE_START เป็นค่า Y
                    var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBNoDataResendPendingSAPS4_Batch",
                        con_name = "DATE_START",
                        display_val = "Y",
                        val1 = c_DATE_START,
                        flag = "EQUIP",
                        updated_by = "FBBNoDataResendPendingSAPS4Batch"
                    };
                    _intfLogCommand.Handle(queryUpdateDate);
                    _logger.Info("Update DISPLAY_VAL DATE_START to Y");


                    ////////////////// CALL PKG WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.P_GET_NODATA_RESEND_PENDING
                    _logger.Info("Call P_GET_NODATA_RESEND_PENDING");
                    var query = new FBBNoDataResendPendingSAPS4Query()
                    {
                        p_Date_Start = c_DATE_START,
                        p_Date_To = c_DATE_TO
                    };
                    var FBBNoDataResendPendingSAPS4Response = _queryProcessor.Execute(query);


                    if (FBBNoDataResendPendingSAPS4Response.item_json == null || FBBNoDataResendPendingSAPS4Response.item_json.Count == 0)
                    {
                        _logger.Info("No Data to produce");
                    }
                    else
                    {
                        StringBuilder partnerMessageIDs = new StringBuilder();
                        foreach (var item in FBBNoDataResendPendingSAPS4Response.item_json)
                        {

                            var jsonObject = JObject.Parse(item);
                            string partnerMessageID = jsonObject["body"]?["PartnerMessageID"]?.ToString();
                            if (!string.IsNullOrEmpty(partnerMessageID))
                            {
                                if (partnerMessageIDs.Length > 0)
                                {
                                    partnerMessageIDs.Append(", ");
                                }
                                partnerMessageIDs.Append(partnerMessageID);
                            }
                        }
                        _logger.Info("partnerMessageID : " + partnerMessageIDs);


                        var batch_Size = Get_FBSS_CONFIG_TBL_LOV("FBBNoDataResendPendingSAPS4_Batch", "BATCH_SIZE").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                        List<string> allItems = FBBNoDataResendPendingSAPS4Response.item_json;
                        int batchSize = batch_Size?.VAL1 != null
                            ? batch_Size.VAL1.ToSafeInteger()
                            : 300;
                        List<List<string>> batches = new List<List<string>>();
                        List<string> currentItem = new List<string>(batchSize);
                        foreach (string item in allItems)
                        {
                            currentItem.Add(item);
                            if (currentItem.Count == batchSize)
                            {
                                batches.Add(currentItem);
                                currentItem = new List<string>(batchSize);
                            }

                        }
                        if (currentItem.Count > 0)
                        {
                            batches.Add(currentItem);
                        }

                        ////////////////// CALL GoodsMovementKAFKAQueryHandler
                        _logger.Info("Call GoodsMovementKAFKAQueryHandler");
                        foreach (var itemBatch in batches)
                        {
                            var queryGoodsmovement = new GoodsMovementKAFKAQuery()
                            {
                                action = "PRODUCER",
                                item_json = itemBatch
                            };
                            _queryProcessorGoodsMovementHandler.Handle(queryGoodsmovement);
                        }
                    }






                    ////////////////// UPDATE DISPLAY_VAL DATE_START, DATE_TO เป็นค่า N
                    var updateDateStart_L = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBNoDataResendPendingSAPS4_Batch",
                        con_name = "DATE_START",
                        display_val = "N",
                        val1 = DateTime.Now.ToString("ddMMyyyy HHmmss"),
                        flag = "EQUIP",
                        updated_by = "FBBNoDataResendPendingSAPS4Batch"
                    };
                    _intfLogCommand.Handle(updateDateStart_L);
                    _logger.Info("Update DISPLAY_VAL DATE_START to N");


                    var updateDateTo_L = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBNoDataResendPendingSAPS4_Batch",
                        con_name = "DATE_TO",
                        display_val = "N",
                        val1 = DateTime.Now.ToString("ddMMyyyy HHmmss"),
                        flag = "EQUIP",
                        updated_by = "FBBNoDataResendPendingSAPS4Batch"
                    };
                    _intfLogCommand.Handle(updateDateTo_L);
                    _logger.Info("Update DISPLAY_VAL DATE_TO to N");
                }
                catch (Exception ex)
                {
                    _logger.Info("Exception at FBBNoDataResendPendingSAPS4Job ERROR : " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Exception at ResendPendingS4 Error: "+ ex.Message);
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
  
    }
}
