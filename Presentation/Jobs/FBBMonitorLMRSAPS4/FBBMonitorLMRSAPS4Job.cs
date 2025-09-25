using System;
using System.Collections.Generic;
using System.Linq;

namespace FBBMonitorLMRSAPS4
{
    using Newtonsoft.Json.Linq;
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

    public class FBBMonitorLMRSAPS4Job
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogCommand;
        private readonly IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> _queryProcessorGoodsMovementHandler;

        public FBBMonitorLMRSAPS4Job(
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
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBBMonitorLMRSAPS4_Batch", "DATE_START").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBBMonitorLMRSAPS4_Batch", "DATE_TO").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                var date_diff = Get_FBSS_CONFIG_TBL_LOV("FBBMonitorLMRSAPS4_Batch", "DATE_DIFF").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();

                int Check_DateDiff = 0;


                _logger.Info("DATE_START : DISPLAY_VAL :: " + date_start.DISPLAY_VAL + " VAL_1 :: " + date_start.VAL1);
                _logger.Info("DATE_TO : DISPLAY_VAL :: " + date_to.DISPLAY_VAL + " VAL_1 :: " + date_to.VAL1);
                _logger.Info("DATE_DIFF : DISPLAY_VAL :: " + date_diff.DISPLAY_VAL + " VAL_1 :: " + date_diff.VAL1);
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
                        parsedDateTo = DateTime.Now;
                        c_DATE_TO = parsedDateTo.ToString("ddMMyyyy HHmmss");
                    }

                    ///////////////  DATE_START
                    DateTime parsedDateStart;
                    string c_DATE_START = string.Empty;
                    if (date_start.DISPLAY_VAL == "Y")
                    {
                        DateTime.TryParseExact(date_start.VAL1, "ddMMyyyy HHmmss", null, System.Globalization.DateTimeStyles.None, out parsedDateStart);
                        c_DATE_START = parsedDateStart.ToString("ddMMyyyy HHmmss");
                        //parsedDateStart = DateTime.ParseExact(c_DATE_START, "ddMMyyyy HHmmss", null);
                    }
                    else
                    {
                        parsedDateStart = DateTime.Now.AddDays(-1);
                        c_DATE_START = parsedDateStart.ToString("ddMMyyyy HHmmss");
                    }

                    ///////////////  DATE_DIFF
                    if (date_diff.DISPLAY_VAL == "Y")
                    {
                        Check_DateDiff = date_diff.VAL1.ToSafeInteger();
                        parsedDateStart = parsedDateStart.AddDays(-Check_DateDiff);
                        c_DATE_START = parsedDateStart.ToString("ddMMyyyy HHmmss");
                    }

                    _logger.Info("DATE_START :: " + c_DATE_START);
                    _logger.Info("DATE_TO :: " + c_DATE_TO);

                    ////////////////// UPDATE DISPLAY_VAL DATE_START เป็นค่า Y
                    var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBMonitorLMRSAPS4_Batch",
                        con_name = "DATE_START",
                        display_val = "Y",
                        val1 = c_DATE_START,
                        flag = "EQUIP",
                        updated_by = "FBBMonitorLMRSAPS4Batch"
                    };
                    _intfLogCommand.Handle(queryUpdateDate);
                    _logger.Info("Update DISPLAY_VAL DATE_START to Y");

                    ////////////////// CALL PKG wbb.FBBPAYG_FOA_RESEND_ORDER.p_monitor_lmr 
                    var query = new FBBMonitorLMRSAPS4Query()
                    {
                        p_Date_Start = c_DATE_START,
                        p_Date_To = c_DATE_TO
                    };
                    var FBBMonitorLMRSAPS4Response = _queryProcessor.Execute(query);

                    if (FBBMonitorLMRSAPS4Response.item_json == null || FBBMonitorLMRSAPS4Response.item_json.Count == 0)
                    {
                        _logger.Info("No Data to produce");
                    }
                    else
                    {
                        StringBuilder partnerMessageIDs = new StringBuilder();
                        foreach (var item in FBBMonitorLMRSAPS4Response.item_json)
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


                        ////////////////// CALL GoodsMovementKAFKAQueryHandler
                        _logger.Info("Call GoodsMovementKAFKAQueryHandler");
                        var queryGoodsmovement = new GoodsMovementKAFKAQuery()
                        {
                            action = "PRODUCER",
                            item_json = FBBMonitorLMRSAPS4Response.item_json
                        };
                        _queryProcessorGoodsMovementHandler.Handle(queryGoodsmovement);
                    }

                    ////////////////// UPDATE DISPLAY_VAL DATE_START, DATE_TO เป็นค่า N
                    var updateDateStart_L = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBMonitorLMRSAPS4_Batch",
                        con_name = "DATE_START",
                        display_val = "N",
                        val1 = DateTime.Now.ToString("ddMMyyyy HHmmss"),
                        flag = "EQUIP",
                        updated_by = "FBBMonitorLMRSAPS4Batch"
                    };
                    _intfLogCommand.Handle(updateDateStart_L);
                    _logger.Info("Update DISPLAY_VAL DATE_START to N");


                    var updateDateTo_L = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBMonitorLMRSAPS4_Batch",
                        con_name = "DATE_TO",
                        display_val = "N",
                        val1 = DateTime.Now.ToString("ddMMyyyy HHmmss"),
                        flag = "EQUIP",
                        updated_by = "FBBMonitorLMRSAPS4Batch"
                    };
                    _intfLogCommand.Handle(updateDateTo_L);
                    _logger.Info("Update DISPLAY_VAL DATE_TO to N");
                }
                catch (Exception ex)
                {
                    _logger.Info("Exception at FBBMonitorLMRSAPS4Job ERROR : " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Exception at FBBMonitorLMRSAPS4 Error: " + ex.Message);
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
