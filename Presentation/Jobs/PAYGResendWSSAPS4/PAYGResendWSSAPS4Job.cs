using System;
using System.Collections.Generic;
using System.Linq;

namespace PAYGResendWSSAPS4
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
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBContract.Queries.WebServices;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.ExWebServiceModels;
    //using CompositionRoot;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using WBBEntity.PanelModels.WebServiceModels;
    using WBBContract.Commands.ExWebServices.SAPFixedAsset;
    using System.Xml.Serialization;
    using System.IO;
    using System.Linq;
    using System.Globalization;

    public class PAYGResendWSSAPS4Job
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogCommand;
        private readonly IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> _queryProcessorGoodsMovementHandler;
        private readonly ICommandHandler<UpdateSubmitFoaErrorLogCommand> _UpdateSubmitFoaError;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }
        public PAYGResendWSSAPS4Job(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateSubmitFoaErrorLogCommand> UpdateSubmitFoaError,

            ICommandHandler<UpdateFbssFOAConfigTblCommand> intfLogCommand
            , IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> queryProcessorGoodsMovementHandle)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _UpdateSubmitFoaError = UpdateSubmitFoaError;
            _queryProcessorGoodsMovementHandler = queryProcessorGoodsMovementHandle;
        }
        public void ResendPendingPAYGResendWSSAPS4()
        {
            try
            {


                var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();
                _logger.Info("Flag_RollbackSAP : " + resultFixAssConfig.DISPLAY_VAL);

                if (resultFixAssConfig.DISPLAY_VAL == "N")
                {

                    var date_start = Get_FBSS_CONFIG_TBL_LOV("PAYG_RESENDWSSAPS4_BATCH", "DATE_START").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                    var date_to = Get_FBSS_CONFIG_TBL_LOV("PAYG_RESENDWSSAPS4_BATCH", "DATE_TO").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                    var date_diff = Get_FBSS_CONFIG_TBL_LOV("PAYG_RESENDWSSAPS4_BATCH", "DATE_DIFF").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();

                    var Check_DateDiff = date_diff.DISPLAY_VAL.ToSafeString() == "Y" ? date_diff.VAL1.ToSafeInteger() : 1;

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
                            c_DATE_TO = date_to.VAL1;
                            DateTime.TryParseExact(date_to.VAL1, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out parsedDateTo);
                        }
                        else
                        {
                            c_DATE_TO = parsedDateTo.ToString("ddMMyyyy");
                        }



                        ///////////////  DATE_START
                        DateTime parsedDateStart;
                        string c_DATE_START = string.Empty;
                        if (date_start.DISPLAY_VAL == "Y")
                        {
                            c_DATE_START = date_start.VAL1;
                        }
                        else
                        {
                            parsedDateStart = parsedDateTo.AddDays(-Check_DateDiff);
                            c_DATE_START = parsedDateStart.ToString("ddMMyyyy");
                        }

                        _logger.Info("DATE_START :: " + c_DATE_START);
                        _logger.Info("DATE_TO :: " + c_DATE_TO);

                        ////////////////// UPDATE DISPLAY_VAL DATE_START เป็นค่า Y
                        var queryUpdateDate = new UpdateFbssFOAConfigTblCommand()
                        {
                            con_type = "PAYG_RESENDWSSAPS4_BATCH",
                            con_name = "DATE_START",
                            display_val = "Y",
                            val1 = c_DATE_START,
                            flag = "EQUIP",
                            updated_by = "ResendWSSAPS4"
                        };
                        _intfLogCommand.Handle(queryUpdateDate);
                        _logger.Info("Update DISPLAY_VAL DATE_START to Y");

                        ////////////////// CALL PKG WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.p_get_error_log_saps4


                        _logger.Info("Call P_GET_ERRIR_LOG_SAPS4");
                        var ResultModel = new List<ResendWssaps4Model>();
                        var query = new ResendSsaps4Query()
                        {
                            p_date_start = c_DATE_START,
                            p_date_to = c_DATE_TO
                        };

                        ResultModel = _queryProcessor.Execute(query);
                        int toalrow = ResultModel.Count;
                        if (ResultModel.Count > 0)
                        {
                            foreach (var d in ResultModel)
                            {
                                toalrow = toalrow - 1;
                                string xml = d.IN_XML_FOA;
                                CallWebservice(xml, toalrow);
                            }
                        }
                        else
                        {
                            _logger.Info("Not Value XML");


                        }


                        ////////////////// UPDATE DISPLAY_VAL DATE_START, DATE_TO เป็นค่า N
                        var updateDateStart_L = new UpdateFbssFOAConfigTblCommand()
                        {
                            con_type = "PAYG_RESENDWSSAPS4_BATCH",
                            con_name = "DATE_START",
                            display_val = "N",
                            val1 = DateTime.Now.ToString("ddMMyyyy"),
                            flag = "EQUIP",
                            updated_by = "ResendWSSAPS4"
                        };
                        _intfLogCommand.Handle(updateDateStart_L);
                        _logger.Info("Update DISPLAY_VAL DATE_START to N");


                        var updateDateTo_L = new UpdateFbssFOAConfigTblCommand()
                        {
                            con_type = "PAYG_RESENDWSSAPS4_BATCH",
                            con_name = "DATE_TO",
                            display_val = "N",
                            val1 = DateTime.Now.ToString("ddMMyyyy"),
                            flag = "EQUIP",
                            updated_by = "ResendWSSAPS4"
                        };
                        _intfLogCommand.Handle(updateDateTo_L);
                        _logger.Info("Update DISPLAY_VAL DATE_TO to N");
                    }
                    catch (Exception ex)
                    {
                        _logger.Info("Exception at PAYGResendWSSAPS4Job ERROR : " + ex.Message);
                    }
                }
                else
                {

                }

 
            }
            catch (Exception ex)
            {
                _logger.Info("Exception at ResendPendingS4 Error: "+ ex.Message);
            }


        }


        private string CallWebservice(string XML, int toalrow)
        {
            string accnolog = "";

            var FoaModel = new NewRegistFOA();
            var DataListForSapServices = new List<NewRegistFOA>();
            FoaModel = MappingXmlToClass(XML);

            DataListForSapServices.Add(FoaModel);
            foreach (var d in DataListForSapServices)
            {
                accnolog = d.Access_No;
            }


            _logger.Info("CallSapServices S4HANA");

            CallSapServicesS4HANA(DataListForSapServices, toalrow);
            

            return accnolog;
        }


        public NewRegistFOA MappingXmlToClass(string ResendFoaXMLText)
        {

            var results = new NewRegistFOAResendLogResendWssaps4();
            var NewRegistFOAResults = new NewRegistFOA();
            XmlSerializer serializer = new XmlSerializer(typeof(NewRegistFOAResendLogResendWssaps4));
            try
            {

                serializer = new XmlSerializer(typeof(NewRegistFOAResendLogResendWssaps4));
                using (TextReader reader = new StringReader(ResendFoaXMLText))
                {
                    results = (NewRegistFOAResendLogResendWssaps4)serializer.Deserialize(reader);
                }
                //----------------call MappingXmlToNewRegistFOAModel
                NewRegistFOAResults = MappingXmlToNewRegistFOAModel(results);
                ResendFoaXMLText = "";

                return NewRegistFOAResults;
            }
            catch (Exception ex)
            {
                _logger.Debug(String.Format("XMLMapping Error" + ex.Message.ToSafeString()));
                return NewRegistFOAResults;
            }


        }

        public NewRegistFOA MappingXmlToNewRegistFOAModel(NewRegistFOAResendLogResendWssaps4 MappingXmlToNewRegistFOA)
        {


            var _main = new NewRegistFOA();
            var _product = new List<NewRegistFOAProductList>();
            var _services = new List<NewRegistFOAServiceList>();

            try
            {
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Access_No)) { MappingXmlToNewRegistFOA.Access_No = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.BUILDING_NAME)) { MappingXmlToNewRegistFOA.BUILDING_NAME = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.FOA_Submit_date)) { MappingXmlToNewRegistFOA.FOA_Submit_date = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Mobile_Contact)) { MappingXmlToNewRegistFOA.Mobile_Contact = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.OLT_NAME)) { MappingXmlToNewRegistFOA.OLT_NAME = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.OrderNumber)) { MappingXmlToNewRegistFOA.OrderNumber = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.OrderType)) { MappingXmlToNewRegistFOA.OrderType = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.ProductName)) { MappingXmlToNewRegistFOA.ProductName = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.RejectReason)) { MappingXmlToNewRegistFOA.RejectReason = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.SubcontractorCode)) { MappingXmlToNewRegistFOA.SubcontractorCode = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.SubcontractorName)) { MappingXmlToNewRegistFOA.SubcontractorName = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Post_Date)) { MappingXmlToNewRegistFOA.Post_Date = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Address_ID)) { MappingXmlToNewRegistFOA.Address_ID = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.ORG_ID)) { MappingXmlToNewRegistFOA.ORG_ID = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Reuse_Flag)) { MappingXmlToNewRegistFOA.Reuse_Flag = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Event_Flow_Flag)) { MappingXmlToNewRegistFOA.Event_Flow_Flag = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.UserName)) { MappingXmlToNewRegistFOA.UserName = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Subcontract_Sub_Type)) { MappingXmlToNewRegistFOA.Subcontract_Sub_Type = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Subcontract_Type)) { MappingXmlToNewRegistFOA.Subcontract_Type = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Request_Sub_Flag)) { MappingXmlToNewRegistFOA.Request_Sub_Flag = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Sub_Access_Mode)) { MappingXmlToNewRegistFOA.Sub_Access_Mode = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Product_Owner)) { MappingXmlToNewRegistFOA.Product_Owner = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Main_Promo_Code)) { MappingXmlToNewRegistFOA.Main_Promo_Code = ""; }
                if (string.IsNullOrEmpty(MappingXmlToNewRegistFOA.Team_ID)) { MappingXmlToNewRegistFOA.Team_ID = ""; }

                _main = new NewRegistFOA()
                {

                    Access_No = MappingXmlToNewRegistFOA.Access_No.ToString(),
                    BUILDING_NAME = MappingXmlToNewRegistFOA.BUILDING_NAME.ToString(),
                    FOA_Submit_date = MappingXmlToNewRegistFOA.FOA_Submit_date.ToString(),
                    Mobile_Contact = MappingXmlToNewRegistFOA.Mobile_Contact.ToString(),
                    OLT_NAME = MappingXmlToNewRegistFOA.OLT_NAME.ToString(),
                    OrderNumber = MappingXmlToNewRegistFOA.OrderNumber.ToString(),
                    OrderType = MappingXmlToNewRegistFOA.OrderType.ToString(),
                    // ProductList = MappingXmlToNewRegistFOA.ProductList,
                    ProductName = MappingXmlToNewRegistFOA.ProductName.ToString(),
                    RejectReason = MappingXmlToNewRegistFOA.RejectReason.ToString(),
                    // ServiceList = MappingXmlToNewRegistFOA.ServiceList,
                    SubcontractorCode = MappingXmlToNewRegistFOA.SubcontractorCode.ToString(),
                    SubcontractorName = MappingXmlToNewRegistFOA.SubcontractorName.ToString(),
                    //SubmitFlag = MappingXmlToNewRegistFOA.SubmitFlag.ToSafeString(),
                    SubmitFlag = "APPROVE",
                    Post_Date = MappingXmlToNewRegistFOA.Post_Date.ToString(),

                    Address_ID = MappingXmlToNewRegistFOA.Address_ID.ToString(),
                    ORG_ID = MappingXmlToNewRegistFOA.ORG_ID.ToString(),

                    Reuse_Flag = MappingXmlToNewRegistFOA.Reuse_Flag.ToString(),
                    Event_Flow_Flag = MappingXmlToNewRegistFOA.Event_Flow_Flag.ToString(),
                    UserName = MappingXmlToNewRegistFOA.UserName.ToString(),
                    Subcontract_Type = MappingXmlToNewRegistFOA.Subcontract_Type.ToString(),
                    Subcontract_Sub_Type = MappingXmlToNewRegistFOA.Subcontract_Sub_Type.ToString(),
                    Request_Sub_Flag = MappingXmlToNewRegistFOA.Request_Sub_Flag.ToString(),
                    Sub_Access_Mode = MappingXmlToNewRegistFOA.Sub_Access_Mode.ToString(),

                    Product_Owner = MappingXmlToNewRegistFOA.Product_Owner.ToString(),
                    Main_Promo_Code = MappingXmlToNewRegistFOA.Main_Promo_Code.ToString(),
                    Team_ID = MappingXmlToNewRegistFOA.Team_ID.ToString(),
                    //  ReturnMessage = MappingXmlToNewRegistFOA.ReturnMessage.ToString(),

                };
                //-------------------------------Add ProducList
                var MappingXmlProducList = MappingXmlToNewRegistFOA.ProductList.FirstOrDefault();
                if (MappingXmlProducList != null)
                {

                    var resultProducListFromXml = MappingXmlProducList.NewRegistFOAProductList.ToList();


                    foreach (var result in resultProducListFromXml)
                    {


                        _logger.Info("Call GETPRODUCTLIST : P_GET_MAPPIMG_SAPS4");

                        //check null
                        if (result.SerialNumber == null) {
                            result.SerialNumber = "";
                        }
                        if (result.MaterialCode == null) { 
                            result.MaterialCode = ""; 
                        }
                        if (result.CompanyCode == null) {
                            result.CompanyCode = ""; 
                        }
                        if (result.Plant == null) {
                            result.Plant = ""; 
                        }
                        if (result.StorageLocation == null) { 
                            result.StorageLocation = "";
                        }
                        if (result.SNPattern == null) { 
                            result.SNPattern = "";
                        }
                        if (result.MovementType == null) { 
                            result.MovementType = "";
                        }


                        var ResultModel = new mappingResendWssapslist4Model();
                        var query = new mappingResendSsaps4Query()
                        {
                     

                            p_plant = result.Plant,
                            p_storage_location = result.StorageLocation,
                            p_material_code = result.MaterialCode
                        };

                     

                        ResultModel = _queryProcessor.Execute(query);

                        if(ResultModel.mappinglistPlant != null)
                        {
                            result.Plant = ResultModel.mappinglistPlant.FirstOrDefault().COL2;
                        }
                        if (ResultModel.mappinglistStorageLocation != null)
                        {
                            result.StorageLocation = ResultModel.mappinglistStorageLocation.FirstOrDefault().COL2;
                        }
                        if (ResultModel.mappinglistMaterialCode != null)
                        {
                            result.MaterialCode = ResultModel.mappinglistMaterialCode.FirstOrDefault().COL2;
                        }


                        _product.Add(new NewRegistFOAProductList()
                        {
                            SerialNumber = result.SerialNumber.ToString(),
                            MaterialCode = result.MaterialCode.ToString(),
                            CompanyCode = result.CompanyCode.ToString(),
                            Plant = result.Plant.ToString(),
                            StorageLocation = result.StorageLocation.ToString(),
                            SNPattern = result.SNPattern.ToString(),
                            MovementType = result.MovementType.ToString()
                        });
                    }





                    _main.ProductList = _product;
                }

                //-------------------------------Add ServicesList
                var MappingXmlServicesList = MappingXmlToNewRegistFOA.ServiceList.FirstOrDefault();
                if (MappingXmlServicesList != null)
                {
                    var resultServicesFromXml = MappingXmlServicesList.NewRegistFOAServiceList.ToList();


                    foreach (var result in resultServicesFromXml)
                    {
                        if (result.ServiceName == null) { result.ServiceName = ""; }
                        _services.Add(new NewRegistFOAServiceList()
                        {
                            ServiceName = result.ServiceName.ToString(),

                        });
                    }

                    _main.ServiceList = _services;
                }

                // Cleare Data MappingXmlToNewRegistFOA
                MappingXmlToNewRegistFOA = new NewRegistFOAResendLogResendWssaps4();
                return _main;
            }
            catch (Exception ex)
            {

                return _main;
            }

        }


        public void CallSapServicesS4HANA(List<NewRegistFOA> DataForSapSerivces, int toalrow)
        {

            try
            {

                if (DataForSapSerivces.Count > 0)
                {

                    foreach (var result in DataForSapSerivces)
                    {
                        try
                        {
                            _logger.Debug(String.Format("Remaining:" + toalrow.ToSafeString() + "||CallSapServicesS4HANA:" + result.Access_No));
                            /// _logger.Debug(String.Format());
                          //  string accnolog = "";
                            var query = new NewRegistForSubmitFOA4HANAQuery()
                            {
                                Access_No = result.Access_No,
                                BUILDING_NAME = result.BUILDING_NAME,
                                FOA_Submit_date = result.FOA_Submit_date,
                                Mobile_Contact = result.Mobile_Contact,
                                OLT_NAME = result.OLT_NAME,
                                OrderNumber = result.OrderNumber,
                                OrderType = result.OrderType,
                                ProductList = result.ProductList,
                                ProductName = result.ProductName,
                                RejectReason = result.RejectReason,
                                ServiceList = result.ServiceList,
                                SubcontractorCode = result.SubcontractorCode,
                                SubcontractorName = result.SubcontractorName,
                                SubmitFlag = "ResendWSSAPS4",
                                Post_Date = result.Post_Date,

                                Address_ID = result.Address_ID,
                                ORG_ID = result.ORG_ID,

                                Reuse_Flag = result.Reuse_Flag,
                                Event_Flow_Flag = result.Event_Flow_Flag,
                                UserName = "FOARESENDBATCH",
                                Subcontract_Type = result.Subcontract_Type,
                                Subcontract_Sub_Type = result.Subcontract_Sub_Type,
                                Request_Sub_Flag = result.Request_Sub_Flag,
                                Sub_Access_Mode = result.Sub_Access_Mode,

                                Product_Owner = result.Product_Owner,
                                Main_Promo_Code = result.Main_Promo_Code,
                                Team_ID = result.Team_ID
                            };

                            var data = _queryProcessor.Execute(query);
                            //  Task.WhenAll(data);



                            var SerialNumberupdate = result.ProductList.FirstOrDefault().SerialNumber;

                            UpdateSubmitFoaErrorLog(result.OrderNumber, result.RejectReason, result.OrderType, SerialNumberupdate, result.Access_No, "ResendWSSAPS4");


                            if (data != null)
                            {
                                string dd = data.result;
                            }


                        }
                        catch (Exception ex)
                        {
                            _logger.Debug(String.Format(ex.Message.ToSafeString() + "AccessNo" + result.Access_No));

                        }

                    }




                }
                else
                {
                    //_Logger.Info(" End Resend Foa   Execute SapServices DATA Not Found. ");
                }
                // Clear data DataForSapSerivces
                DataForSapSerivces = new List<NewRegistFOA>();

            }
            catch (Exception ex)
            {
                // Clear data DataForSapSerivces
                DataForSapSerivces = new List<NewRegistFOA>();
                ///_Logger.Info(" Error Resend Execute SapServices " + ex.GetErrorMessage());

            }

        }




        public void UpdateSubmitFoaErrorLog(string orderno, string rejectreason, string ordertype, string serialno, string accessno, string usename)
        {
     
            var culture = CultureInfo.GetCultureInfo("en-US");

            string resendstatus = "L";
            string  updateby = "ResendWSSAPS4";
            string updateddesc = "Done By Batch PAYGResendWSSAPS4";

            DateTime dt = DateTime.Now;
            string updatedate = dt.ToString("dd/MM/yyyy", culture);
            var update = new UpdateSubmitFoaErrorLogCommand()
            {
                order_no = orderno,
                order_type = ordertype,
                reject_reason = rejectreason,
                serial_no = serialno,
                access_number = accessno,
                resend_status = resendstatus,
                updated_by = updateby,
                updated_desc = updateddesc,
            };
            _UpdateSubmitFoaError.Handle(update);
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
