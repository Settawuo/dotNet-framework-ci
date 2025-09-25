using System;
using System.Collections.Generic;
using System.Linq;

namespace FBBMonitorLmr
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
    public class FBBMonitorLmrBatchJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogCommand;
        private string _outErrorResult = string.Empty;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }
        public FBBMonitorLmrBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateFbssFOAConfigTblCommand> intfLogCommand,
              ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _sendSmsCommand = SendSmsCommand;
        }


        public List<SubmitFOAEquipment> GetSubmitFOABatch()
        {
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_MONITORLMR_BATCH", "DATE_START").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_MONITORLMR_BATCH", "DATE_TO").FirstOrDefault();
                var model = new MonitorLmrQuery()
                {
                    dateFrom = date_start.DISPLAY_VAL == "Y" ? date_start.VAL1.ToSafeString() : DateTime.Now.AddDays(-1).ToString("ddMMyyyy HHmmss"),
                    dateTo = date_to.DISPLAY_VAL == "Y" ? date_to.VAL1.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss"),
                };
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetConfig : " + ex.GetErrorMessage());
                return new List<SubmitFOAEquipment>();
            }
        }
        public List<SubmitFOAInstallation> GetSubmitFOAInstallation()
        {
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_MONITORLMR_BATCH", "DATE_START").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_MONITORLMR_BATCH", "DATE_TO").FirstOrDefault();
                var model = new MonitorLmrInsQuery()
                {

                    dateFrom = date_start.VAL3 == "Y" ? date_start.VAL4.ToSafeString() : DateTime.Now.AddDays(-1).ToString("ddMMyyyy HHmmss"),
                    dateTo = date_to.VAL3 == "Y" ? date_to.VAL4.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss"),
                };
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return new List<SubmitFOAInstallation>();
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
        public void ResendPending()
        {
            string chcekDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            StartWatching();
            var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_MONITORLMR_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
            string msg = "";
            if (program_process.DISPLAY_VAL == "Y")
            {

                int indexSuccess = 0;
                int indexError = 0;

                List<SubmitFOAEquipment> resultSubmitFOAList = GetSubmitFOABatch();

                var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                {
                    con_type = "FBB_MONITORLMR_BATCH",
                    con_name = "DATE_START",
                    display_val = "Y",
                    val1 = chcekDate,
                    val2 = msg,
                    flag = "EQUIP",
                    updated_by = "FBBMonitorLmrBatch"
                };
                _intfLogCommand.Handle(queryUpdateDate);
                _logger.Info("logUpdateDate : " + chcekDate);

                string orderno = "", accessnum = "";
                if (resultSubmitFOAList.Any())
                {
                    var _mainList = new List<NewRegistForSubmitFOAQuery>();
                    foreach (var resultlist in resultSubmitFOAList)
                    {
                        // 2.GetProductList
                        var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                        var resultProductList = _queryProcessor.Execute(queryProductList);

                        var _main = new NewRegistForSubmitFOAQuery();
                        var _product = new List<NewRegistFOAProductList>();

                        // 3.ResendSubmitFOA
                        //if (resultlist.STATUS == "Pending")
                        //{
                        if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                        {
                            _main = new NewRegistForSubmitFOAQuery()
                            {
                                Access_No = resultProductList.AccessNo.ToSafeString(),
                                OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                                SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                                SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                                ProductName = resultProductList.ProductName.ToSafeString(),
                                OrderType = resultProductList.OrderType.ToSafeString(),
                                SubmitFlag = "BATCH_MONITOR_LMR",
                                RejectReason = resultProductList.RejectReason.ToSafeString(),
                                FOA_Submit_date = resultProductList.FOA_Submit_date.ToSafeString(),
                                OLT_NAME = resultProductList.OLT_NAME.ToSafeString(),
                                BUILDING_NAME = resultProductList.BUILDING_NAME.ToSafeString(),
                                Mobile_Contact = resultProductList.Mobile_Contact.ToSafeString(),

                                Post_Date = DateTime.Now.ToString("dd/MM/yyyy"),
                                Address_ID = resultProductList.ADDRESS_ID.ToSafeString(),
                                ORG_ID = resultProductList.ORG_ID.ToSafeString(),
                                Reuse_Flag = resultProductList.REUSE_FLAG.ToSafeString(),
                                Event_Flow_Flag = resultProductList.EVENT_FLOW_FLAG.ToSafeString(),

                                Subcontract_Type = resultProductList.SUBCONTRACT_TYPE.ToSafeString(),
                                Subcontract_Sub_Type = resultProductList.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                                Request_Sub_Flag = resultProductList.REQUEST_SUB_FLAG.ToSafeString(),
                                Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString(),
                                Product_Owner = resultProductList.PRODUCT_OWNER.ToSafeString(),
                                Main_Promo_Code = resultProductList.MAIN_PROMO_CODE.ToSafeString(),
                                Team_ID = resultProductList.TEAM_ID.ToSafeString()
                            };

                            _product = resultProductList.ProductList.Select(p =>
                            {
                                return new NewRegistFOAProductList()
                                {
                                    SerialNumber = p.SerialNumber.ToSafeString(),
                                    MaterialCode = p.MaterialCode.ToSafeString(),
                                    CompanyCode = p.CompanyCode.ToSafeString(),
                                    Plant = p.Plant.ToSafeString(),
                                    StorageLocation = p.StorageLocation.ToSafeString(),
                                    SNPattern = p.SNPattern.ToSafeString(),
                                    MovementType = p.MovementType.ToSafeString()
                                };
                            }).ToList();
                            //}

                            _main.ProductList = _product;

                            var _services = new NewRegistFOAServiceList()
                            {
                                ServiceName = resultProductList.ServiceName != null ? resultProductList.ServiceName : ""
                            };

                            List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                            var subStr = _services.ServiceName.Split(',');
                            foreach (var service in subStr)
                            {
                                _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                            }
                            _main.ServiceList = _service;

                            orderno = resultlist.ORDER_NO;
                            accessnum = resultlist.ACCESS_NUMBER;

                            NewRegistForSubmitFOAResponse resultNewRegist = _queryProcessor.Execute(_main);
                            if (resultNewRegist.result != "") indexSuccess += 1;
                            else indexError += 1;

                            _logger.Info("Access No: " + _main.Access_No);
                        }
                    }


                }

                if (indexSuccess > 0 || indexError > 0)
                {
                    msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                    msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
                }
                else
                {
                    msg = "No data";
                }
                _logger.Info("log finish msg : " + msg);
            }
            else
            {
                msg = "FBBMonitorLmrBatch Program process : " + program_process.DISPLAY_VAL;
                _logger.Info("FBBMonitorLmrBatch Program process : " + program_process.DISPLAY_VAL);
            }

            _logger.Info("logJobDone Date : " + chcekDate);
            StopWatching("End ResendPending");
        }
        public void ResendPendingInstall()
        {
            string chcekDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            StartWatching();
            var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_MONITORLMR_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
            string msg = "";
            if (program_process.VAL1 == "Y")
            {

                int indexSuccess = 0;
                int indexError = 0;
                var getInstall = GetSubmitFOAInstallation();

                var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                {
                    con_type = "FBB_MONITORLMR_BATCH",
                    con_name = "DATE_START",
                    display_val = "Y",
                    val1 = chcekDate,
                    val2 = msg,
                    flag = "INSTALL",
                    updated_by = "FBBMonitorLmrBatch"
                };
                _intfLogCommand.Handle(queryUpdateDate);
                _logger.Info("logUpdate Date : " + chcekDate);

                if (getInstall.Any())
                {
                    var _mainList = new List<NewRegistForSubmitFOAQuery>();
                    foreach (var resultlist in getInstall)
                    {
                        // 2.GetProductList

                        var _main = new NewRegistForSubmitFOAQuery();
                        var _product = new List<NewRegistFOAProductList>();

                        _main = new NewRegistForSubmitFOAQuery()
                        {
                            Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                            OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                            OrderType = "RESEND_INS",
                            SubmitFlag = "RESEND_INS",
                            Product_Owner = resultlist.PRODUCT_OWNER.ToSafeString(),
                            Main_Promo_Code = resultlist.MAIN_PROMO_CODE.ToSafeString(),
                            Team_ID = resultlist.TEAM_ID.ToSafeString()
                        };


                        _main.ProductList = _product;

                        var _services = new NewRegistFOAServiceList()
                        {
                            ServiceName = resultlist.SERVICE_NAME != null ? resultlist.SERVICE_NAME : ""
                        };

                        List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                        var subStr = _services.ServiceName.Split(',');
                        foreach (var service in subStr)
                        {
                            _service.Add(new NewRegistFOAServiceList() { ServiceName = service });
                        }
                        _main.ServiceList = _service;


                        NewRegistForSubmitFOAResponse resultNewRegist = _queryProcessor.Execute(_main);
                        if (resultNewRegist.result != "") indexSuccess += 1;
                        else indexError += 1;

                        _logger.Info("Access No: " + _main.Access_No);
                    }
                }

                if (indexSuccess > 0 || indexError > 0)
                {
                    msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                    msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
                }
                else
                {
                    msg = "No data";
                }
                _logger.Info("FBBMonitorLmrBatch log finish msg : " + msg);
            }
            else
            {
                msg = "FBBMonitorLmrBatch install Program process : " + program_process.VAL1;
                _logger.Info("FBBMonitorLmrBatch install Program process : " + program_process.VAL1);
            }


            _logger.Info("logJobDone Date : " + chcekDate);
            StopWatching("End ResendPending_Ins");
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
                            command.FullUrl = "FBBMonitorLmrBatch";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBMonitorLmrBatch Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
