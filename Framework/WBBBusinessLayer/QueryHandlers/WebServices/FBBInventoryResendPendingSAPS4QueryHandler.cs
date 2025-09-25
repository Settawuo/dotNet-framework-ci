using Confluent.Kafka;
using Inetlab.SMPP.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.WebServices;
using WBBContract.QueryModels.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using static WBBEntity.PanelModels.WebServiceModels.FBBInventoryResendPendingSAPS4QueryModel;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class FBBInventoryResendPendingSAPS4QueryHandler : IQueryHandler<FBBInventoryResendPendingSAPS4Query, FBBInventoryResendPendingSAPS4QueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBInventoryResendPendingSAPS4QueryModel> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        private readonly IWBBUnitOfWork _uow;

        public FBBInventoryResendPendingSAPS4QueryHandler(ILogger logger, IEntityRepository<FBBInventoryResendPendingSAPS4QueryModel> objService,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> hisLog,
           IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog,
           IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _cfgLov = cfgLov;
            _uow = uow;
            _hisLog = hisLog;
            _intfLog = intfLog;
        }

        public FBBInventoryResendPendingSAPS4QueryModel Handle(FBBInventoryResendPendingSAPS4Query query)
        {            
            // Define Input Parameters
            var P_DATE_START = new OracleParameter();
            P_DATE_START.ParameterName = "P_DATE_START";
            P_DATE_START.OracleDbType = OracleDbType.Varchar2;
            P_DATE_START.Direction = ParameterDirection.Input;
            P_DATE_START.Value = query.p_Date_Start;
            
            var P_DATE_TO = new OracleParameter();
            P_DATE_TO.ParameterName = "P_DATE_TO";
            P_DATE_TO.OracleDbType = OracleDbType.Varchar2;
            P_DATE_TO.Direction = ParameterDirection.Input;
            P_DATE_TO.Value = query.p_Date_To;

            // Define Output Parameters
            var p_ws_header_cur = new OracleParameter();
            p_ws_header_cur.ParameterName = "p_ws_header_cur";
            p_ws_header_cur.OracleDbType = OracleDbType.RefCursor;
            p_ws_header_cur.Direction = ParameterDirection.Output;

            var p_ws_item_cur = new OracleParameter();
            p_ws_item_cur.ParameterName = "p_ws_item_cur";
            p_ws_item_cur.OracleDbType = OracleDbType.RefCursor;
            p_ws_item_cur.Direction = ParameterDirection.Output;

            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.Size = 2000;
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            //GoodsMovementKAFKAQuery response = new GoodsMovementKAFKAQuery();
            FBBInventoryResendPendingSAPS4QueryModel responseJson = new FBBInventoryResendPendingSAPS4QueryModel();
            PkgFbbFoaOrderManagementResponse executeResults = new PkgFbbFoaOrderManagementResponse();
            NewRegistForSubmitFOAS4HANAResponse newFOAResponse = new NewRegistForSubmitFOAS4HANAResponse();
            NewRegistForSubmitFOAS4HANAResponse NewRegistForSubmitFOAResponseResult = new NewRegistForSubmitFOAS4HANAResponse();
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            HistoryLogCommand hLog = new HistoryLogCommand();
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, "", "", "call package : P_GET_INV_RESEND_PENDING", "FBBInventoryResendPendingSAPS4QueryHandler", "", "", "");
            List<string> jsonResult = new List<string>();
            try
            {
                var result_get_pending_asset = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.P_GET_INV_RESEND_PENDING",
                new object[]
                    {
                        //Parameter Input
                        P_DATE_START,
                        P_DATE_TO,
                        //Parameter Output
                        p_ws_header_cur,
                        p_ws_item_cur,
                        ret_code,
                        ret_msg
                });
                if (result_get_pending_asset != null)
                {
                    executeResults.ret_code = result_get_pending_asset[2] != null ? result_get_pending_asset[2].ToSafeString() : "-1";
                    executeResults.ret_msg = result_get_pending_asset[3] != null ? result_get_pending_asset[3].ToSafeString() : "";
                    NewRegistForSubmitFOAResponseResult.result = ret_code.Value.ToSafeString();
                    NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToSafeString();
                    var converterDataTable = new DataTableToXmlConverter();

                    string headerXml = result_get_pending_asset[0] is DataTable dt ? converterDataTable.ConvertDataTableToCustomHeaderXml(dt) : "";
                    string itemsXml = result_get_pending_asset[1] is DataTable dt2 ? converterDataTable.ConvertDataTableToCustomBodyXml(dt2) : "";

                    responseJson = new FBBInventoryResendPendingSAPS4QueryModel
                    {
                        ret_code = "Success"
                    };

                    XmlToJsonConverter converter = new XmlToJsonConverter();
                    

                    var headerKAFKA = new JObject();
                    string dateTimeString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    var ESB_KAFKA_HEADER = from item in _cfgLov.Get()
                                           where item.LOV_TYPE == "ESB_KAFKA_HEADER" && item.ACTIVEFLAG == "Y"
                                           orderby item.LOV_ID ascending
                                           select item;

                    foreach (var item in ESB_KAFKA_HEADER)
                    {
                        if (item.LOV_NAME == "groupTags")
                        {
                            if (item.LOV_VAL1 != null)
                            {
                                string jsonContent = "[" + item.LOV_VAL1 + "]";
                                headerKAFKA[item.LOV_NAME] = JArray.Parse(jsonContent);
                            }
                            else
                            {
                                headerKAFKA[item.LOV_NAME] = new JArray { };
                            }
                        }
                        else if (item.LOV_NAME == "identity")
                        {
                            if (item.LOV_VAL1 != null)
                            {
                                string jsonContent = "{" + item.LOV_VAL1 + "}";
                                headerKAFKA[item.LOV_NAME] = JObject.Parse(jsonContent);
                            }
                            else
                            {
                                headerKAFKA[item.LOV_NAME] = new JObject { };
                            }
                        }
                        else
                        {
                            headerKAFKA[item.LOV_NAME] = item.LOV_VAL1 != null ? item.LOV_VAL1 : string.Empty;
                        }
                    }

                    jsonResult = converter.ConvertXmlToJson(headerXml, itemsXml, headerKAFKA);
                    responseJson = new FBBInventoryResendPendingSAPS4QueryModel
                    {
                        ret_code = "Success",
                        item_json = jsonResult
                    };



                    foreach (var item in jsonResult)
                    {
                        InterfaceLogPayGCommand logItem = new InterfaceLogPayGCommand();
                        var jsonObject = JObject.Parse(item);

                        // Access the PartnerMessageID
                        string partnerMessageID = jsonObject["body"]?["PartnerMessageID"]?.ToString();
                        string internetNo = jsonObject["body"]?["GRGISlipNo"]?.ToString();

                        //insert interface log
                        logItem = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, item, partnerMessageID, "Call GoodsMovementKAFKA", "FBBInventoryResendPendingSAPS4QueryHandler", internetNo, "Fixed Asset", "Exs.");

                    }

                }
                else
                {
                    responseJson = new FBBInventoryResendPendingSAPS4QueryModel
                    {
                        ret_code = "Error",
                        ret_msg = "Package P_GET_INV_RESEND_PENDING Data Return Null!"
                    };
                }
            }
            catch (Exception ex)
            {
                responseJson = new FBBInventoryResendPendingSAPS4QueryModel
                {
                    ret_code = "Error",
                    ret_msg = ex.Message.ToSafeString()
                };
            }

            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, jsonResult, log, responseJson.ret_code, responseJson.ret_code == "Success" ? "" : responseJson.ret_msg, "");
            return responseJson;
        }

        public class HistoryLogModel
        {
            public List<Dictionary<string, object>> header { get; set; }
            public List<Dictionary<string, object>> body { get; set; }
            public string ReturnCode { get; set; }
            public string ReturnMessage { get; set; }
        }


        public List<Dictionary<string, object>> ConvertCursorToList(DataTable cursor)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in cursor.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in cursor.Columns)
                {
                    object value = row[col];

                    if (value is string strValue)
                    {
                        string decodedValue = WebUtility.HtmlDecode(strValue);

                        try
                        {
                            var xml = XElement.Parse(decodedValue);

                            if (col.ColumnName.Equals("SerialNumber", StringComparison.OrdinalIgnoreCase))
                            {
                                dict[col.ColumnName] = xml;
                            }
                            else
                            {
                                dict[col.ColumnName] = xml.ToString(SaveOptions.DisableFormatting);
                            }
                        }
                        catch
                        {
                            dict[col.ColumnName] = decodedValue;
                        }
                    }
                    else
                    {
                        dict[col.ColumnName] = value;
                    }
                }

                list.Add(dict);
            }

            return list;
        }

        public static HistoryLogCommand StartHistoryLog<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> historyLog, T query,
        string transactionId, string actionName, string msg, string createBy)
        {
            var dbIntfCmd = new HistoryLogCommand
            {
                ActionBy = actionName,
                TRANSACTION_ID = transactionId,
                IN_FOA = query.DumpToXml(),
                INSTALLATION = query.DumpToXml(),
                IN_SAP = query.DumpToXml(),
                OUT_SAP = query.DumpToXml(),
                OUT_FOA = query.DumpToXml(),
                REQUEST_STATUS = msg,
                CREATED_BY = createBy,
            };

            var log = HistoryLogHelper.Log(uow, historyLog, dbIntfCmd);
            //uow.Persist();

            dbIntfCmd.HISTORY_ID = log.HISTORY_ID;
            return dbIntfCmd;
        }

        public static void UpdateHistoryLog<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> historyLog, T query,
            HistoryLogCommand dbIntfCmd, string transactionId, string actionName, string msg, string createBy)
        {
            dbIntfCmd.ActionBy = actionName;
            dbIntfCmd.TRANSACTION_ID = transactionId;
            dbIntfCmd.IN_FOA = query.DumpToXml();
            dbIntfCmd.INSTALLATION = query.DumpToXml();
            dbIntfCmd.IN_SAP = query.DumpToXml();
            dbIntfCmd.OUT_SAP = query.DumpToXml();
            dbIntfCmd.OUT_FOA = query.DumpToXml();
            dbIntfCmd.REQUEST_STATUS = msg;
            HistoryLogHelper.Log(uow, historyLog, dbIntfCmd);
            //uow.Persist();
        }
        public static async Task<GoodsMovementKAFKAQuery> producerDataAsync(string jsonString, string BootstrapServers, string SslCaLocation, string SslKeystoreLocation
                    , string SslKeystorePassword, string SslKeyPassword, string SaslUsername, string SaslPassword, string TopicProduce, string envHeader, string envHeaderEncoding)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = BootstrapServers,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SslCaLocation = SslCaLocation,
                SslKeystoreLocation = SslKeystoreLocation,
                SslKeystorePassword = SslKeystorePassword,
                SslKeyPassword = SslKeyPassword,
                SaslMechanism = SaslMechanism.ScramSha256,
                SaslUsername = SaslUsername,
                SaslPassword = SaslPassword,
                Acks = Acks.All, // For reliability, adjust as needed
            };

            GoodsMovementKAFKAQuery result = new GoodsMovementKAFKAQuery(); // Correctly named 'result'
            var message = JsonConvert.DeserializeObject<dynamic>(jsonString);
            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                var jsonValue = JsonConvert.SerializeObject(message);
                var produceMessage = new Message<Null, string>
                {
                    Value = jsonValue,
                    Headers = new Headers
                    {
                        new Header(envHeader, Encoding.UTF8.GetBytes(envHeaderEncoding)),
                    }
                };


                try
                {
                    var deliveryResult = await producer.ProduceAsync(TopicProduce.ToSafeString(), produceMessage);

                    result.Return_Code = "Success";
                    result.Return_Message = "Producer Success";
                    result.Return_Transactions = $@"
                        Status: {(deliveryResult.Status == PersistenceStatus.Persisted ? "Success" : "Failed")}
                        Topic: {deliveryResult.Topic}
                        Offset: {deliveryResult.Offset}";
                }
                catch (ProduceException<Null, string> e)
                {
                    result.Return_Code = "Error";
                    result.Return_Message = $"Failed to produce : {e.Error.Reason}";
                    result.Return_Transactions = $"Failed to produce : {e.Error.Reason}";
                }
            }
            return result;
        }

        public static string GetSafeConsumerConfigInfo(ConsumerConfig config)
        {
            return $"\nBootstrapServers: {config.BootstrapServers}\n" +
                $", SaslUsername: {config.SaslUsername}\n" +
                $", GroupId: {config.GroupId}\n";
        }

        public class DataTableToXmlConverter
        {
            public string ConvertDataTableToCustomHeaderXml(DataTable dt)
            {
                var xml = new XElement("HEADERSAP",
                    dt.AsEnumerable().Select(row => new XElement("ItemHeader",
                        dt.Columns.Cast<DataColumn>().Select(col =>
                            new XElement(col.ColumnName, row[col] ?? ""))
                    ))
                );

                return xml.ToString();
            }

            public string ConvertDataTableToCustomBodyXml(DataTable dt)
            {
                var xml = new XElement("BODYSAP",
                    dt.AsEnumerable().Select(row =>
                    {
                        var itemBody = new XElement("ItemBody");

                        foreach (var col in dt.Columns.Cast<DataColumn>())
                        {
                            if (col.ColumnName.Equals("SerialNumber", StringComparison.OrdinalIgnoreCase))
                            {
                                var serialNumberValue = row[col]?.ToString() ?? "";

                                try
                                {
                                    var serialElements = XElement.Parse($"<Root>{serialNumberValue}</Root>").Elements();
                                    itemBody.Add(serialElements);
                                }
                                catch (Exception ex)
                                {
                                    itemBody.Add(new XElement("Error", $"Invalid XML: {ex.Message}"));
                                }
                            }
                            else
                            {
                                itemBody.Add(new XElement(col.ColumnName, row[col] ?? ""));
                            }
                        }

                        return itemBody;
                    })
                );

                return xml.ToString();
            }
        }

        public class XmlToJsonConverter
        {
            public List<string> ConvertXmlToJson(string headerXml, string itemsXml, JObject headerKAFKA)
            {
                XDocument headerDoc = XDocument.Parse(headerXml);
                XDocument itemsDoc = XDocument.Parse(itemsXml);
                var headers = headerDoc.Root.Elements("ItemHeader");
                var items = itemsDoc.Root.Elements("ItemBody");

                List<string> jsonStrings = new List<string>();

                foreach (var header in headers)
                {
                    string companyCode = header.Element("CompanyCode")?.Value;
                    string movementType = header.Element("MovementType")?.Value;
                    string partnerMessageID = header.Element("PartnerMessageID")?.Value;

                    var matchingItems = items.Where(i =>
                                    i.Element("CompanyCode")?.Value == companyCode &&
                                    i.Element("MovementType")?.Value == movementType &&
                                    i.Element("PartnerMessageID")?.Value == partnerMessageID ||
                                    (string.IsNullOrEmpty(i.Element("CompanyCode")?.Value) &&
                                    companyCode == "1200" &&
                                    i.Element("MovementType")?.Value == movementType) &&
                                    i.Element("PartnerMessageID")?.Value == partnerMessageID
                    ).ToList();

                    var headerObj = new JObject();
                    var PartnerMessageID = "";
                    foreach (var element in header.Elements())
                    {
                        if (element.Name.LocalName == "PartnerMessageID")
                        {
                            PartnerMessageID = element.Value;
                        }
                        if (element.Name.LocalName != "CompanyCode" && element.Name.LocalName != "MovementType")
                        {
                            headerObj[element.Name.LocalName] = element.Value;
                        }
                    }

                    var itemArray = new JArray();
                    foreach (var item in matchingItems)
                    {
                        var itemObj = new JObject();
                        var serialArray = new JArray();
                        foreach (var itemElement in item.Elements())
                        {
                            if (itemElement.Name.LocalName == "Serial")
                            {
                                var serialObj = new JObject();
                                foreach (var serialElement in itemElement.Elements())
                                {
                                    serialObj[serialElement.Name.LocalName] = serialElement.Value;
                                }
                                serialArray.Add(serialObj);
                            }
                            else if (itemElement.Name.LocalName == "Receivingissuingstoragelocatio")
                            {
                                itemObj["Receivingissuingstoragelocation"] = itemElement.Value;
                            }
                            else if (itemElement.Name.LocalName == "ShelfLifeExpirationorBestBefor")
                            {
                                itemObj["ShelfLifeExpirationorBestBeforeDate"] = itemElement.Value;
                            }
                            else
                            {
                                itemObj[itemElement.Name.LocalName] = itemElement.Value;
                            }
                        }

                        if (serialArray.Count > 0)
                        {
                            itemObj["Serial"] = serialArray;
                        }
                        itemObj.Remove("PartnerMessageID");
                        itemArray.Add(itemObj);
                    }

                    var body = new JObject(headerObj);
                    body["Item"] = itemArray;

                    string dateTimeString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    headerKAFKA["session"] = PartnerMessageID;
                    headerKAFKA["transaction"] = PartnerMessageID;
                    headerKAFKA["timestamp"] = PartnerMessageID;
                    headerKAFKA["useCaseExpiryTime"] = dateTimeString;
                    headerKAFKA["useCaseStartTime"] = dateTimeString;


                    var enrichedHeader = new JObject
                    {
                        ["header"] = headerKAFKA,
                        ["body"] = body
                    };

                    jsonStrings.Add(enrichedHeader.ToString());
                }
                return jsonStrings;
            }
        }

        public GoodsMovementKAFKAQuery ProducePendingAsset()
        {
            // Define Output Parameters
            var p_ws_header_cur = new OracleParameter();
            p_ws_header_cur.ParameterName = "p_ws_header_cur";
            p_ws_header_cur.OracleDbType = OracleDbType.RefCursor;
            p_ws_header_cur.Direction = ParameterDirection.Output;

            var p_ws_item_cur = new OracleParameter();
            p_ws_item_cur.ParameterName = "p_ws_item_cur";
            p_ws_item_cur.OracleDbType = OracleDbType.RefCursor;
            p_ws_item_cur.Direction = ParameterDirection.Output;

            var ret_code_get_pending_asset = new OracleParameter();
            ret_code_get_pending_asset.ParameterName = "ret_code";
            ret_code_get_pending_asset.Size = 2000;
            ret_code_get_pending_asset.OracleDbType = OracleDbType.Varchar2;
            ret_code_get_pending_asset.Direction = ParameterDirection.Output;

            //var ret_msg_get_pending_asset = new OracleParameter();
            //ret_msg_get_pending_asset.ParameterName = "ret_msg";
            //ret_msg_get_pending_asset.Size = 2000;
            //ret_msg_get_pending_asset.OracleDbType = OracleDbType.Varchar2;
            //ret_msg_get_pending_asset.Direction = ParameterDirection.Output;

            GoodsMovementKAFKAQuery response = new GoodsMovementKAFKAQuery();
            PkgFbbFoaOrderManagementResponse executeResults = new PkgFbbFoaOrderManagementResponse();
            NewRegistForSubmitFOAS4HANAResponse newFOAResponse = new NewRegistForSubmitFOAS4HANAResponse();
            NewRegistForSubmitFOAS4HANAResponse NewRegistForSubmitFOAResponseResult = new NewRegistForSubmitFOAS4HANAResponse();
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            HistoryLogCommand hLog = new HistoryLogCommand();
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, "", "", "call package : p_get_pending_asset", "GoodsMovementKAFKAQueryHandler", "", "", "");

            try
            {
                var result_get_pending_asset = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_pending_asset",
                new object[]
                    {
                            //Parameter Output
                            p_ws_header_cur,
                            p_ws_item_cur,
                            ret_code_get_pending_asset
                            //ret_msg_get_pending_asset
                });

                if (result_get_pending_asset != null)
                {
                    executeResults.ret_code = result_get_pending_asset[2] != null ? result_get_pending_asset[2].ToSafeString() : "-1";
                    //executeResults.ret_msg = result_get_pending_asset[3] != null ? result_get_pending_asset[3].ToSafeString() : "";
                    NewRegistForSubmitFOAResponseResult.result = ret_code_get_pending_asset.Value.ToSafeString();
                    NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToSafeString();
                    var converterDataTable = new DataTableToXmlConverter();

                    string headerXml = result_get_pending_asset[0] is DataTable dt ? converterDataTable.ConvertDataTableToCustomHeaderXml(dt) : "";
                    string itemsXml = result_get_pending_asset[1] is DataTable dt2 ? converterDataTable.ConvertDataTableToCustomBodyXml(dt2) : "";

                    response = new GoodsMovementKAFKAQuery
                    {
                        Return_Code = "Success"
                    };

                    XmlToJsonConverter converter = new XmlToJsonConverter();
                    List<string> jsonResult = new List<string>();

                    var headerKAFKA = new JObject();
                    string dateTimeString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    var ESB_KAFKA_HEADER = from item in _cfgLov.Get()
                                           where item.LOV_TYPE == "ESB_KAFKA_HEADER" && item.ACTIVEFLAG == "Y"
                                           orderby item.LOV_ID ascending
                                           select item;

                    foreach (var item in ESB_KAFKA_HEADER)
                    {
                        if (item.LOV_NAME == "groupTags")
                        {
                            if (item.LOV_VAL1 != null)
                            {
                                string jsonContent = "[" + item.LOV_VAL1 + "]";
                                headerKAFKA[item.LOV_NAME] = JArray.Parse(jsonContent);
                            }
                            else
                            {
                                headerKAFKA[item.LOV_NAME] = new JArray { };
                            }
                        }
                        else if (item.LOV_NAME == "identity")
                        {
                            if (item.LOV_VAL1 != null)
                            {
                                string jsonContent = "{" + item.LOV_VAL1 + "}";
                                headerKAFKA[item.LOV_NAME] = JObject.Parse(jsonContent);
                            }
                            else
                            {
                                headerKAFKA[item.LOV_NAME] = new JObject { };
                            }
                        }
                        else
                        {
                            headerKAFKA[item.LOV_NAME] = item.LOV_VAL1 != null ? item.LOV_VAL1 : string.Empty;
                        }
                    }

                    jsonResult = converter.ConvertXmlToJson(headerXml, itemsXml, headerKAFKA);


                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, jsonResult, log, response.Return_Code, response.Return_Code == "Success" ? "" : response.Return_Message, "");

                    foreach (var item in jsonResult)
                    {
                        var jsonObject = JObject.Parse(item);

                        // Access the PartnerMessageID
                        string partnerMessageID = jsonObject["body"]?["PartnerMessageID"]?.ToString();

                        //insert interface log
                        log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, item, partnerMessageID, "Call GoodsMovementKAFKA", "GoodsMovementKAFKAQueryHandler", "", "Fixed Asset", "Exs.");

                        //insert history log
                        hLog = StartHistoryLog(_uow, _hisLog, "", "", "IN_FOA", null, "FBB");

                        UpdateHistoryLog(_uow, _hisLog, item, hLog, partnerMessageID, "INSTALLATION", "", "");

                        UpdateHistoryLog(_uow, _hisLog, "", hLog, partnerMessageID, "OUT_FOA", "", "");
                    }


                    var query = new GoodsMovementKAFKAQuery()
                    {
                        action = "PRODUCER",
                        item_json = jsonResult
                    };

                    return query;
                }
                else
                {
                    response = new GoodsMovementKAFKAQuery
                    {
                        Return_Code = "Error",
                        Return_Message = "Package P_GET_PENDING_ASSET Data Return Null!"
                    };
                }

            }
            catch (Exception e)
            {
                response = new GoodsMovementKAFKAQuery
                {
                    Return_Code = "Error",
                    Return_Message = e.Message.ToSafeString()
                };

               InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, response, log, response.Return_Code, response.Return_Code == "Success" ? "" : response.Return_Message, "");
            }

            return null;
        }

    }
}

