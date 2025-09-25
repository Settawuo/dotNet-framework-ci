using AIRNETEntity.Extensions;
using AIRNETEntity.StoredProc;
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
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetTrackingQueryHandler : IQueryHandler<GetTrackingQuery, List<TrackingModel>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<TrackingModel> _trackingModel;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetTrackingQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<TrackingModel> trackingModel)
        {
            _logger = logger;
            _trackingModel = trackingModel;
            _intfLog = intfLog;
            _uow = uow;
        }

        public List<TrackingModel> Handle(GetTrackingQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var v_error_msg = new OracleParameter();
            v_error_msg.OracleDbType = OracleDbType.Varchar2;
            v_error_msg.Size = 2000;
            v_error_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var cursor = new OracleParameter();
            cursor.OracleDbType = OracleDbType.RefCursor;
            cursor.Direction = ParameterDirection.Output;

            List<TrackingModel> executeResult = new List<TrackingModel>();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_First_Name + "|" + query.P_Last_Name, "PROC_LIST_ORD", "GetTracking", query.P_Id_Card, "FBB", "WEB");

                // 3209800123027
                executeResult = _trackingModel.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_LIST_ORD",
                   new
                   {
                       p_id_card = query.P_Id_Card,
                       p_first_name = query.P_First_Name,
                       p_last_name = query.P_Last_Name,
                       p_location_code = query.P_Location_Code,
                       p_asc_code = query.P_Asc_Code,
                       p_date_from = query.P_Date_From,
                       p_date_to = query.P_Date_To,
                       p_cust_name = query.P_Cust_Name,
                       p_user = query.P_User,

                       ret_code = ret_code,
                       ret_msg = v_error_msg,
                       ret_data = cursor

                   }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                //var Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                //var Return_Desc = v_error_msg.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return executeResult;


        }

    }
}
