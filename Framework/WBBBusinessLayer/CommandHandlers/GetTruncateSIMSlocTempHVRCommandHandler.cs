using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.FBBShareplexModels;
using WBBEntity.Models;
using static WBBBusinessLayer.CommandHandlers.GetTruncateSIMSlocTempCommandHandler;

namespace WBBBusinessLayer.CommandHandlers
{
    public class GetTruncateSIMSlocTempHVRCommandHandler : ICommandHandler<TruncateSIMSlocTempHVRCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IFBBHVREntityRepository<WFS_TEAM_ATTR> _wfsTeamAttr;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public GetTruncateSIMSlocTempHVRCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IFBBHVREntityRepository<WFS_TEAM_ATTR> wfsTeamAttr,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _wfsTeamAttr = wfsTeamAttr;
            _intfLog = intfLog;
        }

        public void Handle(TruncateSIMSlocTempHVRCommand command)
        {
            List<FBBPAYG_SIM_SLOC_TEMP> list = new List<FBBPAYG_SIM_SLOC_TEMP>();

            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.Size = 2000;
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;

            #region fetch data from wfm_r8.wfs_team_attr
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [TruncateSIMSlocTempHVRCommand]", "get data : wfm_r8.wfs_team_attr", "GetTruncateSIMSlocTempHVRCommandHandler", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {
                var cur_data = new NpgsqlParameter();
                cur_data.ParameterName = "cur_data";
                cur_data.NpgsqlDbType = NpgsqlDbType.Refcursor;
                cur_data.Direction = ParameterDirection.InputOutput;
                cur_data.Value = "cur_data";

                var executeResult = _wfsTeamAttr.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbbpayg_loadsim_p_get_ship_to_sloc",
                  new object[]
                  {
                       cur_data
                  });


                if (executeResult.Any())
                {
                    DataTable resp = (DataTable)executeResult[0];
                    foreach (DataRow row in resp.Rows)
                    {
                        FBBPAYG_SIM_SLOC_TEMP model = new FBBPAYG_SIM_SLOC_TEMP
                        {
                            SHIP_ID = row["ship_to"].ToString(),
                            STORAGE_LOCATION = row["stage_local"].ToString(),
                            CREATE_DATE = DateTime.TryParse(row["create_date"].ToString(), out DateTime tempDate) ? (DateTime?)tempDate : null,
                            CREATE_BY = "FBBPAYGLoadSIM",
                        };
                        list.Add(model);
                    }
                }
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", log, "Success", "", "FBB_BATCH");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info("wfm_r8.wfs_team_attr Exception : " + ex.GetErrorMessage());
            }
            #endregion

            #region create list cur for send to pk
            InterfaceLogPayGCommand log2 = new InterfaceLogPayGCommand();
            log2 = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [TruncateSIMSlocTempHVRCommand]", "call package : PKG_FBBPAYG_LOAD_SIM_TEST.p_clearsert_sim_sloc_temp", "GetTruncateSIMSlocTempHVRCommandHandler", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {
                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBBPAYG_SIM_SLOC_TEMP_ARRAY = list.Select(a => new FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping
                    {
                        SHIP_ID = a.SHIP_ID.ToSafeString(),
                        STORAGE_LOCATION = a.STORAGE_LOCATION.ToSafeString(),
                        CREATE_BY = a.CREATE_BY.ToSafeString()
                    }).ToArray()
                };


                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("P_FBBPAYG_SIM_SLOC_TEMP", "FBBPAYG_SIM_SLOC_TEMP_ARRAY", packageMappingObjectModel);

                var executeResults = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBPAYG_LOAD_SIM.p_clearsert_sim_sloc_temp",
                          new object[]
                      {
                          //Parameter Input
                          packageMapping,

                          //Parameter Output
                          ret_code,
                          ret_msg
                      });

                //Return
                command.RET_CODE = ret_code.Value.ToSafeString();
                command.RET_MSG = ret_msg.Value.ToSafeString();
                command.Total = list.Count;

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, executeResults, log2, ret_code.Value.ToSafeString().Equals("0") ? "Success" : ret_msg.Value.ToSafeString(), "", "FBB_BATCH");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log2, "Error", ex.ToSafeString(), "FBB_BATCH");
                _logger.Info("PKG_FBBPAYG_LOAD_SIM_TEST.p_clearsert_sim_sloc_temp : " + ex.GetErrorMessage());
            }
            #endregion
        }

    }
}