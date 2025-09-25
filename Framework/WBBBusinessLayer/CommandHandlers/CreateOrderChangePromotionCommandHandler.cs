using System;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.CommandHandlers
{
    public class CreateOrderChangePromotionCommandHandler : ICommandHandler<CreateOrderChangePromotionCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;
        private readonly IEntityRepository<FBB_ORD_CHANGE_PACKAGE> _OrdChangePackageTable;

        public CreateOrderChangePromotionCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            IEntityRepository<FBB_ORD_CHANGE_PACKAGE> OrdChangePackageTable)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
            _OrdChangePackageTable = OrdChangePackageTable;
        }

        public void Handle(CreateOrderChangePromotionCommand command)
        {
            var objReq = new SFFServices.SffRequest();
            bool canCreateObj = true;
            var Curr_DateTime = DateTime.Now;
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command, command.NonMobileNo + command.client_ip, "CreateOrderChangePromotionCommand", "CrateObjectRequest", null, "FBB|" + command.FullUrl, "FBBOR016");

            try
            {
                //TODO: R20.6 add FBB_ORD_CHANGE_PACKAGE
                try
                {
                    if (command.ListChangePackageModel != null)
                    {
                        DateTime CurrDt = DateTime.Now;
                        foreach (var tmp in command.ListChangePackageModel)
                        {
                            FBB_ORD_CHANGE_PACKAGE newrow = new FBB_ORD_CHANGE_PACKAGE()
                            {
                                ORDER_NO = tmp.order_no.ToSafeString(),
                                NON_MOBILE_NO = tmp.non_mobile_no.ToSafeString(),
                                RELATE_MOBILE = tmp.relate_mobile.ToSafeString(),
                                SFF_PROMOTION_CODE = tmp.sff_promotion_code.ToSafeString(),
                                ACTION_STATUS = tmp.action_status.ToSafeString(),
                                PACKAGE_STATE = tmp.package_state.ToSafeString(),
                                PROJECT_NAME = tmp.project_name.ToSafeString(),
                                CREATED_BY = "FBBOR016",
                                CREATED_DATE = CurrDt,
                                UPDATED_BY = "FBBOR016",
                                UPDATED_DATE = CurrDt,
                                PRODUCT_SEQ = tmp.product_seq.ToSafeString(),
                                BUNDLING_ACTION = command.BUNDLING_ACTION,
                                OLD_RELATE_MOBILE = command.OLD_RELATE_MOBILE,
                                MOBILE_CONTACT = command.MOBILE_CONTACT
                            };
                            _OrdChangePackageTable.Create(newrow);
                        }
                        _uow.Persist();
                    }
                }
                catch (Exception ex)
                {
                    var logcreate = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command.ListChangePackageModel, command.NonMobileNo + command.client_ip, "CreateOrderChangePromotionCommand", "Create_ORD_CHANGE_PACKAGE", null, "FBB|" + command.FullUrl, "FBBOR016");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, new SFFServices.SffResponse(), logcreate, "Failed", ex.Message, "FBBOR016");
                }

                //end add


                var listoflist = new List<SFFServices.ParameterList>();
                foreach (var tmp in command.ListAction)
                {
                    if (tmp.PromotionCode != null && tmp.PromotionCode != "")
                    {
                        // R20.6 Add by Aware : Atipon (Add Condition Check Send Sff Flag)
                        if (tmp.SendSffFlag == "Y")
                        {
                            SFFServices.ParameterList plist = new SFFServices.ParameterList()
                            {
                                ParameterType = "Promotion",
                                Parameter = new SFFServices.Parameter[]
                                {
                                    new SFFServices.Parameter() { Name = "promotionCode", Value = tmp.PromotionCode },
                                    new SFFServices.Parameter() { Name = "actionStatus", Value = tmp.ActionStatus },
                                    new SFFServices.Parameter() { Name = "overRuleStartDate", Value = tmp.Overrule },
                                    new SFFServices.Parameter() { Name = "chargeType", Value = "Post-paid" }
                                }
                            };

                            listoflist.Add(plist);
                        }
                    }
                }

                /// Add PlayBox

                if (command.promotionPlayBox != null)
                {
                    /// Add PlayBox Data

                    List<SFFServices.ParameterList> parameterList1 = new List<SFFServices.ParameterList>();
                    SFFServices.ParameterList parameter1 = new SFFServices.ParameterList();
                    parameter1.ParameterType = "";
                    parameter1.Parameter = new SFFServices.Parameter[]
                        {
                           new SFFServices.Parameter() { Name = "accessMode", Value = command.promotionPlayBox.accessMode.ToSafeString() },
                           new SFFServices.Parameter() { Name = "addressId", Value = command.promotionPlayBox.addressId.ToSafeString() },
                           new SFFServices.Parameter() { Name = "appointmentDate", Value = command.promotionPlayBox.appointmentDate.ToSafeString() },
                           new SFFServices.Parameter() { Name = "contactMobilePhone", Value = command.promotionPlayBox.contactMobilePhone.ToSafeString() },
                           new SFFServices.Parameter() { Name = "contactName", Value = command.promotionPlayBox.contactName.ToSafeString() },
                           new SFFServices.Parameter() { Name = "contentName", Value = command.promotionPlayBox.contentName.ToSafeString() },
                           new SFFServices.Parameter() { Name = "installAddress1", Value = command.promotionPlayBox.installAddress1.ToSafeString() },
                           new SFFServices.Parameter() { Name = "installAddress2", Value = command.promotionPlayBox.installAddress2.ToSafeString() },
                           new SFFServices.Parameter() { Name = "installAddress3", Value = command.promotionPlayBox.installAddress3.ToSafeString() },
                           new SFFServices.Parameter() { Name = "installAddress4", Value = command.promotionPlayBox.installAddress4.ToSafeString() },
                           new SFFServices.Parameter() { Name = "installAddress5", Value = command.promotionPlayBox.installAddress5.ToSafeString() },
                           new SFFServices.Parameter() { Name = "relateMobile", Value = command.promotionPlayBox.relateMobile.ToSafeString() },
                           new SFFServices.Parameter() { Name = "serialNumber", Value = command.promotionPlayBox.serialNumber.ToSafeString() },
                           new SFFServices.Parameter() { Name = "timeSlot", Value = command.promotionPlayBox.timeSlot.ToSafeString() },
                           new SFFServices.Parameter() { Name = "installedFlag", Value = command.promotionPlayBox.installedFlag.ToSafeString() }
                        };
                    parameterList1.Add(parameter1);

                    List<SFFServices.ParameterList> parameterList = new List<SFFServices.ParameterList>();
                    SFFServices.ParameterList parameter = new SFFServices.ParameterList();
                    parameter.ParameterType = "Playbox";
                    parameter.Parameter = new SFFServices.Parameter[]
                        {
                           new SFFServices.Parameter() { Name = "serviceCode", Value = command.promotionPlayBox.serviceCode.ToSafeString() },
                           new SFFServices.Parameter() { Name = "actionStatus", Value = "Add" }
                        };
                    parameter.ParameterList1 = parameterList1.ToArray();

                    parameterList.Add(parameter);

                    // Service FBB Appointment Data

                    List<SFFServices.ParameterList> parameterListApp1 = new List<SFFServices.ParameterList>();
                    SFFServices.ParameterList parameterApp1 = new SFFServices.ParameterList();
                    parameterApp1.ParameterType = "";
                    parameterApp1.Parameter = new SFFServices.Parameter[]
                        {
                           new SFFServices.Parameter() { Name = "appointmentDate", Value = command.appointmentDate },
                           new SFFServices.Parameter() { Name = "installationCapacity", Value = "" },
                           new SFFServices.Parameter() { Name = "reservedId", Value = command.reservedId },
                           new SFFServices.Parameter() { Name = "timeslot", Value = command.timeslot },
                           new SFFServices.Parameter() { Name = "fbbContactNo1 ", Value = "" },
                           new SFFServices.Parameter() { Name = "fbbContactNo2", Value = "" },
                           new SFFServices.Parameter() { Name = "remarkForSubcontract", Value = "" },
                           new SFFServices.Parameter() { Name = "playBoxAmount", Value = "" },
                           new SFFServices.Parameter() { Name = "urgentFlag", Value = "2" },
                           new SFFServices.Parameter() { Name = "reservedPort", Value = "" },
                           new SFFServices.Parameter() { Name = "symptom", Value = "" },
                           new SFFServices.Parameter() { Name = "subAccessMode", Value = "" },
                           new SFFServices.Parameter() { Name = "paidAmt", Value = "" },
                           new SFFServices.Parameter() { Name = "paymentMethod", Value = "" },
                           new SFFServices.Parameter() { Name = "transactionId", Value = "" },
                           new SFFServices.Parameter() { Name = "subRequestFlag", Value = "" }
                        };
                    parameterListApp1.Add(parameterApp1);


                    SFFServices.ParameterList parameterApp = new SFFServices.ParameterList();
                    parameterApp.ParameterType = "Appointment";
                    parameterApp.Parameter = new SFFServices.Parameter[]
                        {
                           new SFFServices.Parameter() { Name = "serviceCode", Value = command.servicCodeApp },
                           new SFFServices.Parameter() { Name = "actionStatus", Value = "Add" }
                        };
                    parameterApp.ParameterList1 = parameterListApp1.ToArray();

                    parameterList.Add(parameterApp);

                    SFFServices.ParameterList plist = new SFFServices.ParameterList();
                    plist.ParameterType = "Service";
                    plist.ParameterList1 = parameterList.ToArray();

                    listoflist.Add(plist);

                }



                //if (command.ProjectName == "FMB1")
                //{
                //    var objParam = new List<SFFServices.Parameter>();
                //    if (command.acTion == "Add" || command.acTion == "Replace" || string.IsNullOrEmpty(command.acTion))
                //    {
                //        objParam.Add(new SFFServices.Parameter() { Name = "relateMobileNo", Value = command.RelateMobile });
                //    }
                //    if (command.acTion == "Replace")
                //    {
                //        objParam.Add(new SFFServices.Parameter() { Name = "oldRelateMobile", Value = command.oldRelateMobile });
                //    }

                //    if (objParam.Count > 0)
                //    {
                //        SFFServices.ParameterList plist = new SFFServices.ParameterList()
                //        {
                //            ParameterType = "relateMobile",
                //            Parameter = objParam.ToArray()
                //        };
                //        listoflist.Add(plist);
                //    }
                //}

                string refNo = command.OrderNo;
                if (command.ProjectName == null)
                {
                    refNo = "";
                }

                var objReqParam = new SFFServices.Parameter[]
                {
                    new SFFServices.Parameter() { Name = "orderType", Value = "Change Promotion" },
                    new SFFServices.Parameter() { Name = "mobileNo", Value = command.NonMobileNo },
                    new SFFServices.Parameter() { Name = "orderChannel", Value = "WEB" },
                    new SFFServices.Parameter() { Name = "orderReson", Value = "956" },
                    new SFFServices.Parameter() { Name = "userName", Value = "AWN_FBB" },
                    new SFFServices.Parameter() { Name = "chargeFeeFlag", Value = "N" },
                    new SFFServices.Parameter() { Name = "ascCode", Value = command.ascCode.ToSafeString() },
                    new SFFServices.Parameter() { Name = "locationCd", Value = command.locationCd.ToSafeString() },
                    new SFFServices.Parameter() { Name = "club900Mobile", Value = "" },
                    new SFFServices.Parameter() { Name = "employeeID", Value = command.EmployeeID.ToSafeString() },
                    new SFFServices.Parameter() { Name = "sourceSystem", Value = "WEB_FBB" },
                    new SFFServices.Parameter() { Name = "saleStaffName", Value = command.EmployeeName.ToSafeString() }
                }
                .ToList();

                if (!string.IsNullOrEmpty(command.mobileNumberContact))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "mobileNumberContact", Value = command.mobileNumberContact });
                }
                if (!string.IsNullOrEmpty(command.acTion))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "actionRelateMobile", Value = command.acTion });
                }
                if (!string.IsNullOrEmpty(command.ProjectName))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "projectName", Value = command.ProjectName });
                }
                if (command.acTion == "Add" || (string.IsNullOrEmpty(command.acTion) && (command.ProjectName == "FMB1")) || command.ProjectName == "Waiting_FMC" || command.ProjectName == "FMC")
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "referenceNo", Value = refNo });
                }

                //R20.6 add field  projectOption, relateOption
                if (!string.IsNullOrEmpty(command.new_project_name_opt))
                {
                    objReqParam.Add(new SFFServices.Parameter() { Name = "projectOption", Value = command.new_project_name_opt });
                }

                var objParam = new List<SFFServices.Parameter>();
                if (command.acTion == "Replace")
                {
                    objParam.Add(new SFFServices.Parameter() { Name = "relateMobileNo", Value = command.RelateMobile });
                    objParam.Add(new SFFServices.Parameter() { Name = "relateOption", Value = "" });
                    objParam.Add(new SFFServices.Parameter() { Name = "oldRelateMobile", Value = command.oldRelateMobile });

                    if (objParam.Count > 0)
                    {
                        SFFServices.ParameterList plist = new SFFServices.ParameterList()
                        {
                            ParameterType = "relateMobile",
                            Parameter = objParam.ToArray()
                        };
                        listoflist.Add(plist);
                    }

                }

                //Relate_mobile list
                if (!string.IsNullOrEmpty(command.new_mobile_check_right))
                {
                    objParam = new List<SFFServices.Parameter>();
                    objParam.Add(new SFFServices.Parameter() { Name = "relateMobileNo", Value = command.new_mobile_check_right });
                    objParam.Add(new SFFServices.Parameter() { Name = "relateOption", Value = command.new_mobile_check_right_opt });

                    if (objParam.Count > 0)
                    {
                        SFFServices.ParameterList plist = new SFFServices.ParameterList()
                        {
                            ParameterType = "relateMobile",
                            Parameter = objParam.ToArray()
                        };
                        listoflist.Add(plist);
                    }
                }

                if (!string.IsNullOrEmpty(command.new_mobile_get_benefit))
                {
                    objParam = new List<SFFServices.Parameter>();

                    objParam.Add(new SFFServices.Parameter() { Name = "relateMobileNo", Value = command.new_mobile_get_benefit });
                    objParam.Add(new SFFServices.Parameter() { Name = "relateOption", Value = command.new_mobile_get_benefit_opt });
                    if (command.ProjectName == "FMB1" && command.acTion == "Replace")
                    {
                        objParam.Add(new SFFServices.Parameter() { Name = "oldRelateMobile", Value = command.oldRelateMobile });
                    }
                    if (objParam.Count > 0)
                    {
                        SFFServices.ParameterList plist = new SFFServices.ParameterList()
                        {
                            ParameterType = "relateMobile",
                            Parameter = objParam.ToArray()
                        };
                        listoflist.Add(plist);
                    }
                }
                //R20.6 end


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

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objReq, log, "Success", "", "FBBOR016");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, new SFFServices.SffRequest(), log, "Failed", ex.Message, "FBBOR016");
                canCreateObj = false;
            }

            if (canCreateObj)
            {
                InterfaceLogCommand log2 = null;
                Curr_DateTime = DateTime.Now;
                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, objReq, command.NonMobileNo, "SFFServices.SFFServiceService", "evOMWebFBBCreateOrderChangePromotion", null, "FBB|" + command.FullUrl, "FBBOR016");
                try
                {
                    using (var service = new SFFServices.SFFServiceService())
                    {
                        var objResp = service.ExecuteService(objReq);

                        if (objResp.ErrorMessage == null)
                        {
                            foreach (var data in objResp.ParameterList.Parameter)
                            {
                                if (data.Name == "VALIDATE_FLAG")
                                {
                                    command.VALIDATE_FLAG = data.Value;
                                    if (data.Value != "Y")
                                    {
                                        foreach (var subdata in objResp.ParameterList.ParameterList1)
                                        {
                                            foreach (var error in subdata.Parameter)
                                            {
                                                if (error.Name == "ERROR_MASSAGE")
                                                {
                                                    command.ERROR_MSG = error.Value == null ? "" : error.Value;
                                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", command.ERROR_MSG, "FBBOR016");
                                                    break;
                                                }
                                            }
                                        }
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", "VALIDATE_FLAG != Y and not found ERROR_MESSAGE", "FBBOR016");
                                    }
                                    else
                                    {
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Success", "", "FBBOR016");
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", objResp.ErrorMessage, "FBBOR016");
                            command.VALIDATE_FLAG = "N";
                            command.ERROR_MSG = objResp.ErrorMessage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, new SFFServices.SffResponse(), log2, "Failed", ex.Message, "FBBOR016");
                    command.VALIDATE_FLAG = "N";
                    command.ERROR_MSG = ex.Message;
                }
            }
        }
    }

    public class CreateOrderMeshPromotionCommandHandler : ICommandHandler<CreateOrderMeshPromotionCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

        public CreateOrderMeshPromotionCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
        {
            _logger = logger;
            _uow = uow;

            _interfaceLog = interfaceLog;
        }

        public void Handle(CreateOrderMeshPromotionCommand command)
        {
            bool canCreateObj = true;
            SFFServices.SffRequest objReq = null;
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command, command.NonMobileNo + command.client_ip, "CreateOrderMeshPromotionCommand", "CreateOrderMeshPromotion", null, "FBB|" + command.FullUrl, "FBBWEB");

            try
            {
                if (command.GetOrderChangeService != null && command.GetOrderChangeService.OrdChangeServiceList != null && command.GetOrderChangeService.OrdChangeServiceList.Count > 0)
                {
                    OrdChangeService OrdChangeServicePara = new OrdChangeService();
                    OrdChangeServicePara = command.GetOrderChangeService.OrdChangeServiceList.FirstOrDefault();

                    var listoflist = new List<SFFServices.ParameterList>();

                    /// Add Service Para
                    SFFServices.ParameterList pServiceAppointment = new SFFServices.ParameterList();
                    pServiceAppointment.Parameter = new SFFServices.Parameter[]
                        {
                           new SFFServices.Parameter() { Name = "actionStatus", Value = OrdChangeServicePara.actionStatus.ToSafeString() },
                           new SFFServices.Parameter() { Name = "serviceCode", Value = OrdChangeServicePara.serviceCode.ToSafeString() }
                        };

                    //serviceAttribute
                    if (command.GetOrderChangeService != null && command.GetOrderChangeService.OrdServiceAttributeList != null && command.GetOrderChangeService.OrdServiceAttributeList.Count > 0)
                    {
                        List<SFFServices.ParameterList> pServiceAttributeList = new List<SFFServices.ParameterList>();
                        foreach (var item in command.GetOrderChangeService.OrdServiceAttributeList)
                        {
                            SFFServices.Parameter[] serviceAppointmentDateParameterList = new SFFServices.Parameter[]
                        {
                            new SFFServices.Parameter() { Name = "serviceAttributeName", Value = item.serviceAttributeName.ToSafeString() },
                            new SFFServices.Parameter() { Name = "serviceAttributeValue", Value = item.serviceAttributeValue.ToSafeString() }
                        };
                            SFFServices.ParameterList serviceAppointmentDateList = new SFFServices.ParameterList()
                            {
                                Parameter = serviceAppointmentDateParameterList
                            };

                            pServiceAttributeList.Add(serviceAppointmentDateList);
                        }
                        pServiceAppointment.ParameterList1 = pServiceAttributeList.ToArray();
                        listoflist.Add(pServiceAppointment);
                    }
                    ///

                    /// Add OrderFee
                    if (command.GetOrderChangeService != null && command.GetOrderChangeService.OrdFeeList != null && command.GetOrderChangeService.OrdFeeList.Count > 0)
                    {
                        List<SFFServices.ParameterList> pOrderFeeList = new List<SFFServices.ParameterList>();
                        foreach (var item in command.GetOrderChangeService.OrdFeeList)
                        {
                            SFFServices.ParameterList parameterOrderFee = new SFFServices.ParameterList();
                            parameterOrderFee.Parameter = new SFFServices.Parameter[]
                            {
                                new SFFServices.Parameter() { Name = "orderFeeCd", Value = item.parameterValue.ToSafeString() }
                            };
                            pOrderFeeList.Add(parameterOrderFee);
                        }

                        SFFServices.ParameterList pOrderFee = new SFFServices.ParameterList();
                        pOrderFee.ParameterType = "OrderFee";
                        pOrderFee.ParameterList1 = pOrderFeeList.ToArray();

                        listoflist.Add(pOrderFee);

                    }

                    ///

                    SFFServices.Parameter[] objReqParam = new SFFServices.Parameter[]
                    {
                        new SFFServices.Parameter() { Name = "mobileNo", Value = OrdChangeServicePara.mobileNo.ToSafeString() },
                        new SFFServices.Parameter() { Name = "orderChannel", Value = OrdChangeServicePara.orderChannel.ToSafeString() },
                        new SFFServices.Parameter() { Name = "locationCd", Value = OrdChangeServicePara.locationCd.ToSafeString() },
                        new SFFServices.Parameter() { Name = "ascCode", Value = OrdChangeServicePara.ascCode.ToSafeString() },
                        new SFFServices.Parameter() { Name = "orderType", Value = OrdChangeServicePara.orderType.ToSafeString() },
                        new SFFServices.Parameter() { Name = "userName", Value = OrdChangeServicePara.userName.ToSafeString() },
                        new SFFServices.Parameter() { Name = "referenceNo", Value = OrdChangeServicePara.referenceNo.ToSafeString() },
                        new SFFServices.Parameter() { Name = "employeeID", Value = OrdChangeServicePara.employeeID.ToSafeString() }
                    };

                    objReq = new SFFServices.SffRequest()
                    {
                        Event = "evOMCreateOrderChangeService",
                        ParameterList = new SFFServices.ParameterList()
                        {
                            Parameter = objReqParam,
                            ParameterList1 = listoflist.ToArray()
                        }
                    };
                }

                if (objReq == null)
                {
                    canCreateObj = false;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objReq, log, "Success", "", "FBBWEB");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, new SFFServices.SffRequest(), log, "Failed", ex.Message, "FBBWEB");
                canCreateObj = false;
                command.VALIDATE_FLAG = "N";
                command.ERROR_MSG = ex.Message;
            }
            if (canCreateObj)
            {
                InterfaceLogCommand log2 = null;
                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, objReq, command.NonMobileNo, "SFFServices.SFFServiceService", "evOMCreateOrderChangeService", null, "FBB|" + command.FullUrl, "FBBOR016");
                try
                {
                    using (var service = new SFFServices.SFFServiceService())
                    {
                        var objResp = service.ExecuteService(objReq);
                        if (objResp.ErrorMessage == null)
                        {
                            foreach (var data in objResp.ParameterList.Parameter)
                            {
                                if (data.Name == "VALIDATE_FLAG")
                                {
                                    command.VALIDATE_FLAG = data.Value;
                                    if (data.Value != "Y")
                                    {
                                        foreach (var subdata in objResp.ParameterList.ParameterList1)
                                        {
                                            foreach (var error in subdata.Parameter)
                                            {
                                                if (error.Name == "ERROR_MASSAGE")
                                                {
                                                    command.ERROR_MSG = error.Value == null ? "" : error.Value;
                                                    break;
                                                }
                                            }
                                        }
                                        command.ERROR_MSG = "VALIDATE_FLAG != Y and not found ERROR_MESSAGE";
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", "VALIDATE_FLAG != Y and not found ERROR_MESSAGE", "FBBWEB");
                                    }
                                    else
                                    {
                                        command.ERROR_MSG = "";
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Success", "", "FBBWEB");
                                        break;
                                    }
                                }
                            }
                            if (command.ERROR_MSG == "")
                            {
                                foreach (var data in objResp.ParameterList.Parameter)
                                {
                                    if (data.Name.ToSafeString() == "ORDER_NO")
                                    {
                                        command.sffOrderNo = data.Value.ToSafeString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            command.VALIDATE_FLAG = "N";
                            command.ERROR_MSG = objResp.ErrorMessage;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, objResp, log2, "Failed", objResp.ErrorMessage, "FBBWEB");
                        }
                    }
                }
                catch (Exception ex)
                {
                    command.VALIDATE_FLAG = "N";
                    command.ERROR_MSG = ex.Message;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log2, "Failed", ex.Message, "FBBWEB");
                }
            }
            else
            {
                command.VALIDATE_FLAG = "N";
                command.ERROR_MSG = "Can not CreateObj";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", "Can not CreateObj", "FBBWEB");
            }

        }
    }
}
