using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SavePackageUserGroupCommandHandler : ICommandHandler<SavePackageUserGroupCommand>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _objService;

        public SavePackageUserGroupCommandHandler(ILogger logger,
            IAirNetEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(SavePackageUserGroupCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_msg = new OracleParameter();
                o_return_msg.OracleDbType = OracleDbType.Varchar2;
                o_return_msg.Size = 2000;
                o_return_msg.Direction = ParameterDirection.Output;

                var packageUserObjectModel = new PackageUserObjectModel();
                packageUserObjectModel.AIR_PACKAGE_USER_ARRAY = command.airPackageUserArray.Select(a => new Air_Package_User_ArrayMapping
                {
                    service_option = a.SERVICE_OPTION.ToSafeString(),
                    user_group = a.USER_GROUP.ToSafeString(),
                    update_by = a.UPD_BY.ToSafeString(),
                    effective_dtm = a.EFFECTIVE_DTM.ToSafeString(),
                    expire_dtm = a.EXPIRE_DTM.ToSafeString()
                }).ToArray();

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var packageUser = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_PACKAGE_USER_ARRAY", "AIR_PACKAGE_USER_ARRAY", packageUserObjectModel);

                var executeResult = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR012.save_package_user_group",
                out paramOut,
                   new
                   {
                       p_service_option = command.service_option.ToSafeString(),
                       p_package_code = command.package_code.ToSafeString(),

                       /// array
                       p_air_package_user_array = packageUser,
                       /// Out
                       o_return_code = o_return_code,
                       o_return_msg = o_return_msg

                   });
                command.return_msg = o_return_msg.Value.ToSafeString();
                string return_code = o_return_code.Value.ToSafeString();
                command.return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.return_code = -1;
                command.return_msg = "Error call Save PackageUser service " + ex.Message;
            }

        }

        #region Mapping AIR_PACKAGE_USER_ARRAY Type Oracle

        public class PackageUserObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Air_Package_User_ArrayMapping[] AIR_PACKAGE_USER_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageUserObjectModel Null
            {
                get
                {
                    PackageUserObjectModel obj = new PackageUserObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AIR_PACKAGE_USER_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AIR_PACKAGE_USER_ARRAY = (Air_Package_User_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_PACKAGE_USER_RECORD")]
        public class Air_Package_UserOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Air_Package_User_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("AIR_PACKAGE_USER_ARRAY")]
        public class AirPackageUserObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageUserObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Air_Package_User_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Air_Package_User_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SERVICE_OPTION")]
            public string service_option { get; set; }

            [OracleObjectMappingAttribute("USER_GROUP")]
            public string user_group { get; set; }

            [OracleObjectMappingAttribute("UPD_BY")]
            public string update_by { get; set; }

            [OracleObjectMappingAttribute("EFFECTIVE_DTM")]
            public string effective_dtm { get; set; }

            [OracleObjectMappingAttribute("EXPIRE_DTM")]
            public string expire_dtm { get; set; }

            #endregion Attribute Mapping

            public static Air_Package_User_ArrayMapping Null
            {
                get
                {
                    Air_Package_User_ArrayMapping obj = new Air_Package_User_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "SERVICE_OPTION", service_option);
                OracleUdt.SetValue(con, udt, "USER_GROUP", user_group);
                OracleUdt.SetValue(con, udt, "UPD_BY", update_by);
                OracleUdt.SetValue(con, udt, "EFFECTIVE_DTM", effective_dtm);
                OracleUdt.SetValue(con, udt, "EXPIRE_DTM", expire_dtm);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  AIR_PACKAGE_USER_ARRAY Type Oracle
    }
}
