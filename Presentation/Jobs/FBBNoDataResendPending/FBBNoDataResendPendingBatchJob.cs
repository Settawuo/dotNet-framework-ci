using System;
using System.Collections.Generic;
using System.Linq;

namespace FBBNoDataResendPending
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.FBBWebConfigCommands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.ExWebServices;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.ExWebServiceModels;
    //using CompositionRoot;
    using WBBEntity.PanelModels.FBBWebConfigModels;

    public class FBBNoDataResendPendingBatchJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssConfigTblCommand> _intfLogCommand;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogFOACommand;
        private string _outErrorResult = string.Empty;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private string dateFrom = "";
        private string dateTo = "";
        private string dateFromINS = "";
        private string dateToINS = "";

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("FBBNoDataResendPendingBatchJob : " + _timer.Elapsed);
        }
        public FBBNoDataResendPendingBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
             ICommandHandler<UpdateFbssConfigTblCommand> intfLogCommand,
             ICommandHandler<UpdateFbssFOAConfigTblCommand> intfLogFOACommand,
              ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _intfLogFOACommand = intfLogFOACommand;
            _sendSmsCommand = SendSmsCommand;
        }

        public List<SubmitFOAEquipment> GetSubmitFOABatch()
        {
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_START").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_TO").FirstOrDefault();
                var model = new SubmitFOANoDataReportBatchQuery()
                {
                    companyCode = "ALL",
                    dateFrom = date_start.DISPLAY_VAL == "Y" ? date_start.VAL1.ToSafeString() : DateTime.Now.AddDays(-1).ToString("ddMMyyyy HHmmss"),
                    dateTo = date_to.DISPLAY_VAL == "Y" ? date_to.VAL1.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss"),
                    internetNo = "ALL",
                    materialCode = "ALL",
                    orderNo = "ALL",
                    orderType = "ALL",
                    plant = "ALL",
                    productName = "ALL",
                    serviceName = "",
                    status = "Pending",
                    storLocation = "ALL",
                    subcontractorCode = "ALL"
                };
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetConfig : " + ex.GetErrorMessage());
                SendSms();
                return new List<SubmitFOAEquipment>();
            }
        }
        public List<SubmitFOAInstallation> GetSubmitFOAInstallation()
        {
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_START").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_TO").FirstOrDefault();
                var model = new SubmitFOANoDataInsQuery()
                {
                    dateFrom = date_start.VAL3 == "Y" ? date_start.VAL4.ToSafeString() : DateTime.Now.AddDays(-1).ToString("ddMMyyyy HHmmss"),
                    dateTo = date_to.VAL3 == "Y" ? date_to.VAL4.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss"),
                    internetNo = "ALL",
                    orderNo = "ALL",
                    productName = "ALL",
                    serviceName = "",
                    status = "Pending"
                };
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                SendSms();
                return new List<SubmitFOAInstallation>();
            }
        }

        public List<SubmitFOAInstallationNew> GetSubmitFOAInstallationNew(string date_time_start_process)
        {
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_START_INS").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_TO_INS").FirstOrDefault();
                string new_date_from = date_time_start_process.ToSafeString() != "" ? date_time_start_process : DateTime.Now.AddDays(-2).ToString("ddMMyyyy HHmmss");
                var model = new SubmitFOANoDataInsNewQuery()
                {
                    dateFrom = date_start.DISPLAY_VAL == "Y" ? date_start.VAL1.ToSafeString() : new_date_from,
                    dateTo = date_to.DISPLAY_VAL == "Y" ? date_to.VAL1.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss"),
                    //internetNo = "ALL",
                    //orderNo = "ALL",
                    //productName = "ALL",
                    //serviceName = "",
                    //status = "Pending"
                };

                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                SendSms();
                return new List<SubmitFOAInstallationNew>();
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

        public string Get_FBB_CFG_LOV(string LOV_TYPE, string LOV_NAME)
        {
            try
            {
                var query = new GetLovQuery()
                {
                    LovType = LOV_TYPE,
                    LovName = LOV_NAME
                };
                var _FbbCfgLov = _queryProcessor.Execute(query);

                string PROCESS_INS = (from e in _FbbCfgLov select e.ActiveFlag).FirstOrDefault();

                return PROCESS_INS;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return "";
            }
        }

        public void ResendPending()
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            string msg = "";
            StartWatching();
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.DISPLAY_VAL == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;

                    List<SubmitFOAEquipment> resultSubmitFOAList = GetSubmitFOABatch();
                    //update date

                    var queryUpdateDate = new UpdateFbssConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START",
                        display_val = "Y",
                        val1 = checkDate,
                        val2 = msg,
                        flag = "EQUIP",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogCommand.Handle(queryUpdateDate);
                    _logger.Info("Logupdate Date: " + checkDate);
                    int i = 1;
                    string orderno = "", accessnum = "";
                    if (resultSubmitFOAList.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in resultSubmitFOAList)
                        {
                            var task = Task.Run(async delegate
                            {
                                try
                                {
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var resultProductList = new SubmitFOAResend();
                                    // 2.GetProductList
                                    var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                                    try
                                    {
                                        resultProductList = _queryProcessor.Execute(queryProductList);
                                    }
                                    catch (Exception Ex)
                                    {
                                        _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg: queryProductList :" + Ex.Message.ToString());
                                        throw Ex;
                                    }

                                    var _product = new List<NewRegistFOAProductList>();

                                    if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                                    {
                                        #region ProcessResendPending

                                        _main = new NewRegistForSubmitFOAQuery()
                                        {
                                            Access_No = resultProductList.AccessNo.ToSafeString(),
                                            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                                            SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                                            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                                            ProductName = resultProductList.ProductName.ToSafeString(),
                                            OrderType = resultProductList.OrderType.ToSafeString(),
                                            SubmitFlag = "BATCH_RESEND_NODATA",
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
                                            Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString()
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
                                        if (resultNewRegist.result == "") indexError += 1;
                                        else indexSuccess += 1;

                                        _logger.Info("Access No: " + _main.Access_No);

                                        #endregion ProcessResendPending
                                    }

                                }
                                catch (Exception Ex)
                                {
                                    string strMessage = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + strMessage);

                                    //----------------------------------------------------------------
                                    //Try for GetProductListQuery,Send To SAP
                                    //----------------------------------------------------------------
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            task.Wait();
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
                    _logger.Info("FBBNoDataResendPendingBatch Program process : " + program_process.DISPLAY_VAL);
                }

                StopWatching();
                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                string strMessage = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + strMessage);
                SendSms();
                StopWatching();

                //--------------------------------------------------------
                //Try for Fucntion Get_FBSS_CONFIG_TBL_LOV,GetSubmitFOABatch,UpdateFbssConfigTblCommand
                //--------------------------------------------------------
            }

        }

        private string StackTraceEx(Exception ex)
        {
            List<string> listMethod = new List<string>();
            var stackTrace = new StackTrace(ex, true);
            for (int i = 0; i < stackTrace.FrameCount; i++)
                listMethod.Add(stackTrace.GetFrame(i).GetMethod().DeclaringType.FullName.ToString() + "." + stackTrace.GetFrame(i).GetMethod().Name.ToString());

            return String.Join(",", listMethod) + ":" + ex.Message.ToString();
        }

        public void ResendPendingInstall()
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            StartWatching();
            string msg = "";
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.VAL1 == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;
                    var getInstall = GetSubmitFOAInstallation();
                    //update date

                    var queryUpdateDate = new UpdateFbssConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START",
                        display_val = "N",
                        val1 = checkDate,
                        val2 = msg,
                        flag = "INSTALL",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogCommand.Handle(queryUpdateDate);
                    _logger.Info("LogUpdate Date: " + checkDate);
                    if (getInstall.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in getInstall)
                        {

                            // 2.GetProductList
                            var t = Task.Run(async delegate
                            {

                                try
                                {
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var _product = new List<NewRegistFOAProductList>();

                                    _main = new NewRegistForSubmitFOAQuery()
                                    {
                                        Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                                        OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                                        OrderType = "RESEND_INS",
                                        SubmitFlag = "BATCH_RESEND_NODATA_INS",
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
                                catch (Exception Ex)
                                {
                                    var Message = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + Message);
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            t.Wait();


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
                    _logger.Info("ResendPendingInstall log finish msg : " + msg);
                }
                else
                {
                    _logger.Info("FBBNoDataResendPendingBatch ResendPendingInstall Program process : " + program_process.VAL1);
                }


                StopWatching();
                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                var Message = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + Message);
                SendSms();
                StopWatching();
            }

        }

        #region R2104
        public void ResendPendingNew()
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            string msg = "";
            StartWatching();
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.DISPLAY_VAL == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;

                    List<SubmitFOAEquipment> resultSubmitFOAList = GetSubmitFOABatch();
                    //update date

                    var queryUpdateDate = new UpdateFbssConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START",
                        display_val = "N",
                        val1 = checkDate,
                        val2 = msg,
                        flag = "EQUIP",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogCommand.Handle(queryUpdateDate);
                    _logger.Info("Logupdate Date: " + checkDate);
                    int i = 1;
                    string orderno = "", accessnum = "";
                    if (resultSubmitFOAList.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in resultSubmitFOAList)
                        {
                            var task = Task.Run(async delegate
                            {
                                try
                                {
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var resultProductList = new SubmitFOAResend();
                                    // 2.GetProductList
                                    var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                                    try
                                    {
                                        resultProductList = _queryProcessor.Execute(queryProductList);
                                    }
                                    catch (Exception Ex)
                                    {
                                        _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg: queryProductList :" + Ex.Message.ToString());
                                        throw Ex;
                                    }

                                    var _product = new List<NewRegistFOAProductList>();

                                    if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                                    {
                                        #region ProcessResendPending

                                        _main = new NewRegistForSubmitFOAQuery()
                                        {
                                            Access_No = resultProductList.AccessNo.ToSafeString(),
                                            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                                            SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                                            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                                            ProductName = resultProductList.ProductName.ToSafeString(),
                                            OrderType = resultProductList.OrderType.ToSafeString(),
                                            SubmitFlag = "BATCH_RESEND_NODATA",
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
                                            Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString()
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
                                        if (resultNewRegist.result == "") indexError += 1;
                                        else indexSuccess += 1;

                                        _logger.Info("Access No: " + _main.Access_No);

                                        #endregion ProcessResendPending
                                    }

                                }
                                catch (Exception Ex)
                                {
                                    string strMessage = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + strMessage);

                                    //----------------------------------------------------------------
                                    //Try for GetProductListQuery,Send To SAP
                                    //----------------------------------------------------------------
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            task.Wait();
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
                    _logger.Info("FBBNoDataResendPendingBatch Program process : " + program_process.DISPLAY_VAL);
                }

                StopWatching();
                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                string strMessage = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + strMessage);
                SendSms();
                StopWatching();

                //--------------------------------------------------------
                //Try for Fucntion Get_FBSS_CONFIG_TBL_LOV,GetSubmitFOABatch,UpdateFbssConfigTblCommand
                //--------------------------------------------------------
            }

        }

        public void ResendPendingInstallNew(string date_time_start_process)
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            StartWatching();
            string msg = "";
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS_INS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.DISPLAY_VAL == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;
                    var getInstall = GetSubmitFOAInstallationNew(date_time_start_process);

                    //Update Date_Start_Ins
                    var queryUpdateDateStart = new UpdateFbssConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START_INS",
                        display_val = "N",
                        val1 = DateTime.Now.ToString("ddMMyyyy HHmmss"),
                        flag = "EQUIP_INS",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogCommand.Handle(queryUpdateDateStart);

                    _logger.Info("LogUpdate Date: " + checkDate);
                    if (getInstall.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in getInstall)
                        {

                            // 2.GetProductList
                            var t = Task.Run(async delegate
                            {

                                try
                                {
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var _product = new List<NewRegistFOAProductList>();

                                    _main = new NewRegistForSubmitFOAQuery()
                                    {
                                        #region New _main
                                        Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                                        OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                                        SubcontractorCode = resultlist.SUBCONTRACT_CODE.ToSafeString(),
                                        SubcontractorName = resultlist.SUBCONTRACT_NAME.ToSafeString(),
                                        ProductName = resultlist.PRODUCT_NAME.ToSafeString(),
                                        OrderType = resultlist.ORDER_TYPE.ToSafeString(),
                                        SubmitFlag = "RESEND_NODATA_INS",
                                        RejectReason = resultlist.REJECT_REASON.ToSafeString(),
                                        FOA_Submit_date = resultlist.SUBMIT_DATE.ToSafeString(),
                                        OLT_NAME = resultlist.OLT_NAME.ToSafeString(),
                                        BUILDING_NAME = resultlist.BUILDING_NAME.ToSafeString(),
                                        Mobile_Contact = resultlist.MOBILE_CONTACT.ToSafeString(),

                                        Post_Date = DateTime.Now.ToString("dd/MM/yyyy"),
                                        Address_ID = resultlist.ADDRESS_ID.ToSafeString(),
                                        ORG_ID = resultlist.ORG_ID.ToSafeString(),
                                        Reuse_Flag = resultlist.REUSE_FLAG.ToSafeString(),
                                        Event_Flow_Flag = resultlist.EVENT_FLOW_FLAG.ToSafeString(),

                                        Subcontract_Type = resultlist.SUBCONTRACT_TYPE.ToSafeString(),
                                        Subcontract_Sub_Type = resultlist.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                                        Request_Sub_Flag = resultlist.REQUEST_SUB_FLAG.ToSafeString(),
                                        Sub_Access_Mode = resultlist.SUB_ACCESS_MODE.ToSafeString()
                                        #endregion

                                        #region Old _main
                                        //Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                                        //OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                                        //OrderType = "RESEND_INS",
                                        //SubmitFlag = "BATCH_RESEND_NODATA_INS",
                                        #endregion
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
                                catch (Exception Ex)
                                {
                                    var Message = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + Message);
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            t.Wait();


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
                    _logger.Info("ResendPendingInstall log finish msg : " + msg);
                }
                else
                {
                    _logger.Info("FBBNoDataResendPendingBatch ResendPendingInstall Program process : " + program_process.DISPLAY_VAL);
                }


                StopWatching();
                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                var Message = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + Message);
                SendSms();
                StopWatching();
            }

        }

        #endregion

        #region R2106
        public void ResendPendingNewR2106()
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            string msg = "";
            StartWatching();
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.DISPLAY_VAL == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;

                    List<SubmitFOAEquipment> resultSubmitFOAList = GetSubmitFOABatchR2106();

                    //update date
                    var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START",
                        display_val = "Y",
                        val1 = dateFrom,
                        val2 = dateTo,
                        flag = "EQUIP_PENDING",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogFOACommand.Handle(queryUpdateDate);
                    _logger.Info("Logupdate Date: " + checkDate);
                    int i = 1;
                    string orderno = "", accessnum = "";
                    if (resultSubmitFOAList.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in resultSubmitFOAList)
                        {
                            var task = Task.Run(async delegate
                            {
                                try
                                {
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var resultProductList = new SubmitFOAResend();
                                    // 2.GetProductList
                                    var queryProductList = new GetProductListQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER };
                                    try
                                    {
                                        resultProductList = _queryProcessor.Execute(queryProductList);
                                    }
                                    catch (Exception Ex)
                                    {
                                        _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg: queryProductList :" + Ex.Message.ToString());
                                        throw Ex;
                                    }

                                    var _product = new List<NewRegistFOAProductList>();

                                    if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                                    {
                                        #region ProcessResendPending

                                        _main = new NewRegistForSubmitFOAQuery()
                                        {
                                            Access_No = resultProductList.AccessNo.ToSafeString(),
                                            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                                            SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                                            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                                            ProductName = resultProductList.ProductName.ToSafeString(),
                                            OrderType = resultProductList.OrderType.ToSafeString(),
                                            SubmitFlag = "BATCH_RESEND_NODATA",
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
                                            Sub_Access_Mode = resultProductList.SUB_ACCESS_MODE.ToSafeString()
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
                                        if (resultNewRegist.result == "") indexError += 1;
                                        else indexSuccess += 1;

                                        _logger.Info("Access No: " + _main.Access_No);

                                        #endregion ProcessResendPending
                                    }

                                }
                                catch (Exception Ex)
                                {
                                    string strMessage = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + strMessage);

                                    //----------------------------------------------------------------
                                    //Try for GetProductListQuery,Send To SAP
                                    //----------------------------------------------------------------
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            task.Wait();
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
                    _logger.Info("FBBNoDataResendPendingBatch Program process : " + program_process.DISPLAY_VAL);
                }

                StopWatching();
                //update dateEnd
                var queryUpdateDateEnd = new UpdateFbssFOAConfigTblCommand()
                {
                    con_type = "FBB_NODATARESENDPENDING_BATCH",
                    con_name = "DATE_START",
                    display_val = "N",
                    val1 = dateFrom,
                    val2 = dateTo,
                    flag = "EQUIP_PENDING",
                    updated_by = "FBBNoDataResendPendingBatchJob"
                };
                _intfLogFOACommand.Handle(queryUpdateDateEnd);

                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                string strMessage = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + strMessage);
                SendSms();
                StopWatching();

                //--------------------------------------------------------
                //Try for Fucntion Get_FBSS_CONFIG_TBL_LOV,GetSubmitFOABatch,UpdateFbssConfigTblCommand
                //--------------------------------------------------------
            }

        }

        public void ResendPendingInstallNewR2106(string date_time_start_process)
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            StartWatching();
            string msg = "";
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS_INS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.DISPLAY_VAL == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;
                    var getInstall = GetSubmitFOAInstallationNewR2106(date_time_start_process);

                    //Update Date_Start_Ins
                    var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START_INS",
                        display_val = "Y",
                        val1 = dateFromINS,
                        val2 = dateToINS,
                        flag = "INSTALL_PENDING",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogFOACommand.Handle(queryUpdateDate);

                    _logger.Info("LogUpdate Date: " + checkDate);
                    if (getInstall.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in getInstall)
                        {

                            // 2.GetProductList
                            var t = Task.Run(async delegate
                            {

                                try
                                {
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var _product = new List<NewRegistFOAProductList>();

                                    _main = new NewRegistForSubmitFOAQuery()
                                    {
                                        #region New _main
                                        Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                                        OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                                        SubcontractorCode = resultlist.SUBCONTRACT_CODE.ToSafeString(),
                                        SubcontractorName = resultlist.SUBCONTRACT_NAME.ToSafeString(),
                                        ProductName = resultlist.PRODUCT_NAME.ToSafeString(),
                                        OrderType = resultlist.ORDER_TYPE.ToSafeString(),
                                        SubmitFlag = "RESEND_NODATA_INS",
                                        RejectReason = resultlist.REJECT_REASON.ToSafeString(),
                                        FOA_Submit_date = resultlist.SUBMIT_DATE.ToSafeString(),
                                        OLT_NAME = resultlist.OLT_NAME.ToSafeString(),
                                        BUILDING_NAME = resultlist.BUILDING_NAME.ToSafeString(),
                                        Mobile_Contact = resultlist.MOBILE_CONTACT.ToSafeString(),

                                        Post_Date = DateTime.Now.ToString("dd/MM/yyyy"),
                                        Address_ID = resultlist.ADDRESS_ID.ToSafeString(),
                                        ORG_ID = resultlist.ORG_ID.ToSafeString(),
                                        Reuse_Flag = resultlist.REUSE_FLAG.ToSafeString(),
                                        Event_Flow_Flag = resultlist.EVENT_FLOW_FLAG.ToSafeString(),

                                        Subcontract_Type = resultlist.SUBCONTRACT_TYPE.ToSafeString(),
                                        Subcontract_Sub_Type = resultlist.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                                        Request_Sub_Flag = resultlist.REQUEST_SUB_FLAG.ToSafeString(),
                                        Sub_Access_Mode = resultlist.SUB_ACCESS_MODE.ToSafeString()
                                        #endregion

                                        #region Old _main
                                        //Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                                        //OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                                        //OrderType = "RESEND_INS",
                                        //SubmitFlag = "BATCH_RESEND_NODATA_INS",
                                        #endregion
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
                                catch (Exception Ex)
                                {
                                    var Message = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + Message);
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            t.Wait();


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

                    //Update Date_Start_Ins
                    var queryUpdateDateEnd = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START_INS",
                        display_val = "N",
                        val1 = dateFromINS,
                        val2 = dateToINS,
                        flag = "INSTALL_PENDING",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogFOACommand.Handle(queryUpdateDateEnd);

                    _logger.Info("ResendPendingInstall log finish msg : " + msg);
                }
                else
                {
                    _logger.Info("FBBNoDataResendPendingBatch ResendPendingInstall Program process : " + program_process.DISPLAY_VAL);
                }


                StopWatching();
                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                var Message = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + Message);
                SendSms();
                StopWatching();
            }

        }

        public List<SubmitFOAEquipment> GetSubmitFOABatchR2106()
        {
            int Check_DateDiv = 0;
            dateFrom = "";
            dateTo = "";
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_START").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_TO").FirstOrDefault();
                var date_div = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_DIV").FirstOrDefault();
                Check_DateDiv = date_div.DISPLAY_VAL.ToSafeString() == "Y" ? date_div.VAL1.ToSafeInteger() + 1 : 1;
                var model = new SubmitFOANoDataReportBatchQuery()
                {
                    companyCode = "ALL",
                    dateFrom = date_start.DISPLAY_VAL == "Y" ? date_start.VAL1.ToSafeString() : DateTime.Now.AddDays(-Check_DateDiv).ToString("ddMMyyyy HHmmss"),
                    dateTo = date_to.DISPLAY_VAL == "Y" ? date_to.VAL1.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss"),
                    internetNo = "ALL",
                    materialCode = "ALL",
                    orderNo = "ALL",
                    orderType = "ALL",
                    plant = "ALL",
                    productName = "ALL",
                    serviceName = "",
                    status = "Pending",
                    storLocation = "ALL",
                    subcontractorCode = "ALL"
                };
                dateFrom = model.dateFrom;
                dateTo = model.dateTo;
                _logger.Info("LogUpdate date_start.DISPLAY_VAL: " + date_start.DISPLAY_VAL + " dateFrom " + dateFrom);
                _logger.Info("LogUpdate date_to.DISPLAY_VAL: " + date_to.DISPLAY_VAL + " dateTo " + dateTo);
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetConfig : " + ex.GetErrorMessage());
                SendSms();
                return new List<SubmitFOAEquipment>();
            }
        }

        public List<SubmitFOAInstallationNew> GetSubmitFOAInstallationNewR2106(string date_time_start_process)
        {
            int Check_DateDiv = 0;
            dateFromINS = "";
            dateToINS = "";
            try
            {
                var date_start = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_START_INS").FirstOrDefault();
                var date_to = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_TO_INS").FirstOrDefault();
                string new_date_from = date_time_start_process.ToSafeString() != "" ? date_time_start_process : DateTime.Now.AddDays(-2).ToString("ddMMyyyy HHmmss");
                var model = new SubmitFOANoDataInsNewQuery()
                {
                    dateFrom = date_start.DISPLAY_VAL == "Y" ? date_start.VAL1.ToSafeString() : new_date_from,
                    dateTo = date_to.DISPLAY_VAL == "Y" ? date_to.VAL1.ToSafeString() : DateTime.Now.ToString("ddMMyyyy HHmmss"),
                    //internetNo = "ALL",
                    //orderNo = "ALL",
                    //productName = "ALL",
                    //serviceName = "",
                    //status = "Pending"
                };
                dateFromINS = model.dateFrom;
                dateToINS = model.dateTo;

                _logger.Info("LogUpdate date_start.DISPLAY_VAL: " + date_start.DISPLAY_VAL + " dateFromINS " + dateFromINS);
                _logger.Info("LogUpdate date_to.DISPLAY_VAL: " + date_to.DISPLAY_VAL + " dateToINS " + dateToINS);
                return _queryProcessor.Execute(model);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                SendSms();
                return new List<SubmitFOAInstallationNew>();
            }
        }
        #endregion

        #region R2111
        public void ResendPendingInstallNewR2111(string date_time_start_process)
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            StartWatching();
            string msg = "";
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS_INS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.DISPLAY_VAL == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;
                    var getInstall = GetSubmitFOAInstallationNewR2106(date_time_start_process);

                    //Update Date_Start_Ins
                    var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START_INS",
                        display_val = "Y",
                        val1 = dateFromINS,
                        val2 = dateToINS,
                        flag = "INSTALL_PENDING",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogFOACommand.Handle(queryUpdateDate);

                    _logger.Info("LogUpdate Date: " + checkDate);
                    if (getInstall.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in getInstall)
                        {

                            // 2.GetProductList
                            var t = Task.Run(async delegate
                            {

                                try
                                {
                                    SubmitFOAResend resultProductList = new SubmitFOAResend();
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var _product = new List<NewRegistFOAProductList>();
                                    var queryProductList = new GetProductListByPackageQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER, flag_auto_resend = null };
                                    try
                                    {
                                        resultProductList = _queryProcessor.Execute(queryProductList);
                                    }
                                    catch (Exception Ex)
                                    {
                                        _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg: queryProductList :" + Ex.Message.ToString());
                                        throw Ex;
                                    }

                                    _main = new NewRegistForSubmitFOAQuery()
                                    {
                                        #region New _main
                                        Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                                        OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                                        SubcontractorCode = resultlist.SUBCONTRACT_CODE.ToSafeString(),
                                        SubcontractorName = resultlist.SUBCONTRACT_NAME.ToSafeString(),
                                        ProductName = resultlist.PRODUCT_NAME.ToSafeString(),
                                        OrderType = resultlist.ORDER_TYPE.ToSafeString(),
                                        SubmitFlag = "RESEND_NODATA_INS",
                                        RejectReason = resultlist.REJECT_REASON.ToSafeString(),
                                        FOA_Submit_date = resultlist.SUBMIT_DATE.ToSafeString(),
                                        OLT_NAME = resultlist.OLT_NAME.ToSafeString(),
                                        BUILDING_NAME = resultlist.BUILDING_NAME.ToSafeString(),
                                        Mobile_Contact = resultlist.MOBILE_CONTACT.ToSafeString(),

                                        Post_Date = DateTime.Now.ToString("dd/MM/yyyy"),
                                        Address_ID = resultlist.ADDRESS_ID.ToSafeString(),
                                        ORG_ID = resultlist.ORG_ID.ToSafeString(),
                                        Reuse_Flag = resultlist.REUSE_FLAG.ToSafeString(),
                                        Event_Flow_Flag = resultlist.EVENT_FLOW_FLAG.ToSafeString(),

                                        Subcontract_Type = resultlist.SUBCONTRACT_TYPE.ToSafeString(),
                                        Subcontract_Sub_Type = resultlist.SUBCONTRACT_SUB_TYPE.ToSafeString(),
                                        Request_Sub_Flag = resultlist.REQUEST_SUB_FLAG.ToSafeString(),
                                        Sub_Access_Mode = resultlist.SUB_ACCESS_MODE.ToSafeString(),
                                        Product_Owner = resultlist.PRODUCT_OWNER.ToSafeString(),
                                        Main_Promo_Code = resultlist.MAIN_PROMO_CODE.ToSafeString(),
                                        Team_ID = resultlist.TEAM_ID.ToSafeString()
                                        #endregion

                                        #region Old _main
                                        //Access_No = resultlist.ACCESS_NUMBER.ToSafeString(),
                                        //OrderNumber = resultlist.ORDER_NO.ToSafeString(),
                                        //OrderType = "RESEND_INS",
                                        //SubmitFlag = "BATCH_RESEND_NODATA_INS",
                                        #endregion
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
                                catch (Exception Ex)
                                {
                                    var Message = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + Message);
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            t.Wait();


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

                    //Update Date_Start_Ins
                    var queryUpdateDateEnd = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START_INS",
                        display_val = "N",
                        val1 = dateFromINS,
                        val2 = dateToINS,
                        flag = "INSTALL_PENDING",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogFOACommand.Handle(queryUpdateDateEnd);

                    _logger.Info("ResendPendingInstall log finish msg : " + msg);
                }
                else
                {
                    _logger.Info("FBBNoDataResendPendingBatch ResendPendingInstall Program process : " + program_process.DISPLAY_VAL);
                }


                StopWatching();
                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                var Message = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + Message);
                SendSms();
                StopWatching();
            }
        }

        public void ResendPendingNewR2111()
        {
            string checkDate = DateTime.Now.ToString("ddMMyyyy HHmmss");
            string msg = "";
            StartWatching();
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                var lovTime = Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "TIME_CONFIG").FirstOrDefault();
                double span = 1;
                if (lovTime != null)
                {
                    span = lovTime.DISPLAY_VAL.ToSafeDouble();
                }
                if (program_process.DISPLAY_VAL == "Y")
                {

                    int indexSuccess = 0;
                    int indexError = 0;

                    List<SubmitFOAEquipment> resultSubmitFOAList = GetSubmitFOABatchR2106();

                    //update date
                    var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBB_NODATARESENDPENDING_BATCH",
                        con_name = "DATE_START",
                        display_val = "Y",
                        val1 = dateFrom,
                        val2 = dateTo,
                        flag = "EQUIP_PENDING",
                        updated_by = "FBBNoDataResendPendingBatchJob"
                    };
                    _intfLogFOACommand.Handle(queryUpdateDate);
                    _logger.Info("Logupdate Date: " + checkDate);
                    int i = 1;
                    string orderno = "", accessnum = "";
                    if (resultSubmitFOAList.Any())
                    {
                        var _mainList = new List<NewRegistForSubmitFOAQuery>();
                        foreach (var resultlist in resultSubmitFOAList)
                        {
                            var task = Task.Run(async delegate
                            {
                                try
                                {
                                    var _main = new NewRegistForSubmitFOAQuery();
                                    var resultProductList = new SubmitFOAResend();
                                    // 2.GetProductList
                                    var queryProductList = new GetProductListByPackageQuery() { OrderNo = resultlist.ORDER_NO, AccessNo = resultlist.ACCESS_NUMBER, flag_auto_resend = null };
                                    try
                                    {
                                        resultProductList = _queryProcessor.Execute(queryProductList);
                                    }
                                    catch (Exception Ex)
                                    {
                                        _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg: queryProductList :" + Ex.Message.ToString());
                                        throw Ex;
                                    }

                                    var _product = new List<NewRegistFOAProductList>();

                                    if (orderno != resultlist.ORDER_NO && accessnum != resultlist.ACCESS_NUMBER)
                                    {
                                        #region ProcessResendPending

                                        _main = new NewRegistForSubmitFOAQuery()
                                        {
                                            Access_No = resultProductList.AccessNo.ToSafeString(),
                                            OrderNumber = resultProductList.OrderNumber.ToSafeString(),
                                            SubcontractorCode = resultProductList.SubcontractorCode.ToSafeString(),
                                            SubcontractorName = resultProductList.SubcontractorName.ToSafeString(),
                                            ProductName = resultProductList.ProductName.ToSafeString(),
                                            OrderType = resultProductList.OrderType.ToSafeString(),
                                            SubmitFlag = "BATCH_RESEND_NODATA",
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
                                        if (resultNewRegist.result == "") indexError += 1;
                                        else indexSuccess += 1;

                                        _logger.Info("Access No: " + _main.Access_No);

                                        #endregion ProcessResendPending
                                    }

                                }
                                catch (Exception Ex)
                                {
                                    string strMessage = StackTraceEx(Ex);
                                    _logger.Info("Access No: " + resultlist.ACCESS_NUMBER + "||Msg:" + strMessage);

                                    //----------------------------------------------------------------
                                    //Try for GetProductListQuery,Send To SAP
                                    //----------------------------------------------------------------
                                }
                                await Task.Delay(TimeSpan.FromSeconds(span));
                            });
                            task.Wait();
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
                    _logger.Info("FBBNoDataResendPendingBatch Program process : " + program_process.DISPLAY_VAL);
                }

                StopWatching();
                //update dateEnd
                var queryUpdateDateEnd = new UpdateFbssFOAConfigTblCommand()
                {
                    con_type = "FBB_NODATARESENDPENDING_BATCH",
                    con_name = "DATE_START",
                    display_val = "N",
                    val1 = dateFrom,
                    val2 = dateTo,
                    flag = "EQUIP_PENDING",
                    updated_by = "FBBNoDataResendPendingBatchJob"
                };
                _intfLogFOACommand.Handle(queryUpdateDateEnd);

                _logger.Info("LogJobDone Date: " + checkDate);
            }
            catch (Exception ex)
            {
                string strMessage = StackTraceEx(ex);
                _logger.Error("FBBNoDataResendPendingBatch Error : " + strMessage);
                SendSms();
                StopWatching();

                //--------------------------------------------------------
                //Try for Fucntion Get_FBSS_CONFIG_TBL_LOV,GetSubmitFOABatch,UpdateFbssConfigTblCommand
                //--------------------------------------------------------
            }
        }

        #endregion

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
                            command.FullUrl = "FBBNoDataResendPendingBatch";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBNoDataResendPendingBatch Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }
        }

    }
}
