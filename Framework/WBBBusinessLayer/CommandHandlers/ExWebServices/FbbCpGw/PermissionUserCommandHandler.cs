using AIRNETEntity.Extensions;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw
{
    public class PermissionUserCommandHandler : ICommandHandler<PermissionUserCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public PermissionUserCommandHandler(ILogger logger,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(PermissionUserCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command.P_FBBOR045_ACIM_ARRAY, command.P_TRANSACTION_NO, "PermissionUserCommandHandler", "PermissionUserCommandHandler", command.P_TRANSACTION_NO, "FBB", "WEB");

                var P_TRANSACTION_NO = new OracleParameter();
                P_TRANSACTION_NO.ParameterName = "P_TRANSACTION_NO";
                P_TRANSACTION_NO.OracleDbType = OracleDbType.Varchar2;
                P_TRANSACTION_NO.Size = 1000;
                P_TRANSACTION_NO.Direction = ParameterDirection.Input;
                P_TRANSACTION_NO.Value = command.P_TRANSACTION_NO.ToSafeString();

                var P_ACTION = new OracleParameter();
                P_ACTION.ParameterName = "P_ACTION";
                P_ACTION.OracleDbType = OracleDbType.Varchar2;
                P_ACTION.Size = 1000;
                P_ACTION.Direction = ParameterDirection.Input;
                P_ACTION.Value = command.P_ACTION.ToSafeString();

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "P_RETURN_CODE";
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Size = 2000;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "P_RETURN_MESSAGE";
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Size = 2000;
                p_return_message.Direction = ParameterDirection.Output;

                var IO_PROCESS_FAIL = new OracleParameter();
                IO_PROCESS_FAIL.ParameterName = "IO_PROCESS_FAIL";
                IO_PROCESS_FAIL.OracleDbType = OracleDbType.RefCursor;
                IO_PROCESS_FAIL.Direction = ParameterDirection.Output;


                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBBOR045_ACIM_ARRAY =
                      command.P_FBBOR045_ACIM_ARRAY.Select(
                          a => new FBBOR045_ACIM_ARRAY_Mapping
                          {
                              USER_NAME = a.USER_NAME,
                              FIRST_NAME = a.FIRST_NAME,
                              LAST_NAME = a.LAST_NAME,
                              EMAIL = a.EMAIL,
                              ROLE = a.ROLE,
                              ROLE_PAST = a.ROLE_PAST,
                              PERIOD = a.PERIOD,
                              START_DATE = a.START_DATE,
                              END_DATE = a.END_DATE,
                              LOCATION_CODE = a.LOCATION_CODE,
                              LOCATION_NAME = a.LOCATION_NAME

                          }).ToArray()
                };
                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("P_FBBOR045_ACIM_ARRAY", "FBBOR045_ACIM_ARRAY", packageMappingObjectModel);

                //var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR045_TEST.ACITON_USER2",
                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR045.ACITON_USER",
                   new object[]
                   {
                       //List
                       P_TRANSACTION_NO,
                       P_ACTION,
                       packageMapping,

                       //Return
                       IO_PROCESS_FAIL,
                       p_return_code,
                       p_return_message

                   });



                var d_list_service_info_cur = (DataTable)executeResult[0];

                var temp = new List<DETAIL_USER_RESPONSE>();

                foreach (DataRow item in d_list_service_info_cur.Rows)
                {
                    temp.Add(new DETAIL_USER_RESPONSE { USER_NAME = item[0].ToString() });
                }

                command.IO_PROCESS_FAIL = temp;
                command.P_RETURN_CODE = p_return_code.Value.ToSafeString();
                command.P_RETURN_MESSAGE = p_return_message.Value.ToSafeString();

                var res_result = new PermissionUserResponse
                {
                    ReturnCode = command.P_RETURN_CODE.ToSafeString(),
                    ReturnMessage = command.P_RETURN_MESSAGE.ToSafeString(),
                    USER_ARRAY = temp
                };
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, res_result, log, "Success", "Success", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.P_RETURN_CODE = "50000";
                command.P_RETURN_MESSAGE = "System Error";
            }
        }



        #region PackageMappingObjectModel
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBBOR045_ACIM_ARRAY_Mapping[] FBBOR045_ACIM_ARRAY { get; set; }


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
                OracleUdt.SetValue(con, udt, 0, FBBOR045_ACIM_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBBOR045_ACIM_ARRAY = (FBBOR045_ACIM_ARRAY_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBBOR045_ACIM_RECORD")]
        public class FBBOR045_ACIM_ARRAY_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBBOR045_ACIM_ARRAY_Mapping();
            }
        }

        [OracleCustomTypeMapping("FBBOR045_ACIM_ARRAY")]
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
                return new FBBOR045_ACIM_ARRAY_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBBOR045_ACIM_ARRAY_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("USER_NAME")]
            public string USER_NAME { get; set; }

            [OracleObjectMapping("FIRST_NAME")]
            public string FIRST_NAME { get; set; }

            [OracleObjectMapping("LAST_NAME")]
            public string LAST_NAME { get; set; }

            [OracleObjectMapping("EMAIL")]
            public string EMAIL { get; set; }

            [OracleObjectMapping("ROLE")]
            public string ROLE { get; set; }

            [OracleObjectMapping("ROLE_PAST")]
            public string ROLE_PAST { get; set; }

            [OracleObjectMapping("PERIOD")]
            public string PERIOD { get; set; }

            [OracleObjectMapping("START_DATE")]
            public string START_DATE { get; set; }

            [OracleObjectMapping("END_DATE")]
            public string END_DATE { get; set; }

            [OracleObjectMapping("LOCATION_CODE")]
            public string LOCATION_CODE { get; set; }

            [OracleObjectMapping("LOCATION_NAME")]
            public string LOCATION_NAME { get; set; }


            #endregion Attribute Mapping

            public static FBBOR045_ACIM_ARRAY_Mapping Null
            {
                get
                {
                    var obj = new FBBOR045_ACIM_ARRAY_Mapping();
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
                OracleUdt.SetValue(con, udt, "USER_NAME", USER_NAME);
                OracleUdt.SetValue(con, udt, "FIRST_NAME", FIRST_NAME);
                OracleUdt.SetValue(con, udt, "LAST_NAME", LAST_NAME);
                OracleUdt.SetValue(con, udt, "EMAIL", EMAIL);
                OracleUdt.SetValue(con, udt, "ROLE", ROLE);
                OracleUdt.SetValue(con, udt, "ROLE_PAST", ROLE_PAST);
                OracleUdt.SetValue(con, udt, "PERIOD", PERIOD);
                OracleUdt.SetValue(con, udt, "START_DATE", START_DATE);
                OracleUdt.SetValue(con, udt, "END_DATE", END_DATE);
                OracleUdt.SetValue(con, udt, "LOCATION_CODE", LOCATION_CODE);
                OracleUdt.SetValue(con, udt, "LOCATION_NAME", LOCATION_NAME);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
