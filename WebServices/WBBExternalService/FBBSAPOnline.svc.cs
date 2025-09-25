using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.ExWebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBContract.Queries.FBBWebConfigQueries;
using System.Linq;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FBBSAPOnline" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FBBSAPOnline.svc or FBBSAPOnline.svc.cs at the Solution Explorer and start debugging.
    public class FBBSAPOnline : IFBBSAPOnline
    {
        private ILogger _logger;
        private ProcessorAsync _Qobj;

        //private IQueryProcessorAsync _queryProcessorAsync;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLogPayGCommand;
        private readonly ICommandHandler<SumitFOAErrorLogCommand> _SumitFOAErrorLogCommand;
        private readonly IQueryProcessor _queryFixedAssetConfig;



        public FBBSAPOnline(ILogger logger,
            IQueryProcessor queryProcessor,
            IQueryProcessorAsync queryProcessorAsync,
            ICommandHandler<InterfaceLogPayGCommand> intfLogPayGCommand,
             ICommandHandler<SumitFOAErrorLogCommand> SumitFOAErrorLogCommand,
           IQueryProcessor queryFixedAssetConfig)
        {
            _logger = logger;
            _Qobj = new ProcessorAsync
            {
                queryProcessor = queryProcessor,
                queryProcessorAsync = queryProcessorAsync
            };
            _intfLogPayGCommand = intfLogPayGCommand;
            _SumitFOAErrorLogCommand = SumitFOAErrorLogCommand;
            _queryFixedAssetConfig = queryFixedAssetConfig;
        }

        public NewRegistForSubmitFOAResponse NewRegistForSubmitFOA(NewRegistForSubmitFOAQuery model)
        {
            InterfaceLogPayGCommand log = null;
            log = StartInterfacePayG<NewRegistForSubmitFOAQuery>(model, "", "FOA Send", "NewRegistForSubmitFOA", model.Access_No, model.OrderType, "Exs.");

            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (ValidationResult message in validationResults)
                {
                    return new NewRegistForSubmitFOAResponse() { result = "-1", errorReason = message.ErrorMessage };
                }
            }
            insertErrorLog(model);
            //NewRegistForSubmitFOAQuery query = new NewRegistForSubmitFOAQuery()
            //{
            //    Access_No = model.Access_No,
            //    OrderNumber = model.OrderNumber,
            //    SubcontractorCode = model.SubcontractorCode,
            //    SubcontractorName = model.SubcontractorName,
            //    ProductName = model.ProductName,
            //    ServiceName = model.ServiceName,
            //    OrderType = model.OrderType,
            //    ProductList = Constants.ToDataTable(model.ProductList),
            //    submitFlag = model.submitFlag,
            //};

            //_newRegistForSubmitFOACommand.Handle(model);

            //return null;

            try
            {
                var query = new GetFixedAssetConfigQuery()
                {
                    Program = "Flag_RollbackSAP"
                };
                var _FbssConfig = _queryFixedAssetConfig.Execute(query).FirstOrDefault();

                if(_FbssConfig.DISPLAY_VAL =="Y") {

                    //Task.Delay(5000);
                    // Run async process
                    var task = _Qobj.queryProcessorAsync.ExecuteAsync(model);

                    var awaiter = task.GetAwaiter(); //หน้าที่รอผลแทนเรา 
                    awaiter.OnCompleted(() =>
                    {

                        var result = awaiter.GetResult();
                        _logger.Info("FOA Send Result:" + result);

                    });

                    // Task.WhenAll(task);

                }
                else if(_FbssConfig.DISPLAY_VAL == "N")
                {
                    var hanaQuery = ConvertToHANAQuery(model);

                    // Execute the HANA query
                    var hanaResponse = _Qobj.queryProcessorAsync.ExecuteAsync(hanaQuery);

                    var awaiter = hanaResponse.GetAwaiter(); //หน้าที่รอผลแทนเรา 
                    awaiter.OnCompleted(() =>
                    {

                        var result = awaiter.GetResult();
                        _logger.Info("FOA Send Result:" + result);

                    });

                    // Task.WhenAll(task);
                }

            }
            catch (Exception Ex)
            {
                string msg = Ex.Message.ToSafeString();

                _logger.Info("NewRegistForSubmitFOAQuery Error:" + msg);


                //  log = StartInterfacePayG<NewRegistForSubmitFOAQuery>(model, "", "FOA Error", "NewRegistForSubmitFOA", model.Access_No, model.OrderType, msg);


            }



            var response = new NewRegistForSubmitFOAResponse
            {
                result = "Success",
                errorReason = string.Empty
            };

            EndInterfacePayG<NewRegistForSubmitFOAResponse>(response, log, "Success", "NewRegistForSubmitFOAResponse", "Exs.");
            return response;
        }

        private NewRegistForSubmitFOA4HANAQuery ConvertToHANAQuery(NewRegistForSubmitFOAQuery model)
        {
            var hanaQuery = new NewRegistForSubmitFOA4HANAQuery();

            // Get properties of both types
            var sourceProperties = typeof(NewRegistForSubmitFOAQuery).GetProperties();
            var targetProperties = typeof(NewRegistForSubmitFOA4HANAQuery).GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                // Find a matching property in the target type
                var targetProperty = targetProperties.FirstOrDefault(tp => tp.Name == sourceProperty.Name && tp.PropertyType == sourceProperty.PropertyType);

                if (targetProperty != null && sourceProperty.GetValue(model) != null)
                {
                    // Set the value in the target object
                    targetProperty.SetValue(hanaQuery, sourceProperty.GetValue(model));
                }
            }

            return hanaQuery;
        }

        public NewRegisterResponse NewRegistOrder(NewRegisterQuery query)
        {
            return _Qobj.queryProcessor.Execute(query);
        }
        public JoinOrderResponse JoinOrder(JoinOrderQuery query)
        {
            return _Qobj.queryProcessor.Execute(query);
        }
        public InstallationCostResponse InstallationCost(InstallationCostQuery query)
        {
            return _Qobj.queryProcessor.Execute(query);
        }
        public TerminateServiceResponse TerminateService(TerminateServiceQuery query)
        {
            return _Qobj.queryProcessor.Execute(query);
        }
        public RenewServiceResponse RenewService(RenewServiceQuery query)
        {
            return _Qobj.queryProcessor.Execute(query);
        }

        private InterfaceLogPayGCommand StartInterfacePayG<T>(T query, string transactionId, string methodName, string serviceName, string InIDCardNO, string InterfaceNode, string createBy)
        {
            var dbIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_ID_CARD_NO = InIDCardNO,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = InterfaceNode,
                CREATED_BY = createBy,
            };

            _intfLogPayGCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterfacePayG<T>(T output, InterfaceLogPayGCommand dbIntfCmd, string result, string reason, string updateBy)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogPayGCommand.Handle(dbIntfCmd);
        }
        private void insertErrorLog(NewRegistForSubmitFOAQuery model)
        {
            string dd = DateTime.Now.ToShortDateString();

            var error = new SumitFOAErrorLogCommand
            {
                access_number = model.Access_No,
                in_xml_foa = model.DumpToXml(),
                resend_status = "",
                created_by = "WebServiceFOA",
                created_date = DateTime.Now.ToShortDateString(),
                updated_by = "WebServiceFOA",
                updated_date = DateTime.Now.ToShortDateString(),
                p_return_msg = "-"


            };

            _SumitFOAErrorLogCommand.Handle(error);


        }
    }
}
