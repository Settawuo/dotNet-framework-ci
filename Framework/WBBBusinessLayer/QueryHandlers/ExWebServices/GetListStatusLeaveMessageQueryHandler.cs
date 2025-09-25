using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetListStatusLeaveMessageQueryHandler : IQueryHandler<GetListStatusLeaveMessageQuery, ListStatusLeaveMessageResponse>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<StatusLeaveMessageModel> _objService;

        public GetListStatusLeaveMessageQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<StatusLeaveMessageModel> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public ListStatusLeaveMessageResponse Handle(GetListStatusLeaveMessageQuery query)
        {
            var listStatusLeaveMessage = new ListStatusLeaveMessageResponse();

            try
            {
                _logger.Info("GetListStatusLeaveMessageQueryHandler Start");
                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.Decimal;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.ParameterName = "return_message";
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var p_seacrh_status_name = new OracleParameter();
                p_seacrh_status_name.ParameterName = "p_seacrh_status_name";
                p_seacrh_status_name.OracleDbType = OracleDbType.RefCursor;
                p_seacrh_status_name.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                List<StatusLeaveMessageModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBOR021.PROC_SEARCH_STATUS",
                    new
                    {
                        p_cust_name = query.CustName.ToSafeString(),
                        p_cust_surname = query.CustSurname.ToSafeString(),
                        p_contact_mobile = query.ContactMobile.ToSafeString(),
                        p_refference_no = query.RefferenceNo.ToSafeString(),

                        // return code
                        return_code = return_code,
                        return_message = return_message,
                        p_seacrh_status_name = p_seacrh_status_name

                    }).ToList();

                if (executeResult.Count > 0) // return 0 pass value to screen 
                {
                    listStatusLeaveMessage.ReturnCode = return_code.Value.ToSafeString();
                    listStatusLeaveMessage.ReturnMessage = return_message.Value.ToSafeString();

                    if (return_code.Value.ToSafeString() == "0")
                    {
                        listStatusLeaveMessage.SearchStatusList = executeResult.Select(p =>
                                                {
                                                    return new StatusLeaveMessageList
                                                    {
                                                        RefferenceNo = p.Refference_No.ToSafeString(),
                                                        Language = p.Language.ToSafeString(),
                                                        ServiceSpeed = p.Service_Speed.ToSafeString(),
                                                        Name = p.Name.ToSafeString(),
                                                        ContactMobile = p.Contact_Mobile.ToSafeString(),
                                                        RecordDate = p.Record_Date.ToSafeString(),
                                                        Status = p.Status.ToSafeString(),
                                                        ContactCustomer = p.Contact_Customer.ToSafeString(),
                                                        CheckCoverage = p.Check_Coverage.ToSafeString(),
                                                        CustomerRegister = p.Customer_Register.ToSafeString()
                                                    };
                                                }).ToList();
                    }
                    else //return -1 error
                    {
                        listStatusLeaveMessage.SearchStatusList = new List<StatusLeaveMessageList>();
                    }

                    _logger.Info("End WBB.PKG_FBBOR021.PROC_SEARCH_STATUS output msg: " + return_message.Value.ToSafeString());
                    return listStatusLeaveMessage;
                }
                else
                {
                    listStatusLeaveMessage.ReturnCode = "-1";
                    listStatusLeaveMessage.ReturnMessage = "Error return -1 call service WBB.PKG_FBBOR021.PROC_SEARCH_STATUS output msg: " + return_message.Value.ToSafeString();
                    listStatusLeaveMessage.SearchStatusList = new List<StatusLeaveMessageList>();

                    _logger.Info("Error return -1 call service WBB.PKG_FBBOR021.PROC_SEARCH_STATUS output msg: " + return_message.Value.ToSafeString());
                    return listStatusLeaveMessage;
                }
            }
            catch (Exception ex)
            {
                listStatusLeaveMessage.ReturnCode = "-1";
                listStatusLeaveMessage.ReturnMessage = "Error call service WBB.PKG_FBBOR021.PROC_SEARCH_STATUS: " + ex.Message;
                listStatusLeaveMessage.SearchStatusList = new List<StatusLeaveMessageList>();
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBOR021.PROC_SEARCH_STATUS: " + ex.Message);
                return listStatusLeaveMessage;
            }
        }
    }
}
