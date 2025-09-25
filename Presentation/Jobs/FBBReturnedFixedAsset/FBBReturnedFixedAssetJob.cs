using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBReturnedFixedAsset
{
    class FBBReturnedFixedAssetJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private int _countError = 0;
        private int _countSuccess = 0;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;

        public FBBReturnedFixedAssetJob(ILogger logger
            , IQueryProcessor queryProcessor,
             ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
        }

        public void ReturnedFixedAsset()
        {
            _logger.Info("Start ReturnedFixedAsset");
            StartWatching();

            try
            {
                var returnedCpe = GetReturnedOrders();
                var lovReturned = GetLovList("FBB_RETURNED", "FBB_RETURNED_PATH").FirstOrDefault();
                double span = 1;
                if (lovReturned != null)
                {
                    span = lovReturned.LovValue2.ToSafeDouble();
                }

                var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();
                _logger.Info("Flag_RollbackSAP : " + resultFixAssConfig.DISPLAY_VAL);

                if (resultFixAssConfig.DISPLAY_VAL == "Y")
                {
                    foreach (var item in returnedCpe.cur)
                    {
                        var t = Task.Run(async delegate
                        {
                            CallSAPWebService(item);
                            await Task.Delay(TimeSpan.FromSeconds(span));
                        });
                        t.Wait();
                    }

                }
                else
                {
                    foreach (var item in returnedCpe.cur)
                    {
                        var t = Task.Run(async delegate
                        {
                            CallSAPWebServiceS4(item);
                            await Task.Delay(TimeSpan.FromSeconds(span));
                        });
                        t.Wait();
                    }
                }

                if (_countError > 0)
                    SendSms($"Batch FBBReturnedFixedAssetJob DONE. Success {_countSuccess.ToString()} Error {_countError.ToString()}");

                _logger.Debug($"Batch FBBReturnedFixedAssetJob DONE. Success {_countSuccess.ToString()} Error {_countError.ToString()}");

            }
            catch (Exception e)
            {
                _logger.Error("Error " + e.Message);
                SendSms("FBBReturnedFixedAsset Error");
            }
            finally
            {
                StopWatching("End ReturnedFixedAsset");
            }
        }
        public List<LovModel> GET_FBSS_FIXED_ASSET_CONFIG(string product_name)
        {
            var query = new GetFixedAssetConfigQuery()
            {
                Program = product_name
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
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
                SendSms("FBBReturnedFixedAsset Error");
                _logger.Error("Error GetLovList : " + ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        private void CallSAPWebService(FBBReturnedCpeModel item)
        {
            _logger.Debug(String.Format("Returned Access Number: {0}", item.ACCESS_NUMBER));

            var SERVICE_LIST = new List<NewRegistFOAServiceList>();
            SERVICE_LIST.Add(new NewRegistFOAServiceList { ServiceName = item.SERVICE_LIST });

            string msg = "";

            NewRegistForSubmitFOAResponse result = null;
            try
            {
                var query = new NewRegistForSubmitFOAQuery
                {
                    Access_No = item.ACCESS_NUMBER,
                    BUILDING_NAME = item.BUILDING_NAME,
                    FOA_Submit_date = item.FOA_SUBMIT_DATE == null ? "" : item.FOA_SUBMIT_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("en-US")),
                    Mobile_Contact = item.MOBILE_CONTACT,
                    OLT_NAME = item.OLT_NAME,
                    OrderNumber = item.ORDER_NO,
                    OrderType = item.ORDER_TYPE,
                    ProductList = XDocument.Parse(item.PRODUCT_LIST).Descendants("PRODUCT").Select(node =>
                        new NewRegistFOAProductList
                        {
                            SerialNumber = node.Element("SN") == null ? "" : node.Element("SN").Value,
                            MaterialCode = node.Element("MATERIAL_CODE") == null ? "" : node.Element("MATERIAL_CODE").Value,
                            CompanyCode = node.Element("COMPANY_CODE") == null ? "" : node.Element("COMPANY_CODE").Value,
                            Plant = node.Element("PLANT") == null ? "" : node.Element("PLANT").Value,
                            StorageLocation = node.Element("STORAGE_LOCATION") == null ? "" : node.Element("STORAGE_LOCATION").Value,
                            SNPattern = node.Element("SN_PATTERN") == null ? "" : node.Element("SN_PATTERN").Value,
                            MovementType = node.Element("MOVEMENT_TYPE") == null ? "" : "Returned (Old SN)"
                        }).ToList(),
                    ProductName = item.PRODUCT_NAME,
                    RejectReason = item.REJECT_REASON,
                    ServiceList = SERVICE_LIST,
                    SubcontractorCode = item.SUBCONTRACT_CODE,
                    SubcontractorName = item.SUBCONTRACT_NAME,
                    SubmitFlag = item.SUBMIT_FLAG,
                    Post_Date = item.POST_DATE,
                    Address_ID = item.ADDESS_ID,
                    ORG_ID = item.ORG_ID,
                    Reuse_Flag = item.REUSE_FLAG,
                    Event_Flow_Flag = item.EVENT_FLOW_FLAG,
                    UserName = "FBBReturnedFixedAssetJob",
                    Subcontract_Type = item.SUBCONTRACT_TYPE,
                    Subcontract_Sub_Type = item.SUBCONTRACT_SUB_TYPE,
                    Request_Sub_Flag = item.REQUEST_SUB_FLAG,
                    Sub_Access_Mode = item.SUB_ACCESS_MODE,
                    Product_Owner = item.PRODUCT_OWNER,
                    Main_Promo_Code = item.MAIN_PROMO_CODE,
                    Team_ID = item.TEAM_ID,
                    ReturnMessage = ""
                };

                result = _queryProcessor.Execute(query);

                if (result.result.Contains("-1"))
                    _countError += 1;


                msg = String.Format("SAP Access Number: {0}|{1}"
               , item.ACCESS_NUMBER
               , result.result == "-1" ? "ERROR" : result.errorReason).TrimEnd('|');
                _logger.Debug(msg);

                _countSuccess += 1;
            }
            catch (Exception ex)
            {
                _countError += 1;
                msg = String.Format("ERROR Access Number: {0}|{1}"
                , item.ACCESS_NUMBER
                , ex.Message.ToString());
                _logger.Debug(msg);
            }
        }

        private void CallSAPWebServiceS4(FBBReturnedCpeModel item)
        {
            _logger.Debug(String.Format("Returned Access Number: {0}", item.ACCESS_NUMBER));

            var SERVICE_LIST = new List<NewRegistFOAServiceList>();
            SERVICE_LIST.Add(new NewRegistFOAServiceList { ServiceName = item.SERVICE_LIST });

            string msg = "";

            NewRegistForSubmitFOAS4HANAResponse result = null;
            try
            {
                var query = new NewRegistForSubmitFOA4HANAQuery
                {
                    Access_No = item.ACCESS_NUMBER,
                    BUILDING_NAME = item.BUILDING_NAME,
                    FOA_Submit_date = item.FOA_SUBMIT_DATE == null ? "" : item.FOA_SUBMIT_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("en-US")),
                    Mobile_Contact = item.MOBILE_CONTACT,
                    OLT_NAME = item.OLT_NAME,
                    OrderNumber = item.ORDER_NO,
                    OrderType = item.ORDER_TYPE,
                    ProductList = XDocument.Parse(item.PRODUCT_LIST).Descendants("PRODUCT").Select(node =>
                        new NewRegistFOAProductList
                        {
                            SerialNumber = node.Element("SN") == null ? "" : node.Element("SN").Value,
                            MaterialCode = node.Element("MATERIAL_CODE") == null ? "" : node.Element("MATERIAL_CODE").Value,
                            CompanyCode = node.Element("COMPANY_CODE") == null ? "" : node.Element("COMPANY_CODE").Value,
                            Plant = node.Element("PLANT") == null ? "" : node.Element("PLANT").Value,
                            StorageLocation = node.Element("STORAGE_LOCATION") == null ? "" : node.Element("STORAGE_LOCATION").Value,
                            SNPattern = node.Element("SN_PATTERN") == null ? "" : node.Element("SN_PATTERN").Value,
                            MovementType = node.Element("MOVEMENT_TYPE") == null ? "" : "Returned (Old SN)"
                        }).ToList(),
                    ProductName = item.PRODUCT_NAME,
                    RejectReason = item.REJECT_REASON,
                    ServiceList = SERVICE_LIST,
                    SubcontractorCode = item.SUBCONTRACT_CODE,
                    SubcontractorName = item.SUBCONTRACT_NAME,
                    SubmitFlag = item.SUBMIT_FLAG,
                    Post_Date = item.POST_DATE,
                    Address_ID = item.ADDESS_ID,
                    ORG_ID = item.ORG_ID,
                    Reuse_Flag = item.REUSE_FLAG,
                    Event_Flow_Flag = item.EVENT_FLOW_FLAG,
                    UserName = "FBBReturnedFixedAssetJob",
                    Subcontract_Type = item.SUBCONTRACT_TYPE,
                    Subcontract_Sub_Type = item.SUBCONTRACT_SUB_TYPE,
                    Request_Sub_Flag = item.REQUEST_SUB_FLAG,
                    Sub_Access_Mode = item.SUB_ACCESS_MODE,
                    Product_Owner = item.PRODUCT_OWNER,
                    Main_Promo_Code = item.MAIN_PROMO_CODE,
                    Team_ID = item.TEAM_ID,
                    ReturnMessage = ""
                };

                result = _queryProcessor.Execute(query);

                if (result.result.Contains("-1"))
                    _countError += 1;


                msg = String.Format("SAP Access Number: {0}|{1}"
               , item.ACCESS_NUMBER
               , result.result == "-1" ? "ERROR" : result.errorReason).TrimEnd('|');
                _logger.Debug(msg);

                _countSuccess += 1;
            }
            catch (Exception ex)
            {
                _countError += 1;
                msg = String.Format("ERROR Access Number: {0}|{1}"
                , item.ACCESS_NUMBER
                , ex.Message.ToString());
                _logger.Debug(msg);
            }
        }

        private FBBReturnedFixedAssetModel GetReturnedOrders()
        {
            var result = _queryProcessor.Execute(new FBBReturnedFixedAssetQuery());

            result.cur.RemoveAll(item => item.ORDER_TYPE != "RETURNED");
            if (result.ret_code == "0")
            {
                _logger.Info(result.cur.Count == 0
                    ? "No returned order."
                    : String.Format("Send {0} order(s) to returned process", result.cur.Count));
            }
            else
            {
                _logger.Error(result.ret_code);
            }

            return result;
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
        public void SendSms(string msg)
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
                            command.FullUrl = "FBBReturnedFixedAsset";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = msg;
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
