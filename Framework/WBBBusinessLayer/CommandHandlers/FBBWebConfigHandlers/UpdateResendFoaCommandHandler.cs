using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdateResendFoaCommandHandler : ICommandHandler<ResendFoaCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public UpdateResendFoaCommandHandler(
            ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;

        }
        public void Handle(ResendFoaCommand command)
        {

            _logger.Info("Start p_update_foa_resend_error_log");
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();
                //FBB_foa_access_list
                //p_FBB_foa_access_list
                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBB_FOA_ACCESS_LIST =
                        command.p_FBB_foa_access_list.Select(
                            a => new FBB_FOA_RESEND_LOG_LISTMapping
                            {
                                ACCESS_NUMBER = a.ACCESS_NUMBER,
                                RESEND_STATUS = a.RESEND_STATUS,
                            }).ToArray()
                };
                var p_FBB_foa_access_list = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_FBB_foa_access_list", "FBB_FOA_ACCESS_LIST", packageMappingObjectModel);
                var executePassword = _objService.ExecuteStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_update_foa_resend_error_log",
             out paramOut,
               new
               {
                   p_FBB_foa_access_list,
                   //  p_access_number = command.ACCESS_NUMBER.ToSafeString(),
                   p_user_name = command.UPDATED_BY,
                   // p_resend_status = command.RESEND_STATUS,

                   ret_code,
                   ret_msg

               });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_message = ret_msg.Value.ToSafeString();
                _logger.Info("End p_update_foa_resend_error_log" + ret_msg);

            }
            catch (Exception ex)
            {
                command.ret_code = "1";
                command.ret_message = "ERROR" + ":" + ex.Message.ToString() + "  :" + command.RESEND_STATUS + " :" + command.UPDATED_BY + " :" + command.UPDATED_DATE;
                _logger.Info("ERROR UpdateResendFoaCommandHandler: " + ex.GetErrorMessage() + " Command Param ACCESS_NUMBER" + command.ACCESS_NUMBER);
            }
        }
        #region Mapping FBB_FOA_ACCESS_LIST Type Oracle

        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_FOA_RESEND_LOG_LISTMapping[] FBB_FOA_ACCESS_LIST { get; set; }

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
                OracleUdt.SetValue(con, udt, 0, FBB_FOA_ACCESS_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_FOA_ACCESS_LIST = (FBB_FOA_RESEND_LOG_LISTMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBB_FOA_ACCESS_REC")]
        public class FBB_FOA_RESEND_LOG_LIST_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_FOA_RESEND_LOG_LISTMapping();
            }
        }
        [OracleCustomTypeMapping("FBB_FOA_ACCESS_LIST")]
        public class FBB_FOA_RESEND_LOG_LISTMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new FBB_FOA_RESEND_LOG_LISTMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_FOA_RESEND_LOG_LISTMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("ACCESS_NUMBER")]
            public string ACCESS_NUMBER { get; set; }
            [OracleObjectMapping("RESEND_STATUS")]
            public string RESEND_STATUS { get; set; }

            #endregion Attribute Mapping

            public static FBB_FOA_RESEND_LOG_LISTMapping Null
            {
                get
                {
                    var obj = new FBB_FOA_RESEND_LOG_LISTMapping();
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
                OracleUdt.SetValue(con, udt, "ACCESS_NUMBER", ACCESS_NUMBER);
                OracleUdt.SetValue(con, udt, "RESEND_STATUS", RESEND_STATUS);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
