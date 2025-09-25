using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetMeshParameterReserveQueryHandler : IQueryHandler<GetMeshParameterReserveQuery, MeshParameterReserveModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetMeshParameterReserveQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public MeshParameterReserveModel Handle(GetMeshParameterReserveQuery query)
        {
            MeshParameterReserveModel executeResults = new MeshParameterReserveModel();
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.nonMobile, "GetMeshParameterReserveQuery", "FBBService", null, "FBB", "");

            try
            {
                var NON_MOBILE = new OracleParameter();
                NON_MOBILE.ParameterName = "NON_MOBILE";
                NON_MOBILE.Size = 2000;
                NON_MOBILE.OracleDbType = OracleDbType.Varchar2;
                NON_MOBILE.Direction = ParameterDirection.Input;
                NON_MOBILE.Value = query.nonMobile;

                var P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "P_ADDRESS_ID";
                P_ADDRESS_ID.Size = 2000;
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = query.p_addressID;

                var PROMOTION_MAIN = new OracleParameter();
                PROMOTION_MAIN.ParameterName = "PROMOTION_MAIN";
                PROMOTION_MAIN.Size = 2000;
                PROMOTION_MAIN.OracleDbType = OracleDbType.Varchar2;
                PROMOTION_MAIN.Direction = ParameterDirection.Input;
                PROMOTION_MAIN.Value = query.promotionMain;

                var APPOINTMENT_DATE = new OracleParameter();
                APPOINTMENT_DATE.ParameterName = "APPOINTMENT_DATE";
                APPOINTMENT_DATE.Size = 2000;
                APPOINTMENT_DATE.OracleDbType = OracleDbType.Varchar2;
                APPOINTMENT_DATE.Direction = ParameterDirection.Input;
                APPOINTMENT_DATE.Value = query.appointmentDate;

                var TIME_SLOT = new OracleParameter();
                TIME_SLOT.ParameterName = "TIME_SLOT";
                TIME_SLOT.Size = 2000;
                TIME_SLOT.OracleDbType = OracleDbType.Varchar2;
                TIME_SLOT.Direction = ParameterDirection.Input;
                TIME_SLOT.Value = query.timeSlot;

                var LANQUAGE_SCREEN = new OracleParameter();
                LANQUAGE_SCREEN.ParameterName = "LANQUAGE_SCREEN";
                LANQUAGE_SCREEN.Size = 2000;
                LANQUAGE_SCREEN.OracleDbType = OracleDbType.Varchar2;
                LANQUAGE_SCREEN.Direction = ParameterDirection.Input;
                LANQUAGE_SCREEN.Value = query.language;

                var INSTALL_ADDRESS_1 = new OracleParameter();
                INSTALL_ADDRESS_1.ParameterName = "INSTALL_ADDRESS_1";
                INSTALL_ADDRESS_1.Size = 2000;
                INSTALL_ADDRESS_1.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_1.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_1.Value = query.installAddress1;

                var INSTALL_ADDRESS_2 = new OracleParameter();
                INSTALL_ADDRESS_2.ParameterName = "INSTALL_ADDRESS_2";
                INSTALL_ADDRESS_2.Size = 2000;
                INSTALL_ADDRESS_2.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_2.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_2.Value = query.installAddress2;

                var INSTALL_ADDRESS_3 = new OracleParameter();
                INSTALL_ADDRESS_3.ParameterName = "INSTALL_ADDRESS_3";
                INSTALL_ADDRESS_3.Size = 2000;
                INSTALL_ADDRESS_3.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_3.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_3.Value = query.installAddress3;

                var INSTALL_ADDRESS_4 = new OracleParameter();
                INSTALL_ADDRESS_4.ParameterName = "INSTALL_ADDRESS_4";
                INSTALL_ADDRESS_4.Size = 2000;
                INSTALL_ADDRESS_4.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_4.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_4.Value = query.installAddress4;

                var INSTALL_ADDRESS_5 = new OracleParameter();
                INSTALL_ADDRESS_5.ParameterName = "INSTALL_ADDRESS_5";
                INSTALL_ADDRESS_5.Size = 2000;
                INSTALL_ADDRESS_5.OracleDbType = OracleDbType.Varchar2;
                INSTALL_ADDRESS_5.Direction = ParameterDirection.Input;
                INSTALL_ADDRESS_5.Value = query.installAddress5;

                // R20.5 add by Aware : Atipon Wiparsmongkol
                var OUTSERVICELEVEL = new OracleParameter();
                OUTSERVICELEVEL.ParameterName = "OUTSERVICELEVEL";
                OUTSERVICELEVEL.Size = 2000;
                OUTSERVICELEVEL.OracleDbType = OracleDbType.Varchar2;
                OUTSERVICELEVEL.Direction = ParameterDirection.Input;
                OUTSERVICELEVEL.Value = query.outServiceLevel;

                var OPTION_MESH = new OracleParameter();
                OPTION_MESH.ParameterName = "OPTION_MESH";
                OPTION_MESH.Size = 2000;
                OPTION_MESH.OracleDbType = OracleDbType.Varchar2;
                OPTION_MESH.Direction = ParameterDirection.Input;
                OPTION_MESH.Value = query.optionMesh;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "return_code";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "return_message";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_CALL = new OracleParameter();
                RETURN_CALL.ParameterName = "return_call";
                RETURN_CALL.OracleDbType = OracleDbType.RefCursor;
                RETURN_CALL.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR041.RESERVE_TIMESLOT");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.RESERVE_TIMESLOT",
                    new object[]
                    {
                         NON_MOBILE,
                         P_ADDRESS_ID,
                         PROMOTION_MAIN,
                         APPOINTMENT_DATE,
                         TIME_SLOT,
                         LANQUAGE_SCREEN,
                         INSTALL_ADDRESS_1,
                         INSTALL_ADDRESS_2,
                         INSTALL_ADDRESS_3,
                         INSTALL_ADDRESS_4,
                         INSTALL_ADDRESS_5,
                         OUTSERVICELEVEL,
                         OPTION_MESH,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_CALL
                    });

                executeResults.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";
                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.RES_COMPLETE_CUR = data1.DataTableToList<ParameterReserve>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RES_COMPLETE_CUR, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR041.RESERVE_TIMESLOT handles : " + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.StackTrace, "");
                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
