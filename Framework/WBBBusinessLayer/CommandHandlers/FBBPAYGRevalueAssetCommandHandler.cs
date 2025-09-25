using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract.Commands;
using WBBContract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace WBBBusinessLayer.CommandHandlers
{
    public class FBBPAYGRevalueAssetCommandHandler : ICommandHandler<FBBPAYGRevalueAssetCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPAYGRevalueAssetGetDateTBL> _objService;
        private readonly IEntityRepository<object> _objServiceRe;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _interfaceLog;

        public FBBPAYGRevalueAssetCommandHandler(ILogger logger, IEntityRepository<FBBPAYGRevalueAssetGetDateTBL> objService, IEntityRepository<object> objServiceRe
            , IWBBUnitOfWork uow
            , IEntityRepository<FBB_INTERFACE_LOG_PAYG> interfaceLog)
        {
            _logger = logger;
            _objService = objService;
            _objServiceRe = objServiceRe;
            _uow = uow;
            _interfaceLog = interfaceLog;
        }
        public void Handle(FBBPAYGRevalueAssetCommand command)
        {
            _logger.Info("FBBPAYGRevalueAssetCommandHandler START");
            InterfaceLogPayGCommand log = null;
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _interfaceLog, "WBB.PKG_FBBPAYG_PURGE_DATA.p_get_config_data", ""
                , "FBBPAYGRevalueAssetCommand", "FBBPAYGRevalueAssetCommandHandler", "", "FBB", "Batch FBBPAYG_RevalueAssetBatch");
            try
            {
                FBBPAYGRevalueAssetCommand ReturnPurges = new FBBPAYGRevalueAssetCommand();

                var out_ref_cur = new OracleParameter();
                out_ref_cur.ParameterName = "out_ref_cur";
                out_ref_cur.OracleDbType = OracleDbType.RefCursor;
                out_ref_cur.Direction = ParameterDirection.Input;




            }
            catch (Exception ex)
            {
                _logger.Info("CALL ERROR MSG : " + ex.Message);
            }


        }


        public JObject JsontoSAP()
        {
            string headerXml = @"<HEADERSAP>
                                    <ITEM>
                                    <MessageID></MessageID>
                                    <PartnerName>PAYG</PartnerName>
                                    <PartnerMessageID>2024092301413800007444444</PartnerMessageID>
                                    <FlagTypeHeader>C</FlagTypeHeader>
                                    <DocumentDate>2024-09-22</DocumentDate>
                                    <PostingDate>2024-09-22</PostingDate>
                                    <ReferenceDocumentNumber>R2409004247399</ReferenceDocumentNumber>
                                    <BillofLading></BillofLading>
                                    <GRGISlipNo>8805657724</GRGISlipNo>
                                    <HeaderText></HeaderText>
                                    <MaterialDocument></MaterialDocument>
                                    <MaterialDocumentYear></MaterialDocumentYear>
                                    <CompanyCode>1200</CompanyCode>
                                    <MovementType>241</MovementType>
                                    </ITEM>
                                    <ITEM>
                                    <MessageID></MessageID>
                                    <PartnerName>PAYG</PartnerName>
                                    <PartnerMessageID>2024092301413800007555555</PartnerMessageID>
                                    <FlagTypeHeader>C</FlagTypeHeader>
                                    <DocumentDate>2024-09-22</DocumentDate>
                                    <PostingDate>2024-09-22</PostingDate>
                                    <ReferenceDocumentNumber>R2409004247399</ReferenceDocumentNumber>
                                    <BillofLading></BillofLading>
                                    <GRGISlipNo>8805657724</GRGISlipNo>
                                    <HeaderText></HeaderText>
                                    <MaterialDocument></MaterialDocument>
                                    <MaterialDocumentYear></MaterialDocumentYear>
                                    <CompanyCode>1200</CompanyCode>
                                    <MovementType>251</MovementType>
                                    </ITEM>
                                    <ITEM>
                                    <MessageID></MessageID>
                                    <PartnerName>PAYG</PartnerName>
                                    <PartnerMessageID>2024092301413800007666666</PartnerMessageID>
                                    <FlagTypeHeader>C</FlagTypeHeader>
                                    <DocumentDate>2024-09-22</DocumentDate>
                                    <PostingDate>2024-09-22</PostingDate>
                                    <ReferenceDocumentNumber>R2409004247399</ReferenceDocumentNumber>
                                    <BillofLading></BillofLading>
                                    <GRGISlipNo>8805657724</GRGISlipNo>
                                    <HeaderText></HeaderText>
                                    <MaterialDocument></MaterialDocument>
                                    <MaterialDocumentYear></MaterialDocumentYear>
                                    <CompanyCode>1800</CompanyCode>
                                    <MovementType>251</MovementType>
                                    </ITEM>
                                </HEADERSAP>";
            string itemsXml = @"<ITEMSAP>
                                    <ITEM>
                                        <Item>1</Item>
                                        <FlagTypeItem>A</FlagTypeItem>
                                        <Material>11033762</Material>
                                        <Plant>1204</Plant>
                                        <StorageLocation>6332</StorageLocation>
                                        <Batch></Batch>
                                        <MovementType>241</MovementType>
                                        <StockType></StockType>
                                        <SpecialStock></SpecialStock>
                                        <Vendor></Vendor>
                                        <Customer></Customer>
                                        <SalesOrder></SalesOrder>
                                        <SalesOrderItem></SalesOrderItem>
                                        <Quantity>2</Quantity>
                                        <UnitofMeasurement></UnitofMeasurement>
                                        <PurchaseOrder></PurchaseOrder>
                                        <PurchaserOrderItem></PurchaserOrderItem>
                                        <DeliveryCompletedIndicator></DeliveryCompletedIndicator>
                                        <ItemText>FTTH</ItemText>
                                        <GoodsRecipient></GoodsRecipient>
                                        <UnloadingPoint></UnloadingPoint>
                                        <CostCenter>12030002</CostCenter>
                                        <MainAssetNumber></MainAssetNumber>
                                        <AssetSubnumber></AssetSubnumber>
                                        <Reservation></Reservation>
                                        <ReservationItem></ReservationItem>
                                        <FinalIssueforReservation></FinalIssueforReservation>
                                        <ReceivingIssuingMaterial></ReceivingIssuingMaterial>
                                        <Receivingplantissuingplant></Receivingplantissuingplant>
                                        <Receivingissuingstoragelocation></Receivingissuingstoragelocation>
                                        <ReceivingIssuingBatch></ReceivingIssuingBatch>
                                        <ReasonforMovement></ReasonforMovement>
                                        <AmountinLocalCurrency></AmountinLocalCurrency>
                                        <ShelfLifeExpirationorBestBeforeDate></ShelfLifeExpirationorBestBeforeDate>
                                        <DateofManufacture></DateofManufacture>
                                        <WBSElement></WBSElement>
                                        <GLAccountNumber></GLAccountNumber>
                                        <Delivery></Delivery>
                                        <DeliveryItem></DeliveryItem>
                                        <CompanyCode>1200</CompanyCode>
                                        <AssetClass>7200</AssetClass>
                                        <AssetDescription1>Wifi router</AssetDescription1>
                                        <AssetDescription2>R2409004247399</AssetDescription2>
                                        <EvaluationGroup4>DN14</EvaluationGroup4>
                                        <EvaluationGroup5>NWWFI004</EvaluationGroup5>
                                        <AssetSuperNumber>NWFBB013</AssetSuperNumber>
                                        <Area01_Usefullife></Area01_Usefullife>
                                        <Area01_Period></Area01_Period>
                                        <Area01_Scrap></Area01_Scrap>
                                        <Area01_ScrapAmount></Area01_ScrapAmount>
                                        <Area15_Usefullife></Area15_Usefullife>
                                        <Area15_Period></Area15_Period>
                                        <Area15_Scrap></Area15_Scrap>
                                        <Area15_ScrapAmount></Area15_ScrapAmount>
                                        <ReferencePO></ReferencePO>
                                        <ReferencePOItem></ReferencePOItem>
                                        <AmountforAsset></AmountforAsset>
                                        <AssetMainGroup></AssetMainGroup>
                                        <SequenceNumber></SequenceNumber>
                                        <ReferenceKey1>6100004286</ReferenceKey1>
                                        <ReferenceKey2>บจ.เดอะเบสท์ อินเตอร์เน็ตเวิร์ค (สระบุรี)#TheBest Internetwork (Saraburi)</ReferenceKey2>
                                        <ReferenceKey3>FTTH</ReferenceKey3>
                                        <Serial>
                                            <SerialItem>
                                                <Item>1</Item>
                                                <SerialNumber>ZTEGD7223BD4</SerialNumber>
                                            </SerialItem>
                                            <SerialItem>
                                                <Item>2</Item>
                                                <SerialNumber>ZTEGD7223BD5</SerialNumber>
                                            </SerialItem>
                                        </Serial>
                                    </ITEM>
                                    <ITEM>
                                        <Item>2</Item>
                                        <FlagTypeItem>A</FlagTypeItem>
                                        <Material>11038298</Material>
                                        <Plant>1204</Plant>
                                        <StorageLocation>2010</StorageLocation>
                                        <Batch></Batch>
                                        <MovementType>241</MovementType>
                                        <StockType></StockType>
                                        <SpecialStock></SpecialStock>
                                        <Vendor></Vendor>
                                        <Customer></Customer>
                                        <SalesOrder></SalesOrder>
                                        <SalesOrderItem></SalesOrderItem>
                                        <Quantity>1</Quantity>
                                        <UnitofMeasurement></UnitofMeasurement>
                                        <PurchaseOrder></PurchaseOrder>
                                        <PurchaserOrderItem></PurchaserOrderItem>
                                        <DeliveryCompletedIndicator></DeliveryCompletedIndicator>
                                        <ItemText>FTTH</ItemText>
                                        <GoodsRecipient></GoodsRecipient>
                                        <UnloadingPoint></UnloadingPoint>
                                        <CostCenter>12030002</CostCenter>
                                        <MainAssetNumber></MainAssetNumber>
                                        <AssetSubnumber></AssetSubnumber>
                                        <Reservation></Reservation>
                                        <ReservationItem></ReservationItem>
                                        <FinalIssueforReservation></FinalIssueforReservation>
                                        <ReceivingIssuingMaterial></ReceivingIssuingMaterial>
                                        <Receivingplantissuingplant></Receivingplantissuingplant>
                                        <Receivingissuingstoragelocation></Receivingissuingstoragelocation>
                                        <ReceivingIssuingBatch></ReceivingIssuingBatch>
                                        <ReasonforMovement></ReasonforMovement>
                                        <AmountinLocalCurrency></AmountinLocalCurrency>
                                        <ShelfLifeExpirationorBestBeforeDate></ShelfLifeExpirationorBestBeforeDate>
                                        <DateofManufacture></DateofManufacture>
                                        <WBSElement></WBSElement>
                                        <GLAccountNumber></GLAccountNumber>
                                        <Delivery></Delivery>
                                        <DeliveryItem></DeliveryItem>
                                        <CompanyCode>1200</CompanyCode>
                                        <AssetClass>7200</AssetClass>
                                        <AssetDescription1>Wifi router</AssetDescription1>
                                        <AssetDescription2>R2409004247399</AssetDescription2>
                                        <EvaluationGroup4>DN14</EvaluationGroup4>
                                        <EvaluationGroup5>NWWFI004</EvaluationGroup5>
                                        <AssetSuperNumber>NWFBB013</AssetSuperNumber>
                                        <Area01_Usefullife></Area01_Usefullife>
                                        <Area01_Period></Area01_Period>
                                        <Area01_Scrap></Area01_Scrap>
                                        <Area01_ScrapAmount></Area01_ScrapAmount>
                                        <Area15_Usefullife></Area15_Usefullife>
                                        <Area15_Period></Area15_Period>
                                        <Area15_Scrap></Area15_Scrap>
                                        <Area15_ScrapAmount></Area15_ScrapAmount>
                                        <ReferencePO></ReferencePO>
                                        <ReferencePOItem></ReferencePOItem>
                                        <AmountforAsset></AmountforAsset>
                                        <AssetMainGroup></AssetMainGroup>
                                        <SequenceNumber></SequenceNumber>
                                        <ReferenceKey1>6100004286</ReferenceKey1>
                                        <ReferenceKey2>บจ.เดอะเบสท์ อินเตอร์เน็ตเวิร์ค (สระบุรี)#TheBest Internetwork (Saraburi)</ReferenceKey2>
                                        <ReferenceKey3>FTTH</ReferenceKey3>
                                        <Serial>
                                            <SerialItem>
                                                <Item>1</Item>
                                                <SerialNumber>ZTEGD7223BD4D</SerialNumber>
                                            </SerialItem>
                                        </Serial>
                                    </ITEM>
                                    <ITEM>
                                        <Item>3</Item>
                                        <FlagTypeItem>C</FlagTypeItem>
                                        <Material>11036462</Material>
                                        <Plant>1204</Plant>
                                        <StorageLocation>6332</StorageLocation>
                                        <Batch></Batch>
                                        <MovementType>251</MovementType>
                                        <StockType></StockType>
                                        <SpecialStock></SpecialStock>
                                        <Vendor></Vendor>
                                        <Customer></Customer>
                                        <SalesOrder></SalesOrder>
                                        <SalesOrderItem></SalesOrderItem>
                                        <Quantity>1</Quantity>
                                        <UnitofMeasurement></UnitofMeasurement>
                                        <PurchaseOrder></PurchaseOrder>
                                        <PurchaserOrderItem></PurchaserOrderItem>
                                        <DeliveryCompletedIndicator></DeliveryCompletedIndicator>
                                        <ItemText>R2409004247399</ItemText>
                                        <GoodsRecipient></GoodsRecipient>
                                        <UnloadingPoint></UnloadingPoint>
                                        <CostCenter></CostCenter>
                                        <MainAssetNumber></MainAssetNumber>
                                        <AssetSubnumber></AssetSubnumber>
                                        <Reservation></Reservation>
                                        <ReservationItem></ReservationItem>
                                        <FinalIssueforReservation></FinalIssueforReservation>
                                        <ReceivingIssuingMaterial></ReceivingIssuingMaterial>
                                        <Receivingplantissuingplant></Receivingplantissuingplant>
                                        <Receivingissuingstoragelocation></Receivingissuingstoragelocation>
                                        <ReceivingIssuingBatch></ReceivingIssuingBatch>
                                        <ReasonforMovement></ReasonforMovement>
                                        <AmountinLocalCurrency></AmountinLocalCurrency>
                                        <ShelfLifeExpirationorBestBeforeDate></ShelfLifeExpirationorBestBeforeDate>
                                        <DateofManufacture></DateofManufacture>
                                        <WBSElement></WBSElement>
                                        <GLAccountNumber></GLAccountNumber>
                                        <Delivery></Delivery>
                                        <DeliveryItem></DeliveryItem>
                                        <CompanyCode></CompanyCode>
                                        <AssetClass></AssetClass>
                                        <AssetDescription1></AssetDescription1>
                                        <AssetDescription2></AssetDescription2>
                                        <EvaluationGroup4></EvaluationGroup4>
                                        <EvaluationGroup5></EvaluationGroup5>
                                        <AssetSuperNumber></AssetSuperNumber>
                                        <Area01_Usefullife></Area01_Usefullife>
                                        <Area01_Period></Area01_Period>
                                        <Area01_Scrap></Area01_Scrap>
                                        <Area01_ScrapAmount></Area01_ScrapAmount>
                                        <Area15_Usefullife></Area15_Usefullife>
                                        <Area15_Period></Area15_Period>
                                        <Area15_Scrap></Area15_Scrap>
                                        <Area15_ScrapAmount></Area15_ScrapAmount>
                                        <ReferencePO></ReferencePO>
                                        <ReferencePOItem></ReferencePOItem>
                                        <AmountforAsset></AmountforAsset>
                                        <AssetMainGroup></AssetMainGroup>
                                        <SequenceNumber></SequenceNumber>
                                        <ReferenceKey1>6100004286</ReferenceKey1>
                                        <ReferenceKey2>บจ.เดอะเบสท์ อินเตอร์เน็ตเวิร์ค (สระบุรี)#TheBest Internetwork (Saraburi)</ReferenceKey2>
                                        <ReferenceKey3>FTTH</ReferenceKey3>
                                        <Serial>
                                            <SerialItem>
                                                <Item>1</Item>
                                                <SerialNumber>ZTEGD7223BDA</SerialNumber>
                                            </SerialItem>
                                        </Serial>
                                    </ITEM>
                                    <ITEM>
                                            <Item>4</Item>
                                            <FlagTypeItem>C</FlagTypeItem>
                                            <Material>11036462</Material>
                                            <Plant>1204</Plant>
                                            <StorageLocation>6332</StorageLocation>
                                            <Batch></Batch>
                                            <MovementType>251</MovementType>
                                            <StockType></StockType>
                                            <SpecialStock></SpecialStock>
                                            <Vendor></Vendor>
                                            <Customer></Customer>
                                            <SalesOrder></SalesOrder>
                                            <SalesOrderItem></SalesOrderItem>
                                            <Quantity>1</Quantity>
                                            <UnitofMeasurement></UnitofMeasurement>
                                            <PurchaseOrder></PurchaseOrder>
                                            <PurchaserOrderItem></PurchaserOrderItem>
                                            <DeliveryCompletedIndicator></DeliveryCompletedIndicator>
                                            <ItemText>R2409004247399</ItemText>
                                            <GoodsRecipient></GoodsRecipient>
                                            <UnloadingPoint></UnloadingPoint>
                                            <CostCenter></CostCenter>
                                            <MainAssetNumber></MainAssetNumber>
                                            <AssetSubnumber></AssetSubnumber>
                                            <Reservation></Reservation>
                                            <ReservationItem></ReservationItem>
                                            <FinalIssueforReservation></FinalIssueforReservation>
                                            <ReceivingIssuingMaterial></ReceivingIssuingMaterial>
                                            <Receivingplantissuingplant></Receivingplantissuingplant>
                                            <Receivingissuingstoragelocation></Receivingissuingstoragelocation>
                                            <ReceivingIssuingBatch></ReceivingIssuingBatch>
                                            <ReasonforMovement></ReasonforMovement>
                                            <AmountinLocalCurrency></AmountinLocalCurrency>
                                            <ShelfLifeExpirationorBestBeforeDate></ShelfLifeExpirationorBestBeforeDate>
                                            <DateofManufacture></DateofManufacture>
                                            <WBSElement></WBSElement>
                                            <GLAccountNumber></GLAccountNumber>
                                            <Delivery></Delivery>
                                            <DeliveryItem></DeliveryItem>
                                            <CompanyCode>1800</CompanyCode>
                                            <AssetClass></AssetClass>
                                            <AssetDescription1></AssetDescription1>
                                            <AssetDescription2></AssetDescription2>
                                            <EvaluationGroup4></EvaluationGroup4>
                                            <EvaluationGroup5></EvaluationGroup5>
                                            <AssetSuperNumber></AssetSuperNumber>
                                            <Area01_Usefullife></Area01_Usefullife>
                                            <Area01_Period></Area01_Period>
                                            <Area01_Scrap></Area01_Scrap>
                                            <Area01_ScrapAmount></Area01_ScrapAmount>
                                            <Area15_Usefullife></Area15_Usefullife>
                                            <Area15_Period></Area15_Period>
                                            <Area15_Scrap></Area15_Scrap>
                                            <Area15_ScrapAmount></Area15_ScrapAmount>
                                            <ReferencePO></ReferencePO>
                                            <ReferencePOItem></ReferencePOItem>
                                            <AmountforAsset></AmountforAsset>
                                            <AssetMainGroup></AssetMainGroup>
                                            <SequenceNumber></SequenceNumber>
                                            <ReferenceKey1>6100004286</ReferenceKey1>
                                            <ReferenceKey2>บจ.เดอะเบสท์ อินเตอร์เน็ตเวิร์ค (สระบุรี)#TheBest Internetwork (Saraburi)</ReferenceKey2>
                                            <ReferenceKey3>FTTH</ReferenceKey3>
                                            <Serial>
                                                <SerialItem>
                                                    <Item>1</Item>
                                                    <SerialNumber>ZTEGD7223BDA</SerialNumber>
                                                </SerialItem>
                                            </Serial>
                                        </ITEM>
                                    </ITEMSAP>
                                    ";

            XDocument headerDoc = XDocument.Parse(headerXml);
            XDocument itemsDoc = XDocument.Parse(itemsXml);
            var headers = headerDoc.Root.Elements("ITEM");
            var items = itemsDoc.Root.Elements("ITEM");

            List<string> jsonStrings = new List<string>();
            var enrichedHeader = new JObject();
            foreach (var header in headers)
            {
                string companyCode = header.Element("CompanyCode")?.Value;
                string movementType = header.Element("MovementType")?.Value;

                var matchingItems = items.Where(i =>
                    i.Element("CompanyCode")?.Value == companyCode && i.Element("MovementType")?.Value == movementType ||
                    (string.IsNullOrEmpty(i.Element("CompanyCode")?.Value) && companyCode == "1200" && i.Element("MovementType")?.Value == movementType)
                ).ToList();

                var headerObj = new JObject();
                //var PartnerMessageID = "";
                foreach (var element in header.Elements())
                {
                    //if (element.Name.LocalName == "PartnerMessageID")
                    //{
                    //    PartnerMessageID = element.Value;
                    //}
                    //// Skip adding CompanyCode and MovementType to the headerObj
                    //if (element.Name.LocalName != "CompanyCode" && element.Name.LocalName != "MovementType")
                    //{
                    //    headerObj[element.Name.LocalName] = element.Value;
                    //}
                    headerObj[element.Name.LocalName] = element.Value;
                }

                var itemArray = new JArray();
                foreach (var item in matchingItems)
                {
                    var itemObj = new JObject();
                    foreach (var itemElement in item.Elements())
                    {
                        //if (itemElement.Name.LocalName == "Serial")
                        //{
                        //    var serialArray = new JArray();
                        //    foreach (var serialItem in itemElement.Elements("SerialItem"))
                        //    {
                        //        var serialObj = new JObject();
                        //        foreach (var serialElement in serialItem.Elements())
                        //        {
                        //            serialObj[serialElement.Name.LocalName] = serialElement.Value;
                        //        }
                        //        serialArray.Add(serialObj);
                        //    }
                        //    itemObj[itemElement.Name.LocalName] = serialArray;
                        //}
                        //else
                        //{
                        //    itemObj[itemElement.Name.LocalName] = itemElement.Value;
                        //}
                        itemObj[itemElement.Name.LocalName] = itemElement.Value;
                    }
                    itemArray.Add(itemObj);
                }

                var headerKAFKA = new JObject();
                var body = new JObject(headerObj);
                body["Item"] = itemArray;


                string dateTimeString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                //var ESB_KAFKA_HEADER = from item in _cfgLov.Get()
                //                       where item.LOV_TYPE == "ESB_KAFKA_HEADER" && item.ACTIVEFLAG == "Y"
                //                       orderby item.LOV_ID ascending
                //                       select item;

                //foreach (var item in ESB_KAFKA_HEADER)
                //{
                //    if (item.LOV_NAME == "groupTags")
                //    {
                //        if (item.LOV_VAL1 != null)
                //        {
                //            string jsonContent = "[" + item.LOV_VAL1 + "]";
                //            headerKAFKA[item.LOV_NAME] = JArray.Parse(jsonContent);
                //        }
                //        else
                //        {
                //            headerKAFKA[item.LOV_NAME] = new JArray { };
                //        }
                //    }
                //    else if (item.LOV_NAME == "identity")
                //    {
                //        if (item.LOV_VAL1 != null)
                //        {
                //            string jsonContent = "{" + item.LOV_VAL1 + "}";
                //            headerKAFKA[item.LOV_NAME] = JObject.Parse(jsonContent);
                //        }
                //        else
                //        {
                //            headerKAFKA[item.LOV_NAME] = new JObject { };
                //        }
                //    }
                //    else
                //    {

                //        headerKAFKA[item.LOV_NAME] = item.LOV_VAL1 != null ? item.LOV_VAL1 : string.Empty;
                //    }
                //}

                //headerKAFKA["session"] = PartnerMessageID;
                //headerKAFKA["transaction"] = PartnerMessageID;
                //headerKAFKA["timestamp"] = PartnerMessageID;
                //headerKAFKA["useCaseExpiryTime"] = dateTimeString;
                //headerKAFKA["useCaseStartTime"] = dateTimeString;


                enrichedHeader = new JObject
                {
                    //["header"] = headerKAFKA,
                    ["body"] = body
                };


                //var enrichedHeader = new JObject
                //{
                //    ["header"] = new JObject
                //    {
                //        ["agent"] = "",
                //        ["baseApiVersion"] = "none",
                //        ["broker"] = "",
                //        ["channel"] = "PayG",
                //        ["communication"] = "unicast",
                //        ["from"] = "PayG",
                //        ["groupTags"] = new JArray(),
                //        ["identity"] = new JObject(),
                //        ["messageType"] = "command",
                //        ["functionName"] = "GoodsMovement",
                //        ["orgService"] = "",
                //        ["schemaVersion"] = "none",
                //        ["scope"] = "global",
                //        ["session"] = "2024091620145800000000059",
                //        ["timestamp"] = "2024-10-22T10:15:11.418Z",
                //        ["tmfSpec"] = "none",
                //        ["transaction"] = "2024091620145800000000059",
                //        ["useCase"] = "",
                //        ["useCaseAge"] = "",
                //        ["useCaseExpiryTime"] = "2024-10-22T10:15:11.418Z",
                //        ["useCaseStartTime"] = "2024-10-22T10:15:11.418Z",
                //        ["useCaseStep"] = "",
                //        ["version"] = "5.0"
                //    },
                //    ["body"] = body
                //};



                jsonStrings.Add(enrichedHeader.ToString());
            }
            
            return enrichedHeader;
        }

    }
}
