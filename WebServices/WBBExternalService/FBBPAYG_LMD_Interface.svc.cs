using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbPaygLMD;
using WBBEntity.Extensions;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FBBPAYG_LMD_Interface" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FBBPAYG_LMD_Interface.svc or FBBPAYG_LMD_Interface.svc.cs at the Solution Explorer and start debugging.
    public class FBBPAYG_LMD_Interface : IFBBPAYG_LMD_Interface
    {
        private ILogger _logger;
        private IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLogCommand;
        private readonly ICommandHandler<UpdateInvoiceCommand> _updateInvoiceCommand;
        private readonly ICommandHandler<UpdateOrderStatusCommand> _updateOrderStatusCommand;
        public FBBPAYG_LMD_Interface(ILogger logger,
           IQueryProcessor queryProcessor,
           ICommandHandler<InterfaceLogPayGCommand> intfLogCommand,
            ICommandHandler<UpdateInvoiceCommand> updateInvoiceCommand,
            ICommandHandler<UpdateOrderStatusCommand> updateOrderStatusCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _updateInvoiceCommand = updateInvoiceCommand;
            _updateOrderStatusCommand = updateOrderStatusCommand;
        }
        public UpdateInvoiceResponse UpdateInvoice(UpdateInvoice command)
        {
            UpdateInvoiceResponse response = new UpdateInvoiceResponse();
            InterfaceLogPayGCommand log = StartInterface<UpdateInvoice>(command, MethodBase.GetCurrentMethod(), command.Transaction_ID, "");
            var mandatory = false;
            var invalid = false;


            try
            {
                // check Invalid Action
                invalid = command.Action.ToUpper().Equals("CREATE") ||
                    command.Action.ToUpper().Equals("UPDATE") ||
                    command.Action.ToUpper().Equals("DELETE");

                if (invalid)
                {
                    CheckMandatoryUpdateInvoice(ref mandatory, command);

                    if (mandatory)
                    {
                        response = new UpdateInvoiceResponse
                        {
                            Result_code = warning.Failedcode,
                            Result_message = warning.Missing
                        };
                        EndInterface<UpdateInvoiceResponse>(response, log, Constants.Result.Failed, response.Result_message);
                    }
                    else
                    {
                        UpdateInvoiceCommand udcommand = new UpdateInvoiceCommand
                        {
                            Action = command.Action.ToUpper(),
                            Transaction_ID = command.Transaction_ID,
                            Invoice_no_Old = command.Invoice_no_Old,
                            Invoice_no_New = command.Invoice_no_New,
                            Invoice_date = command.Invoice_date,
                            Create_By = command.Create_By,
                            Create_Date = command.Create_Date,
                            Order_list = command.Order_list,
                            ret_code = "",
                            ret_msg = ""
                        };

                        _updateInvoiceCommand.Handle(udcommand);

                        if (udcommand.ret_code == "0")
                        {
                            response = new UpdateInvoiceResponse
                            {
                                Result_code = warning.Successcode,
                                Result_message = udcommand.ret_msg
                            };
                            EndInterface<UpdateInvoiceResponse>(response, log, Constants.Result.Success, response.Result_message);
                            response.Result_message = Constants.Result.Success;
                        }
                        else
                        {
                            response = new UpdateInvoiceResponse
                            {
                                Result_code = warning.Failedcode,
                                Result_message = udcommand.ret_msg
                            };
                            EndInterface<UpdateInvoiceResponse>(response, log, Constants.Result.Failed, Constants.Result.Failed);
                            response.Result_message = Constants.Result.Failed;
                        }
                    }
                }
                else
                {
                    response = new UpdateInvoiceResponse
                    {
                        Result_code = warning.Failedcode,
                        Result_message = command.Action.ToSafeString() == "" ? warning.Missing : warning.Invalid
                    };
                    EndInterface<UpdateInvoiceResponse>(response, log, Constants.Result.Failed, response.Result_message);
                }
            }
            catch
            {
                var error = new UpdateInvoiceResponse
                {
                    Result_code = warning.Failedcode,
                    Result_message = warning.System
                };
                EndInterface<UpdateInvoiceResponse>(response, log, Constants.Result.Failed, error.Result_message);

                return error;
            }

            return response;
        }

        public UpdateOrderStatusResponse UpdateOrderStatus(UpdateOrderStatus command)
        {
            InterfaceLogPayGCommand log = StartInterface<UpdateOrderStatus>(command, MethodBase.GetCurrentMethod(), command.Transaction_ID, "");
            var response = new UpdateOrderStatusResponse();
            var chkInvoiceno = false;
            try
            {

                if (command == null)
                {
                    response = new UpdateOrderStatusResponse
                    {
                        Result_code = warning.Failedcode,
                        Result_message = warning.Missing
                    };
                    EndInterface<UpdateOrderStatusResponse>(response, log, Constants.Result.Failed, warning.Missing);
                }
                else
                {
                    var invalid = command.Status.ToUpper().Equals("CONFIRM PAID") ||
                    command.Status.ToUpper().Equals("PAID") ||
                    command.Status.ToUpper().Equals("HOLD");

                    foreach (var _ in command.Invoice_List.Where(item => item.Invoice_no == "").Select(item => new { }))
                        chkInvoiceno = true;

                    if (invalid &&
                        !chkInvoiceno &&
                        command.Transaction_ID.ToSafeString() != "" &&
                        command.Update_By.ToSafeString() != "" &&
                        command.Update_Date.ToSafeString() != "")
                    {
                        UpdateOrderStatusCommand updateStatus = new UpdateOrderStatusCommand
                        {
                            Status = command.Status,
                            Transaction_ID = command.Transaction_ID,
                            Update_By = command.Update_By,
                            Update_Date = command.Update_Date,
                            Invoice_List = command.Invoice_List
                        };
                        _updateOrderStatusCommand.Handle(updateStatus);

                        if (updateStatus.ret_code == "0")
                        {
                            response = new UpdateOrderStatusResponse
                            {
                                Result_code = warning.Successcode,
                                Result_message = updateStatus.ret_msg
                            };
                            EndInterface<UpdateOrderStatusResponse>(response, log, Constants.Result.Success, response.Result_message);
                            response.Result_message = Constants.Result.Success;
                        }
                        else
                        {
                            response = new UpdateOrderStatusResponse
                            {
                                Result_code = warning.Failedcode,
                                Result_message = updateStatus.ret_msg
                            };

                            EndInterface<UpdateOrderStatusResponse>(response, log, Constants.Result.Failed, Constants.Result.Failed);
                            response.Result_message = Constants.Result.Failed;
                        }
                    }
                    else
                    {
                        string Result_message = "";
                        if (!invalid)
                        {
                            Result_message = command.Status.ToSafeString() != "" ? warning.Invalid : warning.Missing;
                        }
                        else
                        {
                            if (command.Status.ToSafeString() == "" ||
                            command.Transaction_ID.ToSafeString() == "" ||
                            command.Update_By.ToSafeString() == "" ||
                            command.Update_Date.ToSafeString() == "" ||
                            chkInvoiceno)
                            {
                                Result_message = warning.Missing;
                            }
                        }


                        response = new UpdateOrderStatusResponse
                        {
                            Result_code = warning.Failedcode,
                            Result_message = Result_message
                        };
                        EndInterface<UpdateOrderStatusResponse>(response, log, Constants.Result.Failed, response.Result_message);
                    }
                }
            }
            catch
            {
                response = new UpdateOrderStatusResponse
                {
                    Result_code = warning.Failedcode,
                    Result_message = warning.System
                };

                EndInterface<UpdateOrderStatusResponse>(response, log, Constants.Result.Failed, warning.System);
            }

            return response;
        }


        private void CheckMandatoryUpdateInvoice(ref bool mandatory, UpdateInvoice command)
        {
            //Check mandatory
            switch (command.Action.ToUpper())
            {
                case "CREATE" when command.Invoice_no_New.ToSafeString() == "":
                    mandatory = true;
                    break;
                case "UPDATE" when (command.Invoice_no_New.ToSafeString() == "" && command.Invoice_no_Old.ToSafeString() == "") ||
                        (command.Invoice_no_New.ToSafeString() != "" && command.Invoice_no_Old.ToSafeString() == "") ||
                        (command.Invoice_no_New.ToSafeString() == "" && command.Invoice_no_Old.ToSafeString() != ""):
                    mandatory = true;
                    break;
                case "DELETE" when command.Invoice_no_Old.ToSafeString() == "":
                    mandatory = true;
                    break;
                default:
                    break;
            }

            if (command.Invoice_date.ToSafeString() == "" ||
                command.Create_By.ToSafeString() == "" ||
                command.Create_Date.ToSafeString() == "" ||
                command.Transaction_ID.ToSafeString() == "")
                mandatory = true;

            if (command.Order_list.Count > 0)
                foreach (var item in from item in command.Order_list
                                     where item.Access_No.ToSafeString() == "" ||
                                     item.Order_no.ToSafeString() == ""
                                     select new { })
                    mandatory = true;

        }

        private Boolean CheckFormatDateTime(string datetime)
        {
            DateTime foundDate;
            Match matchResult = Regex.Match(datetime.Trim(), "^(?<day>[0-3]?[0-9])/(?<month>[0-3]?[0-9])/(?<year>(?:[0-9]{2})?[0-9]{2}) (?<hour>[0-9]{2}):(?<minute>[0-9]{2}):(?<second>[0-9]{2})$");
            if (matchResult.Success)
            {
                int year = int.Parse(matchResult.Groups["year"].Value);
                if (year < 50) year += 2000;
                else if (year < 100) year += 1900;
                else if (year > 2500) year -= 543;
                try
                {
                    foundDate = new DateTime(year,
                        int.Parse(matchResult.Groups["month"].Value),
                        int.Parse(matchResult.Groups["day"].Value));
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean CheckFormatDate(string date)
        {
            DateTime foundDate;
            Match matchResult = Regex.Match(date, "^(?<day>[0-3]?[0-9])/(?<month>[0-3]?[0-9])/(?<year>(?:[0-9]{2})?[0-9]{2})$");
            if (matchResult.Success)
            {
                int year = int.Parse(matchResult.Groups["year"].Value);
                if (year < 50) year += 2000;
                else if (year < 100) year += 1900;
                else if (year > 2500) year -= 543;
                try
                {
                    foundDate = new DateTime(year,
                        int.Parse(matchResult.Groups["month"].Value),
                        int.Parse(matchResult.Groups["day"].Value));
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private InterfaceLogPayGCommand StartInterface<T>(T query, MethodBase methodName, string transactionId, string idCardNo)
        {
            var dbIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName.Name,
                SERVICE_NAME = "FBBPAYG_LMD_Interface",
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "Subcontract Payment",
                CREATED_BY = "LMD",
            };
            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }
        private void EndInterface<T>(T output, InterfaceLogPayGCommand dbIntfCmd, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            //switch (result)
            //{
            //	case "Missing parameter":
            //		break;
            //	case "Invalid Parameter":
            //		break;
            //	case "System Error":
            //		break;
            //	case "Success":
            //		break;
            //	default:
            //		result = "Failed";
            //		break;
            //}

            dbIntfCmd.ActionType = ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (!reason.Contains("System")) ? "Success" : "Failed";
            dbIntfCmd.OUT_RESULT = (result == "Success") ? "Success" : "Failed";
            dbIntfCmd.OUT_ERROR_RESULT = reason;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();
            dbIntfCmd.UPDATED_BY = "LMD";

            _intfLogCommand.Handle(dbIntfCmd);
        }



        private class warning
        {
            internal static readonly string Missing = "Missing parameter";
            internal static readonly string Invalid = "Invalid Parameter";
            internal static readonly string System = "System Error";
            internal static readonly string Failedcode = "0001";
            internal static readonly string Successcode = "0000";
        }
    }


}
