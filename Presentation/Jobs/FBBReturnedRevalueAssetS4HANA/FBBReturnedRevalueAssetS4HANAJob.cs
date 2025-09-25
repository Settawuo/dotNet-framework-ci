using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace FBBReturnedRevalueAssetS4HANA
{
    public class FBBReturnedRevalueAssetS4HANAJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FBBPAYGRevalueAssetCommand> _BatvhRevalue;
        //private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        //public FBBPAYGRevalueAssetBatch(ILogger logger,
        //    IQueryProcessor queryProcessor,
        //    ICommandHandler<FBBPAYGRevalueAssetBatchCommand> batvhPurge,
        //    ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        //{
        public FBBReturnedRevalueAssetS4HANAJob(ILogger logger,
        IQueryProcessor queryProcessor,
        ICommandHandler<FBBPAYGRevalueAssetCommand> batvhRevalue)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _BatvhRevalue = batvhRevalue;
            //_sendMail = sendMail;
        }
        public async Task ProcessBatch()
        {
            //bool result = false;
            try
            {

                var PROGRAM_PROCESS = Get_FBSS_CONFIG_TBL_LOV("FBB_RETURNEDREVALUE_S4", "PROGRAM_PROCESS").FirstOrDefault();
                //var DATE_START = Get_FBSS_CONFIG_TBL_LOV("FBB_RETURNEDREVALUE_S4", "DATE_START").FirstOrDefault();
                //var DATE_TO = Get_FBSS_CONFIG_TBL_LOV("FBB_RETURNEDREVALUE_S4", "DATE_TO").FirstOrDefault();
                //var DATE_DIV = Get_FBSS_CONFIG_TBL_LOV("FBB_RETURNEDREVALUE_S4", "DATE_DIV").FirstOrDefault();
                //string p_date_start = string.Empty;

                //if (DATE_START.DISPLAY_VAL == "Y")
                //{
                //    p_date_start = DATE_START.VAL1;
                //}
                //else
                //{
                //    if(DATE_DIV.DISPLAY_VAL == "Y")
                //    {
                //        if (int.TryParse(DATE_DIV.VAL1, out int val1DateDiv))
                //        {
                //            p_date_start = DateTime.Now.AddDays(val1DateDiv - 1).ToString();
                //            //p_date_start = DateTime.Now.AddDays(val1DateDiv - 1).ToString("yyyy-MM-dd");
                //        }
                //        else
                //        {
                //            p_date_start = DateTime.Now.AddDays(-(1 + 1)).ToString();
                //            //p_date_start = DateTime.Now.AddDays(-(1 + 1)).ToString("yyyy-MM-dd");
                //        }
                //    }
                //}

                var p_date_start = string.Empty;
                var p_date_to = string.Empty;
                var queryDate = new FBSSConfigQuery
                {
                    CON_TYPE = "FBB_RETURNEDREVALUE_S4"
                };
                var VAL_LIST = _queryProcessor.Execute(queryDate);

                if(VAL_LIST.PROGRAM_PROCESS == true)
                {

                    foreach (var item in VAL_LIST.VAL_LIST)
                    {
                        if (item.CON_NAME == "DATE_START")
                        {
                            p_date_start = item.VAL1;
                        }

                        if (item.CON_NAME == "DATE_TO")
                        {
                            p_date_to = item.VAL1;
                        }
                    }

                    _logger.Info("Batch FBBReturnedRevalueAssetS4HANA start");

                    var query = new FBBReturnedRevalueAssetS4HANAQuery
                    {
                        p_date_start = p_date_start,
                        p_date_to = p_date_to
                    };
                    _queryProcessor.Execute(query);
                }


                _logger.Info("FBBReturnedRevalueAssetS4HANA end");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckProcessBatch : " + ex.Message);
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


        private void CallSAPWebService(FBBReturnedCpeModel item)
        {
            //_logger.Debug(String.Format("Returned Access Number: {0}", item.ACCESS_NUMBER));

            //var SERVICE_LIST = new List<NewRegistFOAServiceList>();
            //SERVICE_LIST.Add(new NewRegistFOAServiceList { ServiceName = item.SERVICE_LIST });

            //string msg = "";

            //NewRegistForSubmitFOAResponse result = null;
            //try
            //{
            //    var query = new NewRegistForSubmitFOAQuery
            //    {
            //        Access_No = item.ACCESS_NUMBER,
            //        BUILDING_NAME = item.BUILDING_NAME,
            //        FOA_Submit_date = item.FOA_SUBMIT_DATE == null ? "" : item.FOA_SUBMIT_DATE.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("en-US")),
            //        Mobile_Contact = item.MOBILE_CONTACT,
            //        OLT_NAME = item.OLT_NAME,
            //        OrderNumber = item.ORDER_NO,
            //        OrderType = item.ORDER_TYPE,
            //        ProductList = XDocument.Parse(item.PRODUCT_LIST).Descendants("PRODUCT").Select(node =>
            //            new NewRegistFOAProductList
            //            {
            //                SerialNumber = node.Element("SN") == null ? "" : node.Element("SN").Value,
            //                MaterialCode = node.Element("MATERIAL_CODE") == null ? "" : node.Element("MATERIAL_CODE").Value,
            //                CompanyCode = node.Element("COMPANY_CODE") == null ? "" : node.Element("COMPANY_CODE").Value,
            //                Plant = node.Element("PLANT") == null ? "" : node.Element("PLANT").Value,
            //                StorageLocation = node.Element("STORAGE_LOCATION") == null ? "" : node.Element("STORAGE_LOCATION").Value,
            //                SNPattern = node.Element("SN_PATTERN") == null ? "" : node.Element("SN_PATTERN").Value,
            //                MovementType = node.Element("MOVEMENT_TYPE") == null ? "" : "Returned (Old SN)"
            //            }).ToList(),
            //        ProductName = item.PRODUCT_NAME,
            //        RejectReason = item.REJECT_REASON,
            //        ServiceList = SERVICE_LIST,
            //        SubcontractorCode = item.SUBCONTRACT_CODE,
            //        SubcontractorName = item.SUBCONTRACT_NAME,
            //        SubmitFlag = item.SUBMIT_FLAG,
            //        Post_Date = item.POST_DATE,
            //        Address_ID = item.ADDESS_ID,
            //        ORG_ID = item.ORG_ID,
            //        Reuse_Flag = item.REUSE_FLAG,
            //        Event_Flow_Flag = item.EVENT_FLOW_FLAG,
            //        UserName = "FBBReturnedFixedAssetJob",
            //        Subcontract_Type = item.SUBCONTRACT_TYPE,
            //        Subcontract_Sub_Type = item.SUBCONTRACT_SUB_TYPE,
            //        Request_Sub_Flag = item.REQUEST_SUB_FLAG,
            //        Sub_Access_Mode = item.SUB_ACCESS_MODE,
            //        ReturnMessage = ""
            //    };

            //    result = _queryProcessor.Execute(query);

            //    if (result.result.Contains("-1"))
            //        _countError += 1;


            //    msg = String.Format("SAP Access Number: {0}|{1}"
            //   , item.ACCESS_NUMBER
            //   , result.result == "-1" ? "ERROR" : result.errorReason).TrimEnd('|');
            //    _logger.Debug(msg);

            //    _countSuccess += 1;
            //}
            //catch (Exception ex)
            //{
            //    _countError += 1;
            //    msg = String.Format("ERROR Access Number: {0}|{1}"
            //    , item.ACCESS_NUMBER
            //    , ex.Message.ToString());
            //    _logger.Debug(msg);
            //}
        }

    }
}
