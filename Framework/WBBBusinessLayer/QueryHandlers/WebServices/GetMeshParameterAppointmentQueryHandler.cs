using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetMeshParameterAppointmentQueryHandler : IQueryHandler<GetMeshParameterAppointmentQuery, MeshParameterAppointmentModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<object> _objService;

        public GetMeshParameterAppointmentQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
            _objService = objService;
        }

        public MeshParameterAppointmentModel Handle(GetMeshParameterAppointmentQuery query)
        {
            MeshParameterAppointmentModel executeResults = new MeshParameterAppointmentModel();
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.nonMobile, "GetMeshParameterAppointmentQuery", "FBBService", null, "FBB", "");

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

                var CHANNEL = new OracleParameter();
                CHANNEL.ParameterName = "CHANNEL";
                CHANNEL.Size = 2000;
                CHANNEL.OracleDbType = OracleDbType.Varchar2;
                CHANNEL.Direction = ParameterDirection.Input;
                CHANNEL.Value = query.Channel;

                var INSTALLATION_DATE = new OracleParameter();
                INSTALLATION_DATE.ParameterName = "INSTALLATION_DATE";
                INSTALLATION_DATE.Size = 2000;
                INSTALLATION_DATE.OracleDbType = OracleDbType.Varchar2;
                INSTALLATION_DATE.Direction = ParameterDirection.Input;
                INSTALLATION_DATE.Value = query.installDate;

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

                var OFFICER_CHANNEL = new OracleParameter();
                OFFICER_CHANNEL.ParameterName = "OFFICER_CHANNEL";
                OFFICER_CHANNEL.Size = 2000;
                OFFICER_CHANNEL.OracleDbType = OracleDbType.Varchar2;
                OFFICER_CHANNEL.Direction = ParameterDirection.Input;
                OFFICER_CHANNEL.Value = query.officerChannel;

                var FLAG_CALL = new OracleParameter();
                FLAG_CALL.ParameterName = "FLAG_CALL";
                FLAG_CALL.Size = 2000;
                FLAG_CALL.OracleDbType = OracleDbType.Varchar2;
                FLAG_CALL.Direction = ParameterDirection.Input;
                FLAG_CALL.Value = query.flag_call;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "return_code";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "return_message";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_APPOINTMENT = new OracleParameter();
                RETURN_APPOINTMENT.ParameterName = "RETURN_APPOINTMENT";
                RETURN_APPOINTMENT.OracleDbType = OracleDbType.RefCursor;
                RETURN_APPOINTMENT.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR041.APPOINTMENT");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.APPOINTMENT",
                    new object[]
                    {
                         NON_MOBILE,
                         P_ADDRESS_ID,
                         CHANNEL,
                         INSTALLATION_DATE,
                         LANQUAGE_SCREEN,
                         INSTALL_ADDRESS_1,
                         INSTALL_ADDRESS_2,
                         INSTALL_ADDRESS_3,
                         INSTALL_ADDRESS_4,
                         INSTALL_ADDRESS_5,
                         OUTSERVICELEVEL,
                         OPTION_MESH,
                         OFFICER_CHANNEL,
                         FLAG_CALL,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_APPOINTMENT
                    });

                executeResults.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";
                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.RES_COMPLETE_CUR = data1.DataTableToList<ParameterAppointment>();
                    if (executeResults.RES_COMPLETE_CUR != null && executeResults.RES_COMPLETE_CUR.Count > 0)
                    {
                        var inputparam = (from z in _lov.Get()
                                          where z.LOV_NAME == "MESH-JOIN" && z.LOV_TYPE == "TIME_SLOT_MESH" && z.ACTIVEFLAG == "Y"
                                          select z).ToList();
                        if (inputparam != null && inputparam.Count > 0)
                        {
                            foreach (var item in executeResults.RES_COMPLETE_CUR)
                            {
                                item.TIME_SLOT_REGISTER_HR = inputparam.FirstOrDefault().LOV_VAL3.ToSafeString();
                                item.TIME_SLOT_REGISTER_FLOWACTION_NO = inputparam.FirstOrDefault().LOV_VAL4.ToSafeString();
                            }
                        }

                    }
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RES_COMPLETE_CUR, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR041.APPOINTMENT handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
