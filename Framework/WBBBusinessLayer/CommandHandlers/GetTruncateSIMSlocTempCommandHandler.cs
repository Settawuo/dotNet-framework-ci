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

namespace WBBBusinessLayer.CommandHandlers
{
    public class GetTruncateSIMSlocTempCommandHandler : ICommandHandler<TruncateSIMSlocTempCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IFBBShareplexEntityRepository<WFS_TEAM_ATTR> _wfsTeamAttr;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public GetTruncateSIMSlocTempCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IFBBShareplexEntityRepository<WFS_TEAM_ATTR> wfsTeamAttr,
            IEntityRepository<object> objService,
           IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _wfsTeamAttr = wfsTeamAttr;
            _intfLog = intfLog;
        }

        public void Handle(TruncateSIMSlocTempCommand command)
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
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [TruncateSIMSlocTempCommand]", "get data : wfm_r8.wfs_team_attr", "GetTruncateSIMSlocTempCommandHandler", "", "FBBPAYGLoadSIM", "FBB_BATCH");
            try
            {
                //get ship_id for loop delete
                var result = (from wfs in _wfsTeamAttr.Get()
                              select new
                              {
                                  SHIP_TO = wfs.SHIP_TO,
                                  STAGE_LOCAL = wfs.STAGE_LOCAL,
                                  CREATE_DATE = wfs.CREATE_DATE
                              });
                if (result.Any())
                {
                    //modified model 
                    //int k = 0;
                    foreach (var item in result)
                    {
                        FBBPAYG_SIM_SLOC_TEMP model = new FBBPAYG_SIM_SLOC_TEMP()
                        {
                            SHIP_ID = item.SHIP_TO.ToSafeString(),
                            STORAGE_LOCATION = item.STAGE_LOCAL.ToSafeString(),
                            //SHIP_ID = k.ToSafeString(),
                            //STORAGE_LOCATION = k.ToSafeString(),
                            CREATE_DATE = item.CREATE_DATE,
                            CREATE_BY = "FBBPAYGLoadSIM",
                        };
                        //k++;
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
            log2 = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, "in_report_name [TruncateSIMSlocTempCommand]", "call package : PKG_FBBPAYG_LOAD_SIM_TEST.p_clearsert_sim_sloc_temp", "GetTruncateSIMSlocTempCommandHandler", "", "FBBPAYGLoadSIM", "FBB_BATCH");
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

        #region PackageMappingObjectModel
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping[] FBBPAYG_SIM_SLOC_TEMP_ARRAY { get; set; }


            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModel Null
            {
                get
                {
                    var obj = new PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBBPAYG_SIM_SLOC_TEMP_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBBPAYG_SIM_SLOC_TEMP_ARRAY = (FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBBPAYG_SIM_SLOC_TEMP_RECORD")]
        public class FBBOR045_ACIM_ARRAY_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping();
            }
        }

        [OracleCustomTypeMapping("FBBPAYG_SIM_SLOC_TEMP_ARRAY")]
        public class FBBOR045_ACIM_ARRAY_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("SHIP_ID")]
            public string SHIP_ID { get; set; }

            [OracleObjectMapping("STORAGE_LOCATION")]
            public string STORAGE_LOCATION { get; set; }

            [OracleObjectMapping("CREATE_BY")]
            public string CREATE_BY { get; set; }

            #endregion Attribute Mapping

            public static FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping Null
            {
                get
                {
                    var obj = new FBBPAYG_SIM_SLOC_TEMP_ARRAY_Mapping();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, "SHIP_ID", SHIP_ID);
                OracleUdt.SetValue(con, udt, "STORAGE_LOCATION", STORAGE_LOCATION);
                OracleUdt.SetValue(con, udt, "CREATE_BY", CREATE_BY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}