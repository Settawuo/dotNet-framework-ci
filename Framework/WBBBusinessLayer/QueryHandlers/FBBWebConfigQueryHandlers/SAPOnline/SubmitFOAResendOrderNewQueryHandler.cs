using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
//using OracleCustomTypeUtilities = AIRNETEntity.Extensions.OracleCustomTypeUtilities;
using static WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory.NewRegistForSubmitFOA4HANAQueryHandler;
using static WBBBusinessLayer.QueryHandlers.WebServices.GoodsMovementKAFKAQueryHandler;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBEntity.Models;
using WBBBusinessLayer.FBBSAPOnline1;
using System.Data.Entity.Core.Objects;
using System.Xml.Linq;
using System.Net;
using SaveOptions = System.Xml.Linq.SaveOptions;
using Newtonsoft.Json;
using HistoryLogModel = WBBBusinessLayer.QueryHandlers.WebServices.GoodsMovementKAFKAQueryHandler.HistoryLogModel;
using WBBEntity.PanelModels.WebServiceModels;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
using WBBEntity.PanelModels.ExWebServiceModels;



/* change history
 *ch0001 29/01/2020 --Get data revalue-- validate Accessno  แทนการ validate date from and date to
 */

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{

        //public class SubmitFOAResendOrderNewQueryHandler : IQueryHandler<SubmitFOAResendOrderNewQuery, List<ResendOrderNewHandler>>

    public class SubmitFOAResendOrderNewQueryHandler : IQueryHandler<SubmitFOAResendOrderNewQuery, ResendOrderGetData>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        private readonly IEntityRepository<SubmitFOAInstallationReport> _submitOrderLog;
        private readonly IEntityRepository<ResendOrderNewHandler> _submitInstallLog;
        private readonly IEntityRepository<ResendOrderGetData> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        private readonly IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> _queryProcessorGoodsMovementHandler;

        public SubmitFOAResendOrderNewQueryHandler(
            ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog,
             IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> hisLog,
            IEntityRepository<SubmitFOAInstallationReport> submitOrderLog,
            IEntityRepository<ResendOrderGetData> objService,
             IEntityRepository<FBB_CFG_LOV> cfgLov,
             IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> queryProcessorGoodsMovementHandle,
            IEntityRepository<ResendOrderNewHandler> submitInstallLog)
        {
            _logger = logger;
            _intfLog = intfLog;
            _hisLog = hisLog;
            _submitOrderLog = submitOrderLog;
            _submitInstallLog = submitInstallLog;
            _objService = objService;
            _cfgLov = cfgLov;
                _queryProcessorGoodsMovementHandler = queryProcessorGoodsMovementHandle;
        }

        public ResendOrderGetData Handle(SubmitFOAResendOrderNewQuery query)
        {

            HistoryLogCommand hLog = new HistoryLogCommand();
            ResendOrderGetData result = new ResendOrderGetData();

            try
            {

                var packageMappingObjectModel = new PackageMappingObjectModel
            {
                    RESENDODER_GET_LIST =
                              query.list_p_get_oder.Select(
                                 a => new RESEND_ODER_LISTMapping
                                 {
                                     TRANS_ID = a.trans_id,
                                     SERIAL_NUMBER = a.SerialNumber,
                                     MATERIAL_CODE = a.MaterialCode,
                                     COMPANY_CODE = a.CompanyCode,
                                     PLANT = a.Plant,
                                     STORAGE_LOCATION = a.StorageLocation,
                                 

                                 }).ToArray()
            };


            var RESEND_ODER_DATA_LIST = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_resend_data", "WBB.t_ws_get_resend_order_tab", packageMappingObjectModel);

            var p_resend_data = new OracleParameter();
                p_resend_data.OracleDbType = OracleDbType.RefCursor;
                p_resend_data.Value = RESEND_ODER_DATA_LIST;
                p_resend_data.Direction = ParameterDirection.Input;

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


            var p_ws_header_cur = new OracleParameter();
                p_ws_header_cur.ParameterName = "p_ws_header_cur";
                p_ws_header_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_header_cur.Direction = ParameterDirection.Output;


                var p_ws_item_cur = new OracleParameter();
                p_ws_item_cur.ParameterName = "p_ws_item_cur";
                p_ws_item_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_item_cur.Direction = ParameterDirection.Output;

                var tab_name = new OracleParameter();
                tab_name.ParameterName = "tab_name";
                tab_name.OracleDbType = OracleDbType.Varchar2;
                tab_name.Value = query.tab_name;
                tab_name.Direction = ParameterDirection.Input;



                //GoodsMovementKAFKAQuery response = new GoodsMovementKAFKAQuery();
                ResendOrderGetData responseJson = new ResendOrderGetData();
                PkgFbbFoaOrderManagementResponse executeResults = new PkgFbbFoaOrderManagementResponse();
                NewRegistForSubmitFOAS4HANAResponse newFOAResponse = new NewRegistForSubmitFOAS4HANAResponse();
                NewRegistForSubmitFOAS4HANAResponse NewRegistForSubmitFOAResponseResult = new NewRegistForSubmitFOAS4HANAResponse();
                InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
                log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, "", "", "call package : p_get_data_resend_order", "PKG_FBBPAYG_FOA_RESEND_ORDER", "", "", "");
                List<string> jsonResult = new List<string>();
                GoodsMovementKAFKAQuery response = new GoodsMovementKAFKAQuery();
                var result_resend_pending = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.p_get_data_resend_order",
                    new object[]
                    {
                        RESEND_ODER_DATA_LIST,
                        tab_name,
                        p_ws_header_cur,
                        p_ws_item_cur,
                        ret_code,
                        ret_msg

                    });


                //var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.p_get_data_resend_order",
                //        new object[]
                //        {
                //                                RESEND_ODER_DATA_LIST,
                //                                tab_name = tab_name,
                //                                p_ws_header_cur,
                //                                p_ws_item_cur,
                //                                ret_code,
                //                                ret_msg
                                                
                //        });

            DataTable resp = new DataTable();



                if (result_resend_pending != null)
                {
                    executeResults.ret_code = result_resend_pending[2] != null ? result_resend_pending[2].ToSafeString() : "-1";
                    executeResults.ret_msg = result_resend_pending[3] != null ? result_resend_pending[3].ToSafeString() : "";

                    NewRegistForSubmitFOAResponseResult.result = ret_code.Value.ToSafeString();

                    NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToSafeString();
                    var converterDataTable = new DataTableToXmlConverter();

                    string headerXml = result_resend_pending[0] is DataTable dt ? converterDataTable.ConvertDataTableToCustomHeaderXml(dt) : "";
                    string itemsXml = result_resend_pending[1] is DataTable dt2 ? converterDataTable.ConvertDataTableToCustomBodyXml(dt2) : "";

                    responseJson = new ResendOrderGetData
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
                    responseJson = new ResendOrderGetData
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

                        //insert interface log
                        logItem = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, item, partnerMessageID, "Call GoodsMovementKAFKA", "SubmitFOAResendOrderNewQueryHandler", "", "Fixed Asset", "Exs.");

                    }


                    var queryGoodsMovementKAFKA = new GoodsMovementKAFKAQuery()
                    {
                        action = "PRODUCER",
                        item_json = jsonResult
                    };
                 var responseGoodsMovementHandler =  _queryProcessorGoodsMovementHandler.Handle(queryGoodsMovementKAFKA);



                    result = new ResendOrderGetData
                    {
                        ret_code = responseGoodsMovementHandler.ret_code
          
                    };

                }
                else
                {
                    result = new ResendOrderGetData
                    {
                        ret_code = "-1",
                        ret_msg = "Package p_get_data_resend_order Data Return Null!"
                    };
                }

                return result;


            }
            catch (Exception ex)
            {
                _logger.Info("p_get_data_resend_order : Error.");

                result.ret_code = "-1";
                result.ret_msg = "Error call save event service " + ex.Message;
                return result;

            }

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
                                dict[col.ColumnName] = xml.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
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

        //public List<Dictionary<string, object>> ConvertCursorToList(DataTable cursor)
        //{
        //    var list = new List<Dictionary<string, object>>();

        //    foreach (DataRow row in cursor.Rows)
        //    {
        //        var dict = new Dictionary<string, object>();

        //        foreach (DataColumn col in cursor.Columns)
        //        {
        //            object value = row[col];

        //            if (value is string strValue)
        //            {
        //                string decodedValue = WebUtility.HtmlDecode(strValue);

        //                try
        //                {
        //                    var xml = XElement.Parse(decodedValue);

        //                    if (col.ColumnName.Equals("SerialNumber", StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        dict[col.ColumnName] = xml;
        //                    }
        //                    else
        //                    {
        //                        dict[col.ColumnName] = xml.ToString(SaveOptions.DisableFormatting);
        //                    }
        //                }
        //                catch
        //                {
        //                    dict[col.ColumnName] = decodedValue;
        //                }
        //            }
        //            else
        //            {
        //                dict[col.ColumnName] = value;
        //            }
        //        }

        //        list.Add(dict);
        //    }

        //    return list;
        //}


        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public RESEND_ODER_LISTMapping[] RESENDODER_GET_LIST { get; set; }

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
                OracleUdt.SetValue(con, udt, 0, RESENDODER_GET_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                RESENDODER_GET_LIST = (RESEND_ODER_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("WBB.t_ws_get_resend_order_rec")]
        public class RESEND_ODER_LISTMappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new RESEND_ODER_LISTMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.t_ws_get_resend_order_tab")]
        public class RESEND_ODER_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new RESEND_ODER_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class RESEND_ODER_LISTMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping


            [OracleObjectMapping("SERIAL_NUMBER")]
            public string SERIAL_NUMBER { get; set; }

            [OracleObjectMapping("MATERIAL_CODE")]
            public string MATERIAL_CODE { get; set; }

            [OracleObjectMapping("COMPANY_CODE")]
            public string COMPANY_CODE { get; set; }

            [OracleObjectMapping("PLANT")]
            public string PLANT { get; set; }

            [OracleObjectMapping("STORAGE_LOCATION")]
            public string STORAGE_LOCATION { get; set; }

            [OracleObjectMapping("TRANS_ID")]
            public string TRANS_ID { get; set; }

            #endregion Attribute Mapping

            public static RESEND_ODER_LISTMapping Null
            {
                get
                {
                    var obj = new RESEND_ODER_LISTMapping();
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
                OracleUdt.SetValue(con, udt, "SERIAL_NUMBER", SERIAL_NUMBER);
                OracleUdt.SetValue(con, udt, "MATERIAL_CODE", MATERIAL_CODE);
                OracleUdt.SetValue(con, udt, "COMPANY_CODE", COMPANY_CODE);
                OracleUdt.SetValue(con, udt, "PLANT", PLANT);
                OracleUdt.SetValue(con, udt, "STORAGE_LOCATION", STORAGE_LOCATION);
                OracleUdt.SetValue(con, udt, "TRANS_ID", TRANS_ID);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }


        }
    }
}




