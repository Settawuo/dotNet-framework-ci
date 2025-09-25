using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{

    public class ResendFoaController : FBBConfigController
    {
        //
        // GET: /ResendFoa/
        private readonly IQueryProcessor _queryProcessor;
        private IQueryProcessorAsync _queryProcessorAsync;
        private readonly ICommandHandler<ResendFoaCommand> _UpdateResendFoaCommand;

        public ResendFoaController(ILogger logger, IQueryProcessorAsync queryProcessorAsync
            , ICommandHandler<ResendFoaCommand> UpdateResendFoaCommand, IQueryProcessor queryProcessor


          )
        {
            _Logger = logger;
            _queryProcessorAsync = queryProcessorAsync;
            _UpdateResendFoaCommand = UpdateResendFoaCommand;
            _queryProcessor = queryProcessor;


        }
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            return View();
        }


        public NewRegistFOA MappingXmlToNewRegistFOAModel(NewRegistFOAResendLog MappingXmlToNewRegistFOA, bool InstallationCostFlag = false)
        {


            var _main = new NewRegistFOA();
            var _product = new List<NewRegistFOAProductList>();
            var _services = new List<NewRegistFOAServiceList>();

            try
            {



                _main = new NewRegistFOA()
                {
                    Access_No = MappingXmlToNewRegistFOA.Access_No.ToSafeString(),
                    BUILDING_NAME = MappingXmlToNewRegistFOA.BUILDING_NAME.ToSafeString(),
                    FOA_Submit_date = MappingXmlToNewRegistFOA.FOA_Submit_date.ToSafeString(),
                    Mobile_Contact = MappingXmlToNewRegistFOA.Mobile_Contact.ToSafeString(),
                    OLT_NAME = MappingXmlToNewRegistFOA.OLT_NAME.ToSafeString(),
                    OrderNumber = MappingXmlToNewRegistFOA.OrderNumber.ToSafeString(),
                    OrderType = MappingXmlToNewRegistFOA.OrderType.ToSafeString(),
                    // ProductList = MappingXmlToNewRegistFOA.ProductList,
                    ProductName = MappingXmlToNewRegistFOA.ProductName.ToSafeString(),
                    RejectReason = MappingXmlToNewRegistFOA.RejectReason.ToSafeString(),
                    // ServiceList = MappingXmlToNewRegistFOA.ServiceList,
                    SubcontractorCode = MappingXmlToNewRegistFOA.SubcontractorCode.ToSafeString(),
                    SubcontractorName = MappingXmlToNewRegistFOA.SubcontractorName.ToSafeString(),
                    //SubmitFlag = MappingXmlToNewRegistFOA.SubmitFlag.ToSafeString(),
                    SubmitFlag = "WEB_RESEND_FOA",
                    Post_Date = MappingXmlToNewRegistFOA.Post_Date.ToSafeString(),

                    Address_ID = MappingXmlToNewRegistFOA.Address_ID.ToSafeString(),
                    ORG_ID = MappingXmlToNewRegistFOA.ORG_ID.ToSafeString(),

                    Reuse_Flag = MappingXmlToNewRegistFOA.Reuse_Flag.ToSafeString(),
                    Event_Flow_Flag = MappingXmlToNewRegistFOA.Event_Flow_Flag.ToSafeString(),
                    UserName = MappingXmlToNewRegistFOA.UserName.ToSafeString(),
                    Subcontract_Type = MappingXmlToNewRegistFOA.Subcontract_Type.ToSafeString(),
                    Subcontract_Sub_Type = MappingXmlToNewRegistFOA.Subcontract_Sub_Type.ToSafeString(),
                    Request_Sub_Flag = MappingXmlToNewRegistFOA.Request_Sub_Flag.ToSafeString(),
                    Product_Owner = MappingXmlToNewRegistFOA.Product_Owner.ToSafeString(),
                    Main_Promo_Code = MappingXmlToNewRegistFOA.Main_Promo_Code.ToSafeString(),
                    Team_ID = MappingXmlToNewRegistFOA.Team_ID.ToSafeString(),
                    Sub_Access_Mode = MappingXmlToNewRegistFOA.Sub_Access_Mode.ToSafeString(),
                    ReturnMessage = MappingXmlToNewRegistFOA.ReturnMessage.ToSafeString(),

                };
                //-------------------------------Add ProducList
                var MappingXmlProducList = MappingXmlToNewRegistFOA.ProductList.FirstOrDefault();
                if (MappingXmlProducList != null)
                {
                    var resultProducListFromXml = MappingXmlProducList.NewRegistFOAProductList.ToList();

                    foreach (var result in resultProducListFromXml)
                    {
                        _product.Add(new NewRegistFOAProductList()
                        {
                            SerialNumber = result.SerialNumber.ToSafeString(),
                            MaterialCode = result.MaterialCode.ToSafeString(),
                            CompanyCode = result.CompanyCode.ToSafeString(),
                            Plant = result.Plant.ToSafeString(),
                            StorageLocation = result.StorageLocation.ToSafeString(),
                            SNPattern = result.SNPattern.ToSafeString(),
                            MovementType = result.MovementType.ToSafeString()
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
                        _services.Add(new NewRegistFOAServiceList()
                        {
                            ServiceName = result.ServiceName.ToSafeString(),

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
                _Logger.Info("ERROR MappingXml To NewRegistFOAModel For SAP:" + ex.GetErrorMessage());

                // --------------------new object----Cleare Data
                _main = new NewRegistFOA();
                _product = new List<NewRegistFOAProductList>();
                _services = new List<NewRegistFOAServiceList>();

                MappingXmlToNewRegistFOA = new NewRegistFOAResendLog();
                // ------------------end--new object----Cleare Data
                _main.ReturnMessage = "Invalid XML Format.";

                return _main;
            }

        }
        public NewRegistFOA MappingXmlToNewRegistFOAModelNew(NewRegistFOAResendLog MappingXmlToNewRegistFOA, bool InstallationCostFlag = false)
        {


            var _main = new NewRegistFOA();
            var _product = new List<NewRegistFOAProductList>();
            var _services = new List<NewRegistFOAServiceList>();
            var submitFlag = InstallationCostFlag ? "WEB_RESEND_FOA" : "WEB_RESEND_FOA_EQUIPMENT";

            try
            {



                _main = new NewRegistFOA()
                {
                    Access_No = MappingXmlToNewRegistFOA.Access_No.ToSafeString(),
                    BUILDING_NAME = MappingXmlToNewRegistFOA.BUILDING_NAME.ToSafeString(),
                    FOA_Submit_date = MappingXmlToNewRegistFOA.FOA_Submit_date.ToSafeString(),
                    Mobile_Contact = MappingXmlToNewRegistFOA.Mobile_Contact.ToSafeString(),
                    OLT_NAME = MappingXmlToNewRegistFOA.OLT_NAME.ToSafeString(),
                    OrderNumber = MappingXmlToNewRegistFOA.OrderNumber.ToSafeString(),
                    OrderType = MappingXmlToNewRegistFOA.OrderType.ToSafeString(),
                    // ProductList = MappingXmlToNewRegistFOA.ProductList,
                    ProductName = MappingXmlToNewRegistFOA.ProductName.ToSafeString(),
                    RejectReason = MappingXmlToNewRegistFOA.RejectReason.ToSafeString(),
                    // ServiceList = MappingXmlToNewRegistFOA.ServiceList,
                    SubcontractorCode = MappingXmlToNewRegistFOA.SubcontractorCode.ToSafeString(),
                    SubcontractorName = MappingXmlToNewRegistFOA.SubcontractorName.ToSafeString(),
                    //SubmitFlag = MappingXmlToNewRegistFOA.SubmitFlag.ToSafeString(),
                    //SubmitFlag = "WEB_RESEND_FOA",
                    SubmitFlag = submitFlag,
                    Post_Date = MappingXmlToNewRegistFOA.Post_Date.ToSafeString(),

                    Address_ID = MappingXmlToNewRegistFOA.Address_ID.ToSafeString(),
                    ORG_ID = MappingXmlToNewRegistFOA.ORG_ID.ToSafeString(),

                    Reuse_Flag = MappingXmlToNewRegistFOA.Reuse_Flag.ToSafeString(),
                    Event_Flow_Flag = MappingXmlToNewRegistFOA.Event_Flow_Flag.ToSafeString(),
                    UserName = MappingXmlToNewRegistFOA.UserName.ToSafeString(),
                    Subcontract_Type = MappingXmlToNewRegistFOA.Subcontract_Type.ToSafeString(),
                    Subcontract_Sub_Type = MappingXmlToNewRegistFOA.Subcontract_Sub_Type.ToSafeString(),
                    Request_Sub_Flag = MappingXmlToNewRegistFOA.Request_Sub_Flag.ToSafeString(),
                    Product_Owner = MappingXmlToNewRegistFOA.Product_Owner.ToSafeString(),
                    Main_Promo_Code = MappingXmlToNewRegistFOA.Main_Promo_Code.ToSafeString(),
                    Team_ID = MappingXmlToNewRegistFOA.Team_ID.ToSafeString(),
                    Sub_Access_Mode = MappingXmlToNewRegistFOA.Sub_Access_Mode.ToSafeString(),
                    ReturnMessage = MappingXmlToNewRegistFOA.ReturnMessage.ToSafeString(),

                };
                //-------------------------------Add ProducList
                var MappingXmlProducList = MappingXmlToNewRegistFOA.ProductList.FirstOrDefault();
                if (MappingXmlProducList != null)
                {
                    var resultProducListFromXml = MappingXmlProducList.NewRegistFOAProductList.ToList();

                    foreach (var result in resultProducListFromXml)
                    {
                        _product.Add(new NewRegistFOAProductList()
                        {
                            SerialNumber = result.SerialNumber.ToSafeString(),
                            MaterialCode = result.MaterialCode.ToSafeString(),
                            CompanyCode = result.CompanyCode.ToSafeString(),
                            Plant = result.Plant.ToSafeString(),
                            StorageLocation = result.StorageLocation.ToSafeString(),
                            SNPattern = result.SNPattern.ToSafeString(),
                            MovementType = result.MovementType.ToSafeString()
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
                        _services.Add(new NewRegistFOAServiceList()
                        {
                            ServiceName = result.ServiceName.ToSafeString(),

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
                _Logger.Info("ERROR MappingXml To NewRegistFOAModel For SAP:" + ex.GetErrorMessage());

                // --------------------new object----Cleare Data
                _main = new NewRegistFOA();
                _product = new List<NewRegistFOAProductList>();
                _services = new List<NewRegistFOAServiceList>();

                MappingXmlToNewRegistFOA = new NewRegistFOAResendLog();
                // ------------------end--new object----Cleare Data
                _main.ReturnMessage = "Invalid XML Format.";

                return _main;
            }

        }
        public NewRegistFOA MappingXmlToClass(string ResendFoaXMLText = "", bool InstallationCostFlag = false)
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
                NewRegistFOAResults = MappingXmlToNewRegistFOAModel(results, InstallationCostFlag);
                ResendFoaXMLText = "";

                return NewRegistFOAResults;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error Resend Foa MappingXml To Class XML To Model :" + ex.GetErrorMessage());
                // --------------------new object----Cleare Data
                results = new NewRegistFOAResendLog();
                NewRegistFOAResults = new NewRegistFOA();
                ResendFoaXMLText = "";
                serializer = new XmlSerializer(typeof(NewRegistFOAResendLog));
                // ------------------End--new object----Cleare Data
                NewRegistFOAResults.ReturnMessage = "Invalid XML Format.";

                return NewRegistFOAResults;
            }


        }        
        
        public NewRegistFOA MappingXmlToClassNew(string ResendFoaXMLText = "", bool InstallationCostFlag = false)
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
                NewRegistFOAResults = MappingXmlToNewRegistFOAModelNew(results, InstallationCostFlag);
                ResendFoaXMLText = "";

                return NewRegistFOAResults;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error Resend Foa MappingXml To Class XML To Model :" + ex.GetErrorMessage());
                // --------------------new object----Cleare Data
                results = new NewRegistFOAResendLog();
                NewRegistFOAResults = new NewRegistFOA();
                ResendFoaXMLText = "";
                serializer = new XmlSerializer(typeof(NewRegistFOAResendLog));
                // ------------------End--new object----Cleare Data
                NewRegistFOAResults.ReturnMessage = "Invalid XML Format.";

                return NewRegistFOAResults;
            }


        }

        public JsonResult GetDataResendFOALog()
        {
            var query = new ResendFoaQuery();
            var ResultModel = new List<ResendFoaModel>();
            try
            {
                query = new ResendFoaQuery() { };

                ResultModel = _queryProcessor.Execute(query);
                if (ResultModel.Count > 0)
                {

                    return Json(
                               new
                               {
                                   Code = 0,
                                   Dataresponse = ResultModel,
                               }, JsonRequestBehavior.AllowGet
                       );

                }
                else
                {
                    ResultModel = new List<ResendFoaModel>();
                    return Json(
                              new
                              {
                                  Code = 1,
                                  Dataresponse = ResultModel
                              }, JsonRequestBehavior.AllowGet
                      );
                }

            }
            catch (Exception ex)
            {
                //-----------------Clear Data 
                ResultModel = new List<ResendFoaModel>();
                query = new ResendFoaQuery();
                //  ResultModel = new List<ResendFoaModel>();
                _Logger.Info("Error ResendFoaController Get Data Resend FOALog :" + ex.GetErrorMessage());
                return Json(
                             new
                             {
                                 Code = 1,
                                 Dataresponse = ResultModel
                             }, JsonRequestBehavior.AllowGet
                     );
            }
        }
        public JsonResult ResendFoaConfirm(string ResendFoaXMLText = "", bool InstallationCostFlag = false)
        {
            string check = "1";
            string ReturnMessage = "";
            var FoaModelList = new NewRegistFOA();
            var DataListForSapServices = new List<NewRegistFOA>();
            var _commanStrList = new ResendFoaCommand();
            var _foa_access_listAndStatus = new List<FBB_foa_access_list>();
            try
            {
                var query = new GetFixedAssetConfigQuery()
                {
                    Program = "Flag_RollbackSAP"
                };
                var _FbssConfig = _queryProcessor.Execute(query).FirstOrDefault();
                if (null == base.CurrentUser)
                {

                    return Json(
                                new
                                {
                                    Code = -2,
                                    message = "Resend FOA Session Expired."
                                }, JsonRequestBehavior.AllowGet);
                }
                if (_FbssConfig.DISPLAY_VAL == "Y")
                {
                    FoaModelList = MappingXmlToClass(ResendFoaXMLText, InstallationCostFlag);
                }
                else
                {
                    //ตัวใหม่
                    FoaModelList = MappingXmlToClassNew(ResendFoaXMLText, InstallationCostFlag);
                }

                ReturnMessage = FoaModelList.ReturnMessage;

                if (string.IsNullOrEmpty(ReturnMessage))
                {
                    check = "0";
                }

                if (check == "0")
                {

                    //------------------------Add List Data  For SAP DataListForSapServices 
                    DataListForSapServices.Add(FoaModelList);

                    //CallSapServices(DataListForSapServices);


                    if (_FbssConfig.DISPLAY_VAL == "Y")
                    {
                        CallSapServices(DataListForSapServices);
                    }
                    else
                    {
                        //ตัวใหม่
                        CallSapS4HANAServices(DataListForSapServices);
                    }
                }
                else
                {


                    return Json(
                        new
                        {
                            Code = 1,
                            message = ReturnMessage
                        }, JsonRequestBehavior.AllowGet);
                }
                // Cleare Object  

                FoaModelList = new NewRegistFOA();
                DataListForSapServices = new List<NewRegistFOA>();
                _commanStrList = new ResendFoaCommand();
                _foa_access_listAndStatus = new List<FBB_foa_access_list>();


                // end if check status ==0
                return Json(
                     new
                     {
                         Code = check,
                         message = ReturnMessage
                     }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                //----------------Clear Data
                check = "";
                ReturnMessage = "";
                // CheckStatusResendFoaBool = false;
                FoaModelList = new NewRegistFOA();
                DataListForSapServices = new List<NewRegistFOA>();
                _commanStrList = new ResendFoaCommand();
                _foa_access_listAndStatus = new List<FBB_foa_access_list>();
                //---------End-------Clear Data
                _Logger.Info("Error Resend Foa Method ResendFoaConfirm :" + e.GetErrorMessage());

                return Json(
                         new
                         {
                             Code = 1,
                             message = "Resend FOA Not Success."
                         }
                        , JsonRequestBehavior.AllowGet);

            }
            // catch

        }
        // All ResendFoaConfirmALL
        public JsonResult ResendFoaConfirmALL(string ResendFoaXMLText = "", string Access_No_text = "", bool InstallationCostFlag = false)
        {

            string checkUpdateResend_status = "";
            string ReturnMessage = "";
            string StatusResend = "";

            var FoaModelList = new NewRegistFOA();
            var DataListForSapServices = new List<NewRegistFOA>();
            var _commanStrList = new ResendFoaCommand();
            var _foa_access_listAndStatus = new List<FBB_foa_access_list>();

            int TotalResend = 0, ErrorResend = 0;

            try
            {
                if (null == base.CurrentUser)
                {
                    _Logger.Info("Error Resend FOA Session Expired.");
                    return Json(
                         new
                         {
                             Code = -2,
                             TotalResend = 0,
                             ErrorResend = 0,
                             message = "Resend FOA Session Expired.",
                         }, JsonRequestBehavior.AllowGet);
                }

                var query = new GetFixedAssetConfigQuery()
                {
                    Program = "Flag_RollbackSAP"
                };
                var _FbssConfig = _queryProcessor.Execute(query).FirstOrDefault();


                //  FoaModelList = new NewRegistFOA(); 
                if (_FbssConfig.DISPLAY_VAL == "Y")
                {
                    FoaModelList = MappingXmlToClass(ResendFoaXMLText, InstallationCostFlag);
                }
                else
                {
                    //ตัวใหม่
                    FoaModelList = MappingXmlToClassNew(ResendFoaXMLText, InstallationCostFlag);
                }

                //   FoaModelList = ConvertStringToXml(ResendFoaXMLText);
                ReturnMessage = FoaModelList.ReturnMessage;
                // ReturnMessage = "Error";

                if (string.IsNullOrEmpty(ReturnMessage))
                {

                    StatusResend = "Y";
                    TotalResend = 1;
                    // Add  For Call SAP
                    DataListForSapServices.Add(FoaModelList);
                }
                else
                {
                    StatusResend = "E";
                    ErrorResend = 1;
                }


                _foa_access_listAndStatus.Add(new FBB_foa_access_list()
                {
                    ACCESS_NUMBER = Access_No_text.ToSafeString(),
                    RESEND_STATUS = StatusResend.ToSafeString(),
                });

                _commanStrList.p_FBB_foa_access_list = _foa_access_listAndStatus;

                var commanStr = new ResendFoaCommand()
                {
                    p_FBB_foa_access_list = _commanStrList.p_FBB_foa_access_list,
                    UPDATED_BY = base.CurrentUser.UserName.ToSafeString(),
                };
                _UpdateResendFoaCommand.Handle(commanStr);
                checkUpdateResend_status = commanStr.ret_code;


                // end    ResendFoaCommand

                if (checkUpdateResend_status == "1")
                {
                    // Cleare DataListForSapServices 
                    DataListForSapServices = new List<NewRegistFOA>();

                    if (string.IsNullOrEmpty(ReturnMessage))
                    {
                        ReturnMessage = "Update Resend FOA Status Not Success.";
                    }
                    // Cleare Object  

                    FoaModelList = new NewRegistFOA();
                    DataListForSapServices = new List<NewRegistFOA>();
                    _commanStrList = new ResendFoaCommand();
                    _foa_access_listAndStatus = new List<FBB_foa_access_list>();

                    return Json(
                        new
                        {
                            Code = 1,
                            TotalResend = TotalResend,
                            ErrorResend = ErrorResend,
                            message = ReturnMessage
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    //------------------------Add List Data  For SAP DataListForSapServices 

                    if (StatusResend == "Y")
                    {

                        if (_FbssConfig.DISPLAY_VAL == "Y")
                        {
                            CallSapServices(DataListForSapServices);
                        }
                        else
                        {
                            //ตัวใหม่
                            CallSapS4HANAServices(DataListForSapServices);
                        }

                        // Cleare Object  

                        FoaModelList = new NewRegistFOA();
                        DataListForSapServices = new List<NewRegistFOA>();
                        _commanStrList = new ResendFoaCommand();
                        _foa_access_listAndStatus = new List<FBB_foa_access_list>();

                        return Json(
                            new
                            {
                                Code = 0,
                                TotalResend = TotalResend,
                                ErrorResend = ErrorResend,
                                message = ReturnMessage
                            }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        // Cleare Object  

                        FoaModelList = new NewRegistFOA();
                        DataListForSapServices = new List<NewRegistFOA>();
                        _commanStrList = new ResendFoaCommand();
                        _foa_access_listAndStatus = new List<FBB_foa_access_list>();
                        Access_No_text = "";

                        return Json(
                                 new
                                 {
                                     Code = 1,
                                     TotalResend = TotalResend,
                                     ErrorResend = ErrorResend,
                                     message = ReturnMessage
                                 }, JsonRequestBehavior.AllowGet);

                    }

                }
                // end if check status ==0 

            }
            catch (Exception e)
            {
                //----------------Clear Data

                checkUpdateResend_status = "";
                ReturnMessage = "";
                StatusResend = "";
                Access_No_text = "";
                FoaModelList = new NewRegistFOA();
                DataListForSapServices = new List<NewRegistFOA>();
                _commanStrList = new ResendFoaCommand();
                _foa_access_listAndStatus = new List<FBB_foa_access_list>();
                //---------End-------Clear Data
                _Logger.Info("Error Resend Foa Method ResendFoaConfirm :" + e.GetErrorMessage());

                return Json(
                         new
                         {
                             Code = 1,
                             TotalResend = TotalResend,
                             ErrorResend = ErrorResend,
                             message = "Resend FOA Not Success."
                         }
                        , JsonRequestBehavior.AllowGet);

            }

        }
        public void CallSapServices(List<NewRegistFOA> DataForSapSerivces)
        {

            try
            {

                if (DataForSapSerivces.Count > 0)
                {

                    foreach (var result in DataForSapSerivces)
                    {
                        try
                        {
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
                                UserName = result.UserName,
                                Subcontract_Type = result.Subcontract_Type,
                                Subcontract_Sub_Type = result.Subcontract_Sub_Type,
                                Request_Sub_Flag = result.Request_Sub_Flag,
                                Product_Owner = result.Product_Owner,
                                Main_Promo_Code = result.Main_Promo_Code,
                                Team_ID = result.Team_ID,
                                Sub_Access_Mode = result.Sub_Access_Mode,
                            };

                            var data = _queryProcessor.Execute(query);
                            //  Task.WhenAll(data);

                        }
                        catch (Exception ex)
                        {
                            //--------Clear  query 
                            _Logger.Info(" ERROR Resend Foa ConfirmALL Method   Execute SapServices " + ex.GetErrorMessage());
                        }

                    }

                }
                else
                {
                    _Logger.Info(" End Resend Foa   Execute SapServices DATA Not Found. ");
                }
                // Clear data DataForSapSerivces
                DataForSapSerivces = new List<NewRegistFOA>();

            }
            catch (Exception ex)
            {
                // Clear data DataForSapSerivces
                DataForSapSerivces = new List<NewRegistFOA>();
                _Logger.Info(" Error Resend Execute SapServices " + ex.GetErrorMessage());

            }

        }
        
        public void CallSapS4HANAServices(List<NewRegistFOA> DataForSapSerivces)
        {

            try
            {

                if (DataForSapSerivces.Count > 0)
                {

                    foreach (var result in DataForSapSerivces)
                    {
                        try
                        {
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
                                UserName = result.UserName,
                                Subcontract_Type = result.Subcontract_Type,
                                Subcontract_Sub_Type = result.Subcontract_Sub_Type,
                                Request_Sub_Flag = result.Request_Sub_Flag,
                                Product_Owner = result.Product_Owner,
                                Main_Promo_Code = result.Main_Promo_Code,
                                Team_ID = result.Team_ID,
                                Sub_Access_Mode = result.Sub_Access_Mode,
                            };

                            var data = _queryProcessor.Execute(query);
                            //  Task.WhenAll(data);

                        }
                        catch (Exception ex)
                        {
                            //--------Clear  query 
                            _Logger.Info(" ERROR Resend Foa S4HANA ConfirmALL Method   Execute SapServices " + ex.GetErrorMessage());
                        }

                    }

                }
                else
                {
                    _Logger.Info(" End Resend Foa S4HANA  Execute SapServices DATA Not Found. ");
                }
                // Clear data DataForSapSerivces
                DataForSapSerivces = new List<NewRegistFOA>();

            }
            catch (Exception ex)
            {
                // Clear data DataForSapSerivces
                DataForSapSerivces = new List<NewRegistFOA>();
                _Logger.Info(" Error Resend S4HANA Execute SapServices " + ex.GetErrorMessage());

            }

        }
    }

}
