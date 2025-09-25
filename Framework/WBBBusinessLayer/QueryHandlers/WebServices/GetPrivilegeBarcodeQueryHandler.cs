using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPrivilegeBarcodeQueryHandler : IQueryHandler<GetPrivilegeBarcodeQuery, GetPrivilegeBarcodeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;

        public GetPrivilegeBarcodeQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
        }

        public GetPrivilegeBarcodeModel Handle(GetPrivilegeBarcodeQuery query)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MobileNo + query.ClientIP, "requestPrivilegeBarcode", "GetPrivilegeBarcodeQuery", query.IDCardNo, "FBB|" + query.FullURL, "");
            GetPrivilegeBarcodeModel getPrivilegeBarcodeModel = new GetPrivilegeBarcodeModel();

            try
            {
                GetPrivilegeBarcodeResponse result = new GetPrivilegeBarcodeResponse();
                string URL_PRIVILEGE = "";
                string USER_NAME = "";
                string FROMPASS = "";// Fixed Code scan : string PASSWORD = "";
                string IP_ADDRESS = "";
                string MSISDN = "";
                string SHORT_CODE = "";

                URL_PRIVILEGE = query.UrlReqPrivilegeBarcode.ToSafeString();
                USER_NAME = query.Username.ToSafeString();
                FROMPASS = query.Password.ToSafeString();
                IP_ADDRESS = query.IpAddress.ToSafeString();
                MSISDN = query.MobileNo.ToSafeString();
                SHORT_CODE = query.ShortCode.ToSafeString();

                string TransactionID = DateTime.Now.ToString("yyyyMMddHHmmss") + query.MobileNo.ToSafeString();

                if (URL_PRIVILEGE != "" && USER_NAME != "" && IP_ADDRESS != "" && FROMPASS != "" && SHORT_CODE != "")
                {
                    GetPrivilegeBarcodeContent contents = new GetPrivilegeBarcodeContent
                    {
                        transactionID = TransactionID,
                        username = USER_NAME,
                        password = FROMPASS,
                        ipAddress = IP_ADDRESS,
                        msisdn = MSISDN,
                        shortcode = SHORT_CODE
                    };
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        string jsonStr = JsonConvert.SerializeObject(contents);
                        InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, jsonStr, query.MobileNo + query.ClientIP, "requestPrivilegeBarcodePost", "GetPrivilegeBarcodeQuery", "", "FBB|" + query.FullURL, "");
                        var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        var resultData = client.PostAsync(URL_PRIVILEGE, content).Result;
                        if (resultData != null && resultData.IsSuccessStatusCode)
                        {
                            string resultContent = resultData.Content.ReadAsStringAsync().Result;
                            result = JsonConvert.DeserializeObject<GetPrivilegeBarcodeResponse>(resultContent);
                            if (result != null)
                            {
                                getPrivilegeBarcodeModel = new GetPrivilegeBarcodeModel
                                {
                                    TransactionID = result.transactionID.ToSafeString(),
                                    HttpStatus = result.httpStatus.ToSafeString(),
                                    Status = result.status.ToSafeString(),
                                    Description = result.description.ToSafeString(),
                                    Msg = result.msg.ToSafeString(),
                                    RegID = result.regId.ToSafeString(),
                                    MsgBarcode = result.msgBarcode.ToSafeString(),
                                    BarcodeType = result.barcodeType.ToSafeString(),
                                    Ssid = result.ssid.ToSafeString()
                                };
                                if (result.status == "20000" && result.httpStatus == "200")
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                                }
                                else if (result.httpStatus == "200")
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                                }
                                else
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "", "");
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                                }
                            }
                            else
                            {
                                getPrivilegeBarcodeModel.TransactionID = "";
                                getPrivilegeBarcodeModel.HttpStatus = "500";
                                getPrivilegeBarcodeModel.Status = "Error";
                                getPrivilegeBarcodeModel.Description = "Sevice No result";
                                getPrivilegeBarcodeModel.Msg = "";
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "Sevice No result", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }

                        }
                        else if (resultData != null)
                        {
                            string resultContent = resultData.Content.ReadAsStringAsync().Result;
                            result = JsonConvert.DeserializeObject<GetPrivilegeBarcodeResponse>(resultContent);
                            if (result != null)
                            {
                                getPrivilegeBarcodeModel = new GetPrivilegeBarcodeModel
                                {
                                    TransactionID = result.transactionID.ToSafeString(),
                                    HttpStatus = result.httpStatus.ToSafeString(),
                                    Status = result.status.ToSafeString(),
                                    Description = result.description.ToSafeString(),
                                    Msg = result.msg.ToSafeString(),
                                    RegID = result.regId.ToSafeString(),
                                    MsgBarcode = result.msgBarcode.ToSafeString(),
                                    BarcodeType = result.barcodeType.ToSafeString(),
                                    Ssid = result.ssid.ToSafeString()
                                };
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getPrivilegeBarcodeModel, log2, "Failed", "Call Service not Success", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }
                            else
                            {
                                getPrivilegeBarcodeModel.TransactionID = "";
                                getPrivilegeBarcodeModel.HttpStatus = "500";
                                getPrivilegeBarcodeModel.Status = "Error";
                                getPrivilegeBarcodeModel.Description = "Call Service not Success";
                                getPrivilegeBarcodeModel.Msg = "resultContent is null";
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getPrivilegeBarcodeModel, log2, "Failed", "Call Service not Success", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }
                        }
                        else
                        {
                            getPrivilegeBarcodeModel.TransactionID = "";
                            getPrivilegeBarcodeModel.HttpStatus = "500";
                            getPrivilegeBarcodeModel.Status = "Error";
                            getPrivilegeBarcodeModel.Description = "Call Service not Success";
                            getPrivilegeBarcodeModel.Msg = "resultData is null";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getPrivilegeBarcodeModel, log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getPrivilegeBarcodeModel, log, "Failed", "", "");
                        }

                    }
                }
                else
                {
                    getPrivilegeBarcodeModel.TransactionID = "";
                    getPrivilegeBarcodeModel.HttpStatus = "500";
                    getPrivilegeBarcodeModel.Status = "Error";
                    getPrivilegeBarcodeModel.Description = "Config Lov is null";
                    getPrivilegeBarcodeModel.Msg = "LOVData is null";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getPrivilegeBarcodeModel, log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                getPrivilegeBarcodeModel.TransactionID = "";
                getPrivilegeBarcodeModel.HttpStatus = "500";
                getPrivilegeBarcodeModel.Status = "Error";
                getPrivilegeBarcodeModel.Description = ex.Message;
                getPrivilegeBarcodeModel.Msg = "";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getPrivilegeBarcodeModel, log, "Failed", ex.GetErrorMessage(), "");
            }
            return getPrivilegeBarcodeModel;
        }
    }

    public class GetPrivilegeBarcodeContent
    {
        public string transactionID { get; set; }
        public string username { get; set; }
        public string ipAddress { get; set; }
        public string password { get; set; }
        public string msisdn { get; set; }
        public string shortcode { get; set; }
    }

    public class GetPrivilegeBarcodeResponse
    {
        public string transactionID { get; set; }
        public string httpStatus { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string msg { get; set; }
        public string regId { get; set; }
        public string msgBarcode { get; set; }
        public string barcodeType { get; set; }
        public string ssid { get; set; }
    }
}
