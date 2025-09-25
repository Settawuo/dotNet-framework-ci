using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LastmilePayment" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select LastmilePayment.svc or LastmilePayment.svc.cs at the Solution Explorer and start debugging.
    public class LastmilePayment : ILastmilePayment
    {
        private ILogger _logger;
        private readonly ICommandHandler<LastmilePaymentCommand> _LastmilePaymentCommand;
        public LastmilePayment(ILogger logger,
              ICommandHandler<LastmilePaymentCommand> LastmilePaymentCommand
            )
        {
            _logger = logger;
            _LastmilePaymentCommand = LastmilePaymentCommand;
        }

        //public void DoWork() { 

        //}
        public LastmilePaymentResponse LastmilePaymentResponseWcf(LastmilePaymentModel model)
        {
            //   ProfileCustomerCommand command, out int ret_code, out string ret_message
            LastmilePaymentResponse result = new LastmilePaymentResponse();
            try
            {

                _logger.Info("Start WCF LastmilePaymentResponseWcf");

                var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

                if (!isValid)
                {
                    _logger.Info("Error: isValid Validator Model");
                    foreach (ValidationResult message in validationResults)
                    {

                        return new LastmilePaymentResponse()
                        {
                            RESULT_CODE = "-1",
                            RESULT_DESCRIPTION = message.ErrorMessage
                        };
                    }
                }
                if (model.ACCESS_NO != null)
                {
                    if (model.ACCESS_NO.StartsWith("888"))
                    {
                        model.PRODUCT_OWNER = "3BB";
                    }
                    else
                    {
                        model.PRODUCT_OWNER = "FBB";
                    }
                }

                var query = new LastmilePaymentCommand()
                {
                    ACCESS_NO = model.ACCESS_NO,
                    ACTION_DATE = model.ACTION_DATE,
                    BUILDING_TYPE = model.BUILDING_TYPE,
                    DISP_DISTANCE = model.DISP_DISTANCE,
                    ESRI_DISTANCE = model.ESRI_DISTANCE,
                    LAST_UPDATE_BY = model.LAST_UPDATE_BY,
                    LAST_UPDATE_DATE = model.LAST_UPDATE_DATE,
                    MAP_DISTANCE = model.MAP_DISTANCE,
                    ORDER_NO = model.ORDER_NO,
                    ORDER_TYPE = model.ORDER_TYPE,
                    REAL_DISTANCE = model.REAL_DISTANCE,
                    REUSED_FLAG = model.REUSED_FLAG,
                    TRANSACTION_ID = model.TRANSACTION_ID,
                    USER_ID = model.USER_ID,
                    //R19.03
                    REQUEST_DISTANCE = model.REQUEST_DISTANCE,
                    APPROVE_DISTANCE = model.APPROVE_DISTANCE,
                    APPROVE_STAFF = model.APPROVE_STAFF,
                    APPROVE_STATUS = model.APPROVE_STATUS,

                    PRODUCT_OWNER = model.PRODUCT_OWNER

                };
                _LastmilePaymentCommand.Handle(query);
                result.RESULT_CODE = query.RESULT_CODE;
                result.RESULT_DESCRIPTION = query.RESULT_DESCRIPTION;
                //result = startLastmilePaymentCommand<LastmilePaymentQuery>(model);
                _logger.Info("End WCF LastmilePaymentResponseWcf Success");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("Error WCF LastmilePaymentResponseWcf" + ex.Message.ToString());
                result.RESULT_CODE = "-1";
                result.RESULT_DESCRIPTION = "Error : " + ex.Message.ToString();
                return result;

            }

        }

    }
}
