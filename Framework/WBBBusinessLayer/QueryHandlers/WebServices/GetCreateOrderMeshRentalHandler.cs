using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetCreateOrderMeshRentalHandler : IQueryHandler<GetCreateOrderMeshRentalQuery, GetCreateOrderMeshRentalModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _obj;

        public GetCreateOrderMeshRentalHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> obj
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _obj = obj;
        }

        public GetCreateOrderMeshRentalModel Handle(GetCreateOrderMeshRentalQuery query)
        {
            SFFServices.SffRequest objReq = null;
            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;
            GetCreateOrderMeshRentalModel result = new GetCreateOrderMeshRentalModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_internet_no,
                    "GetCreateOrderMeshRental", "GetCreateOrderMeshRentalHandler", "", "FBB", "");

                var p_internet_no = new OracleParameter();
                p_internet_no.ParameterName = "p_internet_no";
                p_internet_no.OracleDbType = OracleDbType.Varchar2;
                p_internet_no.Size = 2000;
                p_internet_no.Direction = ParameterDirection.Input;
                p_internet_no.Value = query.p_internet_no;

                var p_payment_order_id = new OracleParameter();
                p_payment_order_id.ParameterName = "p_payment_order_id";
                p_payment_order_id.OracleDbType = OracleDbType.Varchar2;
                p_payment_order_id.Size = 2000;
                p_payment_order_id.Direction = ParameterDirection.Input;
                p_payment_order_id.Value = query.p_payment_order_id;

                var p_penalty_install = new OracleParameter();
                p_penalty_install.ParameterName = "p_penalty_install";
                p_penalty_install.OracleDbType = OracleDbType.Varchar2;
                p_penalty_install.Size = 2000;
                p_penalty_install.Direction = ParameterDirection.Input;
                p_penalty_install.Value = query.p_penalty_install;

                //R22.11 Mesh with arpu
                var p_point = new OracleParameter();
                p_point.ParameterName = "p_point";
                p_point.OracleDbType = OracleDbType.Varchar2;
                p_point.Size = 2000;
                p_point.Direction = ParameterDirection.Input;
                p_point.Value = query.p_point;

                //R22.11 Mesh with arpu
                var p_flag_option = new OracleParameter();
                p_flag_option.ParameterName = "p_flag_option";
                p_flag_option.OracleDbType = OracleDbType.Varchar2;
                p_flag_option.Size = 2000;
                p_flag_option.Direction = ParameterDirection.Input;
                p_flag_option.Value = query.p_flag_option;

                //R22.11 Mesh with arpu
                var p_flag_mesh = new OracleParameter();
                p_flag_mesh.ParameterName = "p_flag_mesh";
                p_flag_mesh.OracleDbType = OracleDbType.Varchar2;
                p_flag_mesh.Size = 2000;
                p_flag_mesh.Direction = ParameterDirection.Input;
                p_flag_mesh.Value = query.p_flag_mesh;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var list_ord_service_rental = new OracleParameter();
                list_ord_service_rental.ParameterName = "list_ord_service_rental";
                list_ord_service_rental.OracleDbType = OracleDbType.RefCursor;
                list_ord_service_rental.Direction = ParameterDirection.Output;

                var list_ord_service = new OracleParameter();
                list_ord_service.ParameterName = "list_ord_service";
                list_ord_service.OracleDbType = OracleDbType.RefCursor;
                list_ord_service.Direction = ParameterDirection.Output;

                var list_ord_appoint_attribute = new OracleParameter();
                list_ord_appoint_attribute.ParameterName = "list_ord_appoint_attribute";
                list_ord_appoint_attribute.OracleDbType = OracleDbType.RefCursor;
                list_ord_appoint_attribute.Direction = ParameterDirection.Output;

                var list_ord_rental_attribute = new OracleParameter();
                list_ord_rental_attribute.ParameterName = "list_ord_rental_attribute";
                list_ord_rental_attribute.OracleDbType = OracleDbType.RefCursor;
                list_ord_rental_attribute.Direction = ParameterDirection.Output;

                var list_ordderfee = new OracleParameter();
                list_ordderfee.ParameterName = "list_ordderfee";
                list_ordderfee.OracleDbType = OracleDbType.RefCursor;
                list_ordderfee.Direction = ParameterDirection.Output;

                var resultExecute = _obj.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.CREATE_ORDER_MESH_RENTAL",
                      new object[]
                      {
                          //Parameter Input
                          p_internet_no,
                          p_payment_order_id,
                          p_penalty_install,
                          p_point,
                          p_flag_option,
                          p_flag_mesh,
                          //Parameter Output
                          ret_code,
                          ret_message,
                          list_ord_service_rental,
                          list_ord_service,
                          list_ord_appoint_attribute,
                          list_ord_rental_attribute,
                          list_ordderfee
                      });

                if (resultExecute != null)
                {

                    result.RespCode = resultExecute[0] != null ? resultExecute[0].ToString() : "-1";
                    result.RespDesc = resultExecute[1] != null ? resultExecute[1].ToString() : "";

                    DataTable dtOrdServiceRentalRespones = (DataTable)resultExecute[2];
                    List<OrdServiceRentalData> OrdServiceRentalList = dtOrdServiceRentalRespones.DataTableToList<OrdServiceRentalData>();
                    result.list_ord_service_rental = OrdServiceRentalList;

                    DataTable dtOrdServiceRespones = (DataTable)resultExecute[3];
                    List<OrdServiceData> OrdServiceList = dtOrdServiceRespones.DataTableToList<OrdServiceData>();
                    result.list_ord_service = OrdServiceList;

                    DataTable dtOrdServiceAppointAttributeRespones = (DataTable)resultExecute[4];
                    List<OrdServiceAppointAttributeData> OrdServiceAppointAttributeList = dtOrdServiceAppointAttributeRespones.DataTableToList<OrdServiceAppointAttributeData>();
                    result.list_ord_appoint_attribute = OrdServiceAppointAttributeList;

                    DataTable dtOrdServiceRentalAttributeRespones = (DataTable)resultExecute[5];
                    List<OrdServiceRentalAttributeData> OrdServiceRentalAttributeList = dtOrdServiceRentalAttributeRespones.DataTableToList<OrdServiceRentalAttributeData>();
                    result.list_ord_rental_attribute = OrdServiceRentalAttributeList;

                    DataTable dtOrdderfeeRespones = (DataTable)resultExecute[6];
                    List<OrdderfeeData> OrdderfeeList = dtOrdderfeeRespones.DataTableToList<OrdderfeeData>();
                    result.list_ordderfee = OrdderfeeList;

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultExecute, log, "Success", "", "");

                    try
                    {
                        log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_internet_no,
                                "GetCreateOrderMeshRentalSetXML", "GetCreateOrderMeshRentalHandler", "", "FBB", "");
                        if (OrdServiceRentalList != null && OrdServiceList != null && OrdServiceAppointAttributeList != null
                            && OrdServiceRentalAttributeList != null && OrdderfeeList != null
                            && OrdServiceRentalList.Count > 0 && OrdServiceList.Count > 0 && OrdServiceAppointAttributeList.Count > 0
                            && OrdServiceRentalAttributeList.Count > 0 && OrdderfeeList.Count > 0
                            )
                        {
                            OrdServiceRentalData OrdServiceRentalPara = new OrdServiceRentalData();
                            OrdServiceRentalPara = OrdServiceRentalList.FirstOrDefault();
                            string ServiceCodeOrdAppoint = OrdServiceList.FirstOrDefault(t => t.curAttriService == "list_ord_appoint_attribute").serviceCode.ToSafeString();
                            string ServiceCodeOrdRental = OrdServiceList.FirstOrDefault(t => t.curAttriService == "list_ord_rental_attribute").serviceCode.ToSafeString();

                            List<SFFServices.ParameterList> listoflist = new List<SFFServices.ParameterList>();

                            /// Add Service Rental Para
                            SFFServices.ParameterList pServiceRental = new SFFServices.ParameterList();
                            pServiceRental.Parameter = new SFFServices.Parameter[]
                                {
                                   new SFFServices.Parameter() { Name = "actionStatus", Value = OrdServiceRentalPara.actionStatus.ToSafeString() },
                                   new SFFServices.Parameter() { Name = "serviceCode", Value = ServiceCodeOrdRental }
                                };

                            //serviceAttribute
                            if (OrdServiceRentalAttributeList != null && OrdServiceRentalAttributeList.Count > 0)
                            {
                                List<SFFServices.ParameterList> pServiceAttributeList = new List<SFFServices.ParameterList>();
                                foreach (var item in OrdServiceRentalAttributeList)
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
                                pServiceRental.ParameterList1 = pServiceAttributeList.ToArray();
                                listoflist.Add(pServiceRental);
                            }
                            ///

                            /// Add list_ord_appoint_attribute
                            SFFServices.ParameterList pServiceAppointment = new SFFServices.ParameterList();
                            pServiceAppointment.Parameter = new SFFServices.Parameter[]
                                {
                                   new SFFServices.Parameter() { Name = "actionStatus", Value = OrdServiceRentalPara.actionStatus.ToSafeString() },
                                   new SFFServices.Parameter() { Name = "serviceCode", Value = ServiceCodeOrdAppoint }
                                };

                            //serviceAttribute
                            if (OrdServiceAppointAttributeList != null && OrdServiceAppointAttributeList.Count > 0)
                            {
                                List<SFFServices.ParameterList> pServiceAttributeList = new List<SFFServices.ParameterList>();
                                foreach (var item in OrdServiceAppointAttributeList)
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

                            /// Add OrderFee
                            if (OrdderfeeList != null && OrdderfeeList.Count > 0)
                            {
                                List<SFFServices.ParameterList> pOrderFeeList = new List<SFFServices.ParameterList>();
                                foreach (var item in OrdderfeeList)
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
                                new SFFServices.Parameter() { Name = "mobileNo", Value = OrdServiceRentalPara.mobileNo.ToSafeString() },
                                new SFFServices.Parameter() { Name = "orderChannel", Value = OrdServiceRentalPara.orderChannel.ToSafeString() },
                                new SFFServices.Parameter() { Name = "locationCd", Value = OrdServiceRentalPara.locationCd.ToSafeString() },
                                new SFFServices.Parameter() { Name = "ascCode", Value = OrdServiceRentalPara.ascCode.ToSafeString() },
                                new SFFServices.Parameter() { Name = "orderType", Value = OrdServiceRentalPara.orderType.ToSafeString() },
                                new SFFServices.Parameter() { Name = "userName", Value = OrdServiceRentalPara.userName.ToSafeString() },
                                new SFFServices.Parameter() { Name = "referenceNo", Value = OrdServiceRentalPara.referenceNo.ToSafeString() },
                                new SFFServices.Parameter() { Name = "employeeID", Value = OrdServiceRentalPara.employeeID.ToSafeString() }
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
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log2, "Failed", "XMLObj IS Null", "");
                        }
                        else
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, objReq, log2, "Success", "", "");

                            InterfaceLogCommand log3 = null;
                            log3 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_internet_no,
                                    "GetCreateOrderMeshRentalSFFServiceService", "GetCreateOrderMeshRentalHandler", "", "FBB", "");
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
                                                if (data.Value != "Y")
                                                {
                                                    foreach (var subdata in objResp.ParameterList.ParameterList1)
                                                    {
                                                        foreach (var error in subdata.Parameter)
                                                        {
                                                            if (error.Name == "ERROR_MASSAGE")
                                                            {
                                                                result.RespDesc = error.Value == null ? "" : error.Value;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    result.RespCode = "-1";
                                                    result.RespDesc = "VALIDATE_FLAG != Y and not found ERROR_MESSAGE";
                                                }
                                                else
                                                {
                                                    result.RespDesc = "";
                                                    break;
                                                }
                                            }
                                        }
                                        if (result.RespDesc == "")
                                        {
                                            foreach (var data in objResp.ParameterList.Parameter)
                                            {
                                                if (data.Name.ToSafeString() == "ORDER_NO")
                                                {
                                                    result.RespCode = "0";
                                                    result.sffOrderNo = data.Value.ToSafeString();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            result.RespCode = "-1";
                                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, objResp, log3, "Failed", result.RespDesc, "FBBWEB");
                                        }

                                        if (result.sffOrderNo != null && result.sffOrderNo != "")
                                        {
                                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, objResp, log3, "Success", "", "");
                                        }
                                    }
                                    else
                                    {
                                        result.RespCode = "-1";
                                        result.RespDesc = objResp.ErrorMessage;
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, objResp, log3, "Failed", objResp.ErrorMessage, "FBBWEB");
                                    }
                                }
                            }
                            catch (Exception ex1)
                            {
                                result.RespCode = "-1";
                                result.RespDesc = ex1.Message;
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log3, "Failed", ex1.Message, "");
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        result.RespCode = "-1";
                        result.RespDesc = ex2.Message;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log2, "Failed", ex2.Message, "");
                    }
                }
                else
                {
                    result.RespCode = "-1";
                    result.RespDesc = "resultExecute is null";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", "resultExecute is null", "");
                }

            }
            catch (Exception ex)
            {
                result.RespCode = "-1";
                result.RespDesc = ex.Message.ToSafeString();
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }

    }
}
