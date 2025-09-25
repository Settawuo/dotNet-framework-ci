using Confluent.Kafka;
using Inetlab.SMPP.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
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
using static WBBEntity.PanelModels.WebServiceModels.GoodsMovementKAFKAQueryModel;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GoodsMovementKAFKAQueryHandler : IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GoodsMovementKAFKAQueryModel> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        private readonly IWBBUnitOfWork _uow;

        public GoodsMovementKAFKAQueryHandler(ILogger logger, IEntityRepository<GoodsMovementKAFKAQueryModel> objService,
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

        public GoodsMovementKAFKAQueryModel Handle(GoodsMovementKAFKAQuery query)
        {
            
                var retun_value = new GoodsMovementKAFKAQueryModel();
            try
            {
                var ESB_KAFKA_CONFIG = from item in _cfgLov.Get()
                                       where item.LOV_TYPE == "ESB_KAFKA_CONFIG"
                                       select item;


                var ESB_HTTP_HEADER = from item in _cfgLov.Get()
                                      where item.LOV_TYPE == "ESB_HTTP_HEADER"
                                      select item;


                var envHeader = ESB_HTTP_HEADER.FirstOrDefault(item => item.LOV_NAME == "env").LOV_NAME ?? "";
                var envHeaderEncoding = ESB_HTTP_HEADER.FirstOrDefault(item => item.LOV_NAME == "env").LOV_VAL1 ?? "";

                var BootstrapServers = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "BootstrapServers").LOV_VAL1 ?? "";
                var SaslPassword = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "SaslPassword").LOV_VAL1 ?? "";
                var SaslUsername = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "SaslUsername").LOV_VAL1 ?? "";
                var SslCaLocation = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "SslCaLocation").LOV_VAL1 ?? "";
                var SslKeyPassword = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "SslKeyPassword").LOV_VAL1 ?? "";
                var SslKeystoreLocation = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "SslKeystoreLocation").LOV_VAL1 ?? "";
                var SslKeystorePassword = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "SslKeystorePassword").LOV_VAL1 ?? "";
                var GroupId = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "GroupId").LOV_VAL1 ?? "";
                var TopicProduce = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "TopicProduce").LOV_VAL1 ?? "";
                var TopicConsume = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "TopicConsume").LOV_VAL1 ?? "";
                var TimeoutConsume = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "TimeoutConsume").LOV_VAL1 ?? "";
                var AmountTempItem = ESB_KAFKA_CONFIG.FirstOrDefault(item => item.LOV_NAME == "AmountTempItem").LOV_VAL1 ?? "";

                //CaLocation for develop
                //SslCaLocation = "C:/kafka/cert/Stg/kafka.server.truststore.pem";
                //SslKeystoreLocation = "C:/kafka/cert/Stg/kafka.server.keystore.p12";
                if (query.action.ToUpper() == "PRODUCER")
                {

                    GoodsMovementKAFKAQuery result = new GoodsMovementKAFKAQuery();
                    foreach (var item in query.item_json)
                    {
                        // Parse the JSON
                        var jsonObject = JObject.Parse(item);

                        // Access the PartnerMessageID
                        string partnerMessageID = jsonObject["body"]?["PartnerMessageID"]?.ToString();
                        string referenceDocumentNumber = jsonObject["body"]?["ReferenceDocumentNumber"]?.ToString();


                        result = producerDataAsync(item, BootstrapServers, SslCaLocation, SslKeystoreLocation
                            , SslKeystorePassword, SslKeyPassword, SaslUsername, SaslPassword, TopicProduce, envHeader, envHeaderEncoding).GetAwaiter().GetResult();

                        InterfaceLogPayGCommand INTERFACE_ID_INF_PAYG = new InterfaceLogPayGCommand();
                        HistoryLogCommand HISTORY_LOG = new HistoryLogCommand();
                        var interfaceId = (from intf in _intfLog.Get()
                                           where intf.IN_TRANSACTION_ID == partnerMessageID && intf.METHOD_NAME == "Call GoodsMovementKAFKA" orderby intf.CREATED_DATE descending
                                           select intf.INTERFACE_ID).FirstOrDefault();

                        var historyId = (from hisLog in _hisLog.Get()
                                         where hisLog.TRANSACTION_ID == partnerMessageID orderby hisLog.CREATED_DATE descending
                                         select hisLog.HISTORY_ID).FirstOrDefault();

                        INTERFACE_ID_INF_PAYG.OutInterfaceLogId = (decimal)interfaceId;
                        INTERFACE_ID_INF_PAYG.IN_TRANSACTION_ID = partnerMessageID;

                        HISTORY_LOG.HISTORY_ID = (long)historyId;
                        HISTORY_LOG.TRANSACTION_ID = partnerMessageID;

                        if (result.Return_Code == "Success")
                        {
                            var p_trans_id = new OracleParameter();
                            p_trans_id.ParameterName = "p_trans_id";
                            p_trans_id.OracleDbType = OracleDbType.Varchar2;
                            p_trans_id.Direction = ParameterDirection.Input;
                            p_trans_id.Size = 2000;
                            p_trans_id.Value = partnerMessageID;

                            var p_order_no = new OracleParameter();
                            p_order_no.ParameterName = "p_order_no";
                            p_order_no.OracleDbType = OracleDbType.Varchar2;
                            p_order_no.Direction = ParameterDirection.Input;
                            p_order_no.Size = 2000;
                            p_order_no.Value = referenceDocumentNumber;

                            var ret_code_tran_log = new OracleParameter();
                            ret_code_tran_log.ParameterName = "ret_code";
                            ret_code_tran_log.OracleDbType = OracleDbType.Decimal;
                            ret_code_tran_log.Direction = ParameterDirection.Output;
                            ret_code_tran_log.Size = 2000;

                            var ret_msg_tran_log = new OracleParameter();
                            ret_msg_tran_log.ParameterName = "ret_msg";
                            ret_msg_tran_log.OracleDbType = OracleDbType.Varchar2;
                            ret_msg_tran_log.Direction = ParameterDirection.Output;
                            ret_msg_tran_log.Size = 2000;

                            var execute_tran_log = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_fixed_asset_tran_log",
                              new
                              {
                                  p_trans_id,
                                  p_order_no,
                                  ret_code_tran_log,
                                  ret_msg_tran_log
                              }).ToList();

                        }
                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", INTERFACE_ID_INF_PAYG, result.Return_Code, result.Return_Message, "");
                        UpdateHistoryLog(_uow, _hisLog, item, HISTORY_LOG, partnerMessageID, "IN_SAP", result.Return_Message, "");

                    }
                    retun_value.ret_code = result.Return_Code;
                    retun_value.ret_msg = result.Return_Message;

                    return retun_value;

                }
                else if (query.action.ToUpper() == "CONSUMER")
                {

                    InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
                    HistoryLogCommand hLog = new HistoryLogCommand();

                    InterfaceLogPayGCommand INTERFACE_ID_INF_PAYG = new InterfaceLogPayGCommand();
                    HistoryLogCommand HISTORY_LOG = new HistoryLogCommand();


                    var config3 = new ConsumerConfig
                    {
                        BootstrapServers = BootstrapServers,
                        SecurityProtocol = SecurityProtocol.SaslSsl,
                        SaslMechanism = SaslMechanism.ScramSha256,
                        SaslUsername = SaslUsername,
                        SaslPassword = SaslPassword,
                        GroupId = GroupId,
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        EnableAutoCommit = true,
                        EnablePartitionEof = true,
                        SslCaLocation = SslCaLocation,
                        SslKeystoreLocation = SslKeystoreLocation,
                        SslKeystorePassword = SslKeystorePassword,
                        SslKeyPassword = SslKeyPassword,
                        //// Reduce waiting time for new messages (default: 100ms)

                        //// Performance tuning
                        //FetchWaitMaxMs = 50,  // Reduced from 100ms
                        //FetchMaxBytes = 52428800,  // 50MB (adjust based on your message size)
                        //MaxPartitionFetchBytes = 5242880,  // 5MB per partition
                        //QueuedMinMessages = 100000,  // Reduced from 1M
                        //QueuedMaxMessagesKbytes = 102400,  // 100MB queue size
                        //FetchMinBytes = 65536,  // 64KB - server will wait for this much data
                        //FetchErrorBackoffMs = 5,
                        //SessionTimeoutMs = 30000,  // Reduced from 45s
                        //HeartbeatIntervalMs = 3000,
                        //MaxPollIntervalMs = 300000,
                        Acks = Acks.All,
                        //PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky,

                        //// Additional performance settings
                        //SocketKeepaliveEnable = true,
                        ////EnableSslCertificateVerification = false,  // Only if you trust your network
                        //StatisticsIntervalMs = 0,  // Disable stats collection
                        //LogConnectionClose = false  // Reduce logging overhead
                    };


                    using (var consumer = new ConsumerBuilder<string, string>(config3).Build())
                    {
                        consumer.Subscribe(TopicConsume.ToSafeString());
                        CancellationTokenSource cts = new CancellationTokenSource();
                        Console.CancelKeyPress += (_, e) =>
                        {
                            e.Cancel = true;
                            cts.Cancel();
                        };
                        var countConsume = 0;
                        var transactionIDList = new ConcurrentBag<string>();
                        var batchItems = new List<GoodsMovementKAFKAList>();
                        bool shouldContinue = true;
                        string transactionID = "";
                        const int maxEmptyPolls = 3; // Max consecutive empty polls before stopping
                        int emptyPollCount = 0;
                        try
                        {
                            while (shouldContinue)
                            {
                                try
                                {
                                    var response = consumer.Consume(TimeSpan.FromSeconds(TimeoutConsume.ToSafeInteger()));
                                    var partitions = consumer.Assignment;


                                    if (response == null)
                                    {
                                        emptyPollCount++;
                                        // 1. Check if we've exceeded maximum empty polls
                                        if (emptyPollCount >= maxEmptyPolls)
                                        {
                                            shouldContinue = false;
                                            break;
                                        }
                                        var assignment = consumer.Assignment;
                                        foreach (var topicPartition in assignment)
                                        {
                                            var watermark = consumer.QueryWatermarkOffsets(topicPartition, TimeSpan.FromSeconds(5));
                                            var position = consumer.Position(topicPartition);

                                            if (position >= watermark.High)
                                            {
                                                shouldContinue = false;
                                                break;
                                            }
                                        }
                                        continue;
                                    }


                                    if (response != null)
                                    {
                                        if (response.Message != null && !string.IsNullOrEmpty(response.Message.Value))
                                        {
                                            emptyPollCount = 0;

                                            Parallel.Invoke(() =>
                                            {
                                                try
                                                {
                                                    var json = response.Message.Value;
                                                    using var doc = JsonDocument.Parse(json);
                                                    var root = doc.RootElement;
                                                    if (!root.TryGetProperty("body", out var body)) return;

                                                    var partnerMessageID = body.TryGetProperty("PartnerMessageID", out var pid)? pid.GetString(): null;

                                                    var interfaceId = (from intf in _intfLog.Get()
                                                                       where intf.IN_TRANSACTION_ID == partnerMessageID && intf.METHOD_NAME == "Call GoodsMovementKAFKA" orderby intf.CREATED_DATE descending
                                                                       select intf.INTERFACE_ID).FirstOrDefault();
                                                    INTERFACE_ID_INF_PAYG.OutInterfaceLogId = (decimal)interfaceId;
                                                    INTERFACE_ID_INF_PAYG.IN_TRANSACTION_ID = partnerMessageID;

                                                    if (body.TryGetProperty("Item", out var items) && items.ValueKind == JsonValueKind.Array)
                                                    {
                                                        var itemsArray = items.EnumerateArray().ToArray();
                                                        var processedItems = new GoodsMovementKAFKAList[itemsArray.Length];

                                                        Parallel.For(0, itemsArray.Length, i =>
                                                        {
                                                            var item = itemsArray[i];
                                                            processedItems[i] = ProcessItem(item, body, partnerMessageID);
                                                            if (!string.IsNullOrEmpty(partnerMessageID))
                                                            {
                                                                transactionIDList.Add(partnerMessageID);
                                                            }
                                                        });

                                                        lock (batchItems)
                                                        {
                                                            batchItems.AddRange(processedItems.Where(i => i != null));
                                                        }
                                                    }

                                                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, json, INTERFACE_ID_INF_PAYG, "Success", "Consumer Success", "FBB_GoodsMovementKAFKABatch");
                                                    if (batchItems.Count >= AmountTempItem.ToSafeInteger())//ทำเป็น config
                                                    {
                                                        SaveBatchToOracle(batchItems);
                                                        batchItems.Clear();
                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    retun_value.ret_code = "Error";
                                                    retun_value.ret_msg = ex.ToSafeString();
                                                    throw;
                                                }
                                            });

                                            retun_value.ret_code = "Success";

                                        }

                                    }
                                }
                                catch (ConsumeException ex)
                                {
                                    retun_value.ret_code = "Error";
                                    retun_value.ret_msg = ex.ToSafeString();
                                    shouldContinue = false;

                                }

                            }

                            if (batchItems.Count > 0)
                            {
                                SaveBatchToOracle(batchItems);
                                batchItems.Clear();
                            }


                        }
                        catch (Exception ex)
                        {
                            retun_value.ret_code = "Error";
                            retun_value.ret_msg = ex.ToSafeString();

                        }
                        finally
                        {
                            consumer.Close();

                            updateTempTable();
                        }
                        retun_value.return_transactions = string.Join(",", transactionIDList);
                    }

                    var getPendingAsset = ProducePendingAsset();
                    if(getPendingAsset.item_json != null)
                    {
                        retun_value.item_json = getPendingAsset.item_json;
                    }
                    if(getPendingAsset.ret_code_pending_asst != null)
                    {
                        retun_value.ret_code_pending_asst = getPendingAsset.ret_code_pending_asst;
                    }
                    if(getPendingAsset.ret_msg_pending_asst != null)
                    {
                        retun_value.ret_msg_pending_asst = getPendingAsset.ret_msg_pending_asst;
                    }
                    return retun_value;
                }

            }
            catch (Exception ex)
            {
                retun_value.ret_code = "Error";
                retun_value.ret_msg = ex.ToSafeString();
            }
            return null;
        }

        private void updateTempTable()
        {

            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();

            try
            {

                log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, "", "", "call package : p_upsert_resp_goodsmovement", "GoodsMovementKAFKAQueryHandler", "", "", "");
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.ParameterName = "ret_msg";
                ret_msg.Size = 2000;
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.p_upsert_resp_goodsmovement",
                new object[]
                {
                                        ret_code,
                                        ret_msg
                }).ToList();
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", log, "Success", executeResult[1].ToSafeString(), "");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", log, "Error", ex.Message.ToSafeString(), "");
                throw;
            }
        }

        private GoodsMovementKAFKAList ProcessItem(JsonElement item, JsonElement body, string partnerMessageID)
        {
            try
            {
                string flagTypeItem = item.TryGetProperty("FlagTypeItem", out var fti) ? fti.GetString() : null;
                string messageType = item.TryGetProperty("MessageType", out var mt) ? mt.GetString() : null;
                string grgiSlip = body.TryGetProperty("GRGISlip", out var gs) ? gs.GetString() : null;
                string referenceDocumentNumber = body.TryGetProperty("ReferenceDocumentNumber", out var rdn) ? rdn.GetString() : null;
                string company = body.TryGetProperty("Company", out var c) ? c.GetString() : null;
                string assetNumber = item.TryGetProperty("AssetNumber", out var an) ? an.GetString() : null;
                string assetSubNumber = item.TryGetProperty("AssetSubNumber", out var asn) ? asn.GetString() : null;
                string messageClass = item.TryGetProperty("MessageClass", out var mc) ? mc.GetString() : null;
                string messageNumber = item.TryGetProperty("MessageNumber", out var mn) ? mn.GetString() : null;
                string messageDesc = item.TryGetProperty("MessageDesc", out var md) ? md.GetString() : null;

                if (messageType == null || messageType == "")
                {
                    messageType = body.TryGetProperty("MessageType", out var rmt) ? rmt.GetString() : null;
                    messageNumber = body.TryGetProperty("MessageNumber", out var rmn) ? rmn.GetString() : null;
                    messageDesc = body.TryGetProperty("MessageDesc", out var rmd) ? rmd.GetString() : null;
                }

                string runGroupMsgType = null;
                if (messageType == "E")
                {
                    runGroupMsgType = item.TryGetProperty("Item", out var i)
                        ? i.GetString()
                        : (item.TryGetProperty("MaterialDocumentItem", out var mdi) ? mdi.GetString() : null);
                    messageType = "ERROR";
                }
                else if (messageType == "S")
                {
                    runGroupMsgType = item.TryGetProperty("Item", out var i) ? i.GetString() : null;
                    messageType = "COMPLETE";
                }

                string MatDocMsgClass = null;
                string DocYearMsgClass = null;

                if (messageClass == "ZSCM00")
                {
                    MatDocMsgClass = item.TryGetProperty("FIAcquisitionDocument", out var fiad) ? fiad.GetString() : null;
                    DocYearMsgClass = item.TryGetProperty("FIAcquisitionDocYear", out var fiady) ? fiady.GetString() : null;
                }
                else if (messageClass == "MIGO")
                {
                    MatDocMsgClass = item.TryGetProperty("MaterialDocument", out var mdC) ? mdC.GetString() : null;
                    DocYearMsgClass = body.TryGetProperty("MatDocYear", out var mdy) ? mdy.GetString() : null;
                }

                return new GoodsMovementKAFKAList
                {
                    p_TRANS_ID = partnerMessageID,
                    p_REC_TYPE = flagTypeItem,
                    p_RUN_GROUP = runGroupMsgType,
                    p_INTERNET_NO = grgiSlip,
                    p_ORDER_NO = referenceDocumentNumber,
                    p_COM_CODE = company,
                    p_ASSET_CODE = assetNumber,
                    p_SUBNUMBER = assetSubNumber,
                    p_MATERIAL_NO = null,
                    p_SERIAL_NO = null,
                    p_MATERIAL_DOC = MatDocMsgClass,
                    p_DOC_YEAR = DocYearMsgClass,
                    p_ERR_STATUS = messageType,
                    p_ERR_CODE = messageNumber,
                    p_ERR_MSG = messageDesc,
                    p_REF_DOC_NO = referenceDocumentNumber
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing item: {ex.Message}");
                return null;
            }
        }

        private void SaveBatchToOracle(List<GoodsMovementKAFKAList> batchItems)
        {
                InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            try
            {

                log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, "", "", "call package : INSERT_TEMP_GOODSMOVEMENT", "GoodsMovementKAFKAQueryHandler", "", "", "");
                var packageMappingObjectGoodsMovement = new PackageMappingObjectModel();
                packageMappingObjectGoodsMovement.GOODS_MOVEMENT_LIST =
                    batchItems.Select(a => new GOODS_MOVEMENT_LISTMapping
                    {
                        TRANS_ID = a.p_TRANS_ID,
                        REC_TYPE = a.p_REC_TYPE,
                        RUN_GROUP = a.p_RUN_GROUP,
                        INTERNET_NO = a.p_INTERNET_NO,
                        ORDER_NO = a.p_ORDER_NO,
                        COM_CODE = a.p_COM_CODE,
                        ASSET_CODE = a.p_ASSET_CODE,
                        SUBNUMBER = a.p_SUBNUMBER,
                        MATERIAL_NO = a.p_MATERIAL_NO,
                        SERIAL_NO = a.p_SERIAL_NO,
                        MATERIAL_DOC = a.p_MATERIAL_DOC,
                        DOC_YEAR = a.p_DOC_YEAR,
                        ERR_STATUS = a.p_ERR_STATUS,
                        ERR_CODE = a.p_ERR_CODE,
                        ERR_MSG = a.p_ERR_MSG,
                        REF_DOC_NO = a.p_REF_DOC_NO
                    }).ToArray();
                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_goodsmovement_data_list", "WBB.GOODS_MOVEMENT_LIST", packageMappingObjectGoodsMovement);


                var p_goodsmovement_data_list = new OracleParameter();
                p_goodsmovement_data_list.OracleDbType = OracleDbType.RefCursor;
                p_goodsmovement_data_list.Value = packageMapping;
                p_goodsmovement_data_list.Direction = ParameterDirection.Input;

                var p_ret_code = new OracleParameter();
                p_ret_code.OracleDbType = OracleDbType.Decimal;
                p_ret_code.ParameterName = "p_ret_code";
                p_ret_code.OracleDbType = OracleDbType.Varchar2;
                p_ret_code.Direction = ParameterDirection.Output;

                var p_ret_msg = new OracleParameter();
                p_ret_msg.OracleDbType = OracleDbType.Varchar2;
                p_ret_msg.ParameterName = "p_ret_msg";
                p_ret_msg.Size = 2000;
                p_ret_msg.OracleDbType = OracleDbType.Varchar2;
                p_ret_msg.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.INSERT_TEMP_GOODSMOVEMENT",
                new object[]
                {
                                        packageMapping,
                                        p_ret_code,
                                        p_ret_msg
                }).ToList();

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", log, "Success", executeResult[1].ToSafeString(), "");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", log, "Error", ex.Message.ToSafeString(), "");
                throw;
            }

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


        private static async Task<GoodsMovementKAFKAQuery> ProduceMessageAsync(
    string jsonString,
    ProducerConfig producerConfig,
    string topic,
    string envHeader,
    string envHeaderEncoding)
        {
            var result = new GoodsMovementKAFKAQuery();

            // Reuse the producer (consider making it a singleton)
            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                var produceMessage = new Message<Null, string>
                {
                    Value = jsonString, // Skip deserialization if not needed
                    Headers = new Headers
            {
                new Header(envHeader, Encoding.UTF8.GetBytes(envHeaderEncoding)),
            }
                };

                try
                {
                    var deliveryResult = await producer.ProduceAsync(topic, produceMessage);

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
                    result.Return_Message = $"Failed to produce: {e.Error.Reason}";
                    result.Return_Transactions = $"Failed to produce: {e.Error.Reason}";
                }
            }

            return result;
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
                if (dt == null || dt.Rows.Count == 0)
                    return string.Empty; // or return "<HEADERSAP></HEADERSAP>"

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
                if (dt == null || dt.Rows.Count == 0)
                    return string.Empty; // or return "<BODYSAP></BODYSAP>"

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

        public GoodsMovementKAFKAQueryModel ProducePendingAsset()
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

            GoodsMovementKAFKAQueryModel response = new GoodsMovementKAFKAQueryModel();
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

                    if(!String.IsNullOrEmpty(headerXml) && !String.IsNullOrEmpty(itemsXml))
                    {
                        response = new GoodsMovementKAFKAQueryModel
                        {
                            ret_code_pending_asst = "Success",
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


                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, jsonResult, log, response.ret_code_pending_asst, response.ret_code_pending_asst == "Success" ? "" : response.ret_msg_pending_asst, "");


                        foreach (var item in jsonResult)
                        {
                            InterfaceLogPayGCommand logItem = new InterfaceLogPayGCommand();
                            var jsonObject = JObject.Parse(item);

                            // Access the PartnerMessageID
                            string partnerMessageID = jsonObject["body"]?["PartnerMessageID"]?.ToString();

                            //insert interface log
                            logItem = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, item, partnerMessageID, "Call GoodsMovementKAFKA", "GoodsMovementKAFKAQueryHandler", "", "Fixed Asset", "Exs.");

                            ////insert history log
                            //hLog = StartHistoryLog(_uow, _hisLog, "", "", "IN_FOA", null, "FBB");

                            //List<Dictionary<string, object>> headerList =
                            //    result_get_pending_asset[0] is DataTable dtHeader ? ConvertCursorToList(dtHeader) : new List<Dictionary<string, object>>();

                            //List<Dictionary<string, object>> itemList =
                            //    result_get_pending_asset[1] is DataTable dtItem ? ConvertCursorToList(dtItem) : new List<Dictionary<string, object>>();

                            //var logData = new HistoryLogModel
                            //{
                            //    header = headerList,
                            //    body = itemList,
                            //    ReturnCode = result_get_pending_asset[2]?.ToSafeString() ?? "-1",
                            //};

                            //string jsonLogData = JsonConvert.SerializeObject(logData, Formatting.Indented);

                            //UpdateHistoryLog(_uow, _hisLog, jsonLogData, hLog, partnerMessageID, "INSTALLATION", "", "");

                            //var response_out_foa = new NewRegistForSubmitFOAResponse
                            //{
                            //    result = "Success",
                            //    errorReason = string.Empty
                            //};
                            //UpdateHistoryLog(_uow, _hisLog, response_out_foa, hLog, partnerMessageID, "OUT_FOA", "", "");
                        }


                        response = new GoodsMovementKAFKAQueryModel()
                        {
                            item_json = jsonResult,
                            ret_code_pending_asst = "Success",
                        };

                    }
                    else
                    {
                        response = new GoodsMovementKAFKAQueryModel
                        {
                            ret_code_pending_asst = "Error",
                            ret_msg_pending_asst = "No Header and Item from P_GET_PENDING_ASSET"
                        };
                    }
                    
                }
                else
                {
                    var err_msg = executeResults.ret_msg.ToSafeString() != null
                        ? executeResults.ret_msg.ToSafeString()
                        : "Package P_GET_PENDING_ASSET Data Return Null!";
                    NewRegistForSubmitFOAResponseResult.result = "";
                    response = new GoodsMovementKAFKAQueryModel
                    {
                        ret_code_pending_asst = "Error",
                        ret_msg_pending_asst = err_msg
                    };
                }

            }
            catch (Exception e)
            {
                response = new GoodsMovementKAFKAQueryModel
                {
                    ret_code_pending_asst = "Error",
                    ret_msg_pending_asst = e.Message.ToSafeString()
                };

               InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, response, log, response.ret_code_pending_asst, response.ret_code_pending_asst == "Success" ? "" : response.ret_msg_pending_asst, "");
            }

            return response;
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

        public class HistoryLogModel
        {
            public List<Dictionary<string, object>> header { get; set; }
            public List<Dictionary<string, object>> body { get; set; }
            public string ReturnCode { get; set; }
            public string ReturnMessage { get; set; }
        }


        public class PackageMappingObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public GOODS_MOVEMENT_LISTMapping[] GOODS_MOVEMENT_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModel Null
            {
                get
                {
                    var obj = new PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, GOODS_MOVEMENT_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                GOODS_MOVEMENT_LIST = (GOODS_MOVEMENT_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }


        [OracleCustomTypeMapping("WBB.GOODS_MOVEMENT_REC")]
        public class GOODS_MOVEMENT_LISTMappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new GOODS_MOVEMENT_LISTMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.GOODS_MOVEMENT_LIST")]
        public class GOODS_MOVEMENT_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new GOODS_MOVEMENT_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class GOODS_MOVEMENT_LISTMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping


            [OracleObjectMappingAttribute("TRANS_ID")]
            public string TRANS_ID { get; set; }

            [OracleObjectMappingAttribute("REC_TYPE")]
            public string REC_TYPE { get; set; }

            [OracleObjectMappingAttribute("RUN_GROUP")]
            public string RUN_GROUP { get; set; }

            [OracleObjectMappingAttribute("INTERNET_NO")]
            public string INTERNET_NO { get; set; }

            [OracleObjectMappingAttribute("ORDER_NO")]
            public string ORDER_NO { get; set; }

            [OracleObjectMappingAttribute("COM_CODE")]
            public string COM_CODE { get; set; }

            [OracleObjectMappingAttribute("ASSET_CODE")]
            public string ASSET_CODE { get; set; }

            [OracleObjectMappingAttribute("SUBNUMBER")]
            public string SUBNUMBER { get; set; }

            [OracleObjectMappingAttribute("MATERIAL_NO")]
            public string MATERIAL_NO { get; set; }

            [OracleObjectMappingAttribute("SERIAL_NO")]
            public string SERIAL_NO { get; set; }

            [OracleObjectMappingAttribute("MATERIAL_DOC")]
            public string MATERIAL_DOC { get; set; }

            [OracleObjectMappingAttribute("DOC_YEAR")]
            public string DOC_YEAR { get; set; }

            [OracleObjectMappingAttribute("ERR_STATUS")]
            public string ERR_STATUS { get; set; }

            [OracleObjectMappingAttribute("ERR_CODE")]
            public string ERR_CODE { get; set; }

            [OracleObjectMappingAttribute("ERR_MSG")]
            public string ERR_MSG { get; set; }

            [OracleObjectMappingAttribute("REF_DOC_NO")]
            public string REF_DOC_NO { get; set; }

            #endregion Attribute Mapping

            public static GOODS_MOVEMENT_LISTMapping Null
            {
                get
                {
                    GOODS_MOVEMENT_LISTMapping obj = new GOODS_MOVEMENT_LISTMapping();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, "TRANS_ID", TRANS_ID);
                OracleUdt.SetValue(con, udt, "REC_TYPE", REC_TYPE);
                OracleUdt.SetValue(con, udt, "RUN_GROUP", RUN_GROUP);
                OracleUdt.SetValue(con, udt, "INTERNET_NO", INTERNET_NO);
                OracleUdt.SetValue(con, udt, "ORDER_NO", ORDER_NO);
                OracleUdt.SetValue(con, udt, "COM_CODE", COM_CODE);
                OracleUdt.SetValue(con, udt, "ASSET_CODE", ASSET_CODE);
                OracleUdt.SetValue(con, udt, "SUBNUMBER", SUBNUMBER);
                OracleUdt.SetValue(con, udt, "MATERIAL_NO", MATERIAL_NO);
                OracleUdt.SetValue(con, udt, "SERIAL_NO", SERIAL_NO);
                OracleUdt.SetValue(con, udt, "MATERIAL_DOC", MATERIAL_DOC);
                OracleUdt.SetValue(con, udt, "DOC_YEAR", DOC_YEAR);
                OracleUdt.SetValue(con, udt, "ERR_STATUS", ERR_STATUS);
                OracleUdt.SetValue(con, udt, "ERR_CODE", ERR_CODE);
                OracleUdt.SetValue(con, udt, "ERR_MSG", ERR_MSG);
                OracleUdt.SetValue(con, udt, "REF_DOC_NO", REF_DOC_NO);

            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

    }
}

