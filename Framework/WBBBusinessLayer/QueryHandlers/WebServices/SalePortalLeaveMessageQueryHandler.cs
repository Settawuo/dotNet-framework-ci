using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class SalePortalLeaveMessageQueryHandler : IQueryHandler<SalePortalLeaveMessageQuery, List<SalePortalLeaveMessageList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SalePortalLeaveMessageList> _objService;

        public SalePortalLeaveMessageQueryHandler(ILogger logger, IEntityRepository<SalePortalLeaveMessageList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<SalePortalLeaveMessageList> Handle(SalePortalLeaveMessageQuery query)
        {
            try
            {
                _logger.Info("SalePortalLeaveMessageQueryHandler Start");
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "list_info_pre_register";
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                List<SalePortalLeaveMessageList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_LIST_INFO_PRE_REGISTER",
                            new
                            {
                                p_reg_date_from = query.p_reg_date_from,
                                p_reg_date_to = query.p_reg_date_to,
                                p_refference_no = query.p_refference_no,
                                p_customer_name = query.p_customer_name,
                                p_contact_mobile = query.p_contact_mobile,
                                p_channel = query.p_channel,
                                p_create_time_from = query.p_reg_time_from,
                                p_create_time_to = query.p_reg_time_to,
                                //  return code
                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                list_info_pre_register = ret_data

                            }).ToList();

                query.ret_code = 0;
                query.ret_msg = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_LIST_INFO_PRE_REGISTER" + ex.Message);
                query.ret_code = -1;
                query.ret_msg = "Error";

                return null;
            }

        }
    }

    public class SalePortalLeaveMessageByRefferenceNoQueryHandler : IQueryHandler<SalePortalLeaveMessageByRefferenceNoQuery, List<SalePortalLeaveMessageList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SalePortalLeaveMessageList> _objService;

        public SalePortalLeaveMessageByRefferenceNoQueryHandler(ILogger logger, IEntityRepository<SalePortalLeaveMessageList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<SalePortalLeaveMessageList> Handle(SalePortalLeaveMessageByRefferenceNoQuery query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "list_info_by_refno";
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                List<SalePortalLeaveMessageList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_LIST_INFO_BY_REFNO",
                            new
                            {

                                p_refference_no = query.p_refference_no,
                                //  return code
                                return_code = ret_code,
                                return_message = ret_msg,
                                list_info_by_refno = ret_data

                            }).ToList();

                query.return_code = 0;
                query.return_message = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_LIST_INFO_BY_REFNO" + ex.Message);
                query.return_code = -1;
                query.return_message = "Error";

                return null;
            }

        }
    }

    public class SalePortalLeaveMessageByRefferenceNoQueryHandler_IM : IQueryHandler<SalePortalLeaveMessageByRefferenceNoQuery_IM, List<SalePortalLeaveMessageList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SalePortalLeaveMessageList> _objService;

        public SalePortalLeaveMessageByRefferenceNoQueryHandler_IM(ILogger logger, IEntityRepository<SalePortalLeaveMessageList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<SalePortalLeaveMessageList> Handle(SalePortalLeaveMessageByRefferenceNoQuery_IM query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "list_info_by_refno";
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                List<SalePortalLeaveMessageList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_IM_LIST_INFO_BY_REFNO",
                            new
                            {

                                p_refference_no = query.p_refference_no,
                                //  return code
                                return_code = ret_code,
                                return_message = ret_msg,
                                list_info_by_refno = ret_data

                            }).ToList();

                query.return_code = 0;
                query.return_message = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBSALEPORTAL_PRE_REGISTER.PROC_IM_LIST_INFO_BY_REFNO" + ex.Message);
                query.return_code = -1;
                query.return_message = "Error";

                return null;
            }
        }
    }
}
