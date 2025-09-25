
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WebServiceLostTransaction
{
    public class WebserviceLostTransactionJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;

        //  
        public WebserviceLostTransactionJob(
            ILogger logger
            , IQueryProcessor queryProcessor

           )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;


        }
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string Message)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} : {1}", Message, _timer.Elapsed));

        }


        public void JobExcute()
        {
            StartWatching();
            _logger.Debug(String.Format("JobExcute"));

            getByTabelErrorLog();
            //   getLostByAccessNumber();
            //  getLostByCSV();
            StopWatching("Stop WebserviceLostTransactionJob Log");
        }

        public void getByTabelErrorLog()
        {
            try
            {
                string configbatch = "Y"; string configDay = "0";
                var data = GetLovList("LostTrans", "Webservice");
                foreach (var d in data)
                {
                    configbatch = d.Text;
                    _logger.Debug(String.Format("ConfigBatch:" + configbatch));
                    configDay = d.LovValue1;
                    _logger.Debug(String.Format("configDay:" + configDay));
                }
                if (configbatch == "Y")
                {
                    _logger.Debug(String.Format("BatchLostTransactionStart:"));

                    DateTime fixDate = DateTime.Now.AddDays(-int.Parse(configDay)).Date;

                    string _date = fixDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    var ResultModel = new List<LosttranModel>();

                    var query = new LostTranQuery()
                    {
                        Date = _date,
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
                }
                else
                {

                    _logger.Debug(String.Format("FLag != Y : "));
                }
            }
            catch (Exception Ex)
            {
                string msg = Ex.Message.ToSafeString();
                _logger.Debug(String.Format("BatchLostTranError:" + msg));
            }



        }
        public void getLostByCSV()
        {
            string acclog = "";
            _logger.Debug(String.Format("getLostByFile"));
            string XMLDATA = "";
            var result = new List<lostTranQueryResponse>();
            result = readXMLFROMFILE();
            int toalrow = result.Count;
            _logger.Debug(String.Format("TotlaRow:" + result.Count.ToSafeString()));
            if (result.Count > 0)
            {
                foreach (var d in result)
                {
                    toalrow = toalrow - 1;
                    XMLDATA = d.XMLRESULT;
                    string acc = CallWebservice(XMLDATA, toalrow);
                    acclog += "'" + acc + "',";
                }
            }
            _logger.Debug(String.Format(acclog));
        }


        public List<lostTranQueryResponse> readXMLFROMFILE()
        {
            var listdata = new List<lostTranQueryResponse>();
            FileInfo[] listfile = null;



            try
            {
                DirectoryInfo d = new DirectoryInfo(@"C:\LostTransaction");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("LostTrans"); //Getting Text files
                string str = "";

                foreach (FileInfo file in Files)
                {
                    listdata = ReadDatatoFile(file);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listdata;
        }


        public List<lostTranQueryResponse> ReadDatatoFile(FileInfo file)
        {
            DirectoryInfo di = new DirectoryInfo(file.FullName);
            var listdata = new List<lostTranQueryResponse>();
            var data = new lostTranQueryResponse();
            try
            {

                if (File.Exists(Path.Combine(file.FullName)))
                {
                    using (StreamReader sr = new StreamReader(file.FullName))
                    {
                        string textResult = "";
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            textResult += line;



                        }

                        var dataList = textResult.Split('|');
                        foreach (var d in dataList)
                        {
                            if (string.IsNullOrEmpty(d) || d == "")
                            {

                            }
                            else
                            {

                                string aa = d;
                                var ddd = new lostTranQueryResponse
                                {
                                    XMLRESULT = aa,
                                };

                                listdata.Add(ddd);
                            }
                        }



                    }
                }
                else
                {
                    _logger.Info("Not Exists file :" + file.FullName);
                }
            }
            catch (Exception ex)
            {
                string errormessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                _logger.Info("Can't to Readfile :" + file.FullName + " Error :" + errormessage);
            }

            return listdata;


        }




        public void getLostByAccessNumber()
        {
            _logger.Debug(String.Format("getLostByDB"));
            string XMLDATA = string.Empty;
            var query = new lostTranQuery
            {

            };
            var result = _queryProcessor.Execute(query);
            string acclog = "";
            int toalrow = result.Count;
            _logger.Debug(String.Format("TotlaRow:" + result.Count.ToSafeString()));
            if (result.Count > 0)
            {
                foreach (var d in result)
                {
                    toalrow = toalrow - 1;
                    XMLDATA = d.XMLRESULT;
                    string acc = CallWebservice(XMLDATA, toalrow);
                    acclog += "'" + acclog + "',";

                }
            }
            _logger.Debug(String.Format(acclog));


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

            var resultFixAssConfig = GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();
           _logger.Info("Flag_RollbackSAP : " + resultFixAssConfig.DISPLAY_VAL);
            if (resultFixAssConfig.DISPLAY_VAL == "Y")
            {
                CallSapServices(DataListForSapServices, toalrow);

            }
            else
            {
                CallSapServicesS4HANA(DataListForSapServices, toalrow);
            }

            ////ของเดิม  CallSapServices(DataListForSapServices, toalrow);
            return accnolog;
        }
        public void CallSapServices(List<NewRegistFOA> DataForSapSerivces, int toalrow)
        {

            try
            {

                if (DataForSapSerivces.Count > 0)
                {

                    foreach (var result in DataForSapSerivces)
                    {
                        try
                        {
                            _logger.Debug(String.Format("Remaining:" + toalrow.ToSafeString() + "||CallWebservice:" + result.Access_No));
                            /// _logger.Debug(String.Format());
                          //  string accnolog = "";
                            var query = new NewRegistForSubmitFOAQuery()
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
                                SubmitFlag = result.SubmitFlag,
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
                                SubmitFlag = result.SubmitFlag,
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

        public NewRegistFOA MappingXmlToClass(string ResendFoaXMLText)
        {

            var results = new NewRegistFOAResendLog();
            var NewRegistFOAResults = new NewRegistFOA();
            XmlSerializer serializer = new XmlSerializer(typeof(NewRegistFOAResendLog));
            try
            {

                serializer = new XmlSerializer(typeof(NewRegistFOAResendLog));
                using (TextReader reader = new StringReader(ResendFoaXMLText))
                {
                    results = (NewRegistFOAResendLog)serializer.Deserialize(reader);
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
        public NewRegistFOA MappingXmlToNewRegistFOAModel(NewRegistFOAResendLog MappingXmlToNewRegistFOA)
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
                        if (result.SerialNumber == null) { result.SerialNumber = ""; }
                        if (result.MaterialCode == null) { result.MaterialCode = ""; }
                        if (result.CompanyCode == null) { result.CompanyCode = ""; }
                        if (result.Plant == null) { result.Plant = ""; }
                        if (result.StorageLocation == null) { result.StorageLocation = ""; }
                        if (result.SNPattern == null) { result.SNPattern = ""; }
                        if (result.MovementType == null) { result.MovementType = ""; }
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
                MappingXmlToNewRegistFOA = new NewRegistFOAResendLog();
                return _main;
            }
            catch (Exception ex)
            {

                return _main;
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
                string msg = ex.Message.ToString();
                return new List<LovValueModel>();
            }
        }
    }

}
