using System;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class CreateOrderChangePromotionDisCountCommandHandler : ICommandHandler<CreateOrderChangePromotionDisCountCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

        public CreateOrderChangePromotionDisCountCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
        }

        public void Handle(CreateOrderChangePromotionDisCountCommand command)
        {
            var objReq = new SFFServices.SffRequest();
            bool canCreateObj = true;
            var Curr_DateTime = DateTime.Now;
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command, command.mobileNo, "CreateOrderChangePromotionDisCountCommand", "CrateObjectRequest", null, "FBB|" + command.FullUrl, "FBBOR018");

            try
            {
                var listoflist = new List<SFFServices.ParameterList>();


                SFFServices.ParameterList plist = new SFFServices.ParameterList()
                {
                    ParameterType = "Promotion",
                    Parameter = new SFFServices.Parameter[]
                        {
                            new SFFServices.Parameter() { Name = "promotionCode", Value = command.promotionCode },
                            new SFFServices.Parameter() { Name = "actionStatus", Value = command.actionStatus },
                            new SFFServices.Parameter() { Name = "overRuleStartDate", Value = command.overRuleStartDate },
                            new SFFServices.Parameter() { Name = "chargeType", Value = command.chargeType },
                            new SFFServices.Parameter() { Name = "waiveFlag", Value = command.waiveFlag }
                        }
                };
                listoflist.Add(plist);

                if (command.promotionCode_1 != null)
                {
                    SFFServices.ParameterList plist1 = new SFFServices.ParameterList()
                    {
                        ParameterType = "Promotion",
                        Parameter = new SFFServices.Parameter[]
                        {
                            new SFFServices.Parameter() { Name = "promotionCode", Value = command.promotionCode_1 },
                            new SFFServices.Parameter() { Name = "actionStatus", Value = command.actionStatus_1 },
                            new SFFServices.Parameter() { Name = "overRuleStartDate", Value = command.overRuleStartDate_1 },
                            new SFFServices.Parameter() { Name = "chargeType", Value = command.chargeType_1 },
                            new SFFServices.Parameter() { Name = "waiveFlag", Value = command.waiveFlag_1 }
                        }
                    };
                    listoflist.Add(plist1);
                }

                if (command.projectName != null)
                {
                    var objParam = new List<SFFServices.Parameter>();
                    if (command.actionRelateMobile == "Add" || string.IsNullOrEmpty(command.actionRelateMobile))
                    {
                        objParam.Add(new SFFServices.Parameter() { Name = "relateMobileNo", Value = command.relateMobileNo });
                    }

                    if (objParam.Count > 0)
                    {
                        SFFServices.ParameterList plist2 = new SFFServices.ParameterList()
                        {
                            ParameterType = "relateMobile",
                            Parameter = objParam.ToArray()
                        };
                        listoflist.Add(plist2);
                    }
                }

                string refNo = command.referenceNo;
                if (command.projectName == null)
                {
                    refNo = "";
                }

                var objReqParam = new SFFServices.Parameter[]
                {
                    new SFFServices.Parameter() { Name = "orderType", Value = command.orderType },
                    new SFFServices.Parameter() { Name = "mobileNo", Value = command.mobileNo },
                    new SFFServices.Parameter() { Name = "orderChannel", Value = command.orderChannel },
                    new SFFServices.Parameter() { Name = "orderReson", Value = command.orderReson },
                    new SFFServices.Parameter() { Name = "userName", Value = command.userName },
                    new SFFServices.Parameter() { Name = "chargeFeeFlag", Value = command.chargeFeeFlag },
                    new SFFServices.Parameter() { Name = "ascCode", Value = command.ascCode },
                    new SFFServices.Parameter() { Name = "locationCd", Value = command.locationCd },
                    new SFFServices.Parameter() { Name = "club900Mobile", Value = command.club900Mobile },
                    new SFFServices.Parameter() { Name = "sourceSystem", Value = command.sourceSystem },
                }
                .ToList();

                if (!string.IsNullOrEmpty(command.mobileNumberContact))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "mobileNumberContact", Value = command.mobileNumberContact });
                }
                if (!string.IsNullOrEmpty(command.actionRelateMobile))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "actionRelateMobile", Value = command.actionRelateMobile });
                }
                if (!string.IsNullOrEmpty(command.projectName))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "projectName", Value = command.projectName });
                }
                if (command.actionRelateMobile == "Add" || (string.IsNullOrEmpty(command.actionRelateMobile) && command.projectName == "FMB1"))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "referenceNo", Value = refNo });
                }

                objReq = new SFFServices.SffRequest()
                {
                    Event = "evOMWebFBBCreateOrderChangePromotion",
                    ParameterList = new SFFServices.ParameterList()
                    {
                        Parameter = objReqParam.ToArray(),
                        #region Test Data
                        //ParameterList1 = new SFFServices.ParameterList[]
                        //{
                        //    new SFFServices.ParameterList() //List Package Change1
                        //    {
                        //        ParameterType = "Promotion",
                        //        Parameter = new SFFServices.Parameter[]
                        //        {
                        //            new SFFServices.Parameter() { Name = "promotionCode", Value = "P09050055" },
                        //            new SFFServices.Parameter() { Name = "actionStatus", Value = "Add" },
                        //            new SFFServices.Parameter() { Name = "productSeq", Value = "" },
                        //            new SFFServices.Parameter() { Name = "promotionStartDt", Value = "" },
                        //            new SFFServices.Parameter() { Name = "overRuleStartDate", Value = "D" },
                        //            new SFFServices.Parameter() { Name = "waiveFlag", Value = "" },
                        //            new SFFServices.Parameter() { Name = "changeType", Value = "Post-paid" },
                        //            //new SFFServices.Parameter() { Name = "userRemark", Value = "xxxxxxxxxxxxxx" },
                        //            new SFFServices.Parameter() { Name = "orderItemReason", Value = "" },
                        //            new SFFServices.Parameter() { Name = "promotionClass", Value = "" }
                        //        }                            
                        //    },
                        //    new SFFServices.ParameterList() //List Package Change2
                        //    {
                        //        ParameterType = "Promotion",
                        //        Parameter = new SFFServices.Parameter[]
                        //        {
                        //            new SFFServices.Parameter() { Name = "promotionCode", Value = "P09050055" },
                        //            new SFFServices.Parameter() { Name = "actionStatus", Value = "Add" },
                        //            new SFFServices.Parameter() { Name = "productSeq", Value = "" },
                        //            new SFFServices.Parameter() { Name = "promotionStartDt", Value = "" },
                        //            new SFFServices.Parameter() { Name = "overRuleStartDate", Value = "D" },
                        //            new SFFServices.Parameter() { Name = "waiveFlag", Value = "" },
                        //            new SFFServices.Parameter() { Name = "changeType", Value = "Post-paid" },
                        //            //new SFFServices.Parameter() { Name = "userRemark", Value = "xxxxxxxxxxxxxx" },
                        //            new SFFServices.Parameter() { Name = "orderItemReason", Value = "" },
                        //            new SFFServices.Parameter() { Name = "promotionClass", Value = "" }
                        //        }                            
                        //    },
                        //    new SFFServices.ParameterList() //List Relate Mobile
                        //    {
                        //        ParameterType = "relateMobile",
                        //        Parameter = new SFFServices.Parameter[]
                        //        {
                        //            new SFFServices.Parameter() { Name = "relateMobileNo", Value = "0899999999" },
                        //            new SFFServices.Parameter() { Name = "relateMobileNo", Value = "0899999999" }
                        //        }
                        //    }
                        //}
                        #endregion
                        ParameterList1 = listoflist.ToArray()
                    }
                };

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objReq, log, "Success", "", "FBBOR018");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, new SFFServices.SffRequest(), log, "Failed", ex.Message, "FBBOR018");
                canCreateObj = false;
            }

            if (canCreateObj)
            {
                InterfaceLogCommand log2 = null;
                Curr_DateTime = DateTime.Now;
                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, objReq, command.mobileNo, "SFFServices.SFFServiceService", "evOMWebFBBCreateOrderChangePromotion", null, "FBB|" + command.FullUrl, "FBBOR018");
                try
                {
                    using (var service = new SFFServices.SFFServiceService())
                    {
                        service.Timeout = 600000;
                        var objResp = service.ExecuteService(objReq);

                        if (objResp.ErrorMessage == null)
                        {
                            foreach (var data in objResp.ParameterList.Parameter)
                            {
                                if (data.Name == "VALIDATE_FLAG")
                                {
                                    command.validateFlag = data.Value;
                                    if (data.Value != "Y")
                                    {
                                        foreach (var subdata in objResp.ParameterList.ParameterList1)
                                        {
                                            foreach (var error in subdata.Parameter)
                                            {
                                                if (error.Name == "ERROR_MASSAGE")
                                                {
                                                    command.errorMsg = error.Value;
                                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", command.errorMsg, "FBBOR018");
                                                    break;
                                                }
                                            }
                                        }
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", "VALIDATE_FLAG != Y and not found ERROR_MESSAGE", "FBBOR018");
                                    }
                                    else
                                    {
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Success", "", "FBBOR018");
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", objResp.ErrorMessage, "FBBOR018");
                            command.validateFlag = "N";
                            command.errorMsg = objResp.ErrorMessage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, new SFFServices.SffResponse(), log2, "Failed", ex.Message, "FBBOR018");
                    command.validateFlag = "N";
                    command.errorMsg = ex.Message;
                }
            }
        }
    }
}
