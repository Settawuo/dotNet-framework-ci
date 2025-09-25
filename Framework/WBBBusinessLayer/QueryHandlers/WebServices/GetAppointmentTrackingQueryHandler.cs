using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebService
{
    public class GetAppointmentTrackingQueryHandler : IQueryHandler<GetAppointmentTrackingQuery, List<AppointmentDisplayTrackingList>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AppointmentDisplayTrackingList> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetAppointmentTrackingQueryHandler(ILogger logger, IWBBUnitOfWork uow, IAirNetEntityRepository<AppointmentDisplayTrackingList> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public List<AppointmentDisplayTrackingList> Handle(GetAppointmentTrackingQuery query)
        {
            _logger.Info("GetAppointmentTrackingQueryHandler Start");

            var output_return_code = new OracleParameter();
            output_return_code.ParameterName = "OUTPUT_return_code";
            output_return_code.OracleDbType = OracleDbType.Decimal;
            output_return_code.Direction = ParameterDirection.Output;

            var output_return_message = new OracleParameter();
            output_return_message.ParameterName = "OUTPUT_return_message";
            output_return_message.OracleDbType = OracleDbType.Varchar2;
            output_return_message.Size = 2000;
            output_return_message.Direction = ParameterDirection.Output;

            var p_display_to_screen = new OracleParameter();
            p_display_to_screen.ParameterName = "P_DISPLAY_TO_SCREEN";
            p_display_to_screen.OracleDbType = OracleDbType.RefCursor;
            p_display_to_screen.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            try
            {
                List<AppointmentDisplayTrackingList> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_APPOINTMENT_DISPLAY.PROC_DISPLAY_TRACKING",
                     new
                     {
                         p_order_no = query.order_no,
                         p_id_card_no = query.id_card_no,
                         p_non_mobile_no = query.non_mobile_no,

                         // return code
                         output_return_code = output_return_code,
                         output_return_message = output_return_message,
                         p_display_to_screen = p_display_to_screen

                     }).ToList();

                if (output_return_code.Value.ToSafeString() == "0") // return 0 pass value to screen 
                {
                    _logger.Info("End AIR_ADMIN.PKG_APPOINTMENT_DISPLAY.PROC_DISPLAY_TRACKING output msg: " + query.output_return_message);
                    return executeResult;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service AIR_ADMIN.PKG_APPOINTMENT_DISPLAY.PROC_DISPLAY_TRACKING output msg: " + output_return_message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBBULK_CORP_REGISTER.PROC_SEARCH_BULK_NUMBER" + ex.Message);
                return null;
            }
        }
    }
}
